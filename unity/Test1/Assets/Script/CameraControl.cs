using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
		private float x=0;
		private float y=0;
		private	bool startPosFlag=true;
		private	bool startPosFlag2=true;
		private	Vector2 startFingerPos;
		private Vector2 nowFingerPos;
		private	Vector2 startFingerPos1;
		private Vector2 nowFingerPos1;
		private	Vector2 startFingerPos2;
		private Vector2 nowFingerPos2;
		private float startFingerX=0;
		private float startFingerY=0;
		private float nowFingerPosX=0;
		private float nowFingerPosY=0;
		public GameObject car;
		Vector3 camera_pos;
		Vector3 camera_rot;
		float distance1=0;
		float distance2=0;
		private float Distance(float x1,float x2,float y1,float y2)
		{
			return Mathf.Sqrt((x1-x2)*(x1-x2)+(y1-y2)*(y1-y2));
		}
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		camera_pos = car.transform.localPosition;
        camera_rot = car.transform.localEulerAngles;
		if(Input.touchCount==1)
        {if ( Input.GetTouch(0).phase == TouchPhase.Began&&startPosFlag == true)

        {
            //Debug.Log("======开始触摸=====");
            startFingerPos = Input.GetTouch(0).position;
			startFingerX=startFingerPos.x;
			startFingerY=startFingerPos.y;			
            startPosFlag = false;
        }

        if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            //Debug.Log("======释放触摸=====");
            startPosFlag = true;
        }
		else
		{
		nowFingerPos = Input.GetTouch(0).position;
		nowFingerPosX=nowFingerPos.x;
		nowFingerPosY=nowFingerPos.y;
		x=nowFingerPosX-startFingerX;
		y=nowFingerPosY-startFingerY;
		camera_rot.y -= x/3;
		camera_rot.z -=y/3;
		car.transform.localEulerAngles = camera_rot;
		startFingerX=nowFingerPosX;
		startFingerY=nowFingerPosY;
		}
       
		}
		else if(Input.touchCount==2)
		{

			if ( Input.GetTouch(0).phase == TouchPhase.Began&&Input.GetTouch(1).phase == TouchPhase.Began&&startPosFlag2 == true)

        {
            //Debug.Log("======双点开始触摸=====");
            startFingerPos1 = Input.GetTouch(0).position;
			startFingerPos2=  Input.GetTouch(1).position;
			distance1=Distance(startFingerPos1.x,startFingerPos2.x,startFingerPos1.y,startFingerPos2.y);
            startPosFlag2 = false;
        }

        if (Input.GetTouch(0).phase == TouchPhase.Ended&&Input.GetTouch(1).phase == TouchPhase.Ended)
        {
            //Debug.Log("======释放触摸=====");
            startPosFlag2 = true;
        }
		else
		{
		nowFingerPos1 = Input.GetTouch(0).position;
		nowFingerPos2 =	Input.GetTouch(1).position;
		distance2=Distance(nowFingerPos1.x,nowFingerPos2.x,nowFingerPos1.y,nowFingerPos2.y);
		Vector3 car_scale=car.transform.localScale;
		if(distance1!=0)
		{
			car_scale*=(1+(distance2-distance1)/distance1/3);		
		}
		else
		{
			car_scale*=1;
		}
		if(car_scale.x>2)
		{
			car_scale=new Vector3(2,2,2);
		}
		else if(car_scale.x<0.3f)
		{
			car_scale=new Vector3(0.3f,0.3f,0.3f);
		}
		car.transform.localScale=car_scale;
		distance1=distance2;
		}
		
		}
	}
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(10,10,100,100));//开始位置为 100,100 宽和高都为100
		if(GUILayout.Button("回归\n原位"))
		{
		camera_rot.y=0;
		camera_rot.z=0;
		car.transform.localEulerAngles = camera_rot;
		car.transform.localScale=new Vector3(0.5f,0.5f,0.5f);
		}
		//GUILayout.Label("distance1	"+distance1);
		//GUILayout.Label("distance2	"+distance2);
		//GUILayout.Label("scale	"+(distance2-distance1)/distance1);
		GUILayout.EndArea();// 有开始就有结束 GUILaout.EndArea()与GUILayout.BeginArea()搭配使用 
	}
}
