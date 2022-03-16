using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        [System.Serializable]
        public class ZombieAttackSettings
        {
            [Tooltip("How long this attack takes")]
            /// <summary>
            /// How long this attack takes
            /// </summary>
            public float time = 0.3f;
            [Tooltip("Name of the attack trigger in the animator")]
            /// <summary>
            /// Name of the attack trigger in the animator
            /// </summary>
            public string triggerName = "Attack";
            [Tooltip("If set to true, the zombies' movement will be disabled during the attack duration")]
            /// <summary>
            /// If set to true, the zombies' movement will be disabled during the attack duration
            /// </summary>
            public bool freezesZombieForAttackDuration;
        }

        [System.Serializable]
        public class ZombieFootstep
        {
            [Tooltip("Sounds for this footstep")]
            /// <summary>
            /// Sounds for this footstep
            /// </summary>
            public AudioClip[] sounds;
        }

        [System.Serializable]
        public class StringZombieFootstepDictionary : SerializableDictionary<string, ZombieFootstep> { }

        [CreateAssetMenu(menuName = "MarsFPSKit/Addons/Zombie Wave Survival/AI/Zombie AI Settings")]
        public class Kit_PvE_ZombieWaveSurvival_ZombieAISettings : ScriptableObject
        {
            [Tooltip("How close the zombie needs to be to the player in order to attack him?")]
            /// <summary>
            /// How close the zombie needs to be to the player in order to attack him?
            /// </summary>
            public float attackDistance = 2f;

            [Tooltip("Settings for the attacks")]
            /// <summary>
            /// Settings for the attacks
            /// </summary>
            public ZombieAttackSettings[] attackSettings;
            [Tooltip("How close do we need to be in order to lock rotation to attacked item?")]
            /// <summary>
            /// How close do we need to be in order to lock rotation to attacked item?
            /// </summary>
            public float attackRotateToDistance = 5f;
            [Tooltip("Smooth value for rotating towards the player we're attacking")]
            /// <summary>
            /// Smooth value for rotating towards the player we're attacking
            /// </summary>
            public float attackRotateSpeed = 10f;

            [Tooltip("Health of the zombie at start")]
            /// <summary>
            /// Health of the zombie at start
            /// </summary>
            public float health = 100f;
            [Tooltip("How big is the zombies' nav mesh agent radius?")]
            /// <summary>
            /// How big is the zombies' nav mesh agent radius?
            /// </summary>
            public float navMeshRadius = 0.5f;
            [Tooltip("How fast does the zombie move via nav mesh agent if root motion is disabled?")]
            /// <summary>
            /// How fast does the zombie move via nav mesh agent if root motion is disabled?
            /// </summary>
            public float navMeshSpeed = 4f;

            [Tooltip("Do we set desired or actual velocity? (root motion or not?)")]
            /// <summary>
            /// Do we set desired or actual velocity? (root motion or not?)
            /// </summary>
            [Header("Animation")]
            public bool setDesiredVelocity;
            [Tooltip("Smooth for setting animation value")]
            /// <summary>
            /// Smooth for setting animation value
            /// </summary>
            public float animSetSmooth = 5f;

            [Tooltip("Sounds that can play upon spawn")]
            /// <summary>
            /// Sounds that can play upon spawn
            /// </summary>
            [Header("Sounds")]
            public AudioClip[] soundSpawn;
            [Tooltip(" Chance in % that a spawn plays upon spawn")]
            [Range(0f, 100f)]
            /// <summary>
            /// Chance in % that a spawn plays upon spawn
            /// </summary>
            public float soundSpawnChance = 50f;
            [Tooltip("Sounds that can play upon damage")]
            /// <summary>
            /// Sounds that can play upon damage
            /// </summary>
            public AudioClip[] soundDamage;
            [Tooltip("Chance in % that a spawn plays upon damage")]
            [Range(0f, 100f)]
            /// <summary>
            /// Chance in % that a spawn plays upon damage
            /// </summary>
            public float soundDamageChance = 50f;
            [Tooltip("Sounds that can play upon death")]
            /// <summary>
            /// Sounds that can play upon death
            /// </summary>
            public AudioClip[] soundDeath;
            [Tooltip("Chance in % that a spawn plays upon death")]
            [Range(0f, 100f)]
            /// <summary>
            /// Chance in % that a spawn plays upon death
            /// </summary>
            public float soundDeathChance = 50f;
            [Tooltip("Sounds that can play randomly")]
            /// <summary>
            /// Sounds that can play randomly
            /// </summary>
            public AudioClip[] soundRandom;
            [Tooltip("Chance in % that a spawn plays upon the end of a random interval")]
            [Range(0f, 100f)]
            /// <summary>
            /// Chance in % that a spawn plays upon the end of a random interval
            /// </summary>
            public float soundRandomChance = 50f;
            [Tooltip("Interval in seconds between random sounds being played")]
            /// <summary>
            /// Interval in seconds between random sounds being played
            /// </summary>
            public Vector2 soundRandomInterval = new Vector2(5, 10);
            [Tooltip("Sounds that can play when the zombie attacks")]
            /// <summary>
            /// Sounds that can play when the zombie attacks
            /// </summary>
            public AudioClip[] soundAttack;
            [Tooltip("Chance in % that a spawn plays when the zombie attacks")]
            [Range(0f, 100f)]
            /// <summary>
            /// Chance in % that a spawn plays when the zombie attacks
            /// </summary>
            public float soundAttackChance = 50f;
            [Tooltip("The footstep array. Maps a tag string to footstep sounds. Concrete should be in here.")]
            /// <summary>
            /// The footstep array. Maps a tag string to footstep sounds. Concrete should be in here.
            /// </summary>
            public StringZombieFootstepDictionary footstepSounds;
            [Tooltip("The volume for footsteps")]
            /// <summary>
            /// The volume for footsteps
            /// </summary>
            public float footStepVolume = 0.6f;
            [Tooltip("Layers that the footsteps can hit to check for surface")]
            /// <summary>
            /// Layers that the footsteps can hit to check for surface
            /// </summary>
            public LayerMask footstepHitLayers = ~0;

            [Tooltip("If this is set to true, the rigidbodies will no longer be stripped from active zombies")]
            /// <summary>
            /// If this is set to true, the rigidbodies will no longer be stripped from active zombies
            /// </summary>
            [Header("Debug")]
            public bool debugDisablePhysicsOptimization;
        }
    }
}