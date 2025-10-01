using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;


public class GameManager : MonoBehaviour {
  [Header("Menus")]
  public string initialMenu = "";
  public bool isMenu = true;
  public List<GenericMenu> menuList;
  Dictionary<string, IMenu> menus;

  [Header("Game Type")]
  public bool isOnline = false;
  public bool isDebug = false;

  MenuStack menuStack;

  void Start() {
    if(this.isDebug) Debug.Log("Scene: #" + Convert.ToString(GameManager.GetScene()));

    this.menuStack = new MenuStack();
    this.menus = new Dictionary<string, IMenu>();

    foreach(IMenu menu in menuList) {
      this.menus[menu.Name] = menu;
      menu.enabled = false;
      this.menuList.HideMenu(menu);
    }
  }
  
  void OnValidate() {
    if(this.menuStack.Count == 0 && this.isMenu)
      this.PushMenu(this.initialMenu);
  }

  void Update() {
    if(this.isMenu && this.menuStack.Count > 0) {
      this.menuStack.Top.MenuUpdate();
    }
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

  public void PushMenu(string key) {
    if(!this.menus.ContainsKey(key)) {
      return;
    }

    // Why does C# allow this variable declaration?
    this.menus.TryGetValue(key, out IMenu menu);

    if(menu == null) return;

    this.menuStack.Push(menu);
  }

  public IMenu PopMenu() {
    return this.menuStack.Pop();
  }

  public static int GetScene() {
    return SceneManager.GetActiveScene().buildIndex;
  }

  public void Quit() {
    // Save stuff here?
    Application.Quit();
  }
}