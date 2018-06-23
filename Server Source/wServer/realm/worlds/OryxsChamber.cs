namespace wServer.realm.worlds
{
    public class OryxsChamber : World
    {
        public OryxsChamber()
        {
            Name = "Oryx's Chamber";
            ClientWorldName = "Oryx's Chamber";
            Background = 0;
            Difficulty = 0;
            ShowDisplays = true;
            AllowTeleport = true;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.oryxchamber.jm", MapType.Json);
        }
    }
}
