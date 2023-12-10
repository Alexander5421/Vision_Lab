using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { Start, Running, End }
    
    // [SerializeField] private UIManager uiManager;
    [SerializeField] private TrialRunner runner;
    [SerializeField] private int trialCount;
    
    private GameState currState;

    // Start is called before the first frame update
    void Start()
    {
        // start menu
        // waiting the button pressed;
        // currState = GameState.Start;
        // uiManager.ShowStartMenu();
    
        // start game
        GameStart();
        
    }
    void GameStart()
    {
        currState = GameState.Running;
        // uiManager.HideStartMenu();
        runner.InitializeTrials(trialCount);
        runner.finish += GameFinish;
    }
    private void GameFinish()
    {
        currState  = GameState.End;
        // uiManager.ShowEndMenu(runner.score);
        print($"GameFinish+ {runner.score}/{trialCount}");
    }

    // Update is called once per frame


    
}