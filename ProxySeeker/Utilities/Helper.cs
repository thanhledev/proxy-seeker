using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace ProxySeeker.Utilities
{
    public static class Helper
    {
        /// <summary>
        /// Find WindowControls inside another control
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        /// <summary>
        /// Parse string value to Struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Nullable<T> Parse<T>(string input) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Generic Type 'T' must be an Enum");
            }
            if (!string.IsNullOrEmpty(input))
            {
                if (Enum.GetNames(typeof(T)).Any(e => e == input))
                    return (T)Enum.Parse(typeof(T), input, true);
            }
            return null;
        }

        /// <summary>
        /// Get full domain name with protocol by a string
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetFullDomain(string url)
        {
            if (url.Contains("http://"))
                return "http://" + new Uri(url).Host;
            else
                return "https://" + new Uri(url).Host;
        }

        /// <summary>
        /// Convert string to UnSignString
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string convertToUnSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            temp = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');

            return Regex.Replace(temp, @"[^0-9a-zA-Z ]+", "").Trim().Replace(" ", "-");
        }

        /// <summary>
        /// Get TimeZoneInfo of a timeZoneId
        /// </summary>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public static TimeZoneInfo GetTimeZoneInfo(string timeZoneId)
        {
            foreach (TimeZoneInfo info in TimeZoneInfo.GetSystemTimeZones())
            {
                if (string.Compare(info.Id, timeZoneId, true) == 0)
                    return info;
            }
            return TimeZoneInfo.Utc;
        }
    }
}
