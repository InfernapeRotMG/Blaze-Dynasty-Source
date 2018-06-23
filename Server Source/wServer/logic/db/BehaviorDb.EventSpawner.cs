#region

using wServer.logic.behaviors;
using wServer.logic.loot;
using wServer.logic.transitions;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private _ Jeebs = () => Behav()
              .Init("Jeebs",
                  new State(
                new TransformOnDeath("Jeebs"),

                                            new State("PlayerChoice",
                          new Taunt("You have finished the round. Be prepared for the next!"),

                        new TimedTransition(5000, "PlayerChoice2")
                              ),
                                            new State("WOWWWW",
                                                new Taunt("Up for another round, that's what I'm doing!"),
                                                 new TimedTransition(3000, "PlayerChoice2")
                                                ),
                          new State("PlayerChoice2",
                                    new Taunt("So choose one of the following 'MALPHAS' 'SEPTAVIUS' Make sure you use capitals!"),

                        new TimedTransition(25, "NOOOOOOWWWW")

                      ),
                          new State("NOOOOOOWWWW",
                                    new ChatTransition("Archdemon Malphas", "MALPHAS"),
                                    new ChatTransition("Septavius the Ghost God", "SEPTAVIUS")

                              ),
                        new State("Malphas Prep",
                                    new Taunt("Malphas that is in 10 seconds"),
                              new TimedTransition(0, "Archdemon Malphas")
                              ),
                    new State("Archdemon Malphas",
                    new ConditionalEffect(ConditionEffectIndex.Invincible),

                    new Spawn("Archdemon Malphas", 1, coolDown: 10000),

                        new TimedTransition(12000, "Archdemon Malphas Check")
                        ),
                    new State("Archdemon Malphas Check",
                        new EntityNotExistsTransition("Archdemon Malphas", 10000, "suicide")
                        ),
                                          new State("Septavius the Ghost God Prep",
                                    new Taunt("Septavius the Ghost God that is in 10 seconds"),
                              new TimedTransition(0, "Septavius the Ghost God")
                              ),
                   
                   new State("Septavius the Ghost God",
                    new ConditionalEffect(ConditionEffectIndex.Invincible),

                    new Spawn("Septavius the Ghost God", 1, coolDown: 10000),

                        new TimedTransition(12000, "Septavius the Ghost God Check")
                        ),
                    new State("Septavius the Ghost God Check",
                        new EntityNotExistsTransition("Septavius the Ghost God", 10000, "suicide")
                        ),

                    new State("suicide",
                        new Suicide()


                        )
                          )


            );
    }
}
