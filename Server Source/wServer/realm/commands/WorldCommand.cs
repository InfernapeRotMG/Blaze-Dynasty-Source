#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using db;
using db.JsonObjects;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm.entities;
using wServer.realm.entities.player;

#endregion

namespace wServer.realm.commands
{
    internal class NexusCommand : Command
    {
        public NexusCommand()
            : base("nexus")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.Client.Reconnect(new ReconnectPacket
            {
                Host = "",
                Port = 2050,
                GameId = 0,
                Name = "Blaze Nexus",
                Key = Empty<byte>.Array,
            });
            return true;
        }
    }

    internal class RealmCommand : Command
    {
        public RealmCommand()
            : base("realm")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.Client.Reconnect(new ReconnectPacket
            {
                Host = "",
                Port = Program.Settings.GetValue<int>("port"),
                GameId = World.RAND_REALM,
                Name = "Realm",
                Key = Empty<byte>.Array,
            });
            return true;
        }
    }

    internal class ShowGiftCode : Command
    {
        public ShowGiftCode()
            : base("giftcode")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var giftCode = player.Client.Account.NextGiftCode();
            if (giftCode == null)
            {
                player.SendError("No new giftcode found.");
                return false;
            }

            var data = AccountDataHelper.GenerateAccountGiftCodeData(player.AccountId, giftCode).Write();
            var qrGenerator = new QrCodeGenerator();
            var qrCode = qrGenerator.CreateQrCode($"{Program.Settings.GetValue<string>("serverDomain")}/account/redeemGiftCode?data={data}", QrCodeGenerator.EccLevel.H);
            var bmp = qrCode.GetGraphic(5);
            var rgbValues = bmp.GetPixels();

            player.Client.SendPacket(new PicPacket
            {
                BitmapData = new BitmapData
                {
                    Bytes = rgbValues,
                    Height = bmp.Height,
                    Width = bmp.Width
                }
            });
            return true;
        }
    }


    internal class TutorialCommand : Command
    {
        public TutorialCommand()
            : base("tutorial")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.Client.Reconnect(new ReconnectPacket
            {
                Host = "",
                Port = Program.Settings.GetValue<int>("port"),
                GameId = World.TUT_ID,
                Name = "Tutorial",
                Key = Empty<byte>.Array,
            });
            return true;
        }
    }

    internal class TradeCommand : Command
    {
        public TradeCommand()
            : base("trade")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (String.IsNullOrWhiteSpace(args[0]))
            {
                player.SendInfo("Usage: /trade <player name>");
                return false;
            }
            player.RequestTrade(time, new RequestTradePacket
            {
                Name = args[0]
            });
            return true;
        }
    }


    internal class WhoCommand : Command
    {
        public WhoCommand()
            : base("who")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            StringBuilder sb = new StringBuilder("Players online: ");
            Player[] copy = player.Owner.Players.Values.ToArray();
            for (int i = 0; i < copy.Length; i++)
            {
                if (i != 0) sb.Append(", ");
                sb.Append(copy[i].Name);
            }

            player.SendInfo(sb.ToString());
            return true;
        }
    }

    internal class ServerCommand : Command
    {
        public ServerCommand()
            : base("server")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.SendInfo(player.Owner.Name);
            return true;
        }
    }

    internal class PauseCommand : Command
    {
        public PauseCommand()
            : base("pause")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.HasConditionEffect(ConditionEffectIndex.Paused))
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Paused,
                    DurationMS = 0
                });
                player.SendInfo("Game resumed.");
            }
            else
            {
                foreach (Enemy i in player.Owner.EnemiesCollision.HitTest(player.X, player.Y, 8).OfType<Enemy>())
                {
                    if (i.ObjectDesc.Enemy)
                    {
                        player.SendInfo("Not safe to pause.");
                        return false;
                    }
                }
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Paused,
                    DurationMS = -1
                });
                player.SendInfo("Game paused.");
            }
            return true;
        }
    }

    internal class TeleportCommand : Command
    {
        public TeleportCommand()
            : base("teleport")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (String.Equals(player.Name.ToLower(), args[0].ToLower()))
                {
                    player.SendInfo("You are already at yourself, and always will be!");
                    return false;
                }

                foreach (KeyValuePair<int, Player> i in player.Owner.Players)
                {
                    if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
                    {
                        player.Teleport(time, new TeleportPacket
                        {
                            ObjectId = i.Value.Id
                        });
                        return true;
                    }
                }
                player.SendInfo(string.Format("Cannot teleport, {0} not found!", args[0].Trim()));
            }
            catch
            {
                player.SendHelp("Usage: /teleport <player name>");
            }
            return false;
        }
    }

    class TellCommand : Command
    {
        public TellCommand() : base("tell") { }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (!player.NameChosen)
            {
                player.SendError("Choose a name!");
                return false;
            }
            if (args.Length < 2)
            {
                player.SendError("Usage: /tell <player name> <text>");
                return false;
            }

            string playername = args[0].Trim();
            string msg = string.Join(" ", args, 1, args.Length - 1);

            if (String.Equals(player.Name.ToLower(), playername.ToLower()))
            {
                player.SendInfo("Quit telling yourself!");
                return false;
            }

            if (playername.ToLower() == "muledump")
            {
                if (msg.ToLower() == "private muledump")
                {
                    player.Client.SendPacket(new TextPacket() //echo to self
                    {
                        ObjectId = player.Id,
                        BubbleTime = 10,
                        Stars = player.Stars,
                        Name = player.Name,
                        Recipient = "Muledump",
                        Text = msg.ToSafeText(),
                        CleanText = ""
                    });

                    player.Manager.Database.DoActionAsync(db =>
                    {
                        var cmd = db.CreateQuery();
                        cmd.CommandText = "UPDATE accounts SET publicMuledump=0 WHERE id=@accId;";
                        cmd.Parameters.AddWithValue("@accId", player.AccountId);
                        cmd.ExecuteNonQuery();
                        player.Client.SendPacket(new TextPacket()
                        {
                            ObjectId = -1,
                            BubbleTime = 10,
                            Stars = 70,
                            Name = "Muledump",
                            Recipient = player.Name,
                            Text = "Your muledump is now hidden, only you can view it now.",
                            CleanText = ""
                        });
                    });
                }
                else if (msg.ToLower() == "public muledump")
                {
                    player.Client.SendPacket(new TextPacket() //echo to self
                    {
                        ObjectId = player.Id,
                        BubbleTime = 10,
                        Stars = player.Stars,
                        Name = player.Name,
                        Recipient = "Muledump",
                        Text = msg.ToSafeText(),
                        CleanText = ""
                    });
                    player.Manager.Database.DoActionAsync(db =>
                    {
                        var cmd = db.CreateQuery();
                        cmd.CommandText = "UPDATE accounts SET publicMuledump=1 WHERE id=@accId;";
                        cmd.Parameters.AddWithValue("@accId", player.AccountId);
                        cmd.ExecuteNonQuery();

                        player.Client.SendPacket(new TextPacket()
                        {
                            ObjectId = -1,
                            BubbleTime = 10,
                            Stars = 70,
                            Name = "Muledump",
                            Recipient = player.Name,
                            Text = "Your muledump is now public, anyone can view it now.",
                            CleanText = ""
                        });
                    });
                }
                else
                {
                    player.Client.SendPacket(new TextPacket() //echo to self
                    {
                        ObjectId = player.Id,
                        BubbleTime = 10,
                        Stars = player.Stars,
                        Name = player.Name,
                        Recipient = "Muledump",
                        Text = msg.ToSafeText(),
                        CleanText = ""
                    });

                    player.Client.SendPacket(new TextPacket()
                    {
                        ObjectId = -1,
                        BubbleTime = 10,
                        Stars = 70,
                        Name = "Muledump",
                        Recipient = player.Name,
                        Text = "U WOT M8, 1v1 IN THE GARAGE!!!!111111oneoneoneeleven",
                        CleanText = ""
                    });
                }
                return true;
            }

            foreach (var i in player.Manager.Clients.Values)
            {
                if (i.Account.NameChosen && i.Account.Name.EqualsIgnoreCase(playername))
                {
                    player.Client.SendPacket(new TextPacket() //echo to self
                    {
                        ObjectId = player.Id,
                        BubbleTime = 10,
                        Stars = player.Stars,
                        Name = player.Name,
                        Recipient = i.Account.Name,
                        Text = msg.ToSafeText(),
                        CleanText = ""
                    });

                    i.SendPacket(new TextPacket() //echo to /tell player
                    {
                        ObjectId = i.Player.Owner.Id == player.Owner.Id ? player.Id : -1,
                        BubbleTime = 10,
                        Stars = player.Stars,
                        Name = player.Name,
                        Recipient = i.Account.Name,
                        Text = msg.ToSafeText(),
                        CleanText = ""
                    });
                    return true;
                }
            }
            player.SendError(string.Format("{0} not found.", playername));
            return false;
        }
    }


    #region Quests
    internal class QuestCommand : Command
    {
        public QuestCommand()
            : base("quest")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args[0] == "claim")
            {
                foreach (var i in player.playerQuestManager.GetPlayerQuests())
                {
                    if (i.completed && !i.rewarded)
                    {
                        var emptySlots = 0;
                        for (var j = 4; j < player.Inventory.Length; j++)
                        {
                            if (player.Inventory[j] == null)
                            {
                                emptySlots++;
                            }
                        }
                        if (emptySlots >= i.reward.Length)
                        {
                            foreach (var item in i.reward)
                            {
                                for (var space = 4; space < player.Inventory.Length; space++)
                                {
                                    if (player.Inventory[space] == null)
                                    {
                                        player.Inventory[space] = item;
                                        break;
                                    }
                                }
                            }
                            player.Experience += i.xpReward;
                            player.Manager.Database.DoActionAsync(db =>
                            {
                                player.CurrentFame =
                                    player.Client.Account.Stats.Fame =
                                        db.UpdateFame(player.Client.Account, i.fameReward);
                                player.Credits =
                                    player.Client.Account.Credits =
                                        db.UpdateCredit(player.Client.Account, i.goldReward);
                            });
                            player.UpdateCount++;
                            i.rewarded = true;
                        }
                        else
                        {
                            player.SendError("You do not have enough room in your inventory to claim the reward!");
                            return false;
                        }
                    }
                }
            }
            else
            {
                player.playerQuestManager.CheckQuestAvailability();
                player.SendInfo("===== Your Quests: =====");
                player.SendInfo($"Quests = {player.playerQuestManager.GetPlayerQuests().Count}");
                foreach (var i in player.playerQuestManager.GetPlayerQuests())
                {
                    if (!i.completed && !i.rewarded)
                    {
                        player.SendInfo($" {i.name}:");
                        player.SendInfo($"  {i.description}");
                        player.SendInfo($"  {i.progress}");
                        player.SendInfo($"  Rewards:");
                        foreach (var j in i.reward)
                            player.SendInfo($"      {j.ObjectId}");
                        player.SendInfo($"  {i.fameReward} Fame");
                        player.SendInfo($"  {i.xpReward} XP");
                    }
                }
            }
            return true;
        }
    }
    #endregion

    internal class MarketCommand : Command
    {
        public MarketCommand()
            : base("market")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Client.Account.Rank < 1 || player.Client.Account.Rank > 9)
            {
                if (args.Length < 1)
                {
                    player.SendError("Incorrect Usage.");
                    player.SendError("Examples:");
                    player.SendError("/market list <itemname>");
                    player.SendError("/market buy <id>");
                    player.SendError("/market sell fame <price> <inventoryslot>");
                    player.SendError("/market sell item <whichitemfor> <inventoryslot>");
                    player.SendError("/market cancel <id>");
                    return false;
                }
                var action = args[0];
                if (action.ToLower() == "list")
                {
                    string itemName = "";
                    for (var j = 1; j < args.Length; j++)
                    {
                        if (j == 3)
                            itemName = args[j];
                        else
                            itemName = itemName + " " + args[j];
                    }
                    player.Manager.Database.DoActionAsync(db =>
                    {
                        //ret.Add($"{rdr.GetString("itemname")},{rdr.GetString("seller")},{rdr.GetInt32("fame")},{rdr.GetString("price")}");
                        if (itemName != null && itemName != "")
                        {
                            var marketList = db.MarketFindItem(itemName);
                            player.SendInfo("Results for : " + itemName);
                            foreach (var i in marketList)
                            {
                                var splitString = i.Split(',');
                                var id = int.Parse(splitString[0]);
                                var name = splitString[1];
                                var seller = splitString[2];
                                var forFame = int.Parse(splitString[3]) == 1 ? true : false;
                                if (forFame)
                                {
                                    var price = int.Parse(splitString[4]);
                                    player.SendInfo($"{id}: '{name}' for {price} fame by {seller}.");
                                }
                                else
                                {
                                    var price = splitString[4];
                                    player.SendInfo($"{id}: '{name}' for {price} by {seller}.");
                                }
                            }
                        }
                        else
                        {
                            var marketList = db.MarketFindItem(all: true);
                            player.SendInfo("All listings:");
                            foreach (var i in marketList)
                            {
                                var splitString = i.Split(',');
                                var id = int.Parse(splitString[0]);
                                var name = splitString[1];
                                var seller = splitString[2];
                                var forFame = int.Parse(splitString[3]) == 1 ? true : false;
                                if (forFame)
                                {
                                    var price = int.Parse(splitString[4]);
                                    player.SendInfo($"{id}: '{name}' for {price} fame by {seller}.");
                                }
                                else
                                {
                                    var price = splitString[4];
                                    player.SendInfo($"{id}: '{name}' for {price} by {seller}.");
                                }
                            }
                        }
                    });
                }
                else if (action.ToLower() == "buy")
                {
                    var id = int.Parse(args[1]);
                    var emptySlot = -1;
                    for (var i = 4; i < player.Inventory.Length; i++)
                    {
                        if (player.Inventory[i] == null)
                        {
                            emptySlot = i;
                            break;
                        }
                    }

                    player.Manager.Database.DoActionAsync(db =>
                    {
                        var marketList = db.MarketFindItem("", id);
                        var splitString = marketList[0].Split(',');
                        var name = splitString[0];
                        var seller = splitString[1];
                        var forFame = int.Parse(splitString[2]) == 1 ? true : false;
                        if (forFame)
                        {
                            if (emptySlot != -1)
                            {
                                var price = int.Parse(splitString[3]);
                                if (player.CurrentFame >= price)
                                {
                                    player.CurrentFame =
                                        player.Client.Account.Stats.Fame =
                                            db.UpdateFame(player.Client.Account, -price);
                                    db.MarketSoldItem(id, player.Name);
                                    ushort itemID;
                                    player.Manager.GameData.IdToObjectType.TryGetValue(name, out itemID);
                                    player.Inventory[emptySlot] = player.Manager.GameData.Items[itemID];
                                    player.SendInfo("You have successfully bought " + name + " for " + price + ".");
                                    player.UpdateCount++;
                                }
                                else
                                {
                                    player.SendError("You do not have enough fame to buy this!");
                                }
                            }
                            else
                            {
                                player.SendError("You do not have enough space in your inventory!");
                            }
                        }
                        else
                        {
                            var price = splitString[3];
                            var bought = false;
                            for (var j = 4; j < player.Inventory.Length; j++)
                            {
                                if (player.Inventory[j] != null)
                                    if (player.Inventory[j].ObjectId == price)
                                    {
                                        player.Inventory[j] = null;
                                        ushort itemID;
                                        db.MarketSoldItem(id, player.Name);
                                        player.Manager.GameData.IdToObjectType.TryGetValue(name, out itemID);
                                        player.Inventory[j] = player.Manager.GameData.Items[itemID];
                                        player.SendInfo("You have successfully bought " + name + " for " + price + ".");
                                        player.UpdateCount++;
                                        bought = true;
                                        break;
                                    }
                            }
                            if (!bought)
                                player.SendError("You do not have the item required to purchase this! You need a " + price + ".");
                        }
                    });

                    //if (player.CurrentFame >= price)
                    //{
                    //    player.CurrentFame =
                    //        player.Client.Account.Stats.Fame =
                    //            db.UpdateFame(player.Client.Account, -Price);
                    //}

                }
                else if (action.ToLower() == "sell")
                {
                    var fameOrItem = args[1];
                    if (fameOrItem.ToLower() == "fame")
                    {
                        var invSlot = int.Parse(args[2]) + 3;
                        var price = int.Parse(args[3]);
                        if (player.Inventory[invSlot] != null)
                        {
                            var item = player.Inventory[invSlot];
                            var itemName = item.ObjectId;
                            player.Inventory[invSlot] = null;
                            player.UpdateCount++;
                            player.Manager.Database.DoActionAsync(db =>
                            {
                                db.MarketAddItem(itemName, player.Name, true, price.ToString());
                                player.SendInfo($"You have successfully added {itemName} to the market for {price} fame.");
                            });
                        }
                    }
                    else if (fameOrItem.ToLower() == "item")
                    {
                        var invSlot = int.Parse(args[2]) + 3;
                        string price = "";
                        for (var j = 3; j < args.Length; j++)
                        {
                            if (j == 3)
                                price = args[j];
                            else
                                price = price + " " + args[j];
                        }
                        if (player.Inventory[invSlot] != null)
                        {
                            ushort priceItem;
                            player.Manager.GameData.IdToObjectType.TryGetValue(price, out priceItem);
                            if (player.Manager.GameData.Items[priceItem] != null)
                            {
                                var item = player.Inventory[invSlot];
                                var itemName = item.ObjectId;
                                player.Inventory[invSlot] = null;
                                player.UpdateCount++;
                                player.Manager.Database.DoActionAsync(db =>
                                {
                                    db.MarketAddItem(itemName, player.Name, false, price);
                                    player.SendInfo($"You have successfully added {itemName} to the market for {price}.");
                                });
                            }
                        }
                    }
                    else
                    {
                        player.SendError("You have not identified what you want in return, item or fame.");
                        player.SendError("Examples:");
                        player.SendError("/market sell fame <inventoryslot> <price>");
                        player.SendError("/market sell item <inventoryslot> <whichitemfor>");
                    }
                }
                else if (action.ToLower() == "claim")
                {
                    player.Manager.Database.DoActionAsync(db =>
                    {
                        var claimedStuff = db.MarketClaim(player.Name);
                        foreach (var i in claimedStuff)
                        {
                            if (i.Value.Item1)
                            {
                                player.CurrentFame =
                                    player.Client.Account.Stats.Fame =
                                        db.UpdateFame(player.Client.Account, int.Parse(i.Value.Item2));
                                player.SendInfo($"You have received your {i.Value.Item2} fame!");
                            }
                            else
                            {
                                ushort priceItem;
                                player.Manager.GameData.IdToObjectType.TryGetValue(i.Value.Item2, out priceItem);
                                for (var j = 4; j < player.Inventory.Length; j++)
                                {
                                    if (player.Inventory[j] == null)
                                    {
                                        player.Inventory[j] = player.Manager.GameData.Items[priceItem];
                                        player.SendInfo($"You have received your {i.Value.Item2} for {i.Key.Item2}!");
                                        player.UpdateCount++;
                                        break;
                                    }
                                }
                            }
                        }
                    });
                }
                else if (action.ToLower() == "cancel")
                {
                    var id = int.Parse(args[1]);
                    var emptySlot = -1;
                    for (var i = 4; i < player.Inventory.Length; i++)
                    {
                        if (player.Inventory[i] == null)
                        {
                            emptySlot = i;
                            break;
                        }
                    }

                    player.Manager.Database.DoActionAsync(db =>
                    {
                        var marketList = db.MarketFindItem("", id);
                        var splitString = marketList[0].Split(',');
                        var name = splitString[0];
                        var seller = splitString[1];
                        var forFame = int.Parse(splitString[2]) == 1 ? true : false;
                        if (seller == player.Name)
                        {
                            if (forFame)
                            {
                                if (emptySlot != -1)
                                {
                                    db.MarketSoldItem(id, player.Name);
                                    ushort itemID;
                                    player.Manager.GameData.IdToObjectType.TryGetValue(name, out itemID);
                                    player.Inventory[emptySlot] = player.Manager.GameData.Items[itemID];
                                    player.SendInfo("You have cancelled your listing.");
                                    db.MarketClaim(player.Name);
                                    player.UpdateCount++;
                                }
                                else
                                {
                                    player.SendError("You do not have enough space in your inventory!");
                                }
                            }
                            else
                            {
                                for (var j = 4; j < player.Inventory.Length; j++)
                                {
                                    if (player.Inventory[j] == null)
                                    {
                                        ushort itemID;
                                        db.MarketSoldItem(id, player.Name);
                                        player.Manager.GameData.IdToObjectType.TryGetValue(name, out itemID);
                                        player.Inventory[j] = player.Manager.GameData.Items[itemID];
                                        player.SendInfo("You have cancelled your listing.");
                                        db.MarketClaim(player.Name);
                                        player.UpdateCount++;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            player.SendError("You cannot cancel an item thats not yours!");
                        }
                    });

                    //if (player.CurrentFame >= price)
                    //{
                    //    player.CurrentFame =
                    //        player.Client.Account.Stats.Fame =
                    //            db.UpdateFame(player.Client.Account, -Price);
                    //}

                }

                else
                {
                    player.SendError("Incorrect Usage.");
                    player.SendError("Examples:");
                    player.SendError("/market list <itemname>");
                    player.SendError("/market buy <id>");
                    player.SendError("/market sell fame <inventoryslot> <price>");
                    player.SendError("/market sell item <inventoryslot> <whichitemfor>");
                    player.SendError("/market claim");
                    return false;
                }
            }
            else
            {
                player.SendError("You cannot use the market!");
            }
            return true;
        }
    }
}