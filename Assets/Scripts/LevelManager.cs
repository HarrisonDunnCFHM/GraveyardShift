using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    //config parameters
    [SerializeField] public int requiredBonesToWin = 10;
    [SerializeField] GameObject levelCompleteBox;
    [SerializeField] GameObject levelLostBox;

    //cached references
    DirtManager dirtManager;
    bool levelWon = false;
    bool levelLost = false;
    public bool outOfMoves = false;
    
    // Start is called before the first frame update
    void Start()
    {
        dirtManager = FindObjectOfType<DirtManager>();
        if (levelCompleteBox != null)
        {
            levelCompleteBox.SetActive(false);
        }
        if (levelLostBox != null)
        {
            levelLostBox.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckForVictory();
        //CheckForLoss();
    }

   

    private void CheckForVictory()
    {
        if(dirtManager == null) { return; }
        if(dirtManager.boneCount >= requiredBonesToWin && !levelLost)
        {
            levelWon = true;
            levelCompleteBox.SetActive(true);
        }
    }

    private void CheckForLoss()
    {
        if (dirtManager == null) { return; }
        if (outOfMoves && !levelWon)
        {
            levelLost = true;
            levelLostBox.SetActive(true);
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

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
