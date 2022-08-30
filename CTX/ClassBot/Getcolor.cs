
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CTX
{
    public class GetColor
    {
        ////// พัฒนาโดยเพจ เขียนโปรแกรมยามว่าง ////////
        
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "GetDC")]
        internal extern static IntPtr GetDC(IntPtr hWnd);
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        internal extern static IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        internal extern static IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        internal extern static IntPtr DeleteDC(IntPtr hDc);
        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        internal extern static IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        internal extern static bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);
        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        internal extern static IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        internal extern static IntPtr DeleteObject(IntPtr hDc);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowDC(Int32 window);
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern uint GetPixel(Int32 dc, Int32 x, Int32 y);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ReleaseDC(Int32 window, Int32 dc);
        public static int ganx;
        public static int gany;
        public static int WinSizeWidth;
        public static int WinSizeHeight;
        public const int SRCCOPY = 0x00CC0020;



        public static bool Status;
        public static bool CKStatus = false;
        public static string CKCOLOR = "";
        public static Color GetColorAt(Int32 hwnd, Int32 x, Int32 y)
        {
            ganx = x;
            gany = y;
            Int32 dc = GetWindowDC(hwnd);
            Int32 a = (int)GetPixel(dc, x, y);
            ReleaseDC(hwnd, dc);
            return Color.FromArgb(255, (a >> 0) & 0xff, (a >> 8) & 0xff, (a >> 16) & 0xff);
        }
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        public static Size GetControlSize(IntPtr iHandle) 
        {
            RECT pRect;
            Size cSize = new Size();
            GetWindowRect(iHandle, out pRect);
            cSize.Width = pRect.Right - pRect.Left;
            cSize.Height = pRect.Bottom - pRect.Top;
            WinSizeWidth = cSize.Width;
            WinSizeHeight = cSize.Height;
            return cSize;
        } //ดึงขนาดจอเกมส์หรือโปรแกรม 
        private static bool HexConverter(System.Drawing.Color c, int PixelColor, int Shade_Variation, IntPtr iHandle)
        {
            GetControlSize(iHandle);
            Status = false;
            Rectangle rect = Rectangle.FromLTRB(0, 0, WinSizeWidth, WinSizeHeight);
            var Dcolor = PixelColor.ToString();
            var PixelColor1 = Convert.ToInt32(Dcolor);
            Color Pixel_Color = Color.FromArgb(PixelColor1);
            Point Pixel_Coords = new Point(0, 0);
            IntPtr hdcSrc = GetDC(iHandle);
            IntPtr hdcDest = CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = CreateCompatibleBitmap(hdcSrc, rect.Width, rect.Height);
            IntPtr hOld = SelectObject(hdcDest, hBitmap);
            BitBlt(hdcDest, 0, 0, rect.Width, rect.Height, hdcSrc, rect.X, rect.Y, SRCCOPY);
            SelectObject(hdcDest, hOld);
            DeleteDC(hdcDest);
            ReleaseDC(iHandle, hdcSrc);
            Bitmap bmp = Bitmap.FromHbitmap(hBitmap);
            BitmapData RegionIn_BitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int[] Formatted_Color = new int[3] { Pixel_Color.B, Pixel_Color.G, Pixel_Color.R };
            unsafe
            {
                byte* row = (byte*)RegionIn_BitmapData.Scan0 + (gany * RegionIn_BitmapData.Stride);
                if (row[ganx * 3] >= (Formatted_Color[0] - Shade_Variation) & row[ganx * 3] <= (Formatted_Color[0] + Shade_Variation))//b
                {
                    if (row[(ganx * 3) + 1] >= (Formatted_Color[1] - Shade_Variation) & row[(ganx * 3) + 1] <= (Formatted_Color[1] + Shade_Variation))//g
                    {
                        if (row[(ganx * 3) + 2] >= (Formatted_Color[2] - Shade_Variation) & row[(ganx * 3) + 2] <= (Formatted_Color[2] + Shade_Variation))// R                           
                        {
                            Pixel_Coords = new Point(ganx + rect.X, gany + rect.Y);
                            Status = true;
                            goto end;
                        }
                    }
                }
            }
        end:
            bmp.UnlockBits(RegionIn_BitmapData);
            DeleteObject(hBitmap);
            bmp.Dispose();
            return Status;
        }//ได้ค่าสีจาก Getcolor  >>> และไปค้นหาสีใน ram แล้วนำไปใช้งาน //google
        public static bool GETCOLOR(IntPtr iHandle, int x, int y, int PixelColor, int Shade_Variation)
        {
            IntPtr appHandle = iHandle;
            return HexConverter(GetColorAt(appHandle.ToInt32(), x, y), PixelColor, Shade_Variation, iHandle);
        }//รับค่าสีมาแล้วส่งไปยัง HexConverter เพิ้อไปค้นหา

        public static string GETCOLORSTRING( int x, int y)
        {
            IntPtr appHandle = GetAppName.appname;
            return HexConverterOLD(GetColorAt(appHandle.ToInt32(), x, y));
        } //รับค่าสีมาแล้วส่งไปยัง HexConverterOLD 

        private static string HexConverterOLD(System.Drawing.Color c) //HexConverter OLD
        {
            return string.Format("0x{0}", c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2"));
        } //ได้ค่าสีจาก Getcolor  >>> และไปค้นหาสี แล้วนำไปใช้งาน //google
        public static bool GETCOLORFAST(IntPtr iHandle, int x, int y, int PixelColorx, int Shade_Variation) //GET COLOR OLD not use Shade_Variation
        {
            
            IntPtr appHandle = iHandle;
            string hexStr = string.Format("{0:x}", PixelColorx);
            hexStr = hexStr.ToUpper();

            
            Console.WriteLine(hexStr);


            if (hexStr.Length == 5)
            {
                hexStr = "0x0" + hexStr;
            }else if (hexStr.Length == 4)
            {
                hexStr = "0x00" + hexStr;
            }
            else if (hexStr.Length == 3)
            {
                hexStr = "0x000" + hexStr;
            }
            else if (hexStr.Length == 2)
            {
                hexStr = "0x0000" + hexStr;
            }
            else if (hexStr.Length == 1)
            {
                hexStr = "0x00000" + hexStr;
            } else
            {
                hexStr = "0x" + hexStr;
            }
        
            if (GETCOLORSTRING(x, y) == hexStr)
            {
              
                CKStatus = true;
                CKCOLOR = hexStr;
            }
            else
            {
                CKStatus = false;
                CKCOLOR = hexStr;
            }
                     return CKStatus;
        }//ดึงค่าสีมาเช็คเปรียบเทียบ
    }
}

////// พัฒนาโดยเพจ เขียนโปรแกรมยามว่าง ////////
////// พัฒนาโดยเพจ เขียนโปรแกรมยามว่าง ////////

