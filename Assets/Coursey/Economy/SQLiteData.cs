using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SQLiteData : MonoBehaviour
{
    public static string dataFileName = "data.db";
    public static string dataSubDirectory = "data";
    public static string path;

    public TextAsset CreateDatabaseScript;

    public static void Initialize()
    {
        if (Directory.Exists(Path.Combine(Application.persistentDataPath, dataSubDirectory)))
        {
            path = Path.Combine(Application.persistentDataPath, dataSubDirectory);
            Debug.Log($"Subdirectory \"{dataSubDirectory}\" already exists with path \"{path}\"");
        }
        else
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, dataSubDirectory));
            path = Path.Combine(Application.persistentDataPath, dataSubDirectory);
            Debug.Log($"Subdirectory \"{dataSubDirectory}\" didn't exist. Creating subdirectory \"{dataSubDirectory}\" with path \"{path}\"");
        }
        Debug.Log($"Reading from \"{path}\"");
    }
    /*private void RunTest()
    {
        using (var basicSql = new BasicSql())
        {
            basicSql.ExecuteNonReader(@"
                CREATE TABLE IF NOT EXISTS Item (id INTEGER PRIMARY KEY , Name VARCHAR(100));
                ");

            var id = DateTime.Now.Millisecond;
            basicSql.ExecuteNonReader(
                "INSERT INTO Item (Id, Name) VALUES ($id, $name)",
                //item.Add(list<T>);

                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("$id", id.ToString()),
                    new KeyValuePair<string, string>("$name", Guid.NewGuid().ToString())
                }
                );

            var name = "";
            basicSql.ExecuteReader(
                "SELECT Name FROM Item WHERE Id = $id",
                //SELECT * FROM tableName WHERE var = value
                //
                //var = list.Select(_ => _.var == value);
                new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("$id", id.ToString())
                },
                (rowData) =>
                {
                    name = rowData["Name"].ToString();
                }
                );

            name = basicSql.ExecuteScalar<string>(
                "SELECT Name FROM Item WHERE Id = $id",
                new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("$id", id.ToString())
                }
                );
        }
    }*/
}

public class BasicSql : IDisposable
{
    private string DataFilePath => Path.Combine(SQLiteData.path, SQLiteData.dataFileName);
    private static SqliteConnection _connection;
    private bool _disposedValue;

    public BasicSql()
    {
        if (_connection == null || _connection.State == System.Data.ConnectionState.Closed)
        {
            OpenConnection();
        }
    }

    public void ExecuteNonReader(string sql)
    {
        if (_connection == null || _connection.State == System.Data.ConnectionState.Closed)
        {
            OpenConnection();
        }
        using (var command = _connection.CreateCommand())
        {
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }
    }

    public void ExecuteNonReader(string sql, IEnumerable<KeyValuePair<string, string>> parameters)
    {
        using (var command = _connection.CreateCommand())
        {
            command.CommandText = sql;
            foreach (var pram in parameters)
            {
                command.Parameters.AddWithValue(pram.Key, pram.Value);
            }
            command.ExecuteNonQuery();
        }
    }

    public void ExecuteReader(string sql, Action<SqliteDataReader> rowAction)
    {
        using (var command = _connection.CreateCommand())
        {
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

    public void ExecuteReader(string sql, IEnumerable<KeyValuePair<string, string>> parameters, Action<SqliteDataReader> rowAction)
    {
        if (_connection == null || _connection.State == System.Data.ConnectionState.Closed)
        {
            OpenConnection();
        }
        using (var command = _connection.CreateCommand())
        {
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

    public N ExecuteScalar<N>(string sql)
    {
        N result = default;
        using (var command = _connection.CreateCommand())
        {
            command.CommandText = sql;
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    result = (N)reader[0];
                }
            }
        }
        return result;
    }

    public string ExecuteScalar(string sql, IEnumerable<KeyValuePair<string, string>> parameters)
    {
        var result = "";
        using (var command = _connection.CreateCommand())
        {
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
        return result;
    }

    private void OpenConnection()
    {
        _connection = new SqliteConnection($"URI=file:{DataFilePath}");
        _connection.Open();
    }

    public void RunScript(TextAsset textAsset)
    {
        using (var cmd = _connection.CreateCommand())
        {
            cmd.CommandText = textAsset.text;
            cmd.ExecuteNonQuery();
        }
    }

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                if (_connection != null && _connection.State != System.Data.ConnectionState.Closed)
                {
                    _connection.Close();
                }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~BasicSql()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
