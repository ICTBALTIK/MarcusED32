using AgenteHelperLibrary.ScaSrv;
using log4net;
using MARCUS.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SFALibrary;
using SFALibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static MARCUS.Helpers.Constant;
using Smart.Gravity.Model;
using MARCUS.Utils;
using System.Collections.ObjectModel;
using javax.xml.transform;

namespace MARCUS.Marcosi
{
    class EE145
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EE145));
        public Keanu Keanu { get; set; }
        public SqlQuery query = new SqlQuery();

        public EE145(Keanu keanu)
        {
            this.Keanu = keanu;
        }

        public SfaLib sfaLib { get; set; }

        private Cascata riferimentoCorrenteGravity;
        private Cascata riferimentoCopied;
        private GravityApiHelper Grav = new GravityApiHelper();

        public Records.VariablesEE145 v = null;
        public Records.RecordsEE145 r = null;
        public List<Records.RecordsModuliLocal> local = null;

        public bool Iframe { get; set; }

        private bool needToRestart = false;
        private bool aminjus = false;
        private bool presentCodiceFiscale = false;
        private bool presentNumeroCliente = false;
        private bool multiModuloAlreadyDone = false;
        private bool scartoWithoutSiebelTry = false;

        private string multiModuloFattoAttivitaList = "";
        private Cascata riferimentoCopy253Riclass;


        public bool Flow()
        {
            Keanu.KillChromeWebDriver();

            try
            {
                if (!Keanu.PepperYourChrome(Keanu.LoginSFA, Keanu.PassSFA, "https://enelcrmt.my.salesforce.com/", "", true))
                    return false;
                if (!PepperYourSfaLib())
                    return false;

                
                Grav.SetGravityCredential(Keanu.LoginAGENTE, Keanu.PassAGENTE);
                Grav.GravitySetup();
                Grav.GravityConnection(Keanu.LavLoginId);

                switch (Keanu.LavName)
                {
                    case "EE145 - INSERIMENTO FASE 1":
                        {
                            while (true)
                            {
                                Keanu.Timy();

                                if (Keanu.StartStop == false)
                                    break;// TUT VIHODITJ NIKITA NE ZABUDJ, NIKITA ()

                                if (needToRestart)
                                {
                                    log.Info("Restarting");
                                    Keanu.KillChromeWebDriver();
                                    needToRestart = false;
                                    if (!Keanu.PepperYourChrome(Keanu.LoginSFA, Keanu.PassSFA, "https://enelcrmt.my.salesforce.com/", "", true))
                                        return false;
                                    if (!PepperYourSfaLib())
                                        return false;
                                }

                                v = new Records.VariablesEE145();
                                r = new Records.RecordsEE145();

                                try
                                {
                                    riferimentoCorrenteGravity = Grav.GravityScoda();
                                    if (riferimentoCorrenteGravity == null)
                                    {
                                        log.Error("Waiting ~30min");
                                        Thread.Sleep(Keanu.Randy(1000));
                                        needToRestart = true;
                                        continue;
                                    }

                                    v.Attivita = riferimentoCorrenteGravity.Dettaglio;
                                    v.SF = riferimentoCorrenteGravity.NumeroCliente;
                                }
                                catch (Exception Ex)
                                {
                                    log.Error(Ex);
                                    break;
                                }

                                if (Workaholic(v.Attivita))
                                {
                                    try
                                    {
                                        Grav.GravityRegistra(riferimentoCorrenteGravity, v.SF, Keanu.LavRegId);
                                        log.Debug($"{riferimentoCorrenteGravity.Dettaglio} {v.SF} FATTO");
                                    }
                                    catch (Exception)
                                    {
                                        log.Error("Errore durante l'inserimento della lavorazione!");
                                        break;
                                    }

                                    Keanu.Bad.Fatto++;
                                    Keanu.ScartoCount = 0;
                                }
                                else
                                {
                                    if (Keanu.TimeToSospeso == true)
                                    {
                                        Grav.GravitySospendi(2);
                                        Keanu.Bad.Sospeso++;
                                        Keanu.TimeToSospeso = false;
                                    }
                                    else
                                    {
                                        try
                                        {

                                        //Grav.GravitySospendi(2);
                                        //Keanu.Bad.Sospeso++;
                                        //Keanu.TimeToSospeso = false;

                                        //For now sospeso instead of scarto


                                            Grav.GravityRegistra(riferimentoCorrenteGravity, v.SF, Keanu.LavScartoId);
                                            log.Debug($"{riferimentoCorrenteGravity.Dettaglio} {v.SF} SCARTO");
                                        }
                                        catch (Exception)
                                        {
                                            log.Error("Errore durante l'inserimento della lavorazione!");
                                            break;
                                        }

                                        Keanu.Bad.Scarto++;
                                        Keanu.ScartoCount++;
                                        if (Keanu.ScartoCount.Equals(10))
                                        {
                                            log.Error("TOO MUCH SCARTO");
                                            //query.Mark($"{Keanu.LavName} {Keanu.LoginAGENTE} too much scarto :(");
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "EE145 - INSERIMENTO FASE 2":
                        {
                            while (true)
                            {
                                Keanu.Timy();

                                if (Keanu.StartStop == false)
                                    break;

                                if (needToRestart)
                                {
                                    log.Info("Restarting");
                                    Keanu.KillChromeWebDriver();
                                    needToRestart = false;
                                    if (!Keanu.PepperYourChrome(Keanu.LoginSFA, Keanu.PassSFA, "https://enelcrmt.my.salesforce.com/", "", true))
                                        return false;
                                    if (!PepperYourSfaLib())
                                        return false;
                                }

                                v = new Records.VariablesEE145();
                                r = new Records.RecordsEE145();

                                try
                                {
                                    riferimentoCorrenteGravity = Grav.GravityScoda();
                                    if (riferimentoCorrenteGravity == null)
                                    {
                                        log.Error("Waiting ~30min");
                                        Thread.Sleep(Keanu.Randy(1000));
                                        needToRestart = true;
                                        continue;
                                    }

                                    v.Attivita = riferimentoCorrenteGravity.Dettaglio;
                                    v.SF = riferimentoCorrenteGravity.NumeroCliente;
                                }
                                catch (Exception Ex)
                                {
                                    log.Error(Ex);
                                    break;
                                }

                                if (Workaholic(v.Attivita))
                                {
                                    try
                                    {
                                        Grav.GravityRegistra(riferimentoCorrenteGravity, v.SF, Keanu.LavRegId);
                                        log.Debug($"{riferimentoCorrenteGravity.Dettaglio} {v.SF} FATTO");
                                    }
                                    catch (Exception)
                                    {
                                        log.Error("Errore durante l'inserimento della lavorazione!");
                                        break;
                                    }

                                    Keanu.Bad.Fatto++;
                                    Keanu.ScartoCount = 0;
                                }
                                else
                                {
                                    if (Keanu.TimeToSospeso == true)
                                    {
                                        Grav.GravitySospendi(2);
                                        Keanu.Bad.Sospeso++;
                                        Keanu.TimeToSospeso = false;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            Grav.GravityRegistra(riferimentoCorrenteGravity, v.SF, Keanu.LavScartoId);
                                            log.Debug($"{riferimentoCorrenteGravity.Dettaglio} {v.SF} SCARTO");
                                        }
                                        catch (Exception)
                                        {
                                            log.Error("Errore durante l'inserimento della lavorazione!");
                                            break;
                                        }

                                        Keanu.Bad.Scarto++;
                                        Keanu.ScartoCount++;
                                        if (Keanu.ScartoCount.Equals(10))
                                        {
                                            log.Error("TOO MUCH SCARTO");
                                            //query.Mark($"{Keanu.LavName} {Keanu.LoginAGENTE} too much scarto :(");
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "EE145 - NON DI COMPETENZA":
                        {
                            while (true)
                            {
                                Keanu.Timy();

                                if (Keanu.StartStop == false)
                                    break;

                                if (needToRestart)
                                {
                                    log.Info("Restarting");
                                    Keanu.KillChromeWebDriver();
                                    needToRestart = false;
                                    if (!Keanu.PepperYourChrome(Keanu.LoginSFA, Keanu.PassSFA, "https://enelcrmt.my.salesforce.com/", "", true))
                                        return false;
                                    if (!PepperYourSfaLib())
                                        return false;
                                }

                                v = new Records.VariablesEE145();
                                r = new Records.RecordsEE145();
                                local = new List<Records.RecordsModuliLocal>();

                                try
                                {
                                    riferimentoCorrenteGravity = Grav.GravityScoda();
                                    if (riferimentoCorrenteGravity == null)
                                    {
                                        log.Error("Waiting ~30min");
                                        Thread.Sleep(Keanu.Randy(1000));
                                        needToRestart = true;
                                        continue;
                                    }

                                    v.Attivita = riferimentoCorrenteGravity.Dettaglio;
                                    v.SF = riferimentoCorrenteGravity.NumeroCliente;

                                    log.Info("******");
                                    log.Info(v.Attivita);
                                }
                                catch (Exception Ex)
                                {
                                    log.Error(Ex);
                                    break;
                                }

                                if (WorkaholicKompetenzeRiclass(v.Attivita))
                                {
                                    int reso = Keanu.LavRegId;
                                    if (r.Firma.Equals("R"))
                                        reso = Keanu.LavRiclassifica;

                                    try
                                    {
                                        //Grav.GravityRegistra(riferimentoCorrenteGravity, v.SF, Keanu.LavRegId);
                                        Grav.GravityRegistra(riferimentoCorrenteGravity, v.SF, reso);
                                        log.Debug($"{riferimentoCorrenteGravity.Dettaglio} {v.SF} FATTO");
                                    }
                                    catch (Exception)
                                    {
                                        log.Error("Errore durante l'inserimento della lavorazione!");
                                        break;
                                    }

                                    Keanu.Bad.Fatto++;
                                    Keanu.ScartoCount = 0;
                                }
                                else
                                {
                                    if (Keanu.TimeToSospeso == true)
                                    //if (Keanu.TimeToSospeso == true || (r.Firma.Equals("R")))
                                    {

                                        //Multi modulo sospeso 3 for strange docs
                                        //Multi modulo sospeso 4 for in lavorazione or in attesa documents
                                        // Sospeso type 6 - 2 or more modules with riclassifica
                                        Grav.GravitySospendi(Keanu.TimeToSospesoType);
                                        log.Debug($"{riferimentoCorrenteGravity.Dettaglio} SOSPESO");
                                        Keanu.Bad.Sospeso++;
                                        Keanu.TimeToSospeso = false;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            
                                            if(v.AttivitaCopiata == null)  // If new document is fatto, there is no Attivita copy for double scarto - need to duplicate original attivita
                                            {
                                                v.AttivitaCopiata = v.Attivita;
                                            }
                                           

                                            var riferimentoCopy253Riclass = new Cascata()
                                            {
                                                
                                                Dettaglio = v.AttivitaCopiata,
                                                DataRicAcq = Convert.ToDateTime(r.DataInserimento)
                                            };

                                            
                                                Grav.GravityRegistra(riferimentoCopy253Riclass, v.SF, Keanu.LavRiclassifica); //Reso for ee253 riclass (scarto in 2 codas)
                                                log.Debug($"{riferimentoCopy253Riclass.Dettaglio} {v.SF} SCARTO FOR EE253 RICLASS");
                                            
                                            Grav.GravityRegistra(riferimentoCorrenteGravity, v.SF, Keanu.LavScartoId);
                                            log.Debug($"{riferimentoCorrenteGravity.Dettaglio} {v.SF} SCARTO");
                                        }
                                        catch (Exception)
                                        {
                                            log.Error("Errore durante l'inserimento della lavorazione!");
                                            break;
                                        }

                                        Keanu.Bad.Scarto++;
                                        Keanu.ScartoCount++;
                                        if (Keanu.ScartoCount.Equals(10))
                                        {
                                            log.Error("TOO MUCH SCARTO");
                                            //query.Mark($"{Keanu.LavName} {Keanu.LoginAGENTE} too much scarto :(");
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "EE145 - MULTI MODULO":
                        {
                            while (true)
                            {
                                Keanu.Timy();

                                if (Keanu.StartStop == false)
                                    break;

                                if (needToRestart)
                                {
                                    log.Info("Restarting");
                                    Keanu.KillChromeWebDriver();
                                    needToRestart = false;
                                    if (!Keanu.PepperYourChrome(Keanu.LoginSFA, Keanu.PassSFA, "https://enelcrmt.my.salesforce.com/", "", true))
                                        return false;
                                    if (!PepperYourSfaLib())
                                        return false;
                                }

                                v = new Records.VariablesEE145();
                                r = new Records.RecordsEE145();

                                riferimentoCorrenteGravity = Grav.GravityScoda();
                                if (riferimentoCorrenteGravity == null)
                                {
                                    log.Error("Waiting ~30min");
                                    Thread.Sleep(Keanu.Randy(1000));
                                    needToRestart = true;
                                    continue;
                                }

                                v.Attivita = riferimentoCorrenteGravity.Dettaglio;
                                v.SF = riferimentoCorrenteGravity.NumeroCliente;

                                if (!ConfigureMulti())
                                {
                                    if (Keanu.TimeToSospeso == true)
                                    {
                                        Grav.GravitySospendi(2);
                                        Keanu.Bad.Sospeso++;
                                        Keanu.TimeToSospeso = false;
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(v.AttivitaCopiata))
                                        {
                                            v.AttivitaCopiata = "-";
                                        }
                                        try
                                        {
                                            if (aminjus)//FATTO IF CONTAINS A- IN riferimentoCorrente.DETTAGLIO
                                            {
                                                aminjus = false;

                                                try
                                                {
                                                    Grav.GravityRegistra(riferimentoCorrenteGravity, v.AttivitaCopiata, Keanu.LavRegId);
                                                    log.Debug($"{riferimentoCorrenteGravity.Dettaglio} {v.AttivitaCopiata} FATTO");
                                                }
                                                catch (Exception)
                                                {
                                                    log.Error("Errore durante l'inserimento della lavorazione!");
                                                    break;
                                                }

                                                Keanu.Bad.Fatto++;
                                                Keanu.ScartoCount = 0;
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    Grav.GravityRegistra(riferimentoCorrenteGravity, v.AttivitaCopiata, Keanu.LavScartoId);
                                                    log.Debug($"{riferimentoCorrenteGravity.Dettaglio} {v.AttivitaCopiata} SCARTO");
                                                }
                                                catch (Exception)
                                                {
                                                    log.Error("Errore durante l'inserimento della lavorazione!");
                                                    break;
                                                }

                                                Keanu.Bad.Scarto++;
                                                Keanu.ScartoCount++;
                                                if (Keanu.ScartoCount.Equals(10))
                                                {
                                                    log.Error("TOO MUCH SCARTO");
                                                    //query.Mark($"{Keanu.LavName} {Keanu.LoginAGENTE} too much scarto :(");
                                                    break;
                                                }
                                            }
                                        }
                                        catch (Exception Ex)
                                        {
                                            log.Error(Ex);
                                            break;
                                        }
                                    }
                                    continue;//NEED TO CONTINUE WORK IF FATTO/SCARTO WITH BROKEN SF
                                }

                            if(multiModuloAlreadyDone == true) {


                                Grav.GravityRegistra(riferimentoCorrenteGravity, " ", Keanu.LavGiaLavorato);
                                log.Debug($"{riferimentoCorrenteGravity.Dettaglio} GIA LAVORATO");
                                Keanu.Bad.Fatto++;
                                Keanu.ScartoCount = 0;
                                multiModuloAlreadyDone = false;
                                continue;
                            }

                                if (Workaholic(v.AttivitaCopiata))
                                {
                                    try//FATTO
                                    {
                                        Grav.GravityRegistraSpecificForMultiModulo(v.AttivitaCopiata, riferimentoCorrenteGravity, Keanu.LavRegId);
                                        log.Debug($"{v.AttivitaCopiata} {riferimentoCorrenteGravity.Dettaglio} FATTO");
                                    }
                                    catch (Exception)
                                    {
                                        log.Error("Errore durante l'inserimento della lavorazione!");
                                        break;
                                    }

                                    try//GIA LAVORATO
                                    {
                                        Grav.GravityRegistra(riferimentoCorrenteGravity, v.AttivitaCopiata, Keanu.LavGiaLavorato);
                                        log.Debug($"{riferimentoCorrenteGravity.Dettaglio} {v.AttivitaCopiata} GIA LAVORATO");
                                    }
                                    catch (Exception)
                                    {
                                        log.Error("Errore durante l'inserimento della lavorazione!");
                                        break;
                                    }

                                    Keanu.Bad.Fatto++;
                                    Keanu.ScartoCount = 0;
                                }
                                else
                                {
                                    if (Keanu.TimeToSospeso == true)
                                    {
                                        Grav.GravitySospendi(2);
                                        Keanu.Bad.Sospeso++;
                                        Keanu.TimeToSospeso = false;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            Grav.GravityRegistra(riferimentoCorrenteGravity, v.AttivitaCopiata, Keanu.LavScartoId);
                                            log.Debug($"{riferimentoCorrenteGravity.Dettaglio} {v.AttivitaCopiata} SCARTO");
                                        }
                                        catch (Exception)
                                        {
                                            log.Error("Errore durante l'inserimento della lavorazione!");
                                            break;
                                        }

                                        Keanu.Bad.Scarto++;
                                        Keanu.ScartoCount++;
                                        if (Keanu.ScartoCount.Equals(10))
                                        {
                                            log.Error("TOO MUCH SCARTO");
                                            //query.Mark($"{Keanu.LavName} {Keanu.LoginAGENTE} too much scarto :(");
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            catch
            {
                log.Warn($"Flow() fail");
            }

            Keanu.KillChromeWebDriver();
            Grav.GravityConnectionClose();
            return false;
        }

        public bool CopiaAttivita()
        {
            try
            {
                if (!ClickButtonByName("Copia"))
                    return false;

                WaitSpecificPage("Data Inizio");

                GoUP();
                v.AttivitaCopiata = GetFieldValue("Attività", "SPAN_HEADER");
                log.Info(v.AttivitaCopiata);

                if (!ClickButtonByName("Modifica"))
                    return false;

                if (!CompilaCampi("* Stato", "IN LAVORAZIONE", TipoCampo.COMBO, false, false))
                {
                    if (!CompilaCampi("* Status", "IN LAVORAZIONE", TipoCampo.COMBO, false, false))
                    {
                        log.Warn("Problemi nell'impostare lo stato in IN LAVORAZIONE");
                        return false;
                    }
                }

                string podSfa = GetFieldValue("POD");
                if (podSfa.Length < 1)
                    AssociaServizioPod("");

                if (!ClickButtonByName("Salva"))
                    return false;

                int cnt = 0;
                bool modalFind;
                do
                {
                    cnt++;
                    Thread.Sleep(Keanu.Randy(cnt));
                    AttendieConfermaModale("slds-modal__container", "Impossibile modificare", "Barra Atlas", "OK", false, out modalFind);
                    if (modalFind)
                    {
                        if (!ClickButtonByName("Salva"))
                            return false;
                    }
                    Keanu.WaitingGame();
                } while (modalFind && cnt < 15);

                return true;
            }
            catch (Exception Ex)
            {
                return false;
            }
        }

        private bool ConfigureMulti()
        {
            try
            {
                if (!riferimentoCorrenteGravity.Dettaglio.Contains("-"))
                {
                    log.Error("Doesn't contain split symbol '-'");
                    return false;
                }

                if (riferimentoCorrenteGravity.Dettaglio.StartsWith("A-"))
                {
                    aminjus = true;
                    log.Error("Attivita in place where SF should be");
                    return false;
                }

                //var testing = query.GetModuliLocalList(v.SF);

                r = query.GetValuesInserimentoBySF(riferimentoCorrenteGravity.Dettaglio);
                v.SF = riferimentoCorrenteGravity.Dettaglio.Split('-')[0];
                v.NumeroPagina = riferimentoCorrenteGravity.Dettaglio.Split('-')[1];
                log.Info(v.SF);
                log.Info(v.NumeroPagina);

                var multiCount = query.GetMultiRelatedCountDocs(v.SF);
                log.Info("Multi in db - " + multiCount);

                if (v.NumeroPagina.Equals("0"))
                {
                    log.Error("Numero pagina 0");
                    return false;
                }

                var listaDocss = (Documento)sfaLib.SearchGenerico(v.SF, SFALibrary.Helpers.Utility.RicercaDocumentoName);
                List<Attivita> tempAtt = new List<Attivita>();
                tempAtt = (List<Attivita>)sfaLib.GetRelatedListItemsNew(listaDocss.RecordId, SFALibrary.Helpers.Utility.AttivitàItemName, SFALibrary.Helpers.Utility.AttivitàApiName, "Attività");

                List<Attivita> specificAttivita = new List<Attivita>();
                specificAttivita = tempAtt.Where(x => x.Tipo.Equals("DMS - Dati Catastali") && x.Stato.Equals("FATTO")).ToList();

                //foreach(Attivita attivita in specificAttivita) {
                    //multiModuloFattoAttivitaList = multiModuloFattoAttivitaList + ";" + attivita.
                //}

                

                if (specificAttivita.Count == 0)
                    specificAttivita = tempAtt.Where(x => x.Tipo.Equals("DMS - Dati Catastali") && x.Stato.Equals("IN LAVORAZIONE")).ToList();

                int datiCatastaliCount = tempAtt.Where(x => x.Tipo.Equals("DMS - Dati Catastali")).ToList().Count();

                int datiCatastaliFattoCount = tempAtt.Where(x => x.Tipo.Equals("DMS - Dati Catastali") && x.Stato.Equals("FATTO")).ToList().Count();
                int datiCatastaliInLavorazioneCount = tempAtt.Where(x => x.Tipo.Equals("DMS - Dati Catastali") && x.Stato.Equals("IN LAVORAZIONE")).ToList().Count();
                int datiCatastaliInAttesaCount = tempAtt.Where(x => x.Tipo.Equals("DMS - Dati Catastali") && x.Stato.Equals("In attesa")).ToList().Count();
                int datiCatastaliAnnulatoCount = tempAtt.Where(x => x.Tipo.Equals("DMS - Dati Catastali") && x.Stato.Equals("ANNULLATO")).ToList().Count();



                log.Info("DMS - Dati Catastali in SFA - " + datiCatastaliCount);

                //put in sospeso if in sfa more att than in db
                if (datiCatastaliCount >= multiCount) {

                    log.Info("DMS - Dati Catastali in SFA with FATTO - " + datiCatastaliFattoCount);
                    log.Info("DMS - Dati Catastali in SFA with IN LAVORAZIONE - " + datiCatastaliInLavorazioneCount);
                    log.Info("DMS - Dati Catastali in SFA with In Attesa - " + datiCatastaliInAttesaCount);
                    log.Info("DMS - Dati Catastali in SFA with ANNULLATO - " + datiCatastaliAnnulatoCount);

                    if(datiCatastaliAnnulatoCount >= 1) {
                        //Scarto doc
                        log.Info("Scarto because datiCatastaliAnnulatoCount >= 1 - " + datiCatastaliAnnulatoCount);
                        return false;
                    }
                    
                    if(datiCatastaliInAttesaCount >= 1 || datiCatastaliInLavorazioneCount >= 1) {
                        //Sospeso doc
                        log.Info("Scarto because datiCatastaliInAttesaCount or datiCatastaliInLavorazioneCount >= 1");
                        //Keanu.TimeToSospeso = true;
                        //Keanu.TimeToSospesoType = 4;
                        return false;
                    }

                    if(datiCatastaliCount == datiCatastaliFattoCount) {
                        log.Info("Gia lavorato because datiCatastaliCount == datiCatastaliFattoCount");
                        multiModuloAlreadyDone = true;
                        return true;
                        //Gia lavorato doc
                    }

                    //Something strange happened, sospeso to check why
                    log.Info("Sospeso because something strange happened");
                    Keanu.TimeToSospeso = true;
                    Keanu.TimeToSospesoType = 3;
                    return false;
                }

                #region Crea copia
                string idAttivita = specificAttivita[0].RecordId;

                UnlockAndCloseAllTabs();
                 

                
                //relatedAtts = (List<Attivita>)sfaLib.GetRelatedListItemsNew(

                //var testvart = (Documento)sfaLib.SearchGenerico(v.SF, SFALibrary.Helpers.Utility.RicercaDocumentoName);

                //var listaDocss = (Documento)sfaLib.SearchGenerico(v.SF, SFALibrary.Helpers.Utility.RicercaDocumentoName);
                //List<Attivita> tempAtt = new List<Attivita>();
                //tempAtt = (List<Attivita>)sfaLib.GetRelatedListItemsNew(listaDocss.RecordId, SFALibrary.Helpers.Utility.AttivitàItemName, SFALibrary.Helpers.Utility.AttivitàApiName, "Attività");


                //Check related docs


                 Keanu.Driver.Navigate().GoToUrl($"https://enelcrmt.lightning.force.com/lightning/r/wrts_prcgvr__Activity__c/{idAttivita}/view");
                Keanu.WaitingGame();
                WaitSpecificPage("Data Inizio");

                if (!CopiaAttivita())
                {
                    log.Error("CopiaAttivita() fail");
                    return false;
                }
                #endregion

                return true;
            }
            catch
            {
                log.Error("ConfigureMulti() fail");
                return false;
            }
        }

        private bool Workaholic(string documento)
        {
            presentCodiceFiscale = false;
            //presentNumeroCliente = false;

            try
            {
                if (!Keanu.LavName.Equals("EE145 - MULTI MODULO"))
                {
                    log.Info(documento);
                    log.Info(v.SF);

                    //if (r.Firma == null)
                    //{
                    //    log.Info("SQL error");
                    //    return false;
                    //}

                    if (r.Firma == "R")
                    {
                        //IF r.FIRMA == "R" THEN VALUES ARE ALREADY PASSED FROM .53 [BALTIK_DB].[dbo].[MODULI_LOCAL]
                    }
                    else
                    {
                        try
                        {
                            r = query.GetValuesInserimentoBySF(v.SF);
                        }
                        catch
                        {
                            Thread.Sleep(Keanu.Randy(3));
                            r = query.GetValuesInserimentoBySF(v.SF);
                        }
                    }

                    try
                    {
                        if (string.IsNullOrEmpty(r.TipoUnita) &&
                            string.IsNullOrEmpty(r.Sezione) &&
                            string.IsNullOrEmpty(r.Foglio) &&
                            string.IsNullOrEmpty(r.Subalterno) &&
                            string.IsNullOrEmpty(r.Particella) &&
                            string.IsNullOrEmpty(r.EstensioneParticella) &&
                            string.IsNullOrEmpty(r.TipoParticella) &&
                            string.IsNullOrEmpty(r.ImmobiliEsclusi))
                        {
                            log.Error($"MODULO VUOTO");
                        }
                    }
                    catch
                    {
                        log.Info("DB empty");
                        return false;
                    }
                }

                if (string.IsNullOrEmpty(r.CodiceFiscale) && !string.IsNullOrEmpty(r.Piva))
                    r.CodiceFiscale = r.Piva;

                if (string.IsNullOrEmpty(r.CodiceFiscale))
                    log.Debug($"CodiceFiscale null");
                else
                    presentCodiceFiscale = true;

                if (string.IsNullOrEmpty(r.NumeroCliente) || r.NumeroCliente.Trim().Length != 9 || r.NumeroCliente.Equals("999999999"))
                    return false;
                else
                {
                    #region SZ
                    if (r.NumeroCliente.Contains("SZ") || r.NumeroCliente.Contains("SG") || r.NumeroCliente.Contains("KZ") || r.NumeroCliente.Contains("SE") || r.NumeroCliente.Contains("AZ") || r.NumeroCliente.Contains("SH") || r.NumeroCliente.Contains("SK"))
                    {
                        r.SZ = r.NumeroCliente;
                        log.Warn($"{r.SZ} {r.SZ.Substring(0, 2)}NEW");
                        if (!GetNumeroClienteBySZ())
                        {
                            log.Warn($"GetNumeroClienteBySZ() fail");
                            return false;
                        }
                        log.Debug($"{r.SZ} - {r.NumeroCliente}");
                    }
                    #endregion
                }

                #region Stato attività
                Attivita att = new Attivita();
                int cnt = 0;
                do
                {
                    cnt++;
                    Thread.Sleep(Keanu.Randy(cnt));
                    att = sfaLib.SearchAttivita(documento, true);
                } while (att == null && cnt < 30);

                log.Error($"att TRY CNT: {cnt}");

                if (att == null)
                {
                    log.Error("att == null");
                    needToRestart = true;
                    return false;
                }

                if (!att.Stato.Equals("IN LAVORAZIONE"))
                {
                    log.Debug($"{att.Stato} <> IN LAVORAZIONE");
                    return false;
                }

                if (att.AssegnatoA.ToUpper().Equals("BARRA ATLAS"))
                {
                    log.Debug($"AssegnatoA {att.AssegnatoA}");
                    return false;
                }

                log.Info($"{att.Stato}");
                log.Info($"{att.AssegnatoA}");
                log.Info($"{att.Tipo}");
                log.Info($"{att.Causale}");
                log.Info($"{att.Descrizione}");
                log.Info($"{att.Specifica}");

                v.Riklasifi = false;
                v.AlreadyTripper = false;

                if (att.Stato.Contains("IN LAVORAZIONE") &&
                    att.Tipo.ToUpper().Contains("DMS - DATI CATASTALI") &&
                    att.Causale.ToUpper().Contains("GESTIONE CLIENTE") &&
                    att.Descrizione.ToUpper().Contains("DATI CATASTALI") &&
                    att.Specifica.ToUpper().Contains("N/A"))
                {
                    //OK
                    v.AlreadyTripper = true;
                }
                else if (att.Stato.Contains("IN LAVORAZIONE") &&
                         att.Tipo.ToUpper().Contains("DMS - DATI CATASTALI"))
                {
                    //OK, BUT NEED TO RICLASSIFY
                    v.Riklasifi = true;
                }
                else
                    return false;
                #endregion

                if (!SfaOrSiebelDecider())
                    return false;

                UnlockAndCloseAllTabs();

                string idAttivita = att.RecordId;
                log.Info($"Id attività: {idAttivita}");
                Keanu.Driver.Navigate().GoToUrl($"https://enelcrmt.lightning.force.com/lightning/r/wrts_prcgvr__Activity__c/{idAttivita}/view");
                Keanu.WaitingGame();
                WaitSpecificPage("Data Inizio");

                if (r.Worktype.Equals("SIEBEL") || r.Worktype.Equals("SALESFORCE + COMMENTO"))
                {
                    if (!ChiudiAttivita(documento, att))
                        return false;
                    return true;
                }

                if (string.IsNullOrEmpty(r.Quote))
                    return false;

                if (!Modificatore("Gestione Cliente", "Dati Catastali", @"N/A"))
                    return false;

                if (!att.AssegnatoA.Equals(Keanu.LoginSFA))
                {
                    if (!CaricaCampiSearch("COLLEGAMENTO Modifica Assegnato A", Keanu.LoginSFA))
                    {
                        log.Warn("Problemi nell'assegnare la matricola");
                        return false;
                    }
                }

                if (!DeleteClienteSfa())
                {
                    log.Warn("DeleteClienteSfa() fail");
                    return false;
                }

                if (!CaricaCampiSearch("COLLEGAMENTOCliente", r.CodiceAnagraficaCliente))
                {
                    log.Info($"Problemi nell'associare il cliente con {r.CodiceAnagraficaCliente} - proviamo con {r.IdRecordSiebel}");
                    if (!CaricaCampiSearch("COLLEGAMENTOCliente", r.IdRecordSiebel))
                    {
                        log.Error("Problemi nell'associare il cliente");
                        return false;
                    }
                }

                if (!CaricaQuote(r.Quote))
                {
                    log.Warn("CaricaQuote() fail");
                    return false;
                }

                if (!AssociaServizioPod(r.Pod))
                    return false;

                if (!ClickButtonByName("Salva"))
                    return false;

                WaitSpecificPage("Data Inizio");

                if (!Keanu.Driver.FindElements(By.XPath("//button[@data-name='Valida Documentazione']")).First().Enabled)
                {
                    log.Error("Valida Documentazione button not active");
                    return false;
                }

                if (!ClickButtonByName("Valida Documentazione"))
                {
                    if (!ClickButtonByName("Valida Documentazione"))
                        return false;
                }

                SwitchToIframe2("oneAlohaPage");
                bool esito = AttendieConfermaModale("slds-modal__container", "Spiacenti per l'interruzione", "Potrebbe essere sufficiente aggiornarla", "OK", false, out bool modalFind);
                if (!esito && modalFind)
                {
                    log.Error("Problemi nel chiudere la modale 'Spiacenti per l'interruzione'");
                    return false;
                }
                if (modalFind)
                    return false;

                if (!FillFields())
                {
                    SwitchToDefaultContent();
                    return false;
                }

                if (!ClickButtonByName("Indietro", "slds-button slds-button--neutral slds-m-around--small cancelAction"))
                    return false;

                //WaitSpecificPage("Data Inizio");

                Keanu.WaitingGame();
                SwitchToDefaultContent();

                if (Keanu.Driver.PageSource.ToString().Contains("Reindirizzamento a Gestione Appuntamento"))
                {
                    if (!ClickButtonByName("Esci", "slds-button slds-button_brand"))
                        return false;

                    Keanu.WaitingGame();

                    if (!ClickButtonByName("Sì", "slds-button slds-button--neutral "))
                        return false;

                    SwitchToDefaultContent();
                }

                if (!WaitSpecificPage("Data Inizio"))
                {
                    needToRestart = true;
                    return false;
                }

                if (!ChiudiAttivita(documento, att))
                    return false;

                return true;
            }
            catch (Exception Ex)
            {
                log.Error($"General Kenobi");
                log.Info(Ex);
                //Keanu.StartStop = false;
                needToRestart = true;
                return false;
            }
        }

        public bool CaricaQuote(string value)
        {
            bool esito = true;
            try
            {
                IWebElement elementSearch = null;
                try
                {
                    elementSearch = GetSezioneAttiva().FindElement(By.XPath("//input[@class = 'slds-input mouse-pointer ' and @aria-label = '" + "COLLEGAMENTO N. Offerta" + "']"));
                }
                catch
                {
                    elementSearch = GetSezioneAttiva().FindElement(By.XPath("//input[contains(@class, 'slds-input mouse-pointer') and @placeholder = 'Ricerca cliente']"));
                }
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", elementSearch);
                SwitchToDefaultContent();
                Thread.Sleep(Keanu.Randy(2));
                IWebElement modalSearch = GetSezioneAttiva().FindElement(By.XPath(".//div[@class = 'slds-modal__content slds-p-around--medium']"));
                IWebElement inputSearch = modalSearch.FindElement(By.XPath(".//input[@class = 'slds-input input uiInput uiInputText uiInput--default uiInput--input']"));
                inputSearch.Click();
                inputSearch.Clear();
                Thread.Sleep(Keanu.Randy(1));
                inputSearch.SendKeys(value);
                IWebElement buttonClienteSearch = modalSearch.FindElement(By.XPath(".//button[@class = 'slds-button slds-button_brand']"));
                Thread.Sleep(Keanu.Randy(1));
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", buttonClienteSearch);
                Thread.Sleep(Keanu.Randy(1));
                while (IsElementPresent(By.XPath("//span[contains(text(), 'Caricamento in corso. . .')]"), Keanu.Driver))
                {
                    log.Info($"*** Ballz");//BALLSACK
                    Thread.Sleep(Keanu.Randy(1));
                }
                Thread.Sleep(Keanu.Randy(1));
                IWebElement tabella = modalSearch.FindElement(By.XPath(".//table[contains(@class, 'keepTableHeight')]"));
                IList<IWebElement> listaTr = tabella.FindElements(By.XPath("./tbody/tr"));
                int numeroRigheTabella = listaTr.Count();
                if (numeroRigheTabella == 0)
                {
                    try
                    {
                        IWebElement buttonAnnulla = Keanu.Driver.FindElement(By.XPath("//div[@class = 'slds cITA_IFM_LCP206_LookupSearchModal cITA_IFM_LCP205_ActivityLayout']")).FindElement(By.XPath(".//button[text() = 'Annulla']"));
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", buttonAnnulla);
                        SwitchToDefaultContent();
                        return false;
                    }
                    catch { }
                }
                int indexNUmeroContratto = 0;
                int indexQuote = 0;
                bool elementExists = false;
                var lstHeaders = tabella.FindElements(By.XPath("./thead/tr/th"));
                try
                {
                    indexQuote = lstHeaders.IndexOf(lstHeaders.Where(q => q.Text.Contains("QUOTE")).First());
                    indexNUmeroContratto = lstHeaders.IndexOf(lstHeaders.Where(q => q.Text.Contains("NUMERO CONTRATTO")).First());
                }
                catch { }
                IWebElement valoreNUMERO_CONTRATTO = null;
                for (int i = 1; i <= numeroRigheTabella; i++)
                {
                    valoreNUMERO_CONTRATTO = tabella.FindElement(By.XPath("./tbody/tr[" + i + "]")).FindElements(By.XPath("./*")).ElementAt(indexNUmeroContratto);
                    var valoreQUOTE = tabella.FindElement(By.XPath("./tbody/tr[" + i + "]")).FindElements(By.XPath("./*")).ElementAt(indexQuote);
                    if (valoreQUOTE.Text.Equals(value))
                    {
                        var valoreAnchor = valoreNUMERO_CONTRATTO.FindElement(By.XPath(".//a"));
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", valoreAnchor);
                        SwitchToDefaultContent();
                        elementExists = true;
                        break;
                    }
                }
                if (!elementExists)
                    return false;
            }
            catch (Exception Ex)
            {
                log.Warn(Ex.ToString());
                esito = false;
            }
            return esito;
        }

        private bool WorkaholicKompetenzeRiclass(string documento)
        {
            presentCodiceFiscale = false;
            presentNumeroCliente = false;

            try
            {
                r = query.GetValuesInserimentoBySF(v.SF);
            }
            catch
            {
                Thread.Sleep(Keanu.Randy(3));
                r = query.GetValuesInserimentoBySF(v.SF);
            }

            if (r.Firma == null)
            {
                log.Info("SQL error");
                return false;
            }
            else if (r.Firma == "C" || r.Firma == "N")//NON DI COMPETENZA
            {
                log.Info($"Type: {r.Firma}");

                if (string.IsNullOrEmpty(r.CodiceFiscale) && !string.IsNullOrEmpty(r.Piva))
                    r.CodiceFiscale = r.Piva;

                if (string.IsNullOrEmpty(r.CodiceFiscale))
                    log.Debug($"CodiceFiscale null");
                else
                    presentCodiceFiscale = true;

                if (string.IsNullOrEmpty(r.NumeroCliente) || r.NumeroCliente.Trim().Length != 9 || r.NumeroCliente.Equals("999999999"))
                    log.Debug($"NumeroCliente null");
                else
                    presentNumeroCliente = true;

                if (presentNumeroCliente)
                {
                    log.Info($"NC style");

                    #region SZ
                    if (r.NumeroCliente.Contains("SZ") || r.NumeroCliente.Contains("SG") || r.NumeroCliente.Contains("KZ") || r.NumeroCliente.Contains("SE") || r.NumeroCliente.Contains("AZ") || r.NumeroCliente.Contains("SH"))
                    {
                        r.SZ = r.NumeroCliente;
                        log.Warn($"{r.SZ} {r.SZ.Substring(0, 2)}NEW");
                        if (!GetNumeroClienteBySZ())
                        {
                            log.Warn($"GetNumeroClienteBySZ() fail");
                            return false;
                        }
                        log.Debug($"{r.SZ} - {r.NumeroCliente}");
                    }
                    #endregion

                    #region NC SfaLiber
                    ServizioEBene datiSfa = new ServizioEBene();
                    DatiCliente dati = new DatiCliente();
                    SFALibrary.Model.Ricerca.RicercaCliente ric = new SFALibrary.Model.Ricerca.RicercaCliente(true, true, false, false);
                    var listServiziEBeniUtenza = (List<ServizioEBene>)sfaLib.SearchGenerico(r.NumeroCliente, SFALibrary.Helpers.Utility.RicercaServiziEBeniName);
                    datiSfa = listServiziEBeniUtenza.Where(x => x.NumeroUtente.Equals(r.NumeroCliente)).FirstOrDefault();//JUST FOR datiSfa.IdAccount
                    foreach (var item in listServiziEBeniUtenza)
                    {
                        if (item.NumeroUtente.Equals(r.NumeroCliente) && !string.IsNullOrEmpty(item.PodPdr))
                        {
                            if (!item.Stato.Contains("Disattiv"))
                            {
                                r.Pod = item.PodPdr;
                                break;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(r.Pod))
                    {
                        log.Error($"POD VUOTO/DISATTIVATO/DISATTIVATA");
                        r.Pod = "";
                    }
                    try
                    {
                        dati = (DatiCliente)sfaLib.GetDatiClienti(datiSfa.IdAccount, ric);
                    }
                    catch
                    {
                        log.Error($"Nessun risultato per {r.CodiceFiscale} in Clienti");
                        presentCodiceFiscale = false;
                    }
                    int result = Keanu.CompareTo(r.CodiceFiscale, dati.CodiceFiscaleTestata);
                    if (result.Equals(0))
                    {
                        //CF OK OR EMPTY
                    }
                    else if (result <= 2)
                    {
                        log.Warn($"A LITTLE BIT WRONG CF");
                        log.Warn(result);
                        //r.CodiceFiscale = dati.CodiceFiscaleTestata;
                    }
                    else//SCARTO
                    {
                        log.Error($"WRONG CF");
                        log.Error(result);
                        presentCodiceFiscale = false;
                    }

                    if (!string.IsNullOrEmpty(dati.CodiceAnagraficaCliente))
                        r.CodiceAnagraficaCliente = dati.CodiceAnagraficaCliente;
                    if (!string.IsNullOrEmpty(dati.IdRecordSiebel))
                        r.IdRecordSiebel = dati.IdRecordSiebel;
                    if (!string.IsNullOrEmpty(dati.CodiceFiscaleTestata))
                        r.CodiceFiscale = dati.CodiceFiscaleTestata;
                    if (!string.IsNullOrEmpty(dati.PartitaIvaTestata))
                        r.PartitaIvaTestata = dati.PartitaIvaTestata;
                    log.Debug($"POD: {r.Pod}");
                    log.Debug($"Codice Anagrafica Cliente: {r.CodiceAnagraficaCliente}");
                    log.Debug($"Id Record Siebel: {r.IdRecordSiebel}");
                    log.Debug($"Codice Fiscale: {r.CodiceFiscale}");
                    log.Debug($"Partita Iva: {r.PartitaIvaTestata}");
                    #endregion
                }
                else if (presentCodiceFiscale)
                {
                    log.Info($"CF style");

                    #region CF SfaLiber
                    //ServizioEBene datiSfa = new ServizioEBene();
                    DatiCliente dati = new DatiCliente();
                    SFALibrary.Model.Ricerca.RicercaCliente ric = new SFALibrary.Model.Ricerca.RicercaCliente(true, true, false, false);
                    var listCliente = (List<Cliente>)sfaLib.SearchGenerico(r.CodiceFiscale, SFALibrary.Helpers.Utility.RicercaClientiName);
                    try
                    {
                        dati = (DatiCliente)sfaLib.GetDatiClienti(listCliente[0].RecordId, ric);
                    }
                    catch
                    {
                        log.Error($"Nessun risultato per {r.CodiceFiscale} in Clienti");
                        presentCodiceFiscale = false;
                    }
                    int result = Keanu.CompareTo(r.CodiceFiscale, dati.CodiceFiscaleTestata);
                    if (result.Equals(0))
                    {
                        //CF OK OR EMPTY
                    }
                    else if (result <= 2)
                    {
                        log.Warn($"A LITTLE BIT WRONG CF");
                        log.Warn(result);
                        //r.CodiceFiscale = dati.CodiceFiscaleTestata;
                    }
                    else
                    {
                        log.Error($"WRONG CF");
                        log.Error(result);
                        presentCodiceFiscale = false;
                    }

                    if (!string.IsNullOrEmpty(dati.CodiceAnagraficaCliente))
                        r.CodiceAnagraficaCliente = dati.CodiceAnagraficaCliente;
                    if (!string.IsNullOrEmpty(dati.IdRecordSiebel))
                        r.IdRecordSiebel = dati.IdRecordSiebel;
                    if (!string.IsNullOrEmpty(dati.CodiceFiscaleTestata))
                        r.CodiceFiscale = dati.CodiceFiscaleTestata;
                    if (!string.IsNullOrEmpty(dati.PartitaIvaTestata))
                        r.PartitaIvaTestata = dati.PartitaIvaTestata;
                    log.Debug($"POD: {r.Pod}");
                    log.Debug($"Codice Anagrafica Cliente: {r.CodiceAnagraficaCliente}");
                    log.Debug($"Id Record Siebel: {r.IdRecordSiebel}");
                    log.Debug($"Codice Fiscale: {r.CodiceFiscale}");
                    log.Debug($"Partita Iva: {r.PartitaIvaTestata}");
                    #endregion
                }
                else
                {
                    log.Info($"Empty style");
                }

                try
                {
                    #region Stato attività
                    Attivita att = new Attivita();
                    int cnt = 0;
                    do
                    {
                        cnt++;
                        Thread.Sleep(Keanu.Randy(cnt));
                        att = sfaLib.SearchAttivita(documento, true);
                    } while (att == null && cnt < 30);

                    log.Error($"att TRY CNT: {cnt}");

                    if (att == null)
                    {
                        log.Error("att == null");
                        needToRestart = true;
                        return false;
                    }

                    if (!att.Stato.Equals("IN LAVORAZIONE"))
                    {
                        log.Debug($"{att.Stato} <> IN LAVORAZIONE");
                        return false;
                    }

                    if (att.AssegnatoA.ToUpper().Equals("BARRA ATLAS"))
                    {
                        log.Debug($"AssegnatoA {att.AssegnatoA}");
                        return false;
                    }

                    log.Info($"{att.Stato}");
                    log.Info($"{att.AssegnatoA}");
                    log.Info($"{att.Tipo}");
                    log.Info($"{att.Causale}");
                    log.Info($"{att.Descrizione}");
                    log.Info($"{att.Specifica}");

                    v.Riklasifi = false;
                    v.AlreadyTripper = false;

                    if (att.Stato.Contains("IN LAVORAZIONE") &&
                        att.Tipo.ToUpper().Contains("DMS - DATI CATASTALI") &&
                        att.Causale.ToUpper().Contains("GESTIONE CLIENTE") &&
                        att.Descrizione.ToUpper().Contains("DATI CATASTALI") &&
                       (att.Specifica.ToUpper().Contains("N/A") || att.Specifica.ToUpper().Contains("NA")))  // Ja tripletaa Causale ir N/A , skatas vai ir NA arii
                    {
                        //OK
                        v.AlreadyTripper = true;
                    }
                    else if (att.Stato.Contains("IN LAVORAZIONE") &&
                             att.Tipo.ToUpper().Contains("DMS - DATI CATASTALI") &&
                             (att.Causale.ToUpper().Contains("N/A") || att.Causale.ToUpper().Contains("NA")))  // Ja tripletaa Causale ir N/A , skatas vai ir NA arii
                    {
                        //OK, BUT NEED TO RICLASSIFY
                        v.Riklasifi = true;
                    }
                    else
                        return false;
                    #endregion

                    UnlockAndCloseAllTabs();

                    string idAttivita = att.RecordId;
                    log.Info($"Id attività: {idAttivita}");
                    Keanu.Driver.Navigate().GoToUrl($"https://enelcrmt.lightning.force.com/lightning/r/wrts_prcgvr__Activity__c/{idAttivita}/view");
                    Keanu.WaitingGame();
                    WaitSpecificPage("Data Inizio");

                    if (!Modificatore("Gestione Cliente", "Dati Catastali", @"N/A"))
                        return false;

                    DeleteSht("customerLookUp");
                    DeleteSht("quoteIdLookUp");
                    DeleteSht("caseItemLookUp");
                    DeleteSht("configItemLookUp");

                    if (presentCodiceFiscale)
                    {
                        if (!CaricaCampiSearch("COLLEGAMENTOCliente", r.CodiceAnagraficaCliente))
                        {
                            log.Info($"Problemi nell'associare il cliente con {r.CodiceAnagraficaCliente} - proviamo con {r.IdRecordSiebel}");
                            if (!CaricaCampiSearch("COLLEGAMENTOCliente", r.IdRecordSiebel))
                            {
                                log.Error("Problemi nell'associare il cliente");
                                return false;
                            }
                        }
                    }

                    if (!SetCommento("Documentazione priva di dati catastale/non di competenza."))
                    {
                        log.Warn("SetCommento() fail");
                        return false;
                    }

                    if (!AssociaServizioPod(r.Pod))
                        return false;

                    if (!CompilaCampi("* Stato", "FATTO", TipoCampo.COMBO, false, false))
                    {
                        if (!CompilaCampi("* Status", "FATTO", TipoCampo.COMBO, false, false))
                        {
                            log.Warn("Problemi nell'impostare lo stato in FATTO");
                            return false;
                        }
                    }

                    if (!ClickButtonByName("Salva"))
                        return false;

                    WaitSpecificPage("Data Inizio");
                }
                catch (Exception Ex)
                {
                    log.Error($"General Kenobi");
                    log.Info(Ex);
                    needToRestart = true;
                    return false;
                }
            }
            else if (r.Firma == "R")//RICLASSIFICA
            {
                log.Info($"Type: {r.Firma}");

                //v.SF = "SMALX52015F5";

                var listaDocss = (Documento)sfaLib.SearchGenerico(v.SF, SFALibrary.Helpers.Utility.RicercaDocumentoName);
                List<Attivita> tempAtt = new List<Attivita>();
                tempAtt = (List<Attivita>)sfaLib.GetRelatedListItemsNew(listaDocss.RecordId, SFALibrary.Helpers.Utility.AttivitàItemName, SFALibrary.Helpers.Utility.AttivitàApiName, "Attività");

                //Check all of the docs

                

                /*
                List<Attivita> specificAttivita = new List<Attivita>();
                specificAttivita = tempAtt.Where(x => x.Tipo.Equals("DMS - Dati Catastali")).ToList();
                foreach (var item in specificAttivita)
                {
                    if (!item.Tipo.Equals("DMS - Dati Catastali"))
                    {
                        log.Error($"Tipo: {item.Tipo}");
                        return false;
                    }
                    else
                    {
                        continue;
                    }
                }
                */

                local = query.GetModuliLocalList(v.SF);
                
                
                
                if (local.Count.Equals(0))
                {
                    //Fatto because no need to update att. Just send to EE253 operators for manual work
                    

                    DoRiclassifica(documento);

                    /*
                    if (tempAtt.Count == 1) {

                        if (tempAtt[0].Tipo == "DMS - Dati Catastali") {
                            log.Info($"Only riclassifica - 1 dati catastali");
                            //TO DO CHECK RIK
                            if (!DoRiclassifica(documento))
                                return false;
                        }else if(tempAtt[0].Tipo.Contains("DMS - ")) {
                            log.Info($"Already riclassifica. Gia lavorato - 1 dms");
                            Keanu.TimeToSospeso = true;
                            Keanu.TimeToSospesoType = 7;
                            return false;
                            //For now sospeso, because reso is not ready
                        } else {
                            log.Info($"Only riclassifica - 1 not dms at all");
                            //TO DO CHECK RIK
                            if (!DoRiclassifica(documento))
                                return false;
                        }
                        
                    }
                    */
                    /*
                    if(tempAtt.Count >= 2) {

                        Attivita att = new Attivita();
                        int cnt = 0;
                        do {
                            cnt++;
                            Thread.Sleep(Keanu.Randy(cnt));
                            att = sfaLib.SearchAttivita(documento, true);
                        } while (att == null && cnt < 30);

                        log.Error($"att TRY CNT: {cnt}");

                        if (att == null) {
                            log.Error("att == null");
                            //needToRestart = true;
                            return false;
                        }

                        int dmsCount = 0;
                        int datiCatastaliCount = 0;

                        foreach (var attivita in tempAtt) {
                            if (attivita.Tipo == "DMS - Dati Catastali") {
                                datiCatastaliCount++;
                            }
                            if(attivita.Tipo.Contains("DMS - ") && attivita.Tipo != "DMS - Dati Catastali") {
                                dmsCount++;
                            }
                        }

                        if(att.Tipo == "DMS - Dati Catastali") {
                            if(dmsCount == 0) {
                                log.Info($"Only riclassifica - no dms");
                                if (!DoRiclassifica(documento))
                                    return false;
                            } else {
                                log.Info($"Already with riclassifica - 1+ dms");
                                return false;
                            }
                        }

                        if(att.Tipo.Contains("DMS - ") && att.Tipo != "DMS - Dati Catastali") {
                            log.Info($"Already riclassifica. Scarto - 1+ dms");
                            return false;
                        } else {
                            log.Info($"Not dms at all - trying riclassifica");
                            if (!DoRiclassifica(documento))
                                return false;
                        }

                    }
                    */
                    /*
                    foreach (var attivita in tempAtt) {

                        if (attivita.Tipo == "DMS - Dati Catastali") {
                            continue;
                        }

                        if (attivita.Tipo.Contains("DMS - ")) {
                            log.Error($"Tipo: {attivita.Tipo} - already with RICLASSIFICA - scarto");
                            return false;
                        }

                    }
                    */


                    
                }
                else
                {
                    log.Info($"Riclassifica + catastali in DB {local.Count}");


                    // Proverka na 2 i bolshe modulja, eti sluchai zakidivatj  v suspeso
                    if (local.Count >= 2)
                    {
                        Keanu.Bad.Scarto++;
                        Keanu.ScartoCount++;
                        return false;

                    }

                        #region Stato attività
                        Attivita att = new Attivita();
                    int cnt = 0;
                    do
                    {
                        cnt++;
                        Thread.Sleep(Keanu.Randy(cnt));
                        att = sfaLib.SearchAttivita(documento, true);
                    } while (att == null && cnt < 30);

                    log.Error($"att TRY CNT: {cnt}");

                    if (att == null)
                    {
                        log.Error("att == null");
                        //needToRestart = true;
                        return false;
                    }

                    if (!att.Stato.Equals("IN LAVORAZIONE"))
                    {
                        log.Debug($"{att.Stato} <> IN LAVORAZIONE");
                        return false;
                    }

                    if (att.AssegnatoA.ToUpper().Equals("BARRA ATLAS"))
                    {
                        log.Debug($"AssegnatoA {att.AssegnatoA}");
                        return false;
                    }

                    log.Info($"{att.Stato}");
                    log.Info($"{att.AssegnatoA}");
                    log.Info($"{att.Tipo}");
                    log.Info($"{att.Causale}");
                    log.Info($"{att.Descrizione}");
                    log.Info($"{att.Specifica}");

                    v.Riklasifi = false;
                    v.AlreadyTripper = false;

                    /*
                    if(att.Tipo != "DMS - Dati Catastali") {
                        log.Info($"Attivita stato not DMS - Dati Catastali. Sospeso for now");
                        Keanu.TimeToSospesoType = 5;
                        Keanu.TimeToSospeso = true;
                        return false;
                    }

                    if(tempAtt.Count >= 2) {
                        log.Info($"2 or more connected att. Sospeso for now");
                        Keanu.TimeToSospesoType = 5;
                        Keanu.TimeToSospeso = true;
                        return false;
                    }
                    */

                    //Change here from 147 to dati catastali
                    /*
                    if (att.Stato.Contains("IN LAVORAZIONE") &&
                        att.Tipo.ToUpper().Contains("DMS - Dati Catastali") &&
                        att.Causale.ToUpper().Contains("N/A") &&
                        att.Descrizione.ToUpper().Contains("N/A") &&
                        att.Specifica.ToUpper().Contains("N/A"))
                    {
                        v.AlreadyTripper = true;
                    }
                    else
                    {
                        v.Riklasifi = true;
                    }
                    */
                    #endregion

                    bool isMainAttClosed = false;

                    foreach (var item in local)
                    {
                        #region Pass between local to r
                        r = new Records.RecordsEE145
                        {
                            NumeroCliente = item.NUMERO_CLIENTE,
                            Qualifica = item.QUALIFICA,
                            ComuneCatastale = item.COMUNE_CATASTALE,
                            TipoUnita = item.TIPO_UNITA,
                            Foglio = item.FOGLIO,
                            EstensioneParticella = item.ESTENSIONE_PARTICELLA,
                            Subalterno = item.SUBALTERNO,
                            Firma = item.FIRMA,
                            ImmobiliEsclusi = item.IMMOBILI_ESCLUSI,
                            ComuneAmministrativo = item.COMUNE_AMMINISTRATIVO,
                            CodiceComuneCatastale = item.CODICE_COMUNE_CATASTALE,
                            Sezione = item.SEZIONE,
                            Particella = item.PARTICELLA,
                            TipoParticella = item.TIPO_PARTICELLA,
                            Data = item.DATA,
                            DataInserimento = item.DATA_INSERIMENTO,
                            //r.Quote = item.
                            //r.NumeroContratto = item.
                            CodiceFiscale = item.CODICE_FISCALE,
                            //r.CodiceIdSiebel = item.
                            //r.Cliente = item.
                            //r.ClienteFromSFALibraryCheck = item.
                            //r.CaseItem = item.
                            //r.ConfigurationItem = item.
                            //r.AttivitaServiziBeni = item.
                            //r.Oggetto = item.
                            //r.Testo = item.
                            //r.Pod = item.
                            Piva = item.PARTITA_IVA,
                            ComuneDomicilio = item.COMUNE_DOMICILIO,
                            ProvinciaDomicilio = item.PROVINCIA_DOMICILIO,
                            ComuneSede = item.COMUNE_SEDE,
                            ProvinciaSede = item.PROVINCIA_SEDE,
                            TopomasticaFornitura = item.TOPONOMASTICA_FORNITURA,
                            NomeViaFornitura = item.NOME_VIA_FORNITURA,
                            CivicoFornitura = item.CIVICO_FORNITURA,
                            LocalitaFornitura = item.LOCALITA_FORNITURA,
                            ComuneFornitura = item.COMUNE_FORNITURA,
                            CapComuneFornitura = item.CAP_COMUNE_FORNITURA,
                            IdUser = item.ID_USER.ToString()
                            //r.Worktype = item.
                            //r.AttivitaPerCommento = item.
                            //r.Act = item.
                            //r.SZ = item.
                            //r.CodiceAnagraficaCliente = item.
                            //r.IdRecordSiebel = item.
                            //r.CodiceFiscaleTestata = item.
                            //r.PartitaIvaTestata = item.
                            //r.TipologiaCliente = item.
                        };
                        #endregion

                        UnlockAndCloseAllTabs();

                        string idAttivita = att.RecordId;
                        log.Info($"Id attività: {idAttivita}");
                        Keanu.Driver.Navigate().GoToUrl($"https://enelcrmt.lightning.force.com/lightning/r/wrts_prcgvr__Activity__c/{idAttivita}/view");
                        Keanu.WaitingGame();
                        WaitSpecificPage("Data Inizio");


                        //Need to take main att for the first module and after seconed need to create copy of main att
                        if (isMainAttClosed)
                        {
                            if (!CopiaAttivita())
                            {
                                log.Error("CopiaAttivita() fail");
                                return false;
                            }
                        }
                        else
                        {
                            v.AttivitaCopiata = v.Attivita;
                            isMainAttClosed = true;
                        }

                        if (Workaholic(v.AttivitaCopiata))
                        {
                            riferimentoCopied = new Cascata()
                            {
                                Dettaglio = v.AttivitaCopiata,
                                DataRicAcq = Convert.ToDateTime(r.DataInserimento)
                            };

                            try
                            {
                                Grav.GravityRegistra(riferimentoCopied, v.SF, 47540);//Gestione Documenti non di competenza/da Riclassificare (con Macro) # FATTO – Confermato
                                log.Debug($"{riferimentoCopied.Dettaglio} {v.SF} FATTO COPY");
                            }
                            catch (Exception)
                            {
                                log.Error("Errore durante l'inserimento della lavorazione!");
                                return false;
                            }

                            Keanu.Bad.Fatto++;
                            Keanu.ScartoCount = 0;
                        }
                        else
                        {
                            //Here to register scarto module into ee253 operatore coda
                            log.Error($"AttivitaCopiata {v.AttivitaCopiata} fail");
                            return false;
                        }
                    }

                    if (!DoRiclassifica(documento))
                        return false;
                }
            }
            else
            {
                log.Info($"Unknown type: {r.Firma}");
                Keanu.TimeToSospesoType = 2;
                Keanu.TimeToSospeso = true;
                return false;
            }

            return true;
        }

        private bool DoRiclassifica(string documento)
        {
            //Register with new reso for ee253 operatore

            //Return true at the start because np need to change att
            return true;

            #region Stato attività
            Attivita att = new Attivita();
            int cnt = 0;
            do
            {
                cnt++;
                Thread.Sleep(Keanu.Randy(cnt));
                att = sfaLib.SearchAttivita(documento, true);
            } while (att == null && cnt < 30);

            log.Error($"att TRY CNT: {cnt}");

            if (att == null)
            {
                log.Error("att == null");
                //needToRestart = true;
                return false;
            }

            if (!att.Stato.Equals("IN LAVORAZIONE"))
            {
                log.Debug($"{att.Stato} <> IN LAVORAZIONE");
                return false;
            }

            if (att.AssegnatoA.ToUpper().Equals("BARRA ATLAS"))
            {
                log.Debug($"AssegnatoA {att.AssegnatoA}");
                return false;
            }

            log.Info($"{att.Stato}");
            log.Info($"{att.AssegnatoA}");
            log.Info($"{att.Tipo}");
            log.Info($"{att.Causale}");
            log.Info($"{att.Descrizione}");
            log.Info($"{att.Specifica}");

            v.Riklasifi = false;
            v.AlreadyTripper = false;


            //Check for RICLASSIFICA Dati catastali
            if (att.Stato.Contains("IN LAVORAZIONE") &&
                att.Tipo.ToUpper().Contains("DMS - Dati Catastali") &&
                att.Causale.ToUpper().Contains("N/A") &&
                att.Descrizione.ToUpper().Contains("N/A") &&
                att.Specifica.ToUpper().Contains("N/A"))
            {
                v.AlreadyTripper = true;
            }
            else
            {
                v.Riklasifi = true;
            }
            #endregion

            UnlockAndCloseAllTabs();

            string idAttivita = att.RecordId;
            log.Info($"Id attività: {idAttivita}");
            Keanu.Driver.Navigate().GoToUrl($"https://enelcrmt.lightning.force.com/lightning/r/wrts_prcgvr__Activity__c/{idAttivita}/view");
            Keanu.WaitingGame();
            WaitSpecificPage("Data Inizio");

            try
            {
                IWebElement attHeader = Keanu.Driver.FindElement(By.XPath("//div[@class = 'slds-page-header titleArea'] "));
                if (attHeader.Text.Contains("Modifica"))
                    ClickButtonByName("Modifica");
                else if (attHeader.Text.Contains("Annulla"))
                    log.Info($"Already Annulla");
                else
                    return false;

                if (v.Riklasifi && !v.AlreadyTripper)
                {
                    if (!Riclassifaktorz())
                    {
                        log.Warn($"Riclassifaktorz() fail");
                        log.Warn($"Probabilmente not main attivita");
                        return false;
                    }
                    //Change here to dati catastali
                    if (!CompilaCampi("* Tipo attività", "DMS - Dati Catastali", Constant.TipoCampo.COMBO))
                        return false;

                    if (!SetTripletta(@"N/A", @"N/A", @"N/A"))
                    {
                        log.Warn($"SetTripletta() fail");
                        return false;
                    }

                    v.AlreadyTripper = true;

                    GoUP();
                    string podSfa = GetFieldValue("POD");
                    if (podSfa.Length < 1)
                        AssociaServizioPod("");

                   //if (!ClickButtonByName("Conferma e Riassegna"))
                   if (!ClickButtonByName("Conferma"))
                        return false;

                    Keanu.WaitingGame();

                    bool esito = AttendieConfermaModale("slds-modal__container", "Unexpeted error", "", "OK", false, out bool modalFind);
                    if (!esito && modalFind)
                    {
                        log.Error("Problemi nel chiudere la modale 'Unexpeted error'");
                        return false;
                    }
                }
            }
            catch
            {
                log.Warn($"Riclassifica fail");
                return false;
            }
            return true;
        }

        private bool ChiudiAttivita(string documento, Attivita att)
        {
            try
            {
                int cnt = 0;
                while (!Keanu.Driver.PageSource.ToString().Contains(documento) && cnt < 15)
                {
                    Thread.Sleep(Keanu.Randy(1));
                    cnt++;
                }

                if (!Modificatore("Gestione Cliente", "Dati Catastali", @"N/A"))
                    return false;

                if (r.Worktype.Equals("SIEBEL") || r.Worktype.Equals("SALESFORCE + COMMENTO"))
                {
                    if (!CaricaCampiSearch("COLLEGAMENTOCliente", r.CodiceAnagraficaCliente))
                    {
                        log.Info($"Problemi nell'associare il cliente con {r.CodiceAnagraficaCliente} - proviamo con {r.IdRecordSiebel}");
                        if (!CaricaCampiSearch("COLLEGAMENTOCliente", r.IdRecordSiebel))
                        {
                            log.Error("Problemi nell'associare il cliente");
                            return false;
                        }
                    }

                    if (!att.AssegnatoA.Equals(Keanu.LoginSFA))
                    {
                        if (!CaricaCampiSearch("COLLEGAMENTO Modifica Assegnato A", Keanu.LoginSFA))
                        {
                            log.Error("Problemi nell'assegnare la matricola");
                            return false;
                        }
                    }
                }

                SetCommento("");

                if (!AssociaServizioPod(r.Pod))
                    return false;

                if (!v.AlreadyTripper)
                {
                    if (!SetTripletta("Gestione Cliente", "Dati Catastali", @"N/A"))
                    {
                        log.Warn($"SetTripletta() fail");
                        return false;
                    }
                }

                if (r.Worktype.Equals("SALESFORCE + COMMENTO"))
                    SetCommento($"gia' gestito con: {r.AttivitaPerCommento}");

                if (!CompilaCampi("* Status", "FATTO", TipoCampo.COMBO, false, false))
                {
                    if (!CompilaCampi("* Stato", "FATTO", TipoCampo.COMBO, false, false))
                    {
                        log.Warn("Problemi nell'impostare lo stato in FATTO");
                        return false;
                    }
                }

                //#region EXTRA CHECK CLIENTE BEFORE SALVA
                //if (!r.TipologiaCliente.ToUpper().Equals("CASA"))
                //{
                //    //string partitaIvaAssociato = GetFieldValue(" Partita IVA");
                //    string partitaIvaAssociato = GetFieldValue(" Value Added Tax (VAT)");
                //    //log.Debug($"Partita IVA associata all'attività: {partitaIvaAssociato}");
                //    if (!partitaIvaAssociato.ToUpper().Equals(r.PartitaIvaTestata.ToUpper()))
                //    {
                //        if (!CaricaCampiSearch("COLLEGAMENTOCliente", r.CodiceAnagraficaCliente))
                //        {
                //            log.Info($"Problemi nell'associare il cliente con {r.CodiceAnagraficaCliente} - proviamo con {r.IdRecordSiebel}");
                //            if (!CaricaCampiSearch("COLLEGAMENTOCliente", r.IdRecordSiebel))
                //            {
                //                log.Error("Problemi nell'associare il cliente");
                //                return false;
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    //string codiceFiscaleAssociato = GetFieldValue(" Codice Fiscale");
                //    string codiceFiscaleAssociato = GetFieldValue(" National Insurance Number (NIN)");
                //    //log.Debug($"Codice Fiscale associato all'attività: {codiceFiscaleAssociato}");
                //    if (!codiceFiscaleAssociato.ToUpper().Equals(r.CodiceFiscale.ToUpper()))
                //    {
                //        if (!CaricaCampiSearch("COLLEGAMENTOCliente", r.CodiceAnagraficaCliente))
                //        {
                //            log.Info($"Problemi nell'associare il cliente con {r.CodiceAnagraficaCliente} - proviamo con {r.IdRecordSiebel}");
                //            if (!CaricaCampiSearch("COLLEGAMENTOCliente", r.IdRecordSiebel))
                //            {
                //                log.Error("Problemi nell'associare il cliente");
                //                return false;
                //            }
                //        }
                //    }
                //}
                //#endregion

                if (!ClickButtonByName("Salva"))
                    return false;

                WaitSpecificPage("Data Inizio");

                if (Keanu.Driver.FindElement(By.XPath("//button[. = 'Valida Documentazione']")).Enabled)
                {
                    log.Error("Valida Documentazione is still active");
                    return false;
                }

                #region LAST CHECK IF EVERYTHING IS OK
                cnt = 0;
                do
                {
                    cnt++;
                    Thread.Sleep(Keanu.Randy(cnt));
                    att = sfaLib.SearchAttivita(documento, true);
                } while (att == null && cnt < 30);

                log.Error($"att TRY CNT: {cnt}");

                if (att == null)
                {
                    log.Error("att == null");
                    //needToRestart = true;
                    return false;
                }

                //if (!r.TipologiaCliente.ToUpper().Equals("CASA"))
                //{
                //    //string partitaIvaAssociato = GetFieldValue(" Partita IVA");
                //    string partitaIvaAssociato = GetFieldValue(" Value Added Tax (VAT)");
                //    log.Debug($"Partita IVA associata all'attività: {partitaIvaAssociato}");
                //    if (!partitaIvaAssociato.ToUpper().Equals(r.PartitaIvaTestata.ToUpper()))
                //    {
                //        log.Error("Last check fail");
                //        return false;
                //    }
                //}
                //else
                //{
                //    //string codiceFiscaleAssociato = GetFieldValue(" Codice Fiscale");
                //    string codiceFiscaleAssociato = GetFieldValue(" National Insurance Number (NIN)");
                //    log.Debug($"Codice Fiscale associato all'attività: {codiceFiscaleAssociato}");
                //    if (!codiceFiscaleAssociato.ToUpper().Equals(r.CodiceFiscale.ToUpper()))
                //    {
                //        log.Error("Last check fail");
                //        return false;
                //    }
                //}

                string statoFinale = att.Stato;
                log.Debug($"Stato finale: {statoFinale}");
                if (!statoFinale.Trim().ToUpper().Equals("FATTO"))
                {
                    log.Error("Stato finale diverso da FATTO");
                    return false;
                }
                else
                    return true;
                #endregion
            }
            catch (Exception Ex)
            {
                log.Warn(Ex.Message);
                needToRestart = true;
                return false;
            }
        }

        private bool SfaOrSiebelDecider()
        {
            #region
            if (!sfaLib.RicercaAnagraficaCliente("", r.NumeroCliente, out Offerta offertaSfa, out ServizioEBene utenzaSfa, out string errore))
            {
                log.Info($"Nessun risultato per {r.NumeroCliente} in Servizi e Beni");
                return false;
            }
            r.Cliente = utenzaSfa.NomeCliente;
            #endregion

            UnlockAndCloseAllTabs();


            /*
            if (!RicercaAsset(r.NumeroCliente)) {
                log.Warn("RicercaAsset() fail");
                return false;
            }
            */


            
          if (!RicercaServiziEBeni(r.NumeroCliente))
            {
                log.Warn("RicercaServiziEBeni() fail");
                return false;
            }
            

            try
            {
                Keanu.WaitingGame();
                SwitchToDefaultContent();
                IWebElement tableResult = null;
                tableResult = TrovaTableResult();
                if (tableResult == null)
                    return false;
                int siebelCnt = 0;
                IWebElement nomeServizioEBeni = null;
                var lstHeadersServizi = tableResult.FindElements(By.XPath("./thead/tr/th"));
                int indexNOME_SERVIZIO_E_BENI = lstHeadersServizi.IndexOf(lstHeadersServizi.Where(q => q.Text.ToUpper().Contains("NOME SERVIZIO E BENE")).First());
                int indexNOME_PRODOTTO = lstHeadersServizi.IndexOf(lstHeadersServizi.Where(q => q.Text.ToUpper().Contains("NOME PRODOTTO")).First());
                int indexNOME_CLIENTE = lstHeadersServizi.IndexOf(lstHeadersServizi.Where(q => q.Text.ToUpper().Contains("NOME CLIENTE")).First());
                IList<IWebElement> listaTrServizi = tableResult.FindElements(By.XPath("./tbody/tr"));
                int row = listaTrServizi.Count();
                //bool esitoServizieBeniTable = false;
                for (int i = 1; i <= row; i++)
                {
                    nomeServizioEBeni = tableResult.FindElement(By.XPath("./tbody/tr[" + i + "]")).FindElements(By.XPath("./*")).ElementAt(indexNOME_SERVIZIO_E_BENI);
                    string nomeProdotto = tableResult.FindElement(By.XPath("./tbody/tr[" + i + "]")).FindElements(By.XPath("./*")).ElementAt(indexNOME_PRODOTTO).Text;
                    r.Cliente = tableResult.FindElement(By.XPath("./tbody/tr[" + i + "]")).FindElements(By.XPath("./*")).ElementAt(indexNOME_CLIENTE).Text;
                    if (nomeServizioEBeni.Text.StartsWith("2-"))
                        siebelCnt++;

                    /*
                    if (nomeServizioEBeni.Text.StartsWith("2-") && Constant.nomeProdotto.Contains(nomeProdotto.ToString()))
                    {
                        for (int j = 1; j <= row; j++)
                        {
                            var checkNOMESERVIZIOEBENI = tableResult.FindElement(By.XPath("./tbody/tr[" + j + "]")).FindElements(By.XPath("./*")).ElementAt(indexNOME_SERVIZIO_E_BENI);
                            if (!checkNOMESERVIZIOEBENI.Text.StartsWith("2-")) {
                                //return false;
                            }
                                
                        }
                        r.AttivitaServiziBeni = nomeServizioEBeni.Text;
                        esitoServizieBeniTable = true;
                        break;
                    }

                    */
                }

                //if (siebelCnt.Equals(row) && !esitoServizieBeniTable)
                //if (siebelCnt.Equals(row))
                //return false;

                r.Worktype = "SALESFORCE";
                if (SalesforceType()) {
                    return true;
                }

                if (scartoWithoutSiebelTry) {
                    scartoWithoutSiebelTry = false;
                    return false;
                }

                r.Worktype = "SIEBEL";
                if (!SfaLiber())
                    return false;

                if (!SiebelType())
                    return false;



                /*
                if (!esitoServizieBeniTable)
                {
                    r.Worktype = "SALESFORCE";
                    if (SalesforceType())
                        return true;
                    return false;
                }
                */



                return true;
            }
            catch
            {
                log.Info($"Nessun risultato per {r.NumeroCliente} in Servizi e Beni");
                return false;
            }
        }

        private bool SiebelType()
        {
            UnlockAndCloseAllTabs();

            /*
            if (!RicercaAsset(r.NumeroCliente)) {
                log.Warn("RicercaAsset() fail");
                return false;
            }
            */

            
            if (!RicercaServiziEBeni(r.NumeroCliente))
            {
                log.Warn("RicercaServiziEBeni() fail");
                return false;
            }
            

            try
            {
                Keanu.WaitingGame();
                SwitchToDefaultContent();
                IWebElement tableResult = null;
                tableResult = TrovaTableResult();
                if (tableResult == null)
                    return false;
                IWebElement nomeServizioEBeni = null;
                var lstHeadersServizi = tableResult.FindElements(By.XPath("./thead/tr/th"));
                int indexNOME_SERVIZIO_E_BENI = lstHeadersServizi.IndexOf(lstHeadersServizi.Where(q => q.Text.ToUpper().Contains("NOME SERVIZIO E BENE")).First());
                int indexNOME_PRODOTTO = lstHeadersServizi.IndexOf(lstHeadersServizi.Where(q => q.Text.ToUpper().Contains("NOME PRODOTTO")).First());
                int indexNOME_CLIENTE = lstHeadersServizi.IndexOf(lstHeadersServizi.Where(q => q.Text.ToUpper().Contains("NOME CLIENTE")).First());
                IList<IWebElement> listaTrServizi = tableResult.FindElements(By.XPath("./tbody/tr"));
                int row = listaTrServizi.Count();
                int checkingIndex = 1;

                UnlockAndCloseAllTabs();



                //bool esitoServizieBeniTable = false;
                bool isSiebelPossible = false;


                for (int i = 1; i <= row; i++)
                {

                    if (!RicercaServiziEBeni(r.NumeroCliente)) {
                        log.Warn("RicercaServiziEBeni() fail");
                        return false;
                    }

                    Keanu.WaitingGame();
                    SwitchToDefaultContent();
                    var beniTable = TrovaTableResult();
                    if (beniTable == null)
                        return false;

                    IWebElement nomeServizioEBeniCheck = null;
                    var lstHeadersServiziCheck = beniTable.FindElements(By.XPath("./thead/tr/th"));
                    int indexNOME_SERVIZIO_E_BENICheck = lstHeadersServiziCheck.IndexOf(lstHeadersServiziCheck.Where(q => q.Text.ToUpper().Contains("NOME SERVIZIO E BENE")).First());
                    int indexNOME_PRODOTTOCheck = lstHeadersServiziCheck.IndexOf(lstHeadersServiziCheck.Where(q => q.Text.ToUpper().Contains("NOME PRODOTTO")).First());
                    int indexNOME_CLIENTECheck = lstHeadersServiziCheck.IndexOf(lstHeadersServiziCheck.Where(q => q.Text.ToUpper().Contains("NOME CLIENTE")).First());

                    nomeServizioEBeniCheck = beniTable.FindElement(By.XPath("./tbody/tr[" + checkingIndex + "]")).FindElements(By.XPath("./*")).ElementAt(indexNOME_SERVIZIO_E_BENICheck);
                    string nomeProdotto = beniTable.FindElement(By.XPath("./tbody/tr[" + checkingIndex + "]")).FindElements(By.XPath("./*")).ElementAt(indexNOME_PRODOTTOCheck).Text;
                    r.Cliente = beniTable.FindElement(By.XPath("./tbody/tr[" + checkingIndex + "]")).FindElements(By.XPath("./*")).ElementAt(indexNOME_CLIENTECheck).Text;

                    //IF EMPTY
                    if (string.IsNullOrEmpty(nomeProdotto)) {
                        nomeProdotto = nomeServizioEBeniCheck.Text;
                    }

                    isSiebelPossible = CheckForSiebelServiziBeni(nomeServizioEBeniCheck);

                    if (isSiebelPossible) {
                        //Possible to make siebel type doc
                        break;
                    } else {
                        checkingIndex = checkingIndex + 1;
                        UnlockAndCloseAllTabs();
                    }


                    //Check every row


                    /*if (Constant.nomeProdotto.Contains(nomeProdotto.ToString()))
                    {
                        r.AttivitaServiziBeni = nomeServizioEBeni.Text;
                        log.Info($"Nome Prodotto: {nomeProdotto}");
                        esitoServizieBeniTable = true;
                        break;
                    }
                    */

                }
                /*
                if (!esitoServizieBeniTable)
                {
                    if(row != 1) {

                        log.Error($"NOME PRODOTTO FAIL");
                        return false;

                        /*
                        //CliccaColonna("NOME SERVIZIO E BENE", nomeServizioEBeni, tableResult);
                        try {
                            IList<IWebElement> lstHeader = tableResult.FindElements(By.XPath("./thead/tr/th"));
                            int indiceColonnaServizioBene = 0;
                            indiceColonnaServizioBene = lstHeader.IndexOf(lstHeader.Where(q => q.Text.ToUpper().Contains("NOME SERVIZIO E BENE")).First());

                            //IWebElement linkRicerca = nomeServizioEBeni.FindElement(By.XPath("//span/a"));
                            IWebElement linkRicerca = tableResult.FindElement(By.XPath("./tbody/tr[" + row + "]")).FindElements(By.XPath("./*")).ElementAt(indexNOME_SERVIZIO_E_BENI);
                            //IWebElement linkRicerca = nomeServizioEBeni.FindElements(By.XPath("./*")).ElementAt(indiceColonnaServizioBene);
                            linkRicerca.Click();
                            
                            Keanu.WaitingGame();
                            if (CheckCorrelato()) {

                            }
                        } catch (Exception) {

                            log.Error($"NOME PRODOTTO FAIL");
                            return false;
                        }

                        */

                  //  }

                //}
                    

                try {
                    if(isSiebelPossible == false) {
                        return false;
                    }
                    /*
                    Actions a1 = new Actions(Keanu.Driver);
                    var anchorNomeServizio = nomeServizioEBeni.FindElement(By.XPath(".//a"));
                    a1.MoveToElement(anchorNomeServizio).Perform();
                    Thread.Sleep(Keanu.Randy(1));
                    a1.Click(anchorNomeServizio).Perform();

                    //while (!IsElementVisible(By.XPath("//h1/div/div/div/span[. = 'Commodity']"), Keanu.Driver))
                    while (!IsElementVisible(By.XPath("//div/div/div/span[. = 'Commodity']"), Keanu.Driver))
                        Thread.Sleep(Keanu.Randy(1));


                    try {
                        var correlatoLink = Keanu.Driver.FindElement(By.XPath("//ul[@role = 'tablist']/li/a[@data-label = 'Correlato']"));
                        correlatoLink.Click();

                    } catch {

                        var correlatoLink2 = Keanu.Driver.FindElement(By.XPath("//ul[@role = 'tablist']/li/a[@data-label = 'Correlato']"));
                        correlatoLink2.Click();
                        log.Error($"Correlato click fail");
                        return false;
                    }
                    */
                    

                    while (!IsElementPresent(By.XPath("//span[. = 'Dati Catastali']"), Keanu.Driver))
                        Thread.Sleep(Keanu.Randy(1));
                } catch {
                    log.Error($"NOME PRODOTTO FAIL");
                    return false;
                }
                

                try
                {
                    for (int i = 0; i < 6; i++)
                        GoDOWN();
                    //var DatiCatastaliNuovo = Keanu.Driver.FindElement(By.XPath("//*[@id='tab-19']/slot/flexipage-component2/slot/lst-related-list-container/div/div[11]//button"));
                    ReadOnlyCollection<IWebElement> DatiCatastaliNuovo = Keanu.Driver.FindElements(By.XPath("//ul[@class = 'slds-button-group-list']/../../../../../..//button[@name = 'New']"));
                    if (DatiCatastaliNuovo.Count > 0)
                    {
                        IWebElement lastButton = DatiCatastaliNuovo.Last();
                        lastButton.Click();
                    }
                }
                catch
                {
                    Keanu.WaitingGame();
                    GoDOWN();
                    Thread.Sleep(5000);
                    for (int i = 0; i < 5; i++)
                        GoDOWN();

                    IWebElement DatiCatastaliNuovo;
                    try {
                        DatiCatastaliNuovo = Keanu.Driver.FindElement(By.XPath("//ul[@class = 'slds-button-group-list']/../../../../../..//button[@name = 'New']"));
                        //DatiCatastaliNuovo = Keanu.Driver.FindElement(By.XPath("//span[. = 'Dati Catastali']/../../../../..//div[@title = 'Nuovo']"));
                    } catch (Exception) {
                        log.Info("Correlato Dati Catastali Fail");
                        return false;

                    }
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", DatiCatastaliNuovo);
                }

                int cnt = 0;
                while (!Keanu.Driver.PageSource.ToString().Contains("Crea Dati Catastali") && cnt < 15)
                    Keanu.WaitingGame();

                var cercaCliente = Keanu.Driver.FindElement(By.XPath("//input[contains(@placeholder,'Cerca Clienti...')]"));
                cercaCliente.Clear();
                if (r.IdRecordSiebel != null)
                    cercaCliente.SendKeys(r.IdRecordSiebel);
                else if (r.CodiceAnagraficaCliente != null)
                    cercaCliente.SendKeys(r.CodiceAnagraficaCliente);
                else
                    cercaCliente.SendKeys(r.CodiceFiscale);
                Thread.Sleep(Keanu.Randy(1));
                cercaCliente.SendKeys(Keys.ArrowDown);
                Thread.Sleep(Keanu.Randy(1));
                cercaCliente.SendKeys(Keys.Enter);
                Thread.Sleep(Keanu.Randy(1));

                IWebElement clienteHeader = null;
                try
                {
                    clienteHeader = Keanu.Driver.FindElement(By.XPath("//div[@class='slds-page-header slds-page-header_joined slds-is-relative forceSearchSearchResultsGridHeader forceSearchResultsGridView']"));
                }
                catch (Exception)
                {
                    cercaCliente = Keanu.Driver.FindElement(By.XPath("//input[contains(@placeholder,'Cerca Clienti...')]"));
                    cercaCliente.Clear();
                    if (r.IdRecordSiebel != null)
                        cercaCliente.SendKeys(r.IdRecordSiebel);
                    else if (r.CodiceAnagraficaCliente != null)
                        cercaCliente.SendKeys(r.CodiceAnagraficaCliente);
                    else
                        cercaCliente.SendKeys(r.CodiceFiscale);
                    Thread.Sleep(Keanu.Randy(1));
                    cercaCliente.SendKeys(Keys.Enter);
                    Thread.Sleep(Keanu.Randy(1));
                    Keanu.WaitingGame();
                    Thread.Sleep(5000);
                    clienteHeader = Keanu.Driver.FindElement(By.XPath("//div[@class='slds-page-header slds-page-header_joined slds-is-relative forceSearchSearchResultsGridHeader forceSearchResultsGridView']"));
                }
                var countRisultato = clienteHeader.FindElements(By.XPath(".//p[contains(. , ' risultat')]")).Where(a => a.Text.Contains("risultat")).First();

                Thread.Sleep(Keanu.Randy(1));

                if (string.IsNullOrEmpty(countRisultato.Text))
                {
                    log.Error("CF VUOTO");
                    var bChiusura = Keanu.Driver.FindElement(By.XPath("//button[@title='Chiudi questa finestra']"));
                    bChiusura.Click();
                    return false;
                }

                Thread.Sleep(Keanu.Randy(1));

                var clienteTable = Keanu.Driver.FindElement(By.XPath("//h2[contains(., 'Cliente')]/../..//table"));

                var lstHeadersClienteRisultati = clienteTable.FindElements(By.XPath("./thead/tr/th"));
                int indexNomeCliente = lstHeadersClienteRisultati.IndexOf(lstHeadersClienteRisultati.Where(q => q.Text.ToUpper().Contains("NOME CLIENTE")).First());
                IList<IWebElement> listaTrClienteRisultati = clienteTable.FindElements(By.XPath("./tbody/tr"));
                int numeroRigheTabellaClienteRisultati = listaTrClienteRisultati.Count();
                bool esitoClienteRisultatiTable = false;
                for (int i = 1; i <= numeroRigheTabellaClienteRisultati; i++)
                {
                    var nomeCliente = clienteTable.FindElement(By.XPath("./tbody/tr[" + i + "]")).FindElements(By.XPath("./*")).ElementAt(indexNomeCliente);
                    if (nomeCliente.Text.Equals(r.Cliente))
                    {
                        esitoClienteRisultatiTable = true;
                        var anchorNome = nomeCliente.FindElement(By.XPath("./a"));
                        Actions a = new Actions(Keanu.Driver);
                        a.MoveToElement(anchorNome).Perform();
                        Thread.Sleep(Keanu.Randy(1));
                        a.Click(anchorNome).Perform();
                        break;
                    }
                }
                if (!esitoClienteRisultatiTable)
                {
                    var bChiusura = Keanu.Driver.FindElement(By.XPath("//button[@title='Chiudi questa finestra']"));
                    bChiusura.Click();
                    return false;
                }

                Keanu.WaitingGame();

                if (r.ImmobiliEsclusi == "6")
                    r.ImmobiliEsclusi = "4";

                if (r.ImmobiliEsclusi == "5")
                    r.ImmobiliEsclusi = "3";

                if (!InsertValue("Comune Amministrativo", r.ComuneAmministrativo))
                    return false;
                if (!InsertValue("Comune Catastale", r.ComuneCatastale))
                    return false;
                if (!InsertValue("Codice Comune Catastale", r.CodiceComuneCatastale))
                    return false;
                if (!InsertValue("Foglio", r.Foglio))
                    return false;
                if (!InsertValue("Sezione", r.Sezione))
                    return false;
                if (!InsertValue("Particella", r.Particella))
                    return false;
                if (!InsertValue("Estensione Particella", r.EstensioneParticella))
                    return false;
                if (!InsertValue("Subalterno", r.Subalterno))
                    return false;

                if (!InserValueCombo("Tipo Particella", r.TipoParticella))
                    return false;

                if (!InserValueCombo("Tipo Unità", r.TipoUnita))
                    return false;

                if (!InserValueCombo("Immobili Esclusi", r.ImmobiliEsclusi))
                    return false;

                if (!InserValueCombo("Qualifica", r.Qualifica))
                    return false;

                Keanu.WaitingGame();

                ClickButtonByName("Salva", "slds-button slds-button_brand");

                cnt = 0;
                while (IsElementVisible(By.XPath("//button[@class = 'footer-button page-error-button slds-button slds-button_icon-error slds-m-right_small']"), Keanu.Driver))
                {
                    if (cnt == 5)
                    {
                        log.Error("È stato rilevato un errore.");
                        try
                        {
                            ClickButtonByName("Annulla", "slds-button slds-button_neutral");
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        return false;
                    }
                    Thread.Sleep(Keanu.Randy(1));
                    cnt++;
                }

                return true;
            }
            catch
            {
                log.Warn("SiebelType() fail");
                return false;
            }
        }

        private bool CheckForSiebelServiziBeni(IWebElement nomeServizioEBeni) {
            try {


                Actions a1 = new Actions(Keanu.Driver);
                var anchorNomeServizio = nomeServizioEBeni.FindElement(By.XPath(".//a"));
                a1.MoveToElement(anchorNomeServizio).Perform();
                Thread.Sleep(Keanu.Randy(1));
                a1.Click(anchorNomeServizio).Perform();
                Thread.Sleep(Keanu.Randy(2));

                var correlatoLink1 = Keanu.Driver.FindElement(By.XPath(".//a[@data-label = 'Correlato']"));
                correlatoLink1.Click();
                
                Thread.Sleep(Keanu.Randy(2));

                for (int i = 0; i < 6; i++) {
                    GoDOWN();
                }

                var DatiCatastaliNuovo = Keanu.Driver.FindElement(By.XPath("//ul[@class = 'slds-button-group-list']/../../../../../..//button[@name = 'New']"));
                //var DatiCatastaliNuovo = Keanu.Driver.FindElement(By.XPath("//span[. = 'Dati Catastali']/../../../../..//div[@title = 'Nuovo']"));
                if (DatiCatastaliNuovo != null) {
                    log.Info("Correlato Dati Catastali Success");
                    return true;
                } else {
                    log.Info("Correlato Dati Catastali Fail");
                    return false;
                }

            } catch {
                return false;
            }

            return false;
        }
        private bool CheckCorrelato() {
            log.Info("Check Correlato Dati Catastali");

            Keanu.Driver.FindElement(By.LinkText("Correlato")).Click();

            while (!IsElementPresent(By.XPath("//span[. = 'Dati Catastali']"), Keanu.Driver)) {
                Thread.Sleep(Keanu.Randy(1));
            }
            try {
                for (int i = 0; i < 6; i++) {
                    GoDOWN();
                }
                var DatiCatastaliNuovo = Keanu.Driver.FindElement(By.XPath("//span[. = 'Dati Catastali']/../../../../..//div[@title = 'Nuovo']"));
                if (DatiCatastaliNuovo != null) {
                    log.Info("Correlato Dati Catastali Success");
                    return true;
                }

            } catch (Exception) {
                log.Error($"CORRELATO DATI CATASTALI FAIL");
                return false;
            }
            return false;
            }

        private bool SalesforceType()
        {
            try
            {
                //Keanu.Driver.Navigate().GoToUrl($"https://enelcrmt.lightning.force.com/lightning/r/Account/{utenzaSfa.idAccount}/view");
                //Keanu.WaitingGame();

                if (!RicercaParametroInTabellaRisultati())
                    return false;

                if (!SfaLiber())
                    return false;

                try
                {
                    for (int i = 0; i < 6; i++)
                        GoDOWN();
                    var spanDocumentiDaValidare = Keanu.Driver.FindElement(By.XPath("//span[. = 'Documenti Da Validare']"));
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", spanDocumentiDaValidare);
                }
                catch
                {
                    Keanu.WaitingGame();
                    GoDOWN();
                    Thread.Sleep(5000);
                    for (int i = 0; i < 5; i++)
                        GoDOWN();
                    var spanDocumentiDaValidare = Keanu.Driver.FindElement(By.XPath("//span[. = 'Documenti Da Validare']"));
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", spanDocumentiDaValidare);
                }

                int cnt = 0;
                while (!IsElementVisible(By.XPath($"//h1[. = 'Documenti Da Validare']"), Keanu.Driver))
                {
                    if (cnt == 5)
                    {
                        log.Info("W8 for Documenti Da Validare fail");
                        return false;
                    }
                    Thread.Sleep(Keanu.Randy(1));
                    cnt++;
                }

                Keanu.WaitingGame();
                var table = Keanu.Driver.FindElement(By.XPath("//h1[.='Documenti Da Validare']/../../../../../..//table")); //orgininal
                //var table = Keanu.Driver.FindElement(By.XPath("//h1[@class='slds-page-header__title listViewTitle slds-truncate' and @title='Documenti Da Validare']")); //labots 
                var lstHeaders = table.FindElements(By.XPath("./thead/tr/th"));
                int indexDOCUMENTI_DI_VALIDAZIONE = lstHeaders.IndexOf(lstHeaders.Where(q => q.Text.Contains("Documenti di Validazione")).First());
                int indexMODELLO = lstHeaders.IndexOf(lstHeaders.Where(q => q.Text.Contains("Modello")).First());
                int indexSTATO = lstHeaders.IndexOf(lstHeaders.Where(q => q.Text.Contains("Stato")).First());
                IList<IWebElement> listaTr = table.FindElements(By.XPath("./tbody/tr"));
                IList<IWebElement> listaML_DatiCatastali = listaTr.Where(modello => modello.Text.Contains("ML_DatiCatastali")).ToList();
                IList<IWebElement> listaDA_VALIDARE = listaML_DatiCatastali.Where(STATO => STATO.Text.Contains("DA VALIDARE")).ToList();
                IList<IWebElement> listaVALIDATO = listaML_DatiCatastali.Where(STATO => STATO.Text.Contains("VALIDATO")).ToList();

                if (!listaDA_VALIDARE.Count.Equals(0))
                {
                    if (!LookingForOffertaWithSpecificPodInDocumentiDaValidareTab(listaDA_VALIDARE))//ML_DatiCatastali + DA VALIDARE
                    {
                        if (!LookingForOffertaWithSpecificPodInDocumentiDaValidareTab(listaVALIDATO))//ML_DatiCatastali + VALIDATO
                        {
                            if (r.Quote.Equals("VUOTO"))
                            {
                                log.Error("VUOTO, TRY SIEBEL TYPE");
                                if (!SiebelType())
                                    return false;
                                else
                                {
                                    r.Worktype = "SIEBEL";
                                    return true;
                                }
                            }
                            if (!SiebelType())
                                return false;
                            else
                            {
                                r.Worktype = "SIEBEL";
                                return true;
                            }
                        }
                        else
                        {
                            log.Info($"Gia Validato");//WORKTYPE = SALESFORCE, BUT FIRST GET GIA VALIDATO ATTIVITA NUMBER. IMPORTANT!!!
                            if (!GetAttivitaForCommento())
                            {
                                log.Info($"No ML_DatiCatastali per gia validato");
                                return false;
                            }
                            return true;
                        }
                    }
                    else
                        return true;//SALESFORCE - VALIDAZIONE DOCUMENTO
                }
                else if (!listaVALIDATO.Count.Equals(0))//IF DA VALIDARE DOESNT EXIST, BUT VALIDATO DOES
                {
                    if (!LookingForOffertaWithSpecificPodInDocumentiDaValidareTab(listaVALIDATO))
                    {
                        if (!SiebelType())
                            return false;
                        else
                        {
                            r.Worktype = "SIEBEL";
                            return true;
                        }
                    }
                    else
                    {
                        log.Info($"Gia Validato");//WORKTYPE = SALESFORCE, BUT FIRST GET GIA VALIDATO ATTIVITA NUMBER. IMPORTANT!!!
                        if (!GetAttivitaForCommento())
                        {
                            log.Info($"No ML_DatiCatastali per gia validato");
                            scartoWithoutSiebelTry = true;
                            return false;
                        }
                        return true;
                    }
                }
                else
                {
                    log.Info($"Senza ML_DatiCatastali");
                    if (!SiebelType())
                        return false;
                    else
                    {
                        r.Worktype = "SIEBEL";
                        return true;
                    }
                }
            }
            catch (Exception Ex)
            {
                log.Warn(Ex.ToString());
                needToRestart = true;
                return false;
            }
        }

        private bool InsertAndCheckIt(IWebElement actibleElementCheckList, String searchType, String searchValue, String insertValue, int action = 0)
        {
            try
            {
                Thread.Sleep(500);
                IWebElement txtField = null;

                if (searchType.ToUpper().Equals("ID"))
                    txtField = actibleElementCheckList.FindElement(By.Id(searchValue));
                else if (searchType.ToUpper().Equals("XPATH"))
                    txtField = actibleElementCheckList.FindElement(By.XPath(searchValue));
                else
                    return false;

                if (action == 1)
                {
                    try
                    {
                        var buttonValidaDocumento = actibleElementCheckList.FindElements(By.TagName("button")).Where(a => a.Text.Contains("Valida Documento")).First();
                        if (!buttonValidaDocumento.Enabled)
                            txtField.Click();
                        if (!buttonValidaDocumento.Enabled)
                            return false;
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

                if (!txtField.GetAttribute("value").Equals(insertValue))
                {
                    if (action == 0)
                    {
                        if (txtField.GetAttribute("value").Length > 0 && !txtField.TagName.Contains("select"))
                            txtField.Clear();
                        switch (searchValue)
                        {
                            case "ITA_IFM_Cadastral_Exclusive_Property__c":
                                {
                                    if (insertValue.Equals("5"))
                                    {
                                        txtField.SendKeys("3");
                                    }
                                    else if (insertValue.Equals("6"))
                                    {
                                        txtField.SendKeys("4");
                                    }
                                    else
                                        txtField.SendKeys(insertValue);
                                    break;
                                }
                            default:
                                {
                                    txtField.SendKeys(insertValue);
                                    break;
                                }
                        }
                    }
                    else if (action == 1)//CLICK
                    {
                        txtField.Click();
                    }
                    switch (searchValue)
                    {
                        case "ITA_IFM_Cadastral_Type_Part__c":
                            {
                                if (insertValue.Equals("E"))
                                {
                                    if (!txtField.GetAttribute("value").Equals("Edificabile"))
                                    { return false; }
                                }
                                else if (insertValue.Equals("F"))
                                {
                                    if (!txtField.GetAttribute("value").Equals("Fondiaria"))
                                    { return false; }
                                }
                                else
                                {
                                    if (!txtField.GetAttribute("value").Equals(insertValue))
                                    { return false; }
                                }
                                break;
                            }
                        default:
                            {
                                if (!txtField.GetAttribute("value").Equals(insertValue))
                                { return false; }
                                break;
                            }
                    }
                }
            }
            catch
            {
                log.Info($"{insertValue} - KO");
                return false;
            }
            return true;
        }

        private bool FillFields()
        {
            bool mainDatiCatastaliDone = false;
            try
            {
                IWebElement tabella = Keanu.Driver.FindElement(By.XPath(".//table[contains(@class, 'slds-table slds-table--bordered slds-table--cell-buffer')]"));
                if (tabella == null)
                    return false;
                try
                {
                    var rowWithFilter = tabella.FindElements(By.XPath("//*[@name='options']/parent::*/parent::*/parent::*")).Where(a => a.Text.Contains("ML_DatiCatastali") && a.Text.Contains("DA VALIDARE") && a.Text.Contains(r.Pod)).ToList();
                    log.Info($"Righe da validare: {rowWithFilter.Count}");
                    if (rowWithFilter.Count == 0)
                    {
                        log.Error($"{r.Pod} gia validato o senza ML_DatiCatastali");
                        return false;
                    }
                    foreach (var item in rowWithFilter)
                    {
                        IWebElement radioButton = item.FindElement(By.XPath(".//input[@type = 'radio' and @name = 'options']"));
                        ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click()", radioButton);

                        ClickButtonByName("Effettua Checklist", "slds-button slds-button--neutral slds-col--bump-left slds-button--brand slds-m-horizontal--x-large");

                        WaitSpecificPage("Elementi Checklist");
                        IWebElement articleElementChecklist = Keanu.Driver.FindElement(By.Id("elementi-checklist-card"));
                        if (mainDatiCatastaliDone)//IF MAIN IS VALIDATED THEN NEXT ONES JUST "Cliente Dichiara dati catastali" – SI / Firma - CHECKED.
                        {

                            bool everythingInserted = true; 
                            if(!InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Certify_Cadastral_Data__c", "SI")) {
                                everythingInserted = false;
                            }
                            if (!InsertAndCheckIt(articleElementChecklist, "XPath", "//*[@id='ITA_IFM_Signature__c']/parent::*/span", "", 1)) {
                                everythingInserted = false;
                            }
                            if (!everythingInserted) {
                                log.Error($"ML_DatiCatastali insert fail");
                                return false;
                            }

                            //InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Certify_Cadastral_Data__c", "SI");
                            //InsertAndCheckIt(articleElementChecklist, "XPath", "//*[@id='ITA_IFM_Signature__c']/parent::*/span", "", 1);
                        }
                        else
                        {

                            bool everythingInserted = true;
                            if (!InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Certify_Cadastral_Data__c", "SI")) {
                                everythingInserted = false;
                            }
                            if (!InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Qualification__c", r.Qualifica)) {
                                everythingInserted = false;
                            }
                            if (!InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_City__c", r.ComuneCatastale)) {
                                everythingInserted = false;
                            }
                            if (!InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_Unit_Type__c", r.TipoUnita)) {
                                everythingInserted = false;
                            }
                            if (!InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_Paper__c", r.Foglio)) {
                                everythingInserted = false;
                            }
                            if (!InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_PartExt__c", r.EstensioneParticella)) {
                                everythingInserted = false;
                            }
                            if (!InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_SubPlot__c", r.Subalterno)) {
                                everythingInserted = false;
                            }
                            if (!InsertAndCheckIt(articleElementChecklist, "XPath", "//*[@id='ITA_IFM_Signature__c']/parent::*/span", "", 1)) {
                                everythingInserted = false;
                            }
                            if (!InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_Exclusive_Property__c", r.ImmobiliEsclusi)) {
                                everythingInserted = false;
                            }
                            if (!InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Administrative_City__c", r.ComuneAmministrativo)) {
                                everythingInserted = false;
                            }
                            if (!InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_Code__c", r.CodiceComuneCatastale)) {
                                everythingInserted = false;
                            }
                            if (!InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_Section__c", r.Sezione)) {
                                everythingInserted = false;
                            }
                            if (!InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_Part__c", r.Particella)) {
                                everythingInserted = false;
                            }
                            if (!InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_Type_Part__c", r.TipoParticella)) {
                                everythingInserted = false;
                            }
                            if (!InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_SubscriptionDate__c", DateTime.Now.ToShortDateString())) {
                                everythingInserted = false;
                            }

                            if (!everythingInserted) {
                                log.Error($"ML_DatiCatastali insert fail");
                                return false;
                            }


                            //InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Certify_Cadastral_Data__c", "SI");
                            //InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Qualification__c", r.Qualifica);
                            //InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_City__c", r.ComuneCatastale);
                            //InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_Unit_Type__c", r.TipoUnita);
                            //InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_Paper__c", r.Foglio);
                            //InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_PartExt__c", r.EstensioneParticella);
                            //InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_SubPlot__c", r.Subalterno);
                            //InsertAndCheckIt(articleElementChecklist, "XPath", "//*[@id='ITA_IFM_Signature__c']/parent::*/span", "", 1);
                            //InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_Exclusive_Property__c", r.ImmobiliEsclusi);
                            //InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Administrative_City__c", r.ComuneAmministrativo);
                            //InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_Code__c", r.CodiceComuneCatastale);
                            //InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_Section__c", r.Sezione);
                            //InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_Part__c", r.Particella);
                            //InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_Cadastral_Type_Part__c", r.TipoParticella);
                            //InsertAndCheckIt(articleElementChecklist, "ID", "ITA_IFM_SubscriptionDate__c", DateTime.Now.ToShortDateString());
                            mainDatiCatastaliDone = true;
                        }
                        if (!ClickButtonByName("Valida Documento", "slds-button slds-button--neutral slds-m-around--small slds-button--brand ValidaDocumento"))
                            return false;
                    }
                }
                catch
                {
                    log.Error($"ML_DatiCatastali with {r.Pod} is disabled");
                    return false;
                }
            }
            catch
            {
                log.Warn($"FillFields() fail");
                return false;
            }
            return true;
        }

        public bool Riclassifaktorz()
        {
            try
            {
                IWebElement attHeader = Keanu.Driver.FindElement(By.XPath("//div[@class = 'slds-page-header titleArea'] "));
                if (attHeader.Text.Contains("Conferma e Riassegna"))
                    return true;
                if (!attHeader.Text.Contains("Riclassifica"))
                {
                    log.Debug("No Riclassifica button");
                    return false;
                }
                ClickButtonByName("Riclassifica");
                return true;
            }
            catch
            {
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

        private bool Modificatore(string causale, string descrizione, string specifica)
        {
            try
            {
                IWebElement attHeader = Keanu.Driver.FindElement(By.XPath("//div[@class = 'slds-page-header titleArea'] "));
                if (attHeader.Text.Contains("Modifica"))
                    ClickButtonByName("Modifica");
                else if (attHeader.Text.Contains("Annulla"))
                    log.Info($"Already Annulla");
                else
                    return false;

                if (v.Riklasifi && !v.AlreadyTripper)
                {
                    if (!Riclassifaktorz())
                    {
                        log.Warn($"Riclassifaktorz() fail");
                        //return false;
                    }
                    if (!SetTripletta(causale, descrizione, specifica))
                    {
                        log.Warn($"SetTripletta() fail");
                        return false;
                    }

                    v.AlreadyTripper = true;

                    GoUP();
                    string podSfa = GetFieldValue("* POD");
                    if (podSfa.Length < 1)
                        AssociaServizioPod("");

                    if (!ClickButtonByName("Conferma"))
                        return false;

                    if (!ClickButtonByName("Modifica"))
                        return false;

                    Keanu.WaitingGame();

                    bool esito = AttendieConfermaModale("slds-modal__container", "Unexpeted error", "", "OK", false, out bool modalFind);
                    if (!esito && modalFind)
                    {
                        log.Error("Problemi nel chiudere la modale 'Unexpeted error'");
                        return false;
                    }
                }
            }
            catch
            {
                log.Warn($"Modificatore() fail");
            }
            return true;
        }

        public bool GoToSottoTab(string nomeSottoTab = "")
        {
            bool esito = false;
            int numeroTentativi = 0;
            do
            {
                try
                {
                    SwitchToDefaultContent();

                    //recupero la lista dei tab aperti
                    IList<IWebElement> listTabAperti = Keanu.Driver.FindElements(By.XPath("//li[contains(@class,'tabItem slds-tabs--default__item slds-sub-tabs__item')]"));
                    for (int i = 0; i < listTabAperti.Count; i++)
                    {
                        IWebElement tab = listTabAperti[i].FindElement(By.XPath("./a"));
                        string nmTab = tab.GetAttribute("title");
                        nmTab = System.Text.RegularExpressions.Regex.Replace(nmTab, @"\s{2,}", " ");
                        nomeSottoTab = System.Text.RegularExpressions.Regex.Replace(nomeSottoTab, @"\s{2,}", " ");
                        //***09/02/2021 INIZIO MODIFICA***
                        if (nmTab.Contains("Caricamento in corso"))
                        {
                            //Cliccare su Aggiorna Scheda
                            IWebElement div1 = listTabAperti[i].FindElement(By.XPath(".//div[contains(@class,'slds-dropdown-trigger slds-dropdown-trigger--click slds-p-left--none slds-p-right--none ')]"));
                            IWebElement div2 = div1.FindElement(By.XPath(".//div[contains(@class,'slds-dropdown-trigger slds-dropdown-trigger_click tabActionsList')]"));
                            IWebElement button1 = div2.FindElement(By.XPath(".//button[contains(@class, 'slds-button slds-button_icon-container slds-button_icon-x-small')]"));
                            ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", button1);
                            Thread.Sleep(Keanu.Randy(2));
                            IWebElement div3 = div2.FindElement(By.XPath(".//div[contains(@class, 'slds-dropdown slds-dropdown_right')]"));
                            IWebElement ul = div3.FindElement(By.XPath(".//ul[contains(@class, 'dropdown__list slds-dropdown__list slds-dropdown_length-with-icon-10')]"));
                            IWebElement li = ul.FindElement(By.XPath(".//li[contains(@class, 'slds-dropdown__item refreshTab')]"));
                            IWebElement button2 = div2.FindElement(By.XPath(".//a"));
                            ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", button2);
                            Thread.Sleep(Keanu.Randy(2));
                            Keanu.WaitingGame();
                            tab = listTabAperti[i].FindElement(By.XPath("./a"));
                            nmTab = tab.GetAttribute("title");
                            nmTab = System.Text.RegularExpressions.Regex.Replace(nmTab, @"\s{2,}", " ");
                            nomeSottoTab = System.Text.RegularExpressions.Regex.Replace(nomeSottoTab, @"\s{2,}", " ");
                        }
                        //***09/02/2021 FINE MODIFICA***
                        if (nmTab.Contains(nomeSottoTab)) { ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", tab); Keanu.WaitingGame(); esito = true; break; }
                        else { esito = false; }
                    }
                    numeroTentativi++;
                }
                catch (Exception e) { log.Error($"Errore nel cambio Tab, tipo di errore: \n {e.Message}"); esito = false; numeroTentativi++; }
            } while (!esito && numeroTentativi < 5);

            return esito;
        }

        private bool LookingForOffertaWithSpecificPodInDocumentiDaValidareTab(IList<IWebElement> listaRows)
        {
            try
            {
                foreach (IWebElement row in listaRows)
                {
                    CloseSubItemExcept("Documenti Da Validare");

                    GoToSottoTab("Documenti Da Validare");

                    var tdDocumentiDiValidazione = row.FindElement(By.XPath(".//th//a[contains(@title, 'Validation Document-')]"));
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", tdDocumentiDiValidazione);

                    Keanu.WaitingGame();

                    try
                    {
                        IWebElement valoreQuoteClickable = null;
                        try {
                            valoreQuoteClickable = Keanu.Driver.FindElements(By.XPath(".//span[. = 'Offerta']/../..//span//a//span")).Last();
                        } catch {
                            log.Error($"Offerta empty");
                            r.Quote = "VUOTO";
                            return false;
                        }
                        
                        string trimmed = valoreQuoteClickable.Text.Trim();
                        if (trimmed.StartsWith("2") || trimmed.StartsWith("3") || trimmed.StartsWith("4") || trimmed.StartsWith("5"))
                        {
                            log.Info($"Quote: {trimmed}");
                            r.Quote = trimmed;
                            ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", valoreQuoteClickable);
                        }
                        else
                        {
                            log.Error($"QUOTE EMPTY");
                            return false;
                        }
                    }
                    catch
                    {
                        r.Quote = "VUOTO";
                        continue;
                    }

                    Keanu.WaitingGame();

                    IWebElement divElementiOfferta = Keanu.Driver.FindElement(By.XPath("//div[contains(text(), \"Elementi dell'Offerta\")]"));
                    ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", divElementiOfferta);
                    Thread.Sleep(Keanu.Randy(1));
                    IWebElement elementiDellOferta = Keanu.Driver.FindElement(By.XPath("//div[@class = 'slds-p-around-medium slds-show']"));
                    if (elementiDellOferta.Text.Contains(r.Pod))
                    {
                        log.Info($"Elementi dell'Offerta contains {r.Pod}");
                        return true;
                    }
                    else
                    {
                        log.Info($"Elementi dell'Offerta doesn't contain {r.Pod}");
                        continue;
                    }
                }
                return false;
            }
            catch (Exception Ex)
            {
                return false;
            }
        }

        public bool RicercaParametroInTabellaRisultati()
        {
            try
            {
                SwitchToDefaultContent();
                IWebElement tableResult = null;
                tableResult = TrovaTableResult();
                if (tableResult == null)
                    return false;
                IWebElement elementRowRicerca = null;
                elementRowRicerca = tableResult.FindElements(By.XPath("tbody/tr")).ToList()[0];
                if (!CliccaColonna("NOME CLIENTE", elementRowRicerca, tableResult))
                    CliccaColonna("NOME CLIENTE", elementRowRicerca, tableResult);
                Keanu.WaitingGame();
                return true;
            }
            catch
            {
                log.Warn("RicercaParametroInTabellaRisultati() fail");
                return false;
            }
        }

        public bool CliccaColonna(string nomeColonna, IWebElement elementRow, IWebElement tableResult = null)
        {
            try
            {
                if (tableResult == null)
                    tableResult = TrovaTableResult();
                IList<IWebElement> lstHeader = tableResult.FindElements(By.XPath("./thead/tr/th"));
                int indiceColonnaCliente = 0;
                try
                {
                    indiceColonnaCliente = lstHeader.IndexOf(lstHeader.Where(q => q.Text.ToUpper().Contains(nomeColonna)).First());
                }
                catch
                {
                    indiceColonnaCliente = tableResult.FindElements(By.XPath("./thead/tr/th[translate(@title, 'abcdefghijklmnopqrstuvwxyz', 'ABCDEFGHIJKLMNOPQRSTUVWXYZ')='" + nomeColonna + "'][1]/preceding-sibling::th")).Count;
                    log.Info("Indice colonna");
                }
                IWebElement linkRicerca = elementRow.FindElements(By.XPath("./*/*/*")).ElementAt(indiceColonnaCliente);
                linkRicerca.Click();
                log.Info("Click su " + linkRicerca.Text);
                Keanu.WaitingGame();
                if (nomeColonna.Equals("CLIENTE"))
                    WaitSpecificPage("Info Aggiuntive");
                else
                    WaitSpecificPage(linkRicerca.Text);
                return true;
            }
            catch
            {
                log.Warn("CliccaColonna() fail");
                return false;
            }
        }

        private bool GetAttivitaForCommento()
        {
            try
            {
                List<Offerta> listaOfferte = (List<Offerta>)sfaLib.SearchGenerico(r.Quote, SFALibrary.Helpers.Utility.RicercaOfferteName, false);
                if (listaOfferte == null || listaOfferte.Count == 0)
                    return false;
                Offerta offerta = listaOfferte[0];
                if (offerta == null)
                    return false;
                List<Documento> documentiDaLavorareFromSfalib = new List<Documento>();
                documentiDaLavorareFromSfalib = (List<Documento>)sfaLib.GetRelatedListItemsNew(offerta.RecordId, SFALibrary.Helpers.Utility.DocumentiDaValidareItemName, SFALibrary.Helpers.Utility.DocumentiDaValidareApiName, "Documenti da validare");
                List<Documento> documentiConFiltro = new List<Documento>();
                documentiConFiltro = documentiDaLavorareFromSfalib.Where(d => d.Modello.Trim().Contains("ML_DatiCatastali") && d.Stato.Trim().ToUpper().Equals("VALIDATO")).ToList();
                if (documentiConFiltro == null || documentiConFiltro.Count == 0)
                    return false;
                var lista = documentiConFiltro.GroupBy(x => x.ActivityId).ToList();
                log.Debug($"Attività count: {lista.Count()}");
                List<string> listaNumAtt = new List<string>();
                foreach (var item in lista)
                {
                    string b = item.Key;
                    Attivita attivitaDiValidazione = (Attivita)sfaLib.GetRecord(b, SFALibrary.Helpers.Utility.RicercaAttivitaName);
                    string numAtt = attivitaDiValidazione.Numero;
                    listaNumAtt.Add(numAtt);
                }
                r.AttivitaPerCommento = listaNumAtt[0];
                log.Debug($"Attività per commento: {r.AttivitaPerCommento}");
                r.Worktype = "SALESFORCE + COMMENTO";
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool InsertValue(string elementName, string value)
        {
            Thread.Sleep(750);
            var element = Keanu.Driver.FindElement(By.XPath($"//span[. = '{elementName}']/../..//input"));
            element.Clear();
            element.SendKeys(value);
            if (!element.GetAttribute("value").Equals(value))
                return false;
            return true;
        }

        private bool InserValueCombo(string elementName, string value)
        {
            try {
                Thread.Sleep(Keanu.Randy(1));
                if (string.IsNullOrEmpty(value))
                    return true;
                //var element = Keanu.Driver.FindElement(By.XPath($"//span[. = '{elementName}']/../..//input"));
                var element = Keanu.Driver.FindElement(By.XPath($"//button[contains(@aria-label,'{elementName}')]"));
                Thread.Sleep(Keanu.Randy(1));
                element.SendKeys(value);
                Thread.Sleep(500);
                element.SendKeys(Keys.Enter);
                Thread.Sleep(Keanu.Randy(1));
                return true;
            } catch {
                return false;
            }
        }

        public void FindElementOnPage(IWebDriver driver, By by)
        {
            try
            {
                OpenQA.Selenium.Remote.RemoteWebElement element = (OpenQA.Selenium.Remote.RemoteWebElement)driver.FindElement(by);
                var hack = element.LocationOnScreenOnceScrolledIntoView;
                Thread.Sleep(Keanu.Randy(1));
            }
            catch { }
        }

        public void FindElementOnPage(IWebElement elem)
        {
            try
            {
                OpenQA.Selenium.Remote.RemoteWebElement element = (OpenQA.Selenium.Remote.RemoteWebElement)elem;
                var hack = element.LocationOnScreenOnceScrolledIntoView;
                Thread.Sleep(Keanu.Randy(1));
            }
            catch { }
        }

        private bool IsElementVisible(By by, IWebDriver driver = null, IWebElement elementDriver = null)
        {
            WebDriverWait waitElement = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            try
            {
                waitElement.Until(ExpectedConditions.ElementIsVisible(by));
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

        private void GoUP()
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)Keanu.Driver;
            js.ExecuteScript("window.scrollBy(0,-1000)", "");
            Thread.Sleep(1000);
        }

        private void GoDOWN()
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)Keanu.Driver;
            js.ExecuteScript($"window.scrollBy(0,300)", "");
            Thread.Sleep(1000);
        }

        public bool SfaLiber()
        {
            ServizioEBene datiSfa = new ServizioEBene();
            DatiCliente dati = new DatiCliente();
            SFALibrary.Model.Ricerca.RicercaCliente ric = new SFALibrary.Model.Ricerca.RicercaCliente(true, true, false, false);
            var listServiziEBeniUtenza = (List<ServizioEBene>)sfaLib.SearchGenerico(r.NumeroCliente, SFALibrary.Helpers.Utility.RicercaServiziEBeniName);
            datiSfa = listServiziEBeniUtenza.Where(x => x.NumeroUtente.Equals(r.NumeroCliente)).FirstOrDefault();//JUST FOR datiSfa.IdAccount
            foreach (var item in listServiziEBeniUtenza)
            {
                if (item.NumeroUtente.Equals(r.NumeroCliente) && !string.IsNullOrEmpty(item.PodPdr))
                {
                    if (!item.Stato.Contains("Disattiv"))
                    {
                        r.Pod = item.PodPdr;
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(r.Pod))
            {
                log.Error($"POD VUOTO/DISATTIVATO/DISATTIVATA");
                return false;
            }
            try
            {
                dati = (DatiCliente)sfaLib.GetDatiClienti(datiSfa.IdAccount, ric);
            }
            catch
            {
                log.Error($"Cannot get dati clienti");
                return false;
            }

            if (presentCodiceFiscale)
            {
                int result = Keanu.CompareTo(r.CodiceFiscale, dati.CodiceFiscaleTestata);
                if (result.Equals(0))
                {
                    //CF OK
                }
                else if (result <= 2)
                {
                    log.Warn($"A LITTLE BIT WRONG CF");
                    log.Warn(result);
                    //r.CodiceFiscale = dati.CodiceFiscaleTestata;
                }
                else//SCARTO
                {
                    log.Error($"WRONG CF");
                    log.Error(result);
                    return false;
                }
            }
            else
            {
                log.Warn("CF/PIVA in maschera was empty, take from SFA");
            }

            //int result = Keanu.CompareTo(r.CodiceFiscale, dati.CodiceFiscaleTestata);
            //if (result.Equals(0))
            //{
            //    //CF OK
            //}
            //else if (result <= 2)
            //{
            //    log.Warn($"A LITTLE BIT WRONG CF");
            //    log.Warn(result);
            //    r.CodiceFiscale = dati.CodiceFiscaleTestata;
            //}
            //else//SCARTO
            //{
            //    log.Error($"WRONG CF");
            //    log.Error(result);
            //    return false;
            //}

            if (!string.IsNullOrEmpty(dati.CodiceAnagraficaCliente))
                r.CodiceAnagraficaCliente = dati.CodiceAnagraficaCliente;
            if (!string.IsNullOrEmpty(dati.IdRecordSiebel))
                r.IdRecordSiebel = dati.IdRecordSiebel;
            if (!string.IsNullOrEmpty(dati.CodiceFiscaleTestata))
                r.CodiceFiscale = dati.CodiceFiscaleTestata;
            if (!string.IsNullOrEmpty(dati.PartitaIvaTestata))
                r.PartitaIvaTestata = dati.PartitaIvaTestata;
            log.Debug($"POD: {r.Pod}");
            log.Debug($"Codice Anagrafica Cliente: {r.CodiceAnagraficaCliente}");
            log.Debug($"Id Record Siebel: {r.IdRecordSiebel}");
            log.Debug($"Codice Fiscale: {r.CodiceFiscale}");
            log.Debug($"Partita Iva: {r.PartitaIvaTestata}");
            return true;
        }

        public bool RicercaServiziEBeni(string valore) //need valore
        {
            bool esito = true;
            string chiave = "Servizi e Beni";
            try
            {
                Keanu.WaitingGame();
                IWebElement ElementCercaSfa = null;
                while (ElementCercaSfa == null)
                {
                    try
                    {
                        ElementCercaSfa = Keanu.Driver.FindElement(By.XPath("//input[@class = 'slds-input slds-text-color_default slds-p-left--none slds-size--1-of-1 input default input uiInput uiInputTextForAutocomplete uiInput--{remove}']"));
                    }
                    catch { }
                }
                IJavaScriptExecutor javascript = Keanu.Driver as IJavaScriptExecutor;
                IWebElement ElementRicerca = null;
                while (ElementRicerca == null || (ElementRicerca.GetAttribute("Title") != chiave))
                {
                    var attivita = Keanu.Driver.FindElements(By.XPath("//span[@title = \"Servizi e Beni\"]"));
                    if (attivita.Count < 1)
                    {
                        IWebElement ElementFilterSearch = Keanu.Driver.FindElement(By.XPath("//div[@class = 'slds-combobox__form-element slds-input-has-icon slds-input-has-icon_right']"));
                        ElementFilterSearch.Click();
                        Thread.Sleep(Keanu.Randy(3));
                        IWebElement ElementTuttiGliElementiRicercabili = null;
                        int cunt = 0;
                        while (ElementTuttiGliElementiRicercabili == null && cunt != 3)
                        {
                            cunt++;
                            if (cunt == 3)
                            {
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
                            try
                            {
                                ElementTuttiGliElementiRicercabili = Keanu.Driver.FindElement(By.XPath("//ul[@aria-label = 'Tutti gli elementi ricercabili']"));
                            }
                            catch
                            {
                                try
                                {
                                    Thread.Sleep(Keanu.Randy(5));
                                    ElementFilterSearch = Keanu.Driver.FindElement(By.XPath("//div[@class = 'slds-combobox__form-element slds-input-has-icon slds-input-has-icon_right']"));
                                    ElementFilterSearch.Click();
                                    Thread.Sleep(Keanu.Randy(5));
                                }
                                catch
                                {
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
                    attivita = Keanu.Driver.FindElements(By.XPath("//span[@title = \"Servizi e Beni\"]"));
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
            }
            catch
            {
                Keanu.Driver.Navigate().Refresh();
                Keanu.WaitingGame();
                Keanu.WaitingGame();
                 Keanu.WaitingGame();
                Thread.Sleep(Keanu.Randy(15));
                return false;
            }
        }

        public bool RicercaAsset(string valore) {
            bool esito = true;
            string chiave = "Asset";
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
                    var attivita = Keanu.Driver.FindElements(By.XPath("//span[@title = \"Asset\"]"));
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
                    attivita = Keanu.Driver.FindElements(By.XPath("//span[@title = \"Asset\"]"));
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

        private bool GetNumeroClienteBySZ()
        {
            try
            {
                List<Offerta> listaOfferte = (List<Offerta>)sfaLib.SearchGenerico(r.SZ, SFALibrary.Helpers.Utility.RicercaOfferteName);
                string idRichiesta;
                string numeroRichiesta;
                if (listaOfferte == null || listaOfferte.Count() == 0)
                    return true;
                else
                {
                    idRichiesta = listaOfferte[0].IdRichiesta;
                    numeroRichiesta = listaOfferte[0].NumeroRichiesta;
                }

                UnlockAndCloseAllTabs();
                Keanu.Driver.Navigate().GoToUrl($"https://enelcrmt.lightning.force.com/lightning/r/Case/{idRichiesta}/view");
                Keanu.WaitingGame();

                IWebElement divElementiOrdine = Keanu.Driver.FindElement(By.XPath("//div[contains(text(), \"Elementi dell'Ordine\")]"));
                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", divElementiOrdine);
                Keanu.WaitingGame();
                var mediumElements = Keanu.Driver.FindElements(By.XPath("//div[@class = 'slds-p-around-medium slds-show']")).FirstOrDefault();
                var elements = Keanu.Driver.FindElements(By.XPath("//div[@class = 'slds-p-top_xx-small']"));

                IWebElement elementiDellOrdine = elements.Where(c => c.Text.Contains("Tipo: COMMODITY")).First();
                if (!elementiDellOrdine.Text.Contains("Tipo: COMMODITY"))
                    return false;

                IWebElement oiLink = elementiDellOrdine.FindElement(By.TagName("a"));
                string oiText = oiLink.Text.Trim();

                ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("arguments[0].click();", oiLink);
                Keanu.WaitingGame();
                Keanu.WaitingGame();//EXTRA BECAUSE OF CATCH

                IWebElement labelNumeroCliente = Keanu.Driver.FindElements(By.XPath(".//span[. = 'Numero cliente']/..")).Last();
                //IWebElement siblingDiv = labelNumeroCliente.FindElement(By.XPath("following-sibling::div"));
                var hhh = labelNumeroCliente.FindElements(By.XPath("//span/slot[1]/lightning-formatted-text"))[2];

                //IWebElement labelNumeroCliente = Keanu.Driver.FindElements(By.XPath(".//span[. = 'Numero cliente']/..")).Last();
                //IWebElement siblingDiv = labelNumeroCliente.FindElement(By.XPath("following-sibling::div"));
                //var hhh = siblingDiv.FindElements(By.XPath("//span/slot/slot"))[5];
                //var hhh = siblingDiv.FindElement(By.XPath("//span/slot/lightning-formatted-text"));

                if (string.IsNullOrEmpty(hhh.Text.Trim()) || hhh.Text.Trim().Length != 9)
                    return false;

                r.NumeroCliente = hhh.Text.Trim();
                return true;
            }
            catch (Exception Ex)
            {
                needToRestart = true;
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

        private IWebElement TrovaTableResult()
        {
            IWebElement tableResult = null;
            while (tableResult == null)
            {
                try
                {
                    tableResult = GetSezioneAttiva().FindElement(By.XPath(".//table[@class = 'slds-table forceRecordLayout slds-table--header-fixed slds-table--edit slds-table--bordered resizable-cols slds-table--resizable-cols uiVirtualDataTable']"));
                }
                catch { }
                if (tableResult != null)
                    break;
                try
                {
                    tableResult = GetSezioneAttiva().FindElement(By.XPath(".//table[@class = 'slds-table forceRecordLayout slds-table--header-fixed slds-table--edit slds-table--bordered resizable-cols slds-table--resizable-cols uiVirtualDataTable slds-no-cell-focus']"));
                }
                catch { }
            }
            return tableResult;
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
                        //SwitchToDefaultContent();
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
                needToRestart = true;
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
                    if (Iframe || isNotSezioneAttiva)
                    {
                        contentText = Keanu.Driver.PageSource.ToString();
                    }
                    else
                    {
                        contentText = GetSezioneAttiva().Text;
                    }
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

        public bool CheckPulsanteAbilitatoByClassTextButton(string classe, string textButton)
        {
            try
            {
                IWebElement button;
                if (Iframe)
                    button = Keanu.Driver.FindElement(By.XPath($".//button[@class = '{classe}' and contains(text(), '{textButton}')]"));
                else
                    button = GetSezioneAttiva().FindElement(By.XPath($".//button[@class = '{classe}' and contains(text(), '{textButton}')]"));
                return button.Enabled;
            }
            catch (Exception Ex)
            {
                log.Error(Ex.ToString());
                return false;
            }
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
                CompilaCampi("* POD/PDR/COD.MIGRAZIONE  ", "", Constant.TipoCampo.INPUT);
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
                                esito = CompilaCampi("* POD", pod, Constant.TipoCampo.INPUT);
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
                log.Warn(Ex.ToString());
                needToRestart = true;
                return false;
            }
            return true;
        }

        public bool DeleteSht(string item)
        {
            try
            {
                GoUP();
                if (Keanu.Driver.FindElement(By.Id(item)).GetAttribute("value").Length > 1)
                {
                    var cl = Keanu.Driver.FindElement(By.Id(item));
                    var del = cl.FindElement(By.XPath("..//span[@title= 'Click to delete value...']"));
                    Actions a = new Actions(Keanu.Driver);
                    a.MoveToElement(cl).Perform();
                    Thread.Sleep(500);
                    a.Click(del).Perform();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteClienteSfa()
        {
            try
            {
                GoUP();
                if (Keanu.Driver.FindElement(By.Id("customerLookUp")).GetAttribute("value").Length > 1)
                {
                    var cl = Keanu.Driver.FindElement(By.Id("customerLookUp"));
                    var del = cl.FindElement(By.XPath("..//span[@title= 'Click to delete value...']"));
                    Actions a = new Actions(Keanu.Driver);
                    a.MoveToElement(cl).Perform();
                    Thread.Sleep(500);
                    a.Click(del).Perform();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteOfferta()
        {
            try
            {
                if (Keanu.Driver.FindElement(By.Id("quoteIdLookUp")).GetAttribute("value").Length > 2)
                {
                    var offerta = Keanu.Driver.FindElement(By.Id("quoteIdLookUp"));
                    var del = offerta.FindElement(By.XPath("..//span[@title= 'Click to delete value...']"));
                    Actions a = new Actions(Keanu.Driver);
                    a.MoveToElement(offerta).Perform();
                    Thread.Sleep(500);
                    a.Click(del).Perform();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool DeleteCaseItem()
        {
            try
            {
                if (Keanu.Driver.FindElement(By.Id("caseItemLookUp")).GetAttribute("value").Length > 2)
                {
                    var offerta = Keanu.Driver.FindElement(By.Id("caseItemLookUp"));
                    var del = offerta.FindElement(By.XPath("..//span[@title= 'Click to delete value...']"));
                    Actions a = new Actions(Keanu.Driver);
                    a.MoveToElement(offerta).Perform();
                    Thread.Sleep(500);
                    a.Click(del).Perform();
                }
                return true;
            }
            catch
            {
                return false;
            }
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
                    catch
                    {
                        log.Info("ButtonAnnulla");
                    }
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

        public void CloseSubItemExcept(string ItemName)
        {
            try
            {
                IWebElement DivNavigation = null;
                try
                {
                    DivNavigation = Keanu.Driver.FindElement(By.XPath("//div[@class='oneWorkspaceCollection']"));
                }
                catch (Exception)
                {
                    DivNavigation = Keanu.Driver.FindElements(By.XPath("//div[@class='tabContainer oneConsoleTabContainer slds-grid active navexConsoleTabContainer']")).Last();
                }
                var LstButtonClose = DivNavigation.FindElements(By.XPath(".//button[@class = 'slds-button slds-button_icon-x-small slds-button_icon-container']"));
                if (LstButtonClose.Count == 0)
                    LstButtonClose = DivNavigation.FindElements(By.XPath(".//button[@class = 'slds-button slds-button_icon slds-button_icon-x-small slds-button_icon-container']"));

                foreach (IWebElement ButtonClose in LstButtonClose) {
                    if (!ButtonClose.Text.Contains(ItemName)) {
                        Thread.Sleep(Keanu.Randy(1));
                        ButtonClose.Click();
                    }
                }

            }
            catch (Exception Ex)
            {
                log.Error($"CloseSubItemExcept() DivNavigation fail");
                throw;
            }
        }
    }
}