using UnityEngine;
using UnityEditor;
using MarsFPSKit;
using MarsFPSKit.Weapons;
using System.Linq;
using System.Collections.Generic;
using Photon.Pun;
using MarsFPSKit.ZombieWaveSurvival;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(Kit_PvE_ZombieWaveSurvival_WeaponPurchase))]
public class Kit_PvE_ZombieWaveSurvival_EditorWeaponPurchase : Editor
{
    public static bool foldoutWeapons;

    public static bool foldoutSettings;

    void Awake()
    {
        Kit_PvE_ZombieWaveSurvival_WeaponPurchase spawner = (Kit_PvE_ZombieWaveSurvival_WeaponPurchase)target;

        if (!spawner.main)
        {
            spawner.main = FindObjectOfType<Kit_IngameMain>();
        }
    }

    public override void OnInspectorGUI()
    {
        Kit_PvE_ZombieWaveSurvival_WeaponPurchase spawner = (Kit_PvE_ZombieWaveSurvival_WeaponPurchase)target;

        spawner.main = EditorGUILayout.ObjectField("Main", spawner.main, typeof(Kit_IngameMain), true) as Kit_IngameMain;

        if (spawner.main)
        {
            //First, clamp ID
            spawner.weaponToBuy = Mathf.Clamp(spawner.weaponToBuy, 0, spawner.main.gameInformation.allWeapons.Length - 1);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Selected Weapon: " + spawner.main.gameInformation.allWeapons[spawner.weaponToBuy].weaponName);
            spawner.weaponToBuy = EditorGUILayout.IntSlider("Weapon ID", spawner.weaponToBuy, 0, spawner.main.gameInformation.allWeapons.Length - 1);

            if (spawner.main.gameInformation.allWeapons[spawner.weaponToBuy].GetType() == typeof(Kit_ModernWeaponScript))
            {
                if ((spawner.main.gameInformation.allWeapons[spawner.weaponToBuy] as Kit_ModernWeaponScript).reloadMode == ReloadMode.Chambered || (spawner.main.gameInformation.allWeapons[spawner.weaponToBuy] as Kit_ModernWeaponScript).reloadMode == ReloadMode.ProceduralChambered)
                {
                    spawner.weaponToBuyStartBullets = EditorGUILayout.IntSlider("Bullets Left", spawner.weaponToBuyStartBullets, 0, (spawner.main.gameInformation.allWeapons[spawner.weaponToBuy] as Kit_ModernWeaponScript).bulletsPerMag + 1);
                }
                else
                {
                    spawner.weaponToBuyStartBullets = EditorGUILayout.IntSlider("Bullets Left", spawner.weaponToBuyStartBullets, 0, (spawner.main.gameInformation.allWeapons[spawner.weaponToBuy] as Kit_ModernWeaponScript).bulletsPerMag);
                }

                spawner.weaponToBuyStartBulletsLeftToReload = EditorGUILayout.IntField("Bullets Left to Reload", spawner.weaponToBuyStartBulletsLeftToReload);

                //Check if attachments line up
                if (spawner.main.gameInformation.allWeapons[spawner.weaponToBuy].firstPersonPrefab.GetComponent<Kit_WeaponRenderer>().attachmentSlots.Length != spawner.weaponToBuyAttachments.Length)
                {
                    //Create new array
                    spawner.weaponToBuyAttachments = new int[spawner.main.gameInformation.allWeapons[spawner.weaponToBuy].firstPersonPrefab.GetComponent<Kit_WeaponRenderer>().attachmentSlots.Length];
                }

                //Loop through attachments
                for (int o = 0; o < spawner.weaponToBuyAttachments.Length; o++)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    //Display Attachment name
                    GUILayout.Label("Attachment selected in slot [" + o + "]: " + spawner.main.gameInformation.allWeapons[spawner.weaponToBuy].firstPersonPrefab.GetComponent<Kit_WeaponRenderer>().attachmentSlots[o].attachments[spawner.weaponToBuyAttachments[o]].name);

                    spawner.weaponToBuyAttachments[o] = EditorGUILayout.IntSlider("Attachment ID: ", spawner.weaponToBuyAttachments[o], 0, spawner.main.gameInformation.allWeapons[spawner.weaponToBuy].firstPersonPrefab.GetComponent<Kit_WeaponRenderer>().attachmentSlots[o].attachments.Length - 1);

                    EditorGUILayout.EndVertical();
                }
            }
            else
            {
                spawner.weaponToBuyStartBullets = EditorGUILayout.IntField("Bullets Left", spawner.weaponToBuyStartBullets);
                spawner.weaponToBuyStartBulletsLeftToReload = EditorGUILayout.IntField("Bullets Left to Reload", spawner.weaponToBuyStartBulletsLeftToReload);
            }
            EditorGUILayout.EndVertical();

            foldoutSettings = EditorGUILayout.Foldout(foldoutSettings, "Settings");
            if (foldoutSettings)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                spawner.weaponPrice = EditorGUILayout.IntField("Weapon Price:: ", spawner.weaponPrice);
                spawner.ammoPrice = EditorGUILayout.IntField("Ammo Price:: ", spawner.ammoPrice);
                EditorGUILayout.EndVertical();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(spawner);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }
    }
}