// <copyright file="Logger.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace AutomationFramework
{
    using System;
    using System.Text;
    using System.IO;
    using System.Configuration;
    using System.Reflection;

    /// <summary>
    /// Framework logger class to log execution flow.
    /// </summary>
    public static class Logger
    {

        private static StreamWriter swLogFile;
        public static StringBuilder sbResults;
        public enum MSG { ERROR, MESSAGE, EXCEPTION, STEP_PASS, STEP_FAIL, TESTCASE_PASS, TESTCASE_FAIL, DEBUGMODE };
        public const string DEBUGMODE_ConfigValue = "DEBUGMODE";
        public static string strLogFile = "";
        public static bool DEBUGMODE = true;
        public static void CreateLogger()
        {
            string strLogFilePath = LoggerFilePath();
            if (string.IsNullOrEmpty(strLogFile))
                strLogFile = "TestLOG-" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Millisecond + ".txt";

            if (!Directory.Exists(strLogFilePath))
                Directory.CreateDirectory(strLogFilePath);

            strLogFilePath = strLogFilePath + "\\" + strLogFile; // "\\TestLOG.txt";

            swLogFile = new StreamWriter(strLogFilePath, true);

            sbResults = new StringBuilder();

            //Boolean.TryParse(config[Logger.DEBUGMODE_ConfigValue], out Logger.DEBUGMODE);
        }

        public static string LoggerFilePath()
        {
            string strLogFilePath = Assembly.GetExecutingAssembly().Location;

            if (DEBUGMODE) Console.WriteLine("Log File Path is < " + strLogFilePath + " > ...");

            DirectoryInfo dirInfo = new DirectoryInfo(strLogFilePath);

            if (dirInfo.Parent.Name.ToUpper() != "OUT")
            {
                strLogFilePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\TestLOG";
            }
            else
            {
                strLogFilePath = dirInfo.Parent.Parent.Parent.Parent.FullName + "\\TestLOG";
            }

            //string strLogFilePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\TestLOG";

            return strLogFilePath;
        }

        public static void LOGMessage(MSG msgType, string strMessage)
        {
            switch (msgType)
            {
                case MSG.ERROR:
                    swLogFile.WriteLine(DateTime.Now + "\t" + "ERROR#######:- " + "\t" + strMessage);
                    swLogFile.WriteLine("");

                    sbResults.AppendLine(DateTime.Now + "\t" + "ERROR#######:- " + "\t" + strMessage);
                    sbResults.AppendLine("");

                    Console.WriteLine(DateTime.Now + "\t" + "ERROR#######:- " + "\t" + strMessage);
                    Console.WriteLine("");

                    break;
                case MSG.MESSAGE:
                    swLogFile.WriteLine(DateTime.Now + "\t" + "MESSAGE:- " + "\t" + strMessage);
                    sbResults.AppendLine(DateTime.Now + "\t" + "MESSAGE:- " + "\t" + strMessage);
                    Console.WriteLine(DateTime.Now + "\t" + "MESSAGE:- " + "\t" + strMessage);
                    break;
                case MSG.TESTCASE_PASS:
                    swLogFile.WriteLine(DateTime.Now + "\t" + "TEST CASE - PASS******" + "\t" + strMessage);
                    swLogFile.WriteLine("");
                    swLogFile.WriteLine("============================================================================");

                    sbResults.AppendLine(DateTime.Now + "\t" + "TEST CASE - PASS******" + "\t" + strMessage);
                    sbResults.AppendLine("");

                    Console.WriteLine(DateTime.Now + "\t" + "TEST CASE - PASS******" + "\t" + strMessage);
                    Console.WriteLine("");
                    Console.WriteLine("============================================================================");
                    break;
                case MSG.TESTCASE_FAIL:
                    swLogFile.WriteLine(DateTime.Now + "\t" + "TEST CASE - FAIL#######:- " + "\t" + strMessage);
                    swLogFile.WriteLine("");
                    swLogFile.WriteLine("============================================================================");

                    sbResults.AppendLine(DateTime.Now + "\t" + "TEST CASE - FAIL#######:- " + "\t" + strMessage);
                    sbResults.AppendLine("");

                    Console.WriteLine(DateTime.Now + "\t" + "TEST CASE - FAIL#######:- " + "\t" + strMessage);
                    Console.WriteLine("");
                    swLogFile.WriteLine("============================================================================");

                    throw new Exception("TEST CASE - FAIL#######:- " + "\t" + strMessage);
                case MSG.EXCEPTION:

                    ExplainExceptionMessage(strMessage);

                    swLogFile.WriteLine(DateTime.Now + "\t" + "ERROR-EXCEPTION#######:- " + "\t" + strMessage);
                    swLogFile.WriteLine("");
                    swLogFile.WriteLine("****************************************************************************");

                    sbResults.AppendLine(DateTime.Now + "\t" + "ERROR-EXCEPTION#######:- " + "\t" + strMessage);
                    sbResults.AppendLine("");

                    Console.WriteLine(DateTime.Now + "\t" + "ERROR-EXCEPTION#######:- " + "\t" + strMessage);
                    Console.WriteLine("");
                    Console.WriteLine("****************************************************************************");

                    throw new Exception("ERROR-EXCEPTION#######:- " + "\t" + strMessage);
                case MSG.STEP_PASS:
                    swLogFile.WriteLine(DateTime.Now + "\t" + "STEP - PASS******:- " + "\t" + strMessage);
                    swLogFile.WriteLine("");

                    sbResults.AppendLine(DateTime.Now + "\t" + "STEP - PASS******:- " + "\t" + strMessage);
                    sbResults.AppendLine("");

                    Console.WriteLine(DateTime.Now + "\t" + "STEP - PASS******:- " + "\t" + strMessage);
                    Console.WriteLine("");
                    break;
                case MSG.STEP_FAIL:
                    swLogFile.WriteLine(DateTime.Now + "\t" + "STEP - FAIL#######:- " + "\t" + strMessage);
                    Console.WriteLine(DateTime.Now + "\t" + "STEP - FAIL#######:- " + "\t" + strMessage);
                    sbResults.AppendLine(DateTime.Now + "\t" + "STEP - FAIL#######:- " + "\t" + strMessage);

                    goto case MSG.TESTCASE_FAIL;
                case MSG.DEBUGMODE:
                    if (DEBUGMODE)
                    {
                        swLogFile.WriteLine(DateTime.Now + "\t" + "MESSAGE:- " + "\t" + strMessage);
                        Console.WriteLine(DateTime.Now + "\t" + "MESSAGE:- " + "\t" + strMessage);
                    }
                    break;

                default:
                    swLogFile.WriteLine(DateTime.Now + "\t" + strMessage);
                    Console.WriteLine(DateTime.Now + "\t" + strMessage);
                    break;
            }
        }

        public static void ExplainExceptionMessage(string strMessage)
        {
            if (strMessage.ToUpper().Contains("INPUT STRING WAS NOT IN A CORRECT FORMAT."))
            {
                swLogFile.WriteLine(DateTime.Now + "\t" + "ERROR-EXCEPTION#######:- " + "\t" + "Check whether correct Method is called with right Type/Number of parameters...");
                Console.WriteLine(DateTime.Now + "\t" + "ERROR-EXCEPTION#######:- " + "\t" + "Check whether correct Method is called with right Type/Number of parameters...");
            }
        }
        public static void CloseLOGFile()
        {
            swLogFile.Close();
        }

    }
}
