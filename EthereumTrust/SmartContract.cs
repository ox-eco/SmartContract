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
    /// Contract Script Hash:0x81f1f7cb058c498e68e74ab813d1c377af36d209
    /// </summary>
    public class OutputRestriction : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(string ethereumAddress)
        {
            Transaction tx = OX.SmartContract.Framework.Services.System.ExecutionEngine.ScriptContainer as Transaction;
            try
            {
                foreach (var attr in tx.GetAttributes())
                {
                    if (attr.Usage == 0xfd)
                    {
                        var ethAddress = Ethereum.EncodeUTF8AndEcRecover(tx.Hash.AsString(), attr.Data.AsString());
                        if (ethAddress.ToLower() == ethereumAddress.ToLower()) return true;
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
