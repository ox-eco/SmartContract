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
    /// 0x22633905896e95036579f476f6fdfb05000264ab
    /// </summary>
    public class FlashState : Framework.SmartContract
    {
        [DisplayName("onRegister")]
        public static event Action<string, byte[]> onRegister;
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
                case "getflashstateinterval":
                    return GetFlashStateInterval((int)args[0], (int)args[1], (long)args[2]);
                case "domainquery":
                    return DomainQuery((string)args[0]);
                case "ownerquery":
                    return OwnerQuery((byte[])args[0]);
                case "register":
                    return Register((string)args[0], (byte[])args[1]);
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
        [DisplayName("getFlashStateInterval")]
        public static int GetFlashStateInterval(int balanceMultiple, int flashStateNumber, long totalOXSBalance)
        {
            if (balanceMultiple < 1) return 0;
            if (balanceMultiple >= 1000) return 2;
            if (balanceMultiple >= 100)
            {
                return flashStateNumber > 5000 ? 10 : 2;
            }
            if (balanceMultiple >= 10)
            {
                if (flashStateNumber < 1000) return 2;
                else if (flashStateNumber < 5000) return 10;
                else return 100;
            }
            if (flashStateNumber == 0 || flashStateNumber > 5000) return 0;
            var k = totalOXSBalance / 10000;
            if (k > 100_000_000L * 5000) return 0;
            if (k > 100_000_000L * 2000)
                return flashStateNumber < 1000 ? 10 : 100;
            else
                return flashStateNumber < 1000 ? 2 : 10;
        }
        [DisplayName("register")]
        public static bool Register(string domain, byte[] owner)
        {
            if (owner.Length != 20)
                throw new InvalidOperationException("The parameters from and to SHOULD be 20-byte addresses.");
            if (!Runtime.CheckWitness(owner)) return false;
            var length = domain.Length;
            if (length < 3 || length > 20) return false;
            StorageMap domainSet = Storage.CurrentContext.CreateMap("domainset");
            byte[] value = domainSet.Get(domain);
            if (value != null) return false;
            domainSet.Put(domain, owner);
            StorageMap domainReverseSet = Storage.CurrentContext.CreateMap("domainreverseset");
            domainReverseSet.Put(owner, domain);
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
            markset.Put(owner, markdata);
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

        public static byte[] DomainQuery(string domain)
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
