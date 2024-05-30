using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class FleeState : State
{
    protected FleeStateData _fleeStateData;
    protected Vector3 _direction = Vector3.zero;
    
    public FleeState(Entity entity, FiniteStateMachine stateMachine, int animBoolNameHash, FleeStateData fleeStateData) : base(entity, stateMachine, animBoolNameHash)
    {
        _fleeStateData = fleeStateData;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        _entity.Rb.MovePosition(_entity.transform.position + _fleeStateData.MoveSpeed * Time.fixedDeltaTime * _direction.normalized);
    }
}
