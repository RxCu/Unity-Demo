using System;

using TMPro;

namespace UI {
	public class WinMenu : GenericMenu {
		public TextMeshProUGUI infoText;
		
		public void Show() {
			// Update the text
			float time = GameManager.State.Playtime;

			int minutes = (int) (time / 60);
			float seconds = time % 60;
			
			String text = "Time:  ";
		 
			text += Convert.ToString(minutes) + ":" + Convert.ToString(seconds);
			text += "\nJumps: " + Convert.ToString(GameManager.State.jumps);
			text += "\nFires: " + Convert.ToString(GameManager.State.fires);

			this.infoText.text = text;
		}
	}
}
