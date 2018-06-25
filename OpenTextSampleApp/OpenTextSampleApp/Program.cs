using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Data;

namespace OpenTextSampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("OpenText Sample Application Execution begins");
                string FolderLocation = ConfigurationManager.AppSettings["FolderLocation"];
                string ErrorFolderLocation = ConfigurationManager.AppSettings["ErrorFolderLocation"];
                string ArchiveFolderLocation = ConfigurationManager.AppSettings["ArchiveFolderLocation"];
                string ServerName = ConfigurationManager.AppSettings["Mail.ServerName"];

                Console.WriteLine("Reading from folder : {0}", FolderLocation);
                string FileName = Helper.GetProcessFile(FolderLocation);
                if (string.IsNullOrEmpty(FileName))
                {
                    throw new Exception("No XML File is available at the given location");
                }
                Console.WriteLine("Name of XML File is : {0}", Path.GetFileName(FileName));

                Console.WriteLine("Waiting for {0} seconds", ConfigurationManager.AppSettings["WaitTime"]);
                System.Threading.Thread.Sleep(Convert.ToInt32(ConfigurationManager.AppSettings["WaitTime"]) * 1000);

                Console.WriteLine("Wait Period Done. Searching for PDF file with same name");
                ValidatePDFFileExists(FolderLocation + @"\" + Path.GetFileNameWithoutExtension(FileName) + ".pdf", FileName, ErrorFolderLocation + @"\" + Path.GetFileName(FileName));

                Console.WriteLine("Validating with Control XSD");
                XMLValidation(FileName, ErrorFolderLocation + @"\" + Path.GetFileName(FileName), FolderLocation + @"\" + Path.GetFileNameWithoutExtension(FileName) + ".pdf");
                Console.WriteLine("XSD Validation successful");

                Console.WriteLine("XML Data Import Starts");
                DataSet XMLData = new DataSet();
                XMLData = Helper.ImportXmls(FileName);
                Console.WriteLine("XML Data Import Successful");

                Console.WriteLine("Entity Filling Starts");
                Entity instance = FillEntity(XMLData);

                Console.WriteLine("Entity Filling Successful");

                if (ConfigurationManager.AppSettings["EmailToBeSent"] == "Y")
                {
                    Console.WriteLine("Email begins");
                    bool MailStatus = EMailer.SendMail(instance.Sender, instance.Sender, instance.RecipientsCSV, instance.Subject, instance.MessageBody, ServerName, Path.GetDirectoryName(FileName) + @"\" + Path.GetFileNameWithoutExtension(FileName) + ".pdf");
                    if (MailStatus)
                    {
                        Console.WriteLine("Email successfully sent");
                    }
                    else
                    {
                        Console.WriteLine("Email sending failed");
                    }
                }
                else
                {
                    Console.WriteLine("Skipping Email Logic");
                }
                Console.WriteLine("Moving File to Archive");
                File.Move(FileName, ArchiveFolderLocation + @"\" + Path.GetFileName(FileName));
                File.Move(Path.GetDirectoryName(FileName) + @"\" + Path.GetFileNameWithoutExtension(FileName) + ".pdf", ArchiveFolderLocation + @"\" + Path.GetFileNameWithoutExtension(FileName) + ".pdf");
                Console.WriteLine("File Moved");

                if (ConfigurationManager.AppSettings["DBFillingEnabled"] == "Y")
                {
                    Console.WriteLine("DB Logging starts");
                    string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                    SQLLayer.WriteToDB(connection, instance);
                    Console.WriteLine("DB Logging successful");
                }
                else
                {
                    Console.WriteLine("Skipping DB Audit Logging Logic");
                }                

                Console.WriteLine("Program excution successfully completed !!!");

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
        private static void ValidatePDFFileExists(string FileLocation, string FileName, string ErrorLocation)
        {
            if (!File.Exists(FileLocation))
            {
                File.Move(FileName, ErrorLocation);
                throw new Exception("No PDF File available at the given location. XML file moved to Error Folder");
            }
        }
        private static void XMLValidation(string FileName, string ErrorFolderLocation, string FolderLocation)
        {
            string XSDValidationErrors = Helper.ValidateDocument(FileName, ConfigurationManager.AppSettings["XSDFolder"]);
            if (!string.IsNullOrEmpty(XSDValidationErrors))
            {
                File.Move(FileName, ErrorFolderLocation);
                File.Move(FolderLocation, Path.GetDirectoryName(ErrorFolderLocation) + @"\" + Path.GetFileNameWithoutExtension(FileName) + ".pdf");
                throw new Exception(XSDValidationErrors);
            }
        }
        private static Entity FillEntity(DataSet ds)
        {
            Entity entityInstance = new Entity();
            entityInstance.Sender = Convert.ToString(ds.Tables[0].Rows[0]["Sender"]);
            entityInstance.Subject = Convert.ToString(ds.Tables[0].Rows[0]["Subject"]);
            entityInstance.MessageBody = Convert.ToString(ds.Tables[0].Rows[0]["MessageBody"]);
            entityInstance.Recipients = new List<string>();
            foreach (DataRow recipient in ds.Tables[2].Rows)
            {
                entityInstance.Recipients.Add(Convert.ToString(recipient["recipient_Text"]));
            }
            entityInstance.RecipientsCSV = string.Join(",", entityInstance.Recipients.ToArray());
            return entityInstance;
        }
    }
}
