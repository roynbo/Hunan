/*
    * 初代手模式，现在已经放弃不用
    * 作者：郭宁波
 */
package com.example.bo_.runmars;

import android.content.Intent;
import android.graphics.Typeface;
import android.os.Handler;
import android.os.Message;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
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

public class HandmodeActivity extends AppCompatActivity {
    private RockerView rockerView2;
    int screenWidth;
    int screenHeight;
    private Button btn_start;                //车启动按钮
    private Button btn_halt;                //暂停按钮
    private Button btn_reset;                //复位按钮
    private Button btn_waistleft;
    private Button btn_waistright;
    private Button btn_back;
    private Button btn_catch;
    private Button btn_free;
    private Button btn_ready;
    Button btn_hand_return;
    TextView hand_velocity_x;                       //开关显示文本
    TextView hand_velocity_y;                        //指令文本框
    public static int instruct1=-1;
    public static int instruct2=-1;
    public static int index=-1;
    public  static double carVelX=-1;
    public static double carVelY=-1;
    public  static double handVelX=-1;
    public static double handVelY=-1;
    public static double singleVel=-1;
    Typeface typeFace;
    // 主线程Handler
    // 用于将从服务器获取的消息显示出来
    private Handler mMainHandler;
    //    // Socket变量
    private Socket socket;

    // 线程池
    // 为了方便展示,此处直接采用线程池进行线程管理,而没有一个个开线程
    private ExecutorService mThreadPool;

    /**
     * 接收服务器消息 变量
     */
    // 输入流对象
    InputStream is;

    // 输入流读取器对象
    InputStreamReader isr;
    BufferedReader br;

    // 接收服务器发送过来的消息
    String response;


    /**
     * 发送消息到服务器 变量
     */
    // 输出流对象
    OutputStream outputStream;
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
        setContentView(R.layout.activity_handmode);
        DisplayMetrics dm = getResources().getDisplayMetrics();
        screenWidth = dm.widthPixels;
        screenHeight = dm.heightPixels;
        rockerView2 = (RockerView) findViewById(R.id.rockerView2);
        rockerView2.setRockerChangeListener(new RockerView.RockerChangeListener() {
            @Override
            public void report(float x, float y) {
                setLayout(rockerView2, (int)x, (int)y);
            }
        });
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
        typeFace = Typeface.createFromAsset(getAssets(), "font/Eurostile-BoldExtendedTwo.otf");
        hand_velocity_x=(TextView)findViewById(R.id.hand_velocity_x);
        hand_velocity_y=(TextView)findViewById(R.id.hand_velocity_y);
        btn_start=(Button)findViewById(R.id.btn_hand_start);
        btn_start.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                creatCode(1,0,0,0,0,0,0,0);
            }
        });
        btn_halt=(Button)findViewById(R.id.btn_hand_halt);
        btn_halt.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                creatCode(2,0,0,0,0,0,0,0);
            }
        });
        btn_reset=(Button)findViewById(R.id.btn_hand_reset);
        btn_reset.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                creatCode(3,0,0,0,0,0,0,0);
            }
        });
        btn_catch=(Button)findViewById(R.id.btn_hand_close);
        btn_catch.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                creatCode(9,0,14,-5,0,0,0,0);
            }
        });
        btn_free=(Button)findViewById(R.id.btn_hand_open);
        btn_free.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                creatCode(9,0,14,5,0,0,0,0);
            }
        });
        //btn_home=(Button)findViewById(R.id.btn_);
        //btn_home.setOnClickListener(new View.OnClickListener() {
           // @Override
            //public void onClick(View v) {
           //     creatCode(6,0,0,0,0,0,0,0);
         //   }
      //  });
        //btn_ready=(Button)findViewById(R.id.bt_mm_ready);
        //btn_ready.setOnClickListener(new View.OnClickListener() {
        //    @Override
         //   public void onClick(View v) {
        //        creatCode(7,0,0,0,0,0,0,0);
        //    }
      //  });
        btn_waistleft=(Button)findViewById(R.id.btn_waist_left);
        btn_waistleft.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                creatCode(9,0,8,5,0,0,0,0);
            }
        });
        btn_waistright=(Button)findViewById(R.id.btn_waist_right);
        btn_waistright.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                creatCode(9,0,8,-5,0,0,0,0);
            }
        });
        btn_back=(Button)findViewById(R.id.btn_back);
        btn_back.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                creatCode(11,0,8,-5,0,0,0,0);
            }
        });
        btn_ready=(Button)findViewById(R.id.btn_ready);
        btn_ready.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                creatCode(12,0,8,-5,0,0,0,0);
            }
        });
        btn_hand_return=(Button)findViewById(R.id.btn_hand_return);
        btn_hand_return.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent1=new Intent(HandmodeActivity.this,MainActivity.class);
                startActivity(intent1);
                Toast.makeText(HandmodeActivity.this,"返回主菜单！",Toast.LENGTH_SHORT).show();
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
        BigDecimal b1   =   new   BigDecimal(dx/100*0.03);
        double   f1   =   b1.setScale(2,   BigDecimal.ROUND_HALF_UP).doubleValue();
        BigDecimal b2   =   new   BigDecimal(dy/100*0.03);
        double   f2   =   b2.setScale(2,   BigDecimal.ROUND_HALF_UP).doubleValue();
        if(f1==0&&f2==0)
            creatCode(2,0,0,0,0,0,0,0);
        else
            creatCode(2,5,0,0,0,0,f1*1,-f2*1);
        DecimalFormat df = new DecimalFormat("0.00");
        hand_velocity_x.setText(df.format(-f2*1000)+"mm/s");
        hand_velocity_y.setText(df.format(-f1*1000)+"mm/s");
        hand_velocity_x.setTypeface(typeFace);
        hand_velocity_y.setTypeface(typeFace);

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
