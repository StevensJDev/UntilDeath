using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Gameplay/FPS Toggle")]
        public class Kit_OptionsFPSCounter : Kit_OptionBase
        {
            public override string GetDisplayName()
            {
                return "FPS Counter";
            }

            public override string GetHoverText()
            {
                return "Shows the FPS Counter";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Toggle;
            }

            public override void OnToggleStart(TextMeshProUGUI txt, Toggle toggle)
            {
                bool value = PlayerPrefsExtended.GetBool("fpsToggle", false);

                toggle.isOn = value;
                OnToggleChange(txt, value);
            }

            public override void OnToggleChange(TextMeshProUGUI txt, bool newValue)
            {
                Kit_GameSettings.isFPSCounter = newValue;
                PlayerPrefsExtended.SetBool("fpsToggle", newValue);
            }
        }
    }
}