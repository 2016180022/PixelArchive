using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float damage;
    public int piercingCount;

    private Vector2Int targetPos;
    private bool bombCreate;
    private bool nowExploding;
    private float bombTimeLimit;

    Rigidbody2D rigid;
    BoxCollider2D bCollider;
    SpriteRenderer sprite;

    public void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        bCollider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        bombCreate = false;

        //안전 코드
        Invoke("Destroy", 10);
    }

    void FixedUpdate() {
        if (bombCreate == true) {
            checkBomb(targetPos);
            bombTimeLimit += Time.deltaTime;
        }
        if (nowExploding == true) {
            bombTimeLimit += Time.deltaTime;
            if (bombTimeLimit > 10) {
                nowExploding = false;
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Border") || other.CompareTag("BorderObj")) {
            if (gameObject.CompareTag("Bomb")) explodeBomb();
            else { Destroy(gameObject); }
        }
        //동일 객체에 대해서 1회만 충돌 검사를 시행하는 방법이 아니면 PC 기능은 다른 방법으로 구현해볼 생각을 해봐야 할듯
        if (gameObject.CompareTag("Bullet") || gameObject.CompareTag("Bomb")) {
            if (other.CompareTag("Enemy")) {
                Destroy(gameObject);
            }
            else if (other.CompareTag("Obj")) {
                Destroy(gameObject);
                ObjectData obj = other.GetComponent<ObjectData>();
                if (obj.isDropable) obj.dropItem();
                Destroy(other.gameObject);
            }
        }
    }

    public void getTargetPos(Vector2 targetPos) {
        this.targetPos = Vector2Int.RoundToInt(targetPos);
        bombCreate = true;
    }

    void checkBomb(Vector2Int targetPos) {
        Vector2Int nowPos = new Vector2Int();
        nowPos.x = (int)gameObject.transform.position.x;
        nowPos.y = (int)gameObject.transform.position.y;

        if (nowPos == targetPos) {
            explodeBomb();
        }
        else if (bombTimeLimit > 3) explodeBomb();
    }
    void explodeBomb() {
        rigid.velocity = Vector2.zero;
        bCollider.enabled = true;
        sprite.enabled = false;
        bombCreate = false;
        nowExploding = true;
        Debug.Log("Bomb Exploded");
    }

    void Destroy() {
        Destroy(gameObject);
    }
}
