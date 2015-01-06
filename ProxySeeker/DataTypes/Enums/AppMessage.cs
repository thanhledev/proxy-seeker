using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProxySeeker.DataTypes
{
    public sealed class AppMessage
    {
        private readonly string value;
        private readonly int key;

        public static readonly AppMessage CloseApplicationWarning = new AppMessage(1, "Application will be shutdowned! Proceed?");
        public static readonly AppMessage SaveWarning = new AppMessage(2, "Are your sure?");
        public static readonly AppMessage CloseWarning = new AppMessage(3, "Any unsaved values will be lost! Proceed?");

        private AppMessage(int key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public override string ToString()
        {
            return value;
        }
    }
}
