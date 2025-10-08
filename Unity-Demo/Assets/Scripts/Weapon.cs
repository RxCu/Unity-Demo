using System;
using System.Collections;
using UnityEngine;


public class Weapon : MonoBehaviour {
  PlayerController player;

  public GameObject projectile;
  public AudioSource weaponSpeaker;
  public Transform firePoint;
  public Camera firingDirection;

  [Header("Meta")]
  public bool fireable = true;
  public bool reloading = false;
  public bool auto = false;
  public int weaponID;
  public string weaponName;

  [Header("General")]
  public float projLifespan;
  public float projVelocity;
  public float reloadCooldown;
  public float rof;
  public int fireModes;
  public int currentFireMode;
  public int clip;
  public int clipSize;

  [Header("Ammo")]
  public int ammo;
  public int maxAmmo;
  public int ammoRefill;
	public bool infiniteAmmo = false;

  void Start() {
    this.weaponSpeaker = this.GetComponent<AudioSource>();

    if(this.firePoint == null)
      this.firePoint = this.transform.GetChild(0);
  }

  public void Attack() {
    // Melee weapons have negative weapon IDs
    if(!this.fireable || this.clip <= 0) return;

    this.weaponSpeaker.Play();

    if(weaponID > 0) {
      this.Fire();
    }
  }

  void Fire() {
    GameObject p = Instantiate(this.projectile, this.firePoint.position, this.firePoint.rotation);

    p.GetComponent<Rigidbody>().AddForce(this.firingDirection.transform.forward * this.projVelocity);

    Destroy(p, this.projLifespan);

    this.clip--;
    this.fireable = false;
    StartCoroutine("cooldownFire");
  }

  public void Reload() {
    if(this.clip >= this.clipSize) return;

    int reloadCount = Math.Clamp(this.ammo, 0, this.clipSize - this.clip);

    this.clip += reloadCount;

		if(!this.infiniteAmmo)
			this.ammo -= reloadCount;

		this.reloading = true;
    StartCoroutine("cooldownReload");
  }

  public void Equip(PlayerController player) {
    if(player.currentWeapon != null) {
      player.currentWeapon.Unequip();
    }

    player.currentWeapon = this;

    this.transform.SetPositionAndRotation(player.weaponAnchor.position, player.weaponAnchor.rotation);
    this.transform.SetParent(player.weaponAnchor);

    this.GetComponent<Rigidbody>().isKinematic = true;
    this.GetComponent<Collider>().isTrigger = true;

    // Get player camera
    this.firingDirection = player.mainCamera;
    this.player = player;
  }

  public void Unequip() {
    player.currentWeapon = null;

    this.transform.SetParent(null);

    this.GetComponent<Rigidbody>().isKinematic = false;
    this.GetComponent<Collider>().isTrigger = false;

    this.firingDirection = null;
    this.player = null;
  }

  IEnumerator cooldownFire() {
    yield return new WaitForSeconds(this.rof);

    this.fireable = clip > 0;
  }

	IEnumerator cooldownReload() {
    yield return new WaitForSeconds(this.reloadCooldown);

		this.reloading = false;
		this.fireable = true;
	}
}
