//-----------------------------------------------------------------------
// <copyright company="工品一号" file="Program.cs">
//  Copyright (c)  V1.0.0.0  
//  创建作者:   刘少林
//  创建时间:   2021-06-29 10:31:54
//  功能描述:   应用程序的主入口点
//  历史版本:
//          2021-06-29 刘少林 应用程序的主入口点
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AutoUpdater
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AutoUpdaterForm());
        }
    }
}
