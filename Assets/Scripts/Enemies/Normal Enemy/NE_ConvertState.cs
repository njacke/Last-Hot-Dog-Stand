using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NE_ConvertState : ConvertState
{
    private NormalEnemy _normalEnemy;

    public NE_ConvertState(Entity entity, FiniteStateMachine stateMachine, int animBoolNameHash, ConvertStateData convertStateData, NormalEnemy normalEnemy) : base(entity, stateMachine, animBoolNameHash, convertStateData)
    {
        _normalEnemy = normalEnemy;
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
