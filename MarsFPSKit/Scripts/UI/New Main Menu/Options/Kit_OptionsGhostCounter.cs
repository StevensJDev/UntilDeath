using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Gameplay/Ghost Toggle")]
        public class Kit_OptionsGhostCounter : Kit_OptionBase
        {
            public override string GetDisplayName()
            {
                return "Ghost Counter";
            }

            public override string GetHoverText()
            {
                return "Shows the Ghost Counter";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Toggle;
            }

            public override void OnToggleStart(TextMeshProUGUI txt, Toggle toggle)
            {
                bool value = PlayerPrefsExtended.GetBool("ghostToggle", false);

                toggle.isOn = value;
                OnToggleChange(txt, value);
            }

            public override void OnToggleChange(TextMeshProUGUI txt, bool newValue)
            {
                Kit_GameSettings.isGhostCounter = newValue;
                PlayerPrefsExtended.SetBool("ghostToggle", newValue);
            }
        }
    }
}