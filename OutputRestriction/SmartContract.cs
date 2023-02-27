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
    /// Contract Script Hash:0x66c52b843fca2c6c5cba7cd58583d1d2db070de3
    /// </summary>
    public class OutputRestriction : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(bool mustFlag, byte[] scriptHashes, byte[] ownerPubKey, byte[] pubkey, byte[] signature)
        {
            Transaction tx = OX.SmartContract.Framework.Services.System.ExecutionEngine.ScriptContainer as Transaction;
            string sh = "";
            foreach (var refer in tx.GetReferences())
            {
                var r = refer.ScriptHash.AsString();
                if (sh == "")
                {
                    sh = r;
                }
                else if (sh != r)
                {
                    return false;
                }
            }
            var c = scriptHashes.Length / 20;
            foreach (var output in tx.GetOutputs())
            {
                bool ok = false;
                var s = output.ScriptHash.AsString();
                for (int i = 0; i < c; i++)
                {
                    if (s == scriptHashes.Range(i * 20, 20).AsString())
                    {
                        ok = true;
                        break;
                    }
                }
                if (s == sh) ok = true;
                if (!ok) return false;
            }
            //must flag ScriptHash or Public Key
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
                    if (attr.Usage == 0xfe)
                    {
                        if (attr.Data.AsString() != sh)
                        {
                            return false;
                        }
                        flaged = true;
                    }
                }
                if (!flaged) return false;
            }
            return VerifySignature(signature, pubkey) || VerifySignature(signature, ownerPubKey) || Runtime.CheckWitness(pubkey) || Runtime.CheckWitness(ownerPubKey);
        }
    }
}
