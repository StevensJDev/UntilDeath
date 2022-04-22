using Photon.Pun;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Kit_PvE_ZombieWaveSurvival_WeaponPurchase : Kit_InteractableObject
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
            [Header("Settings")]
            /// <summary>
            /// Which weapon id can we buy here?
            /// </summary>
            public int weaponToBuy;
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
            /// <summary>
            /// How much does the weapon cost?
            /// </summary>
            public int weaponPrice;
            /// <summary>
            /// How much does it cost when we buy ammo?
            /// </summary>
            public int ammoPrice;
            /// <summary>
            /// Buy ammo text
            /// </summary>
            private static readonly string ammoText = "to buy ammo for ";
            /// <summary>
            /// Buy weapon text
            /// </summary>
            private static readonly string weaponText = "to buy ";

            private void Start()
            {
                //Find main reference
                main = FindObjectOfType<Kit_IngameMain>();
            }

            public override bool CanInteract(Kit_PlayerBehaviour who)
            {
                if (who.weaponManager.GetCurrentWeapon(who) == weaponToBuy)
                {
                    if (!who.weaponManager.IsCurrentWeaponFull(who))
                    {
                        interactionText = "Press [" + PlayerPrefs.GetString("Interact", "F") + "] " + ammoText + main.gameInformation.allWeapons[weaponToBuy].weaponName + " [$" + ammoPrice + "]";

                        if (zws.localPlayerData.money >= ammoPrice)
                        {
                            //Buy ammo
                            return true;
                        }
                    }
                }
                else
                {
                    //Check if we already have that gun equipped
                    if (main.myPlayer.weaponManager.CanBuyWeapon(main.myPlayer, weaponToBuy))
                    {
                        interactionText = "Press [" + PlayerPrefs.GetString("Interact", "F") + "] " + weaponText + main.gameInformation.allWeapons[weaponToBuy].weaponName + " [$" + weaponPrice + "]";

                        if (zws.localPlayerData.money >= weaponPrice)
                        {
                            //Buy weapon
                            return true;
                        }
                    }
                }

                return false;
            }

            public override void Interact(Kit_PlayerBehaviour who)
            {
                if (who.weaponManager.GetCurrentWeapon(who) == weaponToBuy)
                {
                    if (!who.weaponManager.IsCurrentWeaponFull(who))
                    {
                        //Buy ammo
                        if (zws.localPlayerData.money >= ammoPrice)
                        {
                            zws.localPlayerData.SpendMoney(ammoPrice);
                            //Since we have this weapon selected in the slot, restock ammo for current slot
                            who.weaponManager.RestockAmmo(who, false);
                        }
                    }
                }
                else
                {
                    //Buy weapon
                    if (zws.localPlayerData.money >= weaponPrice)
                    {
                        //Check if we already have that gun equipped
                        if (main.myPlayer.weaponManager.CanBuyWeapon(who, weaponToBuy))
                        {
                            //Spend that mf money and get the economy going
                            zws.localPlayerData.SpendMoney(weaponPrice);

                            int[] slot = new int[0];

                            int[][] emptySlots = main.myPlayer.weaponManager.GetSlotsWithEmptyWeapon(who);

                            if (emptySlots.Length > 0)
                            {
                                for (int i = 0; i < emptySlots.Length; i++)
                                {
                                    if (main.gameInformation.allWeapons[weaponToBuy].canFitIntoSlots.Contains(emptySlots[i][0]))
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

                                if (!main.gameInformation.allWeapons[weaponToBuy].canFitIntoSlots.Contains(slot[0]))
                                {
                                    //Set to the slot that it fits to
                                    slot[0] = main.gameInformation.allWeapons[weaponToBuy].canFitIntoSlots[0];
                                }
                            }
                            

                            who.photonView.RPC("ReplaceWeapon", RpcTarget.AllBuffered, slot, weaponToBuy, weaponToBuyStartBullets, weaponToBuyStartBulletsLeftToReload, weaponToBuyAttachments);

                            who.weaponManager.SetDesiredWeapon(who, slot);
                        }
                    }
                }
            }
        }
    }
}