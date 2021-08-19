//-----------------------------------------------------------------------
// <copyright company="工品一号" file="XmlHelper.cs">
//  Copyright (c)  V1.0.0.0  
//  创建作者:   刘少林
//  创建时间:   2017-02-05 13:23:21
//  功能描述:   XML文档帮助类
//  历史版本:
//          2017-02-05 13:23:21 刘少林 创建XmlHelper类
// </copyright>
//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright open code source file="XmlHelper.cs">
//  Copyright (c)  V1.0.0.0  
//  creator:   arison
//  create time:   2021-06-29 10:31:54
//  function description:   xml file operater's helper class
//  history version:
//          2021-06-29 arison create xml file operater's helper class
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Windows.Forms;
using System.Xml;

namespace AutoUpdater
{
    /// <summary>
    /// XML文档帮助类
    /// xml file operater's helper class
    /// </summary>
    /// <remarks>
    /// 通过Init方法传递XML文件路径，所有操作如需保存，则直接保存到此传入的路径
    /// if some operates will be saved ,then it will be save into the path that from the Init method's parameter :filePath
    /// </remarks>
    sealed public class XmlHelper
    {
        static private XmlDocument document = null;
        static private string path = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public static void Init(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            string _path = filePath.Trim();
            if (_path.Length > 0 &&
                path != _path)
            {
                document = new XmlDocument();
                path = _path;
                try
                {
                    document.Load(path);
                }
                catch (Exception ex)
                {
                    document = null;
                    path = null;
                    LogHelper.Write(ex);
                    throw new Exception("updater.xml文件异常，无法加载!");
                }
            }
        }

        /// <summary>
        /// 修改updater.xml的值
        ///  set the updater.xml file 's key/value
        /// </summary>
        /// <param name="key">
        /// config key值
        /// xml key's name
        /// </param>
        /// <param name="value">
        /// 修改后的value
        /// updated value(will be updated into updater.xml file)
        /// </param>
        public static void SetConfigValue(string key, string value)
        {
            if (document == null)
            {
                throw new Exception("请先执行Init初始化对应Xml文件!");
            }
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
            document.Save(path);
        }

        /// <summary>
        /// 根据key获取value值
        /// get the value by the key name
        /// </summary>
        /// <param name="appKey">
        /// xml key值
        /// xml key's name
        /// </param>
        /// <returns>value</returns>
        public static string GetConfigValue(string appKey)
        {
            if (document == null)
            {
                throw new Exception("请先执行Init初始化对应Xml文件!");
            }
            XmlElement xmlElement = null;
            try
            {
                XmlNode xmlNode = document.SelectSingleNode("//appSettings");
                xmlElement = (XmlElement)xmlNode.SelectSingleNode("//add[@key=\"" + appKey + "\"]");
            }
            catch (XmlException ex)
            {
                throw ex;
            }
            string result;
            if (xmlElement != null) result = xmlElement.GetAttribute("value");
            else result = string.Empty;
            return result;
        }
    }
}
