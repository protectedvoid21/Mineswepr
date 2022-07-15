using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [SerializeField] private Color hoverColor;
    private Color defaultColor;

    private Image image;

    private GameManager gameManager;
    private Block block;
    private int x;
    private int y;

    public bool isClicked { get; private set; }
    public bool isFlagged { get; private set; }
    public bool isHinted => image.sprite == BlockAssets.instance.hinted;

    private void Awake() {
        gameManager = FindObjectOfType<GameManager>();
        image = GetComponent<Image>();
        defaultColor = image.color;
    }

    public void Setup(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public void SetBlock(Block block) {
        this.block = block;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(!isClicked) {
            image.color = hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if(!isClicked) {
            image.color = defaultColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if(isClicked || isHinted) {
            return;
        }
        if(eventData.button == PointerEventData.InputButton.Left && !isFlagged) {
            if(!gameManager.IsFirstClicked) {
                gameManager.FirstClick();
                FindObjectOfType<BlockSpawner>().GenerateMap(x, y);
            }
            AudioManager.instance.Play("click");
            Reveal();
        }
        else if(eventData.button == PointerEventData.InputButton.Right) {
            if(isFlagged) {
                gameManager.AddMineToText(1);
                image.sprite = BlockAssets.instance.defaultBlock;
            }
            else {
                gameManager.AddMineToText(-1);
                image.sprite = BlockAssets.instance.flagged;
            }
            AudioManager.instance.Play("flag");
            isFlagged = !isFlagged;  
        }
    }

    public void Reveal() {
        if(isClicked) {
            return;
        }
        isClicked = true;
        image.color = defaultColor;
        if(block.isMine) {
            image.sprite = BlockAssets.instance.mine;
            gameManager.GameOver();
            return;
        }
        else if(block.number == 0) {
            if(isFlagged) {
                gameManager.AddMineToText(1);
            }
            image.sprite = BlockAssets.instance.clear;
            FindObjectOfType<BlockSpawner>().RevealClearRegion(x, y);
        }
        else {
            image.sprite = BlockAssets.instance.numbers[block.number - 1];
        }
        gameManager.DecreaseBombCount();
    }

    public void ForceReveal() {
        if(isClicked) {
            return;
        }

        isClicked = true;
        image.color = defaultColor;
        if(block.isMine) {
            image.sprite = BlockAssets.instance.mine;
        }
        else if(block.number == 0) {
            image.sprite = BlockAssets.instance.clear;
        }
        else {
            image.sprite = BlockAssets.instance.numbers[block.number - 1];
        }
    }

    public void MarkHinted() {
        if (!isFlagged) {
            image.sprite = BlockAssets.instance.hinted;
        }
    }
}
