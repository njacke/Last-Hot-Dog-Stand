using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CooldownsUI : MonoBehaviour
{
    [SerializeField] private GameObject _cooldownPrefab;
    [SerializeField] private Sprite[] _bun1Sprites;
    [SerializeField] private Sprite[] _bun2Sprites;
    [SerializeField] private Sprite[] _bun3Sprites;
    [SerializeField] private Sprite[] _dog1Sprites;
    [SerializeField] private Sprite[] _dog2Sprites;
    [SerializeField] private Sprite[] _dog3Sprites;
    [SerializeField] private Sprite[] _sauce1Sprites;
    [SerializeField] private Sprite[] _sauce2Sprites;
    [SerializeField] private Sprite[] _sauce3Sprites;

    private Dictionary<Cooldown.CDType, Sprite[]> _cdTypeSpritesDict;
    private Dictionary<Cooldown.CDType, GameObject> _cdTypeGameObjects;
    private Dictionary<Enum, Cooldown.CDType> _typeConvertDict;

    private void Start() {
        InitializeCooldowns();
        InitializeTypeConvertDict();
    }

    private void OnEnable() {
        StandController.OnIngredientChange += UpdateIngredientSelection;
        StandController.OnIngredientChangeFailed += TriggerWarning;
        Cooldown.OnCooldownThresholdReached += UpdateSprite;
    }

    private void OnDisable() {
        StandController.OnIngredientChange -= UpdateIngredientSelection;
        StandController.OnIngredientChangeFailed -= TriggerWarning;
        Cooldown.OnCooldownThresholdReached -= UpdateSprite;
    }

    private void InitializeTypeConvertDict() {
        _typeConvertDict = new Dictionary<Enum, Cooldown.CDType>() {
            { HotDogDataModel.Buns.BunOne, Cooldown.CDType.BunOne },
            { HotDogDataModel.Buns.BunTwo, Cooldown.CDType.BunTwo },
            { HotDogDataModel.Buns.BunThree, Cooldown.CDType.BunThree },
            { HotDogDataModel.Buns.None, Cooldown.CDType.None },
            { HotDogDataModel.Dogs.DogOne, Cooldown.CDType.DogOne },
            { HotDogDataModel.Dogs.DogTwo, Cooldown.CDType.DogTwo },
            { HotDogDataModel.Dogs.DogThree, Cooldown.CDType.DogThree },
            { HotDogDataModel.Dogs.None, Cooldown.CDType.None },
            { HotDogDataModel.Sauces.SauceOne, Cooldown.CDType.SauceOne },
            { HotDogDataModel.Sauces.SauceTwo, Cooldown.CDType.SauceTwo },
            { HotDogDataModel.Sauces.SauceThree, Cooldown.CDType.SauceThree },
            { HotDogDataModel.Sauces.None, Cooldown.CDType.None },
        };
    }

    private void InitializeCooldowns() {
        _cdTypeSpritesDict = new Dictionary<Cooldown.CDType, Sprite[]>();
        _cdTypeGameObjects = new Dictionary<Cooldown.CDType, GameObject>();

        _cdTypeSpritesDict.Add(Cooldown.CDType.BunOne, _bun1Sprites);
        _cdTypeGameObjects.Add(Cooldown.CDType.BunOne, InstantiateSingleCD(_bun1Sprites, "Bun One CD"));

        _cdTypeSpritesDict.Add(Cooldown.CDType.BunTwo, _bun2Sprites);
        _cdTypeGameObjects.Add(Cooldown.CDType.BunTwo, InstantiateSingleCD(_bun2Sprites, "Bun Two CD"));

        _cdTypeSpritesDict.Add(Cooldown.CDType.BunThree, _bun3Sprites);
        _cdTypeGameObjects.Add(Cooldown.CDType.BunThree, InstantiateSingleCD(_bun3Sprites, "Bun Three CD"));

        _cdTypeSpritesDict.Add(Cooldown.CDType.DogOne, _dog1Sprites);
        _cdTypeGameObjects.Add(Cooldown.CDType.DogOne, InstantiateSingleCD(_dog1Sprites, "Dog One CD"));

        _cdTypeSpritesDict.Add(Cooldown.CDType.DogTwo, _dog2Sprites);
        _cdTypeGameObjects.Add(Cooldown.CDType.DogTwo, InstantiateSingleCD(_dog2Sprites, "Dog Two CD"));

        _cdTypeSpritesDict.Add(Cooldown.CDType.DogThree, _dog3Sprites);
        _cdTypeGameObjects.Add(Cooldown.CDType.DogThree, InstantiateSingleCD(_dog3Sprites, "Dog Three CD"));

        _cdTypeSpritesDict.Add(Cooldown.CDType.SauceOne, _sauce1Sprites);
        _cdTypeGameObjects.Add(Cooldown.CDType.SauceOne, InstantiateSingleCD(_sauce1Sprites, "Sauce One CD"));

        _cdTypeSpritesDict.Add(Cooldown.CDType.SauceTwo, _sauce2Sprites);
        _cdTypeGameObjects.Add(Cooldown.CDType.SauceTwo, InstantiateSingleCD(_sauce2Sprites, "Sauce Two CD"));

        _cdTypeSpritesDict.Add(Cooldown.CDType.SauceThree, _sauce3Sprites);
        _cdTypeGameObjects.Add(Cooldown.CDType.SauceThree, InstantiateSingleCD(_sauce3Sprites, "Sauce Three CD"));
    }

    private GameObject InstantiateSingleCD(Sprite[] cooldownSprites, string objectName) {
        var newCD = Instantiate(_cooldownPrefab, transform);
        newCD.name = objectName;        
        newCD.GetComponent<Image>().sprite = cooldownSprites[0];

        return newCD;
    }

    private void UpdateIngredientSelection() {
        var currentHotDogData = GameManager.Instance.StandController.CurrentHotDogData;

        Cooldown.CDType selectedBun = _typeConvertDict[currentHotDogData.Bun];
        Cooldown.CDType selectedDog = _typeConvertDict[currentHotDogData.Dog];
        Cooldown.CDType selectedSauce = _typeConvertDict[currentHotDogData.Sauce];

        var currentTypes = new List<Cooldown.CDType>() {
            selectedBun,
            selectedDog,
            selectedSauce
        };

        foreach (var item in _cdTypeGameObjects) {
            var skillSelect = item.Value.GetComponentInChildren<SkillSelectUI>();

            if (currentTypes.Contains(item.Key)) {
                skillSelect.SetSkillActive(true);
            }
            else {
                skillSelect.SetSkillActive(false);
            }
        }        
    }

    private void TriggerWarning(Enum ingredientType, string message) {
        if (_typeConvertDict.TryGetValue(ingredientType, out var cdType)) {
            if (cdType != Cooldown.CDType.None) {
                _cdTypeGameObjects[cdType].GetComponentInChildren<SkillWarningUI>().ShowWarning();
                Debug.Log(message);
            }   
            else {
                Debug.Log("Type None was passed in CooldownsUI.TriggerWarning");
            }         
        }
        else {
            Debug.Log("CooldownsUI.TriggerWarning couldn't retrieve value from dict.");
        }
    }

    private void UpdateSprite(Cooldown.CDType cdType, int spriteIndex) {
        Debug.Log(cdType.ToString());
        if (cdType != Cooldown.CDType.None) {    
            var newSprite = _cdTypeSpritesDict[cdType][spriteIndex];
            _cdTypeGameObjects[cdType].GetComponent<Image>().sprite = newSprite;
        }
    }
}