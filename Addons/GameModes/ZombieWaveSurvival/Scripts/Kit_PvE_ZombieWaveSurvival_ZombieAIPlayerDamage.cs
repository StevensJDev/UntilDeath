using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        /// <summary>
        /// Applies damage on trigger enter
        /// </summary>
        public class Kit_PvE_ZombieWaveSurvival_ZombieAIPlayerDamage : MonoBehaviour
        {
            /// <summary>
            /// Renderer of the zombie
            /// </summary>
            public Kit_PvE_ZombieWaveSurvival_ZombieAIRenderer zombieRenderer;

            /// <summary>
            /// Damage to apply
            /// </summary>
            public float damage = 20f;
            /// <summary>
            /// Which death sound to play
            /// </summary>
            public int deathSoundCategory;
            /// <summary>
            /// Did we damage the player?
            /// </summary>
            private bool damageWasApplied = false;

            private void OnEnable()
            {
                damageWasApplied = false;
            }

            private void OnTriggerEnter(Collider other)
            {
                if (!damageWasApplied && zombieRenderer && zombieRenderer.zombie)
                {
                    Kit_PvE_ZombieWaveSurvival_ZombieDamageable damageable = other.GetComponentInParent<Kit_PvE_ZombieWaveSurvival_ZombieDamageable>();

                    if (damageable != null)
                    {
                        if (damageable.IsAlive())
                        {
                            damageable.ApplyDamage(damage);
                            damageWasApplied = true;
                        }
                    }

                    Kit_PlayerBehaviour pb = other.GetComponentInParent<Kit_PlayerBehaviour>();

                    if (pb)
                    {
                        if (pb.photonView.IsMine)
                        {
                            pb.vitalsManager.ApplyEnvironmentalDamage(pb, damage, deathSoundCategory);
                            damageWasApplied = true;
                        }
                    }
                }
            }
        }
    }
}