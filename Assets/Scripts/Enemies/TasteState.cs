using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TasteState : State
{
    protected TasteStateData _checkStateData;

    public TasteState(Entity entity, FiniteStateMachine stateMachine, int animBoolNameHash, TasteStateData checkStateData) : base(entity, stateMachine, animBoolNameHash)
    {
        _checkStateData = checkStateData;
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
