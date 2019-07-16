#include "Vehicle.h"
void Vehicle::Power(int axisID, bool enable)
{
	DrivingWheel[axisID].AxisCommand.Enable = enable;
}
void Vehicle::MoveAbsolute(int axisID, double velo, double pos)
{
	DrivingWheel[axisID].AxisCommand.MoveAbsolute = true;
	DrivingWheel[axisID].AxisCommand.Velocity = velo;
	DrivingWheel[axisID].AxisCommand.Position = pos;
}
void Vehicle::MoveRelative(int axisID, double velo, double pos)
{
	DrivingWheel[axisID].AxisCommand.MoveRelative = true;
	DrivingWheel[axisID].AxisCommand.Velocity = velo;
	DrivingWheel[axisID].AxisCommand.Position = pos;
}
void Vehicle::MoveVelocity(int axisID, double velo)
{
	DrivingWheel[axisID].AxisCommand.MoveVelocity = true;
	DrivingWheel[axisID].AxisCommand.Velocity = velo;
}
void Vehicle::Reset(int axisID)
{
	DrivingWheel[axisID].AxisCommand.Reset = true;
}
void Vehicle::Home(int axisID)
{
	DrivingWheel[axisID].AxisCommand.Home = true;
}
void Vehicle::Halt(int axisID)
{
	DrivingWheel[axisID].AxisCommand.Halt = true;
}