using log4net;
using Smart.Gravity.Client;
using Smart.Gravity.Model;
using Smart.Gravity.Model.GravityFile;
using Smart.Gravity.Model.Logger;
using Smart.Network.WebApi;
using Smart.Security.Model;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MARCUS.Utils
{
    public class GravityApiHelper
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(GravityApiHelper));

        private string _gravityUser = string.Empty;
        private string _gravityPassword = string.Empty;
        //private Factory _factory;
        private WebApiClient _webApiClient;

        //public GravityApiHelper(Factory factory)
        //{
        //    _factory = factory;
        //}

        public void SetGravityCredential(string username, string password)
        {
            _gravityUser = username;
            _gravityPassword = password;
        }

        public User User { get; set; }

        public User GravitySetup()
        {
            try
            {
                GravityManagement.Setup();
                User = GravityManagement.Login(_gravityUser, _gravityPassword);
                if (User == null) { throw new Exception(); }
                GravityManagement.Loop(true);
                GravityManagement.StartedApp();
            }
            catch (Exception e) { _log.Debug(e.Message); throw new Exception(e.Message); };

            return User;
        }

        public void GravityConnection(int idDettaglioTipoLavorazione)
        {
            GravityManagement.CloseConnection();
            if (!GravityManagement.CreateConnection(idDettaglioTipoLavorazione))
                throw new Exception("Impossibile effettuare la connessione in Gravity nella coda di lavorazione") { };
        }

        public void GravityConnectionClose()
        {
            GravityManagement.CloseConnection();
        }

        public Cascata GravityScoda()
        {
            Cascata cascata = null;
            try
            {
                cascata = GravityManagement.GetNext();
            }
            catch (Exception e) { _log.Error(e.Message); }

            return cascata;
        }

        public void GravityRegistra(Cascata nuovo, string numeroCliente, int idDettReso, string note = "", int profilo = 0)
        {
            bool esito = true;
            int numeroTentativi = 0;
            do
            {
                if (numeroTentativi > 0) { Thread.Sleep(30000); }
                try
                {
                    esito = GravityManagement.RegistraLavorato(
                               new List<string>() { nuovo.Dettaglio },
                               nuovo.Dettaglio,
                               numeroCliente,
                               nuovo.DataRicAcq,
                               1,
                               idDettReso,
                               note,
                               profilo
                               );
                }
                catch (Exception e) { esito = false; _log.Error($"GravityException: {e.Message}"); }

                numeroTentativi++;

            } while (!esito && numeroTentativi <= 5);

            if (numeroTentativi > 5)
                throw new Exception($"Registrazione non andata a buon fine dell'attività: {nuovo.Dettaglio} - attività madre, verificare manualmente") { };

            _log.Info($"Registrazione avvenuta - idDettReso: {idDettReso}");
        }

        public void GravityRegistraSpecificForMultiModulo(string numeroCliente, Cascata nuovo, int idDettReso, string note = "", int profilo = 0)
        {
            bool esito = true;
            int numeroTentativi = 0;
            do
            {
                if (numeroTentativi > 0) { Thread.Sleep(30000); }
                try
                {
                    esito = GravityManagement.RegistraLavorato(
                               new List<string>() { numeroCliente },
                               numeroCliente,
                               nuovo.Dettaglio,
                               nuovo.DataRicAcq,
                               1,
                               idDettReso,
                               note,
                               profilo
                               );
                }
                catch (Exception e) { esito = false; _log.Error($"GravityException: {e.Message}"); }

                numeroTentativi++;

            } while (!esito && numeroTentativi <= 5);

            if (numeroTentativi > 5)
                throw new Exception($"Registrazione non andata a buon fine dell'attività: {numeroCliente} - attività madre, verificare manualmente") { };

            _log.Info($"Registrazione avvenuta - idDettReso: {idDettReso}");
        }

        public void GravitySbloccaSospesi(int sottoCoda, int idStatoSospensione)
        {
            try
            {
                bool esito = GravityManagement.SbloccaSospesiByIdDettTipoLavIdStatoSospensione(sottoCoda, idStatoSospensione);
                if (!esito)
                    throw new Exception("Sblocco sospesi non andato a buon fine");
            }
            catch (Exception e) { _log.Debug("Nessun sospeso"); }

        }

        public void GravitySospendi(int idStatoSospensione, bool sendMail = false, List<string> to = null, string corpo = "")
        {

            try
            {
                bool esito = GravityManagement.SospendiCurrentCascata(idStatoSospensione);
                if (!esito)
                    throw new Exception("Sospensione cascata non andata a buon fine");
            }
            catch (Exception e) { _log.Error(e.Message); throw new Exception(e.Message); }

            //if (sendMail)
            //{
            //    try
            //    {
            //        _factory.SendMail("Sospensione riferimento in lavorazione", corpo, to);
            //    }
            //    catch (Exception ex) { _log.Debug(ex.Message); }
            //}
        }

        private static readonly Dictionary<int, CambioPasswordEnel> _bufferCurrentUserCambioPasswordEnel = new Dictionary<int, CambioPasswordEnel>();

        public CambioPasswordEnel GravityLastPassByIdUser(int idUser)
        {
            try
            {
                if (!_bufferCurrentUserCambioPasswordEnel.ContainsKey(idUser)) { _bufferCurrentUserCambioPasswordEnel[idUser] = GravityManagement.GravityWebApi.Prepare().Get().QueryStringParam().Users().SetMethod("GetLastPasswordByIdUser").AddPar("idUser", idUser).Execute<CambioPasswordEnel>(); }
                return _bufferCurrentUserCambioPasswordEnel[idUser];
            }
            catch (Exception e) { _log.Error(e.Message); throw new Exception(e.Message); { } }
        }

        public User GravityGetUserByIdUser(int idUser)
        {
            try
            {
                return GravityManagement.GetUserById(idUser);
            }
            catch (Exception e) { _log.Error(e.Message); throw new Exception(e.Message) { }; }
        }

        public List<GravityDbLogTracking> GetTrackingLog(string riferimento)
        {
            try
            {
                return GravityManagement.GravityWebApi.Prepare().Get().QueryStringParam().Tracking().SetMethod("GetTrackingLog").AddPar("riferimento", riferimento).Execute<List<GravityDbLogTracking>>();
            }
            catch (Exception e) { _log.Error(e.Message); throw new Exception(e.Message) { }; }
        }

        public GravityRowFile GetElementRowFileByIdCascata(int idCascata)
        {
            GravityRowFile result = null;
            try
            {
                result = GravityManagement.GravityWebApi.Prepare().Get().QueryStringParam().GravityFile().SetMethod("GetJsonRowValueByIdCascata")
               .AddPar("idCascata", idCascata)
               .Execute<GravityRowFile>();
            }
            catch (Exception ex) { _log.Error(ex); return null; }
            return result;
        }

        public bool SaveRowFile(int idCascata, string idRiferimento, string jsonValue)
        {
            bool _result = false;
            try
            {
                _result = GravityManagement.GravityWebApi.Prepare()
                                                         .Get()
                                                         .QueryStringParam()
                                                         .GravityFile()
                                                         .SetMethod("SaveJsonRowValueByIdCascata")
                                                         .AddPar("idCascata", idCascata)
                                                         .AddPar("idRiferimento", idRiferimento)
                                                         .AddPar("jsonValue", jsonValue)
                                                         .Execute<bool>();
            }
            catch (Exception ex) { _log.Error(ex); return false; }
            return _result;
        }

        public bool SaveJsonRowValueByIdCascataByObject(GravityRowFile gravityRowFile)
        {
            bool _result = false;
            try
            {
                _result = GravityManagement.GravityWebApi.Prepare()
                                                         .Post()
                                                         .JsonParam()
                                                         .GravityFile()
                                                         .SetMethod("SaveJsonRowValueByIdCascataByObject")
                                                         .SetJSONParameter(gravityRowFile)
                                                         .Execute<bool>();
            }
            catch (Exception ex) { _log.Error(ex); return false; }
            return _result;
        }
    }
}