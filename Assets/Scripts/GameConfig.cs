using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig.asset", menuName = "Cosmic Lounge/Create/Game Config")]
public class GameConfig : ScriptableObject
{
    [SerializeField] private int gridRows;
    [SerializeField] private int gridColumns;
    [SerializeField] private BlockConfig[] blocks;
    [SerializeField] private SpriteMapping[] spriteMap;
    [SerializeField] private float blockMoveDownSpeed;
    [SerializeField] private int lineClearMultiplier;
    [SerializeField] private int lineClearScore;

    public BlockConfig[] Blocks => blocks;
    public SpriteMapping[] SpriteMaps => spriteMap;
    public int GridRows => gridRows;
    public int GridColumns => gridColumns;
    public float BlockMoveDownFactor => blockMoveDownSpeed;

    public int LineClearScore => lineClearScore;
    public int LineClearMultiplier => lineClearMultiplier;
}

[System.Serializable]
public class BlockConfig
{
    /*
    1 1 0
    0 1 1
    The above block will be stored as 1,1,1,0,0,1,1 in data array.
    The value can come from backend also.
    row = 2
    column = 3
    id = Not in use currently but can be used to identify different blocks too
    */

    [SerializeField] private string id;
    [SerializeField] private int[] indexes;
    [SerializeField] private int row;
    [SerializeField] private int column;
    [SerializeField] private Sprite blockSprite;

    public string Id => id;
    public int[] Indexes => indexes;
    public int Row => row;
    public int Column => column;
    public Sprite BlockSprite => blockSprite;
}

[System.Serializable]
public class SpriteMapping
{
    [SerializeField] private string id;
    [SerializeField] private Sprite mappedSprite;

    public string MappedId => id;
    public Sprite MappedSprite => mappedSprite;
}