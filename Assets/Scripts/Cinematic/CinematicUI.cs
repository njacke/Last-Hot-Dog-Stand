using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class CinematicUI : MonoBehaviour
{
    [SerializeField] private float _startDelay = 1f;
    [SerializeField] private float _endDelay = 1f;
    [SerializeField] private float _slideDelay = 1f;
    [SerializeField] private float _slidesFadeInTime = 2f;
    //[SerializeField] private float _titleFadeInTime = 2f;
    [SerializeField] private float _musicVolume = .8f;
    [SerializeField] private Sprite[] _cinematicSprites;    
    [SerializeField] private Image _bgImage;
    [SerializeField] private Image _frontImage;
    [SerializeField] private Image _titleImage;

    private SpriteFadeUI _spriteFadeUI;
    public static event Action<bool, float> OnTitleDrop;

    private void Start() {
        _spriteFadeUI = GetComponent<SpriteFadeUI>();     

        StartCoroutine(PlayCinematicRoutine());  
    }

    private IEnumerator PlayCinematicRoutine() {
        int imageIndex = 0;

        yield return new WaitForSecondsRealtime(_startDelay);  

        foreach (var sprite in _cinematicSprites) {
            _bgImage.sprite = _frontImage.sprite;
            _frontImage.sprite = _cinematicSprites[imageIndex];
            StartCoroutine(_spriteFadeUI.FadeInRoutine(_frontImage, _slidesFadeInTime));                
            imageIndex++;
            yield return new WaitForSecondsRealtime(_slideDelay);
        }
        
        _bgImage.enabled = false;
        _frontImage.enabled = false;
        _titleImage.enabled = true;
        OnTitleDrop?.Invoke(true, _musicVolume);
        yield return new WaitForSecondsRealtime(_endDelay);

        GameManager.Instance.LoadInstructions();
    }

    public void PlayCinematic() {
        StartCoroutine(PlayCinematicRoutine());
    }
}
