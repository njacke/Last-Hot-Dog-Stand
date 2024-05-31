using System;
using UnityEngine;
using UnityEngine.UI;

public class WarningDisplayUI : MonoBehaviour
{
    [SerializeField] private float _maxWarningTime = 0.5f;
    [SerializeField] private Sprite _spriteCD;
    [SerializeField] private Sprite _spriteBunS;
    [SerializeField] private Sprite _spriteDogS;
    [SerializeField] private Sprite _spriteSauceS;
    [SerializeField] private Sprite _spriteBunP;
    [SerializeField] private Sprite _spriteDogP;
    private float _timeSinceLastWarning = 0f;  
    private Image _image;  


    public enum WarningType {
        None,
        OnCD,
        BunSelected,
        DogSelected,
        SauceSelected,
        BunPick,
        DogPick,
    }


    private void Awake() {
        _image = GetComponent<Image>();
        _image.enabled = false;        
    }

    private void Update() {
        _timeSinceLastWarning += Time.deltaTime;        
        if (_timeSinceLastWarning > _maxWarningTime && _image.enabled) {
            _image.enabled = false;
        }
    }


    private void OnEnable() {
        StandController.OnIngredientChangeFailed += UpdateWarning;
    }

    private void OnDisable() {
        StandController.OnIngredientChangeFailed -= UpdateWarning;
    }

    private void UpdateWarning(Enum ingredientType, WarningDisplayUI.WarningType warningType) {
        Sprite newWarningSprite = null;

        switch (warningType) {
            case WarningType.OnCD:
                newWarningSprite = _spriteCD;
                break;
            case WarningType.BunSelected:
                newWarningSprite = _spriteBunS;
                break;
            case WarningType.DogSelected:
                newWarningSprite = _spriteDogS;
                break;
            case WarningType.SauceSelected:
                newWarningSprite = _spriteSauceS;
                break;
            case WarningType.BunPick:
                newWarningSprite = _spriteBunP;
                break;
            case WarningType.DogPick:
                newWarningSprite = _spriteDogP;
                break;
            default:
                break;
        }

        if (newWarningSprite != null) {
            _image.enabled = true;
            _image.sprite = newWarningSprite;
            _timeSinceLastWarning = 0f;
        }
    }
}