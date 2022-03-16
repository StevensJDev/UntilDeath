using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        /// <summary>
        /// Instance based settings of zombie attacks
        /// </summary>
        [System.Serializable]
        public class ZombieAttackInstance
        {
            [Tooltip("This object gets enabled for the time defined in the settings and should damage the player")]
            /// <summary>
            /// This object gets enabled for the time defined in the settings and should damage the player
            /// </summary>
            public GameObject enableObject;
        }

        public class Kit_PvE_ZombieWaveSurvival_ZombieAI : MonoBehaviourPunCallbacks, IPunObservable
        {
            /// <summary>
            /// Our Nav Mesh Agent
            /// </summary>
            [Header("Components")]
            public NavMeshAgent nma;
            /// <summary>
            /// Renderer
            /// </summary>
            public Kit_PvE_ZombieWaveSurvival_ZombieAIRenderer activeRenderer;


            #region Runtime
            /// <summary>
            /// Player we are currently attacking
            /// </summary>
            private Kit_PlayerBehaviour playerToAttack;
            /// <summary>
            /// How many hit points we have left
            /// </summary>
            private float hitPoints = 100f;
            private float originalHP = 100f;
            /// <summary>
            /// When can we attack the player again?
            /// </summary>
            private float nextAttackPossibleAt;
            /// <summary>
            /// Until which point in time is the movement frozen?
            /// </summary>
            private float movementFrozenUntil;
            /// <summary>
            /// Reference to main
            /// </summary>
            private Kit_IngameMain main;
            [HideInInspector]
            /// <summary>
            /// Last forward vector from where we were shot
            /// </summary>
            public Vector3 ragdollForward;
            [HideInInspector]
            /// <summary>
            /// Last force which we were shot with
            /// </summary>
            public float ragdollForce;
            [HideInInspector]
            /// <summary>
            /// Last point from where we were shot
            /// </summary>
            public Vector3 ragdollPoint;
            [HideInInspector]
            /// <summary>
            /// Which collider should the force be applied to?
            /// </summary>
            public int ragdollId;
            /// <summary>
            /// Vertical value for anim
            /// </summary>
            private float animVer;
            /// <summary>
            /// Horizontal value for anim
            /// </summary>
            private float animHor;
            /// <summary>
            /// If true, the root motion should propel us forward
            /// </summary>
            private bool rootMotionMove;
            /// <summary>
            /// World to local
            /// </summary>
            private Vector3 localVelocity;
            /// <summary>
            /// When is the next sound due?
            /// </summary>
            private float nextRandomSoundAt;
            /// <summary>
            /// Cached reference to the game mode
            /// </summary>
            public static Kit_PvE_ZombieWaveSurvival zws;
            /// <summary>
            /// Until when is a triggered animation (via an actual trigger) in progress?
            /// </summary>
            private float animationTriggerInProgressUntil;
            /// <summary>
            /// The direction we must be looking in during the animation
            /// </summary>
            private Vector3 animationTriggerLookDirection;
            /// <summary>
            /// If assigned, this damageable will be attacked
            /// </summary>
            private Kit_PvE_ZombieWaveSurvival_ZombieDamageable activeDamageable;
            #endregion

            #region Unity Calls
            private void Start()
            {
                nma.enabled = photonView.IsMine;

                //Find main
                main = FindObjectOfType<Kit_IngameMain>();

                zws = main.currentPvEGameModeBehaviour as Kit_PvE_ZombieWaveSurvival;

                zws.ZombieSpawned();

                int zombie = (int)photonView.InstantiationData[0];
                int skin = (int)photonView.InstantiationData[1];

                //Create renderer
                GameObject go = Instantiate(zws.zombiePrefabs[zombie].skins[skin], transform, false);
                activeRenderer = go.GetComponentInChildren<Kit_PvE_ZombieWaveSurvival_ZombieAIRenderer>();
                activeRenderer.zombie = this;

                //Set scale
                nma.radius = activeRenderer.settings.navMeshRadius;

                if (!activeRenderer.settings.debugDisablePhysicsOptimization)
                {
                    //Remove rigidbodies
                    Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
                    for (int i = 0; i < bodies.Length; i++)
                    {
                        Joint joint = bodies[i].GetComponent<Joint>();
                        if (joint)
                        {
                            Destroy(joint);
                        }


                        if (!bodies[i].GetComponent<Kit_PvE_ZombieWaveSurvival_ZombieAIPlayerDamage>())
                        {
                            Destroy(bodies[i]);
                        }
                    }

                    //Ragdoll
                    for (int i = 0; i < activeRenderer.colliders.Length; i++)
                    {
                        activeRenderer.colliders[i].isTrigger = true;
                    }
                }
                else
                {
                    //Ragdoll
                    for (int i = 0; i < activeRenderer.colliders.Length; i++)
                    {
                        activeRenderer.colliders[i].isTrigger = true;
                    }

                    for (int i = 0; i < activeRenderer.rigidbodies.Length; i++)
                    {
                        activeRenderer.rigidbodies[i].isKinematic = true;
                    }
                }

                if (photonView.IsMine)
                {
                    //Set initial HP
                    hitPoints = activeRenderer.settings.health;
                    originalHP = hitPoints;
                }



                //Spawn voice
                if (activeRenderer.settings.soundSpawn.Length > 0 && activeRenderer.voiceSource)
                {
                    //Check random ?
                    if (Random.Range(0, 100) >= 100 - activeRenderer.settings.soundSpawnChance)
                    {
                        //Get sound
                        int sound = Random.Range(0, activeRenderer.settings.soundSpawn.Length);

                        //Send RPC
                        photonView.RPC("RpcPlayVoice", RpcTarget.All, (byte)0, sound);
                    }
                }

                //Set random voice timer
                nextRandomSoundAt = Time.time + Random.Range(activeRenderer.settings.soundRandomInterval.x, activeRenderer.settings.soundRandomInterval.y);
            }

            private void Update()
            {
                if (photonView.IsMine)
                {
                    if (animationTriggerInProgressUntil > Time.time)
                    {
                        nma.radius = 0f;
                        nma.speed = 0f;
                    }
                    else if (movementFrozenUntil > Time.time)
                    {
                        nma.radius = Mathf.Lerp(nma.radius, activeRenderer.settings.navMeshRadius, Time.deltaTime * 3f); //This might seem odd but this is to prevent  zombies from instantly pushing each other away
                        nma.speed = 0f;
                    }
                    else if (activeRenderer.useRootMotion)
                    {
                        nma.radius = Mathf.Lerp(nma.radius, activeRenderer.settings.navMeshRadius, Time.deltaTime * 3f); //This might seem odd but this is to prevent  zombies from instantly pushing each other away
                        nma.speed = 0.01f; //We need a really small amount of nav mesh agent's movement so that agents don't intersect with each other
                    }
                    else
                    {
                        nma.radius = Mathf.Lerp(nma.radius, activeRenderer.settings.navMeshRadius, Time.deltaTime * 3f); //This might seem odd but this is to prevent  zombies from instantly pushing each other away
                        nma.speed = activeRenderer.settings.navMeshSpeed;
                    }

                    if (animationTriggerInProgressUntil > Time.time)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(animationTriggerLookDirection), Time.deltaTime * 5f);
                    }
                    else if (activeDamageable != null)
                    {
                        //Set our destination to that player
                        nma.destination = activeDamageable.GetAttackPosition();

                        float targetDistance = Vector3.Distance(transform.position, activeDamageable.GetAttackPosition());

                        if (movementFrozenUntil > Time.time)
                        {
                            nma.speed = 0f;
                        }
                        else if (targetDistance < activeRenderer.settings.attackRotateToDistance)
                        {
                            nma.updateRotation = false;

                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(activeDamageable.GetAttackRotationLookAt() - transform.position, Vector3.up), Time.deltaTime * activeRenderer.settings.attackRotateSpeed);
                        }
                        else
                        {
                            nma.updateRotation = false;

                            if (nma.hasPath)
                            {
                                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nma.steeringTarget - transform.position, Vector3.up), Time.deltaTime * 5f);
                            }
                        }

                        //Check if we are close enough, don't let others push us at this time
                        if (targetDistance < activeRenderer.settings.attackDistance)
                        {
                            nma.speed = 0f;
                        }

                        if (Time.time >= nextAttackPossibleAt)
                        {
                            //Check if we are close enough
                            if (targetDistance < activeRenderer.settings.attackDistance)
                            {
                                int attack = Random.Range(0, activeRenderer.settings.attackSettings.Length);

                                if (activeRenderer.settings.attackSettings[attack].freezesZombieForAttackDuration)
                                {
                                    movementFrozenUntil = Time.time + activeRenderer.settings.attackSettings[attack].time;
                                }

                                //Set time
                                nextAttackPossibleAt = Time.time + activeRenderer.settings.attackSettings[attack].time + 0.1f;

                                //Send RPC
                                photonView.RPC("RpcAttack", RpcTarget.All, attack);

                                //Attack voice
                                if (activeRenderer.settings.soundAttack.Length > 0 && activeRenderer.voiceSource)
                                {
                                    //Check random ?
                                    if (Random.Range(0, 100) >= 100 - activeRenderer.settings.soundAttackChance)
                                    {
                                        //Get sound
                                        int sound = Random.Range(0, activeRenderer.settings.soundAttack.Length);

                                        //Send RPC
                                        photonView.RPC("RpcPlayVoice", RpcTarget.All, (byte)3, sound);
                                    }
                                }
                            }
                        }

                        if (!activeDamageable.IsAlive())
                        {
                            activeDamageable = null;
                        }
                    }
                    else if (playerToAttack)
                    {
                        //Set our destination to that player
                        nma.destination = playerToAttack.transform.position;

                        float targetDistance = Vector3.Distance(transform.position, playerToAttack.transform.position);

                        if (movementFrozenUntil > Time.time)
                        {
                            nma.speed = 0f;
                        }
                        else if (targetDistance < activeRenderer.settings.attackRotateToDistance)
                        {
                            nma.updateRotation = false;

                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerToAttack.transform.position - transform.position, Vector3.up), Time.deltaTime * activeRenderer.settings.attackRotateSpeed);
                        }
                        else
                        {
                            nma.updateRotation = false;

                            if (nma.hasPath)
                            {
                                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nma.steeringTarget - transform.position, Vector3.up), Time.deltaTime * 5f);
                            }
                        }

                        if (Time.time >= nextAttackPossibleAt)
                        {
                            //Check if we are close enough
                            if (targetDistance < activeRenderer.settings.attackDistance)
                            {
                                int attack = Random.Range(0, activeRenderer.settings.attackSettings.Length);

                                if (activeRenderer.settings.attackSettings[attack].freezesZombieForAttackDuration)
                                {
                                    movementFrozenUntil = Time.time + activeRenderer.settings.attackSettings[attack].time;
                                }

                                //Set time
                                nextAttackPossibleAt = Time.time + activeRenderer.settings.attackSettings[attack].time + 0.1f;

                                //Send RPC
                                photonView.RPC("RpcAttack", RpcTarget.All, attack);

                                //Attack voice
                                if (activeRenderer.settings.soundAttack.Length > 0 && activeRenderer.voiceSource)
                                {
                                    //Check random ?
                                    if (Random.Range(0, 100) >= 100 - activeRenderer.settings.soundAttackChance)
                                    {
                                        //Get sound
                                        int sound = Random.Range(0, activeRenderer.settings.soundAttack.Length);

                                        //Send RPC
                                        photonView.RPC("RpcPlayVoice", RpcTarget.All, (byte)3, sound);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        rootMotionMove = false;
                        animHor = 0f;
                        animVer = 0f;

                        if (main.allActivePlayers.Count > 0)
                        {
                            //Pick a random player to attack
                            playerToAttack = main.allActivePlayers[Random.Range(0, main.allActivePlayers.Count)];
                        }
                    }

                    if (movementFrozenUntil > Time.time)
                    {
                        rootMotionMove = false;
                        animHor = 0f;
                        animVer = 0f;
                    }      
                    else if (activeRenderer.settings.setDesiredVelocity)
                    {
                        localVelocity = transform.InverseTransformDirection(nma.desiredVelocity);
                        animHor = localVelocity.x;
                        animVer = localVelocity.z;
                    }
                    else if (activeDamageable != null)
                    {
                        if (activeRenderer.useRootMotion)
                        {
                            rootMotionMove = nma.remainingDistance > activeRenderer.settings.attackDistance;
                        }
                        else
                        {
                            localVelocity = transform.InverseTransformDirection(nma.velocity);
                            animHor = localVelocity.x;
                            animVer = localVelocity.z;
                        }
                    }
                    else
                    {
                        if (activeRenderer.useRootMotion)
                        {
                            rootMotionMove = nma.remainingDistance > 0.3f;
                        }
                        else
                        {
                            localVelocity = transform.InverseTransformDirection(nma.velocity);
                            animHor = localVelocity.x;
                            animVer = localVelocity.z;
                        }
                    }

                    if (Time.time >= nextRandomSoundAt)
                    {
                        //Random voice
                        if (activeRenderer.settings.soundSpawn.Length > 0 && activeRenderer.voiceSource)
                        {
                            //Check random ?
                            if (Random.Range(0, 100) >= 100 - activeRenderer.settings.soundSpawnChance)
                            {
                                //Get sound
                                int sound = Random.Range(2, activeRenderer.settings.soundSpawn.Length);

                                //Send RPC
                                photonView.RPC("RpcPlayVoice", RpcTarget.All, (byte)0, sound);
                            }
                        }

                        //Set random voice timer
                        nextRandomSoundAt = Time.time + Random.Range(activeRenderer.settings.soundRandomInterval.x, activeRenderer.settings.soundRandomInterval.y);
                    }
                }
                else
                {
                    nma.speed = 0f;

                    if (activeDamageable != null)
                    {
                        if (!activeDamageable.IsAlive())
                        {
                            activeDamageable = null;
                        }
                    }
                }

                if (activeRenderer.settings.setDesiredVelocity)
                {
                    activeRenderer.anim.SetFloat("hor", animHor, activeRenderer.settings.animSetSmooth, Time.deltaTime);
                    activeRenderer.anim.SetFloat("ver", animVer, activeRenderer.settings.animSetSmooth, Time.deltaTime);
                }
                else
                {
                    if (activeRenderer.useRootMotion)
                    {
                        activeRenderer.anim.SetBool("move", rootMotionMove);
                    }
                    else
                    {
                        activeRenderer.anim.SetFloat("hor", animHor, activeRenderer.settings.animSetSmooth, Time.deltaTime);
                        activeRenderer.anim.SetFloat("ver", animVer, activeRenderer.settings.animSetSmooth, Time.deltaTime);
                    }
                }
            }

            private void OnDestroy()
            {
                if (!main.isShuttingDown)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        //Check for drops
                        for (int i = 0; i < zws.allDrops.Length; i++)
                        {
                            int id = i;
                            //Get random chacne
                            int rdm = Random.Range(0, 100);

                            //Check if it was enough to spawn this drop
                            if (rdm > 100 - zws.allDrops[id].dropChance)
                            {
                                //Create drop
                                zws.allDrops[id].CreateDrop(main, zws, transform.position, transform.rotation, id);
                                //Stop
                                break;
                            }
                        }
                    }

                    zws.ZombieKilled();

                    if (activeRenderer.anim)
                    {
                        Destroy(activeRenderer.anim);
                    }

                    if (!activeRenderer.settings.debugDisablePhysicsOptimization)
                    {
                        int zombie = (int)photonView.InstantiationData[0];
                        int skin = (int)photonView.InstantiationData[1];

                        AddRigidbodiesRecursive(activeRenderer.transform, zws.zombiePrefabs[zombie].skins[skin].transform, false);

                        activeRenderer.rigidbodies = activeRenderer.GetComponentsInChildren<Rigidbody>();
                    }

                    //Destroy attack colliders
                    for (int i = 0; i < activeRenderer.attacks.Length; i++)
                    {
                        if (activeRenderer.attacks[i].enableObject)
                        {
                            Destroy(activeRenderer.attacks[i].enableObject);
                        }
                    }

                    //Set layer to AIDead
                    activeRenderer.transform.SetLayerRecursively(20);

                    //Create ragdoll
                    for (int i = 0; i < activeRenderer.colliders.Length; i++)
                    {
                        activeRenderer.colliders[i].isTrigger = false;
                        activeRenderer.colliders[i].gameObject.layer = 20; //Don't collide with player
                    }

                    for (int i = 0; i < activeRenderer.rigidbodies.Length; i++)
                    {
                        activeRenderer.rigidbodies[i].isKinematic = false;
                    }

                    //Unparent
                    activeRenderer.transform.parent = null;
                    //Add force after one frame to allow physics system to process the new rigidbodies and joints
                    activeRenderer.StartCoroutine(activeRenderer.ApplyForceAfterOneFrame(ragdollId, ragdollForward * ragdollForce, ragdollPoint));

                    if (activeRenderer.settings.soundDeath.Length > 0 && activeRenderer.voiceSource)
                    {
                        //Random chance ?
                        if (Random.Range(0, 100) >= 100 - activeRenderer.settings.soundDeathChance)
                        {
                            //Play the right sound
                            int sound = Random.Range(0, activeRenderer.settings.soundDeath.Length);
                            activeRenderer.voiceSource.clip = activeRenderer.settings.soundDeath[sound];
                            activeRenderer.voiceSource.loop = false;
                            activeRenderer.voiceSource.Play();
                        }
                    }

                    Destroy(activeRenderer.gameObject, activeRenderer.ragdollLiveTime);
                }
            }

            private void OnTriggerEnter(Collider collider)
            {
                Kit_PvE_ZombieWaveSurvival_ZombieDamageable damageable = collider.GetComponentInParent<Kit_PvE_ZombieWaveSurvival_ZombieDamageable>();

                if (damageable != null)
                {
                    if (damageable.IsAlive())
                    {
                        activeDamageable = damageable;
                    }
                }

                if (photonView.IsMine)
                {
                    if (Time.time > animationTriggerInProgressUntil)
                    {
                        //Check if we hit a trigger
                        Kit_PvE_ZombieWaveSurvival_ZombieAnimationTrigger animTrigger = collider.GetComponent<Kit_PvE_ZombieWaveSurvival_ZombieAnimationTrigger>();
                        if (animTrigger)
                        {
                            photonView.RPC("RpcPlayAnimationTrigger", RpcTarget.All, animTrigger.animationToTrigger, animTrigger.animationLength, animTrigger.transform.TransformDirection(animTrigger.lookDirection));
                        }
                    }
                }
            }
            #endregion

            #region Health
            public void Nuke()
            {
                if (photonView.IsMine)
                {
                    ragdollForce = 0f;

                    //Kill us
                    PhotonNetwork.Destroy(gameObject);
                }
            }

            public void InstaKill()
            {
                if (photonView.IsMine)
                {
                    hitPoints = 1f;
                }
            }

            public void OriginalHealth()
            {
                if (photonView.IsMine)
                {
                    hitPoints = originalHP;
                }
            }

            public void LocalDamage(float dmg, int gunID, Vector3 shotPos, Vector3 forward, float force, Vector3 hitPos, bool shotBot, int shotId, int ragdollId)
            {
                //Set ragdoll
                ragdollForce = force;
                ragdollForward = forward;
                this.ragdollId = ragdollId;
                ragdollPoint = hitPos;

                //Send RPC
                photonView.RPC("RpcDamage", RpcTarget.MasterClient, dmg, gunID, shotPos, forward, force, hitPos, shotBot, shotId, ragdollId);
            }

            [PunRPC]
            public void RpcDamage(float dmg, int gunID, Vector3 shotPos, Vector3 forward, float force, Vector3 hitPos, bool shotBot, int shotId, int ragdollId)
            {
                if (photonView.IsMine)
                {
                    //Damage voice
                    if (activeRenderer.settings.soundDamage.Length > 0 && activeRenderer.voiceSource)
                    {
                        //Check random ?
                        if (Random.Range(0, 100) >= 100 - activeRenderer.settings.soundDamageChance)
                        {
                            //Get sound
                            int sound = Random.Range(0, activeRenderer.settings.soundDamage.Length);

                            //Send RPC
                            photonView.RPC("RpcPlayVoice", RpcTarget.All, (byte)1, sound);
                        }
                    }

                    //Set ragdoll
                    ragdollForce = force;
                    ragdollForward = forward;
                    this.ragdollId = ragdollId;
                    ragdollPoint = hitPos;

                    //Apply damage
                    hitPoints -= dmg;

                    if (hitPoints <= 0)
                    {
                        if (shotBot)
                        {

                        }
                        else
                        {
                            //Send Event to give us kill xp
                            PhotonNetwork.RaiseEvent(101, null, new RaiseEventOptions { TargetActors = new int[] { shotId }, Receivers = ReceiverGroup.All }, new ExitGames.Client.Photon.SendOptions { Reliability = true });
                        }

                        //Kill us
                        PhotonNetwork.Destroy(gameObject);
                    }
                }
            }
            #endregion

            #region Attacking
            [PunRPC]
            public void RpcAttack(int attack)
            {
                if (attackRoutineCur != null) StopCoroutine(attackRoutineCur);

                //Start coroutine
                attackRoutineCur = StartCoroutine(AttackRoutine(attack));
            }

            private Coroutine attackRoutineCur;

            private IEnumerator AttackRoutine(int attack)
            {
                //Enable
                activeRenderer.attacks[attack].enableObject.SetActive(true);
                //Set trigger
                activeRenderer.anim.SetTrigger(activeRenderer.settings.attackSettings[attack].triggerName);
                //Wait
                yield return new WaitForSeconds(activeRenderer.settings.attackSettings[attack].time);
                //Disable
                activeRenderer.attacks[attack].enableObject.SetActive(false);
            }
            #endregion

            #region Sound
            [PunRPC]
            public void RpcPlayVoice(byte type, int index)
            {
                if (activeRenderer.voiceSource && !activeRenderer.voiceSource.isPlaying)
                {
                    switch (type)
                    {
                        //Spawn
                        case 0:
                            activeRenderer.voiceSource.clip = activeRenderer.settings.soundSpawn[index];
                            break;
                        //Damage
                        case 1:
                            activeRenderer.voiceSource.clip = activeRenderer.settings.soundDamage[index];
                            break;
                        //Random
                        case 2:
                            activeRenderer.voiceSource.clip = activeRenderer.settings.soundRandom[index];
                            break;
                        //Attack
                        case 3:
                            activeRenderer.voiceSource.clip = activeRenderer.settings.soundAttack[index];
                            break;
                    }

                    activeRenderer.voiceSource.loop = false;
                    activeRenderer.voiceSource.Play();
                }
            }
            #endregion

            #region Triggered animations
            [PunRPC]
            public void RpcPlayAnimationTrigger(string animToPlay, float length, Vector3 dir)
            {
                activeRenderer.anim.SetTrigger(animToPlay);
                //Don't play attack animation during a movement animation
                activeRenderer.anim.SetTrigger("AbortAttack");
                animationTriggerInProgressUntil = Time.time + length;
                animationTriggerLookDirection = dir;
            }
            #endregion

            #region Photon
            void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(ragdollForce);
                    stream.SendNext(ragdollForward);
                    stream.SendNext(ragdollId);
                    stream.SendNext(ragdollPoint);
                    stream.SendNext(animHor);
                    stream.SendNext(animVer);
                    stream.SendNext(rootMotionMove);
                }
                else
                {
                    ragdollForce = (float)stream.ReceiveNext();
                    ragdollForward = (Vector3)stream.ReceiveNext();
                    ragdollId = (int)stream.ReceiveNext();
                    ragdollPoint = (Vector3)stream.ReceiveNext();
                    animHor = (float)stream.ReceiveNext();
                    animVer = (float)stream.ReceiveNext();
                    rootMotionMove = (bool)stream.ReceiveNext();
                }
            }

            public override void OnMasterClientSwitched(Player newMasterClient)
            {
                //Will be true for new master client
                nma.enabled = photonView.IsMine;
            }
            #endregion

            #region Helpers
            /// <summary>
            /// This adds rigidbodies from the prefab to the existing object in the scene
            /// </summary>
            /// <param name="toAdd"></param>
            /// <param name="prefab"></param>
            /// <param name="first"></param>
            void AddRigidbodiesRecursive(Transform toAdd, Transform prefab, bool first = false)
            {
                Rigidbody body = prefab.GetComponent<Rigidbody>();

                if (body && !toAdd.GetComponent<Rigidbody>())
                {
                    Rigidbody newBody = toAdd.gameObject.AddComponent(body);
                    //This doesn't seem to copy correctly in some versions of Unity, so move this manually too.
                    newBody.mass = body.mass;
                    //Don't collide with player anymore (AIDead layer)
                    toAdd.gameObject.layer = 20;
                    //Apply physics
                    newBody.isKinematic = false;
                }

                Joint joint = prefab.GetComponent<Joint>();

                if (joint && !first && !toAdd.GetComponent<Joint>())
                {
                    Joint newJoint = toAdd.gameObject.AddComponent<Joint>(joint);
                    newJoint.connectedBody = GetParentBody(toAdd);
                    if (newJoint is CharacterJoint)
                    {
                        (newJoint as CharacterJoint).enableProjection = true;
                    }
                }

                for (int i = 0; i < toAdd.childCount; i++)
                {
                    for (int o = 0; o < prefab.childCount; o++)
                    {
                        if (toAdd.GetChild(i).name == prefab.GetChild(o).name)
                        {
                            AddRigidbodiesRecursive(toAdd.GetChild(i), prefab.GetChild(o));
                            break;
                        }
                    }
                }
            }

            Rigidbody GetParentBody(Transform trans)
            {
                Rigidbody toReturn = null;

                int tries = 0;

                Transform curParent = trans.parent;

                while (!toReturn && curParent && tries < 10)
                {
                    toReturn = curParent.GetComponent<Rigidbody>();
                    curParent = curParent.parent;
                    tries++;
                }

                return toReturn;
            }
            #endregion
        }
    }
}