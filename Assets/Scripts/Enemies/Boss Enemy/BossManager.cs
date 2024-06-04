using System.Collections;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [SerializeField] private GameObject _bossPrefab;
    [SerializeField] private float _dialogueDur = 2f;
    [SerializeField] private float _transitionMusicVolume = 0.5f;
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private Transform _fightStartPosition;
    [SerializeField] private Transform _fightTargetPosition;
    public bool IsBossSpawned { get; private set; }
    public float TransitionMusicVolume { get => _transitionMusicVolume; }
    private bool _dialogueDisplayed = false;
    private bool _transitionInProgress = false;
    private BossEnemy _boss;

    private void Update() {
        if (_transitionInProgress && GameManager.Instance.CurrentGameState == GameManager.GameState.BossTransition && IsBossSpawned && !_dialogueDisplayed) {
            _boss.SetIsAtFightStartPos();
            if (_boss.IsAtFightStartPos) {
                StartCoroutine(TransitionRoutine());
            }        
        }
    }

    private IEnumerator TransitionRoutine() {
        _boss.DisplayIntroDialogue(_dialogueDur);
        FleeAllEnemies();
        _dialogueDisplayed = true;
        yield return new WaitForSecondsRealtime(_dialogueDur);
        
        _boss.SetMoveTargetPos(_fightTargetPosition.position);
        GameManager.Instance.LoadBoss(true);
    }

    public void StartTransition() {
        _transitionInProgress = true;
        InstantiateBossAtSpawnPoint();
        StopAllEnemies();
        HideAllHotDogs();
    }

    private void StopAllEnemies() {
        var enemies = FindObjectsOfType<NormalEnemy>();
        foreach (var enemy in enemies) {
            enemy.SetMoveTargetPos(enemy.transform.position);
            enemy.StateMachine.ChangeState(enemy.MoveState);
        }
    }

    private void FleeAllEnemies() {
        var enemies = FindObjectsOfType<NormalEnemy>();
        foreach (var enemy in enemies) {
            enemy.StateMachine.ChangeState(enemy.FleeState);
        }
    }

    private void HideAllHotDogs() {
        var hotDogs = FindObjectsOfType<HotDog>();
        foreach (var hotDog in hotDogs) {
            hotDog.HideSprite();
        }
    }

    private void InstantiateBossAtSpawnPoint() {
        _boss = Instantiate(_bossPrefab, _spawnPosition.position, Quaternion.identity).GetComponent<BossEnemy>();
        IsBossSpawned = true;
    }

    public void InstantiateBossAtFightPos() {
        _boss = Instantiate(_bossPrefab, _spawnPosition.position, Quaternion.identity).GetComponent<BossEnemy>();
        _boss.transform.position = _boss.FightStartPos;
        IsBossSpawned = true;
    }
}
