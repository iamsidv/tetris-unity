using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuView : BaseView
{
    public override void OnScreenEnter()
    {
        base.OnScreenEnter();

        SignalService.OnSpaceBarPressedEvent += OnSpaceBarPressedEvent;
    }

    private void OnSpaceBarPressedEvent()
    {

        if (MainManager.CurrentGameState == GameState.Ready)
            MainManager.instance.StartGame();
    }

    public override void OnScreenExit()
    {
        base.OnScreenExit();

        SignalService.OnSpaceBarPressedEvent -= OnSpaceBarPressedEvent;
    }
}
