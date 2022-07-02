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
        /*public const int THIS_IS_A_CONST;
        private const int THIS_IS_ANOTHER;
        private int _thisIsPrivate;
        public int ThisIsPublic;

        private void Foo()
        {
            var something = "this is local";
        }*/



        private const bool _debugThisClass = true;
        public static bool gameLoaded { get; private set; } = false;
        public bool gameRunning = true;
        public int tickCounter = 0;

        public static int seed = 0;//8675309;
        public static FactionController factionController;
        public static TradeStationController tradeStationController;
        void Start()
        {
            SQLiteData.Initialize();
            gameLoaded = GameSettings.LoadSettings() && MathTools.Initialize(seed) && NameRandomizer.Initialize();
            DataBaseInteract.ClearDataBase();
            DataBaseInteract.CreateDataBase();
            factionController = new FactionController();
            EconomyItemController.Initialize();
            EconomyEventController.Initialize();
            tradeStationController = new TradeStationController();

            Debug.Log($"The economy will run at {GameSettings.TicksPerSecond} tick(s) per second.");
            //InvokeRepeating("GameLoop", 1f, 1f / GameSettings.TicksPerSecond);
        }
        public void StopGame()
        {
            gameLoaded = false;
            Debug.Log($"Game has stopped.");
            Application.Quit();
        }
        public void GameLoop()
        {
            using(new TimedBlock("Master game loop (one tick)", _debugThisClass))
            {
                if (gameRunning && gameLoaded)
                {
                    if (tickCounter >= GameSettings.TicksPerSecond)
                    {
                        GameTime.Seconds++;
                        tickCounter = 0;
                    }

                    tickCounter++;

                    Debug.Log(GameTime.GetGameTimeString());

                    using (new TimedBlock("EVENT CONTROLLER GAME LOOP"))
                        EconomyController.GameLoop();

                    using (new TimedBlock("FACTION GAME LOOP"))
                        factionController.GameLoop();

                    if (GameTime.GetSecondsRunning() >= (60f * GameSettings.MinutesGameWillRunFloat))
                    {
                        gameRunning = false;
                        StopGame();
                    }
                }
            }
        }
    }
}
