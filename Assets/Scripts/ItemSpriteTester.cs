using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemSpriteTester : MonoBehaviour
{
    public enum SizeMode { Grid16x16, Grid8x8 }
    public SizeMode resolution = SizeMode.Grid16x16;
    public string itemToGenerate = "chalice"; // Options: "chalice", "sunstone", "bat", "key"

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (resolution == SizeMode.Grid16x16)
            sr.sprite = ItemSpriteGenerator16x16.CreateItemSprite(itemToGenerate);
        else
            sr.sprite = ItemSpriteGenerator8x8.CreateItemSprite8x8(itemToGenerate);
    }
}