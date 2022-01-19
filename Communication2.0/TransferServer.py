from Sock_Base import SockBase
import time
from socket import *
from FileTransferTransform.FileToBytes import *
import threading
from Sock_Base import SockBase
from ClientAttribute import ClientInfo
from CommandExcuor import *
Server_info = {
    "IP": "127.0.0.1",
    "PORT": 5089,
    "BUFSIZE": 1024,
}


class TransferStaion(SockBase):
    def __init__(self):
        super().__init__(player="server")
        self.master_cmd = ""
        self.slave_info = ""
        self.socketmap = {}
        self.ClientCmd = {"flash": lambda sock, ID: self.send_existSock(sock, ID)}
        self.locksmap = {"MTOS": threading.Lock(), "STOM": threading.Lock()}
        self.isChatting = []
        self.connect_clients = {}

    # 主线程
    def Run(self):
        ID = 0
        while True:
            print('waiting for connection...')
            tcpCliSock, addr = self.Client_Socket.accept()
            print('...connnecting from:', addr)
            C = ClientInfo(ID=(ID), addr=addr, Sock=tcpCliSock)

            for each in self.connect_clients:
                if C.addr == self.connect_clients[each].addr:
                    each.killThread = True

            # 开2个线程一个收一个发
            self.connect_clients[ID] = C
            C.recvTD = self.Recv_TD(tcpCliSock, ID)
            C.sendTD = self.Send_TD(tcpCliSock, ID)
            ID += 1

    def Recv_Loop(self, sock, ID):
        try:
            while True:
                if self.connect_clients[ID].killThread:
                    raise "killThread error"

                ret = self.recv_package(sock)
                # 已连接client
                if self.connect_clients[ID].ConnectID != None:
                    self.connect_clients[self.connect_clients[ID].ConnectID].sendMessage = ret
                    continue

                self.As_Severfunc(sock, ret, ID)

        except:
            if ID in self.connect_clients:
                del self.connect_clients[ID]
        sock.close()

        # 发送线程

    def Send_Loop(self, sock, ID):
        try:
            while True:
                if self.connect_clients[ID].killThread:
                    raise "killThread error"
                if self.connect_clients[ID].sendMessage != "" and self.connect_clients[ID].isConnect:
                    # ret = sock.send(self.connect_clients[ID].sendMessage.encode())
                    self.send_package(sock, self.connect_clients[ID].sendMessage)
                    self.connect_clients[ID].sendMessage = ""
                # 没发送就阻塞住了
                time.sleep(1)
        except:
            if ID in self.connect_clients:
                del self.connect_clients[ID]
        sock.close()

    def send_existSock(self, sock, ID):
        cmd = "请输入你需要连接的对象:" + ",".join(
            [str(i) for i in self.connect_clients.keys() if i != ID and i not in self.isChatting])
        print("发送了" + cmd)
        self.send_package(sock, cmd.encode())

    def Recv_File(self, fileName, sock):
        data = self.recv_package(sock)
        bytesTofile(fileName, data)

        self.send_package(sock, "finish".encode())

    def As_Severfunc(self, sock, ret, ID):
        # 连接
        if str(ret, encoding="utf-8")[:4] == "conn":  # conn0
            if self.connect_clients[ID].ConnectID != None:
                self.send_package(sock, "目标正在通话中".encode())
            else:
                # 强制被动连接
                # print(int(str(ret[4:], encoding="utf-8")))
                self.connect_clients[ID].ConnectID = int(str(ret[4:], encoding="utf-8").strip())
                if self.connect_clients[self.connect_clients[ID].ConnectID].ConnectID == ID:
                    self.connect_clients[ID].isConnect = True
                    self.send_package(sock, "连接成功！".encode())
                    return

                self.send_package(self.connect_clients[self.connect_clients[ID].ConnectID].Sock,
                                  ("connfrom:" + str(ID)).encode())
                t1 = time.time()
                while self.connect_clients[self.connect_clients[ID].ConnectID].ConnectID != ID:
                    self.connect_clients[ID].isConnect = True
                    if (time.time() - t1 > 30):
                        print("connect fail")
                        raise "killThread error"
                    print("等待响应中")
                print("connect ok")


                self.send_package(sock, "连接成功!".encode())
        # 与服务器通信
        elif str(ret, encoding="utf-8")[:5] == "file:" and self.connect_clients[ID].ConnectID == None:
            self.send_package(sock, "ok".encode())
            self.Recv_File(str(ret, encoding="utf-8")[5:], sock)
            return
        # server com
        elif str(ret, encoding="utf-8")[:2] == "py":
            self.tran_py(sock)

        try:
            if self.connect_clients[ID].ConnectID == None:
                # print(str(ret, encoding="utf-8"))
                f = self.ClientCmd[str(ret, encoding="utf-8")]
                f(sock, ID)
        except:
            pass
        time.sleep(1)

    def tran_py(self, sock):
        f = open("test.py", "rb")
        data = f.read()
        f.close()
        print(data)
        self.send_package(sock, data)


if __name__ == "__main__":
    t = TransferStaion()
    t.Run()

