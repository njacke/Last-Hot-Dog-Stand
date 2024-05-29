using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotDog : MonoBehaviour
{
    [SerializeField] private float _projectileSpeed = 5f;
    [SerializeField] private float _moveToParentDuration = .5f;
    [SerializeField] private HotDogSpritesData _hotDogSpritesData;

    [SerializeField] private SpriteRenderer _bunSpriteRenderer;
    [SerializeField] private SpriteRenderer _dogSpriteRenderer;
    [SerializeField] private SpriteRenderer _sauceSpriteRenderer;

    private float _linearT = 0f;

    private Dictionary<HotDogDataModel.Buns, Sprite> _bunSprites1Dict;
    private Dictionary<HotDogDataModel.Dogs, Sprite> _dogSprites1Dict;
    private Dictionary<HotDogDataModel.Sauces, Sprite> _sauceSprites1Dict;
    private Dictionary<HotDogDataModel.Buns, Sprite> _bunSprites2Dict;
    private Dictionary<HotDogDataModel.Dogs, Sprite> _dogSprites2Dict;
    private Dictionary<HotDogDataModel.Sauces, Sprite> _sauceSprites2Dict;
    private Dictionary<HotDogDataModel.Buns, Sprite> _bunSprites3Dict;
    private Dictionary<HotDogDataModel.Dogs, Sprite> _dogSprites3Dict;
    private Dictionary<HotDogDataModel.Sauces, Sprite> _sauceSprites3Dict;

    private Rigidbody2D _rb;

    public CapsuleCollider2D MyCapsuleCollider { get; set; }
    public Vector3 MoveDirection { get; set; } = Vector3.zero;
    public bool HasHit { get; set; } = false;
    public bool HasBeenTasted { get; set; } = false;
    public HotDogDataModel HotDogData { get; set; }

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();

        MyCapsuleCollider = GetComponent<CapsuleCollider2D>();

        HotDogData = new HotDogDataModel(HotDogDataModel.Buns.None, HotDogDataModel.Dogs.None, HotDogDataModel.Sauces.None);

        InitializeSpriteDicts();
    }

    private void FixedUpdate() {
        if (!HasHit) {
            MoveProjectile();
        }
    }

    private void MoveProjectile() {
        if(MoveDirection != Vector3.zero) {
            _rb.MovePosition(this.transform.position + _projectileSpeed * Time.fixedDeltaTime * MoveDirection);
        }
    }

    public IEnumerator MoveToParentRoutine() {
        float timePassed = 0f;
        Vector3 startPos = this.transform.localPosition;

        while (this.transform.localPosition != Vector3.zero) {
            timePassed += Time.deltaTime;
            _linearT = timePassed / _moveToParentDuration;

            this.transform.localPosition = Vector3.Lerp(startPos, Vector3.zero, _linearT);

            yield return null;
        }
    }

    private void InitializeSpriteDicts() {
        _bunSprites1Dict = new Dictionary<HotDogDataModel.Buns, Sprite> {
            { HotDogDataModel.Buns.BunOne, _hotDogSpritesData.BunSprites[0] },
            { HotDogDataModel.Buns.BunTwo, _hotDogSpritesData.BunSprites[1] },
            { HotDogDataModel.Buns.BunThree, _hotDogSpritesData.BunSprites[2] },
            { HotDogDataModel.Buns.None, _hotDogSpritesData.EmptySprite }
        };

        _dogSprites1Dict = new Dictionary<HotDogDataModel.Dogs, Sprite> {
            { HotDogDataModel.Dogs.DogOne, _hotDogSpritesData.DogSprites[0] },
            { HotDogDataModel.Dogs.DogTwo, _hotDogSpritesData.DogSprites[1] },
            { HotDogDataModel.Dogs.DogThree, _hotDogSpritesData.DogSprites[2] },
            { HotDogDataModel.Dogs.None, _hotDogSpritesData.EmptySprite }
        };

        _sauceSprites1Dict = new Dictionary<HotDogDataModel.Sauces, Sprite> {
            { HotDogDataModel.Sauces.SauceOne, _hotDogSpritesData.SauceSprites[0] },
            { HotDogDataModel.Sauces.SauceTwo, _hotDogSpritesData.SauceSprites[1] },
            { HotDogDataModel.Sauces.SauceThree, _hotDogSpritesData.SauceSprites[2] },
            { HotDogDataModel.Sauces.None, _hotDogSpritesData.EmptySprite }
        };

       _bunSprites2Dict = new Dictionary<HotDogDataModel.Buns, Sprite> {
            { HotDogDataModel.Buns.BunOne, _hotDogSpritesData.BunSprites[3] },
            { HotDogDataModel.Buns.BunTwo, _hotDogSpritesData.BunSprites[4] },
            { HotDogDataModel.Buns.BunThree, _hotDogSpritesData.BunSprites[5] },
            { HotDogDataModel.Buns.None, _hotDogSpritesData.EmptySprite }
        };

        _dogSprites2Dict = new Dictionary<HotDogDataModel.Dogs, Sprite> {
            { HotDogDataModel.Dogs.DogOne, _hotDogSpritesData.DogSprites[3] },
            { HotDogDataModel.Dogs.DogTwo, _hotDogSpritesData.DogSprites[4] },
            { HotDogDataModel.Dogs.DogThree, _hotDogSpritesData.DogSprites[5] },
            { HotDogDataModel.Dogs.None, _hotDogSpritesData.EmptySprite }
        };

        _sauceSprites2Dict = new Dictionary<HotDogDataModel.Sauces, Sprite> {
            { HotDogDataModel.Sauces.SauceOne, _hotDogSpritesData.SauceSprites[3] },
            { HotDogDataModel.Sauces.SauceTwo, _hotDogSpritesData.SauceSprites[4] },
            { HotDogDataModel.Sauces.SauceThree, _hotDogSpritesData.SauceSprites[5] },
            { HotDogDataModel.Sauces.None, _hotDogSpritesData.EmptySprite }
        };

       _bunSprites3Dict = new Dictionary<HotDogDataModel.Buns, Sprite> {
            { HotDogDataModel.Buns.BunOne, _hotDogSpritesData.BunSprites[6] },
            { HotDogDataModel.Buns.BunTwo, _hotDogSpritesData.BunSprites[7] },
            { HotDogDataModel.Buns.BunThree, _hotDogSpritesData.BunSprites[8] },
            { HotDogDataModel.Buns.None, _hotDogSpritesData.EmptySprite }
        };

        _dogSprites3Dict = new Dictionary<HotDogDataModel.Dogs, Sprite> {
            { HotDogDataModel.Dogs.DogOne, _hotDogSpritesData.DogSprites[6] },
            { HotDogDataModel.Dogs.DogTwo, _hotDogSpritesData.DogSprites[7] },
            { HotDogDataModel.Dogs.DogThree, _hotDogSpritesData.DogSprites[8] },
            { HotDogDataModel.Dogs.None, _hotDogSpritesData.EmptySprite }
        };

        _sauceSprites3Dict = new Dictionary<HotDogDataModel.Sauces, Sprite> {
            { HotDogDataModel.Sauces.SauceOne, _hotDogSpritesData.SauceSprites[6] },
            { HotDogDataModel.Sauces.SauceTwo, _hotDogSpritesData.SauceSprites[7] },
            { HotDogDataModel.Sauces.SauceThree, _hotDogSpritesData.SauceSprites[8] },
            { HotDogDataModel.Sauces.None, _hotDogSpritesData.EmptySprite }
        };

    }

    public void UpdateSprites() {

        Debug.Log ("Current state is " + HotDogData.State.ToString());

        switch (HotDogData.State) {
            case HotDogDataModel.EatenState.Full:
                _bunSpriteRenderer.sprite = _bunSprites1Dict[HotDogData.Bun];
                _dogSpriteRenderer.sprite = _dogSprites1Dict[HotDogData.Dog];
                _sauceSpriteRenderer.sprite = _sauceSprites1Dict[HotDogData.Sauce];
                break;

            case HotDogDataModel.EatenState.OneBite:
                _bunSpriteRenderer.sprite = _bunSprites2Dict[HotDogData.Bun];
                _dogSpriteRenderer.sprite = _dogSprites2Dict[HotDogData.Dog];
                _sauceSpriteRenderer.sprite = _sauceSprites2Dict[HotDogData.Sauce];
                break;
            
            case HotDogDataModel.EatenState.TwoBites:
                _bunSpriteRenderer.sprite = _bunSprites3Dict[HotDogData.Bun];
                _dogSpriteRenderer.sprite = _dogSprites3Dict[HotDogData.Dog];
                _sauceSpriteRenderer.sprite = _sauceSprites3Dict[HotDogData.Sauce];   
                break;

            case HotDogDataModel.EatenState.Eaten:
                _bunSpriteRenderer.sprite = _hotDogSpritesData.EmptySprite;
                _dogSpriteRenderer.sprite = _hotDogSpritesData.EmptySprite;
                _sauceSpriteRenderer.sprite = _hotDogSpritesData.EmptySprite;  
                break;
            
            default:
                break;
        }
    }

    public void SetNextState() {
        switch (HotDogData.State) {
            case HotDogDataModel.EatenState.Full:
                HotDogData.State = HotDogDataModel.EatenState.OneBite;
                break;
            case HotDogDataModel.EatenState.OneBite:
                HotDogData.State = HotDogDataModel.EatenState.TwoBites;
                break;
            case HotDogDataModel.EatenState.TwoBites:
                HotDogData.State = HotDogDataModel.EatenState.Eaten;
                break;
            default:
                break;
        }

        UpdateSprites();
    }
}
