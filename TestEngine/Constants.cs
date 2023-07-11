// <copyright file="Constants.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace AutomationFramework
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Configuration;


    /// <summary>
    /// class containing all constants being used in the framework.
    /// </summary>
    public static class Constants
    {

        public const string DATASTORE_Path_ConfigValue = "DATASTORE_Path";
        public const string DATASTORE_String = "DATASTORE:";
        public const string CSV_DATAFILES_PATH = "CSV_DATAFILES_PATH";
        public const string LOGGER_CAPTURE_SQL_QUERY_ConfigValue = "LOGGER_CAPTURE_SQL_QUERY";
        public const string DB_NETWORK_ERROR_RETRY_ConfigValue = "DB_NETWORK_ERROR_RETRY";
        public const string EXTERNAL_DLLs_ConfigValue = "EXTERNAL_DLLs";
        public const string DATABASE_ConfigValue = "DATABASE";
        public static bool LOGGER_CAPTURE_SQL_QUERY = false;
        public static int DB_NETWORK_ERROR_RETRY = 3;
        public static string DATABASE = "SQL";
         public const string CONTENT_TYPE = "application/json";

        public static string GetConfigValue(IConfiguration config, string configKey, Boolean bRaiseError = true)
        {

            try
            {
                string configValue = config[configKey];
                if (string.IsNullOrEmpty(configValue))
                    if (bRaiseError)
                        Logger.LOGMessage(Logger.MSG.EXCEPTION, "Unable to Get Config Value '" + configKey + "' !");
                    else
                        return "";

                return configValue;
            }
            catch (Exception ex)
            {
                Logger.LOGMessage(Logger.MSG.EXCEPTION, "Unable to Get Config Value '" + configKey + "' !");
                return "";
            }
        }

        public static void SetConfigDefault_int(IConfiguration config, string configKey, ref int iDefaultVar)
        {

            try
            {
                int configValue = 0;
                int.TryParse(config[configKey], out configValue);
                if (configValue != 0)
                {
                    iDefaultVar = configValue;
                }
            }
            catch (Exception ex)
            {
                Logger.LOGMessage(Logger.MSG.EXCEPTION, ex.Message);
            }
        }

        public static void SetConfigDefaults(IConfiguration config)
        {
            try
            {
                Boolean.TryParse(config[Constants.LOGGER_CAPTURE_SQL_QUERY_ConfigValue], out Constants.LOGGER_CAPTURE_SQL_QUERY);
                Constants.SetConfigDefault_int(config, Constants.DB_NETWORK_ERROR_RETRY_ConfigValue, ref Constants.DB_NETWORK_ERROR_RETRY);
            }
            catch (Exception ex)
            {
                Logger.LOGMessage(Logger.MSG.EXCEPTION, "Unable to Set config Defaults... " + ex.Message);
            }
        }

    }
}
