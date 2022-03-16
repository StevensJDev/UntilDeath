using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        /// <summary>
        /// Keeps rotation steady
        /// </summary>
        public class Kit_PvE_ZombieWaveSurvival_HUDKeepRotationSteady : MonoBehaviour
        {
            private void Update()
            {
                //Rotate
                transform.rotation = Quaternion.identity;
            }
        }
    }
}