using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private int _targetLevelScore = 10;
    [SerializeField] private int _affinitiesPerNE = 2;    
    [SerializeField] private float _playSpaceOffset = 3f;
    [SerializeField] private float _gameOverDelay = 1f;
    [SerializeField] private float _defaultMusicVolume = .8f;
    [SerializeField] private AudioSource _musicPlayer;
    private float _worldBorderX = 7.5f;
    private float _worldBorderY = 8f;
    private int _currentLevelScore = 0;
    private bool firstLvlOneLoaded = false;

    private Camera _mainCamera;
    private GameObject _menuCanvas;
    private GameObject _uiCanvas;
    private GameObject _cinematicCanvas;
    private LevelProgressUI _levelProgressUI;
    private MenuDisplayUI _menuDisplayUI;
    private CinematicUI _cinematicUI;

    private readonly string MENU_CANVAS_NAME_STRING = "Menu Canvas";
    private readonly string UI_CANVAS_NAME_STRING = "UI Canvas";
    private readonly string UI_CINEMATIC_NAME_STRING = "Cinematic Canvas";
    
    public GameState CurrentGameState { get; private set; }
    public NormalEnemy.NEType LastDeathTrigger { get; private set; } = NormalEnemy.NEType.Lawyer;
    public bool PlayerControlsLocked { get; private set; } = true;
    public bool MenuControlsLocked { get; private set; } = true;
    public Dictionary<NormalEnemy.NEType, Affinities> NEAffinitiesDict { get; private set; }
    public EnemySpawner EnemySpawner { get; private set; }
    public StandController StandController { get; private set; }
    public HotDogPreviewer HotDogPreviewer { get; private set; }
    public TurretController TurretController { get; private set; }
    public BossManager BossManager { get; private set; }
    public bool GameOverTriggered { get; private set; } = false;

    public enum GameState {
        None,
        Intro,
        Level,
        Boss,
        BossTransition,
    }

    protected override void Awake() {
        base.Awake();

        InitializeNEAffinities();
    }

    private void Start()
    {
        GameStartInitialization();
    }

    private void OnEnable() {
        CinematicUI.OnTitleDrop += PlaySoundtrack;
    }

    private void OnDisable() {
        CinematicUI.OnTitleDrop -= PlaySoundtrack;        
    }

    private void GameStartInitialization()
    {
        // UI and camera references
        _mainCamera = Camera.main;

        _menuCanvas = GameObject.Find(MENU_CANVAS_NAME_STRING);
        _uiCanvas = GameObject.Find(UI_CANVAS_NAME_STRING);
        _cinematicCanvas = GameObject.Find(UI_CINEMATIC_NAME_STRING);

        _menuDisplayUI = _menuCanvas.GetComponentInChildren<MenuDisplayUI>();
        _levelProgressUI = _uiCanvas.GetComponentInChildren<LevelProgressUI>();
        _cinematicUI = _cinematicCanvas.GetComponentInChildren<CinematicUI>();

        // Game object references
        EnemySpawner = FindObjectOfType<EnemySpawner>();
        StandController = FindObjectOfType<StandController>();
        TurretController = FindObjectOfType<TurretController>();
        HotDogPreviewer = FindObjectOfType<HotDogPreviewer>();
        BossManager = FindObjectOfType<BossManager>();

        InitializeWorldBorders();
        
        GameReset(false);

        LoadStartScreen();
    }

    private void InitializeWorldBorders() {
        if (_mainCamera != null) {
            Vector3 topRight = _mainCamera.ViewportToWorldPoint(new Vector3 (1, 1, _mainCamera.nearClipPlane));
            _worldBorderX = topRight.x;
            _worldBorderY = topRight.y;

            Debug.Log($"World borders initialized at X: {_worldBorderX} and Y: {_worldBorderY}");
        }
        else {
            Debug.Log("Failed to initialize world borders.");
        }
    }

    private void InitializeNEAffinities() {
        NEAffinitiesDict = new Dictionary<NormalEnemy.NEType, Affinities>();

        var enemyTypesList = Enum.GetValues(typeof(NormalEnemy.NEType)).Cast<NormalEnemy.NEType>().ToList();
        
        var bunsList = Enum.GetValues(typeof(HotDogDataModel.Buns)).Cast<HotDogDataModel.Buns>().Where(b => b != HotDogDataModel.Buns.None).ToList();
        var dogsList = Enum.GetValues(typeof(HotDogDataModel.Dogs)).Cast<HotDogDataModel.Dogs>().Where(d => d != HotDogDataModel.Dogs.None).ToList();
        var saucesList = Enum.GetValues(typeof(HotDogDataModel.Sauces)).Cast<HotDogDataModel.Sauces>().Where(s => s != HotDogDataModel.Sauces.None).ToList();

        // fail-safe if not enough ingredients to distribute
        //TODO: need to add test for minReq to distribute if uneven amount of ingredients among cats
        int totalIngredients = bunsList.Count + dogsList.Count + saucesList.Count;
        int reqIngredients = enemyTypesList.Count * _affinitiesPerNE;
        if (totalIngredients < reqIngredients) {
            Debug.Log("Not enough ingredients to distribute affinities.");
            return;
        }

        // RNGesus        
        ShuffleList(enemyTypesList);
        ShuffleList(bunsList);
        ShuffleList(dogsList);
        ShuffleList(saucesList);

        var allCatsList = new List<IList> {
            bunsList,
            dogsList,
            saucesList,
        };

        ShuffleList(allCatsList);

        // construct affinity for each enemy        
        foreach (var enemy in enemyTypesList) {         
            var bAffinity = HotDogDataModel.Buns.None;
            var dAffinity = HotDogDataModel.Dogs.None; 
            var sAffinity = HotDogDataModel.Sauces.None;    

            int affinitiesAssigned = 0;

            while (affinitiesAssigned < _affinitiesPerNE) {    
                // assign first element from reordered cat lists            
                foreach (var cat in allCatsList) {
                    if (affinitiesAssigned >= _affinitiesPerNE) {
                        break;
                    }

                    if (cat.Count > 0) {
                        if (cat is List<HotDogDataModel.Buns> && bAffinity == HotDogDataModel.Buns.None) {
                            bAffinity = (HotDogDataModel.Buns)cat[0];
                            cat.RemoveAt(0);
                            affinitiesAssigned++;
                        }
                        else if (cat is List<HotDogDataModel.Dogs> && dAffinity == HotDogDataModel.Dogs.None) {
                            dAffinity = (HotDogDataModel.Dogs)cat[0];
                            cat.RemoveAt(0);
                            affinitiesAssigned++;
                        }
                        else if (cat is List<HotDogDataModel.Sauces> && sAffinity == HotDogDataModel.Sauces.None) {
                            sAffinity = (HotDogDataModel.Sauces)cat[0];
                            cat.RemoveAt(0);
                            affinitiesAssigned++;
                        }
                    }
                }
            }

            NEAffinitiesDict.Add(enemy, new Affinities(bAffinity, dAffinity, sAffinity));

            // re-sort to optimize ingredient distribution so cats with more elements get assigned first 
            allCatsList = allCatsList.OrderByDescending(list => list.Count).ToList();
        }

        Debug.Log("Normal enemies affinities initilization succesfull.");
    }

    private void ShuffleList<T>(List<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    public void ConversionDone(NormalEnemy.NEType enemyType) {
        // freeze time
        // trigger game over transition
        // load game over screen (based on enemy type)
    }

    public bool IsOutsidePlaySpace(Vector3 pos) {
        if (pos.x > _worldBorderX + _playSpaceOffset) {
            return true;
        }
        if (pos.x < -_worldBorderX - _playSpaceOffset) {
            return true;
        }
        if (pos.y > _worldBorderY + _playSpaceOffset) {
            return true;
        }
        if (pos.y < -_worldBorderY - _playSpaceOffset) {
            return true;
        }
        
        return false;
    }

    public void AddScore() {
        _currentLevelScore++;
        _levelProgressUI.UpdateSprite(_currentLevelScore);

        if (_currentLevelScore == _targetLevelScore && !GameOverTriggered) {
            LoadBossTransition();
        }
    }

    private void LoadStartScreen() {
        CurrentGameState = GameState.Intro;
        _menuCanvas.SetActive(true);
        _uiCanvas.SetActive(false);
        _cinematicCanvas.SetActive(false);

        PlayerControlsLocked = true;
        MenuControlsLocked = false;

        _menuDisplayUI.DisplayMenu(MenuDisplayUI.MenuType.StartScreen);

    }

    public void LoadIntroCinematic() {
        PlayerControlsLocked = true;
        MenuControlsLocked = true;

        _menuCanvas.SetActive(false);
        _uiCanvas.SetActive(true);
        _cinematicCanvas.SetActive(true);

        _menuDisplayUI.HideAllMenus();

        _cinematicUI.PlayCinematic();       

    }

    public void LoadInstructions() {
        PlayerControlsLocked = true;
        MenuControlsLocked = false;

        _menuCanvas.SetActive(true);
        _uiCanvas.SetActive(true);
        _cinematicCanvas.SetActive(false);

        _menuDisplayUI.DisplayMenu(MenuDisplayUI.MenuType.Instructions);

    }

    public void LoadControlsIntro() {
        PlayerControlsLocked = true;
        MenuControlsLocked = false;

        _menuCanvas.SetActive(true);
        _uiCanvas.SetActive(true);
        _cinematicCanvas.SetActive(false);

        _menuDisplayUI.DisplayMenu(MenuDisplayUI.MenuType.ControlsIntro);

    }

    public void LoadControlsGame() {
        PlayerControlsLocked = true;
        MenuControlsLocked = false;

        _menuCanvas.SetActive(true);
        _uiCanvas.SetActive(true);
        _cinematicCanvas.SetActive(false);

        _menuDisplayUI.DisplayMenu(MenuDisplayUI.MenuType.ControlsGame);
    }

    public void LoadLevelOne() {
        CurrentGameState = GameState.Level;
        GameReset(false);
        
        GameOverTriggered = false;
        _currentLevelScore = 0;
        _levelProgressUI.UpdateSprite(_currentLevelScore);
        EnemySpawner.ResetES(true);

        MenuControlsLocked = true;
        PlayerControlsLocked = false;

        _uiCanvas.SetActive(true);
        _menuCanvas.SetActive(false);
        _cinematicCanvas.SetActive(false);

        if (firstLvlOneLoaded) {
            PlaySoundtrack(true, _defaultMusicVolume);
        }

        firstLvlOneLoaded = true;
    }

    public void LoadBossTransition() {
        CurrentGameState = GameState.BossTransition;
        MenuControlsLocked = true;
        PlayerControlsLocked = true;

        _uiCanvas.SetActive(true);
        _menuCanvas.SetActive(false);
        _cinematicCanvas.SetActive(false);

        BossManager.StartTransition();

        TurretController.ResetTurret();
        StandController.ResetAllCooldowns();
        StandController.ResetCurrentHotDogData();

        _musicPlayer.volume = BossManager.TransitionMusicVolume;
    }

    public void LoadBoss(bool isTransition) {
        CurrentGameState = GameState.Boss;
        GameReset(isTransition);

        GameOverTriggered = false;
        MenuControlsLocked = true;
        PlayerControlsLocked = false;

        _uiCanvas.SetActive(true);
        _menuCanvas.SetActive(false);
        _cinematicCanvas.SetActive(false);

        if (!isTransition) {
            BossManager.InstantiateBossAtFightPos();
        }

        if (!isTransition) {
            PlaySoundtrack(true, _defaultMusicVolume);
        }
        else {
            _musicPlayer.volume = _defaultMusicVolume;
        }
    }

    public void LoadGameOverFromMenu() {
        PlayerControlsLocked = true;
        MenuControlsLocked = false;

        _uiCanvas.SetActive(true);
        _menuCanvas.SetActive(true);
        _cinematicCanvas.SetActive(false);

        PlaySoundtrack(false, _defaultMusicVolume);

        _menuDisplayUI.DisplayMenu(MenuDisplayUI.MenuType.GameOver);
    }

    public void LoadGameOverFromGame(NormalEnemy.NEType enemyType) {
        StartCoroutine(LoadGameOverWithDelayRoutine(enemyType));
    }

    private IEnumerator LoadGameOverWithDelayRoutine(NormalEnemy.NEType enemyType)
    {
        PlayerControlsLocked = true;
        LastDeathTrigger = enemyType;
        GameOverTriggered = true;

        yield return new WaitForSecondsRealtime(_gameOverDelay);
        GameReset(false);

        MenuControlsLocked = false;
        _uiCanvas.SetActive(true);
        _menuCanvas.SetActive(true);
        _cinematicCanvas.SetActive(false);

        PlaySoundtrack(false, 0f);

        _menuDisplayUI.DisplayMenu(MenuDisplayUI.MenuType.GameOver);
    }

    private void GameReset(bool isTransition) {
        EnemySpawner.ResetES(false);
        TurretController.ResetTurret();
        StandController.ResetAllCooldowns();
        StandController.ResetCurrentHotDogData();
        HotDogPreviewer.UpdatePreviewSprites();
        _levelProgressUI.UpdateSprite(_currentLevelScore);
        DestroyRemainingGameObjects(isTransition);
    }

    private void DestroyRemainingGameObjects(bool isTransition) {
        var normalEnemies = FindObjectsOfType<NormalEnemy>();
        foreach (var enemy in normalEnemies) {
            Destroy(enemy.gameObject);
        }

        var hotDogs = FindObjectsOfType<HotDog>();
        foreach (var hotDog in hotDogs) {
            Destroy(hotDog.gameObject);
        }
        
        if (!isTransition) {
            var bosses = FindObjectsOfType<BossEnemy>();
            foreach (var boss in bosses) {
                Destroy(boss.gameObject);
            }       
        }
    }

    public void LoadWinScreen() {
        PlayerControlsLocked = true;
        MenuControlsLocked = false;

        _uiCanvas.SetActive(true);
        _menuCanvas.SetActive(true);
        _cinematicCanvas.SetActive(false);

        _menuDisplayUI.DisplayMenu(MenuDisplayUI.MenuType.WinScreen);
    }

    public void ReloadGame() {
        SceneManager.LoadScene(0);
    }

    private void PlaySoundtrack(bool isActive, float volume) {
        if (isActive) {
            _musicPlayer.loop = true;
            _musicPlayer.volume = volume;
            _musicPlayer.Play();
        }
        else {
            _musicPlayer.Stop();
        }
    }
}