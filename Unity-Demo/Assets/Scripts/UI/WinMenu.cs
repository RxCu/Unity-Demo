using System;

using UnityEngine;

using TMPro;

namespace UI {
	public class WinMenu : GenericMenu {
		public TextMeshProUGUI infoText;

		public override void Show() {
			// Update the text
			TimeSpan t = TimeSpan.FromSeconds((double)GameManager.State.Playtime);

			string time = t.ToString(@"mm\:ss\.FF");
			
			if(t.Hours > 0) {
				time = t.ToString(@"h\:") + time;
			}

			// just in case somehow
			if(t.Days > 0) {
				time = t.ToString(@"d\:") + time;
			}
			
			String text = "Time:  " + time;
		 
			text += "\nJumps: " + Convert.ToString(GameManager.State.jumps);
			text += "\nFires: " + Convert.ToString(GameManager.State.fires);

			this.infoText.text = text;
		}
	}
}
