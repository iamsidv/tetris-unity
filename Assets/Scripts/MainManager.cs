using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    

    public int currentScore = 0;

    public GameState CurrentGameState { get; private set; }

    public void SetGameState(GameState state)
    {
        CurrentGameState = state;
        //SignalService.OnGameStateUpdated
    }

    private void Start()
    {
        SetGameReadyState();
    }

    private void SetGameReadyState()
    {
        SetGameState(GameState.Ready);
        MenuManager.ShowMenu<MainMenuView>();
        MenuManager.HideMenu<GameplayView>();
    }

    private void StartGame()
    {
        SetGameState(GameState.Running);
        currentScore = 0;
        MenuManager.HideMenu<MainMenuView>();
        var menu = MenuManager.ShowMenu<GameplayView>();
        menu.DisplayScore(currentScore);
    }
}


public enum GameState
{
    Ready,
    Running,
    GameOver
}