using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    public float yawSensitivity = 0.5f;
    public float pitchSensitivity = 0.5f;
    public float speed = 5f;

    public bool hideHudOnPause = false;
    public bool paused = false;

    public GameObject cameraObject;
    public GameObject hud;
    public GameObject pauseMenu;

    InputAction moveAction;
    InputAction lookAction;
    InputAction jumpAction;

    float yaw;
    float pitch;

    private void Start() {
        this.moveAction = InputSystem.actions.FindAction("Move");
        this.jumpAction = InputSystem.actions.FindAction("Jump");
        this.lookAction = InputSystem.actions.FindAction("Look");

        this.yaw   = 0;
        this.pitch = 0;      
    }

    public void Pause() {
        Cursor.lockState = CursorLockMode.None;
        this.paused = true;

        this.pauseMenu.GetComponent<Canvas>().enabled = true;
        this.hud.GetComponent<Canvas>().enabled = !this.hideHudOnPause;
    }

    public void Unpause() {
        Cursor.lockState = CursorLockMode.Locked;
        this.paused = false;

        this.pauseMenu.GetComponent<Canvas>().enabled = false;
        this.hud.GetComponent<Canvas>().enabled = true;
    }

    void Update() {
        if(Input.GetKey(KeyCode.Escape)) {
            this.Pause();
        } else if(Input.GetKey(KeyCode.Space)) {
            this.Unpause();
        }

        if(this.paused) return;

        // Movement
        Vector2 moveValue = this.moveAction.ReadValue<Vector2>();

        Vector3 move = new Vector3(moveValue.x, 0, moveValue.y);

        this.gameObject.transform.Translate(move * Time.deltaTime * this.speed);

        // Camera
        Vector2 lookValue = this.lookAction.ReadValue<Vector2>();

        yaw   += lookValue.x * this.yawSensitivity;
        pitch -= lookValue.y * this.pitchSensitivity;

        pitch = Math.Clamp(pitch, -90, 90);

        Quaternion cameraRotation = Quaternion.Euler(pitch, 0, 0);
        Quaternion playerRotation = Quaternion.Euler(0, yaw, 0);

        this.cameraObject.transform.localRotation = cameraRotation;
        this.gameObject.transform.rotation = playerRotation;
    }
}
