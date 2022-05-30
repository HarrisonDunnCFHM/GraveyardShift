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
    SpecialPieceManager dirtManager;
    bool levelWon = false;
    public bool levelLost = false;
    public bool outOfMoves = false;
    AudioManager audioManager;
    
    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.ResetSliders();
        dirtManager = FindObjectOfType<SpecialPieceManager>();
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

    public void LevelLost()
    {
        if (dirtManager == null) { return; }
        levelLost = true;
        levelLostBox.SetActive(true);
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
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 2);
    }

    public void LoadHowTo()
    {
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
