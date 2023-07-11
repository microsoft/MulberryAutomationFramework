// <copyright file="TestEngine.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace AutomationFramework
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Automation framework test engine.
    /// </summary>
    public class TestEngine
    {
        private XmlDocument dataStore;
        private string underscoreString = new string('_', 75);
        private string lineString = new string('-', 75);
        private string equalString = new string('=', 30);
        private string errorString = "###--->ERROR<---###";
        public Dictionary<string, dynamic> returnParameters { get; set; }

        public void ExecuteTestCasesFromXML(IConfiguration config, TestCaseModel testCaseModel, string strTestCasesXMLName, string strTestCasesXMLPath)
        {
            try
            {
                string strTestCaseXML = Path.GetFullPath(strTestCasesXMLPath + "\\" + strTestCasesXMLName);
                string strSummary = "";

                int iPassCount = 0, iFailCount = 0;

                Trace.WriteLine(string.Empty);
                Trace.WriteLine(underscoreString);
                Trace.WriteLine("SUMMARY"); Trace.WriteLine(equalString);
                Trace.WriteLine("Test Cases XML Location: - " + strTestCaseXML);

                Console.WriteLine(string.Empty);
                Console.WriteLine(underscoreString);
                Console.WriteLine("Test Cases XML Location: - " + strTestCaseXML);

                Trace.WriteLine(string.Empty);
                Console.WriteLine(string.Empty);

                TestCasesFromXML testCaseFromXML = null;
                XmlSerializer serializer = new XmlSerializer(typeof(TestCasesFromXML));

                var testXml = "";
                try
                {
                    testXml = XElement.Load(strTestCaseXML).ToString();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(errorString + " :- Unable to find < " + strTestCaseXML + " > File!");
                    Trace.WriteLine(ex.Message);

                    Console.WriteLine(errorString + " :- Unable to find < " + strTestCaseXML + " > File!");
                    Console.WriteLine(ex.Message);

                    return;
                }

                using (var sr = new StringReader(testXml))
                {
                    testCaseFromXML = (TestCasesFromXML)serializer.Deserialize(sr);
                }

                Trace.WriteLine("Executing Tests...");
                Trace.WriteLine(string.Empty);

                Console.WriteLine("Executing Tests...");
                Console.WriteLine(string.Empty);


                for (int iTestCaseCount = 0; iTestCaseCount < testCaseFromXML.testCases.Count; iTestCaseCount++)
                {
                    try
                    {
                        ExecuteTestCase(config, testCaseModel, testCaseFromXML.testCases[iTestCaseCount].TestCaseXMLName, strTestCasesXMLPath);

                        Trace.WriteLine(testCaseFromXML.testCases[iTestCaseCount].TestCaseName + "\t" + "PASS");

                        Console.WriteLine(testCaseFromXML.testCases[iTestCaseCount].TestCaseName + "\t" + "PASS");
                        Console.WriteLine(lineString);

                        strSummary = strSummary + (testCaseFromXML.testCases[iTestCaseCount].TestCaseName + "\t" + "PASS");

                        iPassCount++;
                    }
                    catch (Exception ex)
                    {

                        Trace.WriteLine(testCaseFromXML.testCases[iTestCaseCount].TestCaseName + "\t" + "FAIL");

                        Console.WriteLine(ex.Message);
                        Console.WriteLine(testCaseFromXML.testCases[iTestCaseCount].TestCaseName + "\t" + "FAIL");
                        Console.WriteLine(lineString);

                        strSummary = strSummary + (testCaseFromXML.testCases[iTestCaseCount].TestCaseName + "\t" + "FAIL");

                        iFailCount++;
                    }
                }

                Trace.WriteLine(string.Empty);
                Trace.WriteLine("PASS = " + iPassCount + "\t" + "FAIL = " + iFailCount);
                Trace.WriteLine(string.Empty);
                Trace.WriteLine("Completed Tests...");
                Trace.WriteLine(string.Empty);
                Trace.WriteLine("LOG: - " + Logger.LoggerFilePath() + "\\TestLOG.txt");
                Trace.WriteLine(underscoreString);
                Trace.WriteLine(string.Empty);

                Console.WriteLine(string.Empty);
                Console.WriteLine("PASS = " + iPassCount + "\t" + "FAIL = " + iFailCount);
                Console.WriteLine(string.Empty);
                Console.WriteLine("Completed Tests...");
                Console.WriteLine(string.Empty);
                Console.WriteLine("LOG: - " + Logger.LoggerFilePath() + "\\TestLOG.txt");
                Console.WriteLine(underscoreString);
                Console.WriteLine(string.Empty);

                return;

            }
            catch (Exception ex)
            {
                Trace.WriteLine(errorString + " : " + ex.Message);
                Console.WriteLine(errorString + " : " + ex.Message);
            }

        }

        public void ExecuteTestCase(IConfiguration config, TestCaseModel testCaseModel, string xmlName, string xmlPath)
        {
            string strTestCaseFilePath = "";
            TestCase testCase = null;

            try
            {

                if (Logger.DEBUGMODE) Console.WriteLine("Creating Logger File ...");
                Logger.CreateLogger();

                strTestCaseFilePath = xmlPath + "\\" + xmlName;
                testCase = CreateTestCaseTasks(strTestCaseFilePath);

                Logger.LOGMessage(Logger.MSG.DEBUGMODE, "Adding Assembly ...");
                AddAssembly();

                //Setting Config Defaults from Configuration file.
                Logger.LOGMessage(Logger.MSG.DEBUGMODE, "Extracting Config Values ...");
            
                Constants.SetConfigDefaults(config);

                Logger.LOGMessage(Logger.MSG.DEBUGMODE, "Executing Test Case ...");
                ExecuteTestCaseTasks(config, testCaseModel, testCase);


                Logger.CloseLOGFile();

            }
            catch (Exception ex)
            {
                Logger.CloseLOGFile();

                //This will Fail the Test Case.  *********DO NOT CHANGE********
                //The exception is used in ExecuteTestCasesFromXML() to FAIL the TC.                
                throw new Exception(ex.Message + "\n" + (ex.InnerException == null ? "" : ex.InnerException.Message));
            }
        }

        private void AddAssembly()
        {
            General.RefAssemblyLookupList = new List<Assembly>();

            foreach (Assembly b in AppDomain.CurrentDomain.GetAssemblies())
            {
                General.RefAssemblyLookupList.Add(b);
            }
        }

        private TestCase CreateTestCaseTasks(string testCaseFilePath)
        {
            TestCase testCase = null;
            try
            {
                Logger.LOGMessage(Logger.MSG.MESSAGE, "Creating Test Case Object...");
                XmlSerializer serializer = new XmlSerializer(typeof(TestCase));

                Logger.LOGMessage(Logger.MSG.MESSAGE, "Loading File testcase.xml file...");
                var testXml = XElement.Load(testCaseFilePath).ToString();

                Logger.LOGMessage(Logger.MSG.MESSAGE, "Reading File testcase.xml file...");
                using (var sr = new StringReader(testXml))
                {
                    testCase = (TestCase)serializer.Deserialize(sr);
                }

                Logger.LOGMessage(Logger.MSG.MESSAGE, "Successfully Created Testcase Object...");
                return testCase;
            }
            catch (Exception ex)
            {
                Logger.LOGMessage(Logger.MSG.EXCEPTION, ex.Message + "\t" + (ex.InnerException == null ? "" : ex.InnerException.Message));
                return testCase;
            }
        }

        private TestCases CreateTestCasesTasks(string testCaseFilePath)
        {
            TestCases testCases = null;
            try
            {
                Logger.LOGMessage(Logger.MSG.MESSAGE, "Creating Test Case(s) Object...");
                XmlSerializer serializer = new XmlSerializer(typeof(TestCases));

                Logger.LOGMessage(Logger.MSG.MESSAGE, "Loading File testcase.xml file...");
                var testXml = XElement.Load(testCaseFilePath).ToString();

                //Creating TestCases node

                using (var sr = new StringReader(testXml))
                {
                    testCases = (TestCases)serializer.Deserialize(sr);
                }

                Logger.LOGMessage(Logger.MSG.MESSAGE, "Successfully Created Testcase(s) Object...");
                return testCases;
            }
            catch (Exception ex)
            {
                Logger.LOGMessage(Logger.MSG.EXCEPTION, ex.Message + "\t" + (ex.InnerException == null ? "" : ex.InnerException.Message));
                return testCases;
            }
        }

        private void ExecuteTestCaseTasks(IConfiguration config, TestCaseModel testCaseModel, TestCase testCase)
        {
            //Do not put try/catch here.  It will be handled in Calling function.

            string className = string.Empty;
            string functionName = string.Empty;
            List<Param> param = null;
            string ReturnValue = string.Empty;
            object[] localArguments = null;

            Logger.LOGMessage(Logger.MSG.DEBUGMODE, "Extracting Test Case Details ...");
            //Extracting the Test Case ID from xml
            if (!string.IsNullOrEmpty(testCase.TestCaseID))
                Logger.LOGMessage(Logger.MSG.MESSAGE, "TEST CASE ID # " + testCase.TestCaseID);

            //TestCase specific LOG SQL Query.  Do not Remove.
            if (!string.IsNullOrEmpty(testCase.LOGGER_CAPTURE_SQL_QUERY))
                Boolean.TryParse(testCase.LOGGER_CAPTURE_SQL_QUERY, out Constants.LOGGER_CAPTURE_SQL_QUERY);

            Logger.LOGMessage(Logger.MSG.DEBUGMODE, "Executing the Steps of Test Case ...");
            for (int steps = 0; steps < testCase.steps.Count; steps++)
            {
                //Creating the Parameters object
                returnParameters = new Dictionary<string, dynamic>();

                for (int stepNumber = 0; stepNumber < testCase.steps[steps].step.Count; stepNumber++)
                {
                    Logger.LOGMessage(Logger.MSG.MESSAGE, "");
                    Logger.LOGMessage(Logger.MSG.MESSAGE, "Executing Step # " + (stepNumber + 1));

                    className = testCase.steps[steps].step[stepNumber].ClassName;
                    functionName = testCase.steps[steps].step[stepNumber].FunctionName;
                    param = testCase.steps[steps].step[stepNumber].param;

                    //Getting the Return Value Variable to store data
                    ReturnValue = testCase.steps[steps].step[stepNumber].ReturnValue;

                    if (className.ToUpper().Trim() == "DBCONNECTOR")
                    {
                        Constants.DATABASE = (Constants.GetConfigValue(config, Constants.DATABASE_ConfigValue, false) == null ? Constants.DATABASE : Constants.GetConfigValue(config, Constants.DATABASE_ConfigValue, false));

                        switch (Constants.DATABASE.ToUpper().Trim())
                        {
                            case "COSMOS":
                                className = "CosmosDB";
                                break;
                            case "SQL":
                                className = "SQLDB";
                                break;
                            case "TABLE_STORAGE":
                                className = "TableStorage";
                                break;
                            default:
                                className = "CosmosDB";
                                break;
                        }
                    }

                    MethodInfo methodInfo = GetMethodInfo(className, functionName);
                    localArguments = FormatParameters(config,testCaseModel, param, methodInfo);

                    Logger.LOGMessage(Logger.MSG.MESSAGE, "Executing ... Class = " + className + "  Function = " + functionName);

                    if (string.IsNullOrEmpty(ReturnValue))
                    {

                        ExecuteStep(testCaseModel, className, functionName, localArguments);
                    }
                    else
                    {
                        object objReturn;
                        objReturn = ExecuteStepWithReturn(testCaseModel, className, functionName, localArguments);

                        returnParameters.AddParameters(ReturnValue, objReturn);
                    }

                    Logger.LOGMessage(Logger.MSG.STEP_PASS, "");
                }

                //If it reaches here, the Test Case is Pass.  Any Error should be "thrown" as exception before this Line.
                Logger.LOGMessage(Logger.MSG.TESTCASE_PASS, "");
            }
        }

        public MethodInfo GetMethodInfo(string className, string functionName)
        {
            try
            {
                Type type = General.GetClassType(className);

                if (type == null)
                {
                    throw new Exception("Unable to find Class = " + className);
                }

                object instance = Activator.CreateInstance(type);

                MethodInfo methodInfo = type.GetMethod(functionName);

                if (methodInfo == null)
                {
                    throw new Exception("Unable to find Function=" + functionName + " in Class=" + className);
                }

                return methodInfo;
            }
            catch (Exception ex)
            {
                Logger.LOGMessage(Logger.MSG.EXCEPTION, ex.Message);
                return null;
            }
        }

        private object[] FormatParameters(IConfiguration config, TestCaseModel testCaseModel, List<Param> localParameters, MethodInfo callMethod)
        {
            //Do not Change. This may effect all the Test Case execution tasks.  
            try
            {
                ParameterInfo[] methodsParamInfo = callMethod.GetParameters();
                object[] LocalArguments = new object[localParameters.Count];

                int i = 0;
                foreach (Param myParam in localParameters)
                {
                    LocalArguments[i] = Convert.ChangeType(FormatParamValue(config, testCaseModel, myParam.Value), methodsParamInfo[i].ParameterType);
                    i = i + 1;
                }

                return LocalArguments;
            }

            catch (InvalidCastException exInvalidCast)
            {
                LOGMethodDetails(callMethod);
                Logger.LOGMessage(Logger.MSG.EXCEPTION, exInvalidCast.Message);
                return null;
            }
            catch (IndexOutOfRangeException exIndexOutOfRange)
            {
                LOGMethodDetails(callMethod);
                Logger.LOGMessage(Logger.MSG.EXCEPTION, exIndexOutOfRange.Message);
                return null;
            }
            catch (Exception ex)
            {
                Logger.LOGMessage(Logger.MSG.EXCEPTION, ex.Message);
                return null;
            }
        }


        public void LOGMethodDetails(MethodInfo callMethod)
        {
            try
            {
                Logger.LOGMessage(Logger.MSG.MESSAGE, "Calling Method Name : " + callMethod.Name);
                ParameterInfo[] methodsParamInfo = callMethod.GetParameters();

                string strParameterExpected = "";

                Logger.LOGMessage(Logger.MSG.MESSAGE, "Total Number of Parameters Expected : < " + callMethod.GetParameters().Length + " >");

                strParameterExpected = "Parameters Name/Type Expected : (";
                foreach (ParameterInfo myParam in methodsParamInfo)
                {
                    strParameterExpected = strParameterExpected + myParam.Name + "<" + myParam.ParameterType + ">" + ", ";
                }
                strParameterExpected = strParameterExpected.Substring(0, strParameterExpected.Length - 2);
                strParameterExpected = strParameterExpected + ")";
                Logger.LOGMessage(Logger.MSG.MESSAGE, strParameterExpected);
            }
            catch (Exception ex)
            {
                Logger.LOGMessage(Logger.MSG.EXCEPTION, ex.Message);
            }
        }

        private object FormatParamValue(IConfiguration config, TestCaseModel testCaseModel, string paramValue)
        {
            try
            {
                dataStore = new XmlDocument();

                paramValue = paramValue.Trim();

                if (paramValue.StartsWith(Constants.DATASTORE_String))
                {
                    paramValue = paramValue.Substring(Constants.DATASTORE_String.Length);//removing DATASTORE:
                    paramValue = paramValue.Substring(1, paramValue.Length - 2);//removing [ & ]

                    string[] strParams = paramValue.Split(',');

                    string strEntityName = strParams[0].Trim();
                    string strValue = strParams[1].Trim();

                    var testXml = XElement.Load(Constants.GetConfigValue(config, Constants.DATASTORE_Path_ConfigValue)).ToString();

                    dataStore.LoadXml(testXml);

                    XmlNode node = dataStore.SelectSingleNode(string.Format("/DataStore/Entities/Entity[@Name='{0}']/{1}", strEntityName, strValue));
                    //Checking if the Node exists in DataStore
                    if (node == null)
                    {
                        throw new Exception("XML Node < " + string.Format("/DataStore/Entities/Entity[@Name='{0}']/{1}", strEntityName, strValue) + " > Doesn't Exists...!");
                    }
                    else
                    {
                        Logger.LOGMessage(Logger.MSG.MESSAGE, "Extracting Datastore Entity <" + node.Name + ">");
                        paramValue = node.InnerText;
                    }

                    return paramValue;
                }
                else if (paramValue.StartsWith("REF:"))
                {
                    paramValue = paramValue.Substring(4);//removing REF:
                    paramValue = paramValue.Substring(1, paramValue.Length - 2);//removing [ & ]

                    if (returnParameters.ContainsKey(paramValue))
                    {
                        return returnParameters[paramValue];
                    }
                    else
                    {
                        throw new Exception("The REF Parameter < " + paramValue + " > Doesn't Exist !");
                    }
                }
                else if (paramValue.StartsWith("CONFIG:"))
                {
                    paramValue = paramValue.Substring(7);//removing CONFIG:
                    paramValue = paramValue.Substring(1, paramValue.Length - 2);//removing [ & ]

                    if (!string.IsNullOrWhiteSpace(config[paramValue]))
                        return config[paramValue];
                    else
                        throw new Exception("The CONFIG value < " + paramValue + " > doesn't Exist in Configuration file!");

                }
                else if (paramValue.StartsWith("TESTCASEMODEL:"))
                {
                    paramValue = paramValue.Substring(14);//removing TESTCASEMODEL:
                    paramValue = paramValue.Substring(1, paramValue.Length - 2);//removing [ & ]

                    object modelParamValue = testCaseModel.GetType().GetProperty(paramValue).GetValue(testCaseModel);
                    if (modelParamValue != null)
                    {
                        return modelParamValue;
                    }
                    else
                    {
                        throw new Exception("The attribute value < " + paramValue + " > doesn't Exist in TestCaseModel!");
                    }
                }
                else if (paramValue.StartsWith("DATEFORMAT:"))
                {
                    //SYNTAX: DATEFORMAT:[TODAY,MM/dd/yyyy]
                    //SYNTAX: DATEFORMAT:[ADDDAYS(-1),MM/dd/yyyy]

                    paramValue = paramValue.Substring(11);//removing DATEFORMAT:
                    paramValue = paramValue.Substring(1, paramValue.Length - 2);//removing [ & ]
                    int iExtractNumber = 0;
                    int iTempLength = 0;

                    string[] strDateFormat = paramValue.Split(',');
                    if (strDateFormat.Length == 2)
                    {
                        strDateFormat[0] = strDateFormat[0].Trim();

                        if (strDateFormat[0].ToUpper() == "TODAY")
                        {
                            paramValue = DateTime.Today.ToString(strDateFormat[1].Trim());
                        }
                        else
                        {
                            if (strDateFormat[0].ToUpper().StartsWith("ADDDAYS"))
                            {
                                iTempLength = 8;
                                strDateFormat[0] = strDateFormat[0].Substring(iTempLength, strDateFormat[0].Length - (iTempLength + 1));
                                int.TryParse(strDateFormat[0], out iExtractNumber);
                                paramValue = DateTime.Parse(DateTime.Today.AddDays(iExtractNumber).ToString()).ToString(strDateFormat[1].Trim());
                            }
                            else if (strDateFormat[0].ToUpper().StartsWith("ADDMONTHS"))
                            {
                                iTempLength = 10;
                                strDateFormat[0] = strDateFormat[0].Substring(iTempLength, strDateFormat[0].Length - (iTempLength + 1));
                                int.TryParse(strDateFormat[0], out iExtractNumber);
                                paramValue = DateTime.Parse(DateTime.Today.AddMonths(iExtractNumber).ToString()).ToString(strDateFormat[1].Trim());
                            }
                            else if (strDateFormat[0].ToUpper().StartsWith("ADDYEARS"))
                            {
                                iTempLength = 9;
                                strDateFormat[0] = strDateFormat[0].Substring(iTempLength, strDateFormat[0].Length - (iTempLength + 1));
                                int.TryParse(strDateFormat[0], out iExtractNumber);
                                paramValue = DateTime.Parse(DateTime.Today.AddYears(iExtractNumber).ToString()).ToString(strDateFormat[1].Trim());
                            }
                            else
                                paramValue = DateTime.Parse(strDateFormat[0]).ToString(strDateFormat[1].Trim());
                        }

                    }
                    else
                    {
                        throw new Exception("The DATEFORMAT Parameter < " + paramValue + " > is not valid !" + "\t" + "SYNTAX: DATEFORMAT:[TODAY,MM/dd/yyyy]" + "\t" + "SYNTAX: DATEFORMAT:[ADDDAYS(-1),MM/dd/yyyy]");
                    }
                }

                return paramValue;
            }
            catch (Exception ex)
            {
                Logger.LOGMessage(Logger.MSG.EXCEPTION, ex.Message);
                return paramValue;
            }
        }

        public void ExecuteStep(TestCaseModel testCaseModel, string className, string functionName, object[] objValue)
        {
            try
            {
                Type type = General.GetClassType(className);
                object instance = Activator.CreateInstance(type);

                MethodInfo methodInfo = type.GetMethod(functionName);

                //This is for Optional Parameter.
                //TODO:- Check for more optimized approach.
                if (objValue.Length == 0)
                {
                    methodInfo.Invoke(instance, new object[] { Type.Missing });
                    return;
                }

                if (methodInfo.GetParameters().Count() != objValue.Count())
                {
                    string parameters = "";
                    foreach (ParameterInfo paramInfo in methodInfo.GetParameters())
                    {
                        if (paramInfo.ParameterType.Name == nameof(TestCaseModel))
                        {
                            Array.Resize<object>(ref objValue, objValue.Length + 1);
                            objValue[objValue.Length - 1] = Convert.ChangeType(testCaseModel, typeof(TestCaseModel));
                        }
                        else
                        {
                            parameters = parameters + "\t" + paramInfo.Name + "(" + paramInfo.ParameterType + ")";
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(parameters))
                    {
                        Logger.LOGMessage(Logger.MSG.EXCEPTION, "PARAMETER MISMATCH !!!! " + className.ToUpper() + "." + functionName.ToUpper() + "[" + parameters + "]");
                    }
                }

                methodInfo.Invoke(instance, objValue);
            }
            catch (Exception ex)
            {
                //Exception Comes from Method.  Method logs into Logger.
                throw new Exception(ex.Message + "\t" + (ex.InnerException == null ? "" : ex.InnerException.Message));
            }
        }

        //If Return value is expected from the Executed method.
        public object ExecuteStepWithReturn(TestCaseModel testCaseModel, string className, string functionName, object[] objValue)
        {
            try
            {
                Type type = General.GetClassType(className);

                object instance = Activator.CreateInstance(type);

                MethodInfo methodInfo = type.GetMethod(functionName);
                
                if (methodInfo.GetParameters().Length != objValue.Length)
                {
                    var defaultParams = methodInfo.GetParameters().Skip(objValue.Length)
                        .Select(i => i.HasDefaultValue ? i.DefaultValue : null);
                    objValue = objValue.Concat(defaultParams).ToArray();
                }

                if (methodInfo.GetParameters().Count() != objValue.Count())
                {
                    string strParameters = "";
                    foreach (ParameterInfo paramInfo in methodInfo.GetParameters())
                    {
                        if (paramInfo.ParameterType.Name == nameof(TestCaseModel))
                        {
                            Array.Resize<object>(ref objValue, objValue.Length + 1);
                            objValue[objValue.Length - 1] = Convert.ChangeType(testCaseModel, typeof(TestCaseModel));
                        }
                        else
                        {
                            strParameters = strParameters + "\t" + paramInfo.Name + "(" + paramInfo.ParameterType + ")";
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(strParameters))
                    {
                        Logger.LOGMessage(Logger.MSG.EXCEPTION, "PARAMETER MISMATCH !!!! " + className.ToUpper() + "." + functionName.ToUpper() + "[" + strParameters + "]");
                    }
                }

                return methodInfo.Invoke(instance, objValue);
            }
            catch (Exception ex)
            {
                //Exception Comes from Method.  Method logs into Logger.
                throw new Exception(ex.Message + "\t" + (ex.InnerException == null ? "" : ex.InnerException.Message));
            }

        }
    }
}
