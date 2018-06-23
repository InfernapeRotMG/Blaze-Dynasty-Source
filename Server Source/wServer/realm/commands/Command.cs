#region

using System;
using System.Collections.Generic;
using log4net;
using wServer.realm.entities.player;

#endregion

namespace wServer.realm.commands
{
    internal class Ranks
    {
        public static readonly int Player = 0;
        public static readonly int Donor = 1;
        public static readonly int Supporter = 2;
        public static readonly int VIP = 3;
        public static readonly int Helper = 4;
        public static readonly int Moderator = 5;
        public static readonly int Admin = 6;
        public static readonly int Dev = 7;
        public static readonly int HDev = 8;
        public static readonly int CoOwner = 9;
        public static readonly int Owner = 10;
    }

    public abstract class Command
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(Command));

        public Command(string name, int permLevel = 0)
        {
            CommandName = name;
            PermissionLevel = permLevel;
        }

        public string CommandName { get; private set; }
        public int PermissionLevel { get; private set; }

        protected abstract bool Process(Player player, RealmTime time, string[] args);

        private static int GetPermissionLevel(Player player)
        {
            if (player.Client.Account.Rank == 0)
                return 0;
            if (player.Client.Account.Rank == 1)
                return 1;
            if (player.Client.Account.Rank == 2)
                return 2;
            if (player.Client.Account.Rank == 3)
                return 3;
            if (player.Client.Account.Rank == 4)
                return 4;
            if (player.Client.Account.Rank == 5)
                return 5;
            if (player.Client.Account.Rank == 6)
                return 6;
            if (player.Client.Account.Rank == 7)
                return 7;
            if (player.Client.Account.Rank == 8)
                return 8;
            if (player.Client.Account.Rank == 9)
                return 9;
            if (player.Client.Account.Rank == 10)
                return 10;
            if (player.Client.Account.Rank == 11)
                return 11;
            return 0;
        }


        public bool HasPermission(Player player)
        {
            if (GetPermissionLevel(player) < PermissionLevel)
                return false;
            return true;
        }

        public bool Execute(Player player, RealmTime time, string args)
        {
            if (!HasPermission(player))
            {
                player.SendError("You cannot do that command! (No Permission)");
                return false;
            }

            try
            {
                string[] a = args.Split(' ');
                return Process(player, time, a);
            }
            catch (Exception ex)
            {
                log.Error("Error when executing the command.", ex);
                player.SendError("Error when executing the command.");
                return false;
            }
        }
    }

    public class CommandManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CommandManager));

        private readonly Dictionary<string, Command> cmds;

        private RealmManager manager;

        public CommandManager(RealmManager manager)
        {
            this.manager = manager;
            cmds = new Dictionary<string, Command>(StringComparer.InvariantCultureIgnoreCase);
            Type t = typeof(Command);
            foreach (Type i in t.Assembly.GetTypes())
                if (t.IsAssignableFrom(i) && i != t)
                {
                    Command instance = (Command)Activator.CreateInstance(i);
                    cmds.Add(instance.CommandName, instance);
                }
        }

        public IDictionary<string, Command> Commands
        {
            get { return cmds; }
        }

        public bool Execute(Player player, RealmTime time, string text)
        {
            int index = text.IndexOf(' ');
            string cmd = text.Substring(1, index == -1 ? text.Length - 1 : index - 1);
            string args = index == -1 ? "" : text.Substring(index + 1);

            Command command;
            if (!cmds.TryGetValue(cmd, out command))
            {
                player.SendError("Unknown command!");
                return false;
            }
            log.InfoFormat("[Command] <{0}> {1}", player.Name, text);
            return command.Execute(player, time, args);
        }
    }
}