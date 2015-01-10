using System;
using System.Collections.Generic;
using System.Linq;
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
using System.ComponentModel;
using System.Windows.Threading;
using System.IO;
using System.Net;
using System.Web;
using HtmlAgilityPack;
using ProxySeeker.DataTypes;
using ProxySeeker.Utilities;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace ProxySeeker
{
    public sealed class ProxyHandler
    {
        #region variables

        //for instance
        private static ProxyHandler _instance = null;
        private static readonly object _instanceLocker = new object();
        private static readonly object _loadingLocker = new object();
        private static readonly object _usingLocker = new object();
        private static readonly object _foundLocker = new object();
        private static readonly object _stopLocker = new object();
        private static readonly object _dieLocker = new object();
        private static readonly object _finishProxyManagerLocker = new object();
        private string _savePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ProxySeeker\");
        private string _systemIniFile = Environment.CurrentDirectory + "\\system.ini";        
        private string _sourcesFile = Environment.CurrentDirectory + "\\sources.txt";
        static string _statsFile = Environment.CurrentDirectory + "\\proxy_stats.html";
        
        private long wNumber = 16777216;
        private long xNumber = 65536;
        private long yNumber = 256;
        private string proxyPattern = @"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b:\d{2,5}";
        //for working variables
        private static string _defaultValue = "???";
        private bool _autoSearchProxy;

        public bool AutoSearchProxy
        {
            get { return _autoSearchProxy; }
            set { _autoSearchProxy = value; }
        }

        private int _searchProxyInterval;

        public int SearchProxyInterval
        {
            get { return _searchProxyInterval; }
            set { _searchProxyInterval = value; }
        }

        private bool _testProxy;

        public bool TestProxy
        {
            get { return _testProxy; }
            set { _testProxy = value; }
        }

        private bool _checkAnonymous;

        public bool CheckAnonymous
        {
            get { return _checkAnonymous; }
            set { _checkAnonymous = value; }
        }

        private string _checkAnonymousLink;

        public string CheckAnonymousLink
        {
            get { return _checkAnonymousLink; }
            set { _checkAnonymousLink = value; }
        }

        private bool _splitForScraper;

        public bool SplitForScraper
        {
            get { return _splitForScraper; }
            set { _splitForScraper = value; }
        }

        private bool _splitForPoster;

        public bool SplitForPoster
        {
            get { return _splitForPoster; }
            set { _splitForPoster = value; }
        }

        private bool _splitForForums;

        public bool SplitForForums
        {
            get { return _splitForForums; }
            set { _splitForForums = value; }
        }

        private int _threads;

        public int Threads
        {
            get { return _threads; }
            set { _threads = value; }
        }

        private int _timeOut;

        public int TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        private static readonly object _dequeueLocker = new object();
        private static readonly object _enqueueLocker = new object();
        private static readonly object _finishLocker = new object();        
        
        private Queue<string> _cloneProxyFeeder = new Queue<string>();
        private ProxySourceCollection _sources = new ProxySourceCollection();
        private SplitProxyCollection _split = new SplitProxyCollection();
        private bool _isTesting;

        public bool IsTesting
        {
            get { return _isTesting; }
            internal set { _isTesting = value; }
        }

        private bool _isScraping;

        public bool IsScraping
        {
            get { return _isScraping; }
            internal set { _isScraping = value; }
        }

        private Thread[] _tester = new Thread[500];
        private Thread[] _founder = new Thread[10];

        private Thread _proxyManager;
        private Thread _writer;
        private bool _stopManager = false;
        private bool _stopWriter = false;

        public int Total;
        private List<SystemProxy> _alive;
        private Queue<SystemProxy> _testList = new Queue<SystemProxy>();
        private Queue<SystemProxy> _foundProxies = new Queue<SystemProxy>();
        private Queue<DateTime> _wakeUpPoint = new Queue<DateTime>();

        private int _death = 0;

        public List<SystemProxy> Alive
        {
            get { return _alive; }
        }

        private Queue<SystemProxy> _publicProxies = new Queue<SystemProxy>();

        public Queue<SystemProxy> PublicProxies
        {
            get { return _publicProxies; }
        }

        private int _finishWorker;
        private int _finishFounder;

        //for interactions with main window
        private Window _currentWD;

        public Window CurrentWD
        {
            get { return _currentWD; }
            set { _currentWD = value; }
        }        

        private TextBox _aliveNumber = new TextBox();

        public TextBox AliveNumber
        {
            get { return _aliveNumber; }
            set { _aliveNumber = value; }
        }

        private TextBox _totalNumber = new TextBox();

        public TextBox TotalNumber
        {
            get { return _totalNumber; }
            set { _totalNumber = value; }
        }

        private TextBox _deathNumber = new TextBox();

        public TextBox DeathNumber
        {
            get { return _deathNumber; }
            set { _deathNumber = value; }
        }

        private Action<Window, TextBox, string> _updateNumber;        
        public bool isLoadingCompleted = false;

        #endregion

        #region constructors

        public static ProxyHandler Instance
        {
            get
            {
                lock (_instanceLocker)
                {
                    if (_instance == null)
                    {
                        _instance = new ProxyHandler();
                    }
                }
                return _instance;
            }
        }

        ProxyHandler()
        {
            lock (_loadingLocker)
                isLoadingCompleted = false;

            Total = 0;
            _isTesting = false;
            _isScraping = false;
            _alive = new List<SystemProxy>();
            _finishWorker = 0;
            _finishFounder = 0;

            if (!System.IO.Directory.Exists(_savePath))
            {
                System.IO.Directory.CreateDirectory(_savePath);
            }

            if (System.IO.File.Exists(_systemIniFile))
            {
                LoadSettings();
            }
            if(System.IO.File.Exists(_sourcesFile))
            {
                LoadSources();
            }

            CreateWakeUpSchedule();
            _proxyManager = new Thread(proxyManager_DoWork);
            _proxyManager.IsBackground = true;

            _writer = new Thread(WriteProxyToFile);
            _writer.IsBackground = true;

            //if (_autoSearchProxy)
            //{
            //    IsTesting = true;
            //    _proxyManager.Start();
            //}

            lock (_loadingLocker)
                isLoadingCompleted = true;
        }

        #endregion

        #region save & load functions

        /// <summary>
        /// Load all settings
        /// </summary>
        private void LoadSettings()
        {
            //load settings
            _autoSearchProxy = Convert.ToBoolean(IniHelper.GetIniFileString(_systemIniFile, "proxy", "autosearchproxy", _defaultValue));
            _searchProxyInterval = Convert.ToInt32(IniHelper.GetIniFileString(_systemIniFile, "proxy", "searchproxyinterval", _defaultValue));
            _testProxy = Convert.ToBoolean(IniHelper.GetIniFileString(_systemIniFile, "proxy", "testproxy", _defaultValue));
            _checkAnonymous = Convert.ToBoolean(IniHelper.GetIniFileString(_systemIniFile, "proxy", "checkanonymous", _defaultValue));
            _checkAnonymousLink = IniHelper.GetIniFileString(_systemIniFile, "proxy", "checkanonymouslink", _defaultValue);
            _threads = Convert.ToInt32(IniHelper.GetIniFileString(_systemIniFile, "proxy", "threads", _defaultValue));
            _timeOut = Convert.ToInt32(IniHelper.GetIniFileString(_systemIniFile, "proxy", "timeout", _defaultValue));
            _splitForScraper = Convert.ToBoolean(IniHelper.GetIniFileString(_systemIniFile, "proxy", "splitforscraper", _defaultValue));
            _splitForPoster = Convert.ToBoolean(IniHelper.GetIniFileString(_systemIniFile, "proxy", "splitforposter", _defaultValue));
            _splitForForums = Convert.ToBoolean(IniHelper.GetIniFileString(_systemIniFile, "proxy", "splitForForums", _defaultValue));
        }

        private void LoadSources()
        {
            using (StreamReader reader = new StreamReader(_sourcesFile, Encoding.UTF8))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    _sources.Sources.Add(new ProxySource(line));
                }
            }
        }

        public void SaveSystem()
        {
            SaveProxySettings();
        }

        private void SaveProxySettings()
        {
            IniHelper.WriteKey(_systemIniFile, "proxy", "autosearchproxy", _autoSearchProxy.ToString());
            IniHelper.WriteKey(_systemIniFile, "proxy", "searchproxyinterval", _searchProxyInterval.ToString());
            IniHelper.WriteKey(_systemIniFile, "proxy", "testproxy", _testProxy.ToString());
            IniHelper.WriteKey(_systemIniFile, "proxy", "checkanonymous", _checkAnonymous.ToString());
            IniHelper.WriteKey(_systemIniFile, "proxy", "checkanonymouslink", _checkAnonymousLink.ToString());
            IniHelper.WriteKey(_systemIniFile, "proxy", "threads", _threads.ToString());
            IniHelper.WriteKey(_systemIniFile, "proxy", "timeout", _timeOut.ToString());
            IniHelper.WriteKey(_systemIniFile, "proxy", "splitforscraper", _splitForScraper.ToString());
            IniHelper.WriteKey(_systemIniFile, "proxy", "splitforposter", _splitForPoster.ToString());
            IniHelper.WriteKey(_systemIniFile, "proxy", "splitforforums", _splitForForums.ToString());
        }        

        #endregion

        #region testing funtions

        private void tester_DoWork()
        {
            do
            {
                SystemProxy currentProxy = new SystemProxy();

                lock (_dequeueLocker)
                {
                    if (_testList.Count > 0)
                        currentProxy = _testList.Dequeue();
                    else
                        break;
                }

                if (currentProxy.ProxyIp != "" && currentProxy.ProxyPort != "")
                {
                    try
                    {
                        HttpWebRequest anonyRequest = WebRequest.Create(_checkAnonymousLink) as HttpWebRequest;
                        WebProxy test = new WebProxy(currentProxy.ProxyIp, Convert.ToInt32(currentProxy.ProxyPort));
                        anonyRequest.Proxy = test;
                        anonyRequest.Timeout = _timeOut;
                        anonyRequest.KeepAlive = false;
                        anonyRequest.ReadWriteTimeout = _timeOut;
                        anonyRequest.ServicePoint.Expect100Continue = false;
                        var document = new HtmlDocument();
                        bool isDownload = false;

                        using (var anonyResponse = (HttpWebResponse)anonyRequest.GetResponse())
                        {
                            Encoding anonyEncoding = Encoding.UTF8;
                            using (var reader = new StreamReader(anonyResponse.GetResponseStream()))
                            {
                                document.Load(reader.BaseStream, anonyEncoding);
                                isDownload = true;
                            }
                        }

                        if (isDownload)
                        {
                            if (document.DocumentNode.InnerText.Contains("REQUEST_URI = /azenv.php") && document.DocumentNode.InnerText.Contains("HTTP_HOST = www.proxy-listen.de"))
                            {
                                if (_checkAnonymous)
                                {
                                    if (!document.DocumentNode.InnerText.Contains("HTTP_X_FORWARDED_FOR"))
                                    {
                                        lock (_enqueueLocker)
                                        {
                                            FilterProxies(currentProxy);
                                        }
                                    }
                                    else
                                    {
                                        //testing purpose here
                                        lock (_dieLocker)
                                        {
                                            _death++;
                                            _updateNumber.Invoke(_currentWD, _deathNumber, CreateDeathMessage(_death));
                                        }
                                    }
                                }
                                else
                                {
                                    lock (_enqueueLocker)
                                    {
                                        FilterProxies(currentProxy);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //testing purpose here
                            lock (_dieLocker)
                            {
                                _death++;
                                _updateNumber.Invoke(_currentWD, _deathNumber, CreateDeathMessage(_death));
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //testing purpose here
                        lock (_dieLocker)
                        {
                            _death++;
                            _updateNumber.Invoke(_currentWD, _deathNumber, CreateDeathMessage(_death));
                        }
                        
                    }
                }
                else
                    break;
            } while ( _testList.Count > 0);

            tester_DoWorkComplete();
        }

        private void tester_DoWorkComplete()
        {
            lock (_finishLocker)
                _finishWorker++;

            if (_finishWorker == _threads)
            {
                ApplicationMessageHandler.Instance.AddMessage("Finish testing proxies. Found  " + _alive.Count + " alive proxies.");
                UpdateProxies();
                WriteStatistics();
                SplitProxies();
                _isTesting = false;
                _finishWorker = 0;
            }
        }

        #endregion

        #region crawling functions

        /// <summary>
        /// Founder DoWork function
        /// </summary>
        private void founder_DoWork()
        {
            do
            {
                string link = "";

                lock (_foundLocker)
                {
                    if (_cloneProxyFeeder.Count > 0)
                        link = _cloneProxyFeeder.Dequeue();
                    else
                        break;
                }

                if (link != string.Empty)
                {
                    bool IsDownload = false;
                    try
                    {
                        HttpWebRequest request = WebRequest.Create(link) as HttpWebRequest;
                        request.Timeout = 30000;
                        request.KeepAlive = false;
                        request.ReadWriteTimeout = 60000;
                        
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        Encoding encoding = Encoding.UTF8;

                        var document = new HtmlDocument();
                        bool isReaded = false;

                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {                            
                            document.Load(reader.BaseStream, encoding);
                            isReaded = true;                            
                        }
                        if (isReaded)
                        {
                            IProxyScraper pScraper = ProxyScraperFactory.GetProxyScraper(link);
                            List<SystemProxy> proxies = pScraper.GetProxies(document, link);

                            if (proxies.Count > 0)
                            {
                                ApplicationMessageHandler.Instance.AddMessage("Found " + proxies.Count + " proxies at " + link);
                                lock (_enqueueLocker)
                                {
                                    _sources.ChangeFounded(link, proxies.Count);
                                    PrepareTestProxies(proxies, link);
                                }
                            }
                            else
                            {
                                ApplicationMessageHandler.Instance.AddMessage("Found " + proxies.Count + " proxies at " + link);
                                lock (_enqueueLocker)
                                {
                                    _sources.ChangeFounded(link, 0);
                                    _sources.ChangeAdded(link, 0);
                                }
                            }
                            IsDownload = true;
                        }
                    }
                    catch (Exception)
                    {
                        
                    }

                    if (!IsDownload)
                    {
                        ApplicationMessageHandler.Instance.AddMessage("Cannot find any proxy at " + link + "!");
                        lock (_enqueueLocker)
                        {
                            _sources.ChangeFounded(link, 0);
                            _sources.ChangeAdded(link, 0);
                        }
                    }
                }

            } while (true);

            founder_DoWorkComplete();
        }

        /// <summary>
        /// Founder DoWorkComplete function
        /// </summary>
        private void founder_DoWorkComplete()
        {
            lock (_finishLocker)
                _finishFounder++;

            if (_finishFounder == 10)
            {                
                if (_testProxy)
                {
                    ApplicationMessageHandler.Instance.AddMessage("Found  " + _foundProxies.Count + " proxies. Begin testing now!");
                    IsTesting = true;
                    IsScraping = false;
                    UpdateProxies(_foundProxies);
                    CreateTestList();
                    //invoke textboxes here
                    Total = _publicProxies.Count;
                    _updateNumber.Invoke(_currentWD, _aliveNumber, CreateAliveMessage(0, Total));
                    _updateNumber.Invoke(_currentWD, _totalNumber, CreateTotalMessage(Total));
                    for (int i = 0; i < _threads; i++)
                    {
                        _tester[i] = new Thread(tester_DoWork);
                        _tester[i].IsBackground = true;
                        _tester[i].Start();
                    }
                }
                else
                {
                    ApplicationMessageHandler.Instance.AddMessage("Found  " + _foundProxies.Count + " proxies.");
                    IsScraping = false;
                    UpdateProxies(_foundProxies);
                    CreateTestList();                    
                    //invoke textboxes here
                    Total = _publicProxies.Count;
                    _updateNumber.Invoke(_currentWD, _aliveNumber, CreateAliveMessage(0, Total));
                    _updateNumber.Invoke(_currentWD, _totalNumber, CreateTotalMessage(Total));
                }
                _finishFounder = 0;
            }
        }

        #endregion

        #region manager functions

        /// <summary>
        /// manager DoWork function()
        /// </summary>
        private void proxyManager_DoWork()
        {
            do
            {
                DateTime wakeUpPoint = _wakeUpPoint.Dequeue();
                bool flag = false;

                while (!flag)
                {
                    lock (_stopLocker)
                    {
                        if (_stopManager)
                            goto StopManager;
                    }

                    if (DateTime.Now > wakeUpPoint)
                    {
                        if (!IsTesting && !IsScraping)
                        {
                            flag = true;
                            IsScraping = true;
                            CloneProxyFeeder();
                            for (int i = 0; i < 10; i++)
                            {
                                _founder[i] = new Thread(founder_DoWork);
                                _founder[i].IsBackground = true;
                                _founder[i].Start();
                            }
                        }
                    }
                    else
                        Thread.Sleep(60000);
                }

            } while (true);
            StopManager: if (_stopManager) StopManager();
        }

        /// <summary>
        /// Stop function for manager thread
        /// </summary>
        private void StopManager()
        {

        }

        /// <summary>
        /// Switch manager on/off
        /// </summary>
        /// <param name="flag"></param>
        public void SwitchManager(bool flag)
        {
            if (_stopManager)
            {
                if (flag)
                {
                    _proxyManager = new Thread(proxyManager_DoWork);
                    _proxyManager.IsBackground = true;
                    _proxyManager.Start();
                }
            }
            else
            {
                if (!flag)
                    Stop();
            }
        }

        #endregion

        #region utility functions

        /// <summary>
        /// Register all actions to proxy's system
        /// </summary>
        /// <param name="updateProxyTable"></param>
        /// <param name="updateTextBox"></param>
        public void RegisterActions(Action<Window, TextBox, string> updateTextBox)
        {            
            _updateNumber = updateTextBox;
        }

        /// <summary>
        /// Register all controls to proxy's system
        /// </summary>
        /// <param name="wd"></param>
        /// <param name="tbAlive"></param>
        /// <param name="tbDeath"></param>
        /// <param name="proxyTable"></param>
        public void RegisterControls(Window wd, TextBox tbAlive, TextBox tbTotal, TextBox tbDeath)
        {
            _currentWD = wd;
            _aliveNumber = tbAlive;
            _totalNumber = tbTotal;
            _deathNumber = tbDeath;
        }

        /// <summary>
        /// Check whether proxy system has been loaded completely
        /// </summary>
        /// <returns></returns>
        public bool IsLoadingCompleled()
        {
            lock (_loadingLocker)
                return isLoadingCompleted;
        }
        
        public void Initialize()
        {

        }

        public void RunHandler()
        {
            if (!IsTesting && !IsScraping)
            {                
                _proxyManager.Start();
                ApplicationMessageHandler.Instance.AddMessage("Begin running proxy system!");
            }
            else
                ApplicationMessageHandler.Instance.AddMessage("Proxy system is already running!");
        }

        /// <summary>
        /// Create wakeup schedule to find proxy
        /// </summary>
        private void CreateWakeUpSchedule()
        {
            _wakeUpPoint.Clear();
            DateTime holdPoint = DateTime.Now;
            DateTime temp = new DateTime(holdPoint.Year, holdPoint.Month, holdPoint.Day, holdPoint.Hour, holdPoint.Minute, holdPoint.Second);

            _wakeUpPoint.Enqueue(holdPoint);

            do
            {
                temp = temp.AddMinutes(_searchProxyInterval);
                if (temp < holdPoint)
                    continue;
                else
                    _wakeUpPoint.Enqueue(temp);

            } while (temp < holdPoint.AddDays(30));
        }

        /// <summary>
        /// Clone proxy feeder list to queue
        /// </summary>
        private void CloneProxyFeeder()
        {
            _cloneProxyFeeder.Clear();
            foreach (var source in _sources.Sources)
            {
                _cloneProxyFeeder.Enqueue(source.SourceUrl);
            }
        }

        /// <summary>
        /// Prepare test queue
        /// </summary>
        /// <param name="proxies"></param>
        private void PrepareTestProxies(List<SystemProxy> proxies, string link)
        {
            int success = 0;
            foreach (var proxy in proxies)
            {
                Match match = Regex.Match(proxy.ProxyIp + ":" + proxy.ProxyPort, proxyPattern);
                if (match.Success)
                {
                    success++;
                    _foundProxies.Enqueue(proxy);
                    _sources.AddProxyToSource(link, proxy);
                }
            }
            _sources.ChangeAdded(link, success);
        }

        /// <summary>
        /// Update proxy table after crawling and testing
        /// </summary>
        private void UpdateProxies()
        {
            lock (_usingLocker)
            {
                _publicProxies.Clear();
                foreach (var i in _alive)
                {
                    if (_checkAnonymous)
                        _sources.ChangeAnonymous(i);
                    else
                        _sources.ChangeWorking(i);
                    _publicProxies.Enqueue(i);
                }
                _alive = new List<SystemProxy>();                               
            }
        }

        /// <summary>
        /// Update proxy table after crawling
        /// </summary>
        /// <param name="proxies"></param>
        private void UpdateProxies(Queue<SystemProxy> proxies)
        {
            lock (_usingLocker)
            {
                _publicProxies.Clear();
                foreach (var i in proxies)
                {
                    _publicProxies.Enqueue(i);
                }
            }
        }        

        /// <summary>
        /// Create proxy testing list
        /// </summary>
        private void CreateTestList()
        {
            lock (_usingLocker)
            {                
                _testList = CloneProxies(_publicProxies);                
            }
        }

        /// <summary>
        /// Clone a proxy queue
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private Queue<SystemProxy> CloneProxies(Queue<SystemProxy> list)
        {
            Queue<SystemProxy> temp = new Queue<SystemProxy>();

            foreach (var i in list)            
                temp.Enqueue(i);            

            return temp;
        }

        /// <summary>
        /// Add a live proxy to list
        /// </summary>
        /// <param name="proxy"></param>
        private void FilterProxies(SystemProxy proxy)
        {
            _alive.Add(proxy);            
            _updateNumber.Invoke(_currentWD, _aliveNumber, CreateAliveMessage(_alive.Count, _foundProxies.Count));                
        }

        /// <summary>
        /// Stop proxy system
        /// </summary>
        private void Stop()
        {
            lock (_stopLocker)
            {
                _stopManager = true;
            }
        }

        /// <summary>
        /// Create alive message
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private string CreateAliveMessage(int number, int total)
        {
            return "Checked : " + number + " | " + total;
        }

        /// <summary>
        /// Create total message
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private string CreateTotalMessage(int number)
        {
            return "Total : " + number;
        }

        /// <summary>
        /// Create death message
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private string CreateDeathMessage(int number)
        {
            return "Death : " + number;
        }

        /// <summary>
        /// Write proxy source's statistic
        /// </summary>
        private void WriteStatistics()
        {
            StringWriter stringWriter = new StringWriter();

            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter, string.Empty))
            {
                writer.WriteLineNoTabs("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">");
                writer.AddAttribute("xmlns", "http://www.w3.org/1999/xhtml");
                writer.AddAttribute("xml:lang", "en");

                writer.RenderBeginTag(HtmlTextWriterTag.Html);
                writer.RenderBeginTag(HtmlTextWriterTag.Head);

                writer.RenderBeginTag(HtmlTextWriterTag.Title);
                writer.Write("HTTP proxies resources");
                writer.RenderEndTag(); // end title

                writer.RenderEndTag(); // end head

                writer.WriteLine("");
                writer.RenderBeginTag(HtmlTextWriterTag.Body);

                writer.AddAttribute("align", "center");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.RenderBeginTag(HtmlTextWriterTag.P);
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.AddAttribute("size", "4");
                writer.RenderBeginTag(HtmlTextWriterTag.Font);
                writer.Write("HTTP proxies resources");
                writer.RenderEndTag(); //end font
                writer.RenderEndTag(); //end strong
                writer.RenderEndTag(); //end p
                writer.WriteLine("");
                writer.RenderBeginTag(HtmlTextWriterTag.P);
                writer.RenderBeginTag(HtmlTextWriterTag.I);
                writer.Write("Updated &nbsp;");
                writer.RenderEndTag(); //end i
                writer.Write(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"));
                writer.RenderEndTag(); //end p

                writer.RenderEndTag(); //end div

                //write table here
                writer.AddAttribute("width", "80%");
                writer.AddAttribute("align", "center");
                writer.AddAttribute("border", "0");
                writer.AddAttribute("cellspacing", "0");
                writer.AddAttribute("cellpadding", "1");

                writer.RenderBeginTag(HtmlTextWriterTag.Table);

                //write header row
                writer.AddAttribute("bgcolor", "#86a4d7");
                writer.RenderBeginTag(HtmlTextWriterTag.Tr); //begin header row

                writer.AddAttribute("width", "50");
                writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 1st td
                writer.RenderBeginTag(HtmlTextWriterTag.B);
                writer.Write("&nbsp;N");
                writer.RenderEndTag(); //end b
                writer.RenderEndTag(); //end 1st td

                writer.WriteLine("");
                writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 2nd td
                writer.RenderBeginTag(HtmlTextWriterTag.B);
                writer.Write("URL");
                writer.RenderEndTag(); //end b
                writer.RenderEndTag(); //end 2nd td

                writer.WriteLine("");
                writer.AddAttribute("width", "100");
                writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 3rd td
                writer.RenderBeginTag(HtmlTextWriterTag.B);
                writer.Write("Founded");
                writer.RenderEndTag(); //end b
                writer.RenderEndTag(); //end 3rd td

                writer.WriteLine("");
                writer.AddAttribute("width", "100");
                writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 4th td
                writer.RenderBeginTag(HtmlTextWriterTag.B);
                writer.Write("Added");
                writer.RenderEndTag(); //end b
                writer.RenderEndTag(); //end 4th td

                writer.WriteLine("");
                writer.AddAttribute("width", "100");
                writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 5th td
                writer.RenderBeginTag(HtmlTextWriterTag.B);
                writer.Write("Working");
                writer.RenderEndTag(); //end b
                writer.RenderEndTag(); //end 5th td

                writer.WriteLine("");
                writer.AddAttribute("width", "100");
                writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 6th td
                writer.RenderBeginTag(HtmlTextWriterTag.B);
                writer.Write("Anonymous");
                writer.RenderEndTag(); //end b
                writer.RenderEndTag(); //end 6th td

                writer.RenderEndTag(); //end table header row

                int count = 1;
                //write rows value
                foreach (var source in _sources.Sources)
                {
                    if (count < _sources.Sources.Count)
                    {
                        if (count % 2 == 0)
                            writer.AddAttribute("bgcolor", "#eeeef8");
                        else
                            writer.AddAttribute("bgcolor", "#ffffff");
                        writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                        writer.WriteLine("");
                        writer.AddAttribute("style", "BORDER-LEFT: #86a4d7 2px solid;  PADDING-LEFT: 5px;");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 1st td                   
                        writer.Write(count);
                        writer.RenderEndTag(); //end 1st td

                        writer.WriteLine("");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 2nd td
                        writer.AddAttribute("href", source.SourceUrl);
                        writer.AddAttribute("target", "_blank");
                        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        writer.Write(source.SourceUrl);
                        writer.RenderEndTag(); //end anchor
                        writer.RenderEndTag(); //end 2nd td

                        writer.WriteLine("");
                        writer.AddAttribute("width", "100");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 3rd td
                        writer.Write(source.Founded);
                        writer.RenderEndTag(); //end 3rd td

                        writer.WriteLine("");
                        writer.AddAttribute("width", "100");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 4th td
                        writer.Write("+" + source.Added);
                        writer.RenderEndTag(); //end 4th td

                        writer.WriteLine("");
                        writer.AddAttribute("width", "100");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 5th td
                        writer.Write(source.Working);
                        writer.RenderEndTag(); //end 5th td

                        writer.WriteLine("");
                        writer.AddAttribute("style", "BORDER-RIGHT: #86a4d7 2px solid;");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 6th td                   
                        writer.Write(source.Anonymous);
                        writer.RenderEndTag(); //end 6th td

                        writer.RenderEndTag(); //end row
                    }
                    else
                    {
                        if (count % 2 == 0)
                            writer.AddAttribute("bgcolor", "#eeeef8");
                        else
                            writer.AddAttribute("bgcolor", "#ffffff");
                        writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                        writer.WriteLine("");
                        writer.AddAttribute("style", "BORDER-LEFT: #86a4d7 2px solid;  PADDING-LEFT: 5px;BORDER-BOTTOM: #86a4d7 2px solid;");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 1st td                   
                        writer.Write(count);
                        writer.RenderEndTag(); //end 1st td

                        writer.WriteLine("");
                        writer.AddAttribute("style", "BORDER-BOTTOM: #86a4d7 2px solid;");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 2nd td
                        writer.AddAttribute("href", source.SourceUrl);
                        writer.AddAttribute("target", "_blank");
                        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        writer.Write(source.SourceUrl);
                        writer.RenderEndTag(); //end anchor
                        writer.RenderEndTag(); //end 2nd td

                        writer.WriteLine("");
                        writer.AddAttribute("width", "100");
                        writer.AddAttribute("style", "BORDER-BOTTOM: #86a4d7 2px solid;");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 3rd td
                        writer.Write(source.Founded);
                        writer.RenderEndTag(); //end 3rd td

                        writer.WriteLine("");
                        writer.AddAttribute("width", "100");
                        writer.AddAttribute("style", "BORDER-BOTTOM: #86a4d7 2px solid;");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 4th td
                        writer.Write("+" + source.Added);
                        writer.RenderEndTag(); //end 4th td

                        writer.WriteLine("");
                        writer.AddAttribute("width", "100");
                        writer.AddAttribute("style", "BORDER-BOTTOM: #86a4d7 2px solid;");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 5th td
                        writer.Write(source.Working);
                        writer.RenderEndTag(); //end 5th td

                        writer.WriteLine("");
                        writer.AddAttribute("style", "BORDER-RIGHT: #86a4d7 2px solid;BORDER-BOTTOM: #86a4d7 2px solid;");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); //begin 6th td                   
                        writer.Write(source.Anonymous);
                        writer.RenderEndTag(); //end 6th td

                        writer.RenderEndTag(); //end row
                    }
                    count++;
                }

                writer.RenderEndTag(); //end table

                writer.RenderEndTag(); //end body
                writer.RenderEndTag(); //end html
            }

            using (StreamWriter writer = new StreamWriter(_statsFile, false))
            {
                writer.Write(stringWriter.ToString());
            }
        }

        /// <summary>
        /// Split proxies and write to appdata folder
        /// </summary>
        private void SplitProxies()
        {
            int splitNumber = 0;

            if (_splitForScraper)
                splitNumber++;
            if (_splitForPoster)
                splitNumber++;
            if (_splitForForums)
                splitNumber++;

            if (splitNumber > 0)
            {
                if (splitNumber == 1)
                {
                    if (_splitForScraper)
                    {
                        _split.SplitForScraper = true;
                        _split._scraperProxies = _publicProxies.ToList();
                    }

                    if (_splitForPoster)
                    {
                        _split.SplitForPoster = true;
                        _split._posterProxies = _publicProxies.ToList();
                    }

                    if (_splitForForums)
                    {
                        _split.SplitForForums = true;
                        _split._forumsProxies = _publicProxies.ToList();
                    }
                }
                else
                {
                    int take = _publicProxies.Count / splitNumber;

                    int index = 0;

                    if (_splitForScraper)
                    {
                        _split.SplitForScraper = true;
                        _split._scraperProxies = _publicProxies.ToList().Skip(index).Take(take).ToList();
                        index++;
                    }

                    if (_splitForPoster)
                    {
                        _split.SplitForPoster = true;
                        _split._posterProxies = _publicProxies.ToList().Skip(index).Take(take).ToList();
                        index++;
                    }

                    if (_splitForForums)
                    {
                        _split.SplitForForums = true;
                        _split._forumsProxies = _publicProxies.ToList().Skip(index).Take(take).ToList();
                        index++;
                    }
                }
            }

            _writer.Start();
        }


        private void WriteProxyToFile()
        {
            bool isWritten = false;

            if(_split.SplitForScraper)
            {
                do
                {
                    try
                    {
                        using (FileStream scraperStream = new FileStream(_savePath + "scraper.txt", FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            using (StreamWriter scraperWriter = new StreamWriter(scraperStream,Encoding.UTF8))
                            {
                                foreach (var proxy in _split._scraperProxies)
                                    scraperWriter.WriteLine(proxy.ToString());
                            }
                        }
                        isWritten = true;
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(1000);
                    }
                } while (!isWritten);
            }

            isWritten = false;

            if (_split.SplitForPoster)
            {
                do
                {
                    try
                    {
                        using (FileStream posterStream = new FileStream(_savePath + "poster.txt", FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            using (StreamWriter posterWriter = new StreamWriter(posterStream, Encoding.UTF8))
                            {
                                foreach (var proxy in _split._posterProxies)
                                    posterWriter.WriteLine(proxy.ToString());
                            }
                        }
                        isWritten = true;
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(1000);
                    }
                } while (!isWritten);
            }

            isWritten = false;

            if (_split.SplitForForums)
            {
                do
                {
                    try
                    {
                        using (FileStream forumsStream = new FileStream(_savePath + "forums.txt", FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            using (StreamWriter forumsWriter = new StreamWriter(forumsStream, Encoding.UTF8))
                            {
                                foreach (var proxy in _split._forumsProxies)
                                    forumsWriter.WriteLine(proxy.ToString());
                            }
                        }
                        isWritten = true;
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(1000);
                    }
                } while (!isWritten);
            }

            isWritten = false;

            do
            {
                try
                {
                    using (FileStream statsStream = new FileStream(_savePath + "stats.txt", FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        using (StreamWriter statsWriter = new StreamWriter(statsStream, Encoding.UTF8))
                        {
                            statsWriter.Write(DateTime.Now.ToString());
                        }
                    }
                    isWritten = true;
                }
                catch (IOException)
                {
                    Thread.Sleep(1000);
                }

            } while (!isWritten);
        }

        #endregion        
    }
}
