using log4net;
using Newtonsoft.Json;
using Smart.Gravity.Model;
using Smart.Gravity.Model.GravityFile;
using Smart.Security.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartAuto.Utils
{
    public class GravityManagementWebApi
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GravityManagementWebApi));
        public static GravityWepApiClient GravityWebApi { get; private set; }
        public static User CurrentUser { get; private set; }

        public static void Setup()
        {
            GravityWebApi = new GravityWepApiClient();
            GravityWebApi.Setup();
        }

        public static User Login(string username, string password)
        {
            try
            {
                //provo ad autenticarmi sul servizio API REST di Gravity
                GravityWebApi.JwtLogin(username, password);
                //recupero le informazioni dell'utente loggato
                User user = GravityWebApi.Prepare().Get().Users().SetMethod("GetAuthenticatedUser").Execute<User>();
                CurrentUser = user;
                //salvo l'informazione di monitoraggio dell'applicativo in utilizzo
                return user;
            }
            catch (Exception ex) { log.Error(ex); throw ex; }
        }

        //public static int CreaConnessione(int idUser, int idSottoCoda)
        //{
        //    try
        //    {
        //        int result = GravityWebApi.Prepare().Get().QueryStringParam().Runtime().SetMethod("CreaConnessione")
        //        .AddPar("idUser", idUser)
        //        .AddPar("idSottoCoda", idSottoCoda)
        //        .Execute<int>();
        //        return result;
        //    }
        //    catch (Exception ex) { log.Error(ex); return -1; }
        //}

        public static Cascata GravityScoda(int idConnesione)
        {
            Cascata cascata = null;
            try
            {
                cascata = Smart.Gravity.Client.GravityManagement.GetNext();
            }
            catch (Exception e) { log.Error(e.Message); }
            return cascata;

            //string result = "";
            //try
            //{
            //    result = GravityWebApi.Prepare().Get().QueryStringParam().Production().SetMethod("GetNext")
            //    .AddPar("idConnessione", idConnesione)
            //    .Execute<string>();
            //}
            //catch (Exception e) { log.Error(e.Message); }
            //return result;
        }

        public static bool RegistraLavorato(int idConnessione, List<string> riferimenti, string riferimentoBase, string numeroCliente, DateTime dataRicAcq, int esaminati, int idSottoReso, string note = "", int idProfiloDaAlimentare = 0)
        {
            try
            {
                bool risp = GravityWebApi.Prepare().Get().QueryStringParam().Production().SetMethod("RegistraLavorato")
                    .AddPar("idConnessione", idConnessione)
                    .AddPar("riferimentiSerializzato", JsonConvert.SerializeObject(riferimenti))
                    .AddPar("riferimentoBase", riferimentoBase)
                    .AddPar("numeroCliente", numeroCliente)
                    .AddPar("dataRicAcq", dataRicAcq.ToShortDateString() + " " + dataRicAcq.ToLongTimeString())
                    .AddPar("esaminati", esaminati)
                    .AddPar("idSottoReso", idSottoReso)
                    .AddPar("note", note)
                    .AddPar("isInToj", false)
                    .AddPar("idProfiloDaAlimentare", idProfiloDaAlimentare)
                    .Execute<bool>();
                return risp;
            }
            catch (Exception ex) { throw ex; }
        }

        public static List<GravityRowFile> GetJsonRowValueByRiferimentoAndSottocoda(string riferimento, int idSottoCoda)
        {
            List<GravityRowFile> result = null;
            try
            {
                result = GravityWebApi.Prepare().Get().QueryStringParam().GravityFile().SetMethod("GetJsonRowValueByRiferimentoAndSottocoda")
               .AddPar("riferimento", riferimento)
               .AddPar("idSottoCoda", idSottoCoda)
               .Execute<List<GravityRowFile>>();
            }
            catch (Exception ex) { log.Error(ex); return null; }

            return result;
        }

        public static int GetIdSottoResoByRiferimento(string riferimento)
        {

            int result = -1;
            try
            {
                result = GravityWebApi.Prepare().Get().QueryStringParam().Tracking().SetMethod("GetIdSottoResoByRiferimento")
               .AddPar("riferimento", riferimento)
               .Execute<int>();
            }
            catch (Exception ex) { log.Error(ex); return -1; }

            return result;
        }

        public static List<Smart.Gravity.Model.Logger.GravityDbLogTracking> GetTrackingLog(string riferimento)
        {
            List<Smart.Gravity.Model.Logger.GravityDbLogTracking> result = null;
            try
            {
                result = GravityWebApi.Prepare().Get().QueryStringParam().Tracking().SetMethod("GetTrackingLog")
               .AddPar("riferimento", riferimento)
               .Execute<List<Smart.Gravity.Model.Logger.GravityDbLogTracking>>();
            }
            catch (Exception ex) { log.Error(ex); return null; }

            return result;
        }

        public static DateTime GetServerDateTime()
        {
            try
            {
                DateTime result = GravityWebApi.Prepare().Get().Utility().SetMethod("GetServerDateTime")
                .Execute<DateTime>();
                return result;
            }
            catch (Exception ex) { log.Error(ex); return DateTime.Now; }
        }

        public static int GetIdSottoResoByLavorato(string riferimento, int idSottoCoda)
        {

            int result = -1;
            try
            {
                result = GravityWebApi.Prepare().Get().QueryStringParam().Tracking().SetMethod("GetIdSottoResoByLavorato")
               .AddPar("riferimento", riferimento)
                .AddPar("idSottoCoda", idSottoCoda)
               .Execute<int>();
            }
            catch (Exception ex) { log.Error(ex); return -1; }

            return result;
        }

        public static bool UpdateJsonRowValueById(int idRow, string jsonValue)
        {
            bool _result = false;
            try
            {
                _result = GravityWebApi.Prepare().Get().QueryStringParam().GravityFile().SetMethod("UpdateJsonRowValueById")
               .AddPar("idRow", idRow)
               .AddPar("jsonValue", jsonValue)
               .Execute<bool>();
            }
            catch (Exception ex) { log.Error(ex); return false; }
            return _result;
        }

        //public static GravityRowFile GetJsonRowValueByIdCascata(int idCascata)
        //{
        //    GravityRowFile result = null;
        //    try
        //    {
        //        result = GravityWebApi.Prepare().Get().QueryStringParam().GravityFile().SetMethod("GetJsonRowValueByIdCascata")
        //        .AddPar("idCascata", idCascata)
        //        .Execute<GravityRowFile>();
        //    }
        //    catch (Exception ex) { log.Error(ex); return null; }

        //    return result;
        //}

        //public static bool SaveJsonRowValueByIdCascata(int idCascata, string idRiferimento, string jsonValue)
        //{
        //    bool _result = false;
        //    try
        //    {
        //        _result = GravityWebApi.Prepare().Get().QueryStringParam().GravityFile().SetMethod("SaveJsonRowValueByIdCascata")
        //        .AddPar("idCascata", idCascata)
        //        .AddPar("idRiferimento", idRiferimento)
        //        .AddPar("jsonValue", jsonValue)
        //        .Execute<bool>();
        //    }
        //    catch (Exception ex) { log.Error(ex); return false; }
        //    return _result;
        //}

        //public static GravityRowFile GetJsonRowValueByIdCascata(int idCascata)
        //{
        //    GravityRowFile result = null;
        //    try
        //    {
        //        result = GravityWebApi.Prepare().Get().QueryStringParam().GravityFile().SetMethod("GetJsonRowValueByIdCascata")
        //       .AddPar("idCascata", idCascata)
        //       .Execute<GravityRowFile>();
        //    }
        //    catch (Exception ex) { log.Error(ex); return null; }
        //    return result;
        //}

        //public static bool SaveJsonRowValueByIdCascata(int idCascata, string idRiferimento, string jsonValue)
        //{
        //    bool _result = false;
        //    try
        //    {
        //        _result = GravityWebApi.Prepare().Get().QueryStringParam().GravityFile().SetMethod("SaveJsonRowValueByIdCascata")
        //       .AddPar("idCascata", idCascata)
        //       .AddPar("idRiferimento", idRiferimento)
        //       .AddPar("jsonValue", jsonValue)
        //       .Execute<bool>();
        //    }
        //    catch (Exception ex) { log.Error(ex); return false; }
        //    return _result;
        //}

    }
}