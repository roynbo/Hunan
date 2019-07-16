using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

[StructLayout(LayoutKind.Sequential, Pack = 4)]             //强行4字节对齐

public struct command                   //发送的命令
{
    public double carVelX;
    public double carVelY;
    public double handVelX;
    public double handVelY;
    public double singleVel;
    public int index;
    public int instruct1;
    public int instruct2;
};
public struct status                    //轴状态，大臂中臂小臂，4个摆
{
    public double bigarm_position;
    public double middlearm_position;
    public double smallarm_position;
    public double swingarm1;
    public double swingarm2;
    public double swingarm3;
    public double swingarm4;
};
public struct handleData                //操纵柄数据，x方向，y方向，按钮
{
    public int dataX;
    public int dataY;
    public int button_num;
}
public class trans                      //全局类，存了和unity通信的结构体
{
    public static status handStatus;
}


namespace NewUI_test_2
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    ///    
    public partial class MainWindow : Window
    {
        handleData handleCarData;
        handleData handleHandData;
        command sendComd;
        command statusComd;     
        Socket Mysocket = null;
        byte[] arrMsgRec = new byte[1024 * 1024 * 2];                      
        private SerialPort ComDevice_ForCar = new SerialPort();        //开车串口
        private SerialPort ComDevice_ForHand = new SerialPort();        //开机械臂串口
        private List<byte> buffer_ForCar = new List<byte>(4096);       //串口数据缓存
        private List<byte> buffer_ForHand = new List<byte>(4096);       //串口数据缓存
        public int sign = 0;
        #region  插件
        public delegate void AppIdleEvent(Process app);          
        //取得函数的地址，接下来就可以用函数了
        [DllImport("user32.dll")]
        static extern Int32 StWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, Int32 X, Int32 Y, Int32 cx, Int32 cy, UInt32 uFlags);
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRePaint);
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        #endregion
        const int GWL_STYLE = -16;
        const int WS_VISIBLE = 0x10000000;
        int toggle_1 = 0;
        RobotInteraction robotInteraction = new RobotInteraction();     //实例化unity通信类
        public MainWindow()
        {
            InitializeComponent();
            //initCar();                     //串口读
            //initHand();
            Thread threadConnect = new Thread(client);              //开启TCP连接线程
            threadConnect.IsBackground = true;
            threadConnect.Start();
            robotInteraction.udp_Client();                          //开启UDP连接线程
            openU3D();                                              //打开U3D窗口
            Thread threadSendMSg = new Thread(robotInteraction.udp_SendMessage);//开启UDP发送线程
            threadSendMSg.IsBackground = true;
            threadSendMSg.Start();
        }
        #region 串口
        #region 打开串口 void init()
        private void initCar()
        {
            while (ComDevice_ForCar.IsOpen == false)
            {
                ComDevice_ForCar.PortName = "COM1";                   //端口名字
                ComDevice_ForCar.BaudRate = Convert.ToInt32("9600");   //波特率
                ComDevice_ForCar.Parity = (Parity)Convert.ToInt32("0");//校验位
                ComDevice_ForCar.DataBits = Convert.ToInt32("8");      //数据位
                ComDevice_ForCar.StopBits = (StopBits)Convert.ToInt32("1");//停止位
                try
                {
                    ComDevice_ForCar.Open();                           //串口开
                }
                catch (Exception ex)
                {
                }
            }
            ComDevice_ForCar.DataReceived += new SerialDataReceivedEventHandler(Com_DataReceived_ForCar);//绑定事件
        }
        private void initHand()
        {
            while (ComDevice_ForHand.IsOpen == false)
            {
                ComDevice_ForHand.PortName = "COM2";                   //端口名字
                ComDevice_ForHand.BaudRate = Convert.ToInt32("9600");   //波特率
                ComDevice_ForHand.Parity = (Parity)Convert.ToInt32("0");//校验位
                ComDevice_ForHand.DataBits = Convert.ToInt32("8");      //数据位
                ComDevice_ForHand.StopBits = (StopBits)Convert.ToInt32("1");//停止位
                try
                {
                    ComDevice_ForHand.Open();                           //串口开
                }
                catch (Exception ex)
                {
                }
            }
            ComDevice_ForHand.DataReceived += new SerialDataReceivedEventHandler(Com_DataReceived_ForHand);//绑定事件
        }
        #endregion
        #region 接收数据 void Com_DataReceived_ForCar(object sender, SerialDataReceivedEventArgs e)
        private void Com_DataReceived_ForCar(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] readBuffer = new byte[ComDevice_ForCar.BytesToRead];
            byte[] ReDatas = new byte[ComDevice_ForCar.BytesToRead];

            ComDevice_ForCar.Read(ReDatas, 0, ReDatas.Length);//读取数据
            buffer_ForCar.AddRange(ReDatas);
            //2.完整性判断
            while (buffer_ForCar.Count >= 9) //FF XX XX XX XX 08 00 XX XX，9位
            {
                //2.1 查找数据标记头           
                if (buffer_ForCar[0] == 255 && buffer_ForCar[5] == 8) //传输数据有帧头，用于判断，还有第5位肯定出现的08      
                {
                    int len = 8;
                    if (buffer_ForCar.Count < len + 1)
                    {
                        //数据未接收完整跳出循环
                        break;
                    }
                    readBuffer = new byte[len + 1];
                    //得到完整的数据，复制到readBuffer中   
                    buffer_ForCar.CopyTo(0, readBuffer, 0, len + 1);
                    //从缓冲区中清除
                    buffer_ForCar.RemoveRange(0, len + 1);
                    this.AnsysDataForCar(readBuffer);//输出数据
                }
                else //开始标记或版本号不正确时清除          
                {
                    buffer_ForCar.RemoveAt(0);
                }
            }
        }
        private void Com_DataReceived_ForHand(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] readBuffer = new byte[ComDevice_ForHand.BytesToRead];
            byte[] ReDatas = new byte[ComDevice_ForHand.BytesToRead];

            ComDevice_ForHand.Read(ReDatas, 0, ReDatas.Length);//读取数据
            buffer_ForHand.AddRange(ReDatas);
            //2.完整性判断
            while (buffer_ForHand.Count >= 9) //FF XX XX XX XX 08 00 XX XX，9位
            {
                //2.1 查找数据标记头           
                if (buffer_ForHand[0] == 255 && buffer_ForHand[5] == 8) //传输数据有帧头，用于判断，还有第5位肯定出现的08      
                {
                    int len = 8;
                    if (buffer_ForHand.Count < len + 1)
                    {
                        //数据未接收完整跳出循环
                        break;
                    }
                    readBuffer = new byte[len + 1];
                    //得到完整的数据，复制到readBuffer中   
                    buffer_ForHand.CopyTo(0, readBuffer, 0, len + 1);
                    //从缓冲区中清除
                    buffer_ForHand.RemoveRange(0, len + 1);
                    this.AnsysDataForHand(readBuffer);//输出数据
                }
                else //开始标记或版本号不正确时清除          
                {
                    buffer_ForHand.RemoveAt(0);
                }
            }
        }
        #endregion
        #region 串口数据分析 void AnsysData(byte[] data)
        public void AnsysDataForCar(byte[] data)
        {
            byte[] byteVelocityX = new byte[2];
            byte[] byteVelocityY = new byte[2];
            byte[] byteButton = new byte[1];
            byteVelocityX[0] = data[1];
            byteVelocityX[1] = data[2];
            byteVelocityY[0] = data[3];
            byteVelocityY[1] = data[4];
            byteButton[0] = data[7];
            handleCarData.dataX = ByteTo16(byteVelocityX);
            handleCarData.dataY = ByteTo16(byteVelocityY);
            handleCarData.button_num = ByteTo16(byteButton);
            switch (handleCarData.button_num)                                           //按钮判断
            {
                case 0://没按
                    {
                        if (handleCarData.dataX == 2048 && handleCarData.dataY == 2048)//摇柄没动，暂停
                        {
                            if (handleHandData.button_num == 0 && handleHandData.dataX == 2048 && handleHandData.dataY == 2048)
                                creatCode(2, 0, 0, 0, 0, 0, 0, 0);

                        }
                        else
                            creatCode(4, 0, 0, 0, ((double)handleCarData.dataY - 2048) / 1952 * 0.3, -((double)handleCarData.dataX - 2048) / 1952 * 10, 0, 0);            //一般发车模式
                    }
                    break;
                case 1://左上按钮
                    {
                        creatCode(8, 0, 15, -8, 0, 0, 0, 0);//前摆上
                    }
                    break;
                case 2://左下按钮
                    {
                        creatCode(8, 0, 15, 8, 0, 0, 0, 0);                          //前摆下
                    }
                    break;
                case 4://右上按钮
                    {
                        creatCode(8, 0, 16, -8, 0, 0, 0, 0);                        //后摆上
                    }
                    break;
                case 8://右下按钮
                    {
                        creatCode(8, 0, 16, 8, 0, 0, 0, 0);                          //后摆下
                    }
                    break;
                default:
                    break;
            }   
        }
        public void AnsysDataForHand(byte[] data)
        {
            byte[] byteVelocityX = new byte[2];
            byte[] byteVelocityY = new byte[2];
            byte[] byteButton = new byte[1];
            byteVelocityY[0] = data[1];
            byteVelocityY[1] = data[2];
            byteVelocityX[0] = data[3];
            byteVelocityX[1] = data[4];
            byteButton[0] = data[7];
            handleHandData.dataX = ByteTo16(byteVelocityX);
            handleHandData.dataY = ByteTo16(byteVelocityY);
            handleHandData.button_num = ByteTo16(byteButton);
            sendComd.handVelX = ((double)handleHandData.dataX - 2048) / 1952 * 0.04;         //车速度，0.18是一档（后面的按钮改成档位调节）
            sendComd.handVelY = -((double)handleHandData.dataY - 2048) / 1952 * 0.04;           //角速度，5是一档，同上
            switch (handleHandData.button_num)                                           //按钮判断
            {
                case 0://没按
                    {
                           if (Math.Abs(handleHandData.dataX - 2048) > Math.Abs(handleHandData.dataY - 2048))
                            {
                               if(handleHandData.dataX>2048)
                                    creatCode(2, 5, 0, 0, 0, 0, 0.04, 0);//一般发手模式
                               else
                                   creatCode(2, 5, 0, 0, 0, 0, -0.04, 0);//一般发手模式
                            }
                            else
                            {
                                creatCode(9, 0, 8, -((double)handleHandData.dataY - 2048) / 1952 * 4, 0, 0, 0, 0);//机械臂左右
                            }
                        
                    }
                    break;
                case 1://左上按钮
                    {
                        creatCode(2, 5, 0, 0, 0, 0, 0, 0.04);                     //机械臂上移

                    }
                    break;
                case 2://左下按钮
                    {
                        creatCode(2, 5, 0, 0, 0, 0, 0, -0.04);                          //机械臂下降


                    }
                    break;
                case 4://右上按钮
                    {
                        creatCode(9, 0, 14, 80, 0, 0, 0, 0);                       //合
                    }
                    break;
                case 8://右下按钮
                    {
                        creatCode(9, 0, 14, -80, 0, 0, 0, 0);                         //张
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion
        #region 数组转换成十六进制字符串 int ByteTo16(byte[] data)
        public int ByteTo16(byte[] data)
        {
            int dec = 0;
            if (data.Length == 2)//X,Y速度
            {
                dec = Convert.ToInt32(data[0].ToString()) * 16 * 16 + Convert.ToInt32(data[1].ToString());
            }
            else//按钮
            {
                dec = Convert.ToInt32(data[0].ToString());
            }
            return dec;
        }
        #endregion
        #endregion
        public void openU3D()
        {
            AppIdleEvent app = new AppIdleEvent(appIdleHandle);
            ProcessStartInfo startInfo = new ProcessStartInfo(@"unityAPP\game.exe");
            startInfo.UseShellExecute = true;
            Process p = Process.Start(startInfo);
            Thread.Sleep(500);
            p.WaitForInputIdle();
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, app, p);
        }
        public void creatCode(int temp_instruct1, int temp_instruct2, int temp_index, double temp_singleVel, double temp_carVelx, double temp_carVely, double temp_handVelx, double temp_handVely)
        {
            sendComd.instruct1 = temp_instruct1;
            sendComd.instruct2 = temp_instruct2;
            sendComd.index = temp_index;
            sendComd.singleVel = temp_singleVel;
            sendComd.carVelX = temp_carVelx;
            sendComd.carVelY = temp_carVely;
            sendComd.handVelX = temp_handVelx;
            sendComd.handVelY = temp_handVely;
        }
        public void appIdleHandle(Process p)
        {
            var hwndPanel = panel1.Handle;
            WindowInteropHelper helper = new WindowInteropHelper(Window.GetWindow(this));
            Thread.Sleep(1000);
            SetParent(p.MainWindowHandle, hwndPanel);   //将窗体以子窗体嵌入          
            HandleRef hanref = new HandleRef(this, p.MainWindowHandle);
            //SetWindowLong(HandleRef.ToIntPtr(hanref), GWL_STYLE, WS_VISIBLE);    //需要使用user32.dll,这SetWindowLong(HandleRef.ToIntPtr(hanref), GWL_STYLE, WS_VISIBLE);    //需要使用user32.dll,这样就去除了unity3D画面windows边界，并且使得画面能够显示出来样就去除了unity3D画面windows边界，并且使得画面能够显示出来
            MoveWindow(p.MainWindowHandle, 0, 0, (int)this.panel1.Width, (int)this.panel1.Height, true); //(int)this.PContainer.Left, (int)this.PContainer.Top, (int)this.PContainer.Width, (int)this.PContainer.Height, true);
            SetWindowLong(HandleRef.ToIntPtr(hanref), GWL_STYLE, WS_VISIBLE);    //需要使用user32.dll,这样就去除了unity3D画面windows边界，并且使得画面能够显示出来
            Thread.Sleep(1000);
            MoveWindow(p.MainWindowHandle, 0, 0, (int)this.panel1.Width, (int)this.panel1.Height, true);
            SetWindowLong(HandleRef.ToIntPtr(hanref), GWL_STYLE, WS_VISIBLE);

        }
        public void client()//TCP连接
        {        
            Mysocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //获取ip对象
            IPAddress address = IPAddress.Parse("192.168.1.200");
            //创建IP和端口
            IPEndPoint endpoint = new IPEndPoint(address, int.Parse("4999"));
            while (true)
            {
                try
                {
                    Mysocket.Connect(endpoint);
                }
                catch
                {                    
                }
                if (Mysocket.Connected)
                {
                    sign = 1;
                    break;
                }
                else
                {
                    sign = 0;
                }
            }
            System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
            new DeleFunc(Func));
            Thread threadWatch = new Thread(ReceiveMsg);
            threadWatch.IsBackground = true;
            threadWatch.Start();
            Thread threadAutoSend = new Thread(AtuoSends);
            threadAutoSend.IsBackground = true;
            threadAutoSend.Start();
            Thread threadSend = new Thread(Sends);
            threadSend.IsBackground = true;
            threadSend.Start();
        }
        public void ReceiveMsg()//TCP收数据
        {
            while (true)
            {
                int strlong;
                try
                {
                    strlong = Mysocket.Receive(arrMsgRec);
                    trans.handStatus = ByteToStructure<status>(arrMsgRec);
                    robotStatus.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        robotStatus.AppendText("大臂："+(int)trans.handStatus.bigarm_position + " 中臂：" + (int)trans.handStatus.middlearm_position + " 小臂：" + (int)trans.handStatus.smallarm_position + "\n"+
                            "摆臂：" + (int)trans.handStatus.swingarm1 + " " + (int)trans.handStatus.swingarm2 + " " + (int)trans.handStatus.swingarm3 + " " + (int)trans.handStatus.swingarm4 + "\n");
                        robotStatus.ScrollToEnd();
                    }
                               )); 
                }
                catch
                {
                }
            }
        }
        public void Sends()//TCP自动发
        {
            while (true)
            {
                try
                {
                    Mysocket.Send(StructureToByte<command>(sendComd));
                }
                catch
                {
                }
                Delay(10);
            }
        }
        public void OnceSends()//发一次的，按钮
        {          
                try
                {
                    Mysocket.Send(StructureToByte<command>(sendComd));
                }
                catch
                {
                }
        }
        public void AtuoSends()//不停发99，请求返回轴数据
        {
            try
            {
               while(true)
                {
                    statusComd.instruct1 = 99;
                    Mysocket.Send(StructureToByte<command>(statusComd));
                    Delay(100);
                }
            }
            catch
            {
            }
        }
        
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("您是否要关闭窗体？", "窗体关闭提示信息", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.MainWindow.Close();
            }
            else if (result == MessageBoxResult.No)
            {
                return;
            }
        }
        public delegate void DeleFunc();
        public void Func()//连接状态图片切换
        {           
                if (sign == 0)
                {
                    led_connect.Source = new BitmapImage(new Uri(@"skin/red_light.png", UriKind.Relative));
                    led_state.Source = new BitmapImage(new Uri(@"skin/red_light.png", UriKind.Relative));
                    
                }
                else
                {
                    led_connect.Source = new BitmapImage(new Uri(@"skin/blue_light.png", UriKind.Relative));
                    led_state.Source = new BitmapImage(new Uri(@"skin/blue_light.png", UriKind.Relative));
                }           
        }
        public static byte[] StructureToByte<T>(T structure)//结构体转BYTE[]
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            IntPtr bufferIntPtr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure, bufferIntPtr, true);
                Marshal.Copy(bufferIntPtr, buffer, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(bufferIntPtr);
            }
            return buffer;
        }
        public static T ByteToStructure<T>(byte[] dataBuffer)//BYTE[]转结构体
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
        public static void Delay(int delayTime)//延时函数
        {
            DateTime now = DateTime.Now;
            double s;
            do
            {
                TimeSpan spand = DateTime.Now - now;
                s = spand.TotalMilliseconds;
            }
            while (s < delayTime * 10);
        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            creatCode(1, 0, 0, 0, 0, 0, 0, 0);
            OnceSends();
        }

        private void btn_halt_Click(object sender, RoutedEventArgs e)
        {
            creatCode(2, 0, 0, 0, 0, 0, 0, 0);
            OnceSends();
        }

        private void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            creatCode(3, 0, 0, 0, 0, 0, 0, 0);
            OnceSends();
        }

        private void btn_ready_Click(object sender, RoutedEventArgs e)
        {
            creatCode(14, 0, 0, 0, 0, 0, 0, 0);
            OnceSends();
        }

        private void btn_home_Click(object sender, RoutedEventArgs e)
        {
            toggle_1 = 0;
            creatCode(13, 0, 0, 0, 0, 0, 0, 0);
            OnceSends();
        }

        private void Box1_Click(object sender, RoutedEventArgs e)
        {
            if (toggle_1 == 1)
            {
                creatCode(16, 0, 0, 0, 0, 0, 0, 0);
                OnceSends();
            }
        }

        private void Box2_Click(object sender, RoutedEventArgs e)
        {
            if (toggle_1 == 1)
            {
                creatCode(17, 0, 0, 0, 0, 0, 0, 0);
                OnceSends();
            }
        }

        private void Box3_Click(object sender, RoutedEventArgs e)
        {
            if (toggle_1 == 1)
            {
                creatCode(18, 0, 0, 0, 0, 0, 0, 0);
                OnceSends();
            }
        }

        private void Box4_Click(object sender, RoutedEventArgs e)
        {
            if (toggle_1 == 1)
            {
                creatCode(19, 0, 0, 0, 0, 0, 0, 0);
                OnceSends();
            }
        }

        private void btnputdown_Click(object sender, RoutedEventArgs e)
        {
            toggle_1 = 1;
            creatCode(15, 0, 0, 0, 0, 0, 0, 0);
            OnceSends();
        }

    }
    public class RobotInteraction //unity类
    {
        UdpClient udpClient;
        IPAddress remoteIpAdress;
        IPEndPoint remoteIpEndPoint;
        private string GetLocalIp()
        {
            string hostname = Dns.GetHostName();
            IPHostEntry localhost = Dns.GetHostByName(hostname);
            IPAddress localaddr = localhost.AddressList[0];
            return localaddr.ToString();
        }
        //UDP通信设置
        public void udp_Client()
        {
            udpClient = new UdpClient();
            remoteIpAdress = IPAddress.Parse(GetLocalIp());
            remoteIpEndPoint = new IPEndPoint(remoteIpAdress, 10000);
        }       
        public void udp_SendMessage()
        {
            while (true)
            {
                byte[] sendBytes = StructureToByte<status>(trans.handStatus);
                udpClient.Send(sendBytes, sendBytes.Length, remoteIpEndPoint);
            }
        }
        public void udp_SendMessageint(string flag)
        {
            byte[] sendBytes = System.Text.Encoding.Unicode.GetBytes(flag);
            udpClient.Send(sendBytes, sendBytes.Length, remoteIpEndPoint);
        }
        public static byte[] StructureToByte<T>(T structure)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            IntPtr bufferIntPtr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure, bufferIntPtr, true);
                Marshal.Copy(bufferIntPtr, buffer, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(bufferIntPtr);
            }
            return buffer;
        }
    }
}
