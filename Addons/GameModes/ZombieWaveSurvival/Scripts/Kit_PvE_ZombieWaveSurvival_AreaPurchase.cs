using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Kit_PvE_ZombieWaveSurvival_AreaPurchase : Kit_InteractableObject, IPunObservable
        {
            /// <summary>
            /// Rerefence to zws
            /// </summary>
            public Kit_PvE_ZombieWaveSurvival zws;
            /// <summary>
            /// reference to in game main
            /// </summary>
            private Kit_IngameMain main;

            [Tooltip("Price of the area")]
            [Header("Settings")]
            /// <summary>
            /// Price of the area
            /// </summary>
            public int areaPrice;
            [Tooltip("Animator to animate this area. The *unlocked* bool is set to it.")]
            /// <summary>
            /// Animator to animate this area. The *unlocked* bool is set to it.
            /// </summary>
            public Animator anim;
            [Tooltip("Fired when this area is unlocked")]
            /// <summary>
            /// Fired when this area is unlocked
            /// </summary>
            public UnityEvent onUnlocked;

            #region Runtime
            [HideInInspector]
            /// <summary>
            /// Was this area unlocked?
            /// </summary>
            public bool isUnlocked;
            /// <summary>
            /// To check for changes
            /// </summary>
            private bool wasUnlocked;
            #endregion

            #region Unity Calls
            private void Start()
            {
                //Find main reference
                main = FindObjectOfType<Kit_IngameMain>();
            }

            void Update()
            {
                //Just set bool to animator
                anim.SetBool("unlocked", isUnlocked);

                if (isUnlocked != wasUnlocked)
                {
                    if (isUnlocked)
                    {
                        onUnlocked.Invoke();
                    }

                    wasUnlocked = false;
                }
            }
            #endregion

            public override bool CanInteract(Kit_PlayerBehaviour who)
            {
                interactionText = "Press [" + PlayerPrefs.GetString("Interact", "F") + "] to unlock area [$" + areaPrice + "]";

                if (!isUnlocked && zws.localPlayerData.money >= areaPrice)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public override void Interact(Kit_PlayerBehaviour who)
            {
                if (!isUnlocked && zws.localPlayerData.money >= areaPrice)
                {
                    //Spend money
                    zws.localPlayerData.SpendMoney(areaPrice);
                    //Send unlock RPC
                    photonView.RPC("Unlock", RpcTarget.MasterClient);
                }
            }

            #region Photon
            void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(isUnlocked);
                }
                else
                {
                    isUnlocked = (bool)stream.ReceiveNext();
                }
            }

            [PunRPC]
            public void Unlock()
            {
                if (photonView.IsMine)
                {
                    //Just unlock the area
                    isUnlocked = true;

                    //Update array
                    zws.AreaUnlocked(this);
                }
            }
            #endregion
        }
    }
}