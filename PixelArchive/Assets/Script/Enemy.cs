using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyType;
    public float speed;
    public float health;
    public float attackRange;
    public float curShotDelay;
    public float maxShotDelay;
    public GameObject Bullet;

    bool isLive = true;
    Rigidbody2D rigidB;
    Rigidbody2D target;
    SpriteRenderer spriteR;

    void Awake()
    {
        rigidB = GetComponent<Rigidbody2D>();
        spriteR = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        if (!isLive) return;

        float targetDis = Vector2.Distance(target.position, rigidB.position);

        if (targetDis < attackRange) {
            FireAndReload();
        }
        else {
            Vector2 dirVec = target.position - rigidB.position;
            Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
            rigidB.MovePosition(rigidB.position + nextVec);
            rigidB.velocity = Vector2.zero;
        }

    }

    void LateUpdate() {
         if (!isLive) return;

         spriteR.flipX = target.position.x < rigidB.position.x;
    }

    private void FireAndReload() {
        AutoFire();
        Reload();
    }
    private void AutoFire() {
        if (curShotDelay < maxShotDelay) return;

        if (enemyType == "Single") {
            GameObject bullet = Instantiate(Bullet, transform.position, transform.rotation);
            Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = target.position - rigidB.position;
            bulletRigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);
        }
        else if (enemyType == "3Burst") {
            for (int i = 0; i < 3; i++) {
                GameObject bullet = Instantiate(Bullet, transform.position, transform.rotation);
                Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
                Vector2 dirVec = target.position - rigidB.position;
                Vector2 randomVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));

                dirVec += randomVec;

                bulletRigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);
            }
            
        }

        curShotDelay = 0;
    }

    private void Reload() {
        curShotDelay += Time.deltaTime;
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

    private void Dead() {
        gameObject.SetActive(false);
    }

    private void OnEnable() {
        // target = GameManager.instance.player.GetComponent<Rigidbody2D>();
    }
}
