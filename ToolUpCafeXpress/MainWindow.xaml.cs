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

namespace ToolUpCafeXpress
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Notification notification = new Notification();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = notification;
        }
        string content = "0";
        string path = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName;

        string pathLog = System.IO.Path.Combine(Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName, "log.txt");

        string ProfileFolderPath =  "Profile";
        ChromeDriver driver;
        string userName, password, x, y = "";
        bool b = true;
        ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        ManualResetEvent _pauseEvent = new ManualResetEvent(true);
        Thread _thread;
        int countTotal = 0;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (driver != null)
            {
                /*  try
                  {
                      driver.Close();
                      driver.Quit();
                  }
                  catch (Exception)
                  {
                  }*/
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

            b = true;
            _thread = new Thread(doThread);
            _thread.Start();

        }

        public void doThread()
        {
           

            this.Dispatcher.Invoke(() =>
            {
                userName = txtUser.Text;
                password = txtPassword.Text;
            });


            string p_strPath = System.IO.Path.Combine(path, "listing.xlsx");
            List<ExcelEntity> lst = readXLS(p_strPath);


            _pauseEvent.WaitOne(Timeout.Infinite);

            /*  if (_shutdownEvent.WaitOne(0))
                  break;*/

            excuteUpload(lst);





        }
        public List<ExcelEntity> readXLS(string FilePath)
        {
            List<ExcelEntity> lst = new List<ExcelEntity>();
            FileInfo existingFile = new FileInfo(FilePath);
            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                //get the first worksheet in the workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int colCount = worksheet.Dimension.End.Column;  //get Column Count
                int rowCount = worksheet.Dimension.End.Row;     //get row count
                for (int row = 2; row <= rowCount; row++)
                {

                    if (worksheet.Cells[row, 1].Value?.ToString().Trim() != null)
                    {
                        ExcelEntity excelEntity = new ExcelEntity();
                        excelEntity.FolderName = worksheet.Cells[row, 1].Value?.ToString().Trim();
                        excelEntity.ImageName = worksheet.Cells[row, 2].Value?.ToString().Trim();
                        excelEntity.Title = worksheet.Cells[row, 3].Value?.ToString().Trim();
                        excelEntity.Description = worksheet.Cells[row, 4].Value?.ToString().Trim();
                        excelEntity.Tags = worksheet.Cells[row, 5].Value?.ToString().Trim();
                        excelEntity.Stt = worksheet.Cells[row, 8].Value?.ToString().Trim();
                        lst.Add(excelEntity);
                    }
                }
            }

            return lst;
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

        public void excuteUpload(List<ExcelEntity> lst)
        {

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));


            try
            {
                notification.ActionNotifi = "Starting!";
                string p_strPath = System.IO.Path.Combine(path, "log.txt");


                FileInfo logFile = new FileInfo(p_strPath);
                if (logFile.Exists)
                {
                    content = System.IO.File.ReadAllText(p_strPath);
                }
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



                foreach (var excelOb in lst)
                {
                    _pauseEvent.WaitOne(Timeout.Infinite);

                    if (_shutdownEvent.WaitOne(0))
                        break;


                    notification.ActionNotifi = "upload: " + excelOb.ImageName +" Đã up:"+ countTotal;
                    if (Int32.Parse(content) != 0 && Int32.Parse(excelOb.Stt) <= Int32.Parse(content))
                    {
                        continue;
                    }

                    string p_strPath1 = System.IO.Path.Combine(path, excelOb.FolderName+"\\" + excelOb.ImageName);
                    FileInfo existingFile = new FileInfo(p_strPath1);
                    if (!existingFile.Exists)
                    {
                        System.IO.File.WriteAllText(pathLog, excelOb.Stt);
                        continue;
                    }

                    /*  var buttonProfile = driver.FindElement(By.CssSelector("#nav_user_menu"));
                      buttonProfile.Click();
                      var buttonUpload = driver.FindElement(By.CssSelector("#mn-manage"));
                      buttonUpload.Click();*/
                    
                      Thread.Sleep(2000);
                    driver.Url = "https://members.cafepress.com/mydesigns";
                    driver.Navigate();
                    Thread.Sleep(2000);
                    var buttonArtwork = driver.FindElement(By.XPath("/html/body/div[1]/div[9]/div[1]/div[1]/div/div[1]/div[3]"));
                    buttonArtwork.Click();

                   // wait.Until(ExpectedConditions.ElementIsVisible(By.Id("uploadImage")));
                    var inputFile = driver.FindElement(By.Id("uploadImage"));
                    inputFile.SendKeys(p_strPath1);



                    Thread.Sleep(20000);

                  driver.FindElement(By.Id("designDisplayName")).SendKeys(text: excelOb.Description);
                    Thread.Sleep(2000);
                    driver.FindElement(By.Id("designDescription")).SendKeys(text: excelOb.Description);
                    Thread.Sleep(2000);
                    driver.FindElement(By.Id("designDescription")).SendKeys(text: excelOb.Description);
                    
                    Thread.Sleep(2000);



                    if (excelOb.Tags != null)
                    {
                        List<string> lstTags = excelOb.Tags.Split(',').ToList();
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
                    driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div/div/div[1]/div[1]/div[4]/div[2]/div[5]/div[2]/label[1]/input")).Click();

                  
                    Thread.Sleep(2000);

                    var submit = driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div/div/div[1]/div[1]/div[2]/div[2]/button"));
                    submit.Click();

                    Thread.Sleep(10000);

                    System.IO.File.WriteAllText(pathLog, excelOb.Stt);
                    Thread.Sleep(2000);
                    count++;
                    countTotal++;
                    if (count == 50)
                    {
                        notification.ActionNotifi = "Đang chờ 15'";
                        Thread.Sleep(1000000);
                        count = 0;
                    }

                   
                }
                notification.ActionNotifi = "Hết cmnr.";
                b = false;
            }
            catch (Exception e)
            {
              

                Console.WriteLine(e.ToString());
                _pauseEvent.WaitOne(Timeout.Infinite);

                string Path = System.IO.Path.Combine(path, "log.txt");
                FileInfo logFile = new FileInfo(Path);
                if (logFile.Exists)
                {
                    content = System.IO.File.ReadAllText(Path);
                }
                int a = Int32.Parse(content);
                a++;
                System.IO.File.WriteAllText(pathLog, a.ToString());
                notification.ActionNotifi = "Đã có lỗi xảy ra!";
                excuteUpload(lst);
            }

        }



        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            // Signal the shutdown event
            _shutdownEvent.Set();

            // Make sure to resume any paused threads
            _pauseEvent.Set();

            // Wait for the thread to exit
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

        public void Stop()
        {
            // Signal the shutdown event
            _shutdownEvent.Set();

            // Make sure to resume any paused threads
            _pauseEvent.Set();

            // Wait for the thread to exit
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

    public class ExcelEntity
    {
        string folderName, imageName, title, description, tags, stt, url;

        public string FolderName { get => folderName; set => folderName = value; }
        public string ImageName { get => imageName; set => imageName = value; }
        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public string Tags { get => tags; set => tags = value; }
        public string Stt { get => stt; set => stt = value; }
        public string Url { get => url; set => url = value; }

    }
    public class Notification : INotifyPropertyChanged
    {
        protected string action;
        protected string resize;

        public string ActionNotifi
        {
            get { return action; }
            set
            {
                if (action != value)
                {
                    action = value;
                    OnPropertyChanged("ActionNotifi");

                }
            }
        }

        public string ActionResize
        {
            get { return resize; }
            set
            {
                if (resize != value)
                {
                    resize = value;
                    OnPropertyChanged("ActionResize");

                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
            }

        }



    }
}


