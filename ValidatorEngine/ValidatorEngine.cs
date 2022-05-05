// <copyright file="ValidatorEngine.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;

    /// <summary>
    /// Validation class containing all validations common to any test case.
    /// </summary>
    public class ValidatorEngine
    {
        public static string ValidateApiStatus(HttpResponseMessage result, int expectedApiStatusCode)
        {
            if ((int)result.StatusCode != expectedApiStatusCode)
            {
                Logger.LOGMessage(Logger.MSG.EXCEPTION, result.ToString());
                throw new Exception(result.Content.ReadAsStringAsync().Result);
            }
            else
            {
                var resultString = result.Content.ReadAsStringAsync().Result.ToString();
                Logger.LOGMessage(Logger.MSG.MESSAGE, resultString);
                Console.WriteLine(resultString);
                return resultString;
            }
        }

        public bool ValidateString(string actualValue, string expectedValue, string stringOperator)
        {
            try
            {
                actualValue = actualValue.Trim();
                expectedValue = expectedValue.Trim();

                switch (stringOperator.Trim().ToUpper())
                {
                    case "EQUALTO":
                    case "EQUALS":
                    case "EQUAL":
                        Logger.LOGMessage(Logger.MSG.MESSAGE, "Validating... <" + actualValue + "> EQUALS <<" + expectedValue + ">>  ?");
                        if (actualValue.Equals(expectedValue))
                            return true;
                        break;

                    case "STARTSWITH":
                    case "STARTS WITH":
                        Logger.LOGMessage(Logger.MSG.MESSAGE, "Validating... <" + actualValue + "> STARTS WITH <<" + expectedValue + ">>  ?");
                        if (actualValue.StartsWith(expectedValue))
                            return true;
                        break;

                    case "ENDSWITH":
                    case "ENDS WITH":
                        Logger.LOGMessage(Logger.MSG.MESSAGE, "Validating... <" + actualValue + "> ENDS WITH <<" + expectedValue + ">>  ?");
                        if (actualValue.EndsWith(expectedValue))
                            return true;
                        break;

                    default: //"CONTAINS":
                        Logger.LOGMessage(Logger.MSG.MESSAGE, "Validating... <" + actualValue + "> CONTAINS <<" + expectedValue + ">>  ?");
                        if (actualValue.Contains(expectedValue))
                            return true;

                        break;
                }

                Logger.LOGMessage(Logger.MSG.STEP_FAIL, "String Values DOES'NT Match !");
                return false;
            }
            catch (Exception ex)
            {
                Logger.LOGMessage(Logger.MSG.EXCEPTION, this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " ! < " + ex.Message + " >");
                return false;
            }
        }

        public bool ValidateCosmosDbRecordCount(List<dynamic> queryResult, int expectedValue)
        {
            try
            {
                Logger.LOGMessage(Logger.MSG.MESSAGE, "Validating... < CosmosDb Query Result Count = " + queryResult.Count() + ">, Expected Value <<" + expectedValue + ">>");
                if (queryResult.Count().Equals(expectedValue))
                {
                    return true;
                }
                else
                {
                    Logger.LOGMessage(Logger.MSG.STEP_FAIL, "Expected count DOES'NT Match !"); 
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.LOGMessage(Logger.MSG.EXCEPTION, this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + " ! < " + ex.Message + " >");
                return false;
            }
        }
    }
}
