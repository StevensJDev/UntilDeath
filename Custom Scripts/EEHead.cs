using Photon.Pun;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class EEHead : Kit_InteractableObject
        {
            [HideInInspector]
            /// <summary>
            /// reference to in game main
            /// </summary>
            public Kit_IngameMain main;
            public GameObject skull;
            public EasterEgg easterEgg;

            private void Start()
            {
                //Find main reference
                main = FindObjectOfType<Kit_IngameMain>();
            }

            public override bool CanInteract(Kit_PlayerBehaviour who)
            {
                interactionText = "Press [" + PlayerPrefs.GetString("Interact", "F") + "] to pick up.";
                return true;
            }

            public override void Interact(Kit_PlayerBehaviour who)
            {
                skull.SetActive(false);
                easterEgg.hasSkull = true;
            }
        }
    }
}