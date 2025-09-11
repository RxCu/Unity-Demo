using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

  public float speed = 5f;
  public float jumpHeight = 10f;

  public bool paused = true;
  public bool firstPerson = true;

  public float yawSensitivity = 30f;
  public float pitchSensitivity = 30f;

  public Vector2 cameraPitchLimits = new Vector2(-90, 90);

  public Vector3 cameraOffset = new Vector3(0, 0.5f, 0.5f);

  Vector2 moveVector;
  Vector3 jumpForce;

  Camera cameraMain;
  InputAction lookVector;
  Vector2 cameraRotation;

  Rigidbody rb;

  public void Pause() {
    Cursor.lockState = CursorLockMode.None;
    this.paused = true;
  }

  public void Unpause() {
    Cursor.lockState = CursorLockMode.Locked;
    this.paused = false;
  }

  void Start() {
    this.rb = this.GetComponent<Rigidbody>();

    this.moveVector = new Vector2(0, 0);
    this.lookVector = InputSystem.actions.FindAction("Look");

    this.cameraMain = Camera.main;
    this.cameraRotation = Vector2.zero;
  }

  void Update() {
    if(this.UpdatePaused()) return;
    this.UpdateLook();
    this.UpdateMovement();
  }

  void OnValidate() {
    this.jumpForce = new Vector3(0, this.jumpHeight, 0);
  }

  bool UpdatePaused() {
    // TODO: add proper pause actions
    if(Input.GetKey(KeyCode.Escape)) {
      this.Pause();
    }
    else if(Input.GetKey(KeyCode.Space)) {
      this.Unpause();
    }

    return this.paused;
  }

  void UpdateMovement() {
    // Copy data into new Struct
    Vector3 velocity = this.rb.linearVelocity;

    velocity.x = this.moveVector.x * this.speed;
    velocity.z = this.moveVector.y * this.speed;

    this.rb.linearVelocity = (velocity.x * transform.forward) +
                             (velocity.y * transform.up) +
                             (velocity.z * transform.right);
  }

  void UpdateLook() {
    this.cameraMain.transform.position = this.transform.position + this.cameraOffset;

    Vector2 input = this.lookVector.ReadValue<Vector2>();

    this.cameraRotation.x += input.x * this.yawSensitivity * Time.deltaTime;
    this.cameraRotation.y = Mathf.Clamp(this.cameraRotation.y +
      input.y * this.pitchSensitivity * Time.deltaTime, this.cameraPitchLimits.x, this.cameraPitchLimits.y);

    // set player yaw
    this.transform.localRotation = Quaternion.AngleAxis(this.cameraRotation.x, Vector3.up);

    // set camera rotation
    this.cameraMain.transform.rotation = Quaternion.Euler(-this.cameraRotation.y, this.cameraRotation.x, 0);
  }

  public void Move(InputAction.CallbackContext ctx) {
    Vector2 input = ctx.ReadValue<Vector2>();

    this.moveVector.x = input.y;
    this.moveVector.y = input.x;
  }

  public void Jump(InputAction.CallbackContext ctx) {
    // TODO: check if on ground and input was down
    this.rb.AddForce(this.jumpForce);
  }
}