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
using System.Net;
using System.Web;

namespace ProxySeeker.Handlers
{
    public class ApplicationUIHandler
    {
        #region variables

        public List<DockPanel> _hidden;
        public List<DockPanel> _show;

        public Window _currentWD;

        private Action<Window, List<DockPanel>, bool> _hiddenChanged;
        private Action<Window, List<DockPanel>, bool> _showChanged;

        #endregion

        #region constructors

        public ApplicationUIHandler()
        {
            _currentWD = new Window();
            _hidden = new List<DockPanel>();
            _show = new List<DockPanel>();
        }

        #endregion

        #region privateMethods



        #endregion

        #region publicMethods

        /// <summary>
        /// Setup the UI handler
        /// </summary>
        /// <param name="wd"></param>
        /// <param name="hiddenChanged"></param>
        /// <param name="showChanged"></param>
        public void SetupHandle(Window wd, Action<Window, List<DockPanel>, bool> hiddenChanged, Action<Window, List<DockPanel>, bool> showChanged)
        {
            _currentWD = wd;
            _hiddenChanged = hiddenChanged;
            _showChanged = showChanged;
        }

        /// <summary>
        /// Add new dockpanel to hidden
        /// </summary>
        /// <param name="ctr"></param>
        public void AddHidden(DockPanel ctr)
        {
            _hidden.Add(ctr);
        }

        /// <summary>
        /// Add new dockpanel to show
        /// </summary>
        /// <param name="ctr"></param>
        public void AddShow(DockPanel ctr)
        {
            _show.Add(ctr);
        }

        /// <summary>
        /// Run UI Handler
        /// </summary>
        /// <param name="enable"></param>
        public void RunHandle(bool enable)
        {
            if (_hidden.Count > 0)
                _hiddenChanged.Invoke(_currentWD, _hidden, false);
            if (_show.Count > 0)
                _showChanged.Invoke(_currentWD, _show, false);
        }

        /// <summary>
        /// Reverse the UI handler
        /// </summary>
        public void ReverseHandle()
        {
            if (_hidden.Count > 0)
                _hiddenChanged.Invoke(_currentWD, _hidden, false);
            if (_show.Count > 0)
                _showChanged.Invoke(_currentWD, _show, false);
        }

        /// <summary>
        /// Reset the UI handler
        /// </summary>
        public void ResetHandle()
        {
            _hidden.Clear();
            _show.Clear();
        }

        #endregion

        #region utilityMethods



        #endregion
    }
}
