using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Economy
{
    public class SQLiteData
    {
        private const bool _debugThisClass = false;

        public static string DataFileName = "data.db";
        public static string DataSubDirectory = "data";
        public static string Path;
        public TextAsset CreateDatabaseScript;

        public static void Initialize()
        {
            if (Directory.Exists(System.IO.Path.Combine(Application.persistentDataPath, DataSubDirectory)))
            {
                Path = System.IO.Path.Combine(Application.persistentDataPath, DataSubDirectory);
#pragma warning disable CS0162 // Unreachable code detected
                if (_debugThisClass) Debug.Log($"Subdirectory \"{DataSubDirectory}\" already exists with path \"{Path}\"");
#pragma warning restore CS0162 // Unreachable code detected
            }
            else
            {
                Directory.CreateDirectory(System.IO.Path.Combine(Application.persistentDataPath, DataSubDirectory));
                Path = System.IO.Path.Combine(Application.persistentDataPath, DataSubDirectory);
#pragma warning disable CS0162 // Unreachable code detected
                if (_debugThisClass) Debug.Log($"Subdirectory \"{DataSubDirectory}\" didn't exist. Creating subdirectory \"{DataSubDirectory}\" with path \"{Path}\"");
#pragma warning restore CS0162 // Unreachable code detected
            }
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass) Debug.Log($"Reading from \"{Path}\"");
#pragma warning restore CS0162 // Unreachable code detected
        }
    }
    public class BasicSql : IDisposable
    {
        private const bool _debugThisClass =false;
        private string DataFilePath => System.IO.Path.Combine(SQLiteData.Path, SQLiteData.DataFileName);
        private static SqliteConnection _connection;
        private bool _disposedValue;
        private static string DataPath = Application.persistentDataPath;
        public bool UseTransaction;
        private SqliteTransaction _transaction;

        static BasicSql()
        {
        }
        public BasicSql(bool useTransaction = false)
        {
            UseTransaction = useTransaction;
            if (_connection == null || _connection.State == System.Data.ConnectionState.Closed)
            {
                OpenConnection();
            }
        }
        public void BeginTransaction()
        {
            if (!UseTransaction)
            {
                throw new ApplicationException("Cannot start a transaction when BasicSql was told not to use transactions.");
            }

            _transaction = _connection.BeginTransaction();
        }
        public void CommitTransaction()
        {
            _transaction.Commit();
            _transaction = null;
        }
        public void RollbackTransaction()
        {
            _transaction.Rollback();
            _transaction = null;
        }
        public void ExecuteNonReader(string sql)
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass) Debug.Log(sql);
#pragma warning restore CS0162 // Unreachable code detected
            OpenConnection();
            try
            {
                using (var command = _connection.CreateCommand())
                {
                    if (UseTransaction)
                    {
                        command.Transaction = _transaction;
                    }

                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        public void ExecuteNonReader(string sql, IEnumerable<KeyValuePair<string, string>> parameters)
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass) Debug.Log(sql);
#pragma warning restore CS0162 // Unreachable code detected
            OpenConnection();
            try
            {
                using (var command = _connection.CreateCommand())
                {
                    if (UseTransaction)
                    {
                        command.Transaction = _transaction;
                    }

                    command.CommandText = sql;
                    foreach (var pram in parameters)
                    {
                        command.Parameters.AddWithValue(pram.Key, pram.Value);
                    }
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        public void ExecuteReader(string sql, Action<SqliteDataReader> rowAction)
        {
            try
            {
                using (var command = _connection.CreateCommand())
                {
                    if (UseTransaction)
                    {
                        command.Transaction = _transaction;
                    }

                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rowAction?.Invoke(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        public void ExecuteReader(string sql, IEnumerable<KeyValuePair<string, string>> parameters, Action<SqliteDataReader> rowAction)
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass) Debug.Log(sql);
#pragma warning restore CS0162 // Unreachable code detected
            OpenConnection();
            try
            {
                using (var command = _connection.CreateCommand())
                {
                    if (UseTransaction)
                    {
                        command.Transaction = _transaction;
                    }

                    command.CommandText = sql;
                    foreach (var pram in parameters)
                    {
                        command.Parameters.AddWithValue(pram.Key, pram.Value);
                    }
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rowAction?.Invoke(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        public N ExecuteScalar<N>(string sql)
        {
            N result = default;
            try
            {
                using (var command = _connection.CreateCommand())
                {
                    if (UseTransaction)
                    {
                        command.Transaction = _transaction;
                    }

                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = (N)reader[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            return result;
        }
        public N ExecuteScalar<N>(string sql, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            N result = default;
            try
            {
                using (var command = _connection.CreateCommand())
                {
                    if (UseTransaction)
                    {
                        command.Transaction = _transaction;
                    }

                    command.CommandText = sql;
                    foreach (var pram in parameters)
                    {
                        command.Parameters.AddWithValue(pram.Key, pram.Value);
                    }
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = (N)reader[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            return result;
        }
        public string ExecuteScalar(string sql, IEnumerable<KeyValuePair<string, string>> parameters)
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass) Debug.Log(sql);
#pragma warning restore CS0162 // Unreachable code detected
            OpenConnection();
            var result = "";
            try
            {
                using (var command = _connection.CreateCommand())
                {
                    if (UseTransaction)
                    {
                        command.Transaction = _transaction;
                    }

                    command.CommandText = sql;
                    foreach (var pram in parameters)
                    {
                        command.Parameters.AddWithValue(pram.Key, pram.Value);
                    }
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = reader[0]?.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            return result;
        }
        private void OpenConnection()
        {
            if (_connection == null || _connection.State == System.Data.ConnectionState.Closed)
            {
                try
                {
                    _connection = new SqliteConnection($"URI=file:{DataFilePath}");
                    _connection.Open();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
        public void RunScript(TextAsset textAsset)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = textAsset.text;
                cmd.ExecuteNonQuery();
            }
        }
        public static void DebugRowData(SqliteDataReader rowData, bool debugThatClass)
        {
            if (debugThatClass && _debugThisClass)
            {
                var fieldCount = rowData.FieldCount;
                string debug = "";
                for (var currentFieldIdx = 0; currentFieldIdx < fieldCount; currentFieldIdx++)
                {
                    debug += $"{rowData.GetFieldType(currentFieldIdx)} - {rowData.GetName(currentFieldIdx)} - {rowData[currentFieldIdx].ToString()}\n";
                }
                Debug.Log(debug);
            }
        }
        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_connection != null && _connection.State != System.Data.ConnectionState.Closed)
                    {
                        _connection.Close();
                    }
                }
                _disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}