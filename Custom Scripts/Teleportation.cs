using Photon.Pun;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Teleportation : Kit_InteractableObject
        {
            [Header("Settings")]

            /// <summary>
            /// Where will the player land?
            /// </summary>
            public GameObject teleportationDestination;

            public override bool CanInteract(Kit_PlayerBehaviour who)
            {
                interactionText = "Press [" + PlayerPrefs.GetString("Interact", "F") + "] to teleport.";
                return true;
            }

            public override void Interact(Kit_PlayerBehaviour who)
            {
                who.gameObject.transform.position = teleportationDestination.transform.position;
                float y = teleportationDestination.transform.rotation.y;
                who.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, -90f, 0));
            }
        }
    }
}