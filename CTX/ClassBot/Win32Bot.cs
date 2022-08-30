using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace CTX
{
    ////// พัฒนาโดยเพจ เขียนโปรแกรมยามว่าง ////////
    ////// พัฒนาโดยเพจ เขียนโปรแกรมยามว่าง ////////
    public class Win32Bot
    {
        //dll ต่างๆที่จะต้องเรียกมาใช้งาน //google
        [DllImport("User32.dll")]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);
        [DllImport("User32.dll")]
        public static extern int FindWindowEx(int hwndParent, int hwndChildAfter, string strClassName, string strWindowName);
        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr SetFocus(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern Int32 SendMessage(
           IntPtr hWnd,
           int Msg,
           int wParam,
           int lParam);
        public static int MakeLParam(int LoWord, int HiWord)
        {
            return (int)((HiWord << 16) | (LoWord & 0xFFFF));
        }
        [DllImport("User32.dll")]
        public static extern long SetCursorPos(int x, int y);
        [DllImport("User32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImport("user32.dll")]

        //dll ต่างๆที่จะต้องเรียกมาใช้งาน //google
        //////////////////////////////////
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        public const int WM_COMMAND = 0x111;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int WM_LBUTTONDBLCLK = 0x203;
        public const int WM_RBUTTONDOWN = 0x204;
        public const int WM_RBUTTONUP = 0x205;
        public const int WM_RBUTTONDBLCLK = 0x206;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_MOUSEHWHEEL = 0x020E;
        public static int WinSizeWidth;
        public static int WinSizeHeight;
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;
        //เป็นค่า hex ไว้ตั่งค่า ตอนนี้ยังไม่ได้ใช้

        ///////// public enum WindowShowStyle : uint
        ////// {
        //////     Hide = 0,
        //////     ShowNormal = 1,
        //////     ShowMinimized = 2,
        //////     ShowMaximized = 3,
        //////     Maximize = 3,
        //////     ShowNormalNoActivate = 4,
        //////     Show = 5,
        //////     Minimize = 6,
        //////     ShowMinNoActivate = 7,
        //////     ShowNoActivate = 8,
        //////     Restore = 9,
        //////     ShowDefault = 10,
        //////     ForceMinimized = 11
        ////// } // ÂÑ§äÁèãªé
        public static Size GetControlSize(IntPtr iHandle) //get size program
        {
            RECT pRect;
            Size cSize = new Size();
            GetWindowRect(iHandle, out pRect);
            cSize.Width = pRect.Right - pRect.Left;
            cSize.Height = pRect.Bottom - pRect.Top;
            WinSizeWidth = cSize.Width;
            WinSizeHeight = cSize.Height;
            return cSize;
        }//ดึงขนาดจอเกมส์หรือโปรแกรม 
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        public static void MouseClick(IntPtr iHandle, int Gx, int Gy, string TypeClick = "LEFT")/// mousemove in program no sceen
        {
            POINT p = new POINT();
            p.x = Convert.ToInt16(Gx);
            p.y = Convert.ToInt16(Gy);
            Win32Bot.ClientToScreen(iHandle, ref p);
            Win32Bot.SetCursorPos(p.x, p.y);
            if (TypeClick == "LEFT")
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, p.x, p.y, 0, 0);
            }
            else if (TypeClick == "RIGHT")
            {
                mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, p.x, p.y, 0, 0);
            }
        }//คลิกเเมาส์
        public static void MouseMove(IntPtr iHandle, int x, int y) /// mousemove in program no sceen
        {
            POINT p = new POINT();
            p.x = Convert.ToInt16(x);
            p.y = Convert.ToInt16(y);
            Win32Bot.ClientToScreen(iHandle, ref p);
            Win32Bot.SetCursorPos(p.x, p.y);
        }//ย้ายเมาส์
        public static void ActiveWindow(IntPtr iHandle) //setactivate
        {
            IntPtr h = iHandle;
            ShowWindow(h, SW_SHOW);
            SetForegroundWindow(h);
            SetFocus(h);
        }//แอคทืฟโปรแกรม
        public static void HideApp(IntPtr iHandle)
        {
            IntPtr h = iHandle;
            ShowWindow(h, SW_HIDE);
        }//ซ้อนโปรแกรม
        public static void ShowAPP(IntPtr iHandle)
        {
            IntPtr h = iHandle;
            ShowWindow(h, SW_SHOW);
        }//แสดงโปรแกรม

   
        public static void MouseClickDragBG(IntPtr iHandle, int Xstart, int Ystart, int XEnd, int YEnd, int Delay) //STARTPOINT X,Y TO END POINT X,Y
        {
            SendMessage(iHandle, WM_LBUTTONDOWN, 0x00000001, MakeLParam(XEnd, YEnd));
            sleep(Delay);
            SendMessage(iHandle, WM_MOUSEMOVE, 0x00000001, MakeLParam(Xstart, Ystart));
            sleep(Delay);
            SendMessage(iHandle, WM_MOUSEMOVE, 0x00000001, MakeLParam(Xstart,Ystart));
            sleep(Delay);
            SendMessage(iHandle, WM_LBUTTONUP, 0x00000001, MakeLParam(XEnd, YEnd));
            SendMessage(iHandle, WM_LBUTTONUP, 0x00000001, MakeLParam(XEnd, YEnd));

        }//คลิกลากเมาส์จากตำแหน่ง แรก ไปยังตำแหน่ง สุดท้ายที่ต้องการ ไม่ขยับเมาส์
        public static void Clicktobg(IntPtr iHandle, int x, int y, int clickcount = 1) //click bg
        {
            SendMessage(iHandle, WM_LBUTTONDOWN, 0x00000001, MakeLParam(x, y));
            SendMessage(iHandle, WM_LBUTTONUP, 0x00000000, MakeLParam(x, y));
        }//คลิกโดยที่ไม่ขยับเมาส์
        public static void ClickHold(IntPtr iHandle, int x, int y, int slp) //not 100%
        {
            POINT p = new POINT();
            p.x = Convert.ToInt16(x);
            p.y = Convert.ToInt16(y);
            ClientToScreen(iHandle, ref p);
            SetCursorPos(p.x, p.y);
            mouse_event(MOUSEEVENTF_LEFTDOWN, p.x, p.y, 0, 0);
        
            sleep(slp);
      
            mouse_event(MOUSEEVENTF_LEFTUP, p.x, p.y, 0, 0);
        }//คลิกค้างยังไม่เสร็จเท่าไหร่แต่ใช้ได
        public static void sleep(int slp)
        {
            Thread.Sleep(slp);
        }//พักโปรแกรมหรือเรียกว่า สลิปป
        public static void WindowMove(IntPtr iHandle, int x, int y)
        {
            GetControlSize(iHandle);
            MoveWindow(iHandle, x, y, WinSizeWidth, WinSizeHeight, true);
        }//ย้ายหน้าต่างไปยังตำแหน่งต่างๆ
        public static bool CheckProcess(string processname)
        {
            bool StatusCheck = false;
            Process[] p = Process.GetProcessesByName(processname);
            if (p.Length > 0)
            {
                StatusCheck = true;
            }
            return StatusCheck;
        } //เช็คโปรแแกรมว่าเปิดอยุ่หรือป่าว
        public static string GetWinTitle(IntPtr iHandle)
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            if (GetWindowText(iHandle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }//ดึงชื่อ title
        public static void sendKeyBG(IntPtr iHandle) //NOT WORK 
        {
            //PostMessage(iHandle, WM_KEYDOWN, (int)(Keys.Control), 0);
            //PostMessage(iHandle, WM_KEYDOWN, (int)(Keys.A), 0);
            //PostMessage(iHandle, WM_KEYUP, (int)(Keys.Control), 0);
        }//ยังไม่ WORK

        public static void Openprogram(string path)
        {
            Process.Start(path);
        }
    }
}
////// พัฒนาโดยเพจ เขียนโปรแกรมยามว่าง ////////
////// พัฒนาโดยเพจ เขียนโปรแกรมยามว่าง ////////
