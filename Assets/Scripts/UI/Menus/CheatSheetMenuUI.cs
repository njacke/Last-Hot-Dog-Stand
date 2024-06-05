using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatSheetMenuUI : MenuControlsUI
{
    public override void LoadSelection()
    {
        Debug.Log("Load Selection called in Instructions menu");   
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

        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Intro) {
            GameManager.Instance.LoadControlsIntro();
        }
        else {
            GameManager.Instance.LoadControlsGame();
        }
    }
    
}
