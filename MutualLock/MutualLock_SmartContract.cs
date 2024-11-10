using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework;
using System;
using System.Numerics;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using OX.SmartContract.Framework.Services.System;

namespace OX.SmartContract
{
    /// <summary>
    /// Contract Script Hash:0xc2e87efec3a16b5f5f1c4a1dbea2ed2b6072a4df
    /// arg:0202040406060600
    /// return:01
    /// </summary>
    public class MutualLockContract : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(uint lockExpiration, uint Amount, byte[] approveHash, byte[] assetId, byte[] buyerPubKey, byte[] sellerPubKey, byte[] arbiterPubkey, byte[] signature)
        {
            if (VerifySignature(signature, arbiterPubkey) || Runtime.CheckWitness(arbiterPubkey)) return true;
            var selfSH = OX.SmartContract.Framework.Services.System.ExecutionEngine.EntryScriptHash;
            Transaction tx = OX.SmartContract.Framework.Services.System.ExecutionEngine.ScriptContainer as Transaction;
            string sh = "";
            foreach (var refer in tx.GetReferences())
            {
                if (refer.AssetId.AsString() != assetId.AsString())
                    return false;
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
            if (selfSH.AsString() != sh) return false;
         
            var buyer_sh = Runtime.CreateSignatureRedeemScriptHash(buyerPubKey);

            Header header = Blockchain.GetHeader(Blockchain.GetHeight());
            var ts = header.Timestamp;
            var expire = ts > lockExpiration;

            var isLocked = Blockchain.GetMutualLockState(selfSH);
       
            bool ok = false;
          

            var sellerSigned = VerifySignature(signature, sellerPubKey) || Runtime.CheckWitness(sellerPubKey);
            if (isLocked)
            {
                if (sellerSigned)
                {
                    foreach (var output in tx.GetOutputs())
                    {
                        if (output.AssetId.AsString() == assetId.AsString()
                             && output.ScriptHash.AsString() == buyer_sh.AsString()
                             && output.Value >=2* 100_000_000 * Amount)
                        {
                            ok = true;
                            break;
                        }
                    }
                }
            }           
            else
            {
                if (expire)
                {
                    if (sellerSigned) ok = true;
                }
                else
                {
                    if (sellerSigned)
                    {
                        foreach (var output in tx.GetOutputs())
                        {
                            if (output.AssetId.AsString() == assetId.AsString()
                                 && output.ScriptHash.AsString() == buyer_sh.AsString()
                                 && output.Value >= 100_000_000 * Amount)
                            {
                                ok = true;
                                break;
                            }
                        }
                    }
                }
            }
            return ok;
        }
    }
}
