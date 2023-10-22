using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public GameManager gManager;
    public GameObject Bullet;
    //임시로 true 설정
    public bool isBattle = true;

    Rigidbody2D rigid;
    Animator anim;
    GameObject scanObj;
    Camera playerCamera;
    
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update() {
        //scanObj 검사
        if (Input.GetButtonDown("Jump") && scanObj != null) gManager.setText(scanObj);

        if (gManager.isDialogActive) return;

        moveUpdate();

        /*
        //방향 레이 동기화
        if (vDown && v == 1) dirVec = Vector3.up;
        else if (vDown && v == -1) dirVec = Vector3.down;
        else if (hDown && v == -1) dirVec = Vector3.left;
        else if (hDown && v == 1) dirVec = Vector3.right;
        */

    }

    private void moveUpdate() {
        if (inputVec.x != 0) {
            if (inputVec.x != anim.GetInteger("horizonAxisRaw")) anim.SetBool("isDirectionChanged", true);
            else anim.SetBool("isDirectionChanged", false);
            anim.SetInteger("horizonAxisRaw", (int)inputVec.x);
        }
        else anim.SetInteger("horizonAxisRaw", 0);

        if (inputVec.y != 0)  {
            if (inputVec.y != anim.GetInteger("vertAxisRaw")) anim.SetBool("isDirectionChanged", true);
            else anim.SetBool("isDirectionChanged", false);
            anim.SetInteger("vertAxisRaw", (int)inputVec.y);
        }
        else anim.SetInteger("vertAxisRaw", 0);
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
        Vector2 moveVector = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + moveVector);
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

        //불릿 회전
        // Vector3 tempVec = new Vector3(mousePos.x, mousePos.y, bullet.transform.position.z);
        // float bulletRotation = Quaternion.FromToRotation(bullet.transform.position, tempVec).eulerAngles.z;
        // bullet.transform.Rotate(0, 0, bulletRotation, Space.Self);
        float angle = Mathf.Atan2(mousePos.y - bullet.transform.position.y, mousePos.x - bullet.transform.position.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        Debug.Log(angle);

        bulletRigid.AddForce(bulletDir.normalized * 10, ForceMode2D.Impulse);
    }

}
