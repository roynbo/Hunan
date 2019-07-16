using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Text;


[StructLayout(LayoutKind.Sequential, Pack = 4)]

public class ManipulatorControl : MonoBehaviour {
    private const int length_bigarm = 660;
    private const int length_middlearm = 480;
    private const int length_smallarm = 450;

    private float velocityY=0;                           //末端速度，上下
    private float velocityZ =0;                         //末端速度，前后

    private float y1, y2, y3, z1, z2, z3;               //大臂中臂小臂的转动轴位置
    private float theta1, theta2, theta3;               //关节角度，度为单位
    private float omega1, omega2, omega3;               //关节转动角速度，弧度制

    private GameObject bigArm;
    private GameObject middleArm;
    private GameObject smallArm;
    private GameObject swingarm1;
    private GameObject swingarm2;
    private GameObject swingarm3;
    private GameObject swingarm4;
        public struct Status
        {
                public double bigarm_pos;
                public double middlearm_pos;
                public double smallarm_pos;
                public double swingarm1_pos;
                public double swingarm2_pos;
                public double swingarm3_pos;
                public double swingarm4_pos;
        };
        Status status;
    private UdpClient udpSever;
    private IPEndPoint remoteIpEndPoint;
    Thread recThread;
    	void OnApplicationQuit()
    {
        PlayerPrefs.Save();
        recThread.Abort();
        udpSever.Close();
    }
	// Use this for initialization
	void Start () {
        bigArm = GameObject.Find("bigarm");
        middleArm = GameObject.Find("midarma");
        smallArm = GameObject.Find("smallarm");
        swingarm1=GameObject.Find("swingarm1");
        swingarm2=GameObject.Find("swingarm2");
        swingarm3=GameObject.Find("swingarm3");
        swingarm4=GameObject.Find("swingarm4");
        y1 = bigArm.transform.localPosition.y;
        z1 = bigArm.transform.localPosition.z;
                udpSever = new UdpClient(10000);
        remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        recThread = new Thread(udpRecMessage);
        recThread.Start();
	}

	// Update is called once per frame
	void Update () {
        Vector3 swingarm1_rot=swingarm1.transform.localEulerAngles;
        Vector3 swingarm2_rot=swingarm2.transform.localEulerAngles;
        Vector3 swingarm3_rot=swingarm3.transform.localEulerAngles;
        Vector3 swingarm4_rot=swingarm4.transform.localEulerAngles;
        Vector3 big_rot = bigArm.transform.localEulerAngles;
        theta1 = big_rot.z;
        Vector3 middle_pos = middleArm.transform.localPosition;
        Vector3 middle_rot = middleArm.transform.localEulerAngles;
        theta2 = middle_rot.z;
        middle_pos.y = length_bigarm * Mathf.Sin(Mathf.Deg2Rad * theta1) + y1;
        middle_pos.z = -length_bigarm * Mathf.Cos(Mathf.Deg2Rad * theta1) + z1;
        middleArm.transform.localPosition = middle_pos;

        Vector3 small_pos = smallArm.transform.localPosition;
        Vector3 small_rot = smallArm.transform.localEulerAngles;
        theta3 = small_rot.z;
        small_pos.y = length_middlearm * Mathf.Sin(Mathf.Deg2Rad * theta2) + middle_pos.y;
        small_pos.z = length_middlearm * Mathf.Cos(Mathf.Deg2Rad * theta2) + middle_pos.z;
        smallArm.transform.localPosition = small_pos;
        big_rot.z = (float)status.bigarm_pos;
        middle_rot.z = 180f-(float)status.middlearm_pos;
        small_rot.z = (float)status.smallarm_pos;
        swingarm1_rot.z=-(float)status.swingarm1_pos-30;
        swingarm2_rot.z=-(float)status.swingarm2_pos-30;
        swingarm3_rot.z=-(float)status.swingarm3_pos-30;
        swingarm4_rot.z=-(float)status.swingarm4_pos-30;

        bigArm.transform.localEulerAngles = big_rot;
        middleArm.transform.localEulerAngles = middle_rot;
        smallArm.transform.localEulerAngles = small_rot;
        swingarm1.transform.localEulerAngles=swingarm1_rot;
        swingarm2.transform.localEulerAngles=swingarm2_rot;
        swingarm3.transform.localEulerAngles=swingarm3_rot;
        swingarm4.transform.localEulerAngles=swingarm4_rot;



	}

        //将Byte转换为结构体类型
        /// 由byte数组转换为结构体
/// </summary>
        public static T ByteToStructure<T>(byte[] dataBuffer)
        {
        object structure = null;
        int size = Marshal.SizeOf(typeof(T));
        IntPtr allocIntPtr = Marshal.AllocHGlobal(size);
        try
        {
        Marshal.Copy(dataBuffer, 0, allocIntPtr, size);
        structure = Marshal.PtrToStructure(allocIntPtr, typeof(T));
        }
        finally
        {
        Marshal.FreeHGlobal(allocIntPtr);
        }
        return (T)structure;
}
 #region 接受信息
    void udpRecMessage()
    {
        while (true)
        {
            if (udpSever.Available > 0)
            {
                byte[] receiveBytes = udpSever.Receive(ref remoteIpEndPoint);
                status=ByteToStructure<Status>(receiveBytes);
            }
        }
    }
    #endregion
   

        
}

