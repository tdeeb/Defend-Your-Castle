﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defend_Your_Castle
{
    //The Level where gameplay takes place; it handles all the LevelObjects
    public sealed class Level
    {
        // Stores the reference to Game1
        public Game1 Game;

        //The constant that helps determine the rate that the fade changes and the fade that changes day to night
        //The NightFactor determines if a day starts out in day or vice versa
        private const float FadeRate = 185f;
        public const float CelestialDepth = .01f;
        private Fade NightFade;
        private int NightFactor;

        //Starting X and Y position of the sun, respectively
        private const float SunX = 45f;
        private const float SunY = 25f;

        // Multiplier for the amount of bonus gold the player will receive
        private const float BonusGoldMultiplier = 15f;

        //The game's max level
        public const int MaxLevel = 40;

        // Stores the level number of the level
        private int LevelNum;

        //The player; the player's children are the things that are helpful to the player
        //The player and its children persist between levels
        private Player player;

        // The time at which the level will end
        private float LevelEndTime;

        //The list of enemies and other things harmful to the player
        private List<LevelObject> Enemies;

        // Instance of the EnemySpawning class - used to spawn enemies
        private EnemySpawning EnemySpawn;

        //The cloud generator for the level
        private CloudGenerator cloudGenerator;

        // Stores the amount of Gold the player has when the level starts
        // Used to calculate the amount of Gold the player earned in the level
        private int StartingGold;

        // Stores the number of times the player has attacked by tapping/clicking. Used to calculate the player's accuracy rate
        private int NumAttacks;

        // Stores the number of enemies the player has killed by tapping/clicking. Also used to calculate the player's accuracy rate
        private int NumPlayerKills;

        // Stores the number of enemies the player's helpers have killed
        public int NumHelperKills;

        //Gets the night fade
        public Fade GetNightFade
        {
            get { return NightFade; }
        }

        //The starting Y position of the moon
        private float MoonY
        {
            get { return (SunY + FadeRate); }
        }

        // Returns the number of total kills the player earned in the round
        private int NumTotalKills
        {
            get { return (NumPlayerKills + NumHelperKills); }
        }

        // Returns the player's accuracy rate, rounded to the nearest whole number
        private double PlayerAccuracyRate
        {
            get
            {
                // Return 0 if the player hasn't attacked
                if (NumAttacks == 0) return 0;

                // Divide the number of player kills by the number of attacks to get the accuracy rate
                return Math.Round((((double)NumPlayerKills / (double)NumAttacks) * 100d));
            }
        }

        // Returns the amount of gold the player has earned in the level
        private int GoldEarned
        {
            get { return (player.Gold - StartingGold); }
        }

        // Calculates the amount of bonus gold the player has earned at the end of the level
        // Formula: [(# Player Kills * (Accuracy Rate / 100)) * BonusGoldMultiplier]
        private int BonusGold
        {
            get { return ((int)((NumPlayerKills * (PlayerAccuracyRate / 100d)) * BonusGoldMultiplier)); }
        }

        // Returns the total amount of gold the player has earned in the level
        private int TotalGoldEarned
        {
            get { return (GoldEarned + BonusGold); }
        }

        // Returns the Level's EnemySpawn object
        public EnemySpawning GetEnemySpawn
        {
            get { return EnemySpawn; }
        }

        public bool DidEnemiesSpawn
        {
            get { return EnemySpawn.CanStartSpawning; }
        }

        // Returns the Level's CloudGenerator object
        public CloudGenerator GetCloudGenerator
        {
            get { return cloudGenerator; }
        }

        public Level(Player play, Game1 game)
        {
            player = play;
            Game = game;

            Enemies = new List<LevelObject>();

            // Set the level number to 1
            LevelNum = 1;

            //Start out at day
            StartDayNight(true);
            CreateNightFade(true);

            // Set the starting gold to the player's gold amount
            StartingGold = player.Gold;

            // Initialize the EnemySpawning class
            EnemySpawn = new EnemySpawning(this);

            //Create cloud generator
            cloudGenerator = new CloudGenerator();

            //This is here just in case the level somehow gets set to something other than 1
            if (LevelNum >= EnemySpawning.StartNewEnem)
            {
                //+1 for reaching the value
                int newenemies = ((LevelNum - EnemySpawning.StartNewEnem) / EnemySpawning.NextNewEnem) + 1;
                EnemySpawn.AddNewSpawnEnemy(newenemies);
            }

            // Set the level end time
            LevelEndTime = Game1.ActiveTime + LevelDuration;
        }

        public List<LevelObject> GetEnemies
        {
            get { return Enemies; }
        }

        // The duration of the level (in milliseconds)
        private float LevelDuration
        {
            // NOTE: This calculation will need to be changed
            get { return (20000 + ((LevelNum - 1) * 900)); }
        }

        //Creates the night fade based on how long the level lasts
        private void CreateNightFade(bool day)
        {
            //Get how long the level lasts in frames each fade for the level
            int numframes = (int)(LevelDuration / 16.666f);
            float amount = (float)(FadeRate / numframes);

            //Start at night
            if (day == false) amount = -amount;

            //Starts sky blue and goes down to a value closer to purple (darken the sky)
            //We can reverse it and make the level start at night by simply reversing the value of NightFactor
            NightFade = new FadeOnce(Color.LightSkyBlue, amount, 0, FadeRate, 0f);
        }

        //Choose whether to start the level in the day or at night
        private void StartDayNight(bool day)
        {
            NightFactor = (day == true ? 1 : -1);
        }

        public void StartNextLevel()
        {
            // Increase the level number by 1
            LevelNum += 1;

            //Refresh the night fade
            CreateNightFade(true);

            // Reset the spawn delay timer
            EnemySpawn.ResetSpawnDelayTimer();
            
            // Check if a new enemy can be added to the spawn list, and add the enemy if so
            EnemySpawn.CheckAddSpawnEnemy();

            //Refresh the cloud generator
            cloudGenerator.Refresh();

            // Set the level end time
            LevelEndTime = Game1.ActiveTime + LevelDuration;

            // Reset the tracked information
            ResetTrackedInfo();

            // Update the player's HP and Gold on the UI
            player.UpdateHealth();
            player.UpdateGoldAmount();
            player.TouchHit = false;

            // Set the game state to InGame
            Game.ChangeGameState(GameState.InGame);

            //Play music
            SoundManager.PlaySong(LoadAssets.LevelMusic);
        }
        
        // Resets the information that is tracked during the level
        private void ResetTrackedInfo()
        {
            // Store the amount of gold the player has
            StartingGold = player.Gold;

            // Set the number of attacks and kills to 0
            NumAttacks = NumPlayerKills = NumHelperKills = 0;
        }

        public void CheckEndLevel()
        {
            // Check if the level should be ended
            if (Game1.ActiveTime >= LevelEndTime)
            {
                // End the level
                EndLevel();
            }
        }

        public void EndLevel()
        {
            // Clear the enemies list
            Enemies.Clear();
            player.ResetHelpers();

            // Stop the Player's Invincibility if it is active
            player.EndInvincibility();

            // Set the level complete text
            player.GetGamePage.LevelEnd_LevelCompleteText.Text = "Level " + LevelNum + " Complete";

            // Display the player's level stats
            player.GetGamePage.LevelEnd_Kills.Text = "Kills: " + NumPlayerKills;
            player.GetGamePage.LevelEnd_HelperKills.Text = "Helper Kills: " + NumHelperKills;
            player.GetGamePage.LevelEnd_TotalKills.Text = "Total Kills: " + NumTotalKills;
            player.GetGamePage.LevelEnd_AccuracyRate.Text = "Accuracy Rate: " + PlayerAccuracyRate + "%";
            player.GetGamePage.LevelEnd_BonusGold.Text = "Bonus Gold: " + BonusGold;
            player.GetGamePage.LevelEnd_GoldEarned.Text = "Gold Earned: " + GoldEarned;
            player.GetGamePage.LevelEnd_TotalGoldEarned.Text = "Total Gold Earned: " + TotalGoldEarned;

            // Begin the animation to display the level stats
            player.GetGamePage.LevelEnd_Anim.Begin();

            // Give the player the bonus gold
            player.ReceiveGold(BonusGold);

            // Save the game if the player did not just complete the last level
            if (LevelNum < MaxLevel)
                Game.GamePage.SaveGame();

            // Set the game state to LevelEnd
            Game.ChangeGameState(GameState.LevelEnd);

            //Play victory music
            SoundManager.PlaySound(LoadAssets.LevelComplete);
            SoundManager.StopSong();
        }

        public void QuitLevel()
        {
            // Clear the enemies list
            Enemies.Clear();

            // Stop the Level Start animation if it is running
            Game.GamePage.LevelStart_Anim.Stop();

            // Show the Title Screen
            Game.screenManager.ChangeScreen(ScreenManager.Screens.TitleScreen);

            // Set the game state to Screen
            Game.ChangeGameState(GameState.Screen);

            SoundManager.PlaySong(LoadAssets.TitleScreenMusic);
        }

        //Returns the player reference
        public Player GetPlayer
        {
            get { return player; }
        }

        //Adds a player-helping object to the player
        public void AddPlayerHelper(PlayerHelper helper)
        {
            player.AddChild(helper);
            helper.SetPosition();
        }

        //Adds a player-harming object to the level; these objects always have no parents and are added here if removed from their parents
        public void AddEnemy(LevelObject enemy)
        {
            Enemies.Add(enemy);
        }

        // Return the level number
        public int GetLevelNum
        {
            get { return LevelNum; }
        }

        private void UpdateEnemies()
        {
            //Update enemies
            for (int i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i].IsDead == false)
                    Enemies[i].Update(this);
                else
                {
                    Enemies.RemoveAt(i);
                    i--;
                }
            }
        }

        //Decrement the number of attacks if the player swiped and didn't hit an enemy
        public void DecrementNumAttacks()
        {
            NumAttacks -= 1;
        }

        //Make the player hit an enemy if it attacked
        public bool EnemyHit(Rectangle rect)
        {
            //Check if we actually hit an enemy
            bool hitenem = false;

            // Increment the player's number of attacks by 1
            NumAttacks += 1;

            //Find all the enemies we hit
            List<LevelObject> enemies = new List<LevelObject>();

            for (int i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i].CanGetHit(rect) == true)
                {
                    enemies.Add(Enemies[i]);
                    hitenem = true;
                }
            }

            //Find highest Y
            float highestY = 0;
            int index = -1;
            
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].GetPosition.Y > highestY)
                {
                    highestY = enemies[i].GetPosition.Y;
                    index = i;
                }
            }

            //Detect if we made an ineffective hit
            bool ineffectivehit = false;

            //Check if there is an enemy to hit
            if (index >= 0)
            {
                //Check if our weapon can hurt the enemy
                if (player.CurrentWeapon.CanHit(enemies[index].GetWeaponWeakness) == true)
                {
                    enemies[index].Die(this);
                    enemies[index].GrantGold(this, true);

                    // Increment the player's kill count by 1
                    NumPlayerKills += 1;
                }
                //Otherwise play a sound indicating it can't
                else
                {
                    SoundManager.PlaySound(LoadAssets.IneffectiveSwing);
                    ineffectivehit = true;
                }
            }

            //Play the normal weapon swing sound if we havent made an ineffective hit or there are fewer than 25 enemies
            if (ineffectivehit == false && Enemies.Count < 20) SoundManager.PlaySound(LoadAssets.WeaponSwing);

            return hitenem;
        }

        public void Update()
        {
            //Update the night fade
            NightFade.Update();

            // Update the enemies
            UpdateEnemies();

            //Update the player
            player.Update(this);

            // Update the enemy spawn
            EnemySpawn.Update();

            //Update the cloud generator
            cloudGenerator.Update(this);

            // Check if the level can be ended
            CheckEndLevel();
        }

        private void DrawEnemies(SpriteBatch spriteBatch)
        {
            //Draw enemies
            for (int i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i].IsDead == false)
                    Enemies[i].Draw(spriteBatch);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 BGscale = new Vector2(Game1.ScreenSize.X / LoadAssets.LevelBG.Width, Game1.ScreenSize.Y / LoadAssets.LevelBG.Height);
            Color BGcolor = new Color(255 - (int)NightFade.GetCurFade, 255 - (int)NightFade.GetCurFade, 255 - (int)NightFade.GetCurFade);

            //Draw the background
            spriteBatch.Draw(LoadAssets.LevelBG, Vector2.Zero, null, BGcolor, 0f, Vector2.Zero, BGscale, SpriteEffects.None, CelestialDepth + .0001f);
            spriteBatch.Draw(LoadAssets.ScalableBox, Vector2.Zero, null, NightFade.GetColorPlusFade(false), 0f, Vector2.Zero, new Vector2(Game1.ScreenSize.X, Game1.ScreenSize.Y), SpriteEffects.None, 0f);
            spriteBatch.Draw(LoadAssets.DaySun, new Vector2(Game1.ScreenHalf.X + SunX, SunY + NightFade.GetCurFade), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, CelestialDepth);
            spriteBatch.Draw(LoadAssets.NightMoon, new Vector2(Game1.ScreenHalf.X + (SunX + 2), MoonY - NightFade.GetCurFade), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, CelestialDepth);

            //Draw the clouds
            cloudGenerator.Draw(spriteBatch, this);

            DrawEnemies(spriteBatch);

            //Draw the player
            player.Draw(spriteBatch);
        }

        public void LoadLevelData(LevelData levelData)
        {
            LevelNum = levelData.LevelNum;

            if (LevelNum >= EnemySpawning.StartNewEnem)
            {
                //+1 for reaching the value
                int newenemies = ((LevelNum - EnemySpawning.StartNewEnem) / EnemySpawning.NextNewEnem) + 1;
                EnemySpawn.AddNewSpawnEnemy(newenemies);
            }

            // Loop until we reach the level the player should be at
            //for (int i = 1; i < levelData.LevelNum; i++)
            //{
            //    // Increment the level number by 1
            //    LevelNum += 1;
            //
            //    // Try to add an enemy spawn. This will not work if the level number is set directly because of the switch statement
            //    EnemySpawn.CheckAddSpawnEnemy();
            //}
        }


    }
}
