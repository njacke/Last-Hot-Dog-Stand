using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : Entity
{
    [SerializeField] private MoveStateData _moveStateData;
    [SerializeField] private ConvertStateData _convertStateData;
    [SerializeField] private FleeStateData _fleeStateData;

    public BoxCollider2D BoxCollider2D { get; private set; }
    public NE_MoveState MoveState { get; private set; }
    public NE_ConvertState ConvertState { get; private set; }
    public NE_FleeState FleeState { get; private set; }

    private readonly int ANIM_BOOL_MOVE_HASH = Animator.StringToHash("Move");
    private readonly int ANIM_BOOL_CONVERT_HASH = Animator.StringToHash("Convert");
    private readonly int ANIM_BOOL_FLEE_HASH = Animator.StringToHash("Flee");

    public enum NormalEnemyType {
        Businessman,
        Lawyer,
    }

    public override void Awake()
    {
        base.Awake();

        BoxCollider2D = GetComponent<BoxCollider2D>();
    }

    public override void Start() {
        base.Start();

        MoveState = new NE_MoveState(this, StateMachine, ANIM_BOOL_MOVE_HASH, _moveStateData, GetMoveTargetPos(), this, _convertStateData.MinConversionRange);  
        ConvertState = new NE_ConvertState(this, StateMachine, ANIM_BOOL_CONVERT_HASH, _convertStateData, this); 
        FleeState = new NE_FleeState (this, StateMachine, ANIM_BOOL_FLEE_HASH, _fleeStateData, this); 

        StateMachine.Initialize(MoveState);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<HotDog>()) {
            Debug.Log("I was hit by a Hot Dog");
            StateMachine.ChangeState(FleeState);
        }
    }

    private Vector3 GetMoveTargetPos() {
        Vector3 turretPosition = FindObjectOfType<TurretController>().transform.position;        
        return new Vector3 (turretPosition.x, turretPosition.y + 1.5f, + turretPosition.z);
    }
}
