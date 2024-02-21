using javax.xml.crypto;
using log4net;
using MARCUS.Helpers;
using MARCUS.Utils;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Smart.Gravity.Model;
using Smart.Gravity.Model.GravityFile;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace MARCUS.Marcosi
{
    class EE112
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EE112));
        public Keanu Keanu { get; set; }
        public SqlQuery query = new SqlQuery();

        public EE112(Keanu keanu)
        {
            this.Keanu = keanu;
        }

        //private InfoCascata riferimentoCorrente;
        private Cascata riferimentoCorrenteGravity;
        private GravityRowFile ElementoRowFile;
        private GravityApiHelper Grav = new GravityApiHelper();

        public List<Records.ElencoRisultati> ListaElencoRisultati = null;
        public List<Records.ListaLetture> ListaLetture = null;
        public List<Records.ListaLetture> RealeLetture = null;
        public List<Records.ListaLetture> TempRealeLetture = null;
        public List<Records.ArchivioScartiLavorabili> ScartiList = null;
        public List<Records.ArchivioScartiLavorabili> RiabbinabileList = null;
        public List<Records.ArchivioScartiLavorabili> AllScartiList = null;
        public List<Records.ArchivioScartiLavorabili> AutoletturaScartiList = null;
        public List<Records.ArchivioScartiLavorabili> SpostaList = null;
        public List<Records.Flussi> Flussi = null;
        public List<Records.Flussi> SmisListFlussi = null;
        public List<Records.Flussi2G> Flussi2g = null;
        public List<Records.Flussi2G> SmisList = null;
        public List<Records.Flussi2G> TempRealeLettureWichHasToBeModified = null;
        public List<Records.Apparecchiature> ListaApparecchiature = null;
        public List<Records.PostMalone> ListaPostMalone = null;
        public List<Records.AnagraficaTecnica> ListaAnagraficaTecnica = null;

        #region Request
        public IList<Records.Flussi> requestFlussi = null;
        public IList<Records.Flussi2G> requestFlussi2g = null;
        #endregion

        public Records.VariablesEE112DPC v = null;

        const string filename = @"\\\\baltikpdc\PUBLIC\DB\MARCUS\duplicati.txt";
        const string reattiva = @"\\\\baltikpdc\PUBLIC\DB\MARCUS\reattiva.txt";
        const string speshal = @"\\\\baltikpdc\PUBLIC\DB\MARCUS\speshal.txt";
        const string postSalesAnagraficaKoment = "ASSENTE REGISTRAZIONE RIMOZIONE POST SALES/ANAG.TECNICA";

        private readonly double potete = 15;
        private bool needToReloadListaLetture = true;
        private bool chiaviDuplicate;
        private bool commercialeCheckedAndItIsNotOreFreeSubscription;
        private bool commercialeIsOreFreeSubscription;
        private bool annullato;
        private int annullatoCount;
        private int scartiListCountBefore;
        private int scartiListCountAfter;
        private int fattoReso;
        private string reason;
        private bool needToHaveReason;
        private bool breakFromSubscription;
        private bool matPower;
        private bool atleastOneSmisHasBeenModified;
        private bool finishInLavorazioneAfterMatPower;
        private bool ripristinaPower;//DOESN'T COUNT AS MAT POWER
        private string dataAttivazioneMinusOne;
        private string dataAttivazioneStandard;
        private string motivazioneFromUpperSmis;
        private string numeroUtente;
        private int upeCounter;
        private bool needToAnnulla;
        private bool areThereMoreRowsAfterLastSmis;
        private string smisType;
        #region POST SALES & ANAGRAFICA TECNICA
        private string dataSostituzione;
        private bool sostituzioneIsPresentInPostAnagrafica;
        private bool sostituzioneIsChecked;
        private bool presaInCaricoScarto;
        #endregion

        public int ChooseFattoReso()
        {
            int reso = 0;//temp
            log.Warn($"Total upeCounter {upeCounter}");

            if (matPower) {
                reso = 46410;
                log.Info($"Inserimento cambio (MAT Power)");
                return reso;
            }

            if (upeCounter <= 2) {
                reso = 46408;
                log.Info($"Inserimento manuale (fino a 2 letture)");
                return reso;
            }

            if (upeCounter >= 3 && upeCounter <=10) {
                reso = 46409;
                log.Info($"Inserimento manuale ( 3 <= letture <= 10)");
                return reso;
            }

            if (upeCounter >= 11) {
                reso = 48361;
                log.Info($"Inserimento manuale (superiore a 10 letture)");
                return reso;
            }

            if (reso.Equals(0)) {
                log.Error("CAN'T USE OLD FATTO RESO, KILL MARCUS MANUALLY");
                MessageBox.Show("CAN'T USE OLD FATTO RESO, KILL MARCUS MANUALLY");
            }

            /*
            if (matPower)
            {
                reso = 46410;
                log.Info($"Inserimento cambio (MAT Power)");
            }
            //else if (allAreRiabbinabile)
            //{
            //    reso = 46407;
            //    log.Info($"Ripristino/Riabbina OK");
            //}
            else if (upeCounter <= 2)
            {
                reso = 46408;
                log.Info($"Inserimento manuale (fino a 2 letture)");
            }
            else if (upeCounter >= 3)
            {
                reso = 46409;
                log.Info($"Inserimento manuale (superiore a 2 letture)");
            }
            if (reso.Equals(0))
            {
                log.Error("CAN'T USE OLD FATTO RESO, KILL MARCUS MANUALLY");
                MessageBox.Show("CAN'T USE OLD FATTO RESO, KILL MARCUS MANUALLY");
            }

            */




            return reso;
        }

        public bool Flow()
        {
            Keanu.KillChromeWebDriver();
            Keanu.ScartoCount = 0;
            Keanu.TotalCnt = 0;
            if (!Keanu.PepperYourChrome(Keanu.LoginNEXT, Keanu.PassNEXT, "https://next.enelint.global/next-online/", "INSERIRE ALMENO 3 CARATTERI", false))
                return false;

            Grav.SetGravityCredential(Keanu.LoginAGENTE, Keanu.PassAGENTE);
            Grav.GravitySetup();
            Grav.GravityConnection(Keanu.LavLoginId);

            while (true)
            {
                Keanu.TimyNeinItalian();

                if (Keanu.StartStop == false)
                    break;

                chiaviDuplicate = false;
                commercialeCheckedAndItIsNotOreFreeSubscription = false;
                commercialeIsOreFreeSubscription = false;
                annullato = false;
                annullatoCount = 0;
                scartiListCountBefore = 0;
                scartiListCountAfter = 0;
                fattoReso = 0;
                reason = "";
                needToHaveReason = false;
                breakFromSubscription = false;//TO BREAK FROM DOUBLE LOOP IN ANAGRAFICA
                matPower = false;
                atleastOneSmisHasBeenModified = false;
                finishInLavorazioneAfterMatPower = false;
                ripristinaPower = false;
                dataAttivazioneMinusOne = "2010-12-31";
                dataAttivazioneStandard = "2010-12-31";
                numeroUtente = "";
                upeCounter = 0;
                needToAnnulla = false;
                areThereMoreRowsAfterLastSmis = false;
                smisType = "";
                sostituzioneIsPresentInPostAnagrafica = true;
                sostituzioneIsChecked = false;
                dataSostituzione = "";

                if (Keanu.TotalCnt.Equals(9))
                {
                    //log.Info("Restarting");
                    //Keanu.KillChromeWebDriver();
                    //Keanu.ScartoCount = 0;
                    //Keanu.TotalCnt = 0;
                    //if (!Keanu.PepperYourChrome(Keanu.LoginNEXT, Keanu.PassNEXT, "https://next.enelint.global/next-online/", "INSERIRE ALMENO 3 CARATTERI", false))
                    //    return false;

                    log.Info("Restarting");
                    Keanu.KillChromeWebDriver();
                    //Grav.GravityConnectionClose();
                    Keanu.ScartoCount = 0;
                    Keanu.TotalCnt = 0;
                    if (!Keanu.PepperYourChrome(Keanu.LoginNEXT, Keanu.PassNEXT, "https://next.enelint.global/next-online/", "INSERIRE ALMENO 3 CARATTERI", false))
                        return false;

                    Grav.SetGravityCredential(Keanu.LoginAGENTE, Keanu.PassAGENTE);
                    //Grav.GravitySetup();
                    Grav.GravityConnection(Keanu.LavLoginId);
                }

                try
                {
                    //#region Manual row updater
                    //var p = new List<string>();
                    //p.Add("IT001E78710208_774_17122021,352606239");
                    //p.Add("IT001E74444133_771_17122021,352606236");
                    //p.Add("IT001E19449495_769_17122021,352606234");
                    //foreach (var item in p)
                    //{
                    //    var sssss = item.Split(',');
                    //    ElementoRowFile = null;
                    //    ElementoRowFile = Grav.GetElementRowFileByIdCascata(Convert.ToInt32(sssss[1]));
                    //    ElementoRowFile.Riferimento = sssss[0];
                    //    Thread.Sleep(250);
                    //    bool esitoSaveRow = Grav.SaveJsonRowValueByIdCascataByObject(ElementoRowFile);
                    //    if (!esitoSaveRow)
                    //    {
                    //        log.Error("Errore durante l'inserimento della lavorazione!");
                    //        break;
                    //    }
                    //}
                    //#endregion

                    v = new Records.VariablesEE112DPC();
                    //grav.GravityConnection(Keanu.LavLoginId);
                     riferimentoCorrenteGravity = Grav.GravityScoda();
                    if (riferimentoCorrenteGravity == null)
                        break;

                    ElementoRowFile = null;
                    ElementoRowFile = Grav.GetElementRowFileByIdCascata(riferimentoCorrenteGravity.Id);
                    ElementoRowFile.Riferimento = riferimentoCorrenteGravity.Dettaglio;
                    if (ElementoRowFile == null)
                    {
                        Keanu.TimeToSospesoType = 8;
                        Grav.GravitySospendi(Keanu.TimeToSospesoType);
                        log.Debug($"{riferimentoCorrenteGravity.Dettaglio} SOSPESO");
                        Keanu.Bad.Sospeso++;
                        Keanu.TotalCnt++;
                        Keanu.TimeToSospeso = false;
                        continue;
                        //break;
                    }

                    v.Riferimento = riferimentoCorrenteGravity.Dettaglio;
                    string[] split = v.Riferimento.Split('_');
                    v.Pod = split[0];
                    v.Data = split[2];

                    log.Info("******");
                    log.Info(v.Riferimento);
                }
                catch
                {
                    log.Error("GravityScoda() fail");
                    break;
                }

                if (Workaholic())
                {
                    try
                    {
                        bool esitoSaveRow = Grav.SaveJsonRowValueByIdCascataByObject(ElementoRowFile);
                        if (!esitoSaveRow)
                        {
                            Keanu.TimeToSospesoType = 8;
                            Grav.GravitySospendi(Keanu.TimeToSospesoType);
                            log.Debug($"{riferimentoCorrenteGravity.Dettaglio} SOSPESO");
                            Keanu.Bad.Sospeso++;
                            Keanu.TotalCnt++;
                            Keanu.TimeToSospeso = false;
                            break;
                        }
                        else
                        {
                            fattoReso = ChooseFattoReso();
                            Grav.GravityRegistra(riferimentoCorrenteGravity, "OK", fattoReso);
                            log.Debug($"{riferimentoCorrenteGravity.Dettaglio} FATTO");
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    if (chiaviDuplicate)
                        File.AppendAllText(filename, $"{v.Riferimento} {v.Pod} {DateTime.Today.ToShortDateString()}" + Environment.NewLine);
                    Keanu.Bad.Fatto++;
                    Keanu.TotalCnt++;
                    Keanu.ScartoCount = 0;
                    if (needToHaveReason)
                        File.AppendAllText(reattiva, $"{v.Riferimento} {reason} FATTO {DateTime.Today.ToShortDateString()}" + Environment.NewLine);
                }
                else if (Keanu.TimeToRestart)
                {
                    log.Info("Restarting");
                    Keanu.KillChromeWebDriver();
                    Keanu.TotalCnt = 0;
                    if (!Keanu.PepperYourChrome(Keanu.LoginNEXT, Keanu.PassNEXT, "https://next.enelint.global/next-online/", "INSERIRE ALMENO 3 CARATTERI", false))
                        break;
                    Keanu.TimeToRestart = false;
                }
                else if (presaInCaricoScarto) //Ja bija presa in carico errors, scartos ar komento presa in carico errore // JAUNAIS labotais 27/08/2023
                {
                    
                    int scartoReso = Keanu.LavScartoId;
                    string koment = "Presa in carico errore";
                    Grav.GravityRegistra(riferimentoCorrenteGravity, koment, scartoReso);
                    log.Debug($"{riferimentoCorrenteGravity.Dettaglio} {koment}");
                    Keanu.Bad.Scarto++;
                    Keanu.TotalCnt++;
                    Keanu.ScartoCount++;
                    presaInCaricoScarto = false;
                    //break;
                }
                else if (Keanu.TimeToSospeso)
                {
                    //2 log.Info($"FLUSSI2G + ORE FREE/SUBSCRIPTION");
                    //3 log.Error($"F are equal, R are null, potenza > 15");
                    //4 log.Info($"Reale potenza {reale.Potenza} > 15");
                    //5 log.Info($"One of F reale is bigger than scarti");
                    //6 log.Info($"Wrong turn 6");
                    //7 log.Info($"No reale below {scarti.DataLettura.ToShortDateString()}");
                    //8 log.Debug($"MAT Power");
                    //9 log.Debug($"potenzaDTIsNull");

                    Grav.GravitySospendi(Keanu.TimeToSospesoType);
                    log.Debug($"{riferimentoCorrenteGravity.Dettaglio} SOSPESO");
                    Keanu.Bad.Sospeso++;
                    Keanu.TotalCnt++;
                    Keanu.TimeToSospeso = false;
                    continue;
                }
                else
                {
                    try
                    {
                        if (ripristinaPower)//CHECK IF DATA SOSTITUZIONE IS IN POST SALES & ANAGRAFICA TECNICA
                        {
                            if (!sostituzioneIsChecked)
                                CheckPostSalesAnagraficaTecnica();
                        }

                        string koment = "SCARTO";
                        int scartoReso = Keanu.LavScartoId;

                        

                        if (!sostituzioneIsPresentInPostAnagrafica)
                        {
                            koment = postSalesAnagraficaKoment;
                            scartoReso = Keanu.LavScartoNewReso;//Caso Complesso/Non Gestibile
                            File.AppendAllText(speshal, $"{v.Riferimento} {v.Pod} {DateTime.Today.ToShortDateString()} {postSalesAnagraficaKoment}" + Environment.NewLine);
                        }

                        bool esitoSaveRow = Grav.SaveJsonRowValueByIdCascataByObject(ElementoRowFile);
                        if (!esitoSaveRow)
                        {
                            Keanu.TimeToSospesoType = 8;
                            Grav.GravitySospendi(Keanu.TimeToSospesoType);
                            log.Debug($"{riferimentoCorrenteGravity.Dettaglio} SOSPESO");
                            Keanu.Bad.Sospeso++;
                            Keanu.TotalCnt++;
                            Keanu.TimeToSospeso = false;
                            break;
                        }
                        else
                        {
                            Grav.GravityRegistra(riferimentoCorrenteGravity, koment, scartoReso);
                            log.Debug($"{riferimentoCorrenteGravity.Dettaglio} {koment}");
                        }
                    }
                    catch (Exception)
                    {
                        log.Error("Errore durante l'inserimento della lavorazione!");
                        break;
                    }

                    //try
                    //{
                    //    //if (matPower || ripristinaPower)//CHECK IF DATA SOSTITUZIONE IS IN POST SALES & ANAGRAFICA TECNICA
                    //    //{
                    //    //    if (!sostituzioneIsChecked)
                    //    //        CheckPostSalesAnagraficaTecnica();
                    //    //}

                    //    if (ripristinaPower)//CHECK IF DATA SOSTITUZIONE IS IN POST SALES & ANAGRAFICA TECNICA
                    //    {
                    //        if (!sostituzioneIsChecked)
                    //            CheckPostSalesAnagraficaTecnica();
                    //    }

                    //    string koment = "SCARTO";
                    //    int scartoReso = Keanu.LavScartoId;

                    //    if (!sostituzioneIsPresentInPostAnagrafica)
                    //    {
                    //        koment = postSalesAnagraficaKoment;
                    //        scartoReso = Keanu.LavScartoNewReso;//Caso Complesso/Non Gestibile
                    //        File.AppendAllText(speshal, $"{v.Riferimento} {v.Pod} {DateTime.Today.ToShortDateString()} {postSalesAnagraficaKoment}" + Environment.NewLine);
                    //    }

                    //    Grav.GravityRegistra(riferimentoCorrenteGravity, koment, scartoReso);
                    //    //bool esitoSaveRow = Grav.SaveRowFile(riferimentoCorrenteGravity.Id, riferimentoCorrenteGravity.Dettaglio, _jsonRowValue);
                    //    bool esitoSaveRow = Grav.SaveJsonRowValueByIdCascataByObject(ElementoRowFile);
                    //    if (!esitoSaveRow)
                    //    {
                    //        log.Error("Errore durante l'inserimento della lavorazione!");
                    //        break;
                    //    }
                    //    log.Debug($"{riferimentoCorrenteGravity.Dettaglio} {koment}");
                    //}
                    //catch (Exception)
                    //{
                    //    log.Error("Errore durante l'inserimento della lavorazione!");
                    //    break;
                    //}

                    Keanu.Bad.Scarto++;
                    Keanu.TotalCnt++;
                    Keanu.ScartoCount++;
                    if (needToHaveReason)
                        File.AppendAllText(reattiva, $"{v.Riferimento} {reason} SCARTO {DateTime.Today.ToShortDateString()}" + Environment.NewLine);
                    if (Keanu.ScartoCount.Equals(15))
                    {
                        log.Error($"{Keanu.ScartoCount} TOO MUCH SCARTO");
                        break;
                    }
                }
            }

            Keanu.KillChromeWebDriver();
            Grav.GravityConnectionClose();
            return false;
        }

        private bool Workaholic()
        {
            //if (v.Data.Equals("20012021"))
            //{
            //    log.Info($"{v.Data}");
            //    Keanu.ScartoCount = 0;
            //    Keanu.TotalCnt = 0;
            //    return false;
            //}

            //if (v.Riferimento.Equals("IT001E92480517_1585_13092021"))
            //    return false;

            if (v.Pod.Contains("IT020E13027576")) // gadijums kad nedrikst apstradat klientu. informet par sho gadijumu Adriano
            {
                Keanu.TimeToSospeso = true;
                Keanu.TimeToSospesoType = 3;
                log.Error($"SOSPESO POD, MAIL ADRIANO");
                return false;
            }
            


            try
            {
                if (!GoToListaLettura())
                    return false;

                int cnt = 0;
                while (!Keanu.Driver.PageSource.ToString().Contains("SPOSTA") && cnt < 10)
                {
                    GoTo("Scarti");
                    if (cnt.Equals(5))
                        return false;
                    cnt++;
                }

                if (Keanu.Driver.PageSource.ToString().Contains("NESSUN RISULTATO TROVATO"))
                {
                    log.Error($"NESSUN RISULTATO TROVATO");
                    if (!CheckFlussi2GCountNanoTech())
                        return false;
                    return true;
                }

                if (!FillListaScarti())
                {
                    Keanu.TimeToRestart = true;
                    return false;
                }

                #region REMOVE DUPLICATES
                //ScartiList = ScartiList.GroupBy(x => x.DataLettura).Select(y => y.FirstOrDefault()).ToList();
                var TmpScartiList = new List<Records.ArchivioScartiLavorabili>();
                SpostaList = new List<Records.ArchivioScartiLavorabili>();

                string itemDataFirst = "";
                foreach (var item in ScartiList)
                {
                    string tmpData = item.DataLettura.ToShortDateString();
                    if (!tmpData.Equals(itemDataFirst))
                        TmpScartiList.Add(item);
                    else
                    {
                        if (tmpData.Equals(itemDataFirst))
                        {
                            log.Info($"{tmpData} duplicate DataLettura, add to SpostaList");
                            SpostaList.Add(item);
                            continue;
                        }
                        log.Info($"2 Scarti with same date");
                        return false;
                    }
                    itemDataFirst = tmpData;
                }

                ScartiList = TmpScartiList;
                #endregion

                scartiListCountBefore = AllScartiList.Count() - AutoletturaScartiList.Count();

                Indietroer();

                //SpostaList = new List<Records.ArchivioScartiLavorabili>();
                needToReloadListaLetture = true;
                int inLavorazioneCunt = 0;

                ScartiList.Reverse();

                foreach (var scarti in ScartiList)
                {
                    inLavorazioneCunt++;
                    if (needToReloadListaLetture)
                    {
                        if (!FillListaLetture())
                            return false;
                    }
                    log.Debug($"In lavorazione {inLavorazioneCunt}/{ScartiList.Count()} {scarti.DataLettura.ToShortDateString()}");
                    log.Debug($"{scarti.F1} {scarti.F2} {scarti.F3} - {scarti.R1} {scarti.R2} {scarti.R3} - {scarti.Potenza}");
                    if (!LoopLetture(scarti))
                    {
                        if (commercialeCheckedAndItIsNotOreFreeSubscription)
                        {
                            if (breakFromSubscription)//TO BREAK FROM DOUBLE LOOP IN ANAGRAFICA
                                return false;//SOSPESO
                            if (!FillListaLetture())
                                return false;
                            if (!LoopLetture(scarti))
                                return false;//SCARTO
                            else
                                continue;//GET NEXT R BUT NOW ANAGRAFICA COMMERCIALE IS TRUE (CHECKED)
                        }
                        return false;//SCARTO
                    }
                    if (finishInLavorazioneAfterMatPower || atleastOneSmisHasBeenModified)//FINISH MODIFICA/INSERISCI SCARTO IN LAVORAZIONE, AFTER SUCCESSFUL MATPOWER
                    {
                        //if (ripristinaPower)
                        //{
                        //    Keanu.TimeToRestart = true;
                        //    log.Debug($"Ripristinato, restart to check if there are scarti now");
                        //    return false;
                        //}
                        finishInLavorazioneAfterMatPower = false;//NEXT SCARTO DOESN'T NEED TO GO IN THERE, SO FALSE
                        atleastOneSmisHasBeenModified = false;
                        if (!GoToListaLettura())
                            return false;
                        if (!FillListaLetture())
                            return false;
                        if (!LoopLetture(scarti))
                            return false;//SCARTO
                        else
                            continue;
                    }
                }

                if (!FinishingTach())
                    return false;

                if (!CheckFlussi2GCountNanoTech())
                    return false;

                if (matPower || ripristinaPower)//CHECK IF DATA SOSTITUZIONE IS IN POST SALES & ANAGRAFICA TECNICA
                {
                    if (!CheckPostSalesAnagraficaTecnica())
                        return false;
                }

                return true;
            }
            catch
            {
                Keanu.TimeToRestart = true;
                log.Error($"General Kenobi");
                return false;
            }
        }

        private bool CheckPostSalesAnagraficaTecnica()
        {
            try
            {
                //STANDART IS sostituzioneIsPresentInPostAnagrafica = true;
                log.Info($"Check POST SALES & ANAGRAFICA TECNICA");
                dataSostituzione = SmisList.FirstOrDefault().DataMisura.ToShortDateString();

                #region POST SALES
                //https://next.enelint.global/api-next-online/api/v1/protected/scarti/processPostSales?entityId=IT001E61659313.2013-10-21.9999-12-31
                //processDate: "2021-07-05"

                bool postal = false;
                var postSalesData = GetListaPostSales("https://next.enelint.global/api-next-online/api/v1/protected/scarti/processPostSales?entityId=" + v.Pod + "." + dataAttivazioneStandard + ".9999-12-31");
                foreach (var item in postSalesData)
                {
                    string defTempDate = item.DataProcesso.ToString();
                    string[] split = defTempDate.ToString().Split(' ');
                    if (split[0].Equals(dataSostituzione))
                    {
                        log.Debug($"Post Sales contains Data SOSTITUZIONE");
                        postal = true;
                        break;
                    }
                }
                #endregion

                #region ANAGRAFICA TECNICA
                //https://next.enelint.global/api-next-online/api/v1/protected/power/detailTecnica?entityId=IT001E61659313.2013-10-21.9999-12-31
                //dataInizioValiditaMisuratore: "2021-07-05"

                bool anagrafal = false;
                var anagraficaTecnicaData = GetListaAnagraficaTecnica("https://next.enelint.global/api-next-online/api/v1/protected/power/detailTecnica?entityId=" + v.Pod + "." + dataAttivazioneStandard + ".9999-12-31");
                foreach (var item in anagraficaTecnicaData)
                {
                    string defTempDate = item.DataProcesso.ToString();
                    string[] split = defTempDate.ToString().Split(' ');
                    if (split[0].Equals(dataSostituzione))
                    {
                        log.Debug($"Anagrafica Tecnica contains Data SOSTITUZIONE");
                        anagrafal = true;
                        break;
                    }
                }
                #endregion

                sostituzioneIsChecked = true;

                if (postal && anagrafal)
                {
                    log.Debug($"SOSTITUZIONE check done");
                    return true;
                }
                else
                {
                    log.Error($"SOSTITUZIONE check fail");
                    sostituzioneIsPresentInPostAnagrafica = false;
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool FinishingTach()
        {
            int cnt = 0;
            while (!Keanu.Driver.PageSource.ToString().Contains("SPOSTA") && cnt < 10)
            {
                GoTo("Scarti");
                cnt++;
            }

            if (Keanu.Driver.PageSource.ToString().Contains("NESSUN RISULTATO TROVATO"))//WHEN MACRO MODIFIED LISTA LETTURE, IT WIPED SCARTI & NOW THEY ARE EMPTJI
            {
                log.Info("All sposted");
                return true;
            }

            if (!SelectWhatToSposta())
                return false;

            if (!Sposter())
                return false;

            if (Keanu.TimeToSospeso)
                return false;

            //if (matPower || ripristinaPower)
            //{
            //    if (Keanu.Driver.PageSource.ToString().Contains("NESSUN RISULTATO TROVATO"))
            //    {
            //        log.Info("All sposted");
            //        return true;
            //    }
            //    if (!Riabbinatore())
            //        return false;
            //}

            if (Keanu.Driver.PageSource.ToString().Contains("NESSUN RISULTATO TROVATO"))
            {
                log.Info("All sposted");
                return true;
            }
            else if (chiaviDuplicate)
            {
                log.Info("Something is not sposted, CHIAVI DUPLICATE");
                return true;
            }
            else
            {
                log.Info("Something is not sposted");
                return false;
            }
        }

        private bool CheckFlussi2GCountNanoTech()
        {
            log.Info($"CheckFlussi2GCountNanoTech()");

            //if (matPower)
            if (matPower || ripristinaPower)
            {
                log.Info($"matPower || ripristinaPower true");
                return true;
            }
            try
            {
                if (!GoToListaLettura())
                    return false;

                requestFlussi2g = GetListaFlussi2GPower("https://next.enelint.global/api-next-online/api/v1/protected/" + v.Pod + "/measures/mm2g?startDate=" + dataAttivazioneMinusOne + "&endDate=9999-12-31&aggregationMode=MONTHLY&_hidePageLoading=false");
                if (requestFlussi2g.Count().Equals(0))//10062021
                    return true;//NESSUN RISULTATO TROVATO

                List<Records.Flussi2G> tmpSmisList = new List<Records.Flussi2G>();
                foreach (var item in requestFlussi2g)
                {
                    if (item.CodiceFlusso.Equals("SMIS"))
                    {
                        tmpSmisList.Add(item);
                        log.Debug($"{item.DataMisura.ToShortDateString()} {item.Stato} {item.Repository} {item.StatoAbb} {item.Motivazione} {item.Esito} {item.CodiceEsito}");
                    }
                }

                log.Info($"{tmpSmisList.Count()} FLUSSI2G SMIS Before");

                if (tmpSmisList.Count() > 3)
                {
                    log.Info($"More than 3 SMIS");
                    return false;
                }

                SmisList = new List<Records.Flussi2G>();
                string itemDataFirst = "";
                foreach (var item in tmpSmisList)
                {
                    string tmpData = item.DataMisura.ToShortDateString();
                    if (!tmpData.Equals(itemDataFirst))
                        SmisList.Add(item);
                    else
                    {
                        if (tmpData.Equals(itemDataFirst) && item.Repository.Equals("DISCARD"))
                        {
                            log.Info($"Don't add SCARTO to SmisList with same data");
                            continue;
                        }
                        log.Info($"2 SMIS with same date");
                        return false;
                    }
                    itemDataFirst = tmpData;
                }

                log.Info($"{SmisList.Count()} FLUSSI2G SMIS After");

                if (SmisList.Count() > 2)
                {
                    string tmpMonth = SmisList[0].MeseAnno;
                    int monthCounter = 0;
                    foreach (var item in SmisList)
                    {
                        string itemTmpMonth = item.MeseAnno;
                        if (tmpMonth.Equals(itemTmpMonth))
                            monthCounter++;
                        else
                            monthCounter--;
                        tmpMonth = itemTmpMonth;
                    }

                    if (monthCounter > 2)
                    {
                        log.Info($"More than 2 SMIS in one month/SCARTO");
                        return false;
                    }
                }

                if (SmisList.Count().Equals(0))
                {
                    log.Debug($"SMIS 0");
                    return true;
                }
                else
                {
                    needToReloadListaLetture = true;
                    int inLavorazioneCunt = 0;
                    foreach (var smis in SmisList)
                    {
                        if (matPower)//15062021
                            break;

                        inLavorazioneCunt++;
                        if (needToReloadListaLetture)
                        {
                            if (!FillListaLetture())
                                return false;
                        }
                        log.Debug($"In lavorazione {inLavorazioneCunt}/{SmisList.Count()} {smis.DataMisura.ToShortDateString()}");
                        log.Debug($"{smis.F1} {smis.F2} {smis.F3} - {smis.R1} {smis.R2} {smis.R3}");
                        if (!LoopSMIS(smis))
                            return false;//SCARTO/SOSPESO
                    }
                }

                if (matPower)//15062021
                {
                    if (SmisList.Count() > 0)
                    {
                        #region ROW CHEK
                        DateTime lastSmisData = SmisList.LastOrDefault().DataMisura;
                        foreach (var item in requestFlussi2g)
                        {
                            DateTime listData = item.DataMisura;
                            if (listData < lastSmisData)
                            {
                                if (item.CodiceFlusso.Equals("SMIS"))//SKIP SMIS THAT WAS SCARTO
                                    continue;
                                areThereMoreRowsAfterLastSmis = true;
                                break;
                            }
                        }

                        int scartoCnt = 0;
                        int onlineCnt = 0;
                        foreach (var item in SmisList)
                        {
                            if (item.Repository.Equals("DISCARD") || item.Repository.Equals("PARK"))//OR "PARK"
                                scartoCnt++;
                            if (item.Repository.Equals("ONLINE") && item.StatoAbb.Equals("A"))
                                onlineCnt++;
                        }
                        if (onlineCnt.Equals(SmisList.Count()))
                        {
                            bool needToInsertPeriodicaAgain = false;
                            bool dateIsInMidOfMonth = false;
                            var rr = SmisList[0];
                            if (rr.DataMisura.Day >= 2 && rr.DataMisura.Day <= 29)
                            {
                                dateIsInMidOfMonth = true;
                                log.Debug($"Data is in the middle of month - {rr.DataMisura.ToShortDateString()}");
                            }

                            log.Debug($"Both SMIS are VALIDA and A");
                            log.Debug($"Cancella/Ripristina");
                            smisType = "cancellaRipristina";

                            #region CANCELLA EVERY SMIS
                            needToReloadListaLetture = true;
                            foreach (var smis in SmisList)
                            {
                                if (needToReloadListaLetture)
                                {
                                    if (!FillListaLetture())
                                        return false;
                                    needToReloadListaLetture = false;
                                }
                                string smisData = smis.DataMisura.ToShortDateString();
                                foreach (var all in ListaLetture)
                                {
                                    string allData = all.Data.ToShortDateString();
                                    if (smisData == allData)
                                    {
                                        if (allData == rr.DataMisura.ToShortDateString() && dateIsInMidOfMonth)//MIDMONTH
                                            needToInsertPeriodicaAgain = true;

                                        if (!ClickAzioniCancella(all))
                                        {
                                            log.Error($"Couldn't Cancella SMIS");
                                            return false;
                                        }
                                        needToReloadListaLetture = true;
                                    }
                                }
                            }
                            #endregion

                            #region RIPRISTINA
                            var raw = SmisList.FirstOrDefault().DataMisura.ToString();
                            string[] dateTimeSplit = raw.ToString().Split(' ');
                            string[] onlyDateSplit = dateTimeSplit[0].ToString().Split('/');
                            string onlyTime = dateTimeSplit[1].ToString().Replace(":", "");
                            var DataRipristina = onlyDateSplit[2] + onlyDateSplit[1] + onlyDateSplit[0] + onlyTime;
                            //var DataRipristina = SmisList.FirstOrDefault().DataMisura.Year.ToString() + SmisList.FirstOrDefault().DataMisura.Month.ToString() + SmisList.FirstOrDefault().DataMisura.Day.ToString() + SmisList.FirstOrDefault().DataMisura.Hour.ToString() + SmisList.FirstOrDefault().DataMisura.Minute.ToString() + "00";
                            //https://next.enelint.global/api-next-online/api/v1/protected/IT001E22204926/measures/mm2g/20210504000000/0/restore OK LINK
                            var UrlRequestRipristina = "https://next.enelint.global/api-next-online/api/v1/protected/" + v.Pod + "/measures/mm2g/" + DataRipristina + "/0/restore";
                            if (Ripristina(UrlRequestRipristina, out string errore))
                            {
                                log.Info("Ripristinato");
                                ripristinaPower = true;
                                finishInLavorazioneAfterMatPower = true;

                                //NEED TO CHECK IF SMIS HAS BEEN ADDED
                                needToReloadListaLetture = true;
                                int inLavorazioneCunt = 0;
                                foreach (var smis in SmisList)
                                {
                                    if (matPower)
                                    {
                                        log.Debug($"Didn't find SMIS after Ripristina");
                                        return false;
                                    }

                                    inLavorazioneCunt++;
                                    if (needToReloadListaLetture)
                                    {
                                        if (!FillListaLetture())
                                            return false;
                                    }
                                    log.Debug($"In lavorazione {inLavorazioneCunt}/{SmisList.Count()} {smis.DataMisura.ToShortDateString()}");
                                    log.Debug($"{smis.F1} {smis.F2} {smis.F3} - {smis.R1} {smis.R2} {smis.R3}");
                                    if (!LoopSMIS(smis))
                                    {
                                        log.Debug($"Didn't find SMIS after Ripristina");
                                        return false;
                                    }
                                }

                                matPower = false;//IF RIPRISTINATO THEN IT IS NOT matPower

                                if (needToInsertPeriodicaAgain)
                                {
                                    Keanu.Driver.Navigate().Refresh();
                                    WaitLoadPageEE12();
                                    File.AppendAllText(speshal, $"{v.Riferimento} {v.Pod} {DateTime.Today.ToShortDateString()} needToInsertPeriodicaAgain = true" + Environment.NewLine);
                                    if (!InserisciLetturaWithVariablesFromFlussi2Gg(rr))
                                    {
                                        breakFromSubscription = true;
                                        return false;
                                    }
                                }

                                return true;
                            }
                            else
                            {
                                log.Info(errore);
                                return false;
                            }
                            #endregion
                        }
                        if (scartoCnt.Equals(SmisList.Count()))
                        {
                            log.Debug($"Both SMIS are SCARTO");
                            if (areThereMoreRowsAfterLastSmis)
                            {
                                log.Debug($"More rows after last SMIS, 2G2G");//TOO HARD, SCARTO
                                smisType = "2G2G";
                                if (!Cambio2G2G())
                                    return false;
                            }
                            else
                            {
                                log.Debug($"No rows after last SMIS, 1G2G");
                                smisType = "1G2G";
                                if (!Cambio1G2G(true))//flag SI
                                    return false;
                            }
                        }
                        else
                        {
                            log.Debug($"Both SMIS are not SCARTO");
                            if (areThereMoreRowsAfterLastSmis)
                            {
                                log.Debug($"More rows after last SMIS, 2G2G");//TOO HARD, SCARTO
                                smisType = "2G2G";
                                if (!Cambio2G2G())
                                    return false;
                            }
                            else
                            {
                                log.Debug($"No rows after last SMIS, 1G2G");
                                smisType = "1G2G";

                                string defTempDate = SmisList.FirstOrDefault().DataMisura.ToString();
                                string[] split = defTempDate.ToString().Split(' ');
                                string[] split2 = split[0].ToString().Split('/');
                                string dataDecorrenza = split2[2] + "-" + split2[1] + "-" + split2[0];
                                //https://next.enelint.global/api-next-online/api/v1/protected/power/detailTecnica/IT001E66267086.2020-07-03.9999-12-31
                                string UrlRequestInfoCommerciali = "https://next.enelint.global/api-next-online/api/v1/protected/power/detailTecnica/" + v.Pod + "." + dataDecorrenza + ".9999-12-31";

                                //UNCHECK FLAG
                                if (!SetFlag2G(false, UrlRequestInfoCommerciali, "", "", "", out string dtDecorrenza2G_, out string dtMessaRegime2G_, out string dtPassaggioOrario2G_))
                                    return false;

                                if (!Cambio1G2G(false))//flag NO
                                    return false;

                                //CHEK FLAG
                                if (!SetFlag2G(true, UrlRequestInfoCommerciali, dtDecorrenza2G_, dtMessaRegime2G_, dtPassaggioOrario2G_, out string dtDecorrenza2G__, out string dtMessaRegime2G__, out string dtPassaggioOrario2G__))
                                    //if (!SetFlag2G(true, UrlRequestInfoCommerciali, SmisList.FirstOrDefault().DataMisura.ToShortDateString(), "", "", out string dtDecorrenza2G__, out string dtMessaRegime2G__, out string dtPassaggioOrario2G__))
                                    return false;
                            }
                        }
                        #endregion
                    }
                }
                return true;//Everything equal/matPower
            }
            catch
            {
                log.Info($"CheckFlussi2GCountNanoTech() fail");
                log.Debug($"MAT Power");
                return false;
            }
        }

        private bool SelectWhatToSposta()
        {
            if (!FillListaScarti())
            {
                Keanu.TimeToRestart = true;
                return false;
            }

            scartiListCountAfter = AllScartiList.Count() - AutoletturaScartiList.Count();

            int cnt = 0;
            if (AllScartiList.Count() == SpostaList.Count())
            {
                var cAll = Keanu.Driver.FindElement(By.Id("checkmarkId"));
                cAll.Click();
                WaitLoadPageEE12();
            }
            else
            {
                bool found = false;
                foreach (var scarti in AllScartiList)
                {
                    foreach (var sposta in SpostaList)
                    {
                        if (scarti.Pod == sposta.Pod &&
                            scarti.DataLettura == sposta.DataLettura &&
                            scarti.PresaInCarico == sposta.PresaInCarico &&
                            scarti.TipoLettura == sposta.TipoLettura &&
                            scarti.F1 == sposta.F1 &&
                            scarti.F2 == sposta.F2 &&
                            scarti.F3 == sposta.F3 &&
                            scarti.Potenza == sposta.Potenza &&
                            scarti.DataCaricamento == sposta.DataCaricamento &&
                            scarti.CodiceScarto == sposta.CodiceScarto)
                        {
                            cnt = 0;
                            while (true && cnt < 30)
                            {
                                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", scarti.Checkbox);
                                try
                                {
                                    scarti.Checkbox.Click();
                                    Thread.Sleep(Keanu.Randy(1));
                                    found = true;
                                    break;
                                }
                                catch
                                {
                                    cnt++;
                                    Thread.Sleep(Keanu.Randy(1));
                                }
                            }
                        }
                        if (found)
                        {
                            found = false;
                            break;
                        }
                    }
                }

                if (AutoletturaScartiList.Count() != 0)
                {
                    found = false;
                    foreach (var item in AutoletturaScartiList)
                    {
                        cnt = 0;
                        while (true && cnt < 30)
                        {
                            try
                            {
                                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", item.Checkbox);
                                item.Checkbox.Click();
                                Thread.Sleep(Keanu.Randy(1));
                                found = true;
                                break;
                            }
                            catch
                            {
                                cnt++;
                                Thread.Sleep(Keanu.Randy(1));
                            }
                        }
                        //if (found)
                        //{
                        //    found = false;
                        //    break;
                        //}
                    }
                }
            }

            if (scartiListCountBefore > scartiListCountAfter)
                log.Info("scartiListCountBefore > scartiListCountAfter");
            else if (scartiListCountBefore < scartiListCountAfter)
            {
                log.Info("scartiListCountBefore < scartiListCountAfter");
                Keanu.TimeToRestart = true;
                return false;
            }
            else
                log.Info("scartiListCountBefore == scartiListCountAfter");

            return true;
        }

        private bool Riabbinatore()
        {
            if (!FillListaScarti())
            {
                Keanu.TimeToRestart = true;
                return false;
            }

            //ScartiList.Reverse();

            try
            {
                var cAll = Keanu.Driver.FindElement(By.Id("checkmarkId"));
                cAll.Click();
                WaitLoadPageEE12();

                try
                {
                    var bRiabbina = Keanu.Driver.FindElement(By.XPath("//button//span[contains(text(), 'RIABBINA')]"));
                    bRiabbina.Click();
                    WaitLoadPageEE12();

                    var bConferma = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'Conferma')]"));
                    bConferma.Click();
                    WaitLoadPageEE12();

                    if (Keanu.Driver.PageSource.ToString().Contains("Non sono presenti scarti riabbinabili"))
                    {
                        log.Info("Non sono presenti scarti riabbinabili");
                        var bFailOk = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'OK')]"));
                        bFailOk.Click();
                        WaitLoadPageEE12();
                        return false;
                    }

                    var bOk = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'OK')]"));
                    bOk.Click();
                    WaitLoadPageEE12();
                    return true;
                }
                catch (Exception Ex)
                {
                    Keanu.TimeToRestart = true;
                    log.Error($"Riabbina() fail");
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool LoopLetture(Records.ArchivioScartiLavorabili scarti)
        {
            //NIKITA FIX DATA 5 YEARS 

            #region DATA CHEK
            if (scarti.Days < 730)
            {
                log.Info("Data is < 2 years");
            }
           else if (scarti.Days >= 730)
            {
                //5 years
                log.Info("Data is >= 2 years");
                log.Info($"Add to sposta list");
                SpostaList.Add(scarti);
                needToReloadListaLetture = false;
                return true;
            }
            else
            {
                log.Info("Data is 2-10 years");
                foreach (var item in ListaLetture)
                {
                    if (item.Data <= scarti.DataLettura)
                        break;
                    if (item.Tipo == "REALE" &&
                        item.Stato == "validata" &&
                        item.StatoInvioSap == "ACQUISITA")
                    {
                        log.Info($"Found REALE/VALIDA/ACQUISITA");
                        log.Info($"Add to sposta list");
                        SpostaList.Add(scarti);
                        needToReloadListaLetture = false;
                        return true;
                    }
                }
                log.Info($"Did not find REALE/VALIDA/ACQUISITA");
            }
            #endregion

            //dobavitj tuta
            
            /*DateTime dataLetturaOldestDate = DateTime.MinValue;

            foreach (var item in ListaLetture)
            {
                if (item.Data > dataLetturaOldestDate)
                {
                    dataLetturaOldestDate = item.Data;
                }

                

            }

            if (dataLetturaOldestDate > scarti.DataLettura)
            {
                // reale above
                log.Info("Data is >= 5 years");
                log.Info($"Add to sposta list");
                SpostaList.Add(scarti);
                needToReloadListaLetture = false;
                return true;
            }
            */

            needToReloadListaLetture = true;
            foreach (var all in ListaLetture)
            {
                if (scarti.DataLettura == all.Data &&
                   (all.Causale == "SOSTITUZIONE" || all.Causale == "RIMOZIONE"))//09072021 AFTER SMIS MODIFICA NEW SCARTO SHOWED UP, RESTART AND IT NEEDS TO BE SPOSTED
                {
                    log.Info($"all.Causale == {all.Causale}");
                    log.Info($"Add to sposta list");
                    SpostaList.Add(scarti);
                    needToReloadListaLetture = false;
                    return true;
                }
                if (scarti.DataLettura == all.Data &&
                    scarti.F1 == all.F1 &&
                    scarti.F2 == all.F2 &&
                    scarti.F3 == all.F3 &&
                    scarti.R1 == all.R1 &&
                    scarti.R2 == all.R2 &&
                    scarti.R3 == all.R3 &&
                    all.Tipo == "REALE")
                {
                    log.Info($"Everything equal");
                    log.Info($"Add to sposta list");
                    SpostaList.Add(scarti);
                    needToReloadListaLetture = false;
                    return true;
                }
                else if (scarti.DataLettura == all.Data &&
                         scarti.F1 == all.F1 &&
                         scarti.F2 == all.F2 &&
                         scarti.F3 == all.F3 &&
                         all.Tipo == "REALE" &&
                         all.ReattivaIsNull == true &&
                         all.Potenza <= potete)
                {
                    log.Info($"F are equal, R are null, potenza <= 15");
                    log.Info($"Add to sposta list");
                    SpostaList.Add(scarti);
                    needToReloadListaLetture = false;
                    return true;
                }
                else if (scarti.DataLettura == all.Data &&
                         scarti.F1 == all.F1 &&
                         scarti.F2 == all.F2 &&
                         scarti.F3 == all.F3 &&
                         all.Tipo == "REALE" &&
                         all.ReattivaIsNull == true &&
                         all.Potenza > potete)
                {
                    log.Error($"F are equal, R are null, potenza > 15");
                    return false;
                }
                else if (scarti.DataLettura == all.Data)
                {
                    if (all.Tipo == "REALE")
                    {
                        log.Info($"Data is equal, tipo is {all.Tipo}");
                        foreach (var reale in RealeLetture)
                        {
                            if (scarti.DataLettura > reale.Data)
                            {
                                log.Info($"Reale {reale.Data.ToShortDateString()}");
                                if ((scarti.F1 >= reale.F1) &&
                                    (scarti.F2 >= reale.F2) &&
                                    (scarti.F3 >= reale.F3) &&
                                    (scarti.R1 >= reale.R1) &&
                                    (scarti.R2 >= reale.R2) &&
                                    (scarti.R3 >= reale.R3))
                                {
                                    log.Info($"F&R >= reale");
                                    log.Debug($"Modifica {scarti.DataLettura.ToShortDateString()}");
                                    ClickAzioniModifica(all);
                                    if (!ModificaLettura(scarti, reale))
                                        return false;
                                    SpostaList.Add(scarti);
                                    return true;
                                }
                                else if ((scarti.F1 >= reale.F1) &&
                                         (scarti.F2 >= reale.F2) &&
                                         (scarti.F3 >= reale.F3) &&
                                         (reale.Potenza <= potete))
                                {
                                    #region FLUSSI2G ORE FREE SUBSCRIPTION TIPO REALE
                                    if (commercialeCheckedAndItIsNotOreFreeSubscription)
                                    {
                                        log.Debug($"Already checked Anagrafica Commerciale");
                                        log.Info($"F >= reale");
                                        log.Info($"Lettura potenza {all.Potenza} <= 15");
                                        log.Info($"R from reale");
                                        scarti.R1 = reale.R1;
                                        scarti.R2 = reale.R2;
                                        scarti.R3 = reale.R3;
                                        log.Debug($"Modifica {scarti.DataLettura.ToShortDateString()}");
                                        ClickAzioniModifica(all);
                                        if (!ModificaLettura(scarti, reale))
                                        {
                                            breakFromSubscription = true;
                                            return false;
                                        }
                                        SpostaList.Add(scarti);
                                        return true;
                                    }
                                    else if (AnagraficaComerciale())
                                    {
                                        reason = "ore free/subscription";
                                        log.Info($"FLUSSI2G + ORE FREE/SUBSCRIPTION");

                                        TempRealeLetture = new List<Records.ListaLetture>();

                                        if (!FillListaLetture())
                                        {
                                            breakFromSubscription = true;
                                            return false;
                                        }

                                        foreach (var filteredReale in RealeLetture)
                                        {
                                            if (filteredReale.Data < scarti.DataLettura &&//NECESSARY FOR NOT PICKING UPPER ROWS THAN MAIN
                                                filteredReale.R1 <= scarti.R1 &&
                                                filteredReale.R2 <= scarti.R2 &&
                                                filteredReale.R3 <= scarti.R3)
                                            {
                                                break;
                                            }
                                            if ((filteredReale.Data <= scarti.DataLettura) &&
                                                (filteredReale.R1 > scarti.R1 ||
                                                 filteredReale.R2 > scarti.R2 ||
                                                 filteredReale.R3 > scarti.R3))
                                            {
                                                foreach (var item in TempRealeLetture)
                                                {
                                                    if (item.Data.Equals(filteredReale.Data))
                                                    {
                                                        log.Info($"Already contains {filteredReale.Data.ToShortDateString()}");
                                                        breakFromSubscription = true;
                                                        return false;
                                                    }
                                                }
                                                TempRealeLetture.Add(filteredReale);
                                            }
                                        }

                                        log.Debug($"Need to modify {TempRealeLetture.Count()} rows");

                                        int cnt = 0;
                                        while (!Keanu.Driver.PageSource.ToString().Contains("Flussi Misure Distributore 2G") && cnt < 10)
                                        {
                                            GoTo("Flussi 2G");
                                            cnt++;
                                        }

                                        if (!GetAllVariablesFromFlussi2GWithRealeList())
                                        {
                                            breakFromSubscription = true;
                                            return false;
                                        }

                                        Indietroer();

                                        foreach (var item in TempRealeLettureWichHasToBeModified)
                                        {
                                            if (needToReloadListaLetture)
                                            {
                                                if (!FillListaLetturePerFlussi2G())
                                                {
                                                    breakFromSubscription = true;
                                                    return false;
                                                }
                                            }

                                            foreach (var rly in RealeLetture)
                                            {
                                                if (rly.Data.Equals(item.DataMisura))
                                                {
                                                    if (rly.Stato.Equals("annullata"))//INSERISCI
                                                    {
                                                        log.Debug($"Inserisci {item.DataMisura.ToShortDateString()}");
                                                        if (!InserisciLetturaWithVariablesFromFlussi2Gg(item))
                                                        {
                                                            breakFromSubscription = true;
                                                            return false;
                                                        }
                                                    }
                                                    else//DEFAULT MODIFY
                                                    {
                                                        log.Debug($"Modifica {item.DataMisura.ToShortDateString()}");
                                                        ClickAzioniModifica(rly);
                                                        if (!ModificaLetturaWithVariablesFromFlussi2G(item))
                                                        {
                                                            if (annullato)
                                                            {
                                                                if (rly.Causale.Contains("MISURA DI CESSAZIONE") || rly.Causale.Contains("MISURA DI POSA"))
                                                                {
                                                                    log.Error($"Cannot ANNULLA, because causale - {rly.Causale}, scarto");
                                                                    breakFromSubscription = true;
                                                                    return false;
                                                                }
                                                                if (annullatoCount > 0)
                                                                {
                                                                    log.Error($"ANNULLATO COUNT > 0, scarto");
                                                                    breakFromSubscription = true;
                                                                    return false;
                                                                }
                                                                if (!ClickAzioniAnnulla(rly))
                                                                {
                                                                    breakFromSubscription = true;
                                                                    return false;
                                                                }
                                                                needToReloadListaLetture = true;
                                                                annullatoCount++;
                                                            }
                                                            else
                                                            {
                                                                log.Info($"Can't modify, problems with rows below");
                                                                breakFromSubscription = true;
                                                                return false;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        log.Info("All rows modified");
                                        SpostaList.Add(scarti);
                                        needToReloadListaLetture = true;
                                        return true;
                                    }
                                    else
                                    {
                                        reason = "senza ore free/subscription";
                                        Indietroer();
                                        return false;//CommercialeCheckedAndItIsNotOreFreeSubscription
                                    }
                                    #endregion
                                }
                                else if ((scarti.F1 >= reale.F1) &&
                                         (scarti.F2 >= reale.F2) &&
                                         (scarti.F3 >= reale.F3) &&
                                         (reale.Potenza > potete))
                                {
                                    log.Info($"Reale potenza {reale.Potenza} > 15");
                                    return false;
                                }
                                else
                                {
                                    if (MatPowah())
                                        return true;
                                    else
                                    {
                                        breakFromSubscription = true;
                                        return false;
                                    }
                                }
                            }
                        }
                        return LoopReale(scarti);
                    }
                    else
                    {
                        log.Info($"Data equal, but tipo is {all.Tipo}");
                        foreach (var reale in RealeLetture)
                        {
                            if (scarti.DataLettura > reale.Data)
                            {
                                log.Info($"Reale {reale.Data.ToShortDateString()}");
                                if ((scarti.F1 >= reale.F1) &&
                                    (scarti.F2 >= reale.F2) &&
                                    (scarti.F3 >= reale.F3) &&
                                    (scarti.R1 >= reale.R1) &&
                                    (scarti.R2 >= reale.R2) &&
                                    (scarti.R3 >= reale.R3))
                                {
                                    log.Info($"F&R >= reale");
                                    log.Debug($"Modifica {scarti.DataLettura.ToShortDateString()}");
                                    if (all.Tipo == "AUTOLETTURA" || all.Tipo == "STIMATA NEXT")
                                    {
                                        if (!InserisciLettura(scarti, reale))
                                            return false;
                                    }
                                    else
                                    {
                                        ClickAzioniModifica(all);
                                        if (!ModificaLettura(scarti, reale))
                                            return false;
                                    }
                                    SpostaList.Add(scarti);
                                    return true;
                                }
                                else if ((scarti.F1 >= reale.F1) &&
                                         (scarti.F2 >= reale.F2) &&
                                         (scarti.F3 >= reale.F3) &&
                                         (reale.Potenza <= potete))
                                {
                                    #region FLUSSI2G ORE FREE SUBSCRIPTION TIPO IS NOT REALE
                                    if (commercialeCheckedAndItIsNotOreFreeSubscription)
                                    {
                                        log.Debug($"Already checked Anagrafica Commerciale");
                                        log.Info($"F >= reale");
                                        log.Info($"Lettura potenza {all.Potenza} <= 15");
                                        log.Info($"R from reale");
                                        scarti.R1 = reale.R1;
                                        scarti.R2 = reale.R2;
                                        scarti.R3 = reale.R3;
                                        log.Debug($"Modifica {scarti.DataLettura.ToShortDateString()}");
                                        if (all.Tipo == "AUTOLETTURA" || all.Tipo == "STIMATA NEXT")
                                        {
                                            if (!InserisciLettura(scarti, reale))
                                                return false;
                                        }
                                        else
                                        {
                                            ClickAzioniModifica(all);
                                            if (!ModificaLettura(scarti, reale))
                                            {
                                                breakFromSubscription = true;
                                                return false;
                                            }
                                        }
                                        SpostaList.Add(scarti);
                                        return true;
                                    }
                                    else if (AnagraficaComerciale())
                                    {
                                        reason = "ore free/subscription";
                                        log.Info($"FLUSSI2G + ORE FREE/SUBSCRIPTION");

                                        TempRealeLetture = new List<Records.ListaLetture>();

                                        if (!FillListaLetture())
                                        {
                                            breakFromSubscription = true;
                                            return false;
                                        }

                                        //TAKE MY SCARTO DATA EVEN IF IT IS NOT REALE
                                        foreach (var filteredAll in ListaLetture)
                                        {
                                            if (filteredAll.Data == scarti.DataLettura)
                                            {
                                                TempRealeLetture.Add(filteredAll);
                                                break;
                                            }
                                        }

                                        foreach (var filteredReale in RealeLetture)
                                        {
                                            if (filteredReale.Data < scarti.DataLettura &&//NECESSARY FOR NOT PICKING UPPER ROWS THAN MAIN
                                                filteredReale.R1 <= scarti.R1 &&
                                                filteredReale.R2 <= scarti.R2 &&
                                                filteredReale.R3 <= scarti.R3)
                                            {
                                                break;
                                            }
                                            if ((filteredReale.Data < scarti.DataLettura) &&
                                                (filteredReale.R1 > scarti.R1 ||
                                                 filteredReale.R2 > scarti.R2 ||
                                                 filteredReale.R3 > scarti.R3))
                                            {
                                                foreach (var item in TempRealeLetture)
                                                {
                                                    if (item.Data.Equals(filteredReale.Data))
                                                    {
                                                        log.Info($"Already contains {filteredReale.Data.ToShortDateString()}");
                                                        breakFromSubscription = true;
                                                        return false;
                                                    }
                                                }
                                                TempRealeLetture.Add(filteredReale);
                                            }
                                        }

                                        log.Debug($"Need to modify {TempRealeLetture.Count()} rows");

                                        int cnt = 0;
                                        while (!Keanu.Driver.PageSource.ToString().Contains("Flussi Misure Distributore 2G") && cnt < 10)
                                        {
                                            GoTo("Flussi 2G");
                                            cnt++;
                                        }

                                        if (!GetAllVariablesFromFlussi2GWithRealeList())
                                        {
                                            breakFromSubscription = true;
                                            return false;
                                        }

                                        Indietroer();

                                        foreach (var item in TempRealeLettureWichHasToBeModified)
                                        {
                                            if (needToReloadListaLetture)
                                            {
                                                if (!FillListaLetturePerFlussi2G())
                                                {
                                                    breakFromSubscription = true;
                                                    return false;
                                                }
                                            }

                                            //ROW FROM SCARTO WILL BE MODIFIED LAST BECAUSE TempRealeLettureWichHasToBeModified.Reverse()
                                            if (item.DataMisura.Equals(scarti.DataLettura))
                                            {
                                                foreach (var rly in ListaLetture)
                                                {
                                                    if (rly.Data.Equals(item.DataMisura))
                                                    {
                                                        if (rly.Stato.Equals("annullata"))//INSERISCI
                                                        {
                                                            log.Debug($"Inserisci {item.DataMisura.ToShortDateString()}");
                                                            if (!InserisciLetturaWithVariablesFromFlussi2Gg(item))
                                                            {
                                                                breakFromSubscription = true;
                                                                return false;
                                                            }
                                                        }
                                                        else//DEFAULT MODIFY
                                                        {
                                                            log.Debug($"Modifica {item.DataMisura.ToShortDateString()}");
                                                            ClickAzioniModifica(rly);
                                                            if (!ModificaLetturaWithVariablesFromFlussi2G(item))
                                                            {
                                                                if (annullato)
                                                                {
                                                                    if (rly.Causale.Contains("MISURA DI CESSAZIONE") || rly.Causale.Contains("MISURA DI POSA"))
                                                                    {
                                                                        log.Error($"Cannot ANNULLA, because causale - {rly.Causale}, scarto");
                                                                        breakFromSubscription = true;
                                                                        return false;
                                                                    }
                                                                    if (annullatoCount > 0)
                                                                    {
                                                                        log.Error($"ANNULLATO COUNT > 0, scarto");
                                                                        breakFromSubscription = true;
                                                                        return false;
                                                                    }
                                                                    if (!ClickAzioniAnnulla(rly))
                                                                    {
                                                                        breakFromSubscription = true;
                                                                        return false;
                                                                    }
                                                                    needToReloadListaLetture = true;
                                                                    annullatoCount++;
                                                                }
                                                                else
                                                                {
                                                                    log.Info($"Can't modify, problems with rows below");
                                                                    breakFromSubscription = true;
                                                                    return false;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else//OTHER ROWS
                                            {
                                                foreach (var rly in RealeLetture)
                                                {
                                                    if (rly.Data.Equals(item.DataMisura))
                                                    {
                                                        if (rly.Stato.Equals("annullata"))//INSERISCI
                                                        {
                                                            log.Debug($"Inserisci {item.DataMisura.ToShortDateString()}");
                                                            if (!InserisciLetturaWithVariablesFromFlussi2Gg(item))
                                                            {
                                                                breakFromSubscription = true;
                                                                return false;
                                                            }
                                                        }
                                                        else//DEFAULT MODIFY
                                                        {
                                                            log.Debug($"Modifica {item.DataMisura.ToShortDateString()}");
                                                            ClickAzioniModifica(rly);
                                                            if (!ModificaLetturaWithVariablesFromFlussi2G(item))
                                                            {
                                                                if (annullato)
                                                                {
                                                                    if (rly.Causale.Contains("MISURA DI CESSAZIONE") || rly.Causale.Contains("MISURA DI POSA"))
                                                                    {
                                                                        log.Error($"Cannot ANNULLA, because causale - {rly.Causale}, scarto");
                                                                        breakFromSubscription = true;
                                                                        return false;
                                                                    }
                                                                    if (annullatoCount > 0)
                                                                    {
                                                                        log.Error($"ANNULLATO COUNT > 0, scarto");
                                                                        breakFromSubscription = true;
                                                                        return false;
                                                                    }
                                                                    if (!ClickAzioniAnnulla(rly))
                                                                    {
                                                                        breakFromSubscription = true;
                                                                        return false;
                                                                    }
                                                                    needToReloadListaLetture = true;
                                                                    annullatoCount++;
                                                                }
                                                                else
                                                                {
                                                                    log.Info($"Can't modify, problems with rows below");
                                                                    breakFromSubscription = true;
                                                                    return false;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        log.Info("All rows modified");
                                        SpostaList.Add(scarti);
                                        needToReloadListaLetture = true;
                                        return true;
                                    }
                                    else
                                    {
                                        reason = "senza ore free/subscription";
                                        Indietroer();
                                        return false;//CommercialeCheckedAndItIsNotOreFreeSubscription
                                    }
                                    #endregion
                                }
                                else if ((scarti.F1 >= reale.F1) &&
                                         (scarti.F2 >= reale.F2) &&
                                         (scarti.F3 >= reale.F3) &&
                                         (reale.Potenza > potete))
                                {
                                    log.Info($"Reale potenza {reale.Potenza} > 15");
                                    return false;
                                }
                                else
                                {
                                    if (MatPowah())
                                        return true;
                                    else
                                    {
                                        breakFromSubscription = true;
                                        return false;
                                    }
                                }
                            }
                        }
                        return LoopReale(scarti);//foreach RealeLetture
                    }
                }
            }
            return LoopReale(scarti);//foreach ListaLetture
        }

        private bool MatPowah()
        {
            if (matPower || ripristinaPower)
            {
                log.Info($"matPower already true, cannot do anything more");
                return false;
            }
            log.Info($"One of F reale is bigger than scarti");
            log.Info($"Try to MAT Power");

            try
            {
                requestFlussi2g = GetListaFlussi2GPower("https://next.enelint.global/api-next-online/api/v1/protected/" + v.Pod + "/measures/mm2g?startDate=" + dataAttivazioneMinusOne + "&endDate=9999-12-31&aggregationMode=MONTHLY&_hidePageLoading=false");
                if (requestFlussi2g.Count().Equals(0))
                {
                    requestFlussi = GetListaFlussiPower("https://next.enelint.global/api-next-online/api/v1/protected/power/flussi-standard?entityId=" + v.Pod + "." + dataAttivazioneStandard + ".9999-12-31");
                    if (requestFlussi.Count().Equals(0))
                        return false;
                    else
                    {
                        if (!Cambio1G1G())
                            return false;
                        return true;//matPower
                    }
                }

                List<Records.Flussi2G> tmpSmisList = new List<Records.Flussi2G>();
                foreach (var item in requestFlussi2g)
                {
                    if (item.CodiceFlusso.Equals("SMIS"))
                    {
                        tmpSmisList.Add(item);
                        log.Debug($"{item.DataMisura.ToShortDateString()} {item.Stato} {item.Repository} {item.StatoAbb} {item.Motivazione} {item.Esito} {item.CodiceEsito}");
                    }
                }

                log.Info($"{tmpSmisList.Count()} FLUSSI2G SMIS Before");

                if (tmpSmisList.Count().Equals(0))
                {
                    requestFlussi = GetListaFlussiPower("https://next.enelint.global/api-next-online/api/v1/protected/power/flussi-standard?entityId=" + v.Pod + "." + dataAttivazioneStandard + ".9999-12-31");
                    if (requestFlussi.Count().Equals(0))
                        return false;
                    else
                    {
                        if (!Cambio1G1G())
                            return false;
                        return true;//matPower
                    }
                }

                if (tmpSmisList.Count() > 3)
                {
                    log.Info($"More than 3 SMIS");
                    return false;
                }

                SmisList = new List<Records.Flussi2G>();
                string itemDataFirst = "";
                foreach (var item in tmpSmisList)
                {
                    string tmpData = item.DataMisura.ToShortDateString();
                    if (!tmpData.Equals(itemDataFirst))
                        SmisList.Add(item);
                    else
                    {
                        if (tmpData.Equals(itemDataFirst) && item.Repository.Equals("DISCARD"))
                        {
                            log.Info($"Don't add SCARTO to SmisList with same data");
                            continue;
                        }
                        log.Info($"2 SMIS with same date");
                        return false;
                    }
                    itemDataFirst = tmpData;
                }

                log.Info($"{SmisList.Count()} FLUSSI2G SMIS After");

                if (SmisList.Count().Equals(1))
                {
                    log.Info($"Only 1 SMIS");
                    return false;
                }

                if (SmisList.Count() > 2)
                {
                    string tmpMonth = SmisList[0].MeseAnno;
                    int monthCounter = 0;
                    foreach (var item in SmisList)
                    {
                        string itemTmpMonth = item.MeseAnno;
                        if (tmpMonth.Equals(itemTmpMonth))
                            monthCounter++;
                        else
                            monthCounter--;
                        tmpMonth = itemTmpMonth;
                    }

                    if (monthCounter > 2)
                    {
                        log.Info($"More than 2 SMIS in one month/SCARTO");
                        return false;
                    }
                }

                if (SmisList.Count().Equals(0))
                {
                    requestFlussi = GetListaFlussiPower("https://next.enelint.global/api-next-online/api/v1/protected/power/flussi-standard?entityId=" + v.Pod + "." + dataAttivazioneStandard + ".9999-12-31");
                    if (requestFlussi.Count().Equals(0))
                        return false;
                    else
                    {
                        if (!Cambio1G1G())
                            return false;
                        return true;//matPower
                    }
                }
                else
                {
                    needToReloadListaLetture = true;
                    int inLavorazioneCunt = 0;
                    foreach (var smis in SmisList)
                    {
                        if (matPower)//15062021
                            break;

                        inLavorazioneCunt++;
                        if (needToReloadListaLetture)
                        {
                            if (!FillListaLetture())
                                return false;
                        }
                        log.Debug($"In lavorazione {inLavorazioneCunt}/{SmisList.Count()} {smis.DataMisura.ToShortDateString()}");
                        log.Debug($"{smis.F1} {smis.F2} {smis.F3} - {smis.R1} {smis.R2} {smis.R3}");
                        if (!LoopSMIS(smis))
                            return false;//SCARTO/SOSPESO
                    }
                }

                if (atleastOneSmisHasBeenModified && !matPower)
                    return true;

                if (matPower)//15062021
                {
                    if (SmisList.Count() > 0)
                    {
                        #region ROW CHEK
                        DateTime lastSmisData = SmisList.LastOrDefault().DataMisura;
                        foreach (var item in requestFlussi2g)
                        {
                            DateTime listData = item.DataMisura;
                            if (listData < lastSmisData)
                            {
                                if (item.CodiceFlusso.Equals("SMIS"))//SKIP SMIS THAT WAS SCARTO
                                    continue;
                                areThereMoreRowsAfterLastSmis = true;
                                break;
                            }
                        }

                        int scartoCnt = 0;
                        int onlineCnt = 0;
                        foreach (var item in SmisList)
                        {
                            if (item.Repository.Equals("DISCARD") || item.Repository.Equals("PARK"))//OR "PARK"
                                scartoCnt++;
                            if (item.Repository.Equals("ONLINE") && item.StatoAbb.Equals("A"))
                                onlineCnt++;
                        }
                        if (onlineCnt.Equals(SmisList.Count()))
                        {
                            bool needToInsertPeriodicaAgain = false;
                            bool dateIsInMidOfMonth = false;
                            var rr = SmisList[0];
                            if (rr.DataMisura.Day >= 2 && rr.DataMisura.Day <= 29)
                            {
                                dateIsInMidOfMonth = true;
                                log.Debug($"Data is in the middle of month - {rr.DataMisura.ToShortDateString()}");
                                
                                //FOP MatPowah() scarto because wtf
                                // sospeso id 2 - nelza delatj smenu shetchika po seredine mesjaca, oni dolzni menjatsa v nachale i v konce mesjaca (pervij i poslednij denj mesjaca), takie sluchai mi otprovlajem Helene
                                Keanu.TimeToSospesoType = 2;
                                Keanu.TimeToSospeso = true;
                                breakFromSubscription = true;
                                return false;
                            }

                            log.Debug($"Both SMIS are VALIDA and A");
                            log.Debug($"Cancella/Ripristina");
                            smisType = "cancellaRipristina";

                            #region CANCELLA EVERY SMIS
                            needToReloadListaLetture = true;
                            foreach (var smis in SmisList)
                            {
                                if (needToReloadListaLetture)
                                {
                                    if (!FillListaLetture())
                                        return false;
                                    needToReloadListaLetture = false;
                                }
                                string smisData = smis.DataMisura.ToShortDateString();
                                foreach (var all in ListaLetture)
                                {
                                    string allData = all.Data.ToShortDateString();
                                    if (smisData == allData)
                                    {
                                        if (allData == rr.DataMisura.ToShortDateString() && dateIsInMidOfMonth)//MIDMONTH
                                            needToInsertPeriodicaAgain = true;

                                        if (!ClickAzioniCancella(all))
                                        {
                                            log.Error($"Couldn't Cancella SMIS");
                                            return false;
                                        }
                                        needToReloadListaLetture = true;
                                    }
                                }
                            }
                            #endregion

                            #region RIPRISTINA
                            var raw = SmisList.FirstOrDefault().DataMisura.ToString();
                            string[] dateTimeSplit = raw.ToString().Split(' ');
                            string[] onlyDateSplit = dateTimeSplit[0].ToString().Split('/');
                            string onlyTime = dateTimeSplit[1].ToString().Replace(":", "");
                            var DataRipristina = onlyDateSplit[2] + onlyDateSplit[1] + onlyDateSplit[0] + onlyTime;
                            //var DataRipristina = SmisList.FirstOrDefault().DataMisura.Year.ToString() + SmisList.FirstOrDefault().DataMisura.Month.ToString() + SmisList.FirstOrDefault().DataMisura.Day.ToString() + SmisList.FirstOrDefault().DataMisura.Hour.ToString() + SmisList.FirstOrDefault().DataMisura.Minute.ToString() + "00";
                            //https://next.enelint.global/api-next-online/api/v1/protected/IT001E22204926/measures/mm2g/20210504000000/0/restore OK LINK
                            //https://next.enelint.global/api-next-online/api/v1/protected/IT001E22204926/measures/mm2g/2021540000/0/restore NOK LINK
                            var UrlRequestRipristina = "https://next.enelint.global/api-next-online/api/v1/protected/" + v.Pod + "/measures/mm2g/" + DataRipristina + "/0/restore";
                            if (Ripristina(UrlRequestRipristina, out string errore))
                            {
                                log.Info("Ripristinato");
                                ripristinaPower = true;
                                finishInLavorazioneAfterMatPower = true;
                                matPower = false;//IF RIPRISTINATO THEN IT IS NOT matPower

                                //if (needToInsertPeriodicaAgain)
                                //{
                                //    Keanu.Driver.Navigate().Refresh();
                                //    WaitLoadPageEE12();
                                //    File.AppendAllText(speshal, $"{v.Riferimento} {v.Pod} {DateTime.Today.ToShortDateString()} needToInsertPeriodicaAgain = true" + Environment.NewLine);
                                //    if (!InserisciLetturaWithVariablesFromFlussi2Gg(rr))
                                //    {
                                //        breakFromSubscription = true;
                                //        return false;
                                //    }
                                //}

                                return true;
                            }
                            else
                            {
                                log.Info(errore);
                                return false;
                            }
                            #endregion
                        }
                        if (scartoCnt.Equals(SmisList.Count()))
                        {
                            log.Debug($"Both SMIS are SCARTO");
                            if (areThereMoreRowsAfterLastSmis)
                            {
                                log.Debug($"More rows after last SMIS, 2G2G");//TOO HARD, SCARTO
                                smisType = "2G2G";
                                if (!Cambio2G2G())
                                    return false;
                            }
                            else
                            {
                                log.Debug($"No rows after last SMIS, 1G2G");
                                smisType = "1G2G";
                                if (!Cambio1G2G(true))//flag SI
                                    return false;
                            }
                        }
                        else
                        {
                            log.Debug($"Both SMIS are not SCARTO");
                            if (areThereMoreRowsAfterLastSmis)
                            {
                                log.Debug($"More rows after last SMIS, 2G2G");//TOO HARD, SCARTO
                                smisType = "2G2G";
                                if (!Cambio2G2G())
                                    return false;
                            }
                            else
                            {
                                log.Debug($"No rows after last SMIS, 1G2G");
                                smisType = "1G2G";

                                string defTempDate = SmisList.FirstOrDefault().DataMisura.ToString();
                                string[] split = defTempDate.ToString().Split(' ');
                                string[] split2 = split[0].ToString().Split('/');
                                string dataDecorrenza = split2[2] + "-" + split2[1] + "-" + split2[0];
                                //https://next.enelint.global/api-next-online/api/v1/protected/power/detailTecnica/IT001E66267086.2020-07-03.9999-12-31
                                string UrlRequestInfoCommerciali = "https://next.enelint.global/api-next-online/api/v1/protected/power/detailTecnica/" + v.Pod + "." + dataDecorrenza + ".9999-12-31";

                                //UNCHECK FLAG
                                if (!SetFlag2G(false, UrlRequestInfoCommerciali, "", "", "", out string dtDecorrenza2G_, out string dtMessaRegime2G_, out string dtPassaggioOrario2G_))
                                    return false;

                                if (!Cambio1G2G(false))//flag NO
                                    return false;

                                //CHEK FLAG
                                if (!SetFlag2G(true, UrlRequestInfoCommerciali, dtDecorrenza2G_, dtMessaRegime2G_, dtPassaggioOrario2G_, out string dtDecorrenza2G__, out string dtMessaRegime2G__, out string dtPassaggioOrario2G__))
                                    //if (!SetFlag2G(true, UrlRequestInfoCommerciali, SmisList.FirstOrDefault().DataMisura.ToShortDateString(), "", "", out string dtDecorrenza2G__, out string dtMessaRegime2G__, out string dtPassaggioOrario2G__))
                                    return false;
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    return false;//SMIS' ARE EQUAL NO CHANGES HAS BEEN MADE SO DOCUMENT CANNOT BE MODIFIED/INSERTED
                }
                return true;//matPower
            }
            catch
            {
                log.Debug($"MAT Power");
                Keanu.TimeToSospesoType = 5;
                Keanu.TimeToSospeso = true;
                return false;
            }
        }

        private bool LoopReale(Records.ArchivioScartiLavorabili scarti)
        {
            log.Info($"There is no {scarti.DataLettura.ToShortDateString()}, get first reale below");
            foreach (var reale in RealeLetture)
            {
                if (scarti.DataLettura > reale.Data)//if (scarti.DataLettura >= reale.Data)//DONT PICK YOUR OWN DATA SCARTO IF THERE IS NO REALE BELOW
                {
                    log.Info($"Reale {reale.Data.ToShortDateString()}");
                    if ((reale.F1 > scarti.F1) ||
                        (reale.F2 > scarti.F2) ||
                        (reale.F3 > scarti.F3))
                    {
                        if (MatPowah())
                            return true;
                        else
                        {
                            breakFromSubscription = true;
                            return false;
                        }
                    }
                    else if ((scarti.F1 >= reale.F1) &&
                             (scarti.F2 >= reale.F2) &&
                             (scarti.F3 >= reale.F3) &&
                             (scarti.R1 >= reale.R1) &&
                             (scarti.R2 >= reale.R2) &&
                             (scarti.R3 >= reale.R3))
                    {
                        log.Info($"F&R >= reale");
                        log.Debug($"Inserisci {scarti.DataLettura.ToShortDateString()}");
                        if (!InserisciLettura(scarti, reale))
                            return false;
                        SpostaList.Add(scarti);
                        return true;
                    }
                    else if ((scarti.F1 >= reale.F1) &&
                             (scarti.F2 >= reale.F2) &&
                             (scarti.F3 >= reale.F3) &&
                             (reale.Potenza <= potete))
                    {
                        #region FLUSSI2G ORE FREE SUBSCRIPTION INSERISCI
                        if (commercialeCheckedAndItIsNotOreFreeSubscription)
                        {
                            log.Debug($"Already checked Anagrafica Commerciale");
                            log.Info($"R from reale");
                            log.Debug($"Inserisci {scarti.DataLettura.ToShortDateString()}");
                            scarti.R1 = reale.R1;
                            scarti.R2 = reale.R2;
                            scarti.R3 = reale.R3;
                            if (!InserisciLettura(scarti, reale))
                                return false;
                            SpostaList.Add(scarti);
                            return true;
                        }
                        else if (AnagraficaComerciale())
                        {
                            reason = "ore free/subscription";
                            log.Info($"FLUSSI2G + ORE FREE/SUBSCRIPTION");

                            TempRealeLetture = new List<Records.ListaLetture>();

                            if (!FillListaLetture())
                            {
                                breakFromSubscription = true;
                                return false;
                            }

                            var specialForInserisciGetScartiDataLettura = new Records.ListaLetture
                            {
                                Data = scarti.DataLettura//ADD SCARTO DATA TO LIST FOR GETTING VARIABLES FROM FLUSSI2G
                            };
                            TempRealeLetture.Add(specialForInserisciGetScartiDataLettura);

                            foreach (var filteredReale in RealeLetture)
                            {
                                if (filteredReale.Data < scarti.DataLettura &&//NECESSARY FOR NOT PICKING UPPER ROWS THAN MAIN
                                    filteredReale.R1 <= scarti.R1 &&
                                    filteredReale.R2 <= scarti.R2 &&
                                    filteredReale.R3 <= scarti.R3)
                                {
                                    break;
                                }
                                if ((filteredReale.Data < scarti.DataLettura) &&
                                    (filteredReale.R1 > scarti.R1 ||
                                     filteredReale.R2 > scarti.R2 ||
                                     filteredReale.R3 > scarti.R3))
                                {
                                    foreach (var item in TempRealeLetture)
                                    {
                                        if (item.Data.Equals(filteredReale.Data))
                                        {
                                            log.Info($"Already contains {filteredReale.Data.ToShortDateString()}");
                                            breakFromSubscription = true;
                                            return false;
                                        }
                                    }
                                    TempRealeLetture.Add(filteredReale);
                                }
                            }

                            log.Debug($"Need to modify {TempRealeLetture.Count()} rows");

                            int cnt = 0;
                            while (!Keanu.Driver.PageSource.ToString().Contains("Flussi Misure Distributore 2G") && cnt < 10)
                            {
                                GoTo("Flussi 2G");
                                cnt++;
                            }

                            if (!GetAllVariablesFromFlussi2GWithRealeList())
                            {
                                breakFromSubscription = true;
                                return false;
                            }

                            Indietroer();

                            foreach (var item in TempRealeLettureWichHasToBeModified)
                            {
                                if (needToReloadListaLetture)
                                {
                                    if (!FillListaLetturePerFlussi2G())
                                    {
                                        breakFromSubscription = true;
                                        return false;
                                    }
                                }

                                if (item.DataMisura.Equals(scarti.DataLettura))
                                {
                                    log.Info($"F&R from flussi2g");
                                    log.Debug($"Inserisci {item.DataMisura.ToShortDateString()}");
                                    if (!InserisciLetturaWithVariablesFromFlussi2Gg(item))
                                    {
                                        breakFromSubscription = true;
                                        return false;
                                    }
                                    break;
                                }

                                foreach (var rly in RealeLetture)
                                {
                                    if (rly.Data.Equals(item.DataMisura))
                                    {
                                        if (rly.Stato.Equals("annullata"))//INSERISCI
                                        {
                                            log.Debug($"Inserisci {item.DataMisura.ToShortDateString()}");
                                            if (!InserisciLetturaWithVariablesFromFlussi2Gg(item))
                                            {
                                                breakFromSubscription = true;
                                                return false;
                                            }
                                        }
                                        else//DEFAULT MODIFY
                                        {
                                            log.Debug($"Modifica {item.DataMisura.ToShortDateString()}");
                                            ClickAzioniModifica(rly);
                                            if (!ModificaLetturaWithVariablesFromFlussi2G(item))
                                            {
                                                if (annullato)
                                                {
                                                    if (rly.Causale.Contains("MISURA DI CESSAZIONE") || rly.Causale.Contains("MISURA DI POSA"))
                                                    {
                                                        log.Error($"Cannot ANNULLA, because causale - {rly.Causale}, scarto");
                                                        {
                                                            breakFromSubscription = true;
                                                            return false;
                                                        }
                                                    }
                                                    if (annullatoCount > 0)
                                                    {
                                                        log.Error($"ANNULLATO COUNT > 0, scarto");
                                                        {
                                                            breakFromSubscription = true;
                                                            return false;
                                                        }
                                                    }
                                                    if (!ClickAzioniAnnulla(rly))
                                                    {
                                                        breakFromSubscription = true;
                                                        return false;
                                                    }
                                                    needToReloadListaLetture = true;
                                                    annullatoCount++;
                                                }
                                                else
                                                {
                                                    log.Info($"Can't modify, problems with rows below");
                                                    {
                                                        breakFromSubscription = true;
                                                        return false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            log.Info("All rows modified");
                            SpostaList.Add(scarti);
                            needToReloadListaLetture = true;
                            return true;
                        }
                        else
                        {
                            reason = "senza ore free/subscription";
                            Indietroer();
                            return false;//CommercialeCheckedAndItIsNotOreFreeSubscription
                        }
                        #endregion
                    }
                    else if ((scarti.F1 >= reale.F1) &&
                             (scarti.F2 >= reale.F2) &&
                             (scarti.F3 >= reale.F3) &&
                             (reale.Potenza > potete))
                    {
                        log.Info($"Reale potenza {reale.Potenza} > 15");
                        return false;
                    }
                    else
                    {
                        log.Info($"Wrong turn 6");
                        return false;
                    }
                }
            }
            log.Info($"No reale below {scarti.DataLettura.ToShortDateString()}");
            return false;
        }

        private bool LoopSMIS(Records.Flussi2G smis)
        {
            foreach (var all in ListaLetture)
            {
                int idx = ListaLetture.IndexOf(all);
                string smisData = smis.DataMisura.ToShortDateString();
                string allData = all.Data.ToShortDateString();
                if (smisData == allData)
                {
                    log.Info($"smisData == allData");
                    if (all.Causale == "SOSTITUZIONE" || all.Causale == "RIMOZIONE")
                    {
                        log.Info($"all.Causale == {all.Causale}");
                    }
                    else
                    {
                        continue;
                    }

                    if (smisData == allData &&
                        //all.Tipo == "REALE" &&
                        smis.F1 == all.F1 &&
                        smis.F2 == all.F2 &&
                        smis.F3 == all.F3)
                    {
                        log.Info($"Everything equal");
                        needToReloadListaLetture = false;
                        return true;
                    }
                    else
                    {
                        if (smis.Stato == "50" &&
                            all.Causale != "SOSTITUZIONE" &&
                            all.Causale != "RIMOZIONE")
                        {
                            log.Info($"SMIS with stato NON VALIDA doesn't contain SOSTITUZIONE/RIMOZIONE in ListaLetture");
                            File.AppendAllText(speshal, $"{v.Riferimento} {v.Pod} {DateTime.Today.ToShortDateString()} SMIS with stato NON VALIDA doesn't contain SOSTITUZIONE/RIMOZIONE in ListaLetture" + Environment.NewLine);
                            return false;
                        }

                        if (all.Fonte.Contains("STIMATA DA RICOSTR..") || all.Fonte.Contains("Stimata da Ricostr.."))
                        {
                            log.Debug($"Stimata da Ricostruzione per malfunzionnamento");
                            needToReloadListaLetture = false;
                            return true;
                        }

                        if (all.Tipo == "REALE" &&
                            all.Stato == "validata" &&
                            smis.F1 == -1 &&
                            smis.F2 == -1 &&
                            smis.F3 == -1)
                        {
                            log.Debug($"F -1 + REALE + VALIDA");
                            needToReloadListaLetture = false;
                            return true;
                        }

                        if (all.Stato.Equals("dastimare"))
                        {
                            if (!InserisciLetturaWithVariablesFromFlussi2Gg(smis))
                                return false;
                        }
                        else
                        {
                            ClickAzioniModifica(all);
                            if (!ModificaLetturaWithVariablesFromFlussi2GSmisEdition(smis, all))
                            {
                                if (needToAnnulla)
                                {
                                    var whatToAnnulla = ListaLetture[idx + 1];
                                    if (whatToAnnulla.Causale.Equals("MISURA DI POSA"))
                                    {
                                        log.Info($"Don't Annulla MISURA DI POSA");
                                        return false;
                                    }
                                    if (!ClickAzioniAnnulla(whatToAnnulla))
                                        return false;
                                    if (!FillListaLetture())
                                        return false;
                                    foreach (var item in ListaLetture)
                                    {
                                        string sData = smis.DataMisura.ToShortDateString();
                                        string iData = item.Data.ToShortDateString();
                                        if (sData.Equals(iData))
                                        {
                                            if (item.Stato.Equals("dastimare"))//AFTER ANNULLA STATO COULD CHANGE TO DASTIMARE
                                            {
                                                if (!InserisciLetturaWithVariablesFromFlussi2Gg(smis))
                                                    return false;
                                            }
                                            else
                                            {
                                                ClickAzioniModifica(item);
                                                if (!ModificaLetturaWithVariablesFromFlussi2GSmisEdition(smis, item))
                                                    return false;
                                            }
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }

                        log.Info($"Modified");
                        atleastOneSmisHasBeenModified = true;
                        needToReloadListaLetture = true;
                        return true;
                    }
                }
            }

            log.Info($"There is no SOSTITUZIONE/RIMOZIONE with {smis.DataMisura.ToShortDateString()}");

            DateTime minusOneDay = smis.DataMisura.AddDays(-1);
            DateTime plusOneDay = smis.DataMisura.AddDays(1);

            string smisDataMinusOneDay = minusOneDay.ToShortDateString();
            string smisDataExact = smis.DataMisura.ToShortDateString();
            string smisDataPlusOneDay = plusOneDay.ToShortDateString();

            foreach (var all in ListaLetture)
            {
                string allData = all.Data.ToShortDateString();
                if (allData == smisDataMinusOneDay ||
                    allData == smisDataExact ||
                    allData == smisDataPlusOneDay)
                {
                    if (all.Causale == "MISURA DI POSA")
                    {
                        log.Info($"Found MISURA DI POSA {allData}");
                        needToReloadListaLetture = false;
                        return true;
                    }
                }
            }

            log.Info($"matPower true");
            matPower = true;
            return true;
        }

        private bool AnagraficaComerciale()
        {
            needToHaveReason = true;

            if (commercialeIsOreFreeSubscription)
                return true;

            int cnt = 0;
            while (!Keanu.Driver.PageSource.ToString().Contains("Dati Cliente") && cnt < 10)
            {
                GoTo("Anagrafica Commerciale");
                cnt++;
            }

            string txtRateCategory;
            if (Keanu.Driver.PageSource.ToString().Contains("Dati Contratto"))
            {
                try
                {
                    log.Info($"Insta Dati Contratto");
                    var txtRC = Keanu.Driver.FindElement(By.Id("rateCategory"));
                    txtRateCategory = txtRC.GetAttribute("value");
                    log.Debug(txtRateCategory);
                    if (txtRateCategory.Contains("ORE FREE") || txtRateCategory.Contains("SUBSCRIPTION"))
                    {
                        commercialeIsOreFreeSubscription = true;
                        Indietroer();
                        return true;//CONTAINS
                    }
                    else
                    {
                        commercialeCheckedAndItIsNotOreFreeSubscription = true;
                        return false;//DOESN'T CONTAIN
                    }
                }
                catch
                {
                    log.Error($"Rate Category fail");//SOSPESO ANYWAY
                    return true;
                }
            }

            int rowz;
            int skipper = 1;

            try
            {
                var tableLC = Keanu.Driver.FindElement(By.ClassName("firstRow"));
                IList<IWebElement> tableRowsLC = null;
                tableRowsLC = tableLC.FindElements(By.TagName("tr"));
                rowz = tableRowsLC.Count() - 1;
                log.Info($"{rowz} Rows");
            }
            catch
            {
                log.Error($"Table error");
                return true;
            }

            try
            {
                for (int i = 0; i < rowz; i++)
                {
                    var tableLC = Keanu.Driver.FindElement(By.ClassName("firstRow"));
                    IList<IWebElement> tableRowsLC = null;
                    tableRowsLC = tableLC.FindElements(By.TagName("tr"));
                    log.Info($"Checking {skipper} row");
                    foreach (var row in tableRowsLC.Skip(skipper))//SKIP HEADERS
                    {
                        cnt = 0;
                        row.Click();
                        while (!Keanu.Driver.PageSource.ToString().Contains("Dati Contratto") && cnt < 10)
                        {
                            WaitLoadPageEE12();
                            cnt++;
                        }
                        WaitLoadPageEE12();
                        try
                        {
                            var txtRCloop = Keanu.Driver.FindElement(By.Id("rateCategory"));
                            txtRateCategory = txtRCloop.GetAttribute("value");
                            log.Debug(txtRateCategory);
                            if (txtRateCategory.Contains("ORE FREE") || txtRateCategory.Contains("SUBSCRIPTION"))
                            {
                                commercialeIsOreFreeSubscription = true;
                                Indietroer();
                                return true;//CONTAINS
                            }
                            else
                            {
                                //DOESN'T CONTAIN
                                try
                                {
                                    var bIndietro = Keanu.Driver.FindElement(By.XPath("//button[@ngbtooltip='Indietro']"));
                                    bIndietro.Click();
                                    WaitLoadPageEE12();
                                }
                                catch
                                {
                                    log.Error($"Rate Category fail");//SOSPESO ANYWAY
                                    return true;
                                }
                                skipper++;
                                break;
                            }
                        }
                        catch
                        {
                            log.Error($"Rate Category fail");//SOSPESO ANYWAY
                            return true;
                        }
                    }
                }
            }
            catch
            {
                log.Error($"AnagraficaComerciale() fail");
                return true;
            }
            log.Info($"All rows checked");
            commercialeCheckedAndItIsNotOreFreeSubscription = true;
            return false;
        }

        private void Indietroer()
        {
            try
            {
                int cnt = 0;
                while (!Keanu.Driver.PageSource.ToString().Contains("Mostra tutto") && cnt < 15)
                {
                    var bIndietro = Keanu.Driver.FindElement(By.XPath("//button[@ngbtooltip='Indietro']"));
                    bIndietro.Click();
                    WaitLoadPageEE12();
                    cnt++;
                }
            }
            catch
            {
                log.Info($"Indietroer() fail");
                if (!GoToListaLettura())
                    Keanu.TimeToRestart = true;
            }
        }

        private bool FillListaScarti()
        {
            try
            {
                var tableASL = Keanu.Driver.FindElement(By.XPath("//table[@class='firstRow']"));
                IList<IWebElement> tableRowsASL = tableASL.FindElements(By.TagName("tr"));
                int tcunt = 0;
                int cnt = 0;
                while (true && cnt < 30)
                {
                    if (tcunt == tableRowsASL.Count())
                        break;
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", tableRowsASL.Last());
                   // Thread.Sleep(Keanu.Randy(3));
                    tcunt = tableRowsASL.Count();
                    tableRowsASL = tableASL.FindElements(By.TagName("tr"));
                    cnt++;
                }

                AllScartiList = new List<Records.ArchivioScartiLavorabili>();
               if (tableRowsASL.Count > 1000) //check this shit because it is wrong
                {
                    Keanu.TimeToRestart = true;
                    return false;

                }
                foreach (var row in tableRowsASL.Skip(2))//SKIP HEADERS
                {
                    IList<IWebElement> tableColumnsASL = row.FindElements(By.TagName("td"));

                    string f1;
                    string f2;
                    string f3;
                    string r1;
                    string r2;
                    string r3;
                    string potenza;

                    if (string.IsNullOrEmpty(tableColumnsASL[5].Text)) { f1 = "0"; } else { f1 = tableColumnsASL[5].Text; }
                    if (string.IsNullOrEmpty(tableColumnsASL[6].Text)) { f2 = "0"; } else { f2 = tableColumnsASL[6].Text; }
                    if (string.IsNullOrEmpty(tableColumnsASL[7].Text)) { f3 = "0"; } else { f3 = tableColumnsASL[7].Text; }
                    if (string.IsNullOrEmpty(tableColumnsASL[8].Text)) { r1 = "0"; } else { r1 = tableColumnsASL[8].Text; }
                    if (string.IsNullOrEmpty(tableColumnsASL[9].Text)) { r2 = "0"; } else { r2 = tableColumnsASL[9].Text; }
                    if (string.IsNullOrEmpty(tableColumnsASL[10].Text)) { r3 = "0"; } else { r3 = tableColumnsASL[10].Text; }
                    if (string.IsNullOrEmpty(tableColumnsASL[11].Text)) { potenza = "0"; } else { potenza = tableColumnsASL[11].Text; }

                    DateTime today = DateTime.Today;
                    DateTime scartoDate = Convert.ToDateTime(tableColumnsASL[2].Text);
                    TimeSpan result = today - scartoDate;
                    int days = result.Days;

                    try
                    {
                        AllScartiList.Add(new Records.ArchivioScartiLavorabili
                        {
                            Checkbox = tableColumnsASL[0],
                            Pod = tableColumnsASL[1].Text,
                            DataLettura = Convert.ToDateTime(tableColumnsASL[2].Text),
                            PresaInCarico = tableColumnsASL[3].Text,
                            TipoLettura = tableColumnsASL[4].Text,
                            F1 = Convert.ToDouble(f1),
                            F2 = Convert.ToDouble(f2),
                            F3 = Convert.ToDouble(f3),
                            R1 = Convert.ToDouble(r1),
                            R2 = Convert.ToDouble(r2),
                            R3 = Convert.ToDouble(r3),
                            Potenza = Convert.ToDouble(potenza),
                            DataCaricamento = Convert.ToDateTime(tableColumnsASL[13].Text),
                            CodiceScarto = tableColumnsASL[14].Text,
                            Repository = tableColumnsASL[20].Text,
                            Days = days,
                        });
                    }
                    catch
                    {
                        log.Error($"Cannot add to AllScartiList");
                        return false;
                    }
                }

                AutoletturaScartiList = new List<Records.ArchivioScartiLavorabili>();
                if (Keanu.LavName == "EE112 - LIMBO ENEL-D" || Keanu.LavName == "EE112 - LIMBO DT TERZI")
                {
                    foreach (var item in AllScartiList)
                    {
                        if (item.TipoLettura.Contains("AUTOLETTURA") || item.TipoLettura.Contains("STIMATA DISTRIBUTORE"))
                        {
                            if (item.Repository.Equals("RIABBINABILE") || item.Repository.Equals("RIABBINABILE 2G"))
                                AutoletturaScartiList.Add(item);
                        }
                    }
                }
                else
                {
                    foreach (var item in AllScartiList)
                    {
                        if (item.TipoLettura.Contains("AUTOLETTURA") || item.TipoLettura.Contains("STIMATA DISTRIBUTORE"))
                            AutoletturaScartiList.Add(item);
                    }
                }

                ScartiList = new List<Records.ArchivioScartiLavorabili>();
                foreach (var item in AllScartiList)
                {
                    if (item.TipoLettura.Equals("REALE"))
                        ScartiList.Add(item);
                }

                RiabbinabileList = new List<Records.ArchivioScartiLavorabili>();
                foreach (var item in AllScartiList)
                {
                    if (item.Repository.Equals("RIABBINABILE") || item.Repository.Equals("RIABBINABILE 2G"))
                    {
                        if (item.TipoLettura.Contains("AUTOLETTURA") || item.TipoLettura.Contains("STIMATA DISTRIBUTORE"))
                            continue;
                        else
                            RiabbinabileList.Add(item);
                    }
                }

                log.Debug($"{AutoletturaScartiList.Count()} AUTOLETTURA/STIMATA");
                log.Debug($"{ScartiList.Count()} SCARTI, of which {RiabbinabileList.Count()} RIABBINABILE");
                return true;
            }
            catch
            {
                log.Error($"FillListaScarti() fail");
                return false;
            }
        }

        private bool FillListaLetture()
        {
            try
            {
                //log.Info("Refresh");
                Keanu.Driver.Navigate().Refresh();
                WaitLoadPageEE12();
            }
            catch (Exception)
            {

            }

            WaitLoadPageEE12();//TOO MUCH FillListaLetture() fail

            log.Info("Reading table");
            try
            {
                var tableLL = Keanu.Driver.FindElement(By.XPath("//table[@class='firstRow']"));
                IList<IWebElement> tableRowsLL = tableLL.FindElements(By.TagName("tr"));
                string stato = "";
                int tcunt = 0;
                int cnt = 0;
                while (true && cnt < 30)
                {
                    if (tcunt == tableRowsLL.Count())
                        break;
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", tableRowsLL.Last());
                    Thread.Sleep(Keanu.Randy(3));
                    tcunt = tableRowsLL.Count();
                    tableRowsLL = tableLL.FindElements(By.TagName("tr"));
                    cnt++;
                }

                ListaLetture = new List<Records.ListaLetture>();
                foreach (var row in tableRowsLL.Skip(2))//SKIP HEADERS
                {
                    IList<IWebElement> tableColumnsLL = row.FindElements(By.TagName("td"));
                    try
                    {
                        var val = tableColumnsLL[5].FindElements(By.CssSelector("*"));
                        foreach (var item in val)
                        {
                            string s = item.GetAttribute("class");
                            switch (s)
                            {
                                case "validata":
                                    stato = "validata";
                                    break;
                                case "nonValidata":
                                    stato = "nonvalidata";
                                    break;
                                case "annullata":
                                    stato = "annullata";
                                    break;
                                case "daStimare":
                                    stato = "dastimare";
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    catch
                    {
                        stato = "other";
                        log.Error($"Tipo other or empty ListaLetture");
                        return false;
                    }

                    string f1;
                    string f2;
                    string f3;
                    string r1;
                    string r2;
                    string r3;
                    bool reativaIsNull = false;
                    string potenza;
                    string p1;
                    string p2;
                    string p3;
                    bool potenzaDTIsNull = false;

                    //Take Attiva
                    if (string.IsNullOrEmpty(tableColumnsLL[10].Text)) { f1 = "0"; } else { f1 = tableColumnsLL[10].Text; }
                    if (string.IsNullOrEmpty(tableColumnsLL[11].Text)) { f2 = "0"; } else { f2 = tableColumnsLL[11].Text; }
                    if (string.IsNullOrEmpty(tableColumnsLL[12].Text)) { f3 = "0"; } else { f3 = tableColumnsLL[12].Text; }
                    //Take reativa
                    if (string.IsNullOrEmpty(tableColumnsLL[19].Text)) { r1 = "0"; reativaIsNull = true; } else { r1 = tableColumnsLL[19].Text; }
                    if (string.IsNullOrEmpty(tableColumnsLL[20].Text)) { r2 = "0"; reativaIsNull = true; } else { r2 = tableColumnsLL[20].Text; }
                    if (string.IsNullOrEmpty(tableColumnsLL[21].Text)) { r3 = "0"; reativaIsNull = true; } else { r3 = tableColumnsLL[21].Text; }
                    //Take potenza
                    if (string.IsNullOrEmpty(tableColumnsLL[23].Text)) { p1 = "0"; potenzaDTIsNull = true; } else { p1 = tableColumnsLL[23].Text; }
                    if (string.IsNullOrEmpty(tableColumnsLL[24].Text)) { p2 = "0"; potenzaDTIsNull = true; } else { p2 = tableColumnsLL[24].Text; }
                    if (string.IsNullOrEmpty(tableColumnsLL[25].Text)) { p3 = "0"; potenzaDTIsNull = true; } else { p3 = tableColumnsLL[25].Text; }
                    if (string.IsNullOrEmpty(tableColumnsLL[26].Text)) { potenza = "0"; } else { potenza = tableColumnsLL[26].Text; }

                    if (!string.IsNullOrEmpty(tableColumnsLL[1].Text))//IF EMPTY ROW (PROBABLY LAST) DON'T ADD TO LIST
                    {
                        if (stato.Equals("annullata"))  //ask tanja wat to do here
                            continue;//SKIP ALL annullata
                        try
                        {
                            ListaLetture.Add(new Records.ListaLetture
                            {
                                Gestione = tableColumnsLL[0],
                                Data = Convert.ToDateTime(tableColumnsLL[1].Text.Substring(0, 10)),
                                Causale = tableColumnsLL[2].Text,
                                Tipo = tableColumnsLL[3].Text,
                                Fonte = tableColumnsLL[4].Text,
                                Stato = stato,
                                F1 = Convert.ToDouble(f1),
                                F2 = Convert.ToDouble(f2),
                                F3 = Convert.ToDouble(f3),
                                //TotaleF = Convert.ToDouble(totalef),
                                R1 = Convert.ToDouble(r1),
                                R2 = Convert.ToDouble(r2),
                                R3 = Convert.ToDouble(r3),
                                //TotaleR = Convert.ToDouble(totaler),
                                Potenza = Convert.ToDouble(potenza),
                                P1 = Convert.ToDouble(p1),
                                P2 = Convert.ToDouble(p2),
                                P3 = Convert.ToDouble(p3),
                                Rowland = row.Text,
                                ReattivaIsNull = reativaIsNull,
                                PotenzaDTIsNull = potenzaDTIsNull,
                                StatoInvioSap = tableColumnsLL[25].Text,
                            });
                        }
                        catch
                        {
                            log.Error($"Cannot add to ListaLetture");
                            return false;
                        }
                    }
                }

                RealeLetture = new List<Records.ListaLetture>();
                foreach (var item in ListaLetture)
                {
                    if (item.Tipo.Equals("REALE"))
                        RealeLetture.Add(item);
                }
                return true;
            }
            catch
            {
                Keanu.TimeToRestart = true;
                log.Error($"FillListaLetture() fail");
                return false;
            }
        }

        private bool FillListaLetturePerFlussi2G()
        {
            try
            {
                //log.Info("Refresh");
                Keanu.Driver.Navigate().Refresh();
                WaitLoadPageEE12();
            }
            catch (Exception)
            {

            }

            try
            {
                var tableLL = Keanu.Driver.FindElement(By.XPath("//table[@class='firstRow']"));
                IList<IWebElement> tableRowsLL = tableLL.FindElements(By.TagName("tr"));
                string stato = "";
                int tcunt = 0;
                int cnt = 0;
                while (true && cnt < 30)
                {
                    if (tcunt == tableRowsLL.Count())
                        break;
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", tableRowsLL.Last());
                    Thread.Sleep(Keanu.Randy(3));
                    tcunt = tableRowsLL.Count();
                    tableRowsLL = tableLL.FindElements(By.TagName("tr"));
                    cnt++;
                }

                ListaLetture = new List<Records.ListaLetture>();
                foreach (var row in tableRowsLL.Skip(2))//SKIP HEADERS
                {
                    IList<IWebElement> tableColumnsLL = row.FindElements(By.TagName("td"));
                    try
                    {
                        var val = tableColumnsLL[5].FindElements(By.CssSelector("*"));
                        foreach (var item in val)
                        {
                            string s = item.GetAttribute("class");
                            switch (s)
                            {
                                case "validata":
                                    stato = "validata";
                                    break;
                                case "nonValidata":
                                    stato = "nonvalidata";
                                    break;
                                case "annullata":
                                    stato = "annullata";
                                    break;
                                case "daStimare":
                                    stato = "dastimare";
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    catch
                    {
                        stato = "other";
                        log.Error($"Tipo other");
                    }

                    if (!string.IsNullOrEmpty(tableColumnsLL[1].Text))//IF EMPTY ROW (PROBABLY LAST) DON'T ADD TO LIST
                    {
                        if (stato.Equals("annullata")) //sprositj tanju cho delatj. 
                        {
                            log.Debug("Contains stato Annulato");
                            return false;

                        }
                        try
                        {
                            ListaLetture.Add(new Records.ListaLetture
                            {
                                Gestione = tableColumnsLL[0],
                                Data = Convert.ToDateTime(tableColumnsLL[1].Text.Substring(0, 10)),
                                Causale = tableColumnsLL[2].Text,
                                Stato = stato,
                                Tipo = tableColumnsLL[3].Text,
                                Rowland = row.Text,
                            });
                        }
                        catch
                        {
                            log.Error($"Cannot add to ListaLetture");
                            return false;
                        }
                    }
                }

                RealeLetture = new List<Records.ListaLetture>();
                foreach (var item in ListaLetture)
                {
                    if (item.Tipo.Equals("REALE"))
                        RealeLetture.Add(item);
                }
                return true;
            }
            catch
            {
                Keanu.TimeToRestart = true;
                log.Error($"FillListaLetturePerFlussi2G() fail");
                return false;
            }
        }

        private bool GetAllVariablesFromFlussi2GWithRealeList()
        {
            try
            {
                var table = Keanu.Driver.FindElement(By.XPath("//tbody[@class='ng-star-inserted']"));
                IList<IWebElement> tableRows = table.FindElements(By.TagName("tr"));
                int tcunt = 0;
                int cnt = 0;
                while (true && cnt < 30)
                {
                    if (tcunt == tableRows.Count())
                        break;
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", tableRows.Last());
                    Thread.Sleep(Keanu.Randy(3));
                    tcunt = tableRows.Count();
                    tableRows = table.FindElements(By.TagName("tr"));
                    cnt++;
                }

                List<Records.Flussi2G> tempFlussi2g = null;
                tempFlussi2g = new List<Records.Flussi2G>();

                foreach (var row in tableRows)
                {
                    IList<IWebElement> tableColumns = row.FindElements(By.TagName("td"));
                    try
                    {
                        tempFlussi2g.Add(new Records.Flussi2G
                        {
                            Pljus = tableColumns[1],
                            DataMisura = Convert.ToDateTime(tableColumns[4].Text.Substring(0, 10)),
                            DataCaricamento = Convert.ToDateTime(tableColumns[6].Text.Substring(0, 10)),
                        });
                    }
                    catch (Exception Ex)
                    {
                        log.Error($"Cannot add to tempFlussi2g");
                        return false;
                    }
                }

                foreach (var tmpRealeletture in TempRealeLetture)
                {
                    string realeMonthYear = tmpRealeletture.Data.ToString("MM/yyyy");
                    foreach (var tmpflussi2g in tempFlussi2g)
                    {
                        string flussi2gMonthYear = tmpflussi2g.DataMisura.ToString("MM/yyyy");
                        if (flussi2gMonthYear.Equals(realeMonthYear))
                        {
                            if (tmpflussi2g.DataMisura.Equals(tmpRealeletture.Data))
                            {
                                log.Info($"Found {tmpRealeletture.Data.ToShortDateString()}");
                                break;
                            }
                            else
                            {
                                bool found = false;//IF IN ONE MONTH THERE ARE 2+ ROWS
                                foreach (var item in tempFlussi2g)
                                {
                                    if (item.DataMisura.Equals(tmpRealeletture.Data))
                                    {
                                        log.Info($"Found {tmpRealeletture.Data.ToShortDateString()}");
                                        found = true;
                                        break;
                                    }
                                }
                                if (found)
                                    break;

                                log.Debug($"Found {tmpRealeletture.Data.ToShortDateString()}, Expand {tmpflussi2g.DataMisura.ToShortDateString()}");
                                cnt = 0;
                                while (true && cnt < 30)
                                {
                                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", tmpflussi2g.Pljus);
                                    try
                                    {
                                        tmpflussi2g.Pljus.Click();
                                        WaitLoadPageEE12();
                                        Thread.Sleep(Keanu.Randy(3));
                                        break;
                                    }
                                    catch
                                    {
                                        cnt++;
                                        Thread.Sleep(Keanu.Randy(1));
                                        continue;
                                    }
                                }

                                table = Keanu.Driver.FindElement(By.XPath("//tbody[@class='ng-star-inserted']"));
                                tableRows = table.FindElements(By.TagName("tr"));
                                tcunt = 0;
                                cnt = 0;
                                while (true && cnt < 30)
                                {
                                    if (tcunt == tableRows.Count())
                                        break;
                                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", tableRows.Last());
                                    Thread.Sleep(Keanu.Randy(3));
                                    tcunt = tableRows.Count();
                                    tableRows = table.FindElements(By.TagName("tr"));
                                    cnt++;
                                }

                                tempFlussi2g = new List<Records.Flussi2G>();

                                foreach (var row in tableRows)//RELOAD tempFlussi2g FOR NEXT ROWS IN MAIN FOREACH
                                {
                                    IList<IWebElement> tableColumns = row.FindElements(By.TagName("td"));
                                    try
                                    {
                                        tempFlussi2g.Add(new Records.Flussi2G
                                        {
                                            Pljus = tableColumns[1],
                                            DataMisura = Convert.ToDateTime(tableColumns[4].Text.Substring(0, 10)),
                                            DataCaricamento = Convert.ToDateTime(tableColumns[7].Text.Substring(0, 10)),
                                        });
                                    }
                                    catch (Exception Ex)
                                    {
                                        log.Error($"Cannot add to expanded tempFlussi2g");
                                        return false;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }

                WaitLoadPageEE12();

                #region READ EXPANDED TABLE + GET INFO SYMBOL FROM TABLE ROWS
                table = Keanu.Driver.FindElement(By.XPath("//tbody[@class='ng-star-inserted']"));
                tableRows = table.FindElements(By.TagName("tr"));
                tcunt = 0;
                cnt = 0;
                while (true && cnt < 30)
                {
                    if (tcunt == tableRows.Count())
                        break;
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", tableRows.Last());
                    Thread.Sleep(Keanu.Randy(3));
                    tcunt = tableRows.Count();
                    tableRows = table.FindElements(By.TagName("tr"));
                    cnt++;
                }

                tempFlussi2g = new List<Records.Flussi2G>();

                foreach (var row in tableRows)
                {
                    bool info = false;
                    IList<IWebElement> tableColumns = row.FindElements(By.TagName("td"));

                    try
                    {
                        var val = tableColumns[4].FindElements(By.CssSelector("*"));
                        foreach (var item in val)
                        {
                            string s = item.GetAttribute("title");
                            if (!string.IsNullOrEmpty(s))
                            {
                                if (s.Contains("Sono presenti"))
                                    info = true;
                            }
                        }
                    }
                    catch { }

                    string f1 = "";
                    string f2 = "";
                    string f3 = "";
                    string r1 = "";
                    string r2 = "";
                    string r3 = "";

                    if (string.IsNullOrEmpty(tableColumns[10].Text)) { f1 = "0"; } else { f1 = tableColumns[10].Text; }
                    if (string.IsNullOrEmpty(tableColumns[11].Text)) { f2 = "0"; } else { f2 = tableColumns[11].Text; }
                    if (string.IsNullOrEmpty(tableColumns[12].Text)) { f3 = "0"; } else { f3 = tableColumns[12].Text; }
                    if (string.IsNullOrEmpty(tableColumns[13].Text)) { r1 = "0"; } else { r1 = tableColumns[13].Text; }
                    if (string.IsNullOrEmpty(tableColumns[14].Text)) { r2 = "0"; } else { r2 = tableColumns[14].Text; }
                    if (string.IsNullOrEmpty(tableColumns[15].Text)) { r3 = "0"; } else { r3 = tableColumns[15].Text; }

                    if (f1.ToString().Contains(",")) { f1 = f1.Split(',')[0]; }
                    if (f2.ToString().Contains(",")) { f2 = f2.Split(',')[0]; }
                    if (f3.ToString().Contains(",")) { f3 = f3.Split(',')[0]; }

                    if (r1.ToString().Contains(",")) { r1 = r1.Split(',')[0]; }
                    if (r2.ToString().Contains(",")) { r2 = r2.Split(',')[0]; }
                    if (r3.ToString().Contains(",")) { r3 = r3.Split(',')[0]; }

                    try
                    {
                        tempFlussi2g.Add(new Records.Flussi2G
                        {
                            DataMisura = Convert.ToDateTime(tableColumns[4].Text.Substring(0, 10)),
                            DataCaricamento = Convert.ToDateTime(tableColumns[6].Text.Substring(0, 10)),
                            Tipo = tableColumns[7].Text,
                            F1 = Convert.ToDouble(f1),
                            F2 = Convert.ToDouble(f2),
                            F3 = Convert.ToDouble(f3),
                            R1 = Convert.ToDouble(r1),
                            R2 = Convert.ToDouble(r2),
                            R3 = Convert.ToDouble(r3),
                            Rowland = row.Text,
                            InfoSymbol = info,
                            WholeRowElement = row
                        });
                    }
                    catch (Exception Ex)
                    {
                        log.Error($"Cannot add to tempFlussi2g");
                        return false;
                    }
                }
                #endregion

                TempRealeLettureWichHasToBeModified = new List<Records.Flussi2G>();

                //ALREADY ALL WHAT NEED IS EXPANDED
                foreach (var tmpflussi2g in tempFlussi2g)//FLUSSI2G LIST FIRST BECAUSE HAS TO BE ASC DATETIME
                {
                    foreach (var tmprealeletture in TempRealeLetture)
                    {
                        if (tmpflussi2g.DataMisura.Equals(tmprealeletture.Data))
                        {
                            if (tmpflussi2g.InfoSymbol)
                            {
                                cnt = 0;
                                while (true && cnt < 30)
                                {
                                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", tmpflussi2g.WholeRowElement);
                                    try
                                    {
                                        tmpflussi2g.WholeRowElement.Click();
                                        WaitLoadPageEE12();
                                        Thread.Sleep(Keanu.Randy(1));
                                        break;
                                    }
                                    catch
                                    {
                                        cnt++;
                                        Thread.Sleep(Keanu.Randy(1));
                                        continue;
                                    }
                                }

                                //GET DATA FROM POPUP
                                List<Records.Flussi2G> popupFlussi2g = null;
                                popupFlussi2g = new List<Records.Flussi2G>();

                                table = Keanu.Driver.FindElement(By.XPath("//next-table[@class='adv ng-star-inserted']"));
                                IList<IWebElement> tableRowz = table.FindElements(By.TagName("tr"));
                                tcunt = 0;
                                cnt = 0;
                                while (true && cnt < 30)
                                {
                                    if (tcunt == tableRowz.Count())
                                        break;
                                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", tableRowz.Last());
                                    Thread.Sleep(Keanu.Randy(1));
                                    tcunt = tableRowz.Count();
                                    tableRowz = table.FindElements(By.TagName("tr"));
                                    cnt++;
                                }

                                foreach (var row in tableRowz.Skip(2))//SKIP HEADERS
                                {
                                    IList<IWebElement> tableColumns = row.FindElements(By.TagName("td"));

                                    string f1 = "";
                                    string f2 = "";
                                    string f3 = "";
                                    string r1 = "";
                                    string r2 = "";
                                    string r3 = "";

                                    if (string.IsNullOrEmpty(tableColumns[9].Text)) { f1 = "0"; } else { f1 = tableColumns[9].Text; }
                                    if (string.IsNullOrEmpty(tableColumns[10].Text)) { f2 = "0"; } else { f2 = tableColumns[10].Text; }
                                    if (string.IsNullOrEmpty(tableColumns[11].Text)) { f3 = "0"; } else { f3 = tableColumns[11].Text; }
                                    if (string.IsNullOrEmpty(tableColumns[12].Text)) { r1 = "0"; } else { r1 = tableColumns[12].Text; }
                                    if (string.IsNullOrEmpty(tableColumns[13].Text)) { r2 = "0"; } else { r2 = tableColumns[13].Text; }
                                    if (string.IsNullOrEmpty(tableColumns[14].Text)) { r3 = "0"; } else { r3 = tableColumns[14].Text; }

                                    if (f1.ToString().Contains(",")) { f1 = f1.Split(',')[0]; }
                                    if (f2.ToString().Contains(",")) { f2 = f2.Split(',')[0]; }
                                    if (f3.ToString().Contains(",")) { f3 = f3.Split(',')[0]; }

                                    if (r1.ToString().Contains(",")) { r1 = r1.Split(',')[0]; }
                                    if (r2.ToString().Contains(",")) { r2 = r2.Split(',')[0]; }
                                    if (r3.ToString().Contains(",")) { r3 = r3.Split(',')[0]; }

                                    try
                                    {
                                        popupFlussi2g.Add(new Records.Flussi2G
                                        {
                                            DataMisura = Convert.ToDateTime(tableColumns[3].Text.Substring(0, 10)),
                                            F1 = Convert.ToDouble(f1),
                                            F2 = Convert.ToDouble(f2),
                                            F3 = Convert.ToDouble(f3),
                                            R1 = Convert.ToDouble(r1),
                                            R2 = Convert.ToDouble(r2),
                                            R3 = Convert.ToDouble(r3),
                                            Rowland = row.Text,
                                            WholeRowElement = row
                                        });
                                    }
                                    catch (Exception Ex)
                                    {
                                        log.Error($"Cannot add to popupFlussi2g");
                                        return false;
                                    }
                                }

                                try
                                {
                                    var bClose = Keanu.Driver.FindElement(By.XPath("//button[@class='close']"));
                                    bClose.Click();//CLOSE POPUP
                                    WaitLoadPageEE12();
                                }
                                catch (Exception Ex)
                                {
                                    var bClose = Keanu.Driver.FindElement(By.XPath("//button[@class='close']"));
                                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bClose);
                                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bClose);
                                    WaitLoadPageEE12();
                                }

                                TempRealeLettureWichHasToBeModified.Add(popupFlussi2g[0]);
                                break;
                            }
                            else
                            {
                                TempRealeLettureWichHasToBeModified.Add(tmpflussi2g);
                                break;
                            }
                        }
                    }
                }

                if (TempRealeLetture.Count() != TempRealeLettureWichHasToBeModified.Count())
                {
                    log.Error($"Didn't find all rows for modifying");
                    return false;
                }
                log.Info($"Got variables from flussi2g for modifying {TempRealeLettureWichHasToBeModified.Count} rows");
                log.Info($"Reverse tempRealeLettureWichHasToBeModified");
                //TempRealeLettureWichHasToBeModified.OrderBy(x => x.DataMisura).ToList();
                TempRealeLettureWichHasToBeModified.Reverse();
                return true;
            }
            catch (Exception Ex)
            {
                log.Error($"GetAllVariablesFromFlussi2GWithRealeList() fail");
                return false;
            }
        }

        private bool WaitLoadPageEE12()
        {
            try
            {
                int i = 0;
                string page = Keanu.Driver.PageSource.ToString();
                bool pageIsLoaded = false;
                string s = "";
                Thread.Sleep(Keanu.Randy(1));
                while (!pageIsLoaded && i < 30)
                {
                    if (Keanu.Driver.Url.Contains("next-unauthorized"))
                    {
                        //Accesso negato. Contattare l'amministratore di sistema.
                        Keanu.Driver.Navigate().Back();
                        Thread.Sleep(Keanu.Randy(1));
                    }

                    if (!s.Equals(page))
                    {
                        s = page;
                        Thread.Sleep(Keanu.Randy(1));
                        page = Keanu.Driver.PageSource.ToString();
                        i++;
                    }
                    else
                        pageIsLoaded = true;
                }

                while (IsElementPresent(By.XPath("//div[@class = 'spinner-three-bounce full-screen ng-star-inserted']"), Keanu.Driver))
                {
                    log.Info($"*** Ballz");//BALLSACK
                    Thread.Sleep(Keanu.Randy(3));
                }

                return pageIsLoaded;
            }
            catch
            {
                return false;
            }
        }

        private bool Sposter()
        {
            try
            {
                int cnt = 0;
                while (true && cnt < 30)
                {
                    var bModifica = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'MODIFICA')]"));
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bModifica);
                    try
                    {
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bModifica);
                        WaitLoadPageEE12();
                        break;
                    }
                    catch
                    {
                        cnt++;
                        Thread.Sleep(Keanu.Randy(1));
                    }
                }

                IWebElement frame = null;
                cnt = 0;
                while (frame == null && cnt < 3)
                {
                    try
                    {
                        frame = Keanu.Driver.FindElement(By.XPath(".//ngb-modal-window[@class='modal fade show d-block modal-xxl']"));
                    }
                    catch
                    {
                        cnt++;
                        log.Info("Modifica Frame Ex");
                        Thread.Sleep(Keanu.Randy(1));
                    }
                }

                var presaInCarico = Keanu.Driver.FindElement(By.ClassName("control-cb-indicator"));
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", presaInCarico);
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", presaInCarico);
                WaitLoadPageEE12();

                IWebElement ComboNote = Keanu.Driver.FindElement(By.XPath(".//mat-select[@formcontrolname='note']"));
                if (ComboNote.Text.Contains("SELEZIONA"))
                {
                    ComboNote.Click();
                    Thread.Sleep(Keanu.Randy(1));
                    IList<IWebElement> OptionsCausale = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                    IWebElement OptionCausale = null;
                    try
                    {
                        OptionCausale = OptionsCausale.Where(x => x.Text.ToUpper().Contains("ALTRO")).ToList()[0];
                        OptionCausale.Click();
                        Thread.Sleep(Keanu.Randy(1));
                    }
                    catch
                    {
                        // skoree vsego zalogal next, no nuzno smotretj (eshe stoit galochka na presa in carico)
                        //Keanu.TimeToSospeso = true;
                        //Keanu.TimeToSospesoType = 3;
                        presaInCaricoScarto = true;
                        log.Info("Presa in carico checkbox, will go scarto");
                        //log.Info("Note Ex");
                        return false;
                    }
                }

                IWebElement ComboChiusuraScarto = Keanu.Driver.FindElement(By.XPath(".//mat-select[@formcontrolname='chiusuraScarto']"));
                if (ComboChiusuraScarto.Text.Contains("NO"))
                {
                    ComboChiusuraScarto.Click();
                    Thread.Sleep(Keanu.Randy(1));
                    IList<IWebElement> OptionsCausale = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                    IWebElement OptionCausale = null;
                    try
                    {
                        OptionCausale = OptionsCausale.Where(x => x.Text.ToUpper().Contains("SI")).ToList()[0];
                        OptionCausale.Click();
                        Thread.Sleep(Keanu.Randy(1));
                    }
                    catch
                    {
                        log.Info("Note Ex");
                    }
                }

                var bConferma = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'CONFERMA')]"));
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bConferma);
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bConferma);
                WaitLoadPageEE12();

                var bConferma2 = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'Conferma')]"));
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bConferma2);
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bConferma2);
                WaitLoadPageEE12();

                if (Keanu.Driver.PageSource.ToString().Contains("CHIAVI DUPLICATE"))
                {
                    log.Error($"CHIAVI DUPLICATE");
                    chiaviDuplicate = true;
                }
                if (Keanu.Driver.PageSource.ToString().Contains("Operazione effettuata con successo"))
                {
                    log.Debug($"Operazione effettuata con successo");
                }
                if (Keanu.Driver.PageSource.ToString().Contains("Modifica KO - Errore tecnico"))
                {
                    log.Debug($"Modifica KO - Errore tecnico");
                    MkayClose("");
                    return false;
                }

                if (Keanu.Driver.PageSource.ToString().Contains("Modifica non ammessa - Selezione multipla consentila solo per scarti non presi in carico")) {
                    log.Debug($"Modifica non ammessa - Selezione multipla consentila solo per scarti non presi in carico");
                    Keanu.TimeToSospeso = true;
                    Keanu.TimeToSospesoType = 4;
                    return false;
                }

                var bOk = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'OK')]"));
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bOk);
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bOk);
                WaitLoadPageEE12();
                return true;

            }
            catch (Exception)
            {
                return false;
            }

            //try
            //{
            //    var bSposta = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'SPOSTA')]"));
            //    bSposta.Click();
            //    WaitLoadPageEE12();

            //    var bConferma = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'Conferma')]"));
            //    bConferma.Click();
            //    WaitLoadPageEE12();

            //    if (Keanu.Driver.PageSource.ToString().Contains("CHIAVI DUPLICATE"))
            //    {
            //        log.Error($"CHIAVI DUPLICATE");
            //        chiaviDuplicate = true;
            //    }

            //    var bOk = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'OK')]"));
            //    bOk.Click();
            //    WaitLoadPageEE12();
            //    return true;
            //}
            //catch
            //{
            //    Keanu.TimeToRestart = true;
            //    log.Error($"Sposter() fail");
            //    return false;
            //}
        }

        private bool GoTo(string MenuName)
        {
            try
            {
                var bVaiA = Keanu.Driver.FindElement(By.Id("dropdownBasic2"));
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bVaiA);
            }
            catch
            {
                log.Debug("Cannot click Vai A");
                Thread.Sleep(Keanu.Randy(5));
                return false;
            }
            try
            {
                IList<IWebElement> Buttons = Keanu.Driver.FindElements(By.XPath(".//div[@class = 'menuWidth dropdown-menu show']/button")).ToList();
                IWebElement ButtonGoTo = Buttons.Where(x => x.Text.ToUpper().Contains(MenuName.ToUpper())).ToList()[0];
                if (ButtonGoTo != null) ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", ButtonGoTo);
            }
            catch
            {
                log.Error($"Cannot click {MenuName}");
                return false;
            }
            WaitLoadPageEE12();
            return true;
        }

        private void ClickAzioniModifica(Records.ListaLetture all)
        {
            int cnt = 0;
            while (true && cnt < 30)
            {
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", all.Gestione);
                try
                {
                    all.Gestione.Click();
                    IWebElement bModifica = Keanu.Driver.FindElement(By.XPath(".//b[text() = 'Modifica']")).FindElement(By.XPath(".."));
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bModifica);
                    WaitLoadPageEE12();
                    break;
                }
                catch
                {
                    Actions a = new Actions(Keanu.Driver);
                    a.SendKeys(Keys.Escape).Build().Perform();
                    cnt++;
                    Thread.Sleep(Keanu.Randy(1));
                    continue;
                }
            }
        }

        private bool ClickAzioniCancella(Records.ListaLetture all)
        {
            int cnt = 0;
            while (true && cnt < 30)
            {
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", all.Gestione);
                try
                {
                    all.Gestione.Click();
                    IWebElement bCancella = Keanu.Driver.FindElement(By.XPath(".//b[text() = 'Cancella']")).FindElement(By.XPath(".."));
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bCancella);
                    WaitLoadPageEE12();
                    break;
                }
                catch
                {
                    Actions a = new Actions(Keanu.Driver);
                    a.SendKeys(Keys.Escape).Build().Perform();
                    cnt++;
                    Thread.Sleep(Keanu.Randy(1));
                    continue;
                }
            }

            try
            {
                //CONFERMA [1]
                IWebElement bConferma = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'Conferma')]"));
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bConferma);
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bConferma);
                WaitLoadPageEE12();

                //OK
                IWebElement bOk = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'OK')]"));
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bOk);
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bOk);
                WaitLoadPageEE12();

                log.Info($"Cancellato {all.Data.ToShortDateString()}");
                return true;
            }
            catch (Exception)
            {
                MkayClose("CLOSE");
                return false;
            }
        }

        private bool ClickAzioniAnnulla(Records.ListaLetture all)
        {
            int cnt = 0;
            while (true && cnt < 30)
            {
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", all.Gestione);
                try
                {
                    all.Gestione.Click();
                    IWebElement bAnnulla = Keanu.Driver.FindElement(By.XPath(".//b[text() = 'Annulla']")).FindElement(By.XPath(".."));
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bAnnulla);
                    WaitLoadPageEE12();
                    break;
                }
                catch
                {
                    Actions a = new Actions(Keanu.Driver);
                    a.SendKeys(Keys.Escape).Build().Perform();
                    cnt++;
                    Thread.Sleep(Keanu.Randy(1));
                    continue;
                }
            }

            try
            {
                IWebElement ComboCausaleDiRicalcolo = Keanu.Driver.FindElement(By.XPath(".//mat-select[@name='causaleRicalcolo']"));
                ComboCausaleDiRicalcolo.Click();
                IList<IWebElement> OptionsCausaleDiRicalcolo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                IWebElement OptionCausaleDiRicalcolo = null;

                OptionCausaleDiRicalcolo = OptionsCausaleDiRicalcolo.Where(x => x.Text.ToUpper().Contains("RICALCOLO PER LETTURA PRECEDENTEMENTE ERRATA")).ToList()[0];
                OptionCausaleDiRicalcolo.Click();
                WaitLoadPageEE12();

                //CONFERMA
                IWebElement modal = Keanu.Driver.FindElement(By.ClassName("modal-content"));
                IWebElement modalDiv = modal.FindElement(By.XPath("//div[@class='modal-footer']"));
                var buttons = modalDiv.FindElements(By.TagName("button"));
                buttons[0].Click();
                WaitLoadPageEE12();

                //CONFERMA [1]
                IWebElement bConferma = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'Conferma')]"));
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bConferma);
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bConferma);
                WaitLoadPageEE12();

                //OK
                IWebElement bOk = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'OK')]"));
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bOk);
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bOk);
                WaitLoadPageEE12();

                log.Info($"Annullato {all.Data.ToShortDateString()}");
                return true;
            }
            catch
            {
                MkayClose("CLOSE");
                return false;
            }
        }

        private bool InserisciLettura(Records.ArchivioScartiLavorabili Scarti, Records.ListaLetture Reale)
        {
            try
            {
                IWebElement mainframe = null;
                int cnt = 0;
                while (mainframe == null && cnt < 3)
                {
                    try
                    {
                        mainframe = Keanu.Driver.FindElement(By.XPath(".//next-smart-table[@class = 'customTable']"));
                    }
                    catch
                    {
                        cnt++;
                        log.Info("Inserisci Lettura Ex");
                        Thread.Sleep(Keanu.Randy(1));
                    }
                }

                IWebElement ButtonInserisciLettura = null;
                IList<IWebElement> ListaButtonOperativita = Keanu.Driver.FindElements(By.XPath(".//button[@class = 'btn btn-outline-primary-no-border cursor-pointer labelPink']"));
                try
                {
                    ButtonInserisciLettura = ListaButtonOperativita.Where(x => x.Text.ToUpper().Contains("INSERISCI LETTURE")).ToList<IWebElement>()[0];
                }
                catch
                { }
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", ButtonInserisciLettura);

                IWebElement frame = null;
                cnt = 0;
                while (frame == null && cnt < 3)
                {
                    try
                    {
                        frame = Keanu.Driver.FindElement(By.XPath(".//ngb-modal-window[@class = 'modal fade show d-block modal-xxl']"));
                    }
                    catch
                    {
                        cnt++;
                        Thread.Sleep(Keanu.Randy(1));
                    }
                }

                IWebElement CampoData = Keanu.Driver.FindElement(By.XPath(".//input[@name='dtCompetence']"));
                CampoData.Clear();
                CampoData.SendKeys(Scarti.DataLettura.ToShortDateString().Substring(0, 10));
                WaitLoadPageEE12();
                Actions a = new Actions(Keanu.Driver);
                a.SendKeys(Keys.Tab).Build().Perform();
                WaitLoadPageEE12();

                if (Keanu.Driver.PageSource.ToString().Contains("Matricola attiva non presente per la fornitura selezionata"))
                {
                    log.Info($"Matricola attiva non presente per la fornitura selezionata");
                    MkayClose("");
                    return false;
                }

                if (Keanu.Driver.PageSource.ToString().Contains("Alla data specificata esiste un processo commerciale/tecnico"))
                {
                    //CLICK CONFERMA, MAIL 11/02/2021 0743
                    IWebElement bMiniConferma = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'Conferma')]"));
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bMiniConferma);
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bMiniConferma);
                    WaitLoadPageEE12();
                }

                IWebElement ComboTipo = Keanu.Driver.FindElement(By.XPath(".//mat-select[@name='type']"));
                if (!ComboTipo.Text.Contains("REALE"))
                {
                    ComboTipo.Click();
                    Thread.Sleep(Keanu.Randy(1));
                    IList<IWebElement> OptionsTipo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                    IWebElement OptionTipoReale = null;
                    try
                    {
                        OptionTipoReale = OptionsTipo.Where(x => x.Text.ToUpper().Contains("REALE")).ToList()[0];
                        OptionTipoReale.Click();
                        Thread.Sleep(Keanu.Randy(1));
                    }
                    catch
                    {
                        log.Info("Tipo Lettura Ex");
                    }
                }

                IWebElement ComboCausale = Keanu.Driver.FindElement(By.XPath(".//mat-select[@name='causale']"));
                if (ComboCausale.Text.Contains("SELEZIONA"))
                {
                    ComboCausale.Click();
                    Thread.Sleep(Keanu.Randy(1));
                    IList<IWebElement> OptionsCausale = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                    IWebElement OptionCausale = null;
                    try
                    {
                        OptionCausale = OptionsCausale.Where(x => x.Text.ToUpper().Contains("RICALCOLO PER LETTURA PRECEDENTEMENTE ERRATA")).ToList()[0];
                        OptionCausale.Click();
                        Thread.Sleep(Keanu.Randy(1));
                    }
                    catch
                    {
                        log.Info("Causale di Ricalcolo Ex");
                    }
                }

                if (!InsertAttivaReattivaPot(Scarti, Reale, true))
                    return false;

                cnt = 0;
                while (true && cnt < 30)
                {
                    var bConferma = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'CONFERMA')]"));
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bConferma);
                    try
                    {
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bConferma);
                        WaitLoadPageEE12();
                        break;
                    }
                    catch
                    {
                        cnt++;
                        Thread.Sleep(Keanu.Randy(1));
                    }
                }

                WaitLoadPageEE12();

                try
                {
                    cnt = 0;
                    while (!Keanu.Driver.PageSource.ToString().Contains("Operazione effettuata con successo") && cnt < 10)
                    {
                        if (cnt == 9)
                        {
                            log.Info("Some error after conferma");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Stessa entità aggiornata da un altro operatore"))
                        {
                            log.Info("Stessa entità aggiornata da un altro operatore");
                            MkayClose("");
                            //Keanu.TimeToRestart = true;
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Stessa misura aggiornata da un altro operatore"))
                        {
                            log.Info("Stessa misura aggiornata da un altro operatore");
                            MkayClose("");
                            //Keanu.TimeToRestart = true;
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Errore tecnico"))
                        {
                            log.Info("Errore tecnico");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Misura non inserita perché già presente una misura con le stesse segnanti"))
                        {
                            log.Info($"Misura non inserita perché già presente una misura con le stesse segnanti");
                            MkayClose("");
                            log.Info($"Add to sposta list");
                            log.Info($"Don't reload Lista Letture");
                            needToReloadListaLetture = false;
                            return true;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Numero cifre misura precedente non consentito"))
                        {
                            log.Info("Numero cifre misura precedente non consentito");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Numero di cifre superiore a quelle del misuratore"))
                        {
                            log.Info("Numero di cifre superiore a quelle del misuratore");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia attiva nullo e prelievo di potenza non nullo"))
                        {
                            log.Info($"Consumo di energia attiva nullo e prelievo di potenza non nullo");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia attiva superiore al consumo massimo"))
                        {
                            log.Info($"Consumo di energia attiva superiore al consumo massimo");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia reattiva superiore al consumo massimo"))
                        {
                            log.Info($"Consumo di energia reattiva superiore al consumo massimo");
                            MkayClose("OK");

                            if (Reale.Potenza <= potete)
                            {
                                log.Info($"Reale potenza <= 15");

                                if (!InsertAttivaReattivaPot(Scarti, Reale, false))
                                    return false;

                                cnt = 0;
                                while (true && cnt < 30)
                                {
                                    var bConferma = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'CONFERMA')]"));
                                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bConferma);
                                    try
                                    {
                                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bConferma);
                                        WaitLoadPageEE12();
                                        break;
                                    }
                                    catch
                                    {
                                        cnt++;
                                        Thread.Sleep(Keanu.Randy(1));
                                    }
                                }
                                if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia reattiva superiore al consumo massimo"))
                                {
                                    log.Info($"2nd try");
                                    log.Info($"Consumo di energia reattiva superiore al consumo massimo");
                                    MkayClose("");
                                    return false;
                                }
                            }
                            else
                            {
                                log.Info($"Reale potenza > 15");
                                MkayClose("CLOSE");
                                return false;
                            }
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Presenti letture alla stessa data o successive. Selezionare una causale di ricalcolo"))
                        {
                            log.Info("Presenti letture alla stessa data o successive. Selezionare una causale di ricalcolo");
                            IWebElement ComboCausaleDiRicalcolo = Keanu.Driver.FindElement(By.XPath(".//mat-select[@name='causaleRicalcolo']"));
                            ComboCausaleDiRicalcolo.Click();
                            IList<IWebElement> OptionsCausaleDiRicalcolo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                            IWebElement OptionCausaleDiRicalcolo = null;
                            try
                            {
                                OptionCausaleDiRicalcolo = OptionsCausaleDiRicalcolo.Where(x => x.Text.ToUpper().Contains("RICALCOLO PER LETTURA PRECEDENTEMENTE ERRATA")).ToList()[0];
                                OptionCausaleDiRicalcolo.Click();
                                WaitLoadPageEE12();
                            }
                            catch
                            {
                                MkayClose("CLOSE");
                                return false;
                            }
                            try
                            {
                                IWebElement modal = Keanu.Driver.FindElement(By.ClassName("modal-content"));
                                IWebElement modalDiv = modal.FindElement(By.XPath("//div[@class='modal-footer']"));
                                var buttons = modalDiv.FindElements(By.TagName("button"));
                                buttons[1].Click();
                            }
                            catch
                            {
                                MkayClose("CLOSE");
                                return false;
                            }
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Formato non corretto"))
                        {
                            log.Info($"Formato non corretto");
                            MkayClose("CLOSE");
                            return false;
                        }
                        cnt++;
                        WaitLoadPageEE12();
                    }
                    needToReloadListaLetture = true;
                    MkayClose("OK");

                    upeCounter++;
                    log.Warn($"upeCounter {upeCounter}");

                    return true;
                }
                catch
                {
                    MkayClose("CLOSE");
                    return false;
                }
            }
            catch
            {
                MkayClose("CLOSE");
                return false;
            }
        }

        private bool InserisciLetturaWithVariablesFromFlussi2Gg(Records.Flussi2G Flussi)
        {
            try
            {
                IWebElement mainframe = null;
                int cnt = 0;
                while (mainframe == null && cnt < 3)
                {
                    try
                    {
                        mainframe = Keanu.Driver.FindElement(By.XPath(".//next-smart-table[@class = 'customTable']"));
                    }
                    catch
                    {
                        cnt++;
                        log.Info("Inserisci Lettura Ex");
                        Thread.Sleep(Keanu.Randy(1));
                    }
                }

                IWebElement ButtonInserisciLettura = null;
                IList<IWebElement> ListaButtonOperativita = Keanu.Driver.FindElements(By.XPath(".//button[@class = 'btn btn-outline-primary-no-border cursor-pointer labelPink']"));
                try
                {
                    ButtonInserisciLettura = ListaButtonOperativita.Where(x => x.Text.ToUpper().Contains("INSERISCI LETTURE")).ToList<IWebElement>()[0];
                }
                catch
                { }
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", ButtonInserisciLettura);

                IWebElement frame = null;
                cnt = 0;
                while (frame == null && cnt < 3)
                {
                    try
                    {
                        frame = Keanu.Driver.FindElement(By.XPath(".//ngb-modal-window[@class = 'modal fade show d-block modal-xxl']"));
                    }
                    catch
                    {
                        cnt++;
                        Thread.Sleep(Keanu.Randy(1));
                    }
                }

                IWebElement CampoData = Keanu.Driver.FindElement(By.XPath(".//input[@name='dtCompetence']"));
                CampoData.Clear();
                CampoData.SendKeys(Flussi.DataMisura.ToShortDateString().Substring(0, 10));
                WaitLoadPageEE12();
                Actions a = new Actions(Keanu.Driver);
                a.SendKeys(Keys.Tab).Build().Perform();
                WaitLoadPageEE12();

                if (Keanu.Driver.PageSource.ToString().Contains("Matricola attiva non presente per la fornitura selezionata"))
                {
                    log.Info($"Matricola attiva non presente per la fornitura selezionata");
                    MkayClose("");
                    return false;
                }

                if (Keanu.Driver.PageSource.ToString().Contains("Alla data specificata esiste un processo commerciale/tecnico"))
                {
                    //CLICK CONFERMA, MAIL 11/02/2021 0743
                    IWebElement bMiniConferma = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'Conferma')]"));
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bMiniConferma);
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bMiniConferma);
                    WaitLoadPageEE12();
                }

                IWebElement ComboTipo = Keanu.Driver.FindElement(By.XPath(".//mat-select[@name='type']"));
                if (!ComboTipo.Text.Contains("REALE"))
                {
                    ComboTipo.Click();
                    Thread.Sleep(Keanu.Randy(1));
                    IList<IWebElement> OptionsTipo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                    IWebElement OptionTipoReale = null;
                    try
                    {
                        OptionTipoReale = OptionsTipo.Where(x => x.Text.ToUpper().Contains("REALE")).ToList()[0];
                        OptionTipoReale.Click();
                        Thread.Sleep(Keanu.Randy(1));
                    }
                    catch
                    {
                        log.Info("Tipo Lettura Ex");
                    }
                }

                IWebElement ComboCausale = Keanu.Driver.FindElement(By.XPath(".//mat-select[@name='causale']"));
                if (ComboCausale.Text.Contains("SELEZIONA"))
                {
                    ComboCausale.Click();
                    Thread.Sleep(Keanu.Randy(1));
                    IList<IWebElement> OptionsCausale = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                    IWebElement OptionCausale = null;
                    try
                    {
                        OptionCausale = OptionsCausale.Where(x => x.Text.ToUpper().Contains("RICALCOLO PER LETTURA PRECEDENTEMENTE ERRATA")).ToList()[0];
                        OptionCausale.Click();
                        Thread.Sleep(Keanu.Randy(1));
                    }
                    catch
                    {
                        log.Info("Causale di Ricalcolo Ex");
                    }
                }

                var tF1 = Keanu.Driver.FindElement(By.XPath(".//input[@name='attivaF1']"));
                tF1.Clear();
                if (Flussi.F1.ToString().Contains(","))
                    tF1.SendKeys(Flussi.F1.ToString().Split(',')[0]);
                else
                    tF1.SendKeys(Flussi.F1.ToString());

                var tF2 = Keanu.Driver.FindElement(By.XPath(".//input[@name='attivaF2']"));
                tF2.Clear();
                if (Flussi.F2.ToString().Contains(","))
                    tF2.SendKeys(Flussi.F2.ToString().Split(',')[0]);
                else
                    tF2.SendKeys(Flussi.F2.ToString());

                var tF3 = Keanu.Driver.FindElement(By.XPath(".//input[@name='attivaF3']"));
                tF3.Clear();
                if (Flussi.F3.ToString().Contains(","))
                    tF3.SendKeys(Flussi.F3.ToString().Split(',')[0]);
                else
                    tF3.SendKeys(Flussi.F3.ToString());

                try
                {
                    var tR1 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF1']"));
                    tR1.Clear();
                    if (Flussi.R1.ToString().Contains(","))
                        tR1.SendKeys(Flussi.R1.ToString().Split(',')[0]);
                    else
                        tR1.SendKeys(Flussi.R1.ToString());
                }
                catch { }

                try
                {
                    var tR2 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF2']"));
                    tR2.Clear();
                    if (Flussi.R2.ToString().Contains(","))
                        tR2.SendKeys(Flussi.R2.ToString().Split(',')[0]);
                    else
                        tR2.SendKeys(Flussi.R2.ToString());
                }
                catch { }

                try
                {
                    var tR3 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF3']"));
                    tR3.Clear();
                    if (Flussi.R3.ToString().Contains(","))
                        tR3.SendKeys(Flussi.R3.ToString().Split(',')[0]);
                    else
                        tR3.SendKeys(Flussi.R3.ToString());
                }
                catch { }

                if (!InsertPotenzaDTFromReqFlussi2G(Flussi.DataCaricamento))
                    return false;

                cnt = 0;
                while (true && cnt < 30)
                {
                    var bConferma = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'CONFERMA')]"));
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bConferma);
                    try
                    {
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bConferma);
                        WaitLoadPageEE12();
                        break;
                    }
                    catch
                    {
                        cnt++;
                        Thread.Sleep(Keanu.Randy(1));
                    }
                }

                WaitLoadPageEE12();

                try
                {
                    cnt = 0;
                    while (!Keanu.Driver.PageSource.ToString().Contains("Operazione effettuata con successo") && cnt < 10)
                    {
                        if (cnt == 9)
                        {
                            log.Info("Some error after conferma");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Stessa entità aggiornata da un altro operatore"))
                        {
                            log.Info("Stessa entità aggiornata da un altro operatore");
                            MkayClose("");
                            //Keanu.TimeToRestart = true;
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Stessa misura aggiornata da un altro operatore"))
                        {
                            log.Info("Stessa misura aggiornata da un altro operatore");
                            MkayClose("");
                            //Keanu.TimeToRestart = true;
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Errore tecnico"))
                        {
                            log.Info("Errore tecnico");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Misura non inserita perché già presente una misura con le stesse segnanti"))
                        {
                            log.Info($"Misura non inserita perché già presente una misura con le stesse segnanti");
                            MkayClose("");
                            log.Info($"Don't reload Lista Letture");
                            needToReloadListaLetture = false;
                            return true;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Numero cifre misura precedente non consentito"))
                        {
                            log.Info("Numero cifre misura precedente non consentito");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Numero di cifre superiore a quelle del misuratore"))
                        {
                            log.Info("Numero di cifre superiore a quelle del misuratore");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia attiva nullo e prelievo di potenza non nullo"))
                        {
                            log.Info($"Consumo di energia attiva nullo e prelievo di potenza non nullo");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia attiva superiore al consumo massimo"))
                        {
                            log.Info($"Consumo di energia attiva superiore al consumo massimo");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia reattiva superiore al consumo massimo"))
                        {
                            log.Info($"Consumo di energia reattiva superiore al consumo massimo");
                            MkayClose("OK");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Presenti letture alla stessa data o successive. Selezionare una causale di ricalcolo"))
                        {
                            log.Info("Presenti letture alla stessa data o successive. Selezionare una causale di ricalcolo");
                            IWebElement ComboCausaleDiRicalcolo = Keanu.Driver.FindElement(By.XPath(".//mat-select[@name='causaleRicalcolo']"));
                            ComboCausaleDiRicalcolo.Click();
                            IList<IWebElement> OptionsCausaleDiRicalcolo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                            IWebElement OptionCausaleDiRicalcolo = null;
                            try
                            {
                                OptionCausaleDiRicalcolo = OptionsCausaleDiRicalcolo.Where(x => x.Text.ToUpper().Contains("RICALCOLO PER LETTURA PRECEDENTEMENTE ERRATA")).ToList()[0];
                                OptionCausaleDiRicalcolo.Click();
                                WaitLoadPageEE12();
                            }
                            catch
                            {
                                MkayClose("CLOSE");
                                return false;
                            }
                            try
                            {
                                IWebElement modal = Keanu.Driver.FindElement(By.ClassName("modal-content"));
                                IWebElement modalDiv = modal.FindElement(By.XPath("//div[@class='modal-footer']"));
                                var buttons = modalDiv.FindElements(By.TagName("button"));
                                buttons[1].Click();
                                WaitLoadPageEE12();
                            }
                            catch
                            {
                                MkayClose("CLOSE");
                                return false;
                            }
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Formato non corretto"))
                        {
                            log.Info($"Formato non corretto");
                            MkayClose("CLOSE");
                            return false;
                        }
                        cnt++;
                        WaitLoadPageEE12();
                    }
                    needToReloadListaLetture = true;
                    MkayClose("OK");

                    upeCounter++;
                    log.Warn($"upeCounter {upeCounter}");

                    return true;
                }
                catch
                {
                    MkayClose("CLOSE");
                    return false;
                }
            }
            catch
            {
                MkayClose("CLOSE");
                return false;
            }
        }

        private bool ModificaLettura(Records.ArchivioScartiLavorabili Scarti, Records.ListaLetture Reale)
        {
            try
            {
                IWebElement frame = null;
                int cnt = 0;
                while (frame == null && cnt < 3)
                {
                    try
                    {
                        frame = Keanu.Driver.FindElement(By.XPath(".//ngb-modal-window[@class='modal fade show d-block modal-xxl']"));
                    }
                    catch
                    {
                        cnt++;
                        log.Info("Modifica Lettura Ex");
                        Thread.Sleep(Keanu.Randy(1));
                    }
                }

                IWebElement ComboTipo = Keanu.Driver.FindElement(By.XPath(".//mat-select[@name='measureType']"));
                if (!ComboTipo.Text.Contains("REALE"))
                {
                    ComboTipo.Click();
                    Thread.Sleep(Keanu.Randy(1));
                    IList<IWebElement> OptionsTipo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                    IWebElement OptionTipoReale = null;
                    try
                    {
                        OptionTipoReale = OptionsTipo.Where(x => x.Text.ToUpper().Contains("REALE")).ToList()[0];
                        OptionTipoReale.Click();
                        Thread.Sleep(Keanu.Randy(1));
                    }
                    catch
                    {
                        log.Info("Tipo Lettura Ex");
                    }
                }

                IWebElement ComboCausale = Keanu.Driver.FindElement(By.XPath(".//mat-select[@name='causaleRicalcolo']"));
                if (ComboCausale.Text.Contains("SELEZIONA"))
                {
                    ComboCausale.Click();
                    Thread.Sleep(Keanu.Randy(1));
                    IList<IWebElement> OptionsCausale = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                    IWebElement OptionCausale = null;
                    try
                    {
                        OptionCausale = OptionsCausale.Where(x => x.Text.ToUpper().Contains("RICALCOLO PER LETTURA PRECEDENTEMENTE ERRATA")).ToList()[0];
                        OptionCausale.Click();
                        Thread.Sleep(Keanu.Randy(1));
                    }
                    catch
                    {
                        log.Info("Causale di Ricalcolo Ex");
                    }
                }

                if (!InsertAttivaReattivaPot(Scarti, Reale, true))
                    return false;

                cnt = 0;
                while (true && cnt < 30)
                {
                    var bModifica = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'Modifica')]"));
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bModifica);
                    try
                    {
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bModifica);
                        WaitLoadPageEE12();
                        break;
                    }
                    catch
                    {
                        cnt++;
                        Thread.Sleep(Keanu.Randy(1));
                    }
                }

                WaitLoadPageEE12();

                try
                {
                    cnt = 0;
                    while (!Keanu.Driver.PageSource.ToString().Contains("Operazione effettuata con successo") && cnt < 10)
                    {
                        if (cnt == 9)
                        {
                            log.Info("Some error after modifica");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Modifica non ammessa - undefined"))
                        {
                            log.Info("Modifica non ammessa - undefined");
                            MkayClose("");
                            Keanu.TimeToRestart = true;
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Stessa entità aggiornata da un altro operatore"))
                        {
                            log.Info("Stessa entità aggiornata da un altro operatore");
                            MkayClose("");
                            //Keanu.TimeToRestart = true;
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Stessa misura aggiornata da un altro operatore"))
                        {
                            log.Info("Stessa misura aggiornata da un altro operatore");
                            MkayClose("");
                            //Keanu.TimeToRestart = true;
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Errore tecnico"))
                        {
                            log.Info("Errore tecnico");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Misura non inserita perché già presente una misura con le stesse segnanti"))
                        {
                            log.Info($"Misura non inserita perché già presente una misura con le stesse segnanti");
                            MkayClose("");
                            log.Info($"Add to sposta list");
                            log.Info($"Don't reload Lista Letture");
                            needToReloadListaLetture = false;
                            return true;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Numero cifre misura precedente non consentito"))
                        {
                            log.Info("Numero cifre misura precedente non consentito");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Numero di cifre superiore a quelle del misuratore"))
                        {
                            log.Info("Numero di cifre superiore a quelle del misuratore");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia attiva nullo e prelievo di potenza non nullo"))
                        {
                            log.Info($"Consumo di energia attiva nullo e prelievo di potenza non nullo");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia attiva superiore al consumo massimo"))
                        {
                            log.Info($"Consumo di energia attiva superiore al consumo massimo");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia reattiva superiore al consumo massimo"))
                        {
                            log.Info($"Consumo di energia reattiva superiore al consumo massimo");
                            MkayClose("OK");

                            if (Reale.Potenza <= potete)
                            {
                                log.Info($"Reale potenza <= 15");

                                if (!InsertAttivaReattivaPot(Scarti, Reale, false))
                                    return false;

                                cnt = 0;
                                while (true && cnt < 30)
                                {
                                    var bModifica = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'Modifica')]"));
                                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bModifica);
                                    try
                                    {
                                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bModifica);
                                        WaitLoadPageEE12();
                                        break;
                                    }
                                    catch
                                    {
                                        cnt++;
                                        Thread.Sleep(Keanu.Randy(1));
                                    }
                                }
                                if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia reattiva superiore al consumo massimo"))
                                {
                                    log.Info($"2nd try");
                                    log.Info($"Consumo di energia reattiva superiore al consumo massimo");
                                    MkayClose("");
                                    return false;
                                }
                            }
                            else
                            {
                                log.Info($"Reale potenza > 15");
                                MkayClose("CLOSE");
                                return false;
                            }
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Formato non corretto"))
                        {
                            log.Info($"Formato non corretto");
                            MkayClose("CLOSE");
                            return false;
                        }
                        cnt++;
                        WaitLoadPageEE12();
                    }
                    needToReloadListaLetture = true;
                    MkayClose("OK");

                    upeCounter++;
                    log.Warn($"upeCounter {upeCounter}");

                    return true;
                }
                catch
                {
                    MkayClose("CLOSE");
                    return false;
                }
            }
            catch
            {
                MkayClose("CLOSE");
                return false;
            }
        }

        private bool ModificaLetturaWithVariablesFromFlussi2GSmisEdition(Records.Flussi2G Flussi, Records.ListaLetture All)
        {
            if (Keanu.Driver.PageSource.ToString().Contains("Operazione non ammessa su periodo ricostruito") ||
                Keanu.Driver.PageSource.ToString().Contains("Operazione non ammessa  su periodo ricostruito") ||
                Keanu.Driver.PageSource.ToString().Contains("Operazione non ammessa"))
            {
                log.Info("Operazione non ammessa su periodo ricostruito");
                MkayClose("OK");
                return false;
            }
            try
            {
                IWebElement frame = null;
                int cnt = 0;
                while (frame == null && cnt < 3)
                {
                    try
                    {
                        frame = Keanu.Driver.FindElement(By.XPath(".//ngb-modal-window[@class='modal fade show d-block modal-xxl']"));
                    }
                    catch
                    {
                        cnt++;
                        log.Info("Modifica Lettura Ex");
                        Thread.Sleep(Keanu.Randy(1));
                    }
                }

                IWebElement ComboTipo = Keanu.Driver.FindElement(By.XPath(".//mat-select[@name='measureType']"));
                if (!ComboTipo.Text.Contains("REALE"))
                {
                    ComboTipo.Click();
                    Thread.Sleep(Keanu.Randy(1));
                    IList<IWebElement> OptionsTipo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                    IWebElement OptionTipoReale = null;
                    try
                    {
                        OptionTipoReale = OptionsTipo.Where(x => x.Text.ToUpper().Contains("REALE")).ToList()[0];
                        OptionTipoReale.Click();
                        Thread.Sleep(Keanu.Randy(1));
                    }
                    catch
                    {
                        log.Info("Tipo Lettura Ex");
                    }
                }

                IWebElement ComboCausale = Keanu.Driver.FindElement(By.XPath(".//mat-select[@name='causaleRicalcolo']"));
                if (ComboCausale.Text.Contains("SELEZIONA"))
                {
                    ComboCausale.Click();
                    Thread.Sleep(Keanu.Randy(1));
                    IList<IWebElement> OptionsCausale = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                    IWebElement OptionCausale = null;
                    try
                    {
                        OptionCausale = OptionsCausale.Where(x => x.Text.ToUpper().Contains("RICALCOLO PER LETTURA PRECEDENTEMENTE ERRATA")).ToList()[0];
                        OptionCausale.Click();
                        Thread.Sleep(Keanu.Randy(1));
                    }
                    catch
                    {
                        log.Info("Causale di Ricalcolo Ex");
                    }
                }

                var tF1 = Keanu.Driver.FindElement(By.XPath(".//input[@name='attivaF1']"));
                tF1.Clear();
                if (Flussi.F1.Equals(-1))
                    tF1.SendKeys("0");
                else if (Flussi.F1.ToString().Contains(","))
                    tF1.SendKeys(Flussi.F1.ToString().Split(',')[0]);
                else
                    tF1.SendKeys(Flussi.F1.ToString());

                var tF2 = Keanu.Driver.FindElement(By.XPath(".//input[@name='attivaF2']"));
                tF2.Clear();
                if (Flussi.F2.Equals(-1))
                    tF2.SendKeys("0");
                else if (Flussi.F2.ToString().Contains(","))
                    tF2.SendKeys(Flussi.F2.ToString().Split(',')[0]);
                else
                    tF2.SendKeys(Flussi.F2.ToString());

                var tF3 = Keanu.Driver.FindElement(By.XPath(".//input[@name='attivaF3']"));
                tF3.Clear();
                if (Flussi.F3.Equals(-1))
                    tF3.SendKeys("0");
                else if (Flussi.F3.ToString().Contains(","))
                    tF3.SendKeys(Flussi.F3.ToString().Split(',')[0]);
                else
                    tF3.SendKeys(Flussi.F3.ToString());

                try
                {
                    var tR1 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF1']"));
                    if (Flussi.R1.Equals(-1) || Flussi.R1.Equals("-1"))
                    {
                        //SKIP
                    }
                    else
                    {
                        tR1.Clear();
                        if (Flussi.R1.ToString().Contains(","))
                            tR1.SendKeys(Flussi.R1.ToString().Split(',')[0]);
                        else
                            tR1.SendKeys(Flussi.R1.ToString());
                    }

                }
                catch { }

                try
                {
                    var tR2 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF2']"));
                    if (Flussi.R2.Equals(-1) || Flussi.R2.Equals("-1"))
                    {
                        //SKIP
                    }
                    else
                    {
                        tR2.Clear();
                        if (Flussi.R2.ToString().Contains(","))
                            tR2.SendKeys(Flussi.R2.ToString().Split(',')[0]);
                        else
                            tR2.SendKeys(Flussi.R2.ToString());
                    }
                }
                catch { }

                try
                {
                    var tR3 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF3']"));
                    if (Flussi.R3.Equals(-1) || Flussi.R3.Equals("-1"))
                    {
                        //SKIP
                    }
                    else
                    {
                        tR3.Clear();
                        if (Flussi.R3.ToString().Contains(","))
                            tR3.SendKeys(Flussi.R3.ToString().Split(',')[0]);
                        else
                            tR3.SendKeys(Flussi.R3.ToString());
                    }
                }
                catch { }

                if (!InsertPotenzaDTFromReqFlussi2G(Flussi.DataCaricamento))
                    return false;

                cnt = 0;
                while (true && cnt < 30)
                {
                    var bModifica = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'Modifica')]"));
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bModifica);
                    try
                    {
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bModifica);
                        WaitLoadPageEE12();
                        break;
                    }
                    catch
                    {
                        cnt++;
                        Thread.Sleep(Keanu.Randy(1));
                    }
                }

                WaitLoadPageEE12();

                try
                {
                    cnt = 0;
                    while (!Keanu.Driver.PageSource.ToString().Contains("Operazione effettuata con successo") && cnt < 10)
                    {
                        if (cnt == 9)
                        {
                            log.Info("Some error after modifica");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Modifica non ammessa - undefined"))
                        {
                            log.Info("Modifica non ammessa - undefined");
                            MkayClose("");
                            Keanu.TimeToRestart = true;
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Stessa entità aggiornata da un altro operatore"))
                        {
                            log.Info("Stessa entità aggiornata da un altro operatore");
                            MkayClose("");
                            //Keanu.TimeToRestart = true;
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Stessa misura aggiornata da un altro operatore"))
                        {
                            log.Info("Stessa misura aggiornata da un altro operatore");
                            MkayClose("");
                            //Keanu.TimeToRestart = true;
                            return false;
                        }

                        if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia attiva negativo")) {
                            //Impossible to make doc, send list of those docs to adriano for deleting (DPC error number)
                            log.Info("Consumo di energia attiva negativo");
                            MkayClose("");
                            Keanu.TimeToSospeso = true;
                            Keanu.TimeToSospesoType = 7;
                            //Keanu.TimeToRestart = true;
                            return false;
                        }

                        if (Keanu.Driver.PageSource.ToString().Contains("Errore tecnico"))
                        {
                            log.Info("Errore tecnico");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Misura non inserita perché già presente una misura con le stesse segnanti"))
                        {
                            log.Info($"Misura non inserita perché già presente una misura con le stesse segnanti");
                            MkayClose("");
                            log.Info($"Don't reload Lista Letture");
                            needToReloadListaLetture = false;
                            return true;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Numero cifre misura precedente non consentito"))
                        {
                            log.Info("Numero cifre misura precedente non consentito");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Numero di cifre superiore a quelle del misuratore"))
                        {
                            log.Info("Numero di cifre superiore a quelle del misuratore");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia attiva nullo e prelievo di potenza non nullo"))
                        {
                            log.Info($"Consumo di energia attiva nullo e prelievo di potenza non nullo");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia attiva superiore al consumo massimo"))
                        {
                            log.Info($"Consumo di energia attiva superiore al consumo massimo");
                            MkayClose("");
                            needToAnnulla = true;
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia reattiva superiore al consumo massimo"))
                        {
                            log.Info($"Consumo di energia reattiva superiore al consumo massimo");
                            MkayClose("OK");

                            try
                            {
                                var tR1 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF1']"));
                                tR1.Clear();
                                if (All.R1.Equals("-1"))
                                    tR1.SendKeys("0");
                                if (All.R1.ToString().Contains(","))
                                    tR1.SendKeys(All.R1.ToString().Split(',')[0]);
                                else
                                    tR1.SendKeys(All.R1.ToString());
                            }
                            catch { }

                            try
                            {
                                var tR2 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF2']"));
                                tR2.Clear();
                                if (All.R2.Equals("-1"))
                                    tR2.SendKeys("0");
                                if (All.R2.ToString().Contains(","))
                                    tR2.SendKeys(All.R2.ToString().Split(',')[0]);
                                else
                                    tR2.SendKeys(All.R2.ToString());
                            }
                            catch { }

                            try
                            {
                                var tR3 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF3']"));
                                tR3.Clear();
                                if (All.R3.Equals("-1"))
                                    tR3.SendKeys("0");
                                if (All.R3.ToString().Contains(","))
                                    tR3.SendKeys(All.R3.ToString().Split(',')[0]);
                                else
                                    tR3.SendKeys(All.R3.ToString());
                            }
                            catch { }

                            cnt = 0;
                            while (true && cnt < 30)
                            {
                                var bModifica = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'Modifica')]"));
                                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bModifica);
                                try
                                {
                                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bModifica);
                                    WaitLoadPageEE12();
                                    break;
                                }
                                catch
                                {
                                    cnt++;
                                    Thread.Sleep(Keanu.Randy(1));
                                }
                            }
                            if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia reattiva superiore al consumo massimo"))
                            {
                                log.Info($"2nd try");
                                log.Info($"Consumo di energia reattiva superiore al consumo massimo");
                                MkayClose("");
                                return false;
                            }
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Formato non corretto"))
                        {
                            log.Info($"Formato non corretto");
                            MkayClose("CLOSE");
                            return false;
                        }
                        cnt++;
                        WaitLoadPageEE12();
                    }
                    needToReloadListaLetture = true;
                    MkayClose("OK");

                    upeCounter++;
                    log.Warn($"upeCounter {upeCounter}");

                    return true;
                }
                catch
                {
                    MkayClose("CLOSE");
                    return false;
                }
            }
            catch
            {
                MkayClose("CLOSE");
                return false;
            }
        }

        private bool ModificaLetturaWithVariablesFromFlussi2G(Records.Flussi2G Flussi)
        {
            try
            {
                IWebElement frame = null;
                int cnt = 0;
                while (frame == null && cnt < 3)
                {
                    try
                    {
                        frame = Keanu.Driver.FindElement(By.XPath(".//ngb-modal-window[@class='modal fade show d-block modal-xxl']"));
                    }
                    catch
                    {
                        cnt++;
                        log.Info("Modifica Lettura Ex");
                        Thread.Sleep(Keanu.Randy(1));
                    }
                }

                IWebElement ComboTipo = Keanu.Driver.FindElement(By.XPath(".//mat-select[@name='measureType']"));
                if (!ComboTipo.Text.Contains("REALE"))
                {
                    ComboTipo.Click();
                    Thread.Sleep(Keanu.Randy(1));
                    IList<IWebElement> OptionsTipo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                    IWebElement OptionTipoReale = null;
                    try
                    {
                        OptionTipoReale = OptionsTipo.Where(x => x.Text.ToUpper().Contains("REALE")).ToList()[0];
                        OptionTipoReale.Click();
                        Thread.Sleep(Keanu.Randy(1));
                    }
                    catch
                    {
                        log.Info("Tipo Lettura Ex");
                    }
                }

                IWebElement ComboCausale = Keanu.Driver.FindElement(By.XPath(".//mat-select[@name='causaleRicalcolo']"));
                if (ComboCausale.Text.Contains("SELEZIONA"))
                {
                    ComboCausale.Click();
                    Thread.Sleep(Keanu.Randy(1));
                    IList<IWebElement> OptionsCausale = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                    IWebElement OptionCausale = null;
                    try
                    {
                        OptionCausale = OptionsCausale.Where(x => x.Text.ToUpper().Contains("RICALCOLO PER LETTURA PRECEDENTEMENTE ERRATA")).ToList()[0];
                        OptionCausale.Click();
                        Thread.Sleep(Keanu.Randy(1));
                    }
                    catch
                    {
                        log.Info("Causale di Ricalcolo Ex");
                    }
                }

                var tF1 = Keanu.Driver.FindElement(By.XPath(".//input[@name='attivaF1']"));
                tF1.Clear();
                if (Flussi.F1.ToString().Contains(","))
                    tF1.SendKeys(Flussi.F1.ToString().Split(',')[0]);
                else
                    tF1.SendKeys(Flussi.F1.ToString());

                var tF2 = Keanu.Driver.FindElement(By.XPath(".//input[@name='attivaF2']"));
                tF2.Clear();
                if (Flussi.F2.ToString().Contains(","))
                    tF2.SendKeys(Flussi.F2.ToString().Split(',')[0]);
                else
                    tF2.SendKeys(Flussi.F2.ToString());

                var tF3 = Keanu.Driver.FindElement(By.XPath(".//input[@name='attivaF3']"));
                tF3.Clear();
                if (Flussi.F3.ToString().Contains(","))
                    tF3.SendKeys(Flussi.F3.ToString().Split(',')[0]);
                else
                    tF3.SendKeys(Flussi.F3.ToString());

                try
                {
                    var tR1 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF1']"));
                    tR1.Clear();
                    if (Flussi.R1.ToString().Contains(","))
                        tR1.SendKeys(Flussi.R1.ToString().Split(',')[0]);
                    else
                        tR1.SendKeys(Flussi.R1.ToString());
                }
                catch { }

                try
                {
                    var tR2 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF2']"));
                    tR2.Clear();
                    if (Flussi.R2.ToString().Contains(","))
                        tR2.SendKeys(Flussi.R2.ToString().Split(',')[0]);
                    else
                        tR2.SendKeys(Flussi.R2.ToString());
                }
                catch { }

                try
                {
                    var tR3 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF3']"));
                    tR3.Clear();
                    if (Flussi.R3.ToString().Contains(","))
                        tR3.SendKeys(Flussi.R3.ToString().Split(',')[0]);
                    else
                        tR3.SendKeys(Flussi.R3.ToString());
                }
                catch { }

                if (!InsertPotenzaDTFromReqFlussi2G(Flussi.DataCaricamento))
                    return false;

                cnt = 0;
                while (true && cnt < 30)
                {
                    var bModifica = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'Modifica')]"));
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bModifica);
                    try
                    {
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bModifica);
                        WaitLoadPageEE12();
                        break;
                    }
                    catch
                    {
                        cnt++;
                        Thread.Sleep(Keanu.Randy(1));
                    }
                }

                WaitLoadPageEE12();

                try
                {
                    cnt = 0;
                    while (!Keanu.Driver.PageSource.ToString().Contains("Operazione effettuata con successo") && cnt < 10)
                    {
                        if (cnt == 9)
                        {
                            log.Info("Some error after modifica");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Modifica non ammessa - undefined"))
                        {
                            log.Info("Modifica non ammessa - undefined");
                            MkayClose("");
                            Keanu.TimeToRestart = true;
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Stessa entità aggiornata da un altro operatore"))
                        {
                            log.Info("Stessa entità aggiornata da un altro operatore");
                            MkayClose("");
                            //Keanu.TimeToRestart = true;
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Stessa misura aggiornata da un altro operatore"))
                        {
                            log.Info("Stessa misura aggiornata da un altro operatore");
                            MkayClose("");
                            //Keanu.TimeToRestart = true;
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Errore tecnico"))
                        {
                            log.Info("Errore tecnico");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Misura non inserita perché già presente una misura con le stesse segnanti"))
                        {
                            log.Info($"Misura non inserita perché già presente una misura con le stesse segnanti");
                            MkayClose("");
                            log.Info($"Don't reload Lista Letture");
                            needToReloadListaLetture = false;
                            return true;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Numero cifre misura precedente non consentito"))
                        {
                            log.Info("Numero cifre misura precedente non consentito");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Numero di cifre superiore a quelle del misuratore"))
                        {
                            log.Info("Numero di cifre superiore a quelle del misuratore");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia attiva nullo e prelievo di potenza non nullo"))
                        {
                            log.Info($"Consumo di energia attiva nullo e prelievo di potenza non nullo");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia attiva superiore al consumo massimo"))
                        {
                            log.Info($"Consumo di energia attiva superiore al consumo massimo");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Consumo di energia reattiva superiore al consumo massimo"))
                        {
                            log.Info($"Consumo di energia reattiva superiore al consumo massimo");
                            MkayClose("");
                            return false;
                        }
                        if (Keanu.Driver.PageSource.ToString().Contains("Formato non corretto"))
                        {
                            log.Info($"Formato non corretto");
                            MkayClose("CLOSE");
                            return false;
                        }
                        cnt++;
                        WaitLoadPageEE12();
                    }
                    needToReloadListaLetture = true;
                    MkayClose("OK");

                    upeCounter++;
                    log.Warn($"upeCounter {upeCounter}");

                    return true;
                }
                catch
                {
                    MkayClose("CLOSE");
                    return false;
                }
            }
            catch
            {
                MkayClose("CLOSE");
                return false;
            }
        }

        private bool InsertAttivaReattivaPot(Records.ArchivioScartiLavorabili scarti, Records.ListaLetture reale, bool both)
        {
            //TODO MONORARIO documents, for now only TRIORARIO 
            if (both)
            {
                var tF1 = Keanu.Driver.FindElement(By.XPath(".//input[@name='attivaF1']"));
                tF1.Clear();
                if (scarti.F1.ToString().Contains(","))
                    tF1.SendKeys(scarti.F1.ToString().Split(',')[0]);
                else
                    tF1.SendKeys(scarti.F1.ToString());

                var tF2 = Keanu.Driver.FindElement(By.XPath(".//input[@name='attivaF2']"));
                tF2.Clear();
                if (scarti.F2.ToString().Contains(","))
                    tF2.SendKeys(scarti.F2.ToString().Split(',')[0]);
                else
                    tF2.SendKeys(scarti.F2.ToString());

                var tF3 = Keanu.Driver.FindElement(By.XPath(".//input[@name='attivaF3']"));
                tF3.Clear();
                if (scarti.F3.ToString().Contains(","))
                    tF3.SendKeys(scarti.F3.ToString().Split(',')[0]);
                else
                    tF3.SendKeys(scarti.F3.ToString());

                try
                {
                    var tR1 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF1']"));
                    tR1.Clear();
                    if (scarti.R1.ToString().Contains(","))
                        tR1.SendKeys(scarti.R1.ToString().Split(',')[0]);
                    else
                        tR1.SendKeys(scarti.R1.ToString());
                }
                catch { }

                try
                {
                    var tR2 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF2']"));
                    tR2.Clear();
                    if (scarti.R2.ToString().Contains(","))
                        tR2.SendKeys(scarti.R2.ToString().Split(',')[0]);
                    else
                        tR2.SendKeys(scarti.R2.ToString());
                }
                catch { }

                try
                {
                    var tR3 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF3']"));
                    tR3.Clear();
                    if (scarti.R3.ToString().Contains(","))
                        tR3.SendKeys(scarti.R3.ToString().Split(',')[0]);
                    else
                        tR3.SendKeys(scarti.R3.ToString());
                }
                catch { }

                if (!InsertPotenzaDTFromReqFlussi2G(scarti.DataLettura))
                    return false;
            }
            else
            {
                log.Info($"Insert R from reale");

                try
                {
                    var tR1 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF1']"));
                    tR1.Clear();
                    if (reale.R1.ToString().Contains(","))
                        tR1.SendKeys(reale.R1.ToString().Split(',')[0]);
                    else
                        tR1.SendKeys(reale.R1.ToString());
                }
                catch { }

                try
                {
                    var tR2 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF2']"));
                    tR2.Clear();
                    if (reale.R2.ToString().Contains(","))
                        tR2.SendKeys(reale.R2.ToString().Split(',')[0]);
                    else
                        tR2.SendKeys(reale.R2.ToString());
                }
                catch { }

                try
                {
                    var tR3 = Keanu.Driver.FindElement(By.XPath(".//input[@name='reattivaF3']"));
                    tR3.Clear();
                    if (reale.R3.ToString().Contains(","))
                        tR3.SendKeys(reale.R3.ToString().Split(',')[0]);
                    else
                        tR3.SendKeys(reale.R3.ToString());
                }
                catch { }

                if (!InsertPPReale(reale))
                    return false;
            }
            return true;
        }

        private bool InsertPPReale(Records.ListaLetture reale)
        {
            bool req = false;
            try
            {
                var tempStar = Keanu.Driver.FindElement(By.XPath(".//input[@name='potenzaDtF1']"));
                var parent = tempStar.FindElement(By.XPath("./.."));
                var allChilds = parent.FindElements(By.CssSelector("label"));
                foreach (var item in allChilds)
                {
                    if (item.Text.Contains("*"))
                    {
                        req = true;
                        break;
                    }
                }
            }

            catch (Exception)
            {
                throw;
            }

            if (req)
            {
                breakFromSubscription = true;
                log.Debug($"potenzaDTIsNull");
                MkayClose("CLOSE");
                Keanu.TimeToSospesoType = 9;
                Keanu.TimeToSospeso = true;
                return false;

                //try
                //{
                //    var tP1 = Keanu.Driver.FindElement(By.XPath(".//input[@name='potenzaDtF1']"));
                //    tP1.Clear();
                //    if (reale.P1.ToString().Contains(","))
                //        tP1.SendKeys(reale.P1.ToString().Split(',')[0]);
                //    else
                //        tP1.SendKeys(reale.P1.ToString());
                //}
                //catch { }

                //try
                //{
                //    var tP2 = Keanu.Driver.FindElement(By.XPath(".//input[@name='potenzaDtF2']"));
                //    tP2.Clear();
                //    if (reale.P2.ToString().Contains(","))
                //        tP2.SendKeys(reale.P2.ToString().Split(',')[0]);
                //    else
                //        tP2.SendKeys(reale.P2.ToString());
                //}
                //catch { }

                //try
                //{
                //    var tP3 = Keanu.Driver.FindElement(By.XPath(".//input[@name='potenzaDtF3']"));
                //    tP3.Clear();
                //    if (reale.P3.ToString().Contains(","))
                //        tP3.SendKeys(reale.P3.ToString().Split(',')[0]);
                //    else
                //        tP3.SendKeys(reale.P3.ToString());
                //}
                //catch { }
            }
            return true;
        }

        private bool InsertPotenzaDTFromReqFlussi2G(DateTime date)
        {
            bool req = false;
            try
            {
                var tempStar = Keanu.Driver.FindElement(By.XPath(".//input[@name='potenzaDtF1']"));
                var parent = tempStar.FindElement(By.XPath("./.."));
                var allChilds = parent.FindElements(By.CssSelector("label"));
                foreach (var item in allChilds)
                {
                    if (item.Text.Contains("*"))
                    {
                        req = true;
                        break;
                    }
                }
            }

            catch (Exception)
            {
                throw;
            }

            if (req)
            {
                breakFromSubscription = true;
                log.Debug($"potenzaDTIsNull");
                MkayClose("CLOSE");
                Keanu.TimeToSospesoType = 9;
                Keanu.TimeToSospeso = true;
                return false;
            }
            return true;
        }

        private bool GoToListaLettura()
        {
            Keanu.Driver.Navigate().GoToUrl("https://next.enelint.global/next-online/#/page/next-home");
            WaitLoadPageEE12();
            int cnt = 0;
            while (!Keanu.Driver.PageSource.ToString().Contains("Mostra tutto") && cnt < 15)
            {
                if (cnt == 3)
                {
                    log.Info($"No attivo");
                    ///DELTE IF DONT WORK ---REMEMBER----
                    if (Keanu.StartStop == true)
                        break;
                    return false;
                }

                Keanu.Driver.Navigate().GoToUrl("https://next.enelint.global/next-online/#/page/next-home");
                WaitLoadPageEE12();

                var tPod = Keanu.Driver.FindElement(By.XPath("//input[@placeholder='INSERIRE ALMENO 3 CARATTERI']"));
                tPod.Clear();
                tPod.SendKeys(v.Pod);
                WaitLoadPageEE12();

                var bSearch = Keanu.Driver.FindElement(By.XPath("//button[@class='btn btn-sm btn-primary']"));
                bSearch.Click();
                WaitLoadPageEE12();

                WaitLoadPageEE12();//EXTRA WAIT BALLSACK

                if (Keanu.Driver.PageSource.ToString().Contains("Nessun risultato trovato"))
                {
                    log.Error($"Nessun risultato trovato");
                    return false;
                }

                if (Keanu.Driver.PageSource.ToString().Contains("Mostra tutto"))//ONLY ONE ATTIVA AND GOES AUTO TO LETTURE LIST
                {
                    WaitLoadPageEE12();
                }
                else
                {
                    if (!Keanu.Driver.PageSource.ToString().Contains("ATTIVO"))
                    {
                        cnt++;
                        log.Info($"No attivo, try again {cnt}/3");
                        WaitLoadPageEE12();
                        continue;
                        //return false;
                    }
                    try
                    {
                        ListaElencoRisultati = new List<Records.ElencoRisultati>();
                        var tableER = Keanu.Driver.FindElement(By.Id("table_result"));
                        IList<IWebElement> tableRowsER = null;
                        IList<IWebElement> tableColumnsER = null;
                        tableRowsER = tableER.FindElements(By.TagName("tr"));
                        foreach (var row in tableRowsER)
                        {
                            string s = row.Text.ToString();
                            if (s.Contains("ATTIVO"))
                            {
                                tableColumnsER = row.FindElements(By.TagName("td"));
                                try
                                {
                                    tableColumnsER[8].Click();//TRY EXACT CLICC ON ATTIVA
                                    WaitLoadPageEE12();
                                    break;
                                }
                                catch
                                {
                                    row.Click();//CLICC ON ROW
                                    WaitLoadPageEE12();
                                    break;
                                }
                            }
                        }
                    }
                    catch
                    {
                        log.Error($"GoToListaLettura() fail");
                    }
                }
                cnt++;
            }

            GetDataAttivazioneAndNumeroUtente();

            return true;
        }

        private void GetDataAttivazioneAndNumeroUtente()
        {
            try
            {
                #region Example
                //01/09/2020
                //GOT
                //https://next.enelint.global/api-next-online-2g/api/v1/protected/IT001E01316051/measures/mm2g?startDate=2020-09-01&endDate=9999-12-31&aggregationMode=MONTHLY&_hidePageLoading=false

                //NEED
                //https://next.enelint.global/api-next-online-2g/api/v1/protected/IT001E01316051/measures/mm2g?startDate=2020-08-31&endDate=9999-12-31&aggregationMode=MONTHLY&_hidePageLoading=false
                //NEED ONE MONTH BACK
                #endregion

                var dAtt = Keanu.Driver.FindElement(By.Name("dataAttivazione"));
                string dataValue = dAtt.GetAttribute("value");

                DateTime tempDate = new DateTime();
                tempDate = Convert.ToDateTime(dataValue);
                string defTempDate = tempDate.ToString();
                string[] split = defTempDate.ToString().Split(' ');
                string[] split2 = split[0].ToString().Split('/');
                dataAttivazioneStandard = split2[2] + "-" + split2[1] + "-" + split2[0];
                log.Info($"dataAttivazioneStandard {dataAttivazioneStandard}");

                DateTime tempDateMinusOneDay = new DateTime();
                tempDateMinusOneDay = tempDate.AddDays(-1);
                string minusOne = tempDateMinusOneDay.ToString();
                split = minusOne.ToString().Split(' ');
                split2 = split[0].ToString().Split('/');
                dataAttivazioneMinusOne = split2[2] + "-" + split2[1] + "-" + split2[0];
                log.Info($"dataAttivazioneMinusOne {dataAttivazioneMinusOne}");
            }
            catch (Exception)
            {
                dataAttivazioneMinusOne = "2010-12-31";
                log.Warn($"dataAttivazioneMinusOne {dataAttivazioneMinusOne}");
            }

            try
            {
                var utente = Keanu.Driver.FindElements(By.Name("customerNumber")).Last();
                numeroUtente = utente.GetAttribute("value");
                log.Info($"Numero Utente {numeroUtente}");
            }
            catch (Exception)
            {
                numeroUtente = "";
                log.Warn($"Numero Utente {numeroUtente}");
            }
        }

        private void MkayClose(string what)
        {
            try
            {
                IWebElement bOk = null;
                IWebElement bClose = null;
                switch (what)
                {
                    case "OK":
                        bOk = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'OK')]"));
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bOk);
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bOk);
                        WaitLoadPageEE12();
                        break;
                    case "CLOSE":
                        bClose = Keanu.Driver.FindElement(By.XPath("//button[@class='close']"));
                        bClose.Click();
                        WaitLoadPageEE12();
                        break;
                    default:
                        try
                        {
                            bOk = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'OK')]"));
                            ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].scrollIntoView(false);", bOk);
                            ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", bOk);
                            WaitLoadPageEE12();
                        }
                        catch { }
                        try
                        {
                            bClose = Keanu.Driver.FindElement(By.XPath("//button[@class='close']"));
                            bClose.Click();
                            WaitLoadPageEE12();
                        }
                        catch { }
                        break;
                }
            }
            catch
            { }
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

        private bool NavigateToMatPower(string dataMisura)
        {
            try
            {
                Keanu.Driver.Navigate().GoToUrl("https://next.enelint.global/next-online/#/page/power/next-mat/mat");
                WaitLoadPageEE12();

                if (!Keanu.Driver.PageSource.ToString().Contains("DATI GENERALI"))
                {
                    log.Info($"Wrong page");
                    return false;
                }

                Actions a = new Actions(Keanu.Driver);

                var tNumeroUtente = Keanu.Driver.FindElements(By.XPath("//input[@formcontrolname='customerNumber']")).Last();
                tNumeroUtente.Clear();
                tNumeroUtente.SendKeys(numeroUtente);
                a.SendKeys(Keys.Tab).Build().Perform();
                WaitLoadPageEE12();

                var tPod = Keanu.Driver.FindElements(By.XPath("//input[@formcontrolname='pod']")).Last();
                string podChek = tPod.GetAttribute("value");
                if (string.IsNullOrEmpty(podChek))
                {
                    tPod.Clear();
                    tPod.SendKeys(v.Pod);
                    a.SendKeys(Keys.Tab).Build().Perform();
                    WaitLoadPageEE12();
                }

                IWebElement DataProcesso = Keanu.Driver.FindElement(By.XPath(".//input[@name='dataProcesso']"));
                DataProcesso.Clear();
                DataProcesso.SendKeys(dataMisura);
                //DataProcesso.SendKeys(SmisList.FirstOrDefault().DataMisura.ToShortDateString());
                WaitLoadPageEE12();
                a.SendKeys(Keys.Tab).Build().Perform();
                WaitLoadPageEE12();

                var bAvanti = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'Avanti')]"));
                bAvanti.Click();
                WaitLoadPageEE12();

                return true;
            }
            catch
            {
                log.Error($"NavigateToMatPower() fail");
                return false;
            }
        }

        private bool Cambio1G1G()
        {
            try
            {
                List<Records.Flussi> SmisListFlussi = new List<Records.Flussi>();
                foreach (var item in requestFlussi)
                {
                    if (item.CodiceFlusso.Contains("SMIS"))
                    {
                        SmisListFlussi.Add(item);
                        log.Debug($"{item.DataMisura.ToShortDateString()}");
                        //log.Debug($"{item.DataMisura.ToShortDateString()} {item.Stato} {item.StatoAbb} {item.Motivazione} {item.Esito} {item.CodiceEsito}");
                    }
                }

                log.Info($"{SmisListFlussi.Count()} FLUSSI SMIS Before");

                if (SmisListFlussi.Count().Equals(0))
                {
                    log.Info($"0 SMIS in FLUSSI");
                    //File.AppendAllText(speshal, $"{v.Riferimento} {v.Pod} {DateTime.Today.ToShortDateString()} 0 SMIS in FLUSSI" + Environment.NewLine);
                    return false;
                }

                if (SmisListFlussi.Count() > 2)//FOR STANDART FLUSSI THERE CANNOT BE MORE THAN 2 NOT 3 BECAUSE THERE IS NO FILTER
                {
                    log.Info($"More than 2 SMIS in FLUSSI");
                    File.AppendAllText(speshal, $"{v.Riferimento} {v.Pod} {DateTime.Today.ToShortDateString()} More than 3 SMIS in FLUSSI" + Environment.NewLine);
                    return false;
                }

                log.Info($"{SmisListFlussi.Count()} FLUSSI SMIS After");

                if (SmisListFlussi.Count().Equals(1))
                {
                    log.Info($"Only 1 SMIS");
                    return false;
                }

                try
                {
                    log.Info($"1G1G");

                    #region CANCELLA EVERY SMIS
                    needToReloadListaLetture = true;
                    foreach (var smis in SmisListFlussi)
                    {
                        if (needToReloadListaLetture)
                        {
                            if (!FillListaLetture())
                                return false;
                            needToReloadListaLetture = false;
                        }
                        string smisData = smis.DataMisura.ToShortDateString();
                        foreach (var all in ListaLetture)
                        {
                            string allData = all.Data.ToShortDateString();
                            if (smisData == allData)
                            {
                                if (!ClickAzioniCancella(all))
                                {
                                    log.Error($"Couldn't Cancella SMIS");
                                    return false;
                                }
                                needToReloadListaLetture = true;
                            }
                        }
                    }
                    #endregion

                    if (!NavigateToMatPower(SmisListFlussi.FirstOrDefault().DataMisura.ToShortDateString()))
                        return false;

                    #region Dati Smontaggio
                    IWebElement ComboTipoSm = Keanu.Driver.FindElement(By.XPath("//mat-select[@formcontrolname='tipoLetturaSm']"));
                    if (!ComboTipoSm.Text.Contains("REALE"))
                    {
                        ComboTipoSm.Click();
                        Thread.Sleep(Keanu.Randy(1));
                        //IList<IWebElement> OptionsTipo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted mat-active']")).ToList();
                        IList<IWebElement> OptionsTipo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                        IWebElement OptionTipoReale = null;
                        try
                        {
                            OptionTipoReale = OptionsTipo.Where(x => x.Text.ToUpper().Contains("REALE")).ToList()[0];
                            OptionTipoReale.Click();
                            Thread.Sleep(Keanu.Randy(1));
                        }
                        catch
                        {
                            log.Info("Tipo Lettura Ex");
                        }
                    }

                    try
                    {
                        var attivaF1Sm = Keanu.Driver.FindElements(By.XPath(".//input[@name='attivaF1Sm']")).LastOrDefault();
                        attivaF1Sm.Clear();
                        if (SmisListFlussi.LastOrDefault().F1.ToString().Equals("-1"))
                            attivaF1Sm.SendKeys("0");
                        else
                        {
                            if (SmisListFlussi.FirstOrDefault().F1.ToString().Contains(","))
                                attivaF1Sm.SendKeys(SmisListFlussi.LastOrDefault().F1.ToString().Split(',')[0]);
                            else
                                attivaF1Sm.SendKeys(SmisListFlussi.LastOrDefault().F1.ToString());
                        }
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        var attivaF2Sm = Keanu.Driver.FindElements(By.XPath(".//input[@name='attivaF2Sm']")).LastOrDefault();
                        attivaF2Sm.Clear();
                        if (SmisListFlussi.LastOrDefault().F2.ToString().Equals("-1"))
                            attivaF2Sm.SendKeys("0");
                        else
                        {
                            if (SmisListFlussi.FirstOrDefault().F2.ToString().Contains(","))
                                attivaF2Sm.SendKeys(SmisListFlussi.LastOrDefault().F2.ToString().Split(',')[0]);
                            else
                                attivaF2Sm.SendKeys(SmisListFlussi.LastOrDefault().F2.ToString());
                        }
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        var attivaF3Sm = Keanu.Driver.FindElements(By.XPath(".//input[@name='attivaF3Sm']")).LastOrDefault();
                        attivaF3Sm.Clear();
                        if (SmisListFlussi.LastOrDefault().F3.ToString().Equals("-1"))
                            attivaF3Sm.SendKeys("0");
                        else
                        {
                            if (SmisListFlussi.FirstOrDefault().F3.ToString().Contains(","))
                                attivaF3Sm.SendKeys(SmisListFlussi.LastOrDefault().F3.ToString().Split(',')[0]);
                            else
                                attivaF3Sm.SendKeys(SmisListFlussi.LastOrDefault().F3.ToString());
                        }
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        var reattivaF1Sm = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF1Sm']")).LastOrDefault();
                        reattivaF1Sm.Clear();
                        if (SmisListFlussi.LastOrDefault().R1.ToString().Equals("-1"))
                        {
                            var topR1 = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF1']")).FirstOrDefault();
                            string value = topR1.GetAttribute("value");
                            reattivaF1Sm.SendKeys(value);
                        }
                        //reattivaF1Sm.SendKeys("0");
                        else
                        {
                            if (SmisListFlussi.FirstOrDefault().R1.ToString().Contains(","))
                                reattivaF1Sm.SendKeys(SmisListFlussi.LastOrDefault().R1.ToString().Split(',')[0]);
                            else
                                reattivaF1Sm.SendKeys(SmisListFlussi.LastOrDefault().R1.ToString());
                        }
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        var reattivaF2Sm = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF2Sm']")).LastOrDefault();
                        reattivaF2Sm.Clear();
                        if (SmisListFlussi.LastOrDefault().R2.ToString().Equals("-1"))
                        {
                            var topR2 = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF2']")).FirstOrDefault();
                            string value = topR2.GetAttribute("value");
                            reattivaF2Sm.SendKeys(value);
                        }
                        //reattivaF1Sm.SendKeys("0");
                        else
                        {
                            if (SmisListFlussi.FirstOrDefault().R2.ToString().Contains(","))
                                reattivaF2Sm.SendKeys(SmisListFlussi.LastOrDefault().R2.ToString().Split(',')[0]);
                            else
                                reattivaF2Sm.SendKeys(SmisListFlussi.LastOrDefault().R2.ToString());
                        }
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        var reattivaF3Sm = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF3Sm']")).LastOrDefault();
                        reattivaF3Sm.Clear();
                        if (SmisListFlussi.LastOrDefault().R3.ToString().Equals("-1"))
                        {
                            var topR3 = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF3']")).FirstOrDefault();
                            string value = topR3.GetAttribute("value");
                            reattivaF3Sm.SendKeys(value);
                        }
                        //reattivaF1Sm.SendKeys("0");
                        else
                        {
                            if (SmisListFlussi.FirstOrDefault().R3.ToString().Contains(","))
                                reattivaF3Sm.SendKeys(SmisListFlussi.LastOrDefault().R3.ToString().Split(',')[0]);
                            else
                                reattivaF3Sm.SendKeys(SmisListFlussi.LastOrDefault().R3.ToString());
                        }
                    }
                    catch (Exception)
                    {

                    }
                    #endregion

                    log.Info("Smontaggio done");

                    #region Dati Montaggio
                    string flagValue = "NO";

                    IWebElement ComboFlag = Keanu.Driver.FindElement(By.XPath("//mat-select[@formcontrolname='flag2gMont']"));
                    if (!ComboFlag.Text.Contains(flagValue))
                    {
                        ComboFlag.Click();
                        Thread.Sleep(Keanu.Randy(1));
                        IList<IWebElement> OptionsTipo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                        IWebElement OptionTipoReale = null;
                        try
                        {
                            OptionTipoReale = OptionsTipo.Where(x => x.Text.ToUpper().Contains(flagValue)).ToList()[0];
                            OptionTipoReale.Click();
                            Thread.Sleep(Keanu.Randy(1));
                        }
                        catch
                        {
                            log.Info("Flag 2G Ex");
                            return false;
                        }
                    }

                    IWebElement ComboTipoMisuratore = Keanu.Driver.FindElement(By.XPath("//mat-select[@formcontrolname='tipoMisuratoreMnt']"));
                    if (!ComboTipoMisuratore.Text.Contains("TRIORARIO CON REATTIVA"))
                    {
                        ComboTipoMisuratore.Click();
                        Thread.Sleep(Keanu.Randy(1));
                        IList<IWebElement> OptionsTipo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                        IWebElement OptionTipoReale = null;
                        try
                        {
                            OptionTipoReale = OptionsTipo.Where(x => x.Text.ToUpper().Contains("TRIORARIO CON REATTIVA")).ToList()[0];
                            OptionTipoReale.Click();
                            Thread.Sleep(Keanu.Randy(1));
                        }
                        catch
                        {
                            log.Info("Tipo Misuratore Ex");
                        }
                    }

                    var kattiva = Keanu.Driver.FindElement(By.XPath(".//input[@name='kattiva']"));
                    kattiva.Clear();
                    kattiva.SendKeys("1");
                    //kattiva.SendKeys(SmisListFlussi.FirstOrDefault().KA.ToString());

                    var matricolaAttiva = Keanu.Driver.FindElement(By.XPath(".//input[@name='matricolaAttiva']"));
                    matricolaAttiva.Clear();
                    matricolaAttiva.SendKeys(SmisListFlussi.FirstOrDefault().MatricolaA);

                    var cifreAttiva = Keanu.Driver.FindElement(By.XPath(".//input[@name='cifreAttiva']"));
                    cifreAttiva.Clear();
                    cifreAttiva.SendKeys("6");
                    //cifreAttiva.SendKeys(SmisListFlussi.FirstOrDefault().CifreA.ToString());

                    var cifreReattiva = Keanu.Driver.FindElement(By.XPath(".//input[@name='cifreReattiva']"));
                    cifreReattiva.Clear();
                    cifreReattiva.SendKeys("6");
                    //cifreReattiva.SendKeys(SmisListFlussi.FirstOrDefault().CifreR.ToString());

                    var cifrePotenza = Keanu.Driver.FindElement(By.XPath(".//input[@name='cifrePotenza']"));
                    cifrePotenza.Clear();
                    cifrePotenza.SendKeys("6");
                    //cifrePotenza.SendKeys(SmisListFlussi.FirstOrDefault().CifreP.ToString());

                    IWebElement ComboTipo = Keanu.Driver.FindElement(By.XPath("//mat-select[@formcontrolname='tipoLettura']"));
                    if (!ComboTipo.Text.Contains("REALE"))
                    {
                        ComboTipo.Click();
                        Thread.Sleep(Keanu.Randy(1));
                        //IList<IWebElement> OptionsTipo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted mat-active']")).ToList();
                        IList<IWebElement> OptionsTipo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                        IWebElement OptionTipoReale = null;
                        try
                        {
                            OptionTipoReale = OptionsTipo.Where(x => x.Text.ToUpper().Contains("REALE")).ToList()[0];
                            OptionTipoReale.Click();
                            Thread.Sleep(Keanu.Randy(1));
                        }
                        catch
                        {
                            log.Info("Tipo Lettura Ex");
                        }
                    }

                    var attivaF1 = Keanu.Driver.FindElements(By.XPath(".//input[@name='attivaF1']")).LastOrDefault();
                    attivaF1.Clear();
                    if (SmisListFlussi.FirstOrDefault().F1.ToString().Equals("-1"))
                        attivaF1.SendKeys("0");
                    else
                    {
                        if (SmisListFlussi.FirstOrDefault().F1.ToString().Contains(","))
                            attivaF1.SendKeys(SmisListFlussi.FirstOrDefault().F1.ToString().Split(',')[0]);
                        else
                            attivaF1.SendKeys(SmisListFlussi.FirstOrDefault().F1.ToString());
                    }

                    var attivaF2 = Keanu.Driver.FindElements(By.XPath(".//input[@name='attivaF2']")).LastOrDefault();
                    attivaF2.Clear();
                    if (SmisListFlussi.FirstOrDefault().F2.ToString().Equals("-1"))
                        attivaF2.SendKeys("0");
                    else
                    {
                        if (SmisListFlussi.FirstOrDefault().F2.ToString().Contains(","))
                            attivaF2.SendKeys(SmisListFlussi.FirstOrDefault().F2.ToString().Split(',')[0]);
                        else
                            attivaF2.SendKeys(SmisListFlussi.FirstOrDefault().F2.ToString());
                    }

                    var attivaF3 = Keanu.Driver.FindElements(By.XPath(".//input[@name='attivaF3']")).LastOrDefault();
                    attivaF3.Clear();
                    if (SmisListFlussi.FirstOrDefault().F3.ToString().Equals("-1"))
                        attivaF3.SendKeys("0");
                    else
                    {
                        if (SmisListFlussi.FirstOrDefault().F3.ToString().Contains(","))
                            attivaF3.SendKeys(SmisListFlussi.FirstOrDefault().F3.ToString().Split(',')[0]);
                        else
                            attivaF3.SendKeys(SmisListFlussi.FirstOrDefault().F3.ToString());
                    }

                    var reattivaF1 = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF1']")).LastOrDefault();
                    reattivaF1.Clear();
                    if (SmisListFlussi.FirstOrDefault().R1.ToString().Equals("-1"))
                        reattivaF1.SendKeys("0");
                    else
                    {
                        if (SmisListFlussi.FirstOrDefault().R1.ToString().Contains(","))
                            reattivaF1.SendKeys(SmisListFlussi.FirstOrDefault().R2.ToString().Split(',')[0]);
                        else
                            reattivaF1.SendKeys(SmisListFlussi.FirstOrDefault().R3.ToString());
                    }

                    var reattivaF2 = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF2']")).LastOrDefault();
                    reattivaF2.Clear();
                    if (SmisListFlussi.FirstOrDefault().R2.ToString().Equals("-1"))
                        reattivaF2.SendKeys("0");
                    else
                    {
                        if (SmisListFlussi.FirstOrDefault().R2.ToString().Contains(","))
                            reattivaF2.SendKeys(SmisListFlussi.FirstOrDefault().R2.ToString().Split(',')[0]);
                        else
                            reattivaF2.SendKeys(SmisListFlussi.FirstOrDefault().R2.ToString());
                    }

                    var reattivaF3 = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF3']")).LastOrDefault();
                    reattivaF3.Clear();
                    if (SmisListFlussi.FirstOrDefault().R3.ToString().Equals("-1"))
                        reattivaF3.SendKeys("0");
                    else
                    {
                        if (SmisListFlussi.FirstOrDefault().R3.ToString().Contains(","))
                            reattivaF3.SendKeys(SmisListFlussi.FirstOrDefault().R3.ToString().Split(',')[0]);
                        else
                            reattivaF3.SendKeys(SmisListFlussi.FirstOrDefault().R3.ToString());
                    }
                    #endregion

                    log.Info("Montaggio done");

                    //CONFERMA
                    var bConferma = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'Conferma')]"));
                    bConferma.Click();
                    WaitLoadPageEE12();

                    if (Keanu.Driver.PageSource.ToString().Contains("Operazione effettuata con successo. Non prevista creazione device change"))
                    {
                        log.Info("Operazione effettuata con successo. Non prevista creazione device change");
                        var bOk = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'OK')]"));
                        bOk.Click();
                        WaitLoadPageEE12();
                        //return true;
                    }
                    else if (Keanu.Driver.PageSource.ToString().Contains("Operazione effettuata con successo"))
                    {
                        log.Info("Operazione effettuata con successo");
                        var bOk = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'OK')]"));
                        bOk.Click();
                        WaitLoadPageEE12();
                        //return true;
                    }
                    else if (Keanu.Driver.PageSource.ToString().Contains("MAT non ammessa - Operazione non effettuata"))
                    {
                        log.Error("MAT non ammessa - Operazione non effettuata");
                        var bOk = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'OK')]"));
                        bOk.Click();
                        WaitLoadPageEE12();
                        return false;
                    }
                    else if (Keanu.Driver.PageSource.ToString().Contains("MAT eseguita - Cambio alla posa con misuratore di default")) {
                        log.Info("MAT eseguita - Cambio alla posa con misuratore di default");
                        var bOk = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'OK')]"));
                        bOk.Click();
                        WaitLoadPageEE12();
                        
                    }else
                    {
                        log.Error("Mat fail");
                        try
                        {
                            var bOk = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'OK')]"));
                            bOk.Click();
                            WaitLoadPageEE12();
                        }
                        catch (Exception)
                        {

                        }
                        return false;
                    }

                    matPower = true;
                    finishInLavorazioneAfterMatPower = true;

                    return true;
                }
                catch (Exception Ex)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool Cambio1G2G(bool flag)
        {
            try
            {
                log.Info($"1G2G");

                #region CANCELLA EVERY SMIS
                needToReloadListaLetture = true;
                foreach (var smis in SmisList)
                {
                    if (needToReloadListaLetture)
                    {
                        if (!FillListaLetture())
                            return false;
                        needToReloadListaLetture = false;
                    }
                    string smisData = smis.DataMisura.ToShortDateString();
                    foreach (var all in ListaLetture)
                    {
                        string allData = all.Data.ToShortDateString();
                        if (smisData == allData)
                        {
                            if (!ClickAzioniCancella(all))
                            {
                                log.Error($"Couldn't Cancella SMIS");
                                return false;
                            }
                            needToReloadListaLetture = true;
                        }
                    }
                }
                #endregion

                if (!NavigateToMatPower(SmisList.FirstOrDefault().DataMisura.ToShortDateString()))
                    return false;

                #region Dati Smontaggio
                IWebElement ComboTipoSm = Keanu.Driver.FindElement(By.XPath("//mat-select[@formcontrolname='tipoLetturaSm']"));
                if (!ComboTipoSm.Text.Contains("REALE"))
                {
                    ComboTipoSm.Click();
                    Thread.Sleep(Keanu.Randy(1));
                    //IList<IWebElement> OptionsTipo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted mat-active']")).ToList();
                    IList<IWebElement> OptionsTipo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                    IWebElement OptionTipoReale = null;
                    try
                    {
                        OptionTipoReale = OptionsTipo.Where(x => x.Text.ToUpper().Contains("REALE")).ToList()[0];
                        OptionTipoReale.Click();
                        Thread.Sleep(Keanu.Randy(1));
                    }
                    catch
                    {
                        log.Info("Tipo Lettura Ex");
                    }
                }

                try
                {
                    var attivaF1Sm = Keanu.Driver.FindElements(By.XPath(".//input[@name='attivaF1Sm']")).LastOrDefault();
                    attivaF1Sm.Clear();
                    if (SmisList.LastOrDefault().F1.ToString().Equals("-1"))
                        attivaF1Sm.SendKeys("0");
                    else
                    {
                        if (SmisList.FirstOrDefault().F1.ToString().Contains(","))
                            attivaF1Sm.SendKeys(SmisList.LastOrDefault().F1.ToString().Split(',')[0]);
                        else
                            attivaF1Sm.SendKeys(SmisList.LastOrDefault().F1.ToString());
                    }
                }
                catch (Exception)
                {

                }

                try
                {
                    var attivaF2Sm = Keanu.Driver.FindElements(By.XPath(".//input[@name='attivaF2Sm']")).LastOrDefault();
                    attivaF2Sm.Clear();
                    if (SmisList.LastOrDefault().F2.ToString().Equals("-1"))
                        attivaF2Sm.SendKeys("0");
                    else
                    {
                        if (SmisList.FirstOrDefault().F2.ToString().Contains(","))
                            attivaF2Sm.SendKeys(SmisList.LastOrDefault().F2.ToString().Split(',')[0]);
                        else
                            attivaF2Sm.SendKeys(SmisList.LastOrDefault().F2.ToString());
                    }
                }
                catch (Exception)
                {

                }

                try
                {
                    var attivaF3Sm = Keanu.Driver.FindElements(By.XPath(".//input[@name='attivaF3Sm']")).LastOrDefault();
                    attivaF3Sm.Clear();
                    if (SmisList.LastOrDefault().F3.ToString().Equals("-1"))
                        attivaF3Sm.SendKeys("0");
                    else
                    {
                        if (SmisList.FirstOrDefault().F3.ToString().Contains(","))
                            attivaF3Sm.SendKeys(SmisList.LastOrDefault().F3.ToString().Split(',')[0]);
                        else
                            attivaF3Sm.SendKeys(SmisList.LastOrDefault().F3.ToString());
                    }
                }
                catch (Exception)
                {

                }

                try
                {
                    var reattivaF1Sm = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF1Sm']")).LastOrDefault();
                    reattivaF1Sm.Clear();
                    if (SmisList.LastOrDefault().R1.ToString().Equals("-1"))
                    {
                        var topR1 = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF1']")).FirstOrDefault();
                        string value = topR1.GetAttribute("value");
                        reattivaF1Sm.SendKeys(value);
                    }
                    //reattivaF1Sm.SendKeys("0");
                    else
                    {
                        if (SmisList.FirstOrDefault().R1.ToString().Contains(","))
                            reattivaF1Sm.SendKeys(SmisList.LastOrDefault().R1.ToString().Split(',')[0]);
                        else
                            reattivaF1Sm.SendKeys(SmisList.LastOrDefault().R1.ToString());
                    }
                }
                catch (Exception)
                {

                }

                try
                {
                    var reattivaF2Sm = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF2Sm']")).LastOrDefault();
                    reattivaF2Sm.Clear();
                    if (SmisList.LastOrDefault().R2.ToString().Equals("-1"))
                    {
                        var topR2 = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF2']")).FirstOrDefault();
                        string value = topR2.GetAttribute("value");
                        reattivaF2Sm.SendKeys(value);
                    }
                    //reattivaF1Sm.SendKeys("0");
                    else
                    {
                        if (SmisList.FirstOrDefault().R2.ToString().Contains(","))
                            reattivaF2Sm.SendKeys(SmisList.LastOrDefault().R2.ToString().Split(',')[0]);
                        else
                            reattivaF2Sm.SendKeys(SmisList.LastOrDefault().R2.ToString());
                    }
                }
                catch (Exception)
                {

                }

                try
                {
                    var reattivaF3Sm = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF3Sm']")).LastOrDefault();
                    reattivaF3Sm.Clear();
                    if (SmisList.LastOrDefault().R3.ToString().Equals("-1"))
                    {
                        var topR3 = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF3']")).FirstOrDefault();
                        string value = topR3.GetAttribute("value");
                        reattivaF3Sm.SendKeys(value);
                    }
                    //reattivaF1Sm.SendKeys("0");
                    else
                    {
                        if (SmisList.FirstOrDefault().R3.ToString().Contains(","))
                            reattivaF3Sm.SendKeys(SmisList.LastOrDefault().R3.ToString().Split(',')[0]);
                        else
                            reattivaF3Sm.SendKeys(SmisList.LastOrDefault().R3.ToString());
                    }
                }
                catch (Exception)
                {

                }
                #endregion

                log.Info("Smontaggio done");

                #region Dati Montaggio
                string[] split = SmisList.FirstOrDefault().DataMisura.ToString().Split(' ');
                string[] split2 = split[0].ToString().Split('/');
                string kompetence = split2[2] + split2[1] + split2[0] + split[1].Replace(":", "");
                string link = "https://next.enelint.global/api-next-online/api/v1/protected/" + v.Pod + "/measures/mm2g/" + kompetence + "?uid=" + SmisList.FirstOrDefault().Uid + "&repository=" + SmisList.FirstOrDefault().Repository;
                MontaggioRow montaggio = GetAllFromFlussi2GRowMontaggio(link);

                motivazioneFromUpperSmis = montaggio.MotivazioneRettifica;

                string flagValue = "";
                if (flag)
                    flagValue = "SI";
                else
                    flagValue = "NO";

                IWebElement ComboFlag = Keanu.Driver.FindElement(By.XPath("//mat-select[@formcontrolname='flag2gMont']"));
                if (!ComboFlag.Text.Contains(flagValue))
                {
                    ComboFlag.Click();
                    Thread.Sleep(Keanu.Randy(1));
                    IList<IWebElement> OptionsTipo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                    IWebElement OptionTipoReale = null;
                    try
                    {
                        OptionTipoReale = OptionsTipo.Where(x => x.Text.ToUpper().Contains(flagValue)).ToList()[0];
                        OptionTipoReale.Click();
                        Thread.Sleep(Keanu.Randy(1));
                    }
                    catch
                    {
                        log.Info("Flag 2G Ex");
                        return false;
                    }
                }

                if (flag)//IF FALSE THEN MOTIVAZIONE IS DISABLED ANYWAY
                {
                    IWebElement ComboMotivazione = Keanu.Driver.FindElement(By.XPath("//mat-select[@formcontrolname='motivazione']"));
                    ComboMotivazione.Click();
                    Thread.Sleep(Keanu.Randy(1));
                    IList<IWebElement> OptionsMotivazione = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                    IWebElement Option = null;
                    try
                    {
                        Option = OptionsMotivazione.Where(x => x.Text.ToUpper().Contains(montaggio.MotivazioneRettifica)).ToList()[0];
                        Option.Click();
                        Thread.Sleep(Keanu.Randy(1));
                    }
                    catch
                    {
                        log.Info("Motivazione Ex");
                    }
                }

                IWebElement ComboTipoMisuratore = Keanu.Driver.FindElement(By.XPath("//mat-select[@formcontrolname='tipoMisuratoreMnt']"));
                if (!ComboTipoMisuratore.Text.Contains("TRIORARIO CON REATTIVA"))
                {
                    ComboTipoMisuratore.Click();
                    Thread.Sleep(Keanu.Randy(1));
                    IList<IWebElement> OptionsTipo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                    IWebElement OptionTipoReale = null;
                    try
                    {
                        OptionTipoReale = OptionsTipo.Where(x => x.Text.ToUpper().Contains("TRIORARIO CON REATTIVA")).ToList()[0];
                        OptionTipoReale.Click();
                        Thread.Sleep(Keanu.Randy(1));
                    }
                    catch
                    {
                        log.Info("Tipo Misuratore Ex");
                    }
                }

                var kattiva = Keanu.Driver.FindElement(By.XPath(".//input[@name='kattiva']"));
                kattiva.Clear();
                //kattiva.SendKeys("1");
                kattiva.SendKeys(montaggio.KA.ToString());

                var matricolaAttiva = Keanu.Driver.FindElement(By.XPath(".//input[@name='matricolaAttiva']"));
                matricolaAttiva.Clear();
                matricolaAttiva.SendKeys(montaggio.MatricolaA);

                var cifreAttiva = Keanu.Driver.FindElement(By.XPath(".//input[@name='cifreAttiva']"));
                cifreAttiva.Clear();
                cifreAttiva.SendKeys("6");
                //cifreAttiva.SendKeys(montaggio.CifreA.ToString());

                var cifreReattiva = Keanu.Driver.FindElement(By.XPath(".//input[@name='cifreReattiva']"));
                cifreReattiva.Clear();
                cifreReattiva.SendKeys("6");
                //cifreReattiva.SendKeys(montaggio.CifreR.ToString());

                var cifrePotenza = Keanu.Driver.FindElement(By.XPath(".//input[@name='cifrePotenza']"));
                cifrePotenza.Clear();
                cifrePotenza.SendKeys("6");
                //cifrePotenza.SendKeys(montaggio.CifreP.ToString());

                IWebElement ComboTipo = Keanu.Driver.FindElement(By.XPath("//mat-select[@formcontrolname='tipoLettura']"));
                if (!ComboTipo.Text.Contains("REALE"))
                {
                    ComboTipo.Click();
                    Thread.Sleep(Keanu.Randy(1));

                    IList<IWebElement> OptionsTipo = null;
                    if (!flag)//IF FLAG IS ENABLED THEN THERE ARE ONLY ONE OPTION TO CHOOSE FROM, IF DISABLED THEN 2
                        OptionsTipo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted']")).ToList();
                    else
                        OptionsTipo = Keanu.Driver.FindElements(By.XPath(".//mat-option[@class='mat-option ng-star-inserted mat-active']")).ToList();
                    IWebElement OptionTipoReale = null;
                    try
                    {
                        OptionTipoReale = OptionsTipo.Where(x => x.Text.ToUpper().Contains("REALE")).ToList()[0];
                        OptionTipoReale.Click();
                        Thread.Sleep(Keanu.Randy(1));
                    }
                    catch
                    {
                        log.Info("Tipo Lettura Ex");
                    }
                }

                var attivaF1 = Keanu.Driver.FindElements(By.XPath(".//input[@name='attivaF1']")).LastOrDefault();
                attivaF1.Clear();
                if (SmisList.FirstOrDefault().F1.ToString().Equals("-1"))
                    attivaF1.SendKeys("0");
                else
                {
                    if (SmisList.FirstOrDefault().F1.ToString().Contains(","))
                        attivaF1.SendKeys(SmisList.FirstOrDefault().F1.ToString().Split(',')[0]);
                    else
                        attivaF1.SendKeys(SmisList.FirstOrDefault().F1.ToString());
                }

                var attivaF2 = Keanu.Driver.FindElements(By.XPath(".//input[@name='attivaF2']")).LastOrDefault();
                attivaF2.Clear();
                if (SmisList.FirstOrDefault().F2.ToString().Equals("-1"))
                    attivaF2.SendKeys("0");
                else
                {
                    if (SmisList.FirstOrDefault().F2.ToString().Contains(","))
                        attivaF2.SendKeys(SmisList.FirstOrDefault().F2.ToString().Split(',')[0]);
                    else
                        attivaF2.SendKeys(SmisList.FirstOrDefault().F2.ToString());
                }

                var attivaF3 = Keanu.Driver.FindElements(By.XPath(".//input[@name='attivaF3']")).LastOrDefault();
                attivaF3.Clear();
                if (SmisList.FirstOrDefault().F3.ToString().Equals("-1"))
                    attivaF3.SendKeys("0");
                else
                {
                    if (SmisList.FirstOrDefault().F3.ToString().Contains(","))
                        attivaF3.SendKeys(SmisList.FirstOrDefault().F3.ToString().Split(',')[0]);
                    else
                        attivaF3.SendKeys(SmisList.FirstOrDefault().F3.ToString());
                }

                var reattivaF1 = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF1']")).LastOrDefault();
                reattivaF1.Clear();
                if (SmisList.FirstOrDefault().R1.ToString().Equals("-1"))
                    reattivaF1.SendKeys("0");
                else
                {
                    if (SmisList.FirstOrDefault().R1.ToString().Contains(","))
                        reattivaF1.SendKeys(SmisList.FirstOrDefault().R2.ToString().Split(',')[0]);
                    else
                        reattivaF1.SendKeys(SmisList.FirstOrDefault().R3.ToString());
                }

                var reattivaF2 = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF2']")).LastOrDefault();
                reattivaF2.Clear();
                if (SmisList.FirstOrDefault().R2.ToString().Equals("-1"))
                    reattivaF2.SendKeys("0");
                else
                {
                    if (SmisList.FirstOrDefault().R2.ToString().Contains(","))
                        reattivaF2.SendKeys(SmisList.FirstOrDefault().R2.ToString().Split(',')[0]);
                    else
                        reattivaF2.SendKeys(SmisList.FirstOrDefault().R2.ToString());
                }

                var reattivaF3 = Keanu.Driver.FindElements(By.XPath(".//input[@name='reattivaF3']")).LastOrDefault();
                reattivaF3.Clear();
                if (SmisList.FirstOrDefault().R3.ToString().Equals("-1"))
                    reattivaF3.SendKeys("0");
                else
                {
                    if (SmisList.FirstOrDefault().R3.ToString().Contains(","))
                        reattivaF3.SendKeys(SmisList.FirstOrDefault().R3.ToString().Split(',')[0]);
                    else
                        reattivaF3.SendKeys(SmisList.FirstOrDefault().R3.ToString());
                }
                #endregion

                log.Info("Montaggio done");

                //CONFERMA
                var bConferma = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'Conferma')]"));
                bConferma.Click();
                WaitLoadPageEE12();

                if (Keanu.Driver.PageSource.ToString().Contains("Operazione effettuata con successo. Non prevista creazione device change"))
                {
                    log.Info("Operazione effettuata con successo. Non prevista creazione device change");
                    var bOk = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'OK')]"));
                    bOk.Click();
                    WaitLoadPageEE12();
                    //return true;
                }
                else if (Keanu.Driver.PageSource.ToString().Contains("Operazione effettuata con successo"))
                {
                    log.Info("Operazione effettuata con successo");
                    var bOk = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'OK')]"));
                    bOk.Click();
                    WaitLoadPageEE12();
                    //return true;
                }
                else if (Keanu.Driver.PageSource.ToString().Contains("MAT non ammessa - Operazione non effettuata"))
                {
                    log.Error("MAT non ammessa - Operazione non effettuata");
                    var bOk = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'OK')]"));
                    bOk.Click();
                    WaitLoadPageEE12();
                    return false;
                }
                else
                {
                    log.Error("Mat fail");
                    try
                    {
                        var bOk = Keanu.Driver.FindElement(By.XPath("//button[contains(text(), 'OK')]"));
                        bOk.Click();
                        WaitLoadPageEE12();
                    }
                    catch (Exception)
                    {

                    }
                    return false;
                }

                matPower = true;
                finishInLavorazioneAfterMatPower = true;

                return true;
            }
            catch (Exception Ex)
            {
                return false;
            }
        }

        private bool Cambio2G2G()
        {
            log.Info($"2G2G");
            try
            {
                File.AppendAllText(speshal, $"{v.Riferimento} {v.Pod} {DateTime.Today.ToShortDateString()} 2G2G" + Environment.NewLine);
                matPower = false;//BECAUSE THERE WAS NO CAMBIO MISURATORE THEN DON'T CHECK SHIT
                return false;
            }
            catch
            {
                return false;
            }
        }

         #region Request
        public bool Ripristina(String UrlRequestRipristina, out string ErroreRipristina)
        {
            ErroreRipristina = "";
            bool Esito = true;
            try
            {
                var httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(UrlRequestRipristina);
                httpWebRequest.Method = "POST";
                System.Net.NetworkCredential netCredential = new System.Net.NetworkCredential(this.Keanu.LoginNEXT, this.Keanu.PassNEXT, "enelint.global");
                httpWebRequest.Credentials = netCredential;
                httpWebRequest.ContentType = "application/json";

                var httpResponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string responseText = streamReader.ReadToEnd();
                    log.Debug("Response sblocco flusso dt " + responseText);
                    JArray JArray = JArray.Parse(responseText);
                    JToken Token = (JToken)JArray.ElementAt(0);
                    string EsitoJson = (string)Token["id"];
                    log.Debug("Ripristina: " + EsitoJson);
                    if (EsitoJson.Equals("MODAL_SUCCESS"))
                        return true;
                    else
                    {
                        ErroreRipristina = (string)Token["descrizione"];
                        return false;
                    }
                }
            }
            catch (Exception Ex) { return false; }
        }

        public bool ForzaValida(String UrlRequestForzaValida, out string ErroreForzaValida)
        {
            ErroreForzaValida = "";
            bool Esito = true;
            try
            {
                var httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(UrlRequestForzaValida);
                httpWebRequest.Method = "POST";
                System.Net.NetworkCredential netCredential = new System.Net.NetworkCredential(this.Keanu.LoginNEXT, this.Keanu.PassNEXT, "enelint.global");
                httpWebRequest.Credentials = netCredential;
                httpWebRequest.ContentType = "application/json";

                var httpResponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string responseText = streamReader.ReadToEnd();
                    log.Debug("Response sblocco flusso dt " + responseText);
                    JArray JArray = JArray.Parse(responseText);
                    JToken Token = (JToken)JArray.ElementAt(0);
                    string EsitoJson = (string)Token["id"];
                    log.Debug("ForzaValida: " + EsitoJson);
                    if (EsitoJson.Equals("MODAL_SUCCESS"))
                        return true;
                    else
                    {
                        ErroreForzaValida = (string)Token["descrizione"];
                        return false;
                    }
                }
            }
            catch (Exception Ex) { return false; }
        }

        public IList<Records.PostMalone> GetListaPostSales(string UrlRequest)
        {
            IList<Records.PostMalone> records = new List<Records.PostMalone>();
            Records.PostMalone Postal;

            try
            {
                System.Net.HttpWebRequest request = null;
                int tentativi = 0;
                System.Net.HttpWebResponse risposta = null;
                while (tentativi < 5 && risposta == null)
                {
                    try { request = GenerateRequest(UrlRequest, "", "GET", false, this.Keanu.LoginNEXT, this.Keanu.PassNEXT); }
                    catch { }

                    try
                    {
                        risposta = GetResponse(request);
                        if (risposta == null) tentativi++;
                    }
                    catch (Exception Ex)
                    {
                        log.Error(Ex.Message);
                        tentativi++;
                        Thread.Sleep(120000);
                    }
                }

                var encoding = Encoding.ASCII;
                using (var reader = new StreamReader(risposta.GetResponseStream(), encoding))
                {
                    string responseText = reader.ReadToEnd();
                    JArray arr = JArray.Parse(responseText);
                    bool continua = true;
                    int contatore = 0;

                    while (continua)
                    {
                        try
                        {
                            JToken Misura = (JToken)arr.ElementAt(contatore);
                            Postal = new Records.PostMalone();

                            Postal.DataProcesso = Convert.ToDateTime((string)Misura["processDate"]);
                            //try
                            //{
                            //    string val_mese = ((string)Misura["processDate"]).Substring(0, 2);
                            //    string val_giorno = ((string)Misura["processDate"]).Substring(3, 2);
                            //    string val_anno = ((string)Misura["processDate"]).Substring(6, 4);
                            //    Postal.DataProcesso = Convert.ToDateTime(val_giorno + "/" + val_mese + "/" + val_anno);
                            //}
                            //catch { Postal.DataProcesso = DateTime.MinValue; }

                            contatore++;
                            records.Add(Postal);
                        }
                        catch { continua = false; }
                    }
                }
            }
            catch { }
            if (records.Count == 0)
            {
                log.Info("NESSUN RISULTATO TROVATO POST SALES");
            }
            return records;
        }

        public IList<Records.AnagraficaTecnica> GetListaAnagraficaTecnica(string UrlRequest)
        {
            IList<Records.AnagraficaTecnica> records = new List<Records.AnagraficaTecnica>();
            Records.AnagraficaTecnica Anag;

            try
            {
                System.Net.HttpWebRequest request = null;
                int tentativi = 0;
                System.Net.HttpWebResponse risposta = null;
                while (tentativi < 5 && risposta == null)
                {
                    try { request = GenerateRequest(UrlRequest, "", "GET", false, this.Keanu.LoginNEXT, this.Keanu.PassNEXT); }
                    catch { }

                    try
                    {
                        risposta = GetResponse(request);
                        if (risposta == null) tentativi++;
                    }
                    catch (Exception Ex)
                    {
                        log.Error(Ex.Message);
                        tentativi++;
                        Thread.Sleep(120000);
                    }
                }

                var encoding = Encoding.ASCII;
                using (var reader = new StreamReader(risposta.GetResponseStream(), encoding))
                {
                    string responseText = reader.ReadToEnd();
                    JArray arr = JArray.Parse(responseText);
                    bool continua = true;
                    int contatore = 0;

                    while (continua)
                    {
                        try
                        {
                            JToken Misura = (JToken)arr.ElementAt(contatore);
                            Anag = new Records.AnagraficaTecnica();

                            Anag.DataProcesso = Convert.ToDateTime((string)Misura["dataInizioValiditaMisuratore"]);
                            //try
                            //{
                            //    string val_mese = ((string)Misura["dataInizioValiditaMisuratore"]).Substring(0, 2);
                            //    string val_giorno = ((string)Misura["dataInizioValiditaMisuratore"]).Substring(3, 2);
                            //    string val_anno = ((string)Misura["dataInizioValiditaMisuratore"]).Substring(6, 4);
                            //    Anag.DataProcesso = Convert.ToDateTime(val_giorno + "/" + val_mese + "/" + val_anno);
                            //}
                            //catch { Anag.DataProcesso = DateTime.MinValue; }

                            contatore++;
                            records.Add(Anag);
                        }
                        catch { continua = false; }
                    }
                }
            }
            catch { }
            if (records.Count == 0)
            {
                log.Info("NESSUN RISULTATO TROVATO ANAGRAFICA TECNICA");
            }
            return records;
        }

        public IList<Records.Flussi> GetListaFlussiPower(string UrlRequest)
        {
            IList<Records.Flussi> records = new List<Records.Flussi>();
            Records.Flussi Flusso;

            try
            {
                System.Net.HttpWebRequest request = null;
                int tentativi = 0;
                System.Net.HttpWebResponse risposta = null;
                while (tentativi < 5 && risposta == null)
                {
                    try { request = GenerateRequest(UrlRequest, "", "GET", false, this.Keanu.LoginNEXT, this.Keanu.PassNEXT); }
                    catch { }

                    try
                    {
                        risposta = GetResponse(request);
                        if (risposta == null) tentativi++;
                    }
                    catch (Exception Ex)
                    {
                        log.Error(Ex.Message);
                        tentativi++;
                        Thread.Sleep(120000);
                    }
                }

                var encoding = Encoding.ASCII;
                using (var reader = new StreamReader(risposta.GetResponseStream(), encoding))
                {
                    string responseText = reader.ReadToEnd();
                    JArray arr = JArray.Parse(responseText);
                    bool continua = true;
                    int contatore = 0;

                    while (continua)
                    {
                        try
                        {
                            string f1 = "";
                            string f2 = "";
                            string f3 = "";
                            string r1 = "";
                            string r2 = "";
                            string r3 = "";

                            JToken Misura = (JToken)arr.ElementAt(contatore);
                            Flusso = new Records.Flussi();

                            Flusso.Uid = (string)Misura["uid"];
                            Flusso.CodiceFlusso = (string)Misura["codiceFlusso"];
                            Flusso.MeseAnno = (string)Misura["meseAnno"];
                            Flusso.DataMisura = Convert.ToDateTime((string)Misura["dataMisura"]);
                            Flusso.NomeFile = (string)Misura["nomeFile"];
                            try
                            {
                                string val_mese = ((string)Misura["dataCaricamento"]).Substring(0, 2);
                                string val_giorno = ((string)Misura["dataCaricamento"]).Substring(3, 2);
                                string val_anno = ((string)Misura["dataCaricamento"]).Substring(6, 4);
                                Flusso.DataCaricamento = Convert.ToDateTime(val_giorno + "/" + val_mese + "/" + val_anno);
                            }
                            catch { Flusso.DataCaricamento = DateTime.MinValue; }
                            Flusso.Misuratore = (string)Misura["trattamento"];
                            Flusso.Raccolta = (string)Misura["raccolta"];
                            Flusso.TipoDato = (string)Misura["tipoDato"];
                            Flusso.ValidatoDistributore = (string)Misura["valididatoDT"];
                            Flusso.Esito = (string)Misura["esito"];
                            Flusso.Motivazione = (string)Misura["motivazione"];
                            Flusso.MotivazioneRettifica = (string)Misura["motivazioneRettifica"];
                            try { Flusso.KA = (int)Misura["kAttiva"]; } catch (Exception Ex) { Flusso.KA = 1; }
                            try { Flusso.KP = (int)Misura["kPotenza"]; } catch (Exception Ex) { Flusso.KP = 1; }
                            try { Flusso.KR = (int)Misura["kReattiva"]; } catch (Exception Ex) { Flusso.KR = 1; }
                            try { Flusso.CifreA = (int)Misura["cifreAttiva"]; } catch (Exception Ex) { Flusso.CifreA = 6; }
                            try { Flusso.CifreP = (int)Misura["cifrePotenza"]; } catch (Exception Ex) { Flusso.CifreP = 6; }
                            try { Flusso.CifreR = (int)Misura["cifreReattiva"]; } catch (Exception Ex) { Flusso.CifreR = 6; }
                            Flusso.MatricolaA = (string)Misura["matricolaAttiva"];
                            Flusso.MatricolaP = (string)Misura["matricolaPotenza"];
                            Flusso.MatricolaR = (string)Misura["matricolaReattiva"];
                            try
                            {
                                f1 = (string)Misura["energiaAttivaF1"];
                                if (f1.Contains(',')) { f1 = f1.Split(',')[0]; }
                                Flusso.F1 = Convert.ToInt32(f1);
                            }
                            catch (Exception Ex) { Flusso.F1 = -1; }
                            try
                            {
                                f2 = (string)Misura["energiaAttivaF2"];
                                if (f2.Contains(',')) { f2 = f2.Split(',')[0]; }
                                Flusso.F2 = Convert.ToInt32(f2);
                            }
                            catch (Exception Ex) { Flusso.F2 = -1; }
                            try
                            {
                                f3 = (string)Misura["energiaAttivaF3"];
                                if (f3.Contains(',')) { f3 = f3.Split(',')[0]; }
                                Flusso.F3 = Convert.ToInt32(f3);
                            }
                            catch (Exception Ex) { Flusso.F3 = -1; }
                            try
                            {
                                r1 = (string)Misura["energiaReattivaF1"];
                                if (r1.Contains(',')) { r1 = r1.Split(',')[0]; }
                                Flusso.R1 = Convert.ToInt32(r1);
                            }
                            catch (Exception Ex) { Flusso.R1 = -1; }
                            try
                            {
                                r2 = (string)Misura["energiaReattivaF2"];
                                if (r2.Contains(',')) { r2 = r2.Split(',')[0]; }
                                Flusso.R2 = Convert.ToInt32(r2);
                            }
                            catch (Exception Ex) { Flusso.R2 = -1; }
                            try
                            {
                                r3 = (string)Misura["energiaReattivaF3"];
                                if (r3.Contains(',')) { r3 = r3.Split(',')[0]; }
                                Flusso.R3 = Convert.ToInt32(r3);
                            }
                            catch (Exception Ex) { Flusso.R3 = -1; }

                            contatore++;
                            records.Add(Flusso);
                        }
                        catch { continua = false; }
                    }
                }
            }
            catch { }
            if (records.Count == 0)
            {
                log.Info("NESSUN RISULTATO TROVATO FLUSSI");
            }
            return records;
        }

        public IList<Records.Flussi2G> GetListaFlussi2GPower(string UrlRequest)
        {
            IList<Records.Flussi2G> records = new List<Records.Flussi2G>();
            Records.Flussi2G Flusso2G;

            try
            {
                System.Net.HttpWebRequest request = null;
                int tentativi = 0;
                System.Net.HttpWebResponse risposta = null;
                while (tentativi < 5 && risposta == null)
                {
                    try { request = GenerateRequest(UrlRequest, "", "GET", false, this.Keanu.LoginNEXT, this.Keanu.PassNEXT); }
                    catch { }

                    try
                    {
                        risposta = GetResponse(request);
                        if (risposta == null) tentativi++;
                    }
                    catch (Exception Ex)
                    {
                        log.Error(Ex.Message);
                        tentativi++;
                        Thread.Sleep(120000);
                    }
                }

                var encoding = Encoding.ASCII;
                using (var reader = new StreamReader(risposta.GetResponseStream(), encoding))
                {
                    string responseText = reader.ReadToEnd();
                    JObject JObject = JObject.Parse(responseText);
                    bool continua = true;
                    int contatore = 0;
                    JToken TokenListaMisure;
                    if (JObject.TryGetValue("measures", out TokenListaMisure))
                    {
                        while (continua)
                        {
                            try
                            {
                                string f1 = "";
                                string f2 = "";
                                string f3 = "";
                                string r1 = "";
                                string r2 = "";
                                string r3 = "";

                                JToken Misura = (JToken)TokenListaMisure.ElementAt(contatore);
                                Flusso2G = new Records.Flussi2G();

                                Flusso2G.Uid = (string)Misura["uid"];
                                Flusso2G.DataMisura = Convert.ToDateTime((string)Misura["dtCompetence"]);
                                Flusso2G.MeseAnno = ((string)Misura["dtCompetence"]).Substring(5, 2) + "/" + ((string)Misura["dtCompetence"]).Substring(0, 4);
                                Flusso2G.CodiceFlusso = (string)Misura["streamType"];
                                try { Flusso2G.DataCaricamento = Convert.ToDateTime((string)Misura["dtAcquisition"]); }
                                catch { Flusso2G.DataCaricamento = DateTime.MinValue; }
                                try { Flusso2G.DataPubblicazione = Convert.ToDateTime((string)Misura["dtPublication"]); }
                                catch { Flusso2G.DataPubblicazione = DateTime.MinValue; }
                                Flusso2G.Tipo = (string)Misura["measureTypeDesc"];
                                Flusso2G.Stato = (string)Misura["status"];
                                Flusso2G.StatoAbb = (string)Misura["codPairingStatus"];
                                Flusso2G.TipoRettifica = (string)Misura["tipoRettifica"];
                                Flusso2G.Motivazione = (string)Misura["motivazioneRettifica"];
                                Flusso2G.ValidatoDT = (string)Misura["validato"];
                                Flusso2G.Esito = (string)Misura["esito"];
                                Flusso2G.CodiceEsito = (string)Misura["codStatusNotes"];
                                Flusso2G.Repository = ((string)Misura["repository"]).TrimEnd();
                                if (Flusso2G.Repository.EndsWith(" ")) Flusso2G.Repository = Flusso2G.Repository.Substring(0, Flusso2G.Repository.Length - 1);
                                try
                                {
                                    JObject Misure = (JObject)Misura["mapSign"];
                                    try
                                    {
                                        JToken TokenEnergiaAttiva;
                                        if (Misure.TryGetValue("A", out TokenEnergiaAttiva))
                                        {
                                            try
                                            {
                                                f1 = (string)TokenEnergiaAttiva["F1"];
                                                if (f1.Contains(".")) { f1 = f1.Split('.')[0]; }
                                                Flusso2G.F1 = Convert.ToInt32(f1);
                                            }
                                            catch (Exception Ex) { Flusso2G.F1 = -1; }
                                            try
                                            {
                                                f2 = (string)TokenEnergiaAttiva["F2"];
                                                if (f2.Contains(".")) { f2 = f2.Split('.')[0]; }
                                                Flusso2G.F2 = Convert.ToInt32(f2);
                                            }
                                            catch (Exception Ex) { Flusso2G.F2 = -1; }
                                            try
                                            {
                                                f3 = (string)TokenEnergiaAttiva["F3"];
                                                if (f3.Contains(".")) { f3 = f3.Split('.')[0]; }
                                                Flusso2G.F3 = Convert.ToInt32(f3);
                                            }
                                            catch (Exception Ex) { Flusso2G.F3 = -1; }
                                        }
                                    }
                                    catch { }
                                    try
                                    {
                                        JToken TokenEnergiaReattiva;
                                        if (Misure.TryGetValue("R", out TokenEnergiaReattiva))
                                        {
                                            try
                                            {
                                                r1 = (string)TokenEnergiaReattiva["F1"];
                                                if (r1.Contains(".")) { r1 = r1.Split('.')[0]; }
                                                Flusso2G.R1 = Convert.ToInt32(r1);
                                            }
                                            catch (Exception Ex) { Flusso2G.R1 = -1; }
                                            try
                                            {
                                                r2 = (string)TokenEnergiaReattiva["F2"];
                                                if (r2.Contains(".")) { r2 = r2.Split('.')[0]; }
                                                Flusso2G.R2 = Convert.ToInt32(r2);
                                            }
                                            catch (Exception Ex) { Flusso2G.R2 = -1; }
                                            try
                                            {
                                                r3 = (string)TokenEnergiaReattiva["F3"];
                                                if (r3.Contains(".")) { r3 = r3.Split('.')[0]; }
                                                Flusso2G.R3 = Convert.ToInt32(r3);
                                            }
                                            catch (Exception Ex) { Flusso2G.R3 = -1; }
                                        }
                                    }
                                    catch { }
                                }
                                catch (Exception Ex) { }

                                contatore++;
                                records.Add(Flusso2G);
                            }
                            catch { continua = false; }
                        }
                    }
                }
            }
            catch { }
            if (records.Count == 0)
            {
                log.Info("NESSUN RISULTATO TROVATO FLUSSI2G");
            }
            return records;
        }

        public class PP
        {
            public double P1 { get; set; }
            public double P2 { get; set; }
            public double P3 { get; set; }
        }

        public PP GetPotenzaDTFromFlussi2GRow(string UrlRequest)
        {
            PP PotenzaDT = null;
            try
            {
                System.Net.HttpWebRequest request = null;
                int tentativi = 0;
                System.Net.HttpWebResponse risposta = null;
                while (tentativi < 5 && risposta == null)
                {
                    try { request = GenerateRequest(UrlRequest, "", "GET", false, this.Keanu.LoginNEXT, this.Keanu.PassNEXT); }
                    catch { }

                    try
                    {
                        risposta = GetResponse(request);
                        if (risposta == null) tentativi++;
                    }
                    catch (Exception Ex)
                    {
                        log.Error(Ex.Message);
                        tentativi++;
                        Thread.Sleep(120000);
                    }
                }
                var encoding = ASCIIEncoding.ASCII;
                using (var reader = new System.IO.StreamReader(risposta.GetResponseStream(), encoding))
                {
                    string responseText = reader.ReadToEnd();
                    JObject JObjext = JObject.Parse(responseText);
                    PotenzaDT = new PP();

                    PotenzaDT.P1 = Convert.ToInt32((double)JObjext["mapSign"]["LET"]["P"]["F1"]);
                    PotenzaDT.P2 = Convert.ToInt32((double)JObjext["mapSign"]["LET"]["P"]["F2"]);
                    PotenzaDT.P3 = Convert.ToInt32((double)JObjext["mapSign"]["LET"]["P"]["F3"]);
                    PotenzaDT.P1 = (double)JObjext["mapSign"]["LET"]["P"]["F1"];
                    PotenzaDT.P2 = (double)JObjext["mapSign"]["LET"]["P"]["F2"];
                    PotenzaDT.P3 = (double)JObjext["mapSign"]["LET"]["P"]["F3"];
                }
            }
            catch { }
            return PotenzaDT;
        }

        public class MontaggioRow
        {
            public int KA { get; set; }
            public int KP { get; set; }
            public int KR { get; set; }
            public int CifreA { get; set; }
            public int CifreP { get; set; }
            public int CifreR { get; set; }
            public string MatricolaA { get; set; }
            public string MatricolaP { get; set; }
            public string MatricolaR { get; set; }
            public string MotivazioneRettifica { get; set; }
            public string MotivazioneRettificaDesc { get; set; }
        }

        public MontaggioRow GetAllFromFlussi2GRowMontaggio(string UrlRequest)
        {
            MontaggioRow Montaggio = null;
            try
            {
                System.Net.HttpWebRequest request = null;
                int tentativi = 0;
                System.Net.HttpWebResponse risposta = null;
                while (tentativi < 5 && risposta == null)
                {
                    try { request = GenerateRequest(UrlRequest, "", "GET", false, this.Keanu.LoginNEXT, this.Keanu.PassNEXT); }
                    catch { }

                    try
                    {
                        risposta = GetResponse(request);
                        if (risposta == null) tentativi++;
                    }
                    catch (Exception Ex)
                    {
                        log.Error(Ex.Message);
                        tentativi++;
                        Thread.Sleep(120000);
                    }
                }
                var encoding = ASCIIEncoding.ASCII;
                using (var reader = new System.IO.StreamReader(risposta.GetResponseStream(), encoding))
                {
                    string responseText = reader.ReadToEnd();
                    JObject JObjext = JObject.Parse(responseText);
                    Montaggio = new MontaggioRow();

                    Montaggio.KA = (int)JObjext["KA"];
                    Montaggio.KP = (int)JObjext["KP"];
                    Montaggio.KR = (int)JObjext["KR"];
                    Montaggio.CifreA = (int)JObjext["cifreA"];
                    Montaggio.CifreP = (int)JObjext["cifreP"];
                    Montaggio.CifreR = (int)JObjext["cifreR"];
                    try { Montaggio.MatricolaA = (string)JObjext["matricolaA"]; } catch (Exception Ex) { Montaggio.MatricolaA = ""; }
                    try { Montaggio.MatricolaP = (string)JObjext["matricolaP"]; } catch (Exception Ex) { Montaggio.MatricolaP = ""; }
                    try { Montaggio.MatricolaR = (string)JObjext["matricolaR"]; } catch (Exception Ex) { Montaggio.MatricolaR = ""; }
                    try { Montaggio.MotivazioneRettifica = (string)JObjext["motivazioneRettifica"]; } catch (Exception Ex) { Montaggio.MotivazioneRettifica = "02"; }//DEFAULT IS 02 
                    try { Montaggio.MotivazioneRettificaDesc = (string)JObjext["motivazioneRettificaDesc"]; } catch (Exception Ex) { Montaggio.MotivazioneRettificaDesc = ""; }
                }
            }
            catch
            {
                log.Debug("GetAllFromFlussi2GRowMontaggio() fail");
            }
            return Montaggio;
        }

        private System.Net.CookieContainer cookies = new System.Net.CookieContainer();

        public System.Net.HttpWebRequest GenerateRequest(string uri, string content, string method, bool allowAutoRedirect, string username, string password)
        {
            string server = "";
            string dominio = "";

            if (uri == null)
                throw new ArgumentNullException("uri");
            string toAdd = "";
            //if (method == "GET")
            //{
            //    if (uri.Substring(uri.Length - 1, 1) != "?")
            //        toAdd = "?";
            //    toAdd += content;
            //}

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(server + uri + toAdd);

            request.KeepAlive = true;
            request.Method = method;
            request.PreAuthenticate = true;
            request.CookieContainer = cookies;
            request.AllowAutoRedirect = allowAutoRedirect;
            System.Net.NetworkCredential netCredential = new System.Net.NetworkCredential(username, password, "enelint.global");
            request.Credentials = netCredential;
            //request.Credentials = new System.Net.CredentialCache { { new Uri(server + uri), "Negotiate", new System.Net.NetworkCredential(username, password, dominio) } };

            if (method == "POST")
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(content.Replace(' ', '+').Replace(":", "%3A"));
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                request.ContentType = "application/x-www-form-urlencoded";
            }
            return request;
        }

        internal System.Net.HttpWebResponse GetResponse(System.Net.HttpWebRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            System.Net.HttpWebResponse response = null;

            try
            {
                response = (System.Net.HttpWebResponse)request.GetResponse();
                cookies.Add(response.Cookies);
            }
            catch (Exception Ex) { return null; }
            return response;
        }

        public bool SetFlag2G(bool Flag2G, string UrlRequest, string dtDecorrenza2G, string dtMessaRegime2G, string dtPassaggioOrario2G, out string dtDecorrenza2G_, out string dtMessaRegime2G_, out string dtPassaggioOrario2G_)
        {
            dtDecorrenza2G_ = "";
            dtMessaRegime2G_ = "";
            dtPassaggioOrario2G_ = "";

            string ResultGet = "";
            System.Net.HttpWebRequest request = null;
            int tentativi = 0;
            System.Net.HttpWebResponse risposta = null;

            try
            {
                try
                {
                    request = GenerateRequest(UrlRequest, "", "GET", false, this.Keanu.LoginNEXT, this.Keanu.PassNEXT);
                }
                catch { }
                try
                {
                    risposta = GetResponse(request);
                    if (risposta == null) tentativi++;
                }
                catch (Exception Ex)
                {
                    Thread.Sleep(120000);
                }
            }
            catch { }
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(risposta.GetResponseStream(), encoding))
            {
                ResultGet = reader.ReadToEnd();
                log.Debug("Get flag misuratore " + ResultGet);

                JObject JObjext = JObject.Parse(ResultGet);
                dtPassaggioOrario2G_ = (string)JObjext["dtPassaggioOrario2G"];
                log.Debug("Data passaggio 2G " + dtPassaggioOrario2G_);
                if (dtPassaggioOrario2G_ == null) dtPassaggioOrario2G_ = "";

                dtDecorrenza2G_ = (string)JObjext["dtDecorrenza2G"];
                log.Debug("Data decorrenza 2G " + dtDecorrenza2G_);
                if (dtDecorrenza2G_ == null) dtDecorrenza2G_ = "";

                dtMessaRegime2G_ = (string)JObjext["dtDecorrenza2G"];
                if (dtMessaRegime2G_ == null) dtMessaRegime2G_ = "";
                log.Debug("Data messa a regime 2G " + dtMessaRegime2G_);
            }

            dynamic d = JObject.Parse(ResultGet);
            if (!Flag2G)
            {
                d.flg2G = false;
                d.dtDecorrenza2G = "";
                d.dtMessaRegime2G = "";
                d.dtPassaggioOrario2G = "";
                d.codMotivazione2G = "";
            }
            else
            {
                d.flg2G = true;
                d.dtDecorrenza2G = dtDecorrenza2G;
                d.dtMessaRegime2G = dtMessaRegime2G;
                d.dtPassaggioOrario2G = dtPassaggioOrario2G;
                //d.codMotivazione2G = "01";
                d.codMotivazione2G = motivazioneFromUpperSmis;
            }
            try
            {
                var httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://next.enelint.global/api-next-online/api/v1/protected/power/detailTecnica/meters");
                //var httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(UrlRequest);
                httpWebRequest.Method = "PUT";
                System.Net.NetworkCredential netCredential = new System.Net.NetworkCredential(this.Keanu.LoginNEXT, this.Keanu.PassNEXT, "enelint.global");
                httpWebRequest.Credentials = netCredential;
                httpWebRequest.KeepAlive = false;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:60.0) Gecko/20100101 Firefox/60.0";
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string Json = Newtonsoft.Json.JsonConvert.SerializeObject(d);
                    Json = Json.Replace("[", "{").Replace("]", "}").Replace("{{", "{").Replace("}}", "}").Replace("},{", ",");
                    streamWriter.Write(Json);
                    streamWriter.Flush();
                    streamWriter.Close();
                    var httpResponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string responseText = streamReader.ReadToEnd();
                        log.Debug("Responce set flag 2G " + responseText);
                        try
                        {
                            string EsitoJson = "";
                            JObject JObjext = JObject.Parse(responseText);
                            try
                            {
                                EsitoJson = (string)JObjext["id"];
                                log.Debug("Esito riabbina " + EsitoJson);
                            }
                            catch (Exception Ex) { }
                            try
                            {
                                string DescrizioneEsitoJson = (string)JObjext["descrizione"];
                                log.Debug("Descrizione esito riabbina " + DescrizioneEsitoJson);
                            }
                            catch (Exception Ex) { }

                            if (EsitoJson.Equals("MODAL_SUCCESS") || EsitoJson.Equals("MODAL_WARNING"))
                                return true;
                            else
                                return false;
                        }
                        catch (Exception Ex) { }
                    }
                }
            }
            catch (Exception Ex) { }

            return true;
        }
        #endregion
    }
}