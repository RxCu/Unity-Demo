using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour {
  public GameObject pauseMenu;

  public bool isPaused = true;
  public bool isOnline = false;
  public bool debug = false;

  void Start() {
    if(this.debug) Debug.Log("Scene: #" + Convert.ToString(GameManager.GetScene()));

    this.Resume();
  }

  public void LoadLevelByIndex(int level) {
    SceneManager.LoadScene(level);
  }
  
  public void LoadLevelByName(string level) {
    SceneManager.LoadScene(level);
  }

  public void MainMenu() {
    this.LoadLevelByName("Scenes/MainMenu");
  }

  public static int GetScene() {
    return SceneManager.GetActiveScene().buildIndex;
  }

  public void Pause() {
    if(this.isPaused) {
      this.Resume();
      return;
    }

    this.isPaused = true;

    if(this.pauseMenu != null)
      this.pauseMenu.SetActive(true);

    Cursor.visible = true;
    Cursor.lockState = CursorLockMode.None;

    if(!this.isOnline)
      Time.timeScale = 0.0f;
  }

  public void Resume() {
    this.isPaused = false;

    if(this.pauseMenu != null)
      this.pauseMenu.SetActive(false);

    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;

    if(!this.isOnline)
      Time.timeScale = 1.0f;
  }

  public void Quit() {
    // Save stuff here?
    Application.Quit();
  }
}