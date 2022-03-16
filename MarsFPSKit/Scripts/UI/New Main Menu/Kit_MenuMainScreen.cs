using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        public class Kit_MenuMainScreen : MonoBehaviour
        {
            /// <summary>
            /// Menu manager reference
            /// </summary>
            public Kit_MenuManager menuManager;

            /// <summary>
            /// Button to play singleplayer
            /// </summary>
            public Button singleplayerButton;
            /// <summary>
            /// Button to play coop
            /// </summary>
            public Button coopButton;

            private void Start()
            {
                //Enable/Disable based on assigned game modes
                singleplayerButton.gameObject.SetActive(menuManager.game.allSingleplayerGameModes.Length > 0 && menuManager.singleplayer);
                coopButton.gameObject.SetActive(menuManager.game.allCoopGameModes.Length > 0 && menuManager.coop);

                //Create Callback
                singleplayerButton.onClick.AddListener(delegate { PlaySingleplayer(); });
                coopButton.onClick.AddListener(delegate { PlayCoop(); });
            }

            public void PlaySingleplayer()
            {
                menuManager.SwitchMenu(menuManager.singleplayer.singleplayerScreenId);
            }

            public void PlayCoop()
            {
                menuManager.SwitchMenu(menuManager.coop.coopScreenId);
            }

            public void Region()
            {
                if (menuManager.regionScreen)
                {
                    menuManager.SwitchMenu(menuManager.regionScreen.hostScreenId);
                }
            }

            public void Exit()
            {
                if (menuManager.exitScreen)
                {
                    menuManager.SwitchMenu(menuManager.exitScreen.exitScreenId);
                }
            }

            public void Friends()
            {
                if (menuManager.friends)
                {
                    //Callback
                    menuManager.friends.BeforeOpening();
                    //Switch menu
                    menuManager.SwitchMenu(menuManager.friends.friendsScreenId);
                }
            }

            public void Options()
            {
                if (menuManager.options)
                {
                    //Switch menu
                    menuManager.SwitchMenu(menuManager.options.optionsScreenId);
                }
            }

            public void Loadout()
            {
                if (menuManager.loadout)
                {
                    //Switch menu
                    menuManager.loadout.Open();
                }
            }
        }
    }
}
