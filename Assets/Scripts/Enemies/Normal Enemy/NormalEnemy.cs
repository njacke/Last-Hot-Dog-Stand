using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class NormalEnemy : Entity
{
    [SerializeField] private MoveStateData _moveStateData;
    [SerializeField] private ConvertStateData _convertStateData;
    [SerializeField] private FleeStateData _fleeStateData;
    [SerializeField] private TasteStateData _tasteStateData;
    [SerializeField] private Transform _hotDogHolder;
    [SerializeField] private float _tasteDelay = 0.5f;
    [SerializeField] private float _biteDelay = 0.75f;
    [SerializeField] private float _finishDelay = 1f;
    [SerializeField] private NEType _enemyType; 

    private FeedbackBubble _feedbackBubble;
    private ConvertBar _convertBar;
    private HotDog _currentHotDog;
    private GameManager _gameManager;

    private readonly int ANIM_BOOL_MOVE_HASH = Animator.StringToHash("Move");
    private readonly int ANIM_BOOL_CONVERT_HASH = Animator.StringToHash("Convert");
    private readonly int ANIM_BOOL_FLEE_HASH = Animator.StringToHash("Flee");
    private readonly int ANIM_BOOL_TASTE_HASH = Animator.StringToHash("Taste");
    private readonly int ANIM_TRIGGER_BITE_HASH = Animator.StringToHash("Bite");

    public bool HasHotDog { get; private set; } = false;
    public bool IsSatisfied { get; private set; } = false;
    public bool IsInConversionRange { get; private set; } = false;
    public bool IsConverting { get; private set; } = false;

    public BoxCollider2D MyBoxCollider { get; private set; }
    public NE_MoveState MoveState { get; private set; }
    public NE_ConvertState ConvertState { get; private set; }
    public NE_FleeState FleeState { get; private set; }
    public NE_TasteState TasteState { get; private set; }

    public enum NEType {
        Businessman,
        Lawyer,
        Critic,
        Inspector
    }

    public override void Awake()
    {
        base.Awake();

        MyBoxCollider = GetComponent<BoxCollider2D>();
        _feedbackBubble = GetComponentInChildren<FeedbackBubble>();
        _convertBar = GetComponentInChildren<ConvertBar>();
    }

    public override void Start() {
        base.Start();

        _convertBar.gameObject.SetActive(false);
        MoveTargetPos = GetMoveTargetPos();

        MoveState = new NE_MoveState(this, StateMachine, ANIM_BOOL_MOVE_HASH, _moveStateData, this);  
        ConvertState = new NE_ConvertState(this, StateMachine, ANIM_BOOL_CONVERT_HASH, _convertStateData, this); 
        FleeState = new NE_FleeState (this, StateMachine, ANIM_BOOL_FLEE_HASH, _fleeStateData, this);
        TasteState = new NE_TasteState(this, StateMachine, ANIM_BOOL_TASTE_HASH, _tasteStateData, this);

        StateMachine.Initialize(MoveState);

        _gameManager = FindObjectOfType<GameManager>();
    }

    public override void Update() {
        base.Update();

        // set conversion bool
        if (StateMachine.CurrentState != ConvertState) {
            IsInConversionRange = IsInConvertRange();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (_currentHotDog = other.GetComponent<HotDog>()) {
            if (!HasHotDog) {
                StartCoroutine(TasteHotDogRoutine(_currentHotDog));
                Debug.Log("I was hit by a Hot Dog");
            }
            StateMachine.ChangeState(TasteState);
        }
    }

    //TODO: finish with enemy spawner
    private Vector3 GetMoveTargetPos() {
        Vector3 turretPosition = FindObjectOfType<TurretController>().transform.position;        
        return new Vector3 (turretPosition.x, turretPosition.y + 1.5f, + turretPosition.z);
    }

    private IEnumerator TasteHotDogRoutine(HotDog hotDog) {
        HasHotDog = true;
        MyBoxCollider.enabled = false;
        hotDog.MyCapsuleCollider.enabled = false;
        hotDog.HasHit = true;
        hotDog.transform.SetParent(_hotDogHolder);
        StartCoroutine(hotDog.MoveToParentRoutine());

        yield return new WaitForSeconds(_tasteDelay);

        SetAnimBiteTrigger();        
    }

    private IEnumerator FinishHotDogRoutine() {
        yield return new WaitForSeconds(_finishDelay);
        SetAnimBiteTrigger();

        yield return new WaitForSeconds(_biteDelay);
        SetAnimBiteTrigger();
    }

    // triggered at top of bite animation
    private void BiteAnimEvent() {
        Debug.Log("BiteAnimEvent triggered.");
        _currentHotDog.SetNextState();
    }

    private void BiteEndAnimeEvent() {
        Debug.Log("BiteEndAnimEvent triggered");
        if (!IsSatisfied) {
            if (HasAffinityIngredient(_currentHotDog)) {
                IsSatisfied = true;
                _feedbackBubble.DisplayFeedback(true);
            }
            else {
                _feedbackBubble.DisplayFeedback(false);                
                StartCoroutine(ThrowHotDogRoutine(_currentHotDog));
                HasHotDog = false;
                MyBoxCollider.enabled = true;                
            }            
        }
    }

    private IEnumerator ThrowHotDogRoutine(HotDog hotDog) {
        //TODO: finish method
        yield return new WaitForSeconds(1f);
        Destroy(hotDog.gameObject);        
    }

    private bool HasAffinityIngredient(HotDog hotDog) {
        if (hotDog.HotDogData.Bun == _gameManager.NEAffinitiesDict[_enemyType].BunAffinity) {
            return true;
        }        
        if (hotDog.HotDogData.Dog == _gameManager.NEAffinitiesDict[_enemyType].DogAffinity) {
            return true;
        }
        if (hotDog.HotDogData.Sauce == _gameManager.NEAffinitiesDict[_enemyType].SauceAffinity) {
            return true;
        }

        return false;
    }

    public void TasteHotDog() {
        StartCoroutine(TasteHotDogRoutine(_currentHotDog));
    }

    public void FinishHotDog() {
        StartCoroutine(FinishHotDogRoutine());
    }

    public void SetAnimBiteTrigger() {
        Anim.SetTrigger(ANIM_TRIGGER_BITE_HASH);
    }

    public bool IsInConvertRange() {
        float distToTarget = Vector3.Distance(this.transform.position, MoveTargetPos);
        if (distToTarget <= _convertStateData.MinConversionRange) {
            return true;
        }

        return false;
    }

    public void StartConverting() {
        IsConverting = true;
        _convertBar.gameObject.SetActive(true);
    }

    public void StopConverting() {
        IsConverting = false;
        _convertBar.gameObject.SetActive(false);
    }

    public void AddConversionProgress() {
        if (_convertBar.IsProgressCompleted) {
            //call game over
            return;
        }

        _convertBar.AddProgress();        
    }
}
