using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework;
using System;
using System.Numerics;

namespace OX.SmartContract
{
    public class AssetICO : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(string method, object[] args)
        {
            //if (Runtime.Trigger == TriggerType.Verification)
            //{
            //    Transaction tx = OX.SmartContract.Framework.Services.System.ExecutionEngine.ScriptContainer as Transaction;
            //    var inputs = tx.GetInputs();
            //    var outputs = tx.GetOutputs();
            //    if (inputs.Length != 1 || outputs.Length != 1) return false;
            //    BigInteger bn = (int)(inputs[0].PrevIndex);
            //    var key = inputs[0].PrevHash.Concat(ConvertN(bn));
            //    var targetaddr = Storage.Get(Storage.CurrentContext, key);
            //    return outputs[0].ScriptHash.AsBigInteger() == targetaddr.AsBigInteger();
            //}
            //else if (Runtime.Trigger == TriggerType.Application)
            //{
            //    if (method == "prepay")
            //    {
            //        var index = ConvertN((BigInteger)args[0]);
            //        var who = args[1] as byte[];
            //        Transaction tx = OX.SmartContract.Framework.Services.System.ExecutionEngine.ScriptContainer as Transaction;
            //        var outputs = tx.GetInputs();
            //        var key = tx.Hash.Concat(index);
            //        Storage.Put(Storage.CurrentContext, key, who);
            //    }
            //}
            return false;
        }
        static byte[] ConvertN(BigInteger n)
        {
            if (n == 0)
                return new byte[2] { 0x00, 0x00 };
            if (n == 1)
                return new byte[2] { 0x00, 0x01 };
            if (n == 2)
                return new byte[2] { 0x00, 0x02 };
            if (n == 3)
                return new byte[2] { 0x00, 0x03 };
            if (n == 4)
                return new byte[2] { 0x00, 0x04 };
            throw new Exception("not support");
        }
    }
}
