using Photon.Pun;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Kit_PvE_ZombieWaveSurvival_DropInfiniteAmmoManager : MonoBehaviourPun, IPunObservable
        {
            /// <summary>
            /// There should only be one of these at all times
            /// </summary>
            public static Kit_PvE_ZombieWaveSurvival_DropInfiniteAmmoManager instance;
            [HideInInspector]
            /// <summary>
            /// Until which <see cref="PhotonNetwork.Time"/> should we live?
            /// </summary>
            public double liveUntil;
            private bool paused;

            GameObject infiniteAmmoUI;
            Animator anim;
            bool isPlaying = false;

            private void Start()
            {
                isPlaying = false;
                instance = this;

                infiniteAmmoUI = GameObject.Find("MarsFPSKit_IngamePrefab/UI/HUD/Root/Root (Can be hidden)/TempDropsUI/InfiniteAmmo");
                infiniteAmmoUI.SetActive(true);
                anim = infiniteAmmoUI.GetComponent<Animator>();

                //Set initial time
                liveUntil = PhotonNetwork.Time + (float)photonView.InstantiationData[0];
            }

            private void Update()
            {
                if (photonView.IsMine)
                {
                    if ((liveUntil-PhotonNetwork.Time) <= 3.0f && !isPlaying) {
                        anim.SetTrigger("dropIsClosing");
                        isPlaying = true;
                    }

                    if (Time.timeScale == 0f && !paused) {
                        liveUntil = liveUntil - PhotonNetwork.Time;
                        paused = true;
                    } else if (Time.timeScale == 1.0f && paused) {
                        paused = false;
                        liveUntil = PhotonNetwork.Time + liveUntil;
                    } else if (Time.timeScale == 1.0f && PhotonNetwork.Time >= liveUntil) {
                        infiniteAmmoUI.SetActive(false);
                        anim.ResetTrigger("dropIsClosing");
                        PhotonNetwork.Destroy(gameObject);
                    }
                }
            }

            void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(liveUntil);
                }
                else
                {
                    liveUntil = (double)stream.ReceiveNext();
                }
            }
        }
    }
}