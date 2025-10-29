using System;
using System.Collections.Generic;


public class GameState {
	public string name;

	public int jumps;
	public int fires;

	// Time
	float playtime; // "Running" Time
	float startTime; // Absolute Session Time

	public float Playtime {
		get {
			return this.playtime + UnityEngine.Time.time - this.startTime;
		}
	}
	
	public float musicVolume;

	public List<bool> unlocked;
	
	// TODO: Find player state
	public GameState(uint levels) {
		this.musicVolume = 0.5f;
		this.unlocked = new();

		this.jumps = 0;
		this.playtime  = 0.0f;
		this.StartTimer();
		this.fires = 0;

		for(uint i = 0; i < levels; i++) {
			this.unlocked.Add(false);
		}
	}

	public void StartTimer() {
		this.startTime = UnityEngine.Time.time;
	}

	public void LoadState() {
	}
	
	public void SaveState() {
		this.playtime = this.Playtime;
		this.StartTimer();
	}
}
