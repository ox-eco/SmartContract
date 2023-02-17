using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework;
using System;
using System.Numerics;
using System.Linq;

namespace OX.SmartContract
{
    public class OutputRestriction : OX.SmartContract.Framework.SmartContract
    {

        public static readonly byte[] OXC = "0xa18da0f97874a798e46dc037be111c685ee7313375b790913b8aeb6ae920cd46".HexToBytes();
        public static bool Main(byte[] scriptHash, byte[] pubkey, byte[] signature)
        {
            var contractSH = OX.SmartContract.Framework.Services.System.ExecutionEngine.ExecutingScriptHash;
            Transaction tx = OX.SmartContract.Framework.Services.System.ExecutionEngine.ScriptContainer as Transaction;
            var inputs = tx.GetInputs();
            if (inputs.Length != 1) return false;
            var refer = tx.GetReferences().FirstOrDefault();
            if (Equals(refer.AssetId, OXC))
            {
                var input = inputs.FirstOrDefault();
                var referTx = Blockchain.GetTransaction(input.PrevHash);
                How to get the block through transaction?
            }
            //var outputs = tx.GetOutputs();
            return VerifySignature(signature, pubkey);
        }
        public static bool Equals(byte[] A, byte[] B)
        {
            if (A.Length != B.Length)
                return false;
            return A.SequenceEqual(B);
        }
    }
}
