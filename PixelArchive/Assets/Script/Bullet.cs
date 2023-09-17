using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public int piercingCount;

    public void Init(float speed, int piercingCount) {
        this.speed = speed;
        this.piercingCount = piercingCount;
        
        //안전 코드
        Invoke("Destroy", 10);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Border") || other.CompareTag("BorderObj")) {
            Destroy(gameObject);
        }
    }

    void Destroy() {
        Destroy(gameObject);
    }
}
