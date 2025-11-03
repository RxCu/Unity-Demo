using System;
using System.Collections;
using UnityEngine;



public class PlayerPowerup : MonoBehaviour {
  public float multiplier = 1;
	bool collectible = true;
	PlayerController player;

  Renderer renderer_;
	

[Header("Jumping")]
  public bool isJump = true;


[Header("Respawn")]
	public bool respawn = true;
	public float respawnTime = 5.0f;

	void Start() {
			this.renderer_ = this.GetComponent<Renderer>();
			this.player = null;
	}

	void Collect() {
		if(!this.player) return;
		
		this.player.CollectPowerup(this);

		this.collectible = false;
		this.renderer_.enabled = false;
		
		if(this.respawn) StartCoroutine("Respawn");	
	}

  void OnTriggerEnter(Collider other) {
		if((other.tag != "Player")) return;
		
		this.player = other.gameObject.GetComponent<PlayerController>();

		if(!this.collectible) {
			return;
		}

		this.Collect();
	}

	void OnTriggerExit(Collider other) {
		if((other.tag != "Player")) return;

		this.player = null;
	}

	IEnumerator Respawn() {
    yield return new WaitForSeconds(this.respawnTime);
		
		this.collectible = true;
		this.renderer_.enabled = true;

		if(this.player != null) {
			this.Collect();
		}

		// Check if player is
	}
}
