using System;
using System.Collections.Generic;

using UnityEngine;


namespace UI {
  public interface IMenu {
    public bool Enabled { get; set; }
    public bool Visible { get; set; }
    public bool CursorVisible { get; }
    public bool CursorLocked { get; }
    public bool Paused { get; }
    public string Name { get; }
    public GameObject MenuGameObject { get; }

    public void MenuUpdate() { }

    public void Show() { }
    public void Hide() { }
  }

  public class MenuStack : List<IMenu> {
    public IMenu Top {
      get {
        int index = this.Count - 1;
        if(index < 0) return null;
        return this[index];
      }
    }

    public void Push(IMenu menu) {
      IMenu top = this.Top;
      if(top != null) {
        this.HideMenu(top);
      }

      this.Add(menu);
      menu.Enabled = true;
      this.ShowMenu(menu);
    }

    public IMenu Pop() {
      IMenu top = this.Top;

      if(top == null) return null;

      top.Enabled = false;
      this.HideMenu(top);

      int index = this.Count - 1;
      this.RemoveAt(index);

      IMenu visible = this.Top;

      if(visible != null) {
        this.ShowMenu(visible);
      }

      return top;
    }

    public void HideMenu(IMenu menu) {
      menu.Visible = false;
      menu.Hide();
      menu.MenuGameObject.SetActive(false);
    }

    public void ShowMenu(IMenu menu) {
      menu.MenuGameObject.SetActive(true);
      menu.Visible = true;
      menu.Show();
    }
  }


  public class GenericMenu : MonoBehaviour, IMenu {
    bool _visible = false;
    bool _enabled = false;

    [SerializeField]
    protected string menuName = "Menu";
    [SerializeField]
    protected bool cursorLocked = false;
    [SerializeField]
    protected bool cursorVisible = true;
    [SerializeField]
    protected bool pause = true;

    public bool Visible { get => this._visible; set => this._visible = value; }
    public bool Enabled { get => this._enabled; set => this._enabled = value; }
    public bool CursorLocked { get => this.cursorLocked; }
    public bool CursorVisible { get => this.cursorVisible; }
    public bool Paused { get => this.pause; }
    public string Name { get => menuName; }
    public GameObject MenuGameObject {get => this.gameObject; }
  }
}
