﻿namespace Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CsvQuery.Csv;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Testing class CsvSettings
    /// </summary>
    [TestClass]
    public class CsvSettingsFacts:TestBaseClass
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }
        

        [TestMethod]
        public void CanParse()
        {
            var csvText = "\"header1\",\"header2\"\n"
                + "\"text\",\"more text\"\n"
                + "\"here, might fail\",\":/\"";
            var set = new CsvSettings {Separator = ',', TextQualifier = '"'};

            // Act
            var data = set.Parse(csvText);

            // Assert
            Assert.AreEqual(3, data.Count);
            Assert.AreEqual(2, data[2].Length);
        }


        [TestMethod] public void CanParseCrlf() => this.CanParseDifferent("\r\n",',',true);
        [TestMethod] public void CanParseLf() => this.CanParseDifferent("\n", ',', true);
        [TestMethod] public void CanParseUnquoted() => this.CanParseDifferent("\n", ';', false);

        public void CanParseDifferent(string newline, char separator, bool quoted)
        {
            var indata = new List<string[]>
            {
                new[] {"A number", "another number here", "#¤%i"},
                new[] {"1", "2g\"m", "3"},
                new[] {"3", "12", "1,3\""},
                new[] {"4", "2", "3"}
            };
            var csvText = string.Join(newline, indata.Select(x => string.Join(separator.ToString(), x.Select(l => quoted ? $"\"{l.Replace("\"", "\"\"")}\"" : l))));
            var set = new CsvSettings {Separator = separator, TextQualifier = quoted ? '"' : default(char), HasHeader = false};

            // Act
            var data = set.Parse(csvText);

            // Assert
            this.AssertDataEqual(indata, data);
        }

        [TestMethod]
        public void CanParseW3c()
        {
            var csvData = File.ReadAllText(@"TestFiles\w3c.log");
            var settings = CsvAnalyzer.Analyze(csvData);

            var data = settings.Parse(csvData);

            Assert.AreEqual(6, data.Count, "Number of rows incorrect");
        }
    }
}
