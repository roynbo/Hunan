using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace adsClientVisu
{
    public static class GlobalVar
    {
        public static uint S_FunctionInstruct;
        public static uint S_AxisIndex;
        public static double S_Velocity;
        public static double S_VehicleVelo_x;
        public static double S_VehicleVelo_y;
        public static double S_ArmVelo_x;
        public static double S_ArmVelo_y;
        public static double S_ArmVelo_z;
        public static double S_ArmVelo_alpha;
        public static double S_ArmVelo_beta;
        public static double S_ArmVelo_gamma;
        public static int S_EndEffectorMode;
        public static double S_FSwingArmVelo;
        public static double S_BSwingArmVelo;
        public static double S_CouplingCarVelo;
        public static double S_PosB_X;
        public static double S_PosB_Y;
        public static double S_PosB_Z;
        public static double S_PosC_X;
        public static double S_PosC_Y;
        public static double S_PosC_Z;
        public static int S_RouteMode;
        public static double S_RouteVelo;
        public static int S_LaserReady;
        public static int S_LaserPrint;
        public static uint R_ErrCode;
        public static uint R_AxisIndex;
        public static uint R_Enable;
        public static uint flag;
        public static double R_ActVelo;
        public static double R_ActPos;
        public static double R_ActPosX;
        public static double R_ActPosY;
        public static int R_ActCur;
        public static double R_ArmActVelo_x;
        public static double R_ArmActVelo_y;
        public static double R_ArmActVelo_z;
        public static double R_ArmActVelo_alpha;
        public static double R_ArmActVelo_beta;
        public static double R_ArmActVelo_gamma;
        public static double R_Stable;
        public static int R_Zone;
        public static double roll;
        public static double pitch;
        public static double yaw;
       
       
    }
}
namespace WindowsFormsApplication1
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
