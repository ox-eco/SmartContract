﻿using OX.SmartContract.Framework.Services;
using OX.SmartContract.Framework;
using System;
using System.Numerics;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace OX.SmartContract
{

    using OX.SmartContract.Framework;
    using OX.SmartContract.Framework.Services.System;
    using System;
    using System.ComponentModel;
    using System.Numerics;

    namespace NEP5
    {
        public class NEP5 : SmartContract
        {
            [DisplayName("transfer")]
            public static event Action<byte[], byte[], BigInteger> Transferred;

            private static readonly byte[] Owner = "Ad1HKAATNmFT5buNgSxspbW68f4XVSssSw".ToScriptHash(); //Owner Address
            private static readonly BigInteger TotalSupplyValue = 10000000000000000;

            public static object Main(string method, object[] args)
            {
                if (Runtime.Trigger == TriggerType.Verification)
                {
                    return Runtime.CheckWitness(Owner);
                }
                else if (Runtime.Trigger == TriggerType.Application)
                {
                    var callscript = ExecutionEngine.CallingScriptHash;

                    if (method == "balanceOf") return BalanceOf((byte[])args[0]);

                    if (method == "decimals") return Decimals();

                    if (method == "deploy") return Deploy();

                    if (method == "name") return Name();

                    if (method == "symbol") return Symbol();

                    if (method == "supportedStandards") return SupportedStandards();

                    if (method == "totalSupply") return TotalSupply();

                    if (method == "transfer") return Transfer((byte[])args[0], (byte[])args[1], (BigInteger)args[2], callscript);
                }
                return false;
            }

            [DisplayName("balanceOf")]
            public static BigInteger BalanceOf(byte[] account)
            {
                if (account.Length != 20)
                    throw new InvalidOperationException("The parameter account SHOULD be 20-byte addresses.");
                StorageMap asset = Storage.CurrentContext.CreateMap(nameof(asset));
                return asset.Get(account).AsBigInteger();
            }
            [DisplayName("decimals")]
            public static byte Decimals() => 8;

            private static bool IsPayable(byte[] to)
            {
                var c = Blockchain.GetContract(to);
                return c == null || c.IsPayable;
            }

            [DisplayName("deploy")]
            public static bool Deploy()
            {
                if (TotalSupply() != 0) return false;
                StorageMap contract = Storage.CurrentContext.CreateMap(nameof(contract));
                contract.Put("totalSupply", TotalSupplyValue);
                StorageMap asset = Storage.CurrentContext.CreateMap(nameof(asset));
                asset.Put(Owner, TotalSupplyValue);
                Transferred(null, Owner, TotalSupplyValue);
                return true;
            }

            [DisplayName("name")]
            public static string Name() => "GinoMo"; //name of the token

            [DisplayName("symbol")]
            public static string Symbol() => "GM"; //symbol of the token

            [DisplayName("supportedStandards")]
            public static string[] SupportedStandards() => new string[] { "NEP-5", "NEP-7", "NEP-10" };

            [DisplayName("totalSupply")]
            public static BigInteger TotalSupply()
            {
                StorageMap contract = Storage.CurrentContext.CreateMap(nameof(contract));
                return contract.Get("totalSupply").AsBigInteger();
            }
#if DEBUG
            [DisplayName("transfer")] //Only for ABI file
            public static bool Transfer(byte[] from, byte[] to, BigInteger amount) => true;
#endif
            //Methods of actual execution
            private static bool Transfer(byte[] from, byte[] to, BigInteger amount, byte[] callscript)
            {
                //Check parameters
                if (from.Length != 20 || to.Length != 20)
                    throw new InvalidOperationException("The parameters from and to SHOULD be 20-byte addresses.");
                if (amount <= 0)
                    throw new InvalidOperationException("The parameter amount MUST be greater than 0.");
                if (!IsPayable(to))
                    return false;
                if (!Runtime.CheckWitness(from) && from.AsBigInteger() != callscript.AsBigInteger())
                    return false;
                StorageMap asset = Storage.CurrentContext.CreateMap(nameof(asset));
                var fromAmount = asset.Get(from).AsBigInteger();
                if (fromAmount < amount)
                    return false;
                if (from == to)
                    return true;

                //Reduce payer balances
                if (fromAmount == amount)
                    asset.Delete(from);
                else
                    asset.Put(from, fromAmount - amount);

                //Increase the payee balance
                var toAmount = asset.Get(to).AsBigInteger();
                asset.Put(to, toAmount + amount);

                Transferred(from, to, amount);
                return true;
            }
        }
    }
}
