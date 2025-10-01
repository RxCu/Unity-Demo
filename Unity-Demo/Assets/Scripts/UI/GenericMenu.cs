using System;
using System.Collections.Generic;

using UnityEngine;


namespace UI {
  public interface IMenu {
    public bool Enabled { get; set; }
    public bool Visible { get; set; }
    public string Name { get; }
    public GameObject MenuGameObject { get; }

    public void MenuUpdate();

    public void Show();
    public void Hide();
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
        this.HideMenu(menu);
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

      return top;
    }

    public void HideMenu(IMenu menu) {
      menu.Visible = false;
      menu.Hide();
      menu.MenuGameObject.SetActive(false);
    }

    void ShowMenu(IMenu menu) {
      menu.MenuGameObject.SetActive(true);
      menu.Visible = true;
      menu.Show();
    }
  }



  public class GenericMenu : MonoBehaviour, IMenu {
    bool _visible = false;
    bool _enabled = false;

    public string menuName = "Menu";

    public bool Visible { get => this._visible; set => this._visible = value; }
    public bool Enabled { get => this._enabled; set => this._enabled = value;}
    public string Name { get => menuName; }
    public GameObject MenuGameObject {get => this.gameObject;}

    public void MenuUpdate() {

    }

    public void Show() {

    }

    public void Hide() {

    }
  }
}