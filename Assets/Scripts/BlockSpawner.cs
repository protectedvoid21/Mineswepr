using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockSpawner : MonoBehaviour
{
    private const int Top = 1;
    private const int Right = 1;
    private const int Left = -1;
    private const int Down = -1;

    private Block[,] _blocks;
    private BlockObject[,] _blockObjects;

    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private BlockObject blockObject;

    [SerializeField]
    private Transform parentTransform;

    [SerializeField]
    private RectTransform startPosition;

    public int Width { get; private set; }
    public int Height { get; private set; }
    public int BombCount { get; private set; }

    private void Awake()
    {
        Width = PlayerPrefs.GetInt("Width", 10);
        Height = PlayerPrefs.GetInt("Height", 10);
        BombCount = PlayerPrefs.GetInt("Bombs", 15);

        gameManager = FindObjectOfType<GameManager>();

        _blockObjects = new BlockObject[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                _blockObjects[x, y] = SpawnBlock(x, y);
            }
        }
    }

    public void GenerateMap(int clickedX, int clickedY)
    {
        blockObject.GetComponent<Image>().sprite = BlockAssets.instance.defaultBlock;
        _blocks = new Block[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                _blocks[x, y] = new Block();
            }
        }

        System.Random rng = new System.Random();
        GetNeighborUnclickedBlockCount(clickedX, clickedY, out List<int[]> neighborBlocks);
        int bombs = BombCount;
        while (bombs != 0)
        {
            int randX = rng.Next(Width - 1);
            int randY = rng.Next(Height - 1);

            bool neighborMineFound = false;

            if (!_blocks[randX, randY].IsMine && randX != clickedX && randY != clickedY)
            {
                foreach (var block in neighborBlocks)
                {
                    if (randX == block[0] && randY == block[1])
                    {
                        neighborMineFound = true;
                        break;
                    }
                }

                if (neighborMineFound)
                {
                    continue;
                }

                _blocks[randX, randY].IsMine = true;
                bombs--;
            }
        }

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (_blocks[x, y].IsMine)
                {
                    if (x + Right != Width && y + Top != Height)
                    {
                        // Upper right
                        if (_blocks[x + Right, y + Top].IsMine == false)
                        {
                            _blocks[x + Right, y + Top].Number++;
                        }
                    }

                    if (x + Left >= 0 && y + Top != Height)
                    {
                        // Upper left
                        if (_blocks[x + Left, y + Top].IsMine == false)
                        {
                            _blocks[x + Left, y + Top].Number++;
                        }
                    }

                    if (x + Left >= 0 && y + Down >= 0)
                    {
                        // Lower left
                        if (_blocks[x + Left, y + Down].IsMine == false)
                        {
                            _blocks[x + Left, y + Down].Number++;
                        }
                    }

                    if (x + Right != Width && y + Down >= 0)
                    {
                        // Lower right
                        if (_blocks[x + Right, y + Down].IsMine == false)
                        {
                            _blocks[x + Right, y + Down].Number++;
                        }
                    }

                    if (y + Top != Height)
                    {
                        if (_blocks[x, y + Top].IsMine == false)
                        {
                            _blocks[x, y + Top].Number++;
                        }
                    }

                    if (y + Down >= 0)
                    {
                        if (_blocks[x, y + Down].IsMine == false)
                        {
                            _blocks[x, y + Down].Number++;
                        }
                    }

                    if (x + Right != Width)
                    {
                        if (_blocks[x + Right, y].IsMine == false)
                        {
                            _blocks[x + Right, y].Number++;
                        }
                    }

                    if (x + Left >= 0)
                    {
                        if (_blocks[x + Left, y].IsMine == false)
                        {
                            _blocks[x + Left, y].Number++;
                        }
                    }
                }

                _blockObjects[x, y].SetBlock(_blocks[x, y]);
            }
        }
    }

    private BlockObject SpawnBlock(int x, int y)
    {
        BlockObject createdBlock = Instantiate(blockObject, new Vector3(x, y), Quaternion.identity);
        createdBlock.Setup(x, y);
        createdBlock.transform.SetParent(parentTransform, false);

        RectTransform rect = createdBlock.GetComponent<RectTransform>();
        rect.position = new Vector3(x * rect.sizeDelta.x, y * rect.sizeDelta.y) + startPosition.position;

        return createdBlock;
    }

    public void UseHint()
    {
        if (_blocks is null)
        {
            return;
        }

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (!_blocks[x, y].IsMine && _blocks[x, y].Number > 0)
                {
                    if (GetNeighborUnclickedBlockCount(x, y, out List<int[]> neighborBlocks) == _blocks[x, y].Number)
                    {
                        foreach (var block in neighborBlocks)
                        {
                            if (!_blockObjects[block[0], block[1]].IsFlagged &&
                                !_blockObjects[block[0], block[1]].IsHinted)
                            {
                                _blockObjects[block[0], block[1]].MarkHinted();
                                gameManager.AddMineToText(-1);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    private int GetNeighborUnclickedBlockCount(int x, int y, out List<int[]> neighborBlocks)
    {
        int blockCount = 0;
        neighborBlocks = new List<int[]>();

        if (x > 0)
        {
            if (!_blockObjects[x + Left, y].IsClicked)
            {
                neighborBlocks.Add(new int[] { x + Left, y });
                blockCount++;
            }

            if (y > 0)
            {
                if (!_blockObjects[x + Left, y + Down].IsClicked)
                {
                    neighborBlocks.Add(new int[] { x + Left, y + Down });
                    blockCount++;
                }
            }

            if (y + 1 < Height)
            {
                if (!_blockObjects[x + Left, y + Top].IsClicked)
                {
                    neighborBlocks.Add(new int[] { x + Left, y + Top });
                    blockCount++;
                }
            }
        }

        if (x + 1 < Width)
        {
            if (!_blockObjects[x + Right, y].IsClicked)
            {
                neighborBlocks.Add(new int[] { x + Right, y });
                blockCount++;
            }

            if (y > 0)
            {
                if (!_blockObjects[x + Right, y + Down].IsClicked)
                {
                    neighborBlocks.Add(new int[] { x + Right, y + Down });
                    blockCount++;
                }
            }

            if (y + 1 < Height)
            {
                if (!_blockObjects[x + Right, y + Top].IsClicked)
                {
                    neighborBlocks.Add(new int[] { x + Right, y + Top });
                    blockCount++;
                }
            }
        }

        if (y > 0)
        {
            if (!_blockObjects[x, y + Down].IsClicked)
            {
                neighborBlocks.Add(new int[] { x, y + Down });
                blockCount++;
            }
        }

        if (y + 1 < Height)
        {
            if (!_blockObjects[x, y + Top].IsClicked)
            {
                neighborBlocks.Add(new int[] { x, y + Top });
                blockCount++;
            }
        }

        return blockCount;
    }

    public void RevealClearRegion(int x, int y)
    {
        if (x + Right != Width && y + Top != Height)
        {
            // Upper right
            _blockObjects[x + Right, y + Top].Reveal();
        }

        if (x + Left >= 0 && y + Top != Height)
        {
            // Upper left
            _blockObjects[x + Left, y + Top].Reveal();
        }

        if (x + Left >= 0 && y + Down >= 0)
        {
            // Lower left
            _blockObjects[x + Left, y + Down].Reveal();
        }

        if (x + Right != Width && y + Down >= 0)
        {
            // Lower right
            _blockObjects[x + Right, y + Down].Reveal();
        }

        if (y + Top != Height)
        {
            _blockObjects[x, y + Top].Reveal();
        }

        if (y + Down >= 0)
        {
            _blockObjects[x, y + Down].Reveal();
        }

        if (x + Right != Width)
        {
            _blockObjects[x + Right, y].Reveal();
        }

        if (x + Left >= 0)
        {
            _blockObjects[x + Left, y].Reveal();
        }
    }

    public void RevealAllBombs()
    {
        IEnumerator RevealAnimated()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (_blocks[x, y].IsMine)
                    {
                        _blockObjects[x, y].ForceReveal();
                        AudioManager.instance.Play($"bomb{Random.Range(1, 6)}");
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }

            Time.timeScale = 0;
        }

        StartCoroutine(RevealAnimated());
    }

    private void OnValidate()
    {
        BombCount = Mathf.Clamp(BombCount, 1, Width * Height - 10);
    }
}