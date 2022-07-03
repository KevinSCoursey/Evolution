/*
 * 2 million trade stations /
 * each station w/ 4 routes /
 * 25 commodities /
 * 10 factions /
 * 
 * var start = DateTime.Now;
 * ...
 * var stop = DateTime.Now;
 * var elapsed = stop - start;
 * Debug.Log($"This took {elapsed.TotalMilliseconds} ms to run.");
 * 
 * 
 * TODO:
 * Significantly optimize how data is handeled in database speed-wise this is way too slow
 * Re-implement trade routes
 * Re-implement economy events
 */
using System;
using UnityEngine;

namespace Economy
{
    public class GameController : MonoBehaviour
    {
        private const bool _debugThisClass = true;
        public static bool GameLoaded { get; private set; } = false;
        public bool GameRunning = true;
        public int TickCounter = 0;

        public static int Seed = 0;
        void Start()
        {
            SQLiteData.Initialize();
            GameLoaded = GameSettings.LoadSettings() && MathTools.Initialize(Seed) && NameRandomizer.Initialize();
            DataBaseInteract.ClearDataBase();
            DataBaseInteract.CreateDataBase();
            FactionController.Initialize();
            EconomyItemController.Initialize();
            EconomyEventController.Initialize();
            TradeStationController.Initialize();
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass)
            {
                Debug.Log($"The economy will attempt to run at {GameSettings.TicksPerSecond} tick(s) per second.");
            }
#pragma warning restore CS0162 // Unreachable code detected
            InvokeRepeating("GameLoop", 1f, 1f / GameSettings.TicksPerSecond);
        }
        public static bool GameTickReady()
        {
            return FactionController.IsReady && EconomyItemController.IsReady && EconomyEventController.IsReady;
        }
        public void StopGame()
        {
            GameLoaded = false;
            Debug.Log($"Game has stopped.");
            Application.Quit();
        }
        public void GameLoop()
        {
            if (GameRunning && GameLoaded)
            {
                if (GameTickReady())
                {
                    using (new TimedBlock("Master game loop (one tick)", _debugThisClass))
                    {
                        if (TickCounter >= GameSettings.TicksPerSecond)
                        {
                            GameTime.Seconds++;
                            TickCounter = 0;
                        }
                        TickCounter++;
#pragma warning disable CS0162 // Unreachable code detected
                        if (_debugThisClass)
                        {
                            Debug.Log(GameTime.GetGameTimeString());
                        }
#pragma warning restore CS0162 // Unreachable code detected
                        using (new TimedBlock("EVENT CONTROLLER GAME LOOP"))
                        {
                            EconomyController.GameLoop();
                        }
                        using (new TimedBlock("FACTION GAME LOOP"))
                        {
                            FactionController.GameLoop();
                        }
                        if (GameTime.GetSecondsRunning() >= (60f * GameSettings.MinutesGameWillRunFloat))
                        {
                            GameRunning = false;
                            StopGame();
                        }
                    }
                }
                else
                {
#pragma warning disable CS0162 // Unreachable code detected
                    if (_debugThisClass)
                    {
                        Debug.Log($"Master game loop (one tick) wasn't ready! Skipping...");
                    }
#pragma warning restore CS0162 // Unreachable code detected
                }
            }
        }
    }
}
