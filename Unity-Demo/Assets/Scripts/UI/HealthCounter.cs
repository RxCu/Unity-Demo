using System;
using UnityEngine;
using TMPro;


[RequireComponent(typeof(TextMeshProUGUI))]
public class HealthCounter : MonoBehaviour {
  public string suffix = " HP";
  public int decimals = 1;

  public double health = 0.0;
  public EntityHealth entityHealth;

  double point;

  public TextMeshProUGUI textComponent;

  void Start() {
    this.textComponent = this.GetComponent<TextMeshProUGUI>();
    this.entityHealth.onChanged.AddListener(this.OnHealthChanged);
  }

  void OnValidate() {
    this.point = Math.Pow(10, this.decimals);
    this.OnHealthChanged();
  }

  public void OnHealthChanged() {
    this.health = this.entityHealth.Health;
    this.textComponent.text = Convert.ToString(Math.Floor(this.point * this.health) / this.point) + this.suffix;
  }
}