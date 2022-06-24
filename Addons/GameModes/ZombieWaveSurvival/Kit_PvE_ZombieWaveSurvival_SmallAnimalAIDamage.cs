using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Kit_PvE_ZombieWaveSurvival_SmallAnimalAIDamage : MonoBehaviour, IKitDamageable
        {
            /// <summary>
            /// The AI this one belongs to
            /// </summary>
            public Kit_PvE_ZombieWaveSurvival_SmallAnimalAI ai;
            /// <summary>
            /// Damage multiplier
            /// </summary>
            public float multiplier = 1f;
            /// <summary>
            /// Damage to apply
            /// </summary>
            public float damage = 20f;
            /// <summary>
            /// Which death sound to play
            /// </summary>
            public int deathSoundCategory;
            
            void Start()
            {
                if (!ai)
                {
                    ai = GetComponentInChildren<Kit_PvE_ZombieWaveSurvival_SmallAnimalAI>();
                }
            }

            public bool LocalDamage(float dmg, int gunID, Vector3 shotPos, Vector3 forward, float force, Vector3 hitPos, bool shotBot, int shotId)
            {
                if (ai)
                {
                    //Give us money
                    Kit_PvE_ZombieWaveSurvival_SmallAnimalAI.zws.localPlayerData.GainMoney(Kit_PvE_ZombieWaveSurvival_SmallAnimalAI.zws.moneyPerHit);
                    //Relay to ai
                    ai.LocalDamage(dmg, gunID);

                    //Everyone can shoot these
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private void OnTriggerEnter(Collider other)
            {
                Kit_PvE_ZombieWaveSurvival_ZombieDamageable damageable = other.GetComponentInParent<Kit_PvE_ZombieWaveSurvival_ZombieDamageable>();

                if (damageable != null)
                {
                    if (damageable.IsAlive())
                    {
                        damageable.ApplyDamage(damage);
                    }
                }
                
                if (ai.harmfulToPlayer) {
                    Kit_PlayerBehaviour pb = other.GetComponentInParent<Kit_PlayerBehaviour>();

                    if (pb)
                    {
                        if (pb.photonView.IsMine)
                        {
                            pb.vitalsManager.ApplyEnvironmentalDamage(pb, damage, deathSoundCategory);
                        }
                    }
                }
            }
        }
    }
}