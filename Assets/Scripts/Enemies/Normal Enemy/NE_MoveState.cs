using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class NE_MoveState : MoveState
{
    private NormalEnemy _normalEnemy;
    private float _minConversionRange;

    public NE_MoveState(Entity entity, FiniteStateMachine stateMachine, int animBoolNameHash, MoveStateData moveStateData, Vector3 targetPos, NormalEnemy normalEnemy, float minConversionRange) : base(entity, stateMachine, animBoolNameHash, moveStateData, targetPos)
    {
        _normalEnemy = normalEnemy;
        _minConversionRange = minConversionRange;
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
        if (ConvertRangeCheck()) {
            _normalEnemy.StateMachine.ChangeState(_normalEnemy.ConvertState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private bool ConvertRangeCheck() {
        float distToTarget = Vector3.Distance(_normalEnemy.transform.position, _targetPos);
        if (distToTarget <= _minConversionRange) {
            return true;
        }

        return false;
    }
}
