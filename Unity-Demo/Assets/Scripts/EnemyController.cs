using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {
  NavMeshAgent agent;

  void Start() {
    this.agent = this.GetComponent<NavMeshAgent>();
  }

  void Update() {
    this.agent.destination = GameObject.Find("Player").transform.position;
  }
}
