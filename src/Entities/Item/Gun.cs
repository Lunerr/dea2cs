﻿namespace DEA.Entities.Item
{
    public partial class Gun : Weapon
    {
        public override int Damage { get; set; }

        public override int Accuracy { get; set; }

        public override int CrateOdds { get; set; }
    }
}
