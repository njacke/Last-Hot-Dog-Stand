using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotDogPreviewer : MonoBehaviour
{
    [SerializeField] private Sprite[] _bunPreviewSprites;
    [SerializeField] private Sprite[] _dogPreviewSprites;
    [SerializeField] private Sprite[] _saucePreviewSprites;
    [SerializeField] private Sprite _emptySprite;

    [SerializeField] private SpriteRenderer _bunPreviewSpriteRenderer;
    [SerializeField] private SpriteRenderer _dogPreviewSpriteRenderer;
    [SerializeField] private SpriteRenderer _saucePreviewSpriteRenderer;

    private Dictionary<HotDogDataModel.Buns, Sprite> _bunSpritesDict;
    private Dictionary<HotDogDataModel.Dogs, Sprite> _dogSpritesDict;
    private Dictionary<HotDogDataModel.Sauces, Sprite> _sauceSpritesDict;

    private StandController _standController;

    private void Awake() {
        InitializeSpriteDicts();        
    }

    private void Start() {
        _standController = FindObjectOfType<StandController>();

        UpdatePreviewSprites(); 
    }

    private void OnEnable() {
        StandController.OnIngredientChange += UpdatePreviewSprites;
    }

    private void OnDisable() {
        StandController.OnIngredientChange -= UpdatePreviewSprites;
    }

    private void InitializeSpriteDicts() {
        _bunSpritesDict = new Dictionary<HotDogDataModel.Buns, Sprite> {
            { HotDogDataModel.Buns.BunOne, _bunPreviewSprites[0] },
            { HotDogDataModel.Buns.BunTwo, _bunPreviewSprites[1] },
            { HotDogDataModel.Buns.BunThree, _bunPreviewSprites[2] },
            { HotDogDataModel.Buns.None, _emptySprite }
        };

        _dogSpritesDict = new Dictionary<HotDogDataModel.Dogs, Sprite> {
            { HotDogDataModel.Dogs.DogOne, _dogPreviewSprites[0] },
            { HotDogDataModel.Dogs.DogTwo, _dogPreviewSprites[1] },
            { HotDogDataModel.Dogs.DogThree, _dogPreviewSprites[2] },
            { HotDogDataModel.Dogs.None, _emptySprite }
        };

        _sauceSpritesDict = new Dictionary<HotDogDataModel.Sauces, Sprite> {
            { HotDogDataModel.Sauces.SauceOne, _saucePreviewSprites[0] },
            { HotDogDataModel.Sauces.SauceTwo, _saucePreviewSprites[1] },
            { HotDogDataModel.Sauces.SauceThree, _saucePreviewSprites[2] },
            { HotDogDataModel.Sauces.None, _emptySprite }
        };        
    }

    private void UpdatePreviewSprites() {
        var currentHotDogData = _standController.CurrentHotDogData;

        _bunPreviewSpriteRenderer.sprite = _bunSpritesDict[currentHotDogData.Bun];
        _dogPreviewSpriteRenderer.sprite = _dogSpritesDict[currentHotDogData.Dog];
        _saucePreviewSpriteRenderer.sprite = _sauceSpritesDict[currentHotDogData.Sauce];        
    }
}
