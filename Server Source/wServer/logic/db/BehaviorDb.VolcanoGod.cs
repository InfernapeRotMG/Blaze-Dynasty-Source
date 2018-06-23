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
        _ VolcanoGod = () => Behav()
          .Init("Volcano God",
                new State(
                    new RealmPortalDrop(),
                    new State("default",
                        new PlayerWithinTransition(15, "taunt1")
                        ),
                        new State("taunt1",
                    new ConditionalEffect(ConditionEffectIndex.Invincible),
                    new Taunt("Welcome to the Volcano Land, where you shall die!!!"),
                    new TimedTransition(5000, "fight1")
                    ),
                    new State("fight1",
                        new Taunt(1.00, "Taunt 1"),
                        new Wander(0.4),
                        new Shoot(8.4, count: 12, shootAngle: 26, projectileIndex: 1, coolDown: 1200),
                        new Shoot(10, count: 1, projectileIndex: 4, coolDown: 1),
                        new TimedTransition(3000, "fight2")
                       ),
                    new State("fight2",
                        new Wander(0.65),
                        new Shoot(10, count: 1, projectileIndex: 4, coolDown: 1),
                        new Shoot(8.4, count: 6, shootAngle: 16, projectileIndex: 1, coolDown: 1500),
                        new Shoot(8.4, count: 12, shootAngle: 16, predictive: 2, projectileIndex: 2, coolDown: 2200),
                        new TimedTransition(7000, "fight3")
                       ),
                    new State("fight3",
                        new Swirl(1, 8, 10),
                        new Shoot(10, count: 20, shootAngle: 28, projectileIndex: 3, coolDown: 2800),
                        new Shoot(10, count: 12, projectileIndex: 5, coolDown: 1600),
                        new Shoot(8.4, count: 8, shootAngle: 16, predictive: 1, projectileIndex: 1, coolDown: 2200),
                        new TimedTransition(6500, "taunt2")
                       ),
                       new State("taunt2",
                    new ConditionalEffect(ConditionEffectIndex.Invincible),
                    new Taunt("Good job hero, but you are too late. I am ready to EXPLODE!!!"),
                    new TimedTransition(5000, "fight4")
                    ),
                       new State("fight4",
                        new Swirl(1, 8, 10),
                        new Shoot(10, count: 20, shootAngle: 28, projectileIndex: 3, coolDown: 2800),
                        new Shoot(10, count: 12, projectileIndex: 5, coolDown: 1600),
                        new Shoot(8.4, count: 8, shootAngle: 16, predictive: 1, projectileIndex: 1, coolDown: 2200),
                        new TimedTransition(6500, "fight5")
                        ),
                    new State("fight5",
                        new Taunt(1.00, "Taunt 2"),
                        new Follow(0.6, 8, 1),
                        new Shoot(10, count: 20, shootAngle: 28, projectileIndex: 5, coolDown: 2800),
                        new Shoot(10, count: 12, projectileIndex: 0, coolDown: 1600),
                        new Shoot(8.4, count: 28, projectileIndex: 3, coolDown: 6000),
                        new Shoot(8.4, count: 8, shootAngle: 16, projectileIndex: 2, coolDown: 2200),
                        new TimedTransition(7000, "fight6")
                       ),
                    new State("fight6",
                        new Wander(0.5),
                        new ConditionalEffect(ConditionEffectIndex.Armored),
                        new SpecificHeal(1, 100, "Self", coolDown: 1000),
                        new Shoot(10, count: 14, shootAngle: 28, projectileIndex: 0, coolDown: 3000),
                        new Shoot(8.4, count: 8, shootAngle: 16, predictive: 3, projectileIndex: 3, coolDown: 2200),
                        new Shoot(7, count: 13, projectileIndex: 2, coolDown: 800),
                        new TimedTransition(6000, "taunt3")
                       ),
                       new State("taunt3",
                    new ConditionalEffect(ConditionEffectIndex.Invincible),
                    new Taunt("Congrats hero, your fight is almost over. But I will fight till to the end!!!"),
                    new TimedTransition(5000, "fight7")
                    ),
                    new State("fight7",
                        new BackAndForth(1, 7),
                        new Shoot(10, count: 6, shootAngle: 16, projectileIndex: 0, coolDown: 10),
                        new Shoot(8.4, count: 22, projectileIndex: 3, coolDown: 2200),
                        new TimedTransition(4000, "fight8")
                       ),
                    new State("fight8",
                        new Taunt(1.00, "Taunt 3"),
                        new ConditionalEffect(ConditionEffectIndex.StunImmune),
                        new Flash(0xFF0FF0, 2, 2),
                        new Swirl(0.5, 8, 10),
                        new Shoot(10, count: 9, shootAngle: 12, projectileIndex: 2, coolDown: 1000),
                        new Shoot(10, count: 6, shootAngle: 60, projectileIndex: 0, coolDown: 750),
                        new Shoot(10, count: 5, projectileIndex: 4, coolDown: 1250),
                        new Grenade(7, 85, range: 8, coolDown: 20),
                        new TimedTransition(4000, "taunt4")
                        ),
                        new State("taunt4",
                    new ConditionalEffect(ConditionEffectIndex.Invincible),
                    new Taunt("This is the final time I shall talk to you. Congrats hero, you have defeated the volcano..."),
                    new TimedTransition(5000, "fight9")
                       ),
                       new State("fight9",
                           new Taunt(1.00, "Damn You"),
                        new ConditionalEffect(ConditionEffectIndex.StunImmune),
                        new Flash(0xFF0FF0, 2, 2),
                        new Swirl(0.5, 8, 10),
                        new Shoot(10, count: 9, shootAngle: 12, projectileIndex: 2, coolDown: 1000),
                        new Shoot(10, count: 6, shootAngle: 60, projectileIndex: 0, coolDown: 750),
                        new Shoot(10, count: 5, projectileIndex: 4, coolDown: 1250),
                        new Grenade(7, 85, range: 8, coolDown: 20),
                         new TimedTransition(1000, "default")
                             )
                                )
            );
    }
}