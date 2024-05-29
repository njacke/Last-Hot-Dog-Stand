using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackBubble : MonoBehaviour
{
    [SerializeField] private float _displayTime = 1f;
    [SerializeField] private Sprite _positiveSprite;
    [SerializeField] private Sprite _negativeSprite;

    private SpriteRenderer _spriteRenderer;

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;
    }


    public void DisplayFeedback(bool isPositiveFeedback){
        StartCoroutine(DisplayFeedbackRoutine(isPositiveFeedback));
    }

    private IEnumerator DisplayFeedbackRoutine(bool isPositiveFeedback) {
        _spriteRenderer.enabled = true;

        if (isPositiveFeedback) {
            _spriteRenderer.sprite = _positiveSprite;
        }
        else {
            _spriteRenderer.sprite = _negativeSprite;
        }

        yield return new WaitForSeconds(_displayTime);

        _spriteRenderer.enabled = false;
    }
}
