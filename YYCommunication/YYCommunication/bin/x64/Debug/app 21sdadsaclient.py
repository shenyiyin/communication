import json,cv2,numpy
from socket import *
from matplotlib import pyplot as plt
import time,threading
from PIL import Image
import numpy as np
from Sock_Base import SockBase

#枷锁
lock=threading.RLock()

# 只需要发
class Client(SockBase):
    def __init__(self):
        super().__init__(player="client")
        self.master_cmd = ""
        self.slave_info = ""
        self.Nimage = 0
        self.imagew = 0
        self.imageh = 0
        self.Imagesize=240*320
        self.bufsize=1024
        self.see=False
        self.DataList = bytes(0)
    def Run(self):

        #
        # # 保存套接字和线程
        # socks = []
        # tds = []
        # ID = 0
        # while True:
        #     print('waiting for connection...')
        #     tcpCliSock, addr = self.Client_Socket.accept()
        #     socks.append(tcpCliSock)
        #     print('...connnecting from:', addr)
        #     # 开2个线程一个收一个发
        #     self.Recv_TD(tcpCliSock, ID)
        #     self.Send_TD(tcpCliSock, ID)
        #     td = threading.Thread(target=self.ShowImg, args=[])
        #     td.start()
        #     td.join()
        #     ID += 1
        #服务器版本
        # user = "master"
        # self.Client_Socket.send(user.encode())
        #
        # ret = self.Client_Socket.recv(self.bufsize)
        # if ret != "ok".encode():
        #     return
        # print(user + "通讯成功")
        RTD = self.Recv_TD(self.Client_Socket)
        STD = self.Send_TD(self.Client_Socket)
        td = threading.Thread(target=self.ShowImg, args=[])
        td.start()
        td.join()
        RTD.join()
        STD.join()

    def Recv_Loop(self, sock,ID):
        while True:
            cmd =self.Client_Socket.recv(self.bufsize)
            print(str(cmd,encoding="utf-8"))
            # 没发送就阻塞住了
            self.ImageFunc(cmd,sock)
            self.AddressFunc(cmd,sock)

            # time.sleep(1)
        sock.close()

    def Send_Loop(self, sock,ID):
        iscmd = False
        while True:
            # lock.acquire()
            cmd = input("command:")
            if cmd.lower()=="stop":
                self.see=False
            # lock.release()
            if cmd == "0":
                iscmd = True
            elif cmd == "1":
                iscmd = False
            if iscmd:
                cmd = "cmd:" + cmd
            self.Client_Socket.send(cmd.encode())
            # 没发送就阻塞住了
            # data1 = self.Client_Socket.recv(client_info["BUFSIZE"])
            print("client2 running ")
            # time.sleep(1)
        sock.close()

    # RecvFunc
    def ImageFunc(self,cmd,sock):
        TempImage = bytes(0)
        if cmd and cmd.startswith("image".encode()):
            self.see=True
            while self.see:

                w = self.Client_Socket.recv(2)
                h = self.Client_Socket.recv(2)
                self.imagew = int.from_bytes(w, byteorder='big', signed='False')
                self.imageh = int.from_bytes(h, byteorder='big', signed='False')
                while True:
                    temp = self.Client_Socket.recv(self.Imagesize)
                    try:
                        # 结束传图一张
                        if "end".encode() in temp:
                            TempImage += temp[:-3]
                            self.DataList = TempImage[:]
                            self.Nimage += 1
                    except:
                        pass
                    TempImage += temp
    def AddressFunc(self,cmd,sock):
        if cmd and cmd.startswith("addre".encode()):
            data=""
            while True:
                temp = self.Client_Socket.recv(self.bufsize)
                try:
                    # 结束传图一张
                    if "end".encode() in temp:
                        data+=str(temp, encoding="utf-8")[:-3]
                        break
                    data += str(temp, encoding="utf-8")
                except:
                    pass


    def ShowImg(self):
        temp = self.Nimage
        while True:
            # frame=cv2.cvtColor(frame,cv2.COLOR_RGB2YUV)
            # YUVtoY(frame)
            try:
                if self.Nimage > temp:
                    img = Image.frombuffer("L", (self.imagew, self.imageh), self.DataList)
                    # img = cv2.cvtColor(np.asarray(im), cv2.COLOR_RGB2BGR)

                    img = cv2.cvtColor(numpy.asarray(img), cv2.COLOR_RGB2BGR)
                    cv2.imshow("Video1", img)
                    temp = self.Nimage
            except:
                temp = self.Nimage
                # 读取内容
            if cv2.waitKey(10) == ord("q"):
                break




if __name__=="__main__":
    C=Client()
    C.Run()