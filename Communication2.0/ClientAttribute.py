

class ClientInfo(object):
    def __init__(self,ID,Name=None,thread=None,addr=None,Sock=None):
        self.ID=ID
        self.ConnectID = None
        self.Name=Name
        self.thread=thread
        self.addr = addr
        self.Sock = Sock
        self.recvTd=None
        self.sendTd=None
        self.sendMessage=""
        self.savepath="recv_file/"
        self.isConnect=False
        self.killThread=False
    def change(self,ID=None,Name=None,thread=None):
        if ID is not None:
            self.ID=ID
        if Name is not None:
            self.Name=Name
        if thread is not None:
            self.thread = thread
