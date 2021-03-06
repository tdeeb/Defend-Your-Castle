﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Defend_Your_Castle
{
    //A ShopItem that makes the player's castle stronger and take less damage from attacks
    public sealed class StrengthenWalls : ShopItem
    {
        public const float DamageReduction = .03f;
        public const float MaxDamageReduction = .85f;

        public StrengthenWalls(Player shopPlayer, Shop shop) : base(shopPlayer, shop)
        {
            // Set the properties of the item
            Name = "Strengthen Walls";

            MaxLevel = 5;

            BasePrice = price = 4000;
            PriceIncrease = 1000;

            Description = "+3% reduced damage\n";
            //"Strengthen your fort's walls to\nbetter defend against enemy attacks!\n+3% reduced damage";

            // Set the displayed level
            SetDisplayedLevel();

            // Get the path to the image of the item
            ImagePath = "Content/Graphics/ShopIcons/FortifyCastleIcon.png";
        }

        public override void UseItem()
        {
            base.UseItem();

            //Fortify the player castle
            ShopPlayer.StrengthenCastle(1);
        }
    }
}