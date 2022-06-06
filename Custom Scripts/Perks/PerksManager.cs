using Photon.Pun;
using System;
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This will store runtime data for the controlling player
    /// </summary>
    public class PerkManagerControllerRuntimeData
    {
        public bool hasJuggernog;

        public bool hasSpeedCola;
        public bool hasDoubleTap;
        public bool hasQuickRevive;
        public bool hasBunnyHop;
        public bool hasStaminUp;
    }

    /// <summary>
    /// Implements a perk
    /// </summary>
    [CreateAssetMenu(menuName = "MarsFPSKit/Perks Manager")]
    public class PerksManager : PerksManagerBase
    {
        /// Activate and deactivate perk
        public GameObject juggernogUI;
        public GameObject speedcolaUI;
        public GameObject doubleTapUI;
        public GameObject quickReviveUI;
        public GameObject bunnyHopUI;
        public GameObject staminUpUI;

        public override void SetupManager(Kit_PlayerBehaviour pb)
        {
            //Setup runtime data
            PerkManagerControllerRuntimeData runtimeData = new PerkManagerControllerRuntimeData();
            pb.customPerkManagerData = runtimeData; //Assign
        }

        public override void AddJuggernog(Kit_PlayerBehaviour pb, int healthNum)
        {
            juggernogUI = GameObject.Find("MarsFPSKit_IngamePrefab/UI/HUD/Root/Root (Can be hidden)/Perks/JuggernogUI");
            PerkManagerControllerRuntimeData runtimeData = pb.customPerkManagerData as PerkManagerControllerRuntimeData;
            runtimeData.hasJuggernog = true;
            pb.vitalsManager.ChangeHealth(pb, healthNum);
            juggernogUI.SetActive(true);
        }

        public override bool playerHasJuggernog(Kit_PlayerBehaviour pb)
        {
            //Get runtime data
            if (pb.customPerkManagerData != null && pb.customPerkManagerData.GetType() == typeof(PerkManagerControllerRuntimeData))
            {
                PerkManagerControllerRuntimeData runtimeData = pb.customPerkManagerData as PerkManagerControllerRuntimeData;
                return runtimeData.hasJuggernog;
            }
            return false;
        }

        public override void AddSpeedCola(Kit_PlayerBehaviour pb)
        {
            speedcolaUI = GameObject.Find("MarsFPSKit_IngamePrefab/UI/HUD/Root/Root (Can be hidden)/Perks/SpeedColaUI");
            PerkManagerControllerRuntimeData runtimeData = pb.customPerkManagerData as PerkManagerControllerRuntimeData;
            runtimeData.hasSpeedCola = true;
            speedcolaUI.SetActive(true);
        }

        public override bool playerHasSpeedCola(Kit_PlayerBehaviour pb)
        {
            //Get runtime data
            if (pb.customPerkManagerData != null && pb.customPerkManagerData.GetType() == typeof(PerkManagerControllerRuntimeData))
            {
                PerkManagerControllerRuntimeData runtimeData = pb.customPerkManagerData as PerkManagerControllerRuntimeData;
                return runtimeData.hasSpeedCola;
            }
            return false;
        }

        public override void AddDoubleTap(Kit_PlayerBehaviour pb)
        {
            doubleTapUI = GameObject.Find("MarsFPSKit_IngamePrefab/UI/HUD/Root/Root (Can be hidden)/Perks/DoubleTapUI");
            PerkManagerControllerRuntimeData runtimeData = pb.customPerkManagerData as PerkManagerControllerRuntimeData;
            runtimeData.hasDoubleTap = true;
            doubleTapUI.SetActive(true);
        }

        public override bool playerHasDoubleTap(Kit_PlayerBehaviour pb)
        {
            //Get runtime data
            if (pb.customPerkManagerData != null && pb.customPerkManagerData.GetType() == typeof(PerkManagerControllerRuntimeData))
            {
                PerkManagerControllerRuntimeData runtimeData = pb.customPerkManagerData as PerkManagerControllerRuntimeData;
                return runtimeData.hasDoubleTap;
            }
            return false;
        }

        public override void AddQuickRevive(Kit_PlayerBehaviour pb, int speedNum)
        {
            quickReviveUI = GameObject.Find("MarsFPSKit_IngamePrefab/UI/HUD/Root/Root (Can be hidden)/Perks/QuickReviveUI");
            PerkManagerControllerRuntimeData runtimeData = pb.customPerkManagerData as PerkManagerControllerRuntimeData;
            runtimeData.hasQuickRevive = true;
            quickReviveUI.SetActive(true);
        }

        public override bool playerHasQuickRevive(Kit_PlayerBehaviour pb)
        {
            //Get runtime data
            if (pb.customPerkManagerData != null && pb.customPerkManagerData.GetType() == typeof(PerkManagerControllerRuntimeData))
            {
                PerkManagerControllerRuntimeData runtimeData = pb.customPerkManagerData as PerkManagerControllerRuntimeData;
                return runtimeData.hasQuickRevive;
            }
            return false;
        }

        public override void AddStaminUp(Kit_PlayerBehaviour pb)
        {
            staminUpUI = GameObject.Find("MarsFPSKit_IngamePrefab/UI/HUD/Root/Root (Can be hidden)/Perks/StaminUpUI");
            PerkManagerControllerRuntimeData runtimeData = pb.customPerkManagerData as PerkManagerControllerRuntimeData;
            runtimeData.hasStaminUp = true;
            pb.updateStamina(7f, 5f, false);
            staminUpUI.SetActive(true);
        }

        public override bool playerHasStaminUp(Kit_PlayerBehaviour pb)
        {
            //Get runtime data
            if (pb.customPerkManagerData != null && pb.customPerkManagerData.GetType() == typeof(PerkManagerControllerRuntimeData))
            {
                PerkManagerControllerRuntimeData runtimeData = pb.customPerkManagerData as PerkManagerControllerRuntimeData;
                return runtimeData.hasStaminUp;
            }
            return false;
        }


        public override void AddBunnyHop(Kit_PlayerBehaviour pb)
        {
            bunnyHopUI = GameObject.Find("MarsFPSKit_IngamePrefab/UI/HUD/Root/Root (Can be hidden)/Perks/BunnyHopUI");
            PerkManagerControllerRuntimeData runtimeData = pb.customPerkManagerData as PerkManagerControllerRuntimeData;
            runtimeData.hasBunnyHop = true;
            pb.updateJumpMax(2);
            bunnyHopUI.SetActive(true);
        }

        public override bool playerHasBunnyHop(Kit_PlayerBehaviour pb)
        {
            //Get runtime data
            if (pb.customPerkManagerData != null && pb.customPerkManagerData.GetType() == typeof(PerkManagerControllerRuntimeData))
            {
                PerkManagerControllerRuntimeData runtimeData = pb.customPerkManagerData as PerkManagerControllerRuntimeData;
                return runtimeData.hasBunnyHop;
            }
            return false;
        }

        public override void RemoveAllPerks(Kit_PlayerBehaviour pb)
        { 
            PerkManagerControllerRuntimeData runtimeData = pb.customPerkManagerData as PerkManagerControllerRuntimeData;
            
            if (runtimeData.hasJuggernog) {
                // Removes juggernog
                runtimeData.hasJuggernog = false;
                pb.vitalsManager.ChangeHealth(pb, 100);
                juggernogUI.SetActive(false);
            }
            
            if (runtimeData.hasSpeedCola) {
                // Removes speedcola
                runtimeData.hasSpeedCola = false;
                speedcolaUI.SetActive(false);
            }
            
            if (runtimeData.hasBunnyHop) {
                // Removes bunnyhop
                runtimeData.hasBunnyHop = false;
                pb.updateJumpMax(1);
                bunnyHopUI.SetActive(false);
            }
            
            if (runtimeData.hasStaminUp) {
                // Removes staminup
                runtimeData.hasStaminUp = false;
                pb.updateStamina(6f, 4f, true);
                staminUpUI.SetActive(false);
            }

            if (runtimeData.hasDoubleTap) {
                // Removes doubletap
                runtimeData.hasDoubleTap = false;
                doubleTapUI.SetActive(false);
            }

            if (runtimeData.hasQuickRevive) {
                // Removes quickrevive
                runtimeData.hasQuickRevive = false;
                quickReviveUI.SetActive(false);
            }
        }
    }
}