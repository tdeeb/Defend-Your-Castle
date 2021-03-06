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
        //The min and max flying heights for the Flying Enemy
        public const int MinFlyingHeight = 40;
        public const int MaxFlyingHeight = 70;

        //The height the enemy flies
        private float FlyingHeight;

        public FlyingEnemy(Level level, float xdiff, float Y, float flyingheight, float speedadd, int costume)
        {
            ObjectSheet = LoadAssets.FlyingGoblinSheet[costume];
            InvincibleSheet = LoadAssets.FlyingGoblinInvincibleSheet;

            Gold = 150;

            MoveSpeed = new Vector2(1.5f + speedadd, 0);

            Animation = new Animation(true, new AnimFrame(new Rectangle(3, 3, 31, 29), 300), new AnimFrame(new Rectangle(39, 3, 31, 29), 300), new AnimFrame(new Rectangle(75, 3, 31, 29), 300));

            FlyingHeight = flyingheight;
            Position = new Vector2(xdiff - Animation.CurrentAnimFrame.FrameSize.X, Y - Animation.CurrentAnimFrame.FrameSize.Y);

            WeaponWeakness = (int)Player.WeaponTypes.Spear;

            SetProperties(level);
        }

        public override Vector2 GetTruePosition
        {
            get { return new Vector2(Position.X, Position.Y - FlyingHeight); }
        }

        protected override void ChooseNextAction(Level level)
        {
            if (CurAction.GetActionType == Action.ActionType.Moving)
            {
                Animation AttackAnim = new Animation(new AnimFrame(new Rectangle(3, 39, 31, 29), 200), new AnimFrame(new Rectangle(39, 39, 29, 29), 275), new AnimFrame(new Rectangle(75, 40, 32, 28), 75, new Vector2(0, -1)));
                AttackAnim.SetAnimSpeed(Animation.GetSpeed);

                //Attack now
                CurAction = new MeleeAttack(this, AttackAnim, 100, AttackAnim.MaxFrame);
            }
        }
    }
}
