namespace DEA.Entities.Item
{
    public partial class Meat : Food
    {
        public override int Health { get; set; }

        public override int AcquireOdds { get; set; }
    }
}
