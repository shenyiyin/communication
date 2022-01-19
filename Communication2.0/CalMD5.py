import hashlib
def CalMD5(filename=r'C:\Users\DELL\Desktop\UVC 1.5 Class specification.pdf'):
    dig = hashlib.md5()
    with open(filename, 'rb') as file_delect:
        for data in iter(lambda  : file_delect.read(1024),b''):
            dig.update(data)
    return dig.hexdigest()