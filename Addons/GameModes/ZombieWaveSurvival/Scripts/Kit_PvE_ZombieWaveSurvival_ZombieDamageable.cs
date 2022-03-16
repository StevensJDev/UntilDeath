using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        /// <summary>
        /// Implement this in a mono behaviour and zombies will damage this upon entering a trigger with this
        /// Requires a trigger
        /// </summary>
        public interface Kit_PvE_ZombieWaveSurvival_ZombieDamageable
        {
            /// <summary>
            /// Should a zombie still attack this?
            /// </summary>
            /// <returns></returns>
            bool IsAlive();

            /// <summary>
            /// Where we should move
            /// </summary>
            /// <returns></returns>
            Vector3 GetAttackPosition();

            /// <summary>
            /// Where the zombie should look at (not a direction, this is a world position)
            /// </summary>
            /// <returns></returns>
            Vector3 GetAttackRotationLookAt();

            /// <summary>
            /// Apply damage to this damageable
            /// </summary>
            /// <param name="dmg"></param>
            void ApplyDamage(float dmg);
        }
    }
}