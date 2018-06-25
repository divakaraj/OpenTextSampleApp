using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace OpenTextSampleApp
{
    class SQLLayer
    {
        /// <summary>
        /// Logic to store audit record in DB
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="instance"></param>
        public static void WriteToDB(string connectionString, Entity instance)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(@"
                    DECLARE @UniqueID int
                    INSERT INTO AuditRecord VALUES
                    (
	                     @DateTimeOfEntry
	                    ,@SenderAddress
                    )
                    SET @UniqueID = SCOPE_IDENTITY();
                ");
                foreach (string item in instance.Recipients)
                {
                    Sql.Append(@"
                    INSERT INTO RecipientRecord VALUES
                    (
                    	 '" + item + @"'
                    	,@UniqueID
                    )
                ");
                }
                SqlConnection Connection = new SqlConnection(connectionString);
                Connection.Open();
                SqlCommand Command = new SqlCommand(Sql.ToString(), Connection);
                Command.Parameters.Add(new SqlParameter("@DateTimeOfEntry", DateTime.Now));
                Command.Parameters.Add(new SqlParameter("@SenderAddress", instance.Sender));
                Command.ExecuteNonQuery();
                Connection.Close();
            }
            catch (SqlException SQLEx)
            {
                throw SQLEx;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
    }
}
