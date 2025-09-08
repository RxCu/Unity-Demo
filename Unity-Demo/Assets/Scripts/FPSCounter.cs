using System;
using UnityEngine;
using TMPro;


public class FPSCounter : MonoBehaviour {
    public string suffix = " FPS";
    public int decimals = 1;
    public double fps;

    private double point;
    
    private TextMeshProUGUI textComponent;

    void Start() {
        this.textComponent = this.gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Run with Start and public field change.
    void OnValidate() {
        this.point = Math.Pow(10, this.decimals);
    }

    void Update() {
        // FPS is the number of frames over runtime
        this.fps = Time.frameCount / Time.timeAsDouble;
        this.textComponent.text = Convert.ToString(Math.Floor(this.point * fps) / this.point) + this.suffix;
    }
}
