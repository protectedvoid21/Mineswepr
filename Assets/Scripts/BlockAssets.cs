using UnityEngine;

public class BlockAssets : MonoBehaviour
{
    public Sprite defaultBlock;
    public Sprite clear;
    public Sprite mine;
    public Sprite flagged;
    public Sprite hinted;
    public Sprite[] numbers;

    public static BlockAssets instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}