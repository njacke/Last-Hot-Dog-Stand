using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    protected MoveStateData _moveStateData;

    public MoveState(Entity entity, FiniteStateMachine stateMachine, int animBoolNameHash, MoveStateData moveStateData) : base(entity, stateMachine, animBoolNameHash)
    {        
        _moveStateData = moveStateData;
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
        MoveTowardsTargetPos();
    }

    public virtual void MoveTowardsTargetPos() {
        float distToTarget = Vector3.Distance(_entity.transform.position, _entity.MoveTargetPos);
        if (distToTarget <= _moveStateData.MinTargetDistance) {
            Debug.Log("Target reached");
            return;
        }

        Vector3 dir = (_entity.MoveTargetPos - _entity.transform.position).normalized;
        _entity.Rb.MovePosition(_entity.transform.position + _moveStateData.MoveSpeed * Time.fixedDeltaTime * dir);        
    }
}