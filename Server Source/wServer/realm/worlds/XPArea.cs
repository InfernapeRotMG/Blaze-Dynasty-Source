using System;

namespace wServer.realm.worlds
{
    public class XPArea : World
    {
        public XPArea()
        {
            Id = XPAREA_ID;
            Name = "XPArea";
            ClientWorldName = "XPArea";
            Background = 0;
            Difficulty = 0;
            ShowDisplays = true;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.XPArea.jm", MapType.Json);
        }

        public override void Tick(RealmTime time)
        {
            base.Tick(time);

            if(Enemies.Count <= 50)
            {
                for (var i = 0; i < (50 - Enemies.Count); i++)
                {
                    if (this == null) break;
                    if (Players.Count < 1) break;
                    Random r = new Random();
                    ushort id;
                    int xloc;
                    int yloc;
                    Entity enemy = null;
                    var OutOfBoundsBool = true;
                    while (OutOfBoundsBool)
                    {
                        Manager.GameData.IdToObjectType.TryGetValue("XP Gift", out id);
                        xloc = r.Next(0, Map.Width);
                        yloc = r.Next(0, Map.Height);
                        enemy = Entity.Resolve(Manager, id);
                        enemy.Move(xloc, yloc);
                        OutOfBoundsBool = Map[xloc,yloc].Region != TileRegion.Spawn;
                    }
                    EnterWorld(enemy);
                }
            }
        }
    }
}
