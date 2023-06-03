using UnityEngine;

public class GameplayView : BaseView
{
    [SerializeField] private UnityEngine.UI.Text scoreLabel;
    [SerializeField] private UnityEngine.UI.Text titleLabel;

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

    public void DisplayScore(int currentScore)
    {
        scoreLabel.text = $"Score :{currentScore}";
    }

    public void SetTitle(string title)
    {
        titleLabel.text = title;
    }
}