using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrialRunner:MonoBehaviour
{
    public enum Direction
    {
        left,
        right
    }
    public Stimulus targetPrefab;
    public Stimulus distractionPrefab;
    public Transform[] candidatePosition;
    public int score;
    public int currRunIndex = 0;
    public int totalRunCount = 0;
    public Direction correctDirection = Direction.left;
    public Action finish;
    public List<Stimulus> stimuliList = new List<Stimulus>();
    private UserInput userInput;

    public void Awake()
    {
        this.enabled = false;
    }

    public void InitializeTrials(int totalCount)
    {
        totalRunCount = totalCount;
        userInput = new UserInput();
        this.enabled = true;
        userInput.gameplay.Left.performed += ctx =>
        {
            ReceiveInput(false);
        };
        
        userInput.gameplay.Right.performed += ctx =>
        {
            ReceiveInput(true);
        };
        // first run
        NewRun();
    }

    private void OnEnable()
    {
        userInput.Enable();
    }
    private void OnDisable()
    {
        userInput.Disable();
    }

    private void NewRun()
    {
        if (currRunIndex < totalRunCount)
        {
            StartNewRun();
        }
        else
        {
            // finish
            finish.Invoke();
        }
    }
    private void ReceiveInput(bool isRight)
    {
        if (isRight)
        {
            if (correctDirection == Direction.right)
            {
                score++;
            }
        }
        else
        {
            if (correctDirection == Direction.left)
            {
                score++;
            }
        }
        currRunIndex++;
        NewRun();
        
    }

    private void StartNewRun()
    {
        // if stimuliList is not empty, destroy all
        if (stimuliList.Count > 0)
        {
            foreach (var stimulus in stimuliList)
            {
                Destroy(stimulus.gameObject);
            }
            stimuliList.Clear();
        }
        int totalStimuli = Random.Range(candidatePosition.Length-2,candidatePosition.Length+1);
        // from candidatePosition array, randomly select totalStimuli and create a new array
        
        Transform[] stimuliPositions=candidatePosition.ToList().OrderBy(arg => Guid.NewGuid()).Take(totalStimuli).ToArray();
        
        
        int targetLocation= Random.Range(0,totalStimuli);
        // other stimuli are distraction
        for (int i = 0; i < totalStimuli; i++)
        {
            if (i == targetLocation)
            {
                // create target
                // as the child of stimuliPositions[i]
                Stimulus target = Instantiate(targetPrefab,stimuliPositions[i]);
                stimuliList.Add(target);
                // set target direction randomly and set correctDirection
                bool isRight = Random.Range(0, 2) == 1;
                target.SetSphere(isRight);
                correctDirection = isRight ? Direction.right : Direction.left;
            }
            else
            {
                // create distraction
                Stimulus distraction = Instantiate(distractionPrefab, stimuliPositions[i]);
                stimuliList.Add(distraction);
                distraction.SetSphere(correctDirection == Direction.right);
            }
        }
        
        
    }
}