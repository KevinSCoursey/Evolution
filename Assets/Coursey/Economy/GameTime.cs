using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public class GameTime
    {
        public static int Seconds
        {
            get
            {
                return _Seconds;
            }
            set
            {
                if(_Seconds - 60 == 0)
                {
                    _Seconds = 0;
                    Minutes++;
                    TimeBasedCalls();
                    _TimeReference = (int)Time.time;
                }
                else
                {
                    _Seconds = value;
                }
            }
        }
        private static int _Seconds = 1;
        private static int _TimeReference = 0;
        public static int Minutes
        {
            get
            {
                return _Minutes;
            }
            set
            {
                if (_Minutes - 60 == 0)
                {
                    _Minutes = 0;
                    Hours++;
                    TimeBasedCalls();
                }
                else
                {
                    _Minutes = value;
                }
            }
        }
        private static int _Minutes = 0;
        public static int Hours
        {
            get
            {
                return _Hours;
            }
            set
            {
                if (_Hours - 24 == 0)
                {
                    _Hours = 0;
                    Days++;
                }
                else
                {
                    _Hours = value;
                }
            }
        }
        private static int _Hours = 0;
        public static int Days
        {
            get
            {
                return _Days;
            }
            set
            {
                if (_Days - 365 == 0)
                {
                    _Days = 0;
                    Years++;
                }
                else
                {
                    _Days = value;
                }
            }
        }
        private static int _Days = 0;
        public static int Years = 0;
        public static string GetGameTimeString()
        {
            return $"Game Time: {Years} Years, {Days} Days, {Minutes} Minutes, {Seconds} Seconds,\n Real Time: ({DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")})";
        }
        public static int GetSecondsRunning()
        {
            int sec = Seconds;
            if (Minutes > 0) sec = Seconds + Minutes * 60;
            return sec;
        }
        public static void TimeBasedCalls()
        {
            SkipTimeToCatchUp(IsGameRunningBehind());
        }
        public static void SkipTimeToCatchUp(bool isLagging)
        {
            //dosomething
        }
        public static bool IsGameRunningBehind()
        {
            int timeDifference = _TimeReference - (int)Time.time;
            if (timeDifference != 60)
            {
                //broken
                //Debug.Log($"Game is running behind by {timeDifference} seconds!");
            }
            return false;
        }
    }

    public class TimedBlock : IDisposable
    {
        private bool disposedValue;
        private bool _debugThisClass;
        private string _blockName;
        private DateTime _start;

        public TimedBlock(string blockName, bool debugThisClass = false)
        {
            _blockName = blockName;
            _start = DateTime.Now;
            _debugThisClass = debugThisClass;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            if (_debugThisClass)
            {
                var stop = DateTime.Now;
                var span = stop - _start;
                Debug.Log($"{_blockName} took {span.TotalMilliseconds} ms");
            }
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}