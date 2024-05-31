using System.Collections;
using UnityEngine;
using System;

public class BossEnemy : Entity
{
    [SerializeField] private MoveStateData _moveStateData;
    [SerializeField] private TasteStateData _tasteStateData;
    [SerializeField] private Transform _hotDogHolder;
    [SerializeField] private float _tasteDelay = 0.5f;
    [SerializeField] private float _biteDelay = 0.75f;
    [SerializeField] private float _finishDelay = 1f;
    [SerializeField] private float _generateNewWishDelay = .5f;

    private HotDogDataModel _currentWish =  new (HotDogDataModel.Buns.None, HotDogDataModel.Dogs.None, HotDogDataModel.Sauces.None);
    private HotDog _currentHotDog;
    private WishBubble _wishBubble;

    private readonly int ANIM_BOOL_MOVE_HASH = Animator.StringToHash("Move");
    private readonly int ANIM_BOOL_TASTE_HASH = Animator.StringToHash("Taste");
    private readonly int ANIM_TRIGGER_BITE_HASH = Animator.StringToHash("Bite");

    public bool HasHotDog { get; private set; } = false;
    public bool IsSatisfied { get; private set; } = false;

    public BoxCollider2D MyBoxCollider { get; private set; }
    public BE_MoveState MoveState { get; private set; }
    public BE_TasteState TasteState { get; private set; }

    public override void Awake()
    {
        base.Awake();

        MyBoxCollider = GetComponent<BoxCollider2D>();
    }

    public override void Start() {
        base.Start();

        _wishBubble = GetComponentInChildren<WishBubble>();

        // setup state machine
        MoveState = new BE_MoveState(this, StateMachine, ANIM_BOOL_MOVE_HASH, _moveStateData, this);  
        TasteState = new BE_TasteState(this, StateMachine, ANIM_BOOL_TASTE_HASH, _tasteStateData, this);

        StateMachine.Initialize(MoveState);

        StartCoroutine(GenerateNewWishRoutine());
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (_currentHotDog = other.GetComponent<HotDog>()) {
            if (!HasHotDog) {
                StartCoroutine(TasteHotDogRoutine(_currentHotDog));
                Debug.Log("I was hit by a Hot Dog");
            }
        }
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
        //Debug.Log("BiteAnimEvent triggered.");
        _currentHotDog.SetNextState();
    }

    private void BiteEndAnimeEvent() {
        //Debug.Log("BiteEndAnimEvent triggered");
        if (!IsSatisfied) {
            if (IsHotDogWish(_currentHotDog)) {
                IsSatisfied = true;
                WishFullfilled();
                StartCoroutine(GenerateNewWishRoutine());
            }
            else {
                _currentHotDog.DiscardProjectile();
                HasHotDog = false;
                MyBoxCollider.enabled = true;                
            }            
        }
    }

    private bool IsHotDogWish(HotDog hotDog) {
        if (hotDog.HotDogData.Bun == _currentWish.Bun && hotDog.HotDogData.Dog == _currentWish.Dog && hotDog.HotDogData.Sauce == _currentWish.Sauce) {
            return true;
        }
        return false;
    }

    private IEnumerator GenerateNewWishRoutine() {
        yield return new WaitForSeconds(_generateNewWishDelay);
        GenerateNewWish();
        _wishBubble.DisplayWish(_currentWish);

        yield return new WaitForSeconds(_wishBubble.TotalWishDisplayTime);
        MyBoxCollider.enabled = true;
    }

    private void GenerateNewWish() {

        bool isDifferent = false;

        HotDogDataModel.Buns bunWish = HotDogDataModel.Buns.None;
        HotDogDataModel.Dogs dogWish = HotDogDataModel.Dogs.None;
        HotDogDataModel.Sauces sauceWish = HotDogDataModel.Sauces.None;

        while (!isDifferent) {
            var bunsArray = Enum.GetValues(typeof(HotDogDataModel.Buns));
            var bunRNG = UnityEngine.Random.Range(1, bunsArray.Length); // 1 so None is not selected
            bunWish = (HotDogDataModel.Buns)bunsArray.GetValue(bunRNG);

            var dogsArray = Enum.GetValues(typeof(HotDogDataModel.Dogs));
            var dogRNG = UnityEngine.Random.Range(1, dogsArray.Length);
            dogWish = (HotDogDataModel.Dogs)dogsArray.GetValue(dogRNG);

            var saucesArray = Enum.GetValues(typeof(HotDogDataModel.Sauces));
            var sauceRNG = UnityEngine.Random.Range(1, saucesArray.Length);
            sauceWish = (HotDogDataModel.Sauces)saucesArray.GetValue(sauceRNG);

            if (_currentWish == null || bunWish != _currentWish.Bun || dogWish != _currentWish.Dog || sauceWish != _currentWish.Sauce) {
                    isDifferent = true;
                }
        }

        var newWishData = new HotDogDataModel(bunWish, dogWish, sauceWish);
        _currentWish = newWishData;

    }

    private void WishFullfilled() {
        _wishBubble.DisplayWishReset();
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
}

