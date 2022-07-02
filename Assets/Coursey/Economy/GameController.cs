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
 */
using System;
using UnityEngine;

namespace Economy
{
    public class GameController : MonoBehaviour
    {
        private const bool _debugThisClass = false;
        public static bool gameLoaded { get; private set; } = false;
        public bool gameRunning = true;
        public int tickCounter = 0;

        public static int seed = 0;
        void Start()
        {
            SQLiteData.Initialize();
            gameLoaded = GameSettings.LoadSettings() && MathTools.Initialize(seed) && NameRandomizer.Initialize();
            DataBaseInteract.ClearDataBase();
            DataBaseInteract.CreateDataBase();
            FactionController.Initialize();
            EconomyItemController.Initialize();
            EconomyEventController.Initialize();
            TradeStationController.Initialize();
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass)
            {
                Debug.Log($"The economy will run at {GameSettings.TicksPerSecond} tick(s) per second.");
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
            gameLoaded = false;
            Debug.Log($"Game has stopped.");
            Application.Quit();
        }
        public void GameLoop()
        {
            if (gameRunning && gameLoaded && GameTickReady())
            {
                using (new TimedBlock("Master game loop (one tick)", _debugThisClass))
                {
                    if (tickCounter >= GameSettings.TicksPerSecond)
                    {
                        GameTime.Seconds++;
                        tickCounter = 0;
                    }
                    tickCounter++;
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
                        gameRunning = false;
                        StopGame();
                    }
                }
            }
            else if(gameRunning && gameLoaded && !GameTickReady())
            {
#pragma warning disable CS0162 // Unreachable code detected
                if (_debugThisClass)
                {
                    Debug.Log($"Game loop wasn't ready! Skipping...");
                }
#pragma warning restore CS0162 // Unreachable code detected
            }
        }
    }
}
