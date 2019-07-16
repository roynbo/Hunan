using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRolling : MonoBehaviour {
	public float rotationSpeed=5.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 autoRoll=new Vector3(0,rotationSpeed,0);
		GetComponent<Rigidbody>().angularVelocity=autoRoll;
	}
}
