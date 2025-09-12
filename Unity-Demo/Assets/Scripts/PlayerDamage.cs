using System;
using UnityEngine;

public class PlayerHealth {
  public double maxHealth = 100f;
  public double defaultHealth = 100f;

  double health = 1.0;

  public PlayerHealth(double max, double defaulHealth) {
    this.maxHealth = max;
    this.defaultHealth = defaulHealth;
  }

  public void Reset() {
    this.health = this.defaultHealth / this.maxHealth;
  }

  public double GetPercentage() {
    return this.health;
  }

  public void SetPercentage(double percent) {
    this.health = Math.Clamp(percent, 0, this.maxHealth);
  }

  public double Get() {
    return this.GetPercentage() * this.maxHealth;
  }

  public void Set(double hlth) {
    this.SetPercentage(hlth / this.maxHealth);
  }

  public void TakePercentageMax(double damage) {
    this.SetPercentage(this.GetPercentage() - damage);
  }

  public void TakePercentageCurrent(double damage) {
    this.SetPercentage(this.GetPercentage() * damage);
  }

  public void Take(double damage) {
    this.Set(this.Get() - damage);
  }

  public void HealPercentageMax(double heal) {
    this.TakePercentageMax(-heal);
  }

  public void Heal(double heal) {
    this.Take(-heal);
  }

  public void Kill() {
    this.SetPercentage(0.0);
  }

  public bool IsDead() {
    return this.GetPercentage() <= 0.0;
  }
}


public class PlayerDamage : MonoBehaviour {
  public double damage = 10f;

  public bool isHeal = false;
  public bool isPercentage = false;
  public bool isMaxPercentage = true;
  public bool isReset = false;
  public bool setTag = true;

  void Start() {
    if(this.setTag) {
      this.gameObject.tag = "Damage";
    }
  }

  public void Take(PlayerHealth health) {
    if(this.isReset) {
      health.Reset();
      return;
    }

    double dmg = this.isHeal ? -this.damage : this.damage;

    if(this.isPercentage) {
      if(this.isMaxPercentage) {
        health.TakePercentageMax(dmg);
        return;
      }

      health.TakePercentageCurrent(dmg);
      return;
    }

    health.Take(dmg);
  }
}