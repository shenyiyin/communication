using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using Decode_Capture;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Net.Sockets;
namespace YYCommunication
{
    public partial class Form1 : Form
    {
        Client c=new Client();
        VideoWindow f = new VideoWindow();
        delegate void dlg();
        delegate void setShowChartFormInvoke(VideoWindow myform);
        byte[] img = null;
        bool videoPlaying = false,isshow=false;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Master";
        }
        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            c.connect();
            c.recvTdRun(RecvLoop);
        }
        private void RecvLoop()
        {
            String RECVDATA = null;
            while (true)
            {
                c.RECVDATA = c.recv_package();
                RECVDATA = c.RECVDATA;
                if (RECVDATA == null)
                {
                    MessageBox.Show("连接已断开!或重试");
                    return;
                }
                switch(RECVDATA)
                {
                    case "已连接":continue;
                    case "连接成功!": c.isConnect = true; break;
                    case "image":
                        {
                            
                            c.send_package("ok", new byte[] { 0x00, 0x00 });
                            img = c.recv_bytepackage();
                            setShowChartForm(f);
                            isshow = true;
                            continue;
                        }
                    default:
                        if (RECVDATA.StartsWith("connfrom:"))
                        {
                            c.connectClient(int.Parse(RECVDATA.Split(':')[1]));
                            ShowCbx(RECVDATA.Split(':')[1].Split());

                            //Form f=new Form();
                            //f.Text="是否连接"+ RECVDATA.Split(':')[1];
                            //f.ConnectBtn = new System.Windows.Forms.Button();
                        }

                        if (RECVDATA.StartsWith("请输入你需要连接的对象:"))
                        {
                            string[] obj = RECVDATA.Split(':')[1].Split(',');
                            ShowCbx(obj);
                            continue;
                        }

                        if (RECVDATA.StartsWith("file:"))
                        {
                            c.recvFile(RECVDATA.Replace("file:", ""));
                            ShowText("成功接收文件");
                            continue;
                        }
                        break;

                }
      


                ShowText("from:" + RECVDATA);
            }
        }
        private void SendBtn_Click(object sender, EventArgs e)
        {
            bool ret;
            String SENDDATA = SendText.Text;
            c.SENDDATA = SENDDATA;
            if(!c.isConnect&&c.sock!=null&&objcbx.Text!="")
            {
                ret=c.send_package("conn"+objcbx.Text.Trim(), new byte[] { 0x00, 0x00 });

                if (c.connectingID != -1)
                {
                    c.connectID = c.connectingID;
                    c.isConnect = true;
                }
                c.connectingID = int.Parse(objcbx.Text);
                return;
            }

            


            
            //文件传输
            if (SENDDATA.StartsWith("file:"))
            {
                c.sendFile(SENDDATA);
                return;
            }

            else if (SENDDATA.StartsWith("conn"))
            {
                c.connectID=c.connectingID;
                ret = c.send_package(SENDDATA,new byte[] {0x00,0x00});
            }

            else if (SENDDATA!=null)
                ret=c.send_package(SENDDATA, new byte[] { 0x00, 0x00 });
            ShowText("u:"+SENDDATA);
            SendText.Text="";
        }

        void ShowText(String DATA)
        {
            dlg s = delegate
            {
                ViewText.Text+= DATA+"\r\n";
            };
            ViewText.Invoke(s);
        }

        void ShowCbx(String[] objs)
        {
            dlg s = delegate
            {
                objcbx.Items.Clear();
                foreach (string e in objs)
                    objcbx.Items.Add(e);
                objcbx.SelectedIndex = objcbx.Items.Count-1;
            };
            objcbx.Invoke(s);
                
        }

        private void flashBtn_Click(object sender, EventArgs e)
        {
            bool ret=c.send_package("flash", new byte[] { 0x00, 0x00 });
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
                c.bekill = true;
                System.Environment.Exit(0);//退出全部线程  
                System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void filebtn_Click(object sender, EventArgs e)
        {
            string fName="";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fName = openFileDialog1.FileName;
            }
            dlg s = delegate ()
              {
                  SendText.Text = "file:"+fName;
              };
            SendText.Invoke(s);
        }

        private void SendText_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
               e.Effect=DragDropEffects.Copy;
            else
                e.Effect=DragDropEffects.None;
        }

        private void SendText_DragDrop(object sender, DragEventArgs e)
        {
            String[] files = e.Data.GetData(DataFormats.FileDrop, false) as String[];
           // Console.WriteLine(((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString());
            SendText.Text = "file:"+files[0];
        }

        private void listenBtn_Click(object sender, EventArgs e)
        {
            if (listenBtn.Text == "video") listenBtn.Text = "stop";
            if (listenBtn.Text == "stop")
            {
                listenBtn.Text = "video";
                videoPlaying = false;
            }
            //启动监听线程
            Thread td = new Thread(new ThreadStart(VideoPlaying));
            td.Start();
        }

        public void setShowChartForm(VideoWindow myform)
        {
            if (this.InvokeRequired)
            {
                setShowChartFormInvoke _setShowChartFormInvoke = new setShowChartFormInvoke(setShowChartForm);
                this.Invoke(_setShowChartFormInvoke, new object[] { myform });
            }
            else
            {
                myform.show_image(img);
                myform.Show();
            }
        }
        private void VideoPlaying()
        {
            isshow = true;
            videoPlaying = true;
            while (videoPlaying)
            {
                if (isshow)
                {
                    isshow = false;
                    Thread.Sleep(10);
                    c.send_package("image", new byte[] { 0x00, 0x00 });
                } 
            }
        }
    }
}
