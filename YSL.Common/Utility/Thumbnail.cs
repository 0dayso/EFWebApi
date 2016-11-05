using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace YSL.Common.Utility
{
    /// <summary>
    /// ����ͼ������
    /// </summary>
    public class Thumbnail
    {
        int ImgSize = 100;
        string ItemPrefix = "";
        int Minstock = 5;
        static int WaterMark = 1;
        int BigPicWidth = 0;
        int BigPicHeight = 0;
        int SmallPicWidth = 0;
        int SmallPicHeight = 0;
        static string WaterMarkImg = "/images/application_view_tile.png";
        static int WaterMarkPlace = 9;
        int WaterMarkAlpha = 0;
        int GoodsListSize = 14;
        int GoodsListNum = 10;
        int AutoGenImg = 1;
        int TodayOtherGroup = 3;
        int BeforViewNow = 0;
        /// <summary>
        /// Converts the jpge.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="savefilename">The savefilename.</param>
        /// <param name="quality">The quality.</param>
        public static void ConvertJpge(string filename, string savefilename, int quality)
        {
            using (Image oImage = Image.FromFile(filename))
            {
                using (Bitmap tImage = new Bitmap(oImage.Width, oImage.Height))
                {
                    using (Graphics g = Graphics.FromImage(tImage))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.Clear(Color.Transparent);
                        g.DrawImage(oImage, new Rectangle(0, 0, oImage.Width, oImage.Height), new Rectangle(0, 0, oImage.Width, oImage.Height), GraphicsUnit.Pixel);

                        SaveFile(filename, savefilename, tImage, quality);
                    }
                }
            }
        }
        /// <summary>
        /// ��������ͼ��ָ���߿�ü������Ρ�
        /// </summary>
        /// <param name="filename">Դͼ·�������ͣ�System.String��</param>
        /// <param name="savefilename">����ͼ·�������ͣ�System.String��</param>
        /// <param name="width">����ͼ��ȣ����ͣ�System.Int32��</param>
        /// <param name="height">����ͼ�߶ȣ����ͣ�System.Int32��</param>
        /// <param name="quality">����ͼ����(0-100)�����ͣ�System.Int32��</param>
        /// <param name="SaveMode">�ü�ͼƬ���ͣ��ü������ţ���System.Int32��</param>
        public static void Make(string filename, string savefilename, int width, int height, int quality, SaveMode mode)
        {
            using (Image oImage = Image.FromFile(filename))
            {
                Image tImage = setAutoSize(oImage, width, height, mode);
                SaveFile(filename, savefilename, tImage, quality);
            }
        }
        /// <summary>
        /// ��������ͼ��ָ���߿�ü������Ρ�
        /// </summary>
        /// <param name="filename">Դͼ·�������ͣ�System.String��</param>
        /// <param name="savefilename">����ͼ·�������ͣ�System.String��</param>
        /// <param name="width">����ͼ��ȣ����ͣ�System.Int32��</param>
        /// <param name="height">����ͼ�߶ȣ����ͣ�System.Int32��</param>
        /// <param name="quality">����ͼ����(0-100)�����ͣ�System.Int32��</param>
        public static void Make(string filename, string savefilename, int width, int height, int quality)
        {
            using (Image oImage = Image.FromFile(filename))
            {
                Image tImage = setAutoSize(oImage, width, height, SaveMode.HW);
                SaveFile(filename, savefilename, tImage, quality);
            }
        }
        /// <summary>
        /// ��������ͼ��ָ�������š�
        /// </summary>
        /// <param name="filename">Դͼ·�������ͣ�System.String��</param>
        /// <param name="thumbnailPath">����ͼ·�������ͣ�System.String��</param>
        /// <param name="width">����ͼ��ȣ����ͣ�System.Int32��</param>
        /// <param name="height">����ͼ�߶ȣ����ͣ�System.Int32��</param>
        /// <param name="quality">����ͼ����(0-100)�����ͣ�System.Int32��</param>
        public static void Make(string filename, string savefilename, int width, int quality)
        {
            using (Image oImage = Image.FromFile(filename))
            {
                Image tImage = setAutoSize(oImage, width, 0, SaveMode.HW);
                SaveFile(filename, savefilename, tImage, quality);
            }
        }
        /// <summary>
        /// ����ͼƬ
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="savefilename"></param>
        /// <param name="img"></param>
        /// <param name="quality"></param>
        private static void SaveFile(string filename, string savefilename, Image img, int quality)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                {
                    ici = codec;
                }
            }

            using (EncoderParameters encoderParams = new EncoderParameters())
            {
                long[] qualityParam = new long[1];
                if (quality < 0 || quality > 100)
                {
                    quality = 80;
                }
                qualityParam[0] = quality;

                using (EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam))
                {
                    encoderParams.Param[0] = encoderParam;
                    if (WaterMark == 1)
                    {
                        //ˮӡ
                        img = setWatermark(img);
                    }
                    if (ici != null)
                    {
                        img.Save(savefilename, ici, encoderParams);
                    }
                    else
                    {
                        img.Save(savefilename);
                    }
                    img.Dispose();
                }
            }

        }

        #region ��ͼƬ����ˮӡ
        /// <summary>
        /// ��ͼƬ����ˮӡ
        /// </summary>
        /// <returns></returns>
        public static Image setWatermark(Image image)
        {
            //�ж��Ǵ�������ˮӡ����ͼƬˮӡ
            string _watermark = WaterMarkImg;
            if (_watermark != "" && WaterMark == 1)
            {
                try
                {
                    if (_watermark.EndsWith(".gif") || _watermark.EndsWith(".jpg") || _watermark.EndsWith(".png"))
                    {
                        //��ͼƬˮӡ
                        Image copyImage = Image.FromFile(System.Web.HttpContext.Current.Server.MapPath(_watermark));
                        Graphics g = Graphics.FromImage(image);
                        int[] xyPosition = GetPosition(image.Width, image.Height, copyImage.Width, copyImage.Height);
                        int x = xyPosition[0];
                        int y = xyPosition[1];
                        g.DrawImage(copyImage, new Rectangle(x, y, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, GraphicsUnit.Pixel);
                        g.Save();
                        g.Dispose();
                        return image;
                    }
                    else
                    {
                        //������ˮӡ��ע�⣬����Ĵ�������¼�ͼƬˮӡ�Ĵ��벻�ܹ���
                        Graphics g = Graphics.FromImage(image);
                        g.DrawImage(image, 0, 0, image.Width, image.Height);
                        Font f = new Font("Verdana", 24);
                        Brush b = new SolidBrush(Color.White);
                        g.DrawString(_watermark, f, b, 10, 10);
                        g.Save();
                        g.Dispose();
                        return image;
                    }

                }
                catch
                { }

            }
            return image;
        }
        #endregion

        #region ����ˮӡλ��
        /// <summary>
        /// ����ˮӡλ��
        /// </summary>
        /// <param name="imageWidth">���ˮӡͼ�Ŀ��</param>
        /// <param name="imageHeight">���ˮӡͼ�ĸ߶�</param>
        /// <param name="copyImageWidth">ˮӡͼ�Ŀ��</param>
        /// <param name="copyImageHeight">ˮӡͼ�ĸ߶�</param>
        /// <returns>ˮӡ����ͼƬ��X��Y����</returns>
        private static int[] GetPosition(int imageWidth, int imageHeight, int copyImageWidth, int copyImageHeight)
        {
            int[] positions = new int[2];
            #region ˮӡλ��
            int x;
            int y;
            int xOffset = 10; //���ƫ����
            int yOffset = 10; //�߶�ƫ����
            switch (WaterMarkPlace)
            {
                case 1:
                    //���Ͻ�
                    x = xOffset;
                    y = yOffset;
                    break;
                case 2:
                    //���Ϸ�
                    x = (imageWidth - copyImageWidth) / 2;
                    y = yOffset;
                    break;
                case 3:
                    //���Ͻ�
                    x = imageWidth - copyImageWidth - xOffset;
                    y = yOffset;
                    break;
                case 4:
                    //����
                    x = xOffset;
                    y = (imageHeight - copyImageHeight) / 2;
                    break;
                case 5:
                    //����
                    x = (imageWidth - copyImageWidth) / 2;
                    y = (imageHeight - copyImageHeight) / 2;
                    break;
                case 6:
                    //����
                    x = imageWidth - copyImageWidth - xOffset;
                    y = (imageHeight - copyImageHeight) / 2;
                    break;
                case 7:
                    //���½�
                    x = xOffset;
                    y = imageHeight - copyImageHeight - yOffset;
                    break;
                case 8:
                    //���·�
                    x = (imageWidth - copyImageWidth) / 2;
                    y = imageHeight - copyImageHeight - yOffset;
                    break;
                case 9:
                default:
                    //���½�
                    x = imageWidth - copyImageWidth - xOffset;
                    y = imageHeight - copyImageHeight - yOffset;
                    break;
            }
            positions[0] = x;
            positions[1] = y;
            #endregion
            return positions;
        }
        #endregion

        #region ����ͼƬ����ͼ
        public enum SaveMode
        {
            HW = 0x01,
            W = 0x02,
            H = 0x03,
            Cut = 0x04
        }
        ///   <summary>   
        ///   ��������ͼ   
        ///   </summary>   
        ///   <param   name="originalImage">Դͼ</param>   
        ///   <param   name="width">����ͼ���</param>   
        ///   <param   name="height">����ͼ�߶�</param>   
        ///   <param   name="mode">��������ͼ�ķ�ʽ</param>           
        public static Image setAutoSize(Image originalImage, int width, int height, SaveMode mode)
        {
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch ((int)mode)
            {
                case 1://ָ���������    
                    if (towidth >= ow && toheight >= oh)
                    {
                        towidth = ow;
                        toheight = oh;
                    }
                    else
                    {
                        if (ow > oh)
                        {
                            toheight = originalImage.Height * towidth / originalImage.Width;

                        }
                        else
                        {
                            towidth = originalImage.Width * toheight / originalImage.Height;
                        }
                    }

                    break;
                case 2://ָ�����߰�����
                    if (towidth > ow) towidth = ow;
                    toheight = originalImage.Height * towidth / originalImage.Width;
                    break;
                case 3://ָ���ߣ�������   
                    if (toheight > oh) toheight = oh;
                    towidth = originalImage.Width * toheight / originalImage.Height;
                    break;
                case 4://ָ���߿�ü��������Σ�                                   
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * toheight / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //�½�һ��bmpͼƬ   
            Image bitmap = new System.Drawing.Bitmap(width, height);

            //�½�һ������   
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //���ø�������ֵ��   
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //���ø�����,���ٶȳ���ƽ���̶�   
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //��ɫ���
            g.Clear(Color.White); //Color.Transparent��ջ�������͸������ɫ���   

            //��ָ��λ�ò��Ұ�ָ����С����ԭͼƬ��ָ������   
            g.DrawImage(originalImage, new Rectangle((width - towidth) / 2, (height - toheight) / 2, towidth, toheight),
                    new Rectangle(x, y, ow, oh),
                    GraphicsUnit.Pixel);

            g.Dispose();

            return bitmap;
        }
        #endregion

        #region ͼƬ��ת����
        /// <summary>
        /// ��˳ʱ��Ϊ�����ͼ�������ת
        /// </summary>
        /// <param name="b">λͼ��</param>
        /// <param name="angle">��ת�Ƕ�[0,360](ǰ̨����)</param>
        /// <returns></returns>
        public Image Rotate(Image b, int angle)
        {
            angle = angle % 360;

            //����ת��
            double radian = angle * Math.PI / 180.0;
            double cos = Math.Cos(radian);
            double sin = Math.Sin(radian);

            //ԭͼ�Ŀ�͸�
            int w = b.Width;
            int h = b.Height;
            int W = (int)(Math.Max(Math.Abs(w * cos - h * sin), Math.Abs(w * cos + h * sin)));
            int H = (int)(Math.Max(Math.Abs(w * sin - h * cos), Math.Abs(w * sin + h * cos)));

            //Ŀ��λͼ
            Image dsImage = new Bitmap(W, H);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(dsImage);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //����ƫ����
            Point Offset = new Point((W - w) / 2, (H - h) / 2);

            //����ͼ����ʾ������ͼ��������봰�ڵ����ĵ�һ��
            Rectangle rect = new Rectangle(Offset.X, Offset.Y, w, h);
            Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

            g.TranslateTransform(center.X, center.Y);
            //g.RotateTransform(360 - angle);
            g.RotateTransform(angle);
            //�ָ�ͼ����ˮƽ�ʹ�ֱ�����ƽ��
            g.TranslateTransform(-center.X, -center.Y);
            g.DrawImage(b, rect);

            //������ͼ�����б任
            g.ResetTransform();

            g.Save();
            b.Dispose();
            g.Dispose();
            //dsImage.Save("yuancd.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            return dsImage;
        }
        #endregion ͼƬ��ת����

        #region ѹ��ͼƬ
        /// <summary>
        /// �����
        /// ѹ��ͼƬ
        /// </summary>
        /// <param name="contentByte">ͼƬ</param>
        /// <param name="width">ѹ�����</param>
        /// <param name="height">ѹ���߶�</param>
        /// <param name="fileExt">ͼƬ��չ��</param>
        /// <param name="mode">ѹ��ģʽ</param>
        public static byte[] MakeThumbnail(byte[] contentByte, int width, int height, string fileExt, string mode = "HorW")
        {
            try
            {
                Stream inStream = new MemoryStream(contentByte);
                System.Drawing.Image originalImage = System.Drawing.Image.FromStream(inStream);

                int towidth = width;
                int toheight = height;

                int x = 0;
                int y = 0;
                int ow = originalImage.Width;
                int oh = originalImage.Height;

                switch (mode)
                {
                    case "HW"://ָ���߿����ţ����ܱ��Σ�                
                        break;
                    case "HorW":
                        //����ͼ���߼���
                        double newWidth = originalImage.Width;
                        double newHeight = originalImage.Height;

                        //����ڸ߻����ڸߣ���ͼ��������
                        if (originalImage.Width > originalImage.Height || originalImage.Width == originalImage.Height)
                        {
                            //��������ģ��
                            if (originalImage.Width > width)
                            {
                                //��ģ�棬�߰���������
                                newWidth = width;
                                newHeight = originalImage.Height * ((double)width / originalImage.Width);
                            }
                        }
                        //�ߴ��ڿ���ͼ��
                        else
                        {
                            //����ߴ���ģ��
                            if (originalImage.Height > height)
                            {
                                //�߰�ģ�棬����������
                                newHeight = height;
                                newWidth = originalImage.Width * ((double)height / originalImage.Height);
                            }
                        }
                        towidth = (int)newWidth;
                        toheight = (int)newHeight;
                        break;
                    case "W"://ָ�����߰�����                    
                        toheight = originalImage.Height * width / originalImage.Width;
                        break;
                    case "H"://ָ���ߣ�������
                        towidth = originalImage.Width * height / originalImage.Height;
                        break;
                    case "Cut"://ָ���߿�ü��������Σ�                
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        else
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                        break;
                    default:
                        break;
                }

                //�½�һ��bmpͼƬ
                System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

                //�½�һ������
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

                //���ø�������ֵ��
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                //���ø�����,���ٶȳ���ƽ���̶�
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                //��ջ�������͸������ɫ���
                g.Clear(System.Drawing.Color.Transparent);

                MemoryStream resultStream = new MemoryStream();
                try
                {
                    //��ָ��λ�ò��Ұ�ָ����С����ԭͼƬ��ָ������
                    g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                        new System.Drawing.Rectangle(x, y, ow, oh),
                        System.Drawing.GraphicsUnit.Pixel);

                    fileExt = fileExt.ToLower();
                    switch (fileExt)
                    {
                        case "gif":
                            bitmap.Save(resultStream, System.Drawing.Imaging.ImageFormat.Gif);
                            break;
                        case "png":
                            bitmap.Save(resultStream, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        case "bmp":
                        case "jpeg":
                        case "jpg":
                        default:
                            bitmap.Save(resultStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                    }
                    Byte[] resultBuffer = new Byte[resultStream.Length];
                    //�����ж�ȡ�ֽڿ鲢��������д�����������buffer��
                    resultStream.Seek(0, SeekOrigin.Begin);
                    resultStream.Read(resultBuffer, 0, resultBuffer.Length);

                    return resultBuffer;
                }
                catch (System.Exception e)
                {
                   // logger.Error(e.Message);
                }
                finally
                {
                    originalImage.Dispose();
                    bitmap.Dispose();
                    g.Dispose();
                    inStream.Close();
                    inStream.Dispose();
                    resultStream.Close();
                    resultStream.Dispose();
                }
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message);
            }
            return null;
        }
        #endregion
    }
}
