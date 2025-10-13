using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour {
  public bool disableRenderer = true;
  public bool setTrigger = true;
    
  public UnityEvent onTrigger = new UnityEvent();

  void Start() {
    this.tag = "PlayerTrigger";

    if(setTrigger) {
      Collider collide = this.GetComponent<Collider>();

      if(collide)  collide.isTrigger = true;
    }

    if(disableRenderer) {
      Renderer render = this.GetComponent<Renderer>();

      if(render) render.enabled = false;
    }
  }
}
