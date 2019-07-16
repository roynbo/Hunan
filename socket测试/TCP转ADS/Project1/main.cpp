// winsocketTCPServer.cpp : 定义控制台应用程序的入口点。
//

#define _WINSOCK_DEPRECATED_NO_WARNINGS

//服务器  
#include<iostream>  
#include<WinSock2.h>                    // socket 所需要的头文件  
#pragma comment(lib,"WS2_32.lib")// link socket 库  
#define PORT 4999   
#define BUFLEN 1024  
using namespace std;

int nNetTimeout = 1000;//1秒
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

int main()
{
	status robot_status;
	command robot_command;
	memset(&robot_status, 0, sizeof(status));
	memset(&robot_command, 0, sizeof(command));
	HANDLE hThread;
	WSADATA wsaData;
	// 1 启动并初始化winsock(WSAStarup)  
	if (WSAStartup(MAKEWORD(2, 2), &wsaData))//成功返回0  
	{
		return FALSE;
	}
	//2 创建套接字(socket)  
	SOCKET sServer = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (INVALID_SOCKET == sServer)
	{
		WSACleanup();
		return FALSE;
	}
	//3 准备通信地址  
	SOCKADDR_IN addrServer;
	addrServer.sin_family = AF_INET;
	addrServer.sin_port = htons(PORT);
	addrServer.sin_addr.s_addr = INADDR_ANY;//任意可用地址  
											//4 绑定地址与socket（bind）  
	if (SOCKET_ERROR == bind(sServer, (const sockaddr*)&addrServer, sizeof(SOCKADDR_IN)))
	{
		closesocket(sServer);
		WSACleanup();

		return FALSE;
	}
	//5 监听 （listen)  
	if (SOCKET_ERROR == listen(sServer, SOMAXCONN))
	{
		closesocket(sServer);
		WSACleanup();
	}
	// 6 等待客户端连接（accpet）  
	sockaddr_in addrClient;
	int addrClientLen = sizeof(addrClient);
	cout << "Server Start Complete!Waiting For Client" << endl;
	SOCKET sClient = accept(sServer, (sockaddr *)&addrClient, &addrClientLen);
	if (INVALID_SOCKET == sClient)
	{
		cout << WSAGetLastError() << endl;
		closesocket(sServer);
		closesocket(sClient);
		WSACleanup();
		return FALSE;
	}
	printf("Connected client IP:%s\n", inet_ntoa(addrClient.sin_addr));
	while (1)
	{
		//接收时限
		int flag = setsockopt(sServer, SOL_SOCKET, SO_RCVTIMEO, (char *)&nNetTimeout, sizeof(int));
		//接收数据
		int len=recv(sClient, (char*)(&robot_command), sizeof(command), 0);
		if (len < 0)
			break;
		if (flag)
		{
			cout << "recv time out!" << endl;
			break;
		}
		cout << robot_command.instruct1;
		// 发送数据      
		send(sClient, (char*)(&robot_status), sizeof(status), 0);
	}
	printf("Connect closed!\n");
	system("pause");
	return TRUE;
}
/*
注：1：MAKEWORD把参数组成一个WORD（双字节）类型数据并返回这个WORD类型数值，高位代表(修订本)号,低位字节指定主版本号（其代表）
2：socket(AF_INET,//The Internet Protocol version 4 (IPv4) address family
SOCK_STREAM,//, two-way,This socket type uses the Transmission Control Protocol (TCP) for the Internet address family (AF_INET or AF_INET6).
IPPROTO_TCP//The Transmission Control Protocol (TCP). This is a possible value when the af parameter is AF_INET or AF_INET6 and the type parameter is SOCK_STREAM.
)
*/