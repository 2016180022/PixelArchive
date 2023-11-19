using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using System.Linq;

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

}