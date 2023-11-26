using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using System.Linq;
using Unity.Mathematics;

public class GameManager : MonoBehaviour
{
    public bool isDialogActive;
    public GameObject dialogPanel;
    public GameObject scanObj;
    public DialogManager dManager;
    public TMP_Text dialogText;
    public Image charSprite;
    public int dialogIndex;

    public PlayerAction player;
    public static GameManager instance;

    public GameObject[] mobList;                //시연용 제너레이터에 사용할 몬스터 프리팹

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

    //시연용으로 사용할 몬스터 제너레이터
    public void generateMob(Vector2 pos) {
        int i = UnityEngine.Random.Range(0, 2);
        // Vector3 mobPos = new Vector3(pos.x, pos.y, transform.position.z);
        // GameObject mob = Instantiate(mobList[i], mobPos, transform.rotation);
        GameObject mob = Instantiate(mobList[i], pos, transform.rotation);
        Debug.Log("Mob 생성 완료 (Type:" + i + ")");
    }
}