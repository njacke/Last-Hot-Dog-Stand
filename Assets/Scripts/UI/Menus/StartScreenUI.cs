using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenUI : MenuControlsUI
{
    public override void LoadSelection()
    {
        Debug.Log("Load Selection called in start screen");
        GameManager.Instance.LoadIntroCinematic();
    }

    public override void SelectNext()
    {
        return;
    }

    public override void SelectPrevious()
    {
        return;
    }
}
