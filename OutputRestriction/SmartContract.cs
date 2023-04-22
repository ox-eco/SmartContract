using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework;
using System;
using System.Numerics;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;

namespace OX.SmartContract
{
    /// <summary>
    /// Contract Script Hash:0x0f23223738f58f8aabe8da81d0029c65e3a76d25
    /// </summary>
    public class OutputRestriction : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(bool mustFlag, byte[] sideScopes, byte[] targets, byte[] ownerPubKey, byte[] pubkey, byte[] signature)
        {
            Transaction tx = OX.SmartContract.Framework.Services.System.ExecutionEngine.ScriptContainer as Transaction;
            try
            {
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
                byte[] bs = new byte[0];
                if (sideScopes != default && sideScopes.Length > 0)
                {
                    var k = sideScopes.Length / 20;
                    for (int i = 0; i < k; i++)
                    {
                        var sideScriptHash = sideScopes.Range(i * 20, 20);
                        bs.Concat(sideScriptHash);
                        var sshs = Blockchain.GetSides(sideScriptHash, "0x1bb1483c8c1175b37062d7d586bd4b67abb255e2");
                        if (sshs != default && sshs.Length > 0)
                        {
                            foreach (var ssh in sshs)
                            {
                                bs = bs.Concat(ssh);
                            }
                        }
                    }
                }
                var c = targets != default ? targets.Length / 20 : 0;
                var m = bs != default ? bs.Length / 20 : 0;
                foreach (var output in tx.GetOutputs())
                {
                    bool ok = false;
                    var s = output.ScriptHash.AsString();
                    if (c > 0)
                    {
                        for (int i = 0; i < c; i++)
                        {
                            if (s == targets.Range(i * 20, 20).AsString())
                            {
                                ok = true;
                                break;
                            }
                        }
                    }
                    if (!ok && m > 0)
                    {
                        for (int i = 0; i < m; i++)
                        {
                            if (s == bs.Range(i * 20, 20).AsString())
                            {
                                ok = true;
                                break;
                            }
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
            catch
            {
                return false;
            }
        }
    }
}
