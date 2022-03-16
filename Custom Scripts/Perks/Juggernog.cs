using Photon.Pun;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Juggernog : Kit_InteractableObject
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
            /// How much does juggernog cost?
            /// </summary>
            public int juggernogPrice;

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
                if(who.perksManager.playerHasJuggernog(who)){
                    interactionText = "You already have this perk.";
                    return false;
                } else {
                    interactionText = "Press [F] to buy Juggernog [$" + juggernogPrice + "]";
                    return true;
                }
            }

            public override void Interact(Kit_PlayerBehaviour who)
            {
                //Buy jugg
                if (zws.localPlayerData.money >= juggernogPrice)
                {
                    if (!main.myPlayer.perksManager.playerHasJuggernog(who) && power.powerIsOn) {
                        zws.localPlayerData.SpendMoney(juggernogPrice);
                        who.perksManager.AddJuggernog(who, 200); // Adds health by amount of 100
                    }
                }
            }
        }
    }
}