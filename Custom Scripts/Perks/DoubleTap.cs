using Photon.Pun;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class DoubleTap : Kit_InteractableObject
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
            /// How much does doubletap cost?
            /// </summary>
            public int doubleTapPrice;

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
                if(who.perksManager.playerHasDoubleTap(who)){
                    interactionText = "You already have this perk.";
                    return false;
                } else {
                    interactionText = "Press [" + PlayerPrefs.GetString("Interact", "F") + "] to buy Double Tap [$" + doubleTapPrice + "]";
                    return true;
                }
            }

            public override void Interact(Kit_PlayerBehaviour who)
            {
                //Buy speed
                if (zws.localPlayerData.money >= doubleTapPrice)
                {
                    if (!main.myPlayer.perksManager.playerHasDoubleTap(who) && power.powerIsOn) {
                        zws.localPlayerData.SpendMoney(doubleTapPrice);
                        who.perksManager.AddDoubleTap(who);
                        main.gameInformation.statistics.AddPerk();//Call statistics
                    }
                }
            }
        }
    }
}