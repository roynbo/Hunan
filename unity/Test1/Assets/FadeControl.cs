using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeControl : MonoBehaviour {
	public GameObject bigarm=null;
	public GameObject middlearm=null;
	public GameObject smallarm=null;
	private float length_bigarm=660;
	private float length_middlearm=480;
	private float length_smallarm=450;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float velocity_x=0;
		float velocity_y=0;
		Vector3 bigarm_angle=bigarm.transform.localEulerAngles;
		Vector3 middlearm_angle=middlearm.transform.localEulerAngles;
		Vector3 smallarm_angle=smallarm.transform.localEulerAngles;
		if(Input.GetButtonDown("Horizontal")
		{
			
		}
	}
}
