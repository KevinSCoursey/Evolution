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
        private const bool _debugThisClass = true;
        public static bool gameLoaded { get; private set; } = false;
        public bool gameRunning = true;
        public int tickCounter = 0;

        public static int seed = 0;//8675309;
        public static FactionController factionController;
        public static EconomyController economyController;
        public static EconomyItemController economyItemController;
        public static EconomyEventController economyEventController;
        public static TradeStationController tradeStationController;
        void Start()
        {
            SQLiteData.Initialize();
            gameLoaded = GameSettings.LoadSettings() && MathTools.Initialize(seed) && NameRandomizer.Initialize();
            DataBaseInteract.ClearDataBase();
            DataBaseInteract.CreateDataBase();
            factionController = new FactionController();
            economyController = new EconomyController();
            economyItemController = new EconomyItemController();
            economyEventController = new EconomyEventController();
            tradeStationController = new TradeStationController();

            //economyController.SaveData();
            /*factionController.SaveData();
            factionController.GenerateRandomTradeRoutes();
            factionController.SaveTradeRouteData();

            Debug.Log($"The economy will run at {GameSettings.TicksPerSecond} tick(s) per second.");
            InvokeRepeating("GameLoop", 1f, 1f / GameSettings.TicksPerSecond);*/
        }

        //make all functions get factions etc from sql and make a set of functions to do it easy and rewrite the data (this is getting messy!)
        //data restructuring - build a faction and all of its trade stations then write to database and clear from memoory


        public void StopGame()
        {
            gameLoaded = false;
            Debug.Log($"Game has stopped.");
            Application.Quit();
        }
        /*public void GameLoop()
        {
            var start = DateTime.Now;
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
                    economyController.economyEventController.GameLoop();

                using (new TimedBlock("GAME LOOP"))
                    factionController.GameLoop();

                if (GameTime.GetSecondsRunning() >= (60f * GameSettings.MinutesGameWillRunFloat))
                {
                    gameRunning = false;
                    StopGame();
                }
                var stop = DateTime.Now;
                var elapsed = stop - start;
                Debug.Log($"One tick took {elapsed.TotalMilliseconds} ms to run.");
            }
        }*/
    }
}
