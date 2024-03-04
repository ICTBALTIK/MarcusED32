using com.sun.org.apache.xerces.@internal.impl.dv.xs;
using com.sun.org.apache.xpath.@internal.functions;
using com.sun.xml.@internal.bind.v2.model.core;
using Controlli_ee145;
using jdk.nashorn.@internal.objects.annotations;
using log4net;
using MARCUS.Helpers;
using MARCUS.Marcosi;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SFALibrary;
using SFALibrary.Model;
using sun.util.resources.cldr.dz;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Reflection;

namespace MARCUS.Controll
{
    public class Controlli
    {
        public Controlli(Keanu keanu)
        {
            this.Keanu = keanu;
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(Controlli));
        public SfaLib sfaLib { get; set; }
        bool sfalibarsset = false;
        public bool Iframe { get; set; }
        public Keanu Keanu { get; set; }

        public string Matrikola = "MI0000F";
        public string psw = "Dominio$000001";
        public string stato = "";
        public string huinja = "";
        public string statoop = "";
        public int fatto = 26567;
        public DateTime datefromValidare;
        public DateTime DATAs;
        string rifferimento = "";
        string SF = "";
        string RECORD = "";
        string CF = "";
        public bool KO { get; set; } = false;
        public bool OK { get; set; } = false;
        public bool isDataProcessed = true;

        List<ArchivioCatastali> list = new List<ArchivioCatastali>();

        public bool Flow()
        {
            Keanu.KillChromeWebDriver();
            PepperYourSfaLib();
            DbContext db = new DbContext();
            list = db.GetModuliRifferimetno();
            SF = list[0].RIFERIMENTO.ToString();
            CF = list[0].NUMERO_CLIENTE.ToString();
            DATAs = list[0].DATA;
            list = db.GetebanutijAttivita(SF);
            //rifferimento = list[0].RIFERIMENTO.ToString();
            //rifferimento = "A-0780456970";
            //CF = "524180996";
            try
            {
                if (!Keanu.PepperYourChrome(Matrikola, psw, "https://enelcrmt.my.salesforce.com/", "", true))
                    return false;
                GetDataAndGo();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool GetDataAndGo()
        {
            ReadDataFromCSV();
            if (isDataProcessed == true)
            {
                DataGet(rifferimento);
            UnlockAndCloseAllTabs();
            if (CaricaCampiSearch())
            {
                FindLaTarifa();
            }
            else return false;
            
        }
            return true;
        }

        public bool CaricaCampiSearch()
        {
            Keanu.Driver.Navigate().GoToUrl($"https://enelcrmt.lightning.force.com/lightning/r/wrts_prcgvr__Activity__c/{RECORD}/view");
            //WaitSpecificPage("Data Inizio");
            try 
            {
                By documentoLabel = By.XPath("//label[contains(@class, 'slds-form-element__label') and contains(text(), 'Documento')]");
                WebDriverWait wait = new WebDriverWait(Keanu.Driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementIsVisible(documentoLabel));
            }
            catch { }
            System.Threading.Thread.Sleep(4000);
            ((IJavaScriptExecutor)Keanu.Driver).ExecuteScript("window.scrollTo(0, 0);");

            try
                {
                    IWebElement clienteField = Keanu.Driver.FindElement(By.Id("customerLookUp"));
                    string clienteValue = clienteField.GetAttribute("value");

                    if (!string.IsNullOrEmpty(clienteValue))
                    {
                        IWebElement fattoElement = Keanu.Driver.FindElement(By.XPath("//option[@value='FATTO']"));
                        string fattoValue = fattoElement.Text;

                        if (fattoValue == "FATTO")
                        {
                            try
                            {
                                IWebElement ElementSearch = null;
                                Thread.Sleep(700);
                                ElementSearch = Keanu.Driver.FindElement(By.Id("customerLookUp"));
                                ElementSearch.Click();
                                Thread.Sleep(1000);
                                By tabLink = By.XPath("//a[@class='slds-tabs_default__link' and @data-label='Servizi e Beni']");
                                WebDriverWait wait = new WebDriverWait(Keanu.Driver, TimeSpan.FromSeconds(10));
                                wait.Until(ExpectedConditions.ElementIsVisible(tabLink));
                        }
                        catch
                            {
                                log.Debug("Failed to Click customerinfo");
                                CaricaCampiSearch();
                            }

                            try
                            {
                                IWebElement ServiciBeneTab = null;
                                ServiciBeneTab = Keanu.Driver.FindElement(By.XPath("//*[@id='customTab3__item']"));
                                ServiciBeneTab.Click();
                                By pElement = By.XPath("//p[@title='Attive']");
                                WebDriverWait wait = new WebDriverWait(Keanu.Driver, TimeSpan.FromSeconds(10));
                                wait.Until(ExpectedConditions.ElementIsVisible(pElement));
                        }
                        catch
                            {
                                log.Debug("Failed to Click Servici E Bene tab");
                                CaricaCampiSearch();

                            }
                        }
                        else
                        {
                            KO = true;
                            WriteDataToExcel();
                        }
                    }
                    else
                    {
                        KO = true;
                        WriteDataToExcel();
                }
                }
            catch
            { }
            return true;
        }

        public bool FindLaTarifa()
        {
            if (CF.Length > 0)
            {
                try
                {
                    IWebElement CFinsert = null;
                    CFinsert = Keanu.Driver.FindElement(By.XPath("//input[@class='slds-input' and contains(@id, 'input-') and @name='inline-search-input']"));
                    CFinsert.SendKeys(CF);
                    IWebElement paragraph = null;
                    paragraph = Keanu.Driver.FindElement(By.XPath("//span/a[contains(@class, 'asset-label')]")); //change xPath 175 ATT like example
                    if (paragraph != null)
                    {
                        try
                        {
                            IWebElement tarrifa = null;
                            tarrifa = Keanu.Driver.FindElement(By.XPath("//a[contains(@class, 'asset-label') and contains(@data-aura-rendered-by, ':')]"));
                            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)Keanu.Driver;
                            jsExecutor.ExecuteScript("arguments[0].click();", tarrifa);
                            By spanElement = By.XPath("//span[text()='Commodity']");
                            WebDriverWait wait = new WebDriverWait(Keanu.Driver, TimeSpan.FromSeconds(10));
                            wait.Until(ExpectedConditions.ElementIsVisible(spanElement));

                            if (TestAutomationManager.DocumentiDaValidareChecked == false && Keanu.Driver.PageSource.Contains("Commodity"))
                            {
                                if (!CommodityNextSteps())
                                {
                                    //Flow();
                                }
                            }
                        }
                        catch
                        {
                            IWebElement tarrifa = null;
                            tarrifa = Keanu.Driver.FindElement(By.XPath("//a[contains(@class, 'asset-label') and contains(@data-aura-rendered-by, ':')]")); //GAS???
                            tarrifa.Click();
                            return false;
                        }

                    }
                }
                catch
                {}
            }
            return true;
        }

        public class TestAutomationManager
        {
            public static bool DocumentiDaValidareChecked = false;
            public static bool NoOfferta = false;
        }

        public bool CommodityNextSteps()
        {
            IWebElement element = Keanu.Driver.FindElement(By.XPath("//*[contains(@id, 'sectionContent-')]//div//slot//records-record-layout-row[4]//slot//records-record-layout-item[2]//div//div//dl//dd//div//span//slot[1]//force-lookup//div//records-hoverable-link"));
            element.Click();
            By contrattoLabel = By.XPath("//records-entity-label[text()='Contratto']");
            WebDriverWait wait = new WebDriverWait(Keanu.Driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementIsVisible(contrattoLabel));
            if (Keanu.Driver.PageSource.Contains("Correlato"))
            {
                IWebElement detaglio = Keanu.Driver.FindElement(By.XPath("//li[@class='slds-tabs_default__item' and @data-tab-value='detailTab']//a[@id='detailTab__item']"));
                detaglio.Click();
                By nomeClienteLabel = By.XPath("//span[@class='test-id__field-label' and text()='Nome Cliente']");
                WebDriverWait wait1 = new WebDriverWait(Keanu.Driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementIsVisible(nomeClienteLabel));

                if (Keanu.Driver.PageSource.Contains("Offerta"))
                {
                    try
                    {
                        IWebElement offertaItem = Keanu.Driver.FindElement(By.XPath("//records-record-layout-item[@field-label='Offerta']"));
                        // Locate child elements within the 'records-record-layout-item' with the field label 'Offerta'
                        IWebElement offertaLink = offertaItem.FindElement(By.XPath(".//force-lookup//span"));//check if not changing 
                        offertaLink.Click();
                        By clienteLabel = By.XPath("//div[@class='test-id__field-label-container slds-form-element__label no-utility-icon']//span[@class='test-id__field-label' and text()='Cliente']");
                        WebDriverWait wait2 = new WebDriverWait(Keanu.Driver, TimeSpan.FromSeconds(10));
                        wait.Until(ExpectedConditions.ElementIsVisible(clienteLabel));
                        System.Threading.Thread.Sleep(2000);
                    }
                    catch 
                    {
                        TestAutomationManager.NoOfferta = true;
                        WriteDataToExcel();
                    }

                    if (Keanu.Driver.PageSource.Contains("Quote")) //changed from SWARES
                    {
                        try
                        {
                            IList<IWebElement> correlatoElements = Keanu.Driver.FindElements(By.XPath("//a[@id='relatedListsTab__item' and @data-tab-value='relatedListsTab']"));

                            if (correlatoElements.Count > 0)
                            {
                                int index = correlatoElements.Count - 1;
                                correlatoElements[index].Click();
                                System.Threading.Thread.Sleep(500);
                            }
                        }
                        catch
                        {}
                        if (Keanu.Driver.FindElements(By.XPath("//a[contains(@class, 'baseCard__header-title-container')]//span[contains(text(), 'Documenti Da Validare')]//following-sibling::span[contains(text(), '(0)')]")).Any())
                        {
                            DataCatastali();
                        }
                        else
                        {
                            DocumentiValidare();
                        }
                    }
                    log.Debug("Not found on didnr enter next page.");
                    return false;
                }
            }
            return true;
        }

        private void DataCatastali()
        {
            TestAutomationManager.DocumentiDaValidareChecked = true;
            CaricaCampiSearch();
            FindLaTarifa();

            if (Keanu.Driver.PageSource.Contains("Correlato"))
            {
                try
                {
                    IList<IWebElement> correlatoElements = Keanu.Driver.FindElements(By.XPath("//a[@id='relatedListsTab__item' and @data-tab-value='relatedListsTab']"));

                    if (correlatoElements.Count > 0)
                    {
                        int index = correlatoElements.Count - 1;
                        correlatoElements[index].Click();
                        System.Threading.Thread.Sleep(3000);
                    }

                    if (Keanu.Driver.FindElements(By.XPath("//a[contains(@class, 'baseCard__header-title-container')]//span[contains(text(), 'Dati Catastali')]//following-sibling::span[contains(text(), '(0)')]")).Any())
                    {
                        KO = true;
                        WriteDataToExcel();
                    }
                    else
                    {
                        OK = true;
                        WriteDataToExcel();
                    }
                }
                catch { }
            }
        }


        public bool DocumentiValidare()
        {
            if (Keanu.Driver.PageSource.Contains("Offerta"))
            {
                try
                {

                    By docValidaLocator = By.XPath("//span[@class='slds-truncate slds-m-right--xx-small' and @title='Documenti Da Validare']");
                    if (IsElementVisible(docValidaLocator))
                    {
                        IWebElement DocValida = Keanu.Driver.FindElement(docValidaLocator);
                        DocValida.Click();
                        GetValuesFromValidare();
                    }
                    else
                    {
                        while (!IsElementVisible(docValidaLocator))
                        {
                            GoDOWN();
                        }

                        IWebElement DocValida = Keanu.Driver.FindElement(docValidaLocator);
                        DocValida.Click();
                        By documentoDaValidareLabel = By.XPath("//h1[@class='slds-page-header__title listViewTitle slds-truncate' and @title='Documenti Da Validare']");
                        WebDriverWait wait = new WebDriverWait(Keanu.Driver, TimeSpan.FromSeconds(10));
                        wait.Until(ExpectedConditions.ElementIsVisible(documentoDaValidareLabel));
                        System.Threading.Thread.Sleep(3000);
                        GetValuesFromValidare();
                    }
                }
                catch { }
            }
            return true;
        }

        public bool IsElementVisible(By locator)
        {
            IWebElement element = null;
            try
            {
                element = Keanu.Driver.FindElement(locator);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            // JavaScript code to check element visibility
            string script = @"
            var elem = arguments[0];
            var bounding = elem.getBoundingClientRect();
            var top = bounding.top;
            var bottom = bounding.bottom;
            var isVisible = (top >= 0) && (bottom <= (window.innerHeight || document.documentElement.clientHeight));
            return isVisible;";

            return (bool)((IJavaScriptExecutor)Keanu.Driver).ExecuteScript(script, element);
        }

        public void GetValuesFromValidare()
        {

            List<string> modelValues = new List<string>();
            List<string> stateValues = new List<string>();
            SwitchToDefaultContent();
            var table = Keanu.Driver.FindElements(By.XPath("//thead[@data-rowgroup-header]"));
            var table2 = Keanu.Driver.FindElements(By.XPath("//thead[@data-rowgroup-header]/following-sibling::tbody"));
            int indexModeloE = table.IndexOf(table.Where(q => q.Text.Contains("Modello")).First());
            List<string> mlDatiCatastaliValues = new List<string>();
            IList<IWebElement> findshit = table2.Where(modello => modello.Text.Contains("MODULO_DI_ADESIONE")).ToList();

            if (findshit.Count > 0)
            {
                var findValido = findshit.First(valido => valido.Text.Contains("VALIDATO"));
                var text = findshit[1].Text;
                var elements = text.Split(new string[] { "Seleziona elemento" }, StringSplitOptions.RemoveEmptyEntries);
                List<DateTime> dates = new List<DateTime>();

                foreach (var element in elements)
                {
                    if (element.Contains("MODULO_DI_ADESIONE") && element.Contains("VALIDATO"))
                    {
                        // Extract the date from the element
                        var pattern = @"\b\d{2}/\d{2}/\d{4} \d{2}\.\d{2}\b";
                        var regex = new Regex(pattern);
                        var match = regex.Match(element);

                        if (match.Success)
                        {
                            var dateString = match.Value;

                            // Parse the date string to DateTime
                            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy HH.mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                            {
                                // Output the result
                                Console.WriteLine($"MODULO_DI_ADESIONE with VALIDATO found on {date}");

                                // Store the date in the list
                                dates.Add(date);
                                datefromValidare = date;
                                ChecekValidoData();
                            }
                            else
                            {
                                // Handle parsing error if needed
                                Console.WriteLine($"Error parsing date: {dateString}");
                            }
                        }
                    }
                }

                if (findValido != null)
                {
                    var dateSpan = findValido.FindElements(By.XPath("//span[contains(@title, '00/00/0000')]"));
                    dateSpan.ToString().Trim();
                }
            }
            else
            {
                DataCatastali();
            }
        }

        void ChecekValidoData()
        {
            System.Threading.Thread.Sleep(3000);
            if (datefromValidare <= DATAs)
            {
                //ok
                OK = true;
                WriteDataToExcel();
            }
            else
            {
                //not ok, but were to send ?
                KO = true;
                WriteDataToExcel();
            }
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
                    Thread.Sleep((1));
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
                    Thread.Sleep((1));
                }
            }
            return paginaCaricata;
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

        public bool SwitchToDefaultContent() //proveritj
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

        public void DataGet(string rifferimento)
        {
            Attivita attivita = new Attivita();
            attivita = sfaLib.SearchAttivita(rifferimento);

            stato = attivita.Stato;
            RECORD = attivita.RecordId;

            Attivita attivita1 = new Attivita();
            attivita1 = sfaLib.GetActivityLoadConfiguration(RECORD);
        }
        private void GoDOWN()
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)Keanu.Driver;
            js.ExecuteScript($"window.scrollBy(0,300)", "");
            Thread.Sleep(500);
        }

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
                return false;
            }
            return true;
        }

        public void WriteDataToExcel()
        {
            try
            {
                DateTime currentDate = DateTime.Today;
                string fileName = $"ExcelOutput_{currentDate.ToString("yyyy-MM-dd")}.csv";

                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string excelFolderPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(currentDirectory).FullName).FullName).FullName;
                excelFolderPath = Path.Combine(excelFolderPath, "ExcelFiles");
                string filePath = Path.Combine(excelFolderPath, fileName);

                Directory.CreateDirectory(excelFolderPath);

                if (!File.Exists(filePath))
                {
                    using (StreamWriter headerWriter = new StreamWriter(filePath, false))
                    {
                        headerWriter.WriteLine("Rifferimento;Status"); 
                    }
                }

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine($"{rifferimento};{(TestAutomationManager.NoOfferta ? "No offerta" : (OK ? "OK" : (KO ? "KO" : "")))}");
                }

                OK = false;
                KO = false;
                TestAutomationManager.DocumentiDaValidareChecked = false;
                TestAutomationManager.NoOfferta = false;
                GetDataAndGo();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public static int currentProcessedRow = 0;
        public string line;
        public void ReadDataFromCSV()
        {
            try
            {
                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string excelFolderPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(currentDirectory).FullName).FullName).FullName;
                excelFolderPath = Path.Combine(excelFolderPath, "ExcelFiles");
                string filePath = Path.Combine(excelFolderPath, "Data.csv");
                Directory.CreateDirectory(excelFolderPath);

                using (StreamReader reader = new StreamReader(filePath))
                {

                    for (int i = 0; i <= currentProcessedRow; i++)
                    {
                        reader.ReadLine();
                    }

                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(';');
                        if (parts.Length >= 2) 
                        {
                            rifferimento = parts[0].Trim();
                            CF = parts[1].Trim();
                            currentProcessedRow = currentProcessedRow + 1;
                            isDataProcessed = true;
                            return;
                        }
                        else
                        {
                            log.Warn("Invalid data format detected in the following line: " + line);
                        }
                    }

                    log.Warn("No more data to process.");
                    isDataProcessed = false;
                }
            }
            catch (Exception ex)
            {
                log.Error($"An error occurred while reading data from CSV file: {ex.Message}");
            }
        }



        private bool PepperYourSfaLib()
        {
            sfaLib = new SfaLib(Matrikola, psw);
            bool marc = sfaLib.LoginProd();
            if (!marc) { marc = sfaLib.LoginProd(); }
            if (!marc) { log.Error("Login non riuscito su SFA con HTMlUnit."); return false; }
            return true;
        }

    }
    //public class DbContext
    //{
    //    private const string SQL_GET_DATA = @"
    //    SELECT 
    //        ML.RIFERIMENTO, 
    //        ML.NUMERO_CLIENTE, 
    //        ML.DATA,
    //        ML.DATA_INSERIMENTO,
    //        LA.ID_RIFERIMENTO AS ID_RIFERIMENTO_ATT,
    //        LA.ID_CODICE_RESO AS ID_CODICE_RESO
    //    FROM 
    //        [MODULI_CATASTALI].[dbo].[PROD_ARCHIVIO_CATASTALI_ML] ML
    //    INNER JOIN 
    //        [dbsca].[dbo].[LAVORAZIONI_AGENTE] LA ON ML.NUMERO_CLIENTE = LA.NUMERO_CLIENTE
    //    WHERE
    //        ML.RIFERIMENTO LIKE '%-%' AND 
    //        LA.ID_RIFERIMENTO IS NOT NULL AND 
    //        LA.ID_CODICE_RESO IS NOT NULL
    //    ORDER BY 
    //        ML.DATA_INSERIMENTO DESC;";

    //    public List<ArchivioCatastali> GetData()
    //    {
    //        List<ArchivioCatastali> list = new List<ArchivioCatastali>();
    //        using (SqlDataReader rdr = SQLHelper.ExecuteReader(SQLHelper.getConnStringSimpleDoc(), CommandType.Text, SQL_GET_DATA))
    //        {
    //            while (rdr.Read())
    //            {
    //                ArchivioCatastali arch = new ArchivioCatastali();
    //                if (!rdr.IsDBNull(0)) arch.RIFERIMENTO = rdr.GetString(0);
    //                if (!rdr.IsDBNull(1)) arch.NUMERO_CLIENTE = rdr.GetString(1);
    //                if (!rdr.IsDBNull(2)) arch.DATA = rdr.GetDateTime(2);
    //                if (!rdr.IsDBNull(3)) arch.DATA_INSERIMENTO = rdr.GetDateTime(3);
    //                if (!rdr.IsDBNull(4)) arch.ID_RIFERIMENTO_ATT = rdr.GetInt32(4);
    //                if (!rdr.IsDBNull(5)) arch.ID_CODICE_RESO = rdr.GetInt32(5);

    //                list.Add(arch);
    //            }
    //        }
    //        return list;
    //    }
    //}
}
