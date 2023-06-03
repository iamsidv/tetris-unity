using System.Collections;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public int currentScore = 0;

    [SerializeField] private GameState gameState;
    [SerializeField] private float newGameStartDelay = 2f;

    public static GameState CurrentGameState { get; private set; }

    public void SetGameState(GameState state)
    {
        CurrentGameState = state;
        gameState = state;
        SignalService.TriggerUpdateGameState(state);
    }

    private void Start()
    {
        SetGameReadyState();
    }

    private static void SetGameReadyState()
    {
        instance.SetGameState(GameState.Ready);
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

public enum GameState
{
    Ready,
    Running,
    GameOver
}