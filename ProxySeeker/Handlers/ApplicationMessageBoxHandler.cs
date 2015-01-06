using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using ProxySeeker.Utilities;
using ProxySeeker.DataTypes;
using ProxySeeker.ThirdParties;

namespace ProxySeeker
{
    public sealed class ApplicationMessageBoxHandler
    {
        #region variables

        private static ApplicationMessageBoxHandler _instance = null;
        private static readonly object _locker = new object();
        private static readonly object _actionLocker = new object();

        private ConfirmMessageBox _confirmMessageBox;
        private CloseApplicationConfirmBox _closeConfirmMessageBox;

        private Window _owner;

        public Window Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public bool Answer { get; set; }
        public AppExitingMessage exitAnswer { get; set; }

        #endregion

        #region constructors

        public static ApplicationMessageBoxHandler Instance
        {
            get
            {
                lock (_locker)
                {
                    if (_instance == null)
                    {
                        _instance = new ApplicationMessageBoxHandler();
                    }
                }
                return _instance;
            }
        }

        #endregion        

        #region private functions



        #endregion

        #region public functions

        /// <summary>
        /// Setup the window owner of this confirmMessageBox
        /// </summary>
        /// <param name="owner"></param>
        public void SetupOwner(Window owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Get user confirmation for particular action
        /// </summary>
        /// <param name="message"></param>
        public void GetConfirmation(AppMessage message)
        {
            lock (_actionLocker)
            {
                if (!ApplicationHandler.Instance.IsStopNotify)
                {
                    _confirmMessageBox = new ConfirmMessageBox(_owner, message);
                    _confirmMessageBox.ShowDialog();
                    Answer = _confirmMessageBox.Answer;
                }
                else
                    Answer = true;
            }
        }

        /// <summary>
        /// Get user confirmation for exiting application
        /// </summary>
        public void GetExitConfirmation()
        {
            lock (_actionLocker)
            {
                _closeConfirmMessageBox = new CloseApplicationConfirmBox(_owner);
                _closeConfirmMessageBox.ShowDialog();
                exitAnswer = _closeConfirmMessageBox.Answer;
            }
        }

        #endregion

        #region utility functions



        #endregion
    }
}
