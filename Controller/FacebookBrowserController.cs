using Newtonsoft.Json;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool_Facebook.Helper;
using Tool_Facebook.Model;

namespace Tool_Facebook.Controller
{
    public class FacebookBrowserController
    {
		Random random;
		AccountModel account;
		FacebookAPIController apiFB;
		public FacebookBrowserController(AccountModel account)
		{
			this.account = account;
			random = new Random();
		}
		public ResultModel InteractFacebookWWW()
		{
			string url = "";
			DateTime currentTime = DateTime.Now;
			DateTime targetTime = currentTime.AddMinutes(15);
			//int randomLike = 0;
			apiFB = new FacebookAPIController();
			try
			{
				account.driver.Url = "https://www.facebook.com/";
			}
			catch
			{

			}
			if (SeleniumHelper.WaitElement(account.driver, By.Id("email")))
			{
				//var status = LoginByCookie();
				FunctionHelper.EditValueColumn(account, "C_Status", "Đang login!", true);
				if (!SeleniumHelper.SendKeys(account.driver, By.Id("email"), account.C_UID))
					return ResultModel.LoginFail;
				Thread.Sleep(1000);
				if (!SeleniumHelper.SendKeys(account.driver, By.Id("pass"), account.C_Password))
					return ResultModel.LoginFail;
				Thread.Sleep(2000);
				if (!SeleniumHelper.Click(account.driver, By.Name("login")))
					return ResultModel.LoginFail;
				if (SeleniumHelper.WaitElement(account.driver, By.Id("email")))
				{
					return ResultModel.LoginFail;
				}
				//url = account.driver.Url;
				//if (!SeleniumHelper.UrlChange(account.driver, url))
				//{
				//	return ResultModel.LoginFail;
				//}
			}
			url = account.driver.Url;
			if (url.Contains("956/"))
				return ResultModel.CheckPoint956;
			else if (url.Contains("282/"))
				return ResultModel.CheckPoint282;
			else if (url.Contains("checkpoint"))
				return ResultModel.CheckPoint;
			var allCookies = account.driver.Manage().Cookies.AllCookies;
			account.C_Cookie = JsonConvert.SerializeObject(allCookies);
			FunctionHelper.EditValueColumn(account, "C_Status", "Đang tương tác...", true);
			while (DateTime.Now < targetTime)
			{
				//Đọc thông báo
				SeleniumHelper.Click(account.driver, By.CssSelector("a[href=\"/notifications/\"]"));
				SeleniumHelper.Scroll(account.driver, random.Next(400, 1000));
				SeleniumHelper.Click(account.driver, By.CssSelector("a[href=\"/notifications/\"]"));
				Thread.Sleep(random.Next(10000, 20000));
				//SeleniumHelper.Click(account.driver, By.CssSelector("a[href=\"/notifications/\"]"));
				//Lướt newfeed
				for (int i = 0; i < 30; i++)
				{
					SeleniumHelper.Scroll(account.driver, random.Next(400, 1000));
					Thread.Sleep(random.Next(10000, 20000));
					if (i == 2 || i == 10)
					{
						//SeleniumHelper.Click(account.driver, By.CssSelector("div[aria-label=\"Thích\"]"), count: i);
						try
						{
							IWebElement button = account.driver.FindElements(By.CssSelector("div[aria-label=\"Thích\"]"))[i];
							var t = button.Text;
							Actions actions = new Actions(account.driver);
							actions.MoveToElement(button).Click().Perform();
						}
						catch
						{

						}

					}

				}
				//
				try
				{
					account.driver.Url = "https://www.facebook.com/watch";
				}
				catch
				{

				}
				for (int i = 0; i < 20; i++)
				{
					SeleniumHelper.Scroll(account.driver, random.Next(400, 1000));
					Thread.Sleep(random.Next(40000, 120000));
				}
			}
			return ResultModel.Success;
		}
		public ResultModel InteractFacebookMbassic()
		{
			DateTime timeInteract = DateTime.Now.AddMinutes(15);
			//int randomLike = 0;
			apiFB = new FacebookAPIController();
			try
			{
				account.driver.Url = "https://mbasic.facebook.com/";
			}
			catch
			{

			}

			if (SeleniumHelper.WaitElement(account.driver, By.Id("m_login_email")))
			{
				var status = LoginByCookie();
				if (!status)
					return ResultModel.Fail;
			}
			while (timeInteract.Minute - DateTime.Now.Minute > 0)
			{
				try
				{
					account.driver.Url = "https://mbasic.facebook.com/";
				}
				catch
				{

				}
				for (int i = 0; i < 50; i++)
				{
					SeleniumHelper.Scroll(account.driver, random.Next(400, 1000));
					Thread.Sleep(random.Next(1000, 6000));
				}
				//Tham gia group
				try
				{
					account.driver.Url = "https://mbasic.facebook.com/search/top/?q=shopee&refid=46&_rdr";
				}
				catch
				{

				}
				if (SeleniumHelper.GetTextElement(account.driver, By.ClassName("cr")) == "Tham gia")
				{
					SeleniumHelper.Click(account.driver, By.ClassName("cr"));
					try
					{
						//account.driver.ExecuteScript("document.querySelector('button[data-e2e=\"edit-profile-save\"]').removeAttribute('disabled')");

						IWebElement button = account.driver.FindElement(By.ClassName("cr"));
						var t = button.Text;
						Actions actions = new Actions(account.driver);
						actions.MoveToElement(button).Click().Perform();

					}
					catch
					{

					}
					try
					{
						account.driver.Navigate().Back();
					}
					catch
					{

					}
				}
				if (SeleniumHelper.GetTextElement(account.driver, By.ClassName("cr"), 1) == "Tham gia")
				{
					try
					{
						//account.driver.ExecuteScript("document.querySelector('button[data-e2e=\"edit-profile-save\"]').removeAttribute('disabled')");

						IWebElement button = account.driver.FindElements(By.ClassName("cr"))[1];
						var t = button.Text;
						Actions actions = new Actions(account.driver);
						actions.MoveToElement(button).Click().Perform();

					}
					catch
					{

					}
					try
					{
						account.driver.Navigate().Back();
					}
					catch
					{

					}

				}
				//Đọc thông báo
				var elements = account.driver.FindElements(By.ClassName("bf"));
				foreach (var element in elements)
				{
					if (element.Text == "Thông báo")
					{
						element.Click();
						Thread.Sleep(random.Next(10000, 20000));
					}
				}
				//Thêm Bạn Bè
				foreach (var element in elements)
				{
					if (element.Text == "Bạn bè")
					{
						element.Click();
						Thread.Sleep(2000);
					}
				}

				Thread.Sleep(random.Next(10000, 20000));
				//
				try
				{
					account.driver.Url = "https://mbasic.facebook.com/watch";
				}
				catch
				{

				}
				for (int i = 0; i < 20; i++)
				{
					SeleniumHelper.Scroll(account.driver, random.Next(400, 1000));
					Thread.Sleep(random.Next(40000, 120000));
				}
			}
			return ResultModel.Success;
		}
		public bool LoginByCookie()
		{
			FunctionHelper.EditValueColumn(account, "C_Status", "Đến trang login ...");

			var cookies = account.C_Cookie.Split(';');
			foreach (var cookie in cookies)
			{
				try
				{
					var arr = cookie.Split("=".ToCharArray(), 2);
					var ck = new Cookie(arr[0].Trim(), arr[1].Trim());
					account.driver.Manage().Cookies.AddCookie(ck);
				}
				catch
				{
				}
			}
			try
			{
				account.driver.Url = "https://www.facebook.com/";
			}
			catch
			{
				return false;
			}
			if (SeleniumHelper.WaitElement(account.driver, By.Id("m_login_email")))
			{
				return false;
			}
			return true;
		}
	}
}
