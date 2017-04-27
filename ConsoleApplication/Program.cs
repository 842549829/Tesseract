using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Img();
            try
            {

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "20.png");
                Bitmap image = new Bitmap(path);//识别图像
                tessnet2.Tesseract ocr = new tessnet2.Tesseract();//声明一个OCR类
                //ocr.SetVariable("tessedit_char_whitelist", "0123456789+-="); //设置识别变量，当前只能识别数字。

                //ocr.SetVariable("tessedit_char_whitelist", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                //ocr.SetVariable("language_model_penalty_non_freq_dict_word", "0");
                //ocr.SetVariable("language_model_penalty_non_dict_word ", "0");
                //ocr.SetVariable("tessedit_char_blacklist", "xyz");
                //ocr.SetVariable("classify_bln_numeric_mode", "1");

                string language = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Language");
                ocr.Init(language, "eng", false); //应用当前语言包。注，Tessnet2是支持多国语的。语言包下载链接：http://code.google.com/p/tesseract-ocr/downloads/list
                List<tessnet2.Word> result = ocr.DoOCR(image, Rectangle.Empty);//执行识别操作
                foreach (tessnet2.Word word in result)
                {
                    Console.WriteLine("{0} : {1}", word.Confidence, word.Text);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static void Img()
        {
            try
            {
                //string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "20.png");
                //Bitmap image = new Bitmap(path);
                HttpHelper httpHelper = new HttpHelper();
                var d = httpHelper.HttpGet("http://www.variflight.com/flight/fnum/FM9158.html?AE71649A58c77&fdate=20161206", string.Empty, Encoding.UTF8, false, null, 6000 * 10);

                var imgs = GetHtmlImageUrlList(d).Where(item => item.Contains("/flight/detail")).ToArray();

                string url1 = string.Format("http://www.variflight.com{0}", imgs[0]);
                string url2 = string.Format("http://www.variflight.com{0}", imgs[1]);
                string url3 = string.Format("http://www.variflight.com{0}", imgs[2]);
                GetValue(httpHelper, url1);
                GetValue(httpHelper, url2);
                GetValue(httpHelper, url3);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static void GetValue(HttpHelper httpHelper, string url)
        {
            var bytes = httpHelper.HttpByteGet(url, string.Empty, false, false, 60*1000);
            var image = Bitmap(bytes);
            tessnet2.Tesseract ocr = new tessnet2.Tesseract();
            string language = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Language");
            ocr.Init(language, "eng", false);
            List<tessnet2.Word> result = ocr.DoOCR(image, Rectangle.Empty);
            foreach (tessnet2.Word word in result)
            {
                Console.WriteLine("{0} : {1}", word.Confidence, word.Text);
            }
        }

        /// <summary> 
        /// 取得HTML中所有图片的 URL。 
        /// </summary> 
        /// <param name="sHtmlText">HTML代码</param> 
        /// <returns>图片的URL列表</returns> 
        public static string[] GetHtmlImageUrlList(string sHtmlText)
        {
            // 定义正则表达式用来匹配 img 标签 
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            // 搜索匹配的字符串 
            MatchCollection matches = regImg.Matches(sHtmlText);
            int i = 0;
            string[] sUrlList = new string[matches.Count];

            // 取得匹配项列表 
            foreach (Match match in matches)
                sUrlList[i++] = match.Groups["imgUrl"].Value;
            return sUrlList;
        }

        public static Bitmap Bitmap(byte[] bytes)
        {
            Stream memoryStream = new MemoryStream(bytes);
            byte[] buf = new byte[memoryStream.Length];
            memoryStream.Read(buf, 0, (int)memoryStream.Length);
            var map = new Bitmap(Image.FromStream(memoryStream));
            return map;
        }
    }
}
