using UnityEngine;

public class Entity : MonoBehaviour
{
    public FiniteStateMachine StateMachine;
    public Rigidbody2D Rb { get; private set; }
    public Animator Anim { get; private set; }
    public Vector3 MoveTargetPos { get; private set; } = Vector3.zero;

    public virtual void Awake() {
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
    }

    public virtual void Start() {
        StateMachine = new FiniteStateMachine();  
    }

    public virtual void Update() {
        StateMachine.CurrentState.LogicUpdate();
    }

    public virtual void FixedUpdate() {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    public virtual void SetMoveTargetPos(Vector3 pos) {
        MoveTargetPos = pos;
    }
}