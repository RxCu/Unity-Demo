using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using System;
using Unity.VisualScripting;


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

  public bool setTag = true;
  public float speed = 5f;
  public float jumpHeight = 10f;
  public int maxJumps = 2;
  public float validJumpDistance = 0.5f;
  public float validPickupDistance = 1.0f;
  public float holdDistanceMultiplier = 1.5f;

  public double maxHealth = 100f;
  public double defaultHealth = 100f;

  public PlayerHealth health = new PlayerHealth(100, 100);

  public bool resetSceneOnDeath = false;

  public bool paused = true;
  public bool firstPerson = true;

  public Vector3 respawnPoint = new Vector3(0.0f, 1.0f, 0.0f);

  InputAction jumpAction;
  int jumpNum = 0;
  bool canBufferJump = true;

  Vector2 moveVector;

  Camera mainCamera;
  CinemachineBrain cinemachineBrain;

  RaycastHit hit;
  RaycastHit holdHit;

  Rigidbody rb;

  GameObject heldObject;

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

    this.health = new PlayerHealth(this.maxHealth, this.defaultHealth);

    this.rb = this.GetComponent<Rigidbody>();
    this.moveVector = new Vector2(0, 0);
    this.jumpAction = InputSystem.actions.FindAction("Jump");

    // Initialize (Cinemachine) Camera
    if(this.cinemachineBrain == null) {
      if(CinemachineBrain.ActiveBrainCount < 0) {
        throw new MissingMemberException("Could not find Cinemachine Brain");
      }

      this.cinemachineBrain = CinemachineBrain.GetActiveBrain(0);
    }

    this.mainCamera = this.cinemachineBrain.OutputCamera;

    // Initialize Raycasts
    this.hit = new RaycastHit();
    this.holdHit = new RaycastHit();

    if(this.paused) {
      this.Pause();
    }
    else {
      this.Unpause();
    }
  }

  void OnValidate() {
    this.health.maxHealth = this.maxHealth;
    this.health.defaultHealth = this.defaultHealth;
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

    if(this.UpdatePaused()) return;

    this.UpdateRotation();
    this.UpdateMovement();
    this.UpdateHeld();
  }

  bool UpdatePaused() {
    // TODO: add proper pause actions
    if(Input.GetKey(KeyCode.Escape)) {
      this.Pause();
    } else if(Input.GetKey(KeyCode.Return)) {
      this.Unpause();
    }

    return this.paused;
  }

  void UpdateRotation() {
    // set player yaw
    Quaternion rot = this.mainCamera.transform.rotation;
    rot.x = 0;
    rot.z = 0;

    this.transform.rotation = rot;
  }

  void UpdateMovement() {
    // Copy data into new Struct
    Vector3 velocity = this.rb.linearVelocity;

    velocity.x = this.moveVector.x * this.speed;
    velocity.z = this.moveVector.y * this.speed;

    this.rb.linearVelocity = (velocity.x * transform.forward) +
                             (velocity.y * transform.up) +
                             (velocity.z * transform.right);

    // TODO: fix this jump check system
    if(Physics.Raycast(this.transform.position, -this.transform.up,
        out this.hit, this.validJumpDistance)) {

      if(hit.collider == null) return;

      this.jumpNum = 0;

      if(this.canBufferJump && this.jumpAction.phase == InputActionPhase.Performed) {
        this.Jump();
      }
    }
  }

  void UpdateHeld() {
    if(this.heldObject == null) return;

    this.heldObject.transform.position = this.transform.position;
    this.heldObject.transform.rotation = this.mainCamera.transform.rotation;

    this.heldObject.transform.position += this.mainCamera.transform.forward * this.holdDistanceMultiplier;
  }

  /*
   * Callbacks
   */

  public void OnMove(InputAction.CallbackContext ctx) {
    Vector2 input = ctx.ReadValue<Vector2>();

    this.moveVector.x = input.y;
    this.moveVector.y = input.x;
  }

  public void OnJump(InputAction.CallbackContext ctx) {
    if(ctx.phase == InputActionPhase.Canceled) {
      this.canBufferJump = true;
      return;
    }

    this.Jump();
  }

  public void OnInteract(InputAction.CallbackContext ctx) {
    if(ctx.phase == InputActionPhase.Canceled) {
      return;
    }

    if(this.heldObject != null) {
      this.heldObject.GetComponent<Rigidbody>().isKinematic = false;
      this.heldObject.GetComponent<Collider>().isTrigger = false;
      this.heldObject = null;
      return;
    }

    Vector3 target = this.mainCamera.transform.position;
    Vector3 rotation = this.mainCamera.transform.forward;

    if(Physics.Raycast(target, rotation, out this.holdHit, this.validPickupDistance)) {
      if(holdHit.collider == null ||
         this.holdHit.collider.gameObject.tag != "Holdable") return;

      // Hold the object
      this.heldObject = this.holdHit.collider.gameObject;

      this.heldObject.GetComponent<Rigidbody>().isKinematic = true;
      this.heldObject.GetComponent<Collider>().isTrigger = true;
    }

  }

  void Jump() {
    if(this.jumpNum >= this.maxJumps) return;

    // TODO: account for gravity (esp with air jumps)
    this.canBufferJump = false;
    this.rb.AddForce(this.transform.up * this.jumpHeight, ForceMode.Impulse);
    this.jumpNum++;
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