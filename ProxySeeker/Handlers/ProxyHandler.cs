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
        private static readonly object _finishProxyManagerLocker = new object();
        private string _savePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ProxySeeker\");
        private string _systemIniFile = System.IO.Path.Combine(Environment.CurrentDirectory, @"\system.ini");
        private string _dbLocation = System.IO.Path.Combine(Environment.CurrentDirectory, @"\locations.txt");

        private long wNumber = 16777216;
        private long xNumber = 65536;
        private long yNumber = 256;

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
        private string _googleTestServer = "https://www.google.com/search?btnG=1&filter=0&start=0&q=jack+the+ripper";

        private List<IpGeoLocation> _locations = new List<IpGeoLocation>();
        private Queue<string> _cloneProxyFeeder = new Queue<string>();
        public List<string> _publicProxyFeeder =
            new List<string>
            {
                "http://aliveproxy.com/high-anonymity-proxy-list/",
                "http://aliveproxy.com/us-proxy-list/",
                "http://aliveproxy.com/ru-proxy-list/",
                "http://aliveproxy.com/jp-proxy-list/",
                "http://aliveproxy.com/ca-proxy-list/",
                "http://aliveproxy.com/fr-proxy-list/",
                "http://aliveproxy.com/gb-proxy-list/",
                "http://aliveproxy.com/de-proxy-list/",
                "http://aliveproxy.com/anonymous-proxy-list/",
                "http://aliveproxy.com/transparent-proxy-list/",
                "http://checkerproxy.net/all_proxy",
                "http://www.cool-tests.com/Azerbaijan-proxy.php",
                "http://www.cool-tests.com/Albania-proxy.php",
                "http://www.cool-tests.com/Algeria-proxy.php",
                "http://www.cool-tests.com/United-States-proxy.php",
                "http://www.cool-tests.com/United-Kingdom-proxy.php",
                "http://www.cool-tests.com/Netherlands-Antilles-proxy.php",
                "http://www.cool-tests.com/United-Arab-Emirates-proxy.php",
                "http://www.cool-tests.com/Argentina-proxy.php",
                "http://www.cool-tests.com/South-Africa-proxy.php",
                "http://www.cool-tests.com/Bangladesh-proxy.php",
                "http://www.cool-tests.com/Belarus-proxy.php",
                "http://www.cool-tests.com/Belgium-proxy.php",
                "http://www.cool-tests.com/Bulgaria-proxy.php",
                "http://www.cool-tests.com/Brazil-proxy.php",
                "http://www.cool-tests.com/Hungary-proxy.php",
                "http://www.cool-tests.com/Venezuela-proxy.php",
                "http://www.cool-tests.com/VietNam-proxy.php",
                "http://www.cool-tests.com/Ghana-proxy.php",
                "http://www.cool-tests.com/Guatemala-proxy.php",
                "http://www.cool-tests.com/Germany-proxy.php",
                "http://www.cool-tests.com/Netherlands-proxy.php",
                "http://www.cool-tests.com/Hong-Kong-proxy.php",
                "http://www.cool-tests.com/Honduras-proxy.php",
                "http://www.cool-tests.com/Greece-proxy.php",
                "http://www.cool-tests.com/Georgia-proxy.php",
                "http://www.cool-tests.com/Denmark-proxy.php",
                "http://www.cool-tests.com/Europe-proxy.php",
                "http://www.cool-tests.com/Egypt-proxy.php",
                "http://www.cool-tests.com/Israel-proxy.php",
                "http://www.cool-tests.com/India-proxy.php",
                "http://www.cool-tests.com/Indonesia-proxy.php",
                "http://www.cool-tests.com/Iraq-proxy.php",
                "http://www.cool-tests.com/Iran-proxy.php",
                "http://www.cool-tests.com/Ireland-proxy.php",
                "http://www.cool-tests.com/Spain-proxy.php",
                "http://www.cool-tests.com/Italy-proxy.php",
                "http://www.cool-tests.com/Kazakhstan-proxy.php",
                "http://www.cool-tests.com/Cambodia-proxy.php",
                "http://www.cool-tests.com/Canada-proxy.php",
                "http://www.cool-tests.com/Kenya-proxy.php",
                "http://www.cool-tests.com/China-proxy.php",
                "http://www.cool-tests.com/Colombia-proxy.php",
                "http://www.cool-tests.com/Costa-Rica-proxy.php",
                "http://www.cool-tests.com/Latvia-proxy.php",
                "http://www.cool-tests.com/Lebanon-proxy.php",
                "http://www.cool-tests.com/Lithuania-proxy.php",
                "http://www.cool-tests.com/Luxembourg-proxy.php",
                "http://www.cool-tests.com/Macedonia-proxy.php",
                "http://www.cool-tests.com/Malaysia-proxy.php",
                "http://www.cool-tests.com/Maldives-proxy.php",
                "http://www.cool-tests.com/Mexico-proxy.php",
                "http://www.cool-tests.com/Moldova-proxy.php",
                "http://www.cool-tests.com/Mongolia-proxy.php",
                "http://www.cool-tests.com/Myanmar-proxy.php",
                "http://www.cool-tests.com/Nepal-proxy.php",
                "http://www.cool-tests.com/Nigeria-proxy.php",
                "http://www.cool-tests.com/New-Zealand-proxy.php",
                "http://www.cool-tests.com/Pakistan-proxy.php",
                "http://www.cool-tests.com/Paraguay-proxy.php",
                "http://www.cool-tests.com/Peru-proxy.php",
                "http://www.cool-tests.com/Poland-proxy.php",
                "http://www.cool-tests.com/Romania-proxy.php",
                "http://www.cool-tests.com/Russian-Federation-proxy.php",
                "http://www.cool-tests.com/El-Salvador-proxy.php",
                "http://www.cool-tests.com/Saudi-Arabia-proxy.php",
                "http://www.cool-tests.com/Serbia-proxy.php",
                "http://www.cool-tests.com/Slovakia-proxy.php",
                "http://www.cool-tests.com/Slovenia-proxy.php",
                "http://www.cool-tests.com/Thailand-proxy.php",
                "http://www.cool-tests.com/Taiwan-proxy.php",
                "http://www.cool-tests.com/Tanzania-proxy.php",
                "http://www.cool-tests.com/Turkey-proxy.php",
                "http://www.cool-tests.com/Uzbekistan-proxy.php",
                "http://www.cool-tests.com/Ukraine-proxy.php",
                "http://www.cool-tests.com/Philippines-proxy.php",
                "http://www.cool-tests.com/Finland-proxy.php",
                "http://www.cool-tests.com/France-proxy.php",
                "http://www.cool-tests.com/Croatia-proxy.php",
                "http://www.cool-tests.com/Chile-proxy.php",
                "http://www.cool-tests.com/Sweden-proxy.php",
                "http://www.cool-tests.com/Switzerland-proxy.php",
                "http://www.cool-tests.com/Ecuador-proxy.php",
                "http://www.cool-tests.com/South-Korea-proxy.php",
                "http://www.cool-tests.com/Japan-proxy.php",
                "http://fineproxy.org/",
                "http://www.getproxy.jp/",
                "http://www.getproxy.jp/default/2",
                "http://www.getproxy.jp/default/3",
                "http://www.getproxy.jp/default/4",
                "http://www.getproxy.jp/default/5",
                "http://www.google-proxy.net/",
                "http://www.hotvpn.com/ru/proxies/",
                "http://www.hotvpn.com/proxies/2/",
                "http://www.hotvpn.com/proxies/3/",
                "http://www.hotvpn.com/proxies/4/",
                "http://www.hotvpn.com/proxies/5/",
                "http://letushide.com/protocol/https/list_of_free_HTTPS_proxy_servers",
                "http://letushide.com/protocol/https/2/list_of_free_HTTPS_proxy_servers",
                "http://letushide.com/protocol/https/3/list_of_free_HTTPS_proxy_servers",
                "http://letushide.com/protocol/https/4/list_of_free_HTTPS_proxy_servers",
                "http://letushide.com/protocol/https/5/list_of_free_HTTPS_proxy_servers",
                "http://letushide.com/protocol/socks/",
                "http://letushide.com/protocol/socks/2/list_of_free_SOCKS_proxy_servers",
                "http://letushide.com/protocol/http/",
                "http://letushide.com/protocol/http/2/list_of_free_HTTP_proxy_servers",
                "http://letushide.com/protocol/http/3/list_of_free_HTTP_proxy_servers",
                "http://letushide.com/protocol/http/4/list_of_free_HTTP_proxy_servers",
                "http://letushide.com/protocol/http/5/list_of_free_HTTP_proxy_servers",
                "http://notan.h1.ru/hack/xwww/proxy1.html",
                "http://notan.h1.ru/hack/xwww/proxy2.html",
                "http://notan.h1.ru/hack/xwww/proxy3.html",
                "http://proxylist.sakura.ne.jp/index.htm?pages=0",
                "http://proxylist.sakura.ne.jp/index.htm?pages=1",
                "http://proxylist.sakura.ne.jp/index.htm?pages=2",
                "http://nntime.com/",
                "http://nntime.com/proxy-list-02.htm",
                "http://nntime.com/proxy-list-03.htm",
                "http://txt.proxyspy.net/proxy.txt",
                "http://www.rmccurdy.com/scripts/proxy/good.txt",
                "http://proxy-ip-list.com/download/free-proxy-list.txt",
                "http://www.shroomery.org/ythan/proxylist.php/RK=0",
                "http://50na50.net/no_anonim_http.txt",
                "http://www.romantic-collection.net/proxy.txt",
                "http://vmarte.com/proxy/proxy_all.txt",
                "http://reptv.ru/shy/proxylist.txt",
                "http://pozitiv.3owl.com/proxy.txt"
            };

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
        private bool _stopManager = false;

        public int Total;
        private List<SystemProxy> _alive;
        private Queue<SystemProxy> _testList = new Queue<SystemProxy>();
        private Queue<SystemProxy> _foundProxies = new Queue<SystemProxy>();
        private Queue<DateTime> _wakeUpPoint = new Queue<DateTime>();

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
        private int _finishScraper;

        //for interactions with main window
        private Window _currentWD;

        public Window CurrentWD
        {
            get { return _currentWD; }
            set { _currentWD = value; }
        }

        private StackPanel _proxyTable = new StackPanel();

        public StackPanel ProxyTable
        {
            get { return _proxyTable; }
            set { _proxyTable = value; }
        }

        private TextBox _aliveNumber = new TextBox();

        public TextBox AliveNumber
        {
            get { return _aliveNumber; }
            set { _aliveNumber = value; }
        }

        private TextBox _deathNumber = new TextBox();

        public TextBox DeathNumber
        {
            get { return _deathNumber; }
            set { _deathNumber = value; }
        }

        private Action<Window, StackPanel, List<DockPanel>> _updateProxyTable;
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

            if (System.IO.File.Exists(_systemIniFile))
            {
                LoadSettings();
            }
            if (System.IO.File.Exists(_dbLocation))
            {
                LoadDBLocation();
            }

            CreateWakeUpSchedule();
            _proxyManager = new Thread(proxyManager_DoWork);
            _proxyManager.IsBackground = true;

            if (_autoSearchProxy)
            {
                IsTesting = true;
                _proxyManager.Start();
            }

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
        }

        /// <summary>
        /// Load DBLocation
        /// </summary>
        private void LoadDBLocation()
        {
            using (StreamReader reader = new StreamReader(_dbLocation))
            {
                String line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] content = line.Split('|');
                    _locations.Add(new IpGeoLocation(Convert.ToInt64(content[0]), Convert.ToInt64(content[1]), content[2], content[3]));
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

                        HttpWebResponse anonyResponse = (HttpWebResponse)anonyRequest.GetResponse();
                        Encoding anonyEncoding = Encoding.UTF8;

                        var document = new HtmlDocument();
                        bool isDownload = false;
                        using (var reader = new StreamReader(anonyResponse.GetResponseStream()))
                        {
                            try
                            {
                                document.Load(reader.BaseStream, anonyEncoding);
                                isDownload = true;
                            }
                            catch (Exception ex)
                            {

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
                                            //invoke update textbox here
                                        }
                                    }
                                }
                                else
                                {
                                    lock (_enqueueLocker)
                                    {
                                        FilterProxies(currentProxy);
                                        //invoke update textbox here
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                    break;
            } while ( _testList.Count > 0);
        }

        private void tester_DoWorkComplete()
        {
            lock (_finishLocker)
                _finishWorker++;

            if (_finishWorker == _threads)
            {
                UpdateProxies();
                //invoke update proxy table & textboxes here

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

                            lock (_enqueueLocker)
                            {
                                PrepareTestProxies(proxies);
                                ApplicationMessageHandler.Instance.AddMessage("Found " + proxies.Count + " proxies at " + link);
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
                    IsTesting = true;
                    //invoke update proxy table & textbox here

                    for (int i = 0; i < _threads; i++)
                    {
                        
                    }
                }
                else
                {
                    IsScraping = false;
                    UpdateProxies(_foundProxies);
                    CreateTestList();
                    //invoke update proxy table here
                }
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
        public void RegisterActions(Action<Window, StackPanel, List<DockPanel>> updateProxyTable, Action<Window, TextBox, string> updateTextBox)
        {
            _updateProxyTable = updateProxyTable;
            _updateNumber = updateTextBox;
        }

        /// <summary>
        /// Register all controls to proxy's system
        /// </summary>
        /// <param name="wd"></param>
        /// <param name="tbAlive"></param>
        /// <param name="tbDeath"></param>
        /// <param name="proxyTable"></param>
        public void RegisterControls(Window wd, TextBox tbAlive, TextBox tbDeath, StackPanel proxyTable)
        {
            _currentWD = wd;
            _aliveNumber = tbAlive;
            _deathNumber = tbDeath;
            _proxyTable = proxyTable;
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
                IsScraping = true;
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
            foreach (var feed in _publicProxyFeeder)
            {
                _cloneProxyFeeder.Enqueue(feed);
            }
        }

        /// <summary>
        /// Prepare test queue
        /// </summary>
        /// <param name="proxies"></param>
        private void PrepareTestProxies(List<SystemProxy> proxies)
        {
            foreach (var proxy in proxies)
                _foundProxies.Enqueue(proxy);
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

        #endregion        
    }
}
