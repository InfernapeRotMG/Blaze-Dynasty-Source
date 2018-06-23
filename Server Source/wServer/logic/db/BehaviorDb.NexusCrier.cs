#region
using System;
using System.Collections.Generic;
using wServer.logic.behaviors;
using db;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private _ Nexus = () => Behav()
             .Init("Nexus Crier",
                 new State("Active",

                     new Taunt(1, 15000, "Welcome to Blaze Dynasty! Play the game and message Infernape if you want to donate!"),
                     new Taunt(1, 20000, "Hacking is not tolerated! You will be instantly IPBANNED if caught!")
                     )
             );

    }
}