using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooldown
{
    public float StartingCooldown { get; private set; } = 0f;
    public float RemainingCooldown { get; private set;} = 0f;
    public bool IsOnCooldown { get; private set; } = false;

    public Cooldown(float startingCooldown) {
        StartingCooldown = startingCooldown;
    }

    public void ResetCooldown() {
        if (StartingCooldown > 0f) {
            RemainingCooldown = StartingCooldown;
            IsOnCooldown = true;
        }
    }

    public void TrackCooldown() {
        if (RemainingCooldown > 0f) {
            RemainingCooldown -= Time.deltaTime;
        }
        else if (IsOnCooldown) {
            IsOnCooldown = false;
            Debug.Log("New cooldown available.");
        }
    }
}
