using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class BE_MoveState : MoveState
{
    protected BossEnemy _bossEnemy;

    public BE_MoveState(Entity entity, FiniteStateMachine stateMachine, int animBoolNameHash, MoveStateData moveStateData, BossEnemy bossEnemy) : base(entity, stateMachine, animBoolNameHash, moveStateData)
    {
        _bossEnemy = bossEnemy;
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
        Debug.Log("I am in move state");

        // transition to taste state
        if (_bossEnemy.HasHotDog) {
            _bossEnemy.StateMachine.ChangeState(_bossEnemy.TasteState);
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
