using Newtonsoft.Json;
using UnityEngine;

public class Block : MonoBehaviour
{
    private enum BlockState
    {
        Init,
        Ready,
        Move,
        Placed,
        Evaluate
    }

    [SerializeField] private SpriteRenderer blockElement;
    [SerializeField] private SpriteRenderer[] renderedBlocks;
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private GameGrid gameGrid;
    [SerializeField] private float moveSpeed = 1.5f;

    private BlockConfig blockConfig;
    private BlockState currentState;
    private GameRules gameRules;

    private int[,] block;
    private int currentColumn;
    private int currentRow;
    private int nextRow;
    private int lowestRowPlacement;
    private float speedFactor = 1f;
    private bool isDownKeyPressed;

    private void Awake()
    {
        gameRules = new GameRules(gameGrid, gameConfig);
    }

    private void Update()
    {
        if (MainManager.CurrentGameState != GameState.Running)
            return;

        if (currentState == BlockState.Move)
        {
            var targetPosition = gameGrid[nextRow, currentColumn].transform.position;

            var distanceRemaining = Vector3.SqrMagnitude(targetPosition - transform.position);

            if (distanceRemaining < 0.86f)
            {
                currentRow = nextRow;
            }

            if (distanceRemaining < 0.01f)
            {
                nextRow += 1;
                if (nextRow > lowestRowPlacement)
                {
                    StopBlockMovement();
                }
                targetPosition = gameGrid[nextRow, currentColumn].transform.position;
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed * speedFactor);

            if (isDownKeyPressed)
            {
                SignalService.Publish(new UpdateScoreSignal { Value = 1 });
            }
        }
    }

    private void OnEnable()
    {
        SignalService.Subscribe<DirectionButtonPressedSignal>(OnDirectionChangeEvent);
        SignalService.Subscribe<RotateBlockSignal>(OnRotateEvent);
        SignalService.Subscribe<DownArrowPressedSignal>(OnDownButtonPressed);
        SignalService.Subscribe<SpaceBarPressedSignal>(OnBlockTeleportEvent);
    }

    private void OnDisable()
    {
        SignalService.RemoveSignal<DirectionButtonPressedSignal>(OnDirectionChangeEvent);
        SignalService.RemoveSignal<RotateBlockSignal>(OnRotateEvent);
        SignalService.RemoveSignal<DownArrowPressedSignal>(OnDownButtonPressed);
        SignalService.RemoveSignal<SpaceBarPressedSignal>(OnBlockTeleportEvent);
    }

    public void InitialiseBlock(BlockConfig data)
    {
        blockConfig = data;

        block = new int[blockConfig.Row, blockConfig.Column];
        var curPtr = 0;

        for (int i = 0; i < blockConfig.Row; i++)
        {
            for (int j = 0; j < blockConfig.Column; j++)
            {
                block[i, j] = blockConfig.Indexes[curPtr++];
            }
        }

        currentRow = 0;
        nextRow = 0;
        currentColumn = 3;
        currentState = BlockState.Init;

        Debug.Log(JsonConvert.SerializeObject(block));
    }

    public void AdjustBoundPositions()
    {
        if (currentColumn < 0)
            currentColumn = 0;
        else if (currentColumn > gameConfig.GridColumns - block.GetLength(1))
            currentColumn = gameConfig.GridColumns - block.GetLength(1);

        if (currentState != BlockState.Move)
        {
            transform.position = gameGrid[0, currentColumn].transform.position + Vector3.up;
        }
        else
        {
            var currPos = transform.position;
            currPos.x = gameGrid[nextRow, currentColumn].transform.position.x;
            transform.position = currPos;
        }

        PredictLowestPlacement();
    }

    public void SetMoveState()
    {
        if (lowestRowPlacement < 0)
        {
            StopBlockMovement();
        }
        else
        {
            currentState = BlockState.Move;
        }
    }

    public void RenderBlock()
    {
        RenderBlock(block);
    }

    public void Clear()
    {
        foreach (var blockItem in renderedBlocks)
        {
            Destroy(blockItem.gameObject);
        }
        blockConfig = null;
        block = null;
        renderedBlocks = null;
    }

    #region Event Listeners

    private void OnDirectionChangeEvent(DirectionButtonPressedSignal signal)
    {
        var direction = signal.Value;
        if (currentState == BlockState.Move)
        {
            var isMoveValid = gameRules.IsValidMove(currentRow, currentColumn, direction, block);
            if (isMoveValid)
            {
                currentColumn += direction;
                AdjustBoundPositions();
            }
        }
    }

    private void OnRotateEvent(RotateBlockSignal signal)
    {
        if (currentState == BlockState.Move)
        {
            var rotatedBlock = gameRules.Rotate(block.GetLength(0), block.GetLength(1), block);
            var isValidRotation = gameRules.IsValidRotation(currentRow, currentColumn, rotatedBlock);

            if (isValidRotation)
            {
                block = rotatedBlock;
                RenderBlock(block);
                AdjustBoundPositions();
            }
        }
    }

    private void OnDownButtonPressed(DownArrowPressedSignal signal)
    {
        isDownKeyPressed = signal.Value;
        speedFactor = signal.Value ? gameConfig.BlockMoveDownFactor : 1f;
    }

    private void OnBlockTeleportEvent(SpaceBarPressedSignal signal)
    {
        if (MainManager.CurrentGameState != GameState.Running)
            return;

        if (currentState != BlockState.Move)
            return;

        currentState = BlockState.Evaluate;
        PredictLowestPlacement();
        StopBlockMovement();

        SignalService.Publish(new UpdateScoreSignal { Value = (gameConfig.GridRows - lowestRowPlacement) * 35 });

        if (MainManager.CurrentGameState == GameState.GameOver)
        {
            transform.position = gameGrid[nextRow, currentColumn].transform.position;
        }
    }

    #endregion Event Listeners

    private void StopBlockMovement()
    {
        currentState = BlockState.Placed;
        nextRow = lowestRowPlacement;

        var isSuccess = TryRenderBlocksInGrid();

        if (isSuccess)
        {
            SetBlocksVisibility(false);
        }
        else
        {
            SignalService.Publish(new GameStateUpdateSignal { Value = GameState.GameOver });
        }

        StartCoroutine(gameGrid.ValidateGrid(() =>
        {
            SignalService.Publish<BlockPlacedSignal>();
        }));
    }

    private void PredictLowestPlacement()
    {
        lowestRowPlacement = gameRules.FindLowestPlacement(block, currentColumn, nextRow);
    }

    private void RenderBlock(int[,] arr)
    {
        if (renderedBlocks == null || renderedBlocks.Length == 0)
        {
            renderedBlocks = new SpriteRenderer[arr.GetLength(0) * arr.GetLength(1)];

            for (int i = 0; i < renderedBlocks.Length; i++)
            {
                var go = Instantiate(blockElement, Vector3.zero, Quaternion.identity, this.transform);
                go.gameObject.SetActive(false);
                go.transform.localScale = Vector3.one * 0.9f;
                renderedBlocks[i] = go;
            }
        }

        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                var go = renderedBlocks[i + (j * arr.GetLength(0))];
                go.transform.localPosition = new Vector3(j, -i, 0) + Vector3.up * (arr.GetLength(0) - 1);
                go.gameObject.SetActive(arr[i, j] > 0);
                go.sprite = blockConfig.BlockSprite;
            }
        }
    }

    private bool TryRenderBlocksInGrid()
    {
        var rowOffset = lowestRowPlacement - block.GetLength(0) + 1;

        if (rowOffset < 0)
            return false;

        if (lowestRowPlacement < 0)
            return false;

        gameGrid.DrawBlocksOnGrid(rowOffset, currentColumn, block, blockConfig.BlockSprite);
        return true;
    }

    private void SetBlocksVisibility(bool state)
    {
        if (lowestRowPlacement == 0)
            return;

        foreach (var blockItem in renderedBlocks)
        {
            blockItem.gameObject.SetActive(state);
        }
    }
}