using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Addons/Zombie Wave Survival/Character/New")]
        public class Kit_PvE_ZombieWaveSurvival_Character : ScriptableObject
        {
            /// <summary>
            /// Character name
            /// </summary>
            public string characterName = "New Character";

            /// <summary>
            /// If set to true, this character is not limited to one player
            /// </summary>
            public bool characterCanBeSelectedByEveryone;

            [Tooltip("Loadout we spawn with")]
            /// <summary>
            /// Loadout we spawn with
            /// </summary>
            public Loadout loadout;
            [Tooltip("Player model we spawn with")]
            /// <summary>
            /// Player model we spawn with
            /// </summary>
            public PlayerModelConfig playerModel;
        }
    }
}