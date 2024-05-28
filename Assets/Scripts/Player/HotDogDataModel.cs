using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotDogDataModel
{
    public Buns Bun { get; set; }
    public Dogs Dog { get; set; }
    public Sauces Sauce { get; set; }

    public enum Buns {
        BunOne,
        BunTwo,
        BunThree,
        None
    }

    public enum Dogs {
        DogOne,
        DogTwo,
        DogThree,
        None
    }

    public enum Sauces {
        SauceOne,
        SauceTwo,
        SauceThree,
        None
    }

    public HotDogDataModel(Buns bun, Dogs dog, Sauces sauce) {
        Bun = bun;
        Dog = dog;
        Sauce = sauce;
    }
}
