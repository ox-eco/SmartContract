using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework;
using System;
namespace OX.SmartContract
{
    /// <summary>
    /// Contract Script Hash:0x83f4036ded80cce463ae81e02328fc3e028ac892
    /// </summary>
    public class AssetMerge : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(int Channel, int SideType, byte[] Data, int Flag, byte[] pubkey, byte[] signature)
        {
            var ok = VerifySignature(signature, pubkey) || Runtime.CheckWitness(pubkey);
            if (ok) return Blockchain.VerifySlotPubKey(pubkey);
            return false;
        }
    }
}
