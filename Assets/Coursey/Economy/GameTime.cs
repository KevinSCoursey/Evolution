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
                }
                else
                {
                    _Seconds = value;
                }
            }
        }
        private static int _Seconds = 0;
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
        public static void Initialize()
        {
            
        }
    }

}