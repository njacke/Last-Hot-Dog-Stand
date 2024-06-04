using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MenuDisplayUI : MonoBehaviour
{
    [SerializeField] private GameObject _startScreenMenu;
    [SerializeField] private GameObject _instructionsMenu;
    [SerializeField] private GameObject _controlsIntroMenu;
    [SerializeField] private GameObject _controlsGameMenu;
    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private GameObject _winScreenMenu;

    private Dictionary<MenuType, GameObject> _menusDict;

    public enum MenuType {
        StartScreen,
        Instructions,
        ControlsIntro,
        ControlsGame,
        GameOver,
        WinScreen,
    }

    private void Awake() {
        InitializeMenusDict();        
    }

    private void InitializeMenusDict() {
        _menusDict = new Dictionary<MenuType, GameObject>() {
            { MenuType.StartScreen, _startScreenMenu },
            { MenuType.Instructions, _instructionsMenu },
            { MenuType.ControlsIntro, _controlsIntroMenu },
            { MenuType.ControlsGame, _controlsGameMenu },
            { MenuType.GameOver, _gameOverMenu },
            { MenuType.WinScreen, _winScreenMenu },
        };
    } 

    public void DisplayMenu(MenuType menuType) {
        HideAllMenus();

        _menusDict[menuType].SetActive(true);
        
        if (menuType == MenuType.GameOver) {
            _gameOverMenu.GetComponent<GameOverUI>().SetGameOverImage();
        }              
    }

    public void HideAllMenus() {
        foreach (var menu in _menusDict) {
            menu.Value.SetActive(false);
        }
    }
}
