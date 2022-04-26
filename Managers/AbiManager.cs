using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DefiKindom_QuestRunner.Managers
{
    internal static class AbiManager
    {
        public enum AbiTypes
        {
            Hero,
            Profile,
            Quest,
            Jewel,
            Erc721,
            Erc20
        }

        public static string GetAbi(AbiTypes type)
        {
            switch (type)
            {
                case AbiTypes.Hero:
                    return GetResource("dfk-hero.json");
                case AbiTypes.Quest:
                    return GetResource("dfk-quests.json");
                case AbiTypes.Profile:
                    return GetResource("dfk-profile.json");
                case AbiTypes.Jewel:
                    return GetResource("dfk-jewel.json");
                case AbiTypes.Erc721:
                    return GetResource("ERC721.json");
                case AbiTypes.Erc20:
                    return GetResource("erc20-abi.json");
            }

            return null;
        }

        static string GetResource(string filename)
        {
            var resourceContents = "";

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(filename));

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                            resourceContents = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return resourceContents;
        }

    }
}
