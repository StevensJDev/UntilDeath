using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Kit_OptionsButtonScript : MonoBehaviour
{
    public GameObject rebindFade;
    private string RebindsKey = "";
    private string RebindType = "";
    private bool isRebinding;

    
    void Start() {
        rebindFade = GameObject.Find("OptionsScreen/OptionsCanvas/RebindFade");
    }

    void Update() {
        if (isRebinding && Input.anyKeyDown)
        {
            foreach(KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(kcode))
                    RebindsKey = kcode.ToString();
                    RebindComplete();
            }
        }
    }

    public void StartRebinding(string rebindType)
    {
        RebindType = rebindType;
        isRebinding = true;
        rebindFade.SetActive(true);
    }

    private void RebindComplete()
    {
        TextMeshProUGUI bTxt = this.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        bTxt.text = RebindsKey;
        PlayerPrefs.SetString(RebindType, RebindsKey);
        rebindFade.SetActive(false);
        isRebinding = false;
    }
}
