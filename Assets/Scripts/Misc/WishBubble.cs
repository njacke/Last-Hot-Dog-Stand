using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class WishBubble : MonoBehaviour
{
    [SerializeField] private float _displayDelay = 0.5f;
    [SerializeField] private float _displayTime = 0.5f;
    [SerializeField] private Sprite _bubble1Sprite;
    [SerializeField] private Sprite _bubble2Sprite;
    [SerializeField] private Sprite _bubble3Sprite;
    [SerializeField] SpriteRenderer _bunPreviewSprite;
    [SerializeField] SpriteRenderer _dogPreviewSprite;
    [SerializeField] SpriteRenderer _saucePreviewSprite;

    public float TotalWishDisplayTime { get => _displayDelay + _displayTime; }

    private SpriteRenderer _spriteRenderer;

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _spriteRenderer.enabled = false;
        _bunPreviewSprite.enabled = false;
        _dogPreviewSprite.enabled = false;
        _saucePreviewSprite.enabled = false;
    }

    private IEnumerator DisplayWishRoutine(HotDogDataModel hotDogModel) {

        Sprite[] hotDogPreviews = GameManager.Instance.HotDogPreviewer.GetPreviewSpritesOnDemand(hotDogModel);
        
        yield return new WaitForSeconds(_displayDelay);
        
        _spriteRenderer.enabled = true;
        _spriteRenderer.sprite = _bubble1Sprite;
        yield return new WaitForSeconds(_displayTime / 2);
        
        _spriteRenderer.sprite = _bubble2Sprite;
        yield return new WaitForSeconds(_displayTime / 2);

        _spriteRenderer.sprite = _bubble3Sprite;

        _bunPreviewSprite.enabled = true;
        _dogPreviewSprite.enabled = true;
        _saucePreviewSprite.enabled = true;

        _bunPreviewSprite.sprite = hotDogPreviews[0];
        _dogPreviewSprite.sprite = hotDogPreviews[1];
        _saucePreviewSprite.sprite = hotDogPreviews[2];
    }

    public void DisplayWish(HotDogDataModel hotDogModel){
        Debug.Log("Display wish triggered");
        StartCoroutine(DisplayWishRoutine(hotDogModel));
    }

    public void DisplayWishReset() {
        _spriteRenderer.enabled = false;
        _bunPreviewSprite.enabled = false;
        _dogPreviewSprite.enabled = false;
        _saucePreviewSprite.enabled = false;
    }
}
