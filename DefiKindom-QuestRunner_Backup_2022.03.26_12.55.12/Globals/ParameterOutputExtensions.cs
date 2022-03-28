using System;

using Nethereum.ABI.FunctionEncoding;

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
                    if (strResult != null)
                    {
                        return int.Parse(strResult);
                    }
                }
                    
            }
            catch
            {
            }

            return 0;
        }

        public static DateTime ConvertToDateTime(this ParameterOutput param)
        {
            try
            {
                if (param.Result != null)
                {
                    var strResult = param.ConvertToString();
                    if (strResult != null)
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

            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
