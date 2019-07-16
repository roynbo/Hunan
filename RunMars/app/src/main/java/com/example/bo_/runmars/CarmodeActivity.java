/*
 * 车模式
 * 作者：郭宁波
 * 日期：2018/9/19
 * 自动连接机器人IP：192.168.1.200，端口：4999
 * 不断发送信息，没有接收信息，没有断线重连
 * 车启动，暂停，复位，前双摆动，后双摆动
 * 车运动，X前进速度（m/s），Y旋转速度（°/s）
 * 中央图片是开灯按钮
 * 有挡位切换，1，2，3x0.15，5
 * 返回主界面，自动断连
 */


package com.example.bo_.runmars;

import android.content.Intent;
import android.graphics.Typeface;
import android.os.Bundle;
import android.os.Handler;
import android.support.v7.app.AppCompatActivity;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.math.BigDecimal;
import java.net.Socket;
import java.text.DecimalFormat;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import static android.os.SystemClock.sleep;

public class CarmodeActivity extends AppCompatActivity {
    private RockerView rockerView1;
    int screenWidth;
    int screenHeight;
    int level=1;
    int flag=0;
    Button btn_start;                           //车启动
    Button btn_halt;                            //暂停
    Button btn_reset;                           //复位
    Button btn_for_up;                          //前双摆上
    Button btn_for_down;                        //前双摆下
    Button btn_back_up;                         //后双摆上
    Button btn_back_down;                       //后双摆下
    Button mars;                                  //开灯
    Button btn_car_return;                      //返回主菜单
    //等级1-3
    Button btn_level1;
    Button btn_level2;
    Button btn_level3;

    TextView car_velocity_x;                       //X速度文本
    TextView car_velocity_y;                        //Y速度文本

    public static int instruct1=2;
    public static int instruct2=0;
    public static int index=0;
    public  static double carVelX=0;
    public static double carVelY=0;
    public  static double handVelX=0;
    public static double handVelY=0;
    public static double singleVel=0;

    Typeface typeFace;                          //字体
    // Socket变量
    private Socket socket;
    // 输入流对象
    InputStream is;
    // 输出流对象
    OutputStream outputStream;
    // 线程池
    // 为了方便展示,此处直接采用线程池进行线程管理,而没有一个个开线程
    private ExecutorService mThreadPool;

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
        setContentView(R.layout.carmode_activity);
        DisplayMetrics dm = getResources().getDisplayMetrics();
        screenWidth = dm.widthPixels;
        screenHeight = dm.heightPixels;
        rockerView1 = (RockerView) findViewById(R.id.rockerView1);


        rockerView1.setRockerChangeListener(new RockerView.RockerChangeListener() {
            @Override
            public void report(float x, float y) {
                setLayout(rockerView1, (int)x, (int)y);
            }
        });

    /**
     * 创建客户端 & 服务器的连接
     */
        // 利用线程池直接开启一个线程 & 执行该线程
        mThreadPool.execute(new Runnable() {
            @Override
            public void run() {
                try {
                    // 创建Socket对象 & 指定服务端的IP 及 端口号
                    socket = new Socket("192.168.1.200", 4999);
                    // 判断客户端和服务器是否连接成功
                    while(true)
                    {
                        if (!socket.isConnected())
                        {
                            socket = new Socket("192.168.1.200", 4999);
                            //Light.setImageResource(R.drawable.red_light);
                        }
                        else {
                            //Light.setImageResource(R.drawable.blue_light);
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
                                        if(instruct1==3) {
                                            creatCode(2,0,0,0,0,0,0,0);
                                        }
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
        typeFace = Typeface.createFromAsset(getAssets(), "font/Eurostile-BoldExtendedTwo.otf");
        car_velocity_x=(TextView)findViewById(R.id.car_velocity_x);
        car_velocity_y=(TextView)findViewById(R.id.car_velocity_y);
        btn_start=(Button)findViewById(R.id.btn_car_start) ;
        btn_start.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                creatCode(1,0,0,0,0,0,0,0);
                Toast.makeText(CarmodeActivity.this,"车启动！",Toast.LENGTH_SHORT).show();
                Log.d("ABCD", "onClick: ");
            }
        });
        btn_reset=(Button)findViewById(R.id.btn_car_reset);
        btn_reset.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                creatCode(3,0,0,0,0,0,0,0);
                Toast.makeText(CarmodeActivity.this,"复位！",Toast.LENGTH_SHORT).show();
            }
        });
        btn_halt=(Button)findViewById(R.id.btn_car_halt);
        btn_halt.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                creatCode(2,0,0,0,0,0,0,0);
            }
        });
        btn_for_up=(Button)findViewById(R.id.btn_forup);
        btn_for_up.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                //按下操作
                if(motionEvent.getAction()==MotionEvent.ACTION_DOWN){
                    Toast.makeText(CarmodeActivity.this, "前摆臂上升中", Toast.LENGTH_SHORT).show();
                    creatCode(8,0,15,-5,0,0,0,0);
                }
                //抬起操作
                if(motionEvent.getAction()==MotionEvent.ACTION_UP){
                    Toast.makeText(CarmodeActivity.this, "运动停止", Toast.LENGTH_SHORT).show();
                    creatCode(2,0,0,0,0,0,0,0);
                }
                return false;
            }
        });
        btn_for_down=(Button)findViewById(R.id.btn_fordown);
        btn_for_down.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                //按下操作
                if(motionEvent.getAction()==MotionEvent.ACTION_DOWN){
                    Toast.makeText(CarmodeActivity.this, "前摆臂下降中", Toast.LENGTH_SHORT).show();
                    creatCode(8,0,15,5,0,0,0,0);
                }
                //抬起操作
                if(motionEvent.getAction()==MotionEvent.ACTION_UP){
                    Toast.makeText(CarmodeActivity.this, "运动停止", Toast.LENGTH_SHORT).show();
                    creatCode(2,0,0,0,0,0,0,0);
                }
                return false;
            }
        });
        btn_back_up=(Button)findViewById(R.id.btn_backup);
        btn_back_up.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                //按下操作
                if(motionEvent.getAction()==MotionEvent.ACTION_DOWN){
                    Toast.makeText(CarmodeActivity.this, "后摆臂上升中", Toast.LENGTH_SHORT).show();
                    creatCode(8,0,16,5,0,0,0,0);
                }
                //抬起操作
                if(motionEvent.getAction()==MotionEvent.ACTION_UP){
                    Toast.makeText(CarmodeActivity.this, "运动停止", Toast.LENGTH_SHORT).show();
                    creatCode(2,0,0,0,0,0,0,0);
                }
                return false;
            }
        });
        btn_back_down=(Button)findViewById(R.id.btn_backdown);
        btn_back_down.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                //按下操作
                if(motionEvent.getAction()==MotionEvent.ACTION_DOWN){
                    Toast.makeText(CarmodeActivity.this, "后摆臂下降中", Toast.LENGTH_SHORT).show();
                    creatCode(8,0,16,-5,0,0,0,0);
                }
                //抬起操作
                if(motionEvent.getAction()==MotionEvent.ACTION_UP){
                    Toast.makeText(CarmodeActivity.this, "运动停止", Toast.LENGTH_SHORT).show();
                    creatCode(2,0,0,0,0,0,0,0);
                }
                return false;
            }
        });
        mars=(Button)findViewById(R.id.imageView5);
        mars.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                //按下操作
                if(motionEvent.getAction()==MotionEvent.ACTION_DOWN){
                    if(flag==0) {
                        creatCode(20, 0, 0, 0, 0, 0, 0, 0);
                    }
                    else
                        creatCode(21, 0, 0, 0, 0, 0, 0, 0);

                }
                //抬起操作
                if(motionEvent.getAction()==MotionEvent.ACTION_UP){
                   if(flag==0)
                       flag=1;
                   else
                       flag=0;
                    Log.d("ABCD", "flag"+flag);
                }

                return false;
            }
        });
        btn_level1=(Button)findViewById(R.id.btn_level1);
        btn_level1.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                level=1;

            }
        });
        btn_level2=(Button)findViewById(R.id.btn_level2);
        btn_level2.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                level=2;
            }
        });
        btn_level3=(Button)findViewById(R.id.btn_level3);
        btn_level3.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                level=3;
            }
        });
        btn_car_return=(Button)findViewById(R.id.btn_car_return);
        btn_car_return.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent1=new Intent(CarmodeActivity.this,MainActivity.class);
                startActivity(intent1);
                overridePendingTransition(R.anim.zoom_enter, R.anim.zoom_exit);
                Toast.makeText(CarmodeActivity.this,"返回主菜单！",Toast.LENGTH_SHORT).show();
                try {
                    socket.close();
                } catch (IOException e) {
                    e.printStackTrace();
                }
                finish();
            }
        });
    }
    //方向盘函数
    public void setLayout(View v, double dx, double dy) {
        double left = v.getLeft() + dx;
        double top = v.getTop() + dy;
        double right = v.getRight() + dx;
        double bottom = v.getBottom() + dy;
        if (left < 0) {
            left = 0;
            right = left + v.getWidth();
        }
        if (right > screenWidth) {
            right = screenWidth;
            left = right - v.getWidth();
        }
        if (top < 0) {
            top = 0;
            bottom = top + v.getHeight();
        }
        if (bottom > screenHeight) {
            bottom = screenHeight;
            top = bottom - v.getHeight();
        }
        //  v.layout(left, top, right, bottom);
        //TextView tx;

        //默认X最大0.15，Y最大5，但是发现手机不同需要修改不同的系数
        BigDecimal b1   =   new   BigDecimal(dx/224*5);
        double   f1   =   b1.setScale(2,   BigDecimal.ROUND_HALF_UP).doubleValue();
        BigDecimal b2   =   new   BigDecimal(dy/224*0.15);
        double   f2   =   b2.setScale(2,   BigDecimal.ROUND_HALF_UP).doubleValue();
        DecimalFormat df = new DecimalFormat("0.00");
        if(level==1)
        {creatCode(4,0,0,0,-f2*0.4,-f1*0.4,0,0);
            car_velocity_x.setText(df.format(-f2*0.3)+"m/s");
            car_velocity_y.setText(df.format(-f1*0.3)+"°/s");
            car_velocity_x.setTypeface(typeFace);
            car_velocity_y.setTypeface(typeFace);
        }
        else if(level==2)
        {creatCode(4,0,0,0,-f2*2,-f1*2,0,0);
            car_velocity_x.setText(df.format(-f2*2)+"m/s");
            car_velocity_y.setText(df.format(-f1*2)+"°/s");
            car_velocity_x.setTypeface(typeFace);
            car_velocity_y.setTypeface(typeFace);}
        else if(level==3)
        {creatCode(4,0,0,0,-f2*3,-f1*3,0,0);
            car_velocity_x.setText(df.format(-f2*3)+"m/s");
            car_velocity_y.setText(df.format(-f1*3)+"°/s");
            car_velocity_x.setTypeface(typeFace);
            car_velocity_y.setTypeface(typeFace);}

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
    //double转字节数组
    public static byte[] double2Bytes(double d) {
        long value = Double.doubleToRawLongBits(d);
        byte[] byteRet = new byte[8];
        for (int i = 0; i < 8; i++) {
            byteRet[i] = (byte) ((value >> 8 * i) & 0xff);
        }
        return byteRet;
    }
    //int转字节数组
    public static byte[] intToByteArray(int a) {
        return new byte[] {
                (byte) ((a ) & 0xFF),
                (byte) ((a >> 8) & 0xFF),
                (byte) ((a >> 16) & 0xFF),
                (byte) ((a >> 24) & 0xFF),
        };
    }
    //字节数组合并（专用）
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
    //屏蔽物理返回键
    @Override
    public void onBackPressed() {
        //super.onBackPressed();
        Log.d("backreally","back");
    }
}
