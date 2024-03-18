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
    /// 0x5e7cc60fba6bb7bd77c04f378435bb4dd52c512b
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
            return 0;
        }


    }
}
