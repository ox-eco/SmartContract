using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework;
using System;
namespace OX.SmartContract
{
    /// <summary>
    /// Contract Script Hash:0xdd33a90efe30045caf107f714ec461f8616aa8af
    /// </summary>
    public class AssetMerge : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(int SideType, byte[] Data, int Flag, byte[] pubkey, byte[] signature)
        {
            return VerifySignature(signature, pubkey);
        }
    }
}
