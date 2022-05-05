// <copyright file="TestCasesFromXML.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace AutomationFramework
{
    using System.Collections.Generic;

    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("TestCases", Namespace = "", IsNullable = false)]
    /// <summary>
    /// class to execute multiple test cases from an xml file.
    /// </summary>
    public class TestCasesFromXML
    {
        private List<TestCase> TestCases;
        public TestCasesFromXML()
        {
            this.TestCases = new List<TestCase>();
        }

        [System.Xml.Serialization.XmlElementAttribute("TestCase", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]

        public List<TestCase> testCases
        {
            get
            {
                return this.TestCases;
            }
            set
            {
                this.TestCases = value;
            }
        }



    }
}
