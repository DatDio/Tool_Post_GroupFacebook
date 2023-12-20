using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tool_Facebook.Helper
{
    public class SeleniumHelper
    {
        public SeleniumHelper()
        {

        }
        public static bool Click(ChromeDriver driver, By locator, int loop = 20, int count = 0)
        {
            for (int i = 0; i < loop; i++)
            {
                try
                {
                    driver.FindElements(locator)[count].Click();

                    return true;
                }
                catch
                {
                    Thread.Sleep(1000);
                }


            }


            return false;
        }
        public static void Scroll(ChromeDriver driver)
        {
            try
            {
                driver.ExecuteScript("window.scrollBy(0, 600);");
            }
            catch { }

        }
        public static bool GetAttributeTym(ChromeDriver driver)
        {
            try
            {
                if (driver.FindElements(By.ClassName("tiktok-15e07yc-ButtonActionItem"))[0].GetAttribute("aria-pressed") == "false")
                {
                    return false;
                }
            }
            catch
            {

            }

            return true;
        }
        public static int GetLenghtElement(ChromeDriver driver, By locator)
        {
            try
            {
                return driver.FindElement(locator).GetAttribute("innerHTML").Length;
            }
            catch
            {

            }
            return 1;
        }
        public static string GetTextElement(ChromeDriver driver, By locator, int count = 0)
        {
            try
            {
                //	var text = driver.FindElement(locator).Text;
                return driver.FindElements(locator)[count].Text;
            }
            catch
            {

            }
            return "";
        }
        public static bool GetEnableElement(ChromeDriver driver, By locator)
        {
            try
            {
                if (driver.FindElement(locator).Enabled)
                    return true;
            }
            catch
            {

            }
            return false;
        }
        public static bool WaitElement(ChromeDriver driver, By locator, int loop = 20)
        {
            for (int i = 0; i < loop; i++)
            {
                try
                {
                    driver.FindElement(locator);

                    return true;
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }
            return false;
        }
        public static bool WaitElementHidden(ChromeDriver driver, By locator, int loop = 20)
        {
            for (int i = 0; i < loop; i++)
            {
                try
                {
                    driver.FindElement(locator);
                    Thread.Sleep(1000);
                }
                catch
                {
                    return true;

                }
            }
            return false;
        }
        public static bool WaitElementsHidden(ChromeDriver driver, List<By> locators, int loop = 20)
        {
            bool hidden = false;
            for (int i = 0; i < loop; i++)
            {
                for (int j = 0; j < locators.Count; j++)
                {
                    try
                    {
                        driver.FindElement(locators[j]);
                        hidden = false;
                    }
                    catch
                    {
                        hidden = true;
                    }

                }
                if (hidden)
                    return true;
                Thread.Sleep(1000);
            }
            return false;
        }
        public static bool SendKeys(ChromeDriver driver, By locator, string content, int loop = 20, int count = 0)
        {
            for (int i = 0; i < loop; i++)
            {
                try
                {
                    driver.FindElements(locator)[count].SendKeys(content);

                    return true;
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }
            return false;
        }
        public static List<IWebElement> FindElements(ChromeDriver driver, By locator)
        {

            try
            {
                return driver.FindElements(locator).ToList();

            }
            catch
            {
                return new List<IWebElement>();
            }

        }
        public static bool UrlChange(ChromeDriver driver, string url_before, int loop = 20)
        {
            for (int i = 0; i < loop; i++)
            {
                try
                {

                    if (driver.Url != url_before)
                        return true;
                }
                catch
                {

                }
                Thread.Sleep(1000);
            }
            return false;
        }
        public static IWebElement FindElementInViewport(ChromeDriver driver, By locator)
        {
            // Lấy kích thước của viewport
            try
            {
                var viewportWidth = (long)((IJavaScriptExecutor)driver).ExecuteScript("return window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;");
                var viewportHeight = (long)((IJavaScriptExecutor)driver).ExecuteScript("return window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;");

                // Tìm tất cả các phần tử theo bộ chọn (XPath, ID, CSS selector, v.v.)
                var elements = driver.FindElements(locator);

                foreach (var element in elements)
                {
                    var elementLocation = element.Location;
                    if (elementLocation.X >= 0 && elementLocation.Y >= 0 && elementLocation.X < viewportWidth && elementLocation.Y < viewportHeight)
                    {
                        // Trả về phần tử nếu nó nằm trong viewport
                        return element;
                    }
                }
            }
            catch
            {

            }
            return null;

        }
    }
}
