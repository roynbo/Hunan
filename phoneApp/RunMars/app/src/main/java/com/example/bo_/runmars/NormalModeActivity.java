package com.example.bo_.runmars;

import android.content.Intent;
import android.os.Handler;
import android.os.Message;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import static android.os.SystemClock.sleep;

public class NormalModeActivity extends AppCompatActivity {
    public static int instruct1=-1;
    public static int instruct2=-1;
    public static int index=-1;
    public  static double carVelX=-1;
    public static double carVelY=-1;
    public  static double handVelX=-1;
    public static double handVelY=-1;
    public static double singleVel=-1;
    Button btn_swingarm1;
    Button btn_swingarm2;
    Button btn_swingarm3;
    Button btn_swingarm4;
    Button btn_arm_up;
    Button btn_arm_down;
    Button btn_arm_start;
    Button btn_arm_reset;
    TextView index_chose;
    // Socket变量
    private Socket socket;
    // 输入流对象
    InputStream is;
    // 输出流对象
    OutputStream outputStream;
    // 线程池
    // 为了方便展示,此处直接采用线程池进行线程管理,而没有一个个开线程
    private ExecutorService mThreadPool;
    // 主线程Handler
    // 用于将从服务器获取的消息显示出来
    private Handler mMainHandler;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        mThreadPool = Executors.newCachedThreadPool();
        requestWindowFeature(Window.FEATURE_NO_TITLE);//没有标题栏
        getWindow().setFlags(
                WindowManager.LayoutParams.FLAG_FULLSCREEN
                        | WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON,
                WindowManager.LayoutParams.FLAG_FULLSCREEN
                        | WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);// 设置全屏
        // 屏幕长亮
        setContentView(R.layout.activity_normal_mode);
        // 实例化主线程,用于更新接收过来的消息
        mMainHandler = new Handler() {
            @Override
            public void handleMessage(Message msg) {
                switch (msg.what) {
                    case 0:
                        //tx2.setText(dw1_rv_temp);
                        break;
                }
            }
        };
        /**
         * 创建客户端 & 服务器的连接
         */
        // 利用线程池直接开启一个线程 & 执行该线程
        // bt_dw1_debug.setOnClickListener(new View.OnClickListener()

        //@Override
        //  public void onClick(View v) {
        mThreadPool.execute(new Runnable() {
            @Override
            public void run() {

                try {

                    // 创建Socket对象 & 指定服务端的IP 及 端口号
                    socket = new Socket("192.168.1.200", 4999);
                    while(true)
                    {
                        if (!socket.isConnected())
                        {
                            socket = new Socket("192.168.1.200", 4999);
                            //Light.setImageResource(R.drawable.red_light);
                        }
                        else {
                            // Light.setImageResource(R.drawable.blue_light);
                            break;
                        }
                    }
                    // 利用线程池直接开启一个线程 & 执行该线程
                    mThreadPool.execute(new Runnable() {
                        @Override
                        public void run() {

                            try {
                                // 步骤1：从Socket 获得输出流对象OutputStream
                                // 该对象作用：发送数据
                                outputStream = socket.getOutputStream();
                                DataOutputStream pw = new DataOutputStream(outputStream);
                                while (true) {

                                    // 步骤2：写入需要发送的数据到输出流对象中
                                    {
                                        pw.write(byteMerger(double2Bytes(carVelX), double2Bytes(carVelY), double2Bytes(handVelX), double2Bytes(handVelY), double2Bytes(singleVel), intToByteArray(index), intToByteArray(instruct1), intToByteArray(instruct2)));
                                        if(instruct1==1||instruct1==3) {
                                            creatCode(2,0,0,0,0,0,0,0);
                                            // 特别注意：数据的结尾加上换行符才可让服务器端的readline()停止阻塞
                                        }
                                        if(instruct1==6||instruct1==7)
                                            instruct1=1;
                                        // 特别注意：数据的结尾加上换行符才可让服务器端的readline()停止阻塞
                                        // 步骤3：发送数据到服务端
                                        pw.flush();
                                        sleep(100);
                                    }


                                }
                            } catch (IOException e) {
                                e.printStackTrace();
                            }
                        }
                    });
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
        });
        index_chose=findViewById(R.id.textView);
        btn_swingarm1=findViewById(R.id.btn_swingarm1);
        btn_swingarm1.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                index=4;
                index_chose.setText("左前摆臂");
            }
        });
        btn_swingarm2=findViewById(R.id.btn_swingarm2);
        btn_swingarm2.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                index=5;
                index_chose.setText("右前摆臂");
            }
        });
        btn_swingarm3=findViewById(R.id.btn_swingarm3);
        btn_swingarm3.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                index=6;
                index_chose.setText("右后摆臂");
            }
        });
        btn_swingarm4=findViewById(R.id.btn_swingarm4);
        btn_swingarm4.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                index=7;
                index_chose.setText("左后摆臂");
            }
        });
        btn_arm_start=findViewById(R.id.btn_arm_start);
        btn_arm_start.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                creatCode(1,0,0,0,0,0,0,0);
                index_chose.setText("启动");
            }
        });
        btn_arm_reset=findViewById(R.id.btn_arm_reset);
        btn_arm_reset.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                creatCode(3,0,0,0,0,0,0,0);
                index_chose.setText("复位");
            }
        });
        btn_arm_up=findViewById(R.id.btn_arm_up);
        btn_arm_up.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                //按下操作
                if(motionEvent.getAction()==MotionEvent.ACTION_DOWN){
                    instruct1=22;
                    singleVel=-8;
                }
                //抬起操作
                if(motionEvent.getAction()==MotionEvent.ACTION_UP){
                    instruct1=2;
                }
                return false;
            }
        });
        btn_arm_down=findViewById(R.id.btn_arm_down);
        btn_arm_down.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                //按下操作
                if(motionEvent.getAction()==MotionEvent.ACTION_DOWN){
                    instruct1=22;
                    singleVel=8;
                }
                //抬起操作
                if(motionEvent.getAction()==MotionEvent.ACTION_UP){
                    instruct1=2;
                }
                return false;
            }
        });
        Button btn_arm_back=findViewById(R.id.btn_arm_back);
        btn_arm_back.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent1=new Intent(NormalModeActivity.this,MainActivity.class);
                startActivity(intent1);
                overridePendingTransition(R.anim.zoom_enter, R.anim.zoom_exit);
                Toast.makeText(NormalModeActivity.this,"返回主菜单！",Toast.LENGTH_SHORT).show();
                try {
                    socket.close();
                } catch (IOException e) {
                    e.printStackTrace();
                }
                finish();
            }
        });
    }
    public void creatCode(int temp_instruct1,int temp_instruct2,int temp_index,double temp_singleVel,double temp_carVelx,double temp_carVely,double temp_handVelx,double temp_handVely)
    {
        instruct1=temp_instruct1;
        instruct2=temp_instruct2;
        index=temp_index;
        singleVel=temp_singleVel;
        carVelX=temp_carVelx;
        carVelY=temp_carVely;
        handVelX=temp_handVelx;
        handVelY=temp_handVely;
    }
    public static byte[] double2Bytes(double d) {
        long value = Double.doubleToRawLongBits(d);
        byte[] byteRet = new byte[8];
        for (int i = 0; i < 8; i++) {
            byteRet[i] = (byte) ((value >> 8 * i) & 0xff);
        }
        return byteRet;
    }

    public static byte[] intToByteArray(int a) {
        return new byte[] {
                (byte) ((a ) & 0xFF),
                (byte) ((a >> 8) & 0xFF),
                (byte) ((a >> 16) & 0xFF),
                (byte) ((a >> 24) & 0xFF),


        };
    }
    //System.arraycopy()方法
    public static byte[] byteMerger(byte[] bt1, byte[] bt2,byte[] bt3,byte[] bt4,byte[] bt5,byte[] bt6,byte[] bt7,byte[] bt8){
        byte[] bt9 = new byte[bt1.length+bt2.length+bt3.length+bt4.length+bt5.length+bt6.length+bt7.length+bt8.length];
        System.arraycopy(bt1, 0, bt9, 0, bt1.length);
        System.arraycopy(bt2, 0, bt9, bt1.length, bt2.length);
        System.arraycopy(bt3, 0, bt9, bt2.length+bt1.length, bt3.length);
        System.arraycopy(bt4, 0, bt9, bt3.length+bt2.length+bt1.length, bt4.length);
        System.arraycopy(bt5, 0, bt9, bt4.length+bt3.length+bt2.length+bt1.length, bt5.length);
        System.arraycopy(bt6, 0, bt9, bt5.length+bt4.length+bt3.length+bt2.length+bt1.length, bt6.length);
        System.arraycopy(bt7, 0, bt9, bt6.length+bt5.length+bt4.length+bt3.length+bt2.length+bt1.length, bt7.length);
        System.arraycopy(bt8, 0, bt9, bt7.length+bt6.length+bt5.length+bt4.length+bt3.length+bt2.length+bt1.length, bt8.length);
        return bt9;
    }
    @Override
    public void onBackPressed() {
        //super.onBackPressed();
        Log.d("backreally","back");
    }
}
