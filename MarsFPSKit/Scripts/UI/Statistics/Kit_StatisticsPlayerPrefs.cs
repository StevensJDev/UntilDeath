using MarsFPSKit.UI;
using UnityEngine;

namespace MarsFPSKit
{
    [CreateAssetMenu(menuName = "MarsFPSKit/Statistics/PlayerPrefsBased")]
    /// <summary>
    /// Implements simple statistics with player prefs
    /// </summary>
    public class Kit_StatisticsPlayerPrefs : Kit_StatisticsBase
    {
        /// <summary>
        /// Our accumulated kills
        /// </summary>
        public int kills;
        /// <summary>
        /// Our accumulated deaths
        /// </summary>
        public int deaths;
        /// <summary>
        /// Our accumulated assists
        /// </summary>
        public int assists;

        public int rounds;

        public int highestRound;

        public int escapes;

        public override void OnAssist(Kit_IngameMain main)
        {
            assists++;
        }

        public override void OnDeath(Kit_IngameMain main, int weapon)
        {
            deaths++;
        }

        public override void OnDeath(Kit_IngameMain main, string weapon)
        {
            deaths++;
        }

        public override void OnKill(Kit_IngameMain main, int weapon)
        {
            kills++;
        }

        public override void OnKill(Kit_IngameMain main, string reason)
        {
            kills++;
        }

        public override void OnRound(Kit_IngameMain main, int gameRounds)
        {
            rounds++;
            if (gameRounds > highestRound) {
                highestRound++;
            }
        }

        public override void OnEscape(Kit_IngameMain main)
        {
            escapes++;
        }

        public override void OnStart(Kit_MenuManager menu)
        {
            //Reset
            kills = 0;
            deaths = 0;
            assists = 0;
            rounds = 0;
            highestRound = 0;
            escapes = 0;

            //Then load
            kills = PlayerPrefs.GetInt(Kit_GameSettings.userName + "_kills", 0);
            deaths = PlayerPrefs.GetInt(Kit_GameSettings.userName + "_deaths", 0);
            assists = PlayerPrefs.GetInt(Kit_GameSettings.userName + "_assists", 0);
            rounds = PlayerPrefs.GetInt(Kit_GameSettings.userName + "_rounds", 0);
            highestRound = PlayerPrefs.GetInt(Kit_GameSettings.userName + "_highestRound", 0);
            escapes = PlayerPrefs.GetInt(Kit_GameSettings.userName + "_escapes", 0);
        }

        public override void Save(Kit_IngameMain main)
        {
            //Save all
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_kills", kills);
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_deaths", deaths);
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_assists", assists);
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_rounds", rounds);
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_highestRound", highestRound);
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_escapes", escapes);
        }

        public override void Save(Kit_MenuManager menu)
        {
            //Save all
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_kills", kills);
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_deaths", deaths);
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_assists", assists);
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_rounds", rounds);
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_highestRound", highestRound);
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_escapes", escapes);
        }
    }
}