//-----------------------------------------------------------------------
// <copyright 开源 file="CodeTextentBlock.cs">
//  Copyright (c)  V1.0.0.0  
//  创建作者:   刘少林
//  创建时间:   2017-02-05 13:23:21
//  功能描述:   代码中字符常量文本内容集合类
//  历史版本:
//          2017-02-05 13:23:21 刘少林 创建CodeTextentBlock类
// </copyright>
//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright open code source file="CodeTextentBlock.cs">
//  Copyright (c)  V1.0.0.0  
//  creator:   arison
//  create time:   2021-06-29 10:31:54
//  function description:   const text variables in the code source
//  history version:
//          2021-06-29 arison create CodeTextentBlock class
// </copyright>
//-----------------------------------------------------------------------

namespace AutoUpdater
{
    /// <summary>
    /// 代码中字符常量文本内容集合类
    /// const text variables in the code source
    /// </summary>
    sealed public class CodeTextentBlock
    {
        static private bool isChinaMode = true;
        static CodeTextentBlock()
        {
            isChinaMode = "1".Equals(XmlHelper.GetConfigValue("is_cn"));
        }

        /// <summary>
        /// 时间
        /// Time
        /// </summary>
        public static string Time
        {
            get
            {
                return isChinaMode ? "时间" : "Time";
            }
        }

        /// <summary>
        /// 内容
        /// Content
        /// </summary>
        public static string Content
        {
            get
            {
                return isChinaMode ? "内容" : "Content";
            }
        }

        /// <summary>
        /// 方法信息
        /// Method Information
        /// </summary>
        public static string MethodInformation
        {
            get
            {
                return isChinaMode ? "方法信息" : "Method Information";
            }
        }

        /// <summary>
        /// 异常
        /// Exception Message
        /// </summary>
        public static string ExceptionMessage
        {
            get
            {
                return isChinaMode ? "异常" : "Exception Message";
            }
        }

        /// <summary>
        /// 堆栈
        /// Stack
        /// </summary>
        public static string Stack
        {
            get
            {
                return isChinaMode ? "堆栈" : "Stack";
            }
        }

        /// <summary>
        /// 本地更新配置文件updater.xml不存在，无法执行更新程序!
        /// local updater.xml is not found,do not update!
        /// </summary>
        public static string UpdaterConfigIsNotFound
        {
            get
            {
                return isChinaMode ? "本地更新配置文件updater.xml不存在，无法执行更新程序!" : "local updater.xml is not found,do not update!";
            }
        }

        /// <summary>
        /// 加载updater.xml文件异常!
        /// loading updater.xml file throw exception
        /// </summary>
        public static string LoadedUpdaterConfigIsException
        {
            get
            {
                return isChinaMode ? "加载updater.xml文件异常!" : "loading updater.xml file throw exception";
            }
        }

        /// <summary>
        /// 加载更新程序ICON文件异常!
        /// loading updater icon file throw exception
        /// </summary>
        public static string LoadedUpdaterIconFileIsException
        {
            get
            {
                return isChinaMode ? "加载更新程序ICON文件异常!" : "loading updater icon file throw exception";
            }
        }

        /// <summary>
        /// 服务器url路径非法，无法执行更新程序!
        /// server url is invalid,so do not update!
        /// </summary>
        public static string ServerUrlIsInvalid
        {
            get
            {
                return isChinaMode ? "服务器url路径非法，无法执行更新程序!" : "server url is invalid,so do not update!";
            }
        }

        /// <summary>
        /// 服务器更新时间为空，无法执行更新程序!
        /// server update time is empty,so do not update!
        /// </summary>
        public static string ServerUpdateTimeIsEmpty
        {
            get
            {
                return isChinaMode ? "服务器更新时间为空，无法执行更新程序!" : "server update time is empty,so do not update!";
            }
        }

        /// <summary>
        /// 服务器更新时间格式不正确，无法执行更新程序!
        /// server updated time's format is invalid,so do not update!
        /// </summary>
        public static string ServerUpdateTimeFormatIsInvalid
        {
            get
            {
                return isChinaMode ? "服务器更新时间格式不正确，无法执行更新程序!" : "server updated time's format is invalid,so do not update!";
            }
        }

        /// <summary>
        /// 本地更新时间为空，无法执行更新程序!
        /// local updated time is empty,so do not update!
        /// </summary>
        public static string LocalUpdateTimeIsEmpty
        {
            get
            {
                return isChinaMode ? "本地更新时间为空，无法执行更新程序!" : "local updated time is empty,so do not update!";
            }
        }

        /// <summary>
        /// 本地更新时间格式不正确，无法执行更新程序!
        /// local updated time's format is invalid,so do not update!
        /// </summary>
        public static string LocalUpdatedTimeFormatIsInvalid
        {
            get
            {
                return isChinaMode ? "本地更新时间格式不正确，无法执行更新程序!" : "local updated time's format is invalid,so do not update!";
            }
        }

        /// <summary>
        /// 本地更新配置application节点未设置，无法执行更新程序!
        /// local updater.xml file's applicaiton node is not configed,so do not update!
        /// </summary>
        public static string LocalUpdaterConfigApplicationNodeIsEmpty
        {
            get
            {
                return isChinaMode ? "本地更新配置application节点未设置，无法执行更新程序!" : "local updater.xml file's applicaiton node is not configed,so do not update!";
            }
        }

        /// <summary>
        /// 本地更新配置application_exe_file_name_reg节点设置异常，无法执行更新程序!
        /// local updater.xml file's application_exe_file_name_reg node is exception,so do not update!
        /// </summary>
        public static string LocalUpdaterConfigNameRegNodeIsException
        {
            get
            {
                return isChinaMode ? "本地更新配置application_exe_file_name_reg节点设置异常，无法执行更新程序!" : "local updater.xml file's application_exe_file_name_reg node is exception,so do not update!";
            }
        }

        /// <summary>
        /// 正在下载!
        /// Dowloading!
        /// </summary>
        public static string Dowloading
        {
            get
            {
                return isChinaMode ? "正在下载!" : "Dowloading!";
            }
        }

        /// <summary>
        /// 更新进度!
        /// Updater's progress!
        /// </summary>
        public static string UpdaterProgress
        {
            get
            {
                return isChinaMode ? "更新进度!" : "Updater's progress!";
            }
        }
        
    }
}
