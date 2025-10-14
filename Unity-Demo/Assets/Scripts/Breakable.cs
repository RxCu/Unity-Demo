using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;


public class Breakable : MonoBehaviour {
	public List<string> tags;

	public bool destroyOther;
	public bool hideChildren;

	public UnityEvent callback = new UnityEvent();

	void Start() {
		if(this.hideChildren) {
			this.callback.AddListener(this.HideChildren);
		}
	}

	void OnCollisionEnter(Collision other) {
		// TODO: Check other velocity
		if(this.tags.Contains(other.gameObject.tag)) {
			this.callback.Invoke();
			if(this.destroyOther) Destroy(other.gameObject);
		}
	}

	public void HideChildren() {
		Collider[] childColliders = this.GetComponentsInChildren<Collider>(true);

		foreach(var collider in childColliders) {
			collider.gameObject.SetActive(false);
		}
	}
}
