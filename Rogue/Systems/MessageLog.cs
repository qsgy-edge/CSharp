using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.Systems
{
    public class MessageLog
    {
        private static readonly int maxLines = 10;
        private readonly Queue<string> messages;

        // 构造函数
        public MessageLog()
        {
            messages = new Queue<string>();
        }

        // 添加新消息
        public void Add(string message)
        {
            messages.Enqueue(message);

            // 如果消息数量超过最大值，删除最早的消息
            if (messages.Count > maxLines)
            {
                messages.Dequeue();
            }
        }

        // 渲染消息到控制台
        public void Draw(RLConsole console)
        {
            string[] messageArray = messages.ToArray();
            for (int i = 0; i < messageArray.Length; i++)
            {
                console.Print(1, i + 1, messageArray[i], RLColor.White);
            }
        }
    }
}