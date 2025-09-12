using System;
using UnityEngine;
using TMPro;


[RequireComponent(typeof(TextMeshProUGUI))]
public class HealthCounter : MonoBehaviour {
  public string suffix = " HP";
  public int decimals = 1;

  public double health = 0.0;
  public PlayerController player;

  double point;

  TextMeshProUGUI textComponent;

  void Start() {
    this.textComponent = this.GetComponent<TextMeshProUGUI>();
  }

  void OnValidate() {
    this.point = Math.Pow(10, this.decimals);
  }

  void Update() {
    this.health = this.player.health.Get();
    this.textComponent.text = Convert.ToString(Math.Floor(this.point * this.health) / this.point) + this.suffix;
  }
}