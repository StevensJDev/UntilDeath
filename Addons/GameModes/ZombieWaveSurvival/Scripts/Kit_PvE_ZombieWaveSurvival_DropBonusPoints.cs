using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Addons/Zombie Wave Survival/Drops/Bonus Points")]
        public class Kit_PvE_ZombieWaveSurvival_DropBonusPoints : Kit_PvE_ZombieWaveSurvival_DropBase
        {
            private Kit_PvE_ZombieWaveSurvival zws;
            public override void DropPickedUp(Kit_IngameMain main, int id)
            {
                //Sound
                base.DropPickedUp(main, id);

                zws = main.currentPvEGameModeBehaviour as Kit_PvE_ZombieWaveSurvival;
                //give money
                zws.localPlayerData.GainMoney(500);
            }
        }
    }
}