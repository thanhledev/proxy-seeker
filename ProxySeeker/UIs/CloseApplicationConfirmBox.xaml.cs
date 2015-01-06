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

namespace ProxySeeker
{
    /// <summary>
    /// Interaction logic for CloseApplicationConfirmBox.xaml
    /// </summary>
    public partial class CloseApplicationConfirmBox : Window
    {
        #region variables

        private AppExitingMessage _answer;

        public AppExitingMessage Answer
        {
            get { return _answer; }
            set { _answer = value; }
        }

        #endregion

        #region window event handlers

        public CloseApplicationConfirmBox(Window owner)
        {
            InitializeComponent();

            if (owner != null)
            {
                this.Owner = owner;
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            }
            _answer = AppExitingMessage.Cancel;
        }

        #endregion

        #region control event handlers

        /// <summary>
        /// Handle BtnOK_Normal click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Normal_Click(object sender, RoutedEventArgs e)
        {
            _answer = AppExitingMessage.Normal;
            this.Close();
        }

        /// <summary>
        /// Handle BtnOk_Forcing click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Forcing_Click(object sender, RoutedEventArgs e)
        {
            _answer = AppExitingMessage.Forcing;
            this.Close();
        }

        /// <summary>
        /// Handle btnCancel click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _answer = AppExitingMessage.Cancel;
            this.Close();
        }

        #endregion
    }
}
