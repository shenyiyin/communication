using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
namespace Decode_Capture
{
    class Image_Dealer
    {
        public static byte[] getByteStreamFromBitmap(int width, int height, int channel, Bitmap img)
        {
            byte[] bytes = new byte[width * height * channel];

            BitmapData im = img.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, img.PixelFormat);
            int stride = im.Stride;
            int offset = stride - width * channel;
            byte[] temp = new byte[stride * height];
            GC.Collect();
            System.Runtime.InteropServices.Marshal.Copy(im.Scan0, temp, 0, temp.Length);
            img.UnlockBits(im);

            int posreal = 0;
            int posscan = 0;
            for (int c = 0; c < height; c++)
            {
                for (int d = 0; d < width * channel; d++)
                {
                    bytes[posreal++] = temp[posscan++];
                }
                posscan += offset;
            }
            
            return bytes;
        }


        //imagebytes to graybitmap
        public static Bitmap ToGrayBitmap(byte[] rawValues, int width, int height)
        {
            //// 申请目标位图的变量，并将其内存区域锁定
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            //// 获取图像参数
            int stride = bmpData.Stride;　 // 扫描线的宽度
            int offset = stride - width;　 // 显示宽度与扫描线宽度的间隙
            IntPtr iptr = bmpData.Scan0;　 // 获取bmpData的内存起始位置
            int scanBytes = stride * height;　　 // 用stride宽度，表示这是内存区域的大小
            //// 下面把原始的显示大小字节数组转换为内存中实际存放的字节数组
            int posScan = 0, posReal = 0;　　 // 分别设置两个位置指针，指向源数组和目标数组
            byte[] pixelValues = new byte[scanBytes];　 //为目标数组分配内存
            for (int x = 0; x < height; x++)
            {
                //// 下面的循环节是模拟行扫描
                for (int y = 0; y < width; y++)
                {
                    pixelValues[posScan++] = rawValues[posReal++];
                }
                posScan += offset;　 //行扫描结束，要将目标位置指针移过那段“间隙”
            }
            //// 用Marshal的Copy方法，将刚才得到的内存字节数组复制到BitmapData中
            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, iptr, scanBytes);
            bmp.UnlockBits(bmpData);　 // 解锁内存区域
            //// 下面的代码是为了修改生成位图的索引表，从伪彩修改为灰度
            ColorPalette tempPalette;
            using (Bitmap tempBmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
            {
                tempPalette = tempBmp.Palette;
            }
            for (int i = 0; i < 256; i++)
            {
                //灰度化
                tempPalette.Entries[i] = Color.FromArgb(i, i, i);
            }
            bmp.Palette = tempPalette;
            //bmp.Save("F:\\ss.jpg");
            //// 算法到此结束，返回结果
            return bmp;
        }



        delegate void DlgShowImage(PictureBox pic,Bitmap img);
        public static void PicBoxShow(PictureBox pic, Bitmap img)
        {
            pic.Image = img;
        }
        public static void ShowImage(PictureBox pic, Bitmap img)
        {
            DlgShowImage show = PicBoxShow;
            {
                show(pic,img);
            };
        }
    }
}
