using System;

namespace DefiKindom_QuestRunner
{
    internal class HeroProfile
    {
        public HeroProfile()
        {
            SummonInfo = new HeroSummonInfo();
            Info = new HeroInfo();
            State = new HeroState();
            Stats = new HeroStats();
            PrimaryStatGrowth = new HeroStateGrowth();
            SecondaryStatGrowth = new HeroStateGrowth();
            Professions = new HeroStateProfessions();
        }

        public int Id { get; set; }

        public HeroSummonInfo SummonInfo { get; set; }

        public HeroInfo Info { get; set; }

        public HeroState State { get; set; }

        public HeroStats Stats { get; set; }

        public HeroStateGrowth PrimaryStatGrowth { get; set; }

        public HeroStateGrowth SecondaryStatGrowth { get; set; }

        public HeroStateProfessions Professions { get; set; }
    }

    internal class HeroSummonInfo
    {
        public DateTime? SummonedTime { get; set; }
        public DateTime? NextSummonTime { get; set; }
        public int SummonerId { get; set; }
        public int AssistantId { get; set; }
        public int Summons { get; set; }
        public int MaxSummons { get; set; }
    }

    internal class HeroInfo
    {
        public int StatGenes { get; set; }
        public int VisualGenes { get; set; }
        public int Rarity { get; set; }
        public bool Shiny { get; set; }
        public int Generation { get; set; }
        public int FirstName { get; set; }
        public int LastName { get; set; }
        public int ShinyStyle { get; set; }
        public int Class { get; set; }
        public int SubClass { get; set; }
    }

    internal class HeroState
    {
        public DateTime? StaminaFullAt { get; set; }
        public DateTime? HPFullAt { get; set; }
        public DateTime? MPFullAt { get; set; }
        public int Level { get; set; }
        public int XP { get; set; }
        public string CurrentQuest { get; set; }
        public int SP { get; set; }
        public int Status { get; set; }
    }

    internal class HeroStats
    {
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Luck { get; set; }
        public int Agility { get; set; }
        public int Vitality { get; set; }
        public int Endurance { get; set; }
        public int Dexterity { get; set; }
        public int HP { get; set; }
        public int MP { get; set; }
        public int Stamina { get; set; }
    }

    internal class HeroStateGrowth
    {
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Luck { get; set; }
        public int Agility { get; set; }
        public int Vitality { get; set; }
        public int Endurance { get; set; }
        public int Dexterity { get; set; }

        public int HPSm { get; set; }
        public int HPRg { get; set; }
        public int HPLg { get; set; }
        public int MPSm { get; set; }
        public int MPRg { get; set; }
        public int MPLg { get; set; }
    }

    internal class HeroStateProfessions
    {
        public int Mining { get; set; }

        public int Gardening { get; set; }

        public int Foraging { get; set; }

        public int Fishing { get; set; }
    }
}
