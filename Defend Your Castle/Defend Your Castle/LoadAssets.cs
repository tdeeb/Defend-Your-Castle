﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Defend_Your_Castle
{
    //A class for loading all of our assets
    public static class LoadAssets
    {
        private const string GraphicsDir = "Graphics/";
        private const string SoundDir = "Sounds/";
        private const string MusicDir = "Music/";

        public static Texture2D Sword;
        public static Texture2D Warhammer;
        public static Texture2D PlayerCastle;

        //Shop graphics
        public static Texture2D ShopIcon;
        public static Texture2D FortifyWalls;
        public static Texture2D Invincibility;
        public static Texture2D RepairWalls;
        public static Texture2D RepairWallsx10;

        //Debug graphics
        public static Texture2D ScalableBox;
        public static Texture2D testanim;

        public static Song TestSong;
        public static SoundEffect TestSound;

        public static void LoadContent(ContentManager Content)
        {
            LoadGraphics(Content);
            LoadSounds(Content);
            LoadMusic(Content);
        }

        private static Texture2D LoadGraphic(ContentManager Content, string filename)
        {
            return Content.Load<Texture2D>(GraphicsDir + filename);
        }

        private static void LoadGraphics(ContentManager Content)
        {
            Sword = Content.Load<Texture2D>(GraphicsDir + "Alpha Sword");
            Warhammer = Content.Load<Texture2D>(GraphicsDir + "Alpha Warhammer");
            PlayerCastle = Content.Load<Texture2D>(GraphicsDir + "Castle");

            //Shop graphics
            ShopIcon = Content.Load<Texture2D>(GraphicsDir + "ShopIcons/ShopIcon");
            FortifyWalls = Content.Load<Texture2D>(GraphicsDir + "ShopIcons/FortifyWallsIcon");
            Invincibility = Content.Load<Texture2D>(GraphicsDir + "ShopIcons/InvincibilityIcon2");
            RepairWalls = Content.Load<Texture2D>(GraphicsDir + "ShopIcons/RepairWallsIcon");
            RepairWallsx10 = Content.Load<Texture2D>(GraphicsDir + "ShopIcons/RepairWallsx10Icon");

            //Debug graphics
            ScalableBox = Content.Load<Texture2D>(GraphicsDir + "ScalableBox");
            testanim = Content.Load<Texture2D>(GraphicsDir + "TestAnim");
        }

        private static void LoadSounds(ContentManager Content)
        {
            TestSound = Content.Load<SoundEffect>(SoundDir + "test");
        }

        private static void LoadMusic(ContentManager Content)
        {
            TestSong = Content.Load<Song>(MusicDir + "Mario Party - Peaceful Mushroom Village");
        }


    }
}