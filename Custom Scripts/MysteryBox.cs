using Photon.Pun;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MarsFPSKit.Weapons;

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
            private bool mysteryBoxActivated = false;
            private bool isGeneratingWeapon = false;
            private bool generationDone = false;
            public float boxGenerationTime = 3f;
            private float timer = 0f;
            private float runoutTimer = 10f;
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
            /// Which weapon id can we buy here?
            /// </summary>
            public int weaponToBuy;

            private void Start()
            {
                //Find main reference
                main = FindObjectOfType<Kit_IngameMain>();
                timer = 0f;
            }

            void Update() {
                if (mysteryBoxActivated && isGeneratingWeapon && !generationDone) {
                    timer += Time.deltaTime;
                    if (timer >= boxGenerationTime) {
                        isGeneratingWeapon = false;
                        generationDone = true;
                    }
                }

                if (mysteryBoxActivated && !isGeneratingWeapon && generationDone) {
                    runoutTimer -= Time.deltaTime;
                    if (runoutTimer <= 0) {
                        mysteryBoxActivated = false;
                        generationDone = false;
                        runoutTimer = 10f;
                        timer = 0f;
                    }
                }
                // Should not have the weapon(s) currently equiped, so far doesnt seem to be?
                // Shouldnt work with melee weapons (yet), and should be able to remove certain ones,
                // Thinking it should be an int[] that you only add certain ones to.
                // Should also be setting the ammo based on the guns max ammo, not just to 30.
                // Will need to implement some of this stuff with Pack a punch.
            }

            public override bool CanInteract(Kit_PlayerBehaviour who)
            {
                if (!power.powerIsOn) {
                    interactionText = "Need to turn on the power.";
                    return false;
                }
                if (isGeneratingWeapon) {
                    interactionText = "Generating Weapon.";
                    return false;
                }

                if (!mysteryBoxActivated) {
                    interactionText = "Press [" + PlayerPrefs.GetString("Interact", "F") + "] for a random weapon [$" + mysteryBoxPrice + "]";
                    return true;
                } else {
                    interactionText = "Press [" + PlayerPrefs.GetString("Interact", "F") + "] for " + main.gameInformation.allWeapons[weaponToBuy].weaponName;
                    return true;
                }
            }

            public override void Interact(Kit_PlayerBehaviour who)
            {
                if (zws.localPlayerData.money >= mysteryBoxPrice)
                {
                    if (!mysteryBoxActivated) {
                        zws.localPlayerData.SpendMoney(mysteryBoxPrice);
                        // Select random weapon
                        startGeneration(who);
                    } else {
                        gatherWeapon(who);
                    }
                }
            }

            public void startGeneration(Kit_PlayerBehaviour who) {
                mysteryBoxActivated = true;
                isGeneratingWeapon = true;
                generationDone = false;
                int[] hardCodedList = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
                List<int> weapons = new List<int>(hardCodedList);

                WeaponManagerControllerRuntimeData runtimeData = who.customWeaponManagerData as WeaponManagerControllerRuntimeData;
                int weaponId1 = runtimeData.weaponsInUse[0].weaponsInSlot[0].id;
                int weaponId2 = runtimeData.weaponsInUse[1].weaponsInSlot[0].id;
                
                // Remove currently selected weapon numbers from the array
                for (int i = 0; i < weapons.Count; i++) {
                    if (weapons[i] == weaponId1 || weapons[i] == weaponId2) {
                        weapons.RemoveAt(i);
                    }
                }
                weaponToBuy = weapons[Random.Range(1, weapons.Count)];
            }

            public void gatherWeapon(Kit_PlayerBehaviour who) {
                int[] slot = new int[0];

                int[][] emptySlots = main.myPlayer.weaponManager.GetSlotsWithEmptyWeapon(who);

                if (generationDone) {
                    // This code is what should happen after its been selected
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
                    who.weaponManager.RestockAmmo(who, false);
                    mysteryBoxActivated = false;
                    isGeneratingWeapon = false;
                    generationDone = false;
                    timer = 0f;

                    who.photonView.RPC("ReplaceWeapon", RpcTarget.AllBuffered, slot, weaponToBuy, weaponToBuyStartBullets, weaponToBuyStartBulletsLeftToReload, weaponToBuyAttachments);
                    who.weaponManager.SetDesiredWeapon(who, slot);
                }
            }
        }
    }
}