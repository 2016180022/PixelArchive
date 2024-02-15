using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class TileList {
    public GameObject[] Tile;
}

public class TileManager : MonoBehaviour
{
    //Obj[dirType][List]    >2차원 배열은 Inspector 이슈로 보류
    public TileList[] hallTile;
    public TileList[] cornerTile;
    public TileList[] roomTile;
    public TileList[] crossTile;
    public int prevTileDir;                                                     //직전 생성된 Tile의 방향
    public int mapLength;                                                       //유저가 설정하는 맵의 길이 (맵 총 길이, Room타일이 생성되는 거리에 관여)
                                                                                //맵 길이 채우고 나머지 부분에 모두 빈 타일 채워주는 기능?
    public int mapComp;                                                         //유저가 설정하는 맵의 복잡도 (Cross 타일의 생성 개수)
    public int sameTypeTileCount;                                               //같은 타입의 타일 내 종류 수
    
    public UnityEngine.Vector2Int tileSize = new Vector2Int(10, 6);             //Param으로 받거나 따로 관리해줄 예정이지만 일단 하드코딩
    public UnityEngine.Vector2Int nowPivot = new Vector2Int();
    public List<Vector2Int> pivotList;
    public List<String> randomTileType = new List<string>();                    //TileType에 대한 리스트...인데 지금은 사용 안하는듯?
    public List<GameObject> tileObjectList = new List<GameObject>();            //만들어진 Tile의 Object List (삭제 처리용)
    public List<int> tileTypeList = new List<int>();                            //만들어진 TileType에 대한 List (연속 검사용)
    public List<Vector2Int> crossedPivotList = new List<Vector2Int>();          //갈림길 Pivot 리스트. 역순으로 검사해서 진행.
    public List<int> crossedPivotPrevDirList = new List<int>();                 //갈림길 Pivot이 가지는 PrevDir 리스트. 위 리스트와 같이 핸들링.
    public int crossedPivotCount;                                               //갈림길 Pviot의 count. List.count로 핸들링하기에는 동기화 시점을 맞춰줄 수 없어서 따로 처리.
    public int afterCrossedTileCount;                                           //갈림길 이후로부터 생성된 타일 수
    public int nowCrossCount;                                                   //현재 갈림길의 개수
    public int nowRoomCount;                                                    //현재 Room의 개수
    public bool isCross;                                                        //Cross 타입이 나왔는데, 배치 불가능이라 넘어갔을 경우 다음에도 Cross를 넣어줄 처리 인자.

    public Dictionary<String, int> tileTypeAndWeight = new Dictionary<string, int>();    //tileType과 가중치

    public void Start() {
        //Hall/Corner 랜덤 뽑기
        //Tile 타입을 읽어와서 자동으로 List에 추가해주는 기능? > 일단 하드 코딩
        if (randomTileType.Count == 0) {
        randomTileType.Add("Hall");
        randomTileType.Add("Corner");
        }

        //각 type의 Tile이 GameObject에 연결되어 있으면, dict에 추가
        // 타일 배리에이션 기능 추가로 변수가 살짝 변경됨
        // 추후 개선 필요
        if (hallTile.Length > 0) tileTypeAndWeight.Add("Hall", 45);
        if (cornerTile.Length > 0) tileTypeAndWeight.Add("Corner", 45);
        // if (crossTile.Length > 0) tileTypeAndWeight.Add("Cross",10);
        // if (roomTile.Length > 0) tileTypeAndWeight.Add("Room",10);

        pivotList.Add(nowPivot);
        isCross = false;

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

    //생성한 타일 리스트에 저장하는 함수
    private void addTileList(GameObject tile) {
        // 여기서 TileTypeList와 크로스로 체크하기에는 TileTypeList를 최하단에서 한번에 저장해버리기 때문에 불가능
        // 만약 가능한 방법이 있다면 추후 추가
        tileObjectList.Add(tile);
    }

    //생성한 타일 모두 제거하는 함수
    public void deleteAllTile() {
        //오브젝트 모두 Destroy
        foreach(GameObject tile in tileObjectList) {
            Destroy(tile);
        }

        //Pivot 정보도 모두 초기화해줘야 함
        prevTileDir = 8;
        nowCrossCount = 0;
        nowRoomCount = 0;
        crossedPivotCount = 0;
        afterCrossedTileCount = 0;

        nowPivot = new Vector2Int(0,12);

        pivotList.Clear();
        tileObjectList.Clear();
        tileTypeList.Clear();
        crossedPivotList.Clear();
        crossedPivotPrevDirList.Clear();

        isCross = false;
    }

    //수정 각 보기
    // 생성 버튼 클릭 시, 모든 타일 생성 및 성공적으로 생성 시에만 타일을 생성하도록 변경
    // Cross / Room 타일 생성되는 걸 확률이 아니라, 지정한 길이에 따른 계수값으로 생성하도록 변경 (길이 10이면 5에 Cross 생성, 갈림길 이후 3번 진행 후 Room 생성 이런 식)
    // 타일을 생성할 때, 일정 횟수 이상 실패하면 한번 뒤로 가서 다시 생성하는 기능 (만약 깊게 꼬여도 계속 반복하면 언젠가는 빠져 나오지 않을까)
    public void addTile() {
        UnityEngine.Vector3 tilePos = new UnityEngine.Vector3(nowPivot.x, nowPivot.y, transform.position.z);

        //가중치 랜덤 함수
        int tileType;
        if (isCross == true) tileType = 3;
        else {tileType = getRandom(tileTypeAndWeight);}
        // 이거 Cross 벽에 걸려서 생성 못해도 이 변수때문에 계속 생성 못하는 상태가 되어버림... 처리 필요

        //HallType 중복 검사
        if (tileTypeList.Count >= 2) {
            int length = tileTypeList.Count;
            if (tileTypeList[length - 1] == 0 && tileTypeList[length - 2] == 0) {
                tileType = 1;       //Hall이 두번 나왔을 경우, 강제로 Corner 생성
            }
        }

        //맵 길이 처리
        if (crossedPivotCount < 1) {
            if (afterCrossedTileCount > mapLength * 3) {                    //맵 길이가 L3만큼 진행된 이후에만
                if (nowCrossCount < mapComp) tileType = 3;                  //아직 갈림길 개수가 부족하니 갈림길을 하나 더 만들어줌
                else tileType = 2;                                          //남아있는 길이 없고, 갈림길 개수도 충분하니 Room 만들고 맵을 완성
            }
        }
        else {
            if (afterCrossedTileCount > mapLength * 2) {                    //맵 길이가 L2만큼 진행된 이후에 Room 강제 생성
                tileType = 2;
            }
        }

        //Room 개수가 최종 개수에 도달하면 맵 생성 종료
        if (nowCrossCount + 1 == nowRoomCount) {
            Debug.Log("맵 생성이 성공적으로 종료되었습니다.");
            return;
        }

        //생성될 Tile의 베리에이션 결정
        int tileIndex = UnityEngine.Random.Range(0, sameTypeTileCount);

        //randomType 0: Hall / 1: Corner / 2: Room / 3: Cross
        if (tileType == 0) {
            //Hall 생성
                //Hall은 이전 TileDir에 따라 대응하는 1가지 밖에 생성하지 못하기 때문에 확정
            if (prevTileDir == 2) {
                if (!checkTile(2, nowPivot)) return;    //tile 생성 가능 체크
                GameObject hall = Instantiate(hallTile[0].Tile[tileIndex], tilePos, transform.rotation);//tile 생성
                pivotList.Add(nowPivot);                //pivotList에 추가한 pivot 추가
                addTileList(hall);                      //생성한 타일 tileList에 추가
                nowPivot.y -= tileSize.y * 2;           //pivot 위치 동기화
            }
            else if (prevTileDir == 8) {
                if (!checkTile(8, nowPivot)) return;
                GameObject hall = Instantiate(hallTile[0].Tile[tileIndex], tilePos, transform.rotation);
                pivotList.Add(nowPivot);
                addTileList(hall);
                nowPivot.y += tileSize.y * 2;
            }
            else if (prevTileDir == 4) {
                if (!checkTile(4, nowPivot)) return;
                GameObject hall = Instantiate(hallTile[1].Tile[tileIndex], tilePos, transform.rotation);
                pivotList.Add(nowPivot);
                addTileList(hall);
                nowPivot.x -= tileSize.x * 2;
            }
            else if (prevTileDir == 6) {
                if (!checkTile(6, nowPivot)) return;
                GameObject hall = Instantiate(hallTile[1].Tile[tileIndex], tilePos, transform.rotation);
                pivotList.Add(nowPivot);
                addTileList(hall);
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
                    GameObject corner = Instantiate(cornerTile[0].Tile[tileIndex], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    addTileList(corner);
                    nowPivot.x += tileSize.x * 2;
                    prevTileDir = 6;
                }
                else if (cornerRandom == 1) {                   
                    if (!checkTile(4, nowPivot)) return;
                    GameObject corner = Instantiate(cornerTile[1].Tile[tileIndex], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    addTileList(corner);
                    nowPivot.x -= tileSize.x * 2;
                    prevTileDir = 4;
                }
            }
            else if (prevTileDir == 8) {
                if (cornerRandom == 0) {                   
                    if (!checkTile(6, nowPivot)) return;
                    GameObject corner = Instantiate(cornerTile[2].Tile[tileIndex], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    addTileList(corner);
                    nowPivot.x += tileSize.x * 2;
                    prevTileDir = 6;
                }
                else if (cornerRandom == 1) {
                    if (!checkTile(4, nowPivot)) return;
                    GameObject corner = Instantiate(cornerTile[3].Tile[tileIndex], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    addTileList(corner);
                    nowPivot.x -= tileSize.x * 2;
                    prevTileDir = 4;
                }
            }
            else if (prevTileDir == 6) {
                if (cornerRandom == 0) {
                    if (!checkTile(8, nowPivot)) return;
                    GameObject corner = Instantiate(cornerTile[1].Tile[tileIndex], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    addTileList(corner);
                    nowPivot.y += tileSize.y * 2;
                    prevTileDir = 8;
                }
                else if (cornerRandom == 1) {
                    if (!checkTile(2, nowPivot)) return;
                    GameObject corner = Instantiate(cornerTile[3].Tile[tileIndex], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    addTileList(corner);
                    nowPivot.y -= tileSize.y * 2;
                    prevTileDir = 2;
                }
            }
            else if (prevTileDir == 4) {
                if (cornerRandom == 0) {
                    if (!checkTile(8, nowPivot)) return;
                    GameObject corner = Instantiate(cornerTile[0].Tile[tileIndex], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    addTileList(corner);
                    nowPivot.y += tileSize.y * 2;
                    prevTileDir = 8;
                }
                else if (cornerRandom == 1) {
                    if (!checkTile(2, nowPivot)) return;
                    GameObject corner = Instantiate(cornerTile[2].Tile[tileIndex], tilePos, transform.rotation);
                    pivotList.Add(nowPivot);
                    addTileList(corner);
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

            //Room 생성
            if (prevTileDir == 2) {
                GameObject room = Instantiate(roomTile[0].Tile[tileIndex], tilePos, transform.rotation);
                addTileList(room);
                }
            else if (prevTileDir == 4) {
                GameObject room = Instantiate(roomTile[1].Tile[tileIndex], tilePos, transform.rotation);
                addTileList(room);
                }
            else if (prevTileDir == 6) {
                GameObject room = Instantiate(roomTile[2].Tile[tileIndex], tilePos, transform.rotation);
                addTileList(room);
                }
            else if (prevTileDir == 8) {
                GameObject room = Instantiate(roomTile[3].Tile[tileIndex], tilePos, transform.rotation);
                addTileList(room);
                }

            //생성 완료되었으면 카운트 처리
            nowRoomCount++;
            //Room 생성 후에는 더 이상 해당 Pivot을 트래킹 할 필요가 없으므로, crossedPivotList에서 남은 Pivot을 생성해주러 이동
            Debug.Log("이번 갈림길 Pivot의 Room이 생성되었습니다.");
            if (crossedPivotList.Count > 0) {
                //이렇게 처리하려면 처리 완료 시에 역순으로 빼줘야 함
                nowPivot = crossedPivotList[crossedPivotCount - 1];
                prevTileDir = crossedPivotPrevDirList[crossedPivotCount - 1];
                //Pivot을 옮기면 데이터 삭제 처리
                crossedPivotList.RemoveAt(crossedPivotCount - 1);
                crossedPivotPrevDirList.RemoveAt(crossedPivotCount - 1);
                crossedPivotCount--;
                //갈림길이 종료되었으므로 이후에 진행된 타일 수를 초기화
                afterCrossedTileCount = 0;
            }
        }
        else if (tileType == 3) {
            if (prevTileDir == 2) {
                //방향 양 쪽 다 만들 수 있는지 먼저 체크
                if (!checkTile(4, nowPivot) || !checkTile(6, nowPivot)) {
                    //만약 안되면 생성을 못하니까, 다음 턴에 무조건 cross를 생성하는 로직을 넣어야 함
                    //이거 수정 필요
                    isCross = true;
                    return;
                }
                //4, 6 고정이니까 갈림길 하나 만들어주고
                GameObject cross = Instantiate(crossTile[0].Tile[tileIndex], tilePos, transform.rotation);
                pivotList.Add(nowPivot);
                addTileList(cross);

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
                    isCross = true;
                    return;
                }
                GameObject cross = Instantiate(crossTile[3].Tile[tileIndex], tilePos, transform.rotation);
                pivotList.Add(nowPivot);
                addTileList(cross);

                Vector2Int tempPivot = nowPivot;
                tempPivot.x -= tileSize.x * 2;
                crossedPivotList.Add(tempPivot);

                nowPivot.x += tileSize.x * 2;
                prevTileDir = 6;
                crossedPivotPrevDirList.Add(4);
            }
            else if (prevTileDir == 6) {
                if (!checkTile(2, nowPivot) || !checkTile(8, nowPivot)) {
                    isCross = true;
                    return;
                }
                GameObject cross = Instantiate(crossTile[2].Tile[tileIndex], tilePos, transform.rotation);
                pivotList.Add(nowPivot);
                addTileList(cross);

                Vector2Int tempPivot = nowPivot;
                tempPivot.y -= tileSize.y * 2;
                crossedPivotList.Add(tempPivot);

                nowPivot.y += tileSize.y * 2;
                prevTileDir = 8;
                crossedPivotPrevDirList.Add(2);
            }
            else if (prevTileDir == 4) {
                if (!checkTile(2, nowPivot) || !checkTile(8, nowPivot)) {
                    isCross = true;
                    return;
                }
                GameObject cross = Instantiate(crossTile[1].Tile[tileIndex], tilePos, transform.rotation);
                pivotList.Add(nowPivot);
                addTileList(cross);

                Vector2Int tempPivot = nowPivot;
                tempPivot.y -= tileSize.y * 2;
                crossedPivotList.Add(tempPivot);

                nowPivot.y += tileSize.y * 2;
                prevTileDir = 8;
                crossedPivotPrevDirList.Add(2);
            }
            crossedPivotCount++;
            nowCrossCount++;
        }
        
        tileTypeList.Add(tileType); //정상적으로 추가 되었을 때만 tileTypeList에 추가
        //crossedPivot이 잘못 저장되는 케이스가 있는 것 같다.
        
        //타일이 생성된 후에는 이번 갈림길에서 생성된 타일 수 추가 (이번에 생성한 타일이 Hall 및 Corner일 경우에만)
        //갈림길이 없을 때에는 핸들링하지 않고, 갈림길 종료 시에는 초기화 해주므로 별도 조건 없어도 무방해 보임
        if (tileType < 2) afterCrossedTileCount++;

        Debug.Log("nowPivot: (" + nowPivot.x + ", " + nowPivot.y + ")");
        Debug.Log("nowTileType:" + tileType);
        Debug.Log("현재 생성된 Tile의 개수:" + tileObjectList.Count);
        // Debug.Log(pivotList[pivotList.Count - 1].x + ", " + pivotList[pivotList.Count - 1].y);
    }

}
