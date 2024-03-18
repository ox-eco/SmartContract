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
    /// 0x80ca930c74f152a74f222249e8136a77873f3f84
    /// </summary>
    public class FlashStateInterval : Framework.SmartContract
    {
        public static object Main(string operation, params object[] args)
        {
            switch (operation)
            {
                case "getflashstateinterval":
                    return GetFlashStateInterval((int)args[0], (int)args[1], (int)args[2], (long)args[3]);
                default:
                    return false;
            }
        }

        [DisplayName("getFlashStateInterval")]
        public static int GetFlashStateInterval(int txPoolCount, int balanceMultiple, int flashStateNumber, long totalOXSBalance)
        {
            return 0;
        }


    }
}
