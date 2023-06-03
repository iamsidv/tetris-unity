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
            MainManager.Instance.StartGame();
    }

    public override void OnScreenExit()
    {
        base.OnScreenExit();

        SignalService.OnSpaceBarPressedEvent -= OnSpaceBarPressedEvent;
    }
}