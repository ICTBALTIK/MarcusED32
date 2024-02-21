using AgenteHelperLibrary.ScaSrv;
using log4net;
using MARCUS.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SFALibrary;
using SFALibrary.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using static MARCUS.Helpers.Constant;

namespace MARCUS.Marcosi
{
    class EE176 {
        private static readonly ILog log = LogManager.GetLogger(typeof(EE176));
        public Keanu Keanu { get; set; }
        public SqlQuery query = new SqlQuery();

        public EE176(Keanu keanu) {
            this.Keanu = keanu;
        }

        public SfaLib sfaLib { get; set; }

        private InfoCascata riferimentoCorrente;

        public Records.VariablesEE176 v = null;

        public bool Iframe { get; set; }

        private int numScartiConsecutivi = 0;
        private int maxScartiConsecutivi = 50;
        private int cnt = 0;
        int resoDaFare;
        public bool atlestOneAttivitaFilled = false; //Bool kas liecina ka vismas viens no doc ref ir apstradats kad tie ir vairak par 1
        public bool Flow()
        {
            Keanu.KillChromeWebDriver();
            try
            {
                //TODO remove then check for 17330
                if (Keanu.LavLoginId == Costanti.CODA_EE176_MACRO1 || Keanu.LavLoginId == Costanti.CODA_EE176_MACRO2)
                {
                    if (!Keanu.PepperYourChrome(Keanu.LoginSFA, Keanu.PassSFA, "https://enelcrmt.my.salesforce.com/", "", true))
                        return false;
                    if (!PepperYourSfaLib())
                        return false;
                    UnlockAndCloseAllTabs();
                }

                DateTime dataDaConfrontare = new DateTime(2014, 12, 1);

                while (true)
                {
                    if (Keanu.StartStop == false)
                        break;

                    v = new Records.VariablesEE176();

                    Thread.Sleep(Keanu.Randy(2));
                    riferimentoCorrente = Keanu.Agente.NewInfoCascataToWork(-1);
                    //For testing
                    //riferimentoCorrente = new InfoCascata();
                    //riferimentoCorrente.NUMERO_CLIENTE = "SDPMJ0109214";
                    //riferimentoCorrente.DATA_RIC_ACQ_DOC = DateTime.Now.ToString();
                    //riferimentoCorrente.DETTAGLIO = "SDPMJ0109214";


                    if (riferimentoCorrente == null)
                    {
                        log.Info("Finished");
                        break;
                    }
                    log.Info($"Numero cliente : {riferimentoCorrente.NUMERO_CLIENTE}");

                    DateTime dataAcquisizione = DateTime.MinValue;
                    try
                    {
                        dataAcquisizione = Convert.ToDateTime(riferimentoCorrente.DATA_RIC_ACQ_DOC);
                    }
                    catch { }

                    bool flag11 = Keanu.LavLoginId == Costanti.CODA_EE176_ALTRI_MACRO1 && dataAcquisizione >= dataDaConfrontare;
                    if (flag11)
                    {
                        Keanu.Bad.Scarto++;
                        string numeroCliente = riferimentoCorrente.NUMERO_CLIENTE.Trim();
                        Keanu.Agente.RegistraCompleto(riferimentoCorrente.DETTAGLIO.Trim(), numeroCliente, riferimentoCorrente.DATA_RIC_ACQ_DOC.Trim(), Costanti.EE176_MACRO1_ALTRI_RESO_DICEMBRE, string.Concat(Costanti.EE176_MACRO1_ALTRI_RESO_DICEMBRE), "", "", 1, false, true, true);
                    }
                    else
                    {
                        #region ANTI-SCADUTA
                        cnt++;
                        if (cnt.Equals(75))
                        {
                            if (!sfaLib.SessionExpired())
                            {
                                if (!PepperYourSfaLib())
                                    return false;
                            }
                            Keanu.Driver.Navigate().Refresh();
                            Keanu.WaitingGame();
                            Keanu.WaitingGame();
                            cnt = 0;
                        }
                        #endregion

                        string contrattoOrdine = "";
                        //string barcode = query.GetBarcode(riferimentoCorrente.NOTE).Trim();
                        string barcode = query.GetBarcode(riferimentoCorrente.NUMERO_CLIENTE).Trim();
                        if (string.IsNullOrEmpty(barcode))
                        {
                            barcode=query.GetBarcode(riferimentoCorrente.DETTAGLIO).Trim();
                        }
                        log.Info($"BARCODE1: {barcode}");
                        bool flag12 = barcode.Equals("");
                        if (flag12)
                        {
                            barcode = query.GetDataMatrixRicDocumento(riferimentoCorrente.NUMERO_CLIENTE).Trim();
                            //barcode = query.GetDataMatrixRicDocumento(riferimentoCorrente.NOTE).Trim();
                            bool flag13 = barcode.Equals("");
                            log.Info($"BARCODE2: {barcode}");
                            if (flag13)
                            {
                                barcode = query.GetDataMatrixRicDocumento2(riferimentoCorrente.NUMERO_CLIENTE).Trim();
                                //barcode = query.GetDataMatrixRicDocumento2(riferimentoCorrente.NOTE).Trim();
                                log.Info($"BARCODE3: {barcode}");
                            }
                            bool flag14 = !barcode.Equals("");
                            if (flag14)
                            {
                                string[] barcodeSplittato = barcode.Split(new char[] { '*' });
                                string[] array = barcodeSplittato;
                                for (int i = 0; i < array.Length; i++)
                                {
                                    string s = array[i];
                                    bool flag15 = s.StartsWith("C20");
                                    //bool flag15 = s.StartsWith("X20");
                                    if (flag15)
                                    {
                                        contrattoOrdine = s;
                                        break;
                                    }
                                }
                                bool flag16 = !contrattoOrdine.Equals("");
                                if (flag16)
                                {
                                    bool flag17 = contrattoOrdine.Contains("2-");
                                    if (flag17)
                                        contrattoOrdine = contrattoOrdine.Substring(contrattoOrdine.IndexOf("2-"), 9);
                                }
                            }
                        }
                        else
                        {
                            string[] barcodeSplittato2 = barcode.Split(new char[] { ';' });
                            string[] array2 = barcodeSplittato2;
                            for (int j = 0; j < array2.Length; j++)
                            {
                                string s2 = array2[j];
                                bool flag18 = s2.Contains("DataMatrix") && s2.Length > 20 && s2.Contains("2-");
                                if (flag18)
                                {
                                    contrattoOrdine = s2;
                                    break;
                                }
                                bool flag19 = s2.StartsWith("018") && s2.Length == 16;
                                if (flag19)
                                {
                                    contrattoOrdine = s2;
                                    break;
                                }
                            }
                            bool flag20 = !contrattoOrdine.Equals("");
                            if (flag20)
                            {
                                bool flag21 = contrattoOrdine.Contains("2-");
                                if (flag21)
                                    contrattoOrdine = contrattoOrdine.Substring(contrattoOrdine.IndexOf("2-"), 9);
                                else
                                    contrattoOrdine = contrattoOrdine.Substring(7);
                            }
                        }
                        bool flag22 = contrattoOrdine.Equals("");
                        if (flag22)
                        {
                            bool flag23 = Keanu.LavLoginId == Costanti.CODA_EE176_MACRO1 || Keanu.LavLoginId == Costanti.CODA_EE176_ALTRI_MACRO1;
                            if (flag23)
                                resoDaFare = Keanu.ResoRecuperoDati;
                            else
                                resoDaFare = Keanu.LavScartoId;
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            int numJolly = 0;
                            string text = contrattoOrdine;
                            for (int k = 0; k < text.Length; k++)
                            {
                                char c = text[k];
                                bool flag24 = (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '-';
                                if (flag24)
                                    sb.Append(c);
                                else
                                {
                                    sb.Append('?');
                                    numJolly++;
                                }
                            }
                            string contrattoOrdinePulito = sb.ToString();
                            bool flag25 = numJolly > 1;
                            if (flag25)
                            {
                                bool flag26 = Keanu.LavLoginId == Costanti.CODA_EE176_MACRO1 || Keanu.LavLoginId == Costanti.CODA_EE176_ALTRI_MACRO1;
                                if (flag26)
                                    resoDaFare = Keanu.ResoRecuperoDati;
                                else
                                    resoDaFare = Keanu.LavScartoId;
                                log.Info("SFA LIB FAIL");

                            }
                            else
                                resoDaFare = Workaholic(riferimentoCorrente, contrattoOrdinePulito);
                                
                        }

                        if(resoDaFare == 777) {
                            //puts doc in sospesos
                            Keanu.Bad.Sospeso++;
                            log.Info($"Sospeso for now because 2 or more att with type DMS - DOC DA CORRELARE");
                            Keanu.Agente.SospendiInfoCascataCorrente(1);
                            continue;
                        }

                        if (resoDaFare == 888) {
                            //puts doc in sospesos
                            Keanu.Bad.Sospeso++;
                            log.Info($"Sospeso for now because there was an error with new added code related to check document ref count");
                            Keanu.Agente.SospendiInfoCascataCorrente(4);
                            continue;
                        }

                        if (!Regsrhhtrn(resoDaFare))
                            break;
                    }
                }
            }
            catch
            {
                log.Info($"Flow() fail");
                Regsrhhtrn(resoDaFare);
            }

            Keanu.KillChromeWebDriver();
            Keanu.Agente.Logout();
            return false;
        }

        private bool Regsrhhtrn(int resoDaFare)
        {
            bool flag27 = resoDaFare == Costanti.EE176_MACRO1_RESO_OK || resoDaFare == Costanti.EE176_MACRO2_RESO_OK || resoDaFare == Costanti.EE176_MACRO1_ALTRI_RESO_OK || resoDaFare == Costanti.EE176_MACRO2_ALTRI_RESO_OK;
            if (flag27)//FATTO
            {
                Keanu.Bad.Fatto++;
                string numeroCliente = v.AttivitaSfa;
                numScartiConsecutivi = 0;

                log.Info($"{riferimentoCorrente.DETTAGLIO} Fatto");
                Keanu.Agente.RegistraCompleto(riferimentoCorrente.DETTAGLIO.Trim(), numeroCliente, riferimentoCorrente.DATA_RIC_ACQ_DOC.Trim(), resoDaFare, string.Concat(resoDaFare), "", "", 1, false, true, true);
                return true;
            }
            else if (resoDaFare == Costanti.EE176_MACRO1_RESO_RECUPERO_DATI)//SCARTO C1 recupero dati
            {
                
                log.Info($"{riferimentoCorrente.DETTAGLIO} scarto");
                Keanu.Bad.Scarto++;
                string numeroCliente = riferimentoCorrente.NUMERO_CLIENTE.Trim();
                Keanu.Agente.RegistraCompleto(riferimentoCorrente.DETTAGLIO.Trim(), numeroCliente, riferimentoCorrente.DATA_RIC_ACQ_DOC.Trim(), Costanti.EE176_MACRO1_RESO_RECUPERO_DATI, string.Concat(Costanti.EE176_MACRO1_RESO_RECUPERO_DATI), "", "", 1, false, true, true);
                return true;
            }
            else if (resoDaFare == Costanti.EE176_MACRO1_RESO_LAV_MANUALE)//SCARTO C1 lamanuale
            {
                log.Info($"{riferimentoCorrente.DETTAGLIO} scarto");
                Keanu.Bad.Scarto++;
                string numeroCliente = riferimentoCorrente.NUMERO_CLIENTE.Trim();
                numScartiConsecutivi++;
                Keanu.Agente.RegistraCompleto(riferimentoCorrente.DETTAGLIO.Trim(), numeroCliente, riferimentoCorrente.DATA_RIC_ACQ_DOC.Trim(), resoDaFare, string.Concat(resoDaFare), "", "", 1, false, true, true);
                bool flag28 = numScartiConsecutivi > maxScartiConsecutivi;
                if (flag28)
                {
                    numScartiConsecutivi = 0;
                    log.Error("TOO MUCH SCARTO");
                    return false;
                }
                return true;
            }
            else if (resoDaFare == Costanti.EE176_MACRO2_RESO_LAV_MANUALE)//SCARTO C2 lamanuale
            {
                log.Info($"{riferimentoCorrente.DETTAGLIO} scarto");
                Keanu.Bad.Scarto++;
                string numeroCliente = riferimentoCorrente.NUMERO_CLIENTE.Trim();
                numScartiConsecutivi++;

                Keanu.Agente.RegistraCompleto(riferimentoCorrente.DETTAGLIO.Trim(), numeroCliente, riferimentoCorrente.DATA_RIC_ACQ_DOC.Trim(), resoDaFare, string.Concat(resoDaFare), "", "", 1, false, true, true);
                bool flag28 = numScartiConsecutivi > 15;
                if (flag28)
                {
                    numScartiConsecutivi = 0;
                    log.Error("TOO MUCH SCARTO");
                    return false;
                }
                return true;
            }
            else//SCARTO
            {
                log.Info($"{riferimentoCorrente.DETTAGLIO} scarto");
                Keanu.Bad.Scarto++;
                string numeroCliente = riferimentoCorrente.NUMERO_CLIENTE.Trim();
                numScartiConsecutivi++;
                Keanu.Agente.RegistraCompleto(riferimentoCorrente.DETTAGLIO.Trim(), numeroCliente, riferimentoCorrente.DATA_RIC_ACQ_DOC.Trim(), resoDaFare, string.Concat(resoDaFare), "", "", 1, false, true, true);
                bool flag28 = numScartiConsecutivi > maxScartiConsecutivi;
                if (flag28)
                {
                    numScartiConsecutivi = 0;
                    log.Error("TOO MUCH SCARTO");
                    return false;
                }
                return true;
            }
            return true;
        }

        private int Workaholic(InfoCascata nuovo, string offerta)
        {
            string dettaglio = nuovo.DETTAGLIO.Trim().Substring(0, 12);
            int reso;

            try
            {
                if (offerta.Equals("999999999"))
                {
                    log.Debug("999999999");
                    reso = Keanu.LavScartoId;
                    return reso;
                }

                Offerta offertaSfa = new Offerta();
                ServizioEBene utenzaSfa = new ServizioEBene();
                DatiCliente dati = new DatiCliente();
                SFALibrary.Model.Ricerca.RicercaCliente ricerca = new SFALibrary.Model.Ricerca.RicercaCliente(true, true, false, true);

                if (!sfaLib.RicercaAnagraficaCliente(offerta, "", out offertaSfa, out utenzaSfa, out string errore))
                {
                    log.Debug($"Nessun risultato per {offerta} in Offerte");
                    reso = Keanu.LavScartoId;
                    return reso;
                }

                string idCliente = offertaSfa.ClienteRecordId;

                try
                {
                    dati = (DatiCliente)sfaLib.GetDatiClienti(idCliente, ricerca);
                }
                catch
                {
                    reso = Keanu.LavScartoId;
                    return reso;
                }

                if (dati == null)
                {
                    reso = Keanu.LavScartoId;
                    return reso;
                }

                var listaDocss = (Documento)sfaLib.SearchGenerico(dettaglio, SFALibrary.Helpers.Utility.RicercaDocumentoName);
                List<Attivita> tempAtt = new List<Attivita>();
                tempAtt = (List<Attivita>)sfaLib.GetRelatedListItemsNew(listaDocss.RecordId, SFALibrary.Helpers.Utility.AttivitàItemName, SFALibrary.Helpers.Utility.AttivitàApiName, "Attività");


                //MI0004R Matricola is working

                UnlockAndCloseAllTabs();

                //if (!RicercaDocumenti(dettaglio)) {
                 if (!RicercaDocumenti(dettaglio)) {
                    log.Warn("RicercaDocumenti() fail");
                    reso = 888;
                    return reso;
                }

                //Need to read whole table and get data

                List<string> documentRefList = new List<string>();

                try {
                    Keanu.WaitingGame();
                    SwitchToDefaultContent();
                    IWebElement tableResult = null;
                    tableResult = TrovaTableResult();
                    if (tableResult == null) {
                        reso = 888;
                        return reso;
                    }
                        
                    var tableHeaders = tableResult.FindElements(By.XPath("./thead/tr/th"));
                    int indexDocumentRef = tableHeaders.IndexOf(tableHeaders.Where(q => q.Text.Contains("Document Ref")).First());
                    IList<IWebElement> documentRefRows = tableResult.FindElements(By.XPath("./tbody/tr"));
                    int rows = documentRefRows.Count();

                    
                    if(rows >= 2) {
                        //Looks like there is two Document refs for one doc, need to change every att
                        //return 777;
                    }

                    


                    for (int i = 1; i <= rows; i++) {

                        string documentRef = tableResult.FindElement(By.XPath("./tbody/tr[" + i + "]")).FindElements(By.XPath("./*")).ElementAt(indexDocumentRef).Text;
                        documentRefList.Add(documentRef);
                        
                    }
                    }catch {

                    return 888;
                }


                bool isAttivitaFilled = true;

                if(documentRefList.Count == 1) {
                    //Fill only one att
                    UnlockAndCloseAllTabs();

                    if(FillSoloAttivita(dettaglio, dati)) {
                        log.Debug($"Fill solo attivita success");
                        reso = Keanu.LavRegId;
                        return reso;
                    } else {
                        log.Debug("Fill solo attivita fail");
                        reso = Keanu.LavScartoId;
                        return reso;
                    }

                }


                //Fill every att in list
                


                foreach (string documentRef in documentRefList) {

                    UnlockAndCloseAllTabs();
                    
                    Documento documento = (Documento)sfaLib.SearchGenerico(documentRef, SFALibrary.Helpers.Utility.RicercaDocumentoName);
                    if(documento == null) {
                        //Can't find document, should scarto but for now sospeso
                        isAttivitaFilled = false;
                        break;
                    }

                    string recordId = documento.RecordId;
                    if (recordId == null) {
                        //Can't find document, should scarto but for now sospeso
                        isAttivitaFilled = false;
                        break;
                    }

                    if (!FillAttivita(recordId, dati)) {
                        isAttivitaFilled = false;
                        break;
                    }

                }

                if (atlestOneAttivitaFilled == false) {
                    log.Debug("No Attivitas was filled - scarto");
                    reso = Keanu.LavScartoId;
                    return reso;
                }

                if(isAttivitaFilled == true) {
                    log.Debug("Fill attivitas success");
                    reso = Keanu.LavRegId;
                    atlestOneAttivitaFilled = false;
                    return reso;
                } else {
                    log.Debug("Fill attivitas fail");
                    reso = Keanu.LavScartoId;
                    atlestOneAttivitaFilled = false;
                    return reso;
                }
                
            }

            catch
            {
                log.Info("Workaholic() fail");
                reso = Keanu.LavScartoId;
                return reso;
            }
        }

        private bool FillSoloAttivita(string dettaglio, DatiCliente datiCliente) {

            try {
                var listaDocss = (Documento)sfaLib.SearchGenerico(dettaglio, SFALibrary.Helpers.Utility.RicercaDocumentoName);
                List<Attivita> tempAtt = new List<Attivita>();
                tempAtt = (List<Attivita>)sfaLib.GetRelatedListItemsNew(listaDocss.RecordId, SFALibrary.Helpers.Utility.AttivitàItemName, SFALibrary.Helpers.Utility.AttivitàApiName, "Attività");

                List<Attivita> specificAttivita = new List<Attivita>();
                specificAttivita = tempAtt.Where(x => x.Tipo.ToUpper().Equals("DMS - DOC DA CORRELARE")).ToList();


                Keanu.Driver.Navigate().GoToUrl($"https://enelcrmt.lightning.force.com/lightning/r/wrts_prcgvr__Activity__c/{specificAttivita[0].RecordId}/view");
                Keanu.WaitingGame();
                WaitSpecificPage("Data Inizio");

                Attivita att = (Attivita)sfaLib.GetRecord(specificAttivita[0].RecordId, SFALibrary.Helpers.Utility.RicercaAttivitaName);
                v.AttivitaSfa = att.Numero;
                log.Debug(v.AttivitaSfa);

                if (!FillFields(datiCliente)) {
                    return false;
                }

                return true;

            } catch {
                return false;
            }

        }

        private bool FillAttivita(string recordId, DatiCliente datiCliente) {

            try {
                
                //Keanu.Driver.Navigate().GoToUrl($"https://enelcrmt.lightning.force.com/lightning/r/wrts_prcgvr__Activity__c/{recordId}/view");
                Keanu.Driver.Navigate().GoToUrl($"https://enelcrmt.lightning.force.com/lightning/r/{recordId}/related/Activities__r/view");

                Keanu.WaitingGame();
                //WaitSpecificPage("Data Inizio");

                SwitchToDefaultContent();
                IWebElement tableResult = null;
                tableResult = TrovaTableResult();
                if (tableResult == null) {
                    return false;
                }

                var tableHeaders = tableResult.FindElements(By.XPath("./thead/tr/th"));
                int indexDocumentRef = tableHeaders.IndexOf(tableHeaders.Where(q => q.Text.Contains("Activity Name")).First());
                IList<IWebElement> documentRefRows = tableResult.FindElements(By.XPath("./tbody/tr"));
                int rows = documentRefRows.Count();

                if(rows == 0) {
                    //Te vajag likt true, lai macro iet taisit taalaak nakamos dokumentus (Macro taisa 2+ doc ref, fatto - ja ir apstradats vismaz viens doc ref no visiem. Citadak - scarto)
                    //return false;
                    return true;
                }

                if(rows >= 2) {
                    return false;
                }

                string actNumber = tableResult.FindElement(By.XPath("./tbody/tr[" + 1 + "]")).FindElements(By.XPath("./*")).ElementAt(indexDocumentRef).Text;

                if(actNumber == null) {
                    return false;
                }

                UnlockAndCloseAllTabs();
                Keanu.WaitingGame();
                //WaitSpecificPage("Data Inizio");

                Attivita attivitaToFill = (Attivita)sfaLib.Search(actNumber);

                if (attivitaToFill == null) {
                    return false;
                }


                    
                    if (CheckFields(attivitaToFill, datiCliente))
                {
                    atlestOneAttivitaFilled = true;
                    return true;
                }
                    
                
                    //Check here if att already done
                    //If true return true
                    //If false, continue with FillFileds Method
                    //Need to check : POD, Trippleta, Status, cliente ( datiCliente.RagioneSocialeTestata == NomeCliente, comment)

                    //Keanu.WaitingGame();
                    //WaitSpecificPage("Data Inizio");
                    Keanu.Driver.Navigate().GoToUrl($"https://enelcrmt.lightning.force.com/lightning/r/wrts_prcgvr__Activity__c/{attivitaToFill.RecordId}/view");

                Keanu.WaitingGame();
                WaitSpecificPage("Data Inizio");

                if (!FillFields(datiCliente)) {
                    return false;
                }
                atlestOneAttivitaFilled = true;
                return true;

            } catch {

                return false;

            }

        }


        private bool CheckFields (Attivita attivita, DatiCliente datiCliente)
        {

            if (attivita.Stato != "FATTO")
            {
                return false;
            }
            if (attivita.Tipo !="DMS - Doc da correlare")
            {
                return false;
            }
            if (attivita.Causale !="Gestione Documenti")
            {
                return false;
            }
            if (attivita.Descrizione !="Ricezione Documentazione")
            {
                return false;
            }
            if (attivita.Pod != "99999999999999")
            {
                return false;
            }
            if (attivita.Commento != "CONTRATTO DA PE" && attivita.Commento != "ALTRO DOCUMENTO DA PE")
            {
                return false;
            }
            if (attivita.NomeCliente != datiCliente.RagioneSocialeTestata)
            {
                return false;
            }

            return true;
        
        }

            private bool FillFields(DatiCliente datiCliente) {

            try {
                if (!ClickButtonByName("Modifica")) {
                    return false;
                }

                if (!AssociaServizioPod("99999999999999")) {
                    return false;
                }

                if (!SetTripletta(v.Causale, v.Descrizione, v.Specifica)) {
                    return false;
                }

                if (!CaricaCampiSearch("COLLEGAMENTOCliente", datiCliente.CodiceAnagraficaCliente)) {
                    log.Info($"Problemi nell'associare il cliente con {datiCliente.CodiceAnagraficaCliente} - proviamo con {datiCliente.IdRecordSiebel}");
                    if (!CaricaCampiSearch("COLLEGAMENTOCliente", datiCliente.IdRecordSiebel)) {
                        log.Error("Problemi nell'associare il cliente");
                        return false;
                    }
                }

                if (Keanu.LavLoginId == 4977 || Keanu.LavLoginId == 5056) {
                    if (!SetCommento("CONTRATTO DA PE")) {
                        return false;
                    }
                } else {
                    if (!SetCommento("ALTRO DOCUMENTO DA PE")) {
                        return false;
                    }
                }

                if (!CompilaCampi("* Status", "FATTO", TipoCampo.COMBO, false, false)) {
                    if (!CompilaCampi("* Stato", "FATTO", TipoCampo.COMBO, false, false)) {
                        return false;
                    }
                }

                if (!ClickButtonByName("Salva")) {
                    return false;
                }

                WaitSpecificPage("Data Inizio");

                return true;
            } catch {
                return false;
            }

            
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

                if (!CompilaCampi("* Description", Descrizione, Constant.TipoCampo.COMBO))
                {
                    if (!CompilaCampi("* Descrizione", Descrizione, Constant.TipoCampo.COMBO))
                        return false;
                }
                Thread.Sleep(1000);

                if (!CompilaCampi("* Specification", Specifica, Constant.TipoCampo.COMBO))
                {
                    if (!CompilaCampi("* Specifica", Specifica, Constant.TipoCampo.COMBO))
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

        private void GoUP()
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)Keanu.Driver;
            js.ExecuteScript("window.scrollBy(0,-1000)", "");
            Thread.Sleep(500);
        }

        private bool PepperYourSfaLib()
        {
            sfaLib = new SfaLib(Keanu.LoginSFA, Keanu.PassSFA);
            bool marc = sfaLib.LoginProd();
            if (!marc) { marc = sfaLib.LoginProd(); }
            if (!marc) { log.Error("Login non riuscito su SFA con HTMlUnit."); return false; }
            return true;
        }

        public bool SwitchToDefaultContent()
        {
            try
            {
                Iframe = false;
                Keanu.Driver.SwitchTo().DefaultContent();
                return true;
            }
            catch
            {
                Iframe = true;
                return false;
            }
        }

        #region 2021
        public bool SwitchToIframe2(string classParent = "oneAlohaPage")
        {
            WebDriverWait Wait = new WebDriverWait(Keanu.Driver, TimeSpan.FromSeconds(5));

            if (HasActiveSubtab())
            {
                try
                {
                    Iframe = true;
                    Wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.XPath(".//div[@class = 'oneWorkspace active hasFixedFooter hasActiveSubtab navexWorkspace']//section[contains(@class, 'tabContent active oneConsoleTab')]//div[contains(@class, '" + classParent + "')]//iframe")));
                    return true;
                }
                catch
                {
                    try
                    {
                        Iframe = true;
                        Wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.XPath(".//div[@class = 'oneWorkspace active hasFixedFooter hasActiveSubtab oneWorkspace2']//section[contains(@class, 'tabContent active oneConsoleTab')]//div[contains(@class, '" + classParent + "')]//iframe")));
                        return true;
                    }
                    catch
                    {
                        Iframe = false;
                        return false;
                    }
                }
            }
            else
            {
                try
                {
                    Iframe = true;
                    Wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.XPath(".//div[@class = 'oneWorkspace active hasFixedFooter navexWorkspace']//section[contains(@class, 'tabContent active oneConsoleTab')]//div[contains(@class, '" + classParent + "')]//iframe")));
                    return true;
                }
                catch
                {
                    try
                    {
                        Iframe = true;
                        Wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.XPath(".//div[@class = 'oneWorkspace active hasFixedFooter oneWorkspace2']//section[contains(@class, 'tabContent active oneConsoleTab')]//div[contains(@class, '" + classParent + "')]//iframe")));
                        return true;

                    }
                    catch
                    {
                        Iframe = false;
                        return false;
                    }
                }
            }
        }

        public bool UnlockAndCloseAllTabs()
        {
            try
            {
                try
                {
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("document.querySelectorAll(\"div[class *= 'slds-context-bar__secondary navCenter tabBarContainer'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                    Thread.Sleep(500);
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("document.querySelectorAll(\"div[class *= 'slds-context-bar__secondary navCenter tabBarContainer'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                    Thread.Sleep(500);
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("document.querySelectorAll(\"div[class *= 'oneGlobalNav oneConsoleNav'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                    Thread.Sleep(500);
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("document.querySelectorAll(\"div[class *= 'oneGlobalNav oneConsoleNav'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                    Thread.Sleep(500);
                }
                catch { }

                List<IWebElement> modalsContainer = null;
                try
                {
                    SwitchToDefaultContent();
                    modalsContainer = Keanu.Driver.FindElements(By.XPath("//div[contains(@class, 'slds-modal__container')]")).ToList();
                    if (modalsContainer != null)
                    {
                        foreach (var modalContainer in modalsContainer)
                        {
                            try
                            {
                                Thread.Sleep(Keanu.Randy(1));
                                List<IWebElement> pulsanti = modalContainer.FindElements(By.XPath(".//button")).ToList();
                                if (pulsanti.Count == 0)
                                    continue;
                                List<IWebElement> pulsantiSenzaAnnulla = pulsanti.Where(q => !q.Text.ToUpper().Contains("ANNULLA")).ToList();
                                if (pulsanti.Count == 1 || pulsantiSenzaAnnulla.Count == 0)
                                {
                                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", pulsanti[0]);
                                    continue;
                                }
                                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", pulsantiSenzaAnnulla[0]);
                            }
                            catch
                            {
                                log.Info("Modale senza pulsante, può captare");
                            }
                        }
                    }

                    if (Keanu.Driver.PageSource.Contains("div class=\"oneAlohaPage\""))
                    {
                        SwitchToIframe2();
                        modalsContainer = Keanu.Driver.FindElements(By.XPath("//div[contains(@class, 'slds-modal__container')]")).ToList();
                        if (modalsContainer != null)
                        {
                            foreach (var modalContainer in modalsContainer)
                            {
                                try
                                {
                                    Thread.Sleep(Keanu.Randy(1));
                                    List<IWebElement> pulsanti = modalContainer.FindElements(By.XPath(".//button")).ToList();
                                    if (pulsanti.Count == 0)
                                        continue;
                                    List<IWebElement> pulsantiSenzaAnnulla = pulsanti.Where(q => !q.Text.ToUpper().Contains("ANNULLA")).ToList();
                                    if (pulsanti.Count == 1 || pulsantiSenzaAnnulla.Count == 0)
                                    {
                                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", pulsanti[0]);
                                        continue;
                                    }
                                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", pulsantiSenzaAnnulla[0]);
                                }
                                catch
                                {
                                    log.Info("Modale senza pulsante, può captare");
                                }
                            }
                        }
                    }
                }
                catch
                {
                    log.Info("Non dovrebbero esserci modali aperte");
                }

                try
                {
                    WaitSpecificPage("Sblocco Tab", true);
                }
                catch
                {
                    SwitchToDefaultContent();
                    log.Warn("WaitSpecificPage() fail");
                    Keanu.WaitingGame();
                }

                try
                {
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("document.querySelectorAll(\"div[class *= 'slds-context-bar__secondary navCenter tabBarContainer'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                    Thread.Sleep(500);
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("document.querySelectorAll(\"div[class *= 'slds-context-bar__secondary navCenter tabBarContainer'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                    Thread.Sleep(500);
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("document.querySelectorAll(\"div[class *= 'oneGlobalNav oneConsoleNav'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                    Thread.Sleep(500);
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("document.querySelectorAll(\"div[class *= 'oneGlobalNav oneConsoleNav'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                    Thread.Sleep(500);
                }
                catch { }

                SwitchToDefaultContent();

                bool isTabAperte = false;

                try
                {
                    log.Info("Controllo se ci sono tab aperte");
                    //***MODIFICA 08/02/2021*** CASI NUOVI - PROFILI DIVERSI
                    //isTabAperte = this.Driver.FindElements(By.XPath(".//div[contains(@class, 'oneGlobalNav oneConsoleNav')]//div[contains(@class, 'tabContainer')]//li[contains(@class, 'tabItem')]")).Count > 0;
                    isTabAperte = Keanu.Driver.FindElements(By.XPath(".//div[contains(@class, 'tabContainer')]//li[contains(@class, 'tabItem')]")).Count > 0;
                }
                catch { }

                if (!isTabAperte)
                {
                    log.Info("Non ci sono più tab da chiudere");
                    return true;
                }

                IWebElement ulTabPage = null;
                IList<IWebElement> listTabPage = null;
                try
                {
                    ulTabPage = Keanu.Driver.FindElement(By.XPath("//ul[@class='tabBarItems slds-grid']"));
                    listTabPage = ulTabPage.FindElements(By.XPath("./li"));
                }
                catch
                {
                    log.Debug("Non ci sono tab aperte");
                    return true;
                }
                for (int i = 1; i < listTabPage.Count; i++)
                {
                    IWebElement tab = listTabPage[i];
                    IWebElement ancorTab = tab.FindElement(By.XPath("./a"));
                    try
                    {
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", ancorTab);
                        Thread.Sleep(1000);
                        Keanu.Driver.SwitchTo().DefaultContent();
                        IWebElement ulbarra = null;
                        try
                        {
                            Thread.Sleep(Keanu.Randy(1));
                            ulbarra = Keanu.Driver.FindElement(By.XPath("//ul[@class='utilitybar slds-utility-bar']"));
                            IWebElement OpzioneSbloccoTab = ulbarra.FindElements(By.XPath(".//li")).Where(box => box.Text.Equals("Sblocco Tab")).ToList<IWebElement>()[0];
                            IWebElement ButtonOpzioneSbloccoTab = OpzioneSbloccoTab.FindElement(By.XPath(".//button[@class = 'bare slds-button slds-utility-bar__action slds-truncate uiButton']"));
                            ButtonOpzioneSbloccoTab.Click();
                            WaitSpecificPage("Sblocca/Blocca Tab", true);
                            SwitchToDefaultContent();
                            try
                            {
                                IWebElement buttonSbloccaTab = Keanu.Driver.FindElement(By.XPath("//button[text() = 'Sblocca tutti i subtabs']"));
                                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", buttonSbloccaTab);
                                IWebElement buttonTab = Keanu.Driver.FindElement(By.XPath("//button[text() = 'Sblocca il tab in focus']"));
                                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", buttonTab);
                                bool esito = AttendieConfermaModale("slds-modal__container", "Spiacenti per l'interruzione", "Potrebbe essere sufficiente aggiornarla", "OK", false, out bool modalFind);
                                if (!esito && modalFind)
                                {
                                    log.Error("Problemi nel chiudere la modale 'Spiacenti per l'interruzione'");
                                    return false;
                                }
                            }
                            catch { }
                            ButtonOpzioneSbloccoTab.Click();
                            SwitchToDefaultContent();
                            try
                            {
                                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("document.querySelectorAll(\"div[class *= 'slds-context-bar__secondary navCenter tabBarContainer'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                                Thread.Sleep(500);
                                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("document.querySelectorAll(\"div[class *= 'slds-context-bar__secondary navCenter tabBarContainer'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                                Thread.Sleep(500);
                                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("document.querySelectorAll(\"div[class *= 'oneGlobalNav oneConsoleNav'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                                Thread.Sleep(500);
                                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("document.querySelectorAll(\"div[class *= 'oneGlobalNav oneConsoleNav'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                                Thread.Sleep(500);
                            }
                            catch { }
                        }
                        catch
                        {
                            log.Error("Impossibile visualizzare la bar");
                            bool esito = AttendieConfermaModale("slds-modal__container", "Spiacenti per l'interruzione", "Potrebbe essere sufficiente aggiornarla", "OK", false, out bool modalFind);
                            if (!esito && modalFind)
                            {
                                log.Error("Problemi nel chiudere la modale 'Spiacenti per l'interruzione'");
                                return false;
                            }
                        }
                    }
                    catch { }
                }
                if (listTabPage.Count > 0)
                {
                    try
                    {
                        Thread.Sleep(Keanu.Randy(3));
                        IWebElement DivNavigation = Keanu.Driver.FindElement(By.XPath("//div[@class = 'slds-context-bar__secondary navCenter']"));
                        var LstButtonClose = DivNavigation.FindElements(By.XPath(".//button[@class = 'slds-button slds-button_icon-x-small slds-button_icon-container']"));
                        if (LstButtonClose.Count == 0)
                        {
                            LstButtonClose = DivNavigation.FindElements(By.XPath(".//button[@class = 'slds-button slds-button_icon slds-button_icon-x-small slds-button_icon-container']"));
                        }
                        while (LstButtonClose.Count > 0)
                        {
                            foreach (IWebElement ButtonClose in LstButtonClose)
                            {
                                try { ButtonClose.Click(); }
                                catch { }
                            }
                            LstButtonClose = DivNavigation.FindElements(By.XPath(".//button[@class = 'slds-button slds-button_icon slds-button_icon-x-small slds-button_icon-container']"));
                        }
                        SwitchToDefaultContent();
                        Keanu.WaitingGame();
                    }
                    catch { }
                }
            }
            catch (Exception Ex)
            {
                SwitchToDefaultContent();
                log.Warn(Ex.ToString());
                return false;
            }
            return true;
        }

        public IWebElement GetSezioneAttiva()
        {
            IWebElement returnElement;
            if (HasActiveSubtab())
            {
                try//NEW
                {
                    returnElement = Keanu.Driver.FindElement(By.XPath(".//div[@class = 'oneWorkspace active hasFixedFooter hasActiveSubtab navexWorkspace']//section[contains(@class, 'tabContent active oneConsoleTab')]"));
                }
                catch//OLD
                {
                    returnElement = Keanu.Driver.FindElement(By.XPath(".//div[@class = 'oneWorkspace active hasFixedFooter hasActiveSubtab oneWorkspace2']//section[contains(@class, 'tabContent active oneConsoleTab')]"));
                }
            }
            else
            {
                try
                {//NEW
                    returnElement = Keanu.Driver.FindElement(By.XPath(".//div[@class = 'oneWorkspace active hasFixedFooter navexWorkspace']//section[contains(@class, 'tabContent active oneConsoleTab')]"));
                }
                catch
                {//OLD
                    returnElement = Keanu.Driver.FindElement(By.XPath(".//div[@class = 'oneWorkspace active hasFixedFooter oneWorkspace2']//section[contains(@class, 'tabContent active oneConsoleTab')]"));
                }
            }
            return returnElement;
        }

        public bool HasActiveSubtab()
        {
            try
            {
                var val = Keanu.Driver.FindElements(By.CssSelector("div"));
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
        #endregion

        public bool AttendieConfermaModale(string classeContainer, string titoloModale, string testoModale, string buttonText, bool modaleObbligatoria, out bool modalFind, string tipoHeader = "h2")
        {
            int numTent = 0;
            modalFind = false;
            try
            {
                Thread.Sleep(Keanu.Randy(1));
                IWebElement modalCheckList = null;
                IWebElement modalContainer = null;
                IWebElement textCheckList = null;
                bool modaleTrovata = false;
                string xpath = $".//{tipoHeader}[contains(normalize-space(), \"{titoloModale}\")]";

                if (string.IsNullOrEmpty(titoloModale))
                    xpath = $".//*[text() = \"{testoModale}\"]";

                while (!modaleTrovata && numTent < 5)
                {
                    try
                    {
                        var modals = Keanu.Driver.FindElements(By.XPath(xpath));
                        foreach (var item in modals)
                        {
                            if (!string.IsNullOrEmpty(titoloModale) && item.Text.Contains(titoloModale))
                                modalCheckList = item;
                            else if (string.IsNullOrEmpty(titoloModale) && item.Text.Equals(testoModale))
                                modalCheckList = item;
                            if (modalCheckList != null)
                            {
                                try
                                {
                                    modalContainer = modalCheckList.FindElement(By.XPath($"./ancestor::div[contains(@class, '{classeContainer}')]"));
                                    textCheckList = modalContainer.FindElement(By.XPath($".//*[text()[contains(., \"{testoModale}\")]]"));
                                    break;
                                }
                                catch
                                {
                                    modalCheckList = null;
                                }
                            }
                        }

                        if (textCheckList != null)
                        {
                            modalFind = true;
                            modaleTrovata = true;
                        }
                        else
                        {
                            if (!modaleObbligatoria) { return true; }
                        }
                    }
                    catch
                    {
                        if (!modaleObbligatoria)
                        {
                            modalFind = false;
                            return true;
                        }
                        Thread.Sleep(Keanu.Randy(1));
                        numTent++;
                    }
                }

                if (!modaleTrovata)
                {
                    log.Error("Modale non trovata");
                    return false;
                }

                IWebElement footerCheckList = modalContainer.FindElement(By.XPath(".//div[contains(@class, 'slds-modal__footer')]"));
                IWebElement pulsanteContinuaCheckList = footerCheckList.FindElement(By.XPath(".//*[text()[contains(., '" + buttonText + "')]]"));

                while (!pulsanteContinuaCheckList.Enabled && numTent < 15)
                {
                    Thread.Sleep(Keanu.Randy(1));
                    numTent++;
                }

                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", pulsanteContinuaCheckList);

                Thread.Sleep(Keanu.Randy(1));
            }
            catch (Exception Ex)
            {
                log.Error($"Problemi in AttendieConfermaModale - Errore: {Ex}");
                return false;
            }
            return true;
        }

        public bool WaitSpecificPage(string valoredaCercare, bool isNotSezioneAttiva = false)
        {
            string contentText;
            int tent = 0;
            if (Iframe || isNotSezioneAttiva)
                contentText = Keanu.Driver.PageSource.ToString();
            else
                contentText = GetSezioneAttiva().Text;

            bool paginaCaricata = false;
            while (!paginaCaricata && tent < 30)
            {
                if (!contentText.Contains(valoredaCercare))
                {
                    Thread.Sleep(Keanu.Randy(1));
                    if (Iframe || isNotSezioneAttiva) { contentText = Keanu.Driver.PageSource.ToString(); }
                    else { contentText = GetSezioneAttiva().Text; }
                    valoredaCercare = valoredaCercare.Replace("  ", " ");
                    contentText = contentText.Replace("  ", " ");
                    tent++;
                }
                else
                {
                    paginaCaricata = true;
                    Thread.Sleep(Keanu.Randy(1));
                }
            }
            return paginaCaricata;
        }

        public string GetFieldValue(string label, string tipo = "", IWebElement elementoDiPartenza = null)
        {
            string result = "";
            try
            {
                IWebElement sottoNavbarElement = elementoDiPartenza;
                IWebElement labelElement = null;
                IWebElement divFatherElement = null;

                try
                {
                    if (elementoDiPartenza == null)
                        sottoNavbarElement = GetSezioneAttiva();
                }
                catch { log.Info($"Problemi nel recuperare la sezione attiva - iframe {Iframe}"); }

                switch (tipo)
                {
                    case "DOCUMENT":
                        if (Iframe && elementoDiPartenza == null) { labelElement = Keanu.Driver.FindElement(By.XPath(".//label[normalize-space()= \"" + label + "\"]")); }
                        else { labelElement = sottoNavbarElement.FindElement(By.XPath(".//label[normalize-space()= \"" + label + "\"]")); }
                        divFatherElement = labelElement.FindElement(By.XPath(".."));
                        IWebElement documentElement = divFatherElement.FindElement(By.XPath(".//input"));
                        string datalink = documentElement.GetAttribute("data-link");
                        int datalinkIndex = datalink.IndexOf("DocID=");
                        result = datalink.Substring(datalinkIndex + 6);
                        break;
                    case "INPUT":
                        if (Iframe && elementoDiPartenza == null) { labelElement = Keanu.Driver.FindElement(By.XPath(".//label[normalize-space()= \"" + label + "\"]")); }
                        else { labelElement = sottoNavbarElement.FindElement(By.XPath(".//label[normalize-space()= \"" + label + "\"]")); }
                        divFatherElement = labelElement.FindElement(By.XPath(".."));
                        IWebElement inputElement = divFatherElement.FindElement(By.XPath(".//input"));
                        result = inputElement.GetAttribute("value");
                        break;
                    case "INPUT_AND_CLICK":
                        if (Iframe && elementoDiPartenza == null) { labelElement = Keanu.Driver.FindElement(By.XPath(".//label[normalize-space()= \"" + label + "\"]")); }
                        else { labelElement = sottoNavbarElement.FindElement(By.XPath(".//label[normalize-space()= \"" + label + "\"]")); }
                        divFatherElement = labelElement.FindElement(By.XPath(".."));
                        IWebElement inputAndClickElement = divFatherElement.FindElement(By.XPath(".//input"));
                        result = inputAndClickElement.GetAttribute("value");
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", inputAndClickElement);
                        break;
                    case "UL_TABLE_VALUE":
                        if (Iframe && elementoDiPartenza == null) { labelElement = Keanu.Driver.FindElement(By.XPath(".//li//span[contains(text(), \"" + label + "\")]")); }
                        else { labelElement = sottoNavbarElement.FindElement(By.XPath(".//li//span[contains(text(), \"" + label + "\")]")); }
                        IWebElement ulTableValue = labelElement.FindElement(By.XPath("./following-sibling::*"));
                        result = ulTableValue.Text;
                        if (string.IsNullOrEmpty(result)) { result = ulTableValue.GetAttribute("title"); }
                        break;
                    case "UL_TABLE_OI_VALUE":
                        if (Iframe && elementoDiPartenza == null) { labelElement = Keanu.Driver.FindElement(By.XPath(".//a[contains(text(), 'OI-')]")); }
                        else { labelElement = sottoNavbarElement.FindElement(By.XPath(".//a[contains(text(), 'OI-')]")); }
                        result = labelElement.Text;
                        break;
                    case "UL_TABLE_CLICK":
                        IWebElement ulTableValueClick = sottoNavbarElement.FindElement(By.XPath(".//li//a"));
                        result = ulTableValueClick.Text;
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", ulTableValueClick);
                        break;
                    case "UL_TABLE_OI_CLICK":
                        if (Iframe && elementoDiPartenza == null) { labelElement = Keanu.Driver.FindElement(By.XPath(".//a[contains(text(), 'OI-')]")); }
                        else { labelElement = sottoNavbarElement.FindElement(By.XPath(".//a[contains(text(), 'OI-')]")); }
                        result = labelElement.Text;
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", labelElement);
                        break;
                    case "SPAN":
                        if (Iframe && elementoDiPartenza == null) { labelElement = Keanu.Driver.FindElement(By.XPath(".//span[@class = 'test-id__field-label' and normalize-space() = \"" + label + "\"]")); }
                        else { labelElement = sottoNavbarElement.FindElement(By.XPath(".//span[@class = 'test-id__field-label' and normalize-space() = \"" + label + "\"]")); }
                        divFatherElement = labelElement.FindElement(By.XPath("../.."));
                        IWebElement spanValue = null;
                        try { spanValue = divFatherElement.FindElement(By.XPath(".//span[@class = 'uiOutputText']")); }
                        catch { spanValue = divFatherElement.FindElement(By.XPath(".//span[contains(@class, 'test-id__field-value')]//span")); }
                        result = spanValue.Text;
                        break;
                    case "SPAN_AND_CLICK":
                        if (Iframe && elementoDiPartenza == null) { labelElement = Keanu.Driver.FindElement(By.XPath(".//span[@class = 'test-id__field-label' and normalize-space() = \"" + label + "\"]")); }
                        else { labelElement = sottoNavbarElement.FindElement(By.XPath(".//span[@class = 'test-id__field-label' and normalize-space() = \"" + label + "\"]")); }
                        divFatherElement = labelElement.FindElement(By.XPath("../.."));
                        IWebElement spanAndClickValue = divFatherElement.FindElement(By.XPath(".//a[contains(@class, 'outputLookupLink')]"));
                        result = spanAndClickValue.Text;
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", spanAndClickValue);
                        break;
                    case "SPAN_NO_CLICK":
                        labelElement = sottoNavbarElement.FindElement(By.XPath(".//span[@class = 'test-id__field-label' and normalize-space() = \"" + label + "\"]"));
                        divFatherElement = labelElement.FindElement(By.XPath("../.."));
                        IWebElement spanNOClickValue = divFatherElement.FindElement(By.XPath(".//a[contains(@class, 'outputLookupLink')]"));
                        result = spanNOClickValue.Text;
                        break;
                    case "SPAN_TEXTAREA":
                        if (Iframe && elementoDiPartenza == null) { labelElement = Keanu.Driver.FindElement(By.XPath(".//span[@class = 'test-id__field-label' and normalize-space() = \"" + label + "\"]")); }
                        else { labelElement = sottoNavbarElement.FindElement(By.XPath(".//span[@class = 'test-id__field-label' and normalize-space() = \"" + label + "\"]")); }
                        divFatherElement = labelElement.FindElement(By.XPath("../.."));
                        IWebElement spanValueTextArea = divFatherElement.FindElement(By.XPath(".//span[@class = 'uiOutputTextArea']"));
                        result = spanValueTextArea.Text;
                        break;
                    case "SPAN_HEADER":
                        if (Iframe && elementoDiPartenza == null) { labelElement = Keanu.Driver.FindElement(By.XPath(".//span[@class = 'slds-text-title--caps' and normalize-space() = \"" + label + "\"]")); }
                        else { labelElement = sottoNavbarElement.FindElement(By.XPath(".//span[@class = 'slds-text-title--caps' and normalize-space() = \"" + label + "\"]")); }
                        divFatherElement = labelElement.FindElement(By.XPath(".."));
                        IWebElement spanValueHeader = divFatherElement.FindElement(By.XPath("./h1//span"));
                        result = spanValueHeader.Text;
                        break;
                    case "SPAN_DATE":
                        if (Iframe && elementoDiPartenza == null) { labelElement = Keanu.Driver.FindElement(By.XPath(".//span[@class = 'test-id__field-label' and normalize-space() = \"" + label + "\"]")); }
                        else { labelElement = sottoNavbarElement.FindElement(By.XPath(".//span[@class = 'test-id__field-label' and normalize-space() = \"" + label + "\"]")); }
                        divFatherElement = labelElement.FindElement(By.XPath("../.."));
                        IWebElement spanDate = divFatherElement.FindElement(By.XPath(".//span[@class = 'uiOutputDate']"));
                        result = spanDate.Text;
                        break;
                    case "SELECT":
                        if (!Iframe)
                        {
                            result = (string)((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab label.slds-form-element__label'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.querySelector('select').value");
                        }
                        else
                        {
                            labelElement = Keanu.Driver.FindElement(By.XPath(".//label[contains(@class, 'slds-form-element__label') and normalize-space() = \"" + label + "\"]"));
                            divFatherElement = labelElement.FindElement(By.XPath(".."));
                            IWebElement select = divFatherElement.FindElement(By.XPath(".//select"));
                            if (string.IsNullOrEmpty(result)) { result = select.GetAttribute("value"); }
                        }
                        break;
                    case "DD_P":
                        if (Iframe && elementoDiPartenza == null) { labelElement = Keanu.Driver.FindElement(By.XPath(".//p[@class = 'slds-truncate' and normalize-space() = \"" + label + "\"]")); }
                        else { labelElement = sottoNavbarElement.FindElement(By.XPath(".//p[@class = 'slds-truncate' and normalize-space() = \"" + label + "\"]")); }
                        IWebElement ddPValueHeader = null;
                        try
                        {
                            ddPValueHeader = labelElement.FindElement(By.XPath(".//following-sibling::dd//p"));
                        }
                        catch
                        {
                            ddPValueHeader = labelElement.FindElement(By.XPath("../following-sibling::dd//p"));
                        }
                        result = ddPValueHeader.Text;
                        break;
                    case "DT_DD":
                        if (Iframe && elementoDiPartenza == null) { labelElement = Keanu.Driver.FindElement(By.XPath(".//dt[@class = 'slds-item_label slds-text-color_weak slds-truncate' and normalize-space() = \"" + label + "\"]")); }
                        else { labelElement = sottoNavbarElement.FindElement(By.XPath(".//dt[@class = 'slds-item_label slds-text-color_weak slds-truncate' and normalize-space() = \"" + label + "\"]")); }
                        IWebElement dtDdValueHeader = labelElement.FindElement(By.XPath(".//following-sibling::dd"));
                        result = dtDdValueHeader.Text;
                        break;
                    default:
                        string value = null;
                        try
                        {
                            value = (string)((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab label.slds-form-element__label'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].value");
                            if (value != null) { result = value; break; }
                        }
                        catch { }
                        try
                        {
                            value = (string)((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab label.slds-form-element__label'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].innerHTML");
                            if (value != null) { result = value; break; }
                        }
                        catch { }
                        try
                        {
                            value = (string)((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab label.slds-form-element__label'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].children[0].value");
                            if (value != null) { result = value; break; }
                        }
                        catch { }
                        try
                        {
                            value = (string)((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab label.form-element__label'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.value");
                            if (value != null) { result = value; break; }
                        }
                        catch { }
                        try
                        {
                            value = (string)((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab')[0].getElementsByClassName('slds-form-element__label slds-no-flex'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].value");
                            if (value != null) { result = value; break; }
                        }
                        catch { }
                        try
                        {
                            value = (string)((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab')[0].getElementsByClassName('slds-form-element__label slds-no-flex'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].innerHTML");
                            if (value != null) { result = value; break; }
                        }
                        catch { }
                        try
                        {
                            value = (string)((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab')[0].getElementsByClassName('slds-form-element__label slds-no-flex'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].children[0].value");
                            if (value != null) { result = value; break; }
                        }
                        catch { }
                        try
                        {
                            value = (string)((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab')[0].getElementsByClassName('slds-form-element__label slds-no-flex'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.value");
                            if (value != null) { result = value; break; }
                        }
                        catch { }
                        break;
                }
            }
            catch (Exception Ex) { log.Error($"Errore nel recupero del campo la cui label è {label}. Eccezione {Ex.Message}"); }

            return result;
        }

        public bool AssociaServizioPod(string pod)
        {
            try
            {
                GoUP();
                GoUP();
                GoUP();
                CompilaCampi("* POD", "", Constant.TipoCampo.INPUT);
                string tipoServizioSfa = GetFieldValue("Servizio", "SELECT");
                if (tipoServizioSfa == null)
                    tipoServizioSfa = "";

                string tipoServizio = "ELETTRICO";
                if (!string.IsNullOrEmpty(pod) && !pod.StartsWith("IT"))
                    tipoServizio = "GAS";

                string tipoCommodity = "ELETTRICO";
                if (!string.IsNullOrEmpty(pod) && !pod.StartsWith("IT"))
                    tipoCommodity = "GAS";


                bool esito;
                if (!tipoServizioSfa.Equals(tipoServizio))
                {
                    esito = CompilaCampi(" Servizio", tipoServizio, Constant.TipoCampo.COMBO);
                    if (!esito)
                    {
                        esito = CompilaCampi("* Servizio", tipoServizio, Constant.TipoCampo.COMBO);
                        if (!esito)
                        {
                            log.Warn("Problemi nel settare il tipo servizio");
                            return false;
                        }
                    }
                    log.Info("Servizio settato");
                    Thread.Sleep(Keanu.Randy(1));
                }

                esito = CompilaCampi("* Commodity", tipoCommodity, Constant.TipoCampo.COMODITY);

                string podSfa = GetFieldValue("* POD/PDR/COD.MIGRAZIONE  ");
                if (podSfa.StartsWith("999999")) { podSfa = ""; }
                if (string.IsNullOrEmpty(podSfa))
                {
                    if (string.IsNullOrEmpty(pod)) { pod = "IT111E9999999W"; }
                    esito = CompilaCampi("* POD/PDR/COD.MIGRAZIONE  ", pod, Constant.TipoCampo.INPUT);
                    if (!esito) { esito = CompilaCampi("* POD/PDR/COD.MIGRAZIONE  ", pod, Constant.TipoCampo.INPUT); }
                    if (!esito) { esito = CompilaCampi("* POD/PDR/COD.MIGRAZIONE  ", pod, Constant.TipoCampo.INPUT); }
                    if (!esito) { log.Error("Problemi nell'inserire il pod"); return false; }
                    log.Info("Pod inserito");
                    Thread.Sleep(Keanu.Randy(1));
                }
                else
                {
                    if (!string.IsNullOrEmpty(pod) && (pod.Equals("IT111E9999999W") || pod.Equals("99999999999999")))
                    {
                        if (!podSfa.Equals(pod))
                        {
                            esito = CompilaCampi("* POD/PDR/COD.MIGRAZIONE  ", pod, Constant.TipoCampo.INPUT);
                            if (!esito)
                                esito = CompilaCampi("* POD/PDR/COD.MIGRAZIONE  ", pod, Constant.TipoCampo.INPUT);
                            if (!esito)
                            {
                                log.Error("Problemi nell'inserire il pod");
                                return false;
                            }
                            log.Info("Pod inserito");
                            Thread.Sleep(Keanu.Randy(1));
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                log.Error(Ex.ToString());
                return false;
            }
            return true;
        }

        public bool SetCommento(string commento)
        {
            try
            {
                IWebElement SpanCommento = null;
                try
                {
                    SpanCommento = GetSezioneAttiva().FindElement(By.XPath(".//span[@aria-label = ' Commento']"));
                }
                catch { }
                IWebElement textArea = SpanCommento.FindElement(By.XPath(".//textArea"));
                Thread.Sleep(500);
                textArea.Clear();
                textArea.SendKeys(commento);
                Thread.Sleep(500);
                FocusOnInputTypeField(" POD");//VIENE SETTATO IL FOCUS IN MODO DA POTER SALVARE I DATI DEL COMMENTO
            }
            catch (Exception Ex)
            {
                log.Error(Ex.ToString());
                return false;
            }
            return true;
        }

        public bool FocusOnInputTypeField(string labelText)
        {
            if (labelText.Substring(0, 1).Equals(" ")) labelText = labelText.Substring(1, labelText.Length - 1);
            Thread.Sleep(Keanu.Randy(1));
            try
            {
                IWebElement welabel = null;
                try
                {
                    if (!Iframe)
                    {
                        welabel = GetSezioneAttiva().FindElements(By.XPath(".//label[contains(@class, 'slds-form-element__label')]")).Where(box => box.Text.Contains(labelText)).ToList()[0];
                    }
                    else
                    {
                        welabel = Keanu.Driver.FindElements(By.XPath(".//label[contains(@class, 'slds-form-element__label')]")).Where(box => box.Text.Contains(labelText)).ToList()[0];
                    }
                }
                catch
                {
                    if (!Iframe)
                    {
                        welabel = GetSezioneAttiva().FindElements(By.XPath(".//label[contains(@class, 'slds-form-element__label')]")).Where(box => box.Text.Contains(labelText)).ToList()[0];
                    }
                    else
                    {
                        welabel = Keanu.Driver.FindElements(By.XPath(".//label[contains(@class, 'slds-form-element__label')]")).Where(box => box.Text.Contains(labelText)).ToList()[0];
                    }
                }
                if (welabel != null)
                {
                    IWebElement weWrite = null;
                    try
                    {
                        weWrite = welabel.FindElement(By.XPath(".//following-sibling::div"));
                        weWrite = weWrite.FindElement(By.TagName("input"));// Modifica perchè ci potrebbero essere più div alora si va per tag name
                        weWrite.Click();
                        return true;
                    }
                    catch
                    {
                        log.Error("Il campo " + labelText + " non è stato valorizzato correttamente");
                        return false;
                    }
                }
                return false;
            }
            catch
            {
                log.Error("Il campo " + labelText + " non è stato valorizzato correttamente");
                return false;
            }
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
                        welabel = Keanu.Driver.FindElements(By.XPath($"//label[contains(@class,'slds-form-element__label')]{addSpan}")).Where(box => box.Text.Equals(label)).ToList()[0];
                }
                catch
                {
                    if (!iframe)
                        welabel = elementoDiPartenza.FindElements(By.XPath($".//label{addSpan}")).Where(box => box.Text.Equals(label)).ToList()[0];
                    else
                        welabel = Keanu.Driver.FindElements(By.XPath($"//label{addSpan}")).Where(box => box.Text.Equals(label)).ToList()[0];
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
                        case TipoCampo.COMODITY:
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
                                if (value.Contains("ELETTRICO"))
                                {
                                    weWrite.SendKeys(Keys.Enter);
                                    weWrite.SendKeys(Keys.Down);
                                    weWrite.SendKeys(Keys.Down);
                                    weWrite.SendKeys(Keys.Enter);

                                }
                                if (value.Contains("GAS"))
                                {
                                    weWrite.SendKeys(Keys.Enter);
                                    weWrite.SendKeys(Keys.Down);
                                    weWrite.SendKeys(Keys.Down);
                                    weWrite.SendKeys(Keys.Down);
                                    weWrite.SendKeys(Keys.Enter);
                                }

                                log.Info("Il campo " + label + " Commodity è stato valorizzato correttamente");
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        case TipoCampo.RADIO:
                            try
                            {
                                weWrite = welabel.FindElement(By.XPath(".//following-sibling::fieldset"));
                                weWrite = weWrite.FindElements(By.XPath(".//label[@class='slds-radio__label']")).Where(box => box.Text.Equals(value)).ToList<IWebElement>()[0];// Modifica perchè ci potrebbero essere più div allora si va per tag name
                                weWrite.Click();
                            }
                            catch
                            {
                                return false;
                            }
                            return true;
                        case TipoCampo.CHECKBOX:
                            try
                            {
                                weWrite = welabel.FindElement(By.XPath(".//following-sibling::div"));
                                weWrite = weWrite.FindElement(By.XPath(".//label[contains(@class, 'slds-checkbox')]//input[@type='checkbox']"));
                                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", weWrite);
                            }
                            catch
                            {
                                return false;
                            }
                            return true;
                        case TipoCampo.DATA:
                            try
                            {
                                weWrite = welabel.FindElement(By.XPath(".//following-sibling::div"));
                                weWrite = weWrite.FindElement(By.XPath(".//input"));
                                DateTime result;
                                if (!DateTime.TryParse(value, out result)) { log.Error("Valore data non corretto"); return false; }
                                if (weWrite != null)
                                {
                                    weWrite.SendKeys(Keys.Enter);
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
                            catch
                            {
                                return false;
                            }
                        case TipoCampo.INPUT_AND_UL:
                            try
                            {
                                weWrite = welabel.FindElement(By.XPath(".//following-sibling::div"));
                                weWrite = weWrite.FindElement(By.TagName("input"));// Modifica perchè ci potrebbero essere più div alora si va per tag name
                            }
                            catch
                            {
                                try
                                {
                                    weWrite = welabel.FindElement(By.XPath(".//following-sibling::input"));
                                }
                                catch
                                {
                                    return false;
                                }
                            }
                            if (weWrite != null)
                            {
                                weWrite.SendKeys(Keys.Enter);
                                weWrite.Clear();
                                weWrite.SendKeys(value);
                                weWrite = welabel.FindElement(By.XPath(".."));
                                IWebElement wewriteUl = null;
                                try
                                {
                                    wewriteUl = weWrite.FindElement(By.XPath($".//ul//span[contains(text(),'{value}')]"));
                                }
                                catch
                                {
                                    wewriteUl = Keanu.Driver.FindElement(By.XPath($".//table[@class='territory-autocomplete']//td[contains(text(), \"{value}\")]"));
                                }
                                Thread.Sleep(1000);
                                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", wewriteUl);
                                log.Info("Il campo " + label + " è stato valorizzato correttamente");
                                return true;

                            }
                            else
                            {
                                return false;
                            }
                        case TipoCampo.TEXT_AREA:
                            try
                            {
                                weWrite = welabel.FindElement(By.XPath(".//following-sibling::div"));
                                weWrite = weWrite.FindElement(By.TagName("textarea"));// Modifica perchè ci potrebbero essere più div alora si va per tag name
                            }
                            catch
                            {
                                log.Error("Il campo " + label + " non è stato valorizzato correttamente");
                                return false;
                            }
                            if (weWrite != null)
                            {
                                weWrite.SendKeys(Keys.Enter);
                                weWrite.Clear();
                                weWrite.SendKeys(value);
                                weWrite.SendKeys(Keys.Tab);
                                log.Info("Il campo " + label + " è stato valorizzato correttamente");
                                return true;
                            }
                            else
                            {
                                log.Error("Il campo " + label + " non è stato valorizzato correttamente");
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
                            return Keanu.Driver.FindElement(By.XPath(".//div[@class = 'slds-col--padded slds-no-flex slds-align-bottom']")).FindElement(By.XPath(".//button[normalize-space()= '" + textButton + "']"));
                        else
                            return elementoDiPartenza.FindElement(By.XPath(".//div[@class = 'slds-col--padded slds-no-flex slds-align-bottom']")).FindElement(By.XPath(".//button[normalize-space()= '" + textButton + "']"));
                    }
                    else
                    {
                        if (elementoDiPartenza == null)
                            return Keanu.Driver.FindElement(By.XPath(".//" + tipo + "[@class = '" + className + "' and normalize-space()= '" + textButton + "']"));
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
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", button);
            }
            catch
            {
                log.Error($"Cannot click to {textButton}");
                return false;
            }
            Keanu.WaitingGame();
            return true;
        }

        private bool IsElementPresent(By by, IWebDriver driver = null, IWebElement elementDriver = null)
        {
            try
            {
                if (driver != null)
                    driver.FindElement(by);
                else
                    elementDriver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool CaricaCampiSearch(string key, string value)
        {
            log.Debug("Key carica campi search " + key);
            log.Debug("Valore carica campi search " + value);

            bool esito = true;
            try
            {
                IWebElement ElementSearch = null;
                try
                {
                    ElementSearch = GetSezioneAttiva().FindElement(By.XPath("//input[@class = 'slds-input mouse-pointer ' and @aria-label = '" + key + "']"));
                }
                catch
                {
                    ElementSearch = GetSezioneAttiva().FindElement(By.XPath("//input[contains(@class, 'slds-input mouse-pointer') and @placeholder = 'Ricerca cliente']"));
                }
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", ElementSearch);
                SwitchToDefaultContent();
                Thread.Sleep(Keanu.Randy(2));
                IWebElement ModalSearch = GetSezioneAttiva().FindElement(By.XPath(".//div[@class = 'slds-modal__content slds-p-around--medium']"));
                IWebElement InputSearch = ModalSearch.FindElement(By.XPath(".//input[@class = 'slds-input input uiInput uiInputText uiInput--default uiInput--input']"));
                InputSearch.Click();
                InputSearch.Clear();
                Thread.Sleep(Keanu.Randy(1));
                InputSearch.SendKeys(value);
                IWebElement ButtonClienteSearch = ModalSearch.FindElement(By.XPath(".//button[@class = 'slds-button slds-button_brand']"));
                Thread.Sleep(Keanu.Randy(1));
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", ButtonClienteSearch);
                //Thread.Sleep(Keanu.Randy(1));
                Thread.Sleep(Keanu.Randy(1));
                while (IsElementPresent(By.XPath("//span[contains(text(), 'Caricamento in corso. . .')]"), Keanu.Driver))
                {
                    log.Info($"*** Ballz");//BALLSACK
                    Thread.Sleep(Keanu.Randy(1));
                }
                Thread.Sleep(Keanu.Randy(1));
                IWebElement tabella = ModalSearch.FindElement(By.XPath(".//table[contains(@class, 'keepTableHeight')]"));
                IList<IWebElement> listaTr = tabella.FindElements(By.XPath("./tbody/tr"));
                int numeroRigheTabella = listaTr.Count();
                log.Info("NUMERO RIGHE: " + numeroRigheTabella);
                if (numeroRigheTabella == 0)
                {
                    try
                    {
                        IWebElement ButtonAnnulla = Keanu.Driver.FindElement(By.XPath("//div[@class = 'slds cITA_IFM_LCP206_LookupSearchModal cITA_IFM_LCP205_ActivityLayout']")).FindElement(By.XPath(".//button[text() = 'Annulla']"));
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", ButtonAnnulla);
                        SwitchToDefaultContent();
                        return false;
                    }
                    catch { }
                }
                if (key.Equals("COLLEGAMENTOCliente"))
                {
                    if (numeroRigheTabella == 1)
                    {
                        IWebElement anchorCliente = ModalSearch.FindElement(By.XPath(".//a[@class = 'mouse-pointer']"));
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", anchorCliente);
                        SwitchToDefaultContent();
                    }
                    else
                    {
                        IWebElement ButtonAnnulla = Keanu.Driver.FindElement(By.XPath("//div[@class = 'slds cITA_IFM_LCP206_LookupSearchModal cITA_IFM_LCP205_ActivityLayout']")).FindElement(By.XPath(".//button[text() = 'Annulla']"));
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", ButtonAnnulla);
                        SwitchToDefaultContent();
                        log.Info("Sono presenti più righe per il record ID Siebel");
                        return false;
                    }
                }
                else
                {
                    IWebElement anchorCliente = ModalSearch.FindElement(By.XPath(".//a[@class = 'mouse-pointer']"));
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", anchorCliente);
                    SwitchToDefaultContent();
                }
            }
            catch
            {
                log.Warn("CaricaCampiSearch() fail");
                esito = false;
            }
            Thread.Sleep(Keanu.Randy(1));
            return esito;
        }

        public bool RicercaDocumenti(string valore) {
            bool esito = true;
            string chiave = "Documents";
            try {
                Keanu.WaitingGame();
                IWebElement ElementCercaSfa = null;
                while (ElementCercaSfa == null) {
                    try {
                        ElementCercaSfa = Keanu.Driver.FindElement(By.XPath("//input[@class = 'slds-input slds-text-color_default slds-p-left--none slds-size--1-of-1 input default input uiInput uiInputTextForAutocomplete uiInput--{remove}']"));
                    } catch { }
                }
                IJavaScriptExecutor javascript = Keanu.Driver as IJavaScriptExecutor;
                IWebElement ElementRicerca = null;
                while (ElementRicerca == null || (ElementRicerca.GetAttribute("Title") != chiave)) {
                    var attivita = Keanu.Driver.FindElements(By.XPath("//span[@title = \"Documents\"]"));
                    if (attivita.Count < 1) {
                        IWebElement ElementFilterSearch = Keanu.Driver.FindElement(By.XPath("//div[@class = 'slds-combobox__form-element slds-input-has-icon slds-input-has-icon_right']"));
                        ElementFilterSearch.Click();
                        Thread.Sleep(Keanu.Randy(3));
                        IWebElement ElementTuttiGliElementiRicercabili = null;
                        int cunt = 0;
                        while (ElementTuttiGliElementiRicercabili == null && cunt != 3) {
                            cunt++;
                            if (cunt == 3) {
                                Keanu.Driver.Navigate().Refresh();
                                Keanu.WaitingGame();
                                Keanu.WaitingGame();
                                Keanu.WaitingGame();
                                Thread.Sleep(Keanu.Randy(5));
                                ElementFilterSearch = Keanu.Driver.FindElement(By.XPath("//div[@class = 'slds-combobox__form-element slds-input-has-icon slds-input-has-icon_right']"));
                                ElementFilterSearch.Click();
                                Thread.Sleep(Keanu.Randy(15));
                                cunt = 0;
                            }
                            try {
                                ElementTuttiGliElementiRicercabili = Keanu.Driver.FindElement(By.XPath("//ul[@aria-label = 'Tutti gli elementi ricercabili']"));
                            } catch {
                                try {
                                    Thread.Sleep(Keanu.Randy(5));
                                    ElementFilterSearch = Keanu.Driver.FindElement(By.XPath("//div[@class = 'slds-combobox__form-element slds-input-has-icon slds-input-has-icon_right']"));
                                    ElementFilterSearch.Click();
                                    Thread.Sleep(Keanu.Randy(5));
                                } catch {
                                    Keanu.Driver.Navigate().Refresh();
                                    Keanu.WaitingGame();
                                    Keanu.WaitingGame();
                                    Keanu.WaitingGame();
                                    Thread.Sleep(Keanu.Randy(5));
                                    ElementFilterSearch = Keanu.Driver.FindElement(By.XPath("//div[@class = 'slds-combobox__form-element slds-input-has-icon slds-input-has-icon_right']"));
                                    ElementFilterSearch.Click();
                                    Thread.Sleep(Keanu.Randy(15));
                                }
                            }
                        }
                    }
                    attivita = Keanu.Driver.FindElements(By.XPath("//span[@title = \"Documents\"]"));
                    ElementRicerca = attivita[0];
                }
                string parametro = valore;
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", ElementRicerca);
                Thread.Sleep(Keanu.Randy(1));
                try { ElementCercaSfa.Clear(); } catch { }
                Thread.Sleep(Keanu.Randy(1));
                try { ElementCercaSfa.Click(); } catch { }
                Thread.Sleep(Keanu.Randy(1));
                ElementCercaSfa.SendKeys(parametro);
                Thread.Sleep(Keanu.Randy(1));
                ElementCercaSfa.SendKeys(Keys.Return);
                SwitchToDefaultContent();
                Thread.Sleep(Keanu.Randy(1));
                Keanu.WaitingGame();
                return esito;
            } catch {
                Keanu.Driver.Navigate().Refresh();
                Keanu.WaitingGame();
                Keanu.WaitingGame();
                Keanu.WaitingGame();
                Thread.Sleep(Keanu.Randy(15));
                return false;
            }
        }

            private IWebElement TrovaTableResult() {
                IWebElement tableResult = null;
                while (tableResult == null) {
                    try {
                        tableResult = GetSezioneAttiva().FindElement(By.XPath(".//table[@class = 'slds-table forceRecordLayout slds-table--header-fixed slds-table--edit slds-table--bordered resizable-cols slds-table--resizable-cols uiVirtualDataTable']"));
                    } catch { }
                    if (tableResult != null)
                        break;
                    try {
                        tableResult = GetSezioneAttiva().FindElement(By.XPath(".//table[@class = 'slds-table forceRecordLayout slds-table--header-fixed slds-table--edit slds-table--bordered resizable-cols slds-table--resizable-cols uiVirtualDataTable slds-no-cell-focus']"));
                    } catch { }
                  if (tableResult != null)
                    break;
                    try
                    {
                        tableResult = GetSezioneAttiva().FindElement(By.XPath(".//table[@class = 'slds-table slds-table_header-fixed slds-table_bordered slds-table_edit slds-table_resizable-cols']"));
                    }
                    catch { }
                if (tableResult != null)
                    break;

            }
                return tableResult;
            }










        }
}

/*
                UnlockAndCloseAllTabs();

                if (FillAttivita(specificAttivita[0].RecordId, dati)) {
                    log.Debug("Fill attivita success");
                    reso = Keanu.LavRegId;
                    return reso;
                } else {
                    log.Debug("Fill attivita fail");
                    reso = Keanu.LavScartoId;
                    return reso;
                }

                */


//var test = sfaLib.GetRecordRichiesta("449828673");

//Console.WriteLine(test);



//List<Attivita> specificAttivita = new List<Attivita>();
//specificAttivita = tempAtt.Where(x => x.Tipo.ToUpper().Equals("DMS - DOC DA CORRELARE")).ToList();

//if(specificAttivita.Count >= 2) {
//Put in sospeso for now, need to manage all of the atts, not the only first one
//return 777;
//}





/*
Keanu.Driver.Navigate().GoToUrl($"https://enelcrmt.lightning.force.com/lightning/r/wrts_prcgvr__Activity__c/{specificAttivita[0].RecordId}/view");
Keanu.WaitingGame();
WaitSpecificPage("Data Inizio");

Attivita att = (Attivita)sfaLib.GetRecord(specificAttivita[0].RecordId, SFALibrary.Helpers.Utility.RicercaAttivitaName);
v.AttivitaSfa = att.Numero;
log.Debug(v.AttivitaSfa);

if (!ClickButtonByName("Modifica"))
{
    reso = Keanu.LavScartoId;
    return reso;
}

if (!AssociaServizioPod("99999999999999"))
{
    reso = Keanu.LavScartoId;
    return reso;
}

if (!SetTripletta(v.Causale, v.Descrizione, v.Specifica))
{
    reso = Keanu.LavScartoId;
    return reso;
}

if (!CaricaCampiSearch("COLLEGAMENTOCliente", dati.CodiceAnagraficaCliente))
{
    log.Info($"Problemi nell'associare il cliente con {dati.CodiceAnagraficaCliente} - proviamo con {dati.IdRecordSiebel}");
    if (!CaricaCampiSearch("COLLEGAMENTOCliente", dati.IdRecordSiebel))
    {
        log.Error("Problemi nell'associare il cliente");
        reso = Keanu.LavScartoId;
        return reso;
    }
}

if (Keanu.LavLoginId == 4977 || Keanu.LavLoginId == 5056)
{
    if (!SetCommento("CONTRATTO DA PE"))
    {
        reso = Keanu.LavScartoId;
        return reso;
    }
}
else
{
    if (!SetCommento("ALTRO DOCUMENTO DA PE"))
    {
        reso = Keanu.LavScartoId;
        return reso;
    }
}

if (!CompilaCampi("* Status", "FATTO", TipoCampo.COMBO, false, false))
{
    if (!CompilaCampi("* Stato", "FATTO", TipoCampo.COMBO, false, false))
    {
        reso = Keanu.LavScartoId;
        return reso;
    }
}

if (!ClickButtonByName("Salva"))
{
    reso = Keanu.LavScartoId;
    return reso;
}

WaitSpecificPage("Data Inizio");

reso = Keanu.LavRegId;
return reso;
*/