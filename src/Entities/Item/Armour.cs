namespace DEA.Entities.Item
{
    public partial class Armour : CrateItem
    {
        public int DamageReduction { get; set; }

        public override int CrateOdds { get; set; }
    }
}
