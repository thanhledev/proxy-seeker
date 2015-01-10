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
using System.Windows.Shapes;
using ProxySeeker.DataTypes;
using ProxySeeker.Handlers;
using ProxySeeker.ThirdParties;
using ProxySeeker.Utilities;
using System.Windows.Threading;

namespace ProxySeeker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region variables

        // main navigation labels
        List<Label> mainLbls = new List<Label>();

        //ui handlers
        private ApplicationUIHandler _uiHandler = new ApplicationUIHandler();
        private SystemUIView _mainView;
        private static BrushConverter bc = new BrushConverter();
        private static ThicknessConverter tc = new ThicknessConverter();        
        
        private DispatcherTimer _stopTimer = new DispatcherTimer();

        private int _tempAutoSearchProxyInterval = 0;

        #endregion

        #region Window event handlers

        public MainWindow()
        {
            InitializeComponent();

            //prepare for shutdown project
            _stopTimer.Tick += new EventHandler(stopTimer_Tick);
            _stopTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
        }

        public void LoadSystemSettings()
        {
            //load all main navigation labels
            foreach (Label lbl in Utilities.Helper.FindVisualChildren<Label>(this.dpMainNavigation))
            {
                mainLbls.Add(lbl);
            }

            //Initialize all dynamic controls from ini & xml files.
            ckbAutoSearchProxy.IsChecked = ProxyHandler.Instance.AutoSearchProxy;
            _tempAutoSearchProxyInterval = ProxyHandler.Instance.SearchProxyInterval;

            foreach (ComboBoxItem item in cbSearchProxyInterval.Items)
            {
                if (item.Tag.ToString() == _tempAutoSearchProxyInterval.ToString())
                {
                    item.IsSelected = true;
                    break;
                }
            }

            ckbTestProxies.IsChecked = ProxyHandler.Instance.TestProxy;
            ckbCheckAnonymous.IsChecked = ProxyHandler.Instance.CheckAnonymous;
            tbAnonymousCheckSite.Text = ProxyHandler.Instance.CheckAnonymousLink;
            tbProxyThread.Text = ProxyHandler.Instance.Threads.ToString();
            ckbSplitForScraper.IsChecked = ProxyHandler.Instance.SplitForScraper;
            ckbSplitForPoster.IsChecked = ProxyHandler.Instance.SplitForPoster;
            ckbSplitForForums.IsChecked = ProxyHandler.Instance.SplitForForums;

            int seconds = ProxyHandler.Instance.TimeOut / 1000;
            tbProxyTimeout.Text = seconds.ToString();

            //Setup & run the ApplicationStatisticHandler
            ApplicationStatisticsHandler.Instance.SetupHandler(this, tbCPUConsumption, tbRamConsumption, updateStatisticTextBox);
            ApplicationStatisticsHandler.Instance.RunHandler();

            //Setup & run the ApplicationMessageHandler
            ApplicationMessageHandler.Instance.RegisterHandler(this, tbLogs, updateLogs);
            ApplicationMessageHandler.Instance.RunHandler();

            //Setup the ProxyHandler
            ProxyHandler.Instance.RegisterControls(this, tbAliveProxy, tbTotalProxy, tbDeathProxy);
            ProxyHandler.Instance.RegisterActions(updateStatisticTextBox);

            _uiHandler.SetupHandle(this, hiddenChange, showChange);
            _mainView = SystemUIView.WelcomeUI;
            Change_WindowView(_mainView);
        }        

        #endregion

        #region control event handlers

        /// <summary>
        /// Handle Drag&Drop Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dpHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// <summary>
        /// Handle MinimizeInterface button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_MinimizeInterface_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        /// <summary>
        /// Handle CloseInterface button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CloseInterface_Click(object sender, RoutedEventArgs e)
        {
            ApplicationMessageBoxHandler.Instance.GetExitConfirmation();
            switch (ApplicationMessageBoxHandler.Instance.exitAnswer)
            {
                case AppExitingMessage.Cancel:
                    //do nothing
                    break;
                case AppExitingMessage.Normal:
                    //do something
                    ApplicationStatisticsHandler.Instance.StopHandler();
                    _stopTimer.Start();
                    break;
                case AppExitingMessage.Forcing:
                    //do something
                    ApplicationStatisticsHandler.Instance.AbortHandler();
                    _stopTimer.Start();
                    break;
            }
        }

        /// <summary>
        /// Main label click single handler event
        /// </summary>
        private void MainLabel_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Label lbl = e.Source as System.Windows.Controls.Label;
            BrushConverter bc = new BrushConverter();
            foreach (System.Windows.Controls.Label item in mainLbls)
            {
                item.Foreground = (item.Content == lbl.Content) ? (Brush)bc.ConvertFrom("#0098c5") : (Brush)bc.ConvertFrom("#767676");
            }

            switch (lbl.Content.ToString().ToLower())
            {
                case "// guide":
                    Change_WindowView(SystemUIView.WelcomeUI);
                    break;
                case "// seeking":
                    Change_WindowView(SystemUIView.WorkingUI);
                    break;                
                case "// configure":
                    Change_WindowView(SystemUIView.SettingsUI);
                    break;
            };
        }

        /// <summary>
        /// Handle Proxy list previewmousewheel event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        /// <summary>
        /// Handle ProxyThread textbox lost focus event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbProxyThread_LostFocus(object sender, RoutedEventArgs e)
        {
            ProxyHandler.Instance.Threads = Convert.ToInt32(tbProxyThread.Text);
        }

        /// <summary>
        /// Handle ProxyTimeout textbox lost focus event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbProxyTimeout_LostFocus(object sender, RoutedEventArgs e)
        {
            ProxyHandler.Instance.TimeOut = Convert.ToInt32(tbProxyTimeout.Text);
        }

        /// <summary>
        /// Handle AutoSearchProxy checkbox checkedchanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbAutoSearchProxy_CheckedChanged(object sender, RoutedEventArgs e)
        {
            ProxyHandler.Instance.AutoSearchProxy = Convert.ToBoolean(ckbAutoSearchProxy.IsChecked);
            ProxyHandler.Instance.SwitchManager(ProxyHandler.Instance.AutoSearchProxy);
        }

        /// <summary>
        /// Handle SearchProxyInterval comboBox selection changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSearchProxyInterval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSearchProxyInterval.SelectedItem != null)
            {
                ComboBoxItem selected = (ComboBoxItem)cbSearchProxyInterval.SelectedItem;
                ProxyHandler.Instance.SearchProxyInterval = Convert.ToInt32(selected.Tag.ToString());
            }
        }

        /// <summary>
        /// Handle TestProxies checkbox checked/unchecked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbTestProxies_CheckedChanged(object sender, RoutedEventArgs e)
        {
            ProxyHandler.Instance.TestProxy = Convert.ToBoolean(ckbTestProxies.IsChecked);
        }

        /// <summary>
        /// Handle CheckAnonymous checkbox checked/unchecked event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbCheckAnonymous_CheckedChanged(object sender, RoutedEventArgs e)
        {
            ProxyHandler.Instance.CheckAnonymous = Convert.ToBoolean(ckbCheckAnonymous.IsChecked);
        }

        /// <summary>
        /// Handle AnonymousCheckSite textbox lost focus event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbAnonymousCheckSite_LostFocus(object sender, RoutedEventArgs e)
        {
            ProxyHandler.Instance.CheckAnonymousLink = tbAnonymousCheckSite.Text;
        }

        /// <summary>
        /// Handle SplitForScraper checkbox checked/unchecked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbSplitForScraper_CheckedChanged(object sender, RoutedEventArgs e)
        {
            ProxyHandler.Instance.SplitForScraper = Convert.ToBoolean(ckbSplitForScraper.IsChecked);
        }

        /// <summary>
        /// Handle SplitForPoster checkbox checked/unchecked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbSplitForPoster_CheckedChanged(object sender, RoutedEventArgs e)
        {
            ProxyHandler.Instance.SplitForPoster = Convert.ToBoolean(ckbSplitForPoster.IsChecked);
        }

        /// <summary>
        /// Handle SplitForForums checkbox checked/unchecked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbSplitForForums_CheckedChanged(object sender, RoutedEventArgs e)
        {
            ProxyHandler.Instance.SplitForForums = Convert.ToBoolean(ckbSplitForForums.IsChecked);
        }

        /// <summary>
        /// Handle Control button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnControl_Click(object sender, RoutedEventArgs e)
        {
            ProxyHandler.Instance.RunHandler();
        }

        #endregion

        #region action list

        /// <summary>
        /// Action for update CPU & RAM consumption
        /// </summary>
        Action<Window, System.Windows.Controls.TextBox, string> updateStatisticTextBox = (wd, tb, value) =>
        {
            wd.Dispatcher.Invoke(new Action(() =>
            {
                tb.Text = value;
            }));
        };

        /// <summary>
        /// Action for UI Handler (hidden action)
        /// </summary>
        Action<Window, List<System.Windows.Controls.DockPanel>, bool> hiddenChange = (wd, ctr, flag) =>
        {
            wd.Dispatcher.Invoke(new Action(() =>
            {
                foreach (var i in ctr)
                {
                    i.Visibility = (flag) ? Visibility.Hidden : Visibility.Visible;
                }
            }));
        };

        /// <summary>
        /// Action for UI Handler (show action)
        /// </summary>
        Action<Window, List<System.Windows.Controls.DockPanel>, bool> showChange = (wd, ctr, flag) =>
        {
            wd.Dispatcher.Invoke(new Action(() =>
            {
                foreach (var i in ctr)
                {
                    i.Visibility = (flag) ? Visibility.Visible : Visibility.Hidden;
                }
            }));
        };

        /// <summary>
        /// Action for log textbox
        /// </summary>
        Action<Window, TextBox, string> updateLogs = (wd, tb, message) =>
        {
            wd.Dispatcher.Invoke(new Action(() =>
            {
                tb.AppendText(message);
                tb.AppendText(Environment.NewLine);
                tb.ScrollToEnd();
            }));
        };        

        Action<Window, StackPanel, DockPanel> ToogleSelectedProxyDisplay = (wd, sp, dp) =>
        {
            wd.Dispatcher.Invoke(new Action(() =>
            {
                foreach (DockPanel i in Helper.FindVisualChildren<DockPanel>(sp))
                {
                    i.Background = i == dp ? (Brush)bc.ConvertFrom("#464545") : (Brush)bc.ConvertFrom("Black");
                }
            }));
        };

        #endregion

        #region utility functions

        /// <summary>
        /// Handle StopTimer tick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopTimer_Tick(object sender, EventArgs e)
        {
            if (ApplicationStatisticsHandler.Instance.IsStoppingCompleted())
            {
                _stopTimer.Stop();
                System.Windows.Application.Current.Shutdown();
            }
        }

        #endregion

        #region change UI views

        /// <summary>
        /// Change main window interface based on SystemUIView
        /// </summary>
        /// <param name="view"></param>
        private void Change_WindowView(SystemUIView view)
        {
            _uiHandler.ResetHandle();
            switch (view)
            {
                case SystemUIView.WelcomeUI:
                    Display_WelcomeInterface();
                    break;
                case SystemUIView.WorkingUI:
                    Display_WorkingInterface();
                    break;
                case SystemUIView.SettingsUI:
                    Display_SettingsInterface();
                    break;
            }
        }

        /// <summary>
        /// Display Welcome Interface
        /// </summary>
        private void Display_WelcomeInterface()
        {
            //add visible dockpanels
            _uiHandler.AddShow(dpUserWelcome);

            //add hidden dockpanels
            _uiHandler.AddHidden(dpWorking);
            _uiHandler.AddHidden(dpSettings);

            _uiHandler.RunHandle(true);
        }

        /// <summary>
        /// Display working interface
        /// </summary>
        private void Display_WorkingInterface()
        {
            //add visible dockpanels
            _uiHandler.AddShow(dpWorking);

            //add hidden dockpanels
            _uiHandler.AddHidden(dpUserWelcome);
            _uiHandler.AddHidden(dpSettings);

            _uiHandler.RunHandle(true);
        }

        /// <summary>
        /// Display Settings Interface
        /// </summary>
        private void Display_SettingsInterface()
        {
            //add visible dockpanels
            _uiHandler.AddShow(dpSettings);

            //add hidden dockpanels
            _uiHandler.AddHidden(dpWorking);
            _uiHandler.AddHidden(dpUserWelcome);

            _uiHandler.RunHandle(true);
        }

        #endregion                                       

        
        
    }
}
