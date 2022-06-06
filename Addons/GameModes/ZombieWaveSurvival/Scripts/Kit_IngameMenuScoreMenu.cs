using Photon.Pun;
using TMPro;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Kit_IngameMenuScoreMenu : MonoBehaviour
        {
            /// <summary>
            /// Main reference
            /// </summary>
            public Kit_IngameMain main;
            private Kit_PvE_ZombieWaveSurvival zws;
            public TextMeshProUGUI nameUI;
            public TextMeshProUGUI scoreUI;
            public TextMeshProUGUI killsUI;
            public TextMeshProUGUI downsUI;
            public TextMeshProUGUI revivesUI;

            /// <summary>
            /// ID for the score menu
            /// </summary>
            public int pauseMenuId = 7;

            private string score = "0";
            private string kills = "0";
            private string downs = "0";
            private string revives = "0";

            private void Start()
            {
                main = FindObjectOfType<Kit_IngameMain>();
                if (main) {
                    zws = main.currentPvEGameModeBehaviour as Kit_PvE_ZombieWaveSurvival;
                }

                int money = zws.localPlayerData.getMoney();
                score = money.ToString();

                nameUI.text = Kit_GameSettings.userName;
                scoreUI.text = "$" + score;
                killsUI.text = kills;
                downsUI.text = downs;
                revivesUI.text = revives;   
            }

            private void Update()
            {
                Kit_PvE_ZombieWaveSurvival_WaveManager waveManager = FindObjectOfType<Kit_PvE_ZombieWaveSurvival_WaveManager>();
                if (zws) {
                    int money = zws.localPlayerData.getMoney();
                    score = money.ToString();
                } else {
                    zws = main.currentPvEGameModeBehaviour as Kit_PvE_ZombieWaveSurvival;
                }

                nameUI.text = Kit_GameSettings.userName;
                scoreUI.text = "$" + score;
                killsUI.text = waveManager.zombiesKilled.ToString();
                downsUI.text = downs;
                revivesUI.text = revives;
            }
        }
    }
}