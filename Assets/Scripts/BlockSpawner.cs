using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockSpawner : MonoBehaviour {
    private const int Top = 1;
    private const int Right = 1;
    private const int Left = -1;
    private const int Down = -1;

    private Block[,] blocks;
    private BlockObject[,] blockObjects;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BlockObject blockObject;

    [SerializeField] private Transform parentTransform;
    [SerializeField] private RectTransform startPosition;
    
    public int width { get; private set; }
    public int height { get; private set; }
    public int bombCount { get; private set; }

    private void Awake() {
        width = PlayerPrefs.GetInt("Width", 10);
        height = PlayerPrefs.GetInt("Height", 10);
        bombCount = PlayerPrefs.GetInt("Bombs", 15);

        gameManager = FindObjectOfType<GameManager>();
        
        blockObjects = new BlockObject[width, height];
        
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                blockObjects[x, y] = SpawnBlock(x, y);
            }
        }
    }

    public void GenerateMap(int clickedX, int clickedY) {
        blockObject.GetComponent<Image>().sprite = BlockAssets.instance.defaultBlock;
        blocks = new Block[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                blocks[x, y] = new Block();
            }
        }

        System.Random rng = new System.Random();
        GetNeighborUnclickedBlockCount(clickedX, clickedY, out List<int[]> neighborBlocks);
        int bombs = bombCount;
        while (bombs != 0) {
            int randX = rng.Next(width - 1);
            int randY = rng.Next(height - 1);

            bool neighborMineFound = false;

            if(!blocks[randX, randY].isMine && randX != clickedX && randY != clickedY) {
                foreach (var block in neighborBlocks) {
                    if (randX == block[0] && randY == block[1]) {
                        neighborMineFound = true;
                        break;
                    }
                }
                if (neighborMineFound) {
                    continue;
                }
                blocks[randX, randY].isMine = true;
                bombs--;
            }
        }

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (blocks[x, y].isMine) {
                    if (x + Right != width && y + Top != height) { // Upper right
                        if (blocks[x + Right, y + Top].isMine == false) {
                            blocks[x + Right, y + Top].number++;
                        }
                    }
                    if (x + Left >= 0 && y + Top != height) { // Upper left
                        if (blocks[x + Left, y + Top].isMine == false) {
                            blocks[x + Left, y + Top].number++;
                        }
                    }
                    if (x + Left >= 0 && y + Down >= 0) { // Lower left
                        if (blocks[x + Left, y + Down].isMine == false) {
                            blocks[x + Left, y + Down].number++;
                        }
                    }
                    if (x + Right != width && y + Down >= 0) { // Lower right
                        if (blocks[x + Right, y + Down].isMine == false) {
                            blocks[x + Right, y + Down].number++;
                        }
                    }

                    if (y + Top != height) {
                        if (blocks[x, y + Top].isMine == false) {
                            blocks[x, y + Top].number++;
                        }
                    }
                    if (y + Down >= 0) {
                        if (blocks[x, y + Down].isMine == false) {
                            blocks[x, y + Down].number++;
                        }
                    }
                    if (x + Right != width) {
                        if (blocks[x + Right, y].isMine == false) {
                            blocks[x + Right, y].number++;
                        }
                    }
                    if (x + Left >= 0) {
                        if (blocks[x + Left, y].isMine == false) {
                            blocks[x + Left, y].number++;
                        }
                    }
                }
                blockObjects[x, y].SetBlock(blocks[x, y]);
            }
        }
    }

    private BlockObject SpawnBlock(int x, int y) {
        BlockObject createdBlock = Instantiate(blockObject, new Vector3(x, y), Quaternion.identity);
        createdBlock.Setup(x, y);
        createdBlock.transform.SetParent(parentTransform, false);

        RectTransform rect = createdBlock.GetComponent<RectTransform>();
        rect.position = new Vector3(x * rect.sizeDelta.x, y * rect.sizeDelta.y) + startPosition.position;

        return createdBlock;
    }

    public void UseHint() {
        if (blocks is null) {
            return;
        }

        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                if(!blocks[x, y].isMine && blocks[x, y].number > 0) {
                    if(GetNeighborUnclickedBlockCount(x, y, out List<int[]> neighborBlocks) == blocks[x, y].number) {
                        foreach (var block in neighborBlocks) {
                            if (!blockObjects[block[0], block[1]].isFlagged && !blockObjects[block[0], block[1]].isHinted) {
                                blockObjects[block[0], block[1]].MarkHinted();
                                gameManager.AddMineToText(-1);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    private int GetNeighborUnclickedBlockCount(int x, int y, out List<int[]> neighborBlocks) {
        int blockCount = 0;
        neighborBlocks = new List<int[]>();

        if(x > 0) {
            if(!blockObjects[x + Left, y].isClicked) {
                neighborBlocks.Add(new int[] { x + Left, y });
                blockCount++;
            }

            if(y > 0) {
                if(!blockObjects[x + Left, y + Down].isClicked) {
                    neighborBlocks.Add(new int[] { x + Left, y + Down });
                    blockCount++;
                }
            }
            if(y + 1 < height) {
                if(!blockObjects[x + Left, y + Top].isClicked) {
                    neighborBlocks.Add(new int[] { x + Left, y + Top });
                    blockCount++;
                }
            }
        }
        if(x + 1 < width) {
            if(!blockObjects[x + Right, y].isClicked) {
                neighborBlocks.Add(new int[] { x + Right, y });
                blockCount++;
            }

            if(y > 0) {
                if(!blockObjects[x + Right, y + Down].isClicked) {
                    neighborBlocks.Add(new int[] { x + Right, y + Down });
                    blockCount++;
                }
            }
            if(y + 1 < height) {
                if(!blockObjects[x + Right, y + Top].isClicked) {
                    neighborBlocks.Add(new int[] { x + Right, y + Top });
                    blockCount++;
                }
            }
        }

        if(y > 0) {
            if(!blockObjects[x, y + Down].isClicked) {
                neighborBlocks.Add(new int[] { x, y + Down });
                blockCount++;
            }
        }
        if(y + 1 < height) {
            if(!blockObjects[x, y + Top].isClicked) {
                neighborBlocks.Add(new int[] { x, y + Top });
                blockCount++;
            }
        }
        return blockCount;
    }

    public void RevealClearRegion(int x, int y) {
        if (x + Right != width && y + Top != height) { // Upper right
            blockObjects[x + Right, y + Top].Reveal();
        }
        if (x + Left >= 0 && y + Top != height) { // Upper left
            blockObjects[x + Left, y + Top].Reveal();
        }
        if (x + Left >= 0 && y + Down >= 0) { // Lower left
            blockObjects[x + Left, y + Down].Reveal();
        }
        if (x + Right != width && y + Down >= 0) { // Lower right
            blockObjects[x + Right, y + Down].Reveal();
        } 
        if (y + Top != height) {
            blockObjects[x, y + Top].Reveal();
        }
        if (y + Down >= 0) {
            blockObjects[x, y + Down].Reveal();
        }
        if (x + Right != width) {
            blockObjects[x + Right, y].Reveal();
        }
        if (x + Left >= 0) {
            blockObjects[x + Left, y].Reveal();
        }
    }

    public void RevealAllBombs() {
        IEnumerator RevealAnimated() {
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    if(blocks[x, y].isMine) {
                        blockObjects[x, y].ForceReveal();
                        AudioManager.instance.Play($"bomb{UnityEngine.Random.Range(1, 6)}");
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }

            Time.timeScale = 0;
        }

        StartCoroutine(RevealAnimated());
    }

    private void OnValidate() {
        bombCount = Mathf.Clamp(bombCount, 1, width * height - 10);
    }
}
