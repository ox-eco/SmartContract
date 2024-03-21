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
    /// 0xaae5f82a9797ecd43038949ec625be4008cc0eab
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
        [DisplayName("onAddWhiteList")]
        public static event Action<byte[]> onAddWhiteList;
        [DisplayName("onRemoveWhiteList")]
        public static event Action<byte[]> onRemoveWhiteList;

        private static readonly byte[] Admin = "AWoTCyRFD5hC6Z5SwkV2y5284iH64p21Ck".ToScriptHash(); //Owner Address
        public static object Main(string operation, params object[] args)
        {
            switch (operation)
            {
                case "resetadmin":
                    return ResetAdmin((byte[])args[0], (byte[])args[1]);
                case "setintervalfunction":
                    return SetIntervalFunction((int)args[0], (int)args[1], (byte[])args[2]);
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
                case "addwhitelist":
                    return AddWhiteList((byte[])args[0]);
                case "removewhitelist":
                    return RemoveWhiteList((byte[])args[0]);
                case "getwhitelist":
                    return GetWhiteList();
                default:
                    return false;
            }
        }
        [DisplayName("resetadmin")]
        public static bool ResetAdmin(byte[] kind, byte[] newOwner)
        {
            StorageMap adminSet = Storage.CurrentContext.CreateMap("adm");
            bool ok = false;
            byte[] value = adminSet.Get(new byte[] { 0 });
            if (value != null) ok = Runtime.CheckWitness(value);
            if (!ok) ok = Runtime.CheckWitness(Admin);
            if (!ok) return false;
            adminSet.Put(kind, newOwner);
            return true;
        }
        [DisplayName("setintervalfunction")]
        public static bool SetIntervalFunction(int listKind, int multiple, byte[] scripthash)
        {
            StorageMap adminSet = Storage.CurrentContext.CreateMap("adm");
            bool ok = false;
            byte[] value = adminSet.Get(new byte[] { 1 });
            if (value != null) ok = Runtime.CheckWitness(value);
            if (!ok) ok = Runtime.CheckWitness(Admin);
            if (!ok) return false;

            if (multiple < 1 || multiple > 10)
                throw new InvalidOperationException("pool multiple invalid.");
            if (listKind < 0 || listKind > 1)
                throw new InvalidOperationException("listKind invalid.");
            StorageMap domainReverseSet = Storage.CurrentContext.CreateMap("itv");
            domainReverseSet.Put(new byte[] { 0 }, scripthash);
            domainReverseSet.Put(new byte[] { 1 }, multiple);
            domainReverseSet.Put(new byte[] { 2 }, listKind);
            return true;
        }

        [DisplayName("register")]
        public static bool Register(byte[] domain, byte[] owner)
        {
            if (!Runtime.CheckWitness(owner)) return false;
            var length = domain.Length;
            if (length < 6 || length > 20) return false;
            StorageMap domainSet = Storage.CurrentContext.CreateMap("dms");
            byte[] value = domainSet.Get(domain);
            if (value != null) return false;
            var bs = owner.Concat(domain);
            domainSet.Put(domain, bs);
            StorageMap domainReverseSet = Storage.CurrentContext.CreateMap("dmsr");
            domainReverseSet.Put(owner, bs);
            onRegister(domain, owner);
            return true;
        }
        [DisplayName("mark")]
        public static bool Mark(byte[] markdata, byte[] owner)
        {
            if (!Runtime.CheckWitness(owner)) return false;
            var length = markdata.Length;
            if (length == 0 || length > 512) return false;
            StorageMap markset = Storage.CurrentContext.CreateMap("mrk");
            var bs = owner.Concat(markdata);
            markset.Put(owner, bs);
            onMark(markdata, owner);
            return true;
        }
        [DisplayName("addBlackList")]
        public static bool AddBlackList(byte[] owner)
        {
            StorageMap adminSet = Storage.CurrentContext.CreateMap("adm");
            bool ok = false;
            byte[] value = adminSet.Get(new byte[] { 2 });
            if (value != null) ok = Runtime.CheckWitness(value);
            if (!ok) ok = Runtime.CheckWitness(Admin);
            if (!ok) return false;

            StorageMap blacklist = Storage.CurrentContext.CreateMap("bkl");
            var str = owner.AsString();
            byte[] v = blacklist.Get(str);
            if (v != null) return false;
            blacklist.Put(str, owner);
            onAddBlackList(owner);
            return true;
        }
        [DisplayName("removeBlackList")]
        public static bool RemoveBlackList(byte[] owner)
        {
            StorageMap adminSet = Storage.CurrentContext.CreateMap("adm");
            bool ok = false;
            byte[] value = adminSet.Get(new byte[] { 2 });
            if (value != null) ok = Runtime.CheckWitness(value);
            if (!ok) ok = Runtime.CheckWitness(Admin);
            if (!ok) return false;

            StorageMap blacklist = Storage.CurrentContext.CreateMap("bkl");
            var str = owner.AsString();
            byte[] v = blacklist.Get(str);
            if (v == null) return false;
            blacklist.Delete(str);
            onRemoveBlackList(owner);
            return true;
        }
        [DisplayName("addWhiteList")]
        public static bool AddWhiteList(byte[] owner)
        {
            StorageMap adminSet = Storage.CurrentContext.CreateMap("adm");
            bool ok = false;
            byte[] value = adminSet.Get(new byte[] { 3 });
            if (value != null) ok = Runtime.CheckWitness(value);
            if (!ok) ok = Runtime.CheckWitness(Admin);
            if (!ok) return false;

            StorageMap blacklist = Storage.CurrentContext.CreateMap("wtl");
            var str = owner.AsString();
            byte[] v = blacklist.Get(str);
            if (v != null) return false;
            blacklist.Put(str, owner);
            onAddWhiteList(owner);
            return true;
        }
        [DisplayName("removeWhiteList")]
        public static bool RemoveWhiteList(byte[] owner)
        {
            StorageMap adminSet = Storage.CurrentContext.CreateMap("adm");
            bool ok = false;
            byte[] value = adminSet.Get(new byte[] { 3 });
            if (value != null) ok = Runtime.CheckWitness(value);
            if (!ok) ok = Runtime.CheckWitness(Admin);
            if (!ok) return false;

            StorageMap blacklist = Storage.CurrentContext.CreateMap("wtl");
            var str = owner.AsString();
            byte[] v = blacklist.Get(str);
            if (v == null) return false;
            blacklist.Delete(str);
            onRemoveWhiteList(owner);
            return true;
        }
        [DisplayName("getBlackList")]
        public static byte[] GetBlackList()
        {
            byte[] bs = new byte[0];
            var result = Storage.Find("bkl");
            while (result.Next())
            {
                bs = bs.Concat(result.Value);
            }
            return bs;
        }
        [DisplayName("getWhiteList")]
        public static byte[] GetWhiteList()
        {
            byte[] bs = new byte[0];
            var result = Storage.Find("wtl");
            while (result.Next())
            {
                bs = bs.Concat(result.Value);
            }
            return bs;
        }
        [DisplayName("domainQuery")]

        public static byte[] DomainQuery(byte[] domain)
        {
            return Storage.CurrentContext.CreateMap("dms").Get(domain);
        }
        [DisplayName("ownerQuery")]
        public static byte[] OwnerQuery(byte[] owner)
        {
            return Storage.CurrentContext.CreateMap("dmsr").Get(owner);
        }

    }
}
