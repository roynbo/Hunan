/*************************************************************************/
/*���ܣ����������
/*������:������
/*�ش�Ķ���ʱ��:
2018/3/22

*************************************************************************/
#pragma once
#include"Axis.h"
#include"stdlib.h"
#include "stdio.h"

class Vehicle
{
public:
	Axis DrivingWheel[4];						    //���ֹ���8����
	virtual void Power(int axisID, bool enable);
	virtual void MoveAbsolute(int axisID, double velo, double pos);
	virtual void MoveRelative(int axisID, double velo, double pos);
	virtual void MoveVelocity(int axisID, double velo);
	virtual void Reset(int axisID);
	virtual void Home(int axisID);
	virtual void Halt(int axisID);
};