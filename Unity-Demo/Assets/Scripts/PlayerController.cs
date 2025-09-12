using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

  public float speed = 5f;
  public float jumpHeight = 10f;

  public double maxHealth = 100f;
  public double defaultHealth = 100f;

  public PlayerHealth health = new PlayerHealth(100, 100);

  public bool resetSceneOnDeath = false;

  public bool paused = true;
  public bool firstPerson = true;

  public float yawSensitivity = 30f;
  public float pitchSensitivity = 30f;

  public Vector2 cameraPitchLimits = new Vector2(-90.0f, 90.0f);
  public Vector3 cameraOffset = new Vector3(0.0f, 0.5f, 0.5f);

  public Vector3 respawnPoint = new Vector3(0.0f, 1.0f, 0.0f);
  public bool setTag = true;

  Vector2 moveVector;
  Vector3 jumpForce;

  public Transform cameraAnchor;
  InputAction lookVector;
  Vector2 cameraRotation;

  Rigidbody rb;

  public void Pause() {
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
    this.paused = true;
    Time.timeScale = 0.0f;
  }

  public void Unpause() {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    this.paused = false;
    Time.timeScale = 1.0f;
  }

  void Start() {
    if(this.setTag) {
      this.gameObject.tag = "Player";
    }

    this.rb = this.GetComponent<Rigidbody>();
    this.health = new PlayerHealth(this.maxHealth, this.defaultHealth);

    this.moveVector = new Vector2(0, 0);
    this.lookVector = InputSystem.actions.FindAction("Look");

    this.cameraRotation = Vector2.zero;

    if(this.cameraAnchor == null) {
      this.cameraAnchor = this.transform.GetChild(0);
    }

    if(this.paused) {
      this.Pause();
    } else {
      this.Unpause();
    }
  }

  void Update() {
    if(this.health.IsDead()) {
      if(this.resetSceneOnDeath) {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        return;
      }

      this.health.Reset();
      this.transform.position = this.respawnPoint;
      return;
    }

    if(this.UpdatePaused()) {
      this.UpdateRotation();
      return;
    }

    this.UpdateLook();
    this.UpdateRotation();

    this.UpdateMovement();
  }

  void OnValidate() {
    this.jumpForce = new Vector3(0, this.jumpHeight, 0);
    this.health.maxHealth = this.maxHealth;
    this.health.defaultHealth = this.defaultHealth;
  }

  bool UpdatePaused() {
    // TODO: add proper pause actions
    if(Input.GetKey(KeyCode.Escape)) {
      this.Pause();
    } else if(Input.GetKey(KeyCode.Space)) {
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
    //this.cameraAnchor.position = this.transform.position + this.cameraOffset;

    Vector2 input = this.lookVector.ReadValue<Vector2>();

    this.cameraRotation.x += input.x * this.yawSensitivity * Time.deltaTime;
    this.cameraRotation.y = Mathf.Clamp(this.cameraRotation.y +
      input.y * this.pitchSensitivity * Time.deltaTime, this.cameraPitchLimits.x, this.cameraPitchLimits.y);
  }

  void UpdateRotation() {
    // set player yaw
    this.transform.localRotation = Quaternion.AngleAxis(this.cameraRotation.x, Vector3.up);

    // set camera rotation
    this.cameraAnchor.rotation = Quaternion.Euler(-this.cameraRotation.y, this.cameraRotation.x, 0);
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

  void OnTriggerEnter(Collider other) {
    if(other.tag == "Kill")
      this.health.Kill();

    if(other.tag == "Damage") {
      PlayerDamage info = other.GetComponent<PlayerDamage>();
      info.Take(health);
    }
  }
}