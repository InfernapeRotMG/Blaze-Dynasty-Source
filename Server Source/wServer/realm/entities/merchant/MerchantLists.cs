#region

using System;
using System.Collections.Generic;
using System.Linq;
using db.data;
using log4net;

#endregion

namespace wServer.realm.entities
{
    internal class MerchantLists
    {
        public static int[] AccessoryClothList;
        public static int[] AccessoryDyeList;
        public static int[] ClothingClothList;
        public static int[] ClothingDyeList;

        public static Dictionary<int, Tuple<int, CurrencyType>> prices = new Dictionary<int, Tuple<int, CurrencyType>>
        {
            {0xb41, new Tuple<int, CurrencyType>(50000, CurrencyType.Fame)},
            {0xbab, new Tuple<int, CurrencyType>(50000, CurrencyType.Fame)},
            {0xbad, new Tuple<int, CurrencyType>(50000, CurrencyType.Fame)},

            //UT WEAPONS
            {0xb3f, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Crystal Wand
            {0x9c0, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Wand Of Retribution
            {0x9ca, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Wand of Evocation
            {0xc04, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Wand of the Bulwark
            {0xc10, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Coral Bow
            {0xc02, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Doom Bow
            {0x9cd, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Bow of Deep Enchantment
            {0x2299, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Staff of the rising sun
            {0x21a7, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Sentient Staff
            {0x2363, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Phlyactery Staff
            {0x910, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Staff of the Fundamental Core
            {0x9c9, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Sword of Majesty
            {0xa03, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Crystal Sword
            {0xc01, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Demon Blade
            {0xc05, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Ancient Stone Sword
            {0x2367, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Pixie Sword
            {0x21a0, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //etherite dagger
            {0xc0a, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Drirk of Cronus
            {0x9c7, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Dagger of Dire hatred
            {0xcdb, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Cutlass
            {0x915, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Ray katana, , , 
            {0xcdc, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Doku no ken
            {0xaf6, new Tuple<int, CurrencyType>(225, CurrencyType.Fame)},
            {0xa87, new Tuple<int, CurrencyType>(200,CurrencyType.Fame)},
            {0xa86, new Tuple<int, CurrencyType>(600, CurrencyType.Fame)},
            {0xa85, new Tuple<int, CurrencyType>(600, CurrencyType.Fame)},
            {0xa07, new Tuple<int, CurrencyType>(600, CurrencyType.Fame)},
            {0xb02, new Tuple<int, CurrencyType>(225, CurrencyType.Fame)},
            {0xa8d, new Tuple<int, CurrencyType>(200,CurrencyType.Fame)},
            {0xa8c, new Tuple<int, CurrencyType>(600, CurrencyType.Fame)},
            {0xa8b, new Tuple<int, CurrencyType>(600, CurrencyType.Fame)},
            {0xa1e, new Tuple<int, CurrencyType>(600, CurrencyType.Fame)},
            {0xb08, new Tuple<int, CurrencyType>(750, CurrencyType.Fame)},
            {0xaa2, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)},
            {0xaa1, new Tuple<int, CurrencyType>(600, CurrencyType.Fame)},
            {0xaa0, new Tuple<int, CurrencyType>(600, CurrencyType.Fame)},
            {0xa9f, new Tuple<int, CurrencyType>(600, CurrencyType.Fame)},
            {0xb0b, new Tuple<int, CurrencyType>(750, CurrencyType.Fame)},
            {0xa47, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)},
            {0xa84, new Tuple<int, CurrencyType>(600, CurrencyType.Fame)},
            {0xa83, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)},
            {0xa82, new Tuple<int, CurrencyType>(600, CurrencyType.Fame)},
            {0xaff, new Tuple<int, CurrencyType>(225, CurrencyType.Fame)},
            {0xa8a, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)},
            {0xa89, new Tuple<int, CurrencyType>(600, CurrencyType.Fame)},
            {0xa88, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)},
            {0xa19, new Tuple<int, CurrencyType>(600, CurrencyType.Fame)},
            {0xc50, new Tuple<int, CurrencyType>(225, CurrencyType.Fame)},
            {0xc4f, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)},
            {0xc4e, new Tuple<int, CurrencyType>(600, CurrencyType.Fame)},
            {0xc4d, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)},
            {0xc4c, new Tuple<int, CurrencyType>(600, CurrencyType.Fame)},

            //UT RINGS GO HERE
            {0xbac, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Twlight Gemstone
            {0xba0, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Ring of the Sphyinx
            {0xba1, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Ring Of the nile
            {0xba2, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Ring Of the pyramid
            {0xba9, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Ring Of unbound health
            {0xabf, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Ring of Paramount Attack T4
            {0xac0, new Tuple<int, CurrencyType>(120, CurrencyType.Fame)}, //Ring of Paramount Defense T4
            {0xac1, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Ring Of Paramount Speed T4
            {0xac2, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Ring Of Paramount Vitality T4
            {0xac3, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Ring Of Paramount Wisdom T4
            {0xac4, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Ring Of Paramount Dexterity T4
            {0xac5, new Tuple<int, CurrencyType>(120, CurrencyType.Fame)}, //Ring Of Paramount Health T4
            {0xac6, new Tuple<int, CurrencyType>(120, CurrencyType.Fame)}, //Ring Of Paramount Magic T4
            {0xac7, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Ring Of Exalted Attack T5
            {0xac8, new Tuple<int, CurrencyType>(400, CurrencyType.Fame)}, //Ring Of Exalted Defense T5
            {0xac9, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Ring Of Exalted Speed T5
            {0xaca, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Ring Of Exalted Vitality T5
            {0xacb, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Ring Of Exalted Wisdom T5
            {0xacc, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Ring Of Exalted Dexterity T5
            {0xacd, new Tuple<int, CurrencyType>(400, CurrencyType.Fame)}, //Ring Of Exalted Health T5
            {0xace, new Tuple<int, CurrencyType>(400, CurrencyType.Fame)}, //Ring Of Exalted Magic T5
            {0xbaa, new Tuple<int, CurrencyType>(120, CurrencyType.Gold)}, //Ring Of Unbound Magic T6

            //UT ARMORS
            {0x21a8, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Robe of the Twilight
            {0x2360, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Souless Robe
            {0x21a1, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Mantle Of skuld
            {0x2364, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Fairy Plate armor
            {0xc6e, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Resurrected Armor
            {0x2337, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Almandine Armor of Anger
            {0xc18, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Coral Silk
            {0xc28, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Spectral Armor
                        {0xb05, new Tuple<int, CurrencyType>(900, CurrencyType.Fame)}, //Robe of the Grand Sorcerer
            {0xa96, new Tuple<int, CurrencyType>(425, CurrencyType.Fame)}, //Robe of the Elder Warlock T12
            {0xa95, new Tuple<int, CurrencyType>(250, CurrencyType.Fame)}, //Robe of the Moon Wizard T11
            {0xa94, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Robe of the Shadow Magus T10
            {0xa60, new Tuple<int, CurrencyType>(51, CurrencyType.Fame)}, //Robe of the Master T9
            {0xafc, new Tuple<int, CurrencyType>(900, CurrencyType.Fame)}, //Acropolis Armor T13
            {0xa93, new Tuple<int, CurrencyType>(450, CurrencyType.Fame)}, //Abyssal Armor T12
            {0xa92, new Tuple<int, CurrencyType>(250, CurrencyType.Fame)}, //Vengeance Armor T11
            {0xa91, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Desolation Armor T10
            {0xa13, new Tuple<int, CurrencyType>(51, CurrencyType.Fame)}, //Dragonscale Armor T9
            {0xaf9, new Tuple<int, CurrencyType>(900, CurrencyType.Fame)}, //Hydra Skin Armor T13
            {0xa90, new Tuple<int, CurrencyType>(450, CurrencyType.Fame)}, //Griffon Hide Armor T12
            {0xa8f, new Tuple<int, CurrencyType>(250, CurrencyType.Fame)}, //Hippogriff Hide Armor t11
            {0xa8e, new Tuple<int, CurrencyType>(100, CurrencyType.Fame)}, //Roc Leather Armor T10
            {0xad3, new Tuple<int, CurrencyType>(51, CurrencyType.Fame)}, //Drake Hide Armor T9
     
            //UTABILITIES
            {0xc1e, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Tome Of Holy Protection
            {0x235E, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Book of Geb
            {0xc0f, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Shield of Omgur
            {0x2339, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Shield of the mad god
            {0x21a9, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Ancient Spell
            {0xc06, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Seal of Blasphemous Prayer
            {0x2366, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Seal of the Enchanted Forest
            {0x21a2, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Ghastly Drape
            {0xa5a, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Plane Walker
            {0xc07, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Quiver of Thunder
            {0xc08, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Helm of the Juggernaut
            {0xc6d, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Plague Poison
            {0x911, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Skull of Endless Torment
            {0xc1c, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)},//Coral Venom Trap
            {0xc0b, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Orb of Conflict
            {0x912, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Prism of Dancing Swords
            {0xc30, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Scepter of Fulmination
            {0xc59, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Doom Circle T6
            {0x916, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //midnight Star
            {0xb25, new Tuple<int, CurrencyType>(750, CurrencyType.Fame)},
            {0xa5b, new Tuple<int, CurrencyType>(200,CurrencyType.Fame)},
            {0xb22, new Tuple<int, CurrencyType>(750, CurrencyType.Fame)},
            {0xa0c, new Tuple<int, CurrencyType>(200,CurrencyType.Fame)},
            {0xb24, new Tuple<int, CurrencyType>(750, CurrencyType.Fame)},
            {0xa30, new Tuple<int, CurrencyType>(200,CurrencyType.Fame)},
            {0xb26, new Tuple<int, CurrencyType>(750, CurrencyType.Fame)},
            {0xa55, new Tuple<int, CurrencyType>(200,CurrencyType.Fame)},
            {0xb27, new Tuple<int, CurrencyType>(750, CurrencyType.Fame)},
            {0xae1, new Tuple<int, CurrencyType>(200,CurrencyType.Fame)},
            {0xb28, new Tuple<int, CurrencyType>(750, CurrencyType.Fame)},
            {0xa65, new Tuple<int, CurrencyType>(200,CurrencyType.Fame)},
            {0xb29, new Tuple<int, CurrencyType>(750, CurrencyType.Fame)},
            {0xa6b, new Tuple<int, CurrencyType>(200,CurrencyType.Fame)},
            {0xb2a, new Tuple<int, CurrencyType>(750, CurrencyType.Fame)},
            {0xaa8, new Tuple<int, CurrencyType>(200,CurrencyType.Fame)},
            {0xb2b, new Tuple<int, CurrencyType>(750, CurrencyType.Fame)},
            {0xaaf, new Tuple<int, CurrencyType>(200,CurrencyType.Fame)},
            {0xb2c, new Tuple<int, CurrencyType>(750, CurrencyType.Fame)},
            {0xab6, new Tuple<int, CurrencyType>(200,CurrencyType.Fame)},
            {0xb2d, new Tuple<int, CurrencyType>(750, CurrencyType.Fame)},
            {0xa46, new Tuple<int, CurrencyType>(200,CurrencyType.Fame)},
            {0xb23, new Tuple<int, CurrencyType>(750, CurrencyType.Fame)},
            {0xb20, new Tuple<int, CurrencyType>(200,CurrencyType.Fame)},
            {0xb33, new Tuple<int, CurrencyType>(750, CurrencyType.Fame)},
            {0xb32, new Tuple<int, CurrencyType>(200,CurrencyType.Fame)},
            {0xc58, new Tuple<int, CurrencyType>(200,CurrencyType.Fame)},

            //PET FOOD and DIVINE EGGS
            {0xccc, new Tuple<int, CurrencyType>(1500, CurrencyType.Fame)}, //Ambrosia
            {0xccb, new Tuple<int, CurrencyType>(60, CurrencyType.Fame)}, //Fries
            {0xcca, new Tuple<int, CurrencyType>(360, CurrencyType.Fame)}, //Grapes Of Wrath
            {0xcc9, new Tuple<int, CurrencyType>(20, CurrencyType.Fame)}, //Soft Drink
            {0xcc8, new Tuple<int, CurrencyType>(420, CurrencyType.Fame)}, //Superburger
            {0xcc7, new Tuple<int, CurrencyType>(720, CurrencyType.Fame)}, //Double Cheese Burger Deluxe
            {0xcc6, new Tuple<int, CurrencyType>(120, CurrencyType.Fame)}, //Great Taco
            {0xcc5, new Tuple<int, CurrencyType>(180, CurrencyType.Fame)}, //Power Pizza
            {0xcc4, new Tuple<int, CurrencyType>(240, CurrencyType.Fame)}, //Chocolate Ice Cream 
            {0x4008, new Tuple<int, CurrencyType>(3000, CurrencyType.Fame)}, //Toxic Ambrosia
            {0x4005, new Tuple<int, CurrencyType>(5000, CurrencyType.Fame)}, //Potion of Mazy
         
            //EGGS
            {0xc86, new Tuple<int, CurrencyType>(750, CurrencyType.Gold)}, //uncommon feline egg
            {0xc87, new Tuple<int, CurrencyType>(500, CurrencyType.Gold)}, //rare feline egg
            {0xc8a, new Tuple<int, CurrencyType>(750, CurrencyType.Gold)}, //uncommon canine egg
            {0xc8b, new Tuple<int, CurrencyType>(500, CurrencyType.Gold)}, //rare canine egg
            {0xc8e, new Tuple<int, CurrencyType>(750, CurrencyType.Gold)}, //uncommon avian egg
            {0xc8f, new Tuple<int, CurrencyType>(500, CurrencyType.Gold)}, //rare avian egg
            {0xc92, new Tuple<int, CurrencyType>(750, CurrencyType.Gold)}, //uncommon exotic egg
            {0xc93, new Tuple<int, CurrencyType>(500, CurrencyType.Gold)}, //rare exotic egg
            {0xc96, new Tuple<int, CurrencyType>(750, CurrencyType.Gold)}, //uncommon farm egg
            {0xc97, new Tuple<int, CurrencyType>(500, CurrencyType.Gold)}, //rare farm egg
            {0xc9a, new Tuple<int, CurrencyType>(750, CurrencyType.Gold)}, //uncommon woodland egg
            {0xc9b, new Tuple<int, CurrencyType>(500, CurrencyType.Gold)}, //rare woodland egg
            {0xc9e, new Tuple<int, CurrencyType>(750, CurrencyType.Gold)}, //uncommon reptile egg
            {0xc9f, new Tuple<int, CurrencyType>(500, CurrencyType.Gold)}, //rare reptile egg
            {0xca2, new Tuple<int, CurrencyType>(750, CurrencyType.Gold)}, //uncommon insect egg
            {0xca3, new Tuple<int, CurrencyType>(500, CurrencyType.Gold)}, //rare insect egg
            {0xca6, new Tuple<int, CurrencyType>(750, CurrencyType.Gold)}, //uncommon pinguin egg
            {0xca7, new Tuple<int, CurrencyType>(500, CurrencyType.Gold)}, //rare pinguin egg
            {0xcaa, new Tuple<int, CurrencyType>(750, CurrencyType.Gold)}, //uncommon aquatic egg
            {0xcab, new Tuple<int, CurrencyType>(500, CurrencyType.Gold)}, //rare aquatic egg
            {0xcae, new Tuple<int, CurrencyType>(750, CurrencyType.Gold)}, //uncommon spooky egg
            {0xcaf, new Tuple<int, CurrencyType>(500, CurrencyType.Gold)}, //rare spooky egg
            {0xcb2, new Tuple<int, CurrencyType>(750, CurrencyType.Gold)}, //uncommon humanoid egg
            {0xcb3, new Tuple<int, CurrencyType>(500, CurrencyType.Gold)}, //rare humanoid egg
            {0xcb6, new Tuple<int, CurrencyType>(750, CurrencyType.Gold)}, //uncommon ???? egg
            {0xcb7, new Tuple<int, CurrencyType>(500, CurrencyType.Gold)}, //rare ???? egg
            {0xcba, new Tuple<int, CurrencyType>(750, CurrencyType.Gold)}, //uncommon automaton egg
            {0xcbb, new Tuple<int, CurrencyType>(500, CurrencyType.Gold)}, //rare automaton egg
            {0xcbe, new Tuple<int, CurrencyType>(750, CurrencyType.Gold)}, //uncommon mystery egg
            {0xcbf, new Tuple<int, CurrencyType>(500, CurrencyType.Gold)}, //rare mystery egg
            {0xc6c, new Tuple<int, CurrencyType>(750, CurrencyType.Gold)}, //backpack

            //KEYS
            {0x2290, new Tuple<int, CurrencyType>(500, CurrencyType.Fame)}, //Bella's Key - just temporary for testing

            {0x701, new Tuple<int, CurrencyType>(75, CurrencyType.Fame)}, //Undead Lair Key
            {0x705, new Tuple<int, CurrencyType>(50, CurrencyType.Fame)}, //Pirate Cave Key
            {0x70a, new Tuple<int, CurrencyType>(75, CurrencyType.Fame)}, //Abyss of Demons Key
            {0x70b, new Tuple<int, CurrencyType>(50, CurrencyType.Fame)}, //Snake Pit Key
            {0x710, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)}, //Tomb of the Ancients Key
            {0x2294, new Tuple<int, CurrencyType>(350, CurrencyType.Fame)}, //BlazeGod Key
            {0x2235, new Tuple<int, CurrencyType>(0, CurrencyType.Fame)}, //XPARea Key
            {0x71f, new Tuple<int, CurrencyType>(75, CurrencyType.Fame)}, //Sprite World Key
            {0xc11, new Tuple<int, CurrencyType>(150, CurrencyType.Fame)}, //Ocean Trench Key
            {0xc19, new Tuple<int, CurrencyType>(75, CurrencyType.Fame)}, //Totem Key
            {0xc23, new Tuple<int, CurrencyType>(150, CurrencyType.Fame)}, //Manor Key
            {0xc2e, new Tuple<int, CurrencyType>(150, CurrencyType.Fame)}, //Davy's Key
            {0xc2f, new Tuple<int, CurrencyType>(150, CurrencyType.Fame)}, //Lab Key
            {0xcce, new Tuple<int, CurrencyType>(125, CurrencyType.Fame)}, //Deadwater Docks Key
            {0xccf, new Tuple<int, CurrencyType>(125, CurrencyType.Fame)}, //Woodland Labyrinth Key
            {0xcda, new Tuple<int, CurrencyType>(125, CurrencyType.Fame)}, //The Crawling Depths Key
            {0xcdd, new Tuple<int, CurrencyType>(200, CurrencyType.Fame)},//Shatters Key
        };

        public static int[] store10List = { 0xb41, 0xbab, 0xbad, 0xbac };
        public static int[] store11List = { 0xb41, 0xbab, 0xbad, 0xbac };
        public static int[] store12List = { 0xb41, 0xbab, 0xbad, 0xbac };
        public static int[] store13List = { 0xb41, 0xbab, 0xbad, 0xbac };
        public static int[] store14List = { 0xb41, 0xbab, 0xbad, 0xbac };
        public static int[] store15List = { 0xb41, 0xbab, 0xbad, 0xbac };
        public static int[] store16List = { 0xb41, 0xbab, 0xbad, 0xbac };
        public static int[] store17List = { 0xb41, 0xbab, 0xbad, 0xbac };
        public static int[] store18List = { 0xb41, 0xbab, 0xbad, 0xbac };
        public static int[] store19List = { 0xb41, 0xbab, 0xbad, 0xbac };

        public static int[] store1List =
        {
            0xcdd, 0xcda, 0xccf, 0xcce, 0xc2f, 0xc2e, 0xc23, 0xc19, 0xc11, 0x71f, 0x710,
            0x70b, 0x70a, 0x705, 0x701, 0x2290, 0x2294, 0x2235
        };

        public static int[] store20List = { 0xb41, 0xbab, 0xbad, 0xbac };

        //eggs
        public static int[] store2List =
        {
            0xcbf, 0xcbe, 0xcbb, 0xcba, 0xcb7, 0xcb6, 0xcb2, 0xcb3, 0xcae, 0xcaf, 0xcab,
            0xcaa, 0xca7, 0xca6, 0xca3, 0xca2, 0xc9f, 0xc9e, 0xc9b, 0xc9a, 0xc97, 0xc96, 0xc93, 0xc92, 0xc8f, 0xc8e,
            0xc8b, 0xc8a, 0xc87, 0xc86, 0xc6c,
        };

        //pet food
        public static int[] store3List = { 0xccc, 0xccb, 0xcca, 0xcc9, 0xcc8, 0xcc7, 0xcc6, 0xcc5, 0xc6c, 0xcc4, 0x4008, 0x4005 };

        //abilities
        public static int[] store4List =
        {
            0xc1e, 0x235E, 0xc0f, 0x2339, 0x21a9, 0xc06, 0x2366, 0x21a2, 0xa5a, 0xc07, 0xc08, 0xb29, 0xb2a, 0xc6d, 0x911, 0xb2b, 0xc1c, 0xb2c, 0xc0b, 0xb23, 0x912, 0xb33, 0xc30, 0xc59, 0x916, 0xb25, 0xa5b, 0xb22, 0xa0c, 0xb24, 0xa30, 0xb26, 0xa55, 0xb27, 0xae1, 0xb28, 0xa65, 0xa6b, 0xaa8, 0xaaf, 0xab6, 0xb2d, 0xa46, 0xb20, 0xb32, 0xc58 
        };

        //armors
        public static int[] store5List =
        {
           0x21a8, 0x2360, 0x21a1, 0x2364, 0xc6e, 0x2337, 0xc18, 0xc28,  0xb05, 0xa96, 0xa95, 0xa94, 0xa60, 0xafc, 0xa93, 0xa92, 0xa91, 0xa13, 0xaf9,
            0xa90, 0xa8f, 0xa8e, 0xad3 
        };

        //keys
        public static int[] store6List =
        {
           0xb3f, 0x9c0, 0x9ca, 0xc04, 0xc10, 0xc02, 0x9cd, 0x2299, 0x21a7, 0x2363, 0x910
        };

        //Swords&daggers&kattana
        public static int[] store7List =
        {
           0x9c9, 0xa03, 0xc01, 0xc05, 0x2367, 0x21a0, 0xc0a, 0x9c7, 0xcdb, 0x915, 0xcdc, 0xaf6, 0xa87, 0xa86, 0xa85, 0xa07, 0xb02, 0xa8d, 0xa8c, 0xa8b, 0xa1e, 0xb08,
            0xaa2, 0xaa1, 0xaa0, 0xa9f
        };

        //rings
        public static int[] store8List =
        {
           0xbac, 0xba0, 0xba1, 0xba2, 0xba9, 0xabf, 0xac0, 0xac1, 0xac2, 0xac3, 0xac4, 0xac5, 0xac6, 0xac7, 0xac8, 0xac9,
            0xaca, 0xacb, 0xacc, 0xacd, 0xace
        };

        // rings
        public static int[] store9List = { 0xb41, 0xbab, 0xbad, 0xbac };

        private static readonly ILog log = LogManager.GetLogger(typeof(MerchantLists));

        public static void InitMerchatLists(XmlData data)
        {
            log.Info("Loading merchant lists...");
            List<int> accessoryDyeList = new List<int>();
            List<int> clothingDyeList = new List<int>();
            List<int> accessoryClothList = new List<int>();
            List<int> clothingClothList = new List<int>();

            foreach (KeyValuePair<ushort, Item> item in data.Items.Where(_ => noShopCloths.All(i => i != _.Value.ObjectId)))
            {
                if (item.Value.Texture1 != 0 && item.Value.ObjectId.Contains("Clothing") && item.Value.Class == "Dye")
                {
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(50, CurrencyType.Fame));
                    clothingDyeList.Add(item.Value.ObjectType);
                }

                if (item.Value.Texture2 != 0 && item.Value.ObjectId.Contains("Accessory") && item.Value.Class == "Dye")
                {
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(50, CurrencyType.Fame));
                    accessoryDyeList.Add(item.Value.ObjectType);
                }

                if (item.Value.Texture1 != 0 && item.Value.ObjectId.Contains("Cloth") &&
                    item.Value.ObjectId.Contains("Large"))
                {
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(600, CurrencyType.Fame));
                    clothingClothList.Add(item.Value.ObjectType);
                }

                if (item.Value.Texture2 != 0 && item.Value.ObjectId.Contains("Cloth") &&
                    item.Value.ObjectId.Contains("Small"))
                {
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(600, CurrencyType.Fame));
                    accessoryClothList.Add(item.Value.ObjectType);
                }
            }

            ClothingDyeList = clothingDyeList.ToArray();
            ClothingClothList = clothingClothList.ToArray();
            AccessoryClothList = accessoryClothList.ToArray();
            AccessoryDyeList = accessoryDyeList.ToArray();
            log.Info("Merchat lists added.");
        }

        private static readonly string[] noShopCloths =
        {
            "Large Ivory Dragon Scale Cloth", "Small Ivory Dragon Scale Cloth",
            "Large Green Dragon Scale Cloth", "Small Green Dragon Scale Cloth",
            "Large Midnight Dragon Scale Cloth", "Small Midnight Dragon Scale Cloth",
            "Large Blue Dragon Scale Cloth", "Small Blue Dragon Scale Cloth",
            "Large Red Dragon Scale Cloth", "Small Red Dragon Scale Cloth",
            "Large Jester Argyle Cloth", "Small Jester Argyle Cloth",
            "Large Alchemist Cloth", "Small Alchemist Cloth",
            "Large Mosaic Cloth", "Small Mosaic Cloth",
            "Large Spooky Cloth", "Small Spooky Cloth",
            "Large Flame Cloth", "Small Flame Cloth",
            "Large Heavy Chainmail Cloth", "Small Heavy Chainmail Cloth",
        };
    }
}