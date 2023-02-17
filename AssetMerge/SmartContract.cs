using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework;
using System;
namespace OX.SmartContract
{
    public class AssetMerge : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(int SideType, byte[] Data, int Flag, byte[] pubkey, byte[] signature)
        {
            return VerifySignature(signature, pubkey);
        }
    }
}
