using log4net;
using MARCUS.Helpers;
using OpenQA.Selenium;
using SFALibrary;
using SFALibrary.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MARCUS.Marcosi
{
    class SOSPENDI
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SOSPENDI));
        public Keanu Keanu { get; set; }
        public SqlQuery query = new SqlQuery();

        public SOSPENDI(Keanu keanu)
        {
            this.Keanu = keanu;
        }

        public SfaLib sfaLib { get; set; }

        public bool Flow()
        {
            Keanu.KillChromeWebDriver();

            if (!Keanu.PepperYourChrome(Keanu.LoginGEOCALL, Keanu.PassGEOCALL, "http://geocall-ml.enelint.global/Mercato/SSOServlet", "Principale", false))
                return false;
            if (!PepperYourSfaLib())
                return false;
            if (!PepperYourGirtGeocall())
                return false;

            return true;
        }

        private bool PepperYourGirtGeocall()
        {
            try
            {
                var operativa = Keanu.Driver.FindElement(By.Id("TBB_tbm2"));
                operativa.Click();
                Keanu.WaitingGame();

                var estrTask = Keanu.Driver.FindElement(By.XPath("//*[@id='TBB_tbm2']/div[2]/div[2]"));
                estrTask.Click();
                Keanu.WaitingGame();

                var ffff = Keanu.Driver.FindElement(By.XPath("//*[@id='TBB_tbm2']/div[1]/div[2]"));
                ffff.Click();
                Keanu.WaitingGame();

                var stati = Keanu.Driver.FindElement(By.XPath("//input[@name='_snLSSTATIGEOCALL']/parent::*/parent::*/td[2]/img"));
                stati.Click();
                Keanu.WaitingGame();

                var tableLC = Keanu.Driver.FindElement(By.ClassName("tvGrid"));
                var tableRowsLC = tableLC.FindElements(By.TagName("tr"));
                foreach (var item in tableRowsLC)
                {
                    if (item.Text.Contains("222103 PRE ASSEGNATA"))
                    {
                        item.Click();
                        Keanu.WaitingGame();
                        break;
                    }
                }

                tableLC = Keanu.Driver.FindElement(By.ClassName("tvGrid"));
                tableRowsLC = tableLC.FindElements(By.TagName("tr"));
                foreach (var item in tableRowsLC)
                {
                    if (item.Text.Contains("222104 ASSEGNATA"))
                    {
                        item.Click();
                        Keanu.WaitingGame();
                        break;
                    }
                }

                Keanu.WaitingGame();

                var asas = Keanu.Driver.FindElement(By.ClassName("tvCaption"));
                var deep = asas.FindElements(By.TagName("div"));
                foreach (var item in deep)
                {
                    if (item.Text.Equals(" Salva"))
                    {
                        var kk = item.FindElement(By.TagName("img"));
                        kk.Click();
                        Keanu.WaitingGame();
                        break;
                    }
                }

                var tipo = Keanu.Driver.FindElement(By.XPath("//input[@name='_snLD_XENMTATT']/parent::*/parent::*/td[2]/img"));
                tipo.Click();
                Keanu.WaitingGame();

                var descr = Keanu.Driver.FindElement(By.XPath("//input[@name='_syXENMTATTDESCRIZIONE']"));
                descr.SendKeys(Keanu.SospendiTipo);
                Keanu.WaitingGame();

                var cerca = Keanu.Driver.FindElements(By.CssSelector("button[type='SUBMIT']")).Last();
                cerca.Click();
                Keanu.WaitingGame();

                var tv = Keanu.Driver.FindElements(By.ClassName("tvGrid")).Last();
                var tvr = tv.FindElements(By.TagName("tr"));
                foreach (var item in tvr)
                {
                    if (item.Text.Contains(Keanu.SospendiTipo))
                    {
                        item.Click();
                        Keanu.WaitingGame();
                        break;
                    }
                }

                var asasii = Keanu.Driver.FindElements(By.ClassName("tvCaption")).Last();
                var deepi = asasii.FindElements(By.TagName("div"));
                foreach (var item in deepi)
                {
                    if (item.Text.Equals(" Salva"))
                    {
                        var kk = item.FindElement(By.TagName("img"));
                        kk.Click();
                        Keanu.WaitingGame();
                        break;
                    }
                }

                var lcerca = Keanu.Driver.FindElements(By.CssSelector("button[type='SUBMIT']")).First();
                lcerca.Click();
                Keanu.WaitingGame();

                Keanu.WaitingGame();

                int cunt = 0;
                List<string> aktj = new List<string>();
                var workarea = Keanu.Driver.FindElement(By.ClassName("tvGrid"));
                var dsf = workarea.FindElements(By.TagName("tr"));
                foreach (var item in dsf)
                {
                    if (cunt.Equals(5))
                        break;
                    if (item.Text.Contains("ASSEGNATA") && item.Text.Contains(Keanu.SospendiTipo))
                    {
                        var td = item.FindElements(By.TagName("td"));
                        foreach (var exactTD in td)
                        {
                            if (exactTD.Text.StartsWith("A-"))
                            {
                                aktj.Add(exactTD.Text.Trim());
                                log.Info(exactTD.Text.Trim());
                                cunt++;
                                Keanu.WaitingGame();
                                break;
                            }
                        }
                        Keanu.WaitingGame();
                        continue;
                    }
                }

                log.Info($"Got {aktj.Count()} rows");

                if (!aktj.Count.Equals(0))
                {
                    foreach (var item in aktj)
                    {
                        Attivita att = new Attivita();
                        int cnt = 0;
                        do
                        {
                            cnt++;
                            Thread.Sleep(Keanu.Randy(cnt));
                            att = sfaLib.SearchAttivita(item, true);
                        } while (att == null && cnt < 30);

                        if (att == null)
                        {
                            log.Error("att == null");
                            return false;
                        }

                        log.Debug($"{item} {att.Stato}");
                    }
                }

                return true;
            }
            catch
            {
                log.Warn($"PepperYourGirtGeocall() fail");
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
    }
}