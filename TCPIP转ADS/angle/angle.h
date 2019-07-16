/*********************************************************
用途：串口读取倾角仪应用函数
作者：艾田
日期：2016.7.18
***********************************************************/

#pragma once

#include<windows.h>
#include<stdio.h>
#include<stdlib.h>
#include<string.h>

bool openport(char *portname);
bool setupdcb(int rate_arg);
bool setuptimeout(DWORD ReadInterval, DWORD ReadTotalMultiplier, DWORD
	ReadTotalconstant, DWORD WriteTotalMultiplier, DWORD WriteTotalconstant);
int ReceiveChar(BOOL data_count);
bool WriteChar(BYTE* m_szWriteBuffer, DWORD m_nToSend);
void delay_system();
void com_init();
bool get_angle(double*angleX, double*angleY);
void set_zero(void);
