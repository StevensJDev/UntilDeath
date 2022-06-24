using Photon.Pun;
using System.Linq;
using UnityEngine;
using BigBlit.ActivePack;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Power : Kit_InteractableObject, IPunObservable
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
            [Header("Settings")]

            public bool powerIsOn = false;
            public Lever lever;

            private void Start()
            {
                //Find main reference
                main = FindObjectOfType<Kit_IngameMain>();
            }

            private void Update() {
                if (powerIsOn) {
                    lever.ToggleOn();
                }
            }

            public override bool CanInteract(Kit_PlayerBehaviour who)
            {
                if(powerIsOn){
                    interactionText = "Power is already on.";
                    return false;
                } else {
                    interactionText = "Press [" + PlayerPrefs.GetString("Interact", "F") + "] to turn on power.";
                    return true;
                }
            }

            #region Photon
            void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(powerIsOn);
                }
                else
                {
                    powerIsOn = (bool)stream.ReceiveNext();
                }
            }

            [PunRPC]
            public override void Interact(Kit_PlayerBehaviour who)
            {
                if (photonView.IsMine) {
                    if (!powerIsOn) {
                        powerIsOn = true;
                    }
                }
            }
            #endregion
        }
    }
}