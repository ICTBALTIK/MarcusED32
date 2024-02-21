using log4net;
using MARCUS.Helpers;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;

namespace MARCUS.Marcosi
{
    class EE366
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EE366));
        public Keanu Keanu { get; set; }
        public SqlQuery query = new SqlQuery();

        public EE366(Keanu keanu)
        {
            this.Keanu = keanu;
        }

        private readonly List<string> workName = new List<string>
        {
            "Disdetta con suggello",//OK
            "Prima Attivazione",
            "Voltura senza accollo",//OK
            "Voltura con accollo",//OK
            ////"Subentro",//DONT LAUNCH
            ////"Subentro 2",//DONT LAUNCH
            ////"Subentro con modifica contrattuale",//DONT LAUNCH
            "Modifica Potenza",//OK
            "Modifica Tensione"//OK
        };

        Records.DatiR2D datir2d;
        Records.DatiFOUR datifour;
        Records.DatiSII datisii;

        public bool Flow()
        {
            Keanu.KillChromeWebDriver();

            try
            {
                if (!Keanu.PepperYourChrome(Keanu.LoginR2D, Keanu.PassR2D, "http://r2dapps.awselb.enelint.global/r2d/ele/home.do", "Benvenuto su R2D", false))
                    return false;
                if (!Keanu.PepperYourSII(Keanu.LoginSII, Keanu.PassSII, "https://siiportale.acquirenteunico.it/group/acquirente-unico", "userID"))
                    return false;
                if (!Keanu.PepperYourFOUR(Keanu.LoginFOUR, Keanu.PassFOUR, "https://4pt.e-distribuzione.it/login", "Username"))
                    return false;

                foreach (var work in workName)
                {
                    if (!Workaholic(work))
                        break;
                }
            }
            catch
            {
                log.Info($"Flow() fail");
            }

            Keanu.KillChromeWebDriver();
            Keanu.Agente.Logout();
            return false;
        }

        private void Switchah(string driverName = "default")
        {
            int count = 3;
            try
            {
                switch (driverName)
                {
                    case "default":
                        for (int i = 0; i < count; i++)
                        {
                            Keanu.Driver.SwitchTo().Window(Keanu.Driver.WindowHandles.LastOrDefault());
                            Thread.Sleep(Keanu.Randy(1));
                        }
                        break;
                    case "sii":
                        for (int i = 0; i < count; i++)
                        {
                            Keanu.DriverSII.SwitchTo().Window(Keanu.DriverSII.WindowHandles.LastOrDefault());
                            Thread.Sleep(Keanu.Randy(1));
                        }
                        break;
                    case "four":
                        for (int i = 0; i < count; i++)
                        {
                            Keanu.DriverFOUR.SwitchTo().Window(Keanu.DriverFOUR.WindowHandles.LastOrDefault());
                            Thread.Sleep(Keanu.Randy(1));
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

            }
        }

        private bool Workaholic(string work)
        {
            try
            {
                bool thereAreDocuments = true;
                while (thereAreDocuments)
                {

                    #region ANTI-SCADUTA
                    Keanu.DriverSII.Navigate().Refresh();
                    Keanu.DriverFOUR.Navigate().GoToUrl("https://4pt.e-distribuzione.it/s/PT-F5Ric");
                    Keanu.Driver.Navigate().GoToUrl("http://r2dapps.awselb.enelint.global/r2d/ele/home.do");
                    #endregion

                    Switchah();

                    if (!SelectWork(work))
                        break;

                    datir2d = new Records.DatiR2D();
                    datifour = new Records.DatiFOUR();
                    datisii = new Records.DatiSII();

                    string error = "";
                    bool worky = false;

                    switch (work)
                    {
                        case "Disdetta con suggello":
                            worky = DisdettaConSuggello(work, out error, out thereAreDocuments);
                            if (worky && thereAreDocuments)
                                Fatto();
                            else if (!worky && thereAreDocuments)
                                Scarto(error);
                            break;
                        case "Prima Attivazione":
                            worky = PrimaAttivazione(work, out error, out thereAreDocuments);
                            if (worky && thereAreDocuments)
                                Fatto();
                            else if (!worky && thereAreDocuments)
                                Scarto(error);
                            break;
                        case "Voltura senza accollo":
                            worky = VolturaSenzaAccollo(work, out error, out thereAreDocuments);
                            if (worky && thereAreDocuments)
                                Fatto();
                            else if (!worky && thereAreDocuments)
                                Scarto(error);
                            break;
                        case "Voltura con accollo":
                            worky = VolturaConAccollo(work, out error, out thereAreDocuments);
                            if (worky && thereAreDocuments)
                                Fatto();
                            else if (!worky && thereAreDocuments)
                                Scarto(error);
                            break;
                        case "Subentro":

                            break;
                        case "Subentro 2":

                            break;
                        case "Subentro con modifica contrattuale":

                            break;
                        case "Modifica Potenza":
                            worky = ModificaPotenza(work, out error, out thereAreDocuments);
                            if (worky && thereAreDocuments)
                                Fatto();
                            else if (!worky && thereAreDocuments)
                                Scarto(error);
                            break;
                        case "Modifica Tensione":
                            worky = ModificaTensione(work, out error, out thereAreDocuments);
                            if (worky && thereAreDocuments)
                                Fatto();
                            else if (!worky && thereAreDocuments)
                                Scarto(error);
                            break;
                        default:
                            break;
                    }
                    if (Keanu.StartStop == false)
                        return false;
                }
            }
            catch
            {
                return false;
            }

            log.Warn($"{work} finished");
            return true;
        }

        private bool DisdettaConSuggello(string work, out string error, out bool thereAreDocuments)
        {
            if (!R2D(work, out error, out thereAreDocuments))
                return false;

            #region SII
            Switchah("sii");
            try
            {
                Keanu.DriverSII.Navigate().GoToUrl(@"https://siiportale.acquirenteunico.it/group/acquirente-unico/pod-puntuale");
                Keanu.WaitingGame();

                var cmbRuolo = Keanu.DriverSII.FindElement(By.Id("ruolo"));
                cmbRuolo.SendKeys("Utente del dispacciamento EE");

                var txtPod = Keanu.DriverSII.FindElement(By.Id("id-pod"));
                txtPod.SendKeys(datir2d.Pod);

                var rbtnAnalisiPerPeriodo = Keanu.DriverSII.FindElement(By.Id("id-analisi-per-periodo"));
                rbtnAnalisiPerPeriodo.Click();

                var txtDataInizio = Keanu.DriverSII.FindElement(By.Id("id-data-inizio"));
                txtDataInizio.SendKeys(DateTime.Now.AddYears(-1).AddDays(1).ToShortDateString());
                Thread.Sleep(Keanu.Randy(1));

                var txtDataFine = Keanu.DriverSII.FindElement(By.Id("id-data-fine"));
                txtDataFine.SendKeys(DateTime.Now.ToShortDateString());
                Thread.Sleep(Keanu.Randy(1));

                var btnContinue = Keanu.DriverSII.FindElement(By.Id("continua-btn"));
                btnContinue.Click();
                Keanu.WaitingGame();

                IWebElement getFornitureAssociate = null;
                try
                {
                    getFornitureAssociate = Keanu.DriverSII.FindElement(By.XPath("//*[. = 'Forniture Associate']"));
                }
                catch
                {
                    error = $"PDR non attivo in SII";
                    log.Info(error);
                    return false;
                }

                var getTable = getFornitureAssociate.FindElement(By.XPath("..//table[@class='dt_table']"));
                var getRows = getTable.FindElements(By.XPath("//tr[@class='odd']"));
                var necessaryRowCF = getRows.Where(a => a.FindElements(By.XPath("./*")).ElementAt(0).Text.Trim().Equals(datir2d.CF)).ToList();
                var necessaryRow = necessaryRowCF.Where(b => b.FindElements(By.XPath("./*")).ElementAt(1).Text.Trim().Replace(" ", "").Equals(datir2d.PIVA)).ToList();

                Dictionary<string, DateTime> dic = new Dictionary<string, DateTime>();
                int cnt = 1;
                foreach (var i in necessaryRow)
                {
                    var date = i.FindElements(By.XPath("./*")).ElementAt(4).Text;
                    if (string.IsNullOrEmpty(date))
                    {
                        error = $"Data Fine Fornitura sono vuota";
                        log.Info(error);
                        return false;
                    }
                    dic.Add(cnt.ToString(), DateTime.Parse(date));
                    cnt++;
                }
                var list = dic.ToList();
                if (list.Count.Equals(0) || !list.Count.Equals(1))
                {
                    error = $"SII non esiste CF cliente finale";
                    log.Info(error);
                    return false;
                }
                var listSort = (from i in list orderby i.Value descending select i).ToList();
                datisii.DataFine = listSort[0].Value.ToShortDateString();
            }
            catch
            {
                log.Info($"{work} SII fail");
                return false;
            }
            #endregion

            if (!R2D2(work, out error))
                return false;

            return true;
        }

        private bool PrimaAttivazione(string work, out string error, out bool thereAreDocuments)
        {
            if (!R2D(work, out error, out thereAreDocuments))
                return false;

            #region SII PRIMA ATTIVAZIONE
            Switchah("sii");
            try
            {
                do
                {
                    Keanu.DriverSII.Navigate().GoToUrl(@"https://siiportale.acquirenteunico.it/group/acquirente-unico/pod-puntuale");
                    Keanu.WaitingGame();
                } while (!Keanu.DriverSII.PageSource.ToString().Contains("Interrogazione puntuale POD"));

                var cmbRuolo = Keanu.DriverSII.FindElement(By.Id("ruolo"));
                cmbRuolo.SendKeys("Utente del dispacciamento EE");

                var txtPod = Keanu.DriverSII.FindElement(By.Id("id-pod"));
                txtPod.SendKeys(datir2d.Pod);

                var rbtnAnalisiPerPeriodo = Keanu.DriverSII.FindElement(By.Id("id-analisi-per-periodo"));
                rbtnAnalisiPerPeriodo.Click();

                var txtDataInizio = Keanu.DriverSII.FindElement(By.Id("id-data-inizio"));
                txtDataInizio.SendKeys(DateTime.Now.AddYears(-1).AddDays(1).ToShortDateString());
                Thread.Sleep(Keanu.Randy(1));

                var txtDataFine = Keanu.DriverSII.FindElement(By.Id("id-data-fine"));
                txtDataFine.SendKeys(DateTime.Now.ToShortDateString());
                Thread.Sleep(Keanu.Randy(1));

                var btnContinue = Keanu.DriverSII.FindElement(By.Id("continua-btn"));
                btnContinue.Click();
                Keanu.WaitingGame();

                IWebElement getFornitureAssociate = null;
                try
                {
                    getFornitureAssociate = Keanu.DriverSII.FindElement(By.XPath("//*[. = 'Forniture Associate']"));
                }
                catch
                {
                    error = $"PDR non attivo in SII";
                    log.Info(error);
                    return false;
                }

                var getTable = getFornitureAssociate.FindElement(By.XPath("..//table[@class='dt_table']"));
                var getRows = getTable.FindElements(By.XPath("//tr[@class='odd']"));
                if (getRows.Count() > 2)
                {
                    error = $"SII > 2 righe";
                    log.Info(error);
                    return false;
                }
                var getDettaglio = getRows[1].FindElements(By.XPath(".//*[@type = 'button']"));
                getDettaglio[0].Click();
                Keanu.WaitingGame();

                var getTabFornitura = Keanu.DriverSII.FindElement(By.Id("div-fornitura"));

                var getTableTariffa = getTabFornitura.FindElement(By.XPath(".//table[@id='tariffa']"));
                var getTrFromtableTariffatr = getTableTariffa.FindElements(By.XPath(".//tr")).Last();
                datisii.TariffaDistributore = getTrFromtableTariffatr.FindElement(By.XPath(".//td[1]")).Text;
                datisii.DataInizio = getTrFromtableTariffatr.FindElement(By.XPath(".//td[2]")).Text;
            }
            catch
            {
                log.Info($"{work} SII GET fail");
                return false;
            }
            #endregion

            if (!R2D2(work, out error))
                return false;

            return true;
        }

        private bool VolturaSenzaAccollo(string work, out string error, out bool thereAreDocuments)
        {
            if (!R2D(work, out error, out thereAreDocuments))
                return false;

            #region SII GET
            Switchah("sii");
            try
            {
                do
                {
                    Keanu.DriverSII.Navigate().GoToUrl(@"https://siiportale.acquirenteunico.it/group/acquirente-unico/pod-puntuale");
                    Keanu.WaitingGame();
                } while (!Keanu.DriverSII.PageSource.ToString().Contains("Interrogazione puntuale POD"));

                var cmbRuolo = Keanu.DriverSII.FindElement(By.Id("ruolo"));
                cmbRuolo.SendKeys("Utente del dispacciamento EE");

                var txtPod = Keanu.DriverSII.FindElement(By.Id("id-pod"));
                txtPod.SendKeys(datir2d.Pod);

                var rbtnAnalisiPerPeriodo = Keanu.DriverSII.FindElement(By.Id("id-analisi-per-periodo"));
                rbtnAnalisiPerPeriodo.Click();

                var txtDataInizio = Keanu.DriverSII.FindElement(By.Id("id-data-inizio"));
                txtDataInizio.SendKeys(DateTime.Now.AddYears(-1).AddDays(1).ToShortDateString());
                Thread.Sleep(Keanu.Randy(1));

                var txtDataFine = Keanu.DriverSII.FindElement(By.Id("id-data-fine"));
                txtDataFine.SendKeys(DateTime.Now.ToShortDateString());
                Thread.Sleep(Keanu.Randy(1));

                var btnContinue = Keanu.DriverSII.FindElement(By.Id("continua-btn"));
                btnContinue.Click();
                Keanu.WaitingGame();

                IWebElement getFornitureAssociate = null;
                try
                {
                    getFornitureAssociate = Keanu.DriverSII.FindElement(By.XPath("//*[. = 'Forniture Associate']"));
                }
                catch
                {
                    error = $"PDR non attivo in SII";
                    log.Info(error);
                    return false;
                }

                var getTable = getFornitureAssociate.FindElement(By.XPath("..//table[@class='dt_table']"));
                var getRows = getTable.FindElements(By.XPath("//tr[@class='odd']"));
                var necessaryRow = getRows.Where(a => a.Text.Contains(datir2d.NuovoIntestatarioCF)).ToList();
                Dictionary<string, DateTime> dic = new Dictionary<string, DateTime>();
                int count = 1;
                foreach (var i in necessaryRow)
                {
                    var date = i.FindElements(By.XPath("./*")).ElementAt(3).Text;
                    if (string.IsNullOrEmpty(date))
                    {
                        error = $"Data Inizio Fornitura sono vuota";
                        log.Info(error);
                        return false;
                    }
                    dic.Add(count.ToString(), DateTime.Parse(date));
                    count++;
                }
                var list = dic.ToList();
                if (list.Count.Equals(0))
                {
                    error = $"SII non esiste CF cliente intestario";
                    log.Info(error);
                    return false;
                }
                var listSort = (from i in list orderby i.Value descending select i).ToList();
                datisii.DataInizio = listSort[0].Value.ToShortDateString();
            }
            catch
            {
                log.Info($"{work} SII GET fail");
                return false;
            }
            #endregion

            #region SII SET
            Switchah("sii");
            try
            {
                do
                {
                    Keanu.DriverSII.Navigate().GoToUrl(@"https://siiportale.acquirenteunico.it/group/acquirente-unico/ricerca-standard");
                    Keanu.WaitingGame();
                } while (!Keanu.DriverSII.PageSource.ToString().Contains("Archivio Pratiche"));

                var txtPod = Keanu.DriverSII.FindElement(By.Id("pod"));
                txtPod.Clear();
                txtPod.SendKeys(datir2d.Pod);

                var btnProsegui = Keanu.DriverSII.FindElement(By.XPath("//input[@value='Prosegui']"));
                btnProsegui.Click();
                Keanu.WaitingGame();

                var getTable = Keanu.DriverSII.FindElement(By.XPath("//table[@class='gs_table gs_table_pratiche']"));
                var getRows = getTable.FindElements(By.TagName("tr"));
                var necessaryRow = getRows.Where(a => a.Text.Contains("Voltura") && a.Text.Contains("SII")).ToList();

                Dictionary<string, DateTime> dic = new Dictionary<string, DateTime>();
                int count = 1;
                foreach (var i in necessaryRow)
                {
                    var date = i.FindElements(By.XPath("./*")).ElementAt(4).Text;
                    var protocollo = (i.FindElements(By.XPath("./*")).ElementAt(0)).Text;
                    if (!string.IsNullOrEmpty(date))
                    {
                        dic.Add(count.ToString(), DateTime.Parse(date));
                        count++;
                    }
                }
                var list = dic.ToList();
                if (list.Count.Equals(0))
                {
                    error = $"SII non esiste protocollo/codice processo";
                    log.Info(error);
                    return false;
                }
                var listSort = (from i in list orderby i.Value descending select i).ToList();
                datisii.DataChiusura = listSort[0].Value.ToShortDateString();

                var evenNecessarierRow = necessaryRow.Where(a => a.Text.Contains(datisii.DataChiusura)).ToList();
                var getDettaglio = evenNecessarierRow[0].FindElements(By.XPath(".//input[@value='Dettaglio']"));
                getDettaglio[0].Click();
                Keanu.WaitingGame();

                var ProtocolloTD = Keanu.DriverSII.FindElement(By.XPath("//*[@id='testata']/table/tbody/tr[5]/td[2]")).Text.Trim();
                if (!ProtocolloTD.Contains("SII"))
                {
                    error = $"SII non esiste protocollo";
                    log.Info(error);
                    return false;
                }
                datisii.Protocollo = ProtocolloTD;

                var dettaglioTab = Keanu.DriverSII.FindElement(By.XPath("//*[@id='gs_container']/div/ul/li[2]/a"));
                dettaglioTab.Click();
                Keanu.WaitingGame();

                bool esito = false;
                try
                {
                    var allHeaders = Keanu.DriverSII.FindElements(By.TagName("div"));
                    for (int td = 0; allHeaders.Count() > 0; td++)
                    {
                        if (esito)
                            break;
                        switch (allHeaders[td].Text.ToString().Trim())
                        {
                            case "Codice Fiscale":
                                {
                                    datisii.CF = allHeaders[td + 1].Text.ToString().Trim();
                                    break;
                                }
                            case "PIVA":
                                {
                                    datisii.PIVA = allHeaders[td + 1].Text.ToString().Trim();
                                    break;
                                }
                            case "Data Dec. Voltura":
                                {
                                    datisii.DataDecVoltura = allHeaders[td + 1].Text.ToString().Trim();
                                    break;
                                }
                            case "Data Esito":
                                {
                                    datisii.DataEsito = allHeaders[td + 1].Text.ToString().Trim();
                                    esito = true;
                                    break;
                                }
                        }
                    }
                }
                catch (Exception Ex)
                {
                    log.Info($"Cannot get data");
                    return false;
                }

                if (datir2d.NuovoIntestatarioCF == datisii.CF) { }
                else if (!string.IsNullOrEmpty(datisii.PIVA) && datir2d.NuovoIntestatarioPIVA == datisii.PIVA) { }
                else
                {
                    error = $"CF in SII != CF in R2D";
                    log.Info(error);
                    return false;
                }

            }
            catch
            {
                log.Info($"{work} SII SET fail");
                return false;
            }
            #endregion

            if (!R2D2(work, out error))
                return false;

            return true;
        }

        private bool VolturaConAccollo(string work, out string error, out bool thereAreDocuments)
        {
            if (!R2D(work, out error, out thereAreDocuments))
                return false;

            #region SII GET
            Switchah("sii");
            try
            {
                do
                {
                    Keanu.DriverSII.Navigate().GoToUrl(@"https://siiportale.acquirenteunico.it/group/acquirente-unico/pod-puntuale");
                    Keanu.WaitingGame();
                } while (!Keanu.DriverSII.PageSource.ToString().Contains("Interrogazione puntuale POD"));

                var cmbRuolo = Keanu.DriverSII.FindElement(By.Id("ruolo"));
                cmbRuolo.SendKeys("Utente del dispacciamento EE");

                var txtPod = Keanu.DriverSII.FindElement(By.Id("id-pod"));
                txtPod.SendKeys(datir2d.Pod);

                var rbtnAnalisiPerPeriodo = Keanu.DriverSII.FindElement(By.Id("id-analisi-per-periodo"));
                rbtnAnalisiPerPeriodo.Click();

                var txtDataInizio = Keanu.DriverSII.FindElement(By.Id("id-data-inizio"));
                txtDataInizio.SendKeys(DateTime.Now.AddYears(-1).AddDays(1).ToShortDateString());
                Thread.Sleep(Keanu.Randy(1));

                var txtDataFine = Keanu.DriverSII.FindElement(By.Id("id-data-fine"));
                txtDataFine.SendKeys(DateTime.Now.ToShortDateString());
                Thread.Sleep(Keanu.Randy(1));

                var btnContinue = Keanu.DriverSII.FindElement(By.Id("continua-btn"));
                btnContinue.Click();
                Keanu.WaitingGame();

                IWebElement getFornitureAssociate = null;
                try
                {
                    getFornitureAssociate = Keanu.DriverSII.FindElement(By.XPath("//*[. = 'Forniture Associate']"));
                }
                catch
                {
                    error = $"PDR non attivo in SII";
                    log.Info(error);
                    return false;
                }

                var getTable = getFornitureAssociate.FindElement(By.XPath("..//table[@class='dt_table']"));
                var getRows = getTable.FindElements(By.XPath("//tr[@class='odd']"));
                var necessaryRow = getRows.Where(a => a.Text.Contains(datir2d.NuovoIntestatarioCF)).ToList();
                Dictionary<string, DateTime> dic = new Dictionary<string, DateTime>();
                int count = 1;
                foreach (var i in necessaryRow)
                {
                    var date = i.FindElements(By.XPath("./*")).ElementAt(3).Text;
                    if (string.IsNullOrEmpty(date))
                    {
                        error = $"Data Inizio Fornitura sono vuota";
                        log.Info(error);
                        return false;
                    }
                    dic.Add(count.ToString(), DateTime.Parse(date));
                    count++;
                }
                var list = dic.ToList();
                if (list.Count.Equals(0))
                {
                    error = $"SII non esiste CF cliente intestario";
                    log.Info(error);
                    return false;
                }
                var listSort = (from i in list orderby i.Value descending select i).ToList();
                datisii.DataInizio = listSort[0].Value.ToShortDateString();
            }
            catch
            {
                log.Info($"{work} SII GET fail");
                return false;
            }
            #endregion

            #region SII SET
            Switchah("sii");
            try
            {
                do
                {
                    Keanu.DriverSII.Navigate().GoToUrl(@"https://siiportale.acquirenteunico.it/group/acquirente-unico/ricerca-standard");
                    Keanu.WaitingGame();
                } while (!Keanu.DriverSII.PageSource.ToString().Contains("Archivio Pratiche"));

                var txtPod = Keanu.DriverSII.FindElement(By.Id("pod"));
                txtPod.Clear();
                txtPod.SendKeys(datir2d.Pod);

                var btnProsegui = Keanu.DriverSII.FindElement(By.XPath("//input[@value='Prosegui']"));
                btnProsegui.Click();
                Keanu.WaitingGame();

                var getTable = Keanu.DriverSII.FindElement(By.XPath("//table[@class='gs_table gs_table_pratiche']"));
                var getRows = getTable.FindElements(By.TagName("tr"));
                var necessaryRow = getRows.Where(a => a.Text.Contains("Voltura") && a.Text.Contains("SII")).ToList();

                Dictionary<string, DateTime> dic = new Dictionary<string, DateTime>();
                int count = 1;
                foreach (var i in necessaryRow)
                {
                    var date = i.FindElements(By.XPath("./*")).ElementAt(4).Text;
                    var protocollo = (i.FindElements(By.XPath("./*")).ElementAt(0)).Text;
                    if (!string.IsNullOrEmpty(date))
                    {
                        dic.Add(count.ToString(), DateTime.Parse(date));
                        count++;
                    }
                }
                var list = dic.ToList();
                if (list.Count.Equals(0))
                {
                    error = $"SII non esiste protocollo/codice processo";
                    log.Info(error);
                    return false;
                }
                var listSort = (from i in list orderby i.Value descending select i).ToList();
                datisii.DataChiusura = listSort[0].Value.ToShortDateString();

                var evenNecessarierRow = necessaryRow.Where(a => a.Text.Contains(datisii.DataChiusura)).ToList();
                var getDettaglio = evenNecessarierRow[0].FindElements(By.XPath(".//input[@value='Dettaglio']"));
                getDettaglio[0].Click();
                Keanu.WaitingGame();

                var ProtocolloTD = Keanu.DriverSII.FindElement(By.XPath("//*[@id='testata']/table/tbody/tr[5]/td[2]")).Text.Trim();
                if (!ProtocolloTD.Contains("SII"))
                {
                    error = $"SII non esiste protocollo";
                    log.Info(error);
                    return false;
                }
                datisii.Protocollo = ProtocolloTD;

                var dettaglioTab = Keanu.DriverSII.FindElement(By.XPath("//*[@id='gs_container']/div/ul/li[2]/a"));
                dettaglioTab.Click();
                Keanu.WaitingGame();

                bool esito = false;
                try
                {
                    var allHeaders = Keanu.DriverSII.FindElements(By.TagName("div"));
                    for (int td = 0; allHeaders.Count() > 0; td++)
                    {
                        if (esito)
                            break;
                        switch (allHeaders[td].Text.ToString().Trim())
                        {
                            case "Codice Fiscale":
                                {
                                    datisii.CF = allHeaders[td + 1].Text.ToString().Trim();
                                    break;
                                }
                            case "PIVA":
                                {
                                    datisii.PIVA = allHeaders[td + 1].Text.ToString().Trim();
                                    break;
                                }
                            case "Data Dec. Voltura":
                                {
                                    datisii.DataDecVoltura = allHeaders[td + 1].Text.ToString().Trim();
                                    break;
                                }
                            case "Data Esito":
                                {
                                    datisii.DataEsito = allHeaders[td + 1].Text.ToString().Trim();
                                    esito = true;
                                    break;
                                }
                        }
                    }
                }
                catch
                {
                    log.Info($"Cannot get data");
                    return false;
                }

                if (datir2d.NuovoIntestatarioCF == datisii.CF) { }
                else if (!string.IsNullOrEmpty(datisii.PIVA) && datir2d.NuovoIntestatarioPIVA == datisii.PIVA) { }
                else
                {
                    error = $"CF in SII != CF in R2D";
                    log.Info(error);
                    return false;
                }

            }
            catch
            {
                log.Info($"{work} SII SET fail");
                return false;
            }
            #endregion

            if (!R2D2(work, out error))
                return false;

            return true;
        }

        private bool ModificaPotenza(string work, out string error, out bool thereAreDocuments)
        {
            if (!R2D(work, out error, out thereAreDocuments))
                return false;

            if (!FOUR(work, out error))
                return false;

            if (!R2D2(work, out error))
                return false;

            return true;
        }

        private bool ModificaTensione(string work, out string error, out bool thereAreDocuments)
        {
            if (!R2D(work, out error, out thereAreDocuments))
                return false;

            if (!FOUR(work, out error))
                return false;

            if (!R2D2(work, out error))
                return false;

            return true;
        }

        private bool FOUR(string work, out string error)
        {
            Switchah("four");
            error = "";
            try
            {
                do
                {
                    Keanu.DriverFOUR.Navigate().GoToUrl(@"https://4pt.e-distribuzione.it/s/PT-F5Ric");
                    Keanu.WaitingGame();
                } while (!Keanu.DriverFOUR.PageSource.ToString().Contains("F05 Ricerca Richieste"));

                var txtPod = Keanu.DriverFOUR.FindElement(By.XPath("//span[. = 'POD']/../..//input"));
                txtPod.Clear();
                txtPod.SendKeys(datir2d.Pod);
                Thread.Sleep(Keanu.Randy(1));

                var btnCerca = Keanu.DriverFOUR.FindElements(By.XPath("//button[@class = 'slds-button CustomButton']"));
                ((IJavaScriptExecutor)Keanu.DriverFOUR).ExecuteScript("arguments[0].scrollIntoView(false);", btnCerca[0]);
                btnCerca[0].Click();

                while (!Keanu.DriverFOUR.PageSource.ToString().Contains("Dati cercati:"))
                    Thread.Sleep(Keanu.Randy(1));

                var table = Keanu.DriverFOUR.FindElement(By.XPath("//table[@class = 'slds-table slds-table_bordered slds-table_cell-buffer ']"));

                int indexIdProtocollo = 0;
                int indexTipologiaRichiesta = 0;
                int indexStato = 0;
                int indexSottoStato = 0;

                var lstHeader = table.FindElements(By.XPath("./thead/tr/th"));
                indexIdProtocollo = lstHeader.IndexOf(lstHeader.Where(q => q.Text.Contains("ID PROTOCOLLO")).First());
                indexTipologiaRichiesta = lstHeader.IndexOf(lstHeader.Where(q => q.Text.Contains("Tipologia Richiesta")).First());
                indexStato = lstHeader.IndexOf(lstHeader.Where(q => q.Text.Contains("Stato")).First());
                indexSottoStato = lstHeader.IndexOf(lstHeader.Where(q => q.Text.Contains("Sottostato")).First());

                IList<IWebElement> listaTr = table.FindElements(By.XPath("./tbody/tr"));
                int numeroRigheTabella = listaTr.Count();
                bool flag = false;

                for (int i = 1; i <= numeroRigheTabella; i++)
                {
                    var valore = (table.FindElement(By.XPath("./tbody/tr[" + i + "]"))).FindElements(By.XPath("./*")).ElementAt(indexTipologiaRichiesta);
                    string valoreCausaleContatto = valore.Text;
                    var stato = (table.FindElement(By.XPath("./tbody/tr[" + i + "]"))).FindElements(By.XPath("./*")).ElementAt(indexStato);
                    string valoreStatoo = stato.Text;
                    var sottostato = (table.FindElement(By.XPath("./tbody/tr[" + i + "]"))).FindElements(By.XPath("./*")).ElementAt(indexSottoStato);
                    string valoreSottoStato = sottostato.Text;

                    if (valoreCausaleContatto.ToUpper().StartsWith("MC1 - ") && valoreStatoo.ToUpper().Equals("EVASA_") && valoreSottoStato.ToUpper().Equals("EVASA_"))
                    {
                        //var valoreIdProtocollo = (table.FindElement(By.XPath("./tbody/tr[" + i + "]"))).FindElements(By.XPath("./*")).ElementAt(indexIdProtocollo);
                        var valoreIdProtocollo = table.FindElement(By.XPath("./tbody/tr[" + i + "]/td/a"));
                        MoveToObject(valoreIdProtocollo);
                        Thread.Sleep(Keanu.Randy(1));
                        valoreIdProtocollo.Click();
                        Keanu.WaitingGame();
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                {
                    error = $"FOUR senza MC1 & EVASA";
                    log.Info(error);
                    return false;
                }


                while (!Keanu.DriverFOUR.PageSource.ToString().Contains("Dati Identificativi della Fornitura"))
                    Thread.Sleep(Keanu.Randy(3));

                Thread.Sleep(Keanu.Randy(3));

                try
                {
                    var notifiche = Keanu.DriverFOUR.FindElement(By.XPath("//span[. = 'Notifiche']"));
                    notifiche.Click();
                }
                catch
                {
                    Thread.Sleep(Keanu.Randy(10));
                    var notifiche = Keanu.DriverFOUR.FindElement(By.XPath("//span[. = 'Notifiche']"));
                    notifiche.Click();
                }

                while (!Keanu.DriverFOUR.PageSource.ToString().Contains("dettaglio csv"))
                    Thread.Sleep(Keanu.Randy(1));

                var notificheTable = Keanu.DriverFOUR.FindElement(By.XPath("//table[@class= 'slds-table slds-table_bordered slds-max-medium-table_stacked-horizontal']"));
                {
                    int indexIdNotificaFour = 0;
                    int indexCodiceFlusso = 0;
                    int indexData = 0;
                    int indexCSV = 0;

                    var lstHeaderNotifiche = notificheTable.FindElements(By.XPath("./thead/tr/th"));
                    indexIdNotificaFour = lstHeaderNotifiche.IndexOf(lstHeaderNotifiche.Where(q => q.Text.Contains("Id Notifica Four")).First());
                    indexCodiceFlusso = lstHeaderNotifiche.IndexOf(lstHeaderNotifiche.Where(q => q.Text.Contains("Codice Flusso")).First());
                    indexData = lstHeaderNotifiche.IndexOf(lstHeaderNotifiche.Where(q => q.Text.Contains("Data/Ora Notifica")).First());
                    indexCSV = lstHeaderNotifiche.IndexOf(lstHeaderNotifiche.Where(q => q.Text.Contains("dettaglio csv")).First());

                    Thread.Sleep(Keanu.Randy(1));

                    IList<IWebElement> listaTrNotifiche = notificheTable.FindElements(By.XPath("./tbody/tr"));
                    int numeroRigheTabellaNotifiche = listaTrNotifiche.Count();
                    for (int i = 1; i <= numeroRigheTabellaNotifiche; i++)
                    {
                        var codiceFlusso = notificheTable.FindElement(By.XPath("./tbody/tr[" + i + "]")).FindElements(By.XPath("./*")).ElementAt(indexCodiceFlusso);
                        if (!codiceFlusso.Text.Equals("0150"))
                            continue;

                        datifour.NotificaFour = notificheTable.FindElement(By.XPath("./tbody/tr[" + i + "]")).FindElements(By.XPath("./*")).ElementAt(indexIdNotificaFour).Text;
                        var elemData = notificheTable.FindElement(By.XPath("./tbody/tr[" + i + "]")).FindElements(By.XPath("./*")).ElementAt(indexData);
                        datifour.DataOraNotifica = elemData.Text;

                        var gerg = elemData.FindElement(By.XPath(".."));
                        var greg = gerg.FindElements(By.XPath(".//td"));
                        greg[5].Click();
                        Thread.Sleep(Keanu.Randy(3));
                        break;
                    }

                    int iEsito = -1;
                    int iCOD_PRAT_DISTR = -1;
                    int iDATA_ESECUZIONE = -1;
                    int iPOT_DISP = -1;
                    int iTENSIONE = -1;

                    try
                    {
                        using (var reader = new StreamReader($@"C:\\Downloads\{datifour.NotificaFour}.csv"))
                        {
                            var row1 = reader.ReadLine();
                            var row1Values = row1.Split(';').ToList();

                            var row2 = reader.ReadLine();
                            var row2Values = row2.Split(';').ToList();

                            iEsito = row1Values.FindIndex(x => x.ToUpper().Equals("ESITO"));
                            iCOD_PRAT_DISTR = row1Values.FindIndex(x => x.ToUpper().Equals("COD_PRAT_DISTR"));
                            iDATA_ESECUZIONE = row1Values.FindIndex(x => x.ToUpper().Equals("DATA_ESECUZIONE"));
                            iPOT_DISP = row1Values.FindIndex(x => x.ToUpper().Equals("POT_DISP"));
                            iTENSIONE = row1Values.FindIndex(x => x.ToUpper().Equals("TENSIONE"));
                            datifour.ESITO = row2Values[iEsito];
                            datifour.COD_PRAT_DISTR = row2Values[iCOD_PRAT_DISTR];
                            datifour.DATA_ESECUZIONE = row2Values[iDATA_ESECUZIONE];
                            datifour.POT_DISP = row2Values[iPOT_DISP];
                            datifour.TENSIONE = row2Values[iTENSIONE];
                            reader.Dispose();
                        }
                    }
                    catch
                    {
                        log.Info($"Can't read file");
                        return false;
                    }

                    if (!datifour.ESITO.Equals("1"))
                    {
                        error = $"ESITO != 1";
                        log.Info(error);
                        return false;
                    }
                }
            }
            catch
            {
                log.Info($"{work} FOUR fail");
                return false;
            }

            return true;
        }

        private bool R2D(string work, out string error, out bool thereAreDocuments)
        {
            Switchah();
            error = "";
            thereAreDocuments = true;
            bool found = false;
            try
            {
                log.Info($"{work}");

                while (!found)
                {
                    if (!Keanu.Driver.PageSource.ToString().Contains("LAVORAZIONE PRATICHE FITTIZIE"))
                    {
                        try
                        {
                            Keanu.Driver.Navigate().GoToUrl("http://r2dapps.awselb.enelint.global/r2d/ele/home.do");
                            SelectWork(work);
                        }
                        catch
                        {
                            Keanu.Driver.Close();
                            if (!Keanu.PepperYourChrome(Keanu.LoginR2D, Keanu.PassR2D, "http://r2dapps.awselb.enelint.global/r2d/ele/home.do", "Benvenuto su R2D", false))
                                return false;
                            SelectWork(work);
                        }
                    }

                    Keanu.WaitingGame();

                    var tablePraticheFittizie = Keanu.Driver.FindElements(By.XPath("//table")).Last();
                    IList<IWebElement> r = tablePraticheFittizie.FindElements(By.TagName("tr"));
                    foreach (var row in r.Skip(1))//SKIP HEADER
                    {
                        IList<IWebElement> columns = row.FindElements(By.TagName("td"));
                        if (columns[0].Text.StartsWith("IT001E"))
                        {
                            datir2d.Pod = columns[0].Text.Trim();
                            datir2d.IdCrm = columns[1].Text.Trim();
                            datir2d.RagioneSociale = columns[2].Text.Trim();
                            datir2d.PIVA = columns[3].Text.Trim();
                            datir2d.CF = columns[4].Text.Trim();
                            datir2d.Distributore = columns[5].Text.Trim();
                            datir2d.CodiceServizioDt = columns[7].Text.Trim();
                            datir2d.DataLavorazione = Convert.ToDateTime(columns[9].Text.Substring(0, 10));
                            datir2d.Azione = columns[10];

                            log.Info($"{datir2d.Pod} {datir2d.IdCrm}");

                            #region PER VOLTURE & PRIMA ATTIVAZIONE
                            if (work.Equals("Voltura senza accollo") || work.Equals("Voltura con accollo") || work.Equals("Prima Attivazione"))
                            {
                                try
                                {
                                    var dossier = datir2d.Azione.FindElement(By.CssSelector("img")).GetAttribute("onclick");
                                    var dossierNumber = dossier.Substring(dossier.IndexOf('(') + 2, 8);

                                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript($"popup('/r2d/ele/DossierDetailFromTemplate.do?dossierId={dossierNumber}&codServ=POPPS&storico=N');");
                                    Keanu.WaitingGame();
                                    Thread.Sleep(Keanu.Randy(1));

                                    foreach (var item in Keanu.Driver.WindowHandles)
                                    {
                                        Keanu.Driver.SwitchTo().Window(item);
                                        if (Keanu.Driver.PageSource.ToString().Contains("Dettaglio Pratica Ps"))
                                            break;
                                    }

                                    bool allRed = false;
                                    var trs = Keanu.Driver.FindElements(By.TagName("tr"));
                                    foreach (var tr in trs)
                                    {
                                        if (allRed)
                                            break;
                                        var tds = tr.FindElements(By.TagName("td"));
                                        switch (tds[0].Text)
                                        {
                                            case "Codice Pratica Utente":
                                                {
                                                    datir2d.AttualePotenzaDisponibile = tds[1].Text;
                                                    break;
                                                }
                                            case "Nuovo Intestatario Codice Fiscale":
                                                {
                                                    datir2d.NuovoIntestatarioCF = tds[1].Text;
                                                    break;
                                                }
                                            case "Nuovo Intestatario Partita IVA":
                                                {
                                                    datir2d.NuovoIntestatarioPIVA = tds[1].Text;
                                                    break;
                                                }
                                            case "Pod":
                                                {
                                                    datir2d.Pod = tds[1].Text;
                                                    break;
                                                }
                                            case "Tensione Richiesta":
                                                {
                                                    datir2d.TensioneRichiesta = tds[1].Text;
                                                    log.Info($"{datir2d.TensioneRichiesta}");
                                                    break;
                                                }
                                            case "Attuale potenza contrattuale/disponibile Distributore":
                                                {
                                                    datir2d.AttualePotenzaDisponibile = tds[1].Text;
                                                    break;
                                                }
                                            case "Potenza in Franchigia":
                                                {
                                                    datir2d.AttualePotenzaInFranchigia = tds[1].Text;
                                                    break;
                                                }
                                            case "Tipo Misuratore":
                                                {
                                                    datir2d.TipoMisuratore = tds[1].Text;
                                                    log.Info($"{datir2d.TipoMisuratore}");
                                                    allRed = true;
                                                    break;
                                                }
                                        }
                                    }

                                    Keanu.Driver.Close();

                                    foreach (var item in Keanu.Driver.WindowHandles)
                                    {
                                        Keanu.Driver.SwitchTo().Window(item);
                                        if (Keanu.Driver.PageSource.ToString().Contains("LAVORAZIONE PRATICHE FITTIZIE"))
                                            break;
                                    }

                                }
                                catch
                                {
                                    log.Info($"Cannot read Dettaglio Pratica Ps");
                                    return false;
                                }
                            }
                            #endregion

                            datir2d.Azione.Click();
                            Keanu.WaitingGame();
                            var bConferma = Keanu.Driver.FindElement(By.XPath("//a[contains(text(), 'Conferma')]"));
                            bConferma.Click();
                            Thread.Sleep(Keanu.Randy(1));
                            try
                            {
                                var alert = Keanu.Driver.SwitchTo().Alert();
                                alert.Accept();
                                Keanu.WaitingGame();
                            }
                            catch
                            {
                                log.Info($"{work} R2D alert fail");
                            }
                            found = true;
                            break;
                        }
                    }
                    if (found)
                        break;
                    else
                    {
                        try
                        {
                            var paginaSuccessiva = Keanu.Driver.FindElement(By.XPath("//a[contains(@title,'successiva:')]"));
                            paginaSuccessiva.Click();
                            Keanu.WaitingGame();
                            continue;
                        }
                        catch
                        {
                            //log.Info($"{work} R2D, there is no next page");
                            thereAreDocuments = false;
                            return false;
                        }
                    }
                }
            }
            catch
            {
                log.Info($"{work} R2D fail");
                return false;
            }

            Keanu.WaitingGame();

            if (!Keanu.Driver.PageSource.ToString().Contains("Si sta procedendo al caricamento puntuale di una pratica fittizia"))
            {
                error = $"{work} dopo cliccare sulla pulsante [Conferma], nuova pagina non contiene [Evento]";
                log.Info(error);
                return false;
            }

            return true;
        }

        private bool R2D2(string work, out string error)
        {
            Switchah();
            error = "";
            try
            {
                int cnt = 0;
                while (!Keanu.Driver.PageSource.ToString().Contains("SELEZIONE DELLA TIPOLOGIA DI EVENTO DA CARICARE") && cnt < 15)
                {
                    Thread.Sleep(Keanu.Randy(1));
                    cnt++;
                }

                var cEvento = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'workTypeId')]"));
                var selectEvento = new OpenQA.Selenium.Support.UI.SelectElement(cEvento);
                switch (work)
                {
                    case "Disdetta con suggello":
                        selectEvento.SelectByText("Esito Richiesta - Esito D01 Disdetta con suggello");
                        break;
                    case "Prima Attivazione":
                        selectEvento.SelectByText("Esito Richiesta - Esito A01");
                        break;
                    case "Voltura senza accollo":
                        selectEvento.SelectByText("Esito Richiesta - Esito Richiesta Voltura VT1 AU");
                        break;
                    case "Voltura con accollo":
                        selectEvento.SelectByText("Esito Richiesta - Esito Richiesta Voltura con Accollo VT1 AU");
                        break;
                    case "Subentro":
                        selectEvento.SelectByText("Esito Richiesta - Esito S01 subentro a parità di condizioni");
                        break;
                    case "Subentro 2":
                        selectEvento.SelectByText("Esito Richiesta - Esito S01 subentro a parità di condizioni");
                        break;
                    case "Subentro con modifica contrattuale":
                        selectEvento.SelectByText("Esito Richiesta - Esito S01 subentro a parità di condizioni");
                        break;
                    case "Modifica Potenza":
                        selectEvento.SelectByText("Esito Richiesta - Esito Richiesta MC1 Preventivo Rapido");
                        break;
                    case "Modifica Tensione":
                        selectEvento.SelectByText("Esito Richiesta - Esito Richiesta MC1 Preventivo Rapido");
                        break;
                    default:
                        break;
                }
                Keanu.WaitingGame();

                var radioButtons = Keanu.Driver.FindElements(By.XPath("//input[contains(@type,'radio')]"));
                foreach (var item in radioButtons)
                {
                    var rButton = item.GetAttribute("onclick");
                    if (rButton.Contains("value='OK'"))
                        item.Click();
                }
                Keanu.WaitingGame();

                CaricaAndAlert(work);

                IWebElement dataRicezioneEsito = null;
                IWebElement codiceRichiestaDt = null;
                IWebElement dataEsecuzioneDellaPrestazione = null;
                IWebElement codiceCausale = null;
                IWebElement dettaglioCausale = null;
                //IWebElement attualeOpzioneTariffaria = null;
                IWebElement attualeTensioneConsegna = null;
                IWebElement attualePotenzaDisponibile = null;
                IWebElement attualePontenzaInFranchigia = null;
                //IWebElement tipoMisuratore = null;

                switch (work)
                {
                    case "Disdetta con suggello":

                        dataRicezioneEsito = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C372')]"));
                        dataRicezioneEsito.Clear();
                        dataRicezioneEsito.SendKeys(datisii.DataFine);

                        codiceRichiestaDt = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C212')]"));
                        codiceRichiestaDt.Clear();
                        codiceRichiestaDt.SendKeys(DateTime.Now.ToString("dd''MM''yyyy''HH''mm''"));

                        dataEsecuzioneDellaPrestazione = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C54')]"));
                        dataEsecuzioneDellaPrestazione.Clear();
                        dataEsecuzioneDellaPrestazione.SendKeys(datisii.DataFine);

                        codiceCausale = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C213')]"));
                        codiceCausale.Clear();
                        codiceCausale.SendKeys("EVASA");

                        dettaglioCausale = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C214')]"));
                        dettaglioCausale.Clear();
                        dettaglioCausale.SendKeys("EVASA");

                        break;
                    case "Prima Attivazione":
                        dataRicezioneEsito = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C372')]"));
                        dataRicezioneEsito.Clear();
                        dataRicezioneEsito.SendKeys(DateTime.Now.ToShortDateString());

                        codiceRichiestaDt = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C212')]"));
                        codiceRichiestaDt.Clear();
                        codiceRichiestaDt.SendKeys(DateTime.Now.ToString("dd''MM''yyyy''HH''mm''"));

                        dataEsecuzioneDellaPrestazione = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C54')]"));
                        dataEsecuzioneDellaPrestazione.Clear();
                        dataEsecuzioneDellaPrestazione.SendKeys(datisii.DataInizio);

                        var attualeOpzioneTariffaria = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C17')]"));
                        var attualeOpzioneTariffariaEvento = new OpenQA.Selenium.Support.UI.SelectElement(attualeOpzioneTariffaria);
                        attualeOpzioneTariffariaEvento.SelectByText(datisii.TariffaDistributore);
                        //attualeOpzioneTariffaria = keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C17')]"));
                        //attualeOpzioneTariffaria.Clear();
                        //attualeOpzioneTariffaria.SendKeys(datisii.TariffaDistributore);

                        attualeTensioneConsegna = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C14')]"));
                        attualeTensioneConsegna.Clear();
                        attualeTensioneConsegna.SendKeys(datir2d.TensioneRichiesta);

                        attualePotenzaDisponibile = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C257')]"));
                        attualePotenzaDisponibile.Clear();
                        attualePotenzaDisponibile.SendKeys(datir2d.AttualePotenzaDisponibile.Replace(",", "."));

                        var tipoMisuratore = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C284')]"));
                        var tipoMisuratoreEvento = new OpenQA.Selenium.Support.UI.SelectElement(tipoMisuratore);
                        tipoMisuratoreEvento.SelectByText(datir2d.TipoMisuratore);
                        //tipoMisuratore = keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C284')]"));
                        //tipoMisuratore.Clear();
                        //tipoMisuratore.SendKeys(datir2d.TipoMisuratore);

                        attualePontenzaInFranchigia = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C286')]"));
                        attualePontenzaInFranchigia.Clear();
                        attualePontenzaInFranchigia.SendKeys(datir2d.AttualePotenzaInFranchigia.Replace(",", "."));

                        break;
                    case "Voltura senza accollo":

                        dataRicezioneEsito = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C372')]"));
                        dataRicezioneEsito.Clear();
                        dataRicezioneEsito.SendKeys(datisii.DataEsito);

                        codiceRichiestaDt = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C212')]"));
                        codiceRichiestaDt.Clear();
                        codiceRichiestaDt.SendKeys(datisii.Protocollo);

                        dataEsecuzioneDellaPrestazione = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C54')]"));
                        dataEsecuzioneDellaPrestazione.Clear();
                        dataEsecuzioneDellaPrestazione.SendKeys(datisii.DataDecVoltura);

                        codiceCausale = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C213')]"));
                        codiceCausale.Clear();
                        codiceCausale.SendKeys("EVASA");

                        dettaglioCausale = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C214')]"));
                        dettaglioCausale.Clear();
                        dettaglioCausale.SendKeys("EVASA");

                        CaricaAndAlert(work);

                        if (!Keanu.Driver.PageSource.ToString().Contains("Codice Voce"))
                        {
                            error = "R2D, dopo cliccare sulla pulsante [Carica], nuova pagina non contiene [Codice Voce]";
                            log.Info(error);
                            return false;
                        }

                        break;
                    case "Voltura con accollo":

                        dataRicezioneEsito = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C372')]"));
                        dataRicezioneEsito.Clear();
                        dataRicezioneEsito.SendKeys(datisii.DataEsito);

                        codiceRichiestaDt = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C212')]"));
                        codiceRichiestaDt.Clear();
                        codiceRichiestaDt.SendKeys(datisii.Protocollo);

                        dataEsecuzioneDellaPrestazione = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C54')]"));
                        dataEsecuzioneDellaPrestazione.Clear();
                        dataEsecuzioneDellaPrestazione.SendKeys(datisii.DataDecVoltura);

                        codiceCausale = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C213')]"));
                        codiceCausale.Clear();
                        codiceCausale.SendKeys("EVASA");

                        dettaglioCausale = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C214')]"));
                        dettaglioCausale.Clear();
                        dettaglioCausale.SendKeys("EVASA");

                        break;
                    case "Subentro":

                        CaricaAndAlert(work);

                        if (!Keanu.Driver.PageSource.ToString().Contains("Codice Voce"))
                        {
                            error = "R2D, dopo cliccare sulla pulsante [Carica], nuova pagina non contiene [Codice Voce]";
                            log.Info(error);
                            return false;
                        }

                        break;
                    case "Subentro 2":

                        CaricaAndAlert(work);

                        if (!Keanu.Driver.PageSource.ToString().Contains("Codice Voce"))
                        {
                            error = "R2D, dopo cliccare sulla pulsante [Carica], nuova pagina non contiene [Codice Voce]";
                            log.Info(error);
                            return false;
                        }

                        break;
                    case "Subentro con modifica contrattuale":

                        CaricaAndAlert(work);

                        if (!Keanu.Driver.PageSource.ToString().Contains("Codice Voce"))
                        {
                            error = "R2D, dopo cliccare sulla pulsante [Carica], nuova pagina non contiene [Codice Voce]";
                            log.Info(error);
                            return false;
                        }

                        break;
                    case "Modifica Potenza":

                        codiceRichiestaDt = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C212')]"));
                        codiceRichiestaDt.Clear();
                        codiceRichiestaDt.SendKeys(datifour.COD_PRAT_DISTR);

                        dataRicezioneEsito = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C372')]"));
                        dataRicezioneEsito.Clear();
                        if (datifour.DataOraNotifica.Length > 12)
                            dataRicezioneEsito.SendKeys(datifour.DataOraNotifica.Remove(10).Replace('-', '/'));
                        else
                            dataRicezioneEsito.SendKeys(datifour.DataOraNotifica.Replace('-', '/'));

                        dataEsecuzioneDellaPrestazione = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C54')]"));
                        dataEsecuzioneDellaPrestazione.Clear();
                        dataEsecuzioneDellaPrestazione.SendKeys(datifour.DATA_ESECUZIONE);

                        codiceCausale = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C213')]"));
                        codiceCausale.Clear();
                        codiceCausale.SendKeys("EVASA");

                        attualeTensioneConsegna = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C14')]"));
                        attualeTensioneConsegna.Clear();
                        attualeTensioneConsegna.SendKeys(datifour.TENSIONE);

                        attualePotenzaDisponibile = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C257')]"));
                        attualePotenzaDisponibile.Clear();
                        attualePotenzaDisponibile.SendKeys(datifour.POT_DISP);

                        break;
                    case "Modifica Tensione":

                        codiceRichiestaDt = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C212')]"));
                        codiceRichiestaDt.Clear();
                        codiceRichiestaDt.SendKeys(datifour.COD_PRAT_DISTR);

                        dataRicezioneEsito = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C372')]"));
                        dataRicezioneEsito.Clear();
                        if (datifour.DataOraNotifica.Length > 12)
                            dataRicezioneEsito.SendKeys(datifour.DataOraNotifica.Remove(10).Replace('-', '/'));
                        else
                            dataRicezioneEsito.SendKeys(datifour.DataOraNotifica.Replace('-', '/'));

                        dataEsecuzioneDellaPrestazione = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C54')]"));
                        dataEsecuzioneDellaPrestazione.Clear();
                        dataEsecuzioneDellaPrestazione.SendKeys(datifour.DATA_ESECUZIONE);

                        codiceCausale = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C213')]"));
                        codiceCausale.Clear();
                        codiceCausale.SendKeys("EVASA");

                        attualeTensioneConsegna = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C14')]"));
                        attualeTensioneConsegna.Clear();
                        attualeTensioneConsegna.SendKeys(datifour.TENSIONE);

                        attualePotenzaDisponibile = Keanu.Driver.FindElement(By.XPath("//*[contains(@name,'C257')]"));
                        attualePotenzaDisponibile.Clear();
                        attualePotenzaDisponibile.SendKeys(datifour.POT_DISP);

                        break;
                    default:
                        break;
                }

                CaricaAndAlert(work);

                if (!Keanu.Driver.PageSource.ToString().Contains("Aggiornamento effettuato correttamente"))
                {
                    error = "R2D, dopo cliccare sulla pulsante [Carica], nuova pagina non contiene [Aggiornamento effettuato correttamente]";
                    log.Info(error);
                    return false;
                }

                var bIndietro = Keanu.Driver.FindElement(By.XPath("//a[contains(text(), 'Indietro')]"));
                bIndietro.Click();
                Keanu.WaitingGame();

            }
            catch
            {
                log.Info($"{work} R2D2 fail");
                return false;
            }

            return true;
        }

        private void CaricaAndAlert(string work)
        {
            var bCarica = Keanu.Driver.FindElement(By.XPath("//a[contains(text(), 'Carica')]"));
            bCarica.Click();
            Thread.Sleep(Keanu.Randy(1));
            try
            {
                var alert = Keanu.Driver.SwitchTo().Alert();
                alert.Accept();
                Keanu.WaitingGame();
            }
            catch
            {
                log.Info($"{work} alert fail");
            }
        }

        private bool Fatto()
        {
            try
            {
                if (!Keanu.Agente.RegistraCompleto(datir2d.IdCrm, datir2d.Pod, DateTime.Now.ToShortDateString(), Keanu.LavRegId, "", "", "", 1, false, true, false))
                {
                    log.Error("Errore durante l'inserimento della lavorazione!");
                    Keanu.Erry();
                    if (!Keanu.Agente.RegistraCompleto(datir2d.IdCrm, datir2d.Pod, DateTime.Now.ToShortDateString(), Keanu.LavRegId, "", "", "", 1, false, true, false))
                    {
                        log.Error("Errore durante l'inserimento della lavorazione! 2x");
                        return false;
                    }
                }
            }
            catch (Exception Ex)
            {
                log.Error(Ex.ToString());
            }
            Keanu.Bad.Fatto++;
            return true;
        }

        private bool Scarto(string error)
        {
            try
            {
                if (!Keanu.Agente.RegistraCompleto(datir2d.IdCrm, datir2d.Pod + $" {error}", DateTime.Now.ToShortDateString(), Keanu.LavScartoId, "", "", "", 1, false, true, true))
                {
                    log.Error("Errore durante l'inserimento della lavorazione!");
                    Keanu.Erry();
                    if (!Keanu.Agente.RegistraCompleto(datir2d.IdCrm, datir2d.Pod, DateTime.Now.ToShortDateString(), Keanu.LavScartoId, "", "", "", 1, false, true, true))
                    {
                        log.Error("Errore durante l'inserimento della lavorazione! 2x");
                        return false;
                    }

                }
            }
            catch (Exception Ex)
            {
                log.Error(Ex.ToString());
            }
            Keanu.Bad.Scarto++;
            return true;
        }

        private void MoveToObject(IWebElement e)
        {
            try
            {
                OpenQA.Selenium.Interactions.Actions a = new OpenQA.Selenium.Interactions.Actions(Keanu.Driver);
                a.MoveToElement(e);
                a.Perform();
            }
            catch
            {
                Thread.Sleep(Keanu.Randy(1));
            }
        }

        private bool SelectWork(string work)
        {
            try
            {
                if (!PepperYourR2D())
                    return false;
                Keanu.WaitingGame();
                var tableMain = Keanu.Driver.FindElements(By.XPath("//table")).Last();
                IList<IWebElement> rows = tableMain.FindElements(By.TagName("tr"));
                foreach (var row in rows.Skip(2))//SKIP HEADERS
                {
                    IList<IWebElement> columns = row.FindElements(By.TagName("td"));
                    if (columns[0].Text.Equals(work))
                    {
                        while (Keanu.Driver.PageSource.ToString().Contains("Numero ordine CRM"))
                        {
                            Thread.Sleep(Keanu.Randy(1));
                            var label = columns[1].FindElement(By.CssSelector("label"));
                            if (label.Text == "0")
                            {
                                log.Info($"{work} 0");
                                return false;
                            }
                            ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", label);
                            //label.Click();
                            //columns[1].Click();
                            Keanu.WaitingGame();
                            Thread.Sleep(Keanu.Randy(1));
                        }
                        break;
                    }
                }
            }
            catch
            {
                log.Info($"{work} R2D SelectWork() fail");
                return false;
            }
            return true;
        }

        private bool PepperYourR2D()
        {
            int cnt = 0;
            while (!Keanu.Driver.PageSource.ToString().Contains("Tipo lavoro CRM") && cnt < 15)
            {
                var bMenu = Keanu.Driver.FindElement(By.Id("MENU_ARROW_3"));
                bMenu.Click();
                Keanu.WaitingGame();

                var subMenu = Keanu.Driver.FindElements(By.XPath("//div[@id='MENU_SUB_3']//div"));
                foreach (var item in subMenu)
                {
                    if (item.Text.Contains("Gestione Pratiche Fittizie"))
                    {
                        item.Click();
                        Keanu.WaitingGame();
                        break;
                    }
                }
                Keanu.WaitingGame();
                cnt++;
            }
            Keanu.WaitingGame();
            return true;
        }
    }
}