using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Addons/Zombie Wave Survival/Drops/Full Ammo")]
        public class Kit_PvE_ZombieWaveSurvival_DropFullAmmo : Kit_PvE_ZombieWaveSurvival_DropBase
        {
            GameObject fullAmmoUI;

            public override void DropPickedUp(Kit_IngameMain main, int id)
            {
                fullAmmoUI = GameObject.Find("MarsFPSKit_IngamePrefab/UI/HUD/Root/Root (Can be hidden)/Max Ammo UI");
                fullAmmoUI.SetActive(true); // TODO: should really just play an animation
                //Sound
                base.DropPickedUp(main, id);

                for (int i = 0; i < main.allActivePlayers.Count; i++)
                {
                    main.allActivePlayers[i].weaponManager.RestockAmmo(main.allActivePlayers[i], true);
                }

                // fullAmmoUI.SetActive(false);
            }
        }
    }
}