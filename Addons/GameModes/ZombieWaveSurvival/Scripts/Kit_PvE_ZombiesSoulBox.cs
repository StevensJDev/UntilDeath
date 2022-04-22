using Photon.Pun;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Kit_PvE_ZombiesSoulBox : MonoBehaviour
        {
            [Header("Settings")]

            public int numberOfSouls;
            private int ghostsKilled;
            /// <summary>
            /// Is the box filled with enough souls?
            /// </summary>
            public bool boxFilled;

            void Update() {
                if (ghostsKilled >= numberOfSouls) {
                    boxFilled = true;
                }
            }

            public void addSoul() {
                // TODO, will need to have location of ghost and then have the soul fly into the location of the soul box.
                ghostsKilled++;
            }
        }
    }
}