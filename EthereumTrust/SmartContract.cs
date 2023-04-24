using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework;
using System;
using System.Numerics;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace OX.SmartContract
{
    /// <summary>
    /// Contract Script Hash:0x217422aac78eace1dd15ca141779c890d27339e4
    /// </summary>
    public class OutputRestriction : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(string ethereumAddress,byte[] signature)
        {
            if (ethereumAddress == null) return false;
            if (ethereumAddress.Length == 0) return false;
            Transaction tx = OX.SmartContract.Framework.Services.System.ExecutionEngine.ScriptContainer as Transaction;
            foreach (var attr in tx.GetAttributes())
            {
                if (attr.Usage == 0xfd)
                {
                    var ethAddress = Ethereum.EcRecover(tx.InputHash, attr.Data);
                    if (ethAddress == ethereumAddress) return true;
                }
            }
            return false;
        }
    }
}
