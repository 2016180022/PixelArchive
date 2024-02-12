using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public GameObject[] hallTile;
    public GameObject[] cornerTile;
    public GameObject[] roomTile;
    public GameObject[] crossTile;
    public int prevTileDir = 8;                                     //직전 생성된 Tile의 방향
    public int minTileCount = 10;                                   //Room이 생성되기 위한 최소 타일 수
    public int roomCount = 0;
    
    public UnityEngine.Vector2Int tileSize = new Vector2Int(10, 6); //Param으로 받거나 따로 관리해줄 예정이지만 일단 하드코딩
    public UnityEngine.Vector2Int nowPivot = new Vector2Int();
    public List<Vector2Int> pivotList;
    public List<String> randomTileType = new List<string>();
    public List<int> tileTypeList = new List<int>();           //만들어진 TileType에 대한 List (연속 검사용)
    public List<Vector2Int> crossedPivotList = new List<Vector2Int>();          //갈림길 Pivot 리스트. 역순으로 검사해서 진행.
    public List<int> crossedPivotPrevDirList = new List<int>();                 //갈림길 Pivot이 가지는 PrevDir 리스트. 위 리스트와 같이 핸들링.
    public int crossedPivotCount;                                               //갈림길 Pviot의 count. List.count로 핸들링하기에는 동기화 시점을 맞춰줄 수 없어서 따로 처리.
    public bool isCorner;                                                       //Corner 타입이 나왔는데, 배치 불가능이라 넘어갔을 경우 다음에도 corner를 넣어줄 처리 인자.

    public Dictionary<String, int> tileTypeAndWeight = new Dictionary<string, int>();    //tileType과 가중치

    public void Start() {
        //Hall/Corner 랜덤 뽑기
        //Tile 타입을 읽어와서 자동으로 List에 추가해주는 기능? > 일단 하드 코딩
        if (randomTileType.Count == 0) {
        randomTileType.Add("Hall");
        randomTileType.Add("Corner");
        }
        
        //각 type의 Tile이 GameObject에 연결되어 있으면, dict에 추가
        if (hallTile.Length > 0) tileTypeAndWeight.Add("Hall", 45);
        if (cornerTile.Length > 0) tileTypeAndWeight.Add("Corner", 45);
        if (crossTile.Length > 0) tileTypeAndWeight.Add("Cross",10);
        if (roomTile.Length > 0) tileTypeAndWeight.Add("Room",10);

        pivotList.Add(nowPivot);
        isCorner = false;

        //Pivot 기본값 조정
        nowPivot = new Vector2Int(0, 12);
    }

    //공용 사용의 여지가 있으니, 최대한 범용성 있게 함수 구성
    private int getRandom(Dictionary<String,int> weightDict) {
        //sumWeight 변수 설정
        int sumWeight = 0;
        List<int> weightList = new List<int>();

        foreach (var type in weightDict){
            sumWeight += type.Value;
            weightList.Add(sumWeight);
        }
        // Debug.Log("sumWeight " + sumWeight);
        // Debug.Log(weightList[0] + ", " + weightList[1]);
        
        int index = 0;
        int rand = UnityEngine.Random.Range(0, sumWeight);
        for (int i = 0; i < weightList.Count; i++) {
            if (rand < weightList[i]) {
                index = i;
                break;
            }
        }
        // Debug.Log("rand is " + rand + ", now index " + index);
        return index;
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

    //수정 각 보기
    // 생성 버튼 클릭 시, 모든 타일 생성 및 성공적으로 생성 시에만 타일을 생성하도록 변경
    // Cross / Room 타일 생성되는 걸 확률이 아니라, 지정한 길이에 따른 계수값으로 생성하도록 변경 (길이 10이면 5에 Cross 생성, 갈림길 이후 3번 진행 후 Room 생성 이런 식)
    // 타일 타입 당 여러개의 모양 중에서 1가지 생성되록 변경
    // > 이건 지금 타일 타입별로 배열로 받는 방식에서, 타일 타입별로 각자 다 받고 배열로 여러개의 모양을 받는 식으로 하면 될 듯
    // 맵 생성 시에 해당 프리팹을 List에 저장해뒀다가, 삭제 처리 시 모두 한번에 삭제하는 기능 구현
    // 맵 삭제 시 플레이어 다시 0,0으로 보내는 기능도 구현 필요
    public void addTile() {
        UnityEngine.Vector3 tilePos = new UnityEngine.Vector3(nowPivot.x, nowPivot.y, transform.position.z);

        //가중치 랜덤 함수
        int tileType;
        if (isCorner == true) tileType = 3;
        else {tileType = getRandom(tileTypeAndWeight);}
        
        //테스트용 Corner타일 생성
        if (pivotList.Count == 10) tileType = 3;

        //HallType 중복 검사
        if (tileTypeList.Count >= 2) {
            int length = tileTypeList.Count;
            if (tileTypeList[length - 1] == 0 && tileTypeList[length - 2] == 0) {
                tileType = 1;       //Hall이 두번 나왔을 경우, 강제로 Corner 생성
            }
        }

        //randomType 0: Hall / 1: Corner / 2: room / 3: cross
        if (tileType == 0) {
            //Hall 생성
                //Hall은 이전 TileDir에 따라 대응하는 1가지 밖에 생성하지 못하기 때문에 확정
            if (prevTileDir == 2) {
                if (!checkTile(2, nowPivot)) return;    //tile 생성 가능 체크
                GameObject hall = Instantiate(hallTile[0], tilePos, transform.rotation);//tile 생성
                pivotList.Add(nowPivot);                //pivotList에 추가한 pivot 추가
                nowPivot.y -= tileSize.y * 2;           //pivot 위치 동기화
            }
            else if (prevTileDir == 8) {
                if (!checkTile(8, nowPivot)) return;
                GameObject hall = Instantiate(hallTile[0], tilePos, transform.rotation);
                pivotList.Add(nowPivot);
                nowPivot.y += tileSize.y * 2;
            }
            else if (prevTileDir == 4) {
                if (!checkTile(4, nowPivot)) return;
                GameObject hall = Instantiate(hallTile[1], tilePos, transform.rotation);
                pivotList.Add(nowPivot);
                nowPivot.x -= tileSize.x * 2;
            }
            else if (prevTileDir == 6) {
                if (!checkTile(6, nowPivot)) return;
                GameObject hall = Instantiate(hallTile[1], tilePos, transform.rotation);
                pivotList.Add(nowPivot);
                nowPivot.x += tileSize.x * 2;
            }
            else {
                Debug.Log("prevTileDir이 " + prevTileDir + "이/가 될 수 있나요?");
                return;
            }
        }
        else if (tileType == 1) {
            //Corner 생성
            int cornerRandom = UnityEngine.Random.Range(0, 2);
            if (prevTileDir == 2) {
                if (cornerRandom == 0) {
                    if (!checkTile(6, nowPivot)) return;
                    GameObject corner = Instantiate(cornerTile[0], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    nowPivot.x += tileSize.x * 2;
                    prevTileDir = 6;
                }
                else if (cornerRandom == 1) {                   
                    if (!checkTile(4, nowPivot)) return;
                    GameObject corner = Instantiate(cornerTile[1], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    nowPivot.x -= tileSize.x * 2;
                    prevTileDir = 4;
                }
            }
            else if (prevTileDir == 8) {
                if (cornerRandom == 0) {                   
                    if (!checkTile(6, nowPivot)) return;
                    GameObject corner = Instantiate(cornerTile[2], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    nowPivot.x += tileSize.x * 2;
                    prevTileDir = 6;
                }
                else if (cornerRandom == 1) {
                    if (!checkTile(4, nowPivot)) return;
                    GameObject corner = Instantiate(cornerTile[3], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    nowPivot.x -= tileSize.x * 2;
                    prevTileDir = 4;
                }
            }
            else if (prevTileDir == 6) {
                if (cornerRandom == 0) {
                    if (!checkTile(8, nowPivot)) return;
                    GameObject corner = Instantiate(cornerTile[1], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    nowPivot.y += tileSize.y * 2;
                    prevTileDir = 8;
                }
                else if (cornerRandom == 1) {
                    if (!checkTile(2, nowPivot)) return;
                    GameObject corner = Instantiate(cornerTile[3], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    nowPivot.y -= tileSize.y * 2;
                    prevTileDir = 2;
                }
            }
            else if (prevTileDir == 4) {
                if (cornerRandom == 0) {
                    if (!checkTile(8, nowPivot)) return;
                    GameObject corner = Instantiate(cornerTile[0], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    nowPivot.y += tileSize.y * 2;
                    prevTileDir = 8;
                }
                else if (cornerRandom == 1) {
                    if (!checkTile(2, nowPivot)) return;
                    GameObject corner = Instantiate(cornerTile[2], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    nowPivot.y -= tileSize.y * 2;
                    prevTileDir = 2;
                }
            }
            else {
                Debug.Log("prevTileDir이 " + prevTileDir + "이/가 될 수 있나요?");
                return;
            }
        }
        else if (tileType == 2) {
            //최소한의 Tile 길이가 존재해야 하므로, 우선 해당 판정
            if (pivotList.Count < minTileCount) {
                Debug.Log("Room 타일이 생성되려고 했지만, 최소 타일 수에 도달하지 않아 return합니다");
                return;
            }

            //Room 생성
            if (prevTileDir == 2) { GameObject room = Instantiate(roomTile[0], tilePos, transform.rotation); }
            else if (prevTileDir == 4) { GameObject room = Instantiate(roomTile[1], tilePos, transform.rotation); }
            else if (prevTileDir == 6) { GameObject room = Instantiate(roomTile[2], tilePos, transform.rotation); }
            else if (prevTileDir == 8) { GameObject room = Instantiate(roomTile[3], tilePos, transform.rotation); }

            //생성 완료되었으면 카운트 처리
            roomCount++;
            //Room 생성 후에는 더 이상 해당 Pivot을 트래킹 할 필요가 없으므로, crossedPivotList에서 남은 Pivot을 생성해주러 이동
            Debug.Log("이번 갈림길 Pivot의 Room이 생성되었습니다.");
            if (crossedPivotList.Count > 1) {
                //이렇게 처리하려면 처리 완료 시에 역순으로 빼줘야 함
                nowPivot = crossedPivotList[crossedPivotCount - 1];
                prevTileDir = crossedPivotPrevDirList[crossedPivotCount - 1];
                //Pivot을 옮기면 데이터 삭제 처리
                crossedPivotList.RemoveAt(crossedPivotCount - 1);
                crossedPivotPrevDirList.RemoveAt(crossedPivotCount - 1);
                crossedPivotCount--;
            }
        }
        else if (tileType == 3) {
            if (prevTileDir == 2) {
                //방향 양 쪽 다 만들 수 있는지 먼저 체크
                if (!checkTile(4, nowPivot) || !checkTile(6, nowPivot)) {
                    //만약 안되면 생성을 못하니까, 다음 턴에 무조건 corner를 생성하는 로직을 넣어야 함
                    isCorner = true;
                    return;
                }
                //4, 6 고정이니까 갈림길 하나 만들어주고
                GameObject corner = Instantiate(crossTile[0], tilePos, transform.rotation);
                pivotList.Add(nowPivot);

                //이후에 갈 Pivot으로 옮겨줌
                Vector2Int tempPivot = nowPivot;
                tempPivot.x -= tileSize.x * 2;
                crossedPivotList.Add(tempPivot);

                //6 방향으로 진행하도록 PrevDir 6 넣어주고 처리도 해줌
                // pivotList.Add(nowPivot);
                nowPivot.x += tileSize.x * 2;
                prevTileDir = 6;
                //나머지 4는 List에 저장
                crossedPivotPrevDirList.Add(4);
            }
            else if (prevTileDir == 8) {
                if (!checkTile(4, nowPivot) || !checkTile(6, nowPivot)) {
                    isCorner = true;
                    return;
                }
                GameObject corner = Instantiate(crossTile[3], tilePos, transform.rotation);
                pivotList.Add(nowPivot);

                Vector2Int tempPivot = nowPivot;
                tempPivot.x -= tileSize.x * 2;
                crossedPivotList.Add(tempPivot);

                nowPivot.x += tileSize.x * 2;
                prevTileDir = 6;
                crossedPivotPrevDirList.Add(4);
            }
            else if (prevTileDir == 6) {
                if (!checkTile(2, nowPivot) || !checkTile(8, nowPivot)) {
                    isCorner = true;
                    return;
                }
                GameObject corner = Instantiate(crossTile[2], tilePos, transform.rotation);
                pivotList.Add(nowPivot);

                Vector2Int tempPivot = nowPivot;
                tempPivot.y -= tileSize.y * 2;
                crossedPivotList.Add(tempPivot);

                nowPivot.y += tileSize.y * 2;
                prevTileDir = 8;
                crossedPivotPrevDirList.Add(2);
            }
            else if (prevTileDir == 4) {
                if (!checkTile(2, nowPivot) || !checkTile(8, nowPivot)) {
                    isCorner = true;
                    return;
                }
                GameObject corner = Instantiate(crossTile[1], tilePos, transform.rotation);
                pivotList.Add(nowPivot);

                Vector2Int tempPivot = nowPivot;
                tempPivot.y -= tileSize.y * 2;
                crossedPivotList.Add(tempPivot);

                nowPivot.y += tileSize.y * 2;
                prevTileDir = 8;
                crossedPivotPrevDirList.Add(2);
            }
            crossedPivotCount++;
        }
        
        tileTypeList.Add(tileType); //정상적으로 추가 되었을 때만 tileTypeList에 추가
        //crossedPivot이 잘못 저장되는 케이스가 있는 것 같다.

        Debug.Log("nowPivot: (" + nowPivot.x + ", " + nowPivot.y + ")");
        Debug.Log("nowTileType:" + tileType);
        // Debug.Log(pivotList[pivotList.Count - 1].x + ", " + pivotList[pivotList.Count - 1].y);
    }

}
