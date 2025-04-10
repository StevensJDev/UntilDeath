using Photon.Pun;
using System.Linq;
using UnityEngine;
using MarsFPSKit.Weapons;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival {
        public class PackaPunch : Kit_InteractableObject
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
            public int packaPunchPrice;
            private bool packaPunchActivated = false;
            private bool isUpgradingWeapon = false;
            private bool upgradingDone = false;
            public float upgradingTime = 3f;
            private float timer = 0f;
            private float runoutTimer = 10f;

            /// <summary>
            /// Attachments for the weapon to buy
            /// </summary>
            public int[] weaponToUpgradeAttachments;
            /// <summary>
            /// How much ammo do we start with in the gun?
            /// </summary>
            public int weaponToUpgradeStartBullets = 30;
            /// <summary>
            /// How much ammo do we start with to reload?
            /// </summary>
            public int weaponToUpgradeStartBulletsLeftToReload = 60;
            /// <summary>
            /// Which weapon id can we buy here?
            /// </summary>
            public int weaponToUpgrade;
            /// <summary>
            /// Which weapon id can we buy here?
            /// </summary>
            public int otherWeapon;

            private void Start()
            {
                //Find main reference
                main = FindObjectOfType<Kit_IngameMain>();
                timer = 0f;
            }

            void Update() {
                if (packaPunchActivated && isUpgradingWeapon && !upgradingDone) {
                    timer += Time.deltaTime;
                    if (timer >= upgradingTime) {
                        isUpgradingWeapon = false;
                        upgradingDone = true;
                    }
                }

                if (packaPunchActivated && !isUpgradingWeapon && upgradingDone) {
                    runoutTimer -= Time.deltaTime;
                    if (runoutTimer <= 0) {
                        packaPunchActivated = false;
                        upgradingDone = false;
                        runoutTimer = 10f;
                        timer = 0f;
                    }
                }
            }

            public override bool CanInteract(Kit_PlayerBehaviour who)
            {
                WeaponManagerControllerRuntimeData runtimeData = who.customWeaponManagerData as WeaponManagerControllerRuntimeData;
                int weaponId1 = runtimeData.weaponsInUse[0].weaponsInSlot[0].id;
                int weaponId2 = runtimeData.weaponsInUse[1].weaponsInSlot[0].id;

                if (!power.powerIsOn) {
                    interactionText = "Need to turn on the power.";
                    return false;
                }

                if (isUpgradingWeapon) {
                    interactionText = "Upgrading Weapon.";
                    return false;
                }

                if (packaPunchActivated) {
                    interactionText = "Press [" + PlayerPrefs.GetString("Interact", "F") + "] for upgraded weapon.";
                    return true;
                } else if (weaponId1 == 0 && weaponId2 == 0) {
                    interactionText = "You have no weapon :c";
                    return false;
                } else {
                    interactionText = "Press [" + PlayerPrefs.GetString("Interact", "F") + "] to upgrade weapon [$" + packaPunchPrice + "]";
                    return true;
                }
            }

            public override void Interact(Kit_PlayerBehaviour who)
            {
                if (zws.localPlayerData.money >= packaPunchPrice)
                {
                    if (!packaPunchActivated) {
                        zws.localPlayerData.SpendMoney(packaPunchPrice);
                        startUpgrade(who);
                    } else {
                        gatherWeapon(who);
                    }
                }
            }

            public void startUpgrade(Kit_PlayerBehaviour who) {
                removeWeapon(who);

                packaPunchActivated = true;
                isUpgradingWeapon = true;
                upgradingDone = false;
            }

            public void removeWeapon(Kit_PlayerBehaviour who) {
                WeaponManagerControllerRuntimeData runtimeData = who.customWeaponManagerData as WeaponManagerControllerRuntimeData;
                int weaponId1 = runtimeData.weaponsInUse[0].weaponsInSlot[0].id;
                int weaponId2 = runtimeData.weaponsInUse[1].weaponsInSlot[0].id;

                // BUG: If no weapons, then will activate the swimming anim after hitting.
                if (weaponId1 == 0 || weaponId2 == 0) {
                    if (weaponId1 == 0) {
                        weaponToUpgrade = weaponId2;
                    }

                    if (weaponId2 == 0) {
                        weaponToUpgrade = weaponId1;
                    }

                    // Only have one weapon
                    int[] slot = who.weaponManager.GetCurrentlySelectedWeapon(who);
                    who.photonView.RPC("ReplaceWeapon", RpcTarget.AllBuffered, slot, 0, 0, 0, weaponToUpgradeAttachments); // If no other weapons select nothing
                    who.weaponManager.SetDesiredWeapon(who, slot);
                } else {
                    // CLOSE, seems theres still an issue with being able to switch weapons with nothing.
                    int currentGun = who.weaponManager.GetCurrentWeapon(who);
                    if (currentGun == weaponId1) {
                        weaponToUpgrade = weaponId1;

                        int[] slot = who.weaponManager.GetCurrentlySelectedWeapon(who);
                        who.photonView.RPC("ReplaceWeapon", RpcTarget.AllBuffered, slot, 0, 0, 0, weaponToUpgradeAttachments);
                        who.weaponManager.SetDesiredWeapon(who, slot);

                        runtimeData.desiredWeapon[0] = 1;
                        runtimeData.desiredWeapon[1] = 0;
                    } 
                    else if (currentGun == weaponId2) {
                        weaponToUpgrade = weaponId2;

                        int[] slot = who.weaponManager.GetCurrentlySelectedWeapon(who);
                        who.photonView.RPC("ReplaceWeapon", RpcTarget.AllBuffered, slot, 0, 0, 0, weaponToUpgradeAttachments);
                        who.weaponManager.SetDesiredWeapon(who, slot);

                        runtimeData.desiredWeapon[0] = 0;
                        runtimeData.desiredWeapon[1] = 0;
                    }
                }
            }

            public void gatherWeapon(Kit_PlayerBehaviour who) {
                int[] slot = new int[0];

                int[][] emptySlots = main.myPlayer.weaponManager.GetSlotsWithEmptyWeapon(who);

                if (upgradingDone) {
                    who.weaponManager.RestockAmmo(who, false);
                    // NEED TO UPGRADE GUN HERE
                    // This should be changing the gun model altogether instead I think... Which means another set of ints will have to work the same way somehow, maybe a separate, upgraded list will work instead with the same number...

                    // This code is what should happen after its been selected
                    if (emptySlots.Length > 0)
                    {
                        for (int i = 0; i < emptySlots.Length; i++)
                        {
                            if (main.gameInformation.allWeapons[weaponToUpgrade].canFitIntoSlots.Contains(emptySlots[i][0]))
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

                        if (!main.gameInformation.allWeapons[weaponToUpgrade].canFitIntoSlots.Contains(slot[0]))
                        {
                            //Set to the slot that it fits to
                            slot[0] = main.gameInformation.allWeapons[weaponToUpgrade].canFitIntoSlots[0];
                        }
                    }
                    packaPunchActivated = false;
                    isUpgradingWeapon = false;
                    upgradingDone = false;
                    timer = 0f;

                    who.photonView.RPC("ReplaceWeapon", RpcTarget.AllBuffered, slot, weaponToUpgrade, weaponToUpgradeStartBullets, weaponToUpgradeStartBulletsLeftToReload, weaponToUpgradeAttachments);
                    who.weaponManager.SetDesiredWeapon(who, slot);
                }
            }
        }
    }
}