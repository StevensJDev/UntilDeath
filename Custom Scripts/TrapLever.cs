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
            public GameObject damageObj;
            public GameObject otherTrap;
            [Header("Settings")]

            /// <summary>
            /// How much does mystery box cost?
            /// </summary>
            public int trapPrice;
            public float trapTimer = 15f;
            private bool trapCoolingDown = false;
            private bool trapRunning = false;
            private bool trapReady = true;
            public float trapCooldownTimer = 60f;
            private float timer = 0;        

            private void Start()
            {
                //Find main reference
                main = FindObjectOfType<Kit_IngameMain>();
                damageObj.SetActive(false);
            }

            public override bool CanInteract(Kit_PlayerBehaviour who)
            {
                if (!power.powerIsOn) {
                    interactionText = "Need to turn on the power.";
                    return false;
                }

                if (trapCoolingDown || otherTrap.GetComponent<TrapLever>().trapCoolingDown) {
                    interactionText = "Trap cooling down.";
                    return false;
                }

                if (trapRunning || otherTrap.GetComponent<TrapLever>().trapRunning) {
                    interactionText = "Trap running.";
                    return false;
                }

                interactionText = "Press [" + PlayerPrefs.GetString("Interact", "F") + "] to activate trap [$" + trapPrice + "]";
                return true;
            }

            public override void Interact(Kit_PlayerBehaviour who)
            {
                if (zws.localPlayerData.money >= trapPrice)
                {
                    zws.localPlayerData.SpendMoney(trapPrice);
                    damageObj.SetActive(true);
                    trapReady = false;
                    trapRunning = true;
                }
            }

            void Update() {
                if (!trapReady) {
                    if (trapRunning && trapTimer >= timer) {
                        timer += Time.deltaTime;
                    } else if (trapRunning && !trapCoolingDown) {
                        trapRunning = false;
                        damageObj.SetActive(false);
                        trapCoolingDown = true;
                        timer = 0;
                    }

                    if (trapCoolingDown && trapCooldownTimer >= timer) {
                        timer += Time.deltaTime;
                    } else if (!trapRunning && trapCoolingDown) {
                        trapCoolingDown = false;
                        timer = 0;
                        trapReady = true;
                    }
                }
            }
        }
    }
}