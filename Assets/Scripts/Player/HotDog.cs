using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotDog : MonoBehaviour
{
    [SerializeField] private float _projectileSpeed = 5f;
    [SerializeField] private Sprite[] _bunSprites;
    [SerializeField] private Sprite[] _dogSprites;
    [SerializeField] private Sprite[] _sauceSprites;

    [SerializeField] private SpriteRenderer _bunSpriteRenderer;
    [SerializeField] private SpriteRenderer _dogSpriteRenderer;
    [SerializeField] private SpriteRenderer _sauceSpriteRenderer;

    private Dictionary<HotDogDataModel.Buns, Sprite> _bunSpritesDict;
    private Dictionary<HotDogDataModel.Dogs, Sprite> _dogSpritesDict;
    private Dictionary<HotDogDataModel.Sauces, Sprite> _sauceSpritesDict;

    private Rigidbody2D _rb;

    public Vector3 MoveDirection { get; set; } = Vector3.zero;
    public HotDogDataModel HotDogData { get; set; }

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();

        HotDogData = new HotDogDataModel(HotDogDataModel.Buns.None, HotDogDataModel.Dogs.None, HotDogDataModel.Sauces.None);

        InitializeSpriteDicts();
    }

    private void FixedUpdate() {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<NormalEnemy>()) {
            Destroy(gameObject);
        }
    }

    private void Move() {
        if(MoveDirection != Vector3.zero) {
            _rb.MovePosition(this.transform.position + _projectileSpeed * Time.fixedDeltaTime * MoveDirection);
        }
    }

    private void InitializeSpriteDicts() {
        _bunSpritesDict = new Dictionary<HotDogDataModel.Buns, Sprite> {
            { HotDogDataModel.Buns.BunOne, _bunSprites[0] },
            { HotDogDataModel.Buns.BunTwo, _bunSprites[1] },
            { HotDogDataModel.Buns.BunThree, _bunSprites[2] },
            { HotDogDataModel.Buns.None, _bunSprites[3] }
        };

        _dogSpritesDict = new Dictionary<HotDogDataModel.Dogs, Sprite> {
            { HotDogDataModel.Dogs.DogOne, _dogSprites[0] },
            { HotDogDataModel.Dogs.DogTwo, _dogSprites[1] },
            { HotDogDataModel.Dogs.DogThree, _dogSprites[2] },
            { HotDogDataModel.Dogs.None, _dogSprites[3] }
        };

        _sauceSpritesDict = new Dictionary<HotDogDataModel.Sauces, Sprite> {
            { HotDogDataModel.Sauces.SauceOne, _sauceSprites[0] },
            { HotDogDataModel.Sauces.SauceTwo, _sauceSprites[1] },
            { HotDogDataModel.Sauces.SauceThree, _sauceSprites[2] },
            { HotDogDataModel.Sauces.None, _sauceSprites[3] }
        };        
    }

    public void UpdateSprites() {
        _bunSpriteRenderer.sprite = _bunSpritesDict[HotDogData.Bun];
        _dogSpriteRenderer.sprite = _dogSpritesDict[HotDogData.Dog];
        _sauceSpriteRenderer.sprite = _sauceSpritesDict[HotDogData.Sauce];        
    }
}
