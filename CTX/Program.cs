using CTX.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tesseract;
using System.Text.RegularExpressions;
using System.IO;
using IronOcr;
using System.Net;
using RestSharp;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace CTX
{
    internal class Program
    {
        #region data
        //public static Bitmap Wallet_BMP;
        public static Bitmap Recover_BMP;
        public static Bitmap Seed_BMP;
        public static Bitmap Paste_BMP;
        public static Bitmap Next_BMP;
        public static Bitmap WalletName_BMP;
        public static Bitmap Explore_BMP;
        public static Bitmap Convert_BMP;
        public static Bitmap IconGame_BMP;
        public static Bitmap TextConvert_BMP;
        public static Bitmap SeleteCharactor_BMP;
        public static Bitmap C2X_BMP;
        public static Bitmap Confirm_BMP;
        public static Bitmap ConfirmConvert_BMP;
        public static Bitmap Success_BMP;
        public static Bitmap DeleteWallet_BMP;
        public static Bitmap Error_BMP;
        public static Bitmap Selete_BMP;
        public static string lineToken = "";
        #endregion
        public static List<Wallet> Data_Wallet;
        public static string deviceID = null;
        private static string INPUT_TEXT_DEVICES = "adb -s {0} shell input text \"{1}\"";
        private static string ADB_FOLDER_PATH = "";

        private static string OPEN_APP_DEVICES = "adb -s {0} shell monkey -p {1} -c android.intent.category.LAUNCHER 1";
        private static string Kill_APP_DEVICES = "adb -s {0} shell am force-stop {1}";

        public static bool CheckClickRecover;
        public static bool CheckClickUseSeed;
        public static bool CheckInputMneni;
        public static bool CheckInputNameWallet;
        public static bool CheckClickExploer;
        public static bool CheckClickConvert;
        public static bool CheckSeleteCharactor;
        public static bool CheckSuccess;
        public static bool SwapSuccess;
        public static bool LoopSuccess;
        public static bool DonutNotHave;
        public static bool CheckError;
        static void Main(string[] args)
        {
  
            Console.ForegroundColor = ConsoleColor.White;
            
            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Start CTX swap donut.");

            Load();
            Run();

        }



        public static void TestFuntion()
        {
            var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
            
            //Bitmap img = (Bitmap)Bitmap.FromFile("Data//recover.png");
            TesseractEngine eginge = new TesseractEngine("./tessdata", "eng");
            eginge.SetVariable("tessedit_char_whitelist", "0123456789");
            Bitmap cropped = new Bitmap(700, 50);

            using (Image image = screen)
            {
                using (Graphics g = Graphics.FromImage(cropped))
                {
                    g.DrawImage(image, new Rectangle(0, 0, cropped.Width, cropped.Height), new Rectangle(600, 300, 700, 50), GraphicsUnit.Pixel);
                    cropped.Save("crop.png");
                    Bitmap img = (Bitmap)Bitmap.FromFile("crop.png");
                    Bitmap d;
                    int x, y;

                    // Loop through the images pixels to reset color.
                    for (x = 0; x < img.Width; x++)
                    {
                        for (y = 0; y < img.Height; y++)
                        {
                            Color oc = img.GetPixel(x, y);
                            int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                            Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                            img.SetPixel(x, y, nc); // Now greyscale
                        }
                    }
                    d = img;   // d is grayscale version of c
                    Bitmap img2 = (Bitmap)Bitmap.FromFile("crop.png");
                    Page page = eginge.Process(img2, PageSegMode.Auto);
                    d.Save("test.png");
                    string result = page.GetText();

                    Console.WriteLine(result);

                    Console.WriteLine("Crop success");
                }
            }
        }

        public static string GetError()
        {
            try
            {
                var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
                TesseractEngine eginge = new TesseractEngine("./tessdata", "eng", EngineMode.Default);


                Bitmap cropped = new Bitmap(918, 165);

                var error = "";

                using (Image image = screen)
                {
                    using (Graphics g = Graphics.FromImage(cropped))
                    {
                        g.DrawImage(image, new Rectangle(0, 0, cropped.Width, cropped.Height), new Rectangle(64, 780, 918, 165), GraphicsUnit.Pixel);
                        cropped.Save("error.png");


                        Bitmap img2 = (Bitmap)Bitmap.FromFile("error.png");
                        Page page = eginge.Process(img2, PageSegMode.Auto);

                        string result = page.GetText();


                        if (result != "")
                        {
                            error = result;
                            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + error);
                            KAutoHelper.ADBHelper.Tap(deviceID, 538, 1083);

                        }

                    }
                }

                return error;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ex.ToString();
            }
            
        }

        public static void lineNotifyMessage(string msg)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new RestClient("https://notify-api.line.me/api/notify");
                client.Timeout = 10000;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + lineToken);
                request.AddHeader("Content-type", "application/x-www-form-urlencoded");
                request.AddParameter("message", msg);
                var response = client.Execute(request);
                client.Timeout = 10000;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Send Message success");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public static void lineNotify(string msg)
        {
            string token = lineToken;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var request = (HttpWebRequest)WebRequest.Create("https://notify-api.line.me/api/notify");
                var postData = string.Format("message={0}", msg);
                var data = Encoding.UTF8.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.Headers.Add("Authorization", "Bearer " + token);

                using (var stream = request.GetRequestStream()) stream.Write(data, 0, data.Length);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static double GetILT()
        {
            var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
            TesseractEngine eginge = new TesseractEngine("./tessdata", "eng", EngineMode.Default);
            eginge.SetVariable("tessedit_char_whitelist", "0123456789");

            Bitmap cropped = new Bitmap(248, 71);

            

            var ILT = 0.0;

            using (Image image = screen)
            {
                using (Graphics g = Graphics.FromImage(cropped))
                {
                    g.DrawImage(image, new Rectangle(0, 0, cropped.Width, cropped.Height), new Rectangle(51, 848, 248, 71), GraphicsUnit.Pixel);
                    cropped.Save("donut.png");

                    Bitmap img2 = (Bitmap)Bitmap.FromFile("donut.png");
                    Page page = eginge.Process(img2, PageSegMode.Auto);

                    string result = page.GetText();


                    if (result != "")
                    {
                        if (Convert.ToInt32(result) > 0)
                        {
                            ILT = Convert.ToInt32(result) / Math.Pow(10, 6);
                            

                        }
                        if (Convert.ToInt32(result) == 0)
                        {
                            ILT = Convert.ToInt32(result);



                        }
                    }
                    else
                    {
                        ILT = 0;

                        
                    }




                }
            }

            return ILT;
        }
        public static string GetDonus()
        {
            var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
            TesseractEngine eginge = new TesseractEngine("./tessdata", "eng",EngineMode.Default);
            eginge.SetVariable("tessedit_char_whitelist", "0123456789");

            Bitmap cropped = new Bitmap(258, 93);

            var donus = "";

            using (Image image = screen)
            {
                using (Graphics g = Graphics.FromImage(cropped))
                {
                    g.DrawImage(image, new Rectangle(0, 0, cropped.Width, cropped.Height), new Rectangle(805, 555, 258, 93), GraphicsUnit.Pixel);
                    cropped.Save("donut.png");

                    //Bitmap img = (Bitmap)Bitmap.FromFile("donut.png");
                    //Bitmap d;
                    //int x, y;

                    //for (x = 0; x < img.Width; x++)
                    //{
                    //    for (y = 0; y < img.Height; y++)
                    //    {
                    //        Color oc = img.GetPixel(x, y);
                    //        int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                    //        Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                    //        img.SetPixel(x, y, nc); // Now greyscale
                    //    }
                    //}

                    //d = img;
                    //d.Save("donutgray.png");

                    
                  
                    Bitmap img2 = (Bitmap)Bitmap.FromFile("donut.png");
                    Page page = eginge.Process(img2, PageSegMode.Auto);

                    string result = page.GetText();
                    
                    
                    if (result != "")
                    {
                        if (Convert.ToInt32(result) > 0)
                        {
                            donus = result;

                        }
                        if (Convert.ToInt32(result) == 0)
                        {
                            donus = result;

                        
                        
                        }
                    }
                    else
                    {
                        donus = "NOT";
                    }


                   

                }
            }
            return donus;
        }

        public static void LoadBMP()
        {
            //Wallet_BMP = (Bitmap)Bitmap.FromFile("Data//wallet.png");
            try
            {
                Recover_BMP = (Bitmap)Bitmap.FromFile("Data//recover.png");
                Seed_BMP = (Bitmap)Bitmap.FromFile("Data//seed.png");
                Paste_BMP = (Bitmap)Bitmap.FromFile("Data//paste.png");
                Next_BMP = (Bitmap)Bitmap.FromFile("Data//next.png");
                WalletName_BMP = (Bitmap)Bitmap.FromFile("Data//Wallet_Name.png");
                Explore_BMP = (Bitmap)Bitmap.FromFile("Data//explore.png");
                Convert_BMP = (Bitmap)Bitmap.FromFile("Data//convert.png");
                IconGame_BMP = (Bitmap)Bitmap.FromFile("Data//icon_game.png");
                TextConvert_BMP = (Bitmap)Bitmap.FromFile("Data//text_convert.png");
                SeleteCharactor_BMP = (Bitmap)Bitmap.FromFile("Data//selete3.png");
                C2X_BMP = (Bitmap)Bitmap.FromFile("Data//c2x.png");
                Confirm_BMP = (Bitmap)Bitmap.FromFile("Data//confirm.png");
                ConfirmConvert_BMP = (Bitmap)Bitmap.FromFile("Data//confim_convert.png");
                Success_BMP = (Bitmap)Bitmap.FromFile("Data//success.png"); 
                DeleteWallet_BMP = (Bitmap)Bitmap.FromFile("Data//delete.png");
                Error_BMP = (Bitmap)Bitmap.FromFile("Data//error.png");
                Selete_BMP = (Bitmap)Bitmap.FromFile("Data//circle.png");

            }
            catch (Exception e)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + e);
            }

            
        }

        public static void LoadJson()
        {
            try
            {
                string json = System.IO.File.ReadAllText("accounts.json");
                Data_Wallet = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Wallet>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't found accounts.json");
            }

        }

        public static void Load()
        {
            LoadDevices();
            LoadBMP();
            LoadJson();
        }

        public static void LoadDevices()
        {

            try
            {
                var listDevice = KAutoHelper.ADBHelper.GetDevices();

                if (listDevice != null && listDevice.Count > 0)
                {
                    deviceID = listDevice.First();
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Connect devices : " + deviceID);

                }
                else
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Not have devices");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't load devices");
            }
        }

        public static void Run()
        {
            if (deviceID != null)
            {

                var writer = new StreamWriter("filename.txt");
                foreach (Wallet key in Data_Wallet)
                {
                repeat:
                    OpenApp(deviceID, "c2xstation.android");
                    var status = true;
                    CheckInputMneni = false;
                    CheckInputNameWallet = false;
                    CheckClickExploer = false;
                    CheckClickConvert = false;
                    CheckSeleteCharactor = false;
                    CheckClickUseSeed = false;
                    CheckClickRecover = false;
                    CheckSuccess = false;
                    SwapSuccess = false;
                    DonutNotHave = false;
                    CheckError = false;
                    var donut = "";
                    

                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Start " + key.name);
                    while (status)
                    {
                        
                        if (!CheckClickRecover && SwapSuccess == false)
                        {
                            SearchRecover(deviceID, Recover_BMP);
                        }

                        if (!CheckClickUseSeed && SwapSuccess == false)
                        {
                            SearchUseSeed(deviceID, Seed_BMP);
                        }


                        if (CheckInputMneni == false && SwapSuccess == false && CheckClickRecover == true)
                        {
                            SearchPaste(deviceID, Paste_BMP, key.memoni);
                            var count = 0;
                            var loopNext = true;
                            while (loopNext)
                            {
                                var checkPointNext = SearchNext(deviceID, Next_BMP);
                                if (checkPointNext == true)
                                {
                                    loopNext = false;
                                }
                                else if (count > 15)
                                {
                                    KillApp(deviceID, "c2xstation.android");
                                    Thread.Sleep(1000);
                                    goto repeat;
                                }
                                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Count " + count);
                                Thread.Sleep(1000);
                                count++;
                            }
                        }
                        if (CheckInputNameWallet == false && SwapSuccess == false && CheckClickRecover == true && CheckInputMneni == true)
                        {
                            SearchWalletName(deviceID, WalletName_BMP, key.name);
                            bool Next1 = true;
                            while (Next1)
                            {
                                var checkPointNext = SearchNext(deviceID, Next_BMP);
                                if (checkPointNext == true)
                                {
                                    Next1 = false;
                                }
                            }
                        }
                        if (!CheckClickExploer && SwapSuccess == false && CheckClickRecover == true && CheckInputMneni == true && CheckInputNameWallet == true)
                        {
                            SearchExplore(deviceID, Explore_BMP);
                        }

                        if (!CheckClickConvert && SwapSuccess == false && CheckClickRecover == true && CheckInputMneni == true && CheckInputNameWallet == true && CheckClickExploer == true)
                        {
                            SearchConvert(deviceID, Convert_BMP);
                        }

                        var checkTextConvert = SearchTextConvert(deviceID, TextConvert_BMP);
                        if (checkTextConvert == true && SwapSuccess == false)
                        {
                            SearchIconGame(deviceID, IconGame_BMP);
                        }
                        errorNew:
                        if (!CheckSeleteCharactor && SwapSuccess == false)
                        {
                            SearchSeleteCharactor(deviceID, SeleteCharactor_BMP);
                            if (CheckSeleteCharactor)
                            {
                                var checkC2X = SearchC2X(deviceID, C2X_BMP);
                                if (checkC2X)
                                {
                                    Thread.Sleep(2500);
                                    KAutoHelper.ADBHelper.Tap(deviceID, 536, 890);
                                    Thread.Sleep(1000);
                                    donut = GetDonus();
                                    if (donut != "NOT")
                                    {
                                        if (Convert.ToInt32(donut) >= 1400 && Convert.ToInt32(donut) <= 1500)
                                        {
                                            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] You have donut : " + donut);
                                            KAutoHelper.ADBHelper.Tap(deviceID, 561, 739);
                                            InputText(deviceID, donut.ToString());
                                            KAutoHelper.ADBHelper.Tap(deviceID, 529, 1264);

                                            var Next = true;
                                            while (Next)
                                            {
                                                var checkPointNext = SearchNext(deviceID, Next_BMP);
                                                if (checkPointNext == true)
                                                {
                                                    Next = false;
                                                }
                                            }
                                            Thread.Sleep(1000);
                                            var Convert = SearchConfirm(deviceID, Confirm_BMP);
                                            if (Convert)
                                            {
                                                var Next1 = true;
                                                while (Next1)
                                                {
                                                    var checkClick = SearchConfirmConvert(deviceID, ConfirmConvert_BMP);
                                                    if (checkClick == true)
                                                    {
                                                        Next1 = false;
                                                        CheckSuccess = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (Convert.ToInt32(donut) > 1500)
                                        {
                                            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] You have donut : " + donut);
                                            KAutoHelper.ADBHelper.Tap(deviceID, 561, 739);
                                            InputText(deviceID, "1500");
                                            KAutoHelper.ADBHelper.Tap(deviceID, 529, 1264);
                                            var Next = true;
                                            while (Next)
                                            {
                                                var checkPointNext = SearchNext(deviceID, Next_BMP);
                                                if (checkPointNext == true)
                                                {
                                                    Next = false;
                                                }
                                            }
                                            Thread.Sleep(1000);
                                            var Convert = SearchConfirm(deviceID, Confirm_BMP);
                                            if (Convert)
                                            {
                                                var Next1 = true;
                                                while (Next1)
                                                {
                                                    var checkClick = SearchConfirmConvert(deviceID, ConfirmConvert_BMP);
                                                    if (checkClick == true)
                                                    {
                                                        Next1 = false;
                                                        CheckSuccess = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (Convert.ToInt32(donut) < 850)
                                        {
                                            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] You have less than 850 donuts. : " + donut);
                                            KAutoHelper.ADBHelper.Tap(deviceID, 104, 442);
                                            DonutNotHave = true;


                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Fail Search Donus : " + donut);
                                        KAutoHelper.ADBHelper.Tap(deviceID, 104, 442);
                                        DonutNotHave = true;
                                    }



                                }
                            }
                        }

                        if (CheckSuccess)
                        {
                            SearchSuccess(deviceID, Success_BMP, donut,key.name);
                            SearchError(deviceID, Error_BMP);
                        }

                        if (CheckError)
                        {
                            GetError();
                            CheckError = false;
                            CheckSeleteCharactor = false;
                            goto errorNew;
                        }

                        if (SwapSuccess == true)
                        {
                            LoopSuccess = true;
                            while (LoopSuccess)
                            {
                                SearchC2X(deviceID, C2X_BMP);
                                SearchSeleteCharactor(deviceID, SeleteCharactor_BMP);
                                SearchTextConvert(deviceID, TextConvert_BMP);
                                var chk = SearchDeleteWallet(deviceID, DeleteWallet_BMP);
                                if (chk == true)
                                {
                                    writer.WriteLine("Swap = Success " + key.name);
                                    status = false;
                                }
                            }

                        }

                        if (DonutNotHave == true)
                        {
                            LoopSuccess = true;
                            while (LoopSuccess)
                            {
                                SearchC2X(deviceID, C2X_BMP);
                               
                                SearchTextConvert(deviceID, TextConvert_BMP);
                                var chk = SearchDeleteWallet(deviceID, DeleteWallet_BMP);
                                if (chk == true)
                                {
                                    writer.WriteLine("Swap = Fail " + key.name + "you have less than 300 donuts");
                                    status = false;
                                }
                            }
                        }

                        


                    }



                }

                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Swap account total success");


            }
            else
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Please check connect to devices");
            }
        }


        public static void SearchRecover(string deviceID, Bitmap BMP)
        {
            try
            {
                var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
                var walletPoint = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, BMP);

                if (walletPoint != null)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found recover");
                    KAutoHelper.ADBHelper.Tap(deviceID, walletPoint.Value.X, walletPoint.Value.Y);
                    CheckClickRecover = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't find point image recover");
            }
        }

        public static void SearchError(string deviceID, Bitmap BMP)
        {
            try
            {
                var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
                var walletPoint = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, BMP);

                if (walletPoint != null)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found error");
                    CheckError = true;
                    CheckSuccess = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't find point image error");
            }
        }

        public static void SearchSuccess(string deviceID, Bitmap BMP,string donut,string name)
        {
            try
            {
                var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
                var walletPoint = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, BMP);

                if (walletPoint != null)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found Ok");
                    var ILT = GetILT();
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Donut "+Convert.ToInt16(donut) +" convert to ILT "+ ILT);
                    string meg = name + "Donut " + donut + " convert to ILT " + ILT.ToString();
                    lineNotify(meg);
                    KAutoHelper.ADBHelper.Tap(deviceID, 512, 2181);
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Click Ok");
                    SwapSuccess = true;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't find point image button ok");
            }
        }

        public static bool SearchDeleteWallet(string deviceID, Bitmap BMP)
        {
            try
            {
                var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
                var walletPoint = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, BMP);

                if (walletPoint != null)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found delete wallet");
                    KAutoHelper.ADBHelper.Tap(deviceID, walletPoint.Value.X, walletPoint.Value.Y);
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Click delete wallet");
                    Thread.Sleep(1500);
                    KAutoHelper.ADBHelper.Tap(deviceID, 754, 1437);

                    LoopSuccess = false;
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't find point image delete wallet");
                return false;
            }
        }

        public static bool SearchConfirm(string deviceID, Bitmap BMP)
        {
            try
            {
                var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
                var walletPoint = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, BMP);

                if (walletPoint != null)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found confirm");
                    KAutoHelper.ADBHelper.Tap(deviceID, 230, 545);
                    Thread.Sleep(1000);
                    InputText(deviceID, "qwertyuiop");
                    Thread.Sleep(1000);
                    KAutoHelper.ADBHelper.Tap(deviceID, walletPoint.Value.X, walletPoint.Value.Y);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't find point image confirm");
                return false;
            }
        }

       

        public static bool SearchConfirmConvert(string deviceID, Bitmap BMP)
        {
            try
            {
                var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
                var confirmConvertPoint = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, BMP);

                if (confirmConvertPoint != null)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found confirm convert");
                    KAutoHelper.ADBHelper.Tap(deviceID, confirmConvertPoint.Value.X, confirmConvertPoint.Value.Y);
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Click confirm");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't find point image delete wallet");
                return false;
                
            }
        }

        public static bool SearchC2X(string deviceID, Bitmap BMP)
        {
            var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
            var walletPoint = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, BMP);

            if (walletPoint != null && CheckSuccess == false && DonutNotHave != true)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found C2X");
                KAutoHelper.ADBHelper.Tap(deviceID, 373, 434);
                return true;
            }
            else if(walletPoint != null && CheckSuccess == true)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found C2X after success");
                KAutoHelper.ADBHelper.Tap(deviceID, 80, 145);
                return true;
            }
            else if (walletPoint != null && DonutNotHave  == true)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found C2X after not donut");
                KAutoHelper.ADBHelper.Tap(deviceID, 66, 150);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void SearchExplore(string deviceID, Bitmap BMP)
        {
            try
            {
                var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
                var walletPoint = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, BMP);

                if (walletPoint != null)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found explore");
                    KAutoHelper.ADBHelper.Tap(deviceID, walletPoint.Value.X, walletPoint.Value.Y);
                    CheckClickExploer = true;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't find point image explore");
                
            }
        }



        public static void SearchConvert(string deviceID, Bitmap BMP)
        {
            try
            {
                var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
                var convertPoint = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, BMP);
                if (convertPoint != null)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found covert");
                    KAutoHelper.ADBHelper.Tap(deviceID, convertPoint.Value.X, convertPoint.Value.Y);
                    CheckClickConvert = true;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't find point image covert");

            }
        }

        public static void SearchIconGame(string deviceID, Bitmap BMP)
        {
            try
            {
                var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
                var iconGamePoint = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, BMP);
                //var aaa = KAutoHelper.ImageScanOpenCV.Find(screen, BMP);
                //aaa.Save("bbb.png");
                if (iconGamePoint != null)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found icon game");
                    KAutoHelper.ADBHelper.Tap(deviceID, iconGamePoint.Value.X, iconGamePoint.Value.Y);


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't find point image icon game");

            }
        }

        public static void SearchCharactor(string deviceID,Bitmap BMP)
        {
            var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
            var seletePoint = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, BMP);
            if (seletePoint != null && CheckSeleteCharactor == false)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found OrBUll");
                KAutoHelper.ADBHelper.Tap(deviceID, seletePoint.Value.X, seletePoint.Value.Y+155);
                bool Next1 = true;
                while (Next1)
                {
                    var checkPointNext = SearchNext(deviceID, Next_BMP);
                    if (checkPointNext == true)
                    {
                        Next1 = false;
                        CheckSeleteCharactor = true;
                    }
                }
            }

        }

        public static void SearchSeleteCharactor(string deviceID, Bitmap BMP)
        {
            var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
            var seletcCharactorPoint = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, BMP);
            //var aaa = KAutoHelper.ImageScanOpenCV.Find(screen, BMP);
            //aaa.Save("bbb.png");
            if (seletcCharactorPoint != null && CheckSeleteCharactor == false)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found selete charactor");
                KAutoHelper.ADBHelper.Tap(deviceID, seletcCharactorPoint.Value.X, seletcCharactorPoint.Value.Y);
                //Thread.Sleep(1000);
                //KAutoHelper.ADBHelper.Tap(deviceID, 595, 1979);
                SearchCharactor(deviceID, Selete_BMP);

            }
            else if (seletcCharactorPoint != null && CheckSeleteCharactor == true)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found selete charactor after success");
                KAutoHelper.ADBHelper.Tap(deviceID, 68, 149);
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Click back");
            }
        }


        public static bool SearchTextConvert(string deviceID, Bitmap BMP)
        {
            var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
            var iconGamePoint = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, BMP);
            //var aaa = KAutoHelper.ImageScanOpenCV.Find(screen, BMP);
            //aaa.Save("ggg.png");
            if (iconGamePoint != null  && SwapSuccess == false && DonutNotHave != true)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found text convert");


                return true;
            }
            else if (iconGamePoint != null && SwapSuccess == true)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found text convert after swap");
                KAutoHelper.ADBHelper.Tap(deviceID, 1015, 200);
                return true;
            }
            else if (iconGamePoint != null && DonutNotHave == true)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found text convert after NOT donut");
                KAutoHelper.ADBHelper.Tap(deviceID, 1015, 200);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void SearchUseSeed(string deviceID, Bitmap BMP)
        {
            var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
            var walletPoint = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, BMP);
            //var aaa = KAutoHelper.ImageScanOpenCV.Find(screen, BMP);
            //aaa.Save("aaa.png");
            //MessageBox.Show(walletPoint.ToString());
            if (walletPoint != null)
            {

                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found use seed");
                KAutoHelper.ADBHelper.Tap(deviceID, walletPoint.Value.X, walletPoint.Value.Y);

            }
        }

        public static void SearchWalletName(string deviceID, Bitmap BMP,string name)
        {
            var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
            var walletPoint = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, BMP);
 
            if (walletPoint != null)
            {
                string input = name;
                string[] values = input.Split('@');
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found input wallet name");
                KAutoHelper.ADBHelper.Tap(deviceID, 389, 537);
                Thread.Sleep(1000);
                var inputText = InputText(deviceID, values[0]);
                if (inputText != null)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Input wallet name success");
                    KAutoHelper.ADBHelper.Tap(deviceID, 242, 818);
                    var password = InputText(deviceID, "qwertyuiop");
                    if (password != null)
                    {
                        Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Input password success");
                        KAutoHelper.ADBHelper.Tap(deviceID, 298, 1072);
                        var confirmPassword = InputText(deviceID, "qwertyuiop");
                        if (confirmPassword != null)
                        {
                            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Input confirm password success");
                            KAutoHelper.ADBHelper.Tap(deviceID, 326, 1215);
                            CheckInputNameWallet = true;
                        }
                    }
                }

            }
        }

        public static void SearchPaste(string deviceID, Bitmap BMP,string mneni)
        {
            try
            {
                var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
                var walletPoint = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, BMP);

                if (walletPoint != null)
                {

                    Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Found paste");
                    KAutoHelper.ADBHelper.Tap(deviceID, 536, 628);
                    Thread.Sleep(1500);
                    var test = InputText(deviceID, mneni);
                    if (test != null)
                    {
                        Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Input mninome success");
                        Thread.Sleep(1000);
                        KAutoHelper.ADBHelper.Tap(deviceID, 525, 918);
                        CheckInputMneni = true;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't find point image paste");

            }

        }

        public static bool SearchNext(string deviceID, Bitmap BMP)
        {
            
            var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
            var NextPoint = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, BMP);

            if (NextPoint != null)
            {
                KAutoHelper.ADBHelper.Tap(deviceID, NextPoint.Value.X, NextPoint.Value.Y);
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Click next");
                return true;
            }else
            {
                return false;
            }

            

        }

        public static string InputText(string deviceID, string text)
        {
            string cmdCommand = string.Format(INPUT_TEXT_DEVICES, deviceID, text.Replace(" ", "%s").Replace("&", "\\&").Replace("<", "\\<")
                .Replace(">", "\\>")
                .Replace("?", "\\?")
                .Replace(":", "\\:")
                .Replace("{", "\\{")
                .Replace("}", "\\}")
                .Replace("[", "\\[")
                .Replace("]", "\\]")
                .Replace("|", "\\|"));
            string text2 = ExecuteCMD(cmdCommand);
            return text2;
        }

        public static void OpenApp(string deviceID , string app)
        {
            string cmdCommand = string.Format(OPEN_APP_DEVICES, deviceID, app);
            string text2 = ExecuteCMD(cmdCommand);
        }

        public static void KillApp(string deviceID,string app)
        {
            string cmdCommand = string.Format(Kill_APP_DEVICES, deviceID, app);
            string text2 = ExecuteCMD(cmdCommand);
        }
        public static string ExecuteCMD(string cmdCommand)
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.WorkingDirectory = ADB_FOLDER_PATH;
                processStartInfo.FileName = "cmd.exe";
                processStartInfo.CreateNoWindow = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processStartInfo.RedirectStandardInput = true;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.Verb = "runas";
                process.StartInfo = processStartInfo;
                process.Start();
                process.StandardInput.WriteLine(cmdCommand);
                process.StandardInput.Flush();
                process.StandardInput.Close();
                process.WaitForExit();
                return process.StandardOutput.ReadToEnd();
            }
            catch
            {
                return null;
            }
        }
    }
}
