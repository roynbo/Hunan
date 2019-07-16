#pragma once
#include"Axis.h"
#include"stdlib.h"
#include "stdio.h"

class Mainpulator
{
public:
	Axis Mainpu[7];						    //机械臂有大臂、中臂、小臂、腰、回转、夹紧，我把机械臂末端做在了编号6上
	virtual void Power(int axisID, bool enable);
	virtual void MoveAbsolute(int axisID, double velo, double pos);
	virtual void MoveRelative(int axisID, double velo, double pos);
	virtual void MoveVelocity(int axisID, double velo);
	virtual void Reset(int axisID);
	virtual void Home(int axisID);
	virtual void Halt(int axisID);

};