using Photon.Pun;
using UnityEngine;

namespace MarsFPSKit
{
    public abstract class PerksManagerBase : ScriptableObject
    {
        /// <summary>
        /// Called to setup this system
        /// </summary>
        /// <param name=""></param>
        public abstract void SetupManager(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called to add juggernog using this system!
        /// </summary>
        public abstract void AddJuggernog(Kit_PlayerBehaviour pb, int healthNum);

        /// <summary>
        /// Called to see if player has juggernog using this system!
        /// </summary> t6 j
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract bool playerHasJuggernog(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called to add SpeedCola using this system!
        /// </summary>
        public abstract void AddSpeedCola(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called to see if player has SpeedCola using this system!
        /// </summary> t6 j
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract bool playerHasSpeedCola(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called to add Double Tap using this system!
        /// </summary>
        public abstract void AddDoubleTap(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called to see if player has Double Tap using this system!
        /// </summary> t6 j
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract bool playerHasDoubleTap(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called to add Quick Revive using this system!
        /// </summary>
        public abstract void AddQuickRevive(Kit_PlayerBehaviour pb, int speedNum);

        /// <summary>
        /// Called to see if player has Quick Revive using this system!
        /// </summary> t6 j
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract bool playerHasQuickRevive(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called to add StaminUp using this system!
        /// </summary>
        public abstract void AddStaminUp(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called to see if player has StaminUp using this system!
        /// </summary> t6 j
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract bool playerHasStaminUp(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called to add BunnyHop using this system!
        /// </summary>
        public abstract void AddBunnyHop(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called to see if player has BunnyHop using this system!
        /// </summary> t6 j
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract bool playerHasBunnyHop(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called to remove all perks from player.
        /// </summary> t6 j
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract void RemoveAllPerks(Kit_PlayerBehaviour pb);
    }
}