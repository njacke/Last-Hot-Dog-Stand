using UnityEngine;

[CreateAssetMenu(menuName = "Data/Move State Data")]

public class MoveStateData : ScriptableObject
{
    public float MoveSpeed = 1f;
    public float MinTargetDistance = .5f;
}
