using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;


public class EntityHealth : MonoBehaviour {
  [Header("Health")]
  public double maxHealth = 100;
  public double initialHealth = 100;
  public double resetHealth = 100;

  private double _health = 1.0;

  [Header("Triggers")]
  public bool triggerCanDamage = true;
  public bool triggerInstaKill = true;

  [Header("Reset Point")]
  public bool shouldResetHealth = false;
  public bool shouldResetPosition = false;
  public bool shouldResetOnStart = false;
  public bool shouldResetScene = false;
  public Transform resetPosition;

  [Header("Callbacks")]
  public UnityEvent onChanged = new UnityEvent();
  public UnityEvent onDeath = new UnityEvent();

  public double Health {
    get {
      return this.Percent * this.maxHealth;
    }
    set {
      this.Percent = value / this.maxHealth;
    }
  }

  // Most direct access to EntityHealth._health
  public double Percent {
    get {
      return this._health;
    }
    set {
      this._health = Math.Clamp(value, 0, this.maxHealth);
      this.onChanged.Invoke();

      if(this.Dead) {
        this.Reset();
        this.onDeath.Invoke();
      }
    }
  }

  public bool Dead {
    get {
      return this.Percent <= 0.0;
    }
    set {
      if(value) {
        this.Kill();
      } else if(this.Dead) {
        this.Health = this.resetHealth;
      }
    }
  }

  void Start() {
    this.Health = this.initialHealth;
    if(this.shouldResetOnStart) {
      this.ResetPosition();
    }
  }

  void OnTriggerEnter(Collider other) {
    if(this.triggerCanDamage && other.tag == "Kill")
      this.Kill();

    if(this.triggerCanDamage && other.tag == "Damage") {
      EntityDamage info = other.GetComponent<EntityDamage>();
      info.Take(this);
    }
  }

  public void Reset() {
    if(this.shouldResetScene) {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    if(this.shouldResetHealth) {
      this.ResetHealth();
    }
    if(this.shouldResetPosition) {
      this.ResetPosition();
    }
  }

  public void ResetHealth() {
    this.Health = this.resetHealth;
  }

  public void ResetPosition() {
    if(this.resetPosition == null) return;

    this.transform.position = this.resetPosition.position;
    this.transform.rotation = this.resetPosition.rotation;

    // TODO: rotate the Cinemachine camera with the resetPoint
    // PlayerController: UpdateRotation();
  }

  public void TakePercentageMax(double damage) {
    this.Percent -= damage;
  }

  public void TakePercentageCurrent(double damage) {
    this.Percent *= damage;
  }

  public void Take(double damage) {
    this.Health -= damage;
  }

  public void HealPercentageMax(double heal) {
    this.TakePercentageMax(-heal);
  }

  public void Heal(double heal) {
    this.Take(-heal);
  }

  public void Kill() {
    this.Percent = 0;
  }
}