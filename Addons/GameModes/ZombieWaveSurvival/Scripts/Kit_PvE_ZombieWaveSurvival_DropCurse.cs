using Photon.Pun;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Addons/Zombie Wave Survival/Drops/Curse")]
        public class Kit_PvE_ZombieWaveSurvival_DropCurse : Kit_PvE_ZombieWaveSurvival_DropBase
        {
            /// <summary>
            /// How long the curse last
            /// </summary>
            public float liveTime = 30f;
            /// <summary>
            /// Can we extend an existing curse?
            /// </summary>
            public bool canExtendExisting = false;
            /// <summary>
            /// Prefab of the manager
            /// </summary>
            public GameObject curseManagerPrefab;

            public override void DropPickedUp(Kit_IngameMain main, int id)
            {
                if (Kit_PvE_ZombieWaveSurvival_DropCurseManager.instance)
                {
                    if (canExtendExisting)
                    {
                        //Sound
                        base.DropPickedUp(main, id);

                        Kit_PvE_ZombieWaveSurvival_DropCurseManager.instance.liveUntil = PhotonNetwork.Time + liveTime;
                    }
                }
                else
                {
                    //Sound
                    base.DropPickedUp(main, id);

                    object[] instData = new object[1];
                    instData[0] = liveTime;
                    //Create a manager
                    PhotonNetwork.Instantiate(curseManagerPrefab.name, Vector3.zero, Quaternion.identity, 0, instData);
                }
            }
        }
    }
}