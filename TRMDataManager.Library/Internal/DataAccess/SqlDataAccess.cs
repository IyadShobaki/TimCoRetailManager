using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

//TMRDataManager.Libraray is a Class Library (.Net Framework), we will upgrade it to (.Net Core) later
//Remove class1.cs after creation
namespace TRMDataManager.Library.Internal.DataAccess
{
    //This class library is just for the API not the (API and the WPF) 
    //because they are not the same type of project
    //This library will do data access. WPF should not know nothing about database nor have access to it
    internal class SqlDataAccess : IDisposable  //internal means that this class will not be used out side the library
    {
        public string GetConnectionString(string name)
        {
            //to add ConfigurationManager -> click Ctrl + . and click (Add reference to 'System.Configuration.dll'
            //the connection string is in TRMDataManager - Web.config
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
        //we need couple of methods for getting and setting data
        //so we need to install dapper. Right click on the references on TMRDataManager.Libraray
        //then Manage NuGet Packages. Browse and search for Dapper and install it.
        //dapper is kind of micro ORM (Object relational mapper). Allow us to talk to the database and get information
        //and map that data into objects

        //Read method to load data from the database 
        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                //here where dapper come 
                List<T> rows = connection.Query<T>(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }

        //Write method to the database
        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {

                connection.Execute(storedProcedure, parameters,
                     commandType: CommandType.StoredProcedure);
            }
        }

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        // Open connection/start transaction method
        public void StartTransaction(string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);
            _connection = new SqlConnection(connectionString);
            _connection.Open();

            _transaction = _connection.BeginTransaction();

            isClosed = false;
        }

        // load using the transaction
        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {
            List<T> rows = _connection.Query<T>(storedProcedure, parameters,
                commandType: CommandType.StoredProcedure, transaction: _transaction).ToList();

            return rows;
        }
        // save using the transaction
        public void SaveDataInTransaction<T>(string storedProcedure, T parameters)
        {

            _connection.Execute(storedProcedure, parameters,
                        commandType: CommandType.StoredProcedure, transaction: _transaction);
        }

        private bool isClosed = false;

        // close connection/stop transaction method
        public void CommitTransaction()
        {
            _transaction?.Commit();
            _connection?.Close();

            isClosed = true;
        }
        // close connection/stop transaction method
        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _connection?.Close();

            isClosed = true;
        }

        // dispose
        public void Dispose()
        {
            if (!isClosed)
            {
                try
                {
                    CommitTransaction();
                }
                catch
                {
                    // TODO - Log this issue
                }
            }

            _transaction = null;
            _connection = null;
        }




    }
}
