using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;

using NUnit.Framework;

namespace Gilmond
{
    class Program
    {
        IWebDriver driver = new FirefoxDriver();
        IJavaScriptExecutor executor;
        int changeSpeed = 1000;
        string errorURL = "https://www.gilmond.com/products/smar";
        bool fakeError = true;
        bool thorough = true;

        static void Main(string[] args)
        {

        }

        [SetUp]
        public void Initialize()
        {
            executor = driver as IJavaScriptExecutor;
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://www.gilmond.com/about");
        }

        [TearDown]
        public void CleanUp()
        {
            driver.Close();
        }

        public void Wait()
        {
            System.Threading.Thread.Sleep(changeSpeed);
        }

        // Scrolls through the page based on the webpage size and window size
        public void ScrollCheck(IWebDriver _driver)
        {
            int windowHeight = _driver.Manage().Window.Size.Height;
            int scrollActions = (int)Math.Ceiling((float)(driver.FindElement(By.LinkText("Twitter")).Location.Y / windowHeight));

            for (int n = 0; n <= scrollActions; ++n)
            {
                Wait();
                executor.ExecuteScript("window.scrollBy(0, " + (windowHeight - (windowHeight/10)).ToString() + ")");
            }
        }

        // Travels to a given Y position on the webpage
        public void TravelTo(int _locationY)
        {
            long curPosition = (long)executor.ExecuteScript("return window.pageYOffset");
            int distance = Math.Abs(_locationY - (int)curPosition);
            if (_locationY < curPosition) distance *= -1;
            executor.ExecuteScript("window.scrollBy(0, " + distance.ToString() + ")");
        }

        #region Literal

        [Test]
        public void LiteralCheck()
        {
            driver.FindElements(By.ClassName("dropdown"))[2].Click();
            Wait();
            driver.FindElement(By.LinkText("Blog")).Click();
            Wait();
            driver.FindElement(By.ClassName("older-posts")).Click();
            Wait();
            driver.FindElement(By.ClassName("panel-heading")).Click();
            Wait();
            driver.FindElement(By.Id("GilmondLogo")).Click();
            ScrollCheck(driver);
            Wait();

            TravelTo(1150);
            Wait();
            driver.FindElements(By.ClassName("btn-lg"))[2].Click();
            Wait();
            int goal = driver.FindElements(By.ClassName("img-responsive"))[2].Location.Y;
            executor.ExecuteScript("window.scrollBy(0, " + goal.ToString() + ")");
            Wait();

            driver.FindElement(By.LinkText("Contact")).Click();
            TravelTo(950);
            Wait();
            driver.FindElement(By.Id("fa2eaa74-5cba-4d80-ffd0-46353405e699")).SendKeys("Test Name"); // Looks for the Name text box
            Wait();
            driver.FindElement(By.Id("8530fc2d-a7bf-445a-c0c7-fe0d4fb83916")).SendKeys("Test E-Mail"); // Looks for the E-Mail text box
            Wait();
            driver.FindElement(By.Id("eb9ea854-e864-4394-8652-85fac12462f9")).SendKeys("Test Comment"); // Looks for the Comment text box

            System.Threading.Thread.Sleep(5000);
        }

        #endregion

        #region Automatic

        [Test]
        public void AutoCheck()
        {
            List<IWebElement> elements = driver.FindElements(By.ClassName("dropdown")).ToList();

            int tabs = elements.Count; // Helps keep track of what tab the program is on

            Wait();

            // Work through the tabs and traverse each sub page
            for (int i = 0; i < tabs; ++i)
            {
                Console.WriteLine("[Info]: Now checking: " + elements[i].Text);
                CheckDropDown(i, elements[i], driver);
                elements.Clear();
                elements = driver.FindElements(By.ClassName("dropdown")).ToList();

                Console.WriteLine(" ");
            }

            driver.FindElement(By.LinkText("Contact")).Click();
            TravelTo(950);

            Wait();
            driver.FindElement(By.Id("fa2eaa74-5cba-4d80-ffd0-46353405e699")).SendKeys("Test Name"); // Looks for the Name text box
            Wait();
            driver.FindElement(By.Id("8530fc2d-a7bf-445a-c0c7-fe0d4fb83916")).SendKeys("Test E-Mail"); // Looks for the E-Mail text box
            Wait();
            driver.FindElement(By.Id("eb9ea854-e864-4394-8652-85fac12462f9")).SendKeys("Test Comment"); // Looks for the Comment text box
            Wait();

            driver.FindElement(By.Id("GilmondLogo")).Click();

            ScrollCheck(driver);

            System.Threading.Thread.Sleep(5000);
        }

        public void CheckDropDown(int _curDropDown, IWebElement _element, IWebDriver _driver)
        {
            _element.Click();

            string dropDownName = _element.Text;
            List<string> tabs = _element.Text.Split('\r').ToList();

            IWebElement dropDownOption = _driver.FindElement(By.PartialLinkText(tabs[0]));

            // Goes to each page within the dropdown menu
            for (int i = 1; i < tabs.Count; ++i)
            {
                try
                {
                    tabs[i] = tabs[i].Substring(1);
                    dropDownOption = _driver.FindElement(By.PartialLinkText(tabs[i]));

                    if (fakeError && i == 2)
                    {
                        driver.Navigate().GoToUrl(errorURL);
                    }
                    else
                    {
                        dropDownOption.Click();
                    }

                    if (_driver.Url == errorURL)
                    {
                        Console.WriteLine("     [Error]: Page 404: " + tabs[i]);
                    }

                    if (thorough)
                    {
                        ScrollCheck(_driver);
                    }

                    if (i + 1 < tabs.Count)
                    {
                        _driver.FindElements(By.ClassName("dropdown")).ToList()[_curDropDown].Click();
                    }

                    Wait();
                }
                catch (Exception e)
                {
                    Console.WriteLine("     [Warning]: " + e.Message);
                }
            }

            Wait();
        }

        #endregion
    }
}
