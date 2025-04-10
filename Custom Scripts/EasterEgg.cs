using Photon.Pun;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class EasterEgg : Kit_InteractableObject
        {
            [HideInInspector]
            /// <summary>
            /// reference to in game main
            /// </summary>
            public Kit_IngameMain main;
            public GameObject skull;
            public GameObject particles;
            public GameObject teleporter1;
            public GameObject teleporter2;
            public GameObject teleporter1particles;
            public GameObject teleporter2particles;
            public bool hasSkull = false;
            bool skullPlaced = false;
            Kit_PvE_ZombiesSoulBox soulbox;

            private void Start()
            {
                //Find main reference
                main = FindObjectOfType<Kit_IngameMain>();
            }

            public override bool CanInteract(Kit_PlayerBehaviour who)
            {
                if (hasSkull && !skullPlaced) {
                    interactionText = "Press [" + PlayerPrefs.GetString("Interact", "F") + "] to place skull.";
                    return true;
                } else {
                    return false;
                }
            }

            public override void Interact(Kit_PlayerBehaviour who)
            {
                skull.SetActive(true);
                skullPlaced = true;
            }

            public void activateSoulBox(){
                particles.SetActive(true);
                teleporter1.GetComponent<Teleportation>().canTeleport = true;
                teleporter2.GetComponent<Teleportation>().canTeleport = true;
                teleporter1particles.SetActive(true);
                teleporter2particles.SetActive(true);
            }
            //Then do the soul collecting
        }
    }
}