using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;
using System.Diagnostics;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Threading;

namespace ProxySeeker
{
    public sealed class ApplicationMessageHandler
    {
        #region variables

        //instance & locker
        private static ApplicationMessageHandler _instance = null;
        private static readonly object _instanceLocker = new object();

        //using variables
        private static readonly object _usingLocker = new object();
        private Queue<string> _messages = new Queue<string>();
        private string _datetimeFormat = "dd-MM HH:mm:ss";
        private int _interval;

        //window controls
        private Window _currentWD;

        public Window CurrentWD
        {
            get { return _currentWD; }            
        }

        private TextBox _messageTB;

        public TextBox MessageTB
        {
            get { return _messageTB; }            
        }

        private Action<Window, TextBox, string> _updateTextBox;

        public Action<Window, TextBox, string> UpdateTextBox
        {
            get { return _updateTextBox; }            
        }

        //for threading
        private Thread _worker;
        public ManualResetEvent _signal = new ManualResetEvent(false);

        private bool _isStop;

        public bool IsStop
        {
            get { return _isStop; }            
        }

        private bool _stopHandler;
        private static readonly object _stopLocker = new object();
        
        #endregion

        #region constructors

        public static ApplicationMessageHandler Instance
        {
            get
            {
                lock (_instanceLocker)
                {
                    if (_instance == null)
                    {
                        _instance = new ApplicationMessageHandler();
                    }
                }
                return _instance;
            }
        }

        ApplicationMessageHandler()
        {
            _interval = 20;
            _messageTB = new TextBox();
            _messages = new Queue<string>();
            _isStop = false;
            _stopHandler = false;
            _worker = new Thread(worker_DoWork);
            _worker.IsBackground = true;            
        }

        #endregion

        #region private functions

        /// <summary>
        /// ApplicationMessageHandler worker doWork function
        /// </summary>
        private void worker_DoWork()
        {
            do
            {
                _signal.WaitOne();
                lock (_stopLocker)
                {
                    if (_stopHandler)
                        break;
                }

                string message = string.Empty;

                lock (_usingLocker)
                {
                    if (_messages.Count > 0)
                        message = _messages.Dequeue();
                }

                if (message != string.Empty)
                    _updateTextBox.Invoke(_currentWD, _messageTB, CreateMessage(message));

                Thread.Sleep(_interval);

            } while (true);
        }

        /// <summary>
        /// ApplicationMessageHandler worker doWorkComplete function
        /// </summary>
        private void worker_DoWorkComplete()
        {
            _isStop = true;
        }

        #endregion

        #region public functions

        /// <summary>
        /// Register controls to handler
        /// </summary>
        /// <param name="wd"></param>
        /// <param name="tb"></param>
        /// <param name="action"></param>
        public void RegisterHandler(Window wd, TextBox tb, Action<Window, TextBox, string> action)
        {
            _currentWD = wd;
            _messageTB = tb;
            _updateTextBox = action;
            _signal.Set();
        }

        /// <summary>
        /// Start handler
        /// </summary>
        public void RunHandler()
        {
            _worker.Start();
            lock (_stopLocker)
            {
                _stopHandler = false;
                _isStop = false;
            }
        }

        /// <summary>
        /// Stop handler
        /// </summary>
        public void StopHandler()
        {
            lock (_stopLocker)
                _stopHandler = true;
        }

        /// <summary>
        /// Abort handler
        /// </summary>
        public void AbortHandler()
        {
            lock (_stopLocker)
            {
                _worker.Abort();
                _stopHandler = true;
                _isStop = true;
            }
        }

        /// <summary>
        /// Pause handler
        /// </summary>
        public void PauseHandler()
        {
            _signal.Reset();
        }

        /// <summary>
        /// Resume handler
        /// </summary>
        public void ResumeHandler()
        {
            _signal.Set();
        }

        public void AddMessage(string message)
        {
            lock (_usingLocker)
                _messages.Enqueue(message);
        }

        public void Initialize()
        {

        }

        #endregion

        #region utility functions

        /// <summary>
        /// Create message from a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string CreateMessage(string input)
        {
            return DateTime.Now.ToString(_datetimeFormat) + " : " + input;
        }

        #endregion
    }
}
