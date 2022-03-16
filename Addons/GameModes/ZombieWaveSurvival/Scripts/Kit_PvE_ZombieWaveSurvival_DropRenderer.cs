using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Kit_PvE_ZombieWaveSurvival_DropRenderer : MonoBehaviour
        {
            [Tooltip("Animator to fire the destroy event to")]
            /// <summary>
            /// Animator to fire the destroy event to
            /// </summary>
            public Animator anim;
            [Tooltip("If > 0 and anim is assigned, the BeforeDestroy event will be set to the animator X seconds before the drop expires")]
            /// <summary>
            /// If > 0 and anim is assigned, the "BeforeDestroy" event will be set to the animator X seconds before the drop expires
            /// </summary>
            public float fireDestroyEventToAnimatorBeforeSeconds = 1f;
        }
    }
}