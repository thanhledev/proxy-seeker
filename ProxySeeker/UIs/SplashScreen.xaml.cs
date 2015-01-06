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
using System.Windows.Threading;

namespace ProxySeeker
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        #region variables

        private DispatcherTimer timer = new DispatcherTimer();

        #endregion

        #region constructors

        public SplashScreen()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(dispatcherTimer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            timer.Start();
        }

        #endregion

        #region private functions

        /// <summary>
        /// Monitoring whether application system is ready
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (ApplicationHandler.Instance.IsLoadingCompleled())
            {
                CloseSplashScreen();                
                timer.Stop();
            }
        }

        /// <summary>
        /// CLose the splash screen & open mainwindow interface
        /// </summary>
        private void CloseSplashScreen()
        {
            MainWindow main = new MainWindow();
            main.LoadSystemSettings();
            main.Show();
            this.Close();
        }

        #endregion

        #region public functions



        #endregion

        #region utility functions



        #endregion
    }
}
