using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    public GameConfig config;
    public Block block;

    private void OnEnable()
    {
        //UserInput.OnDownButtonPressed += DoAfterBlockPlacedInGrid;
        SignalService.OnBlockPlacedEvent += DoAfterBlockPlacedInGrid;
        SignalService.OnGameStateUpdated += OnGameStateUpdate;
    }

    private void OnDisable()
    {
        //UserInput.OnDownButtonPressed -= DoAfterBlockPlacedInGrid;

        SignalService.OnBlockPlacedEvent -= DoAfterBlockPlacedInGrid;
        SignalService.OnGameStateUpdated -= OnGameStateUpdate;
    }

    private void OnGameStateUpdate(GameState state)
    {
        if (state == GameState.Running)
        {
            ClearOldBlock();
            CreateNewBlock();
        }
    }

    //private void OnTestSignal(TestSignal obj)
    //{
    //    Debug.Log($"_signalTest_ {obj.GetType().Name} received by {this.GetType().Name} with value {obj.Value}");
    //}

    //private void OnButtonPressed(ButtonPressedSignal obj)
    //{
    //    Debug.Log($"_signalTest_ {obj.GetType().Name} received by {this.GetType().Name} with value {obj.Value}");
    //}

    private void DoAfterBlockPlacedInGrid()
    {
        if (MainManager.CurrentGameState == GameState.GameOver)
        {
            MainManager.instance.ShowGameOverScreen();
            return;
        }

        ClearOldBlock();
        CreateNewBlock();
    }

    private void CreateNewBlock()
    {
        var blockData = config.Blocks[Random.Range(0, config.Blocks.Length)];
        block.InitialiseBlock(blockData);
        block.RenderBlock();
        block.AdjustBoundPositions();
    }

    private void ClearOldBlock()
    {
        block.Clear();
    }
}