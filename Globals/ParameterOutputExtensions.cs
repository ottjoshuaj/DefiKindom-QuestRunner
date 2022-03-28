using System;
using System.Collections.Generic;
using System.Linq;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Web3;

namespace DefiKindom_QuestRunner
{
    internal static class ParameterOutputExtensions
    {
        public static string ConvertToString(this ParameterOutput param)
        {
            try
            {
                if (param.Result != null)
                    return Convert.ToString(param.Result);
            }
            catch
            {
            }

            return null;
        }

        public static int ConvertToInt(this ParameterOutput param)
        {
            try
            {
                if (param.Result != null)
                {
                    var strResult = param.ConvertToString();
                    if (!string.IsNullOrWhiteSpace(strResult))
                        return int.Parse(strResult);
                }
                    
            }
            catch
            {
            }

            return 0;
        }

        public static bool ConvertToBool(this ParameterOutput param)
        {
            try
            {
                if (param.Result != null)
                {
                    var strResult = param.ConvertToString();
                    if (!string.IsNullOrWhiteSpace(strResult))
                        return bool.Parse(strResult);
                }

            }
            catch
            {
            }

            return false;
        }

        public static DateTime? ConvertToDateTime(this ParameterOutput param)
        {
            try
            {
                if (param.Result != null)
                {
                    var strResult = param.ConvertToString();
                    if (!string.IsNullOrWhiteSpace(strResult) && strResult != "0")
                    {
                        var dtMs = Convert.ToInt32(strResult);
                        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        var properDateTime = epoch.AddSeconds(dtMs);

                        return properDateTime;
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        public static List<int>ConvertToIntList(this ParameterOutput param)
        {
            try
            {
                var items = new List<int>();

                if (param.Result != null)
                {
                    //Cast result as list System.Numerics.BigInteger
                    var bigIntList = param.Result as List<System.Numerics.BigInteger>;
                    if (bigIntList != null)
                    {
                        items.AddRange(bigIntList.Select(number => int.Parse(Convert.ToString(number))));
                    }
                }

                return items;
            }
            catch
            {
            }

            return null;
        }

        public static List<ParameterOutput> ConvertToParamOutputList(this ParameterOutput param)
        {
            try
            {
                if (param.Result != null)
                    return param.Result as List<ParameterOutput>;
            }
            catch
            {
            }

            return null;
        }
    }
}
