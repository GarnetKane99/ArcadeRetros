using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTileSprite : MonoBehaviour
{
    [SerializeField] private Sprite[] Alternatives;
    [SerializeField] private SpriteRenderer Sprite;

    private void Awake()
    {
        Sprite = GetComponent<SpriteRenderer>();
    }

    public void PieceToUse(int Num)
    {
        Sprite.sprite = Alternatives[Num];
        if(Num < 7)
        {
            Sprite.color = new Color(255, 255, 255, 255);
        }
        else
        {
            Sprite.color = new Color(255, 255, 255, 25f / 255f);
        }
    }
}
