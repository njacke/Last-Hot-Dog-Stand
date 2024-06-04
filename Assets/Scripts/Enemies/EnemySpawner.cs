using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int _maxEnemiesPerLane = 1;
    [SerializeField] float _minSpawnDelay = 1f;
    [SerializeField] float _maxSpawnDelay = 2f;
    [SerializeField] Transform[] _spawnPoints;
    [SerializeField] Transform[] _targetPoints;
    [SerializeField] GameObject[] _enemies;
    private int _lanesTotal = 0;
    private float _timeToSpawnRemaining = 0f;
    private Dictionary<int, Vector3> _lanesIndexSpawnsDict;
    private Dictionary<int, Vector3> _lanesIndexTargetsDict;
    private Dictionary<int, int> _lanesIndexActiveEnemiesDict;

    public bool IsEnabled { get; private set; } = false;

    private void Update() {
        if (IsEnabled) {
            SpawnEnemyWhenReady();
        }
    }

    private void InitializeLanes() {
        _lanesTotal = Mathf.Min(_spawnPoints.Length, _targetPoints.Length);
        if (_lanesTotal <= 0) {
            Debug.Log("Lanes initialization failed. Lanes total is too low: " + _lanesTotal);
            return;
        }

        _lanesIndexSpawnsDict = new Dictionary<int, Vector3>();
        _lanesIndexTargetsDict = new Dictionary<int, Vector3>();
        _lanesIndexActiveEnemiesDict = new Dictionary<int, int>();

        for (int i = 0; i < _lanesTotal; i++) {
            _lanesIndexSpawnsDict.Add(i, _spawnPoints[i].position);
            _lanesIndexTargetsDict.Add(i, _targetPoints[i].position);
            _lanesIndexActiveEnemiesDict.Add(i, 0);            
        }

        Debug.Log("Lanes initialization finished. Lanes total: " + _lanesTotal);
    }

    private void SpawnEnemyWhenReady() {
        _timeToSpawnRemaining -= Time.deltaTime;
        
        if (_timeToSpawnRemaining <= 0f) {
            if (SpawnRndEnemyOnRndLane()) {
                _timeToSpawnRemaining = GetRandomSpawnTime();
            }
        }        
    }

    private float GetRandomSpawnTime() {
        return UnityEngine.Random.Range(_minSpawnDelay, _maxSpawnDelay);
    }


    private bool SpawnRndEnemyOnRndLane() {
        var availableLanes = _lanesIndexActiveEnemiesDict.Where(p => p.Value < _maxEnemiesPerLane).Select(p => p.Key).ToList();

        //Debug.Log($"There are {availableLanes.Count} available lanes.");

        if (availableLanes.Count == 0) {
            //Debug.Log("No available lanes to spawn on.");
            return false;
        }
        
        var laneToSpawnOnIndex = availableLanes[UnityEngine.Random.Range(0, availableLanes.Count)];
        var enemyToSpawnIndex = UnityEngine.Random.Range(0, _enemies.Length);

        if (SpawnEnemy(laneToSpawnOnIndex, enemyToSpawnIndex)) {
            return true;
        }
        return false;
    }

    private bool SpawnEnemy(int laneIndex, int enemyIndex) {
        if (laneIndex > _lanesTotal || enemyIndex > _enemies.Length) {
            Debug.Log("SpawnEnemy failed. Lane or enemy index not in range");
            return false;
        }

        var newEnemy = Instantiate(_enemies[enemyIndex], _lanesIndexSpawnsDict[laneIndex], Quaternion.identity).GetComponent<NormalEnemy>();
        newEnemy.SetLaneAssigned(laneIndex);
        newEnemy.SetMoveTargetPos(_lanesIndexTargetsDict[laneIndex]);

        _lanesIndexActiveEnemiesDict[laneIndex]++;

        //Debug.Log("Enemy spawned on lane: " + laneIndex);

        return true;
    }

    public void ClearEnemyFromLane(int laneIndex) {
        _lanesIndexActiveEnemiesDict[laneIndex]--;

        //Debug.Log("Enemy cleared form lane: " + laneIndex);
    }

    public void ResetES(bool turnOn) {
        IsEnabled = turnOn;
        if (turnOn) {
            InitializeLanes();
            _timeToSpawnRemaining = GetRandomSpawnTime();
        }   
    }
}
