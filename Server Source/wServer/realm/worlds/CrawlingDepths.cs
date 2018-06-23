namespace wServer.realm.worlds
{
    public class TheCrawlingDepths : World
    {
        public TheCrawlingDepths()
        {

            Name = "The Crawling Depths";
            ClientWorldName = "The Crawling Depths";
            Background = 2;
            AllowTeleport = true;
            Difficulty = 0;

        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.crawling.wmap", MapType.Wmap);
        }
    }
}