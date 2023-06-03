using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRules
{
    private GameGrid grid;
    private GameConfig config;

    public GameRules(GameGrid grid, GameConfig config)
    {
        this.grid = grid;
        this.config = config;
    }

    public bool IsValidMove(int row, int column, int direction, int[,] block)
    {
        var newColumn = column + direction;

        var totalColumns = block.GetLength(1);
        var isValid = true;

        var firstColumnItem = newColumn;
        var lastColumnItem = newColumn + totalColumns - 1;

        if (newColumn < 0)
            return false;

        if (lastColumnItem >= config.GridColumns)
            return false;

        if (grid[row, firstColumnItem].cellState != 0)
            return false;

        if (grid[row, lastColumnItem].cellState != 0)
            return false;

        return isValid;
    }

    public int FindLowestPlacement(int[,] block, int columnId, int rowId)
    {
        var lowestRowPlacement = config.GridRows - 1;
        for (int i = 0; i < block.GetLength(1); i++)
        {
            var lastRow = block.GetLength(0) - 1;
            var emptyCellInColumn = 0;
            for (int r = lastRow; r >= 0; r--)
            {
                if (block[r, i] != 0)
                    break;
                emptyCellInColumn += 1;
            }

            var blockColumnId = i + columnId;
            var highestPlacement = 0;
            for (int j = rowId; j < config.GridRows; j++)
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

        return lowestRowPlacement;
    }

    public int[,] Rotate(int row, int column, int[,] block)
    {
        var newRow = column;
        var newColumn = row;

        int[,] newArr = new int[newRow, newColumn];

        for (int j = 0; j < newColumn; j++)
        {
            for (int i = 0; i < newRow; i++)
            {
                var bb = block[row - j - 1, i];
                newArr[i, j] = bb;
            }
        }

        return newArr;
    }

    public bool IsValidRotation(int currentRow, int currentColumn, int[,] block)
    {
        for (int i = 0; i < block.GetLength(0); i++)
        {
            for (int j = 0; j < block.GetLength(1); j++)
            {
                if (block[i, j] == 0)
                    continue;

                if (currentColumn + j >= config.GridColumns)
                    return false;

                if (grid[currentRow + i, currentColumn + j].cellState != 0)
                    return false;
            }
        }

        return true;
    }
}
