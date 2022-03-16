using Photon.Pun;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Addons/Zombie Wave Survival/Drops/Infinite Ammo")]
        public class Kit_PvE_ZombieWaveSurvival_DropInfiniteAmmo : Kit_PvE_ZombieWaveSurvival_DropBase
        {
            /// <summary>
            /// How long the infinite ammo last
            /// </summary>
            public float liveTime = 30f;
            /// <summary>
            /// Can we extend an existing infinite ammo?
            /// </summary>
            public bool canExtendExisting = true;
            /// <summary>
            /// Prefab of the manager
            /// </summary>
            public GameObject infiniteAmmoManagerPrefab;

            public override void DropPickedUp(Kit_IngameMain main, int id)
            {
                if (Kit_PvE_ZombieWaveSurvival_DropInfiniteAmmoManager.instance)
                {
                    if (canExtendExisting)
                    {
                        //Sound
                        base.DropPickedUp(main, id);

                        Kit_PvE_ZombieWaveSurvival_DropInfiniteAmmoManager.instance.liveUntil = PhotonNetwork.Time + liveTime;
                    }
                }
                else
                {
                    //Sound
                    base.DropPickedUp(main, id);

                    object[] instData = new object[1];
                    instData[0] = liveTime;
                    //Create a manager
                    PhotonNetwork.Instantiate(infiniteAmmoManagerPrefab.name, Vector3.zero, Quaternion.identity, 0, instData);
                }
            }
        }
    }
}