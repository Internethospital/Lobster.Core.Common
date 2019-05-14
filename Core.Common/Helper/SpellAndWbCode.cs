using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Core.Common.Helper
{
    /// <summary>
    /// 生成汉子拼音码和五笔码
    /// </summary>
    public class SpellAndWbCode
    {
        #region 变量

        /// <summary>
        /// XMLDoc
        /// </summary>
        static XmlDocument xmld = null;

        static Dictionary<string, string> dicPy = null;
        static Dictionary<string, string> dicWb = null;

        #endregion

        #region 私有方法

        /// <summary>
        /// 读取XML文件中数据
        /// </summary>
        /// <returns>返回String[]</returns>
        private static void getXmlData()
        {
            try
            {
                //获得正在运行类所在的名称空间
                Type type = MethodBase.GetCurrentMethod().DeclaringType;
                //获得当前运行的Assembly
                Assembly _assembly = Assembly.GetAssembly(type);
                //根据名称空间和文件名生成资源名称
                string resourceName = _assembly.GetName().Name + ".Core.CodeConfig.xml";

                //根据资源名称从Assembly中获取此资源的Stream
                Stream stream = _assembly.GetManifestResourceStream(resourceName);
                xmld = new XmlDocument();
                xmld.Load(stream);

                //得到拼音字典
                dicPy = new Dictionary<string, string>();

                XmlNodeList xnl = xmld.GetElementsByTagName("SpellCode");

                foreach (XmlNode xn in xnl)
                {
                    foreach (XmlNode xnn in xn.ChildNodes)
                    {
                        char[] texts = xnn.InnerText.ToCharArray();
                        for (int i = 0; i < texts.Length; i++)
                        {
                            if (!dicPy.ContainsKey(texts[i].ToString()))
                                dicPy.Add(texts[i].ToString(), xnn.Name);
                        }
                    }
                }

                //得到五笔字典
                dicWb = new Dictionary<string, string>();

                XmlNodeList _xnl = xmld.GetElementsByTagName("WBCode");

                foreach (XmlNode xn in _xnl)
                {
                    foreach (XmlNode xnn in xn.ChildNodes)
                    {
                        char[] texts = xnn.InnerText.ToCharArray();
                        for (int i = 0; i < texts.Length; i++)
                        {
                            if (!dicWb.ContainsKey(texts[i].ToString()))
                                dicWb.Add(texts[i].ToString(), xnn.Name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("生成拼音五笔码导入资源数据错误！\n" + e.Message);
            }

        }

        #endregion

        #region 公开方法
        /// <summary>
        /// 获得汉语的拼音码
        /// </summary>
        /// <param name="strName">汉字</param>
        /// <param name="start">搜索的开始位置</param>
        /// <param name="len">搜索的结束位置</param>
        /// <returns>汉语反义成字符串，该字符串只包含大写的英文字母</returns>
        public static string GetSpellCode(string strName, int start, int len)
        {
            getXmlData();
            strName = strName.Trim().Replace(" ", "");
            if (string.IsNullOrEmpty(strName))
            {
                return strName;
            }
            len = len == -1 ? strName.Length : len;
            len = (strName.Length - start) < len ? (strName.Length - start) : len;
            strName = strName.Substring(start, len);

            StringBuilder myStr = new StringBuilder();
            foreach (char vChar in strName)
            {
                // 若是字母或数字则直接输出
                if ((vChar >= 'a' && vChar <= 'z') || (vChar >= 'A' && vChar <= 'Z') || (vChar >= '0' && vChar <= '9'))
                    myStr.Append(char.ToUpper(vChar));
                else
                {
                    // 若字符Unicode编码在编码范围则 查汉字列表进行转换输出
                    if (dicPy.ContainsKey(vChar.ToString()))
                    {
                        myStr.Append(dicPy[vChar.ToString()].ToUpper());
                    }
                }
            }
            return myStr.ToString();
        }
        /// <summary>
        /// 获得汉语的拼音码
        /// </summary>
        /// <param name="strName">汉字</param>
        /// <returns></returns>
        public static string GetSpellCode(string strName)
        {
            return GetSpellCode(strName, 0, -1);
        }
        /// <summary>
        /// 获得汉语的五笔码
        /// </summary>
        /// <param name="strName">汉字</param>
        /// <param name="start">搜索的开始位置</param>
        /// <param name="len">搜索的结束位置</param>
        /// <returns>汉语反义成字符串，该字符串只包含大写的英文字母</returns>
        public static string GetWBCode(string strName, int start, int len)
        {
            getXmlData();

            strName = strName.Trim().Replace(" ", "");
            if (string.IsNullOrEmpty(strName))
            {
                return strName;
            }
            len = len == -1 ? strName.Length : len;
            len = (strName.Length - start) < len ? (strName.Length - start) : len;

            strName = strName.Substring(start, len);

            StringBuilder myStr = new StringBuilder();
            foreach (char vChar in strName)
            {
                // 若是字母或数字则直接输出
                if ((vChar >= 'a' && vChar <= 'z') || (vChar >= 'A' && vChar <= 'Z') || (vChar >= '0' && vChar <= '9'))
                    myStr.Append(char.ToUpper(vChar));
                else
                {
                    // 若字符Unicode编码在编码范围则 查汉字列表进行转换输出
                    if (dicWb.ContainsKey(vChar.ToString()))
                    {
                        myStr.Append(dicWb[vChar.ToString()].ToUpper());
                    }
                }
            }
            return myStr.ToString();
        }
        /// <summary>
        /// 获得汉语的五笔码
        /// </summary>
        /// <param name="strName">汉字</param>
        /// <returns></returns>
        public static string GetWBCode(string strName)
        {
            return GetWBCode(strName, 0, -1);
        }

        #endregion
    }
}
