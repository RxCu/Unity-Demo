using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using UI;

namespace UI {
  public class HealthInterface : HUDElement {
    public EntityHealth entityHealth;

    [Header("Health Text")]
    public TextMeshProUGUI textComponent;

    public string prefix = "";
    public string suffix = " HP";
    public bool textUsePercent = false;
    public bool textShowOfMax = false;
    public string separator = " / ";

    public int decimals = 1;

    double point;


    double health = 0.0;
    double maxHealth = 0.0;
    double percent = 0.0;

    [Header("Health Bar")]
    public Slider healthSlider;

    void Start() {
      this.entityHealth.onChanged.AddListener(this.OnHealthChanged);
    }

    void OnValidate() {
      this.point = Math.Pow(10, this.decimals);
      this.OnHealthChanged();
    }

    public string GetText() {
      double health = this.textUsePercent ? this.percent : this.health;

      string current = this.prefix + Convert.ToString(Math.Floor(this.point * health) / this.point);

      if(this.textShowOfMax) {
        double maxHealth = this.textUsePercent ? 1.0 : this.maxHealth;
        current += this.separator + Convert.ToString(Math.Floor(this.point * maxHealth) / this.point);
      }

      return current + this.suffix;
    }

    public void OnHealthChanged() {
      this.health = this.entityHealth.Health;
      this.maxHealth = this.entityHealth.maxHealth;
      this.percent = this.entityHealth.Percent;

      if(this.textComponent)
        this.textComponent.text = this.GetText();

      if(this.healthSlider)
        this.healthSlider.value = Convert.ToSingle(this.percent);
    }
  }
}