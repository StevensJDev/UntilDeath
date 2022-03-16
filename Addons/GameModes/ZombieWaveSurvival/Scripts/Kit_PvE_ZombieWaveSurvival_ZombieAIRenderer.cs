using System.Collections;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        [System.Serializable]
        public class FootForSound
        {
            [Tooltip("From where the footstep sound is being emitted, should be on the foot")]
            /// <summary>
            /// From where the footstep sound is being emitted, should be on the foot
            /// </summary>
            public AudioSource soundSource;
            [Tooltip("From where the raycast shoots, ideally this is the foot")]
            /// <summary>
            /// From where the raycast shoots, ideally this is the foot
            /// </summary>
            public Transform raycastTransform;
            [Tooltip("How far the raycast to determine the material shoots down")]
            /// <summary>
            /// How far the raycast to determine the material shoots down
            /// </summary>
            public float raycastLength = 2f;
        }

        /// <summary>
        /// This script should be on the same object as the animator
        /// </summary>
        public class Kit_PvE_ZombieWaveSurvival_ZombieAIRenderer : MonoBehaviour
        {
            /// <summary>
            /// Assigned at runtime, reference to the zombie root
            /// </summary>
            public Kit_PvE_ZombieWaveSurvival_ZombieAI zombie;

            [Tooltip("Settings for this ai")]
            /// <summary>
            /// Settings for this ai
            /// </summary>
            public Kit_PvE_ZombieWaveSurvival_ZombieAISettings settings;
            [Tooltip("Attack colliders. These get enabled for the given attack")]
            /// <summary>
            /// Attack colliders. These get enabled for the given attack
            /// </summary>
            public ZombieAttackInstance[] attacks;
            [Tooltip("Animator for this model")]
            /// <summary>
            /// Animator for this model
            /// </summary>
            public Animator anim;
            [Tooltip("This audio source plays the zombie's sounds")]
            /// <summary>
            /// This audio source plays the zombie's sounds
            /// </summary>
            public AudioSource voiceSource;
            [Tooltip("Colliders for ragdoll")]
            /// <summary>
            /// Colliders for ragdoll
            /// </summary>
            public Collider[] colliders;
            [Tooltip("Rigidbodies for ragdoll")]
            /// <summary>
            /// Rigidbodies for ragdoll
            /// </summary>
            public Rigidbody[] rigidbodies;
            [Tooltip("Live time of the ragdoll")]
            /// <summary>
            /// Live time of the ragdoll
            /// </summary>
            public float ragdollLiveTime = 30f;
            [Header("Movement")]
            [Tooltip("If set to true, the nav mesh agent of the zombie will not move the zombie and instead it is moved by animator root motion")]
            /// <summary>
            /// If set to true, the nav mesh agent of the zombie will not move the zombie and instead it is moved by animator root motion
            /// </summary>
            public bool useRootMotion = true;
            [Tooltip("Multiplier for the root motion movement that is applied to the NavMeshAgent. Increase to move the zombie faster")]
            /// <summary>
            /// Multiplier for the root motion movement that is applied to the NavMeshAgent. Increase to move the zombie faster
            /// </summary>
            public float rootMotionMoveMultiplier = 1f;

            /// <summary>
            /// Footsteps to use the sound for
            /// </summary>
            [Tooltip("Footsteps to use the sound for")]
            [Header("Footsteps")]
            public FootForSound[] footSteps;
            /// <summary>
            /// Cached array so we can use raycast non alloc
            /// </summary>
            private RaycastHit[] raycastHits = new RaycastHit[1];
            /// <summary>
            /// When was the last footstep played?
            /// Used to not play sounds too much
            /// </summary>
            private float lastFootstep;

            void OnAnimatorMove()
            {
                if (useRootMotion && zombie.photonView.IsMine)
                {
                    Vector3 pos = anim.rootPosition;
                    pos.y = zombie.nma.nextPosition.y;
                    Vector3 delta = pos - zombie.transform.position;
                    if (zombie.nma.isOnNavMesh)
                    {
                        zombie.nma.Move(delta * rootMotionMoveMultiplier);
                    }
                }
            }

            /// <summary>
            /// This should be called by the animations via Animation Event
            /// </summary>
            /// <param name="index">Which foot to use in the array</param>
            public void Footstep(int index)
            {
                if (index < footSteps.Length && index >= 0)
                {
                    if (Time.time > lastFootstep + 0.2f)
                    {
                        //Check raycast
                        if (Physics.RaycastNonAlloc(footSteps[index].raycastTransform.position + Vector3.up, Vector3.down, raycastHits, footSteps[index].raycastLength, settings.footstepHitLayers.value, QueryTriggerInteraction.Ignore) != 0)
                        {
                            if (settings.footstepSounds.Count <= 0)
                            {
                                Debug.LogWarning("[Zombie] Footstep was triggered but no footstep sounds are assigned", settings);
                            }

                            //Set last played
                            lastFootstep = Time.time;

                            ZombieFootstep footstep = null;
                            //Set sound
                            if (settings.footstepSounds.Contains(raycastHits[0].collider.tag))
                            {
                                footstep = settings.footstepSounds[raycastHits[0].collider.tag];
                            }
                            else
                            {
                                footstep = settings.footstepSounds["Concrete"];
                            }

                            footSteps[index].soundSource.clip = footstep.sounds[Random.Range(0, footstep.sounds.Length)];

                            //Set volume
                            footSteps[index].soundSource.volume = settings.footStepVolume;

                            //Play
                            footSteps[index].soundSource.Play();
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("[Zombie] Footstep triggered but triggered footstep was not found in the array", this);
                }
            }

            public IEnumerator ApplyForceAfterOneFrame(int ragdollId, Vector3 force, Vector3 point)
            {
                yield return new WaitForEndOfFrame();

                //Apply force
                rigidbodies[ragdollId].AddForceAtPosition(force, point, ForceMode.Impulse);
            }
        }
    }
}