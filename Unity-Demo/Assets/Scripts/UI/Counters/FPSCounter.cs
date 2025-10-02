using System;
using UnityEngine;
using TMPro;


namespace UI {
  [RequireComponent(typeof(TextMeshProUGUI))]
  public class FPSCounter : HUDElement {
    public string suffix = " FPS";
    public int decimals = 1;
    public double fps;
  
    double point;
  
    TextMeshProUGUI textComponent;
  
    void Start() {
      this.textComponent = this.GetComponent<TextMeshProUGUI>();
    }
  
    // Run with Start and public field change.
    void OnValidate() {
      this.point = Math.Pow(10, this.decimals);
    }
  
    new void ElementUpdate() {
      // FPS is the number of frames over runtime
      this.fps = Time.frameCount / Time.timeAsDouble;
      this.textComponent.text = Convert.ToString(Math.Floor(this.point * this.fps) / this.point) + this.suffix;
    }
  }
}
