using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Controls/Rebinding Controls")]
        public class Kit_OptionsRebindingControls : Kit_OptionBase
        {
            [SerializeField] public string rebindType = "";
            // [SerializeField] private InputActionReference jumpAction = null;
            // [SerializeField] private PlayerController playerController = null;
            // [SerializeField] private TMP_Text bindingDisplayNameText = null;
            // [SerializeField] private GameObject startRebindObject = null;
            // [SerializeField] private GameObject waitingForInputObject = null;

            // private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

            private const string RebindsKey = "rebinds";

            // private void Start()
            // {
            //     Debug.Log("Button Start");
            //     // string rebinds = PlayerPrefsExtended.GetString(RebindsKey, string.Empty);

            //     // if (string.IsNullOrEmpty(rebinds)) { return; }

            //     // playerController.PlayerInput.actions.LoadBindingOverridesFromJson(rebinds);

            //     // int bindingIndex = jumpAction.action.GetBindingIndexForControl(jumpAction.action.controls[0]);

            //     // bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(
            //     //     jumpAction.action.bindings[bindingIndex].effectivePath,
            //     //     InputControlPath.HumanReadableStringOptions.OmitDevice);
            // }

            // public void Save()
            // {
            //     // string rebinds = playerController.PlayerInput.actions.SaveBindingOverridesAsJson();

            //     // PlayerPrefs.SetString(RebindsKey, rebinds);
            // }

            // public void StartRebinding()
            // {
            //     // startRebindObject.SetActive(false);
            //     // waitingForInputObject.SetActive(true);

            //     // playerController.PlayerInput.SwitchCurrentActionMap("Menu");

            //     // rebindingOperation = jumpAction.action.PerformInteractiveRebinding()
            //     //     .WithControlsExcluding("Mouse")
            //     //     .OnMatchWaitForAnother(0.1f)
            //     //     .OnComplete(operation => RebindComplete())
            //     //     .Start();
            // }

            // private void RebindComplete()
            // {
            //     // int bindingIndex = jumpAction.action.GetBindingIndexForControl(jumpAction.action.controls[0]);

            //     // bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(
            //     //     jumpAction.action.bindings[bindingIndex].effectivePath,
            //     //     InputControlPath.HumanReadableStringOptions.OmitDevice);

            //     // rebindingOperation.Dispose();

            //     // startRebindObject.SetActive(true);
            //     // waitingForInputObject.SetActive(false);

            //     // playerController.PlayerInput.SwitchCurrentActionMap("Gameplay");
            // }

            public override string GetDisplayName()
            {
                return rebindType;
            }

            public override string GetHoverText()
            {
                return "Click to rebind the key for " + rebindType;
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Button;
            }

            public override void OnCreate(TextMeshProUGUI txt, Button button)
            {
                Debug.Log("Button created");
                // bool value = PlayerPrefsExtended.GetString("aimingToggle", false);

                // toggle.isOn = value;
                // OnToggleChange(txt, value);
            }

            public override void OnButtonChange(TextMeshProUGUI txt, Button button, string newValue)
            {
                Debug.Log("Button Pushed");
                // Kit_GameSettings.isAimingToggle = newValue;
                // PlayerPrefsExtended.SetString("aimingToggle", newValue);
            }
        }
    }
}