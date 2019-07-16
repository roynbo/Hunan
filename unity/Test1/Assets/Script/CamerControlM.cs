using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerControlM : MonoBehaviour {
    public float near = 20;
    public float far = 100;
    public Transform car;

    public float sensitivityX = 10;
    public float sensitivityY = 10;
    public float sensitivetyZ = 2;
    public float sensitivetyMove = 2;
    public float sensitivetyMouseWheel = 6;
    private GameObject camera_0;
    private GameObject camera_1;
    private GameObject camera_2;
    private Camera camera;
	// Use this for initialization
	void Start () {
        camera_0 = GameObject.Find("Main Camera");
        camera_1 = GameObject.Find("Second Camera");
        camera_2 = GameObject.Find("Third Camera");
        camera_0.SetActive(true);
        camera_1.SetActive(false);
        camera_2.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (camera_0.activeSelf)
        {
            camera = camera_0.GetComponent<Camera>();
        }
        else if (camera_1.activeSelf)
            camera = camera_1.GetComponent<Camera>();
        else
            camera = camera_2.GetComponent<Camera>();
            
        // 滚轮实现镜头缩进和拉远
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
          
                float num2 = camera.fieldOfView;
                num2 = num2 - Input.GetAxis("Mouse ScrollWheel") * sensitivetyMouseWheel;
                num2 = Mathf.Clamp(num2, near, far);
                camera.fieldOfView = num2;
            
        }
        if (Input.GetKey(KeyCode.F1))
        {
            camera_0.SetActive(true);
            camera_1.SetActive(false);
            camera_2.SetActive(false);
        }
        if (Input.GetKey(KeyCode.F2))
        {

            camera_0.SetActive(false);
            camera_1.SetActive(true);
            camera_2.SetActive(false);
        }
        if (Input.GetKey(KeyCode.F3))
        {

            camera_0.SetActive(false);
            camera_1.SetActive(false);
            camera_2.SetActive(true);
        }
	}
}
