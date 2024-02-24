using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    public static PlayerAction player;

    public int coin = 0;
    public int health = 1;
    public int maxHealth = 3;
    
    public Vector2 inputVec;
    public float speed;
    public float dashSpeed;
    //객체 생성해서 자동으로 할당해주는 방식 안되는 경우가 있어서, 별도로 넣어주기로 함
    public TileManager tManager;
    public GameObject Bullet;
    public GameObject Granade;

    //skill variable
    public GameObject playerShield;
    private bool isActiveShield = false;
    private float shieldActiveTime;
    private bool isActiveDash = false;
    private float dashActiveTime;
    public int[] skillSlot;
    
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
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

        if (isActiveDash) {
            Vector2 moveVector = inputVec.normalized * dashSpeed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + moveVector);
            dashActiveTime += Time.fixedDeltaTime;
            if (dashActiveTime >= 0.3) {
                isActiveDash = false;
            }
        }
        else {
            Vector2 moveVector = inputVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + moveVector);
        }
    
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
        tManager.executeTile();
    }

    void OnBack() {
        tManager.deleteAllTile();
        //타일 삭제 시 Player 0,0으로 돌아가는 기능 추가 필요
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Item") {
            Item item = other.GetComponent<Item>();
            switch(item.type) {
                case Item.itemType.Coin:
                if (coin > 999) coin = 999;
                else {
                    coin += item.value;
                }
                // Debug.Log("Coin " + item.value + "개 획득!");
                break;

                case Item.itemType.Heart:
                if (health < maxHealth) health++;
                // Debug.Log("Hp " + item.value + "회복!");
                break;

                case Item.itemType.Skill:
                getSkill(item.value);
                break;
            }
            Destroy(other.gameObject);
        }
        //Bullet.cs에서 처리해주던 충돌 처리를 Player로 이관
        else if (other.tag == "EnemyBullet") {
            //피격 처리
            // Debug.Log("피격");
            Destroy(other.gameObject);
            if (health > 0) health--;
            //체력 0일 때 종료처리 필요
        }
    }

    void getSkill(int skillIndex) {
        if (skillSlot[0] == 0) skillSlot[0] = skillIndex;
        else if (skillSlot[1] == 0) skillSlot[1] = skillIndex;
        else if (skillSlot[2] == 0) skillSlot[2] = skillIndex;
        // Debug.Log("1번 슬롯: " + skillSlot[0]);
        // Debug.Log("2번 슬롯: " + skillSlot[1]);
        // Debug.Log("3번 슬롯: " + skillSlot[2]);
    }

    void useSkill(int skillIndex) {
        if (skillIndex == 1) {skillFiveShot();}
        else if (skillIndex == 2) {skillShield();}
        else if (skillIndex == 3) skillGranade();
        else if (skillIndex == 4) skillDash();
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

    void skillGranade() {
        //Granade 프리팹 생성(Collider 없는 리소스)
        //Granade에 물리값까지 넣어주고 끝
        GameObject granade = Instantiate(Granade, transform.position, transform.rotation);
        Rigidbody2D granadeRigid = granade.GetComponent<Rigidbody2D>();
        Bullet bGranade = granade.GetComponent<Bullet>();
        Vector2 mousePos = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 granadeDir;
        granadeDir.x = mousePos.x - rigid.position.x;
        granadeDir.y = mousePos.y - rigid.position.y;

        float angle = Mathf.Atan2(mousePos.y - granade.transform.position.y, mousePos.x - granade.transform.position.x) * Mathf.Rad2Deg;
        granade.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        // Debug.Log(angle);

        granadeRigid.AddForce(granadeDir.normalized * 5, ForceMode2D.Impulse);
        bGranade.getTargetPos(mousePos);
    }

    void skillDash() {
        if (!isActiveDash) {
            isActiveDash = true;
            dashActiveTime = 0;
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