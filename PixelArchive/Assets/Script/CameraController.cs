using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Rigidbody2D target;
    float smoothing = 0.2f;
    private void FixedUpdate() {
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, -5);
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);
    }
}
