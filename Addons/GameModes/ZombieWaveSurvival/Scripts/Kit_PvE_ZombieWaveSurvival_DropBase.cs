using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public abstract class Kit_PvE_ZombieWaveSurvival_DropBase : ScriptableObject
        {
            [Tooltip("Chance (in percent) that this drop occurs when a zombie dies")]
            /// <summary>
            /// Chance (in percent) that this drop occurs when a zombie dies
            /// </summary>
            public int dropChance = 25;
            [Tooltip("How long this drop lives in the world")]
            /// <summary>
            /// How long this drop lives in the world
            /// </summary>
            public float dropLiveTime = 30f;
            [Tooltip("Offset of the drop in world position from the zombie that spawned this")]
            /// <summary>
            /// Offset of the drop in world position from the zombie that spawned this
            /// </summary>
            public Vector3 dropOffset = Vector3.up;
            [Tooltip("Prefab of the drop that is displayed")]
            /// <summary>
            /// Prefab of the drop that is displayed
            /// </summary>
            public GameObject dropRendererPrefab;
            [Tooltip("Sound that is played when this drop is picked up")]
            /// <summary>
            /// Sound that is played when this drop is picked up
            /// </summary>
            public AudioClip[] dropSound;

            /// <summary>
            /// Called when a drop is picked up
            /// </summary>
            /// <param name="main"></param>
            public virtual void DropPickedUp(Kit_IngameMain main, int id)
            {
                if (dropSound.Length > 0)
                {
                    //Create table for sound event
                    Hashtable table = new Hashtable(2);
                    table[(byte)0] = id;
                    table[(byte)1] = Random.Range(0, dropSound.Length);
                    //Send event
                    PhotonNetwork.RaiseEvent(102, table, new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.All }, new SendOptions { });
                }
            }

            /// <summary>
            /// Create this drop
            /// </summary>
            /// <param name="main"></param>
            public virtual void CreateDrop(Kit_IngameMain main, Kit_PvE_ZombieWaveSurvival zws, Vector3 positionOfZombieDeath, Quaternion rotationOfZombieDeath, int dropId)
            {
                object[] instData = new object[1];
                //To tell the drop pickup script which drop it should display / trigger
                instData[0] = dropId;
                //Rest is handled by the script
                PhotonNetwork.InstantiateRoomObject(zws.dropPickupPrefab.name, positionOfZombieDeath + dropOffset, rotationOfZombieDeath, 0, instData);
            }
        }
    }
}