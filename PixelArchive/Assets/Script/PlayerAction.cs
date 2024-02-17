using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public GameManager gManager;
    public TileManager tManager;
    public GameObject Bullet;

    //skill variable
    public GameObject playerShield;
    public bool isActiveShield = false;
    float shieldActiveTime;
    public int[] skillSlot;
    
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    GameObject scanObj;
    Camera playerCamera;
    
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        playerCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update() {
        //scanObj 검사
        // if (Input.GetButtonDown("Jump") && scanObj != null) gManager.setText(scanObj);

        if (gManager.isDialogActive) return;

        /*
        //방향 레이 동기화
        if (vDown && v == 1) dirVec = Vector3.up;
        else if (vDown && v == -1) dirVec = Vector3.down;
        else if (hDown && v == -1) dirVec = Vector3.left;
        else if (hDown && v == 1) dirVec = Vector3.right;
        */

    }

    void FixedUpdate() {
        //인풋시스템 리워크로 주석 처리
        /*
        //h, v 움직임 따로 적용
        Vector2 moveVector = isHorizontalMove ? new Vector2(h, 0) : new Vector2(0, v);
        rigid.velocity = moveVector * speed;

        //ray 적용
        Debug.DrawRay(rigid.position, dirVec * 0.7f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 0.7f, LayerMask.GetMask("Obj"));

        if (rayHit.collider != null) scanObj = rayHit.collider.gameObject;
        else scanObj = null;
        */
        Vector2 moveVector = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + moveVector);
        
        if (isActiveShield) {
            shieldActiveTime += Time.fixedDeltaTime;
            playerShield.transform.position = rigid.transform.position;

            if (shieldActiveTime >= 5.0f) {
                playerShield.SetActive(false);
                isActiveShield = false;
            }
        }
    }

    void LateUpdate() {

        anim.SetFloat("Speed", inputVec.magnitude);

        if (inputVec.x != 0) {
            spriter.flipX = inputVec.x < 0;
        }
    }

    //Renewal Input System
    void OnMove(InputValue value) {
        inputVec = value.Get<Vector2>();
    }

    void OnFire() {
        tManager.executeTile();
        GameObject bullet = Instantiate(Bullet, transform.position, transform.rotation);
        Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
        Vector2 mousePos = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 bulletDir;
        bulletDir.x = mousePos.x - rigid.position.x;
        bulletDir.y = mousePos.y - rigid.position.y;

        float angle = Mathf.Atan2(mousePos.y - bullet.transform.position.y, mousePos.x - bullet.transform.position.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        // Debug.Log(angle);

        bulletRigid.AddForce(bulletDir.normalized * 10, ForceMode2D.Impulse);
    }

    void OnJump() {
        // Debug.Log("Jump");
        tManager.createTile();
    }

    void OnDash() {
        tManager.deleteAllTile();
        //타일 삭제 시 플레이어도 다시 0,0으로 보내주는 기능 추가해야 함
    }

    void OnBack() {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Item") {
            Item item = other.GetComponent<Item>();
            switch(item.type) {
                case Item.itemType.Coin:
                Debug.Log("Coin " + item.value + "개 획득!");
                break;
                case Item.itemType.Heart:
                Debug.Log("Hp " + item.value + "회복!");
                break;
                case Item.itemType.Skill:
                // Debug.Log("스킬은 아직 미구현입니다 ㅜ");
                getSkill(item.value);
                break;
            }
            Destroy(other.gameObject);
        }
        //Bullet.cs에서 처리해주던 충돌 처리를 Player로 이관
        else if (other.tag == "EnemyBullet") {
            //피격 처리
            Debug.Log("피격");
            Destroy(other.gameObject);
        }
    }

    void getSkill(int skillIndex) {
        if (skillSlot[0] == 0) skillSlot[0] = skillIndex;
        else if (skillSlot[1] == 0) skillSlot[1] = skillIndex;
        else if (skillSlot[2] == 0) skillSlot[2] = skillIndex;
        Debug.Log("1번 슬롯: " + skillSlot[0]);
        Debug.Log("2번 슬롯: " + skillSlot[1]);
        Debug.Log("3번 슬롯: " + skillSlot[2]);
    }

    void useSkill(int skillIndex) {
        if (skillIndex == 1) {skillFiveShot();}
        else if (skillIndex == 2) {skillShield();}
    }

    void skillFiveShot() {
        OnFire();
        Invoke("OnFire", 0.1f);
        Invoke("OnFire", 0.2f);
        Invoke("OnFire", 0.3f);
        Invoke("OnFire", 0.4f);
    }

    void skillShield() {
        if (!isActiveShield) {
            isActiveShield = true;
            playerShield.SetActive(true);
            shieldActiveTime = 0;
        }
    }

    void OnSkill1() {
        if (skillSlot[0] == 0) return;
        useSkill(skillSlot[0]);
        skillSlot[0] = 0;
    }

    void OnSkill2() {
        if (skillSlot[1] == 0) return;
        useSkill(skillSlot[1]);
        skillSlot[1] = 0;
    }

    void OnSkill3() {
        if (skillSlot[2] == 0) return;
        useSkill(skillSlot[2]);
        skillSlot[2] = 0;
    }
}