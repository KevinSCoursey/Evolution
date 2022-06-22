using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Economy
{ 
    public class GameController : MonoBehaviour
    {
        private const bool _debugThisClass = true;
        private bool gameLoaded = false;
        public bool gameRunning = true;
        public int tickCounter = 0;

        public static int seed = 8675309;
        public FactionController factionController;
        public EconomyController economyController;
        void Start()
        {
            MathTools.Initialize(seed);
            NameRandomizer.Initialize();

            /*for (int i = 0; i < 100; i++)
            {
                Debug.Log(NameRandomizer.GenerateUniqueNamev2(NameType.Faction));
            }*/

            //Just makes this not cause a compile warning
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass) Debug.Log($"Seed: {seed}");
#pragma warning restore CS0162 // Unreachable code detected

            factionController = new FactionController();
            economyController = new EconomyController(factionController);
            factionController.GenerateRandomTradeStations(economyController.economyItemController.items);

            //economyController.economyEventController.TriggerRandomEvent(factionController.GetRandomFaction());
            //economyController.economyEventController.TriggerRandomEvent(factionController.GetRandomFaction(), true);
            gameLoaded = true;
            StartCoroutine(GameLoop());
        }

        public void StopGame()
        {
            gameLoaded = false;
        }

        IEnumerator GameLoop()
        {
            while (gameRunning && gameLoaded)
            {
                if (tickCounter >= GameSettings.TicksPerSecond)
                {
                    GameTime.Seconds++;
                    tickCounter = 0;
                }
                else
                {
                    tickCounter++;
                }

                economyController.economyEventController.GameLoop();
                factionController.GameLoop();

                if(GameTime.Minutes == 1)
                {
                    gameRunning = false;
                }

                yield return new WaitForSeconds(1 / GameSettings.TicksPerSecond);
            }
        }
    }
}
