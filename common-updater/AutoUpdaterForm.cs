//-----------------------------------------------------------------------
// <copyright company="gpyh" file="AutoUpdaterForm.cs">
//  Copyright (c)  V1.0.0.0  
//  creator:   arison
//  create time:   2021-06-29 10:31:54
//  function description:   auto apdate winform exe application，contain process bar.
//  history version:
//          2021-06-29 arison auto apdate winform exe application，contain process bar.
//          2021-07-17 arison auto backup origin version，for being updated version!  
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Zip;
using System.Text.RegularExpressions;

namespace AutoUpdater
{
    /// <summary>
    /// auto apdate winform exe application，contain process bar.
    /// </summary>
    public partial class AutoUpdaterForm : Form
    {
        private string start_path = null;
        public AutoUpdaterForm()
        {
            InitializeComponent();
            if (!PreCheckForStartUpater())
            {
                //exit application,if u use the method Close,the application will throw exception!
                Application.Exit();
            }
            start_path = Application.StartupPath;
        }
        /// <summary>
        /// instantiate webclient
        /// </summary>
        private WebClient downWebClient = new WebClient();
        /// <summary>
        /// server address
        /// </summary>
        private static string serverUpdateHttpAddress;
        /// <summary>
        /// zip file size
        /// </summary>
        private static long size;
        /// <summary>
        /// zip file count
        /// </summary>
        private static int count;
        /// <summary>
        /// compressed file names' set
        /// </summary>
        private static string[] fileNames;
        /// <summary>
        /// compressed package file name
        /// </summary>
        private static string fileName;
        /// <summary>
        /// which compressed package
        /// </summary>
        private static int num;
        /// <summary>
        /// download all bytes
        /// </summary>
        private static long upsize;
        /// <summary>
        /// current download file bytes
        /// </summary>
        private static long filesize;

        private static string localUpdaterFilePath = null;

        /// <summary>
        /// local executable application's name(donot contain extension,eg:.exe,and do not contain file path)
        /// </summary>
        private static string applicationName = null;

        /// <summary>
        /// local executable application's name(contain extension,eg:.exe,and do not contain file path) 
        /// </summary>
        private static string application = null;

        /// <summary>
        /// 从服务端获取的服务端更新时间
        /// </summary>
        /// <remarks>客户端根据这个时间和本地时间做比较，来确定是否更新客户端的程序</remarks>
        private static string theServerUpdateDate = null;

        /// <summary>
        /// 客户端上次更新的日期
        /// </summary>
        private static string thelocalUpdateDate = null;

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

        private void AutoUpdater_Load(object sender, EventArgs e)
        {
            if (DateTime.Compare(Convert.ToDateTime(theServerUpdateDate, CultureInfo.InvariantCulture),
                Convert.ToDateTime(thelocalUpdateDate, CultureInfo.InvariantCulture)) > 0)
            {
                try
                {
                    Process[] ps = Process.GetProcessesByName(applicationName);
                    for (int i = 0; i < ps.Length; i++)
                    {
                        ps[i].Kill();
                    }
                }
                catch (Exception ex)
                {
                    HelperLog.Write(ex);
                    Close();
                    return;
                }
                UpdaterStart();
            }
            else
                UpdaterClose();

        }

        /// <summary>
        /// 窗体初始化预先检验更新初始条件十分满足
        /// </summary>
        /// <returns>true:满足,false:不满足</returns>
        private bool PreCheckForStartUpater()
        {
            #region 更新执行初始验证，不通过将退出更新程序
            if (!File.Exists(LocalUpdaterFilePath))
            {
                MessageBox.Show("本地更新配置文件updater.xml不存在，无法执行更新程序!");
                return false;
            }
            if (document == null)
            {
                document = new XmlDocument();
                try
                {
                    document.Load(LocalUpdaterFilePath);
                }
                catch (Exception ex)
                {
                    HelperLog.Write(ex);
                    MessageBox.Show("updater.xml格式非法，无法加载!");
                    document = null;
                    return false;
                }
            }
            string iconFilePath = start_path + "\\" + GetConfigValue("application_ico_file_name");
            try
            {
                if (File.Exists(iconFilePath))
                {
                    Icon = Icon.ExtractAssociatedIcon(iconFilePath);
                }
            }
            catch (Exception ex)
            {
                HelperLog.Write(ex);
                MessageBox.Show("加载更新程序ICON文件异常!");
                return false;
            }
            //服务器更新包HTTP地址
            serverUpdateHttpAddress = GetConfigValue("url");
            if (string.IsNullOrEmpty(serverUpdateHttpAddress))
            {
                MessageBox.Show("服务器url路径非法，无法执行更新程序");
                return false;
            }
            //服务器的更新时间
            string thePreUpdateDate = GetTheLastUpdateTime();
            if (string.IsNullOrEmpty(thePreUpdateDate))
            {
                MessageBox.Show("服务器更新时间为空，无法执行更新程序");
                return false;
            }
            DateTime tmpTime;
            if (!DateTime.TryParse(thePreUpdateDate, out tmpTime))
            {
                MessageBox.Show("服务器更新时间格式不正确，无法执行更新程序");
                return false;
            }
            //当前的更新时间
            string localUpDate = GetConfigValue("up_date");
            if (string.IsNullOrEmpty(thePreUpdateDate))
            {
                MessageBox.Show("本地更新时间为空，无法执行更新程序");
                return false;
            }
            if (!DateTime.TryParse(localUpDate, out tmpTime))
            {
                MessageBox.Show("本地更新时间格式不正确，无法执行更新程序");
                return false;
            }
            application = GetConfigValue("application");
            if (string.IsNullOrEmpty(application))
            {
                MessageBox.Show("本地更新配置application节点未设置，无法执行更新程序");
                Close();
                return false;
            }
            //更新待启动应用程序名称（不包含后缀.exe）
            string applictionNameReg = GetConfigValue("application_exe_file_name_reg");
            if (string.IsNullOrEmpty(applictionNameReg))
            {
                //默认文件名称正则格式
                applictionNameReg = @"^([-_a-z0-9\u4e00-\u9fa5]+)\.exe$";
            }
            application = application.Trim();
            try
            {
                Regex rgx = new Regex(applictionNameReg, RegexOptions.IgnoreCase);
                if (!rgx.IsMatch(application))
                {
                    MessageBox.Show("本地更新配置application节点未设置，无法执行更新程序");
                    return false;
                }
                applicationName = rgx.Match(application).Groups[1].Value;
            }
            catch (Exception ex)
            {
                HelperLog.Write(ex);
                MessageBox.Show("本地更新配置application_exe_file_name_reg节点设置异常，无法执行更新程序");
                return false;
            }
            return true;
            #endregion 
        }

        /// <summary>
        /// 关闭当前更新exe,执行实例exe 
        /// </summary>
        /// <returns>true:更新成功,false:更新失败</returns>
        private bool UpdaterClose()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + application;
                Process[] ps = Process.GetProcessesByName(applicationName);
                for (int i = 0; i < ps.Length; i++)
                {
                    ps[i].Kill();
                }
                Process.Start(path);
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {
                HelperLog.Write(ex);
                MessageBox.Show(ex.Message);
                return false;
            }
            //关闭当前更新程序
            Application.Exit();
            return true;
        }

        /// <summary>
        /// 执行更新
        /// </summary>
        private void UpdaterStart()
        {
            string filepath = GetConfigValue("FilePath") + "/";
            float tempf;
            this.downWebClient.DownloadProgressChanged += delegate (object wcsender, DownloadProgressChangedEventArgs ex)
            {
                this.label2.Text = string.Format(CultureInfo.InvariantCulture, "正在下载:{0}  [ {1}/{2} ]", new object[]
                {
                    fileName,
                    ConvertSize(ex.BytesReceived),
                    ConvertSize(ex.TotalBytesToReceive)
                });
                filesize = ex.TotalBytesToReceive;
                tempf = (upsize + ex.BytesReceived) / (float)size;
                //this.progressBar1.Value = Convert.ToInt32(tempf * 100f);
                this.progressBar1.Value = Convert.ToInt32(tempf);
                this.progressBar2.Value = ex.ProgressPercentage;
            };
            this.downWebClient.DownloadFileCompleted += delegate (object wcsender, AsyncCompletedEventArgs ex)
            {
                if (ex.Error != null)
                {
                    MessageBox.Show(ex.Error.Message);
                }
                else
                {
                    if (File.Exists(start_path + "\\" + fileName))
                        File.Delete(start_path + "\\" + fileName);
                    UnZip(start_path + "\\" + filepath + fileName, start_path + "\\", "");
                    upsize += filesize;
                    if (fileNames.Length > num)
                    {
                        DownloadFile(num);
                    }
                    else
                    {
                        SetConfigValue("up_date", GetTheLastUpdateTime());
                        UpdaterClose();
                    }
                }
            };
            size = GetUpdateSize(serverUpdateHttpAddress + "UpdateSize.ashx");
            if (size == 0L)
            {
                UpdaterClose();
            }
            num = 0;
            upsize = 0L;
            UpdateList();
            if (fileNames != null)
            {
                if (!Directory.Exists(start_path + "\\" + filepath))
                    Directory.CreateDirectory(start_path + "\\" + filepath);
                this.DownloadFile(0);
            }
        }

        private static void UpdateList()
        {
            string filepath = GetConfigValue("FilePath") + "/";
            string xmlPath = serverUpdateHttpAddress + filepath + "AutoUpdater.xml"; //更新的xml所在的服务器全路径
            WebClient wc = new WebClient();
            DataSet ds = new DataSet();
            ds.Locale = CultureInfo.InvariantCulture;
            try
            {
                Stream sm = wc.OpenRead(xmlPath);
                ds.ReadXml(sm);
                DataTable dt = ds.Tables["UpdateFileList"];  //读取到的zip文件名
                StringBuilder sb = new StringBuilder();
                count = dt.Rows.Count;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i == 0)
                        sb.Append(dt.Rows[i]["UpdateFile"].ToString());
                    else
                        sb.Append("," + dt.Rows[i]["UpdateFile"].ToString());
                }
                fileNames = sb.ToString().Split(new char[] { ',' });
                sm.Close();
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 获取zip文件的大小
        /// </summary>
        /// <param name="filePath">服务器文件路径</param>
        /// <returns>zip文件的大小</returns>
        private static long GetUpdateSize(string filePath)
        {
            long len = 0L;
            try
            {
                WebClient wc = new WebClient();
                Stream sm = wc.OpenRead(filePath);
                XmlTextReader xr = new XmlTextReader(sm);
                while (xr.Read())
                {
                    if (xr.Name == "UpdateSize")
                    {
                        len = Convert.ToInt64(xr.GetAttribute("Size"), CultureInfo.InvariantCulture);
                        break;
                    }
                }
                xr.Close();
                sm.Close();
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return len;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="arry"></param>
        private void DownloadFile(int arry)
        {
            try
            {
                string filepath = GetConfigValue("FilePath") + "/";
                num++;
                fileName = fileNames[arry];
                this.label1.Text = string.Format(
                    CultureInfo.InvariantCulture, "更新进度 {0}/{1}  [ {2} ]",
                    new object[] { num, count, ConvertSize(size) });
                this.progressBar2.Value = 0;
                this.downWebClient.DownloadFileAsync(
                    new Uri(serverUpdateHttpAddress + filepath + fileName),
                    start_path + "\\" + filepath + fileName);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 把zip文件解压缩
        /// </summary>
        /// <param name="fileToUpZip">获取本地zip文件的位置 start_path + "\\" + LabelPrint_AutoUpdater + "/" + LabelPrint.zip</param>
        /// <param name="zipedFolder">存放在本地zip的所在文件夹 start_path + "\\"</param>
        /// <param name="password">""</param>
        public static void UnZip(string fileToUpZip, string zipedFolder, string password)
        {
            if (File.Exists(fileToUpZip))
            {
                ZipInputStream s = null;
                ZipEntry theEntry = null;
                FileStream streamWriter = null;
                try
                {
                    s = new ZipInputStream(File.OpenRead(fileToUpZip));
                    s.Password = password;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        if (theEntry.Name != string.Empty)
                        {
                            string fileName = Path.Combine(zipedFolder, theEntry.Name);
                            if (fileName.EndsWith("/") || fileName.EndsWith("//"))
                            {
                                Directory.CreateDirectory(fileName);
                            }
                            else
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                                streamWriter = File.Create(fileName);
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    int size = s.Read(data, 0, data.Length);
                                    if (size <= 0)
                                        break;
                                    streamWriter.Write(data, 0, size);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (streamWriter != null)
                    {
                        streamWriter.Close();
                    }
                    if (s != null)
                    {
                        s.Close();
                    }
                    GC.Collect();
                    GC.Collect(1);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteSize"></param>
        /// <returns></returns>
        private string ConvertSize(long byteSize)
        {
            float tempf = (float)byteSize;
            string str;
            if (tempf / 1024f > 1f)
            {
                if (tempf / 1024f / 1024f > 1f)
                    str = (tempf / 1024f / 1024f).ToString("##0.00", CultureInfo.InvariantCulture) + "MB";
                else
                    str = (tempf / 1024f).ToString("##0.00", CultureInfo.InvariantCulture) + "KB";
            }
            else
            {
                str = tempf.ToString(CultureInfo.InvariantCulture) + "B";
            }
            return str;
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
            filepath = serverUpdateHttpAddress.TrimEnd('/') + "/" + GetConfigValue("FilePath") + "/" + filepath;
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
            catch (WebException ex)
            {
                HelperLog.Write(ex);
            }
            catch (Exception ex)
            {
                HelperLog.Write(ex);
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
                MessageBox.Show(ex.Message, "提示");
            }
            string result;
            if (xmlElement != null) result = xmlElement.GetAttribute("value");
            else result = string.Empty;
            return result;
        }
    }
}
