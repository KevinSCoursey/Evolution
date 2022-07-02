using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace Economy
{
    public class JsonManager
    {
        private string _path;
        private static bool _debugThisClass = false;
        public JsonManager(string subdirectory = "config")
        {
            if(Directory.Exists(Path.Combine(Application.persistentDataPath, subdirectory)))
            {
                _path = Path.Combine(Application.persistentDataPath, subdirectory);

                if (_debugThisClass)
                {
                    Debug.Log($"Subdirectory \"{subdirectory}\" already exists with path \"{_path}\"");
                }
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, subdirectory));
                _path = Path.Combine(Application.persistentDataPath, subdirectory);
                if (_debugThisClass)
                {
                    Debug.Log($"Subdirectory \"{subdirectory}\" didn't exist. Creating subdirectory \"{subdirectory}\" with path \"{_path}\"");
                }
            }
            if (_debugThisClass)
            {
                Debug.Log($"Reading from \"{_path}\"");
            }
        }
        public T ReadConfig<T>() where T : class
        {
            var name = typeof(T).Name;
            var fileName = Path.Combine(_path, name + ".json");
            var json = File.Exists(fileName)
                ? File.ReadAllText(fileName)
                : null;
            if (string.IsNullOrEmpty(json))
            {
                var result = Activator.CreateInstance<T>();
                if (!File.Exists(fileName))
                {
                    json = JsonConvert.SerializeObject(result, Formatting.Indented);
                    if (_debugThisClass)
                    {
                        Debug.Log($"File \"{name}.json\" didn't exist. Writing file to \"{fileName}\"");
                    }
                    File.WriteAllText(fileName, json);
                }
                return result;
            }
            else
            {
                var value = JsonConvert.DeserializeObject<T>(json);
                return value;
            }
        }
        public void WriteConfig<T>(T objToSerialize) where T : class
        {
            var name = typeof(T).Name;
            var fileName = Path.Combine(_path, name + ".json");
            var json = JsonConvert.SerializeObject(objToSerialize, Formatting.Indented);
            if (_debugThisClass)
            {
                Debug.Log($"Writing file \"{name}.json\" to \"{fileName}\"");
            }
            File.WriteAllText(fileName, json);
        }
    }
}