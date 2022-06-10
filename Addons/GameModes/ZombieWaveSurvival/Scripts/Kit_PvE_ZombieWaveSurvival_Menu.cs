using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using MarsFPSKit.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Kit_PvE_ZombieWaveSurvival_Menu : Kit_MenuPveGameModeBase, IOnEventCallback
        {
            /// <summary>
            /// Menu that gets opened when we are a singleplayer game mode
            /// </summary>
            public int singleplayerMenuId;
            /// <summary>
            /// Menu that gets opened when we are a coop game mode
            /// </summary>
            public int coopMainMenuId;
            /// <summary>
            /// Hosting / Lobby screen
            /// </summary>
            public int coopHostScreenId = 1;
            /// <summary>
            /// Browsing screen id
            /// </summary>
            public int coopBrowserScreenId = 2;

            /// <summary>
            /// Displays the name of our selected map
            /// </summary>
            [Header("Singleplayer Settings")]
            public TextMeshProUGUI spMapName;
            /// <summary>
            /// Currently selected map
            /// </summary>
            private int spCurMap;
            /// <summary>
            /// Displays the name of our selected character
            /// </summary>
            public TextMeshProUGUI spCharacter;
            /// <summary>
            /// Currently selected character
            /// </summary>
            private int spCurChar;
            /// <summary>
            /// Player state
            /// </summary>
            public Kit_MenuPlayerStateBase playerStateSP;
            private bool playerStateUpdated;

            /// <summary>
            /// Displays the name of our selected map
            /// </summary>
            [Header("Coop Settings")]
            public TextMeshProUGUI coopMapName;
            /// <summary>
            /// Currently selected map
            /// </summary>
            private int coopCurMap;
            /// <summary>
            /// Displays the name of our selected character
            /// </summary>
            public TextMeshProUGUI coopCharacter;
            /// <summary>
            /// Start button
            /// </summary>
            public Button coopStartButton;
            /// <summary>
            /// The text that displays the state
            /// </summary>
            public TextMeshProUGUI coopStartButtonText;
            /// <summary>
            /// Ready button
            /// </summary>
            public Button coopReadyButton;
            /// <summary>
            /// The text that displays our ready state
            /// </summary>
            public TextMeshProUGUI coopReadyButtonText;
            /// <summary>
            /// Player state
            /// </summary>
            public Kit_MenuPlayerStateBase playerStateMP;

            /// <summary>
            /// The "Content" object of the Scroll view, where playerEntriesPrefab will be instantiated
            /// </summary>
            [Header("Coop Players")]
            public RectTransform playerEntriesGo;
            /// <summary>
            /// The Player Entry prefab
            /// </summary>
            public GameObject playerEntriesPrefab;
            /// <summary>
            /// Currently active player entries - used for cleanup
            /// </summary>
            private List<GameObject> activePlayerEntries = new List<GameObject>();

            /// <summary>
            /// This is the prefab for the messages
            /// </summary>
            [Header("Coop Chat")]
            public GameObject chatEntryPrefab;
            /// <summary>
            /// This is where the messages get instantiated
            /// </summary>
            public RectTransform chatEntryGo;
            [Tooltip("This lets us input what we want to say")]
            /// <summary>
            /// This lets us input what we want to say
            /// </summary>
            public TMP_InputField chatInput;
            /// <summary>
            /// Used to scroll through the chat histroy
            /// </summary>
            public ScrollRect chatScroll;
            /// <summary>
            /// The server's messages color
            /// </summary>
            public Color chatServerColor = Color.red;
            /// <summary>
            /// The players' messages color
            /// </summary>
            public Color chatPlayerColor = Color.blue;

            /// <summary>
            /// The "Content" object of the Scroll view, where entriesPrefab will be instantiated
            /// </summary>
            [Header("Coop Browser")]
            public RectTransform entriesGo;
            /// <summary>
            /// The Server Browser Entry prefab
            /// </summary>
            public GameObject entriesPrefab;
            /// <summary>
            /// Currently active server browser entries - used for cleanup
            /// </summary>
            private List<GameObject> activeEntries = new List<GameObject>();
            /// <summary>
            /// Cached list of Photon Rooms
            /// </summary>
            private Dictionary<string, RoomInfo> cachedRoomList;
            /// <summary>
            /// Were we in a room? (To fire event for leaving a room)
            /// </summary>
            private bool wasInRoom;

            private Kit_PvE_ZombieWaveSurvival zws;

            #region Unity Calls
            void Awake()
            {
                cachedRoomList = new Dictionary<string, RoomInfo>();
                playerStateUpdated = false;
            }
            #endregion

            public override void SetupMenu(Kit_MenuManager mm, int state, int id)
            {
                base.SetupMenu(mm, state, id);

                //Redraw to default values
                if (myCurrentState == 0)
                {
                    zws = mm.game.allSingleplayerGameModes[id] as Kit_PvE_ZombieWaveSurvival;

                    RedrawSingleplayerMenu();

                    if (PhotonNetwork.InRoom)
                    {
                        Hashtable roomTable = PhotonNetwork.CurrentRoom.CustomProperties;
                        int gameModeType = (int)roomTable["gameModeType"];

                        if (gameModeType == 0)
                        {
                            int gameMode = (int)roomTable["gameMode"];

                            //Checks if we were playing this game mode previously
                            if (gameMode == myId)
                            {
                                //Leave room
                                PhotonNetwork.LeaveRoom();
                                OpenMenu();
                            }
                        }
                    }
                }
                else
                {
                    zws = mm.game.allCoopGameModes[id] as Kit_PvE_ZombieWaveSurvival;

                    RedrawCoopMenu();

                    if (PhotonNetwork.InRoom)
                    {
                        Hashtable roomTable = PhotonNetwork.CurrentRoom.CustomProperties;
                        int gameModeType = (int)roomTable["gameModeType"];

                        if (gameModeType == 1)
                        {
                            int gameMode = (int)roomTable["gameMode"];

                            //Checks if we were playing this game mode previously
                            if (gameMode == myId)
                            {
                                //Open room
                                OnJoinedRoom();
                            }
                        }
                    }
                }
                SingleplayerNextMap();
            }

            void Update() {
                if (!menuManager) {
                    return;
                }
                if (!playerStateUpdated && menuManager.wasLoggedIn) {
                    if (playerStateSP)
                    {
                        playerStateSP.Initialize(menuManager);
                        playerStateUpdated = true;
                    }
                    if (playerStateMP)
                    {
                        playerStateMP.Initialize(menuManager);
                        playerStateUpdated = true;
                    }
                }
            }

            public override void OpenMenu()
            {
                if (myCurrentState == 0)
                {
                    ChangeMenuButton(singleplayerMenuId);
                    if (playerStateSP)
                    {
                        playerStateSP.Initialize(menuManager);
                    }
                }
                else
                {
                    ChangeMenuButton(coopMainMenuId);
                    if (playerStateMP)
                    {
                        playerStateMP.Initialize(menuManager);
                    }
                }
            }

            #region Button Calls


            public void SingleplayerStart()
            {
                StartCoroutine(SingleplayerStartRoutine());
            }

            IEnumerator SingleplayerStartRoutine()
            {
                if (PhotonNetwork.IsConnected)
                    PhotonNetwork.Disconnect();
                while (PhotonNetwork.IsConnected) yield return null;
                PhotonNetwork.OfflineMode = true;
                //Create room options
                RoomOptions options = new RoomOptions();
                //Assign settings
                options.IsVisible = true;
                options.IsOpen = true;
                //Player Limit
                options.MaxPlayers = 1;
                //Create a new hashtable
                options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
                //Map
                options.CustomRoomProperties.Add("map", spCurMap);
                //Game Mode
                options.CustomRoomProperties.Add("gameModeType", 0);
                //Game Mode
                options.CustomRoomProperties.Add("gameMode", myId);
                string[] customLobbyProperties = new string[3];
                customLobbyProperties[0] = "map";
                customLobbyProperties[1] = "gameModeType";
                customLobbyProperties[2] = "gameMode";
                options.CustomRoomPropertiesForLobby = customLobbyProperties;

                //Set character id to custom properties
                Hashtable table = PhotonNetwork.LocalPlayer.CustomProperties;
                table["zwsCharacter"] = spCurChar;
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);

                //Try to create a new room
                if (PhotonNetwork.CreateRoom(null, options, null))
                {

                }
                else
                {
                    //Display error message
                }
            }

            public void SingleplayerNextMap()
            {
                spCurMap++;

                if (spCurMap >= menuManager.game.allSingleplayerGameModes[myId].maps.Length)
                {
                    spCurMap = 0;
                }

                RedrawSingleplayerMenu();
            }

            public void SingleplayerPreviousMap()
            {
                spCurMap--;

                if (spCurMap < 0)
                {
                    spCurMap = menuManager.game.allSingleplayerGameModes[myId].maps.Length - 1;
                }

                RedrawSingleplayerMenu();
            }

            public void SingleplayerNextCharacter()
            {
                spCurChar++;

                if (spCurChar >= zws.characters.Length)
                {
                    spCurChar = 0;
                }

                RedrawSingleplayerMenu();
            }

            public void SingleplayerPreviousCharacter()
            {
                spCurChar--;

                if (spCurChar < 0)
                {
                    spCurChar = zws.characters.Length - 1;
                }

                RedrawSingleplayerMenu();
            }

            /// <summary>
            /// Creates a coop lobby
            /// </summary>
            public void CoopHostGame()
            {
                //Check if we are connected to the Photon Server
                if (PhotonNetwork.IsConnected)
                {
                    //Create room options
                    RoomOptions options = new RoomOptions();
                    //Assign settings
                    options.IsVisible = true;
                    options.IsOpen = true;
                    //Player Limit
                    options.MaxPlayers = menuManager.game.allCoopGameModes[myId].coopPlayerAmount;
                    //Create a new hashtable
                    options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
                    //Map
                    options.CustomRoomProperties.Add("map", coopCurMap);
                    //Game Mode
                    options.CustomRoomProperties.Add("gameModeType", 1);
                    //Game Mode
                    options.CustomRoomProperties.Add("gameMode", myId);
                    string[] customLobbyProperties = new string[3];
                    customLobbyProperties[0] = "map";
                    customLobbyProperties[1] = "gameModeType";
                    customLobbyProperties[2] = "gameMode";
                    options.CustomRoomPropertiesForLobby = customLobbyProperties;
                    PhotonNetwork.OfflineMode = false;
                    //Try to create a new room
                    if (PhotonNetwork.CreateRoom(Kit_GameSettings.userName + "'s game", options, null))
                    {

                    }
                    else
                    {
                        //Display error
                    }
                }
                else
                {
                    if (menuManager.regionScreen)
                    {
                        menuManager.regionScreen.GameStartedNotConnected();
                    }
                }
            }

            public void CoopStart()
            {
                if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
                {
                    if (CoopEveryoneReady())
                    {
                        //Deactivate all input
                        menuManager.eventSystem.enabled = false;
                        //Load the map
                        Kit_SceneSyncer.instance.LoadScene(menuManager.game.allSingleplayerGameModes[myId].maps[coopCurMap].sceneName);
                    }
                }
            }

            public void CoopLeaveLobby()
            {
                if (PhotonNetwork.InRoom && wasInRoom)
                {
                    //Just leave dat room
                    PhotonNetwork.LeaveRoom();
                }
            }

            public void CoopReady()
            {
                Hashtable table = PhotonNetwork.LocalPlayer.CustomProperties;

                if (table.ContainsKey("zwsReady"))
                {
                    bool isNow = (bool)table["zwsReady"];
                    //Flip
                    isNow = !isNow;
                    table["zwsReady"] = isNow;
                }
                else
                {
                    table["zwsReady"] = true;
                }

                //Set back
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);
            }

            public void CoopNextMap()
            {
                if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
                {
                    //Remove Event
                    PhotonNetwork.RaiseEvent(100, coopCurMap, new RaiseEventOptions { CachingOption = EventCaching.RemoveFromRoomCache, Receivers = ReceiverGroup.All }, new SendOptions { Reliability = true });

                    coopCurMap++;

                    if (coopCurMap >= menuManager.game.allSingleplayerGameModes[myId].maps.Length)
                    {
                        coopCurMap = 0;
                    }

                    RedrawCoopMenu();

                    //Send Event
                    PhotonNetwork.RaiseEvent(100, coopCurMap, new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCacheGlobal, Receivers = ReceiverGroup.All }, new SendOptions { Reliability = true });
                }
            }

            public void CoopPreviousMap()
            {
                if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
                {
                    //Remove Event
                    PhotonNetwork.RaiseEvent(100, coopCurMap, new RaiseEventOptions { CachingOption = EventCaching.RemoveFromRoomCache, Receivers = ReceiverGroup.All }, new SendOptions { Reliability = true });

                    coopCurMap--;

                    if (coopCurMap < 0)
                    {
                        coopCurMap = menuManager.game.allSingleplayerGameModes[myId].maps.Length - 1;
                    }

                    RedrawCoopMenu();

                    //Send Event
                    PhotonNetwork.RaiseEvent(100, coopCurMap, new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCacheGlobal, Receivers = ReceiverGroup.All }, new SendOptions { Reliability = true });
                }
            }

            public void CoopNextCharacter()
            {
                Hashtable table = PhotonNetwork.LocalPlayer.CustomProperties;

                int curChar = (int)table["zwsCharacter"];
                int nextChar = curChar;

                bool nextCharFound = false;

                while (!nextCharFound)
                {
                    nextChar++;

                    if (nextChar >= zws.characters.Length) nextChar = 0;

                    if (CoopCharacterIsAvailable(nextChar))
                    {
                        nextCharFound = true;
                    }
                }

                table["zwsCharacter"] = nextChar;
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);

                RedrawCoopMenu();
            }

            public void CoopPreviousCharacter()
            {
                Hashtable table = PhotonNetwork.LocalPlayer.CustomProperties;

                int curChar = (int)table["zwsCharacter"];
                int nextChar = curChar;

                bool nextCharFound = false;

                while (!nextCharFound)
                {
                    nextChar--;

                    if (nextChar < 0) nextChar = zws.characters.Length - 1;

                    if (CoopCharacterIsAvailable(nextChar))
                    {
                        nextCharFound = true;
                    }
                }

                table["zwsCharacter"] = nextChar;
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);

                RedrawCoopMenu();
            }

            /// <summary>
            /// Checks if every player is ready
            /// </summary>
            /// <returns></returns>
            public bool CoopEveryoneReady()
            {
                for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    Hashtable table = PhotonNetwork.PlayerList[i].CustomProperties;

                    if (table.ContainsKey("zwsReady"))
                    {
                        if (!(bool)table["zwsReady"]) return false;
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// Checks if the supplie character ID can still be selected
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public bool CoopCharacterIsAvailable(int id)
            {
                //We can reselect our own char
                if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("zwsCharacter") && (int)PhotonNetwork.LocalPlayer.CustomProperties["zwsCharacter"] == id) return true;

                if (zws.characters[id].characterCanBeSelectedByEveryone) return true;

                for (int i = 0; i < PhotonNetwork.PlayerListOthers.Length; i++)
                {
                    Hashtable table = PhotonNetwork.PlayerListOthers[i].CustomProperties;

                    if (table.ContainsKey("zwsCharacter"))
                    {
                        int playersChar = (int)table["zwsCharacter"];
                        if (playersChar == id) return false;
                    }
                }


                return true;
            }

            /// <summary>
            /// Returns the first character that is free or 0 if none is found
            /// </summary>
            /// <returns></returns>
            public int CoopGetInitialJoinCharacter()
            {
                for (int i = 0; i < zws.characters.Length; i++)
                {
                    int id = i;

                    if (CoopCharacterIsAvailable(id))
                    {
                        return id;
                    }
                }

                return 0;
            }

            #endregion

            #region UI
            private void RedrawSingleplayerMenu()
            {
                spMapName.text = menuManager.game.allSingleplayerGameModes[myId].maps[spCurMap].mapName;
                spCharacter.text = zws.characters[spCurChar].characterName;
            }

            private void RedrawCoopMenu()
            {
                coopMapName.text = menuManager.game.allCoopGameModes[myId].maps[coopCurMap].mapName;

                if (PhotonNetwork.InRoom)
                {
                    if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("zwsCharacter"))
                    {
                        //Redraw our character
                        int character = (int)PhotonNetwork.LocalPlayer.CustomProperties["zwsCharacter"];
                        coopCharacter.text = zws.characters[character].characterName;
                    }
                    else
                    {
                        coopCharacter.text = zws.characters[CoopGetInitialJoinCharacter()].characterName;
                    }

                    if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("zwsReady"))
                    {
                        //Redraw our character
                        bool ready = (bool)PhotonNetwork.LocalPlayer.CustomProperties["zwsReady"];
                        
                        if (ready)
                        {
                            coopReadyButtonText.text = "SET NOT READY";
                        }
                        else
                        {
                            coopReadyButtonText.text = "SET READY";
                        }
                    }
                    else
                    {
                        coopReadyButtonText.text = "SET READY";
                    }

                    if (coopStartButtonText)
                    {
                        if (!CoopEveryoneReady())
                        {
                            coopStartButtonText.text = "WAITING FOR READY";
                        }
                        else
                        {
                            if (PhotonNetwork.IsMasterClient)
                            {
                                coopStartButtonText.text = "START GAME";
                            }
                            else
                            {
                                coopStartButtonText.text = "WAITING FOR HOST";
                            }
                        }
                    }

                    //Redraw players

                    //Clean Up
                    for (int i = 0; i < activePlayerEntries.Count; i++)
                    {
                        //Destroy
                        Destroy(activePlayerEntries[i]);
                    }
                    //Reset list
                    activePlayerEntries = new List<GameObject>();

                    for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                    {
                        int playersCharacter = -1;
                        bool playersReady = false;

                        Hashtable table = PhotonNetwork.PlayerList[i].CustomProperties;
                        if (table.ContainsKey("zwsCharacter"))
                        {
                            playersCharacter = (int)table["zwsCharacter"];
                        }
                        if (table.ContainsKey("zwsReady"))
                        {
                            playersReady = (bool)table["zwsReady"];
                        }

                        //Instantiate entry
                        GameObject go = Instantiate(playerEntriesPrefab, playerEntriesGo) as GameObject;
                        //Set it up
                        TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
                        if (playersCharacter >= 0)
                        {
                            if (playersReady)
                            {
                                //Display that mf's name
                                txt.text = PhotonNetwork.PlayerList[i].NickName + " (" + zws.characters[playersCharacter].characterName + ")" + " [READY]";
                            }
                            else
                            {
                                //Display that mf's name
                                txt.text = PhotonNetwork.PlayerList[i].NickName + " (" + zws.characters[playersCharacter].characterName + ")" + " [NOT READY]";
                            }
                        }
                        else
                        {
                            //Display that mf's name
                            txt.text = PhotonNetwork.PlayerList[i].NickName + "(LOADING)";
                        }
                        //Add it to our active list so it will get cleaned up next time
                        activePlayerEntries.Add(go);
                    }
                }
            }

            private void RedrawBrowser()
            {
                //Clean Up
                for (int i = 0; i < activeEntries.Count; i++)
                {
                    //Destroy
                    Destroy(activeEntries[i]);
                }
                //Reset list
                activeEntries = new List<GameObject>();

                //Instantiate new List
                foreach (RoomInfo info in cachedRoomList.Values)
                {
                    int gameModeType = (int)info.CustomProperties["gameModeType"];
                    int gameMode = (int)info.CustomProperties["gameMode"];

                    //1 = Coop
                    if (gameModeType == 1)
                    {
                        //Check if game mode matches
                        if (gameMode == myId)
                        {
                            //Instantiate entry
                            GameObject go = Instantiate(entriesPrefab, entriesGo) as GameObject;
                            //Set it up
                            Kit_CoopBrowserEntry entry = go.GetComponent<Kit_CoopBrowserEntry>();
                            entry.Setup(this, info);
                            //This sets up the join function
                            entry.joinButton.onClick.AddListener(delegate { JoinRoom(info); });
                            //Add it to our active list so it will get cleaned up next time
                            activeEntries.Add(go);
                        }
                    }
                }
            }

            public void JoinRoom(RoomInfo room)
            {
                //Just join room
                PhotonNetwork.JoinRoom(room.Name);
            }
            #endregion

            #region Photon Calls
            //We just created a room
            public override void OnCreatedRoom()
            {
                //Our room is created and ready
                //Lets load the appropriate map
                //Get the hashtable
                ExitGames.Client.Photon.Hashtable table = PhotonNetwork.CurrentRoom.CustomProperties;
                if ((int)table["gameModeType"] == 0) //Singleplayer - load map - coop does it on button click
                {
                    if ((int)table["gameMode"] == myId)
                    {
                        //Get the correct map
                        int mapToLoad = (int)table["map"];
                        //Deactivate all input
                        menuManager.eventSystem.enabled = false;
                        //Load the map
                        Kit_SceneSyncer.instance.LoadScene(menuManager.game.allSingleplayerGameModes[myId].maps[mapToLoad].sceneName);
                    }
                }

                if ((int)table["gameModeType"] == 1) //COOP
                {
                    if ((int)table["gameMode"] == myId)
                    {
                        //Enable button for host
                        coopStartButton.enabled = true;

                        wasInRoom = true;
                    }
                }
            }

            public override void OnJoinedRoom()
            {
                //Get the hashtable
                ExitGames.Client.Photon.Hashtable table = PhotonNetwork.CurrentRoom.CustomProperties;
                if ((int)table["gameModeType"] == 1) //COOP
                {
                    if ((int)table["gameMode"] == myId)
                    {
                        //Select first char
                        int initialChar = CoopGetInitialJoinCharacter();
                        Hashtable charTable = PhotonNetwork.LocalPlayer.CustomProperties;
                        charTable["zwsCharacter"] = initialChar;
                        charTable["zwsReady"] = false;
                        PhotonNetwork.LocalPlayer.SetCustomProperties(charTable);

                        //Go to host screen
                        ChangeMenuButton(coopHostScreenId);

                        //Redraw that mf
                        RedrawCoopMenu();

                        if (!PhotonNetwork.IsMasterClient)
                        {
                            //Disable the button
                            coopStartButton.enabled = false;
                        }
                        else
                        {
                            coopStartButton.enabled = true;
                        }

                        if (chatEntryGo)
                        {
                            //Clean chat
                            foreach (Transform t in chatEntryGo)
                            {
                                Destroy(t.gameObject);
                            }
                        }

                        wasInRoom = true;
                    }
                }
            }

            public override void OnMasterClientSwitched(Player newMasterClient)
            {
                //Get the hashtable
                ExitGames.Client.Photon.Hashtable table = PhotonNetwork.CurrentRoom.CustomProperties;
                if ((int)table["gameModeType"] == 1) //COOP
                {
                    if ((int)table["gameMode"] == myId)
                    {
                        if (newMasterClient.IsLocal)
                        {
                            //Enable the button
                            coopStartButton.enabled = true;
                        }
                    }
                }

                if (wasInRoom)
                {
                    //Instantiate new go
                    GameObject newEntry = Instantiate(chatEntryPrefab, chatEntryGo, false);
                    //Reset scale
                    newEntry.transform.localScale = Vector3.one;
                    //Setup
                    newEntry.GetComponentInChildren<TextMeshProUGUI>().text = "<color=#" + ColorUtility.ToHtmlStringRGB(chatServerColor) + ">Server: </color>" + newMasterClient.NickName + " is the new master client";
                    //Refresh entries
                    Canvas.ForceUpdateCanvases();
                    LayoutRebuilder.ForceRebuildLayoutImmediate(chatEntryGo); //Force layout update
                    chatScroll.verticalScrollbar.value = 0f;
                    Canvas.ForceUpdateCanvases();
                }
            }

            public override void OnPlayerEnteredRoom(Player newPlayer)
            {
                if (wasInRoom)
                {
                    RedrawCoopMenu();

                    //Instantiate new go
                    GameObject newEntry = Instantiate(chatEntryPrefab, chatEntryGo, false);
                    //Reset scale
                    newEntry.transform.localScale = Vector3.one;
                    //Setup
                    newEntry.GetComponentInChildren<TextMeshProUGUI>().text = "<color=#" + ColorUtility.ToHtmlStringRGB(chatServerColor) + ">Server: </color>" + newPlayer.NickName + " joined";
                    //Refresh entries
                    Canvas.ForceUpdateCanvases();
                    LayoutRebuilder.ForceRebuildLayoutImmediate(chatEntryGo); //Force layout update
                    chatScroll.verticalScrollbar.value = 0f;
                    Canvas.ForceUpdateCanvases();
                }
            }

            public override void OnPlayerLeftRoom(Player otherPlayer)
            {
                if (wasInRoom)
                {
                    RedrawCoopMenu();

                    //Instantiate new go
                    GameObject newEntry = Instantiate(chatEntryPrefab, chatEntryGo, false);
                    //Reset scale
                    newEntry.transform.localScale = Vector3.one;
                    //Setup
                    newEntry.GetComponentInChildren<TextMeshProUGUI>().text = "<color=#" + ColorUtility.ToHtmlStringRGB(chatServerColor) + ">Server: </color>" + otherPlayer.NickName + " left";
                    //Refresh entries
                    Canvas.ForceUpdateCanvases();
                    LayoutRebuilder.ForceRebuildLayoutImmediate(chatEntryGo); //Force layout update
                    chatScroll.verticalScrollbar.value = 0f;
                    Canvas.ForceUpdateCanvases();
                }
            }

            public override void OnLeftRoom()
            {
                if (wasInRoom)
                {
                    //Go back to main screen
                    ChangeMenuButton(coopMainMenuId);
                    //Reset
                    wasInRoom = false;
                }
            }

            public override void OnRoomListUpdate(List<RoomInfo> roomList)
            {
                UpdateCachedRoomList(roomList);
                RedrawBrowser();
            }

            public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
            {
                if (wasInRoom)
                {
                    RedrawCoopMenu();
                }
            }

            void IOnEventCallback.OnEvent(EventData photonEvent)
            {
                if (photonEvent.Code == 100)
                {
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        //Set map
                        coopCurMap = (int)photonEvent.CustomData;
                        //Redraw
                        RedrawCoopMenu();
                    }
                }
                else if (photonEvent.Code == Kit_EventIDs.chatMessageReceived)
                {
                    Hashtable chatInformation = (Hashtable)photonEvent.CustomData;
                    //Get sender
                    Photon.Realtime.Player chatSender = Kit_PhotonPlayerExtensions.Find((int)chatInformation[(byte)2]);
                    if (chatSender != null)
                    {
                        string message = (string)chatInformation[(byte)0];

                        //Instantiate new go
                        GameObject newEntry = Instantiate(chatEntryPrefab, chatEntryGo, false);
                        //Reset scale
                        newEntry.transform.localScale = Vector3.one;
                        //Determine Color
                        Color finalCol = chatPlayerColor;
                        //Setup
                        newEntry.GetComponentInChildren<TextMeshProUGUI>().text = "<color=#" + ColorUtility.ToHtmlStringRGB(finalCol) + ">" + chatSender.NickName + "</color>: " + message;
                        //Refresh entries
                        Canvas.ForceUpdateCanvases();
                        LayoutRebuilder.ForceRebuildLayoutImmediate(chatEntryGo); //Force layout update
                        chatScroll.verticalScrollbar.value = 0f;
                        Canvas.ForceUpdateCanvases();
                    }
                }
            }
            #endregion

            public void ChatSendMessage()
            {
                if (!chatInput.text.IsNullOrWhiteSpace())
                {
                    Hashtable chatMessage = new Hashtable(3);
                    chatMessage[(byte)0] = chatInput.text;
                    chatMessage[(byte)1] = 0; //Message type = all
                    chatMessage[(byte)2] = PhotonNetwork.LocalPlayer.ActorNumber;
                    //Send it to everyone
                    PhotonNetwork.RaiseEvent(Kit_EventIDs.chatMessageReceived, chatMessage, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);

                    chatInput.text = "";

                    //Select text back so we can continue to write
                    chatInput.Select();
                    chatInput.ActivateInputField();
                }
            }

            private void UpdateCachedRoomList(List<RoomInfo> roomList)
            {
                foreach (RoomInfo info in roomList)
                {
                    // Remove room from cached room list if it got closed, became invisible or was marked as removed
                    if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                    {
                        if (cachedRoomList.ContainsKey(info.Name))
                        {
                            cachedRoomList.Remove(info.Name);
                        }

                        continue;
                    }

                    // Update cached room info
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList[info.Name] = info;
                    }
                    // Add new room info to cache
                    else
                    {
                        cachedRoomList.Add(info.Name, info);
                    }
                }
            }
        }
    }
}