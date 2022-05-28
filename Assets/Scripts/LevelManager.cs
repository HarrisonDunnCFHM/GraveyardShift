using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    //config parameters
    [SerializeField] int requiredBonesToWin = 10;

    //cached references
    DirtManager dirtManager;
    bool levelWon = false;
    bool levelLost = false;
    
    // Start is called before the first frame update
    void Start()
    {
        dirtManager = FindObjectOfType<DirtManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForVictory();
        ProcessVictory();
    }

    private void ProcessVictory()
    {
        if (levelLost) { return; }
        {

        }
    }

    private void CheckForVictory()
    {
        if(dirtManager.boneCount >= requiredBonesToWin)
        {
            levelWon = true;
        }
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }
}
