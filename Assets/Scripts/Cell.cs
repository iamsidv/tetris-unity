using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private int rowId;
    private int columnId;

    public int cellState = 0;
    //public int cellColor = 0;

    [SerializeField] private SpriteRenderer spriteRenderer;
    public Sprite GetSprite => spriteRenderer.sprite;

    public void Init(int row, int column)
    {
        rowId = row;
        columnId = column;
    }

    public void Fill()
    {
        cellState = 1;
    }

    public void ClearCell()
    {
        cellState = 0;
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}