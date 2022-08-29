using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Kit_OptionsButtonScript : MonoBehaviour
{
    public GameObject rebindFade;
    public Image keyImage;
    private string RebindsKey = "";
    private string RebindType = "";
    private bool isRebinding;

    public Sprite[] keySprites;
    public Dictionary<string, Sprite> keys = new Dictionary<string, Sprite>();
    
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
        if(keys.ContainsKey(RebindsKey)) {
            keyImage.sprite = keys[RebindsKey];
        }
        PlayerPrefs.SetString(RebindType, RebindsKey);
        rebindFade.SetActive(false);
        isRebinding = false;
    }

    public void setUpKeys() {
        keys.Add("LeftAlt", keySprites[0]); // Alt
        keys.Add("Asterisk", keySprites[1]); // *
        keys.Add("BackQuote", keySprites[2]); // `
        keys.Add("Backslash", keySprites[3]); // \
        keys.Add("Backspace", keySprites[4]); // Backspace
        keys.Add("LeftBracket", keySprites[5]); // [
        keys.Add("RightBracket", keySprites[6]); // ]
        keys.Add("CapsLock", keySprites[7]); // Caps
        keys.Add("Comma", keySprites[8]); // ,
        keys.Add("Context", keySprites[9]); // Context
        keys.Add("LeftControl", keySprites[10]); // Ctrl
        keys.Add("Delete", keySprites[11]); // Del
        keys.Add("End", keySprites[12]); // End
        keys.Add("Return", keySprites[13]); // Enter
        keys.Add("KeypadEnter", keySprites[14]); // Enter2
        keys.Add("Equals", keySprites[15]); // =
        keys.Add("Escape", keySprites[16]); // Esc
        keys.Add("Home", keySprites[17]); // Home
        keys.Add("Insert", keySprites[18]); // Ins
        keys.Add("Minus", keySprites[19]); // -
        keys.Add("Undefined", keySprites[20]); // Null
        keys.Add("Numlock", keySprites[21]); // Num Lock
        keys.Add("PageDown", keySprites[22]); // Page Down
        keys.Add("PageUp", keySprites[23]); // Page Up
        keys.Add("Period", keySprites[24]); // .
        keys.Add("Plus", keySprites[25]); // +
        keys.Add("Print", keySprites[26]); // Prt Scrn
        keys.Add("Question", keySprites[27]); // ?
        keys.Add("DoubleQuote", keySprites[28]); // "
        keys.Add("Semicolon", keySprites[29]); // ;
        keys.Add("LeftShift", keySprites[30]); // Shift
        keys.Add("Slash", keySprites[31]); // /
        keys.Add("Space", keySprites[32]); // Space
        keys.Add("Tab", keySprites[33]); // Tab
        keys.Add("Tilde", keySprites[34]); // ~

        keys.Add("A", keySprites[35]); // A
        keys.Add("B", keySprites[36]); // B
        keys.Add("C", keySprites[37]); // C
        keys.Add("D", keySprites[38]); // D
        keys.Add("E", keySprites[39]); // E
        keys.Add("F", keySprites[40]); // F
        keys.Add("G", keySprites[41]); // G
        keys.Add("H", keySprites[42]); // H
        keys.Add("I", keySprites[43]); // I
        keys.Add("J", keySprites[44]); // J
        keys.Add("K", keySprites[45]); // K
        keys.Add("L", keySprites[46]); // L
        keys.Add("M", keySprites[47]); // M
        keys.Add("N", keySprites[48]); // N
        keys.Add("O", keySprites[49]); // O
        keys.Add("P", keySprites[50]); // P
        keys.Add("Q", keySprites[51]); // Q
        keys.Add("R", keySprites[52]); // R
        keys.Add("S", keySprites[53]); // S
        keys.Add("T", keySprites[54]); // T
        keys.Add("U", keySprites[55]); // U
        keys.Add("V", keySprites[56]); // V
        keys.Add("W", keySprites[57]); // W
        keys.Add("X", keySprites[58]); // X
        keys.Add("Y", keySprites[59]); // Y
        keys.Add("Z", keySprites[60]); // Z

        keys.Add("DownArrow", keySprites[61]); // Down
        keys.Add("LeftArrow", keySprites[62]); // Left
        keys.Add("RightArrow", keySprites[63]); // Right
        keys.Add("UpArrow", keySprites[64]); // Up

        keys.Add("Alpha0", keySprites[65]); // 0
        keys.Add("Alpha1", keySprites[66]); // 1
        keys.Add("Alpha2", keySprites[67]); // 2
        keys.Add("Alpha3", keySprites[68]); // 3
        keys.Add("Alpha4", keySprites[69]); // 4
        keys.Add("Alpha5", keySprites[70]); // 5
        keys.Add("Alpha6", keySprites[71]); // 6
        keys.Add("Alpha7", keySprites[72]); // 7
        keys.Add("Alpha8", keySprites[73]); // 8
        keys.Add("Alpha9", keySprites[74]); // 9

        keys.Add("F1", keySprites[75]); // F1
        keys.Add("F2", keySprites[76]); // F2
        keys.Add("F3", keySprites[77]); // F3
        keys.Add("F4", keySprites[78]); // F4
        keys.Add("F5", keySprites[79]); // F5
        keys.Add("F6", keySprites[80]); // F6
        keys.Add("F7", keySprites[81]); // F7
        keys.Add("F8", keySprites[82]); // F8
        keys.Add("F9", keySprites[83]); // F9
        keys.Add("F10", keySprites[84]); // F10
        keys.Add("F11", keySprites[85]); // F11
        keys.Add("F12", keySprites[86]); // F12

        keys.Add("Mouse0", keySprites[87]); // LeftClick
        keys.Add("Mouse2", keySprites[88]); // ScrollClick
        keys.Add("Mouse1", keySprites[89]); // RightClick
    }
}
