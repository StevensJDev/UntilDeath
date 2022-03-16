using Photon.Pun;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class BunnyHop : Kit_InteractableObject
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
            /// How much does bunnyHop cost?
            /// </summary>
            public int bunnyHopPrice;

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
                if(who.perksManager.playerHasBunnyHop(who)){
                    interactionText = "You already have this perk.";
                    return false;
                } else {
                    interactionText = "Press [F] to buy BunnyHop [$" + bunnyHopPrice + "]";
                    return true;
                }
            }

            public override void Interact(Kit_PlayerBehaviour who)
            {
                //Buy bunnyhop
                if (zws.localPlayerData.money >= bunnyHopPrice)
                {
                    if (!main.myPlayer.perksManager.playerHasBunnyHop(who) && power.powerIsOn) {
                        zws.localPlayerData.SpendMoney(bunnyHopPrice);
                        who.perksManager.AddBunnyHop(who); // Adds extra jump
                    }
                }
            }
        }
    }
}