using Photon.Pun;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Kit_PvE_ZombieWaveSurvival_PlayerManager : MonoBehaviourPun, IPunObservable
        {
            /// <summary>
            /// Amount of money this player has
            /// </summary>
            public int money;
            [HideInInspector]
            /// <summary>
            /// ID this belongs to
            /// </summary>
            public int playerId;
            [HideInInspector]
            /// <summary>
            /// Does this belong to a bot?
            /// </summary>
            public bool playerIsBot;
            /// <summary>
            /// Last amount of money, to register changes for everyone.
            /// </summary>
            private int lastMoney;
            /// <summary>
            /// Main
            /// </summary>
            private Kit_IngameMain main;
            /// <summary>
            /// Player this info belongs to
            /// </summary>
            public Kit_PlayerBehaviour player
            {
                get
                {
                    if (!cachedPlayer)
                    {
                        //Find player
                        for (int i = 0; i < main.allActivePlayers.Count; i++)
                        {
                            if (main.allActivePlayers[i].isBot == playerIsBot && main.allActivePlayers[i].id == playerId)
                            {
                                int id = i;
                                cachedPlayer = main.allActivePlayers[id];
                                break;
                            }
                        }
                    }

                    return cachedPlayer;
                }
            }
            /// <summary>
            /// Cached reference to the player
            /// </summary>
            private Kit_PlayerBehaviour cachedPlayer;

            private void Start()
            {
                playerId = (int)photonView.InstantiationData[0];
                playerIsBot = (bool)photonView.InstantiationData[1];

                //Find main
                main = FindObjectOfType<Kit_IngameMain>();
            }

            public void SpendMoney(int moneyToSpend)
            {
                money -= moneyToSpend;
                CheckForMoneyChange();
            }

            public void GainMoney(int moneyToGain)
            {
                //Check for double points
                if (Kit_PvE_ZombieWaveSurvival_DropDoublePointsManager.instance)
                {
                    //Double them points!
                    moneyToGain *= 2;
                }

                if (Kit_PvE_ZombieWaveSurvival_DropCurseManager.instance) {
                    if ((money - moneyToGain) <= 0) {
                        money = 0;
                    } else {
                        money -= moneyToGain/2;
                    }
                } else {
                    money += moneyToGain;
                }
                CheckForMoneyChange();
            }

            void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(money);
                }
                else
                {
                    money = (int)stream.ReceiveNext();
                    CheckForMoneyChange();
                }
            }

            private void CheckForMoneyChange()
            {
                if (money != lastMoney)
                {
                    int change = (money - lastMoney);
                    //Display change in hud
                    (main.currentGameModeHUD as Kit_PvE_ZombieWaveSurvival_HUD).MoneyChanged(player, money, change);
                    lastMoney = money;
                }
            }
        }
    }
}