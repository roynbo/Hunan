//#pragma once
#include"angle.h"

HANDLE hComm;
OVERLAPPED m_ov;
COMSTAT comstat;
DWORD m_dwCommEvents;

/********************************
*名称：打开串口函数
*功能：打开串口
*参数介绍：
 portname：打开的串口号（如com1）
********************************/
bool openport(char *portname)  //打开串口
{

	hComm = CreateFile(
		portname,//串口号
		GENERIC_READ | GENERIC_WRITE,//允许读写
		0,//通讯设备必须以独占方式打开
		0,//无安全属性
		OPEN_EXISTING,//通讯设备已存在
		FILE_FLAG_OVERLAPPED,//异步通信
		0//通讯设备不能用模板打开
		);
	if (hComm == INVALID_HANDLE_VALUE)//INVALID_HANDLE_VALUE=0xffffffff，串口打开失败的时候返回该值
	{
		//CloseHandle(hComm);
		return FALSE;
	}
	else
		return TRUE;
}
/******************************************************
*名称：DCB结构设置函数
*功能：设置与串口相关的参数，如波特率，数据位数等
*参数介绍：
   rate_arg：波特率（如9600）
******************************************************/
bool setupdcb(int rate_arg)
{
	//	HANDLE hComm;
	DCB dcb;//先声明DCB结构体,再获取DCB配置，再设置，最后看是否设置好
	int rate = rate_arg;
	memset(&dcb, 0, sizeof(dcb));//在一段内存块中填充某个给定的值，是对较大的结构体或数组进行清零操作的一种最快方法

	if (!GetCommState(hComm, &dcb))//获取当前DCB配置
		return FALSE;
	// set DCB to configure the serial port
	dcb.DCBlength = sizeof(dcb);
	/*----------Serial Port Config-------*/
	dcb.BaudRate = rate;
	dcb.Parity = NOPARITY;
	dcb.fParity = 0;
	dcb.StopBits = ONESTOPBIT;
	dcb.ByteSize = 8;
	dcb.fOutxCtsFlow = 0;
	dcb.fOutxDsrFlow = 0;
	dcb.fDtrControl = DTR_CONTROL_DISABLE;
	dcb.fDsrSensitivity = 0;
	dcb.fRtsControl = RTS_CONTROL_DISABLE;
	dcb.fOutX = 0;
	dcb.fInX = 0;
	/*-----------------misc parameters-----*/
	dcb.fErrorChar = 0;
	dcb.fBinary = 1;
	dcb.fNull = 0;
	dcb.fAbortOnError = 0;
	dcb.wReserved = 0;
	dcb.XonLim = 2;
	dcb.XoffLim = 4;
	dcb.XonChar = 0x13;
	dcb.XoffChar = 0x19;
	dcb.EvtChar = 0;//set DCB
	if (!SetCommState(hComm, &dcb))
		return FALSE;
	else
		return TRUE;
}

/***********************************************************************************************8*********
*名称：超时限制函数
*功能：防止串口在输入输出过程中由于出错而导致的超时现象，避免数据阻塞
*参数介绍：
ReadInterval：通信线两个字符间隔的最大时间（ms）。
ReadTotalMultiplier：以毫秒为单位指定一个乘数，该乘数用来计算读操作的总限时时间。
                     每个读操作的总限时时间等于读操作所需的字节数与该值的乘积
ReadTotalconstant：以毫秒为单位指定一个常数，用于计算读操作的总限时时间。
                   每个操作的总限时时间等于ReadTotalMultiplier成员乘以读操作所需字节数再加上该值的和。
WriteTotalMultiplier：以毫秒为单位指定一个乘数，该乘数用来计算写操作的总限时时间。
                     每个写操作的总限时时间等于读操作所需的字节数与该值的乘积
WriteTotalconstant：以毫秒为单位指定一个常数，用于计算写操作的总限时时间。
                    每个操作的总限时时间等于WriteTotalMultiplier成员乘以写操作所需字节数再加上该值的和。
**************************************************************************************************************/
bool setuptimeout(DWORD ReadInterval, DWORD ReadTotalMultiplier, DWORD
	ReadTotalconstant, DWORD WriteTotalMultiplier, DWORD WriteTotalconstant)
{
	//HANDLE hComm;
	COMMTIMEOUTS timeouts;
	timeouts.ReadIntervalTimeout = ReadInterval;
	timeouts.ReadTotalTimeoutConstant = ReadTotalconstant;
	timeouts.ReadTotalTimeoutMultiplier = ReadTotalMultiplier;
	timeouts.WriteTotalTimeoutConstant = WriteTotalconstant;
	timeouts.WriteTotalTimeoutMultiplier = WriteTotalMultiplier;
	if (!SetCommTimeouts(hComm, &timeouts))
		return FALSE;
	else
		return TRUE;
}

/*************************************
*名称：读入串口数据函数
*功能：读入串口中的数据
*参数介绍：
ata_count：data_count为需要读入的字节数
**************************************/
int ReceiveChar(BOOL data_count)//data_count为需要接受的字节
{
	BOOL bRead = TRUE;
	BOOL bResult = TRUE;
	DWORD dwError = 0;
	DWORD BytesRead = 0;
	unsigned char RXBuff;
	unsigned char temp = 0;
	unsigned char high_data = 0;
	unsigned char low_data = 0;
	float angle = 0.0;
	BOOL count = 0;
	for (;;)
	{
		bResult = ClearCommError(hComm, &dwError, &comstat);//在使用ReadFile函数进行读操作前，应先使用ClearCommError函数清除错误

		if (comstat.cbInQue == 0)//COMSTAT结构返回串口状态信息
			//本文只用到了cbInQue成员变量，该成员变量的值代表输入缓冲区的字节数
			continue;
		if (bRead)
		{
			bResult = ReadFile(hComm, &RXBuff, 1, &BytesRead, &m_ov);
			printf("%x   ", RXBuff);
			if (count == 4)
				temp = RXBuff;
			if (count == 5)
				high_data = RXBuff;
			if (count == 6)
				low_data = RXBuff;
			//printf("count=%d\n", count);		
			if (++count == data_count)
			{
				printf("\n");
				break;
			}
			if (!bResult)
			{
				switch (dwError = GetLastError())
				{
				case ERROR_IO_PENDING:
				{
										 bRead = FALSE; break;
				}
				default:{break; }
				}
			}
			else
			{
				bRead = TRUE;
			}
		}

		if (!bRead)
		{

			bRead = TRUE;
			bResult = GetOverlappedResult(hComm,//Handle to COMM port
				&m_ov,//Overlapped structure
				&BytesRead,
				TRUE);

		}
	}
	angle = (high_data / 16) * 10 + (high_data % 16) * 1 + (low_data / 16) * 0.1 + (low_data % 16) * 0.01;
	if (temp == 0x10)
		angle = -angle;
	printf("angle=%f\n", angle);
	return 0;
}

/*************************************
*名称：写入串口数据函数
*功能：向串口中写入数据（指定写入的首地址和写入的字节数）
*参数介绍：
    m_szWriteBuffer：要写入的首地址
    m_nToSend：要写入的字节数
**************************************/
bool WriteChar(BYTE* m_szWriteBuffer, DWORD m_nToSend)
{
	BOOL bWrite = TRUE;
	BOOL bResult = TRUE;
	DWORD BytesSent = 0;
	HANDLE m_hWriteEvent = NULL;
	ResetEvent(m_hWriteEvent);
	if (bWrite){
		m_ov.Offset = 0;
		m_ov.OffsetHigh = 0;//Clear buffer
		bResult = WriteFile(hComm,//?Handle?to?COMM?Port,?串口的句柄???
			m_szWriteBuffer,//?Pointer?to?message?buffer?in?calling?finction?
			//即以该指针的值为首地址的nNumberOfBytesToWrite
			//个字节的数据将要写入串口的发送数据缓冲区
			m_nToSend,
			//?Length?of?message?to?send,?要写入的数据的字节数?
			&BytesSent,//?Where?to?store?the?number?of?bytes?sent?
			//?指向指向一个DWORD数值，该数值返回实际写入的字节数??
			&m_ov);//?Overlapped?structure?
		//?重叠操作时，该参数指向一个OVERLAPPED结构，?
		//?同步操作时，该参数为NULL?
		if (!bResult)//?当ReadFile和WriteFile返回FALSE时，不一定就是操作失?
			//败，线程应该调用GetLastError函数分析返回的结果?
		{
			DWORD dwError = GetLastError();
			switch (dwError)
			{
			case ERROR_IO_PENDING://GetLastError函数返回?
				//ERROR_IO_PENDING。这说明重叠操作还未完成?
			{
									  //?continue?to?GetOverlappedResults()?
									  BytesSent = 0;
									  bWrite = FALSE;
									  break;
			}
			default:
				break;
			}
		}
	}//?end?if(bWrite)?
	if (!bWrite)
	{
		bWrite = TRUE;
		bResult = GetOverlappedResult(hComm,//?Handle?to?COMM?port????????
			&m_ov,//?Overlapped?structure????????
			&BytesSent,//?Stores?number?of?bytes?sent????????
			TRUE);//?Wait?flag?
		//?deal?with?the?error?code?
		if (!bResult)
		{
			printf("GetOverlappedResults()?in?WriteFile()");
		}
	}//?end?if?(!bWrite)?

	//?Verify?that?the?data?size?send?equals?what?we?tried?to?send?
	if (BytesSent != m_nToSend)
	{
		printf("WARNING:WriteFile() error..Bytes Sent:%d;Message Length:%d\n",
			BytesSent, strlen((char*)m_szWriteBuffer));
	}
	return TRUE;
}
/*************************************
*名称：系统延时函数
*功能：延时
*参数介绍：无
**************************************/
void delay_system(void)
{
	for (BOOL i = 0; i < 100; i++)
	    for (BOOL j = 0; j < 100; j++);


}
/*************************************
*名称：串口初始化函数
*功能：串口初始化（打开默认串口com1，设置波特率默认为9600）
*参数介绍：无
**************************************/
void com_init(void)
{
	bool open;
	open = openport("com9");
	if (open == TRUE)
		printf("open com9 success\n");
	if (setupdcb(9600))
		printf("setupDCB success\n");
	if (setuptimeout(0, 0, 0, 0, 0))
		printf("setuptimeout success\n");
	PurgeComm(hComm, PURGE_RXCLEAR | PURGE_TXCLEAR | PURGE_RXABORT | PURGE_TXABORT);
}

/*************************************
*名称：读倾角函数
*功能：读取倾角仪的角度
*参数介绍：无
**************************************/
bool get_angle(double*angleX,double*angleY)
{
	*angleX = 0.0;
	*angleY = 0.0;
	unsigned char read_angle[5] = { 0x68, 0x04, 0x00, 0x04, 0x08 };//同时读取X和Y的倾角
	BOOL data_count = 14;//读取回来的数据的字节个数
	BOOL bRead = TRUE;
	BOOL bResult = TRUE;
	DWORD dwError = 0;
	DWORD BytesRead = 0;
	BOOL count = 0;
	unsigned char RXBuff;
	unsigned char temp1 = 0;
	unsigned char temp2 = 0;
	unsigned char high_dataX = 0;
	unsigned char low_dataX = 0;
	unsigned char high_dataY = 0;
	unsigned char low_dataY = 0;

	WriteChar(read_angle, 5);
	printf("received data:\n");
	for (;;)
	{
		//68 0D 00 84 10 00 07 10 00 25 00 31 10 1E 
		//68 0D 00 84 10 00 09 10 00 25 00 31 10 20
		bResult = ClearCommError(hComm, &dwError, &comstat);//在使用ReadFile函数进行读操作前，应先使用ClearCommError函数清除错误
		if (comstat.cbInQue == 0)//COMSTAT结构返回串口状态信息//本文只用到了cbInQue成员变量，该成员变量的值代表输入缓冲区的字节数	
			continue;
		if (bRead)
		{
			bResult = ReadFile(hComm, &RXBuff, 1, &BytesRead, &m_ov);
			printf("%x   ", RXBuff);
			//采样X轴的数据
			if (count == 4)
				temp1 = RXBuff;
			if (count == 5)
				high_dataX = RXBuff;
			if (count == 6)
				low_dataX = RXBuff;
			//采样Y轴的数据
			if (count == 7)
				temp2 = RXBuff;
			if (count == 8)
				high_dataY = RXBuff;
			if (count == 9)
				low_dataY = RXBuff;

			//printf("count=%d\n", count);		
			if (++count == data_count)
			{
				printf("\n");
				break;
			}
			if (!bResult)
			{
				switch (dwError = GetLastError())
				{
				case ERROR_IO_PENDING:
				{
					 bRead = FALSE;
					 break;
				}
				default:{break; }
				}
			}
			else
			{
				bRead = TRUE;
			}
		}
		if (!bRead)
		{

			bRead = TRUE;
			bResult = GetOverlappedResult(hComm,//Handle to COMM port
				&m_ov,//Overlapped structure
				&BytesRead,
				TRUE);
		}
	}
	//计算X轴的角度
	*angleX = (high_dataX / 16) * 10 + (high_dataX % 16) * 1 + (low_dataX / 16) * 0.1 + (low_dataX % 16) * 0.01;
	if (temp1 == 0x10)
		*angleX = -*angleX;
	//计算Y轴的角度
	*angleY = (high_dataY / 16) * 10 + (high_dataY % 16) * 1 + (low_dataY / 16) * 0.1 + (low_dataY % 16) * 0.01;
	if (temp2 == 0x10)
		*angleY = -*angleY;

	//	printf("angle=%f\n", angle);
	delay_system();
	return true;
	//return angleX;
}

/*************************************
*名称：零点设置函数
*功能：设置倾角仪的绝对零点
*参数介绍：无
**************************************/
void set_zero(void)
{
	unsigned char SET_ZERO[6] = { 0x68, 0x05, 0x00, 0x05, 0x00, 0x0A };
	BOOL data_count = 6;
	BOOL bRead = TRUE;
	BOOL bResult = TRUE;
	DWORD dwError = 0;
	DWORD BytesRead = 0;
	BOOL count = 0;
	unsigned char RXBuff;
	unsigned char result = 0;

	WriteChar(SET_ZERO, 6);
	printf("received data:\n");
	for (;;)
	{
		bResult = ClearCommError(hComm, &dwError, &comstat);//在使用ReadFile函数进行读操作前，应先使用ClearCommError函数清除错误
		if (comstat.cbInQue == 0)//COMSTAT结构返回串口状态信息//本文只用到了cbInQue成员变量，该成员变量的值代表输入缓冲区的字节数	
			continue;
		if (bRead)
		{
			bResult = ReadFile(hComm, &RXBuff, 1, &BytesRead, &m_ov);
			printf("%x   ", RXBuff);
			if (count == 4)
				result = RXBuff;
			//printf("count=%d\n", count);		
			if (++count == data_count)
			{
				printf("\n");
				break;
			}
			if (!bResult)
			{
				switch (dwError = GetLastError())
				{
				case ERROR_IO_PENDING:
				{
										 bRead = FALSE; break;
				}
				default:{break; }
				}
			}
			else
			{
				bRead = TRUE;
			}
		}
		if (!bRead)
		{

			bRead = TRUE;
			bResult = GetOverlappedResult(hComm,//Handle to COMM port
				&m_ov,//Overlapped structure
				&BytesRead,
				TRUE);
		}
	}
	//	printf("angle=%f\n", angle);
	if (result == 0)
		printf("Set zero successfully!\n");
	else
		printf("Set zero error!\n");
	delay_system();

}