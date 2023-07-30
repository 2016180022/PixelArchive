using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public float speed;
    public GameManager gManager;

    Rigidbody2D rigid;
    Animator anim;
    float h;
    float v;
    bool isHorizontalMove;
    Vector3 dirVec;
    GameObject scanObj;
    
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //scanObj 검사
        if (Input.GetButtonDown("Jump") && scanObj != null) gManager.setText(scanObj);

        if (gManager.isDialogActive) return;

        // h, v 방향 움직임 설정
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        // 버튼 다운/업 정보 동기화
        bool hDown = Input.GetButtonDown("Horizontal");
        bool hUp = Input.GetButtonUp("Horizontal");
        bool vDown = Input.GetButtonDown("Vertical");
        bool vUp = Input.GetButtonUp("Vertical");

        //h, v 방향 bool 인자까지 동기화
        if (hDown) isHorizontalMove = true;
        else if (vDown) isHorizontalMove = false;
        else if (hUp || vUp) isHorizontalMove = h != 0;

        if (anim.GetInteger("horizonAxisRaw") != h) {
            anim.SetBool("isDirectionChanged", true);
            anim.SetInteger("horizonAxisRaw", (int)h);
            }
        else if (anim.GetInteger("vertAxisRaw") != v) {
            anim.SetBool("isDirectionChanged", true);         
            anim.SetInteger("vertAxisRaw", (int)v);
        }
        else anim.SetBool("isDirectionChanged", false);

        //방향 레이 동기화
        if (vDown && v == 1) dirVec = Vector3.up;
        else if (vDown && v == -1) dirVec = Vector3.down;
        else if (hDown && v == -1) dirVec = Vector3.left;
        else if (hDown && v == 1) dirVec = Vector3.right;

    }

    void FixedUpdate()
    {
        //h, v 움직임 따로 적용
        Vector2 moveVector = isHorizontalMove ? new Vector2(h, 0) : new Vector2(0, v);
        rigid.velocity = moveVector * speed;

        //ray 적용
        Debug.DrawRay(rigid.position, dirVec * 0.7f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 0.7f, LayerMask.GetMask("Obj"));

        if (rayHit.collider != null) scanObj = rayHit.collider.gameObject;
        else scanObj = null;
    }
}
