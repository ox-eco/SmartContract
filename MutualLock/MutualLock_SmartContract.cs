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
    /// Contract Script Hash:0xd5b2b80d0a174ad67cdb2e0ff6f0d9da56aba5f5
    /// </summary>
    public class MutualLockContract : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(uint lockExpiration, uint Amount, byte[] approveHash, byte[] assetId, byte[] buyerPubKey, byte[] sellerPubKey, byte[] arbiterPubkey, byte[] signature)
        {
            if (VerifySignature(signature, arbiterPubkey) || Runtime.CheckWitness(arbiterPubkey)) return true;
            var selfSH = OX.SmartContract.Framework.Services.System.ExecutionEngine.ExecutingScriptHash;
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

            bool approved = false;
            foreach (var attr in tx.GetAttributes())
            {
                if (attr.Usage == 0xae)
                {
                    if (Blockchain.VerifyApproveHash(approveHash, attr.Data))
                    {
                        approved = true;
                        break;
                    }
                }
            }
            if (!approved) return false;


            var seller_sh = Runtime.CreateSignatureRedeemScriptHash(sellerPubKey);
            var buyer_sh = Runtime.CreateSignatureRedeemScriptHash(buyerPubKey);

            Header header = Blockchain.GetHeader(Blockchain.GetHeight());
            var ts = header.Timestamp;
        
            var isLocked = Blockchain.GetMutualLockState(selfSH);

            if (ts > lockExpiration && !isLocked) 
            {
                return VerifySignature(signature, sellerPubKey) || Runtime.CheckWitness(sellerPubKey);
            }
            else
            {
                var buyerSigned = VerifySignature(signature, buyerPubKey) || Runtime.CheckWitness(buyerPubKey);
                if (!buyerSigned) return false;
                bool amountOk = false;
                foreach (var output in tx.GetOutputs())
                {
                    if (output.AssetId.AsString() == assetId.AsString()
                         && output.ScriptHash.AsString() == seller_sh.AsString()
                         && output.Value >= 100_000_000 * Amount)
                    {
                        amountOk = true;
                        break;
                    }
                }
                return amountOk;
            }
        }
    }
}
