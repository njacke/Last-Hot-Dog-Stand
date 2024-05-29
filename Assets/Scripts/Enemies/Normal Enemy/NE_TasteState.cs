using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NE_TasteState : TasteState
{
    protected NormalEnemy _normalEnemy;

    public NE_TasteState(Entity entity, FiniteStateMachine stateMachine, int animBoolNameHash, TasteStateData checkStateData, NormalEnemy normalEnemy) : base(entity, stateMachine, animBoolNameHash, checkStateData)
    {
        _normalEnemy = normalEnemy;
    }
    public override void Enter()
    {
        base.Enter();
        _normalEnemy.TasteHotDog();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // transition to move state
        if (!_normalEnemy.HasHotDog && !_normalEnemy.IsSatisfied && !_normalEnemy.IsInConversionRange) {
            _normalEnemy.StateMachine.ChangeState(_normalEnemy.MoveState);
        }
        // transition to convert state
        else if (!_normalEnemy.HasHotDog && !_normalEnemy.IsSatisfied && _normalEnemy.IsInConversionRange) {
            _normalEnemy.StateMachine.ChangeState(_normalEnemy.ConvertState);
        }
        // transition to flee state
        else if (_normalEnemy.HasHotDog && _normalEnemy.IsSatisfied) {
            _normalEnemy.StateMachine.ChangeState(_normalEnemy.FleeState);
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

}
