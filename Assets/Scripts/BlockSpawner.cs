using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Random = System.Random;

public class BlockSpawner : MonoBehaviour {
    private const int top = 1;
    private const int right = 1;
    private const int left = -1;
    private const int down = -1;

    private Block[,] blocks;
    private BlockObject[,] blockObjects;
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
        int bombs = bombCount;
        while (bombs != 0) {
            int randX = rng.Next(width - 1);
            int randY = rng.Next(height - 1);

            if(!blocks[randX, randY].isMine && randX != clickedX && randY != clickedY) {
                blocks[randX, randY].isMine = true;
                bombs--;
            }
        }

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (blocks[x, y].isMine) {
                    if (x + right != width && y + top != height) { // Upper right
                        if (blocks[x + right, y + top].isMine == false) {
                            blocks[x + right, y + top].number++;
                        }
                    }
                    if (x + left >= 0 && y + top != height) { // Upper left
                        if (blocks[x + left, y + top].isMine == false) {
                            blocks[x + left, y + top].number++;
                        }
                    }
                    if (x + left >= 0 && y + down >= 0) { // Lower left
                        if (blocks[x + left, y + down].isMine == false) {
                            blocks[x + left, y + down].number++;
                        }
                    }
                    if (x + right != width && y + down >= 0) { // Lower right
                        if (blocks[x + right, y + down].isMine == false) {
                            blocks[x + right, y + down].number++;
                        }
                    }

                    if (y + top != height) {
                        if (blocks[x, y + top].isMine == false) {
                            blocks[x, y + top].number++;
                        }
                    }
                    if (y + down >= 0) {
                        if (blocks[x, y + down].isMine == false) {
                            blocks[x, y + down].number++;
                        }
                    }
                    if (x + right != width) {
                        if (blocks[x + right, y].isMine == false) {
                            blocks[x + right, y].number++;
                        }
                    }
                    if (x + left >= 0) {
                        if (blocks[x + left, y].isMine == false) {
                            blocks[x + left, y].number++;
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
    
    public void RevealClearRegion(int x, int y) {
        if (x + right != width && y + top != height) { // Upper right
            blockObjects[x + right, y + top].Reveal();
        }
        if (x + left >= 0 && y + top != height) { // Upper left
            blockObjects[x + left, y + top].Reveal();
        }
        if (x + left >= 0 && y + down >= 0) { // Lower left
            blockObjects[x + left, y + down].Reveal();
        }
        if (x + right != width && y + down >= 0) { // Lower right
            blockObjects[x + right, y + down].Reveal();
        } 
        if (y + top != height) {
            blockObjects[x, y + top].Reveal();
        }
        if (y + down >= 0) {
            blockObjects[x, y + down].Reveal();
        }
        if (x + right != width) {
            blockObjects[x + right, y].Reveal();
        }
        if (x + left >= 0) {
            blockObjects[x + left, y].Reveal();
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
