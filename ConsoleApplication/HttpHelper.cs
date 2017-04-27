using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ConsoleApplication
{
    /// <summary>InnerException
    /// HTTP请求帮助类
    /// </summary>
    public class HttpHelper
    {
        /// <summary>
        /// 索引
        /// </summary>
        private readonly int index;

        /// <summary>
        /// 获取或设置一个值，该值指示是否与 Internet 资源建立持久性连接。
        /// </summary>
        public bool KeepAlive { get; set; }

        /// <summary>
        /// 获取或设置一个 System.Boolean 值，该值确定是否使用 100-Continue 行为。
        /// </summary>
        public bool Expect100Continue { get; set; }

        /// <summary>
        /// UserAgent
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// AcceptLanguage
        /// </summary>
        public string AcceptLanguage { get; set; }

        /// <summary>
        /// Accept
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// AcceptEncoding
        /// </summary>
        public string AcceptEncoding { get; set; }

        /// <summary>
        /// AcceptCharset
        /// </summary>
        public string AcceptCharset { get; set; }

        /// <summary>
        /// ContentType
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CookieContainer Cookie { get; set; }

        /// <summary>
        /// 实例化
        /// </summary>
        public HttpHelper()
        {
            this.index = GetRandom(0, 3);
            this.Cookie = new CookieContainer();
        }

        /// <summary>
        /// 实例化
        /// </summary>
        public HttpHelper(CookieContainer cookie)
        {
            this.index = GetRandom(0, 3);
            this.Cookie = cookie;
        }

        /// <summary>
        /// 使用get方式访问目标网页，返回html页面
        /// </summary>
        /// <param name="targetURL">请求地址</param>
        /// <param name="referer">获取或设置值的引用页HTTP头</param>
        /// <param name="encoding">encoding</param>
        /// <param name="requestClientIp">是否使用IP欺骗</param>
        /// <param name="proxy">代理IP</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>html</returns>
        public string HttpGet(string targetURL, string referer, Encoding encoding, bool requestClientIp, WebProxy proxy, int timeout)
        {
            //请求目标网页
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(targetURL);
            request.CookieContainer = Cookie;
            request.Method = "GET";
            request.Referer = referer;
            request.Timeout = timeout;
            if (!string.IsNullOrEmpty(AcceptEncoding))
            {
                request.Headers["Accept-Encoding"] = AcceptEncoding;
                request.AutomaticDecompression = DecompressionMethods.GZip;
            }

            if (string.IsNullOrEmpty(Accept) == false)
            {
                request.Accept = Accept;
            }

            if (string.IsNullOrEmpty(AcceptLanguage) == false)
            {
                request.Headers.Add("Accept-Language:" + AcceptLanguage);
            }
            bool isHttps = targetURL.ToLower().Contains("https");
            if (isHttps)
            {
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

            }
            if (proxy != null)
            {
                request.Proxy = proxy;
            }
            if (requestClientIp)
            {
                request.Headers.Add("X-Forwarded-For", this.GetRequestIP());
                request.Headers.Add("Client-Ip", this.GetRequestIP());
            }
            if (this.KeepAlive)
            {
                request.KeepAlive = this.KeepAlive;
            }

            request.ServicePoint.Expect100Continue = this.Expect100Continue;

            request.UserAgent = UserAgent ?? this.GetUseAgent();

            try
            {
                //获取网页响应结果
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Cookie.Add(response.Cookies);
                    using (Stream stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            var html = new StreamReader(stream, encoding).ReadToEnd();
                            return html;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 使用get方式访问目标网页，返回html页面
        /// </summary>
        /// <param name="targetURL">url</param>
        /// <param name="referer">referer</param>
        /// <param name="encoding">encoding</param>
        /// <param name="requestClientIp">是否使用IP欺骗</param>
        /// <param name="isUseProxyIP">是否使用代理IP</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>html</returns>
        public string HttpXMLGet(string targetURL, string referer, Encoding encoding, bool requestClientIp, bool isUseProxyIP, int timeout)
        {
            //请求目标网页
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(targetURL);
            request.CookieContainer = Cookie;
            request.Method = "GET";
            request.Referer = referer;
            request.Timeout = timeout;
            request.Headers.Set("x-requested-with", "XMLHttpRequest");
            if (!string.IsNullOrEmpty(AcceptEncoding))
            {
                request.Headers["Accept-Encoding"] = AcceptEncoding;
                request.AutomaticDecompression = DecompressionMethods.GZip;
            }

            if (string.IsNullOrEmpty(Accept) == false)
            {
                request.Accept = Accept;
            }

            if (string.IsNullOrEmpty(AcceptLanguage) == false)
            {
                request.Headers.Add("Accept-Language:" + AcceptLanguage);
            }
            bool isHttps = targetURL.ToLower().Contains("https");
            if (isHttps)
            {
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

            }

            if (requestClientIp)
            {
                request.Headers.Add("X-Forwarded-For", this.GetRequestIP());
                request.Headers.Add("Client-Ip", this.GetRequestIP());
            }
            if (KeepAlive)
            {
                request.KeepAlive = KeepAlive;
            }

            request.ServicePoint.Expect100Continue = Expect100Continue;

            if (UserAgent != null)
            {
                request.UserAgent = UserAgent;

            }
            else
            {
                request.UserAgent = this.GetUseAgent();
            }

            try
            {
                //获取网页响应结果
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Cookie.Add(response.Cookies);


                    using (Stream stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            var html = new StreamReader(stream, encoding).ReadToEnd();
                            return html;
                        }
                        else
                        {
                            throw new ArgumentNullException("stream");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                if (isUseProxyIP)
                {

                }
                throw ex;
            }

        }

        /// <summary>
        /// 使用get方式访问目标网页，返回图片字节数组
        /// </summary>
        /// <param name="targetURL">url</param>
        /// <param name="referer">referer</param>
        /// <param name="requestClientIp">是否使用IP欺骗</param>
        /// <param name="isUseProxyIP">是否使用代理IP</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>html</returns>
        public byte[] HttpByteGet(string targetURL, string referer, bool requestClientIp, bool isUseProxyIP, int timeout)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(targetURL);
            request.CookieContainer = Cookie;
            request.Method = "GET";
            request.Referer = referer;
            request.Timeout = timeout;
            if (!string.IsNullOrEmpty(AcceptEncoding))
            {
                request.Headers["Accept-Encoding"] = AcceptEncoding;
                request.AutomaticDecompression = DecompressionMethods.GZip;
            }

            if (requestClientIp)
            {
                request.Headers.Add("X-Forwarded-For", this.GetRequestIP());
                request.Headers.Add("Client-Ip", this.GetRequestIP());
            }

            request.UserAgent = this.GetUseAgent();

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Cookie.Add(response.Cookies);
                    using (Stream stream = response.GetResponseStream())
                    {
                        byte[] arrImageByte;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            const int bufferLength = 1024;
                            int actual;
                            byte[] buffer = new byte[bufferLength];
                            while (stream != null && (actual = stream.Read(buffer, 0, bufferLength)) > 0)
                            {
                                ms.Write(buffer, 0, actual);
                            }

                            arrImageByte = new byte[ms.Length];
                            ms.Position = 0;
                            ms.Read(arrImageByte, 0, arrImageByte.Length);
                        }

                        return arrImageByte;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 使用get方式访问目标网页，返回图片
        /// </summary>
        /// <param name="targetURL">url</param>
        /// <param name="referer">referer</param>
        /// <param name="requestClientIp">是否使用IP欺骗</param>
        /// <param name="isUseProxyIP">是否使用代理IP</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>html</returns>
        public Bitmap HttpBitmapGet(string targetURL, string referer, bool requestClientIp, bool isUseProxyIP, int timeout)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(targetURL);
            request.CookieContainer = Cookie;
            request.Method = "GET";
            request.Referer = referer;
            request.Timeout = timeout;
            if (!string.IsNullOrEmpty(AcceptEncoding))
            {
                request.Headers["Accept-Encoding"] = AcceptEncoding;
                request.AutomaticDecompression = DecompressionMethods.GZip;
            }

            if (requestClientIp)
            {
                request.Headers.Add("X-Forwarded-For", this.GetRequestIP());
                request.Headers.Add("Client-Ip", this.GetRequestIP());
            }

            request.UserAgent = this.GetUseAgent();

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Cookie.Add(response.Cookies);
                    using (Stream stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            List<byte> bytes = new List<byte>();
                            int temp = stream.ReadByte();
                            while (temp != -1)
                            {
                                bytes.Add((byte)temp);
                                temp = stream.ReadByte();
                            }
                            Stream memoryStream = new MemoryStream(bytes.ToArray());
                            byte[] buf = new byte[memoryStream.Length];
                            memoryStream.Read(buf, 0, (int)memoryStream.Length);
                            var map = new Bitmap(Image.FromStream(memoryStream));
                            return map;
                        }
                        else
                        {
                            throw new ArgumentNullException("stream");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 使用post方式访问目标网页，返回html页面
        /// </summary>
        /// <param name="targetURL">url</param>
        /// <param name="referer">referer</param>
        /// <param name="parametrs">parametrs</param>
        /// <param name="encoding">encoding</param>
        /// <param name="requestClientIp">是否使用Ip欺骗</param>
        /// <param name="isUseProxyIP">是否使用代理IP</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>html</returns>
        public string HttpPost(string targetURL, string referer, string parametrs, Encoding encoding, bool requestClientIp, bool isUseProxyIP, int timeout)
        {
            string html;
            byte[] data = encoding.GetBytes(parametrs);

            //请求目标网页
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(targetURL);

            request.CookieContainer = this.Cookie;
            request.Method = "POST";    //使用post方式发送数据
            request.ContentType = "application/x-www-form-urlencoded";
            if (!string.IsNullOrEmpty(this.ContentType))
            {
                request.ContentType = this.ContentType;
            }
            request.Referer = referer;
            if (!string.IsNullOrEmpty(this.AcceptEncoding))
            {
                request.Headers["Accept-Encoding"] = AcceptEncoding;
                request.AutomaticDecompression = DecompressionMethods.GZip;
            }
            if (!string.IsNullOrEmpty(this.Accept))
            {
                request.Accept = this.Accept;
            }
            request.Timeout = timeout;
            request.ServicePoint.Expect100Continue = this.Expect100Continue;
            ServicePointManager.Expect100Continue = this.Expect100Continue;
            if (!string.IsNullOrEmpty(this.AcceptLanguage))
            {
                request.Headers.Add("Accept-Language:" + this.AcceptLanguage);
            }
            bool isHttps = targetURL.ToLower().Contains("https");
            if (isHttps)
            {
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            }
            if (requestClientIp)
            {
                request.Headers.Add("X-Forwarded-For", this.GetRequestIP());
                request.Headers.Add("Client-Ip", this.GetRequestIP());
            }
            request.ContentLength = data.Length;
            request.UserAgent = this.UserAgent ?? this.GetUseAgent();
            if (this.KeepAlive)
            {
                request.KeepAlive = this.KeepAlive;
            }
            try
            {

                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        this.Cookie.Add(response.Cookies);
                        using (Stream stream = response.GetResponseStream())
                        {
                            if (stream != null)
                            {
                                html = new StreamReader(stream, encoding).ReadToEnd();
                            }
                            else
                            {
                                throw new ArgumentNullException("stream");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return html;
        }

        /// <summary>
        /// 使用XML post方式访问目标网页，返回html页面
        /// </summary>
        /// <param name="targetURL">url</param>
        /// <param name="referer">referer</param>
        /// <param name="parametrs">parametrs</param>
        /// <param name="encoding">encoding</param>
        /// <param name="requestClientIp">是否使用Ip欺骗</param>
        /// <param name="isUseProxyIP">是否使用代理IP</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>html</returns>
        public string HttpXMLPost(string targetURL, string referer, string parametrs, Encoding encoding, bool requestClientIp, bool isUseProxyIP, int timeout)
        {
            string html;
            byte[] data = encoding.GetBytes(parametrs);

            //请求目标网页
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(targetURL);

            request.CookieContainer = Cookie;
            request.Method = "POST";    //使用post方式发送数据
            if (string.IsNullOrEmpty(ContentType))
            {
                request.ContentType = "text/xml";
            }
            else
            {
                request.ContentType = ContentType;
            }

            request.Referer = referer;
            request.Timeout = timeout;
            if (!string.IsNullOrEmpty(AcceptEncoding))
            {
                request.Headers["Accept-Encoding"] = AcceptEncoding;
                request.AutomaticDecompression = DecompressionMethods.GZip;
            }

            if (string.IsNullOrEmpty(Accept) == false)
            {
                request.Accept = Accept;
            }

            if (string.IsNullOrEmpty(AcceptCharset) == false)
            {
                request.Headers["Accept-Charset"] = AcceptCharset;
                request.ContentType += ";charset=UTF-8";
            }

            request.ServicePoint.Expect100Continue = Expect100Continue;
            ServicePointManager.Expect100Continue = Expect100Continue;
            request.Headers.Set("x-requested-with", "XMLHttpRequest");

            if (string.IsNullOrEmpty(AcceptLanguage) == false)
            {
                request.Headers.Add("Accept-Language:" + AcceptLanguage);
            }
            bool isHttps = targetURL.ToLower().Contains("https");
            if (isHttps)
            {
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

            }

            if (requestClientIp)
            {
                request.Headers.Add("X-Forwarded-For", this.GetRequestIP());
                request.Headers.Add("Client-Ip", this.GetRequestIP());

            }

            request.ContentLength = data.Length;
            if (UserAgent != null)
            {
                request.UserAgent = UserAgent;

            }
            else
            {
                request.UserAgent = this.GetUseAgent();
            }
            if (KeepAlive)
            {
                request.KeepAlive = KeepAlive;
            }

            try
            {

                using (Stream newStream = request.GetRequestStream())
                {

                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {

                        Cookie.Add(response.Cookies);

                        using (Stream stream = response.GetResponseStream())
                        {
                            if (stream != null)
                            {
                                html = new StreamReader(stream, encoding).ReadToEnd();
                            }
                            else
                            {
                                throw new ArgumentNullException("stream");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (isUseProxyIP)
                {

                }
                throw ex;
            }

            return html;
        }

        /// <summary>
        /// 获取 UseAgent
        /// </summary>
        /// <returns></returns>
        public string GetUseAgent()
        {
            List<string> list = new List<string>
            {
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; SV1; .NET CLR 2.0.1124)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)"
            };

            return list[index];
        }

        /// <summary>
        /// 随机产生头信息并随机产生版本
        /// </summary>
        /// <returns>返回头信息</returns>
        public static string RandomUserAgent()
        {
            string[] agent = new string[]{
            "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:{0}.0) G{2}o/20100101 Fire{3}/{1}.0",
            "Mozilla/5.0 (Windows NT 6.1; WOW64; T{2}ent/{0}.0; rv:{1}.0) like Ge{3}",
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWeb{2}/537.36 (KHTML, like Gecko) Chr{3}/{0}.0.1650.63 Safari/{1}.36 QIHU 360SE",
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWeb{2}/534.30 (KHTML, like Gecko) Chr{3}/{0}.0.742.30 Safari/{1}.30",
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Tri{2}t/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR {0}.5.30729; .NET CLR 3.0.30729; Media Cen{3} PC {1}.0; .NET4.0C; .NET4.0E)",
            "Mozilla/4.0 (compatible; MSIE {0}.0; Windows NT {1}.1; {2}; .NET {3} 2.0.1124)",
            "Mozilla/4.0 (compatible; MSIE {0}.0; Windows NT {1}.2; .{2} CLR 1.1.4322; .NET {3} 2.0.50727)",

            "Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_6_8; en-us) AppleWeb{2}/{0}.50 (KHTML, like Gecko) Version/5.1 Saf{3}/{1}.50",
            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-us) AppleWeb{2}/{0}.50 (KHTML, like Gecko) Version/5.1 Saf{3}/{1}.50",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.6; rv:{0}.0.1) Ge{2}/20100101 Fir{3}x/{1}.0.1",
            "Mozilla/5.0 (Windows NT 6.1; rv:{0}.0.1) Ge{2}/20100101 Fir{3}x/{1}.0.1",
            "Opera/9.80 (Macintosh; Intel Mac OS X 10.6.8; U; en) {3}{2}/{0}.8.131 Version/{1}.11",
            "Opera/9.80 (Windows NT 6.1; U; en) Pre{2}/{0}.8.{3} Version/{1}.11",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_7_0) AppleWeb{2}/{0}.11 (KHTML, like Gecko) Ch{3}e/17.0.963.56 Safari/{1}.11",
            "Mozilla/4.0 (compatible; MSIE {0}.0; Wind{2} NT 5.1; Maxt{3} {1}.0)",
            "Mozilla/4.0 (compatible; MSIE {0}.0; Wind{2} NT 5.1; TencentTrav{3}r {1}.0)",
            "Mozilla/4.0 (compatible; MSIE {0}.0; Wind{2} NT {1}.1; The Wo{3})",
            "Mozilla/4.0 (compatible; MSIE {0}.0; Wind{2} NT 5.1; Trident/4.0; SE 2.X MetaSr 1.0; SE 2.X Met{3} 1.0; .NET CLR 2.0.50727; SE 2.X MetaSr {1}.0)",
            "Mozilla/4.0 (compatible; MSIE {0}.0; Wind{2} NT {1}.1; Avant Brow{3})",
            "Mozilla/5.0 (iPhone; U; CPU iPhone OS 4_3_3 like Mac OS X; en-us) AppleWeb{2}/{0}.17.9 (KHTML, like Gecko) Vers{3}/5.0.2 Mobile/8J2 Safari/{1}.18.5",
            "Mozilla/5.0 (iPod; U; CPU iPhone OS 4_3_3 like Mac OS X; en-us) AppleWeb{2}/{0}.17.9 (KHTML, like Gecko) Vers{3}/5.0.2 Mobile/8J2 Safari/{1}.18.5",
            "Mozilla/5.0 (iPad; U; CPU OS 4_3_3 like Mac OS X; en-us) AppleWeb{2}/{0}.17.9 (KHTML, like Gecko) Vers{3}/5.0.2 Mobile/8J2 Safari/{1}.18.5",
            "Mozilla/5.0 (Linux; U; Android 2.3.7; en-us; Nexus One Build/FRF91) AppleWeb{2}/{0}.1 (KHTML, like Gecko) Vers{3}/4.0 Mobile Safari/{1}.1",
            "MQQBrowser/26 Mozilla/5.0 (Linux; U; Android 2.3.7; zh-cn; MB200 Build/GRJ22; CyanogenMod-7) AppleWeb{2}/{0}.1 (KHTML, like Ge{3}) Version/4.0 Mobile Safari/{1}.1",
            "Opera/9.80 (Android 2.3.4; Linux; Opera Mobi/build-1107180945; U; en-GB) Pre{2}/{0}.8.149 Vers{3}/{1}.10",
            "Mozilla/5.0 (Linux; U; Android 3.0; en-us; Xoom Build/HRI39) AppleWeb{2}/{0}.13 (KHTML, like Gecko) Vers{3}/4.0 Safari/{1}.13",
            "Mozilla/5.0 (BlackBerry; U; BlackBerry 9800; en) AppleWeb{2}/{0}.1+ (KHTML, like Gecko) Version/6.0.0.337 Mobile Sa{3}i/{1}.1+",
            "Mozilla/5.0 (hp-tablet; Linux; hpwOS/3.0.0; U; en-US) AppleWeb{2}/{0}.6 (KHTML, like Gecko) wOSBrowser/233.70 Saf{3}/{1}.6 TouchPad/1.0",
            "Mozilla/5.0 (SymbianOS/9.4; Series60/5.0 NokiaN97-1/20.0.019; Prof{2}/MIDP-2.1 Configuration/CLDC-1.1) AppleWeb{3}/{0} (KHTML, like Gecko) BrowserNG/{1}.1.18124",
            "UC{2}7.0.2.{3}/{0}/{1}",
            "NOKIA5700/ UC{2}7.0.2.{3}/{0}/{1}",
            "Openwave/ UC{2}7.0.2.{3}/{0}/{1}",
            "Mozilla/4.0 (compati{2}; MSIE 6.0; ) Op{3}/UCWEB7.0.2.37/{0}/{1}",
            };
            int xiaobiao = new Random(Chaos_GetRandomSeed()).Next(32);
            int[] randomArray = new int[] { 20, 200, 2000, 20000, 200000 };
            int num = new Random(Chaos_GetRandomSeed()).Next(4);
            int num1 = new Random(Chaos_GetRandomSeed()).Next(4);

            //下面开始生成字母
            string[] zimu = new string[100];
            for (int i = 0; i < 100; i++)
            {
                zimu[i] = RandomBrowserMark(false);
            }
            string zimu1 = zimu[new Random(Chaos_GetRandomSeed()).Next(100)] + zimu[new Random(Chaos_GetRandomSeed()).Next(100)];
            string zimu2 = zimu[new Random(Chaos_GetRandomSeed()).Next(100)] + zimu[new Random(Chaos_GetRandomSeed()).Next(100)];
            return string.Format(agent[xiaobiao], new Random(Chaos_GetRandomSeed()).Next(randomArray[num]), new Random(Chaos_GetRandomSeed()).Next(randomArray[num1]), zimu1, zimu2);
        }

        /// <summary>
        /// 随机产生两个字母
        /// </summary>
        /// <param name="isUorL">需要大写形式还是小写形式（true为大写）</param>
        /// <returns></returns>
        public static string RandomBrowserMark(bool isUorL)
        {
            string[] strMark = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            string str;
            str = strMark[new Random(Chaos_GetRandomSeed()).Next(25)];
            str += strMark[new Random(Chaos_GetRandomSeed()).Next(25)];
            if (isUorL)
            {
                return str;
            }
            else
            {
                return str.ToLower();
            }

        }

        /// <summary>
        /// 获取一个请求IP（随机生成C类IP地址：192.168.0.0到192.168.255.255 ）
        /// </summary>
        /// <returns>C类IP地址</returns>
        public string GetRequestIP()
        {
            string first = GetRandom(0, 255).ToString();
            string second = GetRandom(0, 255).ToString();
            string third = GetRandom(0, 255).ToString();
            string fourth = GetRandom(0, 255).ToString();

            return string.Format("{0}.{1}.{2}.{3}", first, second, third, fourth);
        }

        /// <summary>
        /// 获得一个随机数
        /// </summary>
        /// <param name="minValue">随机数最小值</param>
        /// <param name="maxValue">随机数最大值</param>
        /// <returns>随机数</returns>
        private static int GetRandom(int minValue, int maxValue)
        {
            try
            {

                // 生成随机种子  用于随机返回代理IP 
                // int seekSeek = unchecked((int)DateTime.Now.Ticks);
                Random random = new Random(Chaos_GetRandomSeed());
                return random.Next(minValue, maxValue);
            }
            catch
            {
                return minValue;
            }
        }

        /// <summary>
        /// 根据cookie获取CookieContainer
        /// </summary>
        /// <param name="cookieList">cookieList</param>
        /// <returns>CookieContainer</returns>
        public static CookieContainer GetCookieContainer(List<Cookie> cookieList)
        {
            if (cookieList == null || cookieList.Count == 0)
            {
                return null;
            }

            CookieContainer cc = new CookieContainer();
            foreach (Cookie cookie in cookieList)
            {
                cc.Add(cookie);
            }

            return cc;
        }

        /// <summary>
        /// 获取所有cookie
        /// </summary>
        /// <param name="cc">CookieContainer</param>
        /// <returns>CookieList</returns>
        public static List<Cookie> GetAllCookies(CookieContainer cc)
        {
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance, null, cc, new object[] { });
            return (from object pathList in table.Values select (SortedList)pathList.GetType().InvokeMember("m_list", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { }) into lstCookieCol from CookieCollection colCookies in lstCookieCol.Values from Cookie c in colCookies select c).ToList();
        }

        /// <summary>
        /// 加密随机数生成器，生成随机种子
        /// </summary>
        /// <returns>随机种子</returns>
        public static int Chaos_GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// 随机头信息选择器（仅限国航上使用）
        /// </summary>
        /// <param name="helper">helper</param>
        public static void RandomHeadersInfo(HttpHelper helper)
        {
            //int num= new Random(Chaos_GetRandomSeed()).Next(1,3);
            int num = 1;
            switch (num)
            {
                case 1:
                    helper.UserAgent = RandomUserAgent();

                    //  helper.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:37.0) Gecko/20100101 Firefox/37.0";
                    //helper.AcceptEncoding = RandomStrNum();
                    //helper.Expect100Continue = true;
                    helper.AcceptLanguage = "zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3";
                    // helper.AcceptLanguage = RandomStrNum();
                    //helper.AcceptLanguage = RandomStrNum();
                    helper.AcceptEncoding = "gzip, deflate";
                    helper.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                    //helper.UserAgent = RandomStrNum();
                    break;
                case 2:
                    //helper.Accept = "";
                    //helper.AcceptEncoding = "gzip, deflate";
                    helper.UserAgent = "∷";
                    // helper.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                    //helper.Expect100Continue = false;
                    break;
                case 3:
                    //helper.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                    //helper.AcceptEncoding = "gzip, deflate";
                    //helper.UserAgent = RandomStrNum();
                    helper.UserAgent = "∷∷";
                    //helper.UserAgent = RandomUserAgent();                   
                    //helper.AcceptLanguage = "zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3";
                    break;
            }
        }

        /// <summary>
        /// 随机数字产生
        /// </summary>
        /// <returns></returns>
        public static string RandomStrNum()
        {
            string str = string.Empty;
            for (int i = 0; i < 1; i++)
            {
                if (new Random(Chaos_GetRandomSeed()).Next(1, 9) % 2 == 0)
                {
                    str += RandomBrowserMark(new Random(Chaos_GetRandomSeed()).Next(1, 9) % 2 == 0);
                }
                else
                {
                    str += new Random(Chaos_GetRandomSeed()).Next(10, 99);
                }
                string[] strs = { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "-", "=", "{", "}", ":", "'", "?", "/", "<", ">" };
                str += strs[new Random(Chaos_GetRandomSeed()).Next(0, 20)];
            }
            return str;
        }

        /// <summary>
        /// 转换成URL格式
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>value</returns>
        public static string UrlEncode(string value)
        {
            return HttpUtility.UrlEncode(value);
        }
    }
}
