using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    InputAction moveAction;
    InputAction jumpAction;

    public float speed = 5f;

    private void Start() {
        this.moveAction = InputSystem.actions.FindAction("Move");
        this.jumpAction = InputSystem.actions.FindAction("Jump");        
    }

    void Update() {
        Vector2 moveValue = this.moveAction.ReadValue<Vector2>();

        Vector3 move = new Vector3(moveValue.x, 0, moveValue.y);

        this.gameObject.transform.Translate(move * Time.deltaTime * this.speed);
    }
}
