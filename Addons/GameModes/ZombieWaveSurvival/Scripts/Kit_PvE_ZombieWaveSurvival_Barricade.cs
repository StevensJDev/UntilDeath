using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        [System.Serializable]
        public class BarricadeObjects
        {
            [Tooltip("When is this object active?")]
            /// <summary>
            /// When is this object active?
            /// </summary>
            public float activeAtHealth;

            [Tooltip("This animates the barricade")]
            /// <summary>
            /// This animates the barricade
            /// </summary>
            public Animator animator;
            [Tooltip("Sound that plays when this thing is repaired")]
            /// <summary>
            /// Sound that plays when this thing is repaired
            /// </summary>
            public AudioClip[] repairSound;
            [Tooltip("Sound that plays when this barricade is destroyed")]
            /// <summary>
            /// Sound that plays when this barricade is destroyed
            /// </summary>
            public AudioClip[] destroySound;

            [HideInInspector]
            /// <summary>
            /// Is this thing currently destroyed?
            /// </summary>
            public bool isDestroyed;
        }

        public class Kit_PvE_ZombieWaveSurvival_Barricade : Kit_InteractableObject, IPunObservable, Kit_PvE_ZombieWaveSurvival_ZombieDamageable
        {
            [Header("Settings")]
            [Tooltip(" Health the barricade starts with")]
            /// <summary>
            /// Health the barricade starts with
            /// </summary>
            public float maxHealth = 100f;
            [Tooltip("How many hp the barricade gains when being repaired")]
            /// <summary>
            /// How many hp the barricade gains when being repaired
            /// </summary>
            public float repairAmount = 25f;
            [Tooltip("How much money you gain by repairing the barricade")]
            /// <summary>
            /// How much money you gain by repairing the barricade
            /// </summary>
            public int repairMoneyGain = 100;
            [Tooltip("Time between repairs")]
            /// <summary>
            /// Time between repairs
            /// </summary>
            public float repairInterval = 1f;
            [Tooltip("Called when the barricade starts and when the barricade goes from 0 to more health (repair)")]
            /// <summary>
            /// Called when the barricade starts and when the barricade goes from 0 to more health (repair)
            /// </summary>
            public UnityEvent onBarricadeReparied;
            [Tooltip("Called when the barricade is fully destroyed")]
            /// <summary>
            /// Called when the barricade is fully destroyed
            /// </summary>
            public UnityEvent onBarricadeDestroyed;

            [Tooltip("The objects which the barricade consists of. Think of this as the Planks it is created of")]
            /// <summary>
            /// The objects which the barricade consists of. Think of this as the "Planks" it is created of
            /// </summary>
            public BarricadeObjects[] barricadeObjects;

            [Header("Sounds")]
            /// <summary>
            /// The source for the sounds this thing makes
            /// </summary>
            public AudioSource soundSource;

            /// <summary>
            /// Sounds that play when damage is taken
            /// </summary>
            public AudioClip[] damageSounds;

            [Tooltip("Offset for the attack position")]
            [Header("Positions")]
            /// <summary>
            /// Offset for the attack position
            /// </summary>
            public Vector3 attackPositionOffset;
            [Tooltip("Offset for the position that is used for the zombie to look at")]
            /// <summary>
            /// Offset for the position that is used for the zombie to look at
            /// </summary>
            public Vector3 attackLookAtPositionOffset;

            /// <summary>
            /// Health left
            /// </summary>
            private float health;
            /// <summary>
            /// To register changes
            /// </summary>
            private float lastHealth;
            /// <summary>
            /// Last repair at <see cref="Time.time"/>
            /// </summary>
            private float lastRepair;
            /// <summary>
            /// Reference to active game mode
            /// </summary>
            private Kit_PvE_ZombieWaveSurvival zws;
            /// <summary>
            /// Reference to main
            /// </summary>
            private Kit_IngameMain main;

            void Start()
            {
                main = FindObjectOfType<Kit_IngameMain>();
                zws = main.currentPvEGameModeBehaviour as Kit_PvE_ZombieWaveSurvival;

                health = maxHealth;
                lastHealth = maxHealth;

                onBarricadeReparied.Invoke();
            }

            void Update()
            {
                if (health != lastHealth)
                {
                    if (lastHealth > 0)
                    {
                        if (health <= 0)
                        {
                            onBarricadeDestroyed.Invoke();
                        }
                    }
                    else
                    {
                        if (health > 0)
                        {
                            onBarricadeReparied.Invoke();
                        }
                    }

                    if (lastHealth < health)
                    {
                        //Play damage sound
                        if (damageSounds.Length > 0 && soundSource)
                        {
                            if (!soundSource.isPlaying)
                            {
                                //Send RPC
                                soundSource.clip = damageSounds[Random.Range(0, damageSounds.Length)];
                                soundSource.Play();
                            }
                        }
                    }

                    for (int i = 0; i < barricadeObjects.Length; i++)
                    {
                        bool destroyed = health <= barricadeObjects[i].activeAtHealth;
                        //Check if state changed
                        if (destroyed != barricadeObjects[i].isDestroyed)
                        {
                            //Play animation
                            if (destroyed)
                            {
                                if (barricadeObjects[i].animator)
                                {
                                    barricadeObjects[i].animator.Play("Destroy");
                                }

                                //Play sound 
                                if (barricadeObjects[i].destroySound.Length > 0)
                                {
                                    soundSource.clip = barricadeObjects[i].destroySound[Random.Range(0, barricadeObjects[i].destroySound.Length)];
                                    soundSource.Play();
                                }
                            }
                            else
                            {
                                if (barricadeObjects[i].animator)
                                {
                                    barricadeObjects[i].animator.Play("Repair");
                                }

                                //Play sound 
                                if (barricadeObjects[i].repairSound.Length > 0)
                                {
                                    soundSource.clip = barricadeObjects[i].repairSound[Random.Range(0, barricadeObjects[i].repairSound.Length)];
                                    soundSource.Play();
                                }
                            }

                            //Set value
                            barricadeObjects[i].isDestroyed = destroyed;
                        }
                    }

                    lastHealth = health;
                }
            }

            void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(health);
                }
                else
                {
                    health = (float)stream.ReceiveNext();
                }
            }

            bool Kit_PvE_ZombieWaveSurvival_ZombieDamageable.IsAlive()
            {
                return health > 0f;
            }

            void Kit_PvE_ZombieWaveSurvival_ZombieDamageable.ApplyDamage(float dmg)
            {
                if (PhotonNetwork.IsMasterClient && photonView.IsRoomView || photonView.IsMine)
                {
                    if (health > 0)
                    {
                        health -= dmg;
                    }
                }
            }

            public override bool CanInteract(Kit_PlayerBehaviour who)
            {
                return health < maxHealth;
            }

            public override void Interact(Kit_PlayerBehaviour who)
            {
                if (Time.time >= lastRepair + repairInterval)
                {
                    if (health < maxHealth)
                    {
                        //Send repair rpc
                        photonView.RPC("RpcRepair", RpcTarget.MasterClient);

                        if (!zws) zws = main.currentPvEGameModeBehaviour as Kit_PvE_ZombieWaveSurvival;
                        if (zws)
                        {
                            //give money
                            zws.localPlayerData.GainMoney(repairMoneyGain);
                        }
                        //Set timer
                        lastRepair = Time.time;
                    }
                }
            }

            public override bool IsHold()
            {
                //Repair as long as F is held
                return true;
            }


            [PunRPC]
            public void RpcRepair()
            {
                if (PhotonNetwork.IsMasterClient && photonView.IsRoomView || photonView.IsMine)
                {
                    //Repair
                    health = Mathf.Clamp(health + repairAmount, 0, maxHealth);

                    //Once the last barricade has been repaired, set to full health.
                    if (health > barricadeObjects[0].activeAtHealth)
                    {
                        health = maxHealth;
                    }
                }
            }

            Vector3 Kit_PvE_ZombieWaveSurvival_ZombieDamageable.GetAttackPosition()
            {
                return transform.position + transform.TransformDirection(attackPositionOffset);
            }

            Vector3 Kit_PvE_ZombieWaveSurvival_ZombieDamageable.GetAttackRotationLookAt()
            {
                return transform.position + transform.TransformDirection(attackLookAtPositionOffset);
            }

            private void OnDrawGizmos()
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(attackPositionOffset));
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(attackLookAtPositionOffset));
            }
        }
    }
}