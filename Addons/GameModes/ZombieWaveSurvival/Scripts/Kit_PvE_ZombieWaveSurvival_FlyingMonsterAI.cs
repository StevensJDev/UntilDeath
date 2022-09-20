using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Kit_PvE_ZombieWaveSurvival_FlyingMonsterAI :  MonoBehaviourPunCallbacks, IPunObservable
        {
            public Kit_PlayerBehaviour playerToAttack;
            public GameObject waypoint1;
            public GameObject waypoint2;
            public bool harmfulToPlayer;
            public float distanceToPlayer = 10f;
            public float speed = .5f;
            /// <summary>
            /// How many hit points we have left
            /// </summary>
            private float hitPoints = 25f;
            private float originalHP = 25f;
            /// <summary>
            /// Reference to main
            /// </summary>
            private Kit_IngameMain main;
            /// <summary>
            /// Cached reference to the game mode
            /// </summary>
            public static Kit_PvE_ZombieWaveSurvival zws;
            
            void Start() {
                //Find main
                main = FindObjectOfType<Kit_IngameMain>();
                zws = main.currentPvEGameModeBehaviour as Kit_PvE_ZombieWaveSurvival;
            }

            void Update() {
                if (harmfulToPlayer) {
                    // Go towards player
                    if (playerToAttack) {
                        float distance = Vector3.Distance (playerToAttack.gameObject.transform.position, transform.position);
                        if (distance <= distanceToPlayer) {
                            Vector3 targetPosition = new Vector3(playerToAttack.gameObject.transform.position.x, playerToAttack.gameObject.transform.position.y + 1.75f, playerToAttack.gameObject.transform.position.z);
                            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                            transform.LookAt(playerToAttack.gameObject.transform);
                            transform.LookAt(playerToAttack.gameObject.transform.position + 90f * Vector3.left, -180f * Vector3.up);
                        }
                    } else {
                        if (main.allActivePlayers.Count > 0)
                        {
                            // TODO: replace with nearest random player
                            //Pick a random player to attack
                            playerToAttack = main.allActivePlayers[Random.Range(0, main.allActivePlayers.Count)];
                        }
                    }
                } else {
                    Vector3 targetPosition = new Vector3(waypoint1.transform.position.x, waypoint1.transform.position.y - 5f, waypoint1.transform.position.z);
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                }
            }

            public void OriginalHealth()
            {
                if (photonView.IsMine)
                {
                    hitPoints = originalHP;
                }
            }

            public void LocalDamage(float dmg, int gunID)
            {
                //Send RPC
                photonView.RPC("RpcDamage", RpcTarget.MasterClient, dmg, gunID);
            }

            [PunRPC]
            public void RpcDamage(float dmg, int gunID)
            {
                if (photonView.IsMine)
                {
                    //Apply damage
                    hitPoints -= dmg;

                    if (hitPoints <= 0)
                    {
                        //Send Event to give us kill xp
                        // PhotonNetwork.RaiseEvent(101, null, new RaiseEventOptions { TargetActors = new int[] { shotId }, Receivers = ReceiverGroup.All }, new ExitGames.Client.Photon.SendOptions { Reliability = true });

                        //Kill us
                        PhotonNetwork.Destroy(gameObject);
                    }
                }
            }

            void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
        }
    }
}
