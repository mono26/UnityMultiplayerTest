using SLGFramework;

namespace MultiplayerTest
{
    public class GamePlayerSpawnedEvent : BaseGameEvent
    {
        public GamePlayer GamePlayerRef { get; private set; }

        public GamePlayerSpawnedEvent(GamePlayer player)
        {
            this.GamePlayerRef = player;
        }
    }
}