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
        
        private bool _isAlive;

        public bool IsAlive
        {
            get { return _isAlive; }
            set { _isAlive = value; }
        }

        private string _countryCode;

        public string CountryCode
        {
            get { return _countryCode; }
            set { _countryCode = value; }
        }

        private double _speed;

        public double Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        #endregion

        #region constructors

        public SystemProxy()
            : this("", "", "", "", false, "", 0)
        {
        }

        public SystemProxy(string ip, string port, string username, string password)
            : this(ip, port, username, password, false, "", 0)
        {
        }

        public SystemProxy(string ip, string port, string username, string password, bool isAlive, string countryCode, double speed)
        {
            this._proxyIp = ip;
            this._proxyPort = port;
            this._username = username;
            this._password = password;            
            this._isAlive = isAlive;
            this._countryCode = countryCode;
            this._speed = speed;
        }

        #endregion

        #region utility Methods

        public override string ToString()
        {            
            return String.Format("{0}:{1}", _proxyIp, _proxyPort);
        }

        public string showProxySpeed()
        {
            if (_speed < 0)
                return "N/A";
            else
                return _speed.ToString("N3") + " s";
        }
        
        #endregion
    }
}
