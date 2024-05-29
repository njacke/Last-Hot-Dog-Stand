using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class HotDogDataModel
{
    public Buns Bun { get; set; }
    public Dogs Dog { get; set; }
    public Sauces Sauce { get; set; }
    public EatenState State { get; set; }

    public enum Buns {
        None = default,
        BunOne,
        BunTwo,
        BunThree
    }

    public enum Dogs {
        None = default,
        DogOne,
        DogTwo,
        DogThree
    }

    public enum Sauces {
        None = default,
        SauceOne,
        SauceTwo,
        SauceThree
    }

    public enum EatenState {
        Full = default,
        OneBite,
        TwoBites,
        Eaten
    }

    public HotDogDataModel(Buns bun, Dogs dog, Sauces sauce) {
        Bun = bun;
        Dog = dog;
        Sauce = sauce;
        State = EatenState.Full;
    }
}
