#include "Mainpulator.h"
void Mainpulator::Power(int axisID, bool enable)
{
	Mainpu[axisID].AxisCommand.Enable = enable;
}
void Mainpulator::MoveAbsolute(int axisID, double velo, double pos)
{
	Mainpu[axisID].AxisCommand.MoveAbsolute = true;
	Mainpu[axisID].AxisCommand.Velocity = velo;
	Mainpu[axisID].AxisCommand.Position = pos;
}
void Mainpulator::MoveRelative(int axisID, double velo, double pos)
{
	Mainpu[axisID].AxisCommand.MoveRelative = true;
	Mainpu[axisID].AxisCommand.Velocity = velo;
	Mainpu[axisID].AxisCommand.Position = pos;
}
void Mainpulator::MoveVelocity(int axisID, double velo)
{
	Mainpu[axisID].AxisCommand.MoveVelocity = true;
	Mainpu[axisID].AxisCommand.Velocity = velo;
}
void Mainpulator::Reset(int axisID)
{
	Mainpu[axisID].AxisCommand.Reset = true;
}
void Mainpulator::Home(int axisID)
{
	Mainpu[axisID].AxisCommand.Home = true;
}
void Mainpulator::Halt(int axisID)
{
	Mainpu[axisID].AxisCommand.Halt = true;
}