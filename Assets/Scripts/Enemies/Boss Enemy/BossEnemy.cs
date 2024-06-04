using System.Collections;
using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Runtime.InteropServices;

public class BossEnemy : Entity
{
    [SerializeField] private MoveStateData _moveStateData;
    [SerializeField] private TasteStateData _tasteStateData;
    [SerializeField] private Transform _hotDogHolder;
    [SerializeField] private float _tasteDelay = 0.5f;
    //[SerializeField] private float _biteDelay = 0.75f;
    //[SerializeField] private float _finishDelay = 1f;
    [SerializeField] private float _generateNewWishDelay = .5f;
    [SerializeField] private float _fightStartPosY = 4.3125f;
    [SerializeField] private float _fightTargetPosY = -1f;
    [SerializeField] private float _deathRoutineDelay = 0.25f;
    [SerializeField] private float _winScreenDelay = 2f;
    [SerializeField] private Sprite[] _bossSprites;
    [SerializeField] private Sprite[] _deathSprites;
    [SerializeField] private SpriteRenderer _dialogueBoxSpriteRenderer;

    private int _wishFulfilledCount = 0; 
    private HotDogDataModel _currentWish =  new (HotDogDataModel.Buns.None, HotDogDataModel.Dogs.None, HotDogDataModel.Sauces.None);
    private HotDog _currentHotDog;
    private WishBubble _wishBubble;
    private SpriteRenderer _spriteRenderer;

    private readonly int ANIM_BOOL_MOVE_HASH = Animator.StringToHash("Move");
    private readonly int ANIM_BOOL_TASTE_HASH = Animator.StringToHash("Taste");
    private readonly int ANIM_BOOL_BITE_HASH = Animator.StringToHash("Bite");
    private readonly int ANIM_BOOL_EAT_HASH = Animator.StringToHash("Eat");
    private readonly int ANIM_BOOL_VOMMIT_HASH = Animator.StringToHash("Vommit");
    private readonly int ANIM_TRIGGER_DAETH_HASH = Animator.StringToHash("Death");


    public bool HasHotDog { get; private set; } = false;
    public bool IsWishDisplayed { get; private set; } = false;
    public bool InitialWishGenerated { get; private set; } = false;
    public bool IsGeneratingNewWish { get; private set; } = false;
    public bool IsAtFightTargetPos { get; private set; } = false;
    public bool IsAtFightStartPos { get; private set; } = false;
    public bool VommitAttackUsed { get; private set; } = false;
    public Vector3 FightStartPos { get => new(0, _fightStartPosY, 0); }
    public Vector3 FightTargetPos { get => new(0, _fightTargetPosY, 0); }

    public BoxCollider2D MyBoxCollider { get; private set; }
    public BE_MoveState MoveState { get; private set; }
    public BE_TasteState TasteState { get; private set; }

    public override void Awake()
    {
        base.Awake();

        MyBoxCollider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _bossSprites[0];
        _dialogueBoxSpriteRenderer.enabled = false;
    }

    public override void Start() {
        base.Start();

        _wishBubble = GetComponentInChildren<WishBubble>();

        // setup state machine
        MoveState = new BE_MoveState(this, StateMachine, ANIM_BOOL_MOVE_HASH, _moveStateData, this);  
        TasteState = new BE_TasteState(this, StateMachine, ANIM_BOOL_TASTE_HASH, _tasteStateData, this);

        StateMachine.Initialize(MoveState);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (_currentHotDog = other.GetComponent<HotDog>()) {
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

        // remove is wish check when I have vommit routine and always trigger EAT
        if (IsHotDogWish(hotDog)) {
            Anim.SetBool(ANIM_BOOL_EAT_HASH, true);
        }
        else {
            Anim.SetBool(ANIM_BOOL_BITE_HASH, true);
        }
    }


    // triggered at top of bite animation
    private void BiteAnimEvent() {
        //Debug.Log("BiteAnimEvent triggered.");
        _currentHotDog.SetNextState();
    }

    private void BiteEndAnimEvent() {
        //Debug.Log("BiteEndAnimEvent triggered");
        Anim.SetBool(ANIM_BOOL_BITE_HASH, false);

        if (!IsHotDogWish(_currentHotDog)) {
            _currentHotDog.DiscardProjectile();
            HasHotDog = false;
            MyBoxCollider.enabled = true;
        }
    }

    private void EatAnimEvent() {
        _currentHotDog.SetNextState();
    }

    private void EatEndAnimEvent() {
        Debug.Log("Eat end anime event triggered.");
        Anim.SetBool(ANIM_BOOL_EAT_HASH, false);

        WishFulfilled();
        
        MyBoxCollider.enabled = true;
    }

    private void DeathEndAnimEvent() {
        GameManager.Instance.LoadWinScreen();
    }

    private bool IsHotDogWish(HotDog hotDog) {
        Debug.Log("Is hot dog wish entered.");
        if (hotDog.HotDogData.Bun == _currentWish.Bun && hotDog.HotDogData.Dog == _currentWish.Dog && hotDog.HotDogData.Sauce == _currentWish.Sauce) {
            return true;
        }
        return false;
    }

    private IEnumerator GenerateNewWishRoutine() {
        Debug.Log("generate new wish routine entered");
        if (!InitialWishGenerated) {
            InitialWishGenerated = true;
        }

        IsGeneratingNewWish = true;

        _wishBubble.DisplayWishReset();
        IsWishDisplayed = false;

        AssignNewCurrentWish();

        Debug.Log("Came past assign new current wish");
        _wishBubble.DisplayWish(_currentWish);

        if (_currentHotDog != null) {
            Destroy(_currentHotDog.gameObject);
            HasHotDog = false;
        }

        yield return new WaitForSecondsRealtime(_wishBubble.TotalWishDisplayTime);

        IsWishDisplayed = true;
        MyBoxCollider.enabled = true;
        IsGeneratingNewWish = false;
    }

    private void AssignNewCurrentWish() {

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

    private void WishFulfilled() {
        _wishFulfilledCount++;
        Debug.Log("Wish # " + _wishFulfilledCount + "fulfilled.");

        if (_wishFulfilledCount >= _bossSprites.Length) {
            StartCoroutine(DeathRoutine());
            Debug.Log("Player won.");
        }
        else {
            _spriteRenderer.sprite = _bossSprites[_wishFulfilledCount];
            StartCoroutine(GenerateNewWishRoutine());      
        }
    }

    private IEnumerator DeathRoutine() {
        SetMoveTargetPos(this.transform.position);
 
        foreach (var sprite in _deathSprites) {
            _spriteRenderer.sprite = sprite;
            yield return new WaitForSeconds(_deathRoutineDelay);
        }

        yield return new WaitForSeconds(_winScreenDelay);
        GameManager.Instance.LoadWinScreen();
    }

    private IEnumerator UseVommitAttackRoutine() {
        Anim.SetBool(ANIM_BOOL_VOMMIT_HASH, true);

        VommitAttackUsed = true;
        MyBoxCollider.enabled = false;
        _wishBubble.DisplayWishReset();
        yield return null;
    }

    private void VommitAttackEndAnimEvent() {
        GameManager.Instance.LoadGameOverFromGame(NormalEnemy.NEType.Lawyer); //TODO: fix when have time
        Debug.Log("Boss Game Over Triggered");
    }

    public void GenerateNewWish() {
        StartCoroutine(GenerateNewWishRoutine());
    }

    public void TasteHotDog() {
        StartCoroutine(TasteHotDogRoutine(_currentHotDog));
    }

    public void SetIsAtFightTargetPos() {
        float distToTarget = Vector3.Distance(this.transform.position, FightTargetPos);
        if (distToTarget <= _moveStateData.MinTargetDistance) {
            IsAtFightTargetPos = true;
        }        
    }

    public void SetIsAtFightStartPos() {
        float distToTarget = Vector3.Distance(this.transform.position, FightStartPos);
        if (distToTarget <= _moveStateData.MinTargetDistance) {
            IsAtFightStartPos = true;
        }   
    }

    public void UseVommitAttack() {
        StartCoroutine(UseVommitAttackRoutine());
    }

    public void DisplayIntroDialogue(float duration) {
        StartCoroutine(IntroDialogueRoutine(duration));
    }
    
    private IEnumerator IntroDialogueRoutine(float duration) {
        _dialogueBoxSpriteRenderer.enabled = true;
        yield return new WaitForSecondsRealtime(duration);

        _dialogueBoxSpriteRenderer.enabled = false;
    }
}

