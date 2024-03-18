using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework.Services.System;
using OX.SmartContract.Framework;
using System;
using System.Numerics;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.ComponentModel;

namespace OX.SmartContract
{
    /// <summary>
    /// 0x1506f6a646499c41cd6529fa9cdd9cd57ac34918
    /// </summary>
    public class FlashStateInterval : Framework.SmartContract
    {
        public static object Main(string operation, params object[] args)
        {
            switch (operation)
            {
                case "getflashstateinterval":
                    return GetFlashStateInterval((int)args[0], (int)args[1], (long)args[2]);                
                default:
                    return false;
            }
        }

        [DisplayName("getFlashStateInterval")]
        public static int GetFlashStateInterval(int balanceMultiple, int flashStateNumber, long totalOXSBalance)
        {
            if (balanceMultiple < 1) return 0;
            if (balanceMultiple >= 1000) return 2;
            if (balanceMultiple >= 100)
            {
                return flashStateNumber > 5000 ? 10 : 2;
            }
            if (balanceMultiple >= 10)
            {
                if (flashStateNumber < 1000) return 2;
                else if (flashStateNumber < 5000) return 10;
                else return 100;
            }
            if (flashStateNumber == 0 || flashStateNumber > 5000) return 0;
            var k = totalOXSBalance / 10000;
            if (k > 100_000_000L * 5000) return 0;
            if (k > 100_000_000L * 2000)
                return flashStateNumber < 1000 ? 10 : 100;
            else
                return flashStateNumber < 1000 ? 2 : 10;
        }
       

    }
}
