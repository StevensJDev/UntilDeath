using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Kit_PvE_ZombieWaveSurvival_ZombieAIDamage : MonoBehaviour, IKitDamageable
        {
            /// <summary>
            /// The AI this one belongs to
            /// </summary>
            public Kit_PvE_ZombieWaveSurvival_ZombieAI ai;
            /// <summary>
            /// Damage multiplier
            /// </summary>
            public float multiplier = 1f;
            /// <summary>
            /// Ragdoll ID
            /// </summary>
            public int ragdollId;

            void Start()
            {
                if (!ai)
                {
                    ai = GetComponentInParent<Kit_PvE_ZombieWaveSurvival_ZombieAI>();
                }
            }

            public bool LocalDamage(float dmg, int gunID, Vector3 shotPos, Vector3 forward, float force, Vector3 hitPos, bool shotBot, int shotId)
            {
                if (ai)
                {
                    //Give us money
                    Kit_PvE_ZombieWaveSurvival_ZombieAI.zws.localPlayerData.GainMoney(Kit_PvE_ZombieWaveSurvival_ZombieAI.zws.moneyPerHit);

                    //Relay to ai
                    ai.LocalDamage(dmg, gunID, shotPos, forward, force, hitPos, shotBot, shotId, ragdollId);

                    //Everyone can shoot zombies
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}