using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NE_FleeState : FleeState
{
    protected NormalEnemy _normalEnemy;

    public NE_FleeState(Entity entity, FiniteStateMachine stateMachine, int animBoolNameHash, FleeStateData fleeStateData, NormalEnemy normalEnemy) : base(entity, stateMachine, animBoolNameHash, fleeStateData)
    {
        _normalEnemy = normalEnemy;
    }

    public override void Enter()
    {
        base.Enter();
        _direction = GetFleeDirection();
        GameManager.Instance.EnemySpawner.ClearEnemyFromLane(_normalEnemy.LaneAssigned);
        _normalEnemy.HideSprite();
        _normalEnemy.CurrentHotDog.HideSprite();
        
        if (_normalEnemy.HasHotDog && GameManager.Instance.CurrentGameState == GameManager.GameState.Level) {
            _normalEnemy.FinishHotDog();
        }
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

    private Vector3 GetFleeDirection() {
        if (_normalEnemy.transform.position.x > 0) {
            return Vector3.right;
        }        
        return Vector3.left;
    }
}
