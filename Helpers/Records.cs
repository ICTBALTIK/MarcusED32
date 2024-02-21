using OpenQA.Selenium;
using System;

namespace MARCUS.Helpers
{
    public class Records
    {
        #region EE62
        public class VariablesEE62
        {
            public string Attivita { get; set; }
            public string SF { get; set; }
            public string DataAgente { get; set; }
        }

        public class RecordsEE62
        {
            public string Fattura { get; set; }
            public string Importo { get; set; }
            public string CanalePagamento { get; set; }
            public string NumeroCC { get; set; }
            public string Vcy { get; set; }
            public string Agenzia { get; set; }
            public string CodiceSportello { get; set; }
            public string Banca { get; set; }
            public string NumeroTransazione { get; set; }
            public string DescTipoPagamento { get; set; }
            public string DataPagamento { get; set; }
            public string NumeroRicevuta { get; set; }
            public string CF { get; set; }
            public string BP { get; set; }
        }
        #endregion

        #region EE176
        public class VariablesEE176
        {
            public string Attivita { get; set; }
            public string AttivitaSfa { get; set; }
            public string SF { get; set; }
            public string Causale = "Gestione Documenti";
            public string Descrizione = "Ricezione Documentazione";
            public string Specifica = "Contratto";
        }
        #endregion

        #region EE145
        public class VariablesEE145
        {
            public string Attivita { get; set; }
            public string AttivitaCopiata { get; set; }
            public string SF { get; set; }
            public string NumeroPagina { get; set; }
            public bool Riklasifi { get; set; }
            public bool AlreadyTripper { get; set; }
        }

        public class RecordsModuliLocal
        {
            public string DOC_TYPE { get; set; }
            public string SUCC_NUMBER { get; set; }
            public string RIFERIMENTO { get; set; }
            public string NUMERO_CLIENTE { get; set; }
            public string NUMERO_ORDINE { get; set; }
            public string FIRMA { get; set; }
            public string CODICE_FISCALE { get; set; }
            public string COMUNE_DOMICILIO { get; set; }
            public string PROVINCIA_DOMICILIO { get; set; }
            public string COMUNE_SEDE { get; set; }
            public string PROVINCIA_SEDE { get; set; }
            public string PARTITA_IVA { get; set; }
            public string TOPONOMASTICA_FORNITURA { get; set; }
            public string NOME_VIA_FORNITURA { get; set; }
            public string CIVICO_FORNITURA { get; set; }
            public string LOCALITA_FORNITURA { get; set; }
            public string COMUNE_FORNITURA { get; set; }
            public string CAP_COMUNE_FORNITURA { get; set; }
            public string QUALIFICA { get; set; }
            public string COMUNE_AMMINISTRATIVO { get; set; }
            public string COMUNE_CATASTALE { get; set; }
            public string CODICE_COMUNE_CATASTALE { get; set; }
            public string TIPO_UNITA { get; set; }
            public string SEZIONE { get; set; }
            public string FOGLIO { get; set; }
            public string PARTICELLA { get; set; }
            public string SUBALTERNO { get; set; }
            public string ESTENSIONE_PARTICELLA { get; set; }
            public string TIPO_PARTICELLA { get; set; }
            public string IMMOBILI_ESCLUSI { get; set; }
            public DateTime DATA { get; set; }
            public DateTime DATA_ATTIVAZIONE { get; set; }
            public DateTime DATA_INSERIMENTO { get; set; }
            public int ID_USER { get; set; }
        }

        public class RecordsEE145
        {
            public string NumeroCliente { get; set; }
            public string Qualifica { get; set; }
            public string ComuneCatastale { get; set; }
            public string TipoUnita { get; set; }
            public string Foglio { get; set; }
            public string EstensioneParticella { get; set; }
            public string Subalterno { get; set; }
            public string Firma { get; set; }
            public string ImmobiliEsclusi { get; set; }
            public string ComuneAmministrativo { get; set; }
            public string CodiceComuneCatastale { get; set; }
            public string Sezione { get; set; }
            public string Particella { get; set; }
            public string TipoParticella { get; set; }
            public DateTime Data { get; set; }
            public DateTime DataInserimento { get; set; }
            public string Quote { get; set; }
            public string NumeroContratto { get; set; }
            public string CodiceFiscale { get; set; } = "";
            public string CodiceIdSiebel { get; set; } = "";
            public string Cliente { get; set; }
            public string ClienteFromSFALibraryCheck { get; set; }
            public string CaseItem { get; set; }
            public string ConfigurationItem { get; set; }
            public string AttivitaServiziBeni { get; set; }
            public string Oggetto { get; set; }
            public string Testo { get; set; }
            public string Pod { get; set; }
            public string Piva { get; set; }
            public string ComuneDomicilio { get; set; }
            public string ProvinciaDomicilio { get; set; }
            public string ComuneSede { get; set; }
            public string ProvinciaSede { get; set; }
            public string TopomasticaFornitura { get; set; }
            public string NomeViaFornitura { get; set; }
            public string CivicoFornitura { get; set; }
            public string LocalitaFornitura { get; set; }
            public string ComuneFornitura { get; set; }
            public string CapComuneFornitura { get; set; }
            public string IdUser { get; set; }
            public string Worktype { get; set; }
            public string AttivitaPerCommento { get; set; }
            public string Act { get; set; }
            public string SZ { get; set; }
            public string CodiceAnagraficaCliente { get; set; }
            public string IdRecordSiebel { get; set; }
            public string CodiceFiscaleTestata { get; set; }
            public string PartitaIvaTestata { get; set; }
            public string TipologiaCliente { get; set; }
        }
        #endregion

        #region SCODAMENTO
        public class VariablesSCODAMENTO
        {
            public string Attivita { get; set; }
            public string OldAttivita { get; set; }
            public string OldIdAttivitaPerScodamento { get; set; }
            public string DataRic { get; set; }
            public string SF { get; set; }
        }
        #endregion

        #region EE112DPC
        public class VariablesEE112DPC
        {
            public string Riferimento { get; set; }
            public string Pod { get; set; }
            public string DatiMaschera { get; set; }
            public string Data { get; set; }
        }

        public class ArchivioScartiLavorabili
        {
            public IWebElement Checkbox { get; set; }
            public string Pod { get; set; }
            public DateTime DataLettura { get; set; }
            public string PresaInCarico { get; set; }
            public string TipoLettura { get; set; }
            public double F1 { get; set; }
            public double F2 { get; set; }
            public double F3 { get; set; }
            public double R1 { get; set; }
            public double R2 { get; set; }
            public double R3 { get; set; }
            public double Potenza { get; set; }
            public DateTime DataCaricamento { get; set; }
            public string CodiceScarto { get; set; }
            public string Repository { get; set; }
            public int Days { get; set; }
        }

        public class ListaLetture
        {
            public IWebElement Gestione { get; set; }
            public DateTime Data { get; set; }
            public string Causale { get; set; }
            public string Tipo { get; set; }
            public string Fonte { get; set; }
            public string Stato { get; set; }
            public double F1 { get; set; }
            public double F2 { get; set; }
            public double F3 { get; set; }
            //public double TotaleF { get; set; }
            public double R1 { get; set; }
            public double R2 { get; set; }
            public double R3 { get; set; }
            //public double TotaleR { get; set; }
            public double Potenza { get; set; }
            public double P1 { get; set; }
            public double P2 { get; set; }
            public double P3 { get; set; }
            public string Rowland { get; set; }
            public bool ReattivaIsNull { get; set; }
            public bool PotenzaDTIsNull { get; set; }
            public string StatoInvioSap { get; set; }
        }

        public class ElencoRisultati
        {
            public string POD { get; set; }
            public string NumeroUtente { get; set; }
            public string RagioneSociale { get; set; }
            public string DataAttivazione { get; set; }
            public string DataCessazione { get; set; }
            public string StatoFornitura { get; set; }
            public string Distributore { get; set; }
        }

        public class Flussi
        {
            public string Motivazione { get; set; }
            public string Trattamento { get; set; }
            public string Raccolta { get; set; }
            public string TipoDato { get; set; }
            public string Misuratore { get; set; }
            public string ValidatoDistributore { get; set; }
            public string Uid { get; set; }
            public IWebElement CheckBox { get; set; }
            public string CodiceFlusso { get; set; }
            public string MeseAnno { get; set; }
            public DateTime DataMisura { get; set; }
            public DateTime DataCaricamento { get; set; }
            public DateTime DataRipristino { get; set; }
            public IWebElement Gestione { get; set; }
            public IWebElement Pljus { get; set; }
            public string NomeFile { get; set; }
            public DateTime DataPubblicazione { get; set; }
            public string Tipo { get; set; }
            public string Stato { get; set; }
            public string StatoAbb { get; set; }
            public double F1 { get; set; }
            public double F2 { get; set; }
            public double F3 { get; set; }
            public double R1 { get; set; }
            public double R2 { get; set; }
            public double R3 { get; set; }
            public string TipoRettifica { get; set; }
            public string ValidatoDT { get; set; }
            public string Esito { get; set; }
            public string CodiceEsito { get; set; }
            public string Repository { get; set; }
            public string Rowland { get; set; }
            public bool InfoSymbol { get; set; }
            public IWebElement WholeRowElement { get; set; }
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

        public class Flussi2G
        {
            public string Uid { get; set; }
            public IWebElement Gestione { get; set; }
            public IWebElement Pljus { get; set; }
            public string CodiceFlusso { get; set; }
            public string MeseAnno { get; set; }
            public DateTime DataMisura { get; set; }
            public string NomeFile { get; set; }
            public DateTime DataPubblicazione { get; set; }
            public DateTime DataCaricamento { get; set; }
            public string Tipo { get; set; }
            public string Stato { get; set; }
            public string StatoAbb { get; set; }
            public double F1 { get; set; }
            public double F2 { get; set; }
            public double F3 { get; set; }
            public double R1 { get; set; }
            public double R2 { get; set; }
            public double R3 { get; set; }
            public string TipoRettifica { get; set; }
            public string Motivazione { get; set; }
            public string ValidatoDT { get; set; }
            public string Esito { get; set; }
            public string CodiceEsito { get; set; }
            public string Repository { get; set; }
            public string Rowland { get; set; }
            public bool InfoSymbol { get; set; }
            public IWebElement WholeRowElement { get; set; }
        }

        public class Apparecchiature
        {
            public DateTime? DataInizio { get; set; }
            public DateTime? DataFine { get; set; }
            public string TipoApparecchiatura { get; set; }
            public string MatricolaApparecchiatura { get; set; }
            public string TwoG { get; set; }
            public IWebElement WholeRowElement { get; set; }
        }

        public class PostMalone
        {
            public DateTime? DataProcesso { get; set; }
        }

        public class AnagraficaTecnica
        {
            public DateTime? DataProcesso { get; set; }
        }

        public class Smis
        {
            public int Id { get; set; }
            public int IdUser { get; set; }
            public DateTime Data { get; set; }
            public string Pod { get; set; }
            public string Motivazione { get; set; }
            public DateTime DataMontaggio { get; set; }
            public DateTime DataSmontaggio { get; set; }
            public string TipoLettura { get; set; }
            public string TipoMisuratoreSmontaggio { get; set; }
            public string CodiceTipoMisuratoreSmontaggio { get; set; }
            public string Ea0Smontaggio { get; set; }
            public string Ea1Smontaggio { get; set; }
            public string Ea2Smontaggio { get; set; }
            public string Ea3Smontaggio { get; set; }
            public string Ea4Smontaggio { get; set; }
            public string Ea5Smontaggio { get; set; }
            public string Ea6Smontaggio { get; set; }
            public string Er0Smontaggio { get; set; }
            public string Er1Smontaggio { get; set; }
            public string Er2Smontaggio { get; set; }
            public string Er3Smontaggio { get; set; }
            public string Er4Smontaggio { get; set; }
            public string Er5Smontaggio { get; set; }
            public string Er6Smontaggio { get; set; }
            public string P0Smontaggio { get; set; }
            public string P1Smontaggio { get; set; }
            public string P2Smontaggio { get; set; }
            public string P3Smontaggio { get; set; }
            public string P4Smontaggio { get; set; }
            public string P5Smontaggio { get; set; }
            public string P6Smontaggio { get; set; }
            public DateTime DataMessaRegime { get; set; }
            public string TipoMisuratoreMontaggio { get; set; }
            public string CodiceTipoMisuratoreMontaggio { get; set; }
            public string Matricola { get; set; }
            public string Cifre { get; set; }
            public string Tensione { get; set; }
            public string Ka { get; set; }
            public string Kr { get; set; }
            public string Kp { get; set; }
            public string Ea0Montaggio { get; set; }
            public string Ea1Montaggio { get; set; }
            public string Ea2Montaggio { get; set; }
            public string Ea3Montaggio { get; set; }
            public string Ea4Montaggio { get; set; }
            public string Ea5Montaggio { get; set; }
            public string Ea6Montaggio { get; set; }
            public string Er0Montaggio { get; set; }
            public string Er1Montaggio { get; set; }
            public string Er2Montaggio { get; set; }
            public string Er3Montaggio { get; set; }
            public string Er4Montaggio { get; set; }
            public string Er5Montaggio { get; set; }
            public string Er6Montaggio { get; set; }
            public string P0Montaggio { get; set; }
            public string P1Montaggio { get; set; }
            public string P2Montaggio { get; set; }
            public string P3Montaggio { get; set; }
            public string P4Montaggio { get; set; }
            public string P5Montaggio { get; set; }
            public string P6Montaggio { get; set; }
            public string IdAgente { get; set; }
            public string NomeFile { get; set; }
            public int Stato { get; set; }
        }
        #endregion

        #region EE366
        public class DatiR2D
        {
            public string Pod { get; set; }
            public string IdCrm { get; set; }
            public string RagioneSociale { get; set; }
            public string PIVA { get; set; }
            public string CF { get; set; }
            public string Distributore { get; set; }
            public string CodiceServizioDt { get; set; }
            public string NuovoIntestatarioCF { get; set; }
            public string NuovoIntestatarioPIVA { get; set; }
            public string AttualePotenzaDisponibile { get; set; }
            public string AttualePotenzaInFranchigia { get; set; }
            public string TipoMisuratore { get; set; }
            public string TensioneRichiesta { get; set; }
            public DateTime DataLavorazione { get; set; }
            public IWebElement Azione { get; set; }
        }

        public class DatiFOUR
        {
            public string DataOraNotifica { get; set; }
            public string NotificaFour { get; set; }
            public string ESITO { get; set; }
            public string COD_PRAT_DISTR { get; set; }
            public string DATA_ESECUZIONE { get; set; }
            public string POT_DISP { get; set; }
            public string TENSIONE { get; set; }
        }

        public class DatiSII
        {
            public string DataInizio { get; set; }
            public string DataFine { get; set; }
            public string DataChiusura { get; set; }
            public string TariffaDistributore { get; set; }
            public string Protocollo { get; set; }
            public string CF { get; set; }
            public string PIVA { get; set; }
            public string DataDecVoltura { get; set; }
            public string DataEsito { get; set; }
        }
        #endregion

        #region SFACHECKER
        public class SfaChekah
        {
            public string Attivita { get; set; }
            public string SF { get; set; }
            public string StatoSfa { get; set; }
            public string DataA { get; set; }
            public string TipoSfa { get; internal set; }
            public string StatoAgente { get; set; }
            public string UserAgente { get; set; }
            public string Description { get; internal set; }
        }
        #endregion

        #region SIFITES

        public class VariablesSifites
        {

            public string Riferimento { get; set; }

            public string Nota { get; set; }


        }
        public class RecordsSifites
        {
            public string Id { get; set; }
            public string IdDocumento { get; set; }
            public string DataEmissione { get; set; }
            public string Importo { get; set; }
            public string Nota { get; set; }
            public string Fattura { get; set; }
            public string IdOperatore { get; set; }
            public string DataInserimento { get; set; }
            public string Eneltel { get; set; }

        }

        #endregion
    }
}