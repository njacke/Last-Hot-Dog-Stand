using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BE_TasteState : TasteState
{
    protected BossEnemy _bossEnemy;

    public BE_TasteState(Entity entity, FiniteStateMachine stateMachine, int animBoolNameHash, TasteStateData checkStateData, BossEnemy bossEnemy) : base(entity, stateMachine, animBoolNameHash, checkStateData)
    {
        _bossEnemy = bossEnemy;
    }

    public override void Enter()
    {
        base.Enter();
        _bossEnemy.TasteHotDog();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        //Debug.Log("I am in taste state.");

        // transition to move state
        if (!_bossEnemy.HasHotDog && _bossEnemy.IsWishDisplayed) {
            _bossEnemy.StateMachine.ChangeState(_bossEnemy.MoveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

}
