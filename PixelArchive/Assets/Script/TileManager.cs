using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public Tilemap tilemap;
    // public TileBase tempbase;

    public GameObject[] hallTile;
    public GameObject[] cornerTile;
    public GameObject[] roomTile;
    public List<String> randomTileType = new List<string> ();
    public int prevTileDir = 8;
    
    public UnityEngine.Vector2Int tileSize = new Vector2Int(10, 6);   //Param으로 받거나 따로 관리해줄 예정이지만 일단 하드코딩
    public UnityEngine.Vector2Int nowPivot = new Vector2Int(0, 0);
    public List<Vector2Int> pivotList;

    public Dictionary<Vector3Int, bool> objectOnTile;   //오브젝트가 타일 위에 있는가에 대한 정보
    public Dictionary<Vector3Int, int> objectCount;     //타일 위에 있는 오브젝트 개수에 대한 정보

    public void Start() {
        //Hall/Corner 랜덤 뽑기
        //Tile 타입을 읽어와서 자동으로 List에 추가해주는 기능? > 일단 하드 코딩
        if (randomTileType.Count == 0) {
        randomTileType.Add("Hall");
        randomTileType.Add("Corner");
        }
        pivotList.Add(nowPivot);
    }
    public void loadTile() {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            //해당 좌표에 타일이 없으면 넘어감
            if(!tilemap.HasTile(pos)) continue;
            var tile = tilemap.GetTile<TileBase>(pos);
            //정보 초기화
            objectOnTile.Add(pos, true);
            objectCount.Add(pos, 1);
            Debug.Log(objectOnTile);
            Debug.Log(objectCount);
        }
    }

    //중복 타일 체크 함수
    public bool checkTile(int tileDir, Vector2Int nextPivot) {
        if (tileDir == 2) {
            nextPivot.y -= tileSize.y * 2;  //예상되는 타일의 위치로 이동
            foreach(Vector2Int pivot in pivotList) {
                if (pivot == nextPivot) {
                    Debug.Log("(" + nextPivot.x + ", " + nextPivot.y + ") 위치에 " + tileDir + " 방향의 Tile 생성 시 갇힙니다");
                    return false;           //해당 위치의 pivot에 타일을 생성한 적이 있으면 return
                    }   
            }
        }
        else if (tileDir == 8) {
            nextPivot.y += tileSize.y * 2;
            foreach(Vector2Int pivot in pivotList) {
                if (pivot == nextPivot) {
                    Debug.Log("(" + nextPivot.x + ", " + nextPivot.y + ") 위치에 " + tileDir + " 방향의 Tile 생성 시 갇힙니다");
                    return false;
                }
            }
        }
        else if (tileDir == 4) {
            nextPivot.x -= tileSize.x * 2;
            foreach(Vector2Int pivot in pivotList) {
                if (pivot == nextPivot) {
                    Debug.Log("(" + nextPivot.x + ", " + nextPivot.y + ") 위치에 " + tileDir + " 방향의 Tile 생성 시 갇힙니다");
                    return false;
                }
            }
        }
        else if (tileDir == 6) {
            nextPivot.x += tileSize.x * 2;
            foreach(Vector2Int pivot in pivotList) {
                if (pivot == nextPivot) {
                    Debug.Log("(" + nextPivot.x + ", " + nextPivot.y + ") 위치에 " + tileDir + " 방향의 Tile 생성 시 갇힙니다");
                    return false;
                }
            }
        }
        return true;
    }

    public void addTile() {
        //우선 겹치는 거 상관 없이 해봅시다
        UnityEngine.Vector3 tilePos = new UnityEngine.Vector3(nowPivot.x, nowPivot.y, transform.position.z);
        UnityEngine.Vector2Int nextPivot = nowPivot;

        int typeRandom = UnityEngine.Random.Range(0, randomTileType.Count);
        // int typeRandom = 0;
        // int hallrandom = UnityEngine.Random.Range(0, hallTile.Length());
        if (typeRandom == 0) {
            //Hall 생성
                //Hall은 이전 TileDir에 따라 대응하는 1가지 밖에 생성하지 못하기 때문에 확정
            if (prevTileDir == 2) {
                nextPivot.y -= tileSize.y * 2;          //다음 pviot 위치 동기화
                if (!checkTile(2, nextPivot)) return;   //tile 생성 가능 체크
                GameObject hall = Instantiate(hallTile[0], tilePos, transform.rotation);//tile 생성
                pivotList.Add(nowPivot);                //pivotList에 추가한 pivot 추가
                nowPivot = nextPivot;                   //pivot 위치 동기화
            }
            else if (prevTileDir == 8) {
                nextPivot.y += tileSize.y * 2;
                if (!checkTile(8, nextPivot)) return;
                GameObject hall = Instantiate(hallTile[0], tilePos, transform.rotation);
                pivotList.Add(nowPivot);
                nowPivot = nextPivot;
            }
            else if (prevTileDir == 4) {
                nextPivot.x -= tileSize.x * 2;
                if (!checkTile(4, nextPivot)) return;
                GameObject hall = Instantiate(hallTile[1], tilePos, transform.rotation);
                pivotList.Add(nowPivot);
                nowPivot = nextPivot;
            }
            else if (prevTileDir == 6) {
                nextPivot.x += tileSize.x * 2;
                if (!checkTile(6, nextPivot)) return;
                GameObject hall = Instantiate(hallTile[1], tilePos, transform.rotation);
                pivotList.Add(nowPivot);
                nowPivot = nextPivot;
            }
            else {
                Debug.Log("prevTileDir이 " + prevTileDir + "이/가 될 수 있나요?");
                return;
            }
        }
        else if (typeRandom == 1) {
            //Corner 생성
            int cornerRandom = UnityEngine.Random.Range(0, 2);
            if (prevTileDir == 2) {
                if (cornerRandom == 0) {
                    nextPivot.x += tileSize.x * 2;
                    if (!checkTile(6, nextPivot)) return;
                    GameObject corner = Instantiate(cornerTile[0], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    nowPivot = nextPivot;
                    prevTileDir = 6;
                }
                else if (cornerRandom == 1) {
                    nextPivot.x -= tileSize.x * 2;
                    if (!checkTile(4, nextPivot)) return;
                    GameObject corner = Instantiate(cornerTile[1], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    nowPivot = nextPivot;
                    prevTileDir = 4;
                }
            }
            else if (prevTileDir == 8) {
                if (cornerRandom == 0) {
                    nextPivot.x += tileSize.x * 2;
                    if (!checkTile(6, nextPivot)) return;
                    GameObject corner = Instantiate(cornerTile[2], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    nowPivot = nextPivot;
                    prevTileDir = 6;
                }
                else if (cornerRandom == 1) {
                    nextPivot.x -= tileSize.x * 2;
                    if (!checkTile(4, nextPivot)) return;
                    GameObject corner = Instantiate(cornerTile[3], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    nowPivot = nextPivot;
                    prevTileDir = 4;
                }
            }
            else if (prevTileDir == 6) {
                if (cornerRandom == 0) {
                    nextPivot.y += tileSize.y * 2;
                    if (!checkTile(8, nextPivot)) return;
                    GameObject corner = Instantiate(cornerTile[1], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    nowPivot = nextPivot;
                    prevTileDir = 8;
                }
                else if (cornerRandom == 1) {
                    nextPivot.y -= tileSize.y * 2;
                    if (!checkTile(2, nextPivot)) return;
                    GameObject corner = Instantiate(cornerTile[3], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    nowPivot = nextPivot;
                    prevTileDir = 2;
                }
            }
            else if (prevTileDir == 4) {
                if (cornerRandom == 0) {
                    nextPivot.y += tileSize.y * 2;
                    if (!checkTile(8, nextPivot)) return;
                    GameObject corner = Instantiate(cornerTile[0], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    nowPivot = nextPivot;
                    prevTileDir = 8;
                }
                else if (cornerRandom == 1) {
                    nextPivot.y -= tileSize.y * 2;
                    if (!checkTile(2, nextPivot)) return;
                    GameObject corner = Instantiate(cornerTile[2], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    nowPivot = nextPivot;
                    prevTileDir = 2;
                }
            }
            else {
                Debug.Log("prevTileDir이 " + prevTileDir + "이/가 될 수 있나요?");
                return;
            }
        }

        // Debug.Log("nowPivot: (" + nowPivot.x + ", " + nowPivot.y + ")");
        Debug.Log(pivotList);
        // loadTile();
    }

}
