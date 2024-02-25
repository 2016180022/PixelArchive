using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using System.Linq;
using Unity.Mathematics;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerAction pPlayer;
    public ConfigManager cManager;
    public TileManager tManager;

    public bool isLive;

    private void Awake() {
        instance = this;
        isLive = true;
    }

    public void callConfigUI() {
        cManager.Show();
    }

    public void createTile() {
        if (tManager.isEndMapCreate) return;
        
        tManager.createTile();

        if (tManager.isEndMapCreate) tManager.executeTile();
    }

    public void deleteTile() {
        if (!tManager.isEndMapCreate) return;

        tManager.deleteAllTile();
        pPlayer.resetPosition();
    }

    public void pauseGame() {
        isLive = false;
        Time.timeScale = 0;
    }

    public void resumeGame() {
        isLive = true;
        Time.timeScale = 1;
    }
}