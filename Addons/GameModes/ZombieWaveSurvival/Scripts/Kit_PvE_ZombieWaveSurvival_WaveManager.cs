using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Kit_PvE_ZombieWaveSurvival_WaveManager : MonoBehaviourPunCallbacks, IPunObservable
        {
            /// <summary>
            /// Wave survival settings
            /// </summary>
            [Header("Settings")]
            public Kit_PvE_ZombieWaveSurvival zws;

            /// <summary>
            /// Zombies that we still need to spawn
            /// </summary>
            [Header("Runtime")]
            public int zombiesLeftToSpawn;
            /// <summary>
            /// Amount of zombies that are currently alive
            /// </summary>
            public int zombiesAlive;
            /// <summary>
            /// Next spawn check (Photon Network Time)
            /// </summary>
            public double nextSpawnCheck;
            /// <summary>
            /// Current game stage
            /// 0 = Waiting round 1
            /// 1 = Round 1
            /// 2 = Waiting round 2
            /// 3 = Round 2
            /// 4 = Waiting round 3
            /// 5 = Round 3
            /// </summary>
            public Kit_IngameMain main;
            /// <summary>
            /// Current round
            /// </summary>
            public int currentRound
            {
                get
                {
                    return Mathf.CeilToInt(main.gameModeStage / 2) + 1;
                }
            }
            /// <summary>
            /// To auto spawn in a new round
            /// </summary>
            private int lastGameModeStage;
            /// <summary>
            /// Wave ready zombie prefabs
            /// </summary>
            private ZombieSpawnPrefab[] zombiePrefabs;
            /// <summary>
            /// Array of all spawns in the scene
            /// </summary>
            private Kit_PvE_ZombieWaveSurvival_ZombieSpawn[] allSpawns;
            /// <summary>
            /// Spawns that we can use
            /// </summary>
            private Kit_PvE_ZombieWaveSurvival_ZombieSpawn[] useableSpawns;

            #region Unity Calls
            void Start()
            {
                main = FindObjectOfType<Kit_IngameMain>();
                //Find spawns
                allSpawns = FindObjectsOfType<Kit_PvE_ZombieWaveSurvival_ZombieSpawn>();
                //Update list
                RefreshSpawnArray();

                if (photonView.IsMine)
                {
                    main.gameModeStage = 0;
                    main.timer = zws.waitTimeBeforeFirstRound;
                }
                //Get prefabs
                zombiePrefabs = zws.zombiePrefabs.Where(x => currentRound >= x.spawnAfterWave && (x.spawnUntilWave <= 0 || x.spawnUntilWave > currentRound)).ToArray();
            }

            void Update()
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (zombiesLeftToSpawn > 0)
                    {
                        if (PhotonNetwork.Time >= nextSpawnCheck)
                        {
                            nextSpawnCheck = PhotonNetwork.Time + zws.timeBetweenZombieSpawns;

                            int toSpawn = Mathf.Min(zombiesLeftToSpawn, zws.maxAmountOfZombieSpawnsAtOnce, zws.maxAmountOfZombiesAliveAtOnce - zombiesAlive);

                            //Spawn zombies
                            for (int i = 0; i < toSpawn; i++)
                            {
                                if (zombiePrefabs.Length > 0)
                                {
                                    int spawn = Random.Range(0, useableSpawns.Length);
                                    int zombie = Random.Range(0, zombiePrefabs.Length);
                                    int skin = Random.Range(0, zombiePrefabs[zombie].skins.Length);
                                    int globalZombie = System.Array.IndexOf(zws.zombiePrefabs, zombiePrefabs[Random.Range(0, zombiePrefabs.Length)]);

                                    object[] instData = new object[2];

                                    instData[0] = globalZombie;
                                    instData[1] = skin;

                                    PhotonNetwork.InstantiateRoomObject(zombiePrefabs[zombie].prefab.name, useableSpawns[spawn].transform.position, useableSpawns[spawn].transform.rotation, 0, instData);
                                }
                                else
                                {
                                    Debug.LogError("No Zombie Prefabs found");
                                }
                            }
                        }
                    }
                }

                if (main.gameModeStage != lastGameModeStage)
                {
                    if (!main.myPlayer)
                    {
                        main.Spawn();
                    }

                    lastGameModeStage = main.gameModeStage;
                }
            }
            #endregion

            #region RPCs
            [PunRPC]
            public void RpcRoundStart(int round)
            {
                //Tell HUD
                (main.currentGameModeHUD as Kit_PvE_ZombieWaveSurvival_HUD).RoundBegin(round);
            }

            [PunRPC]
            public void RpcRoundEnd(int round)
            {
                //Tell HUD
                (main.currentGameModeHUD as Kit_PvE_ZombieWaveSurvival_HUD).RoundEnd(round);
            }
            #endregion

            /// <summary>
            /// Updates the useable spawn list (checks for unlocked areas)
            /// </summary>
            public void RefreshSpawnArray()
            {
                //Check if area is not assigned or if it was unlocked
                useableSpawns = allSpawns.Where(x => (!x.lockedToArea || x.lockedToArea.isUnlocked)).ToArray();
            }

            public void ZombieSpawned()
            {
                zombiesAlive++;
                zombiesLeftToSpawn--;
            }

            public void ZombieKilled()
            {
                zombiesAlive--;

                //Call statistics
                main.gameInformation.statistics.OnKill(main, 0);

                //Check if all are dead
                if (zombiesAlive <= 0 && zombiesLeftToSpawn <= 0)
                {
                    //Send rpc
                    photonView.RPC("RpcRoundEnd", RpcTarget.All, currentRound);

                    //Wait
                    main.gameModeStage++;
                    main.timer = zws.waitTimeBetweenRounds;

                    //Reset spawners
                    zombiesAlive = 0;
                    zombiesLeftToSpawn = 0;
                }
            }

            public void TimeRunOut()
            {
                //Wait to round
                if (main.gameModeStage % 2 == 0)
                {
                    //Proceed game stage
                    main.gameModeStage++;
                    main.timer = 0f;

                    //Get prefabs
                    zombiePrefabs = zws.zombiePrefabs.Where(x => currentRound >= x.spawnAfterWave && (x.spawnUntilWave <= 0 || x.spawnUntilWave > currentRound)).ToArray();

                    //Set amount of zombies
                    zombiesLeftToSpawn = GetAmountOfZombies(currentRound, PhotonNetwork.PlayerList.Length);

                    //Send rpc
                    photonView.RPC("RpcRoundStart", RpcTarget.All, currentRound);
                }
            }

            void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(zombiesLeftToSpawn);
                    stream.SendNext(zombiesAlive);
                }
                else
                {
                    zombiesLeftToSpawn = (int)stream.ReceiveNext();
                    zombiesAlive = (int)stream.ReceiveNext();
                }
            }

            public override void OnMasterClientSwitched(Player newMasterClient)
            {
                if (newMasterClient.IsLocal)
                {
                    //Get prefabs
                    zombiePrefabs = zws.zombiePrefabs.Where(x => x.spawnAfterWave >= currentRound && (x.spawnUntilWave <= 0 || x.spawnUntilWave > currentRound)).ToArray();
                    //Update spawns
                    RefreshSpawnArray();
                }
            }

            public static int GetAmountOfZombies(int round, int players)
            {
                return (players * 5) * round;
            }
        }
    }
}