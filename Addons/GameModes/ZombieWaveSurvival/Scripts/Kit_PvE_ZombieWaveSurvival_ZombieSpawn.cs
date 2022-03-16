using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Kit_PvE_ZombieWaveSurvival_ZombieSpawn : MonoBehaviour
        {
            [Tooltip("If assigned, this spawn gets only used after the given area has been unlocked")]
            /// <summary>
            /// If assigned, this spawn gets only used after the given area has been unlocked
            /// </summary>
            public Kit_PvE_ZombieWaveSurvival_AreaPurchase lockedToArea;

            private void OnDrawGizmos()
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(transform.position, Vector3.one);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);

                if (lockedToArea)
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawLine(transform.position, lockedToArea.transform.position);
                }
            }
        }
    }
}