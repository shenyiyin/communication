using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Decode_Capture;
namespace YYCommunication
{
    public partial class VideoWindow : Form
    {
        public Bitmap image=null;  
        public VideoWindow()
        {
            //MemoryStream ms = new MemoryStream(img);
            
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //Image_Dealer.ShowImage(pictureBox1, image);
        }

        public void show_image(byte[] img)
        {
            image = Image_Dealer.ToGrayBitmap(img, 640, 480); ;
            Image_Dealer.ShowImage(pictureBox1, image);
        }
    }
}
