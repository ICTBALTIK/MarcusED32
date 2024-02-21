using log4net;
using MARCUS.Helpers;
using SFALibrary;
using SFALibrary.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace MARCUS.Marcosi
{
    class SFACHECKER
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SCODAMENTO));
        public Keanu Keanu { get; set; }
        public SqlQuery query = new SqlQuery();

        public SFACHECKER(Keanu keanu)
        {
            this.Keanu = keanu;
        }

        public SfaLib sfaLib { get; set; }

        public List<Records.SfaChekah> v = new List<Records.SfaChekah>();

        private List<string> aktjivitiis = new List<string>();

        public bool Flow()
        {
            if (Keanu.LavName.Equals("SFA CHECKER AGENTE"))
                Keanu.SfaCheckerPlusAgente = true;//AGENTE Edition
            else
                Keanu.SfaCheckerPlusAgente = false;//SF Editon

            string user = Environment.UserName.ToUpper();
            string path = $"C:\\Users\\{user}\\Desktop\\MARCUSSF\\";
            ExcelManipulations ex = new ExcelManipulations();
            ex.ImportExcel(out aktjivitiis);
            if (aktjivitiis == null)
            {
                log.Error($"null");
                return false;
            }
            if (aktjivitiis.Count > 0)
                log.Debug($"{aktjivitiis.Count} attività");
            else
            {
                log.Error($"{aktjivitiis.Count} attività");
                return false;
            }

            if (!PepperYourSfaLib())
                return false;

            foreach (var act in aktjivitiis)
            {
                try
                {
                    string attt = act;
                    if (attt.Contains('$'))
                        attt = attt.Split('$').First();
                    string sfff = "-";
                    string statoSfa = "";
                    string tipoSfa = "";
                    string statoAgente = "";
                    string userAgente = "";
                    string desc_altro = "";
                    string dataAppertura = "";

                    if (sfaLib == null)
                    {
                        if (!PepperYourSfaLib())
                            return false;
                    }

                    int cnt = 0;

                    Attivita att = new Attivita();
                    cnt = 0;
                    do
                    {
                        if (cnt > 1)
                            Thread.Sleep(Keanu.Randy(5));
                        att = sfaLib.SearchAttivita(attt, true);
                        Thread.Sleep(500);
                        cnt++;
                    } while (cnt < 3 && att == null);

                    if (att == null)
                    {
                        if (!PepperYourSfaLib())
                        {
                            log.Error($"Sfalib errore");
                            Thread.Sleep(Keanu.Randy(10));
                            continue;
                        }
                        att = sfaLib.SearchAttivita(attt, true);
                        if (att == null)
                        {
                            log.Error($"Sfalib errore");
                            Thread.Sleep(Keanu.Randy(10));
                            continue;
                        }
                    }

                    statoSfa = att.Stato;
                    tipoSfa = att.Tipo;
                    dataAppertura = att.DataInizio;

                    Keanu.IdAttivitaPerScodamento = att.RecordId;

                    try
                    {
                        Documento documento = (Documento)sfaLib.GetRecord(att.DocumentRecordId, SFALibrary.Helpers.Utility.RicercaDocumentoName);
                        if (documento.Name != null)
                            sfff = documento.Name.Trim();

                        if (sfff.Length < 3)
                            sfff = "-";
                    }
                    catch (Exception Ex)
                    {
                        sfff = "-";
                    }

                    if (Keanu.SfaCheckerPlusAgente)
                    {
                        USERDataContext yser = new USERDataContext();

                        DALAVORAREDataContext dalavorare = new DALAVORAREDataContext();
                        var daLavorareList = (from f in dalavorare.PROD_VOLUMI_DA_LAVORAREs where f.DETTAGLIO == act select f.ID_STATO_SOSPENSIONE).Take(1).ToList();
                        if (daLavorareList.Count().Equals(0))
                        {
                            PROD_RESIDataContext prodresi = new PROD_RESIDataContext();
                            var prodResiList = (from f in prodresi.PROD_RESIs where f.DETTAGLIO == act orderby f.ID descending select f.ID_USER).Take(1).ToList();//If dalavorare is empty, order prodresi desc to get last agente row
                            if (prodResiList.Count().Equals(0))
                            {

                                LavarazioneDataContext lavarazione = new LavarazioneDataContext();
                                var lavList = (from f in lavarazione.LAVORAZIONI_AGENTEs where f.ID_RIFERIMENTO == act orderby f.ID descending select f.MATR_OPERATORE).Take(1).ToList();
                                if (lavList.Count().Equals(0))
                                {
                                    statoAgente = "NESSUN INFORMATION";
                                }
                                else
                                {
                                    statoAgente = "FATTO";
                                    var deskriptions = (from f in lavarazione.LAVORAZIONI_AGENTEs where f.ID_RIFERIMENTO == act && f.MATR_OPERATORE == lavList[0] select f.ID_CODICE_RESO).Take(1).ToList();//i dont know wtf should work and wroks
                                    var reso = deskriptions[0];
                                    var suns = (from f in prodresi.PROD_RESIs where f.ID_DESCRIZIONE_RESO == reso select f.DESCR_ALTRO).Take(1).ToList();
                                    desc_altro = suns[0];


                                    var yserList = (from f in yser.USERs where f.MATRICOLA == lavList[0] select f).FirstOrDefault();
                                    userAgente = $"{yserList.MATRICOLA} {yserList.NOME} {yserList.COGNOME}";
                                    if (!userAgente.Contains("smartbot"))
                                        userAgente = userAgente.ToUpper();
                                }

                            }
                            else
                            {
                                statoAgente = "FATTO";

                                var deskription = (from f in prodresi.PROD_RESIs where f.DETTAGLIO == act && f.ID_USER == prodResiList[0] select f.DESCR_ALTRO).Take(1).ToList();//If dalavorare is empty, order prodresi desc to get last agente row
                                desc_altro = deskription[0];

                                var yserList = (from f in yser.USERs where f.ID_USER == prodResiList[0] select f).FirstOrDefault();
                                userAgente = $"{yserList.MATRICOLA} {yserList.NOME} {yserList.COGNOME}";
                                if (!userAgente.Contains("smartbot"))
                                    userAgente = userAgente.ToUpper();
                            }
                        }
                        else
                        {
                            if (daLavorareList[0] > 0)
                            {
                                statoAgente = $"SOSPESO ID {daLavorareList[0]}";


                                var tmpUser = (from f in dalavorare.PROD_VOLUMI_DA_LAVORAREs where f.DETTAGLIO == act select f.ID_OPERATORE).Take(1).ToList();


                                if (tmpUser[0] == null)
                                {
                                    userAgente = "Operatore di altra sede";
                                }
                                else
                                {
                                    var yserList = (from f in yser.USERs where f.ID_USER == tmpUser[0] select f).FirstOrDefault();
                                  

                                    if (yserList == null) // ja USER table ir tukshs,tas nozimee ka operators neeksiste datu baazee
                                    {

                                        userAgente = "Operators neeksistee datubazee";
                                    }
                                    else
                                    {
                                        userAgente = $"{yserList.MATRICOLA} {yserList.NOME} {yserList.COGNOME}";
                                    }

                                    if (!userAgente.Contains("smartbot"))
                                    {
                                        userAgente = userAgente.ToUpper();
                                    }
                                }
                            }
                            else if (daLavorareList[0] == 0)
                            {
                                statoAgente = "IN LAVORAZIONE";
                            }
                        }

                        log.Info($"{act} {sfff} {statoSfa}");
                        log.Debug($"{statoAgente} {userAgente}");

                            v.Add(new Records.SfaChekah
                            {
                                Attivita = act,
                                SF = sfff,
                                StatoSfa = statoSfa,
                                TipoSfa = tipoSfa,
                                StatoAgente = statoAgente,
                                UserAgente = userAgente,
                                Description = desc_altro,
                                DataA = dataAppertura
                            });
                    }
                    else
                    {
                        log.Info($"{act} {sfff} {statoSfa}");

                            v.Add(new Records.SfaChekah
                            {
                                Attivita = act,
                                SF = sfff
                            });
                    }

                    Keanu.Bad.Fatto++;
                    if (Keanu.StartStop == false)
                        break;
                }
                catch (Exception Ex)
                {
                    log.Error($"Exception, wait and continue");
                    Thread.Sleep(Keanu.Randy(10));
                    continue;
                }
            }

            if (Keanu.SfaCheckerPlusAgente)
                ex.ExportExcelSFAgente(v, $"SF-{DateTime.Now:ddMM}{Keanu.Re(3)}");
            else
                ex.ExportExcelSF(v, $"SF-{DateTime.Now:ddMM}{Keanu.Re(3)}");

            try { Process.Start(path); } catch { };

            return false;
        }

        private bool PepperYourSfaLib()
        {
            sfaLib = new SfaLib(Keanu.LoginSFA, Keanu.PassSFA);
            bool marc = sfaLib.LoginProd();
            if (!marc) { marc = sfaLib.LoginProd(); }
            if (!marc) { log.Error("Login non riuscito su SFA con HTMlUnit."); return false; }
            return true;
        }
    }
}