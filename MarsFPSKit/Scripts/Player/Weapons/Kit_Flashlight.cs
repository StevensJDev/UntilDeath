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
            // Might be easier to create a manager flashlight on the character model that handles the input then just get one of the flashlights.
            int currentGun = pb.weaponManager.GetCurrentWeapon(pb);
            Debug.Log(pb.main.gameInformation.allWeapons[currentGun].weaponName);
            // Debug.Log(pb.weaponManager.GetCurrentlySelectedWeapon(pb)[1]);
            if (pb.input.flashlight) {
                isFlashlightEnabled = !isFlashlightEnabled;
            }

            if (FPflashlight) {
                FPflashlight.enabled = isFlashlightEnabled;
            }
        }
    }
}