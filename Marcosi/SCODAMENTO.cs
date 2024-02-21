using log4net;
using MARCUS.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SFALibrary;
using SFALibrary.Model;
using System;
using System.Linq;
using System.Threading;
using Smart.Gravity.Model;
using MARCUS.Utils;
using static MARCUS.Helpers.Constant;
using AutoIt;
using System.IO;
using System.Net;

namespace MARCUS.Marcosi
{
    class SCODAMENTO
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SCODAMENTO));
        public Keanu Keanu { get; set; }
        public SqlQuery query = new SqlQuery();

        string acqDate = "";

        public SCODAMENTO(Keanu keanu)
        {
            this.Keanu = keanu;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        }

        public SfaLib sfaLib { get; set; }
        public int sfaLibFails = 0;

        private Cascata riferimentoCorrenteGravity;
        private GravityApiHelper Grav = new GravityApiHelper();

        public Records.VariablesSCODAMENTO v = null;

        public string enelML = @"https://dm2-ee.enelint.global/Document/default.aspx?DocID=";
        public string enelMV = @"https://crmsmart.enelint.global/Crm/DMSServlet?progressivo=0&idriferimento=";

        public int needToRestart = 0;

        public bool Iframe { get; set; }

        public bool Flow()
        {

            Keanu.KillChromeWebDriver();

            if (Keanu.ModificaPodTrip)
            {
                if (!Keanu.PepperYourChrome(Keanu.LoginGEOCALL, Keanu.PassGEOCALL, "http://geocall-ml.enelint.global/Mercato/SSOServlet", "Principale", false))
                    return false;
                if (!PepperYourGeocall())
                    return false;
                if (!Keanu.PepperYourChromeExtra(Keanu.LoginSFA, Keanu.PassSFA, "https://enelcrmt.my.salesforce.com/"))
                //if (!Keanu.PepperYourChromePDF(Keanu.LoginSFA, Keanu.PassSFA, "https://enelcrmt.my.salesforce.com/", "", true))
                    return false;
                if (!PepperYourSfaLib())
                    
                    return false;
            }
            else
            {
                if (!Keanu.PepperYourChrome(Keanu.LoginGEOCALL, Keanu.PassGEOCALL, "http://geocall-ml.enelint.global/Mercato/SSOServlet", "Principale", false))
                    return false;
                if (!PepperYourGeocall())
                    return false;
                if (!PepperYourSfaLib())
                    return false;
                if (!Keanu.PepperYourChromeExtra(Keanu.LoginSFA, Keanu.PassSFA, "https://enelcrmt.my.salesforce.com/"))
                //if (!Keanu.PepperYourChromePDF(Keanu.LoginSFA, Keanu.PassSFA, "https://enelcrmt.my.salesforce.com/", "", true))
                    return false;
            }

            //if (!Keanu.PepperYourChrome(Keanu.LoginGEOCALL, Keanu.PassGEOCALL, "http://geocall-ml.enelint.global/Mercato/SSOServlet", "Principale", false))
            //    return false;
            //if (!PepperYourGeocall())
            //    return false;
            //if (!PepperYourSfaLib())
            //    return false;


            //Changed to gravity style only
            log.Warn("Gravity style");
            Grav.SetGravityCredential(Keanu.LoginAGENTE, Keanu.PassAGENTE);
            Grav.GravitySetup();
            Grav.GravityConnection(Keanu.LavLoginId);
            /*
            if (Keanu.LavName.Equals("SCODAMENTO - EE145 - DATI CATASTALI"))
            {
                log.Warn("Gravity style");
                Grav.SetGravityCredential(Keanu.LoginAGENTE, Keanu.PassAGENTE);
                Grav.GravitySetup();
                Grav.GravityConnection(Keanu.LavLoginId);
            }
            else
            {
                log.Warn($"Agente style");
                if (!Keanu.Agente.LoginAndConnection(Keanu.LoginAGENTE, Keanu.PassAGENTE, Keanu.LavLoginId))
                {
                    log.Error("Impossibile effettuare la connessione in Agente nella coda di lavorazione");
                    return false;
                }
            }
            */

            #region Check how much in coda
            int inCoda = query.GetDaLavorareByIdDettaglioTipoLavorazione(Keanu.IdRemainingCheck);
            log.Info($"Nel codice {inCoda}");
            bool marc;
            while (inCoda > Keanu.ScodamentoMax)
            {
                inCoda = Aspetter(inCoda);
                if (inCoda == -1)
                    return false;
            }
            #endregion

            int counter = 0;
            while (true)
            {

                if (Keanu.Driver == null) {
                    if (!Keanu.PepperYourChrome(Keanu.LoginGEOCALL, Keanu.PassGEOCALL, "http://geocall-ml.enelint.global/Mercato/SSOServlet", "Principale", false))
                        return false;
                    if (!PepperYourGeocall())
                        return false;
                }

                v = new Records.VariablesSCODAMENTO();

                Thread.Sleep(Keanu.Randy(1));
                if (counter == Keanu.ScodamentoMin)
                {
                    counter = 0;
                    inCoda = query.GetDaLavorareByIdDettaglioTipoLavorazione(Keanu.IdRemainingCheck);
                    while (inCoda > Keanu.ScodamentoMax)
                    {
                        inCoda = Aspetter(inCoda);
                        if (inCoda == -1)
                            return false;
                    }
                }

                if (sfaLib == null)
                {
                    if (!PepperYourSfaLib())
                        return false;
                }

                int cnt = 0;
                do
                {
                    try
                    {
                        var prosimmoTaskTd = Keanu.Driver.FindElement(By.Id("MTA-EstrazioniProgrammate-_fSelezioneGruppo"));
                        var prosimoButton = prosimmoTaskTd.FindElement(By.XPath("//button"));
                        prosimoButton.Click();

                        int cintitni = 0;
                        while (!Keanu.Driver.PageSource.Contains("Termina"))
                        {
                            if (cintitni > 1)
                            {
                                Thread.Sleep(Keanu.Randy(60));
                                Actions a = new Actions(Keanu.Driver);
                                a.SendKeys(Keys.Escape).Build().Perform();
                                Thread.Sleep(Keanu.Randy(2));
                                prosimmoTaskTd = Keanu.Driver.FindElement(By.Id("MTA-EstrazioniProgrammate-_fSelezioneGruppo"));
                                prosimoButton = prosimmoTaskTd.FindElement(By.XPath("//button"));
                                prosimoButton.Click();
                                Thread.Sleep(Keanu.Randy(1));
                                cintitni = 0;
                            }
                            Thread.Sleep(Keanu.Randy(2));
                            cintitni++;
                        }

                        marc = false;
                        var allHeaders = Keanu.Driver.FindElements(By.TagName("td"));
                        for (int lbl = 0; allHeaders.Count() > 0; lbl++)
                        {
                            if (marc)
                                break;
                            switch (allHeaders[lbl].Text.ToString().Trim())
                            {
                                case "ID Attività":
                                    {
                                        v.Attivita = "A-0753708712 ";//allHeaders[lbl + 1].Text.ToString().Trim();
                                        Keanu.Bad.Riferimento = v.Attivita;
                                        break;
                                    }
                                case "Data Creazione Attività":
                                    {
                                        v.DataRic = allHeaders[lbl + 1].Text.ToString().Trim();
                                        marc = true;
                                        break;
                                    }
                            }
                        }
                    }
                    catch
                    {
                        log.Info($"Geocall error, try again. Loop {cnt}");
                        Keanu.Driver.Navigate().GoToUrl("http://geocall-ml.enelint.global/Mercato/SSOServlet");
                        Thread.Sleep(Keanu.Randy(3));
                        Actions a = new Actions(Keanu.Driver);
                        a.SendKeys(Keys.Escape).Build().Perform();
                        Thread.Sleep(Keanu.Randy(25));
                        cnt++;
                        continue;
                    }
                    cnt++;
                } while (cnt < 5 && v.OldAttivita == v.Attivita);

                if (v.OldAttivita == v.Attivita)
                {
                    log.Info($"Failure, riavvia tutto");
                    RestratAll();
                    continue;
                }
                else
                {
                    v.OldAttivita = v.Attivita;
                    log.Info($"Attivita {v.Attivita}");
                    log.Info($"Data {v.DataRic}");
                }

                v.SF = "-";
                Attivita att = new Attivita();
                cnt = 0;
                do
                {
                    if (cnt > 1)
                        Thread.Sleep(Keanu.Randy(15));
                    att = sfaLib.SearchAttivita(v.Attivita, true);
                    Thread.Sleep(Keanu.Randy(1));
                    cnt++;
                } while (cnt < 3 && att == null);

                if (att == null)
                {

                    int sfaLibTries = 0;

                    while(sfaLibTries < 3) {

                        Thread.Sleep(Keanu.Randy(5));

                        if (!PepperYourSfaLib()) {

                            if(sfaLibTries == 2) {
                                log.Error($"Sfalib errore - failed to connect, auto registra {v.Attivita} & stop");
                                if (!Reggy()) {
                                    Terminator();
                                    return false;
                                }
                                Terminator();
                                Keanu.KillChromeWebDriver();
                                return false;
                            }
                        } else {
                            break;
                        }

                        sfaLibTries++;
                    }

                    /*

                    if (!PepperYourSfaLib())
                    {
                        log.Error($"Sfalib errore - failed to connect, auto registra {v.Attivita} & stop");
                        if (!Reggy())
                        {
                            Terminator();
                            return false;
                        }
                        Terminator();
                        Keanu.KillChromeWebDriver();
                        return false;
                    }

                    */

                    att = sfaLib.SearchAttivita(v.Attivita, true);
                    if (att == null)
                    {
                        log.Error($"Sfalib errore - failed to find, auto registra {v.Attivita} & stop");
                        if (!Reggy())
                        {
                            Terminator();
                            return false;
                        }
                        Terminator();
                        Keanu.KillChromeWebDriver();
                        return false;
                    }
                }


                this.acqDate = att.DataAcquisizioneEnel;
                Keanu.IdAttivitaPerScodamento = att.RecordId;
                log.Info($"{Keanu.IdAttivitaPerScodamento}");

                log.Info($"Stato {att.Stato}");

                switch (att.Stato)
                {
                    case "FATTO":
                        break;
                    case "ANNULLATO":
                        break;
                    case "DA FARE":
                        break;
                    case "IN LAVORAZIONE":
                        break;
                    case "RESPINTA":
                        Terminator();
                        continue;
                }

                Documento documento = null;
                try {
                    documento = (Documento)sfaLib.GetRecord(att.DocumentRecordId, SFALibrary.Helpers.Utility.RicercaDocumentoName);
                    sfaLibFails = 0;

                } catch (Exception ex) {
                    
                    sfaLibFails = sfaLibFails + 1;
                    sfaLib = null;

                    if (sfaLibFails >= 2) {
                        log.Error($"Double fail with Sfalib - {v.Attivita}");
                        if (!Reggy()) {
                            Terminator();
                            return false;
                        }
                        Terminator();
                        Keanu.KillChromeWebDriver();

                        return false;

                    }

                    log.Error($"Fail with Sfalib - {v.Attivita}");
                    if (!Reggy()) {
                        Terminator();
                        return false;
                    }
                    Terminator();
                    

                    continue;

                }


                
                if (documento != null)
                    v.SF = documento.Name.Trim();

                if (v.SF.Length < 3)
                    v.SF = "-";

                log.Info($"SF {v.SF}");
                
                if(DownloadPDF(v.SF, enelML)) {
                    log.Info($"Document downloaded - {v.SF}");
                } else {
                    log.Error($"Document failed to download - {v.SF}");
                    needToRestart = needToRestart + 1 ;
                }

                if (WasRiferimentoInAgente(v.Attivita, Keanu.IdRemainingCheck))
                {
                    log.Debug($"Already in agente {v.Attivita} {v.SF}");
                    Terminator();
                    continue;
                }

                //Reggy();
                if (!Reggy())
                {
                    Terminator();
                    break;
                }
                Terminator();
                counter++;
                Keanu.Bad.Fatto++;

                #region FOR LOOP MATRICOLE
                if (Keanu.LavName.Equals("SCODAMENTO - EE62 - LOOP MATRICOLE"))
                    log.Info(Keanu.LoginGEOCALL);
                if (Keanu.LavName.Equals("SCODAMENTO - EE62 - LOOP MATRICOLE") && Keanu.Bad.Fatto.Equals(150))
                {
                    Keanu.Bad.Fatto = 0;
                    return false;
                }
                #endregion

                if (Keanu.StartStop == false)
                    break;

                if(needToRestart > 30) {
                    needToRestart = 0;
                    Keanu.KillChromeWebDriver();
                    Keanu.Driver = null;
                    Keanu.DriverExtra = null;
                }
            }
            Keanu.KillChromeWebDriver();
            //Changed to gravity style only
            Grav.GravityConnectionClose();
            /*
            if (Keanu.LavName.Equals("SCODAMENTO - EE145 - DATI CATASTALI"))
                Grav.GravityConnectionClose();
            else
                Keanu.Agente.Logout();
            */
            //Keanu.Agente.Logout();
            //Grav.GravityConnectionClose();
            return false;
        }

        private bool RestratAll()
        {
            log.Info($"Riavvia tutto");
            Keanu.KillChromeWebDriver();
            Thread.Sleep(Keanu.Randy(5));
            //Changed to gravity style only
            Grav.GravityConnection(Keanu.LavLoginId);
            /*
            if (Keanu.LavName.Equals("SCODAMENTO - EE145 - DATI CATASTALI"))
            {
                Grav.GravityConnection(Keanu.LavLoginId);
            }
            else
            {
                if (!Keanu.Agente.LoginAndConnection(Keanu.LoginAGENTE, Keanu.PassAGENTE, Keanu.LavLoginId))
                {
                    log.Error("Impossibile effettuare la connessione in Agente nella coda di lavorazione");
                    return false;
                }
            }
            */

            //if (!Keanu.Agente.LoginAndConnection(Keanu.LoginAGENTE, Keanu.PassAGENTE, Keanu.LavLoginId))
            //{
            //    log.Error("Impossibile effettuare la connessione in Agente nella coda di lavorazione");
            //    return false;
            //}

            //Grav.SetGravityCredential(Keanu.LoginAGENTE, Keanu.PassAGENTE);
            //Grav.GravitySetup();
            //Grav.GravityConnection(Keanu.LavLoginId);

            if (Keanu.ModificaPodTrip)
            {
                if (!Keanu.PepperYourChrome(Keanu.LoginGEOCALL, Keanu.PassGEOCALL, "http://geocall-ml.enelint.global/Mercato/SSOServlet", "Principale", false))
                    return false;
                if (!PepperYourGeocall())
                    return false;
                if (!Keanu.PepperYourChromeExtra(Keanu.LoginSFA, Keanu.PassSFA, "https://enelcrmt.my.salesforce.com/"))
                    return false;
                if (!PepperYourSfaLib())
                    return false;
            }
            else
            {
                if (!Keanu.PepperYourChrome(Keanu.LoginGEOCALL, Keanu.PassGEOCALL, "http://geocall-ml.enelint.global/Mercato/SSOServlet", "Principale", false))
                    return false;
                if (!PepperYourGeocall())
                    return false;
                if (!PepperYourSfaLib())
                    return false;
            }

            //if (!Keanu.PepperYourChrome(Keanu.LoginGEOCALL, Keanu.PassGEOCALL, "http://geocall-ml.enelint.global/Mercato/SSOServlet", "Principale", false))
            //    return false;
            //if (!PepperYourGeocall())
            //    return false;
            //if (!PepperYourSfaLib())
            //    return false;

            return true;
        }

        private bool PepperYourGeocall()
        {
            try
            {
                var operativa = Keanu.Driver.FindElement(By.Id("TBB_tbm2"));
                operativa.Click();
                Keanu.WaitingGame();
                var estrTask = Keanu.Driver.FindElement(By.XPath("//*[@id='TBB_tbm2']/div[1]/div[2]"));
                estrTask.Click();
                Keanu.WaitingGame();
                var combobo = Keanu.Driver.FindElement(By.Name("_lyGRUPPO_SELEZIONATO"));
                var select = new SelectElement(combobo);

                switch (Keanu.LavName)
                {
                    case "SCODAMENTO - EE145 - DATI CATASTALI":
                        select.SelectByText("S_DMS - Dati Catastali");
                        break;
                    case "SCODAMENTO - EE253 - CERTIFICAZIONI DEL CLT":
                        select.SelectByText("S_Dms - Certificazioni del Clt");
                        break;
                    case "SCODAMENTO - EE253 - CONTRATTI GESTONALI":
                        select.SelectByText("S_Dms - Contratti Gestionali");
                        break;
                    case "SCODAMENTO - EE253 - 147":
                        select.SelectByText("S_Dms – 147");
                        break;
                    case "SCODAMENTO - EE62 - SMART LETTONIA":
                        select.SelectByText("S_CRE_Smart_Lettonia");
                        break;
                    case "SCODAMENTO - EE253 - CONTRATTI (NEW)":
                        select.SelectByText("S_DMS - CONTRATTI");
                        break;
                    case "SCODAMENTO - EE231 - PRESCRIZIONE":
                        select.SelectByText("S_DMS - Prescrizione");
                        break;
                    default:
                        break;
                }

                Keanu.WaitingGame();
                log.Info($"PepperYourGeocall() ok");
                return true;
            }
            catch
            {
                log.Warn($"PepperYourGeocall() fail");
                return false;
            }
        }

        private bool PepperYourSfaLib()
        {
            sfaLib = new SfaLib(Keanu.LoginSFA, Keanu.PassSFA);
            bool marc = sfaLib.LoginProd();
            if (!marc) { marc = sfaLib.LoginProd(); }
            if (!marc) { log.Error("Login non riuscito su SFA con HTMlUnit."); return false; }
            return true;
        }

        private bool WasRiferimentoInAgente(string document, int docCheckerId)
        {
            int result = query.GetElementCountInLavorazioniAgenteByRiferimento(document, docCheckerId);
            if (result > 0)
                return true;
            result = query.GetElementCountInProdVolumiLavoratiByDettaglio(document, docCheckerId);
            if (result > 0)
                return true;
            result = query.GetElementCountInProdVolumiDaLavorareByDettaglio(document, docCheckerId);
            if (result > 0)
                return true;
            else
                return false;
        }

        private bool Reggy()
        {

            if(Keanu.LavLoginId == 14364) {
                if(InsertAcqEnelEE62(v.Attivita, v.SF, this.acqDate)) {
                    log.Info($"Document - {v.SF} with Data Acquisizione Enel ");
                } else {
                    log.Info($"Document - {v.SF} Data Acquisizione Enel fail");
                }
            }
            //Changed to gravity style only
            riferimentoCorrenteGravity = new Cascata() {
                Dettaglio = v.Attivita,
                DataRicAcq = Convert.ToDateTime(v.DataRic)
            };

            try {
                Grav.GravityRegistra(riferimentoCorrenteGravity, v.SF, Keanu.LavRegId);
                log.Debug($"{riferimentoCorrenteGravity.Dettaglio} {v.SF} FATTO");
            } catch (Exception) {
                log.Error("Errore durante l'inserimento della lavorazione!");
                return false;
            }
            /*
            if (Keanu.LavName.Equals("SCODAMENTO - EE145 - DATI CATASTALI"))
            {
                riferimentoCorrenteGravity = new Cascata()
                {
                    Dettaglio = v.Attivita,
                    DataRicAcq = Convert.ToDateTime(v.DataRic)
                };

                try
                {
                    Grav.GravityRegistra(riferimentoCorrenteGravity, v.SF, Keanu.LavRegId);
                    log.Debug($"{riferimentoCorrenteGravity.Dettaglio} {v.SF} FATTO");
                }
                catch (Exception)
                {
                    log.Error("Errore durante l'inserimento della lavorazione!");
                    return false;
                }
            }
            else
            {
                try
                {
                    if (!Keanu.Agente.RegistraCompleto(v.Attivita, v.SF, v.DataRic, Keanu.LavRegId, "", "", "", 1, false, true, false))
                    {
                        log.Error("Errore durante l'inserimento della lavorazione!");
                        Keanu.Erry();
                        if (!Keanu.Agente.RegistraCompleto(v.Attivita, v.SF, v.DataRic, Keanu.LavRegId, "", "", "", 1, false, true, false))
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
            }
            */

            return true;

            //try
            //{
            //    if (!Keanu.Agente.RegistraCompleto(v.Attivita, v.SF, v.DataRic, Keanu.LavRegId, "", "", "", 1, false, true, false))
            //    {
            //        log.Error("Errore durante l'inserimento della lavorazione!");
            //        Keanu.Erry();
            //        if (!Keanu.Agente.RegistraCompleto(v.Attivita, v.SF, v.DataRic, Keanu.LavRegId, "", "", "", 1, false, true, false))
            //        {
            //            log.Error("Errore durante l'inserimento della lavorazione! 2x");
            //            return;
            //        }
            //    }
            //}
            //catch (Exception Ex)
            //{
            //    log.Error(Ex.ToString());
            //}
        }

        public bool InsertAcqEnelEE62(string attivita, string numeroCliente, string dataAcqEnel) {

            var acqDb = new EE62_ACQDataContext();

            ACQ_ENEL_EE62 acqRow = new ACQ_ENEL_EE62 {
                RIFERIMENTO = attivita,
                NUMERO_CLIENTE = numeroCliente,
                DATA_ACQ_ENEL = dataAcqEnel
            };

            acqDb.ACQ_ENEL_EE62s.InsertOnSubmit(acqRow);

            try {
                acqDb.SubmitChanges();
            } catch (Exception) {
                //acqDb.GetChangeSet().Inserts.Clear();
                return false;
            }

            return true;
        }

        private bool SetTripletta(string CausaleContatto, string Descrizione, string Specifica)
        {
            try
            {
                if (!CompilaCampi("* Causale Contatto", CausaleContatto, Constant.TipoCampo.COMBO))
                {
                    if (!CompilaCampi(" Causale Contatto", CausaleContatto, Constant.TipoCampo.COMBO))
                        return false;
                }
                Thread.Sleep(1000);

                if (!CompilaCampi("* Descrizione", Descrizione, Constant.TipoCampo.COMBO))
                {
                    if (!CompilaCampi("* Description", Descrizione, Constant.TipoCampo.COMBO))
                        return false;
                }
                Thread.Sleep(1000);

                if (!CompilaCampi("* Specifica", Specifica, Constant.TipoCampo.COMBO))
                {
                    if (!CompilaCampi("* Specification", Specifica, Constant.TipoCampo.COMBO))
                        return false;
                }
                Thread.Sleep(1000);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetFieldValue(string label, string tipo = "", IWebElement elementoDiPartenza = null)
        {
            string result = "";

            switch (tipo)
            {
                default:
                    string value = null;
                    try
                    {
                        value = (string)((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab label.slds-form-element__label'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].value");
                        if (value != null) { result = value; break; }
                    }
                    catch { }
                    try
                    {
                        value = (string)((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab label.slds-form-element__label'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].innerHTML");
                        if (value != null) { result = value; break; }
                    }
                    catch { }
                    try
                    {
                        value = (string)((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab label.slds-form-element__label'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].children[0].value");
                        if (value != null) { result = value; break; }
                    }
                    catch { }
                    try
                    {
                        value = (string)((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab label.form-element__label'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.value");
                        if (value != null) { result = value; break; }
                    }
                    catch { }
                    try
                    {
                        value = (string)((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab')[0].getElementsByClassName('slds-form-element__label slds-no-flex'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].value");
                        if (value != null) { result = value; break; }
                    }
                    catch { }
                    try
                    {
                        value = (string)((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab')[0].getElementsByClassName('slds-form-element__label slds-no-flex'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].innerHTML");
                        if (value != null) { result = value; break; }
                    }
                    catch { }
                    try
                    {
                        value = (string)((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab')[0].getElementsByClassName('slds-form-element__label slds-no-flex'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].children[0].value");
                        if (value != null) { result = value; break; }
                    }
                    catch { }
                    try
                    {
                        value = (string)((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab')[0].getElementsByClassName('slds-form-element__label slds-no-flex'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.value");
                        if (value != null) { result = value; break; }
                    }
                    catch { }
                    break;
            }

            return result;
        }

        public bool HasActiveSubtab()
        {
            try
            {
                var val = Keanu.DriverExtra.FindElements(By.CssSelector("div"));
                foreach (var item in val)
                {
                    string sh = item.GetAttribute("class");
                    //if (sh.Equals("oneWorkspace active hasFixedFooter hasActiveSubtab oneWorkspace2"))
                    if (sh.Equals("oneWorkspace active hasFixedFooter hasActiveSubtab navexWorkspace") || sh.Equals("oneWorkspace active hasFixedFooter hasActiveSubtab oneWorkspace2"))
                        return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IWebElement GetSezioneAttiva()
        {
            IWebElement returnElement;
            if (HasActiveSubtab())
            {
                try//NEW
                {
                    returnElement = Keanu.DriverExtra.FindElement(By.XPath(".//div[@class = 'oneWorkspace active hasFixedFooter hasActiveSubtab navexWorkspace']//section[contains(@class, 'tabContent active oneConsoleTab')]"));
                }
                catch//OLD
                {
                    returnElement = Keanu.DriverExtra.FindElement(By.XPath(".//div[@class = 'oneWorkspace active hasFixedFooter hasActiveSubtab oneWorkspace2']//section[contains(@class, 'tabContent active oneConsoleTab')]"));
                }
            }
            else
            {
                try
                {//NEW
                    returnElement = Keanu.DriverExtra.FindElement(By.XPath(".//div[@class = 'oneWorkspace active hasFixedFooter navexWorkspace']//section[contains(@class, 'tabContent active oneConsoleTab')]"));
                }
                catch
                {//OLD
                    returnElement = Keanu.DriverExtra.FindElement(By.XPath(".//div[@class = 'oneWorkspace active hasFixedFooter oneWorkspace2']//section[contains(@class, 'tabContent active oneConsoleTab')]"));
                }
            }
            return returnElement;
        }

        public IWebElement ButtonExistByName(string textButton, string className = null, string tipo = "button", IWebElement elementoDiPartenza = null)
        {
            try
            {
                if (!Iframe)
                {
                    if (elementoDiPartenza == null)
                        elementoDiPartenza = GetSezioneAttiva();
                    if (className == null)
                        return elementoDiPartenza.FindElement(By.XPath(".//div[@class = 'slds-col--padded slds-no-flex slds-align-bottom']")).FindElement(By.XPath(".//button[normalize-space()= '" + textButton + "']"));
                    else
                        return elementoDiPartenza.FindElement(By.XPath(".//" + tipo + "[@class = '" + className + "' and normalize-space()= '" + textButton + "']"));
                }
                else
                {
                    if (className == null)
                    {
                        if (elementoDiPartenza == null)
                            return Keanu.DriverExtra.FindElement(By.XPath(".//div[@class = 'slds-col--padded slds-no-flex slds-align-bottom']")).FindElement(By.XPath(".//button[normalize-space()= '" + textButton + "']"));
                        else
                            return elementoDiPartenza.FindElement(By.XPath(".//div[@class = 'slds-col--padded slds-no-flex slds-align-bottom']")).FindElement(By.XPath(".//button[normalize-space()= '" + textButton + "']"));
                    }
                    else
                    {
                        if (elementoDiPartenza == null)
                            return Keanu.DriverExtra.FindElement(By.XPath(".//" + tipo + "[@class = '" + className + "' and normalize-space()= '" + textButton + "']"));
                        else
                            return elementoDiPartenza.FindElement(By.XPath(".//" + tipo + "[@class = '" + className + "' and normalize-space()= '" + textButton + "']"));
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public bool ClickButtonByName(string textButton, string className = null, string tipo = "button", IWebElement elementoDiPartenza = null)
        {
            log.Info($"Click su {textButton}");
            try
            {
                IWebElement button = null;
                int i = 0;
                while (button == null && i < 10)
                {
                    button = ButtonExistByName(textButton, className, tipo, elementoDiPartenza);
                    Thread.Sleep(Keanu.Randy(1));
                    i++;
                }
                if (button == null)
                {
                    log.Error($"Button {textButton} does not exist");
                    return false;
                }
                if (tipo.Equals("span"))
                    button.Click();
                else
                    ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("arguments[0].click();", button);
            }
            catch
            {
                log.Error($"Cannot click to {textButton}");
                return false;
            }
            Keanu.WaitingGameExtra();
            return true;
        }

        public bool CompilaCampi(string label, string value, TipoCampo tipologia, bool iframe = false, bool cercaSpan = false, IWebElement elementoDiPartenza = null)
        {
            try
            {
                IWebElement welabel = null;

                if (tipologia == TipoCampo.COMBO)
                    label = label.Trim();
                string addSpan = "";
                string puntoSpan = "";
                if (cercaSpan)
                {
                    addSpan = @"/span";
                    puntoSpan = ".";
                }

                if (elementoDiPartenza == null && !iframe)
                    elementoDiPartenza = GetSezioneAttiva();

                try
                {
                    if (elementoDiPartenza != null)
                        welabel = elementoDiPartenza.FindElements(By.XPath($".//label[contains(@class,'slds-form-element__label')]{addSpan}")).Where(box => box.Text.Equals(label)).ToList()[0];
                    else
                        welabel = Keanu.DriverExtra.FindElements(By.XPath($"//label[contains(@class,'slds-form-element__label')]{addSpan}")).Where(box => box.Text.Equals(label)).ToList()[0];
                }
                catch
                {
                    if (!iframe)
                        welabel = elementoDiPartenza.FindElements(By.XPath($".//label{addSpan}")).Where(box => box.Text.Equals(label)).ToList()[0];
                    else
                        welabel = Keanu.DriverExtra.FindElements(By.XPath($"//label{addSpan}")).Where(box => box.Text.Equals(label)).ToList()[0];
                }

                if (welabel != null)
                {
                    IWebElement weWrite = null;
                    switch (tipologia)
                    {
                        case TipoCampo.COMBO:
                            try
                            {
                                weWrite = welabel.FindElement(By.XPath($"{puntoSpan}.//following-sibling::div"));
                                weWrite = weWrite.FindElement(By.TagName("select"));// Modifica perchè ci potrebbero essere più div allora si va per tag name
                            }
                            catch
                            {
                                try
                                {
                                    weWrite = welabel.FindElement(By.XPath($"{puntoSpan}.//following-sibling::select"));
                                }
                                catch
                                {
                                    return false;
                                }
                            }
                            if (weWrite != null)
                            {
                                weWrite.FindElement(By.XPath(".//option[@value = \"" + value + "\"]")).Click();
                                log.Info("Il campo " + label + " è stato valorizzato correttamente");
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        case TipoCampo.INPUT:
                            try
                            {
                                try
                                {
                                    weWrite = welabel.FindElement(By.XPath(".//following-sibling::div"));
                                    weWrite = weWrite.FindElement(By.TagName("input"));//Modifica perchè ci potrebbero essere più div alora si va per tag name
                                }
                                catch
                                {
                                    weWrite = welabel.FindElement(By.XPath(".//following-sibling::input"));
                                }
                            }
                            catch
                            {
                                return false;
                            }
                            if (weWrite != null)
                            {
                                weWrite.SendKeys(Keys.Enter);
                                weWrite.Clear();
                                Thread.Sleep(1000);
                                weWrite.Clear();
                                weWrite.SendKeys(value);
                                weWrite.SendKeys(Keys.Tab);
                                log.Info("Il campo " + label + " è stato valorizzato correttamente");
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                    }
                    return false;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private void Terminator()
        {
            if (Keanu.ModificaPodTrip)
            {

                try {
                    //First try to close att in geo
                    var buttonTermina = Keanu.Driver.FindElement(By.CssSelector("button[type='SUBMIT']"));
                    buttonTermina.Click();
                    Thread.Sleep(Keanu.Randy(3));
                    if (Keanu.Driver.PageSource.Contains("rossimo Task")) {
                        return;
                    } else {

                        Actions closingAction = new Actions(Keanu.Driver);
                        closingAction.SendKeys(Keys.Escape).Build().Perform();
                        Thread.Sleep(Keanu.Randy(2));

                        try {
                            ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("document.querySelectorAll(\"div[class *= 'slds-context-bar__secondary navCenter tabBarContainer'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                            Thread.Sleep(500);
                            ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("document.querySelectorAll(\"div[class *= 'slds-context-bar__secondary navCenter tabBarContainer'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                            Thread.Sleep(500);
                            ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("document.querySelectorAll(\"div[class *= 'oneGlobalNav oneConsoleNav'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                            Thread.Sleep(500);
                            ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("document.querySelectorAll(\"div[class *= 'oneGlobalNav oneConsoleNav'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                            Thread.Sleep(500);
                        } catch { }

                        Thread.Sleep(Keanu.Randy(1));

                        Keanu.NavigatoreExtra(Keanu.IdAttivitaPerScodamento);
                        Keanu.WaitingGameExtra();

                        IJavaScriptExecutor js = (IJavaScriptExecutor)Keanu.DriverExtra;
                        js.ExecuteScript("window.scrollBy(0,-1000)", "");
                        Thread.Sleep(500);

                        js.ExecuteScript("window.scrollBy(0,-1000)", "");
                        Thread.Sleep(500);

                        ClickButtonByName("Modifica");

                        Thread.Sleep(Keanu.Randy(1));

                        string pod = "99999999999999";
                        string podSfa = GetFieldValue("* POD");
                        if (string.IsNullOrEmpty(podSfa)) {
                            bool esito = CompilaCampi("* POD", pod, Constant.TipoCampo.INPUT);
                            if (!esito) { log.Error("Problemi nell'inserire il pod"); }
                            log.Info("Pod inserito");
                            Thread.Sleep(Keanu.Randy(1));
                        } else {
                            bool esito = CompilaCampi("* POD", podSfa, Constant.TipoCampo.INPUT);
                            if (!esito) { log.Error("Problemi nell'inserire il pod"); }
                            //log.Info("Pod inserito");
                            Thread.Sleep(Keanu.Randy(1));
                            log.Info("Pod already present");
                        }

                        string specifica = GetFieldValue("Specifica");
                        if (string.IsNullOrEmpty(specifica)) {
                            if (Keanu.LavName.Equals("SCODAMENTO - EE62 - SMART LETTONIA")) {
                                if (!SetTripletta("Gestione Clienti", "Credito", @"Dimostrato Pagamento")) {
                                    log.Warn($"SetTripletta() fail");
                                }
                            } else {
                                if (!SetTripletta(@"N/A", @"N/A", @"N/A")) {
                                    log.Warn($"SetTripletta() fail");
                                }
                            }
                        } else {
                            log.Info("Trip already present");
                        }

                        if (!string.IsNullOrEmpty(podSfa) && !string.IsNullOrEmpty(specifica)) {
                            log.Info("Nothing to modify, delete cliente and set stato IN LAVORAZIONE");
                            try {
                                if (Keanu.DriverExtra.FindElement(By.Id("customerLookUp")).GetAttribute("value").Length > 1) {
                                    var cl = Keanu.DriverExtra.FindElement(By.Id("customerLookUp"));
                                    var del = cl.FindElement(By.XPath("..//span[@title= 'Click to delete value...']"));
                                    Actions a = new Actions(Keanu.DriverExtra);
                                    a.MoveToElement(cl).Perform();
                                    Thread.Sleep(500);
                                    a.Click(del).Perform();
                                }
                            } catch { }

                            if (!CompilaCampi("* Stato", "IN LAVORAZIONE", TipoCampo.COMBO, false, false)) {
                                if (!CompilaCampi("* Status", "IN LAVORAZIONE", TipoCampo.COMBO, false, false)) {
                                    log.Warn("Problemi nell'impostare lo stato in FATTO");
                                }
                            }
                        }

                        if (!ClickButtonByName("Salva")) {
                            log.Warn($"ClickButtonByName() fail");
                        }

                        int cnt = 0;
                        var btnTermina = Keanu.Driver.FindElement(By.CssSelector("button[type='SUBMIT']"));
                        btnTermina.Click();
                        while (!Keanu.Driver.PageSource.Contains("rossimo Task")) {
                            try {
                                if (cnt == 5) {
                                    cnt = 0;

                                    try {
                                        Actions a = new Actions(Keanu.Driver);
                                        a.SendKeys(Keys.Escape).Build().Perform();
                                        Thread.Sleep(Keanu.Randy(2));
                                        btnTermina = Keanu.Driver.FindElement(By.CssSelector("button[type='SUBMIT']"));
                                        btnTermina.Click();
                                        Thread.Sleep(Keanu.Randy(2));
                                    } catch {
                                        Actions b = new Actions(Keanu.Driver);
                                        b.SendKeys(Keys.Escape).Build().Perform();
                                        Thread.Sleep(Keanu.Randy(3));
                                        btnTermina = Keanu.Driver.FindElement(By.CssSelector("button[type='SUBMIT']"));
                                        btnTermina.Click();
                                        Thread.Sleep(Keanu.Randy(3));
                                    }

                                    #region
                                    if (!Keanu.Driver.PageSource.Contains("rossimo Task")) {
                                        if (Keanu.DriverExtra == null) {
                                            log.Info($"Open SFA");
                                            if (!Keanu.PepperYourChromeExtra(Keanu.LoginSFA, Keanu.PassSFA, "https://enelcrmt.my.salesforce.com/"))
                                                try { Keanu.DriverExtra.Close(); } catch { throw; }
                                        }
                                        if (!Keanu.IdAttivitaPerScodamento.Equals(v.OldIdAttivitaPerScodamento)) {
                                            try {
                                                ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("document.querySelectorAll(\"div[class *= 'slds-context-bar__secondary navCenter tabBarContainer'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                                                Thread.Sleep(500);
                                                ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("document.querySelectorAll(\"div[class *= 'slds-context-bar__secondary navCenter tabBarContainer'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                                                Thread.Sleep(500);
                                                ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("document.querySelectorAll(\"div[class *= 'oneGlobalNav oneConsoleNav'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                                                Thread.Sleep(500);
                                                ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("document.querySelectorAll(\"div[class *= 'oneGlobalNav oneConsoleNav'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                                                Thread.Sleep(500);
                                            } catch { }

                                            Thread.Sleep(Keanu.Randy(1));

                                            try {
                                                log.Info($"Open {Keanu.IdAttivitaPerScodamento}");
                                                log.Error($"Modify manually");
                                                //if (query.Mark($"pls help on {Keanu.LoginAGENTE}, modifica").Equals(-1))
                                                //    log.Info("Oh, hi Mark");
                                                Keanu.NavigatoreExtra(Keanu.IdAttivitaPerScodamento);
                                                v.OldIdAttivitaPerScodamento = Keanu.IdAttivitaPerScodamento;
                                                cnt = 0;
                                            } catch {
                                                
                                            }
                                        }
                                    }
                                    #endregion

                                }
                            } catch {
                                //log.Info("Continue");
                                Thread.Sleep(Keanu.Randy(5));
                                cnt = 0;
                                continue;
                            }

                            Thread.Sleep(Keanu.Randy(1));
                            cnt++;

                        }

                    }

                } catch {
                    Keanu.Erry();
                }


                

            }
            else
            {
                int cnt = 0;
                //var btnTermina = Keanu.Driver.FindElement(By.XPath("//button"));
                try {
                    var btnTermina = Keanu.Driver.FindElement(By.CssSelector("button[type='SUBMIT']"));
                    btnTermina.Click();
                    while (!Keanu.Driver.PageSource.Contains("rossimo Task")) {
                        try {
                            if (cnt == 5) {
                                cnt = 0;

                                try {
                                    Actions a = new Actions(Keanu.Driver);
                                    a.SendKeys(Keys.Escape).Build().Perform();
                                    Thread.Sleep(Keanu.Randy(2));
                                    btnTermina = Keanu.Driver.FindElement(By.CssSelector("button[type='SUBMIT']"));
                                    btnTermina.Click();
                                    Thread.Sleep(Keanu.Randy(2));
                                } catch {
                                    Actions b = new Actions(Keanu.Driver);
                                    b.SendKeys(Keys.Escape).Build().Perform();
                                    Thread.Sleep(Keanu.Randy(3));
                                    btnTermina = Keanu.Driver.FindElement(By.CssSelector("button[type='SUBMIT']"));
                                    btnTermina.Click();
                                    Thread.Sleep(Keanu.Randy(3));
                                }

                                #region
                                if (!Keanu.Driver.PageSource.Contains("rossimo Task")) {
                                    if (Keanu.DriverExtra == null) {
                                        log.Info($"Open SFA");
                                        if (!Keanu.PepperYourChromeExtra(Keanu.LoginSFA, Keanu.PassSFA, "https://enelcrmt.my.salesforce.com/"))
                                            try { Keanu.DriverExtra.Close(); } catch { throw; }
                                    }
                                    if (!Keanu.IdAttivitaPerScodamento.Equals(v.OldIdAttivitaPerScodamento)) {
                                        try {
                                            ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("document.querySelectorAll(\"div[class *= 'slds-context-bar__secondary navCenter tabBarContainer'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                                            Thread.Sleep(500);
                                            ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("document.querySelectorAll(\"div[class *= 'slds-context-bar__secondary navCenter tabBarContainer'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                                            Thread.Sleep(500);
                                            ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("document.querySelectorAll(\"div[class *= 'oneGlobalNav oneConsoleNav'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                                            Thread.Sleep(500);
                                            ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("document.querySelectorAll(\"div[class *= 'oneGlobalNav oneConsoleNav'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                                            Thread.Sleep(500);
                                        } catch { }

                                        Thread.Sleep(Keanu.Randy(1));

                                        try {
                                            log.Info($"Open {Keanu.IdAttivitaPerScodamento}");
                                            log.Error($"Modify manually");
                                            //if (query.Mark($"pls help on {Keanu.LoginAGENTE}, modifica").Equals(-1))
                                            //    log.Info("Oh, hi Mark");
                                            Keanu.NavigatoreExtra(Keanu.IdAttivitaPerScodamento);
                                            v.OldIdAttivitaPerScodamento = Keanu.IdAttivitaPerScodamento;
                                            cnt = 0;
                                        } catch {
                                            Keanu.DriverExtra.Close();
                                            log.Error($"GoToUrl fail");
                                        }
                                    }
                                }
                                #endregion

                            }
                        } catch {
                            //log.Info("Continue");
                            Thread.Sleep(Keanu.Randy(5));
                            cnt = 0;
                            continue;
                        }
                        log.Info($"Closing SFChrome");
                        Keanu.DriverExtra.Close();
                        Thread.Sleep(Keanu.Randy(1));
                        cnt++;
                    }
                } catch (Exception) {
                    Keanu.Erry();
                }
            }
        }

        private int Aspetter(int inCoda)
        {
            log.Info($"Aspetta, nel codice {inCoda}");
            Thread.Sleep(Keanu.Randy(333));
            RestratAll();
            inCoda = query.GetDaLavorareByIdDettaglioTipoLavorazione(Keanu.IdRemainingCheck);
            return inCoda;
        }



        public bool DownloadPDF(string pdf, string mercato) {
            try {

                /*if (Keanu.DriverExtra != null)
                {
                    Keanu.DriverExtra.Close();
                }
                else { }*/
                

         
                string filePath = "";
                string uri = "";
                

                if (pdf.StartsWith("SDPL") || pdf.StartsWith("SDPM") || pdf.StartsWith("SMPL") || pdf.StartsWith("SWPL") || pdf.StartsWith("STPM") || pdf.StartsWith("STPL") || pdf.StartsWith("SCPL") || pdf.StartsWith("SCPM")|| pdf.StartsWith("STPM") || pdf.StartsWith("SMPM") || pdf.StartsWith("SCPM") || pdf.StartsWith("SWPM")) //Shitie kacha specialos SFPL dokus + jaipieliek pie 5in1IT/5inLV
                {
                    if (Keanu.DriverExtra == null) {
                        if (!Keanu.PepperYourChromeExtra(Keanu.LoginSFA, Keanu.PassSFA, "https://enelcrmt.my.salesforce.com/"))
                            return false;
                    }

                    string desktopPdf = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + pdf + ".pdf";
                    string documentsPdf = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + pdf + ".pdf";
                    filePath = @"\\172.23.0.91\applicazioni\EE145_Maschera_SFA\PDF\" + pdf + ".pdf";

                    AutoItX.AutoItSetOption("WinTitleMatchMode", 2);

                    Thread.Sleep(Keanu.Randy(1));

                    if (AutoItX.WinExists("Save As") == 1) {
                        AutoItX.WinActivate("Save As");
                        AutoItX.Send("{ESC}");
                        log.Error($"Save As window already opened ");
                        return false;
                    }

                    if (AutoItX.WinExists("Confirm Save As") == 1) {
                        AutoItX.WinActivate("Confirm Save As");
                        AutoItX.Send("!n");
                        Thread.Sleep(Keanu.Randy(3));
                        AutoItX.WinActivate("Save As");
                        AutoItX.Send("{ESC}");
                        log.Error($"Confirm Save As window already opened ");
                        return false;
                    }
                    
                    Keanu.DriverExtra.Navigate().GoToUrl(@"https://enelcrmt.lightning.force.com/c/ITA_IFM_LAP021_RetrieveDocuments.app?system=MARI&recordId=" + pdf);
                    Thread.Sleep(Keanu.Randy(2));
                    
                    try {

                        int errorCounter = 0;
                        while (AutoItX.WinActive("Save As") == 0) {
                            Thread.Sleep(Keanu.Randy(3));
                            errorCounter++;
                            if (Keanu.DriverExtra.PageSource.ToString().Contains("Errore nel caricamento del file") || Keanu.DriverExtra.PageSource.ToString().Contains("Errore : 500 Server Error")) {
                                //Keanu.Erny();
                                return false;
                            }
                            if(errorCounter > 15) {
                                return false;
                            }
                            AutoItX.WinActivate("Save As");
                        }
                        Thread.Sleep(Keanu.Randy(1));
                        //AutoIt.AutoItX.GetCon
                        AutoIt.AutoItX.ControlSetText("Save As", "", "[CLASS:Edit;INSTANCE:1]", desktopPdf);
                        Thread.Sleep(Keanu.Randy(1));

                        AutoItX.WinActivate("Save As");
                        Thread.Sleep(Keanu.Randy(1));
                        AutoItX.Send("!s");
                        Thread.Sleep(Keanu.Randy(3));
                        //AutoIt.AutoItX.ControlClick("Save As", "", "[CLASS:Button;INSTANCE:2]", "left", 1);
                        //AutoItX.Send("{ENTER}");

                        if (AutoItX.WinExists("Save As") == 1) {
                            AutoItX.WinActivate("Save As");
                            AutoItX.Send("{ESC}");
                            log.Error($"Save As window already opened after");
                            return false;
                        }

                        if (AutoItX.WinExists("Confirm Save As") == 1) {
                            AutoItX.WinActivate("Confirm Save As");
                            AutoItX.Send("!n");
                            Thread.Sleep(Keanu.Randy(1));
                            AutoItX.WinActivate("Save As");
                            AutoItX.Send("{ESC}");
                            log.Error($"Confirm Save As window already opened after");
                            return false;
                        }

                        Thread.Sleep(Keanu.Randy(2));
                        
                    } catch (Exception Ex) {
                        log.Error(Ex);
                        return false;
                    }

                    try {
                        if(Keanu.LavName.Equals("SCODAMENTO - EE145 - DATI CATASTALI")) {
                            if (File.Exists(desktopPdf)) {
                                File.Copy(desktopPdf, filePath, true);
                                var sfapdfPath = @"\\BALTIKPDC\sfapdf\" + pdf + ".pdf";
                                File.Copy(desktopPdf, sfapdfPath, true);
                                Thread.Sleep(Keanu.Randy(3));
                                File.Delete(desktopPdf);
                            } else if (File.Exists(documentsPdf)) {
                                File.Copy(documentsPdf, filePath, true);
                                Thread.Sleep(Keanu.Randy(3));
                                File.Delete(documentsPdf);
                                
                            } else {
                                log.Error($"{pdf} wrong path");
                                return false;
                            }
                        } else {
                            if (File.Exists(desktopPdf)) {
                                filePath = @"\\BALTIKPDC\sfapdf\" + pdf + ".pdf";
                                File.Copy(desktopPdf, filePath, true);
                                Thread.Sleep(Keanu.Randy(3));
                                File.Delete(desktopPdf);
                                
                            } else {
                                log.Error($"{pdf} wrong path");
                                return false;
                            }

                            

                        }
                        
                    } catch (Exception Ex) {
                        log.Error(Ex);
                        return false;
                    }

                    long length = new FileInfo(filePath).Length;

                    if (length < 1666) {
                        File.Delete(filePath);
                        log.Error($"{pdf} size < 2KB");

                        return false;
                    }

                    //log.Info($"{pdf} downloaded");
                    return true;
                } else {
                    if (pdf.Length > 30) {
                        uri = mercato + pdf;        
                        filePath = @"\\BALTIKPDC\sfapdf\" + pdf + ".pdf";
                    } else {
                        uri = mercato + pdf.Substring(0, 12);
                        filePath = @"\\BALTIKPDC\sfapdf\" + pdf.Substring(0, 12) + ".pdf";
                    }

                    Uri fileLink = new Uri(uri);

                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(fileLink, filePath);

                    long length = new FileInfo(filePath).Length;

                    if (length < 1666) {
                        File.Delete(filePath);
                        log.Error($"{pdf} size < 2KB");

                        return false;
                    }

                    //log.Info($"{pdf} downloaded");
                    return true;
                }
            } catch (Exception Ex) {
                log.Error($"{pdf} (404) Not Found.");
                //notFound = true;
                
                return false;
            }
        }

    }
}