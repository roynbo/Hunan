using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegMovement : MonoBehaviour {
    public float speed = 5;
    private Rigidbody rigidbody;
	// Use this for initialization
	void Start () {
        rigidbody = this.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Alpha8))
        rigidbody.transform.Rotate(Vector3.back  * speed);
        if(Input.GetKeyDown(KeyCode.Alpha2))
            rigidbody.transform.Rotate(-Vector3.back * speed);
	}
}
