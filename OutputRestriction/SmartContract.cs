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
        public static bool Main(object[] scriptHashes, byte[] pubkey, byte[] signature)
        {
            //var contractSH = OX.SmartContract.Framework.Services.System.ExecutionEngine.ExecutingScriptHash;
            Transaction tx = OX.SmartContract.Framework.Services.System.ExecutionEngine.ScriptContainer as Transaction;
            foreach (var output in tx.GetOutputs())
            {
                bool ok = false;
                foreach (var sh in scriptHashes)
                {
                    byte[] bs = (byte[])sh;
                    if (Equals(sh, output.ScriptHash))
                    {
                        ok = true;
                        break;
                    }
                }
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
