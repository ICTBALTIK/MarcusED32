using AgenteHelperLibrary.ScaSrv;
using com.gargoylesoftware.htmlunit.javascript.host.html;
using com.sun.tools.javac.comp;
using com.sun.xml.@internal.ws.api.message;
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
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows;
using static MARCUS.Helpers.Constant;
using System.IO;
using sun.swing;

namespace MARCUS.Marcosi
{
    class SIFITES
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SIFITES));
        public Keanu Keanu { get; set; }
        public SqlQuery query = new SqlQuery();

        public SIFITES(Keanu keanu)
        {
            this.Keanu = keanu;
        }

        private InfoCascata riferimentoCorrente;
        public Records.RecordsSifites r = null;
       

        public bool Iframe { get; set; }

        private int numScartiConsecutivi = 0;
        private readonly int maxScartiConsecutivi = 10;
        private int restartCnt = 1;
        private string reso = "";
        int resoDaFare;
        private string scartoComment = "";

        
        public bool Flow() // BOOL
        {

            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\SifRegHistory.txt"))
            {
                var file = File.Create(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\SifRegHistory.txt");
                file.Close();
            }

            try
            {
                while (true)
                {
                    if (Keanu.StartStop == false)
                    {
                        break;
                    }

                    r = new Records.RecordsSifites();
                    Thread.Sleep(Keanu.Randy(1));
                    riferimentoCorrente = Keanu.Agente.NewInfoCascataToWork(-1);


                    if (riferimentoCorrente == null)
                    {
                        if (restartCnt < 6)
                        {
                            log.Info($"No docs, restarting macro for 6 minutes");
                            log.Info($"Restart counts : {restartCnt}");
                            Thread.Sleep(1000 * 60 * 6); //6 mins of pause = after 5 attempts - 30 min then log off
                            restartCnt++;
                        }
                        else
                        {
                            log.Info("Finished");
                            break;
                        }
                    }
                    else
                    {

                        restartCnt = 1;
                        r = query.GetValuesSifites(riferimentoCorrente.DETTAGLIO);

                        double importo = Double.Parse(r.Importo);
                        log.Info($"IMPORTO : {importo}");
                        log.Info($"NOTA : {r.Nota}");

                        if (importo != 0.00)
                        {

                            if (r.Nota.Contains("COMUN"))
                            {

                                log.Info($" SCARTO - NOTA CONTAINS COMUNE");
                                scartoComment = "Nota ir COMUNE";
                                resoDaFare = Keanu.LavScartoId;
                            }
                            else
                            {
                                log.Info($" REGISTRA - ALL IS OK");
                                resoDaFare = Keanu.LavRegId;
                            }
                        }
                        else
                        {
                            scartoComment = "Importo ir 0,00";
                            log.Info($" SCARTO - IMPORTO IS 0,00");
                            resoDaFare = Keanu.LavScartoId;
                        }


                        if (!Registration(resoDaFare))
                        {
                            break;
                        }
                    }
                }

            }
            catch
            {

                log.Info($"Flow() fail");
            }

            Keanu.Agente.Logout();
            return false;
        }

        private bool Registration(int resoDaFare)
        {


            if (resoDaFare == Keanu.LavRegId)
            {
                reso = "FATTO";
            }
            else
            {
                reso = "SCARTO";
            }

            if (resoDaFare == Keanu.LavRegId)
            {

                Keanu.Bad.Fatto++;
                numScartiConsecutivi = 0;
                log.Info($"{riferimentoCorrente.DETTAGLIO} FATTO");
                Keanu.Agente.RegistraCompleto(riferimentoCorrente.DETTAGLIO.Trim(), "", riferimentoCorrente.DATA_RIC_ACQ_DOC.Trim(), resoDaFare, "", "", "", 1, false, true, true);

                // na vremja txt file kuda zapisivaetsa riferimento , reso i scarto komment
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\SifRegHistory.txt";
                string regHistory = $"riferimento - {riferimentoCorrente.DETTAGLIO} importo - {r.Importo} reso - {reso} " + Environment.NewLine;
                File.AppendAllText(path, regHistory);

                return true;
            }
            else
            {
                numScartiConsecutivi++;
                Keanu.Bad.Scarto++;
                log.Info($"{riferimentoCorrente.DETTAGLIO} SCARTO - {scartoComment}");
                Keanu.Agente.RegistraCompleto(riferimentoCorrente.DETTAGLIO.Trim(), "", riferimentoCorrente.DATA_RIC_ACQ_DOC.Trim(), resoDaFare, "", "", "", 1, false, true, true);


                // na vremja txt file kuda zapisivaetsa riferimento , reso i scarto komment
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\SifRegHistory.txt";
                string regHistory = $"riferimento - {riferimentoCorrente.DETTAGLIO} importo - {r.Importo} reso - {reso} scarto reason - {scartoComment}" + Environment.NewLine;
                File.AppendAllText(path, regHistory);

                if (numScartiConsecutivi >= maxScartiConsecutivi)
                {
                    numScartiConsecutivi = 0;
                    log.Error("TOO MUCH SCARTO - 10");
                    return false;

                }

                return true;
            }
        }

    }
}
