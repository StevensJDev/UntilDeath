using Photon.Pun;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Kit_PvE_ZombieWaveSurvival_DropPickup : MonoBehaviourPun
        {
            [Tooltip("Game Mode reference")]
            /// <summary>
            /// Game Mode reference
            /// </summary>
            public Kit_PvE_ZombieWaveSurvival zws;
            /// <summary>
            /// Reference to main
            /// </summary>
            private Kit_IngameMain main;
            /// <summary>
            /// Currently active renderer
            /// </summary>
            private Kit_PvE_ZombieWaveSurvival_DropRenderer activeRenderer;
            /// <summary>
            /// Active drop id
            /// </summary>
            private int dropId;
            /// <summary>
            /// When to destroy this mf
            /// </summary>
            private float endTime;
            /// <summary>
            /// Was the event for the animator fired?
            /// </summary>
            private bool destroyEventWasFired;

            private void Start()
            {
                //Find main
                main = FindObjectOfType<Kit_IngameMain>();

                dropId = (int)photonView.InstantiationData[0];

                //Create renderer
                activeRenderer = Instantiate(zws.allDrops[dropId].dropRendererPrefab, transform, false).GetComponent< Kit_PvE_ZombieWaveSurvival_DropRenderer>();
                //Set time
                endTime = Time.time + zws.allDrops[dropId].dropLiveTime;
            }

            private void Update()
            {
                if (activeRenderer && activeRenderer.anim && activeRenderer.fireDestroyEventToAnimatorBeforeSeconds > 0)
                {
                    if (Time.time > endTime - activeRenderer.fireDestroyEventToAnimatorBeforeSeconds)
                    {
                        if (!destroyEventWasFired)
                        {
                            activeRenderer.anim.SetTrigger("BeforeDestroy");
                            destroyEventWasFired = true;
                        }
                    }
                }

                if (photonView.IsMine)
                {
                    //We check here because master client could switch at any time and we don't want drops to stay alive then
                    if (Time.time > endTime)
                    {
                        PhotonNetwork.Destroy(gameObject);
                    }
                }
            }

            private void OnTriggerEnter(Collider other)
            {
                Kit_PlayerBehaviour player = other.GetComponent<Kit_PlayerBehaviour>();

                if (player)
                {
                    if (player.photonView.IsMine)
                    {
                        //Hide renderer
                        activeRenderer.gameObject.SetActive(false);
                    }

                    //Check if we should trigger
                    if (photonView.IsMine)
                    {
                        //Call drop
                        zws.allDrops[dropId].DropPickedUp(main, dropId);

                        //Destroy
                        PhotonNetwork.Destroy(gameObject);
                    }
                }
            }
        }
    }
}