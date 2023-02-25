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
    /// Contract Script Hash:0xf7f5a79f40336f1e9bf9ee9bef93651eaab9f04e
    /// </summary>
    public class OutputRestriction : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(bool mustFlag, byte[] scriptHashes, byte[] ownerPubKey, byte[] pubkey, byte[] signature)
        {
            Transaction tx = OX.SmartContract.Framework.Services.System.ExecutionEngine.ScriptContainer as Transaction;
            var c = scriptHashes.Length / 20;
            foreach (var output in tx.GetOutputs())
            {
                bool ok = false;
                var s = output.ScriptHash.AsString();
                for (int i = 0; i < c; i++)
                {
                    if (output.ScriptHash.AsString() == scriptHashes.Range(i * 20, 20).AsString())
                    {
                        ok = true;
                        break;
                    }
                }
                if (!ok) return false;
            }
            if (mustFlag)
            {
                bool flaged = false;
                foreach (var attr in tx.GetAttributes())
                {
                    if (attr.Usage == 0xff)
                    {
                        if (attr.Data.AsString() != ownerPubKey.AsString())
                        {
                            return false;
                        }
                        flaged = true;
                    }
                }
                if (!flaged) return false;
            }
            return VerifySignature(signature, pubkey);
        }
    }
}
