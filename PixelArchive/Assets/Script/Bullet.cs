using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float damage;
    public int piercingCount;

    public void Init() {
        
        //안전 코드
        Invoke("Destroy", 10);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Border") || other.CompareTag("BorderObj")) {
            Destroy(gameObject);
        }
        //동일 객체에 대해서 1회만 충돌 검사를 시행하는 방법이 아니면 PC 기능은 다른 방법으로 구현해볼 생각을 해봐야 할듯
        if (this.CompareTag("Bullet") && other.CompareTag("Enemy")) {
            if (this.piercingCount <= 1) Destroy(gameObject);
            else this.piercingCount -= 1;
        }
        else if (this.CompareTag("EnemyBullet") && other.CompareTag("Player")) {
            Destroy(gameObject);
        }
    }

    void Destroy() {
        Destroy(gameObject);
    }
}
