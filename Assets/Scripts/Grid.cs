using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int rows = 12;
    public int columns = 8;

    private Cell[,] grid;

    [SerializeField] private GameConfig config;
    [SerializeField] Cell cellPrefab;

    public Cell this[int row, int column]
    {
        get
        {
            return grid[row, column];
        }
    }

    private void Awake()
    {
        grid = new Cell[rows, columns];
        PopulateGrid();
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
                if (i == rows - 1 && Random.value > 0.5f)
                {
                    cell.Fill();
                    cell.SetSprite(config.Blocks[3].BlockSprite);
                }
            }
        }
    }



    internal void DrawBlocksOnGrid(int rowPlacement, int column, int[,] arr)
    {


        var rowOffset = rowPlacement - arr.GetLength(0) + 1;

        Debug.Log(JsonConvert.SerializeObject(arr) + " rP " + rowPlacement + " offset " + rowOffset + " col " + column);

        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                //Debug.Log(i + ".." + j);
                if (arr[i, j] == 0)
                    continue;

                //Debug.Log($"x.> {i + rowOffset}, y.> {j + column}");

                grid[i + rowOffset, j + column].Fill();
                grid[i + rowOffset, j + column].SetSprite(config.Blocks[3].BlockSprite);
            }

            //Debug.Log(".................");
        }
    }
}
