using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class OptionsMenu : GenericMenu {
		public GameManager gameManager;
		
		public Slider musicVolumeSlider;

		void Start() {
			this.musicVolumeSlider.normalizedValue = this.gameManager.musicVolume;
		}
		
		public void HandleMusicVolume() {
			this.gameManager.SetMusicVolume(this.musicVolumeSlider.normalizedValue);
		}
	}
}
