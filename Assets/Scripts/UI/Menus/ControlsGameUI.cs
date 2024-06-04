using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsGameUI : MenuControlsUI
{
    public override void LoadSelection()
    {
        Debug.Log("Load Selection called in Controls Menu (game)");   
        StartCoroutine(LoadSelectionRoutine());
    }     

    private IEnumerator LoadSelectionRoutine() {
        yield return new WaitForSeconds(_confirmDelay);
        if (_currentSelection == 0) {
            GameManager.Instance.LoadGameOverFromMenu();            
        }
        else if (_currentSelection == 1) {
            if (GameManager.Instance.CurrentGameState == GameManager.GameState.Level) {
                GameManager.Instance.LoadLevelOne();
            }
            else if (GameManager.Instance.CurrentGameState == GameManager.GameState.Boss) {
                GameManager.Instance.LoadBoss(false);
            }
        }
    }
}
