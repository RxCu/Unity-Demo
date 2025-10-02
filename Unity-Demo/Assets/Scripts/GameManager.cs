using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

using UI;


public class GameManager : MonoBehaviour {
  [Header("Game Type")]
  public bool isOnline = false;
  public bool isDebug = false;
  public float timeScale = 1.0f;

  [Header("Cursor")]
  public bool cursorLocked = true;
  public bool cursorVisible = false;

  [Header("Menus")]
  public string initialMenu = "";
  public bool isMenu = true;
  public List<GenericMenu> menuList;
  Dictionary<string, IMenu> menus;
  MenuStack menuStack = new MenuStack();
  IMenu currentMenu;


  void Start() {
    if(this.isDebug) Debug.Log("Scene: #" + Convert.ToString(SceneManager.GetActiveScene().buildIndex));

    this.menus = new Dictionary<string, IMenu>();

    foreach(IMenu menu in this.menuList) {
      this.menus[menu.Name] = menu;
      menu.Enabled = false;
      this.menuStack.HideMenu(menu);
    }

    this.OnValidate();
  }
  
  void OnValidate() {
    if(this.menus == null)
      return;

    if(this.menuStack.Count == 0 && this.isMenu) {
      this.PushMenu(this.initialMenu);
    } else {
      this.PopMenu();
    }
  }

  void Update() {
    if(this.isMenu && this.menuStack.Count > 0) {
      this.currentMenu = this.menuStack.Top;
      this.currentMenu.MenuUpdate();
    }
  }

  public void LoadLevelByIndex(int level) {
    SceneManager.LoadScene(level);
  }

  public void LoadLevelByName(string level) {
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
    }
    return menu;
  }

  // Why does unity not allow functions with return values in the inspector?
  public void CloseMenu() {
    this.PopMenu();
  }

  public void Quit() {
    Application.Quit();
  }
}