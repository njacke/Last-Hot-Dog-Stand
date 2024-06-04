using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class NE_MoveState : MoveState
{
    protected NormalEnemy _normalEnemy;

    public NE_MoveState(Entity entity, FiniteStateMachine stateMachine, int animBoolNameHash, MoveStateData moveStateData, NormalEnemy normalEnemy) : base(entity, stateMachine, animBoolNameHash, moveStateData)
    {
        _normalEnemy = normalEnemy;
    }

    public override void Enter()
    {
        base.Enter();
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.BossTransition) {
            _normalEnemy.SetMoveTargetPos(_normalEnemy.transform.position);
            _normalEnemy.MyBoxCollider.enabled = false;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Level) {
            // transition to taste state
            if (_normalEnemy.HasHotDog && !_normalEnemy.IsSatisfied) {
                _normalEnemy.StateMachine.ChangeState(_normalEnemy.TasteState);
            }

            //transition to convert state       
            else if (_normalEnemy.IsInConvertRange()) {
                _normalEnemy.StateMachine.ChangeState(_normalEnemy.ConvertState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
