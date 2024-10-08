using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsUI : MenuControlsUI
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

        GameManager.Instance.LoadCheatSheetMenu();
    }    
}
