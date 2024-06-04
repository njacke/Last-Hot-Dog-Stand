using System;
using System.Collections;
using UnityEngine;

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
    [SerializeField] private int _hideLayerOrder  = -1; 
    [SerializeField] private NEType _enemyType; 

    private SpriteRenderer _spriteRenderer;
    private FeedbackBubble _feedbackBubble;
    private ConvertBar _convertBar;

    private readonly int ANIM_BOOL_MOVE_HASH = Animator.StringToHash("Move");
    private readonly int ANIM_BOOL_CONVERT_HASH = Animator.StringToHash("Convert");
    private readonly int ANIM_BOOL_FLEE_HASH = Animator.StringToHash("Flee");
    private readonly int ANIM_BOOL_TASTE_HASH = Animator.StringToHash("Taste");
    private readonly int ANIM_BOOL_BITE_HASH = Animator.StringToHash("Bite");
    private readonly int ANIM_BOOL_EAT_HASH = Animator.StringToHash("Eat");

    public HotDog CurrentHotDog {get; set; }
    public int LaneAssigned { get; private set; }
    public bool HasHotDog { get; private set; } = false;
    public bool IsSatisfied { get; private set; } = false;
    public bool IsConverting { get; private set; } = false;

    public BoxCollider2D MyBoxCollider { get; private set; }
    public NE_MoveState MoveState { get; private set; }
    public NE_ConvertState ConvertState { get; private set; }
    public NE_FleeState FleeState { get; private set; }
    public NE_TasteState TasteState { get; private set; }

    public static event Action<NormalEnemy.NEType, Enum> OnIngredientTasted;

    public enum NEType {
        Lawyer,
        Businessman,
        Critic,
        Inspector,
    }

    public override void Awake()
    {
        base.Awake();

        MyBoxCollider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _feedbackBubble = GetComponentInChildren<FeedbackBubble>();
        _convertBar = GetComponentInChildren<ConvertBar>();
    }

    public override void Start() {
        base.Start();

        // flip sprite
        if (this.transform.position.x < 0f) {
            GetComponent<SpriteRenderer>().flipX = true;
        }

        _convertBar.gameObject.SetActive(false);

        // setup state machine
        MoveState = new NE_MoveState(this, StateMachine, ANIM_BOOL_MOVE_HASH, _moveStateData, this);  
        ConvertState = new NE_ConvertState(this, StateMachine, ANIM_BOOL_CONVERT_HASH, _convertStateData, this); 
        FleeState = new NE_FleeState (this, StateMachine, ANIM_BOOL_FLEE_HASH, _fleeStateData, this);
        TasteState = new NE_TasteState(this, StateMachine, ANIM_BOOL_TASTE_HASH, _tasteStateData, this);

        StateMachine.Initialize(MoveState);

    }

    public override void Update() {
        base.Update();

        if (GameManager.Instance.IsOutsidePlaySpace(this.transform.position)) {
            //TODO: return to pool when I have pools
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (CurrentHotDog = other.GetComponent<HotDog>()) {
            if (!HasHotDog) {
                HasHotDog = true;
                Debug.Log("I was hit by a Hot Dog");
            }
        }
    }

    private IEnumerator TasteHotDogRoutine(HotDog hotDog) {
        MyBoxCollider.enabled = false;
        hotDog.MyCapsuleCollider.enabled = false;
        hotDog.HasHit = true;
        hotDog.transform.SetParent(_hotDogHolder);
        StartCoroutine(hotDog.MoveToParentRoutine());

        yield return new WaitForSeconds(_tasteDelay);

        Anim.SetBool(ANIM_BOOL_BITE_HASH, true);
    }

    private IEnumerator FinishHotDogRoutine() {
        yield return new WaitForSeconds(_finishDelay);
        Anim.SetBool(ANIM_BOOL_BITE_HASH, true);

        yield return new WaitForSeconds(_biteDelay);
        Anim.SetBool(ANIM_BOOL_BITE_HASH, true);
    }

    // triggered at top of bite animation
    private void BiteAnimEvent() {
        //Debug.Log("BiteAnimEvent triggered.");
        CurrentHotDog.SetNextState();
    }

    private void BiteEndAnimEvent() {
        //Debug.Log("BiteEndAnimEvent triggered");
        if (!IsSatisfied && GameManager.Instance.CurrentGameState == GameManager.GameState.Level) {
            if (HasAffinityIngredient(CurrentHotDog)) {
                IsSatisfied = true;
                _feedbackBubble.DisplayFeedback(true);
                GameManager.Instance.AddScore();
            }
            else {
                _feedbackBubble.DisplayFeedback(false);                
                CurrentHotDog.DiscardProjectile();
                HasHotDog = false;
                MyBoxCollider.enabled = true;                
            }
        }
        Anim.SetBool(ANIM_BOOL_BITE_HASH, false);
    }

    private bool HasAffinityIngredient(HotDog hotDog) {
        bool isSatisfied = false;

        if (hotDog.HotDogData.Bun == GameManager.Instance.NEAffinitiesDict[_enemyType].BunAffinity) {
            isSatisfied = true;
            OnIngredientTasted?.Invoke(_enemyType, hotDog.HotDogData.Bun);
        }        
        if (hotDog.HotDogData.Dog == GameManager.Instance.NEAffinitiesDict[_enemyType].DogAffinity) {
            isSatisfied = true;
            OnIngredientTasted?.Invoke(_enemyType, hotDog.HotDogData.Dog);
        }
        if (hotDog.HotDogData.Sauce == GameManager.Instance.NEAffinitiesDict[_enemyType].SauceAffinity) {
            isSatisfied = true;
            OnIngredientTasted?.Invoke(_enemyType, hotDog.HotDogData.Sauce);
        }

        return isSatisfied;
    }

    public void TasteHotDog() {
        StartCoroutine(TasteHotDogRoutine(CurrentHotDog));
    }

    public void FinishHotDog() {
        StartCoroutine(FinishHotDogRoutine());
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
        if (_convertBar.IsProgressCompleted && !GameManager.Instance.GameOverTriggered) {
            GameManager.Instance.LoadGameOverFromGame( _enemyType);
        }

        _convertBar.AddProgress();        
    }

    public void SetLaneAssigned(int laneIndex) {
        LaneAssigned = laneIndex;
    }

    public void HideSprite() {
        _spriteRenderer.sortingOrder = _hideLayerOrder;
    }
}
