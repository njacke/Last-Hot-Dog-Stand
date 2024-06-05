using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsIntroUI : MenuControlsUI
{
    public override void LoadSelection()
    {
        Debug.Log("Load Selection called in Controls Menu (intro)");   
        StartCoroutine(LoadSelectionRoutine());
    }     

    private IEnumerator LoadSelectionRoutine() {
        yield return new WaitForSeconds(_confirmDelay);

        GameManager.Instance.LoadLevelOne();
    }
}
