using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using System;


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
  Rigidbody rb;
  LayerMask mask;

  [Header("Movement")]
  public float speed = 5f;

  Vector2 moveVector = Vector2.zero;

  [Header("Jumping")]
  public float jumpHeight = 5f;
  public int maxJumps = 1;
  public float coyoteTime = 0.1f;
  public float groundDetectDistance = 0.5f;

  int jumpCount = 0;
  double fallTime = 0;

  RaycastHit jumpHit;

  InputAction jumpAction;

  [Header("Interaction")]
  public float validInteractDistance = 1.0f;

  RaycastHit interactHit;

  [Header("Health")]
  public double maxHealth = 100f;
  public double defaultHealth = 100f;
  public bool resetSceneOnDeath = false;
  public Vector3 respawnPoint = new Vector3(0.0f, 1.0f, 0.0f);

  public PlayerHealth health = new PlayerHealth(100, 100);

  [Header("Weapons")]
  public Weapon currentWeapon;
  public Transform weaponAnchor;

  bool attacking = false;

  [Header("Holding")]
  public float holdDistanceMultiplier = 1.5f;
  public float heldObjectMoveTime = 0.2f;

  GameObject heldObject;
  Vector3 heldObjectTarget = Vector3.zero;
  Vector3 heldObjectVelocity = Vector3.zero;
  Rigidbody heldRb;

  [Header("Camera")]
  public bool firstPerson = true;
  public bool paused = true;
  public Camera mainCamera;
  CinemachineBrain cinemachineBrain;


  void Start() {
    this.mask = LayerMask.GetMask("Player");

    this.health = new PlayerHealth(this.maxHealth, this.defaultHealth);

    this.rb = this.GetComponent<Rigidbody>();
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
    this.jumpHit = new RaycastHit();
    this.interactHit = new RaycastHit();

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

  /*
   * Update
   */

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

    if(this.currentWeapon != null && this.heldObject == null && this.currentWeapon.auto && this.attacking) {
      this.currentWeapon.Attack();
    }
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

  void UpdateMovement() {
    // Copy data into new Struct
    Vector3 velocity = this.rb.linearVelocity;

    velocity.x = this.moveVector.x * this.speed;
    velocity.z = this.moveVector.y * this.speed;

    this.rb.linearVelocity = (velocity.x * transform.forward) +
                             (velocity.y * transform.up) +
                             (velocity.z * transform.right);


    if(Physics.Raycast(this.transform.position, -this.transform.up,
        out this.jumpHit, this.groundDetectDistance)) {

      this.fallTime = 0;
      this.jumpCount = 0;

      return;
    }

    // Check coyote time if not on ground
    this.fallTime += Time.deltaTime;

    if(this.fallTime >= this.coyoteTime && this.jumpCount < 1) {
      this.jumpCount = 1;
    }
  }

  void UpdateRotation() {
    // set player yaw
    Quaternion rot = this.mainCamera.transform.rotation;
    rot.x = 0;
    rot.z = 0;

    this.transform.rotation = rot;
    this.weaponAnchor.transform.rotation = this.mainCamera.transform.rotation;
  }

  void UpdateHeld() {
    if(this.heldObject == null) return;

    this.heldObjectTarget = this.transform.position + this.mainCamera.transform.forward * this.holdDistanceMultiplier;
    Vector3.SmoothDamp(this.heldObject.transform.position, this.heldObjectTarget, ref this.heldObjectVelocity, this.heldObjectMoveTime);
    this.heldRb.linearVelocity = this.heldObjectVelocity;
    // TODO: damp rotation
    this.heldObject.transform.rotation = this.mainCamera.transform.rotation;
  }

  /*
   * Misc Player Actions
   */

  void TryJump() {
    if(this.jumpCount >= this.maxJumps) return;

    Vector3 jumpVelocity = this.rb.linearVelocity;
    jumpVelocity.y = this.jumpHeight;
    this.rb.linearVelocity = jumpVelocity;

    this.jumpCount += 1;
  }

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

  /*
   * Input Callbacks
   */

  public void Move(InputAction.CallbackContext ctx) {
    Vector2 input = ctx.ReadValue<Vector2>();

    this.moveVector.x = input.y;
    this.moveVector.y = input.x;
  }

  public void Jump(InputAction.CallbackContext ctx) {
    if(ctx.phase == InputActionPhase.Canceled) return;
    this.TryJump();
  }

  public void Interact(InputAction.CallbackContext ctx) {
    if(ctx.phase == InputActionPhase.Canceled) return;

    if(this.heldObject != null) {
      this.heldRb.useGravity = true;
      this.heldObject.GetComponent<Collider>().excludeLayers = 0;
      this.heldObject = null;
      return;
    }

    if(!Physics.Raycast(this.mainCamera.transform.position,
      this.mainCamera.transform.forward, out this.interactHit, this.validInteractDistance))
      return;

    if(this.interactHit.collider == null) return;

    GameObject other = this.interactHit.collider.gameObject;

    switch(other.tag) {
      case "Holdable":
        this.heldObject = other;
        this.heldRb = this.heldObject.GetComponent<Rigidbody>();

        this.heldRb.useGravity = false;
        this.heldObject.GetComponent<Collider>().excludeLayers = this.mask;
        break;
      case "Weapon":
        other.GetComponent<Weapon>().Equip(this);
        break;
    }
  }

  public void Attack(InputAction.CallbackContext ctx) {
    if(ctx.phase == InputActionPhase.Canceled || this.currentWeapon == null || this.heldObject) {
      this.attacking = false;
      return;
    }

    this.attacking = true;
    this.currentWeapon.Attack();
  }

  public void Reload(InputAction.CallbackContext ctx) {
    if(ctx.phase == InputActionPhase.Canceled || this.currentWeapon == null) return;

    this.currentWeapon.Reload();
  }

  /*
   * Misc Callbacks
   */

  void OnTriggerEnter(Collider other) {
    if(other.tag == "Kill")
      this.health.Kill();

    if(other.tag == "Damage") {
      PlayerDamage info = other.GetComponent<PlayerDamage>();
      info.Take(health);
    }
  }
}