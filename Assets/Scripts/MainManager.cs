using System.Collections;
using UnityEngine;

public enum GameState
{
    Ready,
    Running,
    GameOver
}

public class MainManager : MonoBehaviour
{
    public static GameState CurrentGameState { get; private set; }

    [SerializeField] private GameState gameState;
    [SerializeField] private float newGameStartDelay = 2f;

    private int currentScore = 0;

    private void Start()
    {
        SetGameReadyState();
    }

    private void OnEnable()
    {
        SignalService.Subscribe<UpdateScoreSignal>(AddScore);
        SignalService.Subscribe<GameStateUpdateSignal>(SetGameState);
    }

    private void OnDisable()
    {
        SignalService.RemoveSignal<UpdateScoreSignal>(AddScore);
        SignalService.RemoveSignal<GameStateUpdateSignal>(SetGameState);
    }

    public void SetGameState(GameStateUpdateSignal signal)
    {
        CurrentGameState = gameState = signal.Value;

        switch (gameState)
        {
            case GameState.Ready:
                SetGameReadyState();
                break;
            case GameState.Running:
                StartGame();
                break;
            case GameState.GameOver:
                ShowGameOverScreen();
                break;

            default:
                break;
        }
    }

    private void SetGameReadyState()
    {
        MenuManager.ShowMenu<MainMenuView>();
        MenuManager.HideMenu<GameplayView>();
        gameState = CurrentGameState = GameState.Ready;
    }

    private void StartGame()
    {
        currentScore = 0;
        MenuManager.HideMenu<MainMenuView>();
        var menu = MenuManager.ShowMenu<GameplayView>();
        menu.DisplayScore(currentScore);
        menu.SetTitle(string.Empty);
    }

    private void ShowGameOverScreen()
    {
        MenuManager.HideMenu<MainMenuView>();
        var menu = MenuManager.ShowMenu<GameplayView>();
        menu.DisplayScore(currentScore);
        menu.SetTitle("Gameover");

        StartCoroutine(DelayedCall(newGameStartDelay));
    }

    private IEnumerator DelayedCall(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetGameReadyState();
    }

    private void AddScore(UpdateScoreSignal signal)
    {
        currentScore += signal.Value;
        SignalService.Publish(new DisplayScoreSignal { Value = currentScore });
    }
}