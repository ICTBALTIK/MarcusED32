using log4net;
using log4net.Appender;
using MahApps.Metro.Controls;
using MARCUS.Controll;
using MARCUS.Helpers;
using MARCUS.Marcosi;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace MARCUS
{
    public partial class MainWindow : MetroWindow, IAppender
    {
        #region ScreenSaver
        [DllImport("kernel32.dll")]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [Flags]
        enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
        }

        public static void DisableScreenSaver()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
            log.Info($"ScreenSaver disabled");
        }

        public static void EnableScreenSaver()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
            log.Info($"ScreenSaver enabled");
        }
        #endregion

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Keanu Keanu = new Keanu();

        private BackgroundWorker Bgjnegnj5mfk34 = new BackgroundWorker();
        private BackgroundWorker Pakkjfnjfn = new BackgroundWorker();

        private bool synci = false;

        public void DoAppend(log4net.Core.LoggingEvent loggingEvent)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Action(delegate
            {
                string s = string.Format("{0} {2}", loggingEvent.TimeStamp.ToLongTimeString(), loggingEvent.Level.Name, loggingEvent.MessageObject.ToString());
                s = "\n" + s;
                switch (loggingEvent.Level.Name)
                {
                    case "ERROR": { LogP.Inlines.Add(new Run(s) { Foreground = Brushes.Red }); } break;
                    case "INFO": { LogP.Inlines.Add(new Run(s) { Foreground = Brushes.LightCyan }); } break;
                    case "DEBUG": { LogP.Inlines.Add(new Run(s) { Foreground = Brushes.Wheat }); } break;
                    case "WARN": { LogP.Inlines.Add(new Run(s) { Foreground = Brushes.Orange }); } break;
                    default: { LogP.Inlines.Add(new Run(s)); } break;
                }
                txtLog.ScrollToEnd();
                DoEvents();
            }));
        }

        public static void DoEvents() { Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new System.Action(delegate { })); }

        #region Credenziali
        private readonly string currentUser = Environment.UserName.ToUpper();
        private readonly string serverIP = "172.16.202.100";
        private readonly int serverPort = 48005;//49150 - max allowed
        private readonly Regex regexA = new Regex(@"([Aa]{1})\d{6}");
        private readonly Regex regexAE = new Regex(@"([aeAE]{2})\d{5}");
        private readonly Regex regexAF = new Regex(@"([afAF]{2})\d{5}");
        private readonly Regex regexMI = new Regex(@"([miMI]{2})\w{0,7}");
        private string abilita;
        private bool credentialsActive = false;
        private bool randomActive = false;
        private TextBox whichTextBox;
        private PasswordBox whichPassBox;
        private readonly List<string> defaultloginNext = new List<string>
        {
            "AE32239",
            "AE44270",
            "AE49955",
            "AF40599",
            "AF40601"
        };
        private List<string> makrikuli145253 = new List<string>
        {
            "AE11247",
            "AE32265",
            "AE49959",
            "AE78757",
            "AF01652",
            "AF37377",
            "AE78493",
            "AE95545",
            "AF37374",
            "AF37376",
            "AF01642",
            "AF09795"
        };
        private List<string> makrikuli62 = new List<string>
        {
            "MI000AH",
            "AF71061",
            "AF71062",
            "AF71063"     
        };//Baltik2023--- Dominio$000001
        private readonly string defaultPass = "Baltik2024///";//+PDF.cs
        private readonly string defaultPass1 = "Dominio$000001";//temp psw
        private readonly string defaultPassCQP = "Smart212023/";

        private readonly string defaultSFA = "A494279";
        private readonly string defaultCQP = "AE17965";
        private readonly string defaultloginFOUR = "rccnls77s42g942b@e-distribuzione.pt";
        private readonly string defaultpassFOUR = "Matilde*17";
        private readonly string defaultloginR2D = "A490474";
        private readonly string defaultpassR2D = "Pandino100%E";
        private readonly string defaultloginSII = "mpalumbo.556";
        private readonly string defaultpassSII = "$iiSmart28032021";
        private readonly string defaultloginsmartbotBaltik14 = "MI00007";//SF FAIL
        private readonly string defaultloginsmartbotBaltik15 = "MI00008";//SF FAIL
        private readonly string defaultloginsmartbotBaltik16 = "MI00009";//SF FAIL
        private readonly string defaultloginsmartbotBaltik17 = "MI0000C";//SF FAIL
        private readonly string defaultloginsmartbotBaltik18 = "MI0000D";//SF FAIL
        private readonly string defaultloginsmartbotBaltik19 = "MI0000E";//SF FAIL
        private readonly string defaultloginsmartbotBaltik20 = "MI0000F";//SF OK//+PDF.cs
        private readonly string arc = "MI0000F";
        private readonly string defaultloginEE62SFA = "MI000B2";
        //private readonly string defaultpassEE62SFA = "Dominio$0001";
        //private readonly string defaultpassEE176SFA = "Dominio$0001";
        private readonly string defaultloginEE62Geocall = "MI000AH";
        //private readonly string defaultpassEE62Geocall = "Enel$07102021";
        private readonly string defaultpassEE62GeocallTemp = "Enel$120920231055";
        #endregion

        public MainWindow()
        {
            Application.Current.Resources["Context"] = Keanu;
            DataContext = Application.Current.Resources["Context"];
            InitializeComponent();

            VersionLabel.Content = getRunningVersion().ToString();

            ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root.AddAppender(this);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Response", MatricolaResponse);
            IpCheck();
            log.Info($"SessionId {Keanu.Cr8SessionId(20)}");
            DisableScreenSaver();
            if (synci)
                SyncTime();
            log.Info($"{Keanu.GetTotalFreeSpace()} GB of free space");
            int freeman = Convert.ToInt32(Keanu.GetTotalFreeSpace());
            if (freeman < 5)
            {
                Pakkjfnjfn = new BackgroundWorker();
                Pakkjfnjfn.DoWork += Pakalento;
                Pakkjfnjfn.RunWorkerAsync();
            }
            //GetMatricoleListFromKredentials();
        }

        private string getRunningVersion() {

            return "NEW WIN 10 version";

            /*

            return Assembly.GetEntryAssembly().GetName().Version;

            try {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            } catch (Exception) {
                return Assembly.GetEntryAssembly().GetName().Version;
            }

            */
        }

        private void GetMatricoleListFromKredentials()
        {
            try
            {
                KredenzialiDataContext kredenziali = new KredenzialiDataContext();
                var ljistjik62 = (from f in kredenziali.tbl_matricoles where f.abilita == "62" select f.matricola).ToList();
                makrikuli62 = ljistjik62;
                foreach (var item in makrikuli62)
                {
                    if (item.Contains("AE17966"))
                    {
                        makrikuli62.Remove(item);
                        break;
                    }
                }
                foreach (var item in makrikuli62)
                {
                    if (item.Contains("AE11198"))
                    {
                        makrikuli62.Remove(item);
                        break;
                    }
                }
                log.Info($"62 {makrikuli62.Count()}");
                var ljistjik145253 = (from f in kredenziali.tbl_matricoles where f.abilita == "145 253" select f.matricola).ToList();
                makrikuli145253 = ljistjik145253;
                log.Info($"145 {makrikuli145253.Count()}");
            }
            catch (Exception Ex)
            {
                log.Info($"Get matricola list fail");
            }
        }

        private void SyncTime()
        {
            try
            {
                Process netTime = new Process();
                netTime.StartInfo.FileName = "NET.exe";
                netTime.StartInfo.Arguments = "TIME \\\\baltikpdc /SET /Y";
                netTime.Start();
                log.Info($"Time synced");
            }
            catch (Exception)
            {
                log.Info($"Time not synced");
            }
        }

        private void IpCheck()
        {
            string host = Dns.GetHostName();
            IPAddress ip = Dns.GetHostByName(host).AddressList[0];
            string ipThree = ip.ToString();
            string s = ipThree.Substring(ipThree.Length - 3);
            string ab = ipThree.Substring(ipThree.Length - 2);
            string firstIP = s.Substring(0, 1);
            if (firstIP == "." && int.Parse(ab) <= 60)
            {
                loginAGENTE.Text = "smartbotBaltik" + ab;
                synci = true;
            } else if (firstIP == "." && int.Parse(ab) >= 60 && int.Parse(ab) <= 99) {
                loginAGENTE.Text = "smartbot0" + ab;
            } else
            {
                if (firstIP == "1") { loginAGENTE.Text = "smartbot" + "3" + ab; }
                else if (firstIP == "2") { loginAGENTE.Text = "smartbot" + "4" + ab; }
                else if (firstIP == ".") { loginAGENTE.Text = "smartbot" + "0" + ab; }
            }
        }

        private void MarcusSelector(object sender, DoWorkEventArgs e)
        {
            if (Keanu.LavName.Equals("Select Coda"))
            {
                log.Error("Select coda");
                return;
            }
 
           
          
            else if (Keanu.LavName.StartsWith("EE145 - "))
            {
                EE145 ee145 = new EE145(Keanu);

                switch (Keanu.LavName)
                {
                    case "EE145 - INSERIMENTO FASE 1":
                        {
                            Keanu.LavLoginId = 3732;
                            Keanu.LavRegId = 26569;
                            Keanu.LavScartoId = 26571;
                            break;
                        }
                    case "EE145 - INSERIMENTO FASE 2":
                        {
                            Keanu.LavLoginId = 13283;
                            Keanu.LavRegId = 35430;
                            Keanu.LavScartoId = 35432;
                            break;
                        }
                    case "EE145 - NON DI COMPETENZA":
                        {
                            Keanu.LavLoginId = 16915;
                            Keanu.LavRegId = 40594;
                            Keanu.LavScartoId = 40595;
                            Keanu.LavRiclassifica = 47190;
                            break;
                        }
                    case "EE145 - MULTI MODULO":
                        {
                            Keanu.LavLoginId = 20151;
                            Keanu.LavRegId = 44572;
                            Keanu.LavScartoId = 44573;
                            Keanu.LavGiaLavorato = 44587;
                            break;
                        }
                    default:
                        return;
                }

                if (!ee145.Flow())//USE GRAVITY INSIDE (C)GLASS
                {
                    try { Bgjnegnj5mfk34.CancelAsync(); } catch { }
                    Releaser();
                    return;
                }
            }
            else if (Keanu.LavName.StartsWith("EE176 - "))
            {
                EE176 ee176 = new EE176(Keanu);

                switch (Keanu.LavName)
                {
                    case "EE176 - ALTRI DOC":
                        {
                            Keanu.LavLoginId = Costanti.CODA_EE176_ALTRI_MACRO1;
                            Keanu.ResoRecuperoDati = Costanti.EE176_MACRO1_ALTRI_RESO_RECUPERO_DATI;
                            Keanu.LavScartoId = Costanti.EE176_MACRO1_ALTRI_RESO_LAV_MANUALE;
                            Keanu.LavRegId = Costanti.EE176_MACRO1_ALTRI_RESO_OK;
                            break;
                        }
                    case "EE176 - CONTRATTI 1":
                        {
                            Keanu.LavLoginId = Costanti.CODA_EE176_MACRO1;
                            Keanu.ResoRecuperoDati = Costanti.EE176_MACRO1_RESO_RECUPERO_DATI;
                            Keanu.LavScartoId = Costanti.EE176_MACRO1_RESO_LAV_MANUALE;
                            Keanu.LavRegId = Costanti.EE176_MACRO1_RESO_OK;
                            break;
                        }
                    case "EE176 - CONTRATTI 2":
                        {
                        Keanu.LavLoginId = Costanti.CODA_EE176_MACRO2;
                            Keanu.LavScartoId = Costanti.EE176_MACRO2_RESO_LAV_MANUALE;
                            Keanu.LavRegId = Costanti.EE176_MACRO2_RESO_OK;
                            break;
                        }
                    default:
                        return;
                }

                if (!Keanu.Agente.LoginAndConnection(Keanu.LoginAGENTE, Keanu.PassAGENTE, Keanu.LavLoginId))
                {
                    log.Error("Impossibile effettuare la connessione in Agente nella coda di lavorazione");
                    return;
                }
                if (!ee176.Flow())
                {
                    try { Bgjnegnj5mfk34.CancelAsync(); } catch { }
                    Releaser();
                    return;
                }
            }
            else if (Keanu.LavName.StartsWith("EE112 - "))
            {
                EE112 ee112 = new EE112(Keanu);

                Keanu.LavLoginId = 19478;
                //Keanu.LavRegId = 43639;//Caricamento OK
                Keanu.LavRegId = 0;//Caricamento OK
                Keanu.LavScartoId = 43640;//Scarto
                Keanu.LavScartoNewReso = 45618;//Caso Complesso/Non Gestibile

                if (!ee112.Flow())//USE GRAVITY INSIDE CLASS
                {
                    try { Bgjnegnj5mfk34.CancelAsync(); } catch { }
                    if (Keanu.StartStop == true)
                    {
                        return;
                    }
                }
            }
            else if (Keanu.LavName.StartsWith("EE366 - GIUSEPPE"))
            {
                EE366 ee366 = new EE366(Keanu);

                Keanu.LavLoginId = 18471;
                Keanu.LavRegId = 42477;
                Keanu.LavScartoId = 42483;

                if (!Keanu.Agente.LoginAndConnection(Keanu.LoginAGENTE, Keanu.PassAGENTE, Keanu.LavLoginId))
                {
                    log.Error("Impossibile effettuare la connessione in Agente nella coda di lavorazione");
                    return;
                }
                if (!ee366.Flow())
                {
                    try { Bgjnegnj5mfk34.CancelAsync(); } catch { }
                    if (Keanu.StartStop == true)
                    {
                        return;
                    }
                }
            }
            else if (Keanu.LavName.StartsWith("SCODAMENTO - EE253 - FROM FILE"))
            {
                SCODAMENTOFROMFILE scodamentoFromFile = new SCODAMENTOFROMFILE(Keanu);

                if (!scodamentoFromFile.Flow())
                {
                    //try { Bgjnegnj5mfk34.CancelAsync(); } catch { }//COMMENTED, BECAUSE STOPPED ONLY IF STOP BUTTON IS PRESSED
                    return;
                }
            }
            else if (Keanu.LavName.StartsWith("SCODAMENTO - EE62 - FROM FILE"))
            {
                SCODAMENTOFROMFILE scodamentoFromFile = new SCODAMENTOFROMFILE(Keanu);

                if (!scodamentoFromFile.Flow())
                {
                    //try { Bgjnegnj5mfk34.CancelAsync(); } catch { }//COMMENTED, BECAUSE STOPPED ONLY IF STOP BUTTON IS PRESSED
                    return;
                }
            }
            else if (Keanu.LavName.StartsWith("CONTROLIER"))
            {
                Controlli Controll = new Controlli(Keanu);
                if (Controll.Flow())
                {
                    return;
                }
                //Keanu.DeskeyAutoFilterExport();
            }


            else if (Keanu.LavName.StartsWith("ED32 - SIFITES"))
            {
                SIFITES ED32sifi = new SIFITES(Keanu);

                Keanu.LavLoginId = 13856;
                Keanu.LavRegId = 37195;
                Keanu.LavScartoId = 37743;

                if (!Keanu.Agente.LoginAndConnection(Keanu.LoginAGENTE, Keanu.PassAGENTE, Keanu.LavLoginId))
                {
                    log.Error("Impossibile effettuare la connessione in Agente nella coda di lavorazione");
                    return;
                }
                if (!ED32sifi.Flow())
                {
                    try { Bgjnegnj5mfk34.CancelAsync(); } catch { }
                    if (Keanu.StartStop == true)
                    {
                        return;
                    }
                }

            }
            else if (Keanu.LavName.StartsWith("SCODAMENTO - "))
            {
                SCODAMENTO scodamento = new SCODAMENTO(Keanu);

                switch (Keanu.LavName)
                {
                    case "SCODAMENTO - EE145 - DATI CATASTALI":
                        {
                            Keanu.LavLoginId = 3722;
                            Keanu.LavRegId = 26563;
                            Keanu.IdRemainingCheck = 3731;
                            break;
                        }
                    case "SCODAMENTO - EE253 - CERTIFICAZIONI DEL CLT":
                        {
                            Keanu.LavLoginId = 5172;
                            Keanu.LavRegId = 42589;
                            Keanu.IdRemainingCheck = 17898;
                            break;
                        }
                    case "SCODAMENTO - EE253 - CONTRATTI GESTONALI":
                        {
                            Keanu.LavLoginId = 5172;
                            Keanu.LavRegId = 43177;
                            Keanu.IdRemainingCheck = 17899;
                            break;
                        }
                    case "SCODAMENTO - EE253 - 147":
                        {
                            Keanu.LavLoginId = 5172;
                            Keanu.LavRegId = 43176;
                            Keanu.IdRemainingCheck = 17450;
                            break;
                        }
                    case "SCODAMENTO - EE253 - CONTRATTI (NEW)":
                        {
                            Keanu.LavLoginId = 5172;
                            Keanu.LavRegId = 48931;
                            Keanu.IdRemainingCheck = 22068;
                            break;
                        }
                    case "SCODAMENTO - EE231 - PRESCRIZIONE":
                        {
                            Keanu.LavLoginId = 19432;
                            Keanu.LavRegId = 50077;
                            Keanu.IdRemainingCheck = 19431;
                            break;
                        }

                    case "SCODAMENTO - EE62 - SMART LETTONIA":
                        {
                            Keanu.LavLoginId = 14364;
                            Keanu.LavRegId = 37624;
                            Keanu.IdRemainingCheck = 18029;
                            break;
                        }
                    case "SCODAMENTO - EE62 - LOOP MATRICOLE":
                        {
                            Keanu.LavLoginId = 14364;
                            Keanu.LavRegId = 37624;
                            Keanu.IdRemainingCheck = 18029;
                            break;
                        }
                    default:
                        return;
                }

                if (Keanu.LavName.Equals("SCODAMENTO - EE62 - LOOP MATRICOLE")) {

                    var lastMatrikula = makrikuli62.Last();

                    for (int i = 0; i < lastMatrikula.Length; i++) {
                        var matrikula = makrikuli62[i];
                        Keanu.LoginGEOCALL = matrikula + "@ENELINT.GLOBAL";

                        if (matrikula.Equals("MI000AH"))
                            Keanu.PassGEOCALL = defaultpassEE62GeocallTemp; //defaultPass;//MI000AH
                        else
                            Keanu.PassGEOCALL = defaultpassEE62GeocallTemp;//AF71061, AF71062, AF71063
                        //Keanu.PassGEOCALL = defaultPass;//AF71061, AF71062, AF71063

                        if (matrikula == lastMatrikula) {
                            i = -1;
                        }

                        if (!scodamento.Flow()) {
                            continue;
                        }

                    }
                }
              
                /*
                 {
                    foreach (var item in makrikuli62)
                    {
                        //Keanu.LoginSFA = item;
                        //Keanu.PassSFA = defaultPass;
                        Keanu.LoginGEOCALL = item + "@ENELINT.GLOBAL";
                        if (item.Equals("MI000AH"))
                            Keanu.PassGEOCALL = defaultPass;//MI000AH
                        else
                            Keanu.PassGEOCALL = defaultpassEE62GeocallTemp;//AF71061, AF71062, AF71063
                        //Keanu.PassGEOCALL = defaultPass;//AF71061, AF71062, AF71063
                        if (!scodamento.Flow())
                        {
                            continue;
                        }
                    }

                    Keanu.KillChromeWebDriver();
                    Keanu.Agente.Logout();
                    return;
                }
                */
                else {
                    if (!scodamento.Flow())//USE GRAVITY INSIDE CLASS
                    {
                        try { Bgjnegnj5mfk34.CancelAsync(); } catch { }
                        Releaser();//BECAUSE USING MI000000000
                        return;
                    }
                }

                //if (!scodamento.Flow())//USE GRAVITY INSIDE CLASS
                //{
                //    try { Bgjnegnj5mfk34.CancelAsync(); } catch { }
                //    //Releaser();//BECAUSE USING MI000000000
                //    return;
                //}
            }
            else if (Keanu.LavName.StartsWith("PDF - "))
            {
                PDF pdf = new PDF(Keanu);

                switch (Keanu.LavName)
                {
                    case "PDF - ED15 - INS. DATI CATASTALI":
                        {
                            Keanu.LavLoginId = 3722;
                            Keanu.LavRegId = 26563;
                            Keanu.IdRemainingCheck = 3731;
                            Keanu.PdfType = 151;
                            break;
                        }
                    case "PDF - ED15 - INS. DATI CATASTALI SENZA DATI":
                        {
                            Keanu.LavLoginId = 5172;
                            Keanu.LavRegId = 42589;
                            Keanu.IdRemainingCheck = 17898;
                            Keanu.PdfType = 1080;
                            break;
                        }
                    case "PDF - EE145 - RECUPERO DATI":
                        {
                            Keanu.LavLoginId = 5172;
                            Keanu.LavRegId = 43177;
                            Keanu.IdRemainingCheck = 43177;
                            Keanu.PdfType = 3731;
                            break;
                        }
                    case "PDF - EE62 - DimPag in SAP": {
                        Keanu.LavLoginId = 0;
                        Keanu.LavRegId = 0;
                        Keanu.IdRemainingCheck = 0;
                        Keanu.PdfType = 18029;
                        break;
                    }
                    case "PDF - EE253 - 147": {
                        Keanu.LavLoginId = 0;
                        Keanu.LavRegId = 0;
                        Keanu.IdRemainingCheck = 0;
                        Keanu.PdfType = 17450;
                        break;
                    }
                    case "PDF - EE253 - Cert. del clt": {
                        Keanu.LavLoginId = 0;
                        Keanu.LavRegId = 0;
                        Keanu.IdRemainingCheck = 0;
                        Keanu.PdfType = 17898;
                        break;
                    }
                    case "PDF - EE253 - DMS-Contratti":
                        {
                            Keanu.LavLoginId = 0;
                            Keanu.LavRegId = 0;
                            Keanu.IdRemainingCheck = 0;
                            Keanu.PdfType = 22068;
                            break;
                        }

                    case "PDF - EE231 - PRESCRIZIONE":
                        {
                            Keanu.LavLoginId = 0;
                            Keanu.LavRegId = 0;
                            Keanu.IdRemainingCheck = 0;
                            Keanu.PdfType = 19434;
                            break;
                        }
                    case "PDF - EE253 - Contr. Gest.": {
                        Keanu.LavLoginId = 0;
                        Keanu.LavRegId = 0;
                        Keanu.IdRemainingCheck = 0;
                        Keanu.PdfType = 17899;
                        break;
                    }
                    default:
                        return;
                }

                if (!pdf.Flow())
                {
                    try { Bgjnegnj5mfk34.CancelAsync(); } catch { }
                    return;
                }
            }
            else if (Keanu.LavName.StartsWith("BO SOSPESI - "))
            {
                SOSPESI sospesi = new SOSPESI(Keanu);

                if (!sospesi.Flow())
                {
                    try { Bgjnegnj5mfk34.CancelAsync(); } catch { }
                    return;
                }
            }
            else if (Keanu.LavName.StartsWith("SOSPENDI - "))
            {
                SOSPENDI sospendi = new SOSPENDI(Keanu);

                sospendi.Flow();

                try { Bgjnegnj5mfk34.CancelAsync(); } catch { }
                //Releaser();//BECAUSE USING MI000000000
                return;
            }
            else if (Keanu.LavName.StartsWith("SFA CHECKER"))
            {
                SFACHECKER sfachecker = new SFACHECKER(Keanu);

                sfachecker.Flow();

                try { Bgjnegnj5mfk34.CancelAsync(); } catch { }
                //Releaser();//BECAUSE USING MI000000000
                return;
            }
        }

        private void MarcusSelector_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //log.Info("Tick");
        }

        private void MarcusSelector_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)//log.Error("Errored");
            {
                BtnStart.IsEnabled = true;
                return;
            }
            else if (e.Cancelled)//log.Info("Stopped");
            {
                BtnStart.IsEnabled = true;
                return;
            }
            else//log.Info("Finished");
            {
                BtnStart.IsEnabled = true;
                return;
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            Kewieffnwensss();
        }

        private void Kewieffnwensss()
        {
            Keanu.Bad.Fatto = 0;
            Keanu.Bad.Scarto = 0;
            Keanu.Bad.Sospeso = 0;
            Keanu.StartStop = true;
            BtnStart.IsEnabled = false;
            TxtBoxer();
            Keanu.LavName = CmbLavorazioni.Text.ToString();
            #region Configure Threday
            Bgjnegnj5mfk34 = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            Bgjnegnj5mfk34.DoWork += MarcusSelector;
            Bgjnegnj5mfk34.ProgressChanged += MarcusSelector_ProgressChanged;
            Bgjnegnj5mfk34.RunWorkerCompleted += MarcusSelector_RunWorkerCompleted;
            Bgjnegnj5mfk34.RunWorkerAsync();
            #endregion
        }

        private void TxtBoxer()//USE textbox.Text BECAUSE THEN IT IS POSSIBLE TO EDIT MATRICOLA/PASSWORD WITHOUT UPDATING MARCUS
        {
            Keanu.LoginAGENTE = loginAGENTE.Text;
            Keanu.PassAGENTE = "appaut";
            Keanu.LoginSFA = loginSFA.Text;
            Keanu.PassSFA = passSFA.Password;
            Keanu.LoginGEOCALL = loginGEOCALL.Text + "@ENELINT.GLOBAL";
            Keanu.PassGEOCALL = passGEOCALL.Password;
            Keanu.LoginSAP = loginSAP.Text;
            Keanu.PassSAP = passSAP.Password;
            Keanu.LoginNEXT = loginNEXT.Text + "@ENELINT.GLOBAL";
            Keanu.PassNEXT = passNEXT.Password;
            Keanu.LoginFOUR = loginFOUR.Text;
            Keanu.PassFOUR = passFOUR.Password;
            Keanu.LoginR2D = loginR2D.Text + "@ENELINT.GLOBAL";
            Keanu.PassR2D = passR2D.Password;
            Keanu.LoginSII = loginSII.Text;
            Keanu.PassSII = passSII.Password;
            Keanu.From = "1350";
            Keanu.To = "2255";
            Keanu.SospendiTipo = CmbTipo.Text;
            Keanu.ScodamentoMin = int.Parse(txtMin.Text);
            Keanu.ScodamentoMax = int.Parse(txtMax.Text);
            if (cModificaPodTrip.IsChecked == true)
                Keanu.ModificaPodTrip = true;
            else
                Keanu.ModificaPodTrip = false;
        }

        private void RequestMatricola(string abilita)
        {
            try { NetworkComms.SendObject("Request", serverIP, serverPort, abilita + ":" + currentUser); }
            catch (Exception Ex) { log.Info($"{abilita} {Ex.Message}"); }
        }

        private void MatricolaResponse(PacketHeader header, Connection connection, string message)
        {
            string[] array = message.Split(':');
            ServerResponse response = new ServerResponse
            {
                Matricola = array[0],
                Abilita = array[1],
                Password = array[2]
            };
            Dispatcher.Invoke(() => { AssignToTextBox(response); });
        }

        private void AssignToTextBox(ServerResponse response)
        {
            string matricola = response.Matricola;
            Match matchA = regexA.Match(matricola);
            Match matchAE = regexAE.Match(matricola);
            Match matchAF = regexAF.Match(matricola);
            Match matchMI = regexMI.Match(matricola);

            switch (abilita)
            {
                case "145 253":
                    if (matchA.Success || matchAE.Success || matchAF.Success || matchMI.Success)
                    {
                        //SFA
                        loginSFA.Text = response.Matricola;
                        if (string.IsNullOrEmpty(response.Password))
                            response.Password = defaultPass;
                        passSFA.Password = response.Password;
                        //GEOCALL
                        loginGEOCALL.Text = response.Matricola;
                        if (string.IsNullOrEmpty(response.Password))
                            response.Password = defaultPass;
                        passGEOCALL.Password = response.Password;

                        TakeMatricola145253.IsEnabled = false;
                        TakeMatricola62.IsEnabled = false;

                        randomActive = false;
                        loginSFA.Foreground = Brushes.White;

                        log.Info($"{response.Matricola} {response.Abilita} taken");
                    }
                    else
                        RandomMatrikoler(makrikuli145253);
                    break;
                case "62":
                    if (matchA.Success || matchAE.Success || matchAF.Success || matchMI.Success)
                    {
                        //SFA
                        loginSFA.Text = response.Matricola;
                        if (string.IsNullOrEmpty(response.Password))
                            response.Password = defaultPass;
                        passSFA.Password = response.Password;
                        //GEOCALL
                        loginGEOCALL.Text = response.Matricola;
                        if (string.IsNullOrEmpty(response.Password))
                            response.Password = defaultPass;
                        passGEOCALL.Password = response.Password;

                        TakeMatricola145253.IsEnabled = false;
                        TakeMatricola62.IsEnabled = false;

                        randomActive = false;
                        loginSFA.Foreground = Brushes.White;

                        log.Info($"{response.Matricola} {response.Abilita} taken");
                    }
                    else
                        RandomMatrikoler(makrikuli62);
                    break;
                case "176":
                    if (matchA.Success || matchAE.Success || matchAF.Success || matchMI.Success)
                    {
                        //SFA
                        loginSFA.Text = response.Matricola;
                        if (string.IsNullOrEmpty(response.Password))
                            response.Password = defaultPass1;
                        //response.Password = defaultpassEE176SFA;
                        passSFA.Password = response.Password;

                        TakeMatricola176.IsEnabled = false;

                        randomActive = false;
                        loginSFA.Foreground = Brushes.White;

                        log.Info($"{response.Matricola} {response.Abilita} taken");
                    }
                    else
                        log.Info($"No free matricola for {abilita}");
                    break;
                default:
                    if (matchA.Success || matchAE.Success || matchAF.Success || matchMI.Success)
                    {
                        whichTextBox.Text = response.Matricola;
                        if (string.IsNullOrEmpty(response.Password))
                            response.Password = defaultPass;
                        whichPassBox.Password = response.Password;
                        log.Info($"{response.Matricola} {response.Abilita} taken");
                    }
                    else
                        log.Info($"No free matricola for {abilita}");
                    break;
            }
        }

        private void RandomMatrikoler(List<string> list)
        {
            //log.Info($"No free matricola for {abilita}");
            int r = Keanu.Rand.Next(makrikuli145253.Count());
            loginSFA.Text = list[r];
            passSFA.Password = defaultPass1;
            //loginSFA.Foreground = Brushes.Red;
            //if (CmbLavorazioni.Text.ToString().Equals("SCODAMENTO - EE62 - FROM FILE"))
            //{
            //    loginGEOCALL.Text = loginSFA.Text;
            //    passGEOCALL.Password = defaultPass;
            //    loginGEOCALL.Foreground = Brushes.Red;
            //}
            //randomActive = true;
            //log.Error($"Random {list[r]} from {abilita}");
        }

        private bool ReturnMatricola(TextBox textBox)
        {
            try
            {
                string matricola = textBox.Text.Replace(@"ENELINT\", "").Replace(@"@ENELINT.GLOBAL", "").Replace(@"ENELINT.GLOBAL\", "").Trim();
                if (string.IsNullOrEmpty(matricola))
                    return false;
                Match matchA = regexA.Match(matricola);
                Match matchAE = regexAE.Match(matricola);
                Match matchAF = regexAF.Match(matricola);
                Match matchMI = regexMI.Match(matricola);
                if (matchA.Success || matchAE.Success || matchAF.Success || matchMI.Success)
                {
                    try
                    {
                        NetworkComms.SendObject("Return", serverIP, serverPort, matricola + ":" + abilita + ":" + currentUser);
                        log.Info($"{matricola} {abilita} released");
                        return true;
                    }
                    catch (Exception Ex)
                    {
                        log.Info("MARCUS" + "\t" + Ex.Message + "(" + matricola + ":" + abilita + ":" + currentUser + ")");
                        log.Error($"DB error");
                        return false;
                    }
                }
            }
            catch
            {
                log.Warn($"ReturnMatricola() fail");
            }
            return true;
        }

        private void Credentials()
        {
            if (!credentialsActive)
            {
                Keanu.LavName = CmbLavorazioni.Text.ToString();
                if (string.IsNullOrEmpty(Keanu.LavName)) { log.Error("SELECT CODA"); return; }
                else if (Keanu.LavName.StartsWith("EE62 - "))
                {
                    //SAP
                    abilita = "SAP";
                    RequestMatricola(abilita);
                    whichTextBox = loginSAP;
                    whichPassBox = passSAP;
                    //SFA
                    loginSFA.Text = defaultSFA;
                    passSFA.Password = defaultPass;
                    credentialsActive = true;
                }
                else if (Keanu.LavName.StartsWith("EE145 - "))
                {
                    //SPECIFIC TAKE
                }
                else if (Keanu.LavName.StartsWith("EE112 - "))
                {
                    //NEXT
                    int r = Keanu.Rand.Next(defaultloginNext.Count);
                    loginNEXT.Text = defaultloginNext[r];
                    passNEXT.Password = defaultPass;
                }
                else if (Keanu.LavName.StartsWith("EE366 - "))
                {
                    //FOUR
                    loginFOUR.Text = defaultloginFOUR;
                    passFOUR.Password = defaultpassFOUR;
                    //R2D
                    loginR2D.Text = defaultloginR2D;
                    passR2D.Password = defaultpassR2D;
                    //SII
                    loginSII.Text = defaultloginSII;
                    passSII.Password = defaultpassSII;
                    credentialsActive = true;
                }
                BtnMenu.Content = abilita;
            }
            else
            {
                if (!string.IsNullOrEmpty(abilita))
                    ReturnMatricola(whichTextBox);

                whichTextBox = null;
                whichPassBox = null;

                loginSFA.Text = "";
                passSFA.Password = "";
                loginGEOCALL.Text = "";
                passGEOCALL.Password = "";
                loginSAP.Text = "";
                passSAP.Password = "";
                loginNEXT.Text = "";
                passNEXT.Password = "";
                loginFOUR.Text = "";
                passFOUR.Password = "";
                loginR2D.Text = "";
                passR2D.Password = "";
                loginSII.Text = "";
                passSII.Password = "";

                abilita = "";
                BtnMenu.Content = abilita;
                credentialsActive = false;
            }
        }

        private void BtnCredentials_Click(object sender, RoutedEventArgs e)
        {
            Credentials();
        }

        private void CmbLavorazioni_DropDownClosed(object sender, EventArgs e)
        {
            if (CmbLavorazioni.Text.ToString().StartsWith("SCODAMENTO - "))
            {
                TakeMatricola.Visibility = Visibility.Visible;
                ReleaseMatricola.Visibility = Visibility.Visible;
                BtnCredentials.Visibility = Visibility.Collapsed;

                TakeMatricola62.IsEnabled = false;
                TakeMatricola145253.IsEnabled = false;
                ReleaseMatricola.IsEnabled = false;

                if (CmbLavorazioni.Text.ToString().Equals("SCODAMENTO - EE253 - FROM FILE") || CmbLavorazioni.Text.ToString().Equals("SCODAMENTO - EE62 - FROM FILE"))
                    scodamentoCombo.IsEnabled = true;

                else
                    scodamentoCombo.IsEnabled = true;

                sfaGroupb.Visibility = Visibility.Visible;
                geocallGroupb.Visibility = Visibility.Visible;

                if (CmbLavorazioni.Text.ToString().Equals("SCODAMENTO - EE145 - DATI CATASTALI"))
                    txtMax.Text = "30000";
                if (CmbLavorazioni.Text.ToString().Equals("SCODAMENTO - EE231 - PRESCRIZIONE"))
                    txtMax.Text = "30000";


                if (CmbLavorazioni.Text.ToString().Equals("SCODAMENTO - EE253 - CONTRATTI (NEW)"))
                {
                    txtMax.Text = "70";
                    txtMin.Text = "50";
                    
                }
                    

                if (CmbLavorazioni.Text.ToString().Equals("SCODAMENTO - EE145 - DATI CATASTALI") ||
                    CmbLavorazioni.Text.ToString().Equals("SCODAMENTO - EE253 - CERTIFICAZIONI DEL CLT") ||
                    CmbLavorazioni.Text.ToString().Equals("SCODAMENTO - EE253 - CONTRATTI GESTONALI") ||
                    CmbLavorazioni.Text.ToString().Equals("SCODAMENTO - EE253 - CONTRATTI (NEW)") ||
                    CmbLavorazioni.Text.ToString().Equals("SCODAMENTO - EE231 - PRESCRIZIONE") ||
                    CmbLavorazioni.Text.ToString().Equals("SCODAMENTO - EE253 - 147") ||
                    CmbLavorazioni.Text.ToString().Equals("SCODAMENTO - EE253 - FROM FILE"))
                {
                    cModificaPodTrip.IsChecked = true;
                    if (loginAGENTE.Text.Equals("smartbotBaltik14") || loginAGENTE.Text.Equals("smartbotbaltik14"))
                    {
                        //loginSFA.Text = defaultloginsmartbotBaltik14;
                        loginSFA.Text = defaultloginsmartbotBaltik20;
                        loginGEOCALL.Text = defaultloginsmartbotBaltik14;
                        passSFA.Password = defaultPass1;
                        passGEOCALL.Password = defaultPass1;
                    }
                    else if (loginAGENTE.Text.Equals("smartbotBaltik15") || loginAGENTE.Text.Equals("smartbotbaltik15"))
                    {
                        //loginSFA.Text = defaultloginsmartbotBaltik15;
                        loginSFA.Text = defaultloginsmartbotBaltik20;
                        loginGEOCALL.Text = defaultloginsmartbotBaltik15;
                        passSFA.Password = defaultPass1;
                        passGEOCALL.Password = defaultPass;
                    }
                    else if (loginAGENTE.Text.Equals("smartbotBaltik16") || loginAGENTE.Text.Equals("smartbotbaltik16"))
                    {
                        //loginSFA.Text = defaultloginsmartbotBaltik16;
                        loginSFA.Text = defaultloginsmartbotBaltik20;
                        loginGEOCALL.Text = defaultloginsmartbotBaltik16;
                        passSFA.Password = defaultPass1;
                        passGEOCALL.Password = defaultPass1;
                    }
                    else if (loginAGENTE.Text.Equals("smartbotBaltik17") || loginAGENTE.Text.Equals("smartbotbaltik17"))
                    {
                        //loginSFA.Text = defaultloginsmartbotBaltik17;
                        loginSFA.Text = defaultloginsmartbotBaltik20;
                        loginGEOCALL.Text = defaultloginsmartbotBaltik17;
                        passSFA.Password = defaultPass1;
                        passGEOCALL.Password = defaultPass1;
                    }
                    else if (loginAGENTE.Text.Equals("smartbotBaltik18") || loginAGENTE.Text.Equals("smartbotbaltik18"))
                    {
                        //loginSFA.Text = defaultloginsmartbotBaltik18;
                        loginSFA.Text = defaultloginsmartbotBaltik20;
                        loginGEOCALL.Text = defaultloginsmartbotBaltik18;
                        passSFA.Password = defaultPass1;
                        passGEOCALL.Password = defaultPass1;
                    }
                    else if (loginAGENTE.Text.Equals("smartbotBaltik19") || loginAGENTE.Text.Equals("smartbotbaltik19"))
                    {
                        //loginSFA.Text = defaultloginsmartbotBaltik19;
                        loginSFA.Text = defaultloginsmartbotBaltik20;
                        loginGEOCALL.Text = defaultloginsmartbotBaltik19;
                        passSFA.Password = defaultPass1;
                        passGEOCALL.Password = defaultPass1;
                    }
                    else if (loginAGENTE.Text.Equals("smartbotBaltik20") || loginAGENTE.Text.Equals("smartbotbaltik20"))
                    {
                        loginSFA.Text = defaultloginsmartbotBaltik20;
                        passSFA.Password = defaultPass1;
                        loginGEOCALL.Text = defaultloginsmartbotBaltik20;
                        passGEOCALL.Password = defaultPass1;
                    }
                    else { }

                    //loginGEOCALL.Text = loginSFA.Text;
                    //passGEOCALL.Password = passSFA.Password;
                }
                else if (CmbLavorazioni.Text.ToString().Equals("SCODAMENTO - EE62 - LOOP MATRICOLE"))
                {
                    txtMax.Text = "300";
                    sfaGroupb.IsEnabled = true;
                    geocallGroupb.IsEnabled = true;
                    loginSFA.Text = defaultloginEE62SFA;
                    //passSFA.Password = defaultpassEE62SFA;
                    passSFA.Password = defaultPass1;
                    loginGEOCALL.Text = defaultloginEE62Geocall;
                    passGEOCALL.Password = defaultpassEE62GeocallTemp;
                }
                else//SCODAMENTO - EE62 - SMART LETTONIA && SCODAMENTO - EE62 - FROM FILE
                {
                    txtMax.Text = "300";
                    loginSFA.Text = defaultloginEE62SFA;
                    //passSFA.Password = defaultpassEE62SFA;
                    passSFA.Password = defaultPass;
                    loginGEOCALL.Text = defaultloginEE62Geocall;
                    passGEOCALL.Password = defaultpassEE62GeocallTemp;
                }
            }
            else if (CmbLavorazioni.Text.ToString().StartsWith("EE145 - "))
            {
                TakeMatricola.Visibility = Visibility.Visible;
                ReleaseMatricola.Visibility = Visibility.Visible;
                BtnCredentials.Visibility = Visibility.Collapsed;

                sfaGroupb.Visibility = Visibility.Visible;

                TakeMatricola62.IsEnabled = false;
                TakeMatricola145253.IsEnabled = true;
                ReleaseMatricola.IsEnabled = true;
            }
            else if (CmbLavorazioni.Text.ToString().StartsWith("EE62 - "))
            {
                sfaGroupb.Visibility = Visibility.Visible;
                sapGroupb.Visibility = Visibility.Visible;
            }
            else if (CmbLavorazioni.Text.ToString().StartsWith("EE112 - "))
            {
                nextGroupb.Visibility = Visibility.Visible;
                BtnDuplicati.Visibility = Visibility.Visible;
            }
            else if (CmbLavorazioni.Text.ToString().StartsWith("EE176 - "))
            {
                if (CmbLavorazioni.Text.ToString().Equals("EE176 - CONTRATTI 1") ||
                    CmbLavorazioni.Text.ToString().Equals("EE176 - CONTRATTI 2"))
                {
                    TakeMatricolaEE176.Visibility = Visibility.Visible;
                    ReleaseMatricola.Visibility = Visibility.Visible;
                    BtnCredentials.Visibility = Visibility.Collapsed;

                    sfaGroupb.Visibility = Visibility.Visible;

                    TakeMatricola176.IsEnabled = true;
                    ReleaseMatricola.IsEnabled = true;
                }
                else//EE176 - ALTRI DOC
                    BtnCredentials.IsEnabled = false;
            }
            else if (CmbLavorazioni.Text.ToString().StartsWith("EE366 - "))
            {
                fourGroupb.Visibility = Visibility.Visible;
                r2dGroupb.Visibility = Visibility.Visible;
                siiGroupb.Visibility = Visibility.Visible;

                BtnCredentials.IsEnabled = false;
            }
            else if (CmbLavorazioni.Text.ToString().StartsWith("PDF - "))
            {
                BtnCredentials.IsEnabled = false;
            }
            else if (CmbLavorazioni.Text.ToString().StartsWith("BO SOSPESI - "))
            {
                GroupBoxMain.Visibility = Visibility.Collapsed;
                GroupBoxSospeso.Visibility = Visibility.Visible;
            }
            else if (CmbLavorazioni.Text.ToString().StartsWith("SOSPENDI - "))
            {
                BtnCredentials.IsEnabled = false;

                sfaGroupb.Visibility = Visibility.Visible;
                geocallGroupb.Visibility = Visibility.Visible;
                sospendiCombo.Visibility = Visibility.Visible;
                scodamentoCombo.Visibility = Visibility.Collapsed;

                loginSFA.Text = arc;
                passSFA.Password = defaultPass;
                loginGEOCALL.Text = defaultCQP;
                passGEOCALL.Password = defaultPassCQP;
            }
            else if (CmbLavorazioni.Text.ToString().StartsWith("SFA CHECKER"))
            {
                BtnUpload.Visibility = Visibility.Visible;
                TakeMatricola.Visibility = Visibility.Collapsed;
                ReleaseMatricola.Visibility = Visibility.Collapsed;
                BtnCredentials.Visibility = Visibility.Collapsed;

                sfaGroupb.Visibility = Visibility.Visible;

                loginSFA.Text = defaultloginsmartbotBaltik20;
                passSFA.Password = defaultPass1;
            }

            Credentials();
        }

        private void CmbLavorazioni_DropDownOpened(object sender, EventArgs e)
        {
            GroupBoxMain.Visibility = Visibility.Visible;
            GroupBoxSospeso.Visibility = Visibility.Collapsed;

            Releaser();

            txtMax.Text = "400";

            TakeMatricola.Visibility = Visibility.Collapsed;
            TakeMatricolaEE176.Visibility = Visibility.Collapsed;
            ReleaseMatricola.Visibility = Visibility.Collapsed;
            BtnCredentials.Visibility = Visibility.Visible;
            BtnCredentials.IsEnabled = true;
            BtnUpload.Visibility = Visibility.Collapsed;

            scodamentoCombo.Visibility = Visibility.Visible;
            scodamentoCombo.IsEnabled = false;
            sospendiCombo.Visibility = Visibility.Collapsed;

            sfaGroupb.Visibility = Visibility.Collapsed;
            geocallGroupb.Visibility = Visibility.Collapsed;
            sapGroupb.Visibility = Visibility.Collapsed;
            nextGroupb.Visibility = Visibility.Collapsed;
            fourGroupb.Visibility = Visibility.Collapsed;
            r2dGroupb.Visibility = Visibility.Collapsed;
            siiGroupb.Visibility = Visibility.Collapsed;

            sfaGroupb.IsEnabled = true;
            geocallGroupb.IsEnabled = true;
            sapGroupb.IsEnabled = true;
            nextGroupb.IsEnabled = true;
            fourGroupb.IsEnabled = true;
            r2dGroupb.IsEnabled = true;
            siiGroupb.IsEnabled = true;

            BtnDuplicati.Visibility = Visibility.Collapsed;
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            log.Error("STOP BUTTON WAS PRESSED!");
            Keanu.StartStop = false;
            try { Bgjnegnj5mfk34.CancelAsync(); } catch { }
        }

        private void BtnBrutality_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                log.Error("EMERGENCY SCARTO");
                Keanu.KillChromeWebDriver();
                Keanu.TimeToRestart = false;
                Keanu.StartStop = false;
                try { Bgjnegnj5mfk34.CancelAsync(); } catch { }
                log.Warn("Wait... for thread to finish");
            }
            catch
            {
                log.Error("FAIL");
            }
        }

        private void BtnFatality_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                log.Error("EMERGENCY SOSPESO");
                Keanu.KillChromeWebDriver();
                Keanu.TimeToSospeso = true;
                Keanu.StartStop = false;
                try { Bgjnegnj5mfk34.CancelAsync(); } catch { }
                log.Warn("Wait... for thread to finish");
            }
            catch
            {
                log.Error("FAIL");
            }
        }

        private void Gol_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("C:\\MARCUS\\");
        }

        private void TakeMatricola62_Click(object sender, RoutedEventArgs e)
        {
            abilita = "62";
            RequestMatricola(abilita);
            whichTextBox = loginSFA;
            whichPassBox = passSFA;
            BtnMenu.Content = abilita;
        }

        private void TakeMatricola145253_Click(object sender, RoutedEventArgs e)
        {
            abilita = "145 253";
            RequestMatricola(abilita);
            whichTextBox = loginSFA;
            whichPassBox = passSFA;
            BtnMenu.Content = abilita;
        }

        private void TakeMatricola176_Click(object sender, RoutedEventArgs e)
        {
            abilita = "176";
            RequestMatricola(abilita);
            whichTextBox = loginSFA;
            whichPassBox = passSFA;
            BtnMenu.Content = abilita;
        }

        private void Releaser()
        {
            if (randomActive)
            {
                log.Info("randomActive, can't release");
                log.Info("Clear anyway");
                //loginSFA.Text = "";
                //passSFA.Password = "";
                //loginGEOCALL.Text = "";
                //passGEOCALL.Password = "";
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    if (!string.IsNullOrEmpty(abilita))
                        ReturnMatricola(whichTextBox);

                    whichTextBox = null;
                    whichPassBox = null;

                    //loginSFA.Text = "";
                    //passSFA.Password = "";
                    loginGEOCALL.Text = "";
                    passGEOCALL.Password = "";
                    loginSAP.Text = "";
                    passSAP.Password = "";
                    loginNEXT.Text = "";
                    passNEXT.Password = "";
                    loginFOUR.Text = "";
                    passFOUR.Password = "";
                    loginR2D.Text = "";
                    passR2D.Password = "";
                    loginSII.Text = "";
                    passSII.Password = "";

                    abilita = "";
                    BtnMenu.Content = abilita;
                    credentialsActive = false;

                    if (CmbLavorazioni.Text.ToString().Equals("SCODAMENTO - EE62 - SMART LETTONIA") || CmbLavorazioni.Text.ToString().Equals("SCODAMENTO - EE62 - FROM FILE"))
                    {
                        TakeMatricola145253.IsEnabled = false;
                        TakeMatricola62.IsEnabled = true;
                    }
                    else if (CmbLavorazioni.Text.ToString().StartsWith("EE145 -"))
                    {
                        TakeMatricola145253.IsEnabled = true;
                        TakeMatricola62.IsEnabled = false;
                    }
                    else if (CmbLavorazioni.Text.ToString().StartsWith("EE176 -"))
                    {
                        TakeMatricola176.IsEnabled = true;
                    }
                    else
                    {
                        TakeMatricola145253.IsEnabled = true;
                        TakeMatricola62.IsEnabled = true;
                    }
                });
            }
        }

        private void ReleaseMatricola_Click(object sender, RoutedEventArgs e)
        {
            Releaser();
        }

        private void BtnPakalento_Click(object sender, RoutedEventArgs e)
        {
            Pakkjfnjfn = new BackgroundWorker();
            Pakkjfnjfn.DoWork += Pakalento;
            Pakkjfnjfn.RunWorkerAsync();
        }

        private void Pakalento(object sender, DoWorkEventArgs e)
        {
            log.Info($"Pakalento...");
            List<string> pakalentoList = new List<string>
            {
                Environment.GetFolderPath(Environment.SpecialFolder.InternetCache),
                "C:\\Windows\\Temp\\",
                "C:\\DOCUME~1\\" + currentUser + "\\Temp\\",
                "C:\\Documents and Settings\\" + currentUser + "\\Cookies\\",
                "C:\\Documents and Settings\\" + currentUser + "\\Recent\\",
                "C:\\Documents and Settings\\" + currentUser + "\\Local Settings\\Temp\\",
                "C:\\Documents and Settings\\" + currentUser + "\\Local Settings\\Temporary Internet Files\\",
                "C:\\Documents and Settings\\" + currentUser + "\\Local Settings\\History\\",
                "C:\\Documents and Settings\\Default User\\Cookies\\",
                "C:\\Documents and Settings\\Default User\\Recent\\",
                "C:\\Documents and Settings\\Default User\\Local Settings\\Temp\\",
                "C:\\Documents and Settings\\Default User\\Local Settings\\Temporary Internet Files\\",
                "C:\\Documents and Settings\\Default User\\Local Settings\\History\\"
            };
            foreach (var item in pakalentoList)
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(item);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        try
                        {
                            file.Delete();
                            log.Debug(file.ToString());
                        }
                        catch { }
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        try
                        {
                            dir.Delete(true);
                            log.Debug(dir.ToString());
                        }
                        catch { }
                    }
                }
                catch { }
            }
            log.Info($"Pakalento complete");
            log.Info($"{Keanu.GetTotalFreeSpace()} GB of free space");
            try { Pakkjfnjfn.CancelAsync(); } catch { }
        }

        private void BtnDuplicati_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("\\\\BALTIKPDC\\PUBLIC\\DB\\MARCUS\\");
        }

        private void BDownload_Click(object sender, RoutedEventArgs e)
        {
            Keanu.FaseDiSospesoPerEE253 = "DOWNLOAD";
            Kewieffnwensss();
        }

        private void BElimina_Click(object sender, RoutedEventArgs e)
        {
            Keanu.FaseDiSospesoPerEE253 = "ELIMINA";
            Kewieffnwensss();
        }

        private void BCarica_Click(object sender, RoutedEventArgs e)
        {
            Keanu.FaseDiSospesoPerEE253 = "CARICA";
            Kewieffnwensss();
        }
    }
}