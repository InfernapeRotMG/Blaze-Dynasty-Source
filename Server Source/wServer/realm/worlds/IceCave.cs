namespace wServer.realm.worlds
{
    public class IceCave : World
    {
        public IceCave()
        {
            Name = "Ice Cave";
            ClientWorldName = "Ice Cave";
            Background = 0;
            Difficulty = 0;
            ShowDisplays = true;
            AllowTeleport = true;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.OrgaIceCave.jm", MapType.Json);
        }
    }
}
