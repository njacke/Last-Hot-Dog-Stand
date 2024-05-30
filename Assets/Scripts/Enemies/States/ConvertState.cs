using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertState : State
{
    protected ConvertStateData _convertStateData;

    public ConvertState(Entity entity, FiniteStateMachine stateMachine, int animBoolNameHash, ConvertStateData convertStateData) : base(entity, stateMachine, animBoolNameHash)
    {        
        _convertStateData = convertStateData;
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
    }
}