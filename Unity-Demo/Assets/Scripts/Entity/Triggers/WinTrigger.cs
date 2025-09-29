using UnityEngine;
using TMPro;

public class WinTrigger : MonoBehaviour {
    public GameObject winScreen;
    public GameObject winText;

    void OnTriggerEnter() {
        this.winScreen.GetComponent<Canvas>().enabled = true;
        this.winText.GetComponent<TextMeshProUGUI>().text = "You Win!";
    }
}