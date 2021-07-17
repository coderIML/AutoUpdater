//-----------------------------------------------------------------------
// <copyright company="工品一号" file="Program.cs">
//  Copyright (c)  V1.0.0.0  
//  创建作者:   刘少林
//  创建时间:   2021-06-29 10:31:54
//  功能描述:   将服务器更新配置文件中的更新日期减少一天
//  历史版本:
//          2021-06-29 刘少林 将服务器更新配置文件中的更新日期减少一天
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace UpdateServerXmlAddOneDayProject
{
    /// <summary>
    /// 将服务器更新配置文件中的更新日期减少一天
    /// </summary>
    class Program
    {
        private static string dirPath = null;
        private static string localUpdaterFilePath = null;
        /// <summary>
        /// 本地更新配置文件全路径
        /// </summary>
        public static string LocalUpdaterFilePath
        {
            get
            {
                if (localUpdaterFilePath == null)
                {
                    localUpdaterFilePath = Directory.GetCurrentDirectory() + "\\updater.xml";
                }
                return localUpdaterFilePath;
            }
        }

        /// <summary>
        /// 本地更新updater.xml配置文件对象
        /// </summary>
        private static XmlDocument document = null;

        static void Main(string[] args)
        {
            if (!File.Exists(LocalUpdaterFilePath))
            {
                Console.WriteLine("本地更新配置文件updater.xml不存在，无法执行更新程序!");
                Console.ReadLine();
                return;
            }
            if (document == null)
            {
                document = new XmlDocument();
                try
                {
                    document.Load(LocalUpdaterFilePath);
                }
                catch
                {
                    Console.WriteLine("updater.xml格式非法，无法加载!");
                    document = null;
                    Console.ReadLine();
                    return;
                }
            }
            //服务器更新包HTTP地址
            dirPath = GetConfigValue("Url");
            if (string.IsNullOrEmpty(dirPath))
            {
                Console.WriteLine("服务器Url路径非法，无法执行更新程序");
                Console.ReadLine();
                return;
            }
            string thePreUpdateDate = GetTheLastUpdateTime();        //服务器的更新时间
            if (string.IsNullOrEmpty(thePreUpdateDate))
            {
                Console.WriteLine("服务器更新时间为空，无法执行更新程序");
                Console.ReadLine();
                return;
            }
            DateTime serverTime;
            if (!DateTime.TryParse(thePreUpdateDate, out serverTime))
            {
                Console.WriteLine("服务器更新时间值异常，无法执行更新程序");
                Console.ReadLine();
                return;
            }
            string localUpDate = GetConfigValue("UpDate");   //当前的更新时间
            if (string.IsNullOrEmpty(localUpDate))
            {
                Console.WriteLine("本地更新时间为空，无法执行更新程序");
                Console.ReadLine();
                return;
            }
            DateTime localTime;
            if (!DateTime.TryParse(localUpDate, out localTime))
            {
                Console.WriteLine("本地更新时间值异常，无法执行更新程序");
                Console.ReadLine();
                return;
            }
            SetConfigValue("UpDate", localTime.AddDays(-1).Date.ToString());
            Console.WriteLine("本地更新时间已减少一天");
            Console.ReadLine();
            return;
        }

        /// <summary>
        /// 获取服务器对比的更新时间
        /// </summary>
        /// <returns>2016-01-01</returns>
        public static string GetTheLastUpdateTime()
        {
            //获取服务器xml配置路径
            string filepath = GetConfigValue("ServerXmlUrl");
            //防止有些路径加了反斜杠，有的配置又没有添加，导致重复添加
            filepath = dirPath.TrimEnd('/') + "/" + GetConfigValue("FilePath") + "/" + filepath;
            string LastUpdateTime = string.Empty;
            try
            {
                using (WebClient wc = new WebClient())
                {
                    using (Stream sm = wc.OpenRead(filepath))
                    {
                        using (XmlTextReader xml = new XmlTextReader(sm))
                        {
                            while (xml.Read())
                            {
                                if (xml.Name == "UpdateTime")
                                {
                                    LastUpdateTime = xml.GetAttribute("Date");
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return LastUpdateTime;
        }

        /// <summary>
        /// 修改App.Config的值
        /// </summary>
        /// <param name="key">config key值</param>
        /// <param name="value">修改后的value</param>
        public static void SetConfigValue(string key, string value)
        {
            XmlNode xNode; XmlElement xElem1; XmlElement xElem2;
            xNode = document.SelectSingleNode("//appSettings");
            xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + key + "']");
            if (xElem1 != null) xElem1.SetAttribute("value", value);
            else
            {
                xElem2 = document.CreateElement("add");
                xElem2.SetAttribute("key", key);
                xElem2.SetAttribute("value", value);
                xNode.AppendChild(xElem2);
            }
            document.Save(LocalUpdaterFilePath);
        }

        /// <summary>
        /// 根据key获取value值
        /// </summary>
        /// <param name="appKey">xml key值</param>
        /// <returns>value</returns>
        public static string GetConfigValue(string appKey)
        {
            XmlElement xmlElement = null;
            try
            {
                XmlNode xmlNode = document.SelectSingleNode("//appSettings");
                xmlElement = (XmlElement)xmlNode.SelectSingleNode("//add[@key=\"" + appKey + "\"]");
            }
            catch (XmlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            string result;
            if (xmlElement != null) result = xmlElement.GetAttribute("value");
            else result = string.Empty;
            return result;
        }
    }
}
