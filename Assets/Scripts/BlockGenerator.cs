using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    public GameConfig config;
    public Block block;

    private void OnEnable()
    {
        SignalService.Subscribe<BlockPlacedSignal>(DoAfterBlockPlacedInGrid);
        SignalService.Subscribe<GameStateUpdateSignal>(OnGameStateUpdate);
    }

    private void OnDisable()
    {
        SignalService.RemoveSignal<BlockPlacedSignal>(DoAfterBlockPlacedInGrid);
        SignalService.RemoveSignal<GameStateUpdateSignal>(OnGameStateUpdate);
    }

    #region Event Listeners

    private void DoAfterBlockPlacedInGrid(BlockPlacedSignal signal)
    {
        if (MainManager.CurrentGameState == GameState.GameOver)
        {
            return;
        }

        ClearOldBlock();
        CreateNewBlock();
    }

    private void OnGameStateUpdate(GameStateUpdateSignal signal)
    {
        if (signal.Value == GameState.Running)
        {
            ClearOldBlock();
            CreateNewBlock();
        }
    }

    #endregion Event Listeners

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