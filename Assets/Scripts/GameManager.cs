using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _affinitiesPerNE = 2;
    public Dictionary<NormalEnemy.NEType, Affinities> NEAffinitiesDict { get; private set; }

    private void Awake() {
        //InitializeNEAffinities();
        AffinitiesTest();
    }

    private void AffinitiesTest() {
        int n = 5;
        for (int i = 0; i < n; i++) {
            InitializeNEAffinities();
            foreach (var item in NEAffinitiesDict) {
                Debug.Log($"{item.Key}: {item.Value.BunAffinity}, {item.Value.DogAffinity}, {item.Value.SauceAffinity}");
            }
        }
    }

    private void InitializeNEAffinities() {
        NEAffinitiesDict = new Dictionary<NormalEnemy.NEType, Affinities>();

        var enemyTypesList = Enum.GetValues(typeof(NormalEnemy.NEType)).Cast<NormalEnemy.NEType>().ToList();
        
        var bunsList = Enum.GetValues(typeof(HotDogDataModel.Buns)).Cast<HotDogDataModel.Buns>().Where(b => b != HotDogDataModel.Buns.None).ToList();
        var dogsList = Enum.GetValues(typeof(HotDogDataModel.Dogs)).Cast<HotDogDataModel.Dogs>().Where(d => d != HotDogDataModel.Dogs.None).ToList();
        var saucesList = Enum.GetValues(typeof(HotDogDataModel.Sauces)).Cast<HotDogDataModel.Sauces>().Where(s => s != HotDogDataModel.Sauces.None).ToList();

        // fail-safe if not enough ingredients to distribute
        //TODO: need to add test if uneven amount of ingredients (min required to distribute)
        int totalIngredients = bunsList.Count + dogsList.Count + saucesList.Count;
        int reqIngredients = enemyTypesList.Count * _affinitiesPerNE;
        if (totalIngredients < reqIngredients) {
            Debug.Log("Not enough ingredients to distribute affinities.");
            return;
        }

        // RNGesus        
        ShuffleList(enemyTypesList);
        ShuffleList(bunsList);
        ShuffleList(dogsList);
        ShuffleList(saucesList);

        var allCatsList = new List<object> {
            bunsList,
            dogsList,
            saucesList,
        };

        
        foreach (var enemy in enemyTypesList) {         
            var bAffinity = HotDogDataModel.Buns.None;
            var dAffinity = HotDogDataModel.Dogs.None; 
            var sAffinity = HotDogDataModel.Sauces.None;    

            int affinitiesAssigned = 0;

            while (affinitiesAssigned < _affinitiesPerNE) {

                ShuffleList(allCatsList);

                if (allCatsList[0] is List<HotDogDataModel.Buns> && bunsList.Count != 0) {
                    bAffinity = bunsList[0];  
                    bunsList.RemoveAt(0);               
                }
                else if (allCatsList[0] is List<HotDogDataModel.Dogs> && dogsList.Count != 0) {
                    dAffinity = dogsList[0];
                    dogsList.RemoveAt(0);
                }
                else if (allCatsList[0] is List<HotDogDataModel.Sauces> && dogsList.Count != 0) {
                    sAffinity = saucesList[0];
                    saucesList.RemoveAt(0);
                }

                int[] countsArray = { bunsList.Count, dogsList.Count, saucesList.Count };

                if (AtLeastNIntsAreNotEqualToX(countsArray, dogsList.Count, _affinitiesPerNE)) {
                    NEAffinitiesDict = new Dictionary<NormalEnemy.NEType, Affinities>();
                    Debug.Log("Initialization Failed.");
                    return;
                }
            }

            var affinities = new Affinities(bAffinity, dAffinity, sAffinity);

            NEAffinitiesDict.Add(enemy, affinities);
        }

        Debug.Log("Initilization Succesfull.");
    }

    private void ShuffleList<T>(List<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    private bool AtLeastNIntsAreNotEqualToX(int[] numbers, int x, int minCount) {
        int count = 0;
        
        foreach (int num in numbers) {
            if (num != x) {
                count++;
                if (count >= minCount) {
                    return true;
                }
            }
        }

        return false;
    }
}

