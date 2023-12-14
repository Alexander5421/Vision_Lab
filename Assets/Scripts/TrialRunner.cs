﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
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
    public Transform[] candidatePosition2D;
    public int score;
    public int currRunIndex = 0;
    public int totalRunCount = 0;
    public Direction correctDirection = Direction.left;
    public Action finish;
    public List<Stimulus> stimuliList = new List<Stimulus>();
    private UserInput userInput;
    private Stopwatch stopwatch;
    private EntryInfo entryInfo;
    private string trialType;
    private float cooldown;
    [SerializeField]
    private CSVLogger csvLogger;
    

    public void Awake()
    {
        this.enabled = false; 
        // create a new file
        string fileName = $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}";
        csvLogger.Initialize(fileName);
        InitializeInputs();
    }

    public void InitializeTrials(int totalCount,string trialType)
    {
       
        this.enabled = true;
        score = 0;
        currRunIndex = 0;
        totalRunCount = totalCount;
        this.trialType = trialType;
        // first run
        NewRun();
    }

    private void InitializeInputs()
    {
        userInput = new UserInput();
        userInput.gameplay.Left.performed += ctx =>
        {
            print("input received");
            ReceiveInput(false);
        };

        userInput.gameplay.Right.performed += ctx =>
        {
            print("input received");
            ReceiveInput(true);
        };
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
            ClearStimuli();
            // disable this script
            this.enabled = false;
        }
    }
    private void ReceiveInput(bool isRight)
    {
        stopwatch.Stop();
        entryInfo.respkeys = isRight ? "right" : "left";
        entryInfo.correctness = isRight == (correctDirection == Direction.right) ? "correct" : "incorrect";
        entryInfo.reactionTime = stopwatch.ElapsedMilliseconds.ToString();
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
        // score += ((correctDirection==Direction.right)==isRight)?1:0;
        currRunIndex++;
        csvLogger.AddDataWithUserInfo(entryInfo.Serialize());
        ClearStimuli();
        NewRun();


    }


    private void StartNewRun()
    {
        ClearStimuli();
        // first decide 2D or 3D
        bool is3D = Random.Range(0, 2) == 1;
        var realCandidatePosition = is3D ? candidatePosition : candidatePosition2D;
        int totalStimuli = Random.Range(realCandidatePosition.Length-2,realCandidatePosition.Length+1);
        // from candidatePosition array, randomly select totalStimuli and create a new array
        
        Transform[] stimuliPositions=realCandidatePosition.ToList().OrderBy(arg => Guid.NewGuid()).Take(totalStimuli).ToArray();
        
        
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
                distraction.SetSphere(Random.Range(0, 2) == 1);
            }
        }
        
        entryInfo = new EntryInfo();
        entryInfo.stimuli_locs = stimuliPositions.Select(arg => arg.GetSiblingIndex()).ToList();
        entryInfo.trialType = trialType;
        entryInfo.condition = is3D ? 2 : 1;

        stopwatch = new Stopwatch();
        stopwatch.Start();
    }


    private void ClearStimuli()
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
    }
}