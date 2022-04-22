using Photon.Pun;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Power : Kit_InteractableObject
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

            public bool powerIsOn = false;

            private void Start()
            {
                //Find main reference
                main = FindObjectOfType<Kit_IngameMain>();
            }

            public override bool CanInteract(Kit_PlayerBehaviour who)
            {
                if(powerIsOn){
                    interactionText = "Power is already on.";
                    return false;
                } else {
                    interactionText = "Press [" + PlayerPrefs.GetString("Interact", "F") + "] to turn on power.";
                    return true;
                }
            }

            public override void Interact(Kit_PlayerBehaviour who)
            {
                if (!powerIsOn) {
                    powerIsOn = true; // should be on for all players
                }
            }
        }
    }
}