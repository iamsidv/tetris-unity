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
    public static MainManager Instance { get; private set; }
    public static GameState CurrentGameState { get; private set; }

    [SerializeField] private GameState gameState;
    [SerializeField] private float newGameStartDelay = 2f;

    private int currentScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SetGameReadyState();
    }

    public void SetGameState(GameState state)
    {
        CurrentGameState = state;
        gameState = state;
        SignalService.TriggerUpdateGameState(state);
    }

    private static void SetGameReadyState()
    {
        Instance.SetGameState(GameState.Ready);
        MenuManager.ShowMenu<MainMenuView>();
        MenuManager.HideMenu<GameplayView>();
    }

    public void StartGame()
    {
        currentScore = 0;
        MenuManager.HideMenu<MainMenuView>();
        var menu = MenuManager.ShowMenu<GameplayView>();
        menu.DisplayScore(currentScore);
        menu.SetTitle(string.Empty);
        SetGameState(GameState.Running);
    }

    public void ShowGameOverScreen()
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

    public void AddScore(int score)
    {
        currentScore += score;
        SignalService.TriggerUpdateScore(currentScore);
    }
}