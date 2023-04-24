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
    /// Contract Script Hash:0xeefc59fc3e863f8b6cccd916fd448e07bffd9c99
    /// </summary>
    public class OutputRestriction : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(string ethereumAddress)
        {
            if (ethereumAddress == null) return false;
            if (ethereumAddress.Length == 0) return false;
            Transaction tx = OX.SmartContract.Framework.Services.System.ExecutionEngine.ScriptContainer as Transaction;
            try
            {
                foreach (var attr in tx.GetAttributes())
                {
                    if (attr.Usage == 0xfd)
                    {
                        var ethAddress = Ethereum.EcRecover(tx.InputHash, attr.Data);
                        if (ethAddress != null && ethAddress.Length > 0 && ethAddress == ethereumAddress) return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
