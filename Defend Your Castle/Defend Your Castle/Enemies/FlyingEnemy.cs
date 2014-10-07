﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Defend_Your_Castle
{
    //An enemy that attacks with Melee
    public sealed class FlyingEnemy : Enemy
    {
        public FlyingEnemy(Level level)
        {
            ObjectSheet = LoadAssets.FlyingGoblinSheet;
            InvincibleSheet = LoadAssets.FlyingGoblinInvincibleSheet;

            Animation = new Animation(true, new AnimFrame(new Rectangle(3, 3, 31, 29), 300), new AnimFrame(new Rectangle(39, 3, 31, 29), 300), new AnimFrame(new Rectangle(75, 3, 31, 29), 300));

            WeaponWeakness = (int)Player.WeaponTypes.Spear;

            SetProperties(level);
        }

        protected override void ChooseNextAction(Level level)
        {
            if (CurAction.GetActionType == Action.ActionType.Moving)
            {
                Animation AttackAnim = new Animation(new AnimFrame(new Rectangle(3, 39, 31, 29), 300), new AnimFrame(new Rectangle(39, 39, 29, 29), 300), new AnimFrame(new Rectangle(75, 40, 32, 28), 300, new Vector2(0, -1)));

                //Attack now
                CurAction = new MeleeAttack(this, AttackAnim, 50, 600, 300);
            }
        }
    }
}
