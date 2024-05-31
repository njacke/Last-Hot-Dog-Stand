using System;
using UnityEngine;

public class Cooldown
{
    // setup for 4 cd progress sprites per ability
    // TODO: add setup to constructor or just trigger event every frame?
    private float[] _progressThresholds = { 0.2f, 0.4f, 0.6f, 0.8f };
    private bool[] _thresholdsReached;
    private int _notOnCDSpriteIndex = 0;
    private int _cdStartSpriteIndex = 1;

    public CDType CooldownType { get; private set; } = CDType.None;
    public float StartingCooldown { get; private set; } = 0f;
    public float RemainingCooldown { get; private set; } = 0f;
    public bool IsOnCooldown { get; private set; } = false;
    public static event Action<CDType, int> OnCooldownThresholdReached;

    public enum CDType
    {
        None,
        BunOne,
        BunTwo,
        BunThree,
        DogOne,
        DogTwo,
        DogThree,
        SauceOne,
        SauceTwo,
        SauceThree
    }

    public Cooldown(CDType cooldownType, float startingCooldown)
    {
        CooldownType = cooldownType;
        StartingCooldown = startingCooldown;
        InitializeThresholds();
    }

    private void InitializeThresholds()
    {
        _thresholdsReached = new bool[_progressThresholds.Length];
    }

    public void StartCooldown()
    {
        if (StartingCooldown > 0f) {
            RemainingCooldown = StartingCooldown;

            IsOnCooldown = true;
            OnCooldownThresholdReached?.Invoke(CooldownType, _cdStartSpriteIndex);
            InitializeThresholds();
        }
    }

    public void TrackCooldown() {
        if (RemainingCooldown > 0f)
        {
            RemainingCooldown -= Time.deltaTime;
            float progress = GetProgress();

            for (int i = 0; i < _progressThresholds.Length; i++) {
                if (progress >= _progressThresholds[i] && !_thresholdsReached[i]) {
                    OnCooldownThresholdReached?.Invoke(CooldownType, i + _cdStartSpriteIndex + 1);
                    _thresholdsReached[i] = true;
                }
            }
        }
        else if (IsOnCooldown) {
            IsOnCooldown = false;
            OnCooldownThresholdReached?.Invoke(CooldownType, _notOnCDSpriteIndex);
        }
    }

    private float GetProgress() {
        if (IsOnCooldown && StartingCooldown > 0f)
        {
            return 1 - RemainingCooldown / StartingCooldown;
        }
        return 1f;
    }
}
