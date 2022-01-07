import os

def create_dir(path):
    if not os.path.exists(path):
        os.mkdir(path)
    return os.getcwd()+"\\"+path

def fileTobytes(filename):
    f=open(filename,"rb")
    data=f.read()
    f.close()
    return data


def bytesTofile(filename,data):
    f=open(create_dir("recv_file")+"\\"+filename,"wb")
    f.write(data)
    f.close()