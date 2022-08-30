using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace CTX
{
    ////// พัฒนาโดยเพจ เขียนโปรแกรมยามว่าง ////////
    ////// พัฒนาโดยเพจ เขียนโปรแกรมยามว่าง ////////
    public class GetAppName
    {
        public static string PROCRESSNAME = "dnplayer"; //ชิอ process เอาไว้เช็คโปรแกรม
        public static string APP = "LDPlayer-1"; //ชื่อของ nox
        public static string CLASS = "LDPlayerMainFrame";// Class ของ nox
        public static IntPtr appname = Win32Bot.FindWindow(CLASS, APP); //เอา app และ class มารวมกันเพื่อไปใช้งาน
    } ////ดึงจาก autoit ////
    ////// พัฒนาโดยเพจ เขียนโปรแกรมยามว่าง ////////
}

    