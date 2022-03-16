using Photon.Pun;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Addons/Zombie Wave Survival/Drops/Double Points")]
        public class Kit_PvE_ZombieWaveSurvival_DropDoublePoints : Kit_PvE_ZombieWaveSurvival_DropBase
        {
            /// <summary>
            /// How long the double points last
            /// </summary>
            public float liveTime = 30f;
            /// <summary>
            /// Can we extend an existing double point?
            /// </summary>
            public bool canExtendExisting = true;
            /// <summary>
            /// Prefab of the manager
            /// </summary>
            public GameObject doublePointManagerPrefab;

            public GameObject doublePointsUI;

            public override void DropPickedUp(Kit_IngameMain main, int id)
            {
                if (Kit_PvE_ZombieWaveSurvival_DropDoublePointsManager.instance)
                {
                    if (canExtendExisting)
                    {
                        //Sound
                        base.DropPickedUp(main, id);

                        // Kit_PvE_ZombieWaveSurvival_DropDoublePointsManager.instance.doublePointsUI = doublePointsUI;
                        Kit_PvE_ZombieWaveSurvival_DropDoublePointsManager.instance.liveUntil = PhotonNetwork.Time + liveTime;
                    }
                }
                else
                {
                    //Sound
                    base.DropPickedUp(main, id);

                    object[] instData = new object[1];
                    instData[0] = liveTime;
                    //Create a manager
                    PhotonNetwork.Instantiate(doublePointManagerPrefab.name, Vector3.zero, Quaternion.identity, 0, instData);
                }
            }
        }
    }
}