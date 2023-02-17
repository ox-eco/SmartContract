using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework;
using System;
using System.Numerics;
using System.Linq;
using System.Runtime.Serialization;

namespace OX.SmartContract
{
    public class OutputRestriction : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(byte[] sh1, byte[] sh2, byte[] sh3, byte[] sh4, byte[] sh5, byte[] pubkey, byte[] signature)
        {
            Transaction tx = OX.SmartContract.Framework.Services.System.ExecutionEngine.ScriptContainer as Transaction;
            foreach (var output in tx.GetOutputs())
            {
                var ok = Equals(sh1, output.ScriptHash) || Equals(sh2, output.ScriptHash) || Equals(sh3, output.ScriptHash) || Equals(sh4, output.ScriptHash) || Equals(sh5, output.ScriptHash);
                if (!ok) return false;
            }
            return VerifySignature(signature, pubkey);
        }
        public static bool Equals(byte[] A, byte[] B)
        {
            if (A.Length != B.Length)
                return false;
            for (int i = 0; i < A.Length; i++)
            {
                if (A[i] != B[i]) return false;
            }
            return true;
        }
    }
}
