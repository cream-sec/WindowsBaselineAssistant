using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace WHC.OrderWater.Commons
{
    /// <summary>
    /// 注册表操作辅助类
    /// </summary>
    public sealed class RegistryHelper
    {
        #region 基础操作（读取和保存）
        private static string Software_Key = @"Software\DeepLand\OrderWater";

        /// <summary>
        /// Gets the value by registry key. If the key does not exist, return empty string.
        /// </summary>
        /// <param name="key">registry key</param>
        /// <returns>Returns the value of the specified key.</returns>
        /// 
        private static string ArrayToString(string[] strings)
        {
            string result = string.Empty;
            foreach (var item in strings)
            {
                result = result + item + "\r\n";
            }
            return result;
        }

        private static (RegistryKey, string) FormatBaseKey(string fullSoftwareKey)
        {
            RegistryKey registryKey = Registry.LocalMachine;
            int indexOfFirstBackslash = fullSoftwareKey.IndexOf('\\');
            string baseKey = fullSoftwareKey.Substring(0, indexOfFirstBackslash);
            string subKey = fullSoftwareKey.Substring(indexOfFirstBackslash + 1);
            if (baseKey.Contains("USER"))
            {
                registryKey = Registry.CurrentUser;
            }
            if (baseKey.Contains("ROOT"))
            {
                registryKey = Registry.ClassesRoot;
            }
            if (baseKey.Contains("USERS"))
            {
                registryKey = Registry.Users;
            }
            if (baseKey.Contains("CONFIG"))
            {
                registryKey = Registry.CurrentConfig;
            }
            return (registryKey, subKey);
        }

        public static string GetValue(string softwareKey, string key)
        {
            (RegistryKey registryKey, string subKey) = FormatBaseKey(softwareKey);
            return GetValue(registryKey, subKey, key);
        }

        /// <summary>
        /// Gets the value by registry key. If the key does not exist, return empty string.
        /// </summary>
        /// <param name="key">registry key</param>
        /// <returns>Returns the value of the specified key.</returns>
        public static string GetValue(RegistryKey registryKey, string softwareKey, string key)
        {
            const string parameter = "key";
            if (null == key)
            {
                throw new ArgumentNullException(parameter);
            }
            string strRet = string.Empty;
            try
            {
                RegistryKey regKey = registryKey.OpenSubKey(softwareKey);
                object value = regKey.GetValue(key);
                if (value == null)
                {
                    return "未设置";
                }
                if (value is string[] stringArray)
                {
                    if (stringArray.Length.Equals(0))
                    {
                        return "";
                    }
                    strRet = ArrayToString((string[])value);
                    return strRet;
                }
                strRet = regKey.GetValue(key).ToString();
            }
            catch
            {
                return strRet;
            }
            return strRet;
        }
        /// <summary>
        /// Saves the key and the value to registry.
        /// </summary>
        /// <param name="key">registry key</param>
        /// <param name="value">the value of the key</param>
        /// <returns>Returns true if successful, otherwise return false.</returns>
        public static void SaveValue(string softwareKey, string key, string value)
        {
            (RegistryKey registryKey, string subKey) = FormatBaseKey(softwareKey);
            //return GetValue(registryKey, subKey, key);
            SaveValue(registryKey, subKey, key, value);
        }

        /// <summary>
        /// Saves the key and the value to registry.
        /// </summary>
        /// <param name="key">registry key</param>
        /// <param name="value">the value of the key</param>
        /// <returns>Returns true if successful, otherwise return false.</returns>
        public static void SaveValue(RegistryKey registryKey, string softwareKey, string key, string value)
        {
            try
            {
                const string parameter1 = "key";
                const string parameter2 = "value";
                if (null == key)
                {
                    throw new ArgumentNullException(parameter1);
                }
                if (null == value)
                {
                    throw new ArgumentNullException(parameter2);
                }
                RegistryKey reg;
                reg = registryKey.OpenSubKey(softwareKey, true);
                if (null == reg)
                {
                    reg = registryKey.CreateSubKey(softwareKey);
                }
                reg.SetValue(key, value);
                //return true;
            }
            catch (Exception)
            {
                throw;
                //return false;
            }
        }
        #endregion

        #region 自动启动程序设置

        public static bool CheckStartWithWindows()
        {
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            if (regkey != null && (string)regkey.GetValue(Application.ProductName, "null", RegistryValueOptions.None) != "null")
            {
                Registry.CurrentUser.Flush();
                return true;
            }

            Registry.CurrentUser.Flush();
            return false;
        }

        public static void SetStartWithWindows(bool startWin)
        {
            RegistryKey regkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (regkey != null)
            {
                if (startWin)
                {
                    regkey.SetValue(Application.ProductName, Application.ExecutablePath, RegistryValueKind.String);
                }
                else
                {
                    regkey.DeleteValue(Application.ProductName, false);
                }

                Registry.CurrentUser.Flush();
            }
        }

        #endregion
    }
}