using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Custom Flashlight that will work regardless of weapon
    /// </summary>
    public class Kit_Flashlight : MonoBehaviour
    {
        /// <summary>
        /// Light used for Third Person flashlight
        /// </summary>
        public Light FPflashlight;

        /// <summary>
        /// Is the flashlight enabled?
        /// </summary>
        private bool isFlashlightEnabled;

        void Update()
        {
            if (Input.GetButtonDown("Flashlight")) {
                isFlashlightEnabled = !isFlashlightEnabled;
            }

            if (FPflashlight) {
                FPflashlight.enabled = isFlashlightEnabled;
            }
        }
    }
}