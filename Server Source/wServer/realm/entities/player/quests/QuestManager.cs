using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wServer.realm.entities.player.quests
{
    public class QuestManager
    {
        private Player player;
        private List<PlayerQuest> QuestsList = new List<PlayerQuest>();
        private RealmManager manager;
        private List<Item> itemsTemp;

        private static readonly ILog log = LogManager.GetLogger(typeof(QuestManager));

        public QuestManager(Player player)
        {
            this.player = player;
            manager = player.Manager;
            foreach(var i in Quests)
            {
                for (var j = 0; j < i.rewardIds.Length; j++)
                {
                    i.reward[j] = manager.GameData.Items[i.rewardIds[j]];
                }
            }
        }

        public bool AddPlayerQuest(int id, int progress = -1, bool completed = false, bool rewarded = false)
        {
            var newQuest = Quests[id - 1];
            var foundQuest = false;
            foreach(var i in QuestsList)
            {
                if(i.id == newQuest.id)
                {
                    foundQuest = true;
                }
            }
            if (!foundQuest)
            {
                if (progress != -1)
                {
                    newQuest.actualProgress = progress;
                    var tempString = newQuest.actualProgress + "/" + newQuest.progress.Split(' ')[0].Split('/')[1];
                    var tempString2 = newQuest.progress.Split(' ');
                    for (var j = 1; j < tempString2.Length; j++)
                    {
                        tempString = tempString + " " + tempString2[j];
                    }
                    newQuest.progress = tempString;
                }
                newQuest.completed = completed;
                newQuest.rewarded = rewarded;
                if (newQuest.requiredQuestId == -1)
                {
                    QuestsList.Add(newQuest);
                    return true;
                } else
                {
                    foreach(var i in QuestsList)
                    {
                        if (newQuest.requiredQuestId == i.id && i.completed)
                        {
                            QuestsList.Add(newQuest);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void SetCompetedPlayerQuest(int id, bool completed)
        {
            foreach(var i in QuestsList)
            {
                if(i.id == id)
                {
                    i.completed = completed;
                }
            }
        }

        public void UpdateProgressPlayerQuest(int id, string progress, int actualProgress)
        {
            foreach (var i in QuestsList)
            {
                if (i.id == id)
                {
                    i.progress = progress;
                    i.actualProgress = actualProgress;
                }
            }
        }

        public void CheckQuestAvailability()
        {
            log.Info($"QuestManager test, count:{QuestsList.Count}");
            foreach (var i in Quests)
            {
                var foundQuest = false;
                foreach (var j in QuestsList)
                    if (i.id == j.id)
                    {
                        foundQuest = true;
                        break;
                    }

                if (!foundQuest)
                {
                    log.Info($"QuestManager test, required:{i.requiredQuestId}");

                    if (i.requiredQuestId == -1)
                    {
                        AddPlayerQuest(i.id);
                    }
                    else
                    {
                        foreach (var j in QuestsList)
                        {
                            log.Info($"QuestManager test, id:{i.id}");
                            if (j.id == i.requiredQuestId && j.completed)
                            {
                                AddPlayerQuest(i.id);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public List<PlayerQuest> GetPlayerQuests()
        {
            return QuestsList;
        }

        private PlayerQuest[] Quests = new PlayerQuest[]
        {
            new PlayerQuest(1, "Quest",
                "Welcome to Blaze Dynasty! You must defeat 5 Archdemon Malphas for this quest.", "0/5 Archdemon Malphas Killed", new Item[1],
                new ushort[] {
                    0xa20 // Potion of Def
                }, 100, 0, 1000, -1, actualProgress:0), //100 fame and 1000 xp reward
            new PlayerQuest(2, "Quest 2",// add it here: example:
                "Welcome to Blaze Dynasty! You must defeat 5 Septavius the Ghost God for this quest.", "0/5 Septavius the Ghost God Killed", new Item[2],
                new ushort[] {
                    0xa20, // Potion of Def
                    0xa21
                }, 200, 0, 1000, 1, actualProgress:0),
            new PlayerQuest(3, "Quest 3",// add it here: example:
                "Welcome to Blaze Dynasty! You must defeat 5 Oryx the Mad God 2 for this quest.", "0/5 Oryx the Mad God 2 Killed", new Item[3],//you need to set this to amount of items your rewarding
                new ushort[] {
                    0xa20, // Potion of Def
                    0x32a, // char slot unlocker
                    0x32b // vault chest unlocker
                }, 200, 200, 1000, 1, actualProgress:0)
        };// some cool features : If you change this number to what quest you want finished before unlocking, it will wait, here ill show you



        // This is the part that checks the progress and does everything to do with completing quest
        public void ProcessQuestAction(int id)
        {
            switch (id)
            {
                case 1: //Quest quest
                    foreach(var i in QuestsList)
                    {
                        if (i.id == id && !i.completed)
                        {
                            i.actualProgress++;
                            i.progress = $"{i.actualProgress}/5 Pirates Killed";
                            if (i.actualProgress >= 5)
                            {
                                i.completed = true;
                                if (player != null)
                                    player.SendInfo($"[QUESTS] You have completed {i.name}! Do '/quest claim' to claim your reward!");
                            }
                            break;
                        }
                    }
                    break;
                case 2:
                    foreach (var i in QuestsList)
                    {
                        if (i.id == id && !i.completed)
                        {
                            i.actualProgress++;
                            i.progress = $"{i.actualProgress}/5 Snakes Killed";
                            if (i.actualProgress >= 5)
                            {
                                i.completed = true;
                                if (player != null)
                                    player.SendInfo($"[QUESTS] You have completed {i.name}! Do '/quest claim' to claim your reward!");
                            }
                            break;
                        }
                    }
                    break;
                case 3:// all you do to add a quest is:
                    foreach (var i in QuestsList)
                    {
                        if (i.id == id && !i.completed)
                        {
                            i.actualProgress++;
                            i.progress = $"{i.actualProgress}/5 XP Gifts Killed";
                            if (i.actualProgress >= 5)
                            {
                                i.completed = true;
                                if (player != null)
                                    player.SendInfo($"[QUESTS] You have completed {i.name}! Do '/quest claim' to claim your reward!");
                            }
                            break;
                        }
                    }
                    break;
            }
        }
    }

    public class PlayerQuest
    {
        public int id;
        public string name;
        public string description;
        public string progress;
        public Item[] reward;
        public ushort[] rewardIds;
        public int fameReward;
        public int goldReward;
        public int xpReward;
        public int requiredQuestId;
        public bool completed;
        public bool rewarded;
        public int actualProgress;

        public PlayerQuest(int id, string name, string description, string progress, Item[] reward, ushort[] rewardIds, int fameReward, int goldReward, int xpReward, int requiredQuestId, bool completed = false, bool rewarded = false, int actualProgress = -1)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.progress = progress;
            this.reward = reward;
            this.rewardIds = rewardIds;
            this.fameReward = fameReward;
            this.goldReward = goldReward;
            this.xpReward = xpReward;
            this.requiredQuestId = requiredQuestId;
            this.completed = completed;
            this.rewarded = rewarded;
            this.actualProgress = actualProgress;
        }
    }
}
