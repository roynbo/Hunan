#include "Robot.h"

#define length_BigArm 0.55
#define length_MiddleArm 0.45
#define length_SmallArm 0.338

void Robot::Init()
{
	{
		for (int i = 0; i < 4; i++)
		{
			memset(&vehicle.DrivingWheel[i].AxisCommand, 0, sizeof(vehicle.DrivingWheel[i].AxisCommand));
			memset(&vehicle.DrivingWheel[i].AxisStatus, 0, sizeof(vehicle.DrivingWheel[i].AxisStatus));
		}
		for (int i = 0; i < 4; i++)
		{
			memset(&arm.SwingArm[i].AxisCommand, 0, sizeof(vehicle.DrivingWheel[i].AxisCommand));
			memset(&arm.SwingArm[i].AxisStatus, 0, sizeof(vehicle.DrivingWheel[i].AxisStatus));
		}
		for (int i = 0; i < 7; i++)
		{
			memset(&mainpulator.Mainpu[i].AxisCommand, 0, sizeof(mainpulator.Mainpu[i].AxisCommand));
			memset(&mainpulator.Mainpu[i].AxisStatus, 0, sizeof(mainpulator.Mainpu[i].AxisStatus));
		}
	}
}
void Robot::HandStop()
{
	mainpulator.Mainpu[1].AxisCommand.MoveVelocity = false;
	mainpulator.Mainpu[3].AxisCommand.MoveVelocity = false;
	mainpulator.Mainpu[4].AxisCommand.MoveVelocity = false;
}
void Robot::ComdClear( int instruct)
{
	if (instruct != 0 )
	{
		for (int i = 0; i < 4; i++)
		{
			vehicle.DrivingWheel[i].AxisCommand.Halt = false;
			vehicle.DrivingWheel[i].AxisCommand.MoveAbsolute = false;
			vehicle.DrivingWheel[i].AxisCommand.MoveRelative = false;
			vehicle.DrivingWheel[i].AxisCommand.MoveVelocity = false;
			vehicle.DrivingWheel[i].AxisCommand.Reset = false;
			arm.SwingArm[i].AxisCommand.Halt = false;
			arm.SwingArm[i].AxisCommand.MoveAbsolute = false;
			arm.SwingArm[i].AxisCommand.MoveRelative = false;
			arm.SwingArm[i].AxisCommand.MoveVelocity = false;
			arm.SwingArm[i].AxisCommand.Reset = false;
			arm.SwingArm[i].AxisCommand.SetZero = false;
		}
		for (int i = 0; i < 7; i++)
		{
			mainpulator.Mainpu[i].AxisCommand.Halt = false;
			mainpulator.Mainpu[i].AxisCommand.MoveAbsolute = false;
			mainpulator.Mainpu[i].AxisCommand.MoveRelative = false;
			mainpulator.Mainpu[i].AxisCommand.MoveVelocity = false;
			mainpulator.Mainpu[i].AxisCommand.Reset = false;
			mainpulator.Mainpu[i].AxisCommand.Home = false;
			mainpulator.Mainpu[i].AxisCommand.SetZero = false;
		}
		instruct = 0;
	}
	
}
void Robot::CalculateHandVelo(double velX, double velY,int instruct)
{
	{
		double a = mainpulator.Mainpu[1].AxisStatus.position;
		double b = mainpulator.Mainpu[3].AxisStatus.position;
		double c = mainpulator.Mainpu[4].AxisStatus.position;
		double tempa = a;
		double tempb = b;
		a = a + 90;
		b = b + tempa+90;
		c = c + tempb + tempa+90;
		a = a / 180 * PI;
		b = b / 180 * PI;
		c = c / 180 * PI;
		mainpulator.Mainpu[1].AxisCommand.Velocity = (velX*cos_(b) + velY*sin_(b)) / (length_BigArm*sin_(b - a));
		mainpulator.Mainpu[3].AxisCommand.Velocity = (velY - mainpulator.Mainpu[1].AxisCommand.Velocity*(length_BigArm*cos_(a) + length_MiddleArm*cos_(b))) / (length_MiddleArm*cos_(b));
		mainpulator.Mainpu[4].AxisCommand.Velocity = -mainpulator.Mainpu[1].AxisCommand.Velocity - mainpulator.Mainpu[3].AxisCommand.Velocity;
		mainpulator.Mainpu[1].AxisCommand.Velocity = mainpulator.Mainpu[1].AxisCommand.Velocity / PI * 180;
		mainpulator.Mainpu[3].AxisCommand.Velocity = mainpulator.Mainpu[3].AxisCommand.Velocity / PI * 180;
		mainpulator.Mainpu[4].AxisCommand.Velocity = mainpulator.Mainpu[4].AxisCommand.Velocity / PI * 180;
		mainpulator.Mainpu[1].AxisCommand.MoveVelocity = true;
		mainpulator.Mainpu[3].AxisCommand.MoveVelocity = true;
		mainpulator.Mainpu[4].AxisCommand.MoveVelocity = true;
	}
}
void Robot::DecoderInstruct(int instruct, int axisID, double velo, double pos, double carVelX, double carVelY, double handVelX, double handVelY,int &flag)
{
	if (instruct == 20)				//������ʹ��
	{
		for (int i = 0; i < 4; i++)
		{
			vehicle.DrivingWheel[i].AxisCommand.Enable = true;
			arm.SwingArm[i].AxisCommand.Enable = true;
		}
	}

	else if (instruct == 30)			//��е����ʹ��
	{
		for (int i = 0; i < 7; i++)
		{
			mainpulator.Mainpu[i].AxisCommand.Enable = true;
		}
		flag = 2;
	}

	else if (instruct == 40)				//������ʹ��
	{
		for (int i = 0; i < 4; i++)
		{
			vehicle.DrivingWheel[i].AxisCommand.Enable = false;
			arm.SwingArm[i].AxisCommand.Enable = false;
		}
	}
	else if (instruct == 50)				//��е����ʹ��
	{
		for (int i = 0; i < 7; i++)
		{
			mainpulator.Mainpu[i].AxisCommand.Enable = false;
		}
	}
	else if (instruct == 60)				//ȫ�帴λ
	{
		for (int i = 0; i < 4; i++)
		{
			vehicle.DrivingWheel[i].AxisCommand.Reset = true;
			arm.SwingArm[i].AxisCommand.Reset = true;
			vehicle.DrivingWheel[i].AxisCommand.Enable = false;
			arm.SwingArm[i].AxisCommand.Enable = false;
		}
		for (int i = 0; i < 7; i++)
		{
			mainpulator.Mainpu[i].AxisCommand.Reset = true;
			mainpulator.Mainpu[i].AxisCommand.Enable = false;
		}
	}
	else if (instruct == 70)				//ȫ����ͣ
	{
		for (int i = 0; i < 4; i++)
		{
			vehicle.DrivingWheel[i].AxisCommand.Halt = true;
			arm.SwingArm[i].AxisCommand.Halt = true;
		}
		for (int i = 0; i < 7; i++)
		{
			mainpulator.Mainpu[i].AxisCommand.Halt = true;
		}
	}
	else if (instruct == 90)				//��ģʽ
	{


		double VeloL = 0;
		double VeloR = 0;
		VeloL = carVelX - carVelY / 120;
		VeloR = carVelX + carVelY / 120;
		vehicle.DrivingWheel[0].AxisCommand.Velocity = VeloL;
		vehicle.DrivingWheel[3].AxisCommand.Velocity = VeloL;
		vehicle.DrivingWheel[1].AxisCommand.Velocity = VeloR;
		vehicle.DrivingWheel[2].AxisCommand.Velocity = VeloR;

		for (int i = 0; i < 4; i++)
		{
			vehicle.DrivingWheel[i].AxisCommand.MoveVelocity = true;

		}
	}
	else if (instruct == 100)				//��ģʽ
	{
		
	}
	else if (instruct == 110)				//���ֵ���㶯
	{
		vehicle.DrivingWheel[axisID].AxisCommand.Velocity = velo;
		vehicle.DrivingWheel[axisID].AxisCommand.MoveVelocity = true;
	}
	else if (instruct == 120)				//�ڱ۵���㶯
	{
		arm.SwingArm[axisID].AxisCommand.Velocity = velo;
		arm.SwingArm[axisID].AxisCommand.MoveVelocity = true;
	}

	else if (instruct == 130)				//��е�۵���㶯
	{
		mainpulator.Mainpu[axisID].AxisCommand.Velocity = velo;
		mainpulator.Mainpu[axisID].AxisCommand.MoveVelocity = true;
	}
	else if (instruct == 140)				//�ڱ�����˶�
	{
		arm.SwingArm[axisID].AxisCommand.Velocity = velo;
		arm.SwingArm[axisID].AxisCommand.Position = pos;
		arm.SwingArm[axisID].AxisCommand.MoveRelative = true;
	}
	else if (instruct == 150)			//��е������˶�
	{
		mainpulator.Mainpu[axisID].AxisCommand.Velocity = velo;
		mainpulator.Mainpu[axisID].AxisCommand.Position = pos;
		mainpulator.Mainpu[axisID].AxisCommand.MoveRelative = true;
	}
	else if (instruct == 260)				//ǰ˫�������˶�
	{
		arm.SwingArm[0].AxisCommand.Velocity = velo;
		arm.SwingArm[0].AxisCommand.MoveVelocity = true;
		arm.SwingArm[1].AxisCommand.Velocity = velo;
		arm.SwingArm[1].AxisCommand.MoveVelocity = true;
	}
	else if (instruct == 270)				//��˫�������˶�
	{
		arm.SwingArm[2].AxisCommand.Velocity = velo;
		arm.SwingArm[2].AxisCommand.MoveVelocity = true;
		arm.SwingArm[3].AxisCommand.Velocity = velo;
		arm.SwingArm[3].AxisCommand.MoveVelocity = true;
	}
	else if (instruct == 280)				//����˫�������˶�
	{
		arm.SwingArm[0].AxisCommand.Velocity = velo;
		arm.SwingArm[0].AxisCommand.MoveVelocity = true;
		arm.SwingArm[1].AxisCommand.Velocity = velo;
		arm.SwingArm[1].AxisCommand.MoveVelocity = true;
		arm.SwingArm[2].AxisCommand.Velocity = velo;
		arm.SwingArm[2].AxisCommand.MoveVelocity = true;
		arm.SwingArm[3].AxisCommand.Velocity = velo;
		arm.SwingArm[3].AxisCommand.MoveVelocity = true;
	}
	else if (instruct == 290)
	{
	mainpulator.Mainpu[0].AxisCommand.Velocity = 5;
	mainpulator.Mainpu[0].AxisCommand.Position = 0;
	mainpulator.Mainpu[0].AxisCommand.MoveAbsolute = true;
	mainpulator.Mainpu[1].AxisCommand.Velocity = 5;
	mainpulator.Mainpu[1].AxisCommand.Position = 0;
	mainpulator.Mainpu[1].AxisCommand.MoveAbsolute = true;
	mainpulator.Mainpu[3].AxisCommand.Velocity = 10;
	mainpulator.Mainpu[3].AxisCommand.Position = 0;
	mainpulator.Mainpu[3].AxisCommand.MoveAbsolute = true;
	mainpulator.Mainpu[4].AxisCommand.Velocity = 10;
	mainpulator.Mainpu[4].AxisCommand.Position = 0;
	mainpulator.Mainpu[4].AxisCommand.MoveAbsolute = true;
	}
	else if (instruct == 300)//����
	{
		for (int i = 0; i <= 3; i++)
			arm.SwingArm[i].AxisCommand.SetZero = true;
		mainpulator.Mainpu[1].AxisCommand.SetZero = true;
		mainpulator.Mainpu[3].AxisCommand.SetZero = true;
		mainpulator.Mainpu[4].AxisCommand.SetZero = true;
		mainpulator.Mainpu[0].AxisCommand.SetZero = true;
	}
	else if (instruct == 320)//��λ
	{
		mainpulator.Mainpu[0].AxisCommand.Velocity = 5;
		mainpulator.Mainpu[0].AxisCommand.Position = 0;
		mainpulator.Mainpu[0].AxisCommand.MoveAbsolute = true;
		mainpulator.Mainpu[1].AxisCommand.Velocity = 5;
		mainpulator.Mainpu[1].AxisCommand.Position = 0;
		mainpulator.Mainpu[1].AxisCommand.MoveAbsolute = true;
		mainpulator.Mainpu[3].AxisCommand.Velocity = 10;
		mainpulator.Mainpu[3].AxisCommand.Position = 0;
		mainpulator.Mainpu[3].AxisCommand.MoveAbsolute = true;
		mainpulator.Mainpu[4].AxisCommand.Velocity = 10;
		mainpulator.Mainpu[4].AxisCommand.Position = 0;
		mainpulator.Mainpu[4].AxisCommand.MoveAbsolute = true;
	}
	else if (instruct == 330)//����λ��
	{
		flag = 7;
		mainpulator.Mainpu[0].AxisCommand.Velocity =5 ;
		mainpulator.Mainpu[0].AxisCommand.Position = 0;
		mainpulator.Mainpu[0].AxisCommand.MoveAbsolute = true;
	}
	else if (instruct == 340)//ץȡλ��
	{
		flag = 8;
		for (int i = 0; i < 4; i++)
		{
			arm.SwingArm[i].AxisCommand.Velocity = 8;
			arm.SwingArm[i].AxisCommand.Position = -10;
			arm.SwingArm[i].AxisCommand.MoveAbsolute = true;
		}
		mainpulator.Mainpu[0].AxisCommand.Velocity = 5;
		mainpulator.Mainpu[0].AxisCommand.Position = 0;
		mainpulator.Mainpu[0].AxisCommand.MoveAbsolute = true;
	}
	else if (instruct == 350)//����λ��
	{
		flag = 1;
		for (int i = 0; i < 4; i++)
		{
			arm.SwingArm[i].AxisCommand.Velocity = 8;
			arm.SwingArm[i].AxisCommand.Position = -10;
			arm.SwingArm[i].AxisCommand.MoveAbsolute = true;
		}
		mainpulator.Mainpu[1].AxisCommand.Velocity = 12;
		mainpulator.Mainpu[1].AxisCommand.Position = 10;
		mainpulator.Mainpu[1].AxisCommand.MoveAbsolute = true;
		mainpulator.Mainpu[3].AxisCommand.Velocity = 10;
		mainpulator.Mainpu[3].AxisCommand.Position = -90;
		mainpulator.Mainpu[3].AxisCommand.MoveAbsolute = true;
		mainpulator.Mainpu[4].AxisCommand.Velocity = 10;
		mainpulator.Mainpu[4].AxisCommand.Position = -100;
		mainpulator.Mainpu[4].AxisCommand.MoveAbsolute = true;
	}
	else if (instruct == 360)//����λ��1
	{
		flag = 6;
		for (int i = 0; i < 4; i++)
		{
			arm.SwingArm[i].AxisCommand.Velocity = 8;
			arm.SwingArm[i].AxisCommand.Position = -10;
			arm.SwingArm[i].AxisCommand.MoveAbsolute = true;
		}
		mainpulator.Mainpu[1].AxisCommand.Velocity = 12;
		mainpulator.Mainpu[1].AxisCommand.Position =14;
		mainpulator.Mainpu[1].AxisCommand.MoveAbsolute = true;
		mainpulator.Mainpu[3].AxisCommand.Velocity = 10;
		mainpulator.Mainpu[3].AxisCommand.Position = -105;
		mainpulator.Mainpu[3].AxisCommand.MoveAbsolute = true;
		mainpulator.Mainpu[4].AxisCommand.Velocity = 10;
		mainpulator.Mainpu[4].AxisCommand.Position = -90;
		mainpulator.Mainpu[4].AxisCommand.MoveAbsolute = true;
	}
	else if (instruct == 370)//����λ��2
	{
		flag = 3;
		for (int i = 0; i < 4; i++)
		{
			arm.SwingArm[i].AxisCommand.Velocity = 8;
			arm.SwingArm[i].AxisCommand.Position = -10;
			arm.SwingArm[i].AxisCommand.MoveAbsolute = true;
		}
		mainpulator.Mainpu[1].AxisCommand.Velocity = 12;
		mainpulator.Mainpu[1].AxisCommand.Position = 14;
		mainpulator.Mainpu[1].AxisCommand.MoveAbsolute = true;
		mainpulator.Mainpu[3].AxisCommand.Velocity = 10;
		mainpulator.Mainpu[3].AxisCommand.Position = -105;
		mainpulator.Mainpu[3].AxisCommand.MoveAbsolute = true;
		mainpulator.Mainpu[4].AxisCommand.Velocity = 10;
		mainpulator.Mainpu[4].AxisCommand.Position = -90;
		mainpulator.Mainpu[4].AxisCommand.MoveAbsolute = true;
	}
	else if (instruct == 380)//����λ��3
	{
		flag = 4;
		for (int i = 0; i < 4; i++)
		{
			arm.SwingArm[i].AxisCommand.Velocity = 8;
			arm.SwingArm[i].AxisCommand.Position = -10;
			arm.SwingArm[i].AxisCommand.MoveAbsolute = true;
		}
		mainpulator.Mainpu[1].AxisCommand.Velocity = 12;
		mainpulator.Mainpu[1].AxisCommand.Position = 1;
		mainpulator.Mainpu[1].AxisCommand.MoveAbsolute = true;
		mainpulator.Mainpu[3].AxisCommand.Velocity = 10;
		mainpulator.Mainpu[3].AxisCommand.Position = -95;
		mainpulator.Mainpu[3].AxisCommand.MoveAbsolute = true;
		mainpulator.Mainpu[4].AxisCommand.Velocity = 10;
		mainpulator.Mainpu[4].AxisCommand.Position = -90;
		mainpulator.Mainpu[4].AxisCommand.MoveAbsolute = true;
	}
	else if (instruct == 390)//����λ��4
	{
		flag = 5;
		for (int i = 0; i < 4; i++)
		{
			arm.SwingArm[i].AxisCommand.Velocity = 8;
			arm.SwingArm[i].AxisCommand.Position = -10;
			arm.SwingArm[i].AxisCommand.MoveAbsolute = true;
		}
		mainpulator.Mainpu[1].AxisCommand.Velocity = 12;
		mainpulator.Mainpu[1].AxisCommand.Position = 1;
		mainpulator.Mainpu[1].AxisCommand.MoveAbsolute = true;
		mainpulator.Mainpu[3].AxisCommand.Velocity = 10;
		mainpulator.Mainpu[3].AxisCommand.Position = -95;
		mainpulator.Mainpu[3].AxisCommand.MoveAbsolute = true;
		mainpulator.Mainpu[4].AxisCommand.Velocity = 10;
		mainpulator.Mainpu[4].AxisCommand.Position = -90;
		mainpulator.Mainpu[4].AxisCommand.MoveAbsolute = true;
	}
	else if (instruct == 400)
		vehicle.DrivingWheel[1].AxisCommand.lighting = 0xffffffff;
	else if (instruct == 410)
		vehicle.DrivingWheel[1].AxisCommand.lighting = 0;


	
}


