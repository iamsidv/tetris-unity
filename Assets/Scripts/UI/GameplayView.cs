using UnityEngine;

public class GameplayView : BaseView
{
    [SerializeField] private UnityEngine.UI.Text scoreLabel;
    [SerializeField] private UnityEngine.UI.Text titleLabel;

    public override void OnScreenEnter()
    {
        base.OnScreenEnter();
        SignalService.Subscribe<DisplayScoreSignal>(OnScoreUpdated);
    }

    public override void OnScreenExit()
    {
        base.OnScreenExit();

        SignalService.RemoveSignal<DisplayScoreSignal>(OnScoreUpdated);
    }

    public void DisplayScore(int currentScore)
    {
        scoreLabel.text = $"Score :{currentScore}";
    }

    public void SetTitle(string title)
    {
        titleLabel.text = title;
    }

    private void OnScoreUpdated(DisplayScoreSignal signal) => DisplayScore(signal.Value);
}