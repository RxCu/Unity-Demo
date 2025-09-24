using System;
using UnityEngine;
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

  [Header("Callback")]
  public UnityEvent onChanged = new UnityEvent();

  public double Health {
    get {
      return this.Percent * this.maxHealth;
    }
    set {
      this.Percent = value / this.maxHealth;
    }
  }

  public double Percent {
    get {
      return this._health;
    }
    set {
      this._health = Math.Clamp(value, 0, this.maxHealth);
      this.onChanged.Invoke();
    }
  }

  void Start() {
    this.Health = this.initialHealth;
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
    this.Health = this.resetHealth;
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

  public bool IsDead() {
    return this.Percent <= 0.0;
  }
}