import time
from socket import *
from FileTransferTransform.FileToBytes import *
import threading
from Sock_Base import SockBase
from ClientAttribute import ClientInfo
Server_info={
    "IP":"127.0.0.1",
    "PORT":5089,
    "BUFSIZE":1024,
            }
# server只需要中转所以需要收和发

class Server(SockBase):
    def __init__(self):
        super().__init__(player="server")
        self.master_cmd=""
        self.slave_info=""
        self.socketmap={}
        self.ClientCmd={"flash":lambda sock,ID:self.send_existSock(sock,ID)}
        self.locksmap={"MTOS":threading.Lock(),"STOM":threading.Lock()}
        self.isChatting=[]
        self.connect_clients= {}
    #主线程
    def Run(self):
        ID=0
        while True:
            print('waiting for connection...')
            tcpCliSock, addr = self.Client_Socket.accept()
            print('...connnecting from:', addr)
            C=ClientInfo(ID=(ID),addr=addr,Sock=tcpCliSock)
            #开2个线程一个收一个发
            self.connect_clients[ID] = C
            C.recvTD=self.Recv_TD(tcpCliSock,ID)
            C.sendTD=self.Send_TD(tcpCliSock,ID)
            ID+=1

    # 接收线程 收到的发给对方
    def Recv_Loop(self, sock, ID):
        try:
            while True:
                ret = sock.recv(self.bufsize)
                #已连接
                if self.connect_clients[ID].ConnectID != None:
                    self.connect_clients[self.connect_clients[ID].ConnectID].sendMessage=str(ret, encoding="utf-8")
                    print(self.connect_clients[self.connect_clients[ID].ConnectID].sendMessage)
                    continue

                #连接


                if str(ret, encoding="utf-8")[:4]=="conn":
                    if self.connect_clients[ID].ConnectID!=None:
                        sock.send("目标正在通话中".encode())
                    else:
                        # 强制被动连接
                        self.connect_clients[ID].ConnectID=int(str(ret, encoding="utf-8")[4:])
                        self.connect_clients[int(str(ret, encoding="utf-8")[4:])].ConnectID=ID
                        self.isChatting.extend([ID,int(str(ret, encoding="utf-8")[4:])])
                        sock.send("已连接".encode())
                elif str(ret, encoding="utf-8")[:5]=="file:":
                    self.Recv_File(str(ret, encoding="utf-8")[5:],sock)
                    continue


                try:
                    print(str(ret, encoding="utf-8"))
                    f=self.ClientCmd[str(ret, encoding="utf-8")]
                    f(sock,ID)
                except:
                    pass
                time.sleep(1)
        except:
            del self.connect_clients[ID]
            del self.connect_clients[self.connect_clients[ID].ConnectID]
        sock.close()
    #发送线程
    def Send_Loop(self, sock, ID):
        try:
            while True:
                if self.connect_clients[ID].sendMessage!="":
                    ret = sock.send(self.connect_clients[ID].sendMessage.encode())
                    self.connect_clients[ID].sendMessage=""
                # 没发送就阻塞住了
                time.sleep(1)
        except:
            del self.connect_clients[ID]
            del self.connect_clients[self.connect_clients[ID].ConnectID]
        sock.close()


    def send_existSock(self,sock,ID):
        print(123)
        cmd="请输入你需要连接的对象:"+",".join([str(i) for i in self.connect_clients.keys() if i!=ID and i not in self.isChatting])
        print("发送了"+cmd)
        ret=sock.send(cmd.encode())


    def Recv_File(self,fileName,sock):
        data=bytes(0)
        while True:
            ret=sock.recv(self.bufsize)
            if "yyend".encode() in ret:
                data+=ret[:-5]
                break

            data+=ret
        bytesTofile(fileName,data)
        print("finish")

    def Reconnect(self):
        pass






if __name__=="__main__":
    C=Server()
    C.Run()