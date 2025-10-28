using System;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

using UI;

namespace UI {
	public class HUD : GenericMenu {
		
		public GameObject ammoPanel;
		
		public TextMeshProUGUI clip;
		public TextMeshProUGUI clipSize;
		
		void Start() {
			this.OnDropWeapon();
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
	}
}
