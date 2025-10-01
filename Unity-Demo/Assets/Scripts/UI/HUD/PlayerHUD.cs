using System;
using System.Collections.Generic;

using UnityEngine;

using UI;

namespace UI {
  public class HUDElement : MonoBehaviour {
    public void ElementUpdate() {
      
    }
  }
  
  public class PlayerHUD : GenericMenu {
    public List<HUDElement> elements;
    
    public void MenuUpdate() {
      foreach(HUDElement elm in this.elements) {
        elm.ElementUpdate();
      }
    }
  }
}