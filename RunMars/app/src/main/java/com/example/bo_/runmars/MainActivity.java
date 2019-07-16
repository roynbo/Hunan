/*
    * 主界面
    * 作者：郭宁波
    * 日期：2018/9/19
    * 只有模式选择，不连接机器人
 */
package com.example.bo_.runmars;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.Toast;

public class MainActivity extends AppCompatActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        requestWindowFeature(Window.FEATURE_NO_TITLE);//没有标题栏
        getWindow().setFlags(
                WindowManager.LayoutParams.FLAG_FULLSCREEN
                        | WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON,
                WindowManager.LayoutParams.FLAG_FULLSCREEN
                        | WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);// 设置全屏
        // 屏幕长亮
        setContentView(R.layout.activity_main);
        Button btn_carmode = (Button) findViewById(R.id.btn_carmode);
        Button btn_manipulator = (Button) findViewById(R.id.btn_newmode);
        btn_carmode.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent1 = new Intent(MainActivity.this, CarmodeActivity.class);
                startActivity(intent1);
                overridePendingTransition(R.anim.zoom_enter, R.anim.zoom_exit);
                Toast.makeText(MainActivity.this, "车模式进入！", Toast.LENGTH_SHORT).show();
                finish();
            }
        });
        btn_manipulator.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent1 = new Intent(MainActivity.this, NewMode.class);
                startActivity(intent1);
                overridePendingTransition(R.anim.zoom_enter, R.anim.zoom_exit);
                Toast.makeText(MainActivity.this, "机械臂启动！", Toast.LENGTH_SHORT).show();
                finish();
            }
        });
    }
}
