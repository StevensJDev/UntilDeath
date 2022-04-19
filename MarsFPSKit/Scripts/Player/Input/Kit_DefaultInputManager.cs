using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    public class DefaultInputData
    {
        /// <summary>
        /// When did we check for enemies the last time?
        /// </summary>
        public float lastScan;

        public List<Kit_PlayerBehaviour> enemyPlayersAwareOff = new List<Kit_PlayerBehaviour>();
    }

    [CreateAssetMenu(menuName = "MarsFPSKit/Input Manager/Default")]
    /// <summary>
    /// This is the kit's default input manager
    /// </summary>
    public class Kit_DefaultInputManager : Kit_InputManagerBase
    {
        /// <summary>
        /// How many seconds apart are our scans?
        /// </summary>
        public float scanFrequency = 1f;

        public string[] weaponSlotKeys;

        [Header("Spotting")]
        public LayerMask spottingLayer;
        public LayerMask spottingCheckLayers;
        public float spottingMaxDistance = 50f;
        public Vector2 spottingBoxExtents = new Vector2(30, 30);
        private Vector3 spottingBoxSize;
        public float spottingFov = 90f;
        public float spottingRayDistance = 200f;

        // Keybinding controls
        private KeyCode left; // Not working
        private KeyCode right; // Not working
        private KeyCode up; // Not working
        private KeyCode down; // Not working
        private KeyCode crouch;
        private KeyCode sprint;
        private KeyCode jump;
        private KeyCode dropWeapon;
        private KeyCode lmb;
        private KeyCode rmb;
        private KeyCode reload;
        private KeyCode switchWeapon;
        private KeyCode flashlight;
        private KeyCode scoreboard;
         


        // TODO: Call when exiting the options/controls menu
        public override void InitializeControls(Kit_PlayerBehaviour pb)
        {
            DefaultInputData did = new DefaultInputData();
            pb.inputManagerData = did;
            pb.input.weaponSlotUses = new bool[weaponSlotKeys.Length];
            did.enemyPlayersAwareOff = new List<Kit_PlayerBehaviour>();
            spottingBoxSize = new Vector3(spottingBoxExtents.x, spottingBoxExtents.y, spottingMaxDistance / 2f);

            // Set Keybinds
            // left = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left", "LeftArrow"));
            // right = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right", "RightArrow"));
            // up = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Forwards", "UpArrow"));
            // down = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Backwards", "DownArrow"));

            crouch = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Crouch/Stand", "LeftControl"));
            sprint = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Sprint", "LeftShift"));
            jump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Jump", "Space"));
            dropWeapon = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Interact", "F"));
            lmb = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Shoot/Hit", "Mouse0"));
            rmb = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Aim", "Mouse1"));
            reload = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Reload", "R"));
            switchWeapon = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Quick Switch", "X"));
            flashlight = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Flashlight", "Z"));
            scoreboard = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Scoreboard", "Tab"));

        }

        public override void WriteToPlayerInput(Kit_PlayerBehaviour pb)
        {
            // Constantly being called
            if (pb.inputManagerData != null && pb.inputManagerData.GetType() == typeof(DefaultInputData))
            {
                DefaultInputData did = pb.inputManagerData as DefaultInputData;
                //Get all input
                pb.input.hor = Input.GetAxis("Horizontal");
                pb.input.ver = Input.GetAxis("Vertical");
                pb.input.mouseX = Input.GetAxisRaw("Mouse X"); // Dont change
                pb.input.mouseY = Input.GetAxisRaw("Mouse Y"); // Dont change
                // pb.input.leanLeft = Input.GetButton("Lean Left"); // Not used
                // pb.input.leanRight = Input.GetButton("Lean Right"); // Not used

                pb.input.crouch = Input.GetKey(crouch);
                pb.input.sprint = Input.GetKey(sprint);
                pb.input.jump = Input.GetKeyDown(jump);
                pb.input.dropWeapon = Input.GetKeyDown(dropWeapon);
                pb.input.lmb = Input.GetKey(lmb);
                pb.input.rmb = Input.GetKey(rmb);
                pb.input.reload = Input.GetKey(reload);
                pb.input.switchWeapon = Input.GetKey(switchWeapon);
                pb.input.flashlight = Input.GetKeyDown(flashlight);
                pb.input.scoreboard = Input.GetKeyDown(scoreboard);

                if (pb.input.weaponSlotUses == null || pb.input.weaponSlotUses.Length != weaponSlotKeys.Length) pb.input.weaponSlotUses = new bool[weaponSlotKeys.Length];

                for (int i = 0; i < weaponSlotKeys.Length; i++)
                {
                    int id = i;
                    pb.input.weaponSlotUses[id] = Input.GetButton(weaponSlotKeys[id]);
                }

                //Scan
                if (Time.time > did.lastScan)
                {
                    did.lastScan = Time.time + scanFrequency;
                    ScanForEnemies(pb, did);
                }
            }
        }

        void ScanForEnemies(Kit_PlayerBehaviour pb, DefaultInputData did)
        {
            Collider[] possiblePlayers = Physics.OverlapBox(pb.playerCameraTransform.position + pb.playerCameraTransform.forward * (spottingMaxDistance / 2), spottingBoxSize, pb.playerCameraTransform.rotation, spottingLayer.value);

            //Clean
            did.enemyPlayersAwareOff.RemoveAll(item => item == null);

            //Loop
            for (int i = 0; i < possiblePlayers.Length; i++)
            {
                //Check if it is a player
                Kit_PlayerBehaviour pnb = possiblePlayers[i].transform.root.GetComponent<Kit_PlayerBehaviour>();
                if (pnb && pnb != pb)
                {
                    if (CanSeePlayer(pb, did, pnb))
                    {
                        if (isEnemyPlayer(pb, did, pnb))
                        {
                            if (!did.enemyPlayersAwareOff.Contains(pnb))
                            {
                                //Add to our known list
                                did.enemyPlayersAwareOff.Add(pnb);
                                //Call spotted
                                if (pb.voiceManager)
                                {
                                    pb.voiceManager.SpottedEnemy(pb, pnb);
                                }
                            }
                        }
                    }
                }
            }
        }

        bool CanSeePlayer(Kit_PlayerBehaviour pb, DefaultInputData did, Kit_PlayerBehaviour enemyPlayer)
        {
            if (enemyPlayer)
            {
                RaycastHit hit;
                Vector3 rayDirection = enemyPlayer.playerCameraTransform.position - new Vector3(0, 0.2f, 0f) - pb.playerCameraTransform.position;

                if ((Vector3.Angle(rayDirection, pb.playerCameraTransform.forward)) < spottingFov)
                {
                    if (Physics.Raycast(pb.playerCameraTransform.position, rayDirection, out hit, spottingRayDistance, spottingCheckLayers.value))
                    {
                        if (hit.collider.transform.root == enemyPlayer.transform.root)
                        {
                            return true;
                        }
                        else
                        {

                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        bool isEnemyPlayer(Kit_PlayerBehaviour pb, DefaultInputData did, Kit_PlayerBehaviour enemyPlayer)
        {
            if (pb)
            {
                if (pb.main.currentPvPGameModeBehaviour)
                {
                    if (!pb.main.currentPvPGameModeBehaviour.isTeamGameMode) return true;
                    else
                    {
                        if (pb.myTeam != enemyPlayer.myTeam) return true;
                        else return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}