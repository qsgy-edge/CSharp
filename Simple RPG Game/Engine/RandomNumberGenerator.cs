using System;
using System.Security.Cryptography;

namespace Engine
{
    // 随机数生成器
    public static class RandomNumberGenerator
    {
        private static readonly RNGCryptoServiceProvider generator = new RNGCryptoServiceProvider();

        public static int NumberBetween(int miniumValue, int maximumValue)
        {
            byte[] randomNumber = new byte[1];
            generator.GetBytes(randomNumber);
            double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);
            // 使用 Math.Max, 以防止出现除零错误
            // 为什么要除以 255？因为我们的随机数是一个字节，所以最大值为 255
            // 如果我们不除以 255，我们的随机数将永远不会大于 1/255
            double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);
            // 我们现在将随机数乘以我们的范围，然后向下舍入
            // 这将产生一个从 0 到我们的范围的随机数
            int range = maximumValue - miniumValue + 1;
            double randomValueInRange = Math.Floor(multiplier * range);
            return (int)(miniumValue + randomValueInRange);
        }
    }
}