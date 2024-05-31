using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private int _targetLevelScore = 10;
    [SerializeField] private int _affinitiesPerNE = 2;    
    [SerializeField] private float _playSpaceOffset = 3f;
    private float _worldBorderX = 7.5f;
    private float _worldBorderY = 8f;
    private int _currentLevelScore = 0;

    private Camera _mainCamera;
    private LevelProgressUI _levelProgressUI;
    
    public Dictionary<NormalEnemy.NEType, Affinities> NEAffinitiesDict { get; private set; }
    public EnemySpawner EnemySpawner { get; private set; }
    public StandController StandController { get; private set; }
    public HotDogPreviewer HotDogPreviewer { get; private set; }

    protected override void Awake() {
        base.Awake();

        InitializeNEAffinities();
    }

    private void Start() {        
        _mainCamera = Camera.main;
        _levelProgressUI = FindObjectOfType<LevelProgressUI>();
        EnemySpawner = FindObjectOfType<EnemySpawner>();
        StandController = FindObjectOfType<StandController>();
        HotDogPreviewer = FindObjectOfType<HotDogPreviewer>();

        InitializeWorldBorders();
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

        if (_currentLevelScore == _targetLevelScore) {
            // transition to boss routine
        }        
    }
}