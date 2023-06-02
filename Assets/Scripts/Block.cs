using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    enum States
    {
        Init,
        Ready,
        Move,
        Placed,
        Evaluate
    }


    [SerializeField] private SpriteRenderer blockElement;
    [SerializeField] private SpriteRenderer[] renderedBlocks;

    private int[,] arr;

    public GameConfig config;
    public Grid grid;

    private BlockConfig _data;

    public int startColumnId;
    public int lowestRowPlacement;

    public int initRowId;
    private States currentState;
    public float moveSpeed = 1.5f;
    private float speedFactor = 1f;

    public void InitialiseBlock(BlockConfig data)
    {
        _data = data;

        arr = new int[_data.Row, _data.Column];
        var curPtr = 0;

        for (int i = 0; i < _data.Row; i++)
        {
            for (int j = 0; j < _data.Column; j++)
            {
                arr[i, j] = _data.Indexes[curPtr++];
            }
        }

        initRowId = 0;
        currentState = States.Init;

        Debug.Log(JsonConvert.SerializeObject(arr));
    }

    public void RenderBlock()
    {
        RenderBlock(arr);
    }

    private void Update()
    {
        if (currentState == States.Move)
        {
            var targetPosition = grid[initRowId, startColumnId].transform.position;

            if (Vector3.SqrMagnitude(targetPosition - transform.position) < 0.01f)
            {
                initRowId += 1;

                if (initRowId > lowestRowPlacement)
                {
                    StopBlockMovement();
                }
                targetPosition = grid[initRowId, startColumnId].transform.position;
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed * speedFactor);
        }
    }

    private void OnEnable()
    {
        UserInput.OnDirectionChangeEvent += OnDirectionChangeEvent;
        UserInput.OnRotateEvent += OnRotateEvent;
        UserInput.OnDownButtonPressed += OnDownButtonPressed; ;
        SignalService.OnBlockTeleportEvent += SignalService_OnBlockTeleportEvent;
    }

    private void OnDisable()
    {
        UserInput.OnDirectionChangeEvent -= OnDirectionChangeEvent;
        UserInput.OnRotateEvent -= OnRotateEvent;

        SignalService.OnBlockTeleportEvent -= SignalService_OnBlockTeleportEvent;
    }

    private void OnDirectionChangeEvent(int direction)
    {
        if (currentState == States.Move)
        {
            var isMoveValid = IsValidMove(direction);
            if (isMoveValid)
            {
                startColumnId += direction;
                AdjustBoundPositions();
            }
        }
    }

    public void AdjustBoundPositions()
    {
        if (startColumnId < 0)
            startColumnId = 0;
        else if (startColumnId > grid.columns - arr.GetLength(1))
            startColumnId = grid.columns - arr.GetLength(1);

        if (currentState != States.Move)
        {
            transform.position = grid[initRowId, startColumnId].transform.position;
        }
        else
        {
            var currPos = transform.position;
            currPos.x = grid[initRowId, startColumnId].transform.position.x;
            transform.position = currPos;
        }

        PredictLowestPlacement();
        currentState = States.Move;
    }

    public void Clear()
    {
        foreach (var blockItem in renderedBlocks)
        {
            Destroy(blockItem.gameObject);
        }
        _data = null;
        arr = null;
        renderedBlocks = null;
    }

    private void SignalService_OnBlockTeleportEvent()
    {
        if (currentState != States.Move)
            return;

        currentState = States.Evaluate;
        PredictLowestPlacement();
        StopBlockMovement();
    }

    private void OnDownButtonPressed(bool isPressed)
    {
        speedFactor = isPressed ? config.BlockMoveDownFactor : 1f;
    }

    private void OnRotateEvent()
    {
        if (currentState == States.Move)
        {
            arr = Rotate(arr.GetLength(0), arr.GetLength(1), arr);
            RenderBlock(arr);

            AdjustBoundPositions();
        }
    }

    private void StopBlockMovement()
    {
        currentState = States.Placed;
        initRowId = lowestRowPlacement;
        //transform.position = grid[initRowId, startColumnId].transform.position;

        DrawBlocksOnGrid();

        SetBlocksVisibility(false);

        StartCoroutine(grid.ValidateGrid(() =>
        {
            SignalService.TriggerOnBlockPlacedEvent();
        }));
    }

    private int[,] Rotate(int row, int column, int[,] arr)
    {
        var newRow = column;
        var newColumn = row;

        int[,] newArr = new int[newRow, newColumn];

        for (int j = 0; j < newColumn; j++)
        {
            for (int i = 0; i < newRow; i++)
            {
                var bb = arr[row - j - 1, i];
                newArr[i, j] = bb;
            }
        }

        return newArr;
    }

    private void PredictLowestPlacement()
    {
        lowestRowPlacement = config.GridRows - 1;
        for (int i = 0; i < arr.GetLength(1); i++)
        {
            var lastRow = arr.GetLength(0) - 1;
            var emptyCellInColumn = 0;
            for (int e = lastRow; e >= 0; e--)
            {
                if (arr[e, i] != 0)
                    break;
                emptyCellInColumn += 1;
            }

            var blockColumnId = i + startColumnId;
            var highestPlacement = 0;
            for (int j = initRowId; j < config.GridRows; j++)
            {
                if (grid[j, blockColumnId].cellState != 0)
                {
                    break;
                }
                highestPlacement = j + emptyCellInColumn;
            }

            Debug.Log(highestPlacement + " Empty cell " + emptyCellInColumn);

            if (highestPlacement < lowestRowPlacement)
                lowestRowPlacement = highestPlacement;
        }

        Debug.Log("Lowest Placement is at Row" + lowestRowPlacement);
        //grid.DrawBlocksOnGrid(lowestPlacement, startColumnId, arr, _data.BlockSprite);
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
                go.sprite = _data.BlockSprite;
            }
        }
    }

    private void DrawBlocksOnGrid()
    {
        grid.DrawBlocksOnGrid(lowestRowPlacement, startColumnId, arr, _data.BlockSprite);
    }

    private bool IsValidMove(int direction)
    {
        var newColumn = startColumnId + direction;

        var totalColumns = arr.GetLength(1);
        var isValid = true;

        var firstColumnItem = newColumn;
        var lastColumnItem = newColumn + totalColumns - 1;

        if (newColumn < 0)
            return false;

        if (lastColumnItem >= grid.columns)
            return false;

        if (grid[initRowId, firstColumnItem].cellState != 0)
            return false;

        if (grid[initRowId, lastColumnItem].cellState != 0)
            return false;

        return isValid;
    }

    private void SetBlocksVisibility(bool state)
    {
        foreach (var blockItem in renderedBlocks)
        {
            blockItem.gameObject.SetActive(state);
        }
    }
}
