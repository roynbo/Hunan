/*
    * 现用手模式
    * 作者：郭宁波
    * 日期：2018/9/19
    * 支持末端前后左右上下运动，有引导抓取、放置功能。
 */
package com.example.bo_.runmars;

import android.content.Intent;
import android.graphics.Typeface;
import android.os.Debug;
import android.os.Handler;
import android.os.Message;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.airbnb.lottie.LottieAnimationView;
import com.airbnb.lottie.LottieComposition;

import java.io.DataOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.math.BigDecimal;
import java.net.Socket;
import java.text.DecimalFormat;
import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import static android.os.SystemClock.sleep;

public class NewMode extends AppCompatActivity {
    int screenWidth;
    int screenHeight;
    private Button btn_new_start;                //车启动按钮
    private Button btn_new_halt;                //暂停按钮
    private Button btn_new_reset;                //复位按钮
    private Button btn_new_return;              //返回主菜单
    private Button btn_step_start;              //引导开始
    private Button btn_catch_new;               //抓
    private Button  btn_open_new;               //松
    TextView hand_velocity_x;
    TextView hand_velocity_y;
    Typeface typeFace;
    public AlertDialog step4;                       //步骤4
    public static int instruct1=-1;
    public static int instruct2=-1;
    public static int index=-1;
    public  static double carVelX=-1;
    public static double carVelY=-1;
    public  static double handVelX=-1;
    public static double handVelY=-1;
    public static double singleVel=-1;
    //    // Socket变量
    private Socket socket;
    // 线程池
    // 为了方便展示,此处直接采用线程池进行线程管理,而没有一个个开线程
    private ExecutorService mThreadPool;
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
        setContentView(R.layout.activity_new_mode);
        typeFace = Typeface.createFromAsset(getAssets(), "font/Eurostile-BoldExtendedTwo.otf");
        DisplayMetrics dm = getResources().getDisplayMetrics();
        screenWidth = dm.widthPixels;
        screenHeight = dm.heightPixels;
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
                    while (true) {
                        if (!socket.isConnected()) {
                            socket = new Socket("192.168.1.200", 4999);
                            //Light.setImageResource(R.drawable.red_light);
                        } else {
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
                                        if (instruct1 == 3) {
                                            creatCode(2, 0, 0, 0, 0, 0, 0, 0);
                                            // 特别注意：数据的结尾加上换行符才可让服务器端的readline()停止阻塞
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
        btn_new_return = (Button) findViewById(R.id.btn_new_return);
        btn_new_return.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent1 = new Intent(NewMode.this, MainActivity.class);
                startActivity(intent1);
                overridePendingTransition(R.anim.zoom_enter, R.anim.zoom_exit);
                Toast.makeText(NewMode.this, "返回主菜单！", Toast.LENGTH_SHORT).show();
                try {
                    socket.close();
                } catch (IOException e) {
                    e.printStackTrace();
                }
                finish();
            }
        });
        btn_new_start = (Button) findViewById(R.id.btn_new_start);
        btn_new_start.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                creatCode(1, 0, 0, 0, 0, 0, 0, 0);
            }
        });
        btn_new_halt = (Button) findViewById(R.id.btn_new_halt);
        btn_new_halt.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                creatCode(2, 0, 0, 0, 0, 0, 0, 0);
            }
        });
        btn_new_reset = (Button) findViewById(R.id.btn_new_reset);
        btn_new_reset.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                creatCode(3, 0, 0, 0, 0, 0, 0, 0);
            }
        });
        btn_catch_new = (Button) findViewById(R.id.btn_catch_new);
        btn_open_new = (Button) findViewById(R.id.btn_open_new);
        btn_catch_new.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                //按下操作
                if (motionEvent.getAction() == MotionEvent.ACTION_DOWN) {
                    Toast.makeText(NewMode.this, "正在抓取", Toast.LENGTH_SHORT).show();
                    creatCode(9, 0, 14, 80, 0, 0, 0, 0);
                }
                //抬起操作
                if (motionEvent.getAction() == MotionEvent.ACTION_UP) {
                    Toast.makeText(NewMode.this, "停止抓取", Toast.LENGTH_SHORT).show();
                    creatCode(9, 0, 14, 0, 0, 0, 0, 0);
                }
                return false;
            }
        });
        btn_open_new.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                //按下操作
                if (motionEvent.getAction() == MotionEvent.ACTION_DOWN) {
                    Toast.makeText(NewMode.this, "正在松爪", Toast.LENGTH_SHORT).show();
                    creatCode(9, 0, 14, -80, 0, 0, 0, 0);
                }
                //抬起操作
                if (motionEvent.getAction() == MotionEvent.ACTION_UP) {
                    Toast.makeText(NewMode.this, "停止抓取", Toast.LENGTH_SHORT).show();
                    creatCode(9, 0, 14, 0, 0, 0, 0, 0);
                }
                return false;
            }
        });
        btn_step_start = (Button) findViewById(R.id.btn_step_start);
        btn_step_start.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                AlertDialog.Builder setDeBugDialog = new AlertDialog.Builder(NewMode.this);
                //获取界面
                View dialogView = LayoutInflater.from(NewMode.this).inflate(R.layout.activity_step1, null);
                //初始化控件
                Button btn_ready = (Button) dialogView.findViewById(R.id.btn_ready1);
                Button btn_catch = (Button) dialogView.findViewById(R.id.btn_catch1);
                Button btn_reset1 = (Button) dialogView.findViewById(R.id.btn_reset1);
                //将界面填充到AlertDiaLog容器
                setDeBugDialog.setView(dialogView);
                //取消点击外部消失弹窗
                setDeBugDialog.setCancelable(false);
                //创建AlertDiaLog
                setDeBugDialog.create();
                //AlertDiaLog显示
                final AlertDialog step1 = setDeBugDialog.show();
                btn_reset1.setOnClickListener(new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        step1.dismiss();
                        creatCode(2, 0, 0, 0, 0, 0, 0, 0);
                    }
                });
                btn_ready.setOnClickListener(new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        creatCode(13, 0, 0, 0, 0, 0, 0, 0);
                    }
                });
                btn_catch.setOnClickListener(new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        creatCode(14, 0, 0, 0, 0, 0, 0, 0);
                        step1.dismiss();
                        AlertDialog.Builder setDeBugDialog = new AlertDialog.Builder(NewMode.this);
                        //获取界面
                        View dialogView = LayoutInflater.from(NewMode.this).inflate(R.layout.activity_step6, null);
                        //初始化控件
                        Button btn_reset6 = (Button) dialogView.findViewById(R.id.btn_reset6);
                        Button btn_last_back = (Button) dialogView.findViewById(R.id.btn_last_back);
                        Button btn_last_down = (Button) dialogView.findViewById(R.id.btn_last_down);
                        Button btn_last_front = (Button) dialogView.findViewById(R.id.btn_last_front);
                        Button btn_last_left = (Button) dialogView.findViewById(R.id.btn_last_left);
                        Button btn_last_right = (Button) dialogView.findViewById(R.id.btn_last_right);
                        Button btn_last_up = (Button) dialogView.findViewById(R.id.btn_last_up);
                        Button btn_readyforcatch = (Button) dialogView.findViewById(R.id.btn_readyforcatch);
                        Button btn_backToStep6 = (Button) dialogView.findViewById(R.id.btn_backToStep6);
                        Button btn_catch = (Button) dialogView.findViewById(R.id.btn_catch);
                        Button btn_open = (Button) dialogView.findViewById(R.id.btn_open);
                        //将界面填充到AlertDiaLog容器
                        setDeBugDialog.setView(dialogView);
                        //取消点击外部消失弹窗
                        setDeBugDialog.setCancelable(false);
                        //创建AlertDiaLog
                        setDeBugDialog.create();
                        //AlertDiaLog显示
                        final AlertDialog step6 = setDeBugDialog.show();
                        btn_last_back.setOnTouchListener(new View.OnTouchListener() {
                            @Override
                            public boolean onTouch(View view, MotionEvent motionEvent) {
                                //按下操作
                                if (motionEvent.getAction() == MotionEvent.ACTION_DOWN) {
                                    Toast.makeText(NewMode.this, "机械臂正在后移", Toast.LENGTH_SHORT).show();
                                    creatCode(2, 5, 0, 0, 0, 0, -0.05, 0);
                                }
                                //抬起操作
                                if (motionEvent.getAction() == MotionEvent.ACTION_UP) {
                                    Toast.makeText(NewMode.this, "机械臂停止", Toast.LENGTH_SHORT).show();
                                    creatCode(2, 0, 0, 0, 0, 0, 0, 0);
                                }
                                return false;
                            }
                        });
                        btn_last_down.setOnTouchListener(new View.OnTouchListener() {
                            @Override
                            public boolean onTouch(View view, MotionEvent motionEvent) {
                                //按下操作
                                if (motionEvent.getAction() == MotionEvent.ACTION_DOWN) {
                                    Toast.makeText(NewMode.this, "机械臂正在下降", Toast.LENGTH_SHORT).show();
                                    creatCode(2, 5, 0, 0, 0, 0, 0, -0.05);
                                }
                                //抬起操作
                                if (motionEvent.getAction() == MotionEvent.ACTION_UP) {
                                    Toast.makeText(NewMode.this, "机械臂停止", Toast.LENGTH_SHORT).show();
                                    creatCode(2, 0, 0, 0, 0, 0, 0, 0);
                                }
                                return false;
                            }
                        });
                        btn_last_front.setOnTouchListener(new View.OnTouchListener() {
                            @Override
                            public boolean onTouch(View view, MotionEvent motionEvent) {
                                //按下操作
                                if (motionEvent.getAction() == MotionEvent.ACTION_DOWN) {
                                    Toast.makeText(NewMode.this, "机械臂正在前移", Toast.LENGTH_SHORT).show();
                                    creatCode(2, 5, 0, 0, 0, 0, 0.05, 0);
                                }
                                //抬起操作
                                if (motionEvent.getAction() == MotionEvent.ACTION_UP) {
                                    Toast.makeText(NewMode.this, "机械臂停止", Toast.LENGTH_SHORT).show();
                                    creatCode(2, 0, 0, 0, 0, 0, 0, 0);
                                }
                                return false;
                            }
                        });
                        btn_last_left.setOnTouchListener(new View.OnTouchListener() {
                            @Override
                            public boolean onTouch(View view, MotionEvent motionEvent) {
                                //按下操作
                                if (motionEvent.getAction() == MotionEvent.ACTION_DOWN) {
                                    Toast.makeText(NewMode.this, "机械臂正在左移", Toast.LENGTH_SHORT).show();
                                    creatCode(9, 0, 8, 2, 0, 0, 0, 0);
                                }
                                //抬起操作
                                if (motionEvent.getAction() == MotionEvent.ACTION_UP) {
                                    Toast.makeText(NewMode.this, "机械臂停止", Toast.LENGTH_SHORT).show();
                                    creatCode(2, 0, 0, 0, 0, 0, 0, 0);
                                }
                                return false;
                            }
                        });
                        btn_last_right.setOnTouchListener(new View.OnTouchListener() {
                            @Override
                            public boolean onTouch(View view, MotionEvent motionEvent) {
                                //按下操作
                                if (motionEvent.getAction() == MotionEvent.ACTION_DOWN) {
                                    Toast.makeText(NewMode.this, "机械臂正在右移", Toast.LENGTH_SHORT).show();
                                    creatCode(9, 0, 8, -2, 0, 0, 0, 0);
                                }
                                //抬起操作
                                if (motionEvent.getAction() == MotionEvent.ACTION_UP) {
                                    Toast.makeText(NewMode.this, "机械臂停止", Toast.LENGTH_SHORT).show();
                                    creatCode(2, 0, 0, 0, 0, 0, 0, 0);
                                }
                                return false;
                            }
                        });
                        btn_last_up.setOnTouchListener(new View.OnTouchListener() {
                            @Override
                            public boolean onTouch(View view, MotionEvent motionEvent) {
                                //按下操作
                                if (motionEvent.getAction() == MotionEvent.ACTION_DOWN) {
                                    Toast.makeText(NewMode.this, "机械臂正在上升", Toast.LENGTH_SHORT).show();
                                    creatCode(2, 5, 0, 0, 0, 0, 0, 0.05);
                                }
                                //抬起操作
                                if (motionEvent.getAction() == MotionEvent.ACTION_UP) {
                                    Toast.makeText(NewMode.this, "机械臂停止", Toast.LENGTH_SHORT).show();
                                    creatCode(2, 0, 0, 0, 0, 0, 0, 0);
                                }
                                return false;
                            }
                        });
                        btn_catch.setOnTouchListener(new View.OnTouchListener() {
                            @Override
                            public boolean onTouch(View view, MotionEvent motionEvent) {
                                //按下操作
                                if (motionEvent.getAction() == MotionEvent.ACTION_DOWN) {
                                    Toast.makeText(NewMode.this, "正在抓取", Toast.LENGTH_SHORT).show();
                                    creatCode(9, 0, 14, 80, 0, 0, 0, 0);
                                }
                                //抬起操作
                                if (motionEvent.getAction() == MotionEvent.ACTION_UP) {
                                    Toast.makeText(NewMode.this, "停止抓取", Toast.LENGTH_SHORT).show();
                                    creatCode(9, 0, 14, 0, 0, 0, 0, 0);
                                }
                                return false;
                            }
                        });
                        btn_open.setOnTouchListener(new View.OnTouchListener() {
                            @Override
                            public boolean onTouch(View view, MotionEvent motionEvent) {
                                //按下操作
                                if (motionEvent.getAction() == MotionEvent.ACTION_DOWN) {
                                    Toast.makeText(NewMode.this, "正在松爪", Toast.LENGTH_SHORT).show();
                                    creatCode(9, 0, 14, -80, 0, 0, 0, 0);
                                }
                                //抬起操作
                                if (motionEvent.getAction() == MotionEvent.ACTION_UP) {
                                    Toast.makeText(NewMode.this, "停止抓取", Toast.LENGTH_SHORT).show();
                                    creatCode(9, 0, 14, 0, 0, 0, 0, 0);
                                }
                                return false;
                            }
                        });
                        btn_reset6.setOnClickListener(new View.OnClickListener() {
                            @Override
                            public void onClick(View v) {
                                step6.dismiss();
                                creatCode(2, 0, 0, 0, 0, 0, 0, 0);
                            }
                        });
                        btn_backToStep6.setOnClickListener(new View.OnClickListener() {
                            @Override
                            public void onClick(View v) {
                                step6.dismiss();
                                step1.show();
                            }
                        });
                        btn_readyforcatch.setOnClickListener(new View.OnClickListener() {
                            @Override
                            public void onClick(View v) {
                                creatCode(15, 0, 0, 0, 0, 0, 0, 0);
                                step6.dismiss();
                                AlertDialog.Builder setDeBugDialog = new AlertDialog.Builder(NewMode.this);
                                //获取界面
                                View dialogView = LayoutInflater.from(NewMode.this).inflate(R.layout.activity_step3, null);
                                //初始化控件
                                Button btn_reset3 = (Button) dialogView.findViewById(R.id.btn_reset3);
                                Button btn_backToStep2 = (Button) dialogView.findViewById(R.id.btn_backToStep2);
                                Button btn_chosePosition = (Button) dialogView.findViewById(R.id.btn_chosePosition);
                                //将界面填充到AlertDiaLog容器
                                setDeBugDialog.setView(dialogView);
                                //取消点击外部消失弹窗
                                setDeBugDialog.setCancelable(false);
                                //创建AlertDiaLog
                                setDeBugDialog.create();
                                //AlertDiaLog显示
                                final AlertDialog step3 = setDeBugDialog.show();
                                btn_reset3.setOnClickListener(new View.OnClickListener() {
                                    @Override
                                    public void onClick(View v) {
                                        step3.dismiss();
                                        creatCode(2, 0, 0, 0, 0, 0, 0, 0);
                                    }
                                });
                                btn_backToStep2.setOnClickListener(new View.OnClickListener() {
                                    @Override
                                    public void onClick(View v) {
                                        step3.dismiss();
                                        step6.show();
                                    }
                                });
                                btn_chosePosition.setOnClickListener(new View.OnClickListener() {
                                    @Override
                                    public void onClick(View v) {

                                        step3.dismiss();
                                        AlertDialog.Builder setDeBugDialog = new AlertDialog.Builder(NewMode.this);
                                        //获取界面
                                        View dialogView = LayoutInflater.from(NewMode.this).inflate(R.layout.activity_step4, null);
                                        //初始化控件
                                        Button btn_reset4 = (Button) dialogView.findViewById(R.id.btn_reset4);
                                        Button btn_backToStep3 = (Button) dialogView.findViewById(R.id.btn_backToStep3);
                                        Button btn_box1 = (Button) dialogView.findViewById(R.id.btn_box1);
                                        Button btn_box2 = (Button) dialogView.findViewById(R.id.btn_box2);
                                        Button btn_box3 = (Button) dialogView.findViewById(R.id.btn_box3);
                                        Button btn_box4 = (Button) dialogView.findViewById(R.id.btn_box4);
                                        //将界面填充到AlertDiaLog容器
                                        setDeBugDialog.setView(dialogView);
                                        //取消点击外部消失弹窗
                                        setDeBugDialog.setCancelable(false);
                                        //创建AlertDiaLog
                                        setDeBugDialog.create();
                                        //AlertDiaLog显示
                                        step4 = setDeBugDialog.show();
                                        btn_reset4.setOnClickListener(new View.OnClickListener() {
                                            @Override
                                            public void onClick(View v) {
                                                creatCode(2, 0, 0, 0, 0, 0, 0, 0);
                                                step4.dismiss();
                                            }
                                        });
                                        btn_backToStep3.setOnClickListener(new View.OnClickListener() {
                                            @Override
                                            public void onClick(View v) {
                                                step4.dismiss();
                                                step3.show();
                                            }
                                        });
                                        btn_box1.setOnClickListener(new View.OnClickListener() {
                                            @Override
                                            public void onClick(View v) {
                                                step4.dismiss();
                                                LastStep();
                                                creatCode(16, 0, 0, 0, 0, 0, 0, 0);
                                            }
                                        });
                                        btn_box2.setOnClickListener(new View.OnClickListener() {
                                            @Override
                                            public void onClick(View v) {
                                                step4.dismiss();
                                                LastStep();
                                                creatCode(17, 0, 0, 0, 0, 0, 0, 0);
                                            }
                                        });
                                        btn_box3.setOnClickListener(new View.OnClickListener() {
                                            @Override
                                            public void onClick(View v) {
                                                step4.dismiss();
                                                LastStep();
                                                creatCode(18, 0, 0, 0, 0, 0, 0, 0);
                                            }
                                        });
                                        btn_box4.setOnClickListener(new View.OnClickListener() {
                                            @Override
                                            public void onClick(View v) {
                                                step4.dismiss();
                                                LastStep();
                                                creatCode(19, 0, 0, 0, 0, 0, 0, 0);
                                            }
                                        });
                                    }
                                });
                            }
                        });
                    }
                });

            }
        });
    }
    public void LastStep()
    {
        AlertDialog.Builder setDeBugDialog = new AlertDialog.Builder(NewMode.this);
        //获取界面
        View dialogView = LayoutInflater.from(NewMode.this).inflate(R.layout.activity_step5, null);
        //初始化控件
        Button btn_reset5 = (Button) dialogView.findViewById(R.id.btn_reset5);
        Button btn_backToStep4=(Button)dialogView.findViewById(R.id.btn_backToStep4);
        Button btn_allback=(Button)dialogView.findViewById(R.id.btn_allback);
        Button btn_finish=(Button)dialogView.findViewById(R.id.btn_finish);
        //将界面填充到AlertDiaLog容器
        setDeBugDialog.setView(dialogView);
        //取消点击外部消失弹窗
        setDeBugDialog.setCancelable(false);
        //创建AlertDiaLog
        setDeBugDialog.create();
        //AlertDiaLog显示
        final AlertDialog step5 = setDeBugDialog.show();
        btn_reset5.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                step5.dismiss();
                creatCode(2,0,0,0,0,0,0,0);
            }
        });
        btn_backToStep4.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                step5.dismiss();
                step4.show();
            }
        });
        btn_allback.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                step5.dismiss();
                creatCode(13,0,0,0,0,0,0,0);
            }
        });
        btn_finish.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View view, MotionEvent motionEvent) {
                //按下操作
                if(motionEvent.getAction()==MotionEvent.ACTION_DOWN){
                    Toast.makeText(NewMode.this, "正在松爪", Toast.LENGTH_SHORT).show();
                    creatCode(9,0,14,-50,0,0,0,0);
                }
                //抬起操作
                if(motionEvent.getAction()==MotionEvent.ACTION_UP){
                    Toast.makeText(NewMode.this, "停止抓取", Toast.LENGTH_SHORT).show();
                    creatCode(9,0,14,0,0,0,0,0);
                }
                return false;
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
