using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private SpriteRenderer blockElement;
    [SerializeField] private SpriteRenderer[] renderedBlocks;

    private int[,] arr;

    public GameConfig config;
    public Grid grid;

    private BlockConfig _data;

    public int startColumnId;

    private void OnDirectionChangeEvent(int direction)
    {
        startColumnId += direction;

        AdjustBoundPositions();
    }

    public void AdjustBoundPositions()
    {
        if (startColumnId < 0)
            startColumnId = 0;
        else if (startColumnId > grid.columns - arr.GetLength(1))
            startColumnId = grid.columns - arr.GetLength(1);

        transform.position = grid[0, startColumnId].transform.position;
    }

    void Start()
    {


        //transform.position = grid[config.GridRows - 1, startColumnId].transform.position;
    }

    private void OnEnable()
    {
        UserInput.OnDirectionChangeEvent += OnDirectionChangeEvent;
        UserInput.OnRotateEvent += OnRotateEvent;
    }

    private void OnRotateEvent()
    {
        arr = Rotate(arr.GetLength(0), arr.GetLength(1), arr);
        RenderBlock(arr);

        AdjustBoundPositions();
    }

    private void OnDisable()
    {
        UserInput.OnDirectionChangeEvent -= OnDirectionChangeEvent;
        UserInput.OnRotateEvent -= OnRotateEvent;
    }

    public void InitialiseBlock(BlockConfig data)
    {
        //_data = config.Blocks[Random.Range(0, config.Blocks.Length)];

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

        Debug.Log(JsonConvert.SerializeObject(arr));
    }

    public void RenderBlock()
    {
        RenderBlock(arr);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    arr = Rotate(arr.GetLength(0), arr.GetLength(1), arr);
        //    RenderBlock(arr);

        //    AdjustBoundPositions();
        //}

        if (Input.GetKeyDown(KeyCode.M))
        {
            PredictLowestPlacement();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            StartCoroutine(grid.ValidateGrid());
        }
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

    private void PredictLowestPlacement()
    {
        var lowestPlacement = config.GridRows - 1;
        for (int i = 0; i < arr.GetLength(1); i++)
        {
            var lastRow = arr.GetLength(0) - 1;
            var emptyCellInColumn = 0;
            for (int e = lastRow; e >=0 ; e--)
            {
                if (arr[e, i] != 0)
                    break;
                emptyCellInColumn += 1;
            }

            var blockColumnId = i + startColumnId;
            var highestPlacement = 0;
            for (int j = 1; j < config.GridRows; j++)
            {
                if (grid[j, blockColumnId].cellState != 0)
                {
                    break;
                }
                highestPlacement = j + emptyCellInColumn;
            }

            Debug.Log(highestPlacement + " Empty cell " + emptyCellInColumn);

            if (highestPlacement < lowestPlacement)
                lowestPlacement = highestPlacement;
        }

        Debug.Log("Lowest Placement is at Row" + lowestPlacement);
        grid.DrawBlocksOnGrid(lowestPlacement, startColumnId, arr, _data.BlockSprite);
    }
}
