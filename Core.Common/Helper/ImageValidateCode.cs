using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Core.Common.Helper
{
    /// <summary>
    /// 生成图片验证码类
    /// </summary>
    public class ImageValidateCode
    {

        #region 私有字段
        private string text;
        private byte[] imageByte;
        private static byte[] randb = new byte[4];
        private static RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
        #endregion

        #region 公有属性
        /// <summary>
        /// 验证码
        /// </summary>
        public string Text
        {
            get { return this.text; }
        }

        /// <summary>
        /// 验证码字节数组
        /// </summary>
        public byte[] ImageByte
        {
            get { return this.imageByte; }
        }
        #endregion

        #region 构造函数
        public ImageValidateCode(int count)
        {
            this.text = Rand.Str(count);
            CreateImageBySkiaSharp();
        }
        #endregion

        public void CreateImageBySkiaSharp()
        {
            string text = this.text;
            var zu = text.ToList();
            SKBitmap bmp = new SKBitmap(80, 30);
            using (SKCanvas canvas = new SKCanvas(bmp))
            {
                //背景色
                canvas.DrawColor(SKColors.White);

                using (SKPaint sKPaint = new SKPaint())
                {
                    sKPaint.TextSize = 16;//字体大小
                    sKPaint.IsAntialias = true;//开启抗锯齿                   
                    sKPaint.Typeface = SKTypeface.FromFamilyName("微软雅黑", SKTypefaceStyle.Bold);//字体
                    SKRect size = new SKRect();
                    sKPaint.MeasureText(zu[0].ToString(), ref size);//计算文字宽度以及高度

                    float temp = (bmp.Width / 4 - size.Size.Width) / 2;
                    float temp1 = bmp.Height - (bmp.Height - size.Size.Height) / 2;
                    Random random = new Random();

                    for (int i = 0; i < 4; i++)
                    {
                        sKPaint.Color = new SKColor((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
                        canvas.DrawText(zu[i].ToString(), temp + 20 * i, temp1, sKPaint);//画文字
                    }
                    //干扰线
                    for (int i = 0; i < 5; i++)
                    {
                        sKPaint.Color = new SKColor((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
                        canvas.DrawLine(random.Next(0, 40), random.Next(1, 29), random.Next(41, 80), random.Next(1, 29), sKPaint);
                    }
                }
                //图片
                using (SKImage img = SKImage.FromBitmap(bmp))
                {
                    using (SKData p = img.Encode())
                    {
                        this.imageByte = p.ToArray();
                    }
                }
            }
        }
    }
    /// <summary>
    /// 验证码类
    /// </summary>
    public class Rand
    {
        #region 生成随机数字
        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        public static string Number(int Length)
        {
            return Number(Length, false);
        }

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        public static string Number(int Length, bool Sleep)
        {
            if (Sleep) System.Threading.Thread.Sleep(3);
            string result = "";
            System.Random random = new Random();
            for (int i = 0; i < Length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }
        #endregion

        #region 生成随机字母与数字
        /// <summary>
        /// 生成随机字母与数字
        /// </summary>
        /// <param name="IntStr">生成长度</param>
        public static string Str(int Length)
        {
            return Str(Length, false);
        }

        /// <summary>
        /// 生成随机字母与数字
        /// </summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        public static string Str(int Length, bool Sleep)
        {
            if (Sleep) System.Threading.Thread.Sleep(3);
            char[] Pattern = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'g', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            string result = "";
            int n = Pattern.Length;
            System.Random random = new Random(~unchecked((int)DateTime.Now.ToCstTime().Ticks));
            for (int i = 0; i < Length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            return result;
        }
        #endregion

        #region 生成随机纯字母随机数
        /// <summary>
        /// 生成随机纯字母随机数
        /// </summary>
        /// <param name="IntStr">生成长度</param>
        public static string Str_char(int Length)
        {
            return Str_char(Length, false);
        }

        /// <summary>
        /// 生成随机纯字母随机数
        /// </summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        public static string Str_char(int Length, bool Sleep)
        {
            if (Sleep) System.Threading.Thread.Sleep(3);
            char[] Pattern = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int n = Pattern.Length;
            System.Random random = new Random(~unchecked((int)DateTime.Now.ToCstTime().Ticks));
            for (int i = 0; i < Length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            return result;
        }
        #endregion
    }
}
