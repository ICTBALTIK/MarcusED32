using log4net;
using MARCUS.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using SFALibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static MARCUS.Helpers.Constant;

namespace MARCUS.Marcosi
{
    class SCODAMENTOFROMFILE
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SCODAMENTOFROMFILE));
        public Keanu Keanu { get; set; }

        public SCODAMENTOFROMFILE(Keanu keanu)
        {
            this.Keanu = keanu;
        }

        public bool Iframe { get; set; }

        public SfaLib sfaLib { get; set; }

        //A-0646987787
        //A-0646984797


        private List<string> aktjivitiis = new List<string>();

        public bool Flow()
        {
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

            Keanu.KillChromeWebDriver();

            if (!Keanu.PepperYourChrome(Keanu.LoginGEOCALL, Keanu.PassGEOCALL, "http://geocall-ml.enelint.global/Mercato/SSOServlet", "Principale", false))
                return false;
            if (!PepperYourGeocallFromFile())
                return false;

            if (Keanu.ModificaPodTrip) {
                if (!Keanu.PepperYourChromeExtra(Keanu.LoginSFA, Keanu.PassSFA, "https://enelcrmt.my.salesforce.com/")) {
                    return false;
                }
                if (!PepperYourSfaLib())
                    return false;
            }

            foreach (var act in aktjivitiis)
            {
                Thread.Sleep(Keanu.Randy(1));

                var txtAttivita = Keanu.Driver.FindElement(By.Name("_syIDATTIVITA"));
                txtAttivita.Clear();
                txtAttivita.SendKeys(act.ToString().Trim());

                try
                {

                    for (int submitCounter = 0; submitCounter < 10; submitCounter++)
                    {
                        var cerca = Keanu.Driver.FindElements(By.CssSelector("button[type='SUBMIT']")).Last();
                        if(cerca != null)
                        {
                            cerca.Click();
                            break;
                        }
                        log.Debug($"NO CERCA BUTTON, TRYING MORE: COUNTER - {submitCounter}");
                    }

                    bool found = false;
                    try
                    {
                        var workarea = Keanu.Driver.FindElement(By.ClassName("tvGrid"));
                        var dsf = workarea.FindElements(By.TagName("tr"));
                        foreach (var item in dsf.Skip(1))
                        {
                            if (item.Text.Contains(act))
                            {
                                log.Info(act);
                                found = true;
                                item.Click();
                                break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        log.Debug($"# {act}");
                        Keanu.Bad.Scarto++;
                        continue;
                    }

                    if (!found)
                    {
                        log.Debug($"# {act}");
                        Keanu.Bad.Scarto++;
                        continue;
                    }

                    Keanu.WaitingGame();
                }
                catch
                {
                    log.Error($"Fail on {act}");
                    Keanu.KillChromeWebDriver();
                    break;
                }

                Terminator(act.ToString().Trim());
                Keanu.Bad.Fatto++;
                if (Keanu.StartStop == false)
                    break;
            }

            Keanu.KillChromeWebDriver();
            return false;
        }

        private bool PepperYourGeocallFromFile()
        {
            try
            {
                var operativa = Keanu.Driver.FindElement(By.Id("TBB_tbm2"));
                operativa.Click();
                Keanu.WaitingGame();
                var estrTask = Keanu.Driver.FindElement(By.XPath("//*[@id='TBB_tbm2']/div[1]/div[2]"));
                estrTask.Click();
                Keanu.WaitingGame();
                var perCliente = Keanu.Driver.FindElement(By.Id("MTT-EstrazioniProgrammate-form1"));
                perCliente.Click();
                Keanu.WaitingGame();
                log.Info($"PepperYourGeocallFromFile() ok");
                return true;
            }
            catch
            {
                log.Warn($"PepperYourGeocallFromFile() fail");
                return false;
            }
        }

        private void Terminator(string txtAttivita)
        {


            if (Keanu.ModificaPodTrip) {

                try {
                    //First try to close att in geo
                    var buttonTermina = Keanu.Driver.FindElement(By.CssSelector("button[type='SUBMIT']"));
                    buttonTermina.Click();
                    Thread.Sleep(Keanu.Randy(3));
                    if (Keanu.Driver.PageSource.Contains("Lista Task")) {
                        return;
                    } else {

                        Actions closingAction = new Actions(Keanu.Driver);
                        closingAction.SendKeys(Keys.Escape).Build().Perform();
                        Thread.Sleep(Keanu.Randy(2));

                        try {
                            ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("document.querySelectorAll(\"div[class *= 'slds-context-bar__secondary navCenter tabBarContainer'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                            Thread.Sleep(500);
                            ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("document.querySelectorAll(\"div[class *= 'slds-context-bar__secondary navCenter tabBarContainer'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                            Thread.Sleep(500);
                            ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("document.querySelectorAll(\"div[class *= 'oneGlobalNav oneConsoleNav'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                            Thread.Sleep(500);
                            ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("document.querySelectorAll(\"div[class *= 'oneGlobalNav oneConsoleNav'] div[class *= 'tabContainer'] li[class *= 'tabItem'] div[class *= 'close'] button[title *= 'Chiudi']\").forEach(function(element){element.click()})");
                            Thread.Sleep(500);
                        } catch { }

                        Thread.Sleep(Keanu.Randy(1));


                        var att = sfaLib.SearchAttivita(txtAttivita, true);
                        if (att == null) {
                            log.Error($"Sfalib errore - failed to find");

                            //Terminator();
                            //Keanu.KillChromeWebDriver();
                            return;
                        }

                        Keanu.NavigatoreExtra(att.RecordId);
                        //Keanu.NavigatoreExtra(Keanu.IdAttivitaPerScodamento);
                        Keanu.WaitingGameExtra();

                        IJavaScriptExecutor js = (IJavaScriptExecutor)Keanu.DriverExtra;
                        js.ExecuteScript("window.scrollBy(0,-1000)", "");
                        Thread.Sleep(500);

                        js.ExecuteScript("window.scrollBy(0,-1000)", "");
                        Thread.Sleep(500);

                        ClickButtonByName("Modifica");

                        Thread.Sleep(Keanu.Randy(1));

                        /*
                        string pod = "99999999999999";
                        string podSfa = GetFieldValue("POD");
                        if (string.IsNullOrEmpty(podSfa)) {
                            bool esito = CompilaCampi("* POD", pod, Constant.TipoCampo.INPUT);
                            if (!esito) { log.Error("Problemi nell'inserire il pod"); }
                            log.Info("Pod inserito");
                            Thread.Sleep(Keanu.Randy(1));
                        } else {
                            bool esito = CompilaCampi("* POD", podSfa, Constant.TipoCampo.INPUT);
                            if (!esito) { log.Error("Problemi nell'inserire il pod"); }
                            //log.Info("Pod inserito");
                            Thread.Sleep(Keanu.Randy(1));
                            log.Info("Pod already present");
                        }
                        */

                        /*
                        string specifica = GetFieldValue("Specifica");
                        if (string.IsNullOrEmpty(specifica)) {
                            if (Keanu.LavName.Equals("SCODAMENTO - EE62 - SMART LETTONIA")) {
                                if (!SetTripletta("Gestione Clienti", "Credito", @"Dimostrato Pagamento")) {
                                    log.Warn($"SetTripletta() fail");
                                }
                            } else {
                                if (!SetTripletta(@"N/A", @"N/A", @"N/A")) {
                                    log.Warn($"SetTripletta() fail");
                                }
                            }
                        } else {
                            log.Info("Trip already present");
                        }
                        */

                        /*
                        if (!string.IsNullOrEmpty(podSfa) && !string.IsNullOrEmpty(specifica)) {
                            log.Info("Nothing to modify, delete cliente and set stato IN LAVORAZIONE");
                            try {
                                if (Keanu.DriverExtra.FindElement(By.Id("customerLookUp")).GetAttribute("value").Length > 1) {
                                    var cl = Keanu.DriverExtra.FindElement(By.Id("customerLookUp"));
                                    var del = cl.FindElement(By.XPath("..//span[@title= 'Click to delete value...']"));
                                    Actions a = new Actions(Keanu.DriverExtra);
                                    a.MoveToElement(cl).Perform();
                                    Thread.Sleep(500);
                                    a.Click(del).Perform();
                                }
                            } catch { }

                            if (!CompilaCampi("* Stato", "IN LAVORAZIONE", TipoCampo.COMBO, false, false)) {
                                if (!CompilaCampi("* Status", "IN LAVORAZIONE", TipoCampo.COMBO, false, false)) {
                                    log.Warn("Problemi nell'impostare lo stato in FATTO");
                                }
                            }
                        }
                        */

                        if (!ClickButtonByName("Salva")) {
                            log.Warn($"ClickButtonByName() fail");
                        }

                        int cnt = 0;
                        var btnTermina = Keanu.Driver.FindElement(By.CssSelector("button[type='SUBMIT']"));
                        btnTermina.Click();
                        while (!Keanu.Driver.PageSource.Contains("rossimo Task")) {
                            try {
                                if (cnt == 5) {
                                    cnt = 0;

                                    try {
                                        Actions a = new Actions(Keanu.Driver);
                                        a.SendKeys(Keys.Escape).Build().Perform();
                                        Thread.Sleep(Keanu.Randy(2));
                                        btnTermina = Keanu.Driver.FindElement(By.CssSelector("button[type='SUBMIT']"));
                                        btnTermina.Click();
                                        Thread.Sleep(Keanu.Randy(2));
                                    } catch {
                                        Actions b = new Actions(Keanu.Driver);
                                        b.SendKeys(Keys.Escape).Build().Perform();
                                        Thread.Sleep(Keanu.Randy(3));
                                        btnTermina = Keanu.Driver.FindElement(By.CssSelector("button[type='SUBMIT']"));
                                        btnTermina.Click();
                                        Thread.Sleep(Keanu.Randy(3));
                                    }

                                    #region
                                    if (!Keanu.Driver.PageSource.Contains("rossimo Task")) {
                                        if (Keanu.DriverExtra == null) {
                                            log.Info($"Open SFA");
                                            if (!Keanu.PepperYourChromeExtra(Keanu.LoginSFA, Keanu.PassSFA, "https://enelcrmt.my.salesforce.com/"))
                                                try { Keanu.DriverExtra.Close(); } catch { throw; }
                                        }
                                    }
                                    #endregion

                                }
                            } catch {
                                //log.Info("Continue");
                                Thread.Sleep(Keanu.Randy(5));
                                cnt = 0;
                                continue;
                            }

                            Thread.Sleep(Keanu.Randy(1));
                            cnt++;

                        }

                    }

                } catch {
                    log.Error($"Went to Termina - how?");
                    Keanu.Erry();
                }




            } else {

                int cnt = 0;
                var btnTermina = Keanu.Driver.FindElement(By.CssSelector("button[type='SUBMIT']"));
                btnTermina.Click();
                while (!Keanu.Driver.PageSource.Contains("Lista Task")) {
                    try {
                        if (cnt == 5) {
                            cnt = 0;

                            try {
                                Actions a = new Actions(Keanu.Driver);
                                a.SendKeys(Keys.Escape).Build().Perform();
                                Thread.Sleep(Keanu.Randy(2));
                                btnTermina = Keanu.Driver.FindElement(By.CssSelector("button[type='SUBMIT']"));
                                btnTermina.Click();
                                Thread.Sleep(Keanu.Randy(2));
                            } catch {
                                Actions b = new Actions(Keanu.Driver);
                                b.SendKeys(Keys.Escape).Build().Perform();
                                Thread.Sleep(Keanu.Randy(3));
                                btnTermina = Keanu.Driver.FindElement(By.CssSelector("button[type='SUBMIT']"));
                                btnTermina.Click();
                                Thread.Sleep(Keanu.Randy(3));
                            }
                        }
                    } catch {
                        Thread.Sleep(Keanu.Randy(5));
                        cnt = 0;
                        continue;
                    }

                    Thread.Sleep(Keanu.Randy(1));
                    cnt++;
                }

            }

            
        }


        private bool SetTripletta(string CausaleContatto, string Descrizione, string Specifica) {
            try {
                if (!CompilaCampi("* Causale Contatto", CausaleContatto, Constant.TipoCampo.COMBO)) {
                    if (!CompilaCampi(" Causale Contatto", CausaleContatto, Constant.TipoCampo.COMBO))
                        return false;
                }
                Thread.Sleep(1000);

                if (!CompilaCampi("* Descrizione", Descrizione, Constant.TipoCampo.COMBO)) {
                    if (!CompilaCampi("* Description", Descrizione, Constant.TipoCampo.COMBO))
                        return false;
                }
                Thread.Sleep(1000);

                if (!CompilaCampi("* Specifica", Specifica, Constant.TipoCampo.COMBO)) {
                    if (!CompilaCampi("* Specification", Specifica, Constant.TipoCampo.COMBO))
                        return false;
                }
                Thread.Sleep(1000);

                return true;
            } catch {
                return false;
            }
        }

        public bool CompilaCampi(string label, string value, TipoCampo tipologia, bool iframe = false, bool cercaSpan = false, IWebElement elementoDiPartenza = null) {
            try {
                IWebElement welabel = null;

                if (tipologia == TipoCampo.COMBO)
                    label = label.Trim();
                string addSpan = "";
                string puntoSpan = "";
                if (cercaSpan) {
                    addSpan = @"/span";
                    puntoSpan = ".";
                }

                if (elementoDiPartenza == null && !iframe)
                    elementoDiPartenza = GetSezioneAttiva();

                try {
                    if (elementoDiPartenza != null)
                        welabel = elementoDiPartenza.FindElements(By.XPath($".//label[contains(@class,'slds-form-element__label')]{addSpan}")).Where(box => box.Text.Equals(label)).ToList()[0];

                    else
                        welabel = Keanu.DriverExtra.FindElements(By.XPath($"//label[contains(@class,'slds-form-element__label')]{addSpan}")).Where(box => box.Text.Equals(label)).ToList()[0];
                } catch {
                    if (!iframe)
                        welabel = elementoDiPartenza.FindElements(By.XPath($".//label{addSpan}")).Where(box => box.Text.Equals(label)).ToList()[0];
                    else
                        welabel = Keanu.DriverExtra.FindElements(By.XPath($"//label{addSpan}")).Where(box => box.Text.Equals(label)).ToList()[0];
                }

                if (welabel != null) {
                    IWebElement weWrite = null;
                    switch (tipologia) {
                        case TipoCampo.COMBO:
                            try {
                                weWrite = welabel.FindElement(By.XPath($"{puntoSpan}.//following-sibling::div"));
                                weWrite = weWrite.FindElement(By.TagName("select"));// Modifica perchè ci potrebbero essere più div allora si va per tag name
                            } catch {
                                try {
                                    weWrite = welabel.FindElement(By.XPath($"{puntoSpan}.//following-sibling::select"));
                                } catch {
                                    return false;
                                }
                            }
                            if (weWrite != null) {
                                weWrite.FindElement(By.XPath(".//option[@value = \"" + value + "\"]")).Click();
                                log.Info("Il campo " + label + " è stato valorizzato correttamente");
                                return true;
                            } else {
                                return false;
                            }
                        case TipoCampo.INPUT:
                            try {
                                try {
                                    weWrite = welabel.FindElement(By.XPath(".//following-sibling::div"));
                                    weWrite = weWrite.FindElement(By.TagName("input"));//Modifica perchè ci potrebbero essere più div alora si va per tag name
                                } catch {
                                    weWrite = welabel.FindElement(By.XPath(".//following-sibling::input"));
                                }
                            } catch {
                                return false;
                            }
                            if (weWrite != null) {
                                weWrite.SendKeys(Keys.Enter);
                                weWrite.Clear();
                                Thread.Sleep(1000);
                                weWrite.Clear();
                                weWrite.SendKeys(value);
                                weWrite.SendKeys(Keys.Tab);
                                log.Info("Il campo " + label + " è stato valorizzato correttamente");
                                return true;
                            } else {
                                return false;
                            }
                    }
                    return false;
                }
                return false;
            } catch {
                return false;
            }
        }

        public IWebElement GetSezioneAttiva() {
            IWebElement returnElement;
            if (HasActiveSubtab()) {
                try//NEW
                {
                    returnElement = Keanu.DriverExtra.FindElement(By.XPath(".//div[@class = 'oneWorkspace active hasFixedFooter hasActiveSubtab navexWorkspace']//section[contains(@class, 'tabContent active oneConsoleTab')]"));
                } catch//OLD
                  {
                    returnElement = Keanu.DriverExtra.FindElement(By.XPath(".//div[@class = 'oneWorkspace active hasFixedFooter hasActiveSubtab oneWorkspace2']//section[contains(@class, 'tabContent active oneConsoleTab')]"));
                }
            } else {
                try {//NEW
                    returnElement = Keanu.DriverExtra.FindElement(By.XPath(".//div[@class = 'oneWorkspace active hasFixedFooter navexWorkspace']//section[contains(@class, 'tabContent active oneConsoleTab')]"));
                } catch {//OLD
                    returnElement = Keanu.DriverExtra.FindElement(By.XPath(".//div[@class = 'oneWorkspace active hasFixedFooter oneWorkspace2']//section[contains(@class, 'tabContent active oneConsoleTab')]"));
                }
            }
            return returnElement;
        }

        public bool HasActiveSubtab() {
            try {
                var val = Keanu.DriverExtra.FindElements(By.CssSelector("div"));
                foreach (var item in val) {
                    string sh = item.GetAttribute("class");
                    //if (sh.Equals("oneWorkspace active hasFixedFooter hasActiveSubtab oneWorkspace2"))
                    if (sh.Equals("oneWorkspace active hasFixedFooter hasActiveSubtab navexWorkspace") || sh.Equals("oneWorkspace active hasFixedFooter hasActiveSubtab oneWorkspace2"))
                        return true;
                }
                return false;
            } catch (Exception) {
                return false;
            }
        }

        public string GetFieldValue(string label, string tipo = "", IWebElement elementoDiPartenza = null) {
            string result = "";

            switch (tipo) {
                default:
                    string value = null;
                    try {
                        value = (string)((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab label.slds-form-element__label'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].value");
                        if (value != null) { result = value; break; }
                    } catch { }
                    try {
                        value = (string)((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab label.slds-form-element__label'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].innerHTML");
                        if (value != null) { result = value; break; }
                    } catch { }
                    try {
                        value = (string)((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab label.slds-form-element__label'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].children[0].value");
                        if (value != null) { result = value; break; }
                    } catch { }
                    try {
                        value = (string)((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab label.form-element__label'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.value");
                        if (value != null) { result = value; break; }
                    } catch { }
                    try {
                        value = (string)((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab')[0].getElementsByClassName('slds-form-element__label slds-no-flex'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].value");
                        if (value != null) { result = value; break; }
                    } catch { }
                    try {
                        value = (string)((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab')[0].getElementsByClassName('slds-form-element__label slds-no-flex'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].innerHTML");
                        if (value != null) { result = value; break; }
                    } catch { }
                    try {
                        value = (string)((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab')[0].getElementsByClassName('slds-form-element__label slds-no-flex'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.children[0].children[0].value");
                        if (value != null) { result = value; break; }
                    } catch { }
                    try {
                        value = (string)((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("return Array.prototype.filter.call(document.querySelectorAll('.active.hasFixedFooter.oneWorkspace .tabContent.active.oneConsoleTab')[0].getElementsByClassName('slds-form-element__label slds-no-flex'), function(element){ return RegExp(\'" + label + "\').test(element.textContent); })[0].nextElementSibling.value");
                        if (value != null) { result = value; break; }
                    } catch { }
                    break;
            }

            return result;
        }

        public IWebElement ButtonExistByName(string textButton, string className = null, string tipo = "button", IWebElement elementoDiPartenza = null) {
            try {
                if (!Iframe) {
                    if (elementoDiPartenza == null)
                        elementoDiPartenza = GetSezioneAttiva();
                    if (className == null)
                        return elementoDiPartenza.FindElement(By.XPath(".//div[@class = 'slds-col--padded slds-no-flex slds-align-bottom']")).FindElement(By.XPath(".//button[normalize-space()= '" + textButton + "']"));
                    else
                        return elementoDiPartenza.FindElement(By.XPath(".//" + tipo + "[@class = '" + className + "' and normalize-space()= '" + textButton + "']"));
                } else {
                    if (className == null) {
                        if (elementoDiPartenza == null)
                            return Keanu.DriverExtra.FindElement(By.XPath(".//div[@class = 'slds-col--padded slds-no-flex slds-align-bottom']")).FindElement(By.XPath(".//button[normalize-space()= '" + textButton + "']"));
                        else
                            return elementoDiPartenza.FindElement(By.XPath(".//div[@class = 'slds-col--padded slds-no-flex slds-align-bottom']")).FindElement(By.XPath(".//button[normalize-space()= '" + textButton + "']"));
                    } else {
                        if (elementoDiPartenza == null)
                            return Keanu.DriverExtra.FindElement(By.XPath(".//" + tipo + "[@class = '" + className + "' and normalize-space()= '" + textButton + "']"));
                        else
                            return elementoDiPartenza.FindElement(By.XPath(".//" + tipo + "[@class = '" + className + "' and normalize-space()= '" + textButton + "']"));
                    }
                }
            } catch {
                return null;
            }
        }

        private bool PepperYourSfaLib() {
            sfaLib = new SfaLib(Keanu.LoginSFA, Keanu.PassSFA);
            bool marc = sfaLib.LoginProd();
            if (!marc) { marc = sfaLib.LoginProd(); }
            if (!marc) { log.Error("Login non riuscito su SFA con HTMlUnit."); return false; }
            return true;
        }

        public bool ClickButtonByName(string textButton, string className = null, string tipo = "button", IWebElement elementoDiPartenza = null) {
            log.Info($"Click su {textButton}");
            try {
                IWebElement button = null;
                int i = 0;
                while (button == null && i < 10) {
                    button = ButtonExistByName(textButton, className, tipo, elementoDiPartenza);
                    Thread.Sleep(Keanu.Randy(1));
                    i++;
                }
                if (button == null) {
                    log.Error($"Button {textButton} does not exist");
                    return false;
                }
                if (tipo.Equals("span"))
                    button.Click();
                else
                    ((IJavaScriptExecutor)Keanu.DriverExtra).ExecuteScript("arguments[0].click();", button);
            } catch {
                log.Error($"Cannot click to {textButton}");
                return false;
            }
            Keanu.WaitingGameExtra();
            return true;
        }
    }
}