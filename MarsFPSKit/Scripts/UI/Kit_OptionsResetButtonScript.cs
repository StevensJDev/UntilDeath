using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Controls/Reset Button")]
        public class Kit_OptionsResetButtonScript : Kit_OptionBase
        {
            public override string GetDisplayName()
            {
                return "Reset Button";
            }

            public override string GetHoverText()
            {
                return "Click to reset keys back to default settings.";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.ResetButton;
            }

            public override void onResetButtonClick()
            {
                PlayerPrefs.SetString("Jump", "Space");
                PlayerPrefs.SetString("Sprint", "LeftShift");
                PlayerPrefs.SetString("Aim", "Mouse1");
                PlayerPrefs.SetString("Shoot/Hit", "Mouse0");
                PlayerPrefs.SetString("Reload", "R");
                PlayerPrefs.SetString("Grenade", "G"); // may not work?
                PlayerPrefs.SetString("Special", "H"); // may not work?
                PlayerPrefs.SetString("Crouch/Stand", "C");
                PlayerPrefs.SetString("Flashlight", "Z");
                PlayerPrefs.SetString("Interact", "F");
                PlayerPrefs.SetString("Melee", "V"); // may not work?
                PlayerPrefs.SetString("Quick Switch", "X");
                PlayerPrefs.SetString("Weapon1", "Alpha0"); // may not work?
                PlayerPrefs.SetString("Weapon2", "Alpha1"); // may not work?
                PlayerPrefs.SetString("Chat", "T"); // may not work?
                PlayerPrefs.SetString("Scoreboard", "Tab");

                // Resets all images
                Kit_OptionsButtonScript[] buttons = (Kit_OptionsButtonScript[])FindObjectsOfType<Kit_OptionsButtonScript>();
                foreach (Kit_OptionsButtonScript button in buttons) {
                    button.KeysUpdated();
                }
            }
        }
    }
}