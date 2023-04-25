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
    /// Contract Script Hash:0x7936b6048164a204d18bda47e0871c20892be824
    /// </summary>
    public class OutputRestriction : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(uint lockExpirationIndex,string ethereumAddress,byte[] signature)
        {
            if (lockExpirationIndex > Blockchain.GetHeight()) return false;
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
