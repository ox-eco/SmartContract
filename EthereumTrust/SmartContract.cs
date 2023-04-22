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
    /// Contract Script Hash:0x6bb2a19b06b467bd82a6df6c8532d51fb48ffcb4
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
                        var ethAddress = Ethereum.EncodeUTF8AndEcRecover(tx.Hash.AsString(), attr.Data.AsString());
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
