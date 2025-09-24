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
  public float crouchSpeed = 3f; // TODO
  public float sprintSpeed = 7f; // TODO

  Vector2 moveVector = Vector2.zero;
  float currentSpeed;

  bool crouching = false;
  bool sprinting = false;

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

  [Header("Holding")]
  public float holdDistanceMultiplier = 1.5f;
  public float heldObjectMoveTime = 0.2f;

  GameObject heldObject;
  Vector3 heldObjectTarget = Vector3.zero;
  Vector3 heldObjectVelocity = Vector3.zero;
  Rigidbody heldRb;

  [Header("Weapons")]
  public Weapon currentWeapon;
  public Transform weaponAnchor;

  bool attacking = false;

  [Header("Health")]
  public EntityHealth health;
  public Transform resetPoint;
  public bool resetOnStart = true;
  public bool rotateOnDeath = true; // TODO
  public bool resetSceneOnDeath = false;

  [Header("Camera")]
  public bool firstPerson = true; // TODO

  public Camera mainCamera;
  CinemachineBrain cinemachineBrain;

  bool _paused;

  public bool Paused {
    get {
      return this._paused;
    }
    set {
      this._paused = value;

      Cursor.visible = this.Paused;

      if(this.Paused) {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0.0f;
      } else {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1.0f;
      }
    }
  }



  void Start() {
    this.mask = LayerMask.GetMask("Player");

    this.currentSpeed = this.speed;

    this.rb = this.GetComponent<Rigidbody>();
    this.jumpAction = InputSystem.actions.FindAction("Jump");

    if(this.health == null) {
      this.health = this.GetComponent<EntityHealth>();
    }

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

    if(this.resetOnStart) {
      this.ResetPosition();
    }
  }

  /*
   * Update
   */

  void Update() {
    if(this.Paused) return;

    this.UpdateRotation();
    this.UpdateMovement();
    this.UpdateHeld();

    if(this.currentWeapon != null && this.heldObject == null && this.currentWeapon.auto && this.attacking) {
      this.currentWeapon.Attack();
    }
  }

  void UpdateMovement() {
    // Copy data into new Struct
    Vector3 velocity = this.rb.linearVelocity;

    velocity.x = this.moveVector.x * this.currentSpeed;
    velocity.z = this.moveVector.y * this.currentSpeed;

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

  public bool DropHeld() {
    if(this.heldObject == null) return false;

    this.heldRb.useGravity = true;
    this.heldObject.GetComponent<Collider>().excludeLayers = 0;
    this.heldObject = null;
    return true;
  }

  void Jump() {
    if(this.jumpCount >= this.maxJumps) return;

    Vector3 jumpVelocity = this.rb.linearVelocity;
    jumpVelocity.y = this.jumpHeight;
    this.rb.linearVelocity = jumpVelocity;

    this.jumpCount += 1;
  }

  public void Respawn() {
    if(this.resetSceneOnDeath) {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
      return;
    }

    this.DropHeld();

    if(this.health != null)
      this.health.Reset();

    this.ResetPosition();
  }

  public void ResetPosition() {
    if(this.resetPoint == null) return;
    this.transform.position = this.resetPoint.position;
    this.transform.rotation = this.resetPoint.rotation;

    this.UpdateRotation();

    // TODO: rotate the Cinemachine camera with the resetPoint
  }

  /*
   * Input Callbacks
   */

  public void OnMove(InputAction.CallbackContext ctx) {
    Vector2 input = ctx.ReadValue<Vector2>();

    this.moveVector.x = input.y;
    this.moveVector.y = input.x;
  }

  public void OnLook(InputAction.CallbackContext ctx) {
  }

  public void OnCrouch(InputAction.CallbackContext ctx) {
    // On Held
    this.crouching = ctx.phase != InputActionPhase.Canceled;

    if(this.crouching) {
      this.currentSpeed = this.crouchSpeed;
    } else {
      this.currentSpeed = this.speed;
    }
  }

  public void OnSprint(InputAction.CallbackContext ctx) {
    // On Held
    this.sprinting = ctx.phase != InputActionPhase.Canceled;

    if(this.sprinting) {
      this.currentSpeed = this.sprintSpeed;
    } else {
      this.currentSpeed = this.speed;
    }
  }

  public void OnJump(InputAction.CallbackContext ctx) {
    if(ctx.phase == InputActionPhase.Canceled) return;
    this.Jump();
  }

  public void OnInteract(InputAction.CallbackContext ctx) {
    if(ctx.phase == InputActionPhase.Canceled) return;

    if(this.DropHeld()) return;

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

  public void OnAttack(InputAction.CallbackContext ctx) {
    if(ctx.phase == InputActionPhase.Canceled || this.currentWeapon == null || this.heldObject) {
      this.attacking = false;
      return;
    }

    this.attacking = true;
    this.currentWeapon.Attack();
  }

  public void OnReload(InputAction.CallbackContext ctx) {
    if(ctx.phase == InputActionPhase.Canceled || this.currentWeapon == null) return;

    this.currentWeapon.Reload();
  }

  public void OnPaused(InputAction.CallbackContext ctx) {
    if(ctx.phase != InputActionPhase.Started) return;

    this.Paused = !this.Paused;
  }

  /*
   * Misc Callbacks
   */

  public void OnHealthChanged() {
    if(this.health.IsDead()) {
      this.Respawn();
    }
  }
}