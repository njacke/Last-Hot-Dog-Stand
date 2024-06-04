using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Timeline;

public class StandController : MonoBehaviour
{
    [SerializeField] private float _baseCD = 2f;
    private Dictionary<HotDogDataModel.Buns, Cooldown> _bunsCooldownsDict;
    private Dictionary<HotDogDataModel.Dogs, Cooldown> _dogsCooldownsDict;
    private Dictionary<HotDogDataModel.Sauces, Cooldown> _saucesCooldownsDict;

    public HotDogDataModel CurrentHotDogData { get; private set; }
    public static event Action OnIngredientChange;
    public static event Action <Enum, WarningDisplayUI.WarningType> OnIngredientChangeFailed;


    private void Awake() {
        CurrentHotDogData = new HotDogDataModel(HotDogDataModel.Buns.None, HotDogDataModel.Dogs.None, HotDogDataModel.Sauces.None);

        InitializeCooldownDicts();        
    }

    private void Update() {
        if (!GameManager.Instance.PlayerControlsLocked) {
            PlayerInput();
        }

        TrackCooldowns(_dogsCooldownsDict);
        TrackCooldowns(_bunsCooldownsDict);
        TrackCooldowns(_saucesCooldownsDict);       
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

    private void ChangeIngredient<T>(T ingredientType, Dictionary<T, Cooldown> cooldowns, Action<T> setCurrentIngredient) where T : Enum {
        if (cooldowns.TryGetValue(ingredientType, out var cooldown) && !cooldown.IsOnCooldown) {
            setCurrentIngredient(ingredientType);
            OnIngredientChange?.Invoke();
            //Debug.Log($"New ingredient selected: {ingredientType}");
        }
        else if (cooldown.IsOnCooldown) {
            OnIngredientChangeFailed?.Invoke(ingredientType, WarningDisplayUI.WarningType.OnCD);
        }
    }

    private void ChangeBun(HotDogDataModel.Buns bunType) {
        if (CurrentHotDogData.Bun != HotDogDataModel.Buns.None) {
            OnIngredientChangeFailed?.Invoke(bunType, WarningDisplayUI.WarningType.BunSelected);
            return;
        }

        ChangeIngredient(bunType, _bunsCooldownsDict, ingredient => CurrentHotDogData.Bun = ingredient);
    }

    private void ChangeDog(HotDogDataModel.Dogs dogType) {
        if (CurrentHotDogData.Bun == HotDogDataModel.Buns.None) {
            OnIngredientChangeFailed?.Invoke(dogType, WarningDisplayUI.WarningType.BunPick);
            return;
        }

        if (CurrentHotDogData.Dog != HotDogDataModel.Dogs.None) {
            OnIngredientChangeFailed?.Invoke(dogType, WarningDisplayUI.WarningType.DogSelected);
            return;
        }

        ChangeIngredient(dogType, _dogsCooldownsDict, ingredient => CurrentHotDogData.Dog = ingredient);
    }

    private void ChangeSauce(HotDogDataModel.Sauces sauceType) {
        if (CurrentHotDogData.Dog == HotDogDataModel.Dogs.None) {
            OnIngredientChangeFailed?.Invoke(sauceType, WarningDisplayUI.WarningType.DogPick);
            return;
        }

        if (CurrentHotDogData.Sauce != HotDogDataModel.Sauces.None) {
            OnIngredientChangeFailed?.Invoke(sauceType, WarningDisplayUI.WarningType.SauceSelected);
            return;
        }

        ChangeIngredient(sauceType, _saucesCooldownsDict, ingredient => CurrentHotDogData.Sauce = ingredient);
    }


    private void InitializeCooldownDicts() {
        _bunsCooldownsDict = new Dictionary<HotDogDataModel.Buns, Cooldown> {
            { HotDogDataModel.Buns.BunOne, new Cooldown(Cooldown.CDType.BunOne, _baseCD) },
            { HotDogDataModel.Buns.BunTwo, new Cooldown(Cooldown.CDType.BunTwo, _baseCD) },
            { HotDogDataModel.Buns.BunThree, new Cooldown(Cooldown.CDType.BunThree, _baseCD) }
        };

        _dogsCooldownsDict = new Dictionary<HotDogDataModel.Dogs, Cooldown> {
            { HotDogDataModel.Dogs.DogOne, new Cooldown(Cooldown.CDType.DogOne, _baseCD) },
            { HotDogDataModel.Dogs.DogTwo, new Cooldown(Cooldown.CDType.DogTwo, _baseCD) },
            { HotDogDataModel.Dogs.DogThree, new Cooldown(Cooldown.CDType.DogThree, _baseCD) }
        };


        _saucesCooldownsDict = new Dictionary<HotDogDataModel.Sauces, Cooldown> {
            { HotDogDataModel.Sauces.SauceOne, new Cooldown(Cooldown.CDType.SauceOne, _baseCD) },
            { HotDogDataModel.Sauces.SauceTwo, new Cooldown(Cooldown.CDType.SauceTwo, _baseCD) },
            { HotDogDataModel.Sauces.SauceThree, new Cooldown(Cooldown.CDType.SauceThree, _baseCD) }
        };
    }

    private void TrackCooldowns<T>(Dictionary<T, Cooldown> cooldowns) {
        foreach (var cooldown in cooldowns.Values) {
            cooldown.TrackCooldown();
        }
    }

    public void ResetAllCooldowns() {
        foreach (var cd in _bunsCooldownsDict) {
            cd.Value.ResetCooldown();
        }
        foreach (var cd in _dogsCooldownsDict) {
            cd.Value.ResetCooldown();
        }
        foreach (var cd in _saucesCooldownsDict) {
            cd.Value.ResetCooldown();
        }
    }
    
    public void ResetCurrentHotDogData() {
        CurrentHotDogData.Bun = HotDogDataModel.Buns.None;
        CurrentHotDogData.Dog = HotDogDataModel.Dogs.None;
        CurrentHotDogData.Sauce = HotDogDataModel.Sauces.None;
        CurrentHotDogData.State = HotDogDataModel.EatenState.Full;

        OnIngredientChange?.Invoke();
    }

    public bool IsHotDogComplete() {
        if (CurrentHotDogData.Bun != HotDogDataModel.Buns.None && CurrentHotDogData.Dog != HotDogDataModel.Dogs.None && CurrentHotDogData.Sauce != HotDogDataModel.Sauces.None) {
            return true;
        }
        return false;
    }

    public void StartAllCooldowns() {
        if (IsHotDogComplete()) {
            _bunsCooldownsDict[CurrentHotDogData.Bun].StartCooldown();
            _dogsCooldownsDict[CurrentHotDogData.Dog].StartCooldown();
            _saucesCooldownsDict[CurrentHotDogData.Sauce].StartCooldown();
        }
    }
}
