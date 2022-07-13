using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine
{
    public class MonoBehaviour
    {
        public void RunStart()
        {
            var t = GetType();

            // Start() is a private, so I cannot invoke it directly.  We must use
            // reflection to locate the method (non-public of an instance) so that
            // we can call it.
            var method = GetType().GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(this, null);
        }

        public void InvokeRepeating(string methodName, float time, float repeatRate)
        {
            var method = GetType().GetMethod(methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Thread.Sleep((int)time * 1000);
            while (true)
            {
                method.Invoke(this, null);
                Thread.Sleep((int)repeatRate * 1000);
            }
        }
    }

    public class Application
    {
        public static string persistentDataPath => ".";

        public static void Quit()
        {
            Environment.Exit(0);
        }
    }

    public class Debug
    {
        public static void Log(string message)
        {
            Console.WriteLine(message);
        }

        public static void LogException(Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Console.WriteLine(ex.StackTrace);
        }
    }

    public class Mathf
    {
        public static float Clamp(float value, float floor, float roof)
        {
            return value < floor
                ? floor
                : value > roof
                    ? roof
                    : value;
        }

        public static int Clamp(int value, int floor, int roof)
        {
            return value < floor
                ? floor
                : value > roof
                    ? roof
                    : value;
        }

        public static float Log(float value)
        {
            return (float)Math.Log(value);
        }

        public static float Sin(float value)
        {
            return (float)Math.Sin(value);
        }

        public static float Sqrt(float value)
        {
            return (float)Math.Sqrt(value);
        }

        public static float Pow(float value, float exp)
        {
            return (float)Math.Pow(value, exp);
        }
    }

    public class Time
    {
        private static DateTime _appStart;

        static Time()
        {
            _appStart = DateTime.Now;
        }

        public static int time
        {
            get
            {
                var now = DateTime.Now;
                var span = now - _appStart;
                return (int)span.TotalSeconds;
            }
        }
    }
}
