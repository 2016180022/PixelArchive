using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum itemType {Skill, Coin, Heart};
    public itemType type;
    public int value;

    Vector3 itemPos;
    private float itemDelta = 0.2f;
    private int itemSpeed = 2;

    void Start() {
        itemPos = transform.position;
        if (type == itemType.Skill) {
            value = Random.Range(1, 4);
        }
    }
    void Update() {
        Vector3 temp = itemPos;
        temp.y += itemDelta * Mathf.Sin(Time.time * itemSpeed);
        transform.position = temp;
        // Debug.Log(Mathf.Sin(Time.deltaTime * itemSpeed));
    }
}
