// <copyright file="TestCase.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace AutomationFramework
{
    using System.Collections.Generic;

    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("testCase", Namespace = "", IsNullable = false)]
    /// <summary>
    /// class containing test case xml object node.
    /// </summary>
    public class TestCase
    {
        public TestCase()
        {
            this.listSteps = new List<Steps>();
        }
        private List<Steps> listSteps;

        [System.Xml.Serialization.XmlElementAttribute("steps", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]

        public List<Steps> steps
        {
            get
            {
                return this.listSteps;
            }
            set
            {
                this.listSteps = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute("TestCaseID")]
        public string TestCaseID { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute("LOGGER_CAPTURE_SQL_QUERY")]
        public string LOGGER_CAPTURE_SQL_QUERY { get; set; }

        ////////////
        [System.Xml.Serialization.XmlAttributeAttribute("TestCaseName")]
        public string TestCaseName { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute("TestCaseXMLName")]
        public string TestCaseXMLName { get; set; }
        ///////////

    }

    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class Step
    {
        public Step()
        {
            this.paramField = new List<Param>();
        }
        private List<Param> paramField;

        [System.Xml.Serialization.XmlAttributeAttribute("ClassName")]
        public string ClassName { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute("FunctionName")]
        public string FunctionName { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute("ControlName")]
        public string ControlName { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute("defaultaction")]
        public string DefaultAction { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute("ConnectionString")]
        public string ConnectionString { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute("paramValue")]
        public string paramValue { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("param")]
        public List<Param> param
        {
            get
            {
                return this.paramField;
            }
            set
            {
                this.paramField = value;
            }
        }


        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ReturnValue { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ActualValue { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ExpectedValue { get; set; }



    }


    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class Steps
    {
        public Steps()
        {
            this.stepField = new List<Step>();
        }

        private List<Step> stepField;


        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("step")]
        public List<Step> step
        {
            get
            {
                return this.stepField;
            }
            set
            {
                this.stepField = value;
            }
        }
    }


    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("testCases", Namespace = "", IsNullable = false)]
    public class TestCases
    {
        public TestCases()
        {
            this.testcase = new List<TestCase>();
        }

        private List<TestCase> testcase;


        [System.Xml.Serialization.XmlElementAttribute("testCase", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public List<TestCase> testCase
        {
            get
            {
                return this.testcase;
            }
            set
            {
                this.testcase = value;
            }
        }
    }


    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("param", Namespace = "", IsNullable = false)]
    public class Param
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Value { get; set; }
    }
}
