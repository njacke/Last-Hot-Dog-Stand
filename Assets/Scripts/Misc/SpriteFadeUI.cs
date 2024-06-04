using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteFadeUI : MonoBehaviour
{
    public IEnumerator FadeOutRoutine(Image image, float time) {
        Debug.Log("Fade out routine started.");
        float elapsedTime = 0f;
        float startValue = image.color.a;

        while(elapsedTime < time){
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, 0f, elapsedTime / time);
            image.color = new Color(image.color.r, image.color.g, image.color.b, newAlpha);
            yield return null;
        }
    }

    public IEnumerator FadeInRoutine(Image image, float time) {

        Debug.Log("Fade routine started.");
        float elapsedTime = 0f;
        float startValue = image.color.a;

        while(elapsedTime < time){
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(0f, startValue, elapsedTime / time);
            image.color = new Color(image.color.r, image.color.g, image.color.b, newAlpha);
            yield return null;
        }
    }
}