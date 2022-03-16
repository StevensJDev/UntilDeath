using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Kit_PvE_ZombieWaveSurvival_ZombieAnimationTrigger : MonoBehaviour
        {
            [Tooltip("This trigger is set on the zombies' animator")]
            /// <summary>
            /// This trigger is set on the zombies' animator
            /// </summary>
            public string animationToTrigger = "JumpOverBarricade";
            [Tooltip("How long is the animation? This is used to block the nav mesh agent's movement during the animation")]
            /// <summary>
            /// How long is the animation? This is used to block the nav mesh agent's movement during the animation
            /// </summary>
            public float animationLength = 1.2f;
            [Tooltip("In which direction should the zombie look while the animation is being played?")]
            /// <summary>
            /// In which direction should the zombie look while the animation is being played?
            /// </summary>
            public Vector3 lookDirection = Vector3.forward;

            private void OnDrawGizmos()
            {
                Gizmos.DrawRay(transform.position, transform.TransformDirection(lookDirection));
            }
        }
    }
}