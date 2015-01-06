using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProxySeeker.DataTypes
{
    public class SystemProxy
    {
        #region variables

        private string _proxyIp;

        public string ProxyIp
        {
            get { return _proxyIp; }
            set { _proxyIp = value; }
        }

        private string _proxyPort;

        public string ProxyPort
        {
            get { return _proxyPort; }
            set { _proxyPort = value; }
        }

        private string _username;

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private bool _isPrivate;

        public bool IsPrivate
        {
            get { return _isPrivate; }
            set { _isPrivate = value; }
        }

        private bool _isAlive;

        public bool IsAlive
        {
            get { return _isAlive; }
            set { _isAlive = value; }
        }

        #endregion

        #region constructors

        public SystemProxy()
            : this("", "", "", "", false, false)
        {
        }

        public SystemProxy(string ip, string port, string username, string password)
            : this(ip, port, username, password, false, false)
        {
        }

        public SystemProxy(string ip, string port, string username, string password, bool isPrivate, bool isAlive)
        {
            this._proxyIp = ip;
            this._proxyPort = port;
            this._username = username;
            this._password = password;
            this._isPrivate = isPrivate;
            this._isAlive = isAlive;
        }

        #endregion

        #region utility Methods

        public override string ToString()
        {
            if (IsPrivate)
                return String.Format("{0}:{1}:{2}:{3}", _proxyIp, _proxyPort, _username, _password);
            else
                return String.Format("{0}:{1}", _proxyIp, _proxyPort);
        }

        #endregion
    }
}
