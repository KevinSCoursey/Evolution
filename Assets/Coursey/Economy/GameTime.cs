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
                return _seconds;
            }
            set
            {
                if(_seconds - 60 == 0)
                {
                    _seconds = 0;
                    Minutes++;
                    TimeBasedCalls();
                    _timeReference = (int)Time.time;
                }
                else
                {
                    _seconds = value;
                }
            }
        }
        private static int _seconds = 1;
        private static int _timeReference = 0;
        public static int Minutes
        {
            get
            {
                return _minutes;
            }
            set
            {
                if (_minutes - 60 == 0)
                {
                    _minutes = 0;
                    Hours++;
                    TimeBasedCalls();
                }
                else
                {
                    _minutes = value;
                }
            }
        }
        private static int _minutes = 0;
        public static int Hours
        {
            get
            {
                return _hours;
            }
            set
            {
                if (_hours - 24 == 0)
                {
                    _hours = 0;
                    Days++;
                }
                else
                {
                    _hours = value;
                }
            }
        }
        private static int _hours = 0;
        public static int Days
        {
            get
            {
                return _days;
            }
            set
            {
                if (_days - 365 == 0)
                {
                    _days = 0;
                    Years++;
                }
                else
                {
                    _days = value;
                }
            }
        }
        private static int _days = 0;
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
            int timeDifference = _timeReference - (int)Time.time;
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
        private bool _disposedValue;
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
            if (!_disposedValue)
            {
                if (disposing)
                {
                }
                _disposedValue = true;
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
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}