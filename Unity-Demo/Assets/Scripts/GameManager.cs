using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

using UI;


public class GameManager : MonoBehaviour {
  public static GameState State;
 
  [Header("Game Type")]
  public bool isOnline = false;
  public bool isDebug = false;
  public float timeScale = 1.0f;

  [Header("Cursor")]
  public bool cursorLocked = true;
  public bool cursorVisible = false;
  public HUD hud;

  [Header("Music")]
  public List<AudioSource> songList;
  public int currentSong = 0;

  [Header("Menus")]
  public string initialMenu = "";
  public string pauseMenu = "";
  public bool isMenu = true;
  public List<GenericMenu> menuList;

	// Not in inspector
  public Dictionary<string, IMenu> menus;
  public MenuStack menuStack = new MenuStack();
  public IMenu currentMenu;

  void Start() {
    if(this.isDebug) Debug.Log("Scene: #" + Convert.ToString(SceneManager.GetActiveScene().buildIndex));

    if(State == null) {
      State = new(2);
    }

    this.menus = new Dictionary<string, IMenu>();

    foreach(IMenu menu in this.menuList) {
      this.menus[menu.Name] = menu;
      menu.Enabled = false;
      this.menuStack.HideMenu(menu);
    }

    foreach(AudioSource song in this.songList) {
        song.Stop();
    }

    if(this.currentSong > -1)  this.StartSongIndex(this.currentSong);

    this.OnValidate();
  }
  
  void OnValidate() {
    if(this.menus == null)
      return;

    if(this.menuStack.Count == 0 && this.isMenu) {
      this.PushMenu(this.initialMenu);
    } else {
      this.PopMenu();
      this.menuStack.ShowMenu(this.hud);
    }
  }

  void Update() {
    if(this.isMenu && this.menuStack.Count > 0) {
      this.currentMenu = this.menuStack.Top;
      this.currentMenu.MenuUpdate();
    }
  }

  public void SetMusicVolume(float volume) {
      State.musicVolume = volume;

      if(this.currentSong > -1) {
          this.songList[this.currentSong].volume = State.musicVolume;
      }
  }

  public void StartSongIndex(int index) {
      if(this.currentSong > -1) {
          this.songList[this.currentSong].Stop();
      }
      
      this.songList[index].volume = State.musicVolume;
      this.songList[index].Play();
      this.currentSong = index;
  }

  public void LoadLevelByIndex(int level) {
    SceneManager.LoadScene(level);
  }

  public void LoadLevelByName(string level) {
      if(this.currentSong > -1) {
          this.songList[this.currentSong].Stop();
      }
    SceneManager.LoadScene(level);
  }

  public void PushMenu(string key) {
    if(!this.menus.ContainsKey(key)) {
      return;
    }

    this.menus.TryGetValue(key, out IMenu menu);

    if(menu == null) return;
    if(menu == this.menuStack.Top) {
      this.PopMenu();
      return;
    }

    this.menuStack.Push(menu);
    this.isMenu = true;
    this.menuStack.HideMenu(this.hud);

    Cursor.lockState = menu.CursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
    Cursor.visible = menu.CursorVisible;

    Time.timeScale = menu.Paused ? 0.0f : this.timeScale;
  }

  public IMenu PopMenu() {
    IMenu menu = this.menuStack.Pop();
    if(this.menuStack.Count == 0) {
      this.isMenu = false;

      Cursor.lockState = this.cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
      Cursor.visible = this.cursorVisible;

      Time.timeScale = this.timeScale;
      this.menuStack.ShowMenu(this.hud);
    }
    return menu;
  }

  // Why does unity not allow functions with return values in the inspector?
  public void CloseMenu() {
    this.PopMenu();
  }

  public void Quit() {
    State.SaveState();
    Application.Quit();
  }
}
