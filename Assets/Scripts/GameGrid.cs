using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public int rows = 12;
    public int columns = 8;

    private Cell[,] grid;

    [SerializeField] private GameConfig config;
    [SerializeField] private Cell cellPrefab;

    public Cell this[int row, int column] => grid[row, column];

    private void Awake()
    {
        grid = new Cell[rows, columns];
        PopulateGrid();
    }

    private void OnEnable()
    {
        SignalService.OnGameStateUpdated += OnGameStateUpdate;
    }

    private void OnGameStateUpdate(GameState state)
    {
        if (state == GameState.Running)
            ClearAllCells();
    }

    private void OnDisable()
    {
        SignalService.OnGameStateUpdated -= OnGameStateUpdate;
    }

    private void PopulateGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                var cell = Instantiate(cellPrefab, this.transform);
                cell.transform.localPosition = new Vector3(j, -i, 0);
                cell.Init(i, j);

                grid[i, j] = cell;

                //Test Code
                if (i == rows - 1 && UnityEngine.Random.value > 0.5f)
                {
                    cell.Fill();
                    cell.SetSprite(config.SpriteMaps[UnityEngine.Random.Range(0, 5)].MappedSprite);
                }
            }
        }
    }

    public void DrawBlocksOnGrid(int rowOffset, int column, int[,] arr, Sprite sprite)
    {
        //var rowOffset = rowPlacement - arr.GetLength(0) + 1;

        Debug.Log(JsonConvert.SerializeObject(arr) + " offset " + rowOffset + " col " + column);

        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                if (arr[i, j] == 0)
                    continue;

                grid[i + rowOffset, j + column].Fill();
                grid[i + rowOffset, j + column].SetSprite(sprite);
            }
        }
    }

    public IEnumerator ValidateGrid(Action callback = null)
    {
        var linesFilled = 0;
        for (int i = 0; i < rows; i++)
        {
            if (IsRowFilled(i))
            {
                linesFilled += 1;
            }
        }

        if (linesFilled > 0)
            MainManager.instance.AddScore(config.LineClearScore + (linesFilled - 1) * config.LineClearMultiplier);

        for (int i = rows - 1; i >= 0; i--)
        {
            if (IsRowFilled(i))
            {
                ClearRow(i);
                yield return new WaitForSeconds(0.5f);
                MoveDownRowsTogether(i - 1);
                yield return new WaitForSeconds(0.5f);
                i++;
            }
        }

        yield return null;

        callback?.Invoke();
    }

    private bool IsRowFilled(int row)
    {
        for (int i = 0; i < columns; i++)
        {
            if (grid[row, i].cellState == 0)
                return false;
        }

        return true;
    }

    public void ClearRow(int row)
    {
        var emptySprite = config.SpriteMaps.First(t => t.MappedId.Equals("outline")).MappedSprite;

        for (int i = 0; i < columns; i++)
        {
            grid[row, i].ClearCell();
            grid[row, i].SetSprite(emptySprite);
        }
    }

    public void MovOneRowDown(int row)
    {
        if (row == 0)
            return;

        var emptySprite = config.SpriteMaps.First(t => t.MappedId.Equals("outline")).MappedSprite;
        for (int i = 0; i < columns; i++)
        {
            var currentSprite = grid[row, i].GetSprite;
            var cellState = grid[row, i].cellState;
            grid[row, i].ClearCell();
            grid[row, i].SetSprite(emptySprite);

            grid[row + 1, i].cellState = cellState;
            grid[row + 1, i].SetSprite(currentSprite);
        }
    }

    public void MoveDownRowsTogether(int startRow)
    {
        for (int i = startRow; i >= 0; i--)
        {
            MovOneRowDown(i);
        }
    }

    public void ClearAllCells()
    {
        for (int i = 0; i < rows; i++)
        {
            ClearRow(i);
        }
    }
}