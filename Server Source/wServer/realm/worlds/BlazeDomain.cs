#region

using wServer.networking;

#endregion

namespace wServer.realm.worlds
{
    public class BlazeDomain : World
    {
        public BlazeDomain()
        {
            Name = "Blaze Domain";
            ClientWorldName = "Blaze Domain";
            Dungeon = true;
            Background = 0;
            AllowTeleport = false;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.BlazeRoom.jm", MapType.Json);
        }

        public override World GetInstance(Client psr)
        {
            return Manager.AddWorld(new BlazeDomain());
        }
    }
}