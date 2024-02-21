using log4net;
using MARCUS.Helpers;
using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.IO;

namespace MARCUS.Marcosi
{
    class SOSPESI
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SOSPESI));
        public Keanu Keanu { get; set; }
        public SqlQuery query = new SqlQuery();

        public SOSPESI(Keanu keanu)
        {
            this.Keanu = keanu;
        }

        public bool Flow()
        {
            string user = Environment.UserName.ToUpper();
            string path = $"C:\\Users\\{user}\\Desktop\\MARCUSSOSPESI\\";
            try { Directory.Delete(path, true); } catch (Exception Ex) { Console.WriteLine(Ex.ToString()); }

            switch (Keanu.FaseDiSospesoPerEE253)
            {
                case "DOWNLOAD":
                    {
                        ExcelManipulations ex = new ExcelManipulations();
                        var s147 = query.GetSospesiFromDaLavorareByIdDettaglio("17450", "10");
                        //var s147 = query.GetSospesiFromDaLavorareByIdDettaglio("17450", "14");//Da Richiamare 2
                        ex.ExportExcel(s147, "S_DMS-147");
                        var sCert = query.GetSospesiFromDaLavorareByIdDettaglio("17898", "10");
                        ex.ExportExcel(sCert, "S_DMS-Cert.Cliente");
                        var sContr = query.GetSospesiFromDaLavorareByIdDettaglio("17899", "10");
                        ex.ExportExcel(sContr, "S_DMS-Contr.Gestionali");
                        var sScarti = query.GetSospesiFromDaLavorareByIdDettaglio("5209", "10");
                        ex.ExportExcel(sScarti, "LAV.CODE Scarto da Macro");

                        log.Info("Delete from & upload to:");
                        log.Info("EE253 - LAV.CODE Scarto da Macro");
                        log.Info("EE253 - Lavorazione Attività in SalesForce - S_DMS-147");
                        log.Info("EE253 - Lavorazione Attività in SalesForce - S_DMS-Cert.Cliente");
                        log.Info("EE253 - Lavorazione Attività in SalesForce - S_DMS-Contr.Gestionali");
                        //log.Info("Press ELIMINA");

                        break;
                    }
                case "ELIMINA":
                    {
                        Keanu.KillChromeWebDriver();
                        if (!Keanu.PepperYourGRAVITY("smartest347", "", "http://gravity.smartpaper.it/GravityFile/EliminazioneCode/", "Gravity"))
                            return false;

                        var bSelezionaFile = Keanu.DriverGRAVITY.FindElement(By.Id("btnUpload"));
                        bSelezionaFile.Click();
                        Keanu.WaitingGameGRAVITY();

                        log.Info("Press CARICA");
                        break;
                    }
                case "CARICA":
                    {
                        Keanu.KillChromeWebDriver();
                        if (!Keanu.PepperYourGRAVITY("smartest347", "", "http://gravity.smartpaper.it/GravityFile/CaricamentoCode/", "Gravity"))
                            return false;

                        log.Warn("Finished");
                        break;
                    }
                default:
                    break;
            }

            try { Process.Start(path); } catch { };

            return false;
        }
    }
}