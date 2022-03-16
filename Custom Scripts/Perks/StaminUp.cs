using Photon.Pun;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class StaminUp : Kit_InteractableObject
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
            /// How much does StaminUp cost?
            /// </summary>
            public int staminUpPrice;

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
                if(who.perksManager.playerHasStaminUp(who)){
                    interactionText = "You already have this perk.";
                    return false;
                } else {
                    interactionText = "Press [F] to buy StaminUp [$" + staminUpPrice + "]";
                    return true;
                }
            }

            public override void Interact(Kit_PlayerBehaviour who)
            {
                //Buy staminup
                if (zws.localPlayerData.money >= staminUpPrice)
                {
                    if (!main.myPlayer.perksManager.playerHasStaminUp(who) && power.powerIsOn) {
                        zws.localPlayerData.SpendMoney(staminUpPrice);
                        who.perksManager.AddStaminUp(who); // Adds speed and removes stamina to player
                    }
                }
            }
        }
    }
}