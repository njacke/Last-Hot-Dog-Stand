using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
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
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.BossTransition) {
            _bossEnemy.SetMoveTargetPos(_bossEnemy.FightStartPos);
        }
        else if (GameManager.Instance.CurrentGameState == GameManager.GameState.Boss) {
            _bossEnemy.SetMoveTargetPos(_bossEnemy.FightTargetPos);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        //Debug.Log("I am in move state");

        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Boss) {
            if (!_bossEnemy.InitialWishGenerated) {
                _bossEnemy.GenerateNewWish();
            }

            _bossEnemy.SetIsAtFightTargetPos();

            // vommit finisher
            if (_bossEnemy.IsAtFightTargetPos && !_bossEnemy.VommitAttackUsed) {
                _bossEnemy.UseVommitAttack();                        
            }

            // transition to taste state
            else if (_bossEnemy.HasHotDog && !_bossEnemy.VommitAttackUsed) {
                _bossEnemy.StateMachine.ChangeState(_bossEnemy.TasteState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        if (!_bossEnemy.IsGeneratingNewWish) {
            base.PhysicsUpdate();
        }
    }
}
