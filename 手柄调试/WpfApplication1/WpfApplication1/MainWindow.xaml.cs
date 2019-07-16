using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    public  struct SY
    {
        public int velocityX;
        public int velocityY;
        public int button;
    }
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort ComDevice = new SerialPort();
        private List<byte> buffer = new List<byte>(4096);
        SY handleData;
        public MainWindow()
        {
            InitializeComponent();
            init();

        }
        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void init()
        {
            while (ComDevice.IsOpen == false)
            {
                ComDevice.PortName = "COM14";
                ComDevice.BaudRate = Convert.ToInt32("9600");
                ComDevice.Parity = (Parity)Convert.ToInt32("0");
                ComDevice.DataBits = Convert.ToInt32("8");
                ComDevice.StopBits = (StopBits)Convert.ToInt32("1");
                try
                {
                    ComDevice.Open();
                }
                catch (Exception ex)
                {
                   
                }

            }
            ComDevice.DataReceived += new SerialDataReceivedEventHandler(Com_DataReceived);//绑定事件
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] readBuffer = new byte[ComDevice.BytesToRead];
            byte[] ReDatas = new byte[ComDevice.BytesToRead];
            byte[] byteVelocityX = new byte[2];
            byte[] byteVelocityY = new byte[2];
            byte[] byteButton = new byte[1];
                ComDevice.Read(ReDatas, 0, ReDatas.Length);//读取数据
                buffer.AddRange(ReDatas);
                //2.完整性判断
                while (buffer.Count >=9) //至少包含帧头（2字节）、长度（1字节）、校验位（1字节）；根据设计不同而不同
                {
                    //至少包含标头(1字节),长度(1字节),校验位(2字节)等等
                    //2.1 查找数据标记头           
                    if (buffer[0] ==  255&&buffer[5]==8) //传输数据有帧头，用于判断      
                    {
                        int len = 8;
                        if (buffer.Count < len + 1)
                        {
                            //数据未接收完整跳出循环
                            break;
                        }
                        readBuffer = new byte[len + 1];
                        //得到完整的数据，复制到readBuffer中   
                        buffer.CopyTo(0, readBuffer, 0, len + 1);
                        byteVelocityX[0] = readBuffer[1];
                        byteVelocityX[1] = readBuffer[2];
                        byteVelocityY[0] = readBuffer[3];
                        byteVelocityY[1] = readBuffer[4];
                        byteButton[0] = readBuffer[7];
                        ByteTo16(byteVelocityX);
                        ByteTo16(byteVelocityY);
                        ByteTo16(byteButton);
                        //从缓冲区中清除
                        buffer.RemoveRange(0, len + 1);
                        this.AddData(readBuffer);//输出数据
                  
                        //触发外部处理接收消息事件
                            
                        }
                    else //开始标记或版本号不正确时清除          
                    {
                        buffer.RemoveAt(0);
                    }
                    }


             
            
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data">字节数组</param>
        public void AddData(byte[] data)
        {

                StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < data.Length; i++)
                    {
                            sb.AppendFormat("{0:x2}" + " ", data[i]);
                      }
                            AddContent(sb.ToString().ToUpper()+"\n");
         }


        


        /// <summary>
        /// 输入到显示区域
        /// </summary>
        /// <param name="content"></param>
        private void AddContent(string content)
        {
            data.Dispatcher.BeginInvoke(new Action(delegate
            {
                data.AppendText(content);
                data.ScrollToEnd();
            }
            ));
        }


        /**
  * 数组转换成十六进制字符串
  * @param byte[]
  * @return HexString
  */
        public void ByteTo16(byte[] data)
        {
            int dec = 0;
            if(data.Length==2)
            {
                dec = Convert.ToInt32(data[0].ToString()) * 16 * 16 + Convert.ToInt32(data[1].ToString());
            }
            else
            {
                dec = Convert.ToInt32(data[0].ToString());
            }
            Console.WriteLine(dec+"  ");
        }
    }
}
