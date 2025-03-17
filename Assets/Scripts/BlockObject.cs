using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    private Color hoverColor;

    private Color _defaultColor;

    private Image _image;

    private GameManager  _gameManager;
    private Block _block;
    private int _x;
    private int _y;

    public bool IsClicked { get; private set; }
    public bool IsFlagged { get; private set; }
    public bool IsHinted => _image.sprite == BlockAssets.instance.hinted;

    private void Awake()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        _image = GetComponent<Image>();
        _defaultColor = _image.color;
    }

    public void Setup(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public void SetBlock(Block block)
    {
        _block = block;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsClicked)
        {
            _image.color = hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsClicked)
        {
            _image.color = _defaultColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsClicked || IsHinted)
        {
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left && !IsFlagged)
        {
            if (!_gameManager.IsFirstClicked)
            {
                _gameManager.FirstClick();
                FindFirstObjectByType<BlockSpawner>().GenerateMap(_x, _y);
            }

            AudioManager.instance.Play("click");
            Reveal();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (IsFlagged)
            {
                _gameManager.AddMineToText(1);
                _image.sprite = BlockAssets.instance.defaultBlock;
            }
            else
            {
                _gameManager.AddMineToText(-1);
                _image.sprite = BlockAssets.instance.flagged;
            }

            AudioManager.instance.Play("flag");
            IsFlagged = !IsFlagged;
        }
    }

    public void Reveal()
    {
        if (IsClicked)
        {
            return;
        }

        IsClicked = true;
        _image.color = _defaultColor;
        if (_block.IsMine)
        {
            _image.sprite = BlockAssets.instance.mine;
            _gameManager.GameOver();
            return;
        }

        if (_block.Number == 0)
        {
            if (IsFlagged)
            {
                _gameManager.AddMineToText(1);
            }

            _image.sprite = BlockAssets.instance.clear;
            FindObjectOfType<BlockSpawner>().RevealClearRegion(_x, _y);
        }
        else
        {
            _image.sprite = BlockAssets.instance.numbers[_block.Number - 1];
        }

        _gameManager.DecreaseBombCount();
    }

    public void ForceReveal()
    {
        if (IsClicked)
        {
            return;
        }

        IsClicked = true;
        _image.color = _defaultColor;
        if (_block.IsMine)
        {
            _image.sprite = BlockAssets.instance.mine;
        }
        else if (_block.Number == 0)
        {
            _image.sprite = BlockAssets.instance.clear;
        }
        else
        {
            _image.sprite = BlockAssets.instance.numbers[_block.Number - 1];
        }
    }

    public void MarkHinted()
    {
        if (!IsFlagged)
        {
            _image.sprite = BlockAssets.instance.hinted;
        }
    }
}