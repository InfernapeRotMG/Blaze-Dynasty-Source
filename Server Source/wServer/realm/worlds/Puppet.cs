namespace wServer.realm.worlds
{
    public class PuppetMastersTheatre : World
    {
        public PuppetMastersTheatre()
        {
            Name = "Puppet Master's Theatre";
            ClientWorldName = "Puppet Master's Theatre";
            Background = 0;
            Difficulty = 0;
            ShowDisplays = true;
            AllowTeleport = true;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.OrgaPuppet.jm", MapType.Json);
        }
    }
}
