import time
from socket import *
import threading

# server只需要中转所以需要收和发
class MyTd(threading.Thread):
    def __init__(self, target, args, name=''):
        threading.Thread.__init__(self)
        self.name = name
        self.func = target
        self.setDaemon(True)
        self.args = args
        self.result = self.func(*self.args)

    def get_result(self):
        try:
            return self.result
        except Exception:
            return None

class SockBase(object):
    def __init__(self,player="client"):

        Server_info = {
            "IP":"127.0.0.1", #"106.13.10.65","127.0.0.1"
            "PORT": 5089,
            "BUFSIZE": 512,
        }
        client_info = {
            "IP": "127.0.0.1",  # "106.13.10.65",
            "PORT": 666,
            "BUFSIZE": 512,
        }
        self.bufsize=1024
        if player=="server":
            address=(Server_info["IP"],Server_info["PORT"])
            self.Client_Socket=socket(AF_INET,SOCK_STREAM)
            #服务器绑定自身地址
            self.Client_Socket.bind(address)
            self.Client_Socket.listen(3)
        else:
            address = (Server_info["IP"], Server_info["PORT"])
            self.Client_Socket = socket(AF_INET, SOCK_STREAM)
            # self.Client_Socket.bind((client_info["IP"],client_info["PORT"]))
            # 连接服务器地址
            self.Client_Socket.connect(address)
        self.player=player
    #发送线程
    def Send_TD(self,sock,ID=None):
        if self.player == "server":
            TD = threading.Thread(target=self.Send_Loop, args=[sock,ID,])
        else:
            TD = threading.Thread(target=self.Send_Loop, args=[sock, ])
        TD.setDaemon(True)
        TD.start()
        return TD

    #接收线程
    def Recv_TD(self,sock,ID=None):
        if self.player=="server":
            TD= threading.Thread(target=self.Recv_Loop,args=[sock,ID,])
        else:
            TD = threading.Thread(target=self.Recv_Loop, args=[sock, ])
        TD.setDaemon(True)
        TD.start()
        return  TD
    def Recv_Loop(self,sock,ID):
        while True:
            ret=sock.recv(self.bufsize)
            # 没发送就阻塞住了
            print("收到回复:"+str(ret,encoding="utf-8"))
            time.sleep(1)
        sock.close()
    def Send_Loop(self,sock,ID):
        while True:
            ret=sock.send(self.bufsize)
            # 没发送就阻塞住了
            print("收到回复:"+str(ret,encoding="utf-8"))
            time.sleep(1)
        sock.close()
    #用于客户端或者服务端，并非中转站
    #data type bytes
    def send_package(self,sock,data,head="0n".encode()):
        data=head+int(len(data)).to_bytes(4,"little")+data
        print(len(data))
        n=len(data)/self.bufsize if len(data)/self.bufsize==int(len(data)/self.bufsize) else int(len(data)/self.bufsize)+1
        for i in range(n):
            ret = sock.send(data[i*self.bufsize:i*self.bufsize+self.bufsize])
    # return bytes
    def recv_package(self,sock):
        ret = sock.recv(self.bufsize)
        head=ret[:2]
        length=int.from_bytes(ret[2:6],"little")
        res =ret[6:]
        recvlen=length-(len(ret)-6)
        while recvlen>0:
            if recvlen>self.bufsize:
                ret= sock.recv(self.bufsize)
                recvlen-=self.bufsize
            else:
                ret = sock.recv(recvlen)
                recvlen-=recvlen
            res+=ret
        return res

    def SetClientType(self,sock):
        pass