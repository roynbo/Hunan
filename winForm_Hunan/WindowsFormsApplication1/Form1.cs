using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TwinCAT.Ads;
using System.Threading;
using adsClientVisu;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Thread thread = null;
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

         //连接ADS


        private void Cycle()
        {
            int indexStatus = 0;
            while (true) { 
            //发送要读取的轴的索引
            try
            {
                AdsBinaryWriter binWriter = new AdsBinaryWriter(adsWriteStream);
                adsWriteStream.Position = 0;
                binWriter.Write(indexStatus);
                _tcClient.ReadWrite(0x2, 0x9, adsReadStream, adsWriteStream);
                byte[] dataBuffer = adsReadStream.ToArray();
                lbOutput.Items.Add("要读取的轴的索引为： " + BitConverter.ToUInt32(dataBuffer, 0));
                lbOutput.SelectedIndex = lbOutput.Items.Count - 1;
            }
            catch
            {
                MessageBox.Show("读写轴索引发生错误！");
            }
            //读取对应索引的轴的使能情况
            try
            {
                _tcClient.ReadWrite(0x1, 0x1, adsReadStream, adsWriteStream);
                byte[] dataBuffer3 = adsReadStream.ToArray();
                lbOutput.Items.Add("当前轴的使能情况为： " + BitConverter.ToInt32(dataBuffer3, 0));
                lbOutput.SelectedIndex = lbOutput.Items.Count - 1;
                GlobalVar.R_Enable = BitConverter.ToUInt32(dataBuffer3, 0);
                //DrivingWheel1Enable.Text = GlobalVar.R_Enable.ToString();
            }
            catch
            {
                MessageBox.Show("读取轴使能出现错误！");
            }
            //读取对应索引的轴的速度大小
            try
            {
                _tcClient.ReadWrite(0x1, 0x2, adsReadStream, adsWriteStream);
                byte[] dataBuffer4 = adsReadStream.ToArray();
                lbOutput.Items.Add("当前轴的速度为： " + BitConverter.ToDouble(dataBuffer4, 0));
                lbOutput.SelectedIndex = lbOutput.Items.Count - 1;
                GlobalVar.R_ActVelo = BitConverter.ToDouble(dataBuffer4, 0);
                GlobalVar.R_ActVelo = ((int)(GlobalVar.R_ActVelo * 100)) / 100.0;
                //DrivingWheel1ActVel.Text = GlobalVar.R_ActVelo.ToString();
            }
            catch
            {
                MessageBox.Show("读取当前轴速度出现错误！");
            }
            //读取对应索引轴的位置
            try
            {
                _tcClient.ReadWrite(0x1, 0x3, adsReadStream, adsWriteStream);
                byte[] dataBuffer5 = adsReadStream.ToArray();
                lbOutput.Items.Add("当前轴的位置为： " + BitConverter.ToDouble(dataBuffer5, 0));
                lbOutput.SelectedIndex = lbOutput.Items.Count - 1;
                GlobalVar.R_ActPos = BitConverter.ToDouble(dataBuffer5, 0);
                GlobalVar.R_ActPos = ((int)(GlobalVar.R_ActPos * 100)) / 100.0;
                //DrivingWheel1ActPos.Text = GlobalVar.R_ActPos.ToString();
            }
            catch
            {
                MessageBox.Show("读取当前轴位置出现错误！");
            }
            
           
            //读取对应索引轴的电流
            try
            {
                _tcClient.ReadWrite(0x1, 0x4, adsReadStream, adsWriteStream);
                byte[] dataBuffer9 = adsReadStream.ToArray();
                lbOutput.Items.Add("当前轴的电流为： " + BitConverter.ToInt16(dataBuffer9, 0));
                lbOutput.SelectedIndex = lbOutput.Items.Count - 1;
                GlobalVar.R_ActCur = BitConverter.ToInt16(dataBuffer9, 0);
                //DrivingWheel1ActPos.Text = GlobalVar.R_ActPos.ToString();
            }
            catch
            {
                MessageBox.Show("读取当前轴位置出现错误！");
            }
            //读取对应索引轴的位置
            try
            {
                _tcClient.ReadWrite(0x1, 0x5, adsReadStream, adsWriteStream);
                byte[] dataBuffer6 = adsReadStream.ToArray();
                lbOutput.Items.Add("当前轴的位置为： " + BitConverter.ToDouble(dataBuffer6, 0));
                lbOutput.SelectedIndex = lbOutput.Items.Count - 1;
                GlobalVar.R_ActPosX = BitConverter.ToDouble(dataBuffer6, 0);
                GlobalVar.R_ActPosX = ((int)(GlobalVar.R_ActPosX * 100)) / 100.0;
                tx_handx.Text = GlobalVar.R_ActPosX.ToString();
            }
            catch
            {
                MessageBox.Show("读取当前轴位置出现错误！");
            }
            //读取对应索引轴的位置
            try
            {
                _tcClient.ReadWrite(0x1, 0x6, adsReadStream, adsWriteStream);
                byte[] dataBuffer7 = adsReadStream.ToArray();
                lbOutput.Items.Add("当前轴的位置为： " + BitConverter.ToDouble(dataBuffer7, 0));
                lbOutput.SelectedIndex = lbOutput.Items.Count - 1;
                GlobalVar.R_ActPosY = BitConverter.ToDouble(dataBuffer7, 0);
                GlobalVar.R_ActPosY = ((int)(GlobalVar.R_ActPosY * 100)) / 100.0;
                tx_handy.Text = GlobalVar.R_ActPosY.ToString();
            }
            catch
            {
                MessageBox.Show("读取当前轴位置出现错误！");
            }
            try
            {
                _tcClient.ReadWrite(0x1, 0x7, adsReadStream, adsWriteStream);
                byte[] dataBuffer8 = adsReadStream.ToArray();
                lbOutput.Items.Add("当前轴的位置为： " + BitConverter.ToDouble(dataBuffer8, 0));
                lbOutput.SelectedIndex = lbOutput.Items.Count - 1;
                GlobalVar.R_ActPosX = BitConverter.ToDouble(dataBuffer8, 0);
                GlobalVar.R_ActPosX = ((int)(GlobalVar.R_ActPosX * 100)) / 100.0;
                tx_bigarm.Text = GlobalVar.R_ActPosX.ToString();
            }
            catch
            {
                MessageBox.Show("读取当前轴位置出现错误！");
            }
            try
            {
                _tcClient.ReadWrite(0x1, 0x8, adsReadStream, adsWriteStream);
                byte[] dataBuffer9 = adsReadStream.ToArray();
                lbOutput.Items.Add("当前轴的位置为： " + BitConverter.ToDouble(dataBuffer9, 0));
                lbOutput.SelectedIndex = lbOutput.Items.Count - 1;
                GlobalVar.R_ActPosX = BitConverter.ToDouble(dataBuffer9, 0);
                GlobalVar.R_ActPosX = ((int)(GlobalVar.R_ActPosX * 100)) / 100.0;
                tx_middlearm.Text = GlobalVar.R_ActPosX.ToString();
            }
            catch
            {
                MessageBox.Show("读取当前轴位置出现错误！");
            }
            try
            {
                _tcClient.ReadWrite(0x1, 0x9, adsReadStream, adsWriteStream);
                byte[] dataBuffer10 = adsReadStream.ToArray();
                lbOutput.Items.Add("当前轴的位置为： " + BitConverter.ToDouble(dataBuffer10, 0));
                lbOutput.SelectedIndex = lbOutput.Items.Count - 1;
                GlobalVar.R_ActPosX = BitConverter.ToDouble(dataBuffer10, 0);
                GlobalVar.R_ActPosX = ((int)(GlobalVar.R_ActPosX * 100)) / 100.0;
                tx_smallarm.Text = GlobalVar.R_ActPosX.ToString();
            }
            catch
            {
                MessageBox.Show("读取当前轴位置出现错误！");
            }
            try
            {
                switch (indexStatus)
                {
                    case 0:
                        if (GlobalVar.R_Enable != 0)            //已经使能
                        {
                            DrivingWheel1Enable.Text = "Enable";
                            DrivingWheel1Enable.BackColor = Color.Chartreuse;
                        }
                        else
                        {
                            DrivingWheel1Enable.Text = "Disable";
                            DrivingWheel1Enable.BackColor = Color.Red;
                        }
                        DrivingWheel1ActVel.Text = GlobalVar.R_ActVelo.ToString();
                        DrivingWheel1ActPos.Text = GlobalVar.R_ActPos.ToString();
                        DrivingWheel1ActCur.Text = GlobalVar.R_ActCur.ToString();
                        break;
                    case 1:
                        if (GlobalVar.R_Enable != 0)            //已经使能
                        {
                            DrivingWheel2Enable.Text = "Enable";
                            DrivingWheel2Enable.BackColor = Color.Chartreuse;
                        }
                        else
                        {
                            DrivingWheel2Enable.Text = "Disable";
                            DrivingWheel2Enable.BackColor = Color.Red;
                        }
                        DrivingWheel2ActVel.Text = GlobalVar.R_ActVelo.ToString();
                        DrivingWheel2ActPos.Text = GlobalVar.R_ActPos.ToString();
                        DrivingWheel2ActCur.Text = GlobalVar.R_ActCur.ToString();
                        break;
                    case 2:
                        if (GlobalVar.R_Enable != 0)            //已经使能
                        {
                            DrivingWheel3Enable.Text = "Enable";
                            DrivingWheel3Enable.BackColor = Color.Chartreuse;
                        }
                        else
                        {
                            DrivingWheel3Enable.Text = "Disable";
                            DrivingWheel3Enable.BackColor = Color.Red;
                        }
                        DrivingWheel3ActVel.Text = GlobalVar.R_ActVelo.ToString();
                        DrivingWheel3ActPos.Text = GlobalVar.R_ActPos.ToString();
                        DrivingWheel3ActCur.Text = GlobalVar.R_ActCur.ToString();
                        break;
                    case 3:
                        if (GlobalVar.R_Enable != 0)            //已经使能
                        {
                            DrivingWheel4Enable.Text = "Enable";
                            DrivingWheel4Enable.BackColor = Color.Chartreuse;
                        }
                        else
                        {
                            DrivingWheel4Enable.Text = "Disable";
                            DrivingWheel4Enable.BackColor = Color.Red;
                        }
                        DrivingWheel4ActVel.Text = GlobalVar.R_ActVelo.ToString();
                        DrivingWheel4ActPos.Text = GlobalVar.R_ActPos.ToString();
                        DrivingWheel4ActCur.Text = GlobalVar.R_ActCur.ToString();
                        break;
                    case 4:
                        if (GlobalVar.R_Enable != 0)            //已经使能
                        {
                            SwingArm1Enable.Text = "Enable";
                            SwingArm1Enable.BackColor = Color.Chartreuse;
                        }
                        else
                        {
                            SwingArm1Enable.Text = "Disable";
                            SwingArm1Enable.BackColor = Color.Red;
                        }
                        SwingArm1ActVel.Text = GlobalVar.R_ActVelo.ToString();
                        SwingArm1ActPos.Text = GlobalVar.R_ActPos.ToString();
                        SwingArm1ActCur.Text = GlobalVar.R_ActCur.ToString();
                        break;
                    case 5:
                        if (GlobalVar.R_Enable != 0)            //已经使能
                        {
                            SwingArm2Enable.Text = "Enable";
                            SwingArm2Enable.BackColor = Color.Chartreuse;
                        }
                        else
                        {
                            SwingArm2Enable.Text = "Disable";
                            SwingArm2Enable.BackColor = Color.Red;
                        }
                        SwingArm2ActVel.Text = GlobalVar.R_ActVelo.ToString();
                        SwingArm2ActPos.Text = GlobalVar.R_ActPos.ToString();
                        SwingArm2ActCur.Text = GlobalVar.R_ActCur.ToString();
                        break;
                    case 6:
                        if (GlobalVar.R_Enable != 0)            //已经使能
                        {
                            SwingArm3Enable.Text = "Enable";
                            SwingArm3Enable.BackColor = Color.Chartreuse;
                        }
                        else
                        {
                            SwingArm3Enable.Text = "Disable";
                            SwingArm3Enable.BackColor = Color.Red;
                        }
                        SwingArm3ActVel.Text = GlobalVar.R_ActVelo.ToString();
                        SwingArm3ActPos.Text = GlobalVar.R_ActPos.ToString();
                        SwingArm3ActCur.Text = GlobalVar.R_ActCur.ToString();
                        break;
                    case 7:
                        if (GlobalVar.R_Enable != 0)            //已经使能
                        {
                            SwingArm4Enable.Text = "Enable";
                            SwingArm4Enable.BackColor = Color.Chartreuse;
                        }
                        else
                        {
                            SwingArm4Enable.Text = "Disable";
                            SwingArm4Enable.BackColor = Color.Red;
                        }
                        SwingArm4ActVel.Text = GlobalVar.R_ActVelo.ToString();
                        SwingArm4ActPos.Text = GlobalVar.R_ActPos.ToString();
                        SwingArm4ActCur.Text = GlobalVar.R_ActCur.ToString();
                        break;
                    case 9:
                        if (GlobalVar.R_Enable != 0)            //已经使能
                        {
                            BigArmEnable.Text = "Enable";
                            BigArmEnable.BackColor = Color.Chartreuse;
                        }
                        else
                        {
                            BigArmEnable.Text = "Disable";
                            BigArmEnable.BackColor = Color.Red;
                        }
                        BigArmActVel.Text = GlobalVar.R_ActVelo.ToString();
                        BigArmActPos.Text = GlobalVar.R_ActPos.ToString();
                        BigArmActCur.Text = GlobalVar.R_ActCur.ToString();
                        break;
                    case 10:
                       if (GlobalVar.R_Enable != 0)            //已经使能
                        {
                           FlexEnable.Text = "Enable";
                            FlexEnable.BackColor = Color.Chartreuse;
                        }
                       else
                        {
                            FlexEnable.Text = "Disable";
                            FlexEnable.BackColor = Color.Red;
                        }
                        FlexActVel.Text = GlobalVar.R_ActVelo.ToString();
                        FlexActPos.Text = GlobalVar.R_ActPos.ToString();
                        FlexActCur.Text = GlobalVar.R_ActCur.ToString();
                        break;
                    case 11:
                        if (GlobalVar.R_Enable != 0)            //已经使能
                        {
                            MiddleArmEnable.Text = "Enable";
                            MiddleArmEnable.BackColor = Color.Chartreuse;
                        }
                        else
                        {
                            MiddleArmEnable.Text = "Disable";
                            MiddleArmEnable.BackColor = Color.Red;
                        }
                        MiddleArmActVel.Text = GlobalVar.R_ActVelo.ToString();
                        MiddleArmActPos.Text = GlobalVar.R_ActPos.ToString();
                        MiddleArmActCur.Text = GlobalVar.R_ActCur.ToString();
                        break;
                    case 12:
                        if (GlobalVar.R_Enable != 0)            //已经使能
                       {
                            SmallArmEnable.Text = "Enable";
                            SmallArmEnable.BackColor = Color.Chartreuse;
                        }
                        else
                        {
                            SmallArmEnable.Text = "Disable";
                          SmallArmEnable.BackColor = Color.Red;
                        }
                        SmallArmActVel.Text = GlobalVar.R_ActVelo.ToString();
                        SmallArmActPos.Text = GlobalVar.R_ActPos.ToString();
                        SmallArmActCur.Text = GlobalVar.R_ActCur.ToString();
                        break;
                    case 13:
                        if (GlobalVar.R_Enable != 0)            //已经使能
                        {
                            RotationEnable.Text = "Enable";
                          RotationEnable.BackColor = Color.Chartreuse;
                       }
                      else
                        {
                            RotationEnable.Text = "Disable";
                            RotationEnable.BackColor = Color.Red;
                        }
                        RotationActVel.Text = GlobalVar.R_ActVelo.ToString();
                        RotationActPos.Text = GlobalVar.R_ActPos.ToString();
                        RotationActCur.Text = GlobalVar.R_ActCur.ToString();
                        break;
                    case 14:
                        if (GlobalVar.R_Enable != 0)            //已经使能
                       {
                            ClampEnable.Text = "Enable";
                            ClampEnable.BackColor = Color.Chartreuse;
                        }
                       else
                        {
                            ClampEnable.Text = "Disable";
                            ClampEnable.BackColor = Color.Red;
                        }
                        ClampActVel.Text = GlobalVar.R_ActVelo.ToString();
                        ClampActPos.Text = GlobalVar.R_ActPos.ToString();
                        ClampActCur.Text = GlobalVar.R_ActCur.ToString();
                        break;
                    case 8:
                        if (GlobalVar.R_Enable != 0)            //已经使能
                        {
                           WaistEnable.Text = "Enable";
                            WaistEnable.BackColor = Color.Chartreuse;
                        }
                        else
                       {
                            WaistEnable.Text = "Disable";
                            WaistEnable.BackColor = Color.Red;
                        }
                        WaistActVel.Text = GlobalVar.R_ActVelo.ToString();
                        WaistActPos.Text = GlobalVar.R_ActPos.ToString();
                        WaistActCur.Text = GlobalVar.R_ActCur.ToString();
                        break;
                }
                indexStatus++;
                if (indexStatus == 15)
                {
                    indexStatus = 0;
                }
            }
            catch
            {

            }
            }
        }
        public static void Delay(int delayTime)
        {
            DateTime now = DateTime.Now;
            double s;
            do
            {
                TimeSpan spand = DateTime.Now - now;
                s = spand.TotalMilliseconds;
                Application.DoEvents();
            }
            while (s < delayTime * 10);
        }

        private void SendInstruct(int instruct)
        {
            try
            {
                AdsBinaryWriter binWriter = new AdsBinaryWriter(adsWriteStream);
                adsWriteStream.Position = 0;
                binWriter.Write(instruct);
                _tcClient.ReadWrite(0x02, 0x01, adsReadStream, adsWriteStream);
                byte[] dataBuffer5 = adsReadStream.ToArray();
            }
            catch
            {
                MessageBox.Show("指令发送错误！");
            }
        }
        private void SendAutoInstruct(int instruct)
        {
            try
            {
                AdsBinaryWriter binWriter = new AdsBinaryWriter(adsWriteStream);
                adsWriteStream.Position = 0;
                binWriter.Write(instruct);
                _tcClient.ReadWrite(0x02, 0x10, adsReadStream, adsWriteStream);
                byte[] dataBuffer5 = adsReadStream.ToArray();
            }
            catch
            {
                MessageBox.Show("指令发送错误！");
            }
        }
        private void SendIndexCtr(int indexCtr)                 //发送控制轴的索引
        {
            try
            {
                AdsBinaryWriter binWriter = new AdsBinaryWriter(adsWriteStream);
                adsWriteStream.Position = 0;
                binWriter.Write(indexCtr);
                _tcClient.ReadWrite(0x02, 0x02, adsReadStream, adsWriteStream);
                byte[] dataBuffer2 = adsReadStream.ToArray();
            }
            catch
            {
                MessageBox.Show("索引发送错误！");
            }
        }
        private void SendPos(double Pos)
        {
            try
            {
                AdsBinaryWriter binWriter = new AdsBinaryWriter(adsWriteStream);
                adsWriteStream.Position = 0;
                binWriter.Write(Pos);
                _tcClient.ReadWrite(0x02, 0x03, adsReadStream, adsWriteStream);
                byte[] dataBuffer3 = adsReadStream.ToArray();
            }
            catch
            {
                MessageBox.Show("位置指令发送错误！");
            }
        }
        private void SendVel(double Vel)
        {
            try
            {
                AdsBinaryWriter binWriter = new AdsBinaryWriter(adsWriteStream);
                adsWriteStream.Position = 0;
                binWriter.Write(Vel);
                _tcClient.ReadWrite(0x02, 0x04, adsReadStream, adsWriteStream);
                byte[] dataBuffer4 = adsReadStream.ToArray();
            }
            catch
            {
                MessageBox.Show("速度指令发送错误！");
            }
        }

        private void SendVelX(double VelX)
        {
            try
            {
                AdsBinaryWriter binWriter = new AdsBinaryWriter(adsWriteStream);
                adsWriteStream.Position = 0;
                binWriter.Write(VelX);
                _tcClient.ReadWrite(0x02, 0x07, adsReadStream, adsWriteStream);
                byte[] dataBuffer5 = adsReadStream.ToArray();
            }
            catch
            {
                MessageBox.Show("x速度指令发送错误！");
            }
        }
        private void SendVelY(double VelY)
        {
            try
            {
                AdsBinaryWriter binWriter = new AdsBinaryWriter(adsWriteStream);
                adsWriteStream.Position = 0;
                binWriter.Write(VelY);
                _tcClient.ReadWrite(0x02, 0x08, adsReadStream, adsWriteStream);
                byte[] dataBuffer6 = adsReadStream.ToArray();
            }
            catch
            {
                MessageBox.Show("Y速度指令发送错误！");
            }
        }
        private int GetSelectedIndex()
        {
            try
            {
                if (radioButton1.Checked == true)        //驱动轮1被选中
                {
                    return 0;
                }
                else if (radioButton2.Checked == true)   //驱动轮2被选中
                {
                    return 1;
                }
                else if (radioButton3.Checked == true)     //驱动轮3被选中
                {
                    return 2;
                }
                else if (radioButton4.Checked == true) //驱动轮4被选中
                {
                    return 3;
                }
                else if (radioButton5.Checked == true)     //摆臂1被选中
                {
                    return 4;
                }
                else if (radioButton6.Checked == true)         //摆臂2被选中
                {
                    return 5;
                }
                else if (radioButton7.Checked == true)         //摆臂3被选中
                {
                    return 6;
                }
                else if (radioButton8.Checked == true)          //摆臂4被选中
                {
                    return 7;
                }
                else if (radioButton9.Checked == true)          //腰被选中
                {
                    return 8;
                }
                else if (radioButton10.Checked == true)          //大臂被选中
                {
                    return 9;
                }

                else if (radioButton11.Checked == true)          //伸缩被选中
                {
                    return 10;
                }
                else if (radioButton12.Checked == true)          //中臂被选中
                {
                    return 11;
                }
                else if (radioButton13.Checked == true)          //小臂被选中
                {
                    return 12;
                }
                else if (radioButton14.Checked == true)          //旋转被选中
                {
                    return 13;
                }
                else if (radioButton15.Checked == true)          //夹紧被选中
                {
                    return 14;
                }
                else                                            //没有有效的轴被选中
                {
                    return -1;
                }
            }
            catch
            {
                MessageBox.Show("获取单轴控制轴的索引出错！");
                return -1;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (button1.Text == "车上使能")      //下发车轮上使能操作指令
            {
                button1.Text = "车下使能";
                SendInstruct(20);
                Delay(1);


            }
            else
            {
                button1.Text = "车上使能";       //下发车轮下使能操作指令
                SendInstruct(40);
                Delay(1);


            }
        }

      

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbNetID_TextChanged(object sender, EventArgs e)
        {

        }

        private void btRun_Click(object sender, EventArgs e)
        {
            btRun.Enabled = false;
            thread = new Thread(Cycle);
            thread.IsBackground = true;
            thread.Start();
        }

        private void btConnect_Click(object sender, EventArgs e)
        {
            btConnect.Enabled = false;
            AmsAddress serverAddress = null;             //服务器的ADS地址
            try
            {
                if (tbPort.Text.StartsWith("0x") || tbPort.Text.StartsWith("0X"))
                {
                    string temp = tbPort.Text.Substring(2);
                    serverAddress = new AmsAddress(tbNetID.Text, Int32.Parse(temp, System.Globalization.NumberStyles.HexNumber));
                }
                else
                {
                    serverAddress = new AmsAddress(tbNetID.Text, Int32.Parse(tbPort.Text));
                }
            }
            catch
            {
                MessageBox.Show("ADS Port 或 NetID 输入有误!");
                return;
            }
            try
            {
                _tcClient.Connect(serverAddress.NetId, serverAddress.Port);
                lbOutput.Items.Add("端口" + _tcClient.ClientAddress.Port + "打开成功！");
                lbOutput.SelectedIndex = lbOutput.Items.Count - 1;
            }
            catch
            {
                MessageBox.Show("连接服务器失败！");
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tbPort_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            _tcClient = new TcAdsClient();
            adsReadStream = new AdsStream(100);
            adsWriteStream = new AdsStream(100);
            tabPage1.Text = "通讯";
            tabPage2.Text = "控制";
            tabPage3.Text = "机械臂";
        }

        private void DrivingWheel2Enable_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            while(true)
            {
                SendInstruct(888);
                Delay(1);
            }
          
        }

        private void SwingArm1Enable_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                button1.Text = "车上使能";
                SendInstruct(60);
                Delay(1);


            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!S");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(70);
                Delay(1);


            }
            catch
            {
                MessageBox.Show("暂停命令出错!");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(70);
                Delay(1);


            }
            catch
            {
                MessageBox.Show("急停命令出错!");
            }
        }

        private void DrivingWheel1ActVel_TextChanged(object sender, EventArgs e)
        {

        }

        private void label35_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
           
        }

        private void button9_Click(object sender, EventArgs e)
        {
            double vel = Convert.ToDouble(TbVelocity.Text);
            SendVel(vel);
            Delay(1);
            Delay(1);
            int axisID = GetSelectedIndex();
            SendIndexCtr(axisID);
            if(axisID<4)
            {
                SendInstruct(110);
                Delay(1);

            }
            else if(axisID>=4&&axisID<8)
            {
                SendInstruct(120);
                Delay(1);

            }
            else
            {
                SendInstruct(150);
                Delay(1);
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
           
            {
                double velx = Convert.ToDouble(TbVelx.Text);
                double vely = Convert.ToDouble(TbVely.Text);
                SendVelX(velx);
                Delay(1);
                SendVelY(vely);
                Delay(1);
                SendInstruct(100);
                Delay(1);
            }


           
        }

        private void button11_Click(object sender, EventArgs e)
        {
            double vel = Convert.ToDouble(TbVelocity.Text);
            SendVel(vel);
            Delay(1);
            double pos = Convert.ToDouble(TbPosition.Text);
            SendPos(pos);
            Delay(1);
            int axisID = GetSelectedIndex();
            SendIndexCtr(axisID);
            if (axisID > 8)
            {
                SendInstruct(150);
                Delay(1);
            }
            else
            {
                SendInstruct(140);
                Delay(1);
            }



        }

        private void button12_Click(object sender, EventArgs e)
        {
           
                SendInstruct(888);
                Delay(1);
           



        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            double w = Convert.ToDouble(Tbw.Text);
            double angle = Convert.ToDouble(TbAngle.Text);
            SendVel(w);
            Delay(1);
            SendPos(angle);
            Delay(1);
            SendInstruct(270);
            Delay(1);


        }

        private void label52_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {
         
           
        }

        private void button15_Click(object sender, EventArgs e)
        {
           
            
        }

        private void button16_Click(object sender, EventArgs e)
        {
         
        }

        private void button17_Click(object sender, EventArgs e)
        {
            
            
        }

        private void button18_Click(object sender, EventArgs e)
        {
           
        }

        private void TbVelocity_TextChanged(object sender, EventArgs e)
        {

        }

        private void button14_Click_1(object sender, EventArgs e)
        {
            if (button14.Text == "上使能")      //下发车轮上使能操作指令
            {
                button14.Text = "下使能";
                SendInstruct(30);
                Delay(1);

            }
            else
            {
                button14.Text = "上使能";       //下发车轮下使能操作指令
                SendInstruct(50);
                Delay(1);


            }
        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(70);
                Delay(1);




            }
            catch
            {
                MessageBox.Show("急停命令出错!");
            }
        }

        private void button16_Click_1(object sender, EventArgs e)
        {

            try
            {
                button14.Text = "上使能";
                button14.Text = "上使能";
                SendInstruct(60);
                Delay(1);


            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!");
            }
        }

        private void button17_Click_1(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(300);
                Delay(1);
            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!");
            }
        }

        private void button18_Click_1(object sender, EventArgs e)
        {
            int axisID = GetSelectedIndex();
            SendIndexCtr(axisID);
            double vel = Convert.ToDouble(ArmVel.Text);
            SendVel(vel);

            try
            {

                SendInstruct(130);
                Delay(1);


            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!");
            }

        }

        private void ArmVel_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton12_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton13_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button19_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(14);
                Delay(1);
            }
            catch
            {
                MessageBox.Show("急停命令出错!");
            }

        }

        private void button22_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(13);
                Delay(1);
            }
            catch
            {
                MessageBox.Show("急停命令出错!");
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(15);
                Delay(1);
            }
            catch
            {
                MessageBox.Show("急停命令出错!");
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(16);
                Delay(1);
            }
            catch
            {
                MessageBox.Show("急停命令出错!");
            }
        }

        private void ArmMoveX_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendInstruct(999);
            Delay(1);
           
        }

        private void button23_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(19);
                Delay(1);
            }
            catch
            {
                MessageBox.Show("急停命令出错!");
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(70);
                Delay(1);




            }
            catch
            {
                MessageBox.Show("急停命令出错!");
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            int axisID = GetSelectedIndex();
            SendIndexCtr(axisID);
            try
            {
                SendInstruct(80);
                Delay(1);


            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!");
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(18);
                Delay(1);
            }
            catch
            {
                MessageBox.Show("急停命令出错!");
            }

        }

        private void button27_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(17);
                Delay(1);
            }
            catch
            {
                MessageBox.Show("急停命令出错!");
            }
        }

        private void tx_handx_TextChanged(object sender, EventArgs e)
        {

        }

        private void button28_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(330);
                Delay(1);


            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!");
            }
        }

        private void button29_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(320);
                Delay(1);


            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!");
            }
        }

        private void button30_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(340);
                Delay(1);


            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!");
            }
        }

        private void button31_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(350);
                Delay(1);


            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!");
            }
        }

        private void button32_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(360);
                Delay(1);


            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!");
            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(370);
                Delay(1);


            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!");
            }
        }

        private void button35_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(380);
                Delay(1);


            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!");
            }
        }

        private void button34_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(390);
                Delay(1);


            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!");
            }
        }

        private void button36_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(400);
                Delay(1);


            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!");
            }
        }

        private void button37_Click(object sender, EventArgs e)
        {
            try
            {
                SendInstruct(410);
                Delay(1);


            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!");
            }
        }
        private void AutoStep(int time)
        {
            try
            {
                SendAutoInstruct(1);
                Delay(time);
            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!");
            }
            try
            {
                SendAutoInstruct(0);
                Delay(8);
            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!");
            }
        }
        private void btnAutoOn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                AutoStep(1000);
                AutoStep(200);
                AutoStep(800);
                AutoStep(800);

            }
        }

        private void btnAutoOFF_Click(object sender, EventArgs e)
        {
            try
            {
                SendAutoInstruct(0);
                Delay(1);
            }
            catch
            {
                MessageBox.Show("所有轴复位命令出错!");
            }
        }
    }
}
