using UnityEngine;

public class State
{
    protected FiniteStateMachine _stateMachine;
    protected Entity _entity;
    protected float _startTime;
    protected readonly int ANIM_BOOL_NAME_HASH; 

    public State(Entity entity, FiniteStateMachine stateMachine, int animBoolNameHash) {
        _entity = entity;
        _stateMachine = stateMachine;
        ANIM_BOOL_NAME_HASH = animBoolNameHash;
    }

    public virtual void Enter() {
        _startTime = Time.time;
    }

    public virtual void Exit() {
    }

    public virtual void LogicUpdate() {

    }

    public virtual void PhysicsUpdate() {

    }
}