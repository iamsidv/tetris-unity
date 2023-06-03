using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField] private GameConfig config;
    [SerializeField] private Cell cellPrefab;

    private Cell[,] grid;
    
    public Cell this[int row, int column] => grid[row, column];
    private int Rows => config.GridRows;
    private int Columns => config.GridColumns;

    private void Awake()
    {
        grid = new Cell[Rows, Columns];
        Initialize();
    }

    private void OnEnable()
    {
        SignalService.Subscribe<GameStateUpdateSignal>(OnGameStateUpdate);
    }

    private void OnGameStateUpdate(GameStateUpdateSignal signal)
    {
        if (signal.Value == GameState.Running)
            ClearAllCells();
    }

    private void OnDisable()
    {
        SignalService.RemoveSignal<GameStateUpdateSignal>(OnGameStateUpdate);
    }

    private void Initialize()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                var cell = Instantiate(cellPrefab, this.transform);
                cell.transform.localPosition = new Vector3(j, -i, 0);
                cell.Init(i, j);

                grid[i, j] = cell;

                //Test Code
                if (i == Rows - 1 && UnityEngine.Random.value > 0.5f)
                {
                    cell.Fill();
                    cell.SetSprite(config.SpriteMaps[UnityEngine.Random.Range(0, 5)].MappedSprite);
                }
            }
        }
    }

    public void DrawBlocksOnGrid(int rowOffset, int column, int[,] arr, Sprite sprite)
    {
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
        for (int i = 0; i < Rows; i++)
        {
            if (IsRowFilled(i))
            {
                linesFilled += 1;
            }
        }

        if (linesFilled > 0)
        {
            var score = config.LineClearScore + (linesFilled - 1) * config.LineClearMultiplier;
            SignalService.Publish(new UpdateScoreSignal { Value = score });
        }

        for (int i = Rows - 1; i >= 0; i--)
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

    public void ClearRow(int row)
    {
        var emptySprite = config.SpriteMaps.First(t => t.MappedId.Equals("outline")).MappedSprite;

        for (int i = 0; i < Columns; i++)
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
        for (int i = 0; i < Columns; i++)
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
        for (int i = 0; i < Rows; i++)
        {
            ClearRow(i);
        }
    }

    private bool IsRowFilled(int row)
    {
        for (int i = 0; i < Columns; i++)
        {
            if (grid[row, i].cellState == 0)
                return false;
        }

        return true;
    }
}