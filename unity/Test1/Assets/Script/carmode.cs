using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carmode : MonoBehaviour {
    public float speed = 5;
    public float angularSpeed = 30;
    private Rigidbody rigidbody;
	// Use this for initialization
	void Start () {
        rigidbody = this.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        float v = Input.GetAxis("VerticalCar" );
        rigidbody.velocity = -transform.forward * v * speed;

        float h = Input.GetAxis("HorizontalCar");
        rigidbody.angularVelocity = transform.up * h * angularSpeed;
	}
}
