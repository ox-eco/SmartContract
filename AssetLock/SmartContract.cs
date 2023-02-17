using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework;
using System;
using System.Numerics;

namespace OX.SmartContract
{
    /// <summary>
    /// Contract Script Hash:0x334b191cca29463a62ef69b790e015b2f7467383
    /// </summary>
    public class Lock : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(bool isTimeLock, uint lockExpiration, byte[] pubkey, byte[] signature)
        {
            if (isTimeLock)
            {
                Header header = Blockchain.GetHeader(Blockchain.GetHeight());
                if (lockExpiration > header.Timestamp) return false;
            }
            else
            {
                if (lockExpiration > Blockchain.GetHeight()) return false;
            }
            return VerifySignature(signature, pubkey);
        }
    }
}
