using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjectData : MonoBehaviour
{
    public int id;
    public bool isNpc;
    public bool isDropable = false;

    public GameObject[] Coin;
    public GameObject Heart;
    public GameObject Skill;

    public void dropItem() {
        // Debug.Log("아이템을 드랍합니다.");
        //skill coin heart
        int rand = Random.Range(0, 10);
        //dropCoin
        if (rand < 1) {
            GameObject item = Instantiate(Coin[2], transform.position, transform.rotation);
        }
        else if (rand < 2) {
            GameObject item = Instantiate(Coin[1], transform.position, transform.rotation);
        }
        else if (rand < 5) {
            GameObject item = Instantiate(Coin[0], transform.position, transform.rotation);
        }
        //dropHeart
        else if (rand < 7) {
            GameObject item = Instantiate(Heart, transform.position, transform.rotation);
        }
        //dropSkill
        else if (rand < 10) {
            GameObject item = Instantiate(Skill, transform.position, transform.rotation);
        }
    }

    public float Choose (float[] probs) {
        float total = 0;
        foreach(float elem in probs) {
            total += elem;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++) {
            if (randomPoint < probs[i]) {
                return i;
            }
            else randomPoint -= probs[i];
        }
    return probs.Length - 1;
    }
    
}
