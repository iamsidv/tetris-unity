﻿using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int rowId;
    public int columnId;

    public int cellState = 0;
    //public int cellColor = 0;

    [SerializeField] private SpriteRenderer spriteRenderer;

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

    public Sprite GetSprite => spriteRenderer.sprite;
}