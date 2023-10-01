using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health = 1;
    public Rigidbody2D target;

    bool isLive = true;
    Rigidbody2D rigidB;
    SpriteRenderer spriteR;
    void Awake()
    {
        rigidB = GetComponent<Rigidbody2D>();
        spriteR = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate() {

        if (!isLive) return;

        Vector2 dirVec = target.position - rigidB.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigidB.MovePosition(rigidB.position + nextVec);
        rigidB.velocity = Vector2.zero;
    }

    void LateUpdate() {
         if (!isLive) return;

         spriteR.flipX = target.position.x < rigidB.position.x;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Bullet")) return;

        health -= other.GetComponent<Bullet>().damage;
        if (health > 0) {
            //Live
        }
        else {
            Dead();
        }
    }

    void Dead() {
        gameObject.SetActive(false);
    }
}
