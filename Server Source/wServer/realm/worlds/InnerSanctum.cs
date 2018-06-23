namespace wServer.realm.worlds
{
    public class TheInnerSanctum : World
    {
        public TheInnerSanctum()
        {
            Name = "The Inner Sanctum";
            ClientWorldName = "The Inner Sanctum";
            Background = 0;
            Difficulty = 0;
            ShowDisplays = true;
            AllowTeleport = true;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.OrgeIceCaveBoss.jm", MapType.Json);
        }
    }
}
