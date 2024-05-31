using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AffinityUI : MonoBehaviour
{
    [SerializeField] private Sprite _unrevealedSprite;
    [SerializeField] private Sprite _bun1Sprite;
    [SerializeField] private Sprite _bun2Sprite;
    [SerializeField] private Sprite _bun3Sprite;
    [SerializeField] private Sprite _dog1Sprite;
    [SerializeField] private Sprite _dog2Sprite;
    [SerializeField] private Sprite _dog3Sprite;
    [SerializeField] private Sprite _sauce1Sprite;
    [SerializeField] private Sprite _sauce2Sprite;
    [SerializeField] private Sprite _sauce3Sprite;

    public bool IsRevealed { get; private set; } = false;

    private Image _image;
    private Dictionary<Enum, Sprite> _ingredientTypeSpriteDict;

    private void Awake() {
        _image = GetComponent<Image>();
        _image.sprite = _unrevealedSprite;

        InitializeDict();        
    }

    private void InitializeDict() {
        _ingredientTypeSpriteDict = new Dictionary<Enum, Sprite>() {
            {HotDogDataModel.Buns.BunOne, _bun1Sprite},
            {HotDogDataModel.Buns.BunTwo, _bun2Sprite},
            {HotDogDataModel.Buns.BunThree, _bun3Sprite},

            {HotDogDataModel.Dogs.DogOne, _dog1Sprite},
            {HotDogDataModel.Dogs.DogTwo, _dog2Sprite},
            {HotDogDataModel.Dogs.DogThree, _dog3Sprite},

            {HotDogDataModel.Sauces.SauceOne, _sauce1Sprite},
            {HotDogDataModel.Sauces.SauceTwo, _sauce2Sprite},
            {HotDogDataModel.Sauces.SauceThree, _sauce3Sprite},
        };
    }

    public void RevealAffinity(Enum ingredientType) {
        Debug.Log("RevealAffinity called on " + this.transform.parent.name);
        if (_ingredientTypeSpriteDict.TryGetValue(ingredientType, out var newSprite)) {
            _image.sprite = newSprite;
            IsRevealed = true;
        }
        else {
            Debug.Log("Invalid affinity sprite ingredient type: " + ingredientType.ToString());
        }
    }

    public void HideAffinity() {
        _image.sprite = _unrevealedSprite;
        IsRevealed = false;
    }
}
