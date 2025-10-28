using System;
using System.Collections.Generic;


public class GameState {
	public string name;
	
	public float musicVolume;

	public List<bool> unlocked;
	
	// TODO: Find player state
	public GameState(uint levels) {
		this.musicVolume = 0.5f;
		this.unlocked = new();

		for(uint i = 0; i < levels; i++) {
			this.unlocked.Add(false);
		}
	}

	public void LoadState() {
	}
	
	public void SaveState() {
	}
}
