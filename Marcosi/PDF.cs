using AutoIt;
using log4net;
using MARCUS.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace MARCUS.Marcosi {
    class PDF {
        private static readonly ILog log = LogManager.GetLogger(typeof(EE176));
        public Keanu Keanu { get; set; }

        public PDF(Keanu keanu) {
            this.Keanu = keanu;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        }

        public static bool PdfErrorChoice = false;//This is static so it can be accessed in ErrorOkCancel.xaml.cs

        public string enelML = @"https://dm2-ee.enelint.global/Document/default.aspx?DocID=";
        public string enelMV = @"https://crmsmart.enelint.global/Crm/DMSServlet?progressivo=0&idriferimento=";
        private readonly int minSize = 1666;
        public string detagglio = "";
        public string oldDettaglio = "";
        public bool notFound = false;
        public int restartos = 0;

        public bool Flow() {
            try {

                Keanu.Bad.Scarto = 0;
                Keanu.KillChromeWebDriver();
                Keanu.Driver = null;

                while (true) {
                    Keanu.Timy();
                    if(Keanu.Bad.Scarto >= 5) {
                        log.Error($"Troppi scarto - 5. Check why and restart marcus");
                        return false;
                    }
                    if(Keanu.StartStop == false) {
                        return false;
                    } else {
                        Workaholic();
                    }
                }

                /*
                switch (Keanu.LavName) {
                    case "PDF - ED15 - INS. DATI CATASTALI": {
                        while (true) {
                            Keanu.Timy();
                            if (Keanu.StartStop == false)
                                break;
                            else
                                Workaholic();
                        }
                        break;
                    }
                    case "PDF - ED15 - INS. DATI CATASTALI SENZA DATI": {
                        while (true) {
                            Keanu.Timy();
                            if (Keanu.StartStop == false)
                                break;
                            else
                                Workaholic();
                        }
                        break;
                    }
                    case "PDF - EE145 - RECUPERO DATI": {
                        while (true) {
                            Keanu.Timy();
                            if (Keanu.StartStop == false)
                                break;
                            else
                                Workaholic();
                        }
                        break;
                    }
                    case "PDF - EE62 - DimPag in SAP": {
                        while (true) {
                            Keanu.Timy();
                            if (Keanu.StartStop == false)
                                break;
                            else
                                Workaholic();
                        }
                        break;
                    }
                    case "PDF - EE253 - 147": {
                        while (true) {
                            Keanu.Timy();
                            if (Keanu.StartStop == false)
                                break;
                            else
                                Workaholic();
                        }
                        break;
                    }
                    case "PDF - EE253 - Cert. del clt": {
                        while (true) {
                            Keanu.Timy();
                            if (Keanu.StartStop == false)
                                break;
                            else
                                Workaholic();
                        }
                        break;
                    }
                    case "PDF - EE253 - Contr. Gest.": {
                        while (true) {
                            Keanu.Timy();
                            if (Keanu.StartStop == false)
                                break;
                            else
                                Workaholic();
                        }
                        break;
                    }

                }
                */
            } catch {
                log.Info($"Flow() fail");
            }
            return false;
        }
        
        public bool DownloadPDF(string pdf, string mercato) {
            try {
                string filePath = "";
                string sfapdfPath = "";
                string uri = "";


                filePath = @"\\172.23.0.91\applicazioni\EE145_Maschera_SFA\PDF\" + pdf + ".pdf";
                sfapdfPath = @"\\BALTIKPDC\sfapdf\" + pdf + ".pdf";

                

                if (Keanu.LavName == "PDF - EE62 - DimPag in SAP" || Keanu.LavName == "PDF - EE253 - 147" || Keanu.LavName == "PDF - EE253 - Cert. del clt" || Keanu.LavName == "PDF - EE253 - Contr. Gest." || Keanu.LavName == "PDF - EE253 - DMS-Contratti" || Keanu.LavName == "PDF - EE231 - PRESCRIZIONE") {
                    if (File.Exists(sfapdfPath)) {
                        log.Debug($"{pdf} already downloaded");
                        restartos = 0;
                        return true;
                    }
                }


                if (pdf.StartsWith("SDPL") || pdf.StartsWith("SDPM") || pdf.StartsWith("SMPL") || pdf.StartsWith("SDPM") || pdf.StartsWith("SWPL") || pdf.StartsWith("STPL") || pdf.StartsWith("STPM") || pdf.StartsWith("SCPL") || pdf.StartsWith("SCPM") || pdf.StartsWith("SFPL") || pdf.StartsWith("SFPM") || pdf.StartsWith("SMPM") || pdf.StartsWith("SCPM") || pdf.StartsWith("SWPM")) //Shitie kacha specialos SFPL dokus + jaipieliek pie 5in1IT/5inLV
                {

                    if (Keanu.LavName == "PDF - EE145 - RECUPERO DATI") {
                        if (File.Exists(filePath) && File.Exists(sfapdfPath)) {
                            log.Debug($"{pdf} already downloaded");
                            restartos = 0;
                            return true;
                        }
                    }


                    if (Keanu.Driver == null) {
                        restartos++;
                        if (!Keanu.PepperYourChromePDF("MI0000F", "Dominio$000001", "https://enelcrmt.my.salesforce.com/", "", true)) {
                            
                            return false;
                        }
                    }

                    string desktopPdf = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + pdf + ".pdf";
                    //string documentsPdf = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + pdf + ".pdf";

                    AutoItX.AutoItSetOption("WinTitleMatchMode", 2);

                    if (AutoItX.WinExists("Save As") == 1) {
                        AutoItX.WinActivate("Save As");
                        AutoItX.Send("{ESC}");
                        log.Error($"Save As window already opened");
                        Keanu.Erny();
                        if (PdfErrorChoice) {
                            log.Debug($"Skip {pdf}");
                            restartos = 0;
                            return true;
                        } else {
                            log.Debug($"Stop with {pdf}");
                            Keanu.StartStop = false;
                            restartos = 0;
                            return false;
                        }
                    }

                    if (AutoItX.WinExists("Confirm Save As") == 1) {
                        AutoItX.WinActivate("Confirm Save As");
                        AutoItX.Send("!n");
                        Thread.Sleep(Keanu.Randy(1));
                        AutoItX.WinActivate("Save As");
                        AutoItX.Send("{ESC}");
                        log.Error($"Confirm Save As window already opened");
                        Keanu.Erny();
                        if (PdfErrorChoice) {
                            log.Debug($"Skip {pdf}");
                            restartos = 0;
                            return true;
                        } else {
                            log.Debug($"Stop with {pdf}");
                            Keanu.StartStop = false;
                            restartos = 0;
                            return false;
                        }
                    }

                    Keanu.Driver.Navigate().GoToUrl(@"https://enelcrmt.lightning.force.com/c/ITA_IFM_LAP021_RetrieveDocuments.app?system=MARI&recordId=" + pdf);
                    Thread.Sleep(Keanu.Randy(2));
                    try {
                        int errorCounter = 0;
                        while (AutoItX.WinActive("Save As") == 0){
                            Thread.Sleep(Keanu.Randy(1));
                            errorCounter++;
                            if (Keanu.Driver.PageSource.ToString().Contains("Errore nel caricamento del file") || Keanu.Driver.PageSource.ToString().Contains("Errore : 500 Server Error")) {
                                if (restartos > 5)
                                {
                                    Keanu.Erny();
                                    if (PdfErrorChoice)
                                    {
                                        log.Debug($"Skip {pdf}");
                                        restartos = 0;
                                        return true;
                                    }
                                    else
                                    {
                                        log.Debug($"Stop with {pdf}");
                                        Keanu.StartStop = false;
                                        restartos = 0;
                                        return false;
                                    }
                                }
                                else
                                {
                                    log.Debug($"error I wil restart chrome");
                                    Keanu.KillChromeWebDriver();
                                    Keanu.TimeToRestart = true;
                                    Keanu.Driver = null;
                                    return false;
                                }
                            }

                            if (errorCounter > 15) {
                                {
                                    log.Debug($"error I wil restart chrome");
                                    Keanu.KillChromeWebDriver();
                                    Keanu.TimeToRestart = true;
                                    Keanu.Driver = null;
                                    return false;
                                }
                                /*Keanu.Erny();
                                if (PdfErrorChoice) {
                                    log.Debug($"Skip {pdf}");
                                    return true;
                                } else {
                                    log.Debug($"Stop with {pdf}");
                                    Keanu.StartStop = false;
                                    return false;
                                }*/
                            }

                            AutoItX.WinActivate("Save As");
                        }
                        Thread.Sleep(Keanu.Randy(1));
                        //AutoIt.AutoItX.GetCon
                        AutoIt.AutoItX.ControlSetText("Save As", "", "[CLASS:Edit;INSTANCE:1]", "");
                        //AutoIt.AutoItX.ControlSend("Save As", "", "[CLASS:Edit;INSTANCE:1]", desktopPdf);
                        Thread.Sleep(Keanu.Randy(1));

                        AutoIt.AutoItX.ControlSend("Save As", "", "[CLASS:Edit;INSTANCE:1]", desktopPdf);

                        Thread.Sleep(Keanu.Randy(1));
                        AutoItX.Send("{ENTER}");
                        Thread.Sleep(Keanu.Randy(1));
                        AutoItX.WinActivate("Save As");
                        Thread.Sleep(Keanu.Randy(1));
                        AutoItX.Send("!s");
                        Thread.Sleep(Keanu.Randy(1));
                        //AutoIt.AutoItX.ControlClick("Save As", "", "[CLASS:Button;INSTANCE:2]", "left", 1);
                        //AutoItX.Send("{ENTER}");
                        

                    } catch (Exception Ex) {
                        log.Error(Ex);
                        Keanu.Erny();
                        if (PdfErrorChoice) {
                            log.Debug($"Skip {pdf}");
                            restartos = 0;
                            return true;
                        } else {
                            log.Debug($"Stop with {pdf}");
                            Keanu.StartStop = false;
                            restartos = 0;
                            return false;
                        }
                    }

                    try {
                        if (Keanu.LavName.Equals("PDF - EE145 - RECUPERO DATI")) {
                            if (File.Exists(desktopPdf)) {
                                File.Copy(desktopPdf, filePath, true);
                                sfapdfPath = @"\\BALTIKPDC\sfapdf\" + pdf + ".pdf";
                                File.Copy(desktopPdf, sfapdfPath, true);
                                Thread.Sleep(Keanu.Randy(1));
                                File.Delete(desktopPdf);
                            } else {
                                log.Error($"{pdf} wrong path");
                            }
                        } else {
                            if (File.Exists(desktopPdf)) {
                                filePath = @"\\BALTIKPDC\sfapdf\" + pdf + ".pdf";
                                File.Copy(desktopPdf, filePath, true);
                                Thread.Sleep(Keanu.Randy(1));
                                File.Delete(desktopPdf);
                            } else {
                                log.Error($"{pdf} wrong path");
                            }

                        }

                    } catch (Exception Ex) {
                        log.Error(Ex);
                        Keanu.Erny();
                        if (PdfErrorChoice) {
                            log.Debug($"Skip {pdf}");
                            restartos = 0;
                            return true;
                        } else {
                            log.Debug($"Stop with {pdf}");
                            Keanu.StartStop = false;
                            restartos = 0;
                            return false;
                        }
                    }

                    long length = new FileInfo(filePath).Length;

                    if (length < minSize) {
                        File.Delete(filePath);
                        log.Error($"{pdf} size < 2KB");

                        Keanu.Erny();
                        if (PdfErrorChoice) {
                            log.Debug($"Skip {pdf}");
                            restartos = 0;
                            return true;
                        } else {
                            log.Debug($"Stop with {pdf}");
                            Keanu.StartStop = false;
                            restartos = 0;
                            return false;
                        }
                    }

                    log.Info($"{pdf} downloaded");
                    return true;
                } else {

                    if (Keanu.LavName == "PDF - EE145 - RECUPERO DATI") {
                        if (File.Exists(sfapdfPath)) {
                            log.Debug($"{pdf} already downloaded");
                            restartos = 0;
                            return true;
                        }
                    }

                    if (Keanu.LavName == "PDF - EE145 - RECUPERO DATI" || Keanu.LavName == "PDF - EE62 - DimPag in SAP" || Keanu.LavName == "PDF - EE253 - 147" || Keanu.LavName == "PDF - EE253 - Cert. del clt" || Keanu.LavName == "PDF - EE253 - Contr. Gest." || Keanu.LavName == "PDF - EE253 - DMS-Contratti" || Keanu.LavName == "PDF - EE231 - PRESCRIZIONE") {
                        if (pdf.Length > 30) {
                            uri = mercato + pdf;
                            filePath = @"\\BALTIKPDC\sfapdf\" + pdf + ".pdf";

                        } else {
                            uri = mercato + pdf.Substring(0, 12);
                            filePath = @"\\BALTIKPDC\sfapdf\" + pdf.Substring(0, 12) + ".pdf";
                        } 
                    }

                    if (Keanu.LavName == "PDF - ED15 - INS. DATI CATASTALI" || Keanu.LavName == "PDF - ED15 - INS. DATI CATASTALI SENZA DATI") {
                        if (pdf.Length > 30) {
                            uri = mercato + pdf;
                            filePath = @"\\BALTIKPDC\catasti_PDF\" + pdf + ".pdf";

                        } else {
                            uri = mercato + pdf.Substring(0, 12);
                            filePath = @"\\BALTIKPDC\catasti_PDF\" + pdf.Substring(0, 12) + ".pdf";
                        }
                    }

                    

                    Uri fileLink = new Uri(uri);
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(fileLink, filePath);

                    long length = new FileInfo(filePath).Length;

                    if (length < minSize) {
                        File.Delete(filePath);
                        log.Error($"{pdf} size < 2KB");

                        Keanu.Erny();
                        if (PdfErrorChoice) {
                            log.Debug($"Skip {pdf}");
                            restartos = 0;
                            return true;
                        } else {
                            log.Debug($"Stop with {pdf}");
                            Keanu.StartStop = false;
                            restartos = 0;
                            return false;
                        }
                    }

                    log.Info($"{pdf} downloaded");
                    restartos = 0;
                    return true;
                }
            } catch (Exception Ex) {
                log.Error($"{pdf} (404) Not Found.");
                //notFound = true;
                //return false;
                Keanu.Erny();
                if (PdfErrorChoice) {
                    log.Debug($"Skip {pdf}");
                    restartos = 0;
                    return true;
                } else {
                    log.Debug($"Stop with {pdf}");
                    Keanu.StartStop = false;
                    restartos = 0;
                    return false;
                }
            }
        }

        private bool Workaholic() {
            try {
                Thread.Sleep(Keanu.Randy(1));
                notFound = false;
                bool esito = true;

                DALAVORAREDataContext dalavorare = new DALAVORAREDataContext();
                FATJIKDataContext fatjik = new FATJIKDataContext();


                var idScarico = (from f
                                 in fatjik.MARCUSPs
                                 where f.ID_DETTAGLIO_TIPO_LAVORAZIONE == Keanu.PdfType
                                 orderby f.ID
                                 descending
                                 select f.ID_PROD_VOLUMI_DA_LAVORARE).Take(1).ToList();

                int idProd = idScarico[0].Value;

                List<PROD_VOLUMI_DA_LAVORARE> q = (from c
                                                   in dalavorare.PROD_VOLUMI_DA_LAVORAREs
                                                   where (c.ID_DETTAGLIO_TIPO_LAVORAZIONE == Keanu.PdfType) && c.ID > idProd && c.ID_STATO_SOSPENSIONE == 0
                                                   select c).Take(1).ToList();

                if (q.Count < 1) {
                    log.Debug("Waiting ~15min");
                    if (Keanu.LavName.Equals("PDF - EE145 - RECUPERO DATI") || Keanu.LavName.Equals("PDF - EE62 - DimPag in SAP") || Keanu.LavName.Equals("PDF - EE253 - 147") || Keanu.LavName.Equals("PDF - EE253 - Cert. del clt") || Keanu.LavName.Equals("PDF - EE253 - Contr. Gest.") || Keanu.LavName.Equals("PDF - EE253 - DMS-Contratti") || Keanu.LavName.Equals("PDF - EE231 - PRESCRIZIONE"))//KILL ONLY EE145 PDF
                    {
                        if (Keanu.Driver != null) {
                            try {
                                Keanu.Driver.Close();
                                Keanu.Driver = null;
                            } catch (Exception) {
                                log.Debug("Excepción");
                            }
                        }
                        Keanu.KillChromeWebDriver();
                    }
                    Thread.Sleep(Keanu.Randy(375));
                    restartos = 0;
                    return false;
                }

                detagglio = q[0].DETTAGLIO;
                string pdf = "";

                if (Keanu.LavName.Equals("PDF - EE145 - RECUPERO DATI") || Keanu.LavName.Equals("PDF - EE62 - DimPag in SAP") || Keanu.LavName.Equals("PDF - EE253 - 147") || Keanu.LavName.Equals("PDF - EE253 - Cert. del clt") || Keanu.LavName.Equals("PDF - EE253 - Contr. Gest.") || Keanu.LavName.Equals("PDF - EE253 - DMS-Contratti") || Keanu.LavName.Equals("PDF - EE231 - PRESCRIZIONE")) {
                    pdf = q[0].NUMERO_CLIENTE;
                    if(pdf == "") {
                        pdf = q[0].NOTE;
                    }
                } else {
                    pdf = q[0].DETTAGLIO;
                }
                    
                if(pdf.Length < 12) {

                    Keanu.Bad.Scarto++;
                    log.Error($"{pdf} length < 12 - skipping");

                    MARCUSP newRow = new MARCUSP {
                        ID_PROD_VOLUMI_DA_LAVORARE = q[0].ID,
                        ID_DETTAGLIO_TIPO_LAVORAZIONE = q[0].ID_DETTAGLIO_TIPO_LAVORAZIONE
                    };

                    fatjik.MARCUSPs.InsertOnSubmit(newRow);

                    try {
                        fatjik.SubmitChanges();
                    } catch {
                        try {
                            fatjik.GetChangeSet().Inserts.Clear();
                        } catch {
                            return false;
                        }
                    }

                    return true;
                }

                if (Keanu.LavName == "PDF - ED15 - INS. DATI CATASTALI" || Keanu.LavName == "PDF - ED15 - INS. DATI CATASTALI SENZA DATI") {
                    esito = DownloadPDF(pdf, enelMV);
                } else {
                    esito = DownloadPDF(pdf, enelML);
                }

                if (!esito) {
                    return false;
                }
                

                MARCUSP scarico = new MARCUSP {
                    ID_PROD_VOLUMI_DA_LAVORARE = q[0].ID,
                    ID_DETTAGLIO_TIPO_LAVORAZIONE = q[0].ID_DETTAGLIO_TIPO_LAVORAZIONE
                };

                fatjik.MARCUSPs.InsertOnSubmit(scarico);

                try {
                    fatjik.SubmitChanges();
                } catch {
                    try {
                        fatjik.GetChangeSet().Inserts.Clear();
                    } catch {
                        return false;
                    }
                }

                Keanu.Bad.Fatto++;
                return true;
            } catch(Exception exception) {
                log.Debug($"Connection failure, w8 1min & retry - {exception}");
                Thread.Sleep(Keanu.Randy(60));
                //Keanu.StartStop = false;
                restartos = 0;
                return false;
            }
        }
    }
}