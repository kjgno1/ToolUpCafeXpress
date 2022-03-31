using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToolUpCafePress;
using Image = System.Drawing.Image;

namespace ToolUpCafeXpress
{
    public partial class MainWindow : Window
    {
        DatabaseContext context = new DatabaseContext();
        public Notification notification = new Notification();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = notification;
                _lstImg = context.getListNotUploaded();
                context.UpdateStatus("1332183_1","3");
        }
        string path = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName;

        string ProfileFolderPath =  "Profile";
        ChromeDriver driver;
        string userName, password, x, y = "";
        
        ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        ManualResetEvent _pauseEvent = new ManualResetEvent(true);
        private Thread _thread;
        private int _countTotal;
        private List<ImageInfo> _lstImg;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (driver != null)
            {
                  try
                  {
                      driver.Close();
                      driver.Quit();
                  }
                  catch (Exception)
                  {
                  }
            }
            else
            {
                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;
                ChromeOptions options = new ChromeOptions();

                if (!Directory.Exists(ProfileFolderPath))
                {
                    Directory.CreateDirectory(ProfileFolderPath);
                }

                if (Directory.Exists(ProfileFolderPath))
                {

                    options.AddExcludedArgument("enable-automation");
                    options.AddArguments("user-data-dir=" + path + "\\ChromeProfile");
                    // options.AddArguments("user-data-dir=" + ProfileFolderPath + "\\0");
                    //options.AddArgument("--disable-extensions");
                    //options.AddExtension("./1.6.0_0.crx");

                    driver = new ChromeDriver(service, options);
                }
            }

            _thread = new Thread(DoThread);
            _thread.Start();

        }

        public void DoThread()
        {
           

            Dispatcher.Invoke(() =>
            {
                userName = txtUser.Text;
                password = txtPassword.Text;
            });

            
           



            _pauseEvent.WaitOne(Timeout.Infinite);

            excuteUpload(_lstImg);





        }
       

        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void excuteUpload(List<ImageInfo> lst)
        {

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var currentItem = "";
            try
            {
                notification.ActionNotifi = "Starting!";
              
                var count = 0;
               
               

                driver.Url = "https://members.cafepress.com/mydesigns";
                driver.Navigate();

                if (IsElementPresent(By.CssSelector("input#loginEmail")))
                {
                    var inputUser = driver.FindElement(By.CssSelector("input#loginEmail"));
                    inputUser.SendKeys(Keys.Control + "a");
                    inputUser.SendKeys(Keys.Delete);
                    inputUser.SendKeys(text: userName);
                    var inputPwd = driver.FindElement(By.CssSelector("input#loginPassword"));
                    inputPwd.SendKeys(Keys.Control + "a");
                    inputPwd.SendKeys(Keys.Delete);
                    inputPwd.SendKeys(text: password);
                    driver.FindElement(By.XPath("//*[@id='isPersistentWrapper']/label/span")).Click();

                    Thread.Sleep(2000);
                    driver.FindElement(By.XPath("//*[@id='loginForm']/div[4]/button")).Click();

                    Thread.Sleep(2000);
                    driver.Url = "https://www.cafepress.com/";
                    Thread.Sleep(2000);
                    driver.Url = "https://members.cafepress.com/mydesigns";
                    Thread.Sleep(2000);
                    _pauseEvent.WaitOne(Timeout.Infinite);

                    /* if (_shutdownEvent.WaitOne(0))
                         break;*/


                }



                foreach (var item in lst)
                {
                    _pauseEvent.WaitOne(Timeout.Infinite);
                    currentItem = item.Id;
                    if (_shutdownEvent.WaitOne(0))
                        break;


                    notification.ActionNotifi = "upload: " + item.Name +" Đã up:"+ _countTotal;
                  /*  if (Int32.Parse(content) != 0 && Int32.Parse(excelOb.Stt) <= Int32.Parse(content))
                    {
                        continue;
                    }*/

                    string p_strPath1 = System.IO.Path.Combine(path, "img2"+"\\" + item.Id+".png");
                    FileInfo existingFile = new FileInfo(p_strPath1);
                    if (!existingFile.Exists)
                    {
                        context.UpdateStatus(item.Id,"3");
                        continue;
                    }
                    Thread.Sleep(2000);
                    driver.Url = "https://members.cafepress.com/mydesigns";
                    driver.Navigate();
                    Thread.Sleep(2000);
                    var buttonArtwork = driver.FindElement(By.XPath("/html/body/div[1]/div[9]/div[1]/div[1]/div/div[1]/div[3]"));
                    buttonArtwork.Click();

                    var inputFile = driver.FindElement(By.Id("uploadImage"));
                    inputFile.SendKeys(p_strPath1);



                    Thread.Sleep(20000);

                    driver.FindElement(By.Id("designDisplayName")).SendKeys(text: item.Descriptions);
                    Thread.Sleep(2000);
                    driver.FindElement(By.Id("designDescription")).SendKeys(text: item.Descriptions);
                    Thread.Sleep(2000);



                    if (item.Tags != null)
                    {
                        List<string> lstTags = item.Tags.Split(',').ToList();
                        var inputTag = driver.FindElement(By.Id("designSearchTags"));
                        foreach (string tag in lstTags)
                        {
                            if (tag.Length < 20)
                            {
                                inputTag.SendKeys(tag);
                                inputTag.SendKeys(Keys.Enter);
                                Thread.Sleep(1000);
                            }
                        }

                    }

                    Thread.Sleep(2000);
                    driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div/div/div[1]/div[1]/div[4]/div[2]/div[5]/div[2]/label[1]")).Click();

                  
                    Thread.Sleep(2000);

                    var submit = driver.FindElement(By.ClassName("action-button"));
                    submit.Click();
                    
                    context.UpdateStatus(item.Id,"1");
                    
                    Thread.Sleep(10000);

                    Thread.Sleep(2000);
                    count++;
                    _countTotal++;
                    if (count == 50)
                    {
                        notification.ActionNotifi = "Đang chờ 15'";
                        Thread.Sleep(1000000);
                        count = 0;
                    }

                    if (_countTotal == 200)
                    {
                        Thread.Sleep(10000000);
                        _countTotal = 0;
                    }

                   
                }
                notification.ActionNotifi = "Hết cmnr.";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                context.UpdateStatus(currentItem,"3");
                //update error
                _pauseEvent.WaitOne(Timeout.Infinite);
                notification.ActionNotifi = "Đã có lỗi xảy ra!";
                excuteUpload(context.getListNotUploaded());
            }

        }



        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            _shutdownEvent.Set();

            _pauseEvent.Set();

            _thread.Join();

        }
        public void Pause()
        {
            _pauseEvent.Reset();
        }

        public void Resume()
        {
            _pauseEvent.Set();
        }

        private void btn_download_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                Parallel.ForEach(
                    _lstImg,
                    new ParallelOptions {MaxDegreeOfParallelism = 10},
                    x => FileUltil.Download(x.Url,  x.Id + ".png"));
            }).Start();

        }

        private void btn_resize_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                Parallel.ForEach(
                    _lstImg,
                    new ParallelOptions {MaxDegreeOfParallelism = 10},
                    x =>
                    {
                        if (File.Exists("img/" + x.Name))
                        {
                            Image image = Image.FromFile("img/" + x.Name );
                            FileUltil.ScaleImage(image, 960,x.Name);
                        }
                        
                    });
            }).Start();
        }

        public void Stop()
        {
            _shutdownEvent.Set();

            _pauseEvent.Set();

            _thread.Join();
        }

        private void pauseBtn_Click(object sender, RoutedEventArgs e)
        {
            _pauseEvent.Reset();
        }

        private void remuseBtn_Click(object sender, RoutedEventArgs e)
        {
            _pauseEvent.Set();
        }




    }
    
    
}


