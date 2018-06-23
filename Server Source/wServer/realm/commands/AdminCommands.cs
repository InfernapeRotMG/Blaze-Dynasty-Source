#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using db;
using wServer.networking;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.realm.setpieces;
using wServer.realm.worlds;

#endregion

namespace wServer.realm.commands
{
    internal class ChangePetSkinCommand : Command
    {
        public ChangePetSkinCommand() : base("cpskin", 4) { }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (string.IsNullOrEmpty(args[0]))
            {
                player.SendHelp("Usage: /cpskin <accId> <petId> <SkinId> List of Pet Skin IDS can be found here: http://pastebin.com/hbUfdnbB");
                return false;
            }
            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "UPDATE pets SET skin=@changeskin WHERE accId=@accId AND petId=@petId";
                cmd.Parameters.AddWithValue("@changeskin", args[2]);
                cmd.Parameters.AddWithValue("@petId", args[1]);
                cmd.Parameters.AddWithValue("@accId", args[0]);
                if (cmd.ExecuteNonQuery() == 0)
                {
                    player.SendInfo("Wrong AccId or PetId or SkinId. Try Again");
                }
                else
                    player.SendInfo("Successfully Changed Pet Skin");
            });
            return true;
        }
    }

    internal class InvisibleCommand : Command
    {
        public InvisibleCommand()
        : base("invisible", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.HasConditionEffect(ConditionEffectIndex.Invisible))
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Invisible,
                    DurationMS = 0
                });
                player.SendInfo("Invisible Mode Off");
            }
            else
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Invisible,
                    DurationMS = -1
                });
                player.SendInfo("Invisible Mode On");
            }
            return true;
        }
    }

    internal class DonorSpawnCommand : Command
    {
        public DonorSpawnCommand()
            : base("dspawn", 3)
        {
        }


        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Owner.Name != "Vault" && player.Client.Account.Rank <= 9)
            {
                player.SendError("You must be in your vault to spawn.");
                return false;
            }
            int num;
            if (args.Length > 0 && int.TryParse(args[0], out num)) //multi
            {
                string name = string.Join(" ", args.Skip(1).ToArray());
                World w = player.Manager.GetWorld(player.Owner.Id); //can't use Owner here, as it goes out of scope
                string announce = "Spawning " + num + " of " + name + " in 2.5 seconds . . .";
                ushort objType;
                //creates a new case insensitive dictionary based on the XmlDatas
                Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(player.Manager.GameData.IdToObjectType, StringComparer.OrdinalIgnoreCase);
                if (name.ToLower() == "vault chest" && player.Client.Account.Rank < 1)
                {
                    player.SendInfo("You are not allowed to spawn this entity.");
                    return false;
                }
                if (!icdatas.TryGetValue(name, out objType) ||
                !player.Manager.GameData.ObjectDescs.ContainsKey(objType))
                {
                    player.SendInfo("Unknown entity!");
                    return false;
                }
                int c = int.Parse(args[0]);
                if (c > 1)
                {
                    player.SendInfo("Cant spawn that many enemies. The maximum limit is only 1.");
                    return false;
                }
                w.BroadcastPacket(new NotificationPacket
                {
                    Color = new ARGB(0x00ff00),
                    ObjectId = player.Client.Player.Id,
                    Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"" + announce + "\"}}",
                }, null);
                player.Owner.Timers.Add(new WorldTimer(2500, (world, t) =>
                {
                    for (int i = 0; i < num; i++)
                    {
                        Entity entity = Entity.Resolve(player.Manager, objType);
                        if (player.Owner is Nexus)
                        {
                            entity.Move(108, 39);
                            player.Owner.EnterWorld(entity);
                        }
                        else
                        {
                            entity.Move(player.X, player.Y);
                            player.Owner.EnterWorld(entity);
                        }
                    }
                }));
                player.SendInfo("Success!");
            }
            else
            {
                string name = string.Join(" ", args);
                ushort objType;
                //creates a new case insensitive dictionary based on the XmlDatas
                Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(player.Manager.GameData.IdToObjectType, StringComparer.OrdinalIgnoreCase);
                if (name.ToLower() == "vault chest")
                {
                    player.SendInfo("You are not allowed to spawn this entity.");
                    return false;
                }
                if (!icdatas.TryGetValue(name, out objType) || !player.Manager.GameData.ObjectDescs.ContainsKey(objType))
                {
                    player.SendHelp("Usage: /spawn <entityname>");
                    return false;
                }
                World w = player.Manager.GetWorld(player.Owner.Id); //can't use Owner here, as it goes out of scope
                string announce = "Spawning " + name + " in 5 seconds . . .";
                w.BroadcastPacket(new NotificationPacket
                {
                    Color = new ARGB(0x00ff00),
                    ObjectId = player.Client.Player.Id,
                    Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"" + announce + "\"}}",
                }, null);
                player.Owner.Timers.Add(new WorldTimer(5000, (world, t) =>
                {
                    Entity entity = Entity.Resolve(player.Manager, objType);
                    if (player.Owner is Nexus)
                    {
                        entity.Move(108, 39);
                        player.Owner.EnterWorld(entity);
                    }
                    else
                    {
                        entity.Move(player.X, player.Y);
                        player.Owner.EnterWorld(entity);
                    }
                }));
            }
            return true;
        }
    }

    internal class PetMaxCommand : Command
    {
        public PetMaxCommand()
            : base("petmax", 2)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            Item feed = player.Manager.GameData.Items[0x9d4];
            for (int i = 0; i < 50; i++)
            {
                player.Pet.Feed(feed);
                player.Pet.UpdateCount++;
            }
            return true;
        }
    }

    internal class GRankCommand : Command
    {
        public GRankCommand() : base("guildrank", 10) { }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (string.IsNullOrEmpty(args[0]))
            {
                player.SendHelp("Usage: /guildrank <name> <guildRank>");
                return false;
            }
            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "UPDATE accounts SET guildRank=@guildRank WHERE name=@name";
                cmd.Parameters.AddWithValue("@guildRank", args[1]);
                cmd.Parameters.AddWithValue("@name", args[0]);
                if (cmd.ExecuteNonQuery() == 0)
                {
                    player.SendInfo("Could not change guild rank. Use 0 (Initiate), 10 (Member), 20 (Officer), 30 (Leader), or 40 (Founder)");
                }
                else
                    player.SendInfo("Guild rank successfully changed");
                log.InfoFormat(args[1] + "'s guild rank has been changed");
            });
            return true;
        }
    }

    internal class BerserkerCommand : Command
    {
        public BerserkerCommand()
        : base("berserker", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.HasConditionEffect(ConditionEffectIndex.Berserk))
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Berserk,
                    DurationMS = 0
                });
                player.SendInfo("Berserk Mode Off");
            }
            else
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Berserk,
                    DurationMS = -1
                });
                player.SendInfo("Berserk Mode On");
            }
            return true;
        }
    }

    internal class InvulnerableCommand : Command
    {
        public InvulnerableCommand()
        : base("invulnerable", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.HasConditionEffect(ConditionEffectIndex.Invulnerable))
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Invulnerable,
                    DurationMS = 0
                });
                player.SendInfo("Invulnerable Mode Off");
            }
            else
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Invulnerable,
                    DurationMS = -1
                });
                player.SendInfo("Invulnerable Mode On");
            }
            return true;
        }
    }

    internal class rankCommand : Command
    {
        public rankCommand() : base("rank", 10) { }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length > 2)
            {
                player.SendHelp("Usage: /rank <name> <rank>");
                return false;
            }
            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "UPDATE accounts SET rank=@rank WHERE name=@name";
                cmd.Parameters.AddWithValue("@name", args[0]);
                cmd.Parameters.AddWithValue("@rank", args[1]);
                if (cmd.ExecuteNonQuery() == 0)
                {
                    player.SendError(args[0] + " could not be ranked!");
                }
                else
                {
                    player.SendInfo(args[0] + " successfully ranked");
                }
            });
            return true;
        }
    }

    internal class ServerQuitCommand : Command
    {
        public ServerQuitCommand()
            : base("squit", 7)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.Client.SendPacket(new TextPacket
            {
                BubbleTime = 0,
                Stars = -1,
                Name = "@INFO",
                Text = "Server is turning off in 1 minute. Leave the server to prevent account in use!"
            });
            player.Owner.Timers.Add(new WorldTimer(30000, (world, t) =>
            {
                player.Client.SendPacket(new TextPacket
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "@INFO",
                    Text = "Server is turning off in 30 seconds. Leave the server to prevent account in use!"
                });
            }));

            player.Owner.Timers.Add(new WorldTimer(45000, (world, t) =>
            {
                player.Client.SendPacket(new TextPacket
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "@INFO",
                    Text = "Server is turning off in 15 seconds. Leave the server to prevent account in use!"
                });
            }));
            player.Owner.Timers.Add(new WorldTimer(55000, (world, t) =>
            {
                player.Client.SendPacket(new TextPacket
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "@INFO",
                    Text = "Server is turning off in 5 seconds. Leave the server to prevent account in use!"
                });
            }));
            player.Owner.Timers.Add(new WorldTimer(60000, (world, t) =>
            {
                Environment.Exit(0);
            }));
            return true;
        }
    }


    internal class AllOnlineCommand : Command
    {
        public AllOnlineCommand() : base("online", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var sb = new StringBuilder("Users online: \r\n");
            foreach (Client i in player.Manager.Clients.Values)
            {
                if (i.Stage == ProtocalStage.Disconnected || i.Player == null || i.Player.Owner == null) continue;
                sb.AppendFormat("{0}#{1}@{2}\r\n",
                    i.Account.Name,
                    i.Player.Owner.Name,
                    i.Socket.RemoteEndPoint);
            }

            player.SendInfo(sb.ToString());
            return true;
        }
    }

    internal class VanishCommand : Command
    {
        public VanishCommand() : base("vanish", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (!player.isNotVisible)
            {
                player.isNotVisible = true;
                player.Owner.PlayersCollision.Remove(player);
                if (player.Pet != null)
                    player.Owner.LeaveWorld(player.Pet);
                player.SendInfo("You're now hidden from all players!");
                return true;
            }
            player.isNotVisible = false;

            player.SendInfo("You're now visible to all players!");
            return true;
        }
    }

    internal class SaveCommand : Command
    {
        public SaveCommand() : base("save", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            foreach (Client i in player.Manager.Clients.Values)
                i.Save();

            player.SendInfo("Saved all Clients!");
            return true;
        }
    }

    internal class KickAllCommand : Command
    {
        public KickAllCommand() : base("kickall", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            int clients = 0;
            foreach (var i in player.Manager.Clients.Values.Where(i => i.Account.Name != player.Name))
            {
                i.Disconnect();
                clients++;
            }
            player.SendInfo($"Success, kicked {clients} clients");
            return true;
        }
    }

    internal class SellCommand : Command //Made by Infernape
    {
        public SellCommand() :
            base("sell", 0)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (string.IsNullOrEmpty(args[0]))
            {
                player.SendInfo("Usage: /sell <inv slot>");
                return false;
            }
            string xd = args[0].ToLower();
            switch (xd)
            {
                case "1":
                    if (player.Inventory[4] == null)
                    {
                        player.SendInfo("You cannot sell me nothing, sir.");
                        return false;
                    }
                    if (player.Inventory[4].SellPrice < 0)
                    {
                        player.SendInfo("This item doesnt have a selling price.");
                        return false;
                    }
                    player.Manager.Database.DoActionAsync(db =>
                    {
                        if (player.Inventory[4].SellPrice > 0)
                        {
                            player.SendInfo("sold item for: " + player.Inventory[4].SellPrice);
                            player.CurrentFame = player.Client.Account.Stats.Fame = db.UpdateFame(player.Client.Account, +player.Inventory[4].SellPrice);
                            player.Inventory[4] = null;
                            player.SaveToCharacter();
                            player.UpdateCount++;
                        }
                    });
                    break;
                case "2":
                    if (player.Inventory[5] == null)
                    {
                        player.SendInfo("You cannot sell me nothing, sir.");
                        return false;
                    }
                    if (player.Inventory[5].SellPrice < 0)
                    {
                        player.SendInfo("This item doesnt have a selling price.");
                        return false;
                    }
                    player.Manager.Database.DoActionAsync(db =>
                    {
                        if (player.Inventory[5].SellPrice > 0)
                        {
                            player.SendInfo("sold item for: " + player.Inventory[5].SellPrice);
                            player.CurrentFame = player.Client.Account.Stats.Fame = db.UpdateFame(player.Client.Account, +player.Inventory[5].SellPrice);
                            player.Inventory[5] = null;
                            player.SaveToCharacter();
                            player.UpdateCount++;
                        }
                    });
                    break;
                case "3":
                    if (player.Inventory[6] == null)
                    {
                        player.SendInfo("You cannot sell me nothing, sir.");
                        return false;
                    }
                    if (player.Inventory[6].SellPrice < 0)
                    {
                        player.SendInfo("This item doesnt have a selling price.");
                        return false;
                    }
                    player.Manager.Database.DoActionAsync(db =>
                    {
                        if (player.Inventory[6].SellPrice > 0)
                        {
                            player.SendInfo("sold item for: " + player.Inventory[6].SellPrice);
                            player.CurrentFame = player.Client.Account.Stats.Fame = db.UpdateFame(player.Client.Account, +player.Inventory[6].SellPrice);
                            player.Inventory[6] = null;
                            player.SaveToCharacter();
                            player.UpdateCount++;
                        }
                    });
                    break;
                case "4":
                    if (player.Inventory[7] == null)
                    {
                        player.SendInfo("You cannot sell me nothing, sir.");
                        return false;
                    }
                    if (player.Inventory[7].SellPrice < 0)
                    {
                        player.SendInfo("This item doesnt have a selling price.");
                        return false;
                    }
                    player.Manager.Database.DoActionAsync(db =>
                    {
                        if (player.Inventory[7].SellPrice > 0)
                        {
                            player.SendInfo("sold item for: " + player.Inventory[7].SellPrice);
                            player.CurrentFame = player.Client.Account.Stats.Fame = db.UpdateFame(player.Client.Account, +player.Inventory[7].SellPrice);
                            player.Inventory[7] = null;
                            player.SaveToCharacter();
                            player.UpdateCount++;
                        }
                    });
                    break;
                case "5":
                    if (player.Inventory[8] == null)
                    {
                        player.SendInfo("You cannot sell me nothing, sir.");
                        return false;
                    }
                    if (player.Inventory[8].SellPrice < 0)
                    {
                        player.SendInfo("This item doesnt have a selling price.");
                        return false;
                    }
                    player.Manager.Database.DoActionAsync(db =>
                    {
                        if (player.Inventory[8].SellPrice > 0)
                        {
                            player.SendInfo("sold item for: " + player.Inventory[8].SellPrice);
                            player.CurrentFame = player.Client.Account.Stats.Fame = db.UpdateFame(player.Client.Account, +player.Inventory[8].SellPrice);
                            player.Inventory[8] = null;
                            player.SaveToCharacter();
                            player.UpdateCount++;
                        }
                    });
                    break;
                case "6":
                    if (player.Inventory[9] == null)
                    {
                        player.SendInfo("You cannot sell me nothing, sir.");
                        return false;
                    }
                    if (player.Inventory[9].SellPrice < 0)
                    {
                        player.SendInfo("This item doesnt have a selling price.");
                        return false;
                    }
                    player.Manager.Database.DoActionAsync(db =>
                    {
                        if (player.Inventory[9].SellPrice > 0)
                        {
                            player.SendInfo("sold item for: " + player.Inventory[9].SellPrice);
                            player.CurrentFame = player.Client.Account.Stats.Fame = db.UpdateFame(player.Client.Account, +player.Inventory[9].SellPrice);
                            player.Inventory[9] = null;
                            player.SaveToCharacter();
                            player.UpdateCount++;
                        }
                    });
                    break;
                case "7":
                    if (player.Inventory[10] == null)
                    {
                        player.SendInfo("You cannot sell me nothing, sir.");
                        return false;
                    }
                    if (player.Inventory[10].SellPrice < 0)
                    {
                        player.SendInfo("This item doesnt have a selling price.");
                        return false;
                    }
                    player.Manager.Database.DoActionAsync(db =>
                    {
                        if (player.Inventory[10].SellPrice > 0)
                        {
                            player.SendInfo("sold item for: " + player.Inventory[10].SellPrice);
                            player.CurrentFame = player.Client.Account.Stats.Fame = db.UpdateFame(player.Client.Account, +player.Inventory[10].SellPrice);
                            player.Inventory[10] = null;
                            player.SaveToCharacter();
                            player.UpdateCount++;
                        }
                    });
                    break;
                case "8":
                    if (player.Inventory[11] == null)
                    {
                        player.SendInfo("You cannot sell me nothing, sir.");
                        return false;
                    }
                    if (player.Inventory[11].SellPrice < 0)
                    {
                        player.SendInfo("This item doesnt have a selling price.");
                        return false;
                    }
                    player.Manager.Database.DoActionAsync(db =>
                    {
                        if (player.Inventory[11].SellPrice > 0)
                        {
                            player.SendInfo("sold item for: " + player.Inventory[11].SellPrice);
                            player.CurrentFame = player.Client.Account.Stats.Fame = db.UpdateFame(player.Client.Account, +player.Inventory[11].SellPrice);
                            player.Inventory[11] = null;
                            player.SaveToCharacter();
                            player.UpdateCount++;
                        }
                    });
                    break;
                default:
                    player.SendInfo("Usage: /sell <inv slot>");
                    break;
            }
            return true;
        }
    }

    internal class GlandCommand : Command
    {
        public GlandCommand()
            : base("glands", 0)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Owner is Nexus || player.Owner is PetYard || player.Owner is ClothBazaar || player.Owner is Vault || player.Owner is GuildHall)
            {
                player.SendInfo("You cannot use this command here");
                return false;
            }
            string[] random = "950|960|970|980|990|1000|1010|1020|1030|1040|1050|1060|1070|1080|1090|1100|1100|1110|1120|1130|1140|1050".Split('|');
            int tplocation = new Random().Next(random.Length);
            string topdank = random[tplocation];
            int x, y;
            try
            {
                x = int.Parse(topdank);
                y = int.Parse(topdank);
            }
            catch
            {
                player.SendError("Invalid coordinates!");
                return false;
            }
            player.Move(x + 0.5f, y + 0.5f);
            if (player.Pet != null)
                player.Pet.Move(x + 0.5f, y + 0.5f);
            player.UpdateCount++;
            player.Owner.BroadcastPacket(new GotoPacket
            {
                ObjectId = player.Id,
                Position = new Position
                {
                    X = player.X,
                    Y = player.Y
                }
            }, null);
            player.ApplyConditionEffect(new ConditionEffect()
            {
                Effect = ConditionEffectIndex.Invulnerable,
                DurationMS = 2500,
            });
            player.ApplyConditionEffect(new ConditionEffect()
            {
                Effect = ConditionEffectIndex.Invisible,
                DurationMS = 2500,
            });
            return true;
        }
    }

    internal class GiftCommand : Command
    {
        public GiftCommand()
            : base("gift", 10)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 1)
            {
                player.SendHelp("Usage: /gift <Playername> <Itemname>");
                return false;
            }
            string name = string.Join(" ", args.Skip(1).ToArray()).Trim();
            var plr = player.Manager.FindPlayer(args[0]);
            ushort objType;
            Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(player.Manager.GameData.IdToObjectType,
                StringComparer.OrdinalIgnoreCase);
            if (!icdatas.TryGetValue(name, out objType))
            {
                player.SendError("Item not found, perhaps a spelling error?");
                return false;
            }
            if (!player.Manager.GameData.Items[objType].Secret || player.Client.Account.Rank >= 10)
            {
                for (int i = 0; i < plr.Inventory.Length; i++)
                    if (plr.Inventory[i] == null)
                    {
                        plr.Inventory[i] = player.Manager.GameData.Items[objType];
                        plr.UpdateCount++;
                        plr.SaveToCharacter();
                        player.SendInfo("Success sending " + name + " to " + plr.Name);
                        plr.SendInfo("You got a " + name + " from " + player.Name);
                        break;
                    }
            }
            else
            {
                player.SendError("Item failed sending to " + plr.Name + ", make sure you spelt the command right, and their name!");
                return false;
            }
            return true;
        }
    }


    internal class UnBanCommand : Command
    {
        public UnBanCommand() :
            base("unban", permLevel: 7)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var p = player.Manager.FindPlayer(args[0]);
            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "UPDATE accounts SET banned=0 WHERE id=@accId;";
                cmd.Parameters.AddWithValue("@accId", p.AccountId);
                cmd.ExecuteNonQuery();
            });

            return true;
        }
    }

    internal class MaxPlayer : Command
    {
        public MaxPlayer()
            : base("maxplayer", 10)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (string.IsNullOrEmpty(args[0]))
            {
                player.SendHelp("Usage: /maxplayer <player>");
                return false;
            }
            log.Info("{PLAYER} has just /maxed a player.");
            var plr = player.Manager.FindPlayer(args[0]);
            try
            {
                plr.Stats[0] = plr.ObjectDesc.MaxHitPoints;
                plr.Stats[1] = plr.ObjectDesc.MaxMagicPoints;
                plr.Stats[2] = plr.ObjectDesc.MaxAttack;
                plr.Stats[3] = plr.ObjectDesc.MaxDefense;
                plr.Stats[4] = plr.ObjectDesc.MaxSpeed;
                plr.Stats[5] = plr.ObjectDesc.MaxHpRegen;
                plr.Stats[6] = plr.ObjectDesc.MaxMpRegen;
                plr.Stats[7] = plr.ObjectDesc.MaxDexterity;
                plr.SaveToCharacter();
                plr.Client.Save();
                plr.UpdateCount++;
                plr.SendInfo("You have been maxed by: " + player.Name);
            }
            catch
            {
                player.SendError("Error while maxing players stats!");
                return false;
            }
            return true;
        }
    }



    internal class TestCommand : Command
    {
        public TestCommand()
            : base("t", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            Entity en = Entity.Resolve(player.Manager, "Zombie Wizard");
            en.Move(player.X, player.Y);
            player.Owner.EnterWorld(en);
            player.UpdateCount++;
            //player.Client.SendPacket(new DeathPacket
            //{
            //    AccountId = player.AccountId,
            //    CharId = player.Client.Character.CharacterId,
            //    Killer = "mountains.beholder",
            //    obf0 = 10000,
            //    obf1 = 10000
            //});
            return true;
        }
    }

    internal class ReviveCommand : Command
    {
        public ReviveCommand() : base("revive", 3) { }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (string.IsNullOrEmpty(args[0]))
            {
                player.SendHelp("Usage: /revive <accId> <fame>");
                return false;
            }
            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "UPDATE characters SET dead=0 WHERE accId=@accId AND fame=@base";
                cmd.Parameters.AddWithValue("@base", args[1]);
                cmd.Parameters.AddWithValue("@accId", args[0]);
                if (cmd.ExecuteNonQuery() == 0)
                {
                    player.SendInfo("Could not revive. Make sure you wrote it right.");
                }
                else
                    player.SendInfo("Character Successfully Revived");
            });
            return true;
        }
    }


    internal class KillME : Command
    {
        public KillME()
            : base("killme", 0)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length > 0)
            {
                player.SendHelp("Usage: /killme");
                return false;
            }
            player.HP = 0;
            player.Death(player.Name);
            return true;
        }
    }

    internal class TagCommand : Command
    {
        public TagCommand() : base("tag", 2)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (string.IsNullOrEmpty(args[0]))
            {
                player.SendHelp("******************************************************************");
                player.SendHelp("Usage: /tag <TagYouWant>");
                player.SendHelp("8 Character Limit");
                player.SendHelp("No Spaces, Ex: ''My Tag'', but ''MyTag'' is fine.");
                player.SendHelp("No Profanity");
                player.SendHelp("No tags relating/similar to a staff/donator tag");
                player.SendHelp("******************************************************************");
                return false;
            }
            if (args.Length == 1)
            {
                if (player.Client.Account.Credits < 50)
                {
                    player.SendError("Not enough credits. Credits needed: 50 or above.");
                    return false;
                }
                if (player.Client.Account.Credits >= 50)
                {
                    player.Manager.Database.DoActionAsync(db =>
                    {
                        player.Client.Account.Tag = args[0];
                        player.Credits = db.UpdateCredit(player.Client.Account, -50);
                        player.UpdateCount++;
                    });
                    int cred = player.Client.Account.Credits - 50;
                    player.SendInfo("Your tag is now: " + args[0]);
                    player.SendInfo("You now have " + cred + " gold!");
                    return true;
                }
            }
            return true;
        }
    }

    internal class CFameCommand : Command
    {
        public CFameCommand()
        : base("cfame", 2)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args[0] == "")
            {
                player.SendHelp("Usage: /cfame <Fame Amount>");
                return false;
            }
            try
            {
                int newFame = Convert.ToInt32(args[0]);
                int newXP = Convert.ToInt32(newFame.ToString() + "000");
                player.Fame = newFame;
                player.Experience = newXP;
                player.SaveToCharacter();
                player.Client.Save();
                player.UpdateCount++;
                player.SendInfo("Updated Character Fame To: " + newFame);
            }
            catch
            {
                player.SendInfo("Error Setting Fame");
                return false;
            }
            return true;
        }
    }

    internal class AutoTradeCommand : Command
    {
        public AutoTradeCommand() : base("autotrade", permLevel: 1)
        { }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            Player plr = player.Manager.FindPlayer(args[0]);
            if (plr != null && plr.Owner == player.Owner)
            {
                player.RequestTrade(time, new RequestTradePacket { Name = plr.Name });
                plr.RequestTrade(time, new RequestTradePacket { Name = player.Name });
                return true;
            }
            return true;
        }
    }

    internal class BuyCommand : Command
    {
        public BuyCommand()
            : base("buy", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /buy <Itemname>");
                return false;
            }
            string name = string.Join(" ", args.ToArray()).Trim();
            ushort objType;
            //creates a new case insensitive dictionary based on the XmlDatas
            Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(player.Manager.GameData.IdToObjectType, StringComparer.OrdinalIgnoreCase);
            if (!icdatas.TryGetValue(name, out objType))
            {
                player.SendError("Unknown type!");
                return false;
            }
            if (player.Manager.GameData.Items[objType].Secret)
            {
                player.SendHelp("You cant buy admin items.");
                return false;
            }
            if (player.Client.Account.Credits < 2500)
            {
                player.SendHelp("You do not have enough gold to buy the specific item.");
                return false;
            }
            if (!player.Manager.GameData.Items[objType].Secret || player.Client.Account.Rank >= 4)
            {
                player.Manager.Database.DoActionAsync(db =>
                {
                    for (int i = 4; i < player.Inventory.Length; i++)
                        if (player.Inventory[i] == null)
                        {
                            player.Inventory[i] = player.Manager.GameData.Items[objType];
                            player.Credits -= 2500;
                            db.UpdateCredit(player.Client.Account, -2500);
                            player.SaveToCharacter();
                            player.SendInfo("Successfully bought: " + name);
                            player.UpdateCount++;
                            break;
                        }
                });
            }
            else
            {
                player.SendError("Item cannot be bought!");
                return false;
            }
            return true;
        }
    }

    internal class SetFameCommand : Command
    {
        public SetFameCommand() : base("fame", 1) { }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {


            int MAXFAME = int.Parse(args[0]);
            if (MAXFAME >= 200 && player.Client.Account.Rank <= 2)

                if (string.IsNullOrEmpty(args[0]))
                {
                    player.SendHelp("Usage: /fame <fame ammount>");
                    return false;
                }
            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "UPDATE `stats` SET `fame`=@cre WHERE accId=@accId";
                cmd.Parameters.AddWithValue("@cre", args[0]);
                cmd.Parameters.AddWithValue("@accId", player.AccountId);
                if (cmd.ExecuteNonQuery() == 0)
                {
                    player.SendError("Error setting fame!");
                }
                else
                {
                    player.SendInfo("Success!");
                }
            });
            return true;
        }
    }

    internal class SetGoldCommand : Command
    {
        public SetGoldCommand() : base("gold", 1) { }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (string.IsNullOrEmpty(args[0]))
            {
                player.SendHelp("Usage: /gold <gold>");
                return false;
            }
            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "UPDATE `stats` SET `credits`=@cre WHERE accId=@accId";
                cmd.Parameters.AddWithValue("@cre", args[0]);
                cmd.Parameters.AddWithValue("@accId", player.AccountId);
                if (cmd.ExecuteNonQuery() == 0)
                {
                    player.SendError("Error setting gold!");
                }
                else
                {
                    player.SendInfo("Success!");
                }
            });
            return true;
        }
    }

    internal class AddGiftCodeCommand : Command
    {
        public AddGiftCodeCommand()
            : base("gcode", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(args[0]))
                    player.Manager.FindPlayer(args[0])?.Client.GiftCodeReceived("LevelUp");
                else
                    player.Client.GiftCodeReceived("LevelUp");
            }
            catch (Exception) { }
            return true;
        }
    }

    internal class posCmd : Command
    {
        public posCmd()
            : base("p", 0)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.SendInfo("X: " + (int)player.X + " - Y: " + (int)player.Y);
            return true;
        }
    }

    internal class AddWorldCommand : Command
    {
        public AddWorldCommand()
            : base("addworld", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            Task.Factory.StartNew(() => GameWorld.AutoName(1, true)).ContinueWith(_ => player.Manager.AddWorld(_.Result), TaskScheduler.Default);
            return true;
        }
    }

    internal class LeftToMax : Command
    {
        public LeftToMax()
            : base("lefttomax", 0)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            int Hp = player.ObjectDesc.MaxHitPoints - player.Stats[0];
            int Mp = player.ObjectDesc.MaxMagicPoints - player.Stats[1];
            int Atk = player.ObjectDesc.MaxAttack - player.Stats[2];
            int Def = player.ObjectDesc.MaxDefense - player.Stats[3];
            int Spd = player.ObjectDesc.MaxSpeed - player.Stats[4];
            int Vit = player.ObjectDesc.MaxHpRegen - player.Stats[5];
            int Wis = player.ObjectDesc.MaxMpRegen - player.Stats[6];
            int Dex = player.ObjectDesc.MaxDexterity - player.Stats[7];
            player.SendInfo(Hp / 5 + " Till maxed Health");
            player.SendInfo(Mp / 5 + " Till maxed Mana");
            player.SendInfo(Atk + " Till maxed Attack");
            player.SendInfo(Def + " Till maxed Defense");
            player.SendInfo(Spd + " Till maxed Speed");
            player.SendInfo(Vit + " Till maxed Vitality");
            player.SendInfo(Wis + " Till maxed Wisdom");
            player.SendInfo(Dex + " Till maxed Dexterity");
            return true;
        }
    }

    internal class SpawnCommand : Command
    {
        public SpawnCommand()
            : base("spawn", 10)
        {
        }


        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            int num;
            if (args.Length > 0 && int.TryParse(args[0], out num)) //multi
            {
                string name = string.Join(" ", args.Skip(1).ToArray());
                World w = player.Manager.GetWorld(player.Owner.Id); //can't use Owner here, as it goes out of scope
                string announce = "Spawning " + num + " of " + name + " in 2.5 seconds . . .";
                ushort objType;
                //creates a new case insensitive dictionary based on the XmlDatas
                Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(player.Manager.GameData.IdToObjectType, StringComparer.OrdinalIgnoreCase);
                if (name.ToLower() == "vault chest" && player.Client.Account.Rank < 2)
                {
                    player.SendInfo("You are not allowed to spawn this entity.");
                    return false;
                }
                if (!icdatas.TryGetValue(name, out objType) ||
                !player.Manager.GameData.ObjectDescs.ContainsKey(objType))
                {
                    player.SendInfo("Unknown entity!");
                    return false;
                }
                int c = int.Parse(args[0]);
                if (c > 100)
                {
                    player.SendInfo("Cant spawn that many enemies.");
                    return false;
                }
                w.BroadcastPacket(new NotificationPacket
                {
                    Color = new ARGB(0x00ff00),
                    ObjectId = player.Client.Player.Id,
                    Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"" + announce + "\"}}",
                }, null);
                player.Owner.Timers.Add(new WorldTimer(2500, (world, t) =>
                {
                    for (int i = 0; i < num; i++)
                    {
                        Entity entity = Entity.Resolve(player.Manager, objType);
                        if (player.Owner is Nexus)
                        {
                            entity.Move(108, 39);
                            player.Owner.EnterWorld(entity);
                        }
                        else
                        {
                            entity.Move(player.X, player.Y);
                            player.Owner.EnterWorld(entity);
                        }
                    }
                }));
                player.SendInfo("Success!");
            }
            else
            {
                string name = string.Join(" ", args);
                ushort objType;
                //creates a new case insensitive dictionary based on the XmlDatas
                Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(player.Manager.GameData.IdToObjectType, StringComparer.OrdinalIgnoreCase);
                if (name.ToLower() == "vault chest")
                {
                    player.SendInfo("You are not allowed to spawn this entity.");
                    return false;
                }
                if (!icdatas.TryGetValue(name, out objType) || !player.Manager.GameData.ObjectDescs.ContainsKey(objType))
                {
                    player.SendHelp("Usage: /spawn <entityname>");
                    return false;
                }
                World w = player.Manager.GetWorld(player.Owner.Id); //can't use Owner here, as it goes out of scope
                string announce = "Spawning " + name + " in 5 seconds . . .";
                w.BroadcastPacket(new NotificationPacket
                {
                    Color = new ARGB(0x00ff00),
                    ObjectId = player.Client.Player.Id,
                    Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"" + announce + "\"}}",
                }, null);
                player.Owner.Timers.Add(new WorldTimer(5000, (world, t) =>
                {
                    Entity entity = Entity.Resolve(player.Manager, objType);
                    if (player.Owner is Nexus)
                    {
                        entity.Move(108, 39);
                        player.Owner.EnterWorld(entity);
                    }
                    else
                    {
                        entity.Move(player.X, player.Y);
                        player.Owner.EnterWorld(entity);
                    }
                }));
            }
            return true;
        }
    }

    internal class AddEffCommand : Command
    {
        public AddEffCommand()
            : base("addeff", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /addeff <Effectname or Effectnumber>");
                return false;
            }
            try
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = (ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), args[0].Trim(), true),
                    DurationMS = -1
                });
                {
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Invalid effect!");
                return false;
            }
            return true;
        }
    }

    internal class RemoveEffCommand : Command
    {
        public RemoveEffCommand()
            : base("remeff", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /remeff <Effectname or Effectnumber>");
                return false;
            }
            try
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = (ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), args[0].Trim(), true),
                    DurationMS = 0
                });
                player.SendInfo("Success!");
            }
            catch
            {
                player.SendError("Invalid effect!");
                return false;
            }
            return true;
        }
    }

    internal class GiveCommand : Command
    {
        public GiveCommand()
            : base("give", permLevel: 2)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /give <Itemname>");
                return false;
            }
            string name = string.Join(" ", args.ToArray()).Trim();
            ushort objType;
            //creates a new case insensitive dictionary based on the XmlDatas
            Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(player.Manager.GameData.IdToObjectType,
                StringComparer.OrdinalIgnoreCase);
            if (!icdatas.TryGetValue(name, out objType))
            {
                player.SendError("Unknown type!");
                return false;
            }
            if (!player.Manager.GameData.Items[objType].Secret || player.Client.Account.Rank >= 4)
            {
                for (int i = 0; i < player.Inventory.Length; i++)
                    if (player.Inventory[i] == null)
                    {
                        player.Inventory[i] = player.Manager.GameData.Items[objType];
                        player.UpdateCount++;
                        player.SaveToCharacter();
                        player.SendInfo("Success!");
                        break;
                    }
            }
            else
            {
                player.SendError("Item cannot be given!");
                return false;
            }
            return true;
        }
    }

    internal class TpCommand : Command
    {
        public TpCommand()
            : base("tp", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0 || args.Length == 1)
            {
                player.SendHelp("Usage: /tp <X coordinate> <Y coordinate>");
            }
            else
            {
                int x, y;
                try
                {
                    x = int.Parse(args[0]);
                    y = int.Parse(args[1]);
                }
                catch
                {
                    player.SendError("Invalid coordinates!");
                    return false;
                }
                player.Move(x + 0.5f, y + 0.5f);
                if (player.Pet != null)
                    player.Pet.Move(x + 0.5f, y + 0.5f);
                player.UpdateCount++;
                player.Owner.BroadcastPacket(new GotoPacket
                {
                    ObjectId = player.Id,
                    Position = new Position
                    {
                        X = player.X,
                        Y = player.Y
                    }
                }, null);
            }
            return true;
        }
    }

    class KillAll : Command
    {
        public KillAll() : base("killAll", permLevel: 8)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var iterations = 0;
            var lastKilled = -1;
            var killed = 0;

            var mobName = args.Aggregate((s, a) => string.Concat(s, " ", a));
            while (killed != lastKilled)
            {
                lastKilled = killed;
                foreach (var i in player.Owner.Enemies.Values.Where(e =>
                    e.ObjectDesc?.ObjectId != null && e.ObjectDesc.ObjectId.ContainsIgnoreCase(mobName)))
                {
                    i.Death(time);
                    killed++;
                }
                if (++iterations >= 5)
                    break;
            }

            player.SendInfo($"{killed} enemy killed!");
            return true;
        }
    }

    internal class Kick : Command
    {
        public Kick()
            : base("kick", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /kick <playername>");
                return false;
            }
            try
            {
                foreach (KeyValuePair<int, Player> i in player.Owner.Players)
                {
                    if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
                    {
                        player.SendInfo("Player Disconnected");
                        i.Value.Client.Disconnect();
                    }
                }
            }
            catch
            {
                player.SendError("Cannot kick!");
                return false;
            }
            return true;
        }
    }

    internal class Mute : Command
    {
        public Mute()
            : base("mute", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /mute <playername>");
                return false;
            }
            try
            {
                foreach (KeyValuePair<int, Player> i in player.Owner.Players)
                {
                    if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
                    {
                        i.Value.Muted = true;
                        i.Value.Manager.Database.DoActionAsync(db => db.MuteAccount(i.Value.AccountId));
                        player.SendInfo("Player Muted.");
                    }
                }
            }
            catch
            {
                player.SendError("Cannot mute!");
                return false;
            }
            return true;
        }
    }

    internal class IPCommand : Command
    {
        public IPCommand()
            : base("ip", 6)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var plr = player.Manager.FindPlayer(args[0]);
            var sb = new StringBuilder(plr.Name + "'s IP: ");
            sb.AppendFormat("{0}",
                plr.Client.Socket.RemoteEndPoint);
            player.SendInfo(sb.ToString());
            return true;
        }
    }

    internal class Max : Command
    {
        public Max()
            : base("max", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                player.Stats[0] = player.ObjectDesc.MaxHitPoints;
                player.Stats[1] = player.ObjectDesc.MaxMagicPoints;
                player.Stats[2] = player.ObjectDesc.MaxAttack;
                player.Stats[3] = player.ObjectDesc.MaxDefense;
                player.Stats[4] = player.ObjectDesc.MaxSpeed;
                player.Stats[5] = player.ObjectDesc.MaxHpRegen;
                player.Stats[6] = player.ObjectDesc.MaxMpRegen;
                player.Stats[7] = player.ObjectDesc.MaxDexterity;
                player.SaveToCharacter();
                player.Client.Save();
                player.UpdateCount++;
                player.SendInfo("Success");
            }
            catch
            {
                player.SendError("Error while maxing stats");
                return false;
            }
            return true;
        }
    }

    internal class UnMute : Command
    {
        public UnMute()
            : base("unmute", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /unmute <playername>");
                return false;
            }
            try
            {
                foreach (KeyValuePair<int, Player> i in player.Owner.Players)
                {
                    if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
                    {
                        i.Value.Muted = true;
                        i.Value.Manager.Database.DoActionAsync(db => db.UnmuteAccount(i.Value.AccountId));
                        player.SendInfo("Player Unmuted.");
                    }
                }
            }
            catch
            {
                player.SendError("Cannot unmute!");
                return false;
            }
            return true;
        }
    }

    internal class OryxSay : Command
    {
        public OryxSay()
            : base("osay", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /oryxsay <saytext>");
                return false;
            }
            string saytext = string.Join(" ", args);
            player.SendEnemy("Oryx the Mad God", saytext);
            return true;
        }
    }

    internal class SWhoCommand : Command //get all players from all worlds (this may become too large!)
    {
        public SWhoCommand()
            : base("swho", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            StringBuilder sb = new StringBuilder("All conplayers: ");

            foreach (KeyValuePair<int, World> w in player.Manager.Worlds)
            {
                World world = w.Value;
                if (w.Key != 0)
                {
                    Player[] copy = world.Players.Values.ToArray();
                    if (copy.Length != 0)
                    {
                        for (int i = 0; i < copy.Length; i++)
                        {
                            sb.Append(copy[i].Name);
                            sb.Append(", ");
                        }
                    }
                }
            }
            string fixedString = sb.ToString().TrimEnd(',', ' '); //clean up trailing ", "s

            player.SendInfo(fixedString);
            return true;
        }
    }

    internal class Announcement : Command
    {
        public Announcement()
            : base("announce", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /announce <saytext>");
                return false;
            }
            string saytext = string.Join(" ", args);

            foreach (Client i in player.Manager.Clients.Values)
            {
                i.SendPacket(new TextPacket
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "@ANNOUNCEMENT",
                    Text = " " + saytext
                });
            }
            return true;
        }
    }

    internal class Summon : Command
    {
        public Summon()
            : base("summon", 10)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Owner is Vault || player.Owner is PetYard)
            {
                player.SendInfo("You cant summon in this world.");
                return false;
            }
            foreach (KeyValuePair<string, Client> i in player.Manager.Clients)
            {
                if (i.Value.Player.Name.EqualsIgnoreCase(args[0]))
                {
                    Packet pkt;
                    if (i.Value.Player.Owner == player.Owner)
                    {
                        i.Value.Player.Move(player.X, player.Y);
                        pkt = new GotoPacket
                        {
                            ObjectId = i.Value.Player.Id,
                            Position = new Position(player.X, player.Y)
                        };
                        i.Value.Player.UpdateCount++;
                        player.SendInfo("Player summoned!");
                    }
                    else
                    {
                        pkt = new ReconnectPacket
                        {
                            GameId = player.Owner.Id,
                            Host = "",
                            IsFromArena = false,
                            Key = player.Owner.PortalKey,
                            KeyTime = -1,
                            Name = player.Owner.Name,
                            Port = -1
                        };
                        player.SendInfo("Player will connect to you now!");
                    }

                    i.Value.SendPacket(pkt);

                    return true;
                }
            }
            player.SendError(string.Format("Player '{0}' could not be found!", args));
            return false;
        }
    }

    internal class KillPlayerCommand : Command
    {
        public KillPlayerCommand()
            : base("kill", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            foreach (Client i in player.Manager.Clients.Values)
            {
                if (i.Account.Name.EqualsIgnoreCase(args[0]))
                {
                    i.Player.HP = 0;
                    i.Player.Death("Admin");
                    player.SendInfo("Player killed!");
                    return true;
                }
            }
            player.SendError(string.Format("Player '{0}' could not be found!", args));
            return false;
        }
    }

    internal class RestartCommand : Command
    {
        public RestartCommand()
            : base("restart", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                foreach (KeyValuePair<int, World> w in player.Manager.Worlds)
                {
                    World world = w.Value;
                    if (w.Key != 0)
                    {
                        world.BroadcastPacket(new TextPacket
                        {
                            Name = "@ANNOUNCEMENT",
                            Stars = -1,
                            BubbleTime = 0,
                            Text =
                                "Server restarting soon. Please be ready to disconnect. Estimated server down time: 30 Seconds - 1 Minute. " +
                                "Disconnect now to prevent Account in use."
                        }, null);
                    }
                }
            }
            catch
            {
                player.SendError("Cannot say that in announcement!");
                return false;
            }
            return true;
        }
    }

    //class VitalityCommand : ICommand
    //{
    //    public string Command { get { return "vit"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /vit <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[5] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class DefenseCommand : ICommand
    //{
    //    public string Command { get { return "def"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /def <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[3] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class AttackCommand : ICommand
    //{
    //    public string Command { get { return "att"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /att <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[2] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class DexterityCommand : ICommand
    //{
    //    public string Command { get { return "dex"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /dex <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[7] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class LifeCommand : ICommand
    //{
    //    public string Command { get { return "hp"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /hp <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[0] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class ManaCommand : ICommand
    //{
    //    public string Command { get { return "mp"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /mp <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[1] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class SpeedCommand : ICommand
    //{
    //    public string Command { get { return "spd"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /spd <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[4] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class WisdomCommand : ICommand
    //{
    //    public string Command { get { return "wis"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /spd <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[6] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    internal class BanCommand : Command
    {
        public BanCommand()
            : base("ban", permLevel: 2)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /ban <username>");
            }
            try
            {
                using (Database dbx = new Database())
                {
                    var cmd = dbx.CreateQuery();
                    cmd.CommandText = "UPDATE accounts SET banned=1, rank=0 WHERE name=@name";
                    cmd.Parameters.AddWithValue("@name", args[0]);
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        player.SendInfo("Could not ban");
                        return false;
                    }
                    else
                    {
                        foreach (var i in player.Owner.Players)
                        {
                            if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
                            {
                                i.Value.Client.Disconnect();
                                player.SendInfo("Account successfully Banned");
                                log.InfoFormat(args[0] + " was Banned.");
                                return true;
                            }
                        }
                    }
                }
            }
            catch
            {
                player.SendInfo("Could not ban");
                return false;
            }
            return true;
        }
    }

    //class UnBan : ICommand
    //{
    //    public string Command { get { return "unban"; } }
    //    public int RequiredRank { get { return 4; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        if (args.Length == 0)
    //        {
    //            player.SendHelp("Usage: /unban <username>");
    //        }
    //        try
    //        {
    //            using (Database dbx = new Database())
    //            {
    //                var cmd = dbx.CreateQuery();
    //                cmd.CommandText = "UPDATE accounts SET banned=0, rank=1 WHERE name=@name";
    //                cmd.Parameters.AddWithValue("@name", args[0]);
    //                if (cmd.ExecuteNonQuery() == 0)
    //                {
    //                    player.SendInfo("Could not unban");
    //                }
    //                else
    //                {
    //                    player.SendInfo("Account successfully Unbanned");
    //                    log.InfoFormat(args[1] + " was Unbanned.");

    //                }
    //            }
    //        }
    //        catch
    //        {
    //            player.SendInfo("Could not unban, please unban in database");
    //        }
    //    }
    //}

    //class Rank : ICommand
    //{
    //    public string Command { get { return "rank"; } }
    //    public int RequiredRank { get { return 4; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        if (args.Length < 2)
    //        {
    //            player.SendHelp("Usage: /rank <username> <number>\n0: Player\n1: Donator\n2: Game Master\n3: Developer\n4: Head Developer\n5: Admin");
    //        }
    //        else
    //        {
    //            try
    //            {
    //                using (Database dbx = new Database())
    //                {
    //                    var cmd = dbx.CreateQuery();
    //                    cmd.CommandText = "UPDATE accounts SET rank=@rank WHERE name=@name";
    //                    cmd.Parameters.AddWithValue("@rank", args[1]);
    //                    cmd.Parameters.AddWithValue("@name", args[0]);
    //                    if (cmd.ExecuteNonQuery() == 0)
    //                    {
    //                        player.SendInfo("Could not change rank");
    //                    }
    //                    else
    //                        player.SendInfo("Account rank successfully changed");
    //                }
    //            }
    //            catch
    //            {
    //                player.SendInfo("Could not change rank, please change rank in database");
    //            }
    //        }
    //    }
    //}
    //class GuildRank : ICommand
    //{
    //    public string Command { get { return "grank"; } }
    //    public int RequiredRank { get { return 4; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        if (args.Length < 2)
    //        {
    //            player.SendHelp("Usage: /grank <username> <number>");
    //        }
    //        else
    //        {
    //            try
    //            {
    //                using (Database dbx = new Database())
    //                {
    //                    var cmd = dbx.CreateQuery();
    //                    cmd.CommandText = "UPDATE accounts SET guildRank=@guildRank WHERE name=@name";
    //                    cmd.Parameters.AddWithValue("@guildRank", args[1]);
    //                    cmd.Parameters.AddWithValue("@name", args[0]);
    //                    if (cmd.ExecuteNonQuery() == 0)
    //                    {
    //                        player.SendInfo("Could not change guild rank. Use 10, 20, 30, 40, or 50 (invisible)");
    //                    }
    //                    else
    //                        player.SendInfo("Guild rank successfully changed");
    //                    log.InfoFormat(args[1] + "'s guild rank has been changed");
    //                }
    //            }
    //            catch
    //            {
    //                player.SendInfo("Could not change rank, please change rank in database");
    //            }
    //        }
    //    }
    //}
    //class ChangeGuild : ICommand
    //{
    //    public string Command { get { return "setguild"; } }
    //    public int RequiredRank { get { return 4; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        if (args.Length < 2)
    //        {
    //            player.SendHelp("Usage: /setguild <username> <guild id>");
    //        }
    //        else
    //        {
    //            try
    //            {
    //                using (Database dbx = new Database())
    //                {
    //                    var cmd = dbx.CreateQuery();
    //                    cmd.CommandText = "UPDATE accounts SET guild=@guild WHERE name=@name";
    //                    cmd.Parameters.AddWithValue("@guild", args[1]);
    //                    cmd.Parameters.AddWithValue("@name", args[0]);
    //                    if (cmd.ExecuteNonQuery() == 0)
    //                    {
    //                        player.SendInfo("Could not change guild.");
    //                    }
    //                    else
    //                        player.SendInfo("Guild successfully changed");
    //                    log.InfoFormat(args[1] + "'s guild has been changed");
    //                }
    //            }
    //            catch
    //            {
    //                player.SendInfo("Could not change guild, please change in database.                                Use /setguild <username> <guild id>");
    //            }
    //        }
    //    }
    //}
    internal class AccIdCommand : Command
    {
        public AccIdCommand()
            : base("accid", permLevel: 5)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (string.IsNullOrEmpty(args[0]))
            {
                player.SendHelp("Usage: /accid <player>");
                return false;
            }
            var plr = player.Manager.FindPlayer(args[0]);
            player.SendInfo("Account ID of " + plr.Name + " : " + plr.AccountId);
            return true;
        }
    }

    internal class GGoldCommand : Command
    {
        public GGoldCommand()
            : base("giftgold", 10)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (string.IsNullOrEmpty(args[0]))
            {
                player.SendHelp("Usage: /giftgold <accId> <gold>");
                return false;
            }
            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "UPDATE `stats` SET `credits`=@cre WHERE accId=@accId";
                cmd.Parameters.AddWithValue("@cre", args[1]);
                cmd.Parameters.AddWithValue("@accId", args[0]);
                if (cmd.ExecuteNonQuery() == 0)
                {
                    player.SendError("Error sending gold!");
                }
                else
                {
                    player.SendInfo(args[0] + " 's Gold Was Replaced Successfully");
                }
            });
            return true;
        }
    }

    internal class RogueCommand : Command
    {
        public RogueCommand() : base("rogue", 0)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            for (int i = 0; i < player.Inventory.Length; i++)
            {
                player.Inventory[4] = player.Manager.GameData.Items[0xa8a]; // Agate
                player.Inventory[5] = player.Manager.GameData.Items[0xae1]; // Twlight
                player.Inventory[6] = player.Manager.GameData.Items[0xa90]; // Griffon
                player.Inventory[7] = player.Manager.GameData.Items[0xac5]; // Para HP
                player.UpdateCount++;
            }
            player.SendInfo("Set Given");
            return true;
        }
    }
    internal class ArcherCommand : Command
    {
        public ArcherCommand() : base("archer", 0)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            for (int i = 0; i < player.Inventory.Length; i++)
            {
                player.Inventory[4] = player.Manager.GameData.Items[0xa8d]; // BBow
                player.Inventory[5] = player.Manager.GameData.Items[0xa65]; // Gquiver
                player.Inventory[6] = player.Manager.GameData.Items[0xa90]; // Griffon
                player.Inventory[7] = player.Manager.GameData.Items[0xac5]; // Para HP
                player.UpdateCount++;
            }
            player.SendInfo("Set Given");
            return true;
        }
    }
    internal class WizardCommand : Command
    {
        public WizardCommand() : base("wizard", 0)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            for (int i = 0; i < player.Inventory.Length; i++)
            {
                player.Inventory[4] = player.Manager.GameData.Items[0xaa2]; // Astral
                player.Inventory[5] = player.Manager.GameData.Items[0xa30]; // MNova
                player.Inventory[6] = player.Manager.GameData.Items[0xa96]; // Elder
                player.Inventory[7] = player.Manager.GameData.Items[0xac5]; // Para HP
                player.UpdateCount++;
            }
            player.SendInfo("Set Given");
            return true;
        }
    }
    internal class PriestCommand : Command
    {
        public PriestCommand() : base("priest", 0)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            for (int i = 0; i < player.Inventory.Length; i++)
            {
                player.Inventory[4] = player.Manager.GameData.Items[0xa87]; // Ancient Warning
                player.Inventory[5] = player.Manager.GameData.Items[0xa5b]; // T5 Tome
                player.Inventory[6] = player.Manager.GameData.Items[0xa96]; // Elder
                player.Inventory[7] = player.Manager.GameData.Items[0xac5]; // Para HP
                player.UpdateCount++;
            }
            player.SendInfo("Set Given");
            return true;
        }
    }
    internal class WarriorCommand : Command
    {
        public WarriorCommand() : base("warrior", 0)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            for (int i = 0; i < player.Inventory.Length; i++)
            {
                player.Inventory[4] = player.Manager.GameData.Items[0xa47]; // Sky
                player.Inventory[5] = player.Manager.GameData.Items[0xa6b]; // Ghelm
                player.Inventory[6] = player.Manager.GameData.Items[0xa93]; // Abyssal
                player.Inventory[7] = player.Manager.GameData.Items[0xac5]; // Para HP
                player.UpdateCount++;
            }
            player.SendInfo("Set Given");
            return true;
        }
    }
    internal class KnightCommand : Command
    {
        public KnightCommand() : base("knight", 0)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            for (int i = 0; i < player.Inventory.Length; i++)
            {
                player.Inventory[4] = player.Manager.GameData.Items[0xa47]; // Sky
                player.Inventory[5] = player.Manager.GameData.Items[0xa0c]; // Mith Shield
                player.Inventory[6] = player.Manager.GameData.Items[0xa93]; // Abyssal
                player.Inventory[7] = player.Manager.GameData.Items[0xac5]; // Para HP
                player.UpdateCount++;
            }
            player.SendInfo("Set Given");
            return true;
        }
    }
    internal class PaladinCommand : Command
    {
        public PaladinCommand() : base("paladin", 0)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            for (int i = 0; i < player.Inventory.Length; i++)
            {
                player.Inventory[4] = player.Manager.GameData.Items[0xa47]; // Sky
                player.Inventory[5] = player.Manager.GameData.Items[0xa55]; // T5 Seal
                player.Inventory[6] = player.Manager.GameData.Items[0xa93]; // Abyssal
                player.Inventory[7] = player.Manager.GameData.Items[0xac5]; // Para HP
                player.UpdateCount++;
            }
            player.SendInfo("Set Given");
            return true;
        }
    }
    internal class AssassinCommand : Command
    {
        public AssassinCommand() : base("assassin", 0)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            for (int i = 0; i < player.Inventory.Length; i++)
            {
                player.Inventory[4] = player.Manager.GameData.Items[0xa8a]; // Agate
                player.Inventory[5] = player.Manager.GameData.Items[0xaa8]; // Nightwing
                player.Inventory[6] = player.Manager.GameData.Items[0xa90]; // Griffon
                player.Inventory[7] = player.Manager.GameData.Items[0xac5]; // Para HP
                player.UpdateCount++;
            }
            player.SendInfo("Set Given");
            return true;
        }
    }
    internal class NecromanserCommand : Command
    {
        public NecromanserCommand() : base("necromanser", 0)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            for (int i = 0; i < player.Inventory.Length; i++)
            {
                player.Inventory[4] = player.Manager.GameData.Items[0xaa2]; // Astral
                player.Inventory[5] = player.Manager.GameData.Items[0xaaf]; // Lifedrinker
                player.Inventory[6] = player.Manager.GameData.Items[0xa96]; // Elder
                player.Inventory[7] = player.Manager.GameData.Items[0xac5]; // Para HP
                player.UpdateCount++;
            }
            player.SendInfo("Set Given");
            return true;
        }
    }
    internal class HuntressCommand : Command
    {
        public HuntressCommand() : base("huntress", 0)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            for (int i = 0; i < player.Inventory.Length; i++)
            {
                player.Inventory[4] = player.Manager.GameData.Items[0xa8d]; // BBow
                player.Inventory[5] = player.Manager.GameData.Items[0xab6]; // Dstalker
                player.Inventory[6] = player.Manager.GameData.Items[0xa90]; // Griffon
                player.Inventory[7] = player.Manager.GameData.Items[0xac5]; // Para HP
                player.UpdateCount++;
            }
            player.SendInfo("Set Given");
            return true;
        }
    }
    internal class MysticCommand : Command
    {
        public MysticCommand() : base("mystic", 0)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            for (int i = 0; i < player.Inventory.Length; i++)
            {
                player.Inventory[4] = player.Manager.GameData.Items[0xaa2]; // Astral
                player.Inventory[5] = player.Manager.GameData.Items[0xa46]; // Banishment Orb
                player.Inventory[6] = player.Manager.GameData.Items[0xa96]; // Elder
                player.Inventory[7] = player.Manager.GameData.Items[0xac5]; // Para HP
                player.UpdateCount++;
            }
            player.SendInfo("Set Given");
            return true;
        }
    }
    internal class TricksterCommand : Command
    {
        public TricksterCommand() : base("trickster", 0)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            for (int i = 0; i < player.Inventory.Length; i++)
            {
                player.Inventory[4] = player.Manager.GameData.Items[0xa8a]; // Agate
                player.Inventory[5] = player.Manager.GameData.Items[0xb20]; // Phantoms
                player.Inventory[6] = player.Manager.GameData.Items[0xa90]; // Griffon
                player.Inventory[7] = player.Manager.GameData.Items[0xac5]; // Para HP
                player.UpdateCount++;
            }
            player.SendInfo("Set Given");
            return true;
        }
    }
    internal class SorcererCommand : Command
    {
        public SorcererCommand() : base("Sorcerer", 0)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            for (int i = 0; i < player.Inventory.Length; i++)
            {
                player.Inventory[4] = player.Manager.GameData.Items[0xa87]; // Ancient Warning
                player.Inventory[5] = player.Manager.GameData.Items[0xb32]; // Skybolt
                player.Inventory[6] = player.Manager.GameData.Items[0xa96]; // Elder
                player.Inventory[7] = player.Manager.GameData.Items[0xac5]; // Para HP
                player.UpdateCount++;
            }
            player.SendInfo("Set Given");
            return true;
        }
    }
    internal class NinjaCommand : Command
    {
        public NinjaCommand() : base("ninja", 0)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            for (int i = 0; i < player.Inventory.Length; i++)
            {
                player.Inventory[4] = player.Manager.GameData.Items[0xc4f]; // Muramasa
                player.Inventory[5] = player.Manager.GameData.Items[0xc58]; // Ice Star
                player.Inventory[6] = player.Manager.GameData.Items[0xa90]; // Griffon
                player.Inventory[7] = player.Manager.GameData.Items[0xac5]; // Para HP
                player.UpdateCount++;
            }
            player.SendInfo("Set Given");
            return true;
        }
    }

    internal class SummonAll : Command
    {
        public SummonAll()
            : base("summonall", 10)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            foreach (var i in player.Manager.Clients.Values)
            {

                if (i.Player.Owner == player.Owner)
                {
                    i.Player.Move(player.X, player.Y);
                    i.SendPacket(new GotoPacket
                    {
                        ObjectId = i.Player.Id,
                        Position = new Position(player.X, player.Y)
                    });
                    i.Player.UpdateCount++;
                    player.SendInfo("Players summoned!");
                }
                else
                {
                    i.SendPacket(new ReconnectPacket
                    {
                        GameId = player.Owner.Id,
                        Host = "",
                        IsFromArena = false,
                        Key = player.Owner.PortalKey,
                        KeyTime = -1,
                        Name = player.Owner.Name,
                        Port = -1
                    });
                    player.SendInfo("Players will connect to you now!");
                    return true;
                }
            }
            player.SendError("Players info could not be found!");
            return false;
        }
    }


    internal class GiftFameCommand : Command
    {
        public GiftFameCommand()
            : base("giftfame", permLevel: 8)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (string.IsNullOrEmpty(args[0]))
            {
                player.SendHelp("Usage: /giftfame <accId> <fame>");
                return false;
            }
            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "UPDATE `stats` SET `fame`=@cre WHERE accId=@accId";
                cmd.Parameters.AddWithValue("@cre", args[1]);
                cmd.Parameters.AddWithValue("@accId", args[0]);
                if (cmd.ExecuteNonQuery() == 0)
                {
                    player.SendError("Error sending fame!");
                }
                else
                {
                    player.SendInfo(args[0] + " 's Fame Was Replaced Successfully");
                }
            });
            return true;
        }
    }


    internal class TqCommand : Command
    {
        public TqCommand()
            : base("tq", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Quest == null)
            {
                player.SendError("Player does not have a quest!");
                return false;
            }
            player.Move(player.Quest.X + 0.5f, player.Quest.Y + 0.5f);
            if (player.Pet != null)
                player.Pet.Move(player.Quest.X + 0.5f, player.Quest.Y + 0.5f);
            player.UpdateCount++;
            player.Owner.BroadcastPacket(new GotoPacket
            {
                ObjectId = player.Id,
                Position = new Position
                {
                    X = player.Quest.X,
                    Y = player.Quest.Y
                }
            }, null);
            player.SendInfo("Success!");
            return true;
        }
    }

    //class GodMode : ICommand
    //{
    //    public string Command { get { return "god"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        if (player.HasConditionEffect(ConditionEffects.Invincible))
    //        {
    //            player.ApplyConditionEffect(new ConditionEffect()
    //            {
    //                Effect = ConditionEffectIndex.Invincible,
    //                DurationMS = 0
    //            });
    //            player.SendInfo("Godmode Off");
    //        }
    //        else
    //        {

    //            player.ApplyConditionEffect(new ConditionEffect()
    //            {
    //                Effect = ConditionEffectIndex.Invincible,
    //                DurationMS = -1
    //            });
    //            player.SendInfo("Godmode On");
    //        }
    //    }
    //}
    internal class StarCommand : Command
    {
        public StarCommand()
         : base("stars", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /stars <ammount>");
                    return false;
                }
                else if (args.Length == 1)
                {
                    player.Client.Player.Stars = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
                return false;
            }
            return false;
        }
    }

    internal class LevelCommand : Command
    {
        public LevelCommand()
            : base("level", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /level <ammount>");
                    return false;
                }
                if (args.Length == 1)
                {
                    player.Client.Character.Level = int.Parse(args[0]);
                    player.Client.Player.Level = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
                return false;
            }
            return true;
        }
    }

    internal class VisitCommand : Command
    {
        public VisitCommand()
            : base("visit", 10)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {


            foreach (KeyValuePair<string, Client> i in player.Manager.Clients)
            {
                if (i.Value.Player.Name.EqualsIgnoreCase(args[0]))
                {
                    Packet pkt;
                    if (i.Value.Player.Owner == player.Owner)
                    {
                        player.Move(i.Value.Player.X, i.Value.Player.Y);
                        pkt = new GotoPacket
                        {
                            ObjectId = player.Id,
                            Position = new Position(i.Value.Player.X, i.Value.Player.Y)
                        };
                        i.Value.Player.UpdateCount++;
                        player.SendInfo("He is here already.");
                        return false;
                    }
                    else
                    {
                        player.Client.Reconnect(new ReconnectPacket()
                        {
                            GameId = i.Value.Player.Owner.Id,
                            Host = "",
                            IsFromArena = false,
                            Key = Empty<byte>.Array,
                            KeyTime = -1,
                            Name = i.Value.Player.Owner.Name,
                            Port = -1,
                        });
                        player.SendInfo("You are visiting " + i.Value.Player.Owner.Id + " now");


                        return true;
                    }


                }
            }
            player.SendError(string.Format("Player '{0}' could not be found!", args));
            return false;
        }
    }
    class GodCommand : Command
    {
        public GodCommand()
            : base("god", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.HasConditionEffect(ConditionEffects.Invincible))
            {
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invincible,
                    DurationMS = 0,
                });
                player.SendInfo("Godmode Deactivated");
                return false;
            }
            else
            {
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invincible,
                    DurationMS = -1
                });
                player.SendInfo("Godmode Activated");
            }
            return true;
        }
    }
    class SpectateCommand : Command
    {
        public SpectateCommand()
            : base("spectate", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.HasConditionEffect(ConditionEffectIndex.Stasis))
            {
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Stasis,
                    DurationMS = 0,
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Stunned,
                    DurationMS = 0,
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invisible,
                    DurationMS = 0,
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invincible,
                    DurationMS = 0,
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Quiet,
                    DurationMS = 0,
                });

                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Speedy,
                    DurationMS = 0,
                });
                if (player.Pet != null)
                    player.Owner.EnterWorld(player.Pet);
                if (player.Pet != null)
                    player.Pet.Move(player.X, player.Y);
                player.SendInfo("You aren't spectating anymore!");
                return false;

            }
            else
            {
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Stasis,
                    DurationMS = -1
                });

                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Stunned,
                    DurationMS = -1
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invisible,
                    DurationMS = -1
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invincible,
                    DurationMS = -1
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Quiet,
                    DurationMS = -1
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Speedy,
                    DurationMS = -1
                });

                if (player.Pet != null)
                    player.Owner.LeaveWorld(player.Pet);
                player.SendInfo("You are now spectating!");
            }
            return true;
        }
    }

    internal class FametoAccFame : Command
    {
        public FametoAccFame()
            : base("basefametoaccfame", 0)
        {
        }
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Client.Account.Rank < 4)
            {
                player.SendHelp("Command under development.");
                return false;
            }
            int a1 = Convert.ToInt32(args[0]);
            if (string.IsNullOrEmpty(args[0]))
            {
                player.SendHelp("Usage: /basefametoaccfame <Amount>");
                return false;
            }
            if (a1 > player.Fame)
            {
                player.SendHelp("You do not have this much fame. (Current basefame: " + player.Fame + ")");
                return false;
            }
            if (player.Fame <= 0)
            {
                player.SendInfo("You dont have any fame");
                return false;
            }
            player.Manager.Database.DoActionAsync(db =>
            {
                player.SendInfo("Successfully transferred Basefame into Accountfame (" + a1 + " basefame made into account fame)");
                player.CurrentFame = player.Fame = db.UpdateFame(player.Client.Account, +a1);
                player.SaveToCharacter();
                player.Fame = player.Fame = db.UpdateFame(player.Client.Account, -a1);
                player.Fame -= a1;
                player.UpdateCount++;
            });
            return true;
        }
    }

    internal class SetCommand : Command
    {
        public SetCommand()
            : base("setStat", 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 2)
            {
                try
                {
                    string stat = args[0].ToLower();
                    int amount = int.Parse(args[1]);
                    switch (stat)
                    {
                        case "health":
                        case "hp":
                            player.Stats[0] = amount;
                            break;
                        case "mana":
                        case "mp":
                            player.Stats[1] = amount;
                            break;
                        case "atk":
                        case "attack":
                            player.Stats[2] = amount;
                            break;
                        case "def":
                        case "defence":
                            player.Stats[3] = amount;
                            break;
                        case "spd":
                        case "speed":
                            player.Stats[4] = amount;
                            break;
                        case "vit":
                        case "vitality":
                            player.Stats[5] = amount;
                            break;
                        case "wis":
                        case "wisdom":
                            player.Stats[6] = amount;
                            break;
                        case "dex":
                        case "dexterity":
                            player.Stats[7] = amount;
                            break;
                        default:
                            player.SendError("Invalid Stat");
                            player.SendHelp("Stats: Health, Mana, Attack, Defence, Speed, Vitality, Wisdom, Dexterity");
                            player.SendHelp("Shortcuts: Hp, Mp, Atk, Def, Spd, Vit, Wis, Dex");
                            return false;
                    }
                    player.SaveToCharacter();
                    player.Client.Save();
                    player.UpdateCount++;
                    player.SendInfo("Success");
                }
                catch
                {
                    player.SendError("Error while setting stat");
                    return false;
                }
                return true;
            }
            else if (args.Length == 3)
            {
                foreach (Client i in player.Manager.Clients.Values)
                {
                    if (i.Account.Name.EqualsIgnoreCase(args[0]))
                    {
                        try
                        {
                            string stat = args[1].ToLower();
                            int amount = int.Parse(args[2]);
                            switch (stat)
                            {
                                case "health":
                                case "hp":
                                    i.Player.Stats[0] = amount;
                                    break;
                                case "mana":
                                case "mp":
                                    i.Player.Stats[1] = amount;
                                    break;
                                case "atk":
                                case "attack":
                                    i.Player.Stats[2] = amount;
                                    break;
                                case "def":
                                case "defence":
                                    i.Player.Stats[3] = amount;
                                    break;
                                case "spd":
                                case "speed":
                                    i.Player.Stats[4] = amount;
                                    break;
                                case "vit":
                                case "vitality":
                                    i.Player.Stats[5] = amount;
                                    break;
                                case "wis":
                                case "wisdom":
                                    i.Player.Stats[6] = amount;
                                    break;
                                case "dex":
                                case "dexterity":
                                    i.Player.Stats[7] = amount;
                                    break;
                                default:
                                    player.SendError("Invalid Stat");
                                    player.SendHelp("Stats: Health, Mana, Attack, Defence, Speed, Vitality, Wisdom, Dexterity");
                                    player.SendHelp("Shortcuts: Hp, Mp, Atk, Def, Spd, Vit, Wis, Dex");
                                    return false;
                            }
                            i.Player.SaveToCharacter();
                            i.Player.Client.Save();
                            i.Player.UpdateCount++;
                            player.SendInfo("Success");
                        }
                        catch
                        {
                            player.SendError("Error while setting stat");
                            return false;
                        }
                        return true;
                    }
                }
                player.SendError(string.Format("Player '{0}' could not be found!", args));
                return false;
            }
            else
            {
                player.SendHelp("Usage: /setStat <Stat> <Amount>");
                player.SendHelp("or");
                player.SendHelp("Usage: /setStat <Player> <Stat> <Amount>");
                player.SendHelp("Shortcuts: Hp, Mp, Atk, Def, Spd, Vit, Wis, Dex");
                return false;
            }
        }
    }

    internal class SetpieceCommand : Command
    {
        public SetpieceCommand()
            : base("setpiece", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            ISetPiece piece = (ISetPiece)Activator.CreateInstance(Type.GetType(
                "wServer.realm.setpieces." + args[0], true, true));
            piece.RenderSetPiece(player.Owner, new IntPoint((int)player.X + 1, (int)player.Y + 1));
            return true;
        }
    }

    internal class ListCommands : Command
    {
        public ListCommands()
            : base("commands", 0)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            Dictionary<string, Command> cmds = new Dictionary<string, Command>();
            Type t = typeof(Command);
            foreach (Type i in t.Assembly.GetTypes())
                if (t.IsAssignableFrom(i) && i != t)
                {
                    Command instance = (Command)Activator.CreateInstance(i);
                    cmds.Add(instance.CommandName, instance);
                }
            StringBuilder sb = new StringBuilder("");
            Command[] copy = cmds.Values.ToArray();
            for (int i = 0; i < copy.Length; i++)
            {
                if (i != 0) sb.Append(", ");
                sb.Append(copy[i].CommandName);
            }

            player.SendInfo(sb.ToString());
            return true;
        }
    }
}