using Photon.Pun;
using UnityEngine;

namespace MarsFPSKit
{
    public abstract class Kit_VitalsBase : ScriptableObject
    {
        /// <summary>
        /// Called to setup this system
        /// </summary>
        /// <param name=""></param>
        public abstract void Setup(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Apply fall damage!
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="dmg"></param>
        public abstract void ApplyFallDamage(Kit_PlayerBehaviour pb, float dmg);

        /// <summary>
        /// Apply damage from environment!
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="dmg"></param>
        public abstract void ApplyEnvironmentalDamage(Kit_PlayerBehaviour pb, float dmg, int deathSoundCategory);

        /// <summary>
        /// Commit suicide
        /// </summary>
        /// <param name="pb"></param>
        public abstract void Suicide(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called to apply damage using this system
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="runtimeData"></param>
        public abstract void ApplyDamage(Kit_PlayerBehaviour pb, float dmg, bool botShot, int idWhoShot, int gunID, Vector3 shotFrom);

        /// <summary>
        /// Called to apply damage using this system
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="runtimeData"></param>
        public abstract void ApplyDamage(Kit_PlayerBehaviour pb, float dmg, bool botShot, int idWhoShot, string deathCause, Vector3 shotFrom);

        /// <summary>
        /// Called to apply healing using this system!
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="heal"></param>
        public abstract void ApplyHeal(Kit_PlayerBehaviour pb, float heal);

        /// <summary>
        /// Update callback
        /// </summary>
        /// <param name="pb"></param>
        public abstract void CustomUpdate(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Update player health
        /// </summary>
        /// <param name = "pb"></param>
        public abstract void ChangeHealth(Kit_PlayerBehaviour pb, int healthNum);

        public abstract void GoDown(Kit_PlayerBehaviour pb, string cause, bool botshot = false, int killer = 0, bool hqr = false);

        public abstract void ReviveSP(Kit_PlayerBehaviour pb);

        public abstract bool canRevive();

        /// <summary>
        /// Photonview Serialize callback
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public abstract void OnPhotonSerializeView(Kit_PlayerBehaviour pb, PhotonStream stream, PhotonMessageInfo info);
    }
}
