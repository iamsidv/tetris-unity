using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    public GameConfig config;
    public Block block;

    private void OnEnable()
    {
        UserInput.OnMoveDownEvent += DoGenerateNewBlock;
    }

    private void OnDisable()
    {
        UserInput.OnMoveDownEvent -= DoGenerateNewBlock;
    }

    private void Start()
    {
        DoGenerateNewBlock();
    }

    private void DoGenerateNewBlock()
    {
        var blockData = config.Blocks[Random.Range(0, config.Blocks.Length)];
        //var blockData = config.Blocks[4];
        block.InitialiseBlock(blockData);
        block.RenderBlock();
        block.AdjustBoundPositions();
    }
}
