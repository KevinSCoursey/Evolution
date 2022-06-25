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
        public FactionController factionController;
        public EconomyController economyController;
        void Start()
        {
            gameLoaded = GameSettings.LoadSettings() && MathTools.Initialize(seed) && NameRandomizer.Initialize();

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
            
            //InvokeRepeating("GameLoop", 1f, (float)(1 / GameSettings.TicksPerSecond));
            Debug.Log($"The economy will run at {GameSettings.TicksPerSecond} tick(s) per second.");
            InvokeRepeating("GameLoop", 1f, 1f / GameSettings.TicksPerSecond);
        }

        public void StopGame()
        {
            gameLoaded = false;
        }

        public void GameLoop()
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
                economyController.economyEventController.GameLoop();
                factionController.GameLoop();

                if (GameTime.Minutes >= 1)
                {
                    gameRunning = false;
                }
            }
        }
    }
}
