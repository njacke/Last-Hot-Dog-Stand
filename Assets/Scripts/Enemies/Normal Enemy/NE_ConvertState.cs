using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NE_ConvertState : ConvertState
{
    protected NormalEnemy _normalEnemy;
    protected float _currentConversionDuration = 0f;

    public NE_ConvertState(Entity entity, FiniteStateMachine stateMachine, int animBoolNameHash, ConvertStateData convertStateData, NormalEnemy normalEnemy) : base(entity, stateMachine, animBoolNameHash, convertStateData)
    {
        _normalEnemy = normalEnemy;
    }

    public override void Enter()
    {
        base.Enter();
        _currentConversionDuration = 0f;
        _normalEnemy.StartConverting();
    }

    public override void Exit()
    {
        base.Exit();
        _normalEnemy.StopConverting();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        UpdateConversionProgress();

        // transition to taste state
        if (_normalEnemy.HasHotDog && !_normalEnemy.IsSatisfied) {
            _normalEnemy.StateMachine.ChangeState(_normalEnemy.TasteState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void UpdateConversionProgress() {
        _currentConversionDuration += Time.deltaTime;
        if (_currentConversionDuration > _convertStateData.ConversionSpeed) {
            _normalEnemy.AddConversionProgress();
            _currentConversionDuration = 0f;
        }
    }
}
