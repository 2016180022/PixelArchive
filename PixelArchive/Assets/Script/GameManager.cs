using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Numerics;

public class GameManager : MonoBehaviour
{
    public bool isDialogActive;
    public GameObject dialogPanel;
    public GameObject scanObj;
    public DialogManager dManager;
    public TMP_Text dialogText;
    public Image charSprite;
    public int dialogIndex;

    public GameObject[] Grid;
    public UnityEngine.Vector2Int tileSize = new Vector2Int(10, 6);   //Param으로 받거나 따로 관리해줄 예정이지만 일단 하드코딩
    public UnityEngine.Vector2Int nowPivot = new Vector2Int(0,0);


    public PlayerAction player;
    public static GameManager instance;

    private void Awake() {
        instance = this;
    }

    public void setText(GameObject scanobj)
    {
        scanObj = scanobj;
        ObjectData objData = scanObj.GetComponent<ObjectData>();
        getText(objData.id, objData.isNpc);

        dialogPanel.SetActive(isDialogActive);
    }

    public void getText(int id, bool isNpc) {
        string dData = dManager.GetDialog(id, dialogIndex);
        
        if (dData == null) {
            isDialogActive = false;
            dialogIndex = 0;
            return;
        }

        if (isNpc) {
             dialogText.text = dData.Split("/")[0];
             charSprite.sprite = dManager.GetSprite(id, int.Parse(dData.Split("/")[1]));
             charSprite.color = new Color(1, 1, 1, 1);
        }
        else {
            dialogText.text = dData;
            charSprite.color = new Color(1, 1, 1, 0);
        }

        isDialogActive = true;
        dialogIndex++;
    }

    //TileManager를 따로 만들어서 사용?
    public void addTile() {
        //i * 2 해서 Keypad Direction
        int i = Random.Range(1,5);
        UnityEngine.Vector3 tilePos = new UnityEngine.Vector3(nowPivot.x, nowPivot.y, transform.position.z);
        if(i == 1) {
            tilePos.y -= tileSize.y * 2;
            nowPivot.y -= tileSize.y * 2;
        }
        else if (i == 2) {
            tilePos.x -= tileSize.x * 2;
            nowPivot.x -= tileSize.x * 2;
        } 
        else if (i == 3) {
            tilePos.x += tileSize.x * 2;
            nowPivot.x += tileSize.x * 2;
        }
        else if (i == 4) {
            tilePos.y += tileSize.y * 2;
            nowPivot.y += tileSize.y * 2;
        }

        Debug.Log("random:" + i);
        Debug.Log("nowPivot: " + nowPivot.x + ", " + nowPivot.y);
        GameObject grid = Instantiate(Grid[i-1], tilePos, transform.rotation);
    }
}
