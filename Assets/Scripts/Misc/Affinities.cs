using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Affinities
{
    public HotDogDataModel.Buns BunAffinity { get; private set; } = HotDogDataModel.Buns.None;
    public HotDogDataModel.Dogs DogAffinity { get; private set; } = HotDogDataModel.Dogs.None;
    public HotDogDataModel.Sauces SauceAffinity { get; private set; } = HotDogDataModel.Sauces.None;

    public Affinities(HotDogDataModel.Buns bun, HotDogDataModel.Dogs dog, HotDogDataModel.Sauces sauce) {
        BunAffinity = bun;
        DogAffinity = dog;
        SauceAffinity = sauce;
    }
}
