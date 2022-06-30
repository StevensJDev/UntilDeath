using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        [System.Serializable]
        public class ZombieSpawnPrefab
        {
            [Tooltip("The actual prefab")]
            /// <summary>
            /// The actual prefab
            /// </summary>
            public GameObject prefab;
            [Tooltip("The skins that are available to use for this zombie")]
            /// <summary>
            /// The skins that are available to use for this zombie
            /// </summary>
            public GameObject[] skins;
            [Tooltip("After which wave can this prefab spawn?")]
            /// <summary>
            /// After which wave (inclusive) can this prefab spawn?
            /// </summary>
            public int spawnAfterWave;
            [Tooltip("Until which wave (inclusive) can this zombie spawn?")]
            /// <summary>
            /// Until which wave (inclusive) can this zombie spawn?
            /// </summary>
            public int spawnUntilWave;
        }

        [CreateAssetMenu(menuName = "MarsFPSKit/Addons/Zombie Wave Survival/Game Mode")]
        public class Kit_PvE_ZombieWaveSurvival : Kit_PvE_GameModeBase
        {
            /// <summary>
            /// Characters that can be played in this game mode
            /// </summary>
            public Kit_PvE_ZombieWaveSurvival_Character[] characters;

            /// <summary>
            /// Runtime assigned wave manager for callbacks
            /// </summary>
            public Kit_PvE_ZombieWaveSurvival_WaveManager waveManager;

            /// <summary>
            /// Local player's data
            /// </summary>
            public Kit_PvE_ZombieWaveSurvival_PlayerManager localPlayerData
            {
                get
                {
                    if (!lpd)
                    {
                        Kit_PvE_ZombieWaveSurvival_PlayerManager[] managers = FindObjectsOfType<Kit_PvE_ZombieWaveSurvival_PlayerManager>();

                        for (int i = 0; i < managers.Length; i++)
                        {
                            if (!managers[i].playerIsBot && managers[i].playerId == PhotonNetwork.LocalPlayer.ActorNumber)
                            {
                                int id = i;
                                lpd = managers[id];
                                break;
                            }
                        }
                    }

                    return lpd;
                }
            }
            /// <summary>
            /// Reference stored to local player's data
            /// </summary>
            private Kit_PvE_ZombieWaveSurvival_PlayerManager lpd;

            [Tooltip("Prefab for wave survival manager")]
            [Header("Zombie Wave Survival Settings")]
            /// <summary>
            /// Prefab for wave survival manager
            /// </summary>
            public GameObject waveSurvivalRoundManagerPrefab;
            [Tooltip("How much time before first round starts?")]
            /// <summary>
            /// How much time before first round starts?
            /// </summary>
            public float waitTimeBeforeFirstRound = 30f;
            [Tooltip("How much time between rounds")]
            /// <summary>
            /// How much time between rounds
            /// </summary>
            public float waitTimeBetweenRounds = 30f;
            [Tooltip("How much time between spawns?")]
            /// <summary>
            /// How much time between spawns?
            /// </summary>
            public float timeBetweenZombieSpawns = 1f;
            [Tooltip("How many zombies can we spawn at once?")]
            /// <summary>
            /// How many zombies can we spawn at once?
            /// </summary>
            public int maxAmountOfZombieSpawnsAtOnce;
            [Tooltip("How many zombies can be alive at a time?")]
            /// <summary>
            /// How many zombies can be alive at a time?
            /// </summary>
            public int maxAmountOfZombiesAliveAtOnce = 30;
            [Tooltip("Zombie prefabs")]
            /// <summary>
            /// Zombie prefabs
            /// </summary>
            public ZombieSpawnPrefab[] zombiePrefabs;
            [Tooltip("Prefab for local player data")]
            /// <summary>
            /// Prefab for local player data
            /// </summary>
            public GameObject localPlayerDataPrefab;
            [Header("Money")]
            [Tooltip("How much money we get for one hit on a zombie")]
            /// <summary>
            /// How much money we get for one hit on a zombie
            /// </summary>
            public int moneyPerHit = 20;
            [Tooltip("How much money we get if we kill a zombie")]
            /// <summary>
            /// How much money we get if we kill a zombie
            /// </summary>
            public int moneyPerKill = 100;

            [Tooltip("How long the game displays the game over screen for us")]
            /// <summary>
            /// How long the game displays the game over screen for us
            /// </summary>
            [Header("Game Over")]
            public float gameOverScreenTime = 18f;
            [Tooltip("All drops that can occur")]
            /// <summary>
            /// All drops that can occur
            /// </summary>
            [Header("Drops")]
            public Kit_PvE_ZombieWaveSurvival_DropBase[] allDrops;
            [Tooltip("Prefab to pickup drops")]
            /// <summary>
            /// Prefab to pickup drops
            /// </summary>
            public GameObject dropPickupPrefab;

            public override void ResetStats(Hashtable table)
            {
                //Keep the character setting
                Hashtable oldTable = PhotonNetwork.LocalPlayer.CustomProperties;
                if (oldTable.ContainsKey("zwsChar"))
                {
                    int zwsChar = (int)oldTable["zwsChar"];
                    table.Add("zwsChar", zwsChar);
                }
            }


            public override PlayerModelConfig GetPlayerModel(Kit_PlayerBehaviour pb)
            {
                if (pb.isBot)
                {
                    //Fallback, bots not supported yet
                    return characters[0].playerModel;
                }
                else
                {
                    int character = (int)pb.photonView.Owner.CustomProperties["zwsCharacter"];

                    //Get random player model
                    return characters[character].playerModel;
                }
            }

            public override Loadout GetSpawnLoadout()
            {
                int character = (int)PhotonNetwork.LocalPlayer.CustomProperties["zwsCharacter"];

                //Get random player model
                return characters[character].loadout;
            }

            public override bool CanControlPlayer(Kit_IngameMain main)
            {
                return true;
            }

            public override bool CanSpawn(Kit_IngameMain main, Player player)
            {
                //even means we are between a round
                return main.gameModeStage % 2 == 0;
            }

            public override bool CanStartVote(Kit_IngameMain main)
            {
                return false;
            }

            public override void GameModeProceed(Kit_IngameMain main)
            {
                //No team selection here
                main.pauseMenuState = PauseMenuState.main;

                if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("zwsCharacter"))
                {
                    main.Spawn(true);
                }
                else
                {
                    //Spectate
                    main.spectatorManager.BeginSpectating(main, false);
                    //Open ingame character selection
                }
            }

            public override void OnLocalPlayerDeathCameraEnded(Kit_IngameMain main)
            {
                if (CanSpawn(main, PhotonNetwork.LocalPlayer))
                {
                    //Just respawn
                    main.Spawn();
                }
                else
                {
                    //If game is not game over, start spectating
                    if (main.gameModeStage >= 0)
                    {
                        //Spectate
                        main.spectatorManager.BeginSpectating(main, false);
                    }
                }
            }

            public override void GamemodeSetup(Kit_IngameMain main)
            {
                //Get all spawns
                Kit_PlayerSpawn[] allSpawns = FindObjectsOfType<Kit_PlayerSpawn>();
                //Are there any spawns at all?
                if (allSpawns.Length <= 0) throw new Exception("This scene has no spawns.");
                //Filter all spawns that are appropriate for this game mode
                List<Kit_PlayerSpawn> filteredSpawns = new List<Kit_PlayerSpawn>();
                //Highest spawn index
                int highestIndex = 0;
                for (int i = 0; i < allSpawns.Length; i++)
                {
                    int id = i;
                    //Check if that spawn is useable for this game mode logic
                    if (allSpawns[id].singleplayerGameModes.Contains(this) && main.currentGameModeType == 0)
                    {
                        //Add it to the list
                        filteredSpawns.Add(allSpawns[id]);
                        //Set highest index
                        if (allSpawns[id].spawnGroupID > highestIndex) highestIndex = allSpawns[id].spawnGroupID;
                    }
                    else if (allSpawns[id].coopGameModes.Contains(this) && main.currentGameModeType == 1)
                    {
                        //Add it to the list
                        filteredSpawns.Add(allSpawns[id]);
                        //Set highest index
                        if (allSpawns[id].spawnGroupID > highestIndex) highestIndex = allSpawns[id].spawnGroupID;
                    }
                }

                main.internalSpawns = new List<InternalSpawns>();
                for (int i = 0; i < (highestIndex + 1); i++)
                {
                    main.internalSpawns.Add(null);
                }

                for (int i = 0; i < main.internalSpawns.Count; i++)
                {
                    int id = i;
                    main.internalSpawns[id] = new InternalSpawns();
                    main.internalSpawns[id].spawns = new List<Kit_PlayerSpawn>();
                    for (int o = 0; o < filteredSpawns.Count; o++)
                    {
                        int od = o;
                        if (filteredSpawns[od].spawnGroupID == id)
                        {
                            main.internalSpawns[id].spawns.Add(filteredSpawns[od]);
                        }
                    }
                }

                //Create Local Player Data
                object[] lpdInstData = new object[2];
                lpdInstData[0] = PhotonNetwork.LocalPlayer.ActorNumber;
                lpdInstData[1] = false;
                PhotonNetwork.Instantiate(localPlayerDataPrefab.name, Vector3.zero, Quaternion.identity, 0, lpdInstData);

                if (PhotonNetwork.IsMasterClient)
                {
                    waveManager = FindObjectOfType<Kit_PvE_ZombieWaveSurvival_WaveManager>();

                    if (!waveManager)
                    {
                        GameObject go = PhotonNetwork.InstantiateRoomObject(waveSurvivalRoundManagerPrefab.name, Vector3.zero, Quaternion.identity);
                        waveManager = go.GetComponent<Kit_PvE_ZombieWaveSurvival_WaveManager>();
                    }
                }
            }

            public override void GameModeUpdate(Kit_IngameMain main)
            {

            }

#if UNITY_EDITOR
            public override string[] GetSceneCheckerMessages()
            {
                throw new System.NotImplementedException();
            }

            public override MessageType[] GetSceneCheckerMessageTypes()
            {
                throw new System.NotImplementedException();
            }
#endif

            public override Transform GetSpawn(Kit_IngameMain main, Photon.Realtime.Player player)
            {
                //Define spawn tries
                int tries = 0;
                Transform spawnToReturn = null;
                //Try to get a spawn
                while (!spawnToReturn)
                {
                    //To prevent an unlimited loop, only do it ten times
                    if (tries >= 10)
                    {
                        break;
                    }

                    int layer = 0;

                    //Team deathmatch has no fixed spawns in this behaviour. Only use one layer
                    Transform spawnToTest = main.internalSpawns[layer].spawns[UnityEngine.Random.Range(0, main.internalSpawns[layer].spawns.Count)].transform;
                    //Test the spawn
                    if (spawnToTest)
                    {
                        if (spawnSystemToUse.CheckSpawnPosition(main, spawnToTest, player))
                        {
                            //Assign spawn
                            spawnToReturn = spawnToTest;
                            //Break the while loop
                            break;
                        }
                    }
                    tries++;
                }

                return spawnToReturn;
            }

            public override Transform GetSpawn(Kit_IngameMain main, Kit_Bot bot)
            {
                //Define spawn tries
                int tries = 0;
                Transform spawnToReturn = null;
                //Try to get a spawn
                while (!spawnToReturn)
                {
                    //To prevent an unlimited loop, only do it ten times
                    if (tries >= 10)
                    {
                        break;
                    }
                    int layer = 0;

                    //Team deathmatch has no fixed spawns in this behaviour. Only use one layer
                    Transform spawnToTest = main.internalSpawns[layer].spawns[UnityEngine.Random.Range(0, main.internalSpawns[layer].spawns.Count)].transform;
                    //Test the spawn
                    if (spawnToTest)
                    {
                        if (spawnSystemToUse.CheckSpawnPosition(main, spawnToTest, bot))
                        {
                            //Assign spawn
                            spawnToReturn = spawnToTest;
                            //Break the while loop
                            break;
                        }
                    }
                    tries++;
                }

                return spawnToReturn;
            }

            public override void PlayerDied(Kit_IngameMain main, bool botKiller, int killer, bool botKilled, int killed)
            {

            }

            public override void OnLocalPlayerDestroyed(Kit_PlayerBehaviour pb)
            {
                //Check if we can respawn, if so, no game over.
                if (!CanSpawn(pb.main, PhotonNetwork.LocalPlayer))
                {
                    //Check if everyone is dead
                    if (pb.main.allActivePlayers.Count <= 0 || (pb.main.allActivePlayers.Count == 1 && pb.main.allActivePlayers[0] == pb))
                    {
                        GameObject scoreMenu = GameObject.Find("GhostsMainKit_IngamePrefab/UI/Score Menu");
                        scoreMenu.SetActive(true);
                        pb.main.gameInformation.statistics.Save(pb.main);
                        Debug.Log("Everyone is dead. Game over.");
                        //Set to -1
                        pb.main.gameModeStage = -1;
                        pb.main.timer = gameOverScreenTime;
                    }
                }
            }

            public override void OnLocalPlayerSpawned(Kit_PlayerBehaviour pb)
            {
                //Add nav mesh obstacle to player to correctly interact with ai
                NavMeshObstacle nmo = pb.gameObject.AddComponent<NavMeshObstacle>();
                nmo.shape = NavMeshObstacleShape.Capsule;
                nmo.radius = 0.4f;
                nmo.height = 2f;
            }

            public override void OnPlayerSpawned(Kit_PlayerBehaviour pb)
            {
                //Add nav mesh obstacle to player to correctly interact with ai
                NavMeshObstacle nmo = pb.gameObject.AddComponent<NavMeshObstacle>();
                nmo.shape = NavMeshObstacleShape.Capsule;
                nmo.radius = 0.4f;
                nmo.height = 2f;
            }

            public override void OnPlayerDestroyed(Kit_PlayerBehaviour pb)
            {
                //Check if we can respawn, if so, no game over.
                if (!CanSpawn(pb.main, PhotonNetwork.LocalPlayer))
                {
                    //Check if everyone is dead
                    if (pb.main.allActivePlayers.Count <= 0 || (pb.main.allActivePlayers.Count == 1 && pb.main.allActivePlayers[0] == pb))
                    {
                        GameObject scoreMenu = GameObject.Find("GhostsMainKit_IngamePrefab/UI/Score Menu");
                        scoreMenu.SetActive(true);
                        Debug.Log("Everyone is dead. Game over.");
                        //Set to -1
                        pb.main.gameModeStage = -1;
                        pb.main.timer = gameOverScreenTime;
                    }
                }
            }

            public override void TimeRunOut(Kit_IngameMain main)
            {
                if (main.gameModeStage < 0)
                {
                    //smaller than 0 (-1)  means game over, end game aka go back to menu.
                    Kit_SceneSyncer.instance.LoadScene("MainMenu");
                }
                else
                {
                    if (!waveManager) waveManager = FindObjectOfType<Kit_PvE_ZombieWaveSurvival_WaveManager>();

                    waveManager.TimeRunOut();
                }
            }

            public void ZombieSpawned()
            {
                if (!waveManager) waveManager = FindObjectOfType<Kit_PvE_ZombieWaveSurvival_WaveManager>();

                waveManager.ZombieSpawned();
            }

            public void ZombieKilled()
            {
                if (!waveManager) waveManager = FindObjectOfType<Kit_PvE_ZombieWaveSurvival_WaveManager>();

                if (waveManager) waveManager.ZombieKilled();
            }

            /// <summary>
            /// Called when an area is unlocked
            /// </summary>
            public void AreaUnlocked(Kit_PvE_ZombieWaveSurvival_AreaPurchase area)
            {
                if (!waveManager) waveManager = FindObjectOfType<Kit_PvE_ZombieWaveSurvival_WaveManager>();

                waveManager.RefreshSpawnArray();
            }

            public override void OnPhotonEvent(Kit_IngameMain main, byte eventCode, object content, int senderId)
            {
                //Kill ep
                if (eventCode == 101)
                {
                    //Give kill money
                    localPlayerData.GainMoney(moneyPerKill);
                }
                //Drop sound
                else if (eventCode == 102)
                {
                    Hashtable table = content as Hashtable;
                    int dropId = (int)table[(byte)0];
                    int soundId = (int)table[(byte)1];

                    //Relay to hud
                    (main.currentGameModeHUD as Kit_PvE_ZombieWaveSurvival_HUD).DropSoundPlay(dropId, soundId);
                }
            }
        }
    }
}