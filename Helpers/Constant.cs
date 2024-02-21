using System.Collections.Generic;
using System.ComponentModel;

namespace MARCUS.Helpers
{
    public static class Constant
    {
        public static List<string> nomeProdotto = new List<string>
        {
            "100x100 GiustaXTe",
            "AE__B",
            "Certa Per Te Gas",
            "City Luce",
            "E-Light Gas_02",
            "e-light Luce Impresa",
            "E-Light_02",
            "Enel Tutto OK Gas",
            "Enel Tutto OK Luce",
            "Energia Pura Bioraria Extra",
            "Energia Pura Casa Extra",
            "Energia Pura Casa Special",
            "ENERGIA PURA CASA 360",
            "Energia Pura Casa Promo",
            "Energia Sicura Gas Extra",
            "Energia Sicura Gas Promo",
            "Energia Sicura Impresa",
            "Energia Sicura Luce Extra",
            "ENERGIA SICURA GAS 360",
            "SICURA GAS",
            "EnergiaX65 Gas",
            "EnergiaX65 Luce SENZA POLIZZA",
            "EnergiaX65 Luce",
            "Gas 20 NEW",
            "Gas 20",
            "Gas 30",
            "GAS",
            "Gas30",
            "GiustaXte Bioraria Impresa",
            "GiustaXte Bioraria",
            "GiustaXte Gas Impresa",
            "GIUSTAXTE GAS SPECIAL 3",
            "GiustaXte Gas",
            "GiustaXte Impresa",
            "GiustaXte Luce Impresa",
            "GiustaXte",
            "Luce 20",
            "Luce 20 NEW",
            "Luce 30",
            "Luce30",
            "NEW_Gas 30",
            "NEW_AE__B",
            "NEW_Anno Sicuro Smart Agent",
            "NEW_B1__R",
            "NEW_BL__R",
            "ANNO SICURO GAS CARING",
            "NEW_EL__R",
            "NEW_E-Light Bioraria_WEB",
            "NEW_E-Light Gas_WEB",
            "NEW_ELIGHT_BIO_PLUS__R",
            "NEW_ELIGHT_GAS_PLUS__R",
            "NEW_ELIGHT_PLUS__R",
            "NEW_INSIEME A NOI LUCE",
            "NEW_INSIEME A NOI GAS",
            "Placet RES Gas Fisso",
            "Placet RES Luce Fisso",
            "NEW_E-Light_WEB",
            "NEW_Energia Consumo Semplice",
            "NEW_Energia Flessibile",
            "NEW_ENERGIA PURA CASA PLUS",
            "NEW_ENERGIA PURA",
            "NEW_Energia Sicura Gas Plus_New",
            "NEW_Energia Sicura Gas_15 percento",
            "NEW_Energia Sicura Gas_New",
            "NEW_Energia Sicura Luce_New",
            "Energia Sicura Gas con Polizza",
            "ENERGIA PURA BIORARIA 360",
            "NEW_ESG_RVC_New",
            "NEW_Luce 30",
            "Cor_Soluzione Energia Impresa Business",
            "NEW_MS__B",
            "NEW_OFFERTA STANDARD aeeg",
            "NEW_OFFERTA STANDARD aeeg_BUS",
            "Cor_Trend Sicuro Energia Impresa Business",
            "NEW_PC__R",
            "NEW_SENZA ORARI LUCE",
            "New_Soluzione Energia Impresa Business",
            "NEW_TuttoCompreso Luce",
            "NEW_EnergiaTuttoCompreso",
            "New_Trend Sicuro Energia Impresa Business",
            "NUOVO IPEX TEMPORANEE",
            "OFFERTA_STANDARD",
            "OFFERTA_STANDArd",
            "ENEL ONE NEW",
            "OFFERTA STANDARD__B",
            "Open Energy Extra",
            "Open Energy Special 3",
            "Open Energy",
            "Open Energy Pmi",
            "ORE FREE EXTRA",
            "PER NOI E-Light_02",
            "PER NOI Bioraria_02",
            "Placet BUS Luce Indicizzato",
            "Porta i Tuoi Amici Bioraria 2",
            "Porta i Tuoi Amici Bioraria",
            "Porta i Tuoi Amici Gas 2",
            "Porta i Tuoi Amici Gas",
            "Porta i Tuoi Amici Luce 2",
            "Porta i Tuoi Amici Luce",
            "PRODOTTO_DEFAULT_16_18",
            "PRODOTTO_LARGE",
            "Scegli Tu 100x100",
            "SCEGLI TU EXTRA",
            "Scegli Tu Ore Free",
            "SCEGLI TU SPECIAL 3",
            "Scegli Tu",
            "SEMPLICE GAS",
            "SEMPLICE LUCE",
            "SCEGLI OGGI GAS",
            "SCEGLI OGGI LUCE",
            "SCEGLI OGGI LUCE 30",
            "SCEGLI OGGI LUCE 40",
            "SCEGLI OGGI WEB LUCE",
            "Open Energy Digital",
            "SCEGLI OGGI WEB GAS",
            "Sempre Con Te Impresa",
            "Sempre Con Te",
            "SoloXTe",
            "SoloXTe Gas",
            "Soluzione Energia Impresa Business",
            "New_Soluzione Gas Impresa Business",
            "NEW_Anno Sicuro Gas",
            "Soluzione Energia Impresa Smart",
            "PER NOI E-Light Gas_02",
            "Soluzione Energia Impresa x Te",
            "Soluzione Energia Stagione",
            "Speciale Gas 50 WEB",
            "Speciale Gas",
            "Speciale Luce 50 WEB",
            "Speciale Luce",
            "SpecialeGas25",
            "SpecialeGas30",
            "SpecialeLuce25",
            "SpecialeLuce30",
            "Trend Sicuro Energia Impresa new",
            "Valore Luce Plus",
            "WRMZh_19"
        };

        #region Enumeratori
        public enum TipoCampo { INPUT, COMBO, CHECKBOX, RADIO, DATA, INPUT_AND_UL, TEXT_AREA, COMODITY };
        public enum TipoFornitura { NESSUNA_FORNITURA, SELEZIONA_FORNITURA, MULTI_FORNITURA };
        public enum TipoScroll { SU, GIU, INIZIO, FINE };
        #endregion

        #region Variabili
        public const string CommentoInterazioneAnnullata = "Interazione annullata per errata operatività";
        public const string CommentoInterazioneChiusaMoge = "Eseguita Modifica Anagrafica";
        #endregion

        public enum NomeTabella
        {
            [Description("Attività")]
            Attività,
            [Description("Servizi e Beni")]
            ServiziBeni,
            [Description("Offerte")]
            Offerte,
            [Description("Punti di Fornitura")]
            PuntiDiFornitura,
            [Description("Documents")]
            Documents,
            [Description("Clienti")]
            Clienti,
            [Description("Assets")]
            Assets,
            [Description("Configuration Items")]
            ConfigurationItems,
            [Description("Contatti")]
            Contatti,
            [Description("Richieste")]
            Richieste,
            [Description("Elemento Richiesta")]
            ElementoRichiesta,
            [Description("Dettaglio Allaccio")]
            DettaglioAllaccio,
            [Description("Documenti Da Validare")]
            DocumentiDaValidare
        };
    }

    internal class Costanti
    {
        public static int[] LISTA_CODE_ATTIVE = new int[]
        {
            4977,//[MACRO1] CONTRATTI
		    5056,//[MACRO2] CONTRATTI
		    4976,//[MACRO1] ALTRI DOC
            5105//[MACRO2] ALTRI DOC
        };

        public static int CODA_EE176_MACRO1 = 4977;
        public static int CODA_EE176_MACRO2 = 5056;
        public static int CODA_EE176_ALTRI_MACRO1 = 4976;
        public static int CODA_EE176_ALTRI_MACRO2 = 5105;

        public static int EE176_MACRO1_RESO_OK = 31510;
        public static int EE176_MACRO1_RESO_RECUPERO_DATI = 31685;
        public static int EE176_MACRO1_RESO_LAV_MANUALE = 31684;
        public static int EE176_MACRO2_RESO_OK = 31687;
        public static int EE176_MACRO2_RESO_LAV_MANUALE = 31686;
        public static int EE176_MACRO1_ALTRI_RESO_OK = 31513;
        public static int EE176_MACRO1_ALTRI_RESO_RECUPERO_DATI = 31854;
        public static int EE176_MACRO1_ALTRI_RESO_LAV_MANUALE = 31853;
        public static int EE176_MACRO1_ALTRI_RESO_DICEMBRE = 31859;
        public static int EE176_MACRO2_ALTRI_RESO_OK = 31855;
        public static int EE176_MACRO2_ALTRI_RESO_LAV_MANUALE = 31856;
    }
}