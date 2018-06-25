using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Data;

namespace OpenTextSampleApp
{
    class Helper
    {
        /// <summary>
        /// Logic to read file. Program will read earliest XML file if more than one is available
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        internal static string GetProcessFile(string folderPath)
        {
            var directory = new DirectoryInfo(folderPath);
            var myFileToProcess = (from f in directory.GetFiles("*.xml")
                                   orderby f.LastWriteTime ascending
                                   select f).Take(1);
            if (myFileToProcess.Count() > 0)
                return Convert.ToString(myFileToProcess.ElementAt(0).FullName);
            else
                return string.Empty;
        }

        /// <summary>
        /// Logic to validate XML against XSD
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <param name="xsdFile"></param>
        /// <returns></returns>
        internal static string ValidateDocument(string xmlFile, string xsdFile)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema
            };
            settings.Schemas.Add(XmlSchema.Read(XmlReader.Create(xsdFile), null));
            using (XmlReader reader = XmlReader.Create(xmlFile, settings))
            {
                try
                {
                    while (reader.Read()) ;
                    return string.Empty;
                }
                catch (XmlSchemaValidationException xes)
                {
                    return "Error in XSD Validation : " + "\r\n" + " Message : " + xes.Message + "\r\n" + "Source : " + xes.SourceUri + "\r\n" + "Line Number : " + xes.LineNumber + "\r\n" + "Line Position : " + xes.LinePosition;
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Logic to convert XML data to DataSet
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        internal static DataSet ImportXmls(string filePath)
        {
            using (DataSet impDS = new DataSet())
            {
                try
                {
                    impDS.ReadXml(filePath);
                }
                catch (Exception ex)
                {
                    //Log error in converting XML to DS    
                    throw ex;
                }
                return impDS;
            }
        }
    }
}
