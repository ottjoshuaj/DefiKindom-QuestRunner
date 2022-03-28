using System;

using NBitcoin;
using Nethereum.HdWallet;

namespace DefiKindom_QuestRunner.Helpers
{
    internal static class WalletHelper
    {
        public static Wallet LoadTrueWallet(string words)
        {
            try
            {
                string[] seps = { " " };

                var wordCount = words.Split(seps, StringSplitOptions.RemoveEmptyEntries).Length;
                WordCount count;
                switch (wordCount)
                {
                    case 12:
                        count = WordCount.Twelve;
                        break;
                    case 15:
                        count = WordCount.Fifteen;
                        break;
                    case 18:
                        count = WordCount.Eighteen;
                        break;
                    case 21:
                        count = WordCount.TwentyOne;
                        break;
                    case 24:
                        count = WordCount.TwentyFour;
                        break;
                    default:
                        count = WordCount.Twelve;
                        break;
                }


                return new Wallet(new Mnemonic(Wordlist.English, count).ToString(), "");
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
