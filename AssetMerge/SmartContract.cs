using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework;
using System;
namespace OX.SmartContract
{
    /// <summary>
    /// Contract Script Hash:0xef30ec8e833e8c828f4f20c690e607790897dfec
    /// </summary>
    public class AssetMerge : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(int SideType, byte[] Data, int Flag, byte[] pubkey, byte[] signature)
        {
            if (!Blockchain.VerifySlotPubKey(pubkey)) return false;
            return VerifySignature(signature, pubkey) || Runtime.CheckWitness(pubkey);
        }
    }
}
