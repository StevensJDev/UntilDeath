using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Kit_PvE_ZombieWaveSurvival_HUD : Kit_GameModeHUDBase
        {
            /// <summary>
            /// ZWS reference
            /// </summary>
            public Kit_PvE_ZombieWaveSurvival zws;

            /// <summary>
            /// Active when local player is alive
            /// </summary>
            public GameObject playerAliveRoot;

            /// <summary>
            /// Timer
            /// </summary>
            public TextMeshProUGUI timer;

            /// <summary>
            /// This source plays the sounds
            /// </summary>
            [Header("Round Sounds")]
            public AudioSource roundSoundSource;
            /// <summary>
            /// Round
            /// </summary>
            public TextMeshProUGUI roundNumber;
            /// <summary>
            /// Sound that plays when the round starts
            /// </summary>
            public AudioClip roundStartSound;
            /// <summary>
            /// Sound that plays when the round ends
            /// </summary>
            public AudioClip roundEndSound;

            /// <summary>
            /// Source for drops
            /// </summary>
            [Header("Drop Sounds")]
            public AudioSource dropSoundSource;

            /// <summary>
            /// Source that plays money sounds
            /// </summary>
            [Header("Money Sounds")]
            public AudioSource moneySoundSource;
            /// <summary>
            /// Sound that plays when spending money
            /// </summary>
            public AudioClip moneySpentSound;

            /// <summary>
            /// Root object
            /// </summary>
            [Header("New Wave")]
            public GameObject newWaveRoot;
            /// <summary>
            /// This displays the new round number! (e.g. 1,2,3 etc)
            /// </summary>
            public TextMeshProUGUI newWaveRoundNumber;
            /// <summary>
            /// Animation for new round
            /// </summary>
            public Animation newWaveAnimation;
            /// <summary>
            /// Length of the animation
            /// </summary>
            public float newWaveAnimationLength = 2f;

            [Header("Money")]
            /// <summary>
            /// Money
            /// </summary>
            public TextMeshProUGUI moneyAmount;
            /// <summary>
            /// Prefab for new money
            /// </summary>
            public GameObject moneyNewPrefab;
            /// <summary>
            /// Where new money goes
            /// </summary>
            public RectTransform moneyNewGo;
            /// <summary>
            /// Lifetime
            /// </summary>
            public float moneyNewLifeTime = 2f;

            /// <summary>
            /// Root for game over screen
            /// </summary>
            [Header("Game Over")]
            public GameObject gameOverScreenRoot;

            public TextMeshProUGUI gameOverScreenText;

            //For timer
            private int roundedRestSeconds;
            private int displaySeconds;
            private int displayMinutes;
            /// <summary>
            /// Reference to main
            /// </summary>
            private Kit_IngameMain main;

            public override void HUDInitialize(Kit_IngameMain main)
            {
                this.main = main;
                gameOverScreenText.text = "Are you even trying?";

                // 500 money
                moneyAmount.text = "$500";
            }

            public override void HUDUpdate(Kit_IngameMain main)
            {
                //Enable/disable hud
                playerAliveRoot.SetActiveOptimized(main.myPlayer);
                //Enable/disable game over screern
                gameOverScreenRoot.SetActiveOptimized(main.gameModeStage < 0);

                if (gameOverScreenRoot.activeSelf)
                {
                    //Unlock cursor during game over screen
                    MarsScreen.lockCursor = false;
                }

                if (main.gameModeStage % 2 == 0)
                {
                    roundedRestSeconds = Mathf.CeilToInt(main.timer);
                    displaySeconds = roundedRestSeconds % 60; //Get seconds
                    displayMinutes = roundedRestSeconds / 60; //Get minutes
                                                              //Update text
                    timer.text = string.Format("{0:00} : {1:00}", displayMinutes, displaySeconds);
                    timer.enabled = true;
                }
                else
                {
                    timer.enabled = false;
                }
            }

            public void MoneyChanged(Kit_PlayerBehaviour pb, int money, int change)
            {
                if (pb)
                {
                    if (pb.isFirstPersonActive)
                    {
                        //Set amount
                        moneyAmount.text = "$" + money.ToString();

                        if (change != 0)
                        {
                            //Create prefab and rotate it
                            GameObject go = Instantiate(moneyNewPrefab, moneyNewGo, false);
                            go.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));

                            //Set text
                            TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
                            if (change > 0)
                            {
                                txt.text = "+" + change.ToString();
                            }
                            else
                            {
                                txt.text = change.ToString();

                                //Play spent sound
                                moneySoundSource.clip = moneySpentSound;
                                moneySoundSource.Play();
                            }

                            //Destroy
                            Destroy(go, moneyNewLifeTime);
                        }
                    }
                }
            }

            public void RoundEnd(int round)
            {
                gameOverScreenText.text = "You lasted " + (round+1).ToString() + " rounds.";

                //Just play that sound
                roundSoundSource.clip = roundEndSound;
                roundSoundSource.Play();

                //And try to respawn, too
                main.Spawn();
            }

            public void RoundBegin(int round)
            {
                StartCoroutine(RoundBeginRoutine(round));

                //Play sound too
                roundSoundSource.clip = roundStartSound;
                roundSoundSource.Play();
            }

            IEnumerator RoundBeginRoutine(int round)
            {
                //Set text
                newWaveRoundNumber.text = "Round #" + round.ToString();
                //Set Round number text
                roundNumber.text = round.ToString(); // should probably change to be animation
                //Activate
                newWaveRoot.SetActive(true);
                //Play animation
                // newWaveAnimation.Rewind("NewRound");
                // newWaveAnimation.Play("NewRound");
                //Wait
                yield return new WaitForSeconds(newWaveAnimationLength);
                //Deactivate
                newWaveRoot.SetActive(false);
            }

            public void DropSoundPlay(int drop, int sound)
            {
                if (!dropSoundSource.isPlaying)
                {
                    dropSoundSource.clip = zws.allDrops[drop].dropSound[sound];
                    dropSoundSource.Play();
                }
            }
        }
    }
}