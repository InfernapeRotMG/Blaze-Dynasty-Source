#region

using wServer.networking;

#endregion

//namespace wServer.realm.worlds
//{
//    public class UndeadLair : World
//    {
//        public UndeadLair()
//        {
//            Name = "Undead Lair";
//            ClientWorldName = "dungeons.Undead_Lair";
//            Dungeon = true;
//            Background = 0;
//            AllowTeleport = true;
//        }

//        protected override void Init()
//        {
//            LoadMap("wServer.realm.worlds.maps.UDL.jm", MapType.Json);
//        }

//        public override World GetInstance(Client psr)
//        {
//            return Manager.AddWorld(new UndeadLair());
//        }
//    }
//}

using System;
using System.Collections.Generic;
using wServer.networking;
using wServer.networking.svrPackets;
using System.Linq;
using wServer.realm.terrain;
using wServer.realm.entities;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using wServer.realm.entities.player;

namespace wServer.realm.worlds
{
    public class UndeadLair : World
    {
        public UndeadLair()
        {
            Name = "Undead Lair";
            ClientWorldName = "dungeons.Undead_Lair";
            Dungeon = true;
            Background = 0;
            AllowTeleport = true;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.WIP_UDL.jm", MapType.Json);
        }

        public override World GetInstance(Client psr)
        {
            return Manager.AddWorld(new UndeadLair());
        }

        private readonly string[] NormalEnemies =
        {
            "Lair Skeleton", "Lair Skeleton King", "Lair Skeleton Mage", "Lair Skeleton Swordsman", "Lair Skeleton Veteran", "Lair Mummy", "Lair Mummy King", "Lair Mummy Pharaoh", "Lair Construct Giant", "Lair Construct Titan", "Lair Brown Bat", "Lair Ghost Bat", "Lair Reaper", "Lair Vampire", "Lair Vampire King", "Lair Grey Spectre", "Lair Blue Spectre", "Lair White Spectre"
        };
        private readonly string[] Gods =
        {
             "Ghost God"
        };
        private readonly string[] Bosses =
        {
            "Septavius the Ghost God"
        };
        private readonly string[] EndLoot =
        {
            "Wine Cellar Incantation", "Doom Bow", "Supply Crate Series #1", "Doom Essence"
        };
        private readonly string BasicLoot = "Potion of Wisdom";

        private bool waiting = false;
        private bool waiting2 = false;
        private bool waiting3 = false;
        private int TotalArea = 0;
        private int CurrentArea = 0;
        private int TimesCompleted = 0;
        private bool CurrentAreaSpawned = false;
        List<string> notInZoneTimers = new List<string>();


        private TileRegion[] Areas = new TileRegion[] {
            TileRegion.DungArea_1,
            TileRegion.DungArea_2,
            TileRegion.DungArea_3,
            TileRegion.DungArea_4,
            TileRegion.DungArea_5,
            TileRegion.DungArea_6,
            TileRegion.DungArea_7,
            TileRegion.DungArea_8,
            TileRegion.DungArea_9,
            TileRegion.DungArea_10,
            TileRegion.DungArea_11,
            TileRegion.DungArea_12
        };

        public bool OutOfBounds(float x, float y)
        {
            if (Map.Height >= y && Map.Width >= x && x > -1 && y > 0)
                return !(Map[(int)x, (int)y].Region == Areas[CurrentArea]);
            else
                return true;
        }

        public override void Tick(RealmTime time)
        {
            base.Tick(time);

            var listToKick = new List<Player>();

            CheckInSafeZone();

            if (CurrentArea == 12)
            {
                CurrentArea = 0;
                TimesCompleted++;
                SpawnLoot();
            }

            foreach (var i in Players.Values)
            {
                for (var h = 0; h < notInZoneTimers.Count; h++)
                {
                    var splitS = notInZoneTimers[h].Split(':');
                    if (splitS[0] == i.Name)
                    {
                        if (CurrentArea == 0 && TotalArea < 12)
                        {
                            if (splitS[1] == "11")
                                listToKick.Add(i);
                        }
                        else if (CurrentArea == 0 && TotalArea > 11)
                        {
                            if (splitS[1] == "23")
                                listToKick.Add(i);
                        }
                        else
                            if (splitS[1] == "5")
                            listToKick.Add(i);
                    }
                }
            }

            foreach (var i in listToKick)
            {
                foreach (var h in Players.Values)
                {
                    h.SendInfo($"{i.Name} has been kicked for Idling");
                }
                i.Client.Reconnect(new ReconnectPacket
                {
                    Host = "",
                    Port = Program.Settings.GetValue<int>("port"),
                    GameId = World.NEXUS_ID,
                    Name = "Nexus",
                    Key = Empty<byte>.Array,
                });
            }

            var notInZoneNames = new List<string>();

            if (!CurrentAreaSpawned)
            {
                if (!waiting)
                {
                    var totalPlayers = 0;
                    foreach (var i in Players.Values)
                    {
                        var hasTimer = false;
                        if (Map[(int)i.X, (int)i.Y].Region == Areas[CurrentArea])
                        {
                            totalPlayers++;
                        }
                        else
                        {
                            notInZoneNames.Add(i.Name);

                            if (!waiting3)
                            {
                                waiting3 = true;
                                Task.Factory.StartNew(() =>
                                {
                                    while (!worldTimer(5000)) { }

                                    waiting3 = false;

                                    for (var h = 0; h < notInZoneTimers.Count; h++)
                                    {
                                        var splitS = notInZoneTimers[h].Split(':');
                                        if (splitS[0] == i.Name)
                                        {
                                            hasTimer = true;
                                            notInZoneTimers[h] = $"{i.Name}:{int.Parse(splitS[1]) + 1}";
                                        }
                                    }
                                    if (!hasTimer)
                                        notInZoneTimers.Add($"{i.Name}:0");
                                });
                            }
                        }
                    }
                    if (totalPlayers == Players.Count && Players.Count > 0)
                    {
                        waiting = true;
                        notInZoneTimers.Clear();
                        foreach (var i in Players.Values)
                            BroadcastPacket(new NotificationPacket
                            {
                                ObjectId = i.Id,
                                Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"Area " + (CurrentArea + 1) + " begins in: 5 seconds, Total Areas Completed: " + TotalArea + "\"}}",
                                Color = new ARGB(0xFFFFFF),
                            }, null);
                        Task.Factory.StartNew(() =>
                        {
                            while (!worldTimer(5000)) { }

                            SpawnEnemies();
                            SpawnBosses();
                            waiting = false;
                            CurrentAreaSpawned = true;
                        });
                    }
                    else
                    {
                        var isOrAre = notInZoneNames.Count > 1 ? "are" : "is";
                        var usersNames = string.Join(", ", notInZoneNames.ToArray());
                        if (!waiting2)
                        {
                            waiting2 = true;
                            Task.Factory.StartNew(() =>
                            {
                                while (!worldTimer(5000)) { }

                                if (!waiting)
                                    foreach (var i in Players.Values)
                                        BroadcastPacket(new NotificationPacket
                                        {
                                            ObjectId = i.Id,
                                            Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"Area cannot begin as " + usersNames + " " + isOrAre + " not in the zone.\"}}",
                                            Color = new ARGB(0xFFFFFF),
                                        }, null);
                                waiting2 = false;
                            });
                        }
                    }
                }
            }
            else
            {
                if (!(Enemies.Count == 0)) return;
                CurrentArea++;
                TotalArea++;
                CurrentAreaSpawned = false;
            }
            notInZoneNames.Clear();
        }

        private bool worldTimer(int time)
        {
            var timer = new Stopwatch();
            timer.Start();

            var timeElapsed = false;
            while (!timeElapsed)
                if (timer.ElapsedMilliseconds > time)
                {
                    timer.Stop();
                    timeElapsed = true;
                }
            return true;
        }

        private void CheckInSafeZone()
        {
            foreach (var i in Enemies.Values)
            {
                var xloc = (int)i.X;
                var yloc = (int)i.Y;
                if (Map[xloc, yloc].Region != Areas[CurrentArea])
                {
                    //if (Map[(int)i.SpawnPoint.X, (int)i.SpawnPoint.Y].Region == Areas[CurrentArea])
                    i.Move(i.SpawnPoint.X, i.SpawnPoint.Y);
                    //else
                    //{
                    //    Random r = new Random();
                    //    var def = new List<IntPoint>();
                    //    for (var x = 0; x < Map.Width; x++)
                    //        for (var y = 0; y < Map.Height; y++)
                    //            if (Map[x, y].Region == Areas[CurrentArea])
                    //                def.Add(new IntPoint(x, y));

                    //    var tile = def[r.Next(0, def.Count)];
                    //    i.Move(tile.X, tile.Y);
                    //}
                }
            }
        }

        private void SpawnEnemies()
        {

            try
            {
                List<string> enems = new List<string>();
                Random r = new Random();

                for (int i = 0; i < (TotalArea / 8) + 2; i++)
                {
                    enems.Add(Gods[0]);
                }
                for (int i = 0; i < (TotalArea / 6) + 10; i++)
                {
                    enems.Add(NormalEnemies[r.Next(0, NormalEnemies.Length)]);
                }
                foreach (var i in enems)
                {
                    if (this == null) break;
                    if (Players.Count <= 0) break;
                    bool OutOfBoundsBool = true;
                    ushort id;
                    int xloc;
                    int yloc;
                    Entity enemy = null;
                    while (OutOfBoundsBool)
                    {
                        id = Manager.GameData.IdToObjectType[i];
                        xloc = r.Next(0, Map.Width);
                        yloc = r.Next(0, Map.Height);
                        enemy = Entity.Resolve(Manager, id);
                        enemy.Move(xloc, yloc);
                        OutOfBoundsBool = OutOfBounds(xloc, yloc);
                    }

                    EnterWorld(enemy);
                }
            }
            catch // (Exception ex)
            {
                //  Log.Error(ex);
            }
        }

        private void SpawnBosses()
        {
            List<string> enems = new List<string>();
            Random r = new Random();
            for (int i = 0; i < ((CurrentArea / 12) + 1); i++)
            {
                enems.Add(Bosses[0]);
            }
            foreach (var i in enems)
            {
                if (this == null) break;
                if (Players.Count <= 0) break;
                bool OutOfBoundsBool = true;
                ushort id;
                int xloc;
                int yloc;
                Entity enemy = null;
                while (OutOfBoundsBool)
                {
                    id = Manager.GameData.IdToObjectType[i];
                    xloc = r.Next(0, Map.Width);
                    yloc = r.Next(0, Map.Height);
                    enemy = Entity.Resolve(Manager, id);
                    enemy.Move(xloc, yloc);
                    OutOfBoundsBool = OutOfBounds(xloc, yloc);
                }

                EnterWorld(enemy);
            }
        }

        private void SpawnLoot()
        {
            List<Item> items = new List<Item>();
            Random r = new Random();

            int chance = TimesCompleted >= 10 ? (30) : (TimesCompleted * 3);

            if (r.Next(100) < chance)
            {
                for (int i = 0; i < TimesCompleted; i++)
                {
                    items.Add(Manager.GameData.Items[Manager.GameData.IdToObjectType[BasicLoot]]);
                    items.Add(Manager.GameData.Items[Manager.GameData.IdToObjectType[EndLoot[r.Next(0, EndLoot.Length)]]]);
                }
            }
            else
            {
                for (int i = 0; i < chance / 10 + 1; i++)
                    items.Add(Manager.GameData.Items[Manager.GameData.IdToObjectType[BasicLoot]]);
            }

            foreach (var j in Players)
            {
                Container container = new Container(Manager, 0x0503, 60000, false);
                for (int i = 0; i < items.Count; i++)
                {
                    container.Inventory[i] = items[i];
                }
                container.BagOwners = new[] { j.Value.Client.Account.AccountId };
                bool isInArea = false;
                int xloc = 0;
                int yloc = 0;
                while (!isInArea)
                {
                    xloc = r.Next(Map.Width);
                    yloc = r.Next(Map.Height);
                    isInArea = (Map[xloc, yloc].Region == TileRegion.Item_Spawn_Point);
                }
                container.Move(xloc, yloc);
                EnterWorld(container);
            }
        }
    }
}