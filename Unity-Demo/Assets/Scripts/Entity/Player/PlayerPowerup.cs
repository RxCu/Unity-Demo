using System;
using System.Collections;
using UnityEngine;



public class PlayerPowerup : MonoBehaviour {
  public float multiplier = 1;
	bool collectible = true;

  Renderer renderer;
	

[Header("Jumping")]
  public bool isJump = true;


[Header("Respawn")]
	public bool respawn = true;
	public float respawnTime = 5.0f;

	void Start() {
		this.renderer = this.GetComponent<Renderer>();
	}

  void OnTriggerEnter(Collider other) {
		//if(!(this.collectible) || (other.tag != "Player")) return;

		PlayerController player = other.gameObject.GetComponent<PlayerController>();

		if(!player) return;

		player.currentPowerup = this;

		this.collectible = false;
		this.renderer.enabled = false;
		
		if(this.respawn) StartCoroutine("Respawn");	
	}

	IEnumerator Respawn() {
    yield return new WaitForSeconds(this.respawnTime);
		
		this.collectible = true;
		this.renderer.enabled = true;
	}
}
