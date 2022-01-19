using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
namespace YYCommunication
{
    class Client
    {
        private IPAddress serverIP;
        private IPEndPoint serverFullAddr;
        public Socket sock=null;
        public bool isConnect=false;
        public bool bekill=false;
        public bool isConnectCient = false;
        public int connectID=-1;
        public int connectingID=-1;
        public delegate void Func();
        public int bufsize = 10240;

        public string RECVDATA, SENDDATA;
        public void connect()
        {
            try {
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverIP = IPAddress.Parse("106.13.10.65");//106.13.10.65
                serverFullAddr = new IPEndPoint(serverIP, 5089);
                sock.Connect(serverFullAddr);
                MessageBox.Show("服务器已连接!");
            }
            catch {
                MessageBox.Show("连接失败!");
            }
        }
        public  bool send_package(string data,byte[] head)
        {
            byte[] dataArr = System.Text.Encoding.UTF8.GetBytes(data);
            byte[] lenArr = BitConverter.GetBytes(System.Text.Encoding.UTF8.GetBytes(data).Length);
            byte[] package = new byte[head.Length + lenArr.Length + dataArr.Length];

            head.CopyTo(package, 0);
            lenArr.CopyTo(package, head.Length);
            dataArr.CopyTo(package, head.Length + lenArr.Length);
            if (sock == null)
            {
                MessageBox.Show("服务器未连接!");
                return false;
            }
            int ret=sock.Send(package);
            if (ret== package.Length)
                return true;
            return false;
        }

        public  bool send_package(byte[] data, byte[] head)
        {
            int i,ret;
            byte[] dataArr = data;
            byte[] lenArr = BitConverter.GetBytes(data.Length);
            byte[] package = new byte[head.Length + lenArr.Length + dataArr.Length];

            head.CopyTo(package, 0);
            lenArr.CopyTo(package, head.Length);
            dataArr.CopyTo(package, head.Length + lenArr.Length);
            if (sock == null)
            {
                MessageBox.Show("服务器未连接!");
                return false;
            }
            

            for (i = 0; i < package.Length / bufsize; i++)
            {
                byte[] data1 = new byte[bufsize];
                Array.Copy(package, i * bufsize, data1, 0, bufsize);
                ret = sock.Send(data1);
                if (ret != data1.Length)
                    return false;
                Thread.Sleep(10);
            }


            if (package.Length != i * bufsize)
            {
                byte[] data1 = new byte[package.Length- i * bufsize];
                Array.Copy(package, i * bufsize, data1, 0, package.Length - i * bufsize);
                ret=sock.Send(data1);
                if (ret != data1.Length)
                    return false;
            }

            
            return true;
        }
        public  string recv_package()
        {
            byte[] recvBuffer=new byte[bufsize];
            int dataLen=0,ret = sock.Receive(recvBuffer);
            if(ret==0)return null;
            byte[] dataArr = recvBuffer.Skip(6).Take(recvBuffer.Length-6).ToArray();
            byte[] lenArr = recvBuffer.Skip(2).Take(4).ToArray();
            byte[] head= recvBuffer.Take(2).ToArray();
            int len = System.BitConverter.ToInt32(lenArr, 0);
            if (len<0) len = System.BitConverter.ToInt16(lenArr, 0);
            byte[] package = new byte[len];
            dataLen=dataArr.Length;
            if (len<bufsize)
            {
                recvBuffer = new byte[len];
                dataLen = len;
            }
                
            recvBuffer= dataArr.Take(dataLen).ToArray();
            recvBuffer.CopyTo((byte[])package, 0);
            int lastLen = dataArr.Length;
            len -= dataArr.Length;
            while (len > 0)
            {
                if (len - bufsize > bufsize)
                {
                    recvBuffer = new byte[bufsize];
                    ret = sock.Receive(recvBuffer);
                    len-= bufsize;
                }
                else
                {
                    recvBuffer = new byte[len];
                    ret = sock.Receive(recvBuffer);
                    len-=(len );
                }
                recvBuffer.CopyTo((byte[])package, lastLen);
                lastLen+=recvBuffer.Length;
                if (ret == 0) return null;

            }
            return System.Text.Encoding.UTF8.GetString(package).TrimEnd('\0');
        }

        public byte[] recv_bytepackage()
        {
            byte[] recvBuffer = new byte[bufsize];
            int dataLen = 0, ret = sock.Receive(recvBuffer);
            if (ret == 0) return null;
            byte[] dataArr = recvBuffer.Skip(6).Take(recvBuffer.Length - 6).ToArray();
            byte[] lenArr = recvBuffer.Skip(2).Take(4).ToArray();
            byte[] head = recvBuffer.Take(2).ToArray();
            int len = System.BitConverter.ToInt32(lenArr, 0);
            byte[] package = new byte[len];
            dataLen = dataArr.Length;
            if (len < bufsize)
            {
                recvBuffer = new byte[len];
                dataLen = len;
            }

            recvBuffer = dataArr.Take(dataLen).ToArray();
            recvBuffer.CopyTo((byte[])package, 0);
            int lastLen = dataArr.Length;
            len -= dataArr.Length;
            while (len > 0)
            {
                if (len - bufsize > 0)
                {
                    recvBuffer = new byte[bufsize];
                    ret = sock.Receive(recvBuffer);
                    len -= ret;
                }
                else
                {
                    recvBuffer = new byte[len];
                    ret = sock.Receive(recvBuffer);
                    len -= ret;
                }
                recvBuffer.CopyTo((byte[])package, lastLen);
                lastLen +=ret;
                if (ret == 0) return null;

            }
            return (package);
        }


        public void sendTdRun(Func func)
        {
            Thread SENDTD=new Thread(new ThreadStart(func));
            SENDTD.Start();
        }

        public void recvTdRun(Func func)
        {
            Thread RECVTD = new Thread(new ThreadStart(func));
            RECVTD.Start();
        }



        public  void sendFile(string SENDDATA)
        {
            string filename, path = SENDDATA.Replace("file:", "");
            int i = 0;
           // MessageBox.Show(path.Split('\\').Last());
            //发送文件名
            bool ret=send_package("file:"+ path.Split('\\').Last(), new byte[] { 0x00, 0x00 });

            while (RECVDATA != "ok") ;
            //读取文件
            FileStream fsr = new FileStream(path, FileMode.Open);
            byte[] readBytes = new byte[fsr.Length];
            int count = fsr.Read(readBytes, 0, readBytes.Length);
            //字节拼接
            byte[] resArr = new byte[fsr.Length ];
            readBytes.CopyTo(resArr, 0);
            send_package(resArr, new byte[] { 0x00, 0x00 });
            fsr.Close();
        }

        public  void recvFile(string filename)
        {

            string savePath = "recv_file";
            //string tempdata = Regex.Split(filename, "yyend", RegexOptions.IgnoreCase)[1];
            //bool isend = false;

            //if (Regex.Split(filename, "yyend", RegexOptions.IgnoreCase).Length > 2) isend = true;
            if (!System.IO.Directory.Exists(savePath))
                System.IO.Directory.CreateDirectory(savePath);
            send_package("ok", new byte[2] { 0x00,0x00 });
            Thread.Sleep(500);
            FileStream f=new FileStream(savePath+"//" + filename, FileMode.Create);
            recv_bytefile(f);
            f.Close();
            send_package("recv ok\n",new byte[2]{0x00,0x00});

        }


        public void connectClient(int ID)
        {
            connectingID = ID;
        }

        public  void recv_bytefile(FileStream f)
        {
            byte[] recvBuffer = new byte[bufsize];
            int dataLen = 0, ret = sock.Receive(recvBuffer);
            if (ret== 0) return ;
            byte[] dataArr = recvBuffer.Skip(6).Take(recvBuffer.Length - 6).ToArray();
            byte[] lenArr = recvBuffer.Skip(2).Take(4).ToArray();
            byte[] head = recvBuffer.Take(2).ToArray();
            int len = System.BitConverter.ToInt32(lenArr, 0);
  
            f.Write(dataArr, 0, dataArr.Length);
            dataLen = dataArr.Length;
            if (len < bufsize)
            {
                recvBuffer = new byte[len];
                dataLen = len;
            }

            recvBuffer = dataArr.Take(dataLen).ToArray();
            int lastLen = dataArr.Length;
            len -= dataArr.Length;
            Console.WriteLine(len);
            while (len > 0)
            {
                if (len > bufsize)
                {
                    recvBuffer = new byte[bufsize];
                    ret = sock.Receive(recvBuffer);
                    len -= bufsize;
                }
                else
                {
                    recvBuffer = new byte[len];
                    ret = sock.Receive(recvBuffer);
                    len -= len;

                }
                f.Write(recvBuffer, 0, recvBuffer.Length);
                lastLen += recvBuffer.Length;
                //if (ret == 0) return null;

            }
            return ;
        }
    }
}
