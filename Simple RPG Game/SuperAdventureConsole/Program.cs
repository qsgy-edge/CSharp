using Engine;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace SuperAdventureConsole
{
    public class Program
    {
        private const string PLAYER_DATA_FILE_NAME = "PlayerData.xml";
        private static Player player;

        private static void Main(string[] args)
        {
            // 载入游戏数据
            LoadGameData();
            Console.WriteLine("输入'帮助'查看命令列表");
            Console.WriteLine("");
            DisplayCurrentLocation();
            // 将事件处理程序连接到Player类的事件
            player.PropertyChanged += Player_OnPropertyChanged;
            player.OnMessage += Player_OnMessage;
            // 无限循环，直到用户输入“退出”命令
            while (true)
            {
                // 显示命令提示符，等待用户输入命令
                Console.Write(">");
                string userInput = Console.ReadLine();
                // 如果用户没有输入任何内容，继续等待
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    continue;
                }
                // 将用户输入转换为小写，以便我们可以忽略大小写
                string cleanedInput = userInput.ToLower();
                // 如果用户输入“退出”，则退出程序并保存游戏数据
                if (cleanedInput == "退出")
                {
                    SaveGameData();
                    break;
                }
                // 将用户输入解析为命令和参数
                ParseInput(cleanedInput);
            }
        }

        private static void Player_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentLocation")
            {
                DisplayCurrentLocation();
                if (player.CurrentLocation.VendorWorkingHere != null)
                {
                    Console.WriteLine("你在这里看到一位商人: {0}", player.CurrentLocation.VendorWorkingHere.Name);
                }
            }
        }

        private static void Player_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Message);
            if (e.AddExtraNewLine)
            {
                Console.WriteLine("");
            }
        }

        private static void ParseInput(string input)
        {
            if (input.Contains("帮助") || input == "?")
            {
                Console.WriteLine("可用命令");
                Console.WriteLine("====================================");
                Console.WriteLine("状态 - 显示玩家信息");
                Console.WriteLine("查看 - 显示当前位置");
                Console.WriteLine("物品栏 - 显示玩家物品");
                Console.WriteLine("任务 - 显示玩家任务");
                Console.WriteLine("攻击 - 攻击怪物");
                Console.WriteLine("装备 <武器名称> - 装备指定武器");
                Console.WriteLine("喝 <药水名称> - 喝药水");
                Console.WriteLine("交易 - 交易");
                Console.WriteLine("买 <item name> - 从商人处购买物品");
                Console.WriteLine("卖 <item name> - 将物品卖给商人");
                Console.WriteLine("w - 向北移动");
                Console.WriteLine("s - 向南移动");
                Console.WriteLine("d - 向东移动");
                Console.WriteLine("a - 向西移动");
                Console.WriteLine("退出 - 退出游戏");
            }
            else if (input == "状态")
            {
                Console.WriteLine("当前生命值为： {0}", player.CurrentHitPoints);
                Console.WriteLine("最大生命值为： {0}", player.MaximumHitPoints);
                Console.WriteLine("经验值： {0}", player.ExperiencePoints);
                Console.WriteLine("等级: {0}", player.Level);
                Console.WriteLine("金币: {0}", player.Gold);
            }
            else if (input == "查看")
            {
                DisplayCurrentLocation();
            }
            else if (input.Contains("w"))
            {
                if (player.CurrentLocation.LocationToNorth == null)
                {
                    Console.WriteLine("你不能向北移动");
                }
                else
                {
                    player.MoveNorth();
                }
            }
            else if (input.Contains("d"))
            {
                if (player.CurrentLocation.LocationToEast == null)
                {
                    Console.WriteLine("你不能向东移动");
                }
                else
                {
                    player.MoveEast();
                }
            }
            else if (input.Contains("s"))
            {
                if (player.CurrentLocation.LocationToSouth == null)
                {
                    Console.WriteLine("你不能向南移动");
                }
                else
                {
                    player.MoveSouth();
                }
            }
            else if (input.Contains("a"))
            {
                if (player.CurrentLocation.LocationToWest == null)
                {
                    Console.WriteLine("你不能向西移动");
                }
                else
                {
                    player.MoveWest();
                }
            }
            else if (input == "物品栏")
            {
                foreach (InventoryItem inventoryItem in player.Inventory)
                {
                    Console.WriteLine("{0}: {1}", inventoryItem.Description, inventoryItem.Quantity);
                }
            }
            else if (input == "任务")
            {
                if (player.Quests.Count == 0)
                {
                    Console.WriteLine("你没有任何任务");
                }
                else
                {
                    foreach (PlayerQuest playerQuest in player.Quests)
                    {
                        Console.WriteLine("{0}: {1}", playerQuest.Name,
                            playerQuest.IsCompleted ? "已完成" : "未完成");
                    }
                }
            }
            else if (input.Contains("攻击"))
            {
                if (player.CurrentLocation.MonsterLivingHere == null)
                {
                    Console.WriteLine("这里没有怪物");
                }
                else
                {
                    if (player.CurrentWeapon == null)
                    {
                        // 如果玩家没有武器，就用默认武器攻击
                        player.CurrentWeapon = player.Weapons.FirstOrDefault();
                    }
                    if (player.CurrentWeapon == null)
                    {
                        Console.WriteLine("你没有武器，无法攻击");
                    }
                    else
                    {
                        player.UseWeapon(player.CurrentWeapon);
                    }
                }
            }
            else if (input.StartsWith("装备 "))
            {
                string inputWeaponName = input.Substring(2).Trim();
                if (string.IsNullOrEmpty(inputWeaponName))
                {
                    Console.WriteLine("你必须输入要装备的武器名称");
                }
                else
                {
                    Weapon weaponToEquip =
                        player.Weapons.SingleOrDefault(
                            x => x.Name.ToLower() == inputWeaponName);
                    if (weaponToEquip == null)
                    {
                        Console.WriteLine("你没有： {0}", inputWeaponName);
                    }
                    else
                    {
                        player.CurrentWeapon = weaponToEquip;
                        Console.WriteLine("你装备了 {0}", player.CurrentWeapon.Name);
                    }
                }
            }
            else if (input.StartsWith("喝 "))
            {
                string inputPotionName = input.Substring(1).Trim();
                if (string.IsNullOrEmpty(inputPotionName))
                {
                    Console.WriteLine("你必须输入要喝的药水名称");
                }
                else
                {
                    HealingPotion potionToDrink =
                        player.Potions.SingleOrDefault(
                            x => x.Name.ToLower() == inputPotionName);
                    if (potionToDrink == null)
                    {
                        Console.WriteLine("你没有： {0}", inputPotionName);
                    }
                    else
                    {
                        if (player.CurrentHitPoints == player.MaximumHitPoints)
                        {
                            Console.WriteLine("你已经满血了，不能再喝药水了");
                        }
                        else
                        {
                            player.UsePotion(potionToDrink);
                        }
                    }
                }
            }
            else if (input == "交易")
            {
                if (player.CurrentLocation.VendorWorkingHere == null)
                {
                    Console.WriteLine("这里没有商人");
                }
                else
                {
                    Console.WriteLine("玩家物品");
                    Console.WriteLine("================");
                    if (player.Inventory.Count(x => x.Price != World.UNSELLABLE_ITEM_PRICE) == 0)
                    {
                        Console.WriteLine("你没有任何可以卖的物品");
                    }
                    else
                    {
                        foreach (
                            InventoryItem inventoryItem in player.Inventory.Where(x => x.Price != World.UNSELLABLE_ITEM_PRICE))
                        {
                            Console.WriteLine("{0} {1} 价格： {2}", inventoryItem.Quantity, inventoryItem.Description,
                                inventoryItem.Price);
                        }
                    }
                    Console.WriteLine("");
                    Console.WriteLine("商人物品");
                    Console.WriteLine("================");
                    if (player.CurrentLocation.VendorWorkingHere.Inventory.Count == 0)
                    {
                        Console.WriteLine("商人没有任何物品");
                    }
                    else
                    {
                        foreach (InventoryItem inventoryItem in player.CurrentLocation.VendorWorkingHere.Inventory)
                        {
                            Console.WriteLine("{0} {1} 价格： {2}", inventoryItem.Quantity, inventoryItem.Description,
                                inventoryItem.Price);
                        }
                    }
                }
            }
            else if (input.StartsWith("买 "))
            {
                if (player.CurrentLocation.VendorWorkingHere == null)
                {
                    Console.WriteLine("这里没有商人");
                }
                else
                {
                    string itemName = input.Substring(1).Trim();
                    if (string.IsNullOrEmpty(itemName))
                    {
                        Console.WriteLine("你必须输入要购买的物品名称");
                    }
                    else
                    {
                        // 从商人的库存中获取InventoryItem
                        InventoryItem itemToBuy =
                            player.CurrentLocation.VendorWorkingHere.Inventory.SingleOrDefault(
                                x => x.Details.Name.ToLower() == itemName);
                        // 检查商人是否有玩家输入的物品
                        if (itemToBuy == null)
                        {
                            Console.WriteLine("商人没有 {0}", itemName);
                        }
                        else
                        {
                            // 检查玩家是否有足够的金币购买物品
                            if (player.Gold < itemToBuy.Price)
                            {
                                Console.WriteLine("You do not have enough gold to buy a {0}", itemToBuy.Description);
                            }
                            else
                            {
                                // 将物品添加到玩家的库存中
                                player.AddItemToInventory(itemToBuy.Details);
                                player.Gold -= itemToBuy.Price;
                                Console.WriteLine("你用 {0} 金币购买了 {1}", itemToBuy.Price, itemToBuy.Details.Name);
                            }
                        }
                    }
                }
            }
            else if (input.StartsWith("卖 "))
            {
                if (player.CurrentLocation.VendorWorkingHere == null)
                {
                    Console.WriteLine("这里没有商人");
                }
                else
                {
                    string itemName = input.Substring(1).Trim();
                    if (string.IsNullOrEmpty(itemName))
                    {
                        Console.WriteLine("你必须输入要卖的物品名称");
                    }
                    else
                    {
                        // 获取玩家库存中的InventoryItem
                        InventoryItem itemToSell =
                            player.Inventory.SingleOrDefault(x => x.Details.Name.ToLower() == itemName &&
                                                                   x.Quantity > 0 &&
                                                                   x.Price != World.UNSELLABLE_ITEM_PRICE);
                        // 检查玩家是否有玩家输入的物品
                        if (itemToSell == null)
                        {
                            Console.WriteLine("你没有 {0}", itemName);
                        }
                        else
                        {
                            // 从玩家库存中移除物品
                            player.RemoveItemFromInventory(itemToSell.Details);
                            player.Gold += itemToSell.Price;
                            Console.WriteLine("你卖出了 {0} 获得 {1} 金币", itemToSell.Price, itemToSell.Details.Name);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("我不明白你的意思");
                Console.WriteLine("输入'帮助'查看可用命令");
            }
            // 空行
            Console.WriteLine("");
        }

        private static void DisplayCurrentLocation()
        {
            Console.WriteLine("你位于： {0}", player.CurrentLocation.Name);
            if (player.CurrentLocation.Description != "")
            {
                Console.WriteLine(player.CurrentLocation.Description);
            }
        }

        private static void LoadGameData()
        {
            player = PlayerDataMapper.CreateFromDatabase();
            if (player == null)
            {
                if (File.Exists(PLAYER_DATA_FILE_NAME))
                {
                    player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));
                }
                else
                {
                    player = Player.CreateDefaultPlayer();
                }
            }
        }

        private static void SaveGameData()
        {
            File.WriteAllText(PLAYER_DATA_FILE_NAME, player.ToXmlString());
            PlayerDataMapper.SaveToDatabase(player);
        }
    }
}