﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defend_Your_Castle
{
    public sealed class UpgradeSlower : ShopItem
    {
        public UpgradeSlower(Player shopPlayer, Shop shop) : base(shopPlayer, shop)
        {
            // Set the properties of the item
            Name = "Upgrade Slower";

            MaxLevel = 3;

            price = 0;//15000;

            Description = "Increase the slow power and slow duration of your Slowers.";

            // Get the path to the image of the item
            ImagePath = "Content/Graphics/ShopIcons/SlowerUpgradeIcon.png";
        }

        public override void UseItem()
        {
            base.UseItem();

            List<Slower> playerslowers = ShopPlayer.GetChildren.OfType<Slower>().ToList<Slower>();

            for (int i = 0; i < playerslowers.Count; i++)
            {
                playerslowers[i].IncreaseLevel();
            }
        }
    }
}