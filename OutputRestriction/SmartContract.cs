using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework;
using System;
using System.Numerics;

namespace OX.SmartContract
{
    public class OutputRestriction : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(byte[] scriptHash, byte[] pubkey, byte[] signature)
        {
            
            return VerifySignature(signature, pubkey);
        }
    }
}
