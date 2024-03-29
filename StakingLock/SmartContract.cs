﻿using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework;
using System;
using System.Numerics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace OX.SmartContract
{
    /// <summary>
    /// Contract Script Hash:0x2d44c71dc12d9912845a27273121b8683bcefea1
    /// </summary>
    public class StakingLock : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(byte[] unlocker)
        {
            return Runtime.CheckWitness(unlocker);
        }
        //public static bool Main(byte[] LockHash,uint LockExpiration,uint LockStakingType, byte[] unlocker, byte[] signature)
        //{

        //    //byte LockStakingType = 0x00;
        //    //BigInteger LockExpiration = 0x00;
        //    //byte[] LockHash = default;
        //    //int index = 0;
        //    //var l1 = sizeof(byte);
        //    //var b1 = data.Range(0, l1);
        //    //LockStakingType = b1[0];
        //    //index += l1;
        //    //var l2 = sizeof(uint);
        //    //var b2 = data.Range(index, l2);
        //    //LockExpiration = new BigInteger(b2);
        //    //index += l2;
        //    //var l3 = sizeof(byte);
        //    //var b3 = data.Range(index, l3);
        //    //index += l3;
        //    //if (b3[0] == 1)
        //    //{
        //    //    var l4 = 32;
        //    //    LockHash = data.Range(index, l4);
        //    //}

        //    if (LockStakingType == 1)
        //    {
        //        Header header = Blockchain.GetHeader(Blockchain.GetHeight());
        //        if (LockExpiration > header.Timestamp)return false;
        //    }
        //    else if (LockStakingType == 2)
        //    {
        //        if (LockExpiration > Blockchain.GetHeight()) return false;
        //    }
        //    else if (LockStakingType == 3)
        //    {
        //        //if (LockHash == default) return false;
        //        Transaction tx = OX.SmartContract.Framework.Services.System.ExecutionEngine.ScriptContainer as Transaction;
        //        var attrs = tx.GetAttributes();
        //        bool ok = false;
        //        foreach (var attr in attrs)
        //        {
        //            if (attr.Usage == 0xf1)//remark1
        //            {
        //                var hash = Hash256(attr.Data);
        //                if (Equals(hash, LockHash)) ok = true;
        //            }
        //        }
        //        if (!ok) return false;
        //    }
        //    else
        //        return false;

        //    return Runtime.CheckWitness(unlocker);

        //}
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
