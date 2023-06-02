using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayView : BaseView
{
    [SerializeField] private UnityEngine.UI.Text scoreLabel;

    public override void OnScreenEnter()
    {
        base.OnScreenEnter();
        SignalService.OnScoreUpdated += DisplayScore;
    }

    public override void OnScreenExit()
    {
        base.OnScreenExit();

        SignalService.OnScoreUpdated -= DisplayScore;
    }

    internal void DisplayScore(int currentScore)
    {
        scoreLabel.text = $"Score :{currentScore}";
    }
}
