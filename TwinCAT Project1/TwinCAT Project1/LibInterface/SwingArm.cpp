#include "SwingArm.h"
void SwingArms::Power(int axisID, bool enable)
{
	SwingArm[axisID].AxisCommand.Enable = enable;
}
void SwingArms::MoveAbsolute(int axisID, double velo, double pos)
{
	SwingArm[axisID].AxisCommand.MoveAbsolute = true;
	SwingArm[axisID].AxisCommand.Velocity = velo;
	SwingArm[axisID].AxisCommand.Position = pos;
}
void SwingArms::MoveRelative(int axisID, double velo, double pos)
{
	SwingArm[axisID].AxisCommand.MoveRelative = true;
	SwingArm[axisID].AxisCommand.Velocity = velo;
	SwingArm[axisID].AxisCommand.Position = pos;
}
void SwingArms::MoveVelocity(int axisID, double velo)
{
	SwingArm[axisID].AxisCommand.MoveVelocity = true;
	SwingArm[axisID].AxisCommand.Velocity = velo;
}
void SwingArms::Reset(int axisID)
{
	SwingArm[axisID].AxisCommand.Reset = true;
}
void SwingArms::Home(int axisID)
{
	SwingArm[axisID].AxisCommand.Home = true;
}
void SwingArms::Halt(int axisID)
{
	SwingArm[axisID].AxisCommand.Halt = true;
}