#include <windows.h>
#include <iostream>
#include <process.h> 
#include"winsock.h"
#include<stdio.h>
#include<stdlib.h>
#include<string>
#include "C:\TwinCAT\AdsApi\TcAdsDll\Include\TcAdsDef.h"
#include "C:\TwinCAT\AdsApi\TcAdsDll\Include\TcAdsAPI.h"

#pragma comment(lib, "ws2_32.lib")
#pragma pack(4)
#define MaxSize 1024
using namespace std;
int nNetTimeout = 3000;//3秒
struct status
{
	double bigarm_position;
	double middlearm_position;
	double smallarm_position;
	double swingarm1;
	double swingarm2;
	double swingarm3;
	double swingarm4;
};
unsigned WINAPI HandleClient(void *arg);
void SendMsg(status *msg);
void ErrorHandling(char *msg);
void SendInstruct(int temp);		//发指令号
void SendIndexCtr(int temp);		//发要控制的轴索引
void SendPosition(double temp);		//发位置
void SendVelocity(double temp);		//发普通速度
void SendVelocityX(double temp);	//发X速度
void SendVelocityY(double temp);	//发Y速度
void SendIndexSta(int temp);		//要读取的轴索引
void AdsWrite(int a, int b, double c, double d, double e, double f, double g, int h);	//Ads发送
double pos = 0;
int flag;
int si = 0;
struct command
{
	double carVelX;
	double carVelY;
	double handVelX;
	double handVelY;
	double singleVel;
	int index;
	int instruct1;
	int instruct2;
};


status armStatus;
WSADATA         wsd;            //WSADATA变量  
SOCKET          sServer;        //服务器套接字  
SOCKET          sClient;        //客户端套接字  
SOCKET			clientSocks[256];
SOCKADDR_IN     addrServ;;      //服务器地址  
SOCKADDR_IN     addrClient;;      //客户端地址  
AmsAddr			Addr;
LONG			nErr, nPort;
PAmsAddr		pAddr = &Addr;
int addrClientSz;
int ClientCnt = 0;
int AdsInstruct1;
int AdsInstruct2;
int AdsIndex;
double AdsCarVelx;
double AdsCarVely;
double AdsHandVelx;
double AdsHandVely;
double AdsSingleVel;
BYTE*	pBuffRes = new BYTE[24];
BYTE*	buffers = new BYTE[192];

HANDLE hMutex;
extern OVERLAPPED m_ov;
extern COMSTAT comstat;
extern DWORD m_dwCommEvents;

int main(int argc, char *argv[])
{
	    //Sleep(50000);
		nPort = AdsPortOpen();
		nErr = AdsGetLocalAddress(pAddr);
		pAddr->netId.b[0] = 192;
		pAddr->netId.b[1] = 168;                                                                                     
		pAddr->netId.b[2] = 1;
		pAddr->netId.b[3] = 222;
		cout << "open port:  " << nPort << endl;
		Sleep(500);
	

	HANDLE hThread;

	//if (argc != 2)
	//{
	//	printf("Usage:%s<port>\n", argv[0]);
	//	exit(1);
	//}
	if (WSAStartup(MAKEWORD(2, 2), &wsd) != 0)
		ErrorHandling("WSAStartup() error!");

	hMutex = CreateMutex(NULL, FALSE, NULL);
	sServer = socket(AF_INET, SOCK_STREAM, 0);

	memset(&addrServ, 0, sizeof(addrServ));
	addrServ.sin_addr.S_un.S_addr = htonl(INADDR_ANY);		//htol将主机字节序long型转换为网络字节序
	addrServ.sin_family = AF_INET;
	addrServ.sin_port = htons(4999);						//htos用来将端口转换成字符，1024以上的数字即可
	//printf("Welcome,the Host %s is running!Now Wating for someone comes in!\n", inet_ntoa(addrServ.sin_addr));
	setsockopt(sServer, SOL_SOCKET, SO_RCVTIMEO, (char *)&nNetTimeout, sizeof(int));
	if (bind(sServer, (SOCKADDR*)&addrServ, sizeof(addrServ)) == SOCKET_ERROR)
		ErrorHandling("bind() error");

	if (listen(sServer, 5) == SOCKET_ERROR)
		ErrorHandling("listen() error");

	while (1)
	{
		addrClientSz = sizeof(addrClient);
		sClient = accept(sServer, (SOCKADDR*)&addrClient, &addrClientSz);
		WaitForSingleObject(hMutex, INFINITE);
		clientSocks[ClientCnt++] = sClient;
		ReleaseMutex(hMutex);

		hThread = (HANDLE)_beginthreadex(NULL, 0, HandleClient, (void*)&sClient, 0, NULL);
		printf("Connected client IP:%s\n", inet_ntoa(addrClient.sin_addr));
	}
	closesocket(sServer);
	WSACleanup();
	return 0;
}

unsigned WINAPI HandleClient(void *arg)
{
	SOCKET sClient = *((SOCKET*)arg);
	int strlen = 0, i;
	command comd;
	while (1)
	{
		int len = recv(sClient, (char*)(&comd), sizeof(command), 0);
		if (len > 0)
		{
			AdsCarVelx = comd.carVelX;
			AdsCarVely = comd.carVelY;
			AdsHandVelx = comd.handVelX;
			AdsHandVely = comd.handVelY;
			AdsSingleVel = comd.singleVel;
			AdsInstruct1 = comd.instruct1;
			AdsInstruct2 = comd.instruct2;
			AdsIndex = comd.index;
			//cout <<comd.instruct1<<endl;
			AdsWrite(AdsInstruct1, AdsInstruct2, AdsCarVelx, AdsCarVely, AdsHandVelx, AdsHandVely, AdsSingleVel, AdsIndex);
			send(sClient, (char*)(&armStatus), sizeof(status), 0);
		}
		else
		{
			break;
		}
	}
	WaitForSingleObject(hMutex, INFINITE);
	for (int i = 0; i < ClientCnt; i++)
	{
		if (sClient == clientSocks[i])
		{
			while (i++ < ClientCnt - 1)
				clientSocks[i] = clientSocks[i + 1];
			break;
		}
	}
	ClientCnt--;
	ReleaseMutex(hMutex);
	closesocket(sClient);
	printf("Connect closed!");
	SendInstruct(70);
	Sleep(10);
	return 0;
}

void SendMsg(status *msg)
{
	int i;
	int len = sizeof(status);
	WaitForSingleObject(hMutex, INFINITE);
	for (i = 0; i < ClientCnt; i++)
		send(clientSocks[i], (char*)(&msg), len, 0);
	ReleaseMutex(hMutex);
}

void ErrorHandling(char *msg)
{
	fputs(msg, stderr);
	fputc('\n', stderr);
	exit(1);
}

void SendInstruct(int temp)		//发指令号
{
	pAddr->port = 0xbf02;
	nErr = AdsSyncReadWriteReq(
		pAddr,
		0x02,			// ADS list-read-write command
		0x01,			// number of ADS-sub commands
		0x18,			// we expect an ADS-error-return-code for each ADS-sub command
		pBuffRes,		// provide space for the response containing the return codes
		sizeof(int),			// cbReq : send 48 bytes (IG1, IO1, RLen1, WLen1,
		&temp);		// buffer with data
}

void SendIndexCtr(int temp)		//发要控制的轴索引
{
	pAddr->port = 0xbf02;
	nErr = AdsSyncReadWriteReq(
		pAddr,
		0x02,			// ADS list-read-write command
		0x02,			// number of ADS-sub commands
		0x18,			// we expect an ADS-error-return-code for each ADS-sub command
		pBuffRes,		// provide space for the response containing the return codes
		sizeof(int),			// cbReq : send 48 bytes (IG1, IO1, RLen1, WLen1,
		&temp);		// buffer with data
}
void SendPosition(double temp)	//发位置
{
	pAddr->port = 0xbf02;
	nErr = AdsSyncReadWriteReq(
		pAddr,
		0x02,			// ADS list-read-write command
		0x03,			// number of ADS-sub commands
		0x18,			// we expect an ADS-error-return-code for each ADS-sub command
		pBuffRes,		// provide space for the response containing the return codes
		sizeof(double),			// cbReq : send 48 bytes (IG1, IO1, RLen1, WLen1,
		&temp);		// buffer with data
}
void SendVelocity(double temp)	//发普通速度
{
	pAddr->port = 0xbf02;
	nErr = AdsSyncReadWriteReq(
		pAddr,
		0x02,			// ADS list-read-write command
		0x04,			// number of ADS-sub commands
		0x18,			// we expect an ADS-error-return-code for each ADS-sub command
		pBuffRes,		// provide space for the response containing the return codes
		sizeof(double),			// cbReq : send 48 bytes (IG1, IO1, RLen1, WLen1,
		&temp);		// buffer with data
}
void SendVelocityX(double temp)	//发X速度
{
	pAddr->port = 0xbf02;
	nErr = AdsSyncReadWriteReq(
		pAddr,
		0x02,			// ADS list-read-write command
		0x05,			// number of ADS-sub commands
		0x18,			// we expect an ADS-error-return-code for each ADS-sub command
		pBuffRes,		// provide space for the response containing the return codes
		sizeof(double),			// cbReq : send 48 bytes (IG1, IO1, RLen1, WLen1,
		&temp);		// buffer with data
}
void SendVelocityY(double temp)	//发Y速度
{
	pAddr->port = 0xbf02;
	nErr = AdsSyncReadWriteReq(
		pAddr,
		0x02,			// ADS list-read-write command
		0x06,			// number of ADS-sub commands
		0x18,			// we expect an ADS-error-return-code for each ADS-sub command
		pBuffRes,		// provide space for the response containing the return codes
		sizeof(double),			// cbReq : send 48 bytes (IG1, IO1, RLen1, WLen1,
		&temp);		// buffer with data
}
void SendHandVelocityX(double temp)	//发Y速度
{
	pAddr->port = 0xbf02;
	nErr = AdsSyncReadWriteReq(
		pAddr,
		0x02,			// ADS list-read-write command
		0x07,			// number of ADS-sub commands
		0x18,			// we expect an ADS-error-return-code for each ADS-sub command
		pBuffRes,		// provide space for the response containing the return codes
		sizeof(double),			// cbReq : send 48 bytes (IG1, IO1, RLen1, WLen1,
		&temp);		// buffer with data
}
void SendHandVelocityY(double temp)	//发Y速度
{
	pAddr->port = 0xbf02;
	nErr = AdsSyncReadWriteReq(
		pAddr,
		0x02,			// ADS list-read-write command
		0x08,			// number of ADS-sub commands
		0x18,			// we expect an ADS-error-return-code for each ADS-sub command
		pBuffRes,		// provide space for the response containing the return codes
		sizeof(double),			// cbReq : send 48 bytes (IG1, IO1, RLen1, WLen1,
		&temp);		// buffer with data
}
void SendIndexSta(int temp)		//要读取的轴索引
{
	pAddr->port = 0xbf02;
	nErr = AdsSyncReadWriteReq(
		pAddr,
		0x02,			// ADS list-read-write command
		0x09,			// number of ADS-sub commands
		0x18,			// we expect an ADS-error-return-code for each ADS-sub command
		pBuffRes,		// provide space for the response containing the return codes
		sizeof(int),			// cbReq : send 48 bytes (IG1, IO1, RLen1, WLen1,
		&temp);		// buffer with data
}
double ReadBigarm()
{
	pAddr->port = 0xbf02;
	//传送Z方向倾角
	nErr = AdsSyncReadWriteReq(
		pAddr,
		0x01,			// ADS list-read-write command
		0x07,			// number of ADS-sub commands
		0x20,			// we expect an ADS-error-return-code for each ADS-sub command
		buffers,		// provide space for the response containing the return codes
		sizeof(double),			// cbReq : send 48 bytes (IG1, IO1, RLen1, WLen1,
		&pos);		// buffer with data
	double* temp = ((double*)buffers);
	return *temp;
}

double ReadMiddlearm()
{
	pAddr->port = 0xbf02;
	//传送Z方向倾角
	nErr = AdsSyncReadWriteReq(
		pAddr,
		0x01,			// ADS list-read-write command
		0x08,			// number of ADS-sub commands
		0x20,			// we expect an ADS-error-return-code for each ADS-sub command
		buffers,		// provide space for the response containing the return codes
		sizeof(double),			// cbReq : send 48 bytes (IG1, IO1, RLen1, WLen1,
		&pos);		// buffer with data
	double* temp = ((double*)buffers);
	return *temp;
}

double ReadSmallarm()
{
	pAddr->port = 0xbf02;
	//传送Z方向倾角
	nErr = AdsSyncReadWriteReq(
		pAddr,
		0x01,			// ADS list-read-write command
		0x09,			// number of ADS-sub commands
		0x20,			// we expect an ADS-error-return-code for each ADS-sub command
		buffers,		// provide space for the response containing the return codes
		sizeof(double),			// cbReq : send 48 bytes (IG1, IO1, RLen1, WLen1,
		&pos);		// buffer with data
	double* temp = ((double*)buffers);
	return *temp;
}

double ReadSwingArm()
{
	pAddr->port = 0xbf02;
	//传送Z方向倾角
	nErr = AdsSyncReadWriteReq(
		pAddr,
		0x01,			// ADS list-read-write command
		0x03,			// number of ADS-sub commands
		0x20,			// we expect an ADS-error-return-code for each ADS-sub command
		buffers,		// provide space for the response containing the return codes
		sizeof(double),			// cbReq : send 48 bytes (IG1, IO1, RLen1, WLen1,
		&pos);		// buffer with data
	double* temp = ((double*)buffers);
	return *temp;
}
void AdsWrite(int a, int b, double c, double d, double e, double f, double g,int h)
{
	switch (a)
	{
	case 1:			//上使能
	{
						SendInstruct(20);
						Sleep(10);
						SendInstruct(30);
						Sleep(10);
	}
		break;
	case 2:			//暂停
	{	
		if (b == 5)//车模式
	{
		break;
	}
	else
	{
		SendInstruct(70);
		Sleep(10);
		SendIndexCtr(14);
		Sleep(10);
		SendVelocity(0);
		Sleep(10);
		SendInstruct(130);
		Sleep(10);
	}
	}
		break;
	case 3:			//复位
	{
						SendInstruct(60);
						Sleep(10);
	}
		break;
	case 4:			//车模式
	{
						SendVelocityX(c);
						Sleep(10);
						SendVelocityY(d);
						Sleep(10);
						SendInstruct(90);
						Sleep(10);
	}
		break;
	case 6:			//机械臂寻参
	{
						SendInstruct(80);
						Sleep(10);
	}
		break;
	case 7:			//机械臂归位
	{
						SendInstruct(290);
						Sleep(10);
	}
	case 8:			//双摆运动
	{
						if (h == 15)//前双摆
						{
							SendVelocity(g);
							Sleep(10);
							SendInstruct(260);
							Sleep(10);

						}
						else if(h==16)//后双摆
						{
							SendVelocity(g);
							Sleep(10);
							SendInstruct(270);
							Sleep(10);
						}
	}
		break;
	case 9:			//手抓张合
	{
						SendVelocity(g);
						Sleep(5);
						SendIndexCtr(h);
						Sleep(5);
						SendInstruct(130);
						Sleep(5);
	}
		break;
	case 10://置零
	{
			   SendInstruct(300);
			   Sleep(10);
	}
		break;
	case 12://归位
	{
				SendInstruct(320);
				Sleep(10);
	}
		break;
	case 13://就绪(新)
	{
			   SendInstruct(330);
			   Sleep(10);
	}
		break;
	case 14://抓取位置
	{
				SendInstruct(340);
				Sleep(10);
	}
		break;
	case 15://放置准备
	{
				SendInstruct(350);
				Sleep(10);
	}
		break;
	case 16://盒子1
	{
				SendInstruct(360);
				Sleep(10);
	}
		break;
	case 17://盒子2
	{
				SendInstruct(370);
				Sleep(10);
	}
		break;
	case 18://盒子3
	{
				SendInstruct(380);
				Sleep(10);
	}
		break;
	case 19://盒子4
	{
				SendInstruct(390);
				Sleep(10);
	}
		break; 
	case 20://开灯
	{
				if (si == 0)
				{
					SendInstruct(400);
					Sleep(10);
					si = 1;
				}
	}
		break;
	case 21://关灯
	{
				if (si == 1)
				{
					SendInstruct(410);
					Sleep(10);
					si = 0;
				}
	}
		break;
	case 22:
	{
			   SendVelocity(g);
			   Sleep(5);
			   SendIndexCtr(h);
			   Sleep(5);
			   if(h>=4&&h<8)
					SendInstruct(120);
			   else
				   SendInstruct(130);
			   Sleep(5);
	}
		break;
	case 99:		//读取状态
	{
						armStatus.bigarm_position = ReadBigarm();
						Sleep(10);
						armStatus.middlearm_position = ReadMiddlearm();
						Sleep(10);
						armStatus.smallarm_position = ReadSmallarm();
						Sleep(10);
						SendIndexSta(4);
						Sleep(10);
						armStatus.swingarm1 = ReadSwingArm();
						Sleep(10);
						SendIndexSta(5);
						Sleep(10);
						armStatus.swingarm2 = ReadSwingArm();
						Sleep(10);
						SendIndexSta(6);
						Sleep(10);
						armStatus.swingarm3 = ReadSwingArm();
						Sleep(10);
						SendIndexSta(7);
						Sleep(10);
						armStatus.swingarm4 = ReadSwingArm();
						Sleep(10);
	}
		break;
	default:
		break;
	}
	if (b == 5)//手模式
	{
		SendHandVelocityX(e);
		Sleep(10);
		SendHandVelocityY(f);
		Sleep(10);
		SendInstruct(100);
		Sleep(10);
	}
}