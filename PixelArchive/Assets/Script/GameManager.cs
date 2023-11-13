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

    public GameObject[] hallTile;
    public GameObject[] cornerTile;
    public GameObject[] roomTile;
    public List<String> randomTileType = new List<string> ();
    public int prevTileDir = 8;
    public int tileCount = 0;
    
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

    //TileManager를 따로 만들어서 사용? 해야할듯
    public void addTile() {
        //우선 겹치는 거 상관 없이 해봅시다
        UnityEngine.Vector3 tilePos = new UnityEngine.Vector3(nowPivot.x, nowPivot.y, transform.position.z);
        
        //Hall/Corner 랜덤 뽑기
            //Tile 타입을 읽어와서 자동으로 List에 추가해주는 기능? > 일단 하드 코딩
        if (randomTileType.Count == 0) {
            randomTileType.Add("Hall");
            randomTileType.Add("Corner");
        }
        
        int typeRandom = UnityEngine.Random.Range(0, randomTileType.Count);
        // int typeRandom = 0;
        // int hallrandom = UnityEngine.Random.Range(0, hallTile.Length());
        int cornerRandom = UnityEngine.Random.Range(0, 2);
        if (typeRandom == 0) {
            //Hall 생성
                //Hall은 이전 TileDir에 따라 대응하는 1가지 밖에 생성하지 못하기 때문에 확정
            if (prevTileDir == 2) {
                GameObject hall = Instantiate(hallTile[0], tilePos, transform.rotation);
                nowPivot.y -= tileSize.y * 2;
                tileCount++;
            }
            else if (prevTileDir == 8) {
                GameObject hall = Instantiate(hallTile[0], tilePos, transform.rotation);
                nowPivot.y += tileSize.y * 2;
                tileCount++;
            }
            else if (prevTileDir == 4) {
                GameObject hall = Instantiate(hallTile[1], tilePos, transform.rotation);
                nowPivot.x -= tileSize.x * 2;
                tileCount++;
            }
            else if (prevTileDir == 6) {
                GameObject hall = Instantiate(hallTile[1], tilePos, transform.rotation);                
                nowPivot.x += tileSize.x * 2;
                tileCount++;
            }
            else {
                Debug.Log("prevTileDir이 " + prevTileDir + "이/가 될 수 있나요?");
                return;
            }
        }
        else if (typeRandom == 1) {
            //Corner 생성
            if (prevTileDir == 2) {
                if (cornerRandom == 0) {
                    GameObject corner = Instantiate(cornerTile[0], tilePos, transform.rotation);
                    nowPivot.x += tileSize.x * 2;
                    tileCount++;
                    prevTileDir = 6;
                }
                else if (cornerRandom == 1) {
                    GameObject corner = Instantiate(cornerTile[1], tilePos, transform.rotation);
                    nowPivot.x -= tileSize.x * 2;
                    tileCount++;
                    prevTileDir = 4;
                }
            }
            else if (prevTileDir == 8) {
                if (cornerRandom == 0) {
                    GameObject corner = Instantiate(cornerTile[2], tilePos, transform.rotation);
                    nowPivot.x += tileSize.x * 2;
                    tileCount++;
                    prevTileDir = 6;
                }
                else if (cornerRandom == 1) {
                    GameObject corner = Instantiate(cornerTile[3], tilePos, transform.rotation);
                    nowPivot.x -= tileSize.x * 2;
                    tileCount++;
                    prevTileDir = 4;
                }
            }
            else if (prevTileDir == 6) {
                if (cornerRandom == 0) {
                    GameObject corner = Instantiate(cornerTile[1], tilePos, transform.rotation);
                    nowPivot.y += tileSize.y * 2;
                    tileCount++;
                    prevTileDir = 8;
                }
                else if (cornerRandom == 1) {
                    GameObject corner = Instantiate(cornerTile[3], tilePos, transform.rotation);
                    nowPivot.y -= tileSize.y * 2;
                    tileCount++;
                    prevTileDir = 2;
                }
            }
            else if (prevTileDir == 4) {
                if (cornerRandom == 0) {
                    GameObject corner = Instantiate(cornerTile[0], tilePos, transform.rotation);
                    nowPivot.y += tileSize.y * 2;
                    tileCount++;
                    prevTileDir = 8;
                }
                else if (cornerRandom == 1) {
                    GameObject corner = Instantiate(cornerTile[2], tilePos, transform.rotation);
                    nowPivot.y -= tileSize.y * 2;
                    tileCount++;
                    prevTileDir = 2;
                }
            }
            else {
                Debug.Log("prevTileDir이 " + prevTileDir + "이/가 될 수 있나요?");
                return;
            }
        }

        Debug.Log("nowPivot: (" + nowPivot.x + ", " + nowPivot.y + ")");
    }

}