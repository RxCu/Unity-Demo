using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
  public float speed = 500f;
  public float jumpHeight = 10f;
  public float sensitivity = 1f;

  Vector2 movement;
  Vector2 look;

  // TODO: Better player/camera rotation
  Vector3 rotation;

  Rigidbody rb;

  void Start() {
    this.rb = this.GetComponent<Rigidbody>();

    this.movement = new Vector2(0, 0);
    this.rotation = new Vector3(0, 0, 0);
  }

  void Update() {
    // Copy data into new Struct
    Vector3 velocity = this.rb.linearVelocity;

    velocity.x = this.movement.x * this.speed * Time.deltaTime;
    velocity.z = this.movement.y * this.speed * Time.deltaTime;

    this.rb.linearVelocity = (velocity.x * transform.forward) +
                             (velocity.y * transform.up) +
                             (velocity.z * transform.right);

    this.rotation.y += this.look.x * this.sensitivity * Time.deltaTime;
    this.rb.rotation = Quaternion.Euler(this.rotation);
  }

  public void Move(InputAction.CallbackContext ctx) {
    Vector2 input = ctx.ReadValue<Vector2>();

    this.movement.x = input.y;
    this.movement.y = input.x;
  }

  public void Look(InputAction.CallbackContext ctx) {
    Vector2 input = ctx.ReadValue<Vector2>();

    this.look.x = input.x;
    this.look.y = input.y;
  }
}