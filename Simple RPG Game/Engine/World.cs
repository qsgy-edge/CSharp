using System.Collections.Generic;

namespace Engine
{
    public static class World
    {
        public static readonly List<Item> Items = new List<Item>();
        public static readonly List<Monster> Monsters = new List<Monster>();
        public static readonly List<Quest> Quests = new List<Quest>();
        public static readonly List<Location> Locations = new List<Location>();
        public const int UNSELLABLE_ITEM_PRICE = -1;
        public const int ITEM_ID_RUSTY_SWORD = 1;
        public const int ITEM_ID_RAT_TAIL = 2;
        public const int ITEM_ID_PIECE_OF_FUR = 3;
        public const int ITEM_ID_SNAKE_FANG = 4;
        public const int ITEM_ID_SNAKESKIN = 5;
        public const int ITEM_ID_CLUB = 6;
        public const int ITEM_ID_HEALING_POTION = 7;
        public const int ITEM_ID_SPIDER_FANG = 8;
        public const int ITEM_ID_SPIDER_SILK = 9;
        public const int ITEM_ID_ADVENTURER_PASS = 10;
        public const int MONSTER_ID_RAT = 1;
        public const int MONSTER_ID_SNAKE = 2;
        public const int MONSTER_ID_GIANT_SPIDER = 3;
        public const int QUEST_ID_CLEAR_ALCHEMIST_GARDEN = 1;
        public const int QUEST_ID_CLEAR_FARMERS_FIELD = 2;
        public const int LOCATION_ID_HOME = 1;
        public const int LOCATION_ID_TOWN_SQUARE = 2;
        public const int LOCATION_ID_GUARD_POST = 3;
        public const int LOCATION_ID_ALCHEMIST_HUT = 4;
        public const int LOCATION_ID_ALCHEMISTS_GARDEN = 5;
        public const int LOCATION_ID_FARMHOUSE = 6;
        public const int LOCATION_ID_FARM_FIELD = 7;
        public const int LOCATION_ID_BRIDGE = 8;
        public const int LOCATION_ID_SPIDER_FIELD = 9;

        static World()
        {
            PopulateItems();
            PopulateMonsters();
            PopulateQuests();
            PopulateLocations();
        }

        private static void PopulateItems()
        {
            Items.Add(new Weapon(ITEM_ID_RUSTY_SWORD, "生锈的剑", 0, 5,5));
            Items.Add(new Item(ITEM_ID_RAT_TAIL, "老鼠尾巴",1));
            Items.Add(new Item(ITEM_ID_PIECE_OF_FUR, "皮毛",1));
            Items.Add(new Item(ITEM_ID_SNAKE_FANG, "蛇牙",1));
            Items.Add(new Item(ITEM_ID_SNAKESKIN, "蛇皮",2));
            Items.Add(new Weapon(ITEM_ID_CLUB, "棍子", 3, 10,8));
            Items.Add(new HealingPotion(ITEM_ID_HEALING_POTION, "治疗药水", 5,3));
            Items.Add(new Item(ITEM_ID_SPIDER_FANG, "蜘蛛牙",1));
            Items.Add(new Item(ITEM_ID_SPIDER_SILK, "蜘蛛丝",1));
            Items.Add(new Item(ITEM_ID_ADVENTURER_PASS, "勇者通行证",UNSELLABLE_ITEM_PRICE));
        }

        private static void PopulateMonsters()
        {
            Monster rat = new Monster(MONSTER_ID_RAT, "老鼠", 5, 3, 10, 3, 3);
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_RAT_TAIL), 75, false));
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_PIECE_OF_FUR), 75, true));
            Monster snake = new Monster(MONSTER_ID_SNAKE, "蛇", 5, 3, 10, 3, 3);
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKE_FANG), 75, false));
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKESKIN), 75, true));
            Monster giantSpider = new Monster(MONSTER_ID_GIANT_SPIDER, "巨型蜘蛛", 20, 5, 40, 10, 10);
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_FANG), 75, true));
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_SILK), 25, false));
            Monsters.Add(rat);
            Monsters.Add(snake);
            Monsters.Add(giantSpider);
        }

        private static void PopulateQuests()
        {
            Quest clearAlchemistGarden =
                new Quest(
                    QUEST_ID_CLEAR_ALCHEMIST_GARDEN,
                    "清理炼金术士的花园",
                    "杀死炼金术士花园里的老鼠，带回 3 条老鼠尾巴。您将收到一瓶治疗药水和 10 个金币。", 20, 10);
            clearAlchemistGarden.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_RAT_TAIL), 3));
            clearAlchemistGarden.RewardItem = ItemByID(ITEM_ID_HEALING_POTION);
            Quest clearFarmersField =
                new Quest(
                    QUEST_ID_CLEAR_FARMERS_FIELD,
                    "清理农民的田地",
                    "杀死农田里的蛇，带回3个蛇牙。您将收到一张冒险家通行证和 20 枚金币。", 20, 20);
            clearFarmersField.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_SNAKE_FANG), 3));
            clearFarmersField.RewardItem = ItemByID(ITEM_ID_ADVENTURER_PASS);
            Quests.Add(clearAlchemistGarden);
            Quests.Add(clearFarmersField);
        }

        private static void PopulateLocations()
        {
            // Create each location
            Location home = new Location(LOCATION_ID_HOME, "家", "你的房子 你真得好好收拾收拾了");
            Location townSquare = new Location(LOCATION_ID_TOWN_SQUARE, "城镇广场", "你看到一个喷泉。");
            Location alchemistHut = new Location(LOCATION_ID_ALCHEMIST_HUT, "炼金术士的小屋", "货架上有很多奇怪的植物。");
            alchemistHut.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_ALCHEMIST_GARDEN);
            Location alchemistsGarden = new Location(LOCATION_ID_ALCHEMISTS_GARDEN, "炼金术士的花园", "这里生长着许多植物。");
            alchemistsGarden.MonsterLivingHere = MonsterByID(MONSTER_ID_RAT);
            Location farmhouse = new Location(LOCATION_ID_FARMHOUSE, "农舍", "有一座小农舍，前面有一个农夫。");
            farmhouse.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_FARMERS_FIELD);
            Location farmersField = new Location(LOCATION_ID_FARM_FIELD, "农民的田地", "你会看到这里长着一排排的蔬菜。");
            farmersField.MonsterLivingHere = MonsterByID(MONSTER_ID_SNAKE);
            Location guardPost = new Location(LOCATION_ID_GUARD_POST, "岗哨", "这里有一个又大又难看的守卫。", ItemByID(ITEM_ID_ADVENTURER_PASS));
            Location bridge = new Location(LOCATION_ID_BRIDGE, "桥", "一座石桥横跨宽阔的河流。");
            Location spiderField = new Location(LOCATION_ID_SPIDER_FIELD, "森林", "你看到蜘蛛网覆盖了这片森林的树木。");
            spiderField.MonsterLivingHere = MonsterByID(MONSTER_ID_GIANT_SPIDER);
            // Link the locations together
            home.LocationToNorth = townSquare;
            townSquare.LocationToNorth = alchemistHut;
            townSquare.LocationToSouth = home;
            townSquare.LocationToEast = guardPost;
            townSquare.LocationToWest = farmhouse;
            farmhouse.LocationToEast = townSquare;
            farmhouse.LocationToWest = farmersField;
            farmersField.LocationToEast = farmhouse;
            alchemistHut.LocationToSouth = townSquare;
            alchemistHut.LocationToNorth = alchemistsGarden;
            alchemistsGarden.LocationToSouth = alchemistHut;
            guardPost.LocationToEast = bridge;
            guardPost.LocationToWest = townSquare;
            bridge.LocationToWest = guardPost;
            bridge.LocationToEast = spiderField;
            spiderField.LocationToWest = bridge;
            // Add the locations to the static list
            Locations.Add(home);
            Locations.Add(townSquare);
            Locations.Add(guardPost);
            Locations.Add(alchemistHut);
            Locations.Add(alchemistsGarden);
            Locations.Add(farmhouse);
            Locations.Add(farmersField);
            Locations.Add(bridge);
            Locations.Add(spiderField);
        }

        public static Item ItemByID(int id)
        {
            foreach (Item item in Items)
            {
                if (item.ID == id)
                {
                    return item;
                }
            }
            return null;
        }

        public static Monster MonsterByID(int id)
        {
            foreach (Monster monster in Monsters)
            {
                if (monster.ID == id)
                {
                    return monster;
                }
            }
            return null;
        }

        public static Quest QuestByID(int id)
        {
            foreach (Quest quest in Quests)
            {
                if (quest.ID == id)
                {
                    return quest;
                }
            }
            return null;
        }

        public static Location LocationByID(int id)
        {
            foreach (Location location in Locations)
            {
                if (location.ID == id)
                {
                    return location;
                }
            }
            return null;
        }
    }
}