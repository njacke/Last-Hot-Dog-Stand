using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class MenuControlsUI : MonoBehaviour
{    
    [SerializeField] protected float _selectDelay = .05f;
    [SerializeField] protected float _confirmDelay = 0.2f;
    [SerializeField] protected ButtonUI _button0;
    [SerializeField] protected ButtonUI _button1;
    [SerializeField] protected int _currentSelection = 1;

    public virtual void Update() {
        PlayerInput();
    }

    private void PlayerInput() {
        if (!GameManager.Instance.MenuControlsLocked) {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) {
                Debug.Log("Menu Input for Load Selection registered.");
                LoadSelection();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W)) {
                SelectNext();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S)){
                SelectPrevious();            
            }
        }
    }

    public virtual void SelectNext() {
        if (_currentSelection == 0) {
            _currentSelection = 1;
            _button0.UpdateButtonSpriteRoutine(ButtonUI.ButtonAction.Unselect, _selectDelay);
            _button1.UpdateButtonSpriteRoutine(ButtonUI.ButtonAction.Select, _selectDelay);
        }
    }

    public virtual void SelectPrevious() {
        if (_currentSelection == 1) {
            _currentSelection = 0;
            _button1.UpdateButtonSpriteRoutine(ButtonUI.ButtonAction.Unselect, _selectDelay);
            _button0.UpdateButtonSpriteRoutine(ButtonUI.ButtonAction.Select, _selectDelay);
        }
    }

    public abstract void LoadSelection();
}
