using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wServer.realm;
using wServer.logic.behaviors;
using wServer.logic.loot;
using wServer.logic.transitions;

namespace wServer.logic
{
    partial class BehaviorDb
    {
        _ Xpfarm = () => Behav()
              .Init("XP Gift",
                new State(
                    new Prioritize(
                        new Wander(0.1)
                        )
                    )
            )
        ;
    }
}