using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework;
using System;
using System.Numerics;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Collections.Concurrent;

namespace OX.SmartContract
{
    /// <summary>
    /// Contract Script Hash:0xe64586c07a90ec1a1b0c8fc22868cf3eff94560b
    /// </summary>
    public class OTCWithdrawal : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(uint expirationIndex, byte[] to, byte[] from, byte[] pubkey, byte[] signature)
        {
            Transaction tx = OX.SmartContract.Framework.Services.System.ExecutionEngine.ScriptContainer as Transaction;
            var fromStr = from.AsString();
            var toStr = to.AsString();
            if (expirationIndex > Blockchain.GetHeight())
            {
                foreach (var output in tx.GetOutputs())
                {
                    if (toStr != output.ScriptHash.AsString()) return false;
                }
                return VerifySignature(signature, pubkey) || Runtime.CheckWitness(pubkey);
            }
            else
            {
                foreach (var output in tx.GetOutputs())
                {
                    if (fromStr != output.ScriptHash.AsString()) return false;
                }
                return true;
            }
        }
    }
}
