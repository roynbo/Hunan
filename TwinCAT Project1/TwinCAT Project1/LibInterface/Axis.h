/*************************************************************************/
/*���ܣ����࣬����
/*������:������
/*�ش�Ķ���ʱ��:
2018/5/22

*************************************************************************/
#pragma once
#include"LibInterfaceServices.h"
//��״̬
typedef struct StatusAxis
{
	bool	Enable;											    //��ʹ��״̬
	int		Current;											//�����
	double position;
	double velocity;

}Statusmode;

//������
typedef struct CommandAxis
{
	bool Enable;
	bool Halt;
	bool Home;
	bool SetZero;
	bool Reset;
	bool MoveRelative;
	bool MoveAbsolute;
	bool MoveVelocity;
	double Velocity;
	double Position;
	LONG lighting;
}CommandtoAxis;

class Axis
{
public:
	Statusmode AxisStatus;				//��������״̬
	CommandtoAxis AxisCommand;			//���ᷢ�͵�����
	
public:
	Axis();
	~Axis();
};