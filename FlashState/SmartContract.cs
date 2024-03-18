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
    /// 0x3c8eed1e38d0e3d1a01dfde583851fd96c36603e
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
                case "resetadmin":
                    return ResetAdmin((byte[])args[0]);
                case "setintervalfunction":
                    return SetIntervalFunction((int)args[0], (byte[])args[1]);
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
        [DisplayName("resetadmin")]
        public static bool ResetAdmin(byte[] newOwner)
        {
            StorageMap adminSet = Storage.CurrentContext.CreateMap("adm");
            byte[] value = adminSet.Get(new byte[] { 0 });
            if (value != null) {
                if (!Runtime.CheckWitness(value)) return false;
            }
            else
            {
                if (!Runtime.CheckWitness(Admin)) return false;
            }
            adminSet.Put(new byte[] { 0 }, newOwner);
            return true;
        }
        [DisplayName("setintervalfunction")]
        public static bool SetIntervalFunction(int multiple, byte[] scripthash)
        {
            StorageMap adminSet = Storage.CurrentContext.CreateMap("adm");
            byte[] value = adminSet.Get(new byte[] { 0 });
            if (value != null)
            {
                if (!Runtime.CheckWitness(value)) return false;
            }
            else
            {
                if (!Runtime.CheckWitness(Admin)) return false;
            }
          
            if (multiple < 1 || multiple > 10)
                throw new InvalidOperationException("pool multiple invalid.");
            StorageMap domainReverseSet = Storage.CurrentContext.CreateMap("itv");
            domainReverseSet.Put(new byte[] { 0 }, scripthash);
            domainReverseSet.Put(new byte[] { 1 }, multiple);
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
            byte[] value = adminSet.Get(new byte[] { 0 });
            if (value != null)
            {
                if (!Runtime.CheckWitness(value)) return false;
            }
            else
            {
                if (!Runtime.CheckWitness(Admin)) return false;
            }
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
            byte[] value = adminSet.Get(new byte[] { 0 });
            if (value != null)
            {
                if (!Runtime.CheckWitness(value)) return false;
            }
            else
            {
                if (!Runtime.CheckWitness(Admin)) return false;
            }
            StorageMap blacklist = Storage.CurrentContext.CreateMap("bkl");
            var str = owner.AsString();
            byte[] v = blacklist.Get(str);
            if (v == null) return false;
            blacklist.Delete(str);
            onRemoveBlackList(owner);
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
