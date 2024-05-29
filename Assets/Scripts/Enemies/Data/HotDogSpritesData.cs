using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Hot Dog Sprites Data")]

public class HotDogSpritesData : ScriptableObject
{
    public Sprite[] BunSprites;
    public Sprite[] DogSprites;
    public Sprite[] SauceSprites;
    public Sprite EmptySprite;
}