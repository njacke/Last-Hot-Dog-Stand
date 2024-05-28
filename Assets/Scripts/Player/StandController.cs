using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Timeline;

public class StandController : MonoBehaviour
{
    [SerializeField] private float _baseCD = 2f;
    private bool _inputLocked;
    private Dictionary<HotDogDataModel.Dogs, Cooldown> _dogCooldownsDict;
    private Dictionary<HotDogDataModel.Buns, Cooldown> _bunCooldownsDict;
    private Dictionary<HotDogDataModel.Sauces, Cooldown> _sauceCooldownsDict;

    public HotDogDataModel CurrentHotDogData { get; private set; }
    public static event Action OnIngredientChange;


    private void Awake() {
        CurrentHotDogData = new HotDogDataModel(HotDogDataModel.Buns.None, HotDogDataModel.Dogs.None, HotDogDataModel.Sauces.None);

        InitializeCooldownDicts();        
    }

    private void Update() {
        PlayerInput();

        TrackCooldowns(_dogCooldownsDict);
        TrackCooldowns(_bunCooldownsDict);
        TrackCooldowns(_sauceCooldownsDict);       
    }

    private void PlayerInput() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            ChangeBun(HotDogDataModel.Buns.BunOne);
        }

        if (Input.GetKeyDown(KeyCode.W)) { 
            ChangeBun(HotDogDataModel.Buns.BunTwo);
        }

        if (Input.GetKeyDown(KeyCode.E)) { 
            ChangeBun(HotDogDataModel.Buns.BunThree); 
        }

        if (Input.GetKeyDown(KeyCode.A)) { 
            ChangeDog(HotDogDataModel.Dogs.DogOne); 
        }

        if (Input.GetKeyDown(KeyCode.S)) { 
            ChangeDog(HotDogDataModel.Dogs.DogTwo);
        }

        if (Input.GetKeyDown(KeyCode.D)) { 
            ChangeDog(HotDogDataModel.Dogs.DogThree);
        }

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Y)) {
             ChangeSauce(HotDogDataModel.Sauces.SauceOne); 
        }

        if (Input.GetKeyDown(KeyCode.X)) { 
            ChangeSauce(HotDogDataModel.Sauces.SauceTwo); 
        }

        if (Input.GetKeyDown(KeyCode.C)) { 
            ChangeSauce(HotDogDataModel.Sauces.SauceThree);
        }        
    }

    private void ChangeIngredient<T>(T ingredientType, Dictionary<T, Cooldown> cooldowns, Action<T> setCurrentIngredient) {
        if (cooldowns.TryGetValue(ingredientType, out var cooldown) && !cooldown.IsOnCooldown) {
            setCurrentIngredient(ingredientType);
            cooldown.ResetCooldown();
            Debug.Log($"New ingredient selected: {ingredientType}");
        }
        else {
            Debug.Log("Selected ingredient is on CD/nothing was selected.");
        }

        OnIngredientChange?.Invoke();
    }

    private void ChangeBun(HotDogDataModel.Buns bunType) {
        ChangeIngredient(bunType, _bunCooldownsDict, ingredient => CurrentHotDogData.Bun = ingredient);
    }

    private void ChangeDog(HotDogDataModel.Dogs dogType) {
        if (CurrentHotDogData.Bun == HotDogDataModel.Buns.None) {
            Debug.Log("Pick a bun first!");
            return;
        }

        ChangeIngredient(dogType, _dogCooldownsDict, ingredient => CurrentHotDogData.Dog = ingredient);
    }

    private void ChangeSauce(HotDogDataModel.Sauces sauceType) {
        if (CurrentHotDogData.Dog == HotDogDataModel.Dogs.None) {
            Debug.Log("Pick a sausage first!");
            return;
        }
        ChangeIngredient(sauceType, _sauceCooldownsDict, ingredient => CurrentHotDogData.Sauce = ingredient);
    }


    private void InitializeCooldownDicts() {
        _dogCooldownsDict = new Dictionary<HotDogDataModel.Dogs, Cooldown> {
            { HotDogDataModel.Dogs.DogOne, new Cooldown(_baseCD) },
            { HotDogDataModel.Dogs.DogTwo, new Cooldown(_baseCD) },
            { HotDogDataModel.Dogs.DogThree, new Cooldown(_baseCD) }
        };

        _bunCooldownsDict = new Dictionary<HotDogDataModel.Buns, Cooldown> {
            { HotDogDataModel.Buns.BunOne, new Cooldown(_baseCD) },
            { HotDogDataModel.Buns.BunTwo, new Cooldown(_baseCD) },
            { HotDogDataModel.Buns.BunThree, new Cooldown(_baseCD) }
        };

        _sauceCooldownsDict = new Dictionary<HotDogDataModel.Sauces, Cooldown> {
            { HotDogDataModel.Sauces.SauceOne, new Cooldown(_baseCD) },
            { HotDogDataModel.Sauces.SauceTwo, new Cooldown(_baseCD) },
            { HotDogDataModel.Sauces.SauceThree, new Cooldown(_baseCD) }
        };
    }

    private void TrackCooldowns<T>(Dictionary<T, Cooldown> cooldowns) {
        foreach (var cooldown in cooldowns.Values) {
            cooldown.TrackCooldown();
        }
    }
    public void ResetCurrentHotDogData() {
        CurrentHotDogData.Bun = HotDogDataModel.Buns.None;
        CurrentHotDogData.Dog = HotDogDataModel.Dogs.None;
        CurrentHotDogData.Sauce = HotDogDataModel.Sauces.None;

        OnIngredientChange?.Invoke();
    }

    public bool IsHotDogComplete() {
        if (CurrentHotDogData.Bun != HotDogDataModel.Buns.None && CurrentHotDogData.Dog != HotDogDataModel.Dogs.None && CurrentHotDogData.Sauce != HotDogDataModel.Sauces.None) {
            return true;
        }
        return false;
    }
}
