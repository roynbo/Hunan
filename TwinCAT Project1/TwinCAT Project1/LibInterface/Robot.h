/*************************************************************************/
/*功能：机器人类，轴类具象化
/*创建者:郭宁波
/*重大改动及时间:
2018/5/22
*************************************************************************/

#include "Axis.h"
#include "Vehicle.h"
#include "SwingArm.h"
#include "Mainpulator.h"

class Robot
{
public:
	Vehicle vehicle;
	SwingArms arm;
	Mainpulator mainpulator;
	virtual void ComdClear( int instruct);
	virtual void Init();
	virtual void DecoderInstruct(int instruct, int axisID, double velo, double pos, double carVelX, double carVelY, double handVelX, double handVelY,int &flag);
	virtual void CalculateHandVelo(double velX, double velY,int instruct);
	virtual void HandStop();
};