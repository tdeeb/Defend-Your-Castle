﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defend_Your_Castle
{
    public sealed class UpgradeArcher : ShopItem
    {
        public UpgradeArcher(Player shopPlayer, Shop shop) : base(shopPlayer, shop)
        {
            // Set the properties of the item
            Name = "Upgrade Archer";

            MaxLevel = 3;

            price = 0;//15000;

            Description = "Increase the power and range of your Archers.";

            // Get the path to the image of the item
            ImagePath = "Content/Graphics/ShopIcons/Big UpgradeArcherIcon.png";
        }

        public override void UseItem()
        {
            base.UseItem();

            List<Archer> playerarchers = ShopPlayer.GetChildren.OfType<Archer>().ToList<Archer>();

            for (int i = 0; i < playerarchers.Count; i++)
            {
                playerarchers[i].IncreaseLevel();
            }
        }
    }
}