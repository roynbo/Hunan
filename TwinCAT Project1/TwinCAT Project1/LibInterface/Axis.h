/*************************************************************************/
/*功能：轴类，基础
/*创建者:郭宁波
/*重大改动及时间:
2018/5/22

*************************************************************************/
#pragma once
#include"LibInterfaceServices.h"
//轴状态
typedef struct StatusAxis
{
	bool	Enable;											    //轴使能状态
	int		Current;											//轴电流
	double position;
	double velocity;

}Statusmode;

//轴命令
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
	Statusmode AxisStatus;				//读到的轴状态
	CommandtoAxis AxisCommand;			//给轴发送的命令
	
public:
	Axis();
	~Axis();
};