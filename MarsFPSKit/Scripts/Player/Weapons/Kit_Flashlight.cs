using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Custom Flashlight that will work regardless of weapon
    /// </summary>
    public class Kit_Flashlight : MonoBehaviour
    {
        private Kit_PlayerBehaviour pb;
        /// <summary>
        /// Light used for Third Person flashlight
        /// </summary>
        public Light FPflashlight;

        /// <summary>
        /// Is the flashlight enabled?
        /// </summary>
        private bool isFlashlightEnabled;

        void Start() 
        {
            pb = this.GetComponentInParent<Kit_PlayerBehaviour>();
        }

        void Update()
        {
            UseFlashlight();
        }

        void UseFlashlight() {
            if (pb.input.flashlight) {
                isFlashlightEnabled = !isFlashlightEnabled;
            }

            if (FPflashlight) {
                FPflashlight.enabled = isFlashlightEnabled;
            }
        }
    }
}