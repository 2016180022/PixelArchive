using System;
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
    public int slotIndex;
    Text childText;
    Image[] childImages;

    void Awake() {
        switch (type) {
            case InfoType.Coin:
                childText = GetComponentsInChildren<Text>()[0];
            break;

            case InfoType.Health:
                childImages = GetComponentsInChildren<Image>();
            break;

            case InfoType.Skill:
                childImages = GetComponentsInChildren<Image>();
            break;
        }
    }

    void LateUpdate() {
        switch (type) {
            case InfoType.Coin:
                childText.text = string.Format("{0:F0}", pplayer.coin);
            break;

            case InfoType.Health:
                //Health Icon 1/2/3을 각각 hp값에 따라 활성화
                switch (pplayer.health) {
                    case 1:
                        childImages[0].enabled = true;
                        childImages[1].enabled = false;
                        childImages[2].enabled = false;
                    break;

                    case 2:
                        childImages[0].enabled = true;
                        childImages[1].enabled = true;
                        childImages[2].enabled = false;
                    break;

                    case 3:
                        childImages[0].enabled = true;
                        childImages[1].enabled = true;
                        childImages[2].enabled = true;
                    break;
                }
            break;

            case InfoType.Skill:
                if (pplayer.skillSlot[slotIndex] > 0) {
                    childImages[pplayer.skillSlot[slotIndex]].enabled = true;
                }
                else {
                    childImages[1].enabled = false;
                    childImages[2].enabled = false;
                    childImages[3].enabled = false;
                    childImages[4].enabled = false;
                }
            break;
        }
    }
}
