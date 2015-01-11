using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProxySeeker.DataTypes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace ProxySeeker
{
    public sealed class ApplicationStatisticsHandler
    {
        #region variables

        private static readonly object _locker = new object();
        private static ApplicationStatisticsHandler _instance = null;

        private Thread _cpuWatcher;
        private Thread _ramWatcher;
        private bool _isRunning = false;

        private static Process p = Process.GetCurrentProcess();
        private static PerformanceCounter _cpuCounter = new PerformanceCounter("Process", "% Processor Time", p.ProcessName);
        private static PerformanceCounter _ramCounter = new PerformanceCounter("Process", "Working Set - Private", p.ProcessName);

        private Window _currentWD;

        public Window CurrentWD
        {
            get { return _currentWD; }
            set { _currentWD = value; }
        }

        private TextBox _cpuTextBox;

        public TextBox CpuTextBox
        {
            get { return _cpuTextBox; }
            set { _cpuTextBox = value; }
        }

        private TextBox _ramTextBox;

        public TextBox RamTextBox
        {
            get { return _ramTextBox; }
            set { _ramTextBox = value; }
        }

        public Action<Window, TextBox, string> updateCPUStatistic;
        public Action<Window, TextBox, string> updateRamStatistic;

        private static readonly object _stopApplicationStatisticLocker = new object();
        private bool _stopApplicationStatistic = false;

        private static readonly object _finishApplicationStatisticLocker = new object();
        private int _finishApplicationStatisticCounter = 0;

        #endregion

        #region constructors

        public static ApplicationStatisticsHandler Instance
        {
            get
            {
                lock (_locker)
                {
                    if (_instance == null)
                        _instance = new ApplicationStatisticsHandler();
                }
                return _instance;
            }
        }

        ApplicationStatisticsHandler()
        {
            _cpuTextBox = new TextBox();
            _ramTextBox = new TextBox();
        }

        #endregion

        #region private functions

        /// <summary>
        /// Monitor application's CPU usage in real-time
        /// </summary>
        private void MonitoringCpuUsage()
        {
            while (true)
            {
                lock (_stopApplicationStatisticLocker)
                {
                    if (_stopApplicationStatistic)
                        goto StopThread;
                }
                dynamic firstValue = _cpuCounter.NextValue();
                Thread.Sleep(1000);
                dynamic secondValue = _cpuCounter.NextValue();

                updateCPUStatistic.Invoke(_currentWD, _cpuTextBox, string.Format("{0:N1} %", secondValue));
            }
            StopThread: if (_stopApplicationStatistic) StopStatisticHandle();
        }

        /// <summary>
        /// Monitor application's RAM usage in real-time
        /// </summary>
        private void MonitoringRamUsage()
        {
            while (true)
            {
                lock (_stopApplicationStatisticLocker)
                {
                    if (_stopApplicationStatistic)
                        goto StopThread;
                }
                double ram = _ramCounter.NextValue();
                updateRamStatistic.Invoke(_currentWD, _ramTextBox, string.Format("{0:N2} MB", ram / 1024 / 1024));
                Thread.Sleep(1000);
            }
            StopThread: if (_stopApplicationStatistic) StopStatisticHandle();
        }

        /// <summary>
        /// Logic activities when stop ApplicationStatisticHandler
        /// </summary>
        private void StopStatisticHandle()
        {
            lock (_finishApplicationStatisticLocker)
                _finishApplicationStatisticCounter++;

            if (_finishApplicationStatisticCounter == 2)
            {
                _isRunning = false;
            }
        }

        #endregion

        #region public functions

        /// <summary>
        /// Setup ApplicationStatisticHandler
        /// </summary>
        /// <param name="wd"></param>
        /// <param name="cpu"></param>
        /// <param name="ram"></param>
        /// <param name="updateTextBox"></param>
        public void SetupHandler(Window wd, TextBox cpu, TextBox ram, Action<Window, TextBox, string> updateTextBox)
        {
            _currentWD = wd;
            _cpuTextBox = cpu;
            _ramTextBox = ram;
            updateCPUStatistic = updateTextBox;
            updateRamStatistic = updateTextBox;

            _cpuWatcher = new Thread(MonitoringCpuUsage);
            _cpuWatcher.IsBackground = true;
            _ramWatcher = new Thread(MonitoringRamUsage);
            _ramWatcher.IsBackground = true;
        }

        /// <summary>
        /// Begin running the ApplicationStatisticHandler
        /// </summary>
        public void RunHandler()
        {
            _cpuWatcher.Start();
            _ramWatcher.Start();
            _isRunning = true;
        }

        /// <summary>
        /// Try to stop the ApplicationStatisticHandler normally
        /// </summary>
        public void StopHandler()
        {
            lock (_stopApplicationStatisticLocker)
                _stopApplicationStatistic = true;
        }

        /// <summary>
        /// Try to stop the ApplicationStatisticHandler seriously
        /// </summary>
        public void AbortHandler()
        {
            _cpuWatcher.Abort();
            _ramWatcher.Abort();
            _isRunning = false;
        }

        /// <summary>
        /// Check whether ApplicationStatisticHandler has stopped working
        /// </summary>
        /// <returns></returns>
        public bool IsStoppingCompleted()
        {
            return _isRunning ? false : true;
        }

        #endregion

        #region utility functions



        #endregion
    }
}
