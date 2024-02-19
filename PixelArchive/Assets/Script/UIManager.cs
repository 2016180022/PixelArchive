using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public enum InfoType {Health, Coin, Skill}
    public InfoType type;
    //Instance 불러오기 작동하지 않으므로, 직접 넣어주는 방식으로 구현
    public PlayerAction pplayer;
    Text myText;
    Image myImage;

    void Awake() {
        myText = GetComponent<Text>();
        myImage = GetComponent<Image>();
    }

    void LateUpdate() {
        switch (type) {
            case InfoType.Coin:
                myText.text = string.Format("{0:F0}", pplayer.coin);
            break;

            case InfoType.Skill:
            break;

            case InfoType.Health:
            break;
        }
    }
}
