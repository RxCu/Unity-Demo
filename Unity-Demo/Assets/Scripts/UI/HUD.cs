using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using UI;

namespace UI {
  public class HUD : GenericMenu {
    
[Header("Ammo")]
    public GameObject ammoPanel;
    public TextMeshProUGUI clip;
    public TextMeshProUGUI clipSize;
[Header("Cursor")]
    public GameObject cursorPanel;
    public RawImage cursorBase;
    public RawImage cursorOutline;

		public Color cursorOutlineColor;
		public Color cursorOutlinePowerupColor;
    
    void Start() {
      this.OnDropWeapon();

			if(this.cursorOutlineColor == null) {
				this.cursorOutlineColor = this.cursorOutline.color;
			}
    }
    
    public void OnWeaponChange(Weapon weapon) {
      this.clip.text     = Convert.ToString(weapon.clip);
      this.clipSize.text = Convert.ToString(weapon.clipSize);
      
      this.ammoPanel.SetActive(true);
    }
    
    public void OnDropWeapon() {
      this.clip.text     = "-";
      this.clipSize.text = "-";
      
      this.ammoPanel.SetActive(false);
    }

		public void OnPowerupChange(PlayerPowerup powerup) {
			if(powerup == null) {
				this.cursorOutline.color = this.cursorOutlineColor;
			} else {
				this.cursorOutline.color = this.cursorOutlinePowerupColor;
			}
		}
  }
}
