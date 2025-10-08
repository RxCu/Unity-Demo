using System;
using UnityEngine;


public class EntityDamage : MonoBehaviour {
  public double damage = 10f;

[Header("Percentage")]
  public bool isPercentage = false;
  public bool isMaxPercentage = true;

[Header("Meta")]
  public bool isHeal = false;
  public bool isReset = false;
  public bool setTag = true;
  public GameObject destroy;

  void Start() {
    if(this.setTag) {
      this.gameObject.tag = "Damage";
    }
  }

  public void Take(EntityHealth health) {
    if(this.isReset) {
      health.Reset();

      if(this.destroy != null) {
        Destroy(this.gameObject);
      }

      return;
    }

    double dmg = this.isHeal ? -this.damage : this.damage;

    if(this.isPercentage) {
      if(this.isMaxPercentage) {
        health.TakePercentageMax(dmg);
      } else {
        health.TakePercentageCurrent(dmg);
      }
    } else {
      health.Take(dmg);
    }

    if(this.destroy != null) {
      Destroy(this.destroy);
    }
  }
}
