using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

  public GameObject Star { get; set; }

  public float Speed { get; set; }

  private void Start() {
    Speed = Random.Range(1f, 10f);
  }

  // Update is called once per frame
  void Update () {
		if (Star != null) {
      transform.RotateAround(Star.transform.position, Star.transform.up, Speed*Time.deltaTime);
    }
	}
}
