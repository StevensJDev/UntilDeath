using Photon.Pun;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class TrapLever : Kit_InteractableObject
        {
            /// <summary>
            /// Rerefence to zws
            /// </summary>
            public Kit_PvE_ZombieWaveSurvival zws;
            [HideInInspector]
            /// <summary>
            /// reference to in game main
            /// </summary>
            public Kit_IngameMain main;
            public Power power;
            [Header("Settings")]

            /// <summary>
            /// How much does mystery box cost?
            /// </summary>
            public int trapPrice;


            private void Start()
            {
                //Find main reference
                main = FindObjectOfType<Kit_IngameMain>();
            }

            public override bool CanInteract(Kit_PlayerBehaviour who)
            {
                if (!power.powerIsOn) {
                    interactionText = "Need to turn on the power.";
                    return false;
                }

                interactionText = "Press [" + PlayerPrefs.GetString("Interact", "F") + "] to activate trap [$" + trapPrice + "]";
                return true;
            }

            public override void Interact(Kit_PlayerBehaviour who)
            {
                if (zws.localPlayerData.money >= trapPrice)
                {
                    if (power.powerIsOn) {
                        zws.localPlayerData.SpendMoney(trapPrice);
                        // Select random weapon
                    }
                }
            }

            public void randomWeaponGenerator(int weaponToBuy) {
                
            }
        }
    }
}