using AgenteHelperLibrary;
using com.sun.jmx.defaults;
using jdk.nashorn.@internal.ir;
using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using OtpNet;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MARCUS.Helpers
{
    [AddINotifyPropertyChangedInterface]

    public class Keanu
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Keanu));

        public IWebDriver Driver { get; set; }
        public IWebDriver DriverExtra { get; set; }
        public IWebDriver DriverSII { get; set; }
        public IWebDriver DriverFOUR { get; set; }
        public IWebDriver DriverGRAVITY { get; set; }

        public Random Rand = new Random();
        public WebDriverWait Wait { get; set; }

        public Keanu()
        {

        }

        public bool StartStop { get; set; } = false;//true start, false stop
        public string From { get; set; }
        public string To { get; set; }
        public string LoginAGENTE { get; set; }
        public string PassAGENTE { get; set; }
        public string LoginSFA { get; set; }
        public string PassSFA { get; set; }
        public string LoginGEOCALL { get; set; }
        public string PassGEOCALL { get; set; }
        public string LoginSAP { get; set; }
        public string PassSAP { get; set; }
        public string LoginNEXT { get; set; }
        public string PassNEXT { get; set; }
        public string LoginFOUR { get; set; }
        public string PassFOUR { get; set; }
        public string LoginR2D { get; set; }
        public string PassR2D { get; set; }
        public string LoginSII { get; set; }
        public string PassSII { get; set; }
        public string LavName { get; set; }
        public string SospendiTipo { get; set; }
        public int LavLoginId { get; set; }
        public int LavRegId { get; set; }
        public int LavScartoId { get; set; }
        public int LavScartoNewReso { get; set; }
        public int LavRiclassifica { get; set; }
        public int ResoRecuperoDati { get; set; }
        public int LavGiaLavorato { get; set; }
        public int IdRemainingCheck { get; set; }
        public int PdfType { get; set; }
        public int ScodamentoMin { get; set; }
        public int ScodamentoMax { get; set; }
        public int ScartoCount { get; set; }
        public int TotalCnt { get; set; }
        public bool TimeToRestart { get; set; }
        public bool TimeToSospeso { get; set; }
        public int TimeToSospesoType { get; set; }
        public string FaseDiSospesoPerEE253 { get; set; }
        public string IdAttivitaPerScodamento { get; set; }
        public bool ModificaPodTrip { get; set; }
        public bool SfaCheckerPlusAgente { get; set; }

        public int sfaTimeout = 5;
        public int nextTimeout = 3;

        private AgenteHelper agente;
        public AgenteHelper Agente
        {
            get
            {
                if (agente == null) { agente = new AgenteHelper(); }
                return agente;
            }
        }

        private Records records;
        public Records Records
        {
            get
            {
                if (records == null) { records = new Records(); }
                return records;
            }
            set
            {
                records = value;
            }
        }

        private Badminton bad;
        public Badminton Bad
        {
            get
            {
                if (bad == null) { bad = new Badminton(); }
                return bad;
            }
        }

        public int Randy(int repeats)
        {
            lock (Rand) { return Rand.Next(int.Parse(From) * repeats, int.Parse(To) * repeats); }
        }

        public void Timy()
        {
            int hourMinute = Convert.ToInt32(DateTime.Now.ToString("HHmm"));
            try
            {
                //if (hourMinute >= 2345 || hourMinute < 500)
                if (hourMinute >= 2150 || hourMinute < 500)
                {
                    log.Error($"TURN DOWN FOR WHAT {DateTime.Now}");
                    KillChromeWebDriver();
                    Agente.Logout();
                    Process.Start("shutdown", "/s /t 0");
                    return;
                }
            }
            catch
            { }
        }

        public void TimyNeinItalian()
        {
            int hourMinute = Convert.ToInt32(DateTime.Now.ToString("HHmm"));
            try
            {
                //if (hourMinute >= 2045 || hourMinute < 500)
                if (hourMinute >= 2150 || hourMinute < 500)
                {
                    log.Error($"TURN DOWN FOR WHAT {DateTime.Now}");
                    KillChromeWebDriver();
                    Agente.Logout();
                    Process.Start("shutdown", "/s /t 0");
                    return;
                }
            }
            catch
            { }
        }

        public string GetTotalFreeSpace()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                    return (drive.TotalFreeSpace / (1024 * 1024 * 1024)).ToString();
            }
            return "FAIL";
        }

        public string Cr8SessionId(int len)
        {
            const string c = "qwertyuiopasdfghjklzxcvbm0123456789";
            return new string(Enumerable.Repeat(c, len).Select(s => s[Rand.Next(s.Length)]).ToArray());
        }

        public string Re(int len)
        {
            const string c = "qwertyuiopasdfghjklzxcvbm";
            return new string(Enumerable.Repeat(c, len).Select(s => s[Rand.Next(s.Length)]).ToArray());
        }

        public void ClosePasswordAlert()
        {
            try
            {
                IWebElement mexArancione = Driver.FindElement(By.XPath("//div[@class='slds-theme--warning slds-notify--toast slds-notify slds-notify--toast forceToastMessage']"));
                IWebElement chiudi = mexArancione.FindElement(By.XPath(".//button[@class = 'slds-button slds-button_icon toastClose slds-notify__close slds-button--icon-inverse slds-button_icon-bare']"));
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", chiudi);
            }
            catch { }
        }

        public bool PepperYourChrome(string username, string password, string webadress, string firstTextToWait, bool continua)
        {
            try
            {
                string tpiw = "This page isn’t working";
                string tscbr = "This site can’t be reached";
                string c = "C:\\Downloads\\";
                string d = c + Re(6);
                if (!Directory.Exists(d))
                    Directory.CreateDirectory(d);

                #region webrequest.js
                string w = "chrome.webRequest.onAuthRequired.addListener(function(details){" + "\n" +
                           "\t" + "return {" + "\n" +
                           "\t" + "\t" + "\t" + @"authCredentials: { username: """ + $"{username}" + @""", password: """ + $"{password}" + @""" }" + "\n" +
                           "\t" + "\t" + @"};" + "\n" +
                           @"}," + "\n" +
                           @"{urls:[""<all_urls>""]}," + "\n" +
                           @"['blocking']);";
                try
                {
                    using (FileStream ck = File.Create($"{d}\\webrequest.js"))
                    {
                        byte[] i = new UTF8Encoding(true).GetBytes(w);
                        ck.Write(i, 0, i.Length);
                    }
                }
                catch (Exception Ex) { Console.WriteLine(Ex.ToString()); }
                #endregion

                #region manifest.json
                string m = "{" + "\n" +
                           "\t" + @"""name"": ""Webrequest API""," + "\n" +
                           "\t" + @"""version"": ""1.0""," + "\n" +
                           "\t" + @"""description"": ""Authentication Window Exy666""," + "\n" +
                           "\t" + @"""permissions"": [" + "\n" +
                           "\t" + "\t" + @"""webRequest""," + "\n" +
                           "\t" + "\t" + @"""webRequestBlocking""," + "\n" +
                           "\t" + "\t" + @"""<all_urls>""" + "\n" +
                           "\t" + @"]," + "\n" +
                           "\t" + @"""background"": {" + "\n" +
                           "\t" + "\t" + @"""scripts"": [" + "\n" +
                           "\t" + "\t" + "\t" + @"""webrequest.js""" + "\n" +
                           "\t" + "\t" + @"]" + "\n" +
                           "\t" + @"}," + "\n" +
                           "\t" + @"""manifest_version"": 2" + "\n" +
                           "}";
                try
                {
                    using (FileStream ck = File.Create($"{d}\\manifest.json"))
                    {
                        byte[] i = new UTF8Encoding(true).GetBytes(m);
                        ck.Write(i, 0, i.Length);
                    }
                }
                catch (Exception Ex) { Console.WriteLine(Ex.ToString()); }
                #endregion
                  #region DRIVER SFA CHECK FOR ENEL PAGE
                var ds = ChromeDriverService.CreateDefaultService();
                ds.HideCommandPromptWindow = false;
                var options = new ChromeOptions();

                options.AddArguments($"load-extension={d}");
                options.AddArgument("--start-maximized");
                options.AddArgument("--disable-notifications");
                /*options.AddUserProfilePreference("download.default_directory", c);
                options.AddUserProfilePreference("download.prompt_for_download", false);
                options.AddUserProfilePreference("download.directory_upgrade", true);
                options.AddUserProfilePreference("safebrowsing.enabled", true);
                options.AddUserProfilePreference("safebrowsing", "enabled");
                options.AddArguments($"load-extension={d}");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--safebrowsing-disable-download-protection");
                options.AddArgument("--start-maximized");
                options.AddArgument("--disable-notifications");
                options.AddArgument("--log-level=3");*/
                Driver = new ChromeDriver(ds, options);
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120);
                Driver.Navigate().GoToUrl(webadress);
                if (webadress == "https://enelcrmt.my.salesforce.com/")
                {
                    IWebElement inputa = Driver.FindElement(By.Id("i0116"));
                    if (Driver.FindElements(By.Id("i0116")).Count > 0)
                    {
                        inputa.SendKeys(username + "@enel.com");
                        IWebElement submit = Driver.FindElement(By.Id("idSIButton9"));
                        //Thread.Sleep(Randy(2));
                        submit.Click();
                    }
                    Thread.Sleep(Randy(6));

                   if (LavLoginId != 5056 && LavLoginId != 4977 && LavLoginId != 0)
                    {
                        if (!Driver.PageSource.ToString().Contains("Enter code"))
                        {
                            KillChromeWebDriver();
                            try
                            {
                                PepperYourChrome(LoginSFA, PassSFA, "https://enelcrmt.my.salesforce.com/", "", true);

                            }
                            catch (Exception Ex)
                            {
                                log.Warn("SF SECURITY FAIL");
                                log.Error(Ex.Message);
                                return false;
                            }

                        }

                    }
                }
                Thread.Sleep(Randy(6));
                #endregion
                #region OTP
                try
                {
                    if (Driver.FindElements(By.Id("idTxtBx_SAOTCC_OTC")).Count > 0)
                    {
                        VerificationCode(username);
                        if (Driver.PageSource.ToString().Contains("You didn't enter the expected verification code. Please try again."))
                        {

                            Thread.Sleep(Randy(6));
                            VerificationCode(username);

                            /*if (!Driver.PageSource.ToString().Contains("We couldn't save your action detail selections. Please try again.") && !Driver.PageSource.ToString().Contains("We couldn't save your action selection. Please try again.") && !Driver.PageSource.ToString().Contains("We couldn't save the name and description for the automated action. Please try again.") && !Driver.PageSource.ToString().Contains("We couldn't save your object selection. Please try again."))
                            {
                                Thread.Sleep(Randy(6));
                                VerificationCode(username);
                            }*/

                        }
                    }
                }
                catch (Exception Ex)
                {
                    log.Warn("Catch Codice di Verifica");
                    log.Error(Ex.Message);
                    return false;
                }
                #endregion

                #region CHECKS IF SFA 
                Thread.Sleep(Randy(5));
                if (webadress == "https://enelcrmt.my.salesforce.com/")
                {
                    if (!Driver.PageSource.ToString().Contains("Lightning Service"))
                    {
                        KillChromeWebDriver();
                        try
                        {

                            PepperYourChrome(LoginSFA, PassSFA, "https://enelcrmt.my.salesforce.com/", "", true);

                        }
                        catch (Exception Ex)
                        {
                            log.Warn("SF SECURITY FAIL");
                            log.Error(Ex.Message);
                            return false;
                        }
                    }

                }
                #endregion

                #region Capito
                try
                {
                    if (Driver.FindElements(By.XPath(".//input[@name='show me']")).Count > 0)
                    {
                        var buttonHoCapito = Driver.FindElement(By.XPath("//*[@class='continue']"));
                        buttonHoCapito.Click();
                        WaitingGame();
                    }
                }
                catch (Exception Ex)
                {
                    log.Warn("Catch Ho capito");
                    log.Error(Ex.Message);
                    return false;
                }
                #endregion

                if (continua)
                {
                    try
                    {
                        int numeroTentativi = 0;
                        string contentText = Driver.PageSource.ToString();
                        bool paginaCaricata = false;
                        while (!paginaCaricata)
                        {
                            if (numeroTentativi < 5)
                            {
                                numeroTentativi++;
                                if (!contentText.Contains("Hai eseguito l'accesso a troppe sessioni contemporaneamente"))
                                {
                                    Thread.Sleep(Randy(1));
                                    contentText = Driver.PageSource.ToString();
                                }
                                else
                                {
                                    log.Info("E' presente la pagina Hai eseguito l'accesso a troppe sessioni contemporaneamente");
                                    WaitSpecificPageByXPath("//input[@title = 'Continua']");
                                    IWebElement input = Driver.FindElement(By.XPath("//input[@title = 'Continua']"));
                                    input.Click();
                                    WaitingGame();
                                    contentText = Driver.PageSource.ToString();
                                }
                            }
                            else
                            {
                                log.Info("Non è presente la pagina delle troppe sessioni");
                                break;
                            }
                        }
                        if (paginaCaricata)
                        {
                            WaitSpecificPageByXPath("//input[@title = 'Continua']");
                            IWebElement input = Driver.FindElement(By.XPath("//input[@title = 'Continua']"));
                            input.Click();
                        }
                        contentText = Driver.PageSource.ToString();
                        if (contentText.Contains("Single Sign-On Error"))
                        {
                            log.Warn("Single Sign-On Error");
                            Driver = null;
                            KillChromeWebDriver();
                            return false;
                        }
                        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(sfaTimeout);
                        ClosePasswordAlert();
                    }
                    catch
                    {
                        log.Warn("Catch");
                        return false;
                    }
                }

                try { Directory.Delete(c, true); } catch (Exception Ex) { Console.WriteLine(Ex.ToString()); }

                if (LavName.StartsWith("EE112 - "))
                {
                    WaitLoadPageEE12();
                    Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(nextTimeout);
                }

                if (Driver.PageSource.ToString().Contains(tpiw) || Driver.PageSource.ToString().Contains(tscbr))
                {
                    KillChromeWebDriver();
                    log.Info(tpiw);
                    return false;
                }

                if (!Driver.PageSource.ToString().Contains(firstTextToWait))
                {
                    Thread.Sleep(Randy(20));
                    if (LavName.StartsWith("EE112 - "))
                        WaitLoadPageEE12();
                    if (!Driver.PageSource.ToString().Contains(firstTextToWait))
                    {
                        KillChromeWebDriver();
                        return false;
                    }
                }

                 return true;
            }
            catch
            {
                log.Info($"PepperYourChrome() fail");
                KillChromeWebDriver();
                return false;
            }
        }

        public bool PepperYourChromePDF(string username, string password, string webadress, string firstTextToWait, bool continua)
        {
            try
            {
                string tpiw = "This page isn’t working";
                string tscbr = "This site can’t be reached";
                string c = "C:\\Downloads\\";
                string d = c + Re(6);
                if (!Directory.Exists(d))
                    Directory.CreateDirectory(d);

                #region webrequest.js
                string w = "chrome.webRequest.onAuthRequired.addListener(function(details){" + "\n" +
                           "\t" + "return {" + "\n" +
                           "\t" + "\t" + "\t" + @"authCredentials: { username: """ + $"{username}" + @""", password: """ + $"{password}" + @""" }" + "\n" +
                           "\t" + "\t" + @"};" + "\n" +
                           @"}," + "\n" +
                           @"{urls:[""<all_urls>""]}," + "\n" +
                           @"['blocking']);";
                try
                {
                    using (FileStream ck = File.Create($"{d}\\webrequest.js"))
                    {
                        byte[] i = new UTF8Encoding(true).GetBytes(w);
                        ck.Write(i, 0, i.Length);
                    }
                }
                catch (Exception Ex) { Console.WriteLine(Ex.ToString()); }
                #endregion

                #region manifest.json
                string m = "{" + "\n" +
                           "\t" + @"""name"": ""Webrequest API""," + "\n" +
                           "\t" + @"""version"": ""1.0""," + "\n" +
                           "\t" + @"""description"": ""Authentication Window Exy666""," + "\n" +
                           "\t" + @"""permissions"": [" + "\n" +
                           "\t" + "\t" + @"""webRequest""," + "\n" +
                           "\t" + "\t" + @"""webRequestBlocking""," + "\n" +
                           "\t" + "\t" + @"""<all_urls>""" + "\n" +
                           "\t" + @"]," + "\n" +
                           "\t" + @"""background"": {" + "\n" +
                           "\t" + "\t" + @"""scripts"": [" + "\n" +
                           "\t" + "\t" + "\t" + @"""webrequest.js""" + "\n" +
                           "\t" + "\t" + @"]" + "\n" +
                           "\t" + @"}," + "\n" +
                           "\t" + @"""manifest_version"": 2" + "\n" +
                           "}";
                try
                {
                    using (FileStream ck = File.Create($"{d}\\manifest.json"))
                    {
                        byte[] i = new UTF8Encoding(true).GetBytes(m);
                        ck.Write(i, 0, i.Length);
                    }
                }
                catch (Exception Ex) { Console.WriteLine(Ex.ToString()); }
                #endregion

                var ds = ChromeDriverService.CreateDefaultService();
                ds.HideCommandPromptWindow = false;
                var options = new ChromeOptions();
                options.AddUserProfilePreference("download.default_directory", c);
                options.AddUserProfilePreference("download.prompt_for_download", false);
                options.AddUserProfilePreference("download.directory_upgrade", true);
                options.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
                //options.AddArguments("headless");
                options.AddArguments($"load-extension={d}");
                Driver = new ChromeDriver(ds, options);
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120);
                Driver.Navigate().GoToUrl(webadress);
                if (webadress == "https://enelcrmt.my.salesforce.com/")
                {
                    IWebElement inputa = Driver.FindElement(By.Id("i0116"));
                    inputa.SendKeys(username + "@enel.com");
                    IWebElement submit = Driver.FindElement(By.Id("idSIButton9"));
                    submit.Click();
                }
                else
                    WaitingGame();

                Thread.Sleep(Randy(5));

                #region OTP
                try
                {
                    if (Driver.FindElements(By.XPath(".//input[@id='verificationCodeInput']")).Count > 0)
                    {
                        VerificationCode(username);
                        if (Driver.PageSource.ToString().Contains("Please try again"))
                        {
                            Thread.Sleep(Randy(6));
                            VerificationCode(username);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    log.Warn("Catch Codice di Verifica");
                    log.Error(Ex.Message);
                    return false;
                }
                #endregion

                #region Capito
                try
                {
                    if (Driver.FindElements(By.XPath(".//input[@name='show me']")).Count > 0)
                    {
                        var buttonHoCapito = Driver.FindElement(By.XPath("//*[@class='continue']"));
                        buttonHoCapito.Click();
                        WaitingGame();
                    }
                }
                catch (Exception Ex)
                {
                    log.Warn("Catch Ho capito");
                    log.Error(Ex.Message);
                    return false;
                }
                #endregion

                if (continua)
                {
                    try
                    {
                        int numeroTentativi = 0;
                        string contentText = Driver.PageSource.ToString();
                        bool paginaCaricata = false;
                        while (!paginaCaricata)
                        {
                            if (numeroTentativi < 5)
                            {
                                numeroTentativi++;
                                if (!contentText.Contains("Hai eseguito l'accesso a troppe sessioni contemporaneamente"))
                                {
                                    Thread.Sleep(Randy(1));
                                    contentText = Driver.PageSource.ToString();
                                }
                                else
                                {
                                    log.Info("E' presente la pagina Hai eseguito l'accesso a troppe sessioni contemporaneamente");
                                    WaitSpecificPageByXPath("//input[@title = 'Continua']");
                                    IWebElement input = Driver.FindElement(By.XPath("//input[@title = 'Continua']"));
                                    input.Click();
                                    WaitingGame();
                                    contentText = Driver.PageSource.ToString();
                                }
                            }
                            else
                            {
                                log.Info("Non è presente la pagina delle troppe sessioni");
                                break;
                            }
                        }
                        if (paginaCaricata)
                        {
                            WaitSpecificPageByXPath("//input[@title = 'Continua']");
                            IWebElement input = Driver.FindElement(By.XPath("//input[@title = 'Continua']"));
                            input.Click();
                        }
                        contentText = Driver.PageSource.ToString();
                        if (contentText.Contains("Single Sign-On Error"))
                        {
                            log.Warn("Single Sign-On Error");
                            KillChromeWebDriver();
                            return false;
                        }
                        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(sfaTimeout);
                        ClosePasswordAlert();
                    }
                    catch
                    {
                        log.Warn("Catch");
                        return false;
                    }
                }

                try { Directory.Delete(c, true); } catch (Exception Ex) { Console.WriteLine(Ex.ToString()); }

                if (LavName.StartsWith("EE112 - "))
                {
                    WaitLoadPageEE12();
                    Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(nextTimeout);
                }

                if (Driver.PageSource.ToString().Contains(tpiw) || Driver.PageSource.ToString().Contains(tscbr))
                {
                    KillChromeWebDriver();
                    log.Info(tpiw);
                    return false;
                }

                if (!Driver.PageSource.ToString().Contains(firstTextToWait))
                {
                    Thread.Sleep(Randy(20));
                    if (LavName.StartsWith("EE112 - "))
                        WaitLoadPageEE12();
                    if (!Driver.PageSource.ToString().Contains(firstTextToWait))
                    {
                        KillChromeWebDriver();
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                log.Info($"PepperYourChrome() fail");
                KillChromeWebDriver();
                return false;
            }
        }

        public bool PepperYourChromeExtra(string username, string password, string webadress)
        {
            try
            {
                string c = "C:\\Downloads\\";
                string d = c + Re(6);
                if (!Directory.Exists(d))
                    Directory.CreateDirectory(d);

                #region webrequest.js
                string w = "chrome.webRequest.onAuthRequired.addListener(function(details){" + "\n" +
                           "\t" + "return {" + "\n" +
                           "\t" + "\t" + "\t" + @"authCredentials: { username: """ + $"{username}" + @""", password: """ + $"{password}" + @""" }" + "\n" +
                           "\t" + "\t" + @"};" + "\n" +
                           @"}," + "\n" +
                           @"{urls:[""<all_urls>""]}," + "\n" +
                           @"['blocking']);";
                try
                {
                    using (FileStream ck = File.Create($"{d}\\webrequest.js"))
                    {
                        byte[] i = new UTF8Encoding(true).GetBytes(w);
                        ck.Write(i, 0, i.Length);
                    }
                }
                catch (Exception Ex) { Console.WriteLine(Ex.ToString()); }
                #endregion

                #region manifest.json
                string m = "{" + "\n" +
                           "\t" + @"""name"": ""Webrequest API""," + "\n" +
                           "\t" + @"""version"": ""1.0""," + "\n" +
                           "\t" + @"""description"": ""Authentication Window Exy666""," + "\n" +
                           "\t" + @"""permissions"": [" + "\n" +
                           "\t" + "\t" + @"""webRequest""," + "\n" +
                           "\t" + "\t" + @"""webRequestBlocking""," + "\n" +
                           "\t" + "\t" + @"""<all_urls>""" + "\n" +
                           "\t" + @"]," + "\n" +
                           "\t" + @"""background"": {" + "\n" +
                           "\t" + "\t" + @"""scripts"": [" + "\n" +
                           "\t" + "\t" + "\t" + @"""webrequest.js""" + "\n" +
                           "\t" + "\t" + @"]" + "\n" +
                           "\t" + @"}," + "\n" +
                           "\t" + @"""manifest_version"": 2" + "\n" +
                           "}";
                try
                {
                    using (FileStream ck = File.Create($"{d}\\manifest.json"))
                    {
                        byte[] i = new UTF8Encoding(true).GetBytes(m);
                        ck.Write(i, 0, i.Length);
                    }
                }
                catch (Exception Ex) { Console.WriteLine(Ex.ToString()); }
                #endregion

                var ds = ChromeDriverService.CreateDefaultService();
                ds.HideCommandPromptWindow = true;
                var options = new ChromeOptions();
                options.AddArguments($"load-extension={d}");
                options.AddArgument("--start-maximized");
                options.AddArgument("--disable-notifications");
                options.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
                DriverExtra = new ChromeDriver(ds, options);
                DriverExtra.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                DriverExtra.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120);
                DriverExtra.Manage().Window.Maximize();
                DriverExtra.Navigate().GoToUrl(webadress);
                IWebElement inputa = DriverExtra.FindElement(By.Id("i0116"));
                inputa.SendKeys(username + "@enel.com");
                IWebElement submit = DriverExtra.FindElement(By.Id("idSIButton9"));
                submit.Click();
                DriverExtra.Manage().Window.Size = new System.Drawing.Size(800, 600);
                WaitingGameExtra();

                Thread.Sleep(Randy(5));

                //#region OTP
                //try
                //{
                //    if (DriverExtra.FindElements(By.XPath(".//input[@id='verificationCodeInput']")).Count > 0)
                //    {
                //        string privateKey = Agente.getOtp(username);
                //        log.Warn($"privateKey: {privateKey}");
                //        if (string.IsNullOrEmpty(privateKey)) { throw new Exception("Nessuna chiave privata recuperata da DB"); }
                //        byte[] otpKeyBytes = Base32Encoding.ToBytes(privateKey);
                //        var totp = new Totp(otpKeyBytes);
                //        var twoFactorCode = totp.ComputeTotp();
                //        log.Warn($"OTP: {twoFactorCode}");
                //        Thread.Sleep(Randy(1));
                //        if (string.IsNullOrEmpty(twoFactorCode)) { throw new Exception("Errore nel calcolo OTP"); }
                //        var inputCodiceVerifica = DriverExtra.FindElement(By.XPath(".//input[@id='verificationCodeInput']"));
                //        inputCodiceVerifica.Clear();
                //        inputCodiceVerifica.SendKeys(twoFactorCode);
                //        Thread.Sleep(Randy(1));
                //        var buttonAccedi = DriverExtra.FindElement(By.XPath(".//input[@id='signInButton']"));
                //        buttonAccedi.Click();
                //        WaitingGame();
                //        if (DriverExtra.PageSource.ToString().Contains("Please try again"))
                //        {
                //            Thread.Sleep(Randy(6));
                //            privateKey = Agente.getOtp(username);
                //            log.Warn($"privateKey: {privateKey}");
                //            if (string.IsNullOrEmpty(privateKey)) { throw new Exception("Nessuna chiave privata recuperata da DB"); }
                //            otpKeyBytes = Base32Encoding.ToBytes(privateKey);
                //            totp = new Totp(otpKeyBytes);
                //            twoFactorCode = totp.ComputeTotp();
                //            log.Warn($"OTP: {twoFactorCode}");
                //            Thread.Sleep(Randy(1));
                //            if (string.IsNullOrEmpty(twoFactorCode)) { throw new Exception("Errore nel calcolo OTP"); }
                //            inputCodiceVerifica = DriverExtra.FindElement(By.XPath(".//input[@id='verificationCodeInput']"));
                //            inputCodiceVerifica.Clear();
                //            inputCodiceVerifica.SendKeys(twoFactorCode);
                //            Thread.Sleep(Randy(1));
                //            buttonAccedi = DriverExtra.FindElement(By.XPath(".//input[@id='signInButton']"));
                //            buttonAccedi.Click();
                //            WaitingGame();
                //        }
                //    }
                //}
                //catch (Exception Ex)
                //{
                //    log.Warn("Catch Codice di Verifica");
                //    log.Error(Ex.Message);
                //    return false;
                //}
                //#endregion

                #region Capito
                try
                {
                    if (DriverExtra.FindElements(By.XPath(".//input[@name='show me']")).Count > 0)
                    {
                        var buttonHoCapito = DriverExtra.FindElement(By.XPath("//*[@class='continue']"));
                        buttonHoCapito.Click();
                        WaitingGame();
                    }
                }
                catch (Exception Ex)
                {
                    log.Warn("Catch Ho capito");
                    log.Error(Ex.Message);
                    return false;
                }
                #endregion

                try { Directory.Delete(c, true); } catch (Exception Ex) { Console.WriteLine(Ex.ToString()); }

                return true;
            }
            catch (Exception Ex)
            {
                log.Info($"PepperYourChromeExtra() fail");
                return false;
            }
        }

        public bool PepperYourSII(string username, string password, string webadress, string firstTextToWait)
        {
            try
            {
                string tpiw = "This page isn’t working";
                string c = "C:\\Downloads\\";
                if (!Directory.Exists(c))
                    Directory.CreateDirectory(c);
                var ds = ChromeDriverService.CreateDefaultService();
                ds.HideCommandPromptWindow = false;
                var options = new ChromeOptions();
                options.AddUserProfilePreference("download.default_directory", c);
                options.AddUserProfilePreference("download.prompt_for_download", false);
                options.AddUserProfilePreference("download.directory_upgrade", true);
                options.AddUserProfilePreference("safebrowsing.enabled", true);
                options.AddUserProfilePreference("safebrowsing", "enabled");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--safebrowsing-disable-download-protection");
                options.AddArgument("--start-maximized");
                options.AddArgument("--disable-notifications");
                options.AddArgument("--log-level=3");
                DriverSII = new ChromeDriver(ds, options);
                DriverSII.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                DriverSII.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120);
                DriverSII.Navigate().GoToUrl(webadress);
                WaitingGame();

                Thread.Sleep(Randy(5));

                if (!DriverSII.PageSource.ToString().Contains(firstTextToWait) || DriverSII.PageSource.ToString().Contains(tpiw))
                {
                    log.Info(tpiw);
                    KillChromeWebDriver();
                    return false;
                }

                var tUsername = DriverSII.FindElement(By.Id("_58_login"));
                tUsername.Clear();
                tUsername.SendKeys(username);
                WaitingGame();
                var tPassword = DriverSII.FindElement(By.Id("_58_password"));
                tPassword.Clear();
                tPassword.SendKeys(password);
                WaitingGame();
                var bLogin = DriverSII.FindElement(By.XPath("//*[@class='aui-button-input aui-button-input-submit']"));
                bLogin.Click();
                WaitingGame();

                if (DriverSII.PageSource.ToString().Contains("Login errato."))
                {
                    log.Info("Login errato.");
                    KillChromeWebDriver();
                    return false;
                }

                return true;
            }
            catch
            {
                log.Info($"PepperYourSII() fail");
                KillChromeWebDriver();
                return false;
            }
        }

        public bool PepperYourFOUR(string username, string password, string webadress, string firstTextToWait)
        {
            try
            {
                string tpiw = "This page isn’t working";
                string c = "C:\\Downloads\\";
                if (!Directory.Exists(c))
                    Directory.CreateDirectory(c);
                var ds = ChromeDriverService.CreateDefaultService();
                ds.HideCommandPromptWindow = false;
                var options = new ChromeOptions();
                options.AddUserProfilePreference("download.default_directory", c);
                options.AddUserProfilePreference("download.prompt_for_download", false);
                options.AddUserProfilePreference("download.directory_upgrade", true);
                options.AddUserProfilePreference("safebrowsing.enabled", true);
                options.AddUserProfilePreference("safebrowsing", "enabled");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--safebrowsing-disable-download-protection");
                options.AddArgument("--start-maximized");
                options.AddArgument("--disable-notifications");
                options.AddArgument("--log-level=3");
                DriverFOUR = new ChromeDriver(ds, options);
                DriverFOUR.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                DriverFOUR.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120);
                DriverFOUR.Navigate().GoToUrl(webadress);
                WaitingGameFOUR();

                Thread.Sleep(Randy(5));

                if (!DriverFOUR.PageSource.ToString().Contains(firstTextToWait) || DriverFOUR.PageSource.ToString().Contains(tpiw))
                {
                    log.Info(tpiw);
                    KillChromeWebDriver();
                    StartStop = false;
                    return false;
                }

                var tUsername = DriverFOUR.FindElement(By.Id("username"));
                tUsername.Clear();
                tUsername.SendKeys(username);
                var tPassword = DriverFOUR.FindElement(By.Id("password"));
                tPassword.Clear();
                tPassword.SendKeys(password);
                var bLogin = DriverFOUR.FindElement(By.Id("Login"));
                bLogin.Click();
                WaitingGameFOUR();
                int cnt = 0;
                while (!DriverFOUR.PageSource.ToString().Contains("Click per Accettare") && cnt < 15)
                {
                    WaitingGameFOUR();
                    cnt++;
                }
                var bChk = DriverFOUR.FindElement(By.Name("thePage:j_id2:i:f:pb:d:element___input____Accetta"));
                bChk.Click();
                WaitingGameFOUR();
                Thread.Sleep(Randy(1));
                //var bAvanti = DriverFOUR.FindElement(By.Name("thePage:j_id2:i:f:pb:pbb:nextAjax"));
                var bFine = DriverFOUR.FindElement(By.Name("thePage:j_id2:i:f:pb:pbb:finishAjax"));
                bFine.Click();
                WaitingGameFOUR();
                cnt = 0;
                while (!DriverFOUR.PageSource.ToString().Contains("Portale Four Trader") && cnt < 15)
                {
                    WaitingGameFOUR();
                    cnt++;
                }
                cnt = 0;

                return true;
            }
            catch
            {
                log.Info($"PepperYourFOUR() fail");
                KillChromeWebDriver();
                return false;
            }
        }

        public bool PepperYourGRAVITY(string username, string password, string webadress, string firstTextToWait)
        {
            try
            {
                string tpiw = "This page isn’t working";
                string c = "C:\\Downloads\\";
                if (!Directory.Exists(c))
                    Directory.CreateDirectory(c);
                var ds = ChromeDriverService.CreateDefaultService();
                ds.HideCommandPromptWindow = false;
                var options = new ChromeOptions();
                options.AddUserProfilePreference("download.default_directory", c);
                options.AddUserProfilePreference("download.prompt_for_download", false);
                options.AddUserProfilePreference("download.directory_upgrade", true);
               options.AddUserProfilePreference("safebrowsing.enabled", true);
               options.AddUserProfilePreference("safebrowsing", "enabled");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--safebrowsing-disable-download-protection");
                options.AddArgument("--start-maximized");
                options.AddArgument("--disable-notifications");
                options.AddArgument("--log-level=3");
                DriverGRAVITY = new ChromeDriver(ds, options);
                DriverGRAVITY.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                DriverGRAVITY.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120);
                DriverGRAVITY.Navigate().GoToUrl(webadress);
                WaitingGameGRAVITY();

                Thread.Sleep(Randy(3));

                if (!DriverGRAVITY.PageSource.ToString().Contains(firstTextToWait) || DriverGRAVITY.PageSource.ToString().Contains(tpiw))
                {
                    log.Info(tpiw);
                    KillChromeWebDriver();
                    return false;
                }

                var tUsername = DriverGRAVITY.FindElement(By.Id("UserName"));
                tUsername.Clear();
                tUsername.SendKeys(username);
                WaitingGameGRAVITY();
                var tPassword = DriverGRAVITY.FindElement(By.Id("Password"));
                tPassword.Clear();
                tPassword.SendKeys(password);
                WaitingGameGRAVITY();
                var bLogin = DriverGRAVITY.FindElement(By.Id("btn-login"));
                bLogin.Click();
                WaitingGameGRAVITY();

                return true;
            }
            catch
            {
                log.Info($"PepperYourGRAVITY() fail");
                KillChromeWebDriver();
                return false;
            }
        }

        public void PreperDeskey()
        {
           if(PepperYourDeskey("A494348", "Dominio$0006", "https://deskey.enel.com") == true)
            {
                DeskeyAutoFilterExport();
               // PepperYourDeskey("AE06184", "Dominio$0006", "https://deskey.enel.com");
            };
        }

        public bool PepperYourDeskey(string username, string password, string webadress)
        {
            try
            {
                string tpiw = "This page isn’t working";
                string tscbr = "This site can’t be reached";
                string c = "C:\\Downloads\\";
                string d = c + Re(6);
                if (!Directory.Exists(d))
                    Directory.CreateDirectory(d);


                string w = "chrome.webRequest.onAuthRequired.addListener(function(details){" + "\n" +
                           "\t" + "return {" + "\n" +
                           "\t" + "\t" + "\t" + @"authCredentials: { username: """ + $"{username}" + @""", password: """ + $"{password}" + @""" }" + "\n" +
                           "\t" + "\t" + @"};" + "\n" +
                           @"}," + "\n" +
                           @"{urls:[""<all_urls>""]}," + "\n" +
                           @"['blocking']);";
                try
                {
                    using (FileStream ck = File.Create($"{d}\\webrequest.js"))
                    {
                        byte[] i = new UTF8Encoding(true).GetBytes(w);
                        ck.Write(i, 0, i.Length);
                    }
                }
                catch (Exception Ex) { Console.WriteLine(Ex.ToString()); }

                string m = "{" + "\n" +
                           "\t" + @"""name"": ""Webrequest API""," + "\n" +
                           "\t" + @"""version"": ""1.0""," + "\n" +
                           "\t" + @"""description"": ""Authentication Window Exy666""," + "\n" +
                           "\t" + @"""permissions"": [" + "\n" +
                           "\t" + "\t" + @"""webRequest""," + "\n" +
                           "\t" + "\t" + @"""webRequestBlocking""," + "\n" +
                           "\t" + "\t" + @"""<all_urls>""" + "\n" +
                           "\t" + @"]," + "\n" +
                           "\t" + @"""background"": {" + "\n" +
                           "\t" + "\t" + @"""scripts"": [" + "\n" +
                           "\t" + "\t" + "\t" + @"""webrequest.js""" + "\n" +
                           "\t" + "\t" + @"]" + "\n" +
                           "\t" + @"}," + "\n" +
                           "\t" + @"""manifest_version"": 2" + "\n" +
                           "}";
                try
                {
                    using (FileStream ck = File.Create($"{d}\\manifest.json"))
                    {
                        byte[] i = new UTF8Encoding(true).GetBytes(m);
                        ck.Write(i, 0, i.Length);
                    }
                }
                catch (Exception Ex) { Console.WriteLine(Ex.ToString()); }
                var ds = ChromeDriverService.CreateDefaultService();
                ds.HideCommandPromptWindow = false;
                var options = new ChromeOptions();

                options.AddArguments($"load-extension={d}");
                options.AddArgument("--start-maximized");
                options.AddArgument("--disable-notifications");
                //options.AddArgument("--Incognito");
                /*options.AddUserProfilePreference("download.default_directory", c);
                options.AddUserProfilePreference("download.prompt_for_download", false);
                options.AddUserProfilePreference("download.directory_upgrade", true);
                options.AddUserProfilePreference("safebrowsing.enabled", true);
                options.AddUserProfilePreference("safebrowsing", "enabled");
                options.AddArguments($"load-extension={d}");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--safebrowsing-disable-download-protection");
                options.AddArgument("--start-maximized");
                options.AddArgument("--disable-notifications");
                options.AddArgument("--log-level=3");*/
                Driver = new ChromeDriver(ds, options);
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120);
                Driver.Navigate().GoToUrl(webadress);
                IWebElement inputa = Driver.FindElement(By.Id("i0116"));
                if (Driver.FindElements(By.Id("i0116")).Count > 0)
                {
                    inputa.SendKeys(username + "@enel.com");
                    IWebElement submit = Driver.FindElement(By.Id("idSIButton9"));
                    //Thread.Sleep(Randy(2));
                    submit.Click();
                }


                return true;
            }
            catch
            {
                log.Info($"PepperDeskey fail");
                return false;
            }

        }
        public void DeskeyAutoFilterExport()
        {
            Driver.SwitchTo().Frame("fra_main");
            try 
            {
             IWebElement filterBtn = new WebDriverWait(Driver, TimeSpan.FromSeconds(10)).Until(ExpectedConditions.ElementToBeClickable(By.Id("ContentPlaceHolderContent_IndexImgAddFilterCM")));
             filterBtn.Click();
            }
            catch (Exception ex) { }

            Driver.SwitchTo().Frame("ifrm_AddFilterNotificationCenter");
            try
            {
             IWebElement element2 = new WebDriverWait(Driver, TimeSpan.FromSeconds(10)).Until(ExpectedConditions.ElementToBeClickable(By.Id("ddl_dateEvent_chzn")));
             element2.Click();
            IWebElement element3 = new WebDriverWait(Driver, TimeSpan.FromSeconds(10)).Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a.chzn-single")));
            IWebElement element4 = new WebDriverWait(Driver, TimeSpan.FromSeconds(10)).Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[@class='chzn-single']//span[text()='Intervallo']")));
                element4.Click();
            }
            catch (Exception ex) { }


            


        }


        public void WaitSpecificPageByXPath(string XPathElementoDaCercare, int numTentativi = 50)
        {
            int tentativi = 0;
            bool paginaCaricata = false;
            while (!paginaCaricata)
            {
                if (tentativi >= numTentativi) throw new Exception("Timeout ricerca dell'elemento dopo " + numTentativi + " tentativi");
                try
                {
                    tentativi++;
                    IWebElement elemCercato = Driver.FindElement(By.XPath(XPathElementoDaCercare));
                    paginaCaricata = true;
                }
                catch
                { }
            }
        }

        public void WaitSpecificPageByXPathExtra(string XPathElementoDaCercare, int numTentativi = 50)
        {
            int tentativi = 0;
            bool paginaCaricata = false;
            while (!paginaCaricata)
            {
                if (tentativi >= numTentativi) throw new Exception("Timeout ricerca dell'elemento dopo " + numTentativi + " tentativi");
                try
                {
                    tentativi++;
                    IWebElement elemCercato = DriverExtra.FindElement(By.XPath(XPathElementoDaCercare));
                    paginaCaricata = true;
                }
                catch
                { }
            }
        }

        public bool WaitLoadPageEE12()
        {
            try
            {
                int i = 0;
                string page = Driver.PageSource.ToString();
                bool pageIsLoaded = false;
                string s = "";
                Thread.Sleep(Randy(1));
                while (!pageIsLoaded && i < 30)
                {
                    if (Driver.Url.Contains("next-unauthorized"))
                    {
                        //Accesso negato. Contattare l'amministratore di sistema.
                        Driver.Navigate().Back();
                        Thread.Sleep(Randy(1));
                    }

                    if (!s.Equals(page))
                    {
                        s = page;
                        Thread.Sleep(Randy(1));
                        page = Driver.PageSource.ToString();
                        i++;
                    }
                    else
                        pageIsLoaded = true;
                }
                return pageIsLoaded;
            }
            catch
            {
                return false;
            }
        }

        public bool WaitingGame()
        {
            try
            {
                int i = 0;
                string page = Driver.PageSource.ToString();
                bool pageIsLoaded = false;
                string s = "";
                while (!pageIsLoaded && i < 30)
                {
                    if (!s.Equals(page))
                    {
                        s = page;
                        Thread.Sleep(Randy(1));
                        page = Driver.PageSource.ToString();
                        i++;
                    }
                    else
                        pageIsLoaded = true;
                }
                return pageIsLoaded;
            }
            catch
            {
                log.Warn("WaitingGame() fail");
                return false;
            }
        }

        public bool WaitingGameExtra()
        {
            try
            {
                Thread.Sleep(Randy(2));
                int i = 0;
                string page = DriverExtra.PageSource.ToString();
                bool pageIsLoaded = false;
                string s = "";
                while (!pageIsLoaded && i < 30)
                {
                    if (!s.Equals(page))
                    {
                        s = page;
                        Thread.Sleep(Randy(1));
                        page = DriverExtra.PageSource.ToString();
                        i++;
                    }
                    else
                        pageIsLoaded = true;
                }
                return pageIsLoaded;
            }
            catch (Exception Ex)
            {
                log.Warn("WaitingGameExtra() fail");
                return false;
            }
        }

        public bool WaitingGameGRAVITY()
        {
            try
            {
                int i = 0;
                string page = DriverGRAVITY.PageSource.ToString();
                bool pageIsLoaded = false;
                string s = "";
                while (!pageIsLoaded && i < 30)
                {
                    if (!s.Equals(page))
                    {
                        s = page;
                        Thread.Sleep(Randy(1));
                        page = DriverGRAVITY.PageSource.ToString();
                        i++;
                    }
                    else
                        pageIsLoaded = true;
                }
                return pageIsLoaded;
            }
            catch
            {
                log.Warn("WaitingGameGRAVITY() fail");
                return false;
            }
        }

        public bool WaitingGameFOUR()
        {
            try
            {
                int i = 0;
                string page = DriverFOUR.PageSource.ToString();
                bool pageIsLoaded = false;
                string s = "";
                while (!pageIsLoaded && i < 30)
                {
                    if (!s.Equals(page))
                    {
                        s = page;
                        Thread.Sleep(Randy(1));
                        page = DriverFOUR.PageSource.ToString();
                        i++;
                    }
                    else
                        pageIsLoaded = true;
                }
                return pageIsLoaded;
            }
            catch
            {
                log.Warn("WaitingGameFOUR() fail");
                return false;
            }
        }

        public void KillChromeWebDriver()
        {
            foreach (Process pr in Process.GetProcesses())
            {
                if (pr.ProcessName.Contains("chromedriver") || pr.ProcessName.Contains("chrome"))
                {
                    try { pr.Kill(); } catch { }
                }
            }
            log.Info("RIP chromedriver.exe");
            Thread.Sleep(Randy(3));
        }

        public void KillSap()
        {
            foreach (Process pr in Process.GetProcesses())
            {
                if (pr.ProcessName.Contains("saplogon") || pr.ProcessName.Contains("SAP"))
                {
                    try { pr.Kill(); } catch { }
                }
            }
            log.Info("RIP SAP");
            Thread.Sleep(Randy(3));
        }

        private void VerificationCode(string username)
        {
            string privateKey = Agente.getOtp(username);
            log.Warn($"privateKey: {privateKey}");
            if (string.IsNullOrEmpty(privateKey)) { throw new Exception("Nessuna chiave privata recuperata da DB"); }
            byte[] otpKeyBytes = Base32Encoding.ToBytes(privateKey);
            var totp = new Totp(otpKeyBytes);
            var twoFactorCode = totp.ComputeTotp();
            log.Warn($"OTP: {twoFactorCode}");
            Thread.Sleep(Randy(1));
            if (string.IsNullOrEmpty(twoFactorCode)) { throw new Exception("Errore nel calcolo OTP"); }
            var inputCodiceVerifica = Driver.FindElement(By.Id("idTxtBx_SAOTCC_OTC"));
            inputCodiceVerifica.Clear();
            inputCodiceVerifica.SendKeys(twoFactorCode);
            Thread.Sleep(Randy(1));
            var buttonAccedi = Driver.FindElement(By.Id("idSubmit_SAOTCC_Continue"));
            buttonAccedi.Click();
            WaitingGame();
        }

        public bool NavigatoreExtra(string id)
        {
            DriverExtra.Navigate().GoToUrl($"https://enelcrmt.lightning.force.com/lightning/r/wrts_prcgvr__Activity__c/{id}/view");
            return true;
        }

        public int CompareTo(string a, string b)
        {
            return CalcLevenshteinDistance(a, b);
        }

        private static int CalcLevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return 0;
            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];
            for (int i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (int j = 0; j <= lengthB; distances[0, j] = j++) ;
            for (int i = 1; i <= lengthA; i++)
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = b[j - 1] == a[i - 1] ? 0 : 1;
                    distances[i, j] = Math.Min(Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1), distances[i - 1, j - 1] + cost);
                }
            return distances[lengthA, lengthB];
        }

        public void MoveToObject(IWebElement e)
        {
            try
            {
                OpenQA.Selenium.Interactions.Actions a = new OpenQA.Selenium.Interactions.Actions(Driver);
                a.MoveToElement(e);
                a.Perform();
            }
            catch
            {
                Thread.Sleep(500);
            }
        }

        public void Erry()
        {
            Thread t = new Thread(() =>
            {
                Error e = new Error();
                e.ShowDialog();
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        public void Erny()
        {
            Thread t = new Thread(() =>
            {
                ErrorOkCancel e = new ErrorOkCancel();
                e.ShowDialog();
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }
    }
}