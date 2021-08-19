//-----------------------------------------------------------------------
// <copyright 开源 file="LogHelper.cs">
//  Copyright (c)  V1.0.0.0  
//  创建作者:   刘少林
//  创建时间:   2017-02-05 13:23:21
//  功能描述:   日志帮助类
//  历史版本:
//          2017-02-05 13:23:21 刘少林 创建LogHelper类
// </copyright>
//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright open code source file="LogHelper.cs">
//  Copyright (c)  V1.0.0.0  
//  creator:   arison
//  create time:   2021-06-29 10:31:54
//  function description:   log operater's helper class
//  history version:
//          2021-06-29 arison create log operater's helper class
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace AutoUpdater
{
    /// <summary>
    /// 日志帮助类
    /// log operater's helper class
    /// </summary>
    /// <remarks>
    /// 自定义的日志操作类,日志保留7天
    /// customize log operater's class ,and the log file will be saved for seven days
    /// </remarks>
    sealed public class LogHelper
    {
        private LogHelper() { }

        /// <summary>
        /// 获取调用方法信息
        /// get the invoked method information
        /// </summary>
        /// <returns>
        /// 方法信息
        /// method information
        /// </returns>
        private static string GetMethodInfo()
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            //获取调用此日志的方法信息(日志内部调用此方法，所以Frame基数为2开始)
            //get method information from invoked mehod (inner log method invoked this method,so Frame base number begin 2)
            MethodBase method = st.GetFrame(2).GetMethod();
            StringBuilder builder = new StringBuilder(256);
            //方法名所在命名空间和类
            //namespace and class contain the method
            builder.AppendFormat("\r\n方法所在命名空间和类:{0}\r\n", method.ReflectedType.FullName);
            //调用此日志方法的方法名称
            //the method invoke the log method 
            builder.AppendFormat("方法名称:{0}\r\n", method.Name);
            //获取方法参数签名
            // get the method's paramters signature
            ParameterInfo[] parameters = method.GetParameters();
            foreach (ParameterInfo info in parameters)
            {
                builder.AppendFormat("参数类型:{0},参数名称:{1}\r\n", info.ParameterType.FullName, info.Name);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 获取实体信息
        /// get the entity information
        /// </summary>
        /// <typeparam name="T">
        /// 实体类型
        /// entity type
        /// </typeparam>
        /// <param name="entity">
        /// 实体对象
        /// entity object
        /// </param>
        /// <returns>
        /// 实体信息
        /// entity information
        /// </returns>
        private static string GetEntityInfo<T>(T entity) where T : class, new()
        {
            string entityInfo = string.Empty;
            using (StringWriter sw = new StringWriter())
            {
                //创建XML命名空间
                //create xml namespace
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(sw, entity, ns);
                }
                catch
                {

                }
                entityInfo = sw.ToString();
            }
            return entityInfo;
        }

        /// <summary>
        /// 记录实体日志
        /// record entity log
        /// </summary>
        /// <typeparam name="T">
        /// 实体类型
        /// entity type
        /// </typeparam>
        /// <param name="entity">
        /// 实体对象
        /// entity object
        /// </param>
        /// <param name="content">
        /// 备注
        /// note
        /// </param>
        /// <param name="type">
        /// 日志类型
        /// log type
        /// </param>
        /// <remarks>
        /// 将对象序列化记录到文本日志中去,方便后期查询日志
        /// let the serial object save into text log content,and convenient query the log in the future
        /// </remarks>
        public static void Write<T>(T entity, string content, LogTypes type = LogTypes.Other) where T : class, new()
        {

            Write(GetEntityInfo(entity) + "\r\n" + content, GetMethodInfo(), null, type, "");
        }

        /// <summary>
        /// 记录实体日志
        /// record entity log
        /// </summary>
        /// <typeparam name="T">
        /// 实体类型
        /// entity type
        /// </typeparam>
        /// <param name="entity">
        /// 实体对象
        /// entity object
        /// </param>
        /// <param name="type">
        /// 日志类型
        /// log type
        /// </param>
        public static void Write<T>(T entity, LogTypes type = LogTypes.Other) where T : class, new()
        {
            Write(GetEntityInfo(entity), GetMethodInfo(), null, type, "");
        }

        /// <summary>
        /// 记录实体日志
        /// record entity log
        /// </summary>
        /// <typeparam name="T">
        /// 实体类型
        /// entity type
        /// </typeparam>
        /// <param name="entity">
        /// 实体对象
        /// entity object
        /// </param>
        /// <param name="exception">
        /// 异常对象
        /// exception object
        /// </param>
        /// <param name="type">
        /// 日志类型
        /// log type
        /// </param>
        public static void Write<T>(T entity, Exception exception, LogTypes type = LogTypes.Exception) where T : class, new()
        {
            Write(GetEntityInfo(entity), GetMethodInfo(), exception, type, "");
        }

        /// <summary>
        /// 日志记录
        /// save the log content into log file
        /// </summary>
        /// <param name="content">
        /// 日志内容
        /// log content
        /// </param>
        /// <param name="type">
        /// 日志类型
        /// log content
        /// </param>
        public static void Write(string content, LogTypes type = LogTypes.Other)
        {
            Write(content, GetMethodInfo(), null, type, "");
        }

        /// <summary>
        /// 日志记录
        /// save the log content into log file
        /// </summary>
        /// <param name="exception">
        /// 异常对象
        /// exception object
        /// </param>
        /// <param name="content">
        /// 备注内容
        /// note content
        /// </param>
        /// <param name="type">
        /// 日志类型
        /// log type
        /// </param>
        public static void Write(Exception exception, string content, LogTypes type = LogTypes.Exception)
        {
            Write(content, GetMethodInfo(), exception, type, "");
        }

        /// <summary>
        /// 日志记录
        /// save the log content into log file
        /// </summary>
        /// <param name="exception">
        /// 异常对象
        /// exception object
        /// </param>
        /// <param name="type">
        /// 日志类型
        /// log type
        /// </param>
        public static void Write(Exception exception, LogTypes type = LogTypes.Exception)
        {
            Write(string.Empty, GetMethodInfo(), exception, type, "");
        }

        /// <summary>
        /// 系统日志写入
        /// save the log content into log file
        /// </summary>
        /// <param name="content">
        /// 自定义日志内容
        /// customize log content
        /// </param>
        /// <param name="methodInfo">
        /// 日志发生所在方法信息
        /// log will be build in the method
        /// </param>
        /// <param name="exception">
        /// 异常日志
        /// exception object
        /// </param>
        /// <param name="type">
        /// 日志类型
        /// log type
        /// </param>
        /// <param name="savePath">
        /// 日志保存路径(默认存在应用程序所在文件夹下),后期扩展可用于网络传输日志操作
        /// the path for the log(by default it will be saved the path of applicaiton),then it can be transported in the internet
        /// </param>
        public static void Write(string content, string methodInfo, Exception exception, LogTypes type, string savePath)
        {
            if (!string.IsNullOrEmpty(content) || exception != null)
            {
                //不存在自定义日志内容和异常对象时候无需写入日志文件
                //if exception is not exist or log content is empty,the Write method will be not runned
                string timeString = DateTime.Now.ToString();
                StringBuilder append = new StringBuilder();
                append.Append(CodeTextentBlock.Time).AppendFormat(":{0}\r\n", timeString);
                if (!string.IsNullOrEmpty(content))
                {
                    append.Append(CodeTextentBlock.Content).AppendFormat(":{0}\r\n", content);
                }
                if (!string.IsNullOrEmpty(methodInfo))
                {
                    append.Append(CodeTextentBlock.MethodInformation).AppendFormat(":{0}\r\n", methodInfo);
                }
                if (exception != null)
                {
                    append.Append(CodeTextentBlock.ExceptionMessage).AppendFormat(":{0}\r\n", exception.Message);
                    append.Append(CodeTextentBlock.Stack).AppendFormat(":{0}\r\n", exception.StackTrace);
                }
                append.Append("****************************");
                WriteLog(append.ToString(), type, savePath);
            }
        }

        /// <summary>
        /// 写入日志
        /// save the log content
        /// </summary>
        /// <param name="info">
        /// 日志内容
        /// log content
        /// </param>
        /// <param name="type">
        /// 日志类型
        /// log type
        /// </param>
        /// <param name="savePath">
        /// 日志保存路径(默认存在应用程序所在文件夹下),后期扩展可用于网络传输日志操作
        /// the path for the log(by default it will be saved the path of applicaiton),then it can be transported in the internet
        /// </param>
        private static void WriteLog(string info, LogTypes type, string savePath = "")
        {
            string directory = string.Empty;
            try
            {
                //日志可通过传入绝对路径保存
                //log can be saved into the absolute path
                if (string.IsNullOrEmpty(savePath))
                {
                    //获取当前执行程序所在绝对路径(不包最后斜杠"\"及后续执行文件名称等)
                    //get the current application's path(it can not contain the slash and the executable file name etc.)
                    directory = System.AppDomain.CurrentDomain.BaseDirectory;
                    if (string.IsNullOrEmpty(directory))
                    {
                        if (HttpContext.Current != null)
                        {
                            //网站类请求日志存储
                            //WCF部署到IIS上的时候，HttpContext.Current还是null
                            //website's log will be saved by special
                            //when WCF service is deployed on the IIS server,HttpContext.Current is null too!
                            directory = HttpContext.Current.Server.MapPath("~");
                        }
                        if (directory == string.Empty)
                        {
                            directory = Environment.CurrentDirectory;
                        }
                    }
                }
                //加入下级log文件夹,避免程序异常导致删除其他文件夹
                // create the log folder,avoid delete other important file folder by the speical exception.
                string rootLogPath = directory + "\\log\\";
                //获取已当前天(年月日)命名的文件夹
                //get current day(year month day) name the log folder
                string logPath = directory + "\\log\\" + DateTime.Now.Date.ToString("yyyy-MM-dd");
                DirectoryInfo directInfo = null;
                //检测log下文件夹超过7个则删除最早的文件夹内容
                //原则上只保留7天的异常日志
                //if the log folder contain seven folders or more,the application will delete the primary folders
                //
                if (Directory.Exists(rootLogPath))
                {
                    directInfo = new DirectoryInfo(rootLogPath);
                    //获取log文件夹下的文件夹数量
                    //get the file folder's count in the log folder
                    DirectoryInfo[] directorys = directInfo.GetDirectories();
                    List<DirectoryInfo> willRemoveList = new List<DirectoryInfo>();
                    if (directorys.Length > 30)
                    {
                        //将log文件夹下的文件夹按时间降序排序
                        //let the folders are sorted by descending time in the log folder
                        Array.Sort(directorys, new DirectoryInfoComparer());
                        for (int i = 0; i < directorys.Length - 30; i++)
                        {
                            //添加待删除的文件夹
                            //add the folders that will be deleted quickly
                            willRemoveList.Add(directorys[i]);
                        }
                        if (willRemoveList.Count > 0)
                        {
                            for (int i = 0; i < willRemoveList.Count; i++)
                            {
                                //删除文件夹及以下文件
                                //delete the folders and the files in the folders
                                willRemoveList[i].Delete(true);
                            }
                        }
                    }
                }
                if (!Directory.Exists(logPath))
                {
                    //以当天日期生产的文件夹不存在则重新创建此文件夹
                    //if there are not exist the current day folder,then rebuild the folder by the current day
                    directInfo = Directory.CreateDirectory(logPath);
                }
                else
                {
                    directInfo = new DirectoryInfo(logPath);
                }
                //获取当天日期文件夹下的所有文件
                //get all of the files from the current date folder
                FileInfo[] files = directInfo.GetFiles();
                if (files.Length >= 100)
                {
                    //单个文件夹下不允许日志文件超过100个
                    //single folder refuse contain 100 files more!
                    return;
                }
                string filePath = string.Empty;
                FileInfo[] logFiles = new FileInfo[files.Length];
                int index = 0;
                foreach (FileInfo file in files)
                {
                    if (file.Name.IndexOf(type.ToString(), StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        logFiles[index++] = file;
                    }
                }
                int logCount = index;
                //获取日志类型名称
                //get log type name
                string typeName = type.ToString();

                filePath = logPath + @"\" + typeName + "-" + (logCount > 0 ? (logCount - 1) : 0) + ".txt";

                //指定文件路径下的文件已存在，则根据文件大小情况，进行追加日志
                //if the file existed in the special path,then it will append the log content,if the file size is little！
                if (File.Exists(filePath))
                {
                    FileInfo f = new FileInfo(filePath);
                    //超过1M的日志文件重新生成日志文件
                    //it will create another log file,if currrent file's size more than 1M
                    if (f.Length >= 1048576)
                    {
                        filePath = logPath + @"\" + typeName + "-" + logCount + ".txt";
                        FileStream stream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.SetLength(0);
                        stream.Close();
                    }
                    using (StreamWriter wr = new StreamWriter(new FileStream(filePath, FileMode.Append)))
                    {
                        wr.WriteLine(info);
                        wr.Flush();
                    }
                }
                else
                {
                    FileStream stream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.SetLength(0);
                    stream.Close();
                    using (StreamWriter wr = new StreamWriter(new FileStream(filePath, FileMode.Append)))
                    {
                        wr.WriteLine(info);
                        wr.Flush();
                    }
                }
            }
            catch
            {

            }
        }
    }

    /// <summary>
    /// 用于比较DirectoryInfo类型对象的比较类
    /// the comparer class (it will be used to compare two directoryinfo classes)
    /// </summary>
    /// <remarks>
    /// 只用于比较DirectoryInfo对象
    /// just only compare two directoryinfo object
    /// </remarks>
    public class DirectoryInfoComparer : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            DirectoryInfo prev = ((DirectoryInfo)x);
            DirectoryInfo next = ((DirectoryInfo)y);
            return DateTime.Compare(prev.CreationTime, next.CreationTime);
        }
    }

    /// <summary>
    /// 记录日志类型
    /// log type enum
    /// </summary>
    public enum LogTypes
    {
        /// <summary>
        /// 异常日志
        /// exception log
        /// </summary>
        [Description("异常日志")]
        Exception = 0x0,

        /// <summary>
        /// 特殊日志
        /// special log
        /// </summary>
        [Description("特殊日志")]
        Special = 0x02,

        /// <summary>
        /// 操作日志
        /// </summary>
        [Description("操作日志")]
        Operate = 0x04,

        /// <summary>
        /// 登陆注销
        /// login and logout log
        /// </summary>
        [Description("登陆注销")]
        Login = 0x08,

        /// <summary>
        /// 数据变更
        /// data change log
        /// </summary>
        [Description("数据变更")]
        DataChange = 0x16,

        /// <summary>
        /// 其他日志
        /// others' log
        /// </summary>
        [Description("其他日志")]
        Other = 0x32,

        /// <summary>
        /// 普通错误
        /// normal error log
        /// </summary>
        [Description("普通错误")]
        Fault = 0x64,
    }
}
