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
using ProxySeeker.Utilities;

namespace ProxySeeker
{
    /// <summary>
    /// Interaction logic for ConfirmMessageBox.xaml
    /// </summary>
    public partial class ConfirmMessageBox : Window
    {
        #region variables

        private bool _answer;

        public bool Answer
        {
            get { return _answer; }
            set { _answer = value; }
        }

        #endregion

        #region window event handlers

        public ConfirmMessageBox(Window owner, AppMessage message)
        {
            InitializeComponent();

            if (owner != null)
            {
                this.Owner = owner;
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            }
            lblMessage.Content = message.ToString();
        }

        #endregion

        #region control event handlers

        /// <summary>
        /// Handle Ok Button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _answer = true;
            this.Close();
        }

        /// <summary>
        /// Handle Cancel Button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _answer = false;
            this.Close();
        }

        /// <summary>
        /// Handle StopNotify checker checked changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbStopNotify_CheckedChanged(object sender, RoutedEventArgs e)
        {
            CheckBox ck = sender as CheckBox;
            if (ck.IsChecked.Value)
                ApplicationHandler.Instance.IsStopNotify = true;
            else
                ApplicationHandler.Instance.IsStopNotify = false;
        }

        #endregion

        #region private functions



        #endregion

        #region public functions



        #endregion

        #region utility functions



        #endregion
    }
}
