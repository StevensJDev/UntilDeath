using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Attach this to a trigger in order to slow down the player inside the trigger.
    /// </summary>
    public class Kit_MudTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.gameObject.GetComponent<Kit_PlayerBehaviour>()) {
                Kit_PlayerBehaviour pb = other.transform.root.gameObject.GetComponent<Kit_PlayerBehaviour>();
                pb.updateStamina(2.5f, 2.5f, true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.root.gameObject.GetComponent<Kit_PlayerBehaviour>()) {
                Kit_PlayerBehaviour pb = other.transform.root.gameObject.GetComponent<Kit_PlayerBehaviour>();
                pb.updateStamina(6f, 4f, true);
            }
        }
    }
}