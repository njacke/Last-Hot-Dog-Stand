using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MenuControlsUI
{
    [SerializeField] private Sprite _lawyerGameOverSprite;
    [SerializeField] private Sprite _businessGameOverSprite;
    [SerializeField] private Sprite _criticGameOverSprite;
    [SerializeField] private Sprite _inspectorGameOverSprite;
    [SerializeField] private Sprite _bossGameOverSprite;
    private Image _image;

    private void Awake() {
        _image = GetComponent<Image>();
    }

    public void SetGameOverImage() {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Boss) {
            _image.sprite = _bossGameOverSprite;
        }
        else {
            _image.sprite = GameManager.Instance.LastDeathTrigger switch {
                NormalEnemy.NEType.Lawyer => _lawyerGameOverSprite,
                NormalEnemy.NEType.Businessman => _businessGameOverSprite,
                NormalEnemy.NEType.Critic => _criticGameOverSprite,
                NormalEnemy.NEType.Inspector => _inspectorGameOverSprite,
                _ => _lawyerGameOverSprite,
             };
        }
    }

    public override void LoadSelection()
    {
        Debug.Log("Load Selection called in Game Over Menu");   
        StartCoroutine(LoadSelectionRoutine());
    }

    private IEnumerator LoadSelectionRoutine() {
        yield return new WaitForSeconds(_confirmDelay);
        if (_currentSelection == 0) {
            GameManager.Instance.LoadInstructions();       
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
