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
        private BrushConverter bc = new BrushConverter();
        private ThicknessConverter tc = new ThicknessConverter();
        private FontWeightConverter fwc = new FontWeightConverter();

        private DispatcherTimer _stopTimer = new DispatcherTimer();

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

            //Setup & run the ApplicationStatisticHandler
            ApplicationStatisticsHandler.Instance.SetupHandler(this, tbCPUConsumption, tbRamConsumption, updateStatisticTextBox);
            ApplicationStatisticsHandler.Instance.RunHandler();

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
