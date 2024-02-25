using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigManager : MonoBehaviour
{
    RectTransform rect;

    public GameManager gManager;
    
    void Awake() {
        rect = GetComponent<RectTransform>();
    }

    public void Show() {
        rect.localScale = Vector3.one;
        gManager.pauseGame();
    }

    public void Hide() {
        rect.localScale = Vector3.zero;
        gManager.resumeGame();
    }
}
