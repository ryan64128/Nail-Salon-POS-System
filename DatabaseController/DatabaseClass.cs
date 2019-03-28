using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DatabaseController
{
    public class DatabaseClass
    {
        private SqlConnection _connection;

        //---------- String passed to open database connection 
        //---------- (from right-click on NailSalon -> Properties ->
        //---------- In Properties Window find Connection String
        private string _connectionString = "Data Source = DESKTOP-UFT0K6V\\RYANSQLSERVER; " +
                                          "Initial Catalog = NailSalon; " +
                                          "Integrated Security = True; " +
                                          "Connect Timeout = 30; " +
                                          "Encrypt = False; " +
                                          "TrustServerCertificate = True; " +
                                          "ApplicationIntent = ReadWrite; " +
                                          "MultiSubnetFailover = False";

        //------------------ Publicly Accesible Constants ------------
        public const string CASH = "CASH";
        public const string CREDIT = "CREDIT";
        public const string DISCOUNT = "DISCOUNT";
        enum PAYMENT_TYPE { CASH, CREDIT, DISCOUNT };

        //------------------ Messages --------------------------------
        private const string CONNECTION_OK = "Connection Opened Successfully";
        private const string CONNECTION_OPEN_ERROR = "Error Opening Connection";
        private const string QUERY_OK = "Query Performed Successfully";
        private const string QUERY_ERROR = "Error Performing Query";
        private const string CONNECTION_CLOSE_OK = "Connection Closed Successfully";
        private const string CONNECTION_CLOSE_ERROR = "Error Closing Connection";
        private const string INSERT_OK = "Insert Command Performed Successfully";
        private const string INSERT_ERROR = "Error Performing Insert Command";

        //------------------ State Variables -------------------------
        public const string INVOICE_TABLE_STRING = "Invoice";
        public string makeSQLString(string inputString)
        {
            return "'" + inputString + "'";
        }
        public SqlConnection openConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            try
            {
                connection.Open();
                Console.WriteLine(CONNECTION_OK);
                return connection;
            }
            catch (Exception e)
            {
                Console.WriteLine(CONNECTION_OPEN_ERROR + e);
                return null;
            }
        }
        public void insert(string tableName, string[] attributeNames, string[] attributeValues)
        {
            SqlConnection connection = this.openConnection();
            string insertString = "INSERT INTO ";
            insertString += tableName + "(";
            for (int i = 0; i < attributeNames.Length - 1; i++)
            {
                insertString += attributeNames[i] + ", ";
            }
            insertString += attributeNames[attributeNames.Length - 1] + ") VALUES (";
            for (int i = 0; i < attributeValues.Length - 1; i++)
            {
                insertString += attributeValues[i] + ", ";
            }
            insertString += attributeValues[attributeValues.Length - 1] + ")";
            Console.WriteLine("INSERT_STRING = " + insertString);
            try
            {
                SqlCommand insertCommand = new SqlCommand(insertString, connection);
                insertCommand.ExecuteNonQuery();
                Console.WriteLine(INSERT_OK);
                closeConnection(connection);
            }
            catch (Exception e)
            {
                Console.WriteLine(INSERT_ERROR + e);
            }
        }
        public void insert(string insertString)
        {
            SqlConnection connection = openConnection();
            try
            {
                SqlCommand insertCommand = new SqlCommand(insertString, connection);
                insertCommand.ExecuteNonQuery();
                Console.WriteLine(INSERT_OK);
                closeConnection(connection);
            }
            catch (Exception e)
            {
                Console.WriteLine(INSERT_ERROR + e);
            }
        }
        public void delete(string deleteString)
        {
            SqlConnection connection = openConnection();
            try
            {
                SqlCommand insertCommand = new SqlCommand(deleteString, connection);
                insertCommand.ExecuteNonQuery();
                Console.WriteLine("Delete OK");
                closeConnection(connection);
            }
            catch (Exception e)
            {
                Console.WriteLine("Delete Error" + e);
            }
        }
        public string getLastInsertId(string tableName)
        {
            string queryString = "SELECT IDENT_CURRENT('" + tableName + "')";
            string result = query(queryString);
            Console.WriteLine("SCOPE_IDENTITY = " + result);
            return result;

        }
        public void update(string updateString)
        {
            SqlConnection connection = openConnection();
            try
            {
                SqlCommand updateCommand = new SqlCommand(updateString, connection);
                updateCommand.ExecuteNonQuery();
                Console.WriteLine("Update OK");
                closeConnection(connection);
            }
            catch (Exception e)
            {
                Console.WriteLine("Update Error" + e);
            }
        }
        public string query(string queryString)
        {
            Console.WriteLine("QUERY_STRING = " + queryString);
            SqlConnection connection = openConnection();
            string header = "";
            string lines = "";
            try
            {
                SqlCommand query = new SqlCommand(queryString, connection);
                SqlDataReader reader = query.ExecuteReader();
                Console.WriteLine(QUERY_OK);
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        header += reader.GetName(i) + "\t";
                    }
                    for (int i = 0; i < reader.FieldCount; i++)
                        lines += reader[i].ToString() + " ";
                    lines += "\n";
                }
                reader.Close();
                closeConnection(connection);
                return lines;
            }
            catch (Exception e)
            {
                Console.WriteLine(QUERY_ERROR + e);
                return null;
            }
        }
        public void closeConnection(SqlConnection connection)
        {
            try
            {
                connection.Close();
                Console.WriteLine(CONNECTION_CLOSE_OK);
            }
            catch (Exception e)
            {
                Console.WriteLine(CONNECTION_CLOSE_ERROR + e);
            }
        }
    }
}
