using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework.Services.System;
using OX.SmartContract.Framework;
using System;
using System.Numerics;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.ComponentModel;

namespace OX.SmartContract
{
    /// <summary>
    /// 0x29765c89a60805cda076ce201b2eae89e90e37ed
    /// </summary>
    public class FlashState : Framework.SmartContract
    {
        [DisplayName("onRegister")]
        public static event Action<byte[], byte[]> onRegister;
        [DisplayName("onMark")]
        public static event Action<byte[], byte[]> onMark;
        [DisplayName("onAddBlackList")]
        public static event Action<byte[]> onAddBlackList;
        [DisplayName("onRemoveBlackList")]
        public static event Action<byte[]> onRemoveBlackList;

        private static readonly byte[] Admin = "AWoTCyRFD5hC6Z5SwkV2y5284iH64p21Ck".ToScriptHash(); //Owner Address
        public static object Main(string operation, params object[] args)
        {
            switch (operation)
            {
                case "setintervalfunction":
                    return SetIntervalFunction((byte[])args[0], (byte[])args[1]);
                case "domainquery":
                    return DomainQuery((byte[])args[0]);
                case "ownerquery":
                    return OwnerQuery((byte[])args[0]);
                case "register":
                    return Register((byte[])args[0], (byte[])args[1]);
                case "mark":
                    return Mark((byte[])args[0], (byte[])args[1]);
                case "addblacklist":
                    return AddBlackList((byte[])args[0]);
                case "removeblacklist":
                    return RemoveBlackList((byte[])args[0]);
                case "getblacklist":
                    return GetBlackList();
                default:
                    return false;
            }
        }
        public static bool SetIntervalFunction(byte[] multiple, byte[] scripthash)
        {
            if (!Runtime.CheckWitness(Admin)) return false;
            if (multiple.Length != 1)
                throw new InvalidOperationException("pool multiple invalid.");
            var m = multiple[0];
            if (m < 1 || m > 10)
                throw new InvalidOperationException("pool multiple invalid.");
            StorageMap domainReverseSet = Storage.CurrentContext.CreateMap("intervalfunctionscripthash");
            domainReverseSet.Put(new byte[] { 0 }, scripthash);
            domainReverseSet.Put(new byte[] { 1 }, multiple);
            return true;
        }

        [DisplayName("register")]
        public static bool Register(byte[] domain, byte[] owner)
        {
            if (owner.Length != 20)
                throw new InvalidOperationException("The parameters from and to SHOULD be 20-byte addresses.");
            if (!Runtime.CheckWitness(owner)) return false;
            var length = domain.Length;
            if (length < 6 || length > 20) return false;
            StorageMap domainSet = Storage.CurrentContext.CreateMap("domainset");
            byte[] value = domainSet.Get(domain);
            if (value != null) return false;
            var bs = owner.Concat(domain);
            domainSet.Put(domain, bs);
            StorageMap domainReverseSet = Storage.CurrentContext.CreateMap("domainreverseset");
            domainReverseSet.Put(owner, bs);
            onRegister(domain, owner);
            return true;
        }
        [DisplayName("mark")]
        public static bool Mark(byte[] markdata, byte[] owner)
        {
            if (owner.Length != 20)
                throw new InvalidOperationException("The parameters from and to SHOULD be 20-byte addresses.");
            if (!Runtime.CheckWitness(owner)) return false;
            var length = markdata.Length;
            if (length == 0 || length > 512) return false;
            StorageMap markset = Storage.CurrentContext.CreateMap("markset");
            var bs = owner.Concat(markdata);
            markset.Put(owner, bs);
            onMark(markdata, owner);
            return true;
        }
        [DisplayName("addBlackList")]
        public static bool AddBlackList(byte[] owner)
        {
            if (!Runtime.CheckWitness(Admin)) return false;
            if (owner.Length != 20)
                throw new InvalidOperationException("The parameters from and to SHOULD be 20-byte addresses.");
            StorageMap blacklist = Storage.CurrentContext.CreateMap("blacklist");
            var str = owner.AsString();
            byte[] value = blacklist.Get(str);
            if (value != null) return false;
            blacklist.Put(str, owner);
            onAddBlackList(owner);
            return true;
        }
        [DisplayName("removeBlackList")]
        public static bool RemoveBlackList(byte[] owner)
        {
            if (!Runtime.CheckWitness(Admin)) return false;
            if (owner.Length != 20)
                throw new InvalidOperationException("The parameters from and to SHOULD be 20-byte addresses.");
            StorageMap blacklist = Storage.CurrentContext.CreateMap("blacklist");
            var str = owner.AsString();
            byte[] value = blacklist.Get(str);
            if (value == null) return false;
            blacklist.Delete(str);
            onRemoveBlackList(owner);
            return true;
        }
        [DisplayName("getBlackList")]
        public static byte[] GetBlackList()
        {
            byte[] bs = new byte[0];
            var result = Storage.Find("blacklist");
            while (result.Next())
            {
                bs = bs.Concat(result.Value);
            }
            return bs;
        }
        [DisplayName("domainQuery")]

        public static byte[] DomainQuery(byte[] domain)
        {
            return Storage.CurrentContext.CreateMap("domainset").Get(domain);
        }
        [DisplayName("ownerQuery")]
        public static byte[] OwnerQuery(byte[] owner)
        {
            return Storage.CurrentContext.CreateMap("domainreverseset").Get(owner);
        }

    }
}
