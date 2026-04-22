using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace PayTimeWebClient.Helper
{
    public class ClientDBNameCheck
    {

        #region for check Db In web.config File
        public static bool IsDbNameMatch(string dbName)
        {
            // Retrieve the values from the configuration
            if (ConfigurationManager.AppSettings.AllKeys.Contains("CheckCustomClientDBName"))
            {
                string rawValue = ConfigurationManager.AppSettings["CheckCustomClientDBName"];
                if (!string.IsNullOrEmpty(rawValue))
                {
                    string[] CliendDBids = rawValue.Split(',').Select(s => s.Trim()).ToArray();
                    return CliendDBids.Contains(dbName);
                }
            }
            return false; // Return false if the key doesn't exist or has an empty value
        }
        #endregion
    }
}