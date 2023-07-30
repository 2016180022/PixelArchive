using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    Dictionary<int, string[]> dialogData;
    Dictionary<int, Sprite> charSprite;

    public Sprite[] charSpriteArr;
    
    void Awake() {
        dialogData = new Dictionary<int, string[]>();
        charSprite = new Dictionary<int, Sprite>();
        GenerateData();
    }

    // Update is called once per frame
    void GenerateData() {
        dialogData.Add(1000, new string[] {"반가워./0", "이 곳에는 처음이지?/1"});
        dialogData.Add(100, new string[] {"나무 상자다.", "진짜 그냥 나무 상자다."});

        charSprite.Add(1000, charSpriteArr[0]);
        charSprite.Add(1001, charSpriteArr[1]);
        charSprite.Add(1002, charSpriteArr[2]);
        charSprite.Add(1003, charSpriteArr[3]);
    }

    public string GetDialog(int id, int dialogIndex) {
        if (dialogIndex == dialogData[id].Length) return null;
        else return dialogData[id][dialogIndex];
    }
    
    public Sprite GetSprite(int id, int spriteIndex) {
        return charSprite[id + spriteIndex];
    }
}
