using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CameraController : MonoBehaviour
{
    public Rigidbody2D target;
    public PlayerAction pPlayer;
    PixelPerfectCamera pCamera;
    float smoothing = 0.2f;
    float cameraSpeed = 50.0f;

    void Awake() {
        pCamera = GetComponent<PixelPerfectCamera>();
    }
    void FixedUpdate() {
        if (!pPlayer.isCameraMode) {
            setResolution(1);
            Vector3 targetPos = new Vector3(target.position.x, target.position.y, -5);
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);
        }
        else {
            setResolution(5);
            Vector2 moveVector = pPlayer.inputVec.normalized * cameraSpeed * Time.fixedDeltaTime;
            Vector3 tempVec = new Vector3(transform.position.x + moveVector.x, transform.position.y + moveVector.y, -5);
            transform.position = tempVec;
        }
    }

    void setResolution(int zoom) {
        pCamera.refResolutionX = 320 * zoom;
        pCamera.refResolutionY = 180 * zoom;
    }
}
