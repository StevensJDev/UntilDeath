using Photon.Pun;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Addons/Zombie Wave Survival/Drops/Nuke")]
        public class Kit_PvE_ZombieWaveSurvival_DropNuke : Kit_PvE_ZombieWaveSurvival_DropBase
        {
            /// <summary>
            /// How long the nuke last
            /// </summary>
            public float liveTime = 30f;
            /// <summary>
            /// Can we extend an existing nuke?
            /// </summary>
            public bool canExtendExisting = false;
            /// <summary>
            /// Prefab of the manager
            /// </summary>
            public GameObject nukeManagerPrefab;

            public override void DropPickedUp(Kit_IngameMain main, int id)
            {
                if (Kit_PvE_ZombieWaveSurvival_DropNukeManager.instance)
                {
                    if (canExtendExisting)
                    {
                        //Sound
                        base.DropPickedUp(main, id);

                        Kit_PvE_ZombieWaveSurvival_DropNukeManager.instance.liveUntil = PhotonNetwork.Time + liveTime;
                    }
                }
                else
                {
                    //Sound
                    base.DropPickedUp(main, id);

                    object[] instData = new object[1];
                    instData[0] = liveTime;
                    //Create a manager
                    PhotonNetwork.Instantiate(nukeManagerPrefab.name, Vector3.zero, Quaternion.identity, 0, instData);
                }
            }
        }
    }
}