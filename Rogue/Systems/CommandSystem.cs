﻿using Rogue.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Systems
{
    public class CommandSystem
    {
        // 移动玩家，返回是否成功移动
        public bool MovePlayer(Direction direction)
        {
            int x = Game.Player.X;
            int y = Game.Player.Y;

            switch (direction)
            {
                case Direction.Up:
                    {
                        y = Game.Player.Y - 1;
                        break;
                    }
                case Direction.Down:
                    {
                        y = Game.Player.Y + 1;
                        break;
                    }
                case Direction.Left:
                    {
                        x = Game.Player.X - 1;
                        break;
                    }
                case Direction.Right:
                    {
                        x = Game.Player.X + 1;
                        break;
                    }
                default:
                    {
                        return false;
                    }
            }

            if (Game.DungeonMap.SetActorPosition(Game.Player, x, y))
            {
                return true;
            }

            return false;
        }
    }
}