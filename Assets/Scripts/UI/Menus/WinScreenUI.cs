using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreenUI : MenuControlsUI
{
    public override void LoadSelection()
    {
        Debug.Log("Load Selection called in Win screen");   
        StartCoroutine(LoadSelectionRoutine());    
    }

    public override void SelectNext()
    {
        return;
    }

    public override void SelectPrevious()
    {
        return;
    }

    private IEnumerator LoadSelectionRoutine() {
        yield return new WaitForSeconds(_confirmDelay);
        if (_currentSelection == 0) {
            GameManager.Instance.ReloadGame();
        }
    }
}
