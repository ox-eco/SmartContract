using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework;
using System;
using System.Numerics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace OX.SmartContract
{
    public class Domain : OX.SmartContract.Framework.SmartContract
    {
        public static bool Main(string operation, params object[] args)
        {
            if (Runtime.Trigger == TriggerType.Verification)
            {
            }
            else if (Runtime.Trigger == TriggerType.Application)
            {
                switch (operation)
                {
                    case "query":
                        return  true;//Query((string)args[0]);
                    case "register":
                        return Register((string)args[0], (byte[])args[1]);
                    case "delete":
                        return Delete((string)args[0]);
                    default:
                        return false;
                }
            }
            return false;
        }

        private static byte[] Query(string domain)
        {
            return Storage.Get(Storage.CurrentContext, domain);
        }


        private static bool Register(string domain, byte[] owner)
        {
            // 检查合约的调用者是否是合约的所属者
            if (!Runtime.CheckWitness(owner)) return false;
            byte[] value = Storage.Get(Storage.CurrentContext, domain);
            if (value != null) return false;
            Storage.Put(Storage.CurrentContext, domain, owner);
            return true;
        }

        private static bool Delete(string domain)
        {
            return true;
            // 待完成的其他代码
        }
    }
}
