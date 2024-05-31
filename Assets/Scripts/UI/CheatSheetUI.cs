using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CheatSheetUI : MonoBehaviour
{
    [SerializeField] private GameObject _affinityPrefab;
    [SerializeField] private GameObject _enemyGridPrefab;

    private Dictionary<NormalEnemy.NEType, Affinities> _neAffinitiesDict;
    private Dictionary<AffinityUI, Enum> _lawyerAffUIAffDict;
    private Dictionary<AffinityUI, Enum> _businessAffUIAffDict;
    private Dictionary<AffinityUI, Enum> _criticAffUIAffDict;
    private Dictionary<AffinityUI, Enum> _inspectorAffUIAffDict;    

    private void Start() {
        InitializeAffinities();
    }

    private void OnEnable() {
        NormalEnemy.OnIngredientTasted += RevealIngredient;
    }

    private void OnDisable() {
        NormalEnemy.OnIngredientTasted -= RevealIngredient;
    }

    private void InitializeAffinities() {
        _neAffinitiesDict = GameManager.Instance.NEAffinitiesDict;

        // order for correct display in UI
        var customOrder = new List<NormalEnemy.NEType> {
        NormalEnemy.NEType.Lawyer,
        NormalEnemy.NEType.Businessman,
        NormalEnemy.NEType.Critic,
        NormalEnemy.NEType.Inspector
        };  


        foreach (var enemyKey in customOrder) {

            if (enemyKey == NormalEnemy.NEType.Lawyer) {
                var enemyGrid = InstantiateSingleEnemyGrid("Lawyer");
                _lawyerAffUIAffDict = new Dictionary<AffinityUI, Enum>();
                InitializeNETypeDict(_neAffinitiesDict[enemyKey], _lawyerAffUIAffDict, enemyGrid);
            }
            else if (enemyKey == NormalEnemy.NEType.Businessman) {
                var enemyGrid = InstantiateSingleEnemyGrid("Businessman");
                _businessAffUIAffDict = new Dictionary<AffinityUI, Enum>();
                InitializeNETypeDict(_neAffinitiesDict[enemyKey], _businessAffUIAffDict, enemyGrid);
            }
            else if (enemyKey == NormalEnemy.NEType.Critic) {
                _criticAffUIAffDict = new Dictionary<AffinityUI, Enum>();
                var enemyGrid = InstantiateSingleEnemyGrid("Critic");
                InitializeNETypeDict(_neAffinitiesDict[enemyKey], _criticAffUIAffDict, enemyGrid);
            }
            else if (enemyKey == NormalEnemy.NEType.Inspector) {
                var enemyGrid = InstantiateSingleEnemyGrid("Inspector");
                _inspectorAffUIAffDict = new Dictionary<AffinityUI, Enum>();
                InitializeNETypeDict(_neAffinitiesDict[enemyKey], _inspectorAffUIAffDict, enemyGrid);
            }
        }        
    }


    private void InitializeNETypeDict(Affinities enemyAff, Dictionary<AffinityUI, Enum> enemyDict, Transform enemyGrid)
    {
        int affCount = 0;
        if (enemyAff.BunAffinity != HotDogDataModel.Buns.None) {
            affCount++;
            enemyDict.Add(InstantiateSingleAffinityUI(enemyGrid, "Affinity " + affCount.ToString()), enemyAff.BunAffinity);
        }
        if (enemyAff.DogAffinity != HotDogDataModel.Dogs.None) {
            affCount++;
            enemyDict.Add(InstantiateSingleAffinityUI(enemyGrid, "Affinity " + affCount.ToString()), enemyAff.DogAffinity);
        }
        if (enemyAff.SauceAffinity != HotDogDataModel.Sauces.None) {
            affCount++;
            enemyDict.Add(InstantiateSingleAffinityUI(enemyGrid, "Affinity " + affCount.ToString()), enemyAff.SauceAffinity);
        }
    }

    private Transform InstantiateSingleEnemyGrid(string objectName) {
        var newGrid = Instantiate(_enemyGridPrefab, transform);
        newGrid.name = objectName;
        return newGrid.transform;
    }

    private AffinityUI InstantiateSingleAffinityUI(Transform parent, string objectName) {
        var newAffinity = Instantiate(_affinityPrefab, parent);
        newAffinity.name = objectName;
        return newAffinity.GetComponent<AffinityUI>();
    }

    private void RevealIngredient(NormalEnemy.NEType enemyType, Enum ingredientType) {
        switch (enemyType) {
            case NormalEnemy.NEType.Lawyer:
                foreach (var item in _lawyerAffUIAffDict) {
                    if (item.Value.Equals(ingredientType) && !item.Key.IsRevealed) {
                        item.Key.RevealAffinity(ingredientType);
                    }
                }
                break;

            case NormalEnemy.NEType.Businessman:
                foreach (var item in _businessAffUIAffDict) {
                    if (item.Value.Equals(ingredientType) && !item.Key.IsRevealed) {
                        item.Key.RevealAffinity(ingredientType);
                    }
                }
                break;

            case NormalEnemy.NEType.Critic:
                foreach (var item in _criticAffUIAffDict) {
                    if (item.Value.Equals(ingredientType) && !item.Key.IsRevealed) {
                        item.Key.RevealAffinity(ingredientType);
                    }
                }
                break;

            case NormalEnemy.NEType.Inspector:
                foreach (var item in _inspectorAffUIAffDict) {
                    if (item.Value.Equals(ingredientType) && !item.Key.IsRevealed) {
                        item.Key.RevealAffinity(ingredientType);
                    }
                }
                break;
            
            default:
                Debug.Log("Ingredient (" + ingredientType.ToString() +") is not a vailed affinity for " + enemyType.ToString());
                break;
        }
    }
}
