using Photon.Pun;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class MysteryBox : Kit_InteractableObject
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
            public int mysteryBoxPrice;
            private bool isGeneratingWeapon;
            public int weaponToWin;
            /// <summary>
            /// Attachments for the weapon to buy
            /// </summary>
            public int[] weaponToBuyAttachments;
            /// <summary>
            /// How much ammo do we start with in the gun?
            /// </summary>
            public int weaponToBuyStartBullets = 30;
            /// <summary>
            /// How much ammo do we start with to reload?
            /// </summary>
            public int weaponToBuyStartBulletsLeftToReload = 60;


            private void Start()
            {
                //Find main reference
                main = FindObjectOfType<Kit_IngameMain>();
            }

            public override bool CanInteract(Kit_PlayerBehaviour who)
            {
                if (isGeneratingWeapon) {
                    return false;
                }
                if (!power.powerIsOn) {
                    interactionText = "Need to turn on the power.";
                    return false;
                }

                interactionText = "Press [" + PlayerPrefs.GetString("Interact", "F") + "] for a random weapon [$" + mysteryBoxPrice + "]";
                return true;
            }

            public override void Interact(Kit_PlayerBehaviour who)
            {
                if (zws.localPlayerData.money >= mysteryBoxPrice)
                {
                    if (power.powerIsOn) {
                        zws.localPlayerData.SpendMoney(mysteryBoxPrice);
                        // Select random weapon
                        randomWeaponGenerator(who);
                    }
                }
            }

            public void randomWeaponGenerator(Kit_PlayerBehaviour who) {
                isGeneratingWeapon = true;
                // Should have a timer that runs until the gun is available
                // Should not have the weapon(s) currently equiped
                // Shouldnt work with melee weapons, and should be able to remove certain ones,
                // Thinking it should be an int[] that you only add certain ones to.
                int randomNum = Random.Range(1, 16);

                int[] slot = new int[0];

                int[][] emptySlots = main.myPlayer.weaponManager.GetSlotsWithEmptyWeapon(who);

                if (emptySlots.Length > 0)
                {
                    for (int i = 0; i < emptySlots.Length; i++)
                    {
                        if (main.gameInformation.allWeapons[randomNum].canFitIntoSlots.Contains(emptySlots[i][0]))
                        {
                            int id = i;
                            //Set to the slot that it fits to
                            slot = emptySlots[id];
                        }
                    }
                }

                if (slot.Length == 0)
                {
                    slot = who.weaponManager.GetCurrentlySelectedWeapon(who);

                    if (!main.gameInformation.allWeapons[randomNum].canFitIntoSlots.Contains(slot[0]))
                    {
                        //Set to the slot that it fits to
                        slot[0] = main.gameInformation.allWeapons[randomNum].canFitIntoSlots[0];
                    }
                }

                who.photonView.RPC("ReplaceWeapon", RpcTarget.AllBuffered, slot, randomNum, weaponToBuyStartBullets, weaponToBuyStartBulletsLeftToReload, weaponToBuyAttachments);

                who.weaponManager.SetDesiredWeapon(who, slot);

                isGeneratingWeapon = false;
            }
        }
    }
}