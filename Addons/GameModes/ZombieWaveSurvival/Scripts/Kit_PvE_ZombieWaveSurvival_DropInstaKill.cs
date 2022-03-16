using Photon.Pun;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Addons/Zombie Wave Survival/Drops/Insta Kill")]
        public class Kit_PvE_ZombieWaveSurvival_DropInstaKill : Kit_PvE_ZombieWaveSurvival_DropBase
        {
            /// <summary>
            /// How long the insta-kill last
            /// </summary>
            public float liveTime = 30f;
            /// <summary>
            /// Can we extend an existing insta kill?
            /// </summary>
            public bool canExtendExisting = true;
            /// <summary>
            /// Prefab of the manager
            /// </summary>
            public GameObject instaKillManagerPrefab;
            public GameObject instaKillUI;

            public override void DropPickedUp(Kit_IngameMain main, int id)
            {
                if (Kit_PvE_ZombieWaveSurvival_DropInstaKillManager.instance)
                {
                    if (canExtendExisting)
                    {
                        //Sound
                        base.DropPickedUp(main, id);

                        Kit_PvE_ZombieWaveSurvival_DropInstaKillManager.instance.liveUntil = PhotonNetwork.Time + liveTime;
                    }
                }
                else
                {
                    //Sound
                    base.DropPickedUp(main, id);

                    object[] instData = new object[1];
                    instData[0] = liveTime;
                    //Create a manager
                    PhotonNetwork.Instantiate(instaKillManagerPrefab.name, Vector3.zero, Quaternion.identity, 0, instData);
                }
            }
        }
    }
}