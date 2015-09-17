using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DXTest.App_Data
{
    public class XmlFileEntry
    {
        public string Filename { get; set; }
        public int Id { get; set; }
    }

    public class XmlFile
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public byte[] Blob { get; set; }
    }

    public class DatabaseManager
    {
        private static string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString;
        }

        /// <summary>
        /// Gets the XML file in the database, with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static XmlFile GetXmlfile(string name)
        {
            string connectionString = GetConnectionString();
            XmlFile file = null;

            // Creates a connection to the database
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Id,Name,XmlData FROM dbo.XmlFiles WHERE Name=@name";
                    command.Parameters.AddWithValue("name", name);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int readid = reader.GetInt32(0);
                            string filename = reader.GetString(1);
                            byte[] blob = reader.GetSqlBinary(2).Value;

                            file = new XmlFile() { Filename = filename, Id = readid, Blob = blob };
                        }
                    }
                }
            }

            return file;
        }

        public static bool DoesDatabaseContainXmlFile(string filename)
        {
            string connectionString = GetConnectionString();
            bool doesContain = false;

            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Id FROM dbo.XmlFiles WHERE Name=@name";
                    command.Parameters.AddWithValue("name", filename);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            doesContain = true;
                        }
                    }
                }
            }

            return doesContain;
        }

        public static List<XmlFileEntry> GetAllFiles()
        {
            List<XmlFileEntry> files = new List<XmlFileEntry>();

            string connectionString = GetConnectionString();

            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Id,Name FROM dbo.XmlFiles";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string filename = reader.GetString(1);

                            files.Add(new XmlFileEntry() { Filename = filename, Id = id });
                        }
                    }
                }
            }

            return files;
        }

        /// <summary>
        /// Deletes the XMl file from the database that has the id specified in the parameter
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteXmlFileToDatabase(int id)
        {
            string connectionString = GetConnectionString();

            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM dbo.XmlFiles WHERE Id=@id";
                    command.Parameters.AddWithValue("id", id);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Creates a new XML file in the database
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="serializedXml"></param>
        public static void SaveNewXmlFileToDatabase(string filename, byte[] serializedXml)
        {
            string connectionString = GetConnectionString();

            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO dbo.XmlFiles (Name, XmlData) VALUES(@filename, @xmldata)";
                    command.Parameters.AddWithValue("filename", filename);
                    command.Parameters.AddWithValue("xmldata", serializedXml);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateXmlFile(string filename, byte[] serializedXml)
        {
            string connectionString = GetConnectionString();

            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE dbo.XmlFiles SET XmlData=@xmldata WHERE Name=@filename";
                    command.Parameters.AddWithValue("filename", filename);
                    command.Parameters.AddWithValue("xmldata", serializedXml);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
