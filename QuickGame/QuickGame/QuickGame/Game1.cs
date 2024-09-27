using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace QuickGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //Time of development: 2/2/2021 8:55 pm

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D pixel;
        MouseState mouse;
        MouseState oldMouse;
        Rectangle mouseRec;
        KeyboardState oldKB;
        bool isDebugTrue;

        ///////////////////////////////////////////main menu///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        enum GameState
        {
            main,
            mainS,
            play,
            gameOver,
            pause,
            high
        }

        enum buttonState
        {
            up,
            floating,
            down,
            released
        }

        GameState state = GameState.main;
        const int buttonCount = 3;
        Texture2D title;
        Rectangle titleRec, tsRec;
        Rectangle[] buttonsTex = new Rectangle[buttonCount]; //functions as source rec
        Rectangle[] buttonsRec = new Rectangle[buttonCount];
        int[] buttonTimer = new int[buttonCount];
        buttonState[] bStates = new buttonState[buttonCount];
        Color[] buttonsColor = new Color[buttonCount];
        ///////////////////////////////////////////main game///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        float gOpacity; //global opacity
        float indOp; //individual opacity for etc purposes
        float animOp; //opacity made for animations n shit

        int scoreCH;

        int rgbTimer;
        Color rgbC;
        SpriteFont vhsFont;
        SpriteFont debugFont;
        Texture2D spriteSheet1;
        int mmTimer;

        Rectangle ship, hitbox;
        Rectangle trail;
        Vector2 shipOrigin, trailOrigin; // rot vals

        int asCount; //num of asteroids
        int stCount; //back star count
        Rectangle[] asteroids;
        Rectangle[] backStars;
        int[] asVel, asRot, asSize, asPosX, asPosY; //asteroid rec vals
        int[] stVel, stPosX, stPosY; //back star rec vals
        Color[] stColor;

        int minVel, maxVel; //asteroid velocities that change based off of current score of player

        Random randy;

        int health;
        int invTimer; //invincibility
        int invMax; //max time for invincibility
        int flashTimer; //unused 

        int score; //rate at which meteors spawn
        int velocityX; //velocity used for moving side to side, uses acceleration
        double accX, accY; //acceleration values
        float rAngle; //angle used for velocity based rotations
        String screenRes = "";
        String testest = "";
        double globalSizeScale; //used for slight increases in sizes
        double multiplyVal; //used for speed increases


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //graphics.IsFullScreen = true;
            if (graphics.IsFullScreen) ///////////////////////fix full screen acceleration
            {
                graphics.PreferredBackBufferWidth = 1920;
                graphics.PreferredBackBufferHeight = 1080;
            }
            //Window.AllowUserResizing = true;
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            scoreCH = 1000;
            //Console.SetWindowPosition(1, 1);
            //Console.SetCursorPosition(1, 1);
            //////////////////////////////////////////////////////////////////////////////////////////main menu////////////////////////////////////////////////////////////////////////////////////////
            for (int i = 0; i < buttonCount; i++) //button 1 is quick game, 2 is settings, 3 is quit
            {
                bStates[i] = buttonState.up;
                switch (i)
                {
                    case 0:
                        buttonsColor[i] = Color.White;
                        buttonsTex[i] = new Rectangle(269, 123, 168, 24);
                        buttonsRec[i] = new Rectangle(GraphicsDevice.Viewport.Width / 2 - buttonsTex[i].Width / 2,/* GraphicsDevice.Viewport.Height / 3*/200, buttonsTex[i].Width, buttonsTex[i].Height);
                        break;
                    case 1:
                        buttonsColor[i] = Color.White;
                        buttonsTex[i] = new Rectangle(169, 404, 128, 18);
                        buttonsRec[i] = new Rectangle(GraphicsDevice.Viewport.Width / 2 - buttonsTex[i].Width / 2,/* GraphicsDevice.Viewport.Height / 2*/245, buttonsTex[i].Width, buttonsTex[i].Height);
                        break;
                    case 2:
                        buttonsColor[i] = Color.White;
                        buttonsTex[i] = new Rectangle(0, 217, 174, 24); //how did i forget to add sprites for an exit button wtf
                        buttonsRec[i] = new Rectangle(GraphicsDevice.Viewport.Width / 2 - buttonsTex[i].Width / 2,/* GraphicsDevice.Viewport.Height / 3*/280, buttonsTex[i].Width, buttonsTex[i].Height);
                        break;
                }
            }


            if (graphics.IsFullScreen)
            {
                globalSizeScale = 2.4;
            }
            else globalSizeScale = 1;
            ship = new Rectangle(GraphicsDevice.Viewport.Width / 2/* - (int)ship.Width/2*/, GraphicsDevice.Viewport.Height / 2/* - (int)ship.Height/2*/, (int)(25 * globalSizeScale), (int)(25 * globalSizeScale));
            trail = new Rectangle(GraphicsDevice.Viewport.Width / 2/* - (int)ship.Width/2*/, GraphicsDevice.Viewport.Height / 2/* - (int)ship.Height/2*/, (int)(25 * globalSizeScale), (int)(25 * globalSizeScale));
            hitbox = new Rectangle(ship.X, ship.Y, (int)(25 * globalSizeScale), (int)(25 * globalSizeScale));
            shipOrigin = new Vector2(1, 1);
            trailOrigin = new Vector2(2, 2);

            titleRec = new Rectangle(GraphicsDevice.Viewport.Width / 2 - /*titleRec.Width / 2*/192, GraphicsDevice.Viewport.Height / 10, 384, 104);
            tsRec = new Rectangle(0, 0, 384, 104);
            animOp = 0;

            minVel = 3;
            maxVel = 8;

            rgbTimer = 0;

            asCount = 8;
            stCount = 100; ///////////////////add star count slider
            asteroids = new Rectangle[asCount]; ///////////////////////////////////////////////////////////////////add difficulty option to allow for more asteroids to spawn but score faster
            asVel = new int[asCount];
            asRot = new int[asCount];
            asSize = new int[asCount];
            asPosX = new int[asCount];
            asPosY = new int[asCount];

            backStars = new Rectangle[stCount];
            stVel = new int[stCount];
            stPosX = new int[stCount];
            stPosY = new int[stCount];
            stColor = new Color[stCount];

            randy = new Random();

            multiplyVal = 1.6;

            int dfgsdfgsdfg = randy.Next(1, 6);
            switch (dfgsdfgsdfg)
            {
                case 1:
                    Window.Title = "The Beyond";
                    break;
                case 2:
                    Window.Title = "The Beyond.";
                    break;
                case 3:
                    Window.Title = "The Beyond?";
                    break;
                case 4:
                    Window.Title = "The Beyond!!!";
                    break;
                case 5:
                    Window.Title = "Bananas!";
                    break;
            }


            health = 3;
            invTimer = 0;
            invMax = 120;

            indOp = 1f; /////////////////////////////////////////////////////////////////////////////////////////////add debug menu

            //asteroids spawn & generation
            for (int i = 0; i < asCount; i++)
            {
                asSize[i] = randy.Next(10, 50);
                asPosX[i] = randy.Next(GraphicsDevice.Viewport.Width - asSize[i]);
                asPosY[i] = randy.Next(-200, -25);
                asteroids[i] = new Rectangle(asPosX[i], asPosY[i], (int)(asSize[i] * globalSizeScale), (int)(asSize[i] * globalSizeScale));
                asVel[i] = randy.Next(minVel, maxVel);
            }

            //background stars spawn & generation
            for (int i = 0; i < stCount; i++)
            {
                stVel[i] = randy.Next(minVel, maxVel);
                stPosX[i] = randy.Next(GraphicsDevice.Viewport.Width);
                stPosY[i] = randy.Next(-100, 500);
                backStars[i] = new Rectangle(stPosX[i], stPosY[i], (int)(1 * globalSizeScale), (int)(1 * globalSizeScale));
                stColor[i] = new Color(randy.Next(1, 255), randy.Next(1, 255), randy.Next(1, 255));
            }
            //testest =/* (String)*/;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pixel = this.Content.Load<Texture2D>("pixel");
            spriteSheet1 = this.Content.Load<Texture2D>("Quick Game Sprite Sheet v1");
            vhsFont = this.Content.Load<SpriteFont>("spritefont1");
            debugFont = this.Content.Load<SpriteFont>("spritefont2");
            title = this.Content.Load<Texture2D>("TB Title Sprites Dark");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            mouseRec = new Rectangle(mouse.X, mouse.Y, 1, 1);
            // Allows the game to exit
            screenRes = Window.ClientBounds + "";

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            KeyboardState kb = Keyboard.GetState();
            mouse = Mouse.GetState();
            // TODO: Add your update logic here
            if (graphics.IsFullScreen)
            {
                graphics.PreferredBackBufferWidth = 1920;
                graphics.PreferredBackBufferHeight = 1080;
            }
            else
            {
                graphics.PreferredBackBufferWidth = 800;
                graphics.PreferredBackBufferHeight = 480;
            }
            if (!isDebugTrue)
            {
                globalSizeScale = 1;
                if (graphics.IsFullScreen)
                    globalSizeScale = 2.4;
            }



            if (state == GameState.main)
            {
                for (int i = 0; i < buttonCount; i++)
                {
                    if (bStates[0] == buttonState.released) //player clicked on quick game
                    {
                        state = GameState.play;
                        //health = 3;
                        //score = 0;
                    }
                        

                    if (mouseRec.Intersects(buttonsRec[i]))
                    {
                        bStates[i] = buttonState.floating;
                        if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)//determining state of mouse clicks
                        {
                            bStates[i] = buttonState.down;
                            buttonsColor[i] = Color.Gray;
                        }
                        else if (mouse.LeftButton == ButtonState.Released && oldMouse.LeftButton == ButtonState.Pressed)
                        {
                            bStates[i] = buttonState.released;
                            buttonsColor[i] = Color.White;
                        }

                    }
                    else
                    {
                        bStates[i] = buttonState.up;
                        buttonsColor[i] = Color.White;
                    }
                    switch (bStates[i])
                    {
                        default:
                            buttonTimer[i] = 0;
                            break;
                        case buttonState.floating:
                            buttonTimer[i]++; //button animation timer
                            break;

                    }
                    switch (buttonTimer[0]) ////////animations for quick game button
                    {
                        case 0:
                            buttonsTex[0].X = 269;
                            buttonsTex[0].Y = 123;
                            break;
                        case 3:
                            buttonsTex[0].X = 205;
                            buttonsTex[0].Y = 186;
                            break;
                        case 6:
                            buttonsTex[0].X = 0;
                            buttonsTex[0].Y = 242;
                            break;
                        case 9:
                            buttonsTex[0].X = 0;
                            buttonsTex[0].Y = 342;
                            break;
                        case 12:
                            buttonsTex[0].X = 0;
                            buttonsTex[0].Y = 367;
                            break;
                        case 15:
                            buttonsTex[0].X = 0;
                            buttonsTex[0].Y = 392;
                            break;
                        case 18:
                            buttonsTex[0].X = 169;
                            buttonsTex[0].Y = 242;
                            break;
                        case 21:
                            buttonsTex[0].X = 0;
                            buttonsTex[0].Y = 417;
                            break;
                        case 24:
                            buttonsTex[0].X = 169;
                            buttonsTex[0].Y = 267;
                            break;
                        case 27:
                            buttonsTex[0].X = 0;
                            buttonsTex[0].Y = 267;
                            break;
                        case 30:
                            buttonsTex[0].X = 0;
                            buttonsTex[0].Y = 292;
                            break;
                        case 33:
                            buttonsTex[0].X = 0;
                            buttonsTex[0].Y = 317;
                            buttonTimer[0] = 0;
                            break;
                    }

                }

                //background star movement
                for (int i = 0; i < stCount; i++)
                {
                    if (!(stPosY[i] > GraphicsDevice.Viewport.Height + 20))
                    {
                        stPosY[i] += (int)stVel[i];
                        backStars[i] = new Rectangle(stPosX[i], stPosY[i], 1, 1);
                    }

                    if (stPosY[i] > GraphicsDevice.Viewport.Height + 20)
                    {
                        stPosX[i] = randy.Next(GraphicsDevice.Viewport.Width);
                        stPosY[i] = randy.Next(-200, -25);
                        backStars[i] = new Rectangle(stPosX[i], stPosY[i], 1, 1);
                        stVel[i] = randy.Next(minVel, maxVel);
                    }
                }
                mmTimer++;
                if (animOp != 1f)
                    animOp += .005f;

                switch (mmTimer)
                {
                    case 0:
                        tsRec = new Rectangle(0, 0, 384, 104);
                        break;
                    case 1:
                        tsRec = new Rectangle(0, 735, 384, 104);
                        break;
                    case 2:
                        tsRec = new Rectangle(770, 105, 384, 104);
                        break;
                    case 3:
                        tsRec = new Rectangle(770, 210, 384, 104);
                        break;
                    case 4:
                        tsRec = new Rectangle(770, 315, 384, 104);
                        break;
                    case 5:
                        tsRec = new Rectangle(770, 420, 384, 104);
                        break;
                    case 6:
                        tsRec = new Rectangle(770, 525, 384, 104);
                        break;
                    case 7:
                        tsRec = new Rectangle(770, 630, 384, 104);
                        break;
                    case 8:
                        tsRec = new Rectangle(770, 735, 384, 104);
                        break;
                    case 9:
                        tsRec = new Rectangle(0, 105, 384, 104);
                        break;
                    case 10:
                        tsRec = new Rectangle(0, 210, 384, 104);
                        break;
                    case 11:
                        tsRec = new Rectangle(0, 315, 384, 104);
                        break;
                    case 12:
                        tsRec = new Rectangle(385, 0, 384, 104);
                        break;
                    case 13:
                        tsRec = new Rectangle(385, 105, 384, 104);
                        break;
                    case 14:
                        tsRec = new Rectangle(385, 210, 384, 104);
                        break;
                    case 15:
                        tsRec = new Rectangle(385, 315, 384, 104);
                        break;
                    case 16:
                        tsRec = new Rectangle(0, 420, 384, 104);
                        break;
                    case 17:
                        tsRec = new Rectangle(0, 525, 384, 104);
                        break;
                    case 18:
                        tsRec = new Rectangle(0, 630, 384, 104);
                        break;
                    case 19:
                        tsRec = new Rectangle(385, 420, 384, 104);
                        break;
                    case 20:
                        tsRec = new Rectangle(0, 840, 384, 104);
                        break;
                    case 21:
                        tsRec = new Rectangle(385, 525, 384, 104);
                        break;
                    case 22:
                        tsRec = new Rectangle(385, 630, 384, 104);
                        break;
                    case 23:
                        tsRec = new Rectangle(385, 735, 384, 104);
                        break;
                    case 24:
                        tsRec = new Rectangle(385, 840, 384, 104);
                        break;
                    case 25:
                        tsRec = new Rectangle(770, 0, 384, 104);
                        break;
                    case 26:
                        tsRec = new Rectangle(0, 0, 384, 104);
                        break;
                    case 27:
                        tsRec = new Rectangle(0, 735, 384, 104);
                        break;
                    case 28:
                        tsRec = new Rectangle(770, 105, 384, 104);
                        break;
                    case 29:
                        tsRec = new Rectangle(770, 210, 384, 104);
                        break;
                    case 30:
                        tsRec = new Rectangle(770, 315, 384, 104);
                        break;
                    case 31:
                        tsRec = new Rectangle(770, 420, 384, 104);
                        break;
                    case 32:
                        tsRec = new Rectangle(770, 525, 384, 104);
                        break;
                    case 33:
                        tsRec = new Rectangle(770, 630, 384, 104);
                        break;
                    case 34:
                        tsRec = new Rectangle(770, 735, 384, 104);
                        break;
                    case 35:
                        tsRec = new Rectangle(0, 105, 384, 104);
                        break;
                    case 36:
                        tsRec = new Rectangle(0, 210, 384, 104);
                        break;
                    case 37:
                        tsRec = new Rectangle(0, 315, 384, 104);
                        break;
                    case 38:
                        tsRec = new Rectangle(385, 0, 384, 104);
                        break;
                    case 39:
                        tsRec = new Rectangle(385, 105, 384, 104);
                        break;
                    case 40:
                        tsRec = new Rectangle(385, 210, 384, 104);
                        break;
                    case 41:
                        tsRec = new Rectangle(385, 315, 384, 104);
                        break;
                    case 42:
                        tsRec = new Rectangle(0, 420, 384, 104);
                        break;
                    case 43:
                        tsRec = new Rectangle(0, 525, 384, 104);
                        break;
                    case 44:
                        tsRec = new Rectangle(0, 630, 384, 104);
                        break;
                    case 45:
                        tsRec = new Rectangle(385, 420, 384, 104);
                        break;
                    case 46:
                        tsRec = new Rectangle(0, 840, 384, 104);
                        break;
                    case 47:
                        tsRec = new Rectangle(385, 525, 384, 104);
                        break;
                    case 48:
                        tsRec = new Rectangle(385, 630, 384, 104);
                        break;
                    case 49:
                        tsRec = new Rectangle(385, 735, 384, 104);
                        break;
                    case 50:
                        tsRec = new Rectangle(385, 840, 384, 104);
                        break;
                    case 51:
                        title = this.Content.Load<Texture2D>("TB Title Sprites");
                        tsRec = new Rectangle(0, 0, 384, 104);
                        break;

                }


            }






            if (state == GameState.play)
            {
                if (health <= 0)
                    state = GameState.main;
                if (kb.IsKeyDown(Keys.OemTilde) && oldKB.IsKeyUp(Keys.OemTilde) && !isDebugTrue)
                    isDebugTrue = true;
                else if (kb.IsKeyDown(Keys.OemTilde) && oldKB.IsKeyUp(Keys.OemTilde) && isDebugTrue)
                    isDebugTrue = false;
                if (kb.IsKeyDown(Keys.OemMinus) && oldKB.IsKeyUp(Keys.OemMinus) && isDebugTrue)
                    globalSizeScale -= .1;
                if (kb.IsKeyDown(Keys.OemPlus) && oldKB.IsKeyUp(Keys.OemPlus) && isDebugTrue)
                    globalSizeScale += .1;
                if (kb.IsKeyDown(Keys.F11) && oldKB.IsKeyUp(Keys.F11) && !graphics.IsFullScreen)
                    graphics.IsFullScreen = true;
                else if (kb.IsKeyDown(Keys.F11) && oldKB.IsKeyUp(Keys.F11) && graphics.IsFullScreen)
                    graphics.IsFullScreen = false;
                score++; //essentially the score counter of the game

                for (int i = 0; i < asCount; i++)
                {
                    if (hitbox.Intersects(asteroids[i]) && invTimer == 0) ///////////collision
                    {
                        health--;
                        invTimer = invMax;
                    }
                }



                if (invTimer != 0)
                {
                    invTimer--;
                    if (invTimer <= invMax && invTimer > (invMax * .95))
                    {
                        indOp = 0f;
                    }
                    if (invTimer <= (invMax * .95) && invTimer > (invMax * .9))
                    {
                        indOp = 1f;
                    }
                    if (invTimer <= (invMax * .9) && invTimer > (invMax * .85))
                    {
                        indOp = 0f;
                    }
                    if (invTimer <= (invMax * .85) && invTimer > (invMax * .8))
                    {
                        indOp = 1f;
                    }
                    if (invTimer <= (invMax * .8) && invTimer > (invMax * .75))
                    {
                        indOp = 0f;
                    }
                    if (invTimer <= (invMax * .75) && invTimer > (invMax * .7))
                    {
                        indOp = 1f;
                    }
                    if (invTimer <= (invMax * .7) && invTimer > (invMax * .65))
                    {
                        indOp = 0f;
                    }
                    if (invTimer <= (invMax * .65) && invTimer > (invMax * .6))
                    {
                        indOp = 1f;
                    }
                    if (invTimer <= (invMax * .6) && invTimer > (invMax * .55))
                    {
                        indOp = 0f;
                    }
                    if (invTimer <= (invMax * .55) && invTimer > (invMax * .5))
                    {
                        indOp = 1f;
                    }
                    if (invTimer <= (invMax * .5) && invTimer > (invMax * .45))
                    {
                        indOp = 0f;
                    }
                    if (invTimer <= (invMax * .45) && invTimer > (invMax * .4))
                    {
                        indOp = 1f;
                    }
                    if (invTimer <= (invMax * .4) && invTimer > (invMax * .35))
                    {
                        indOp = 0f;
                    }
                    if (invTimer <= (invMax * .35) && invTimer > (invMax * .3))
                    {
                        indOp = 1f;
                    }
                    if (invTimer <= (invMax * .3) && invTimer > (invMax * .25))
                    {
                        indOp = 0f;
                    }
                    if (invTimer <= (invMax * .25) && invTimer > (invMax * .2))
                    {
                        indOp = 1f;
                    }
                    if (invTimer <= (invMax * .2) && invTimer > (invMax * .15))
                    {
                        indOp = 0f;
                    }
                    if (invTimer <= (invMax * .15) && invTimer > (invMax * .1))
                    {
                        indOp = 1f;
                    }
                    if (invTimer <= (invMax * .1) && invTimer > (invMax * .05))
                    {
                        indOp = 0f;
                    }
                    if (invTimer <= (invMax * .05))
                    {
                        indOp = 1f;
                    }
                }

                //score velocity
                if (score >= scoreCH)
                {
                    minVel++;
                    maxVel++;
                    scoreCH = (int)(scoreCH * multiplyVal);
                    if (multiplyVal > 1.1)
                        multiplyVal -= .1;
                }
                //if (score >= 1000 && score < 2000)
                //{
                //    minVel = 4;
                //    maxVel = 9;
                //}
                //if (score >= 2001 && score < 3000)
                //{
                //    minVel = 5;
                //    maxVel = 10;
                //}
                //if (score >= 3001 && score < 4000)
                //{
                //    minVel = 6;
                //    maxVel = 11;
                //}
                //if (score >= 4001 && score < 5000)
                //{
                //    minVel = 7;
                //    maxVel = 12;
                //}
                //if (score >= 5001 && score < 7500)
                //{
                //    minVel = 8;
                //    maxVel = 13;
                //}
                //if (score >= 7501 && score < 9000)
                //{
                //    minVel = 9;
                //    maxVel = 14;
                //}
                //if (score >= 9001 && score < 12000)
                //{
                //    minVel = 10;
                //    maxVel = 15;
                //}

                //asteroid gen 2
                //if(randy.Next())

                //asteroid movement
                for (int i = 0; i < asCount; i++)
                {
                    if (!(asPosY[i] > GraphicsDevice.Viewport.Height + 20))
                    {
                        asPosY[i] += (int)asVel[i];
                        asteroids[i] = new Rectangle(asPosX[i], asPosY[i], (int)(asSize[i] * globalSizeScale), (int)(asSize[i] * globalSizeScale));
                    }

                    if (asPosY[i] > GraphicsDevice.Viewport.Height + 20)
                    {
                        asSize[i] = randy.Next(10, 50);
                        asPosX[i] = randy.Next(GraphicsDevice.Viewport.Width - 25);
                        asPosY[i] = randy.Next(-200, -25);
                        asteroids[i] = new Rectangle(asPosX[i], asPosY[i], (int)(asSize[i] * globalSizeScale), (int)(asSize[i] * globalSizeScale));
                        asVel[i] = randy.Next(minVel, maxVel);
                    }
                }


                //background star movement
                for (int i = 0; i < stCount; i++)
                {
                    if (!(stPosY[i] > GraphicsDevice.Viewport.Height + 20))
                    {
                        stPosY[i] += (int)stVel[i];
                        backStars[i] = new Rectangle(stPosX[i], stPosY[i], 1, 1);
                    }

                    if (stPosY[i] > GraphicsDevice.Viewport.Height + 20)
                    {
                        stPosX[i] = randy.Next(GraphicsDevice.Viewport.Width);
                        stPosY[i] = randy.Next(-200, -25);
                        backStars[i] = new Rectangle(stPosX[i], stPosY[i], 1, 1);
                        stVel[i] = randy.Next(minVel, maxVel);
                    }
                }



                //rgb timer
                rgbTimer++;
                if (rgbTimer > 0 && rgbTimer < 3)
                    rgbC = Color.Red;
                if (rgbTimer > 4 && rgbTimer < 8)
                    rgbC = Color.Orange;
                if (rgbTimer > 9 && rgbTimer < 12)
                    rgbC = Color.Yellow;
                if (rgbTimer > 13 && rgbTimer < 15)
                    rgbC = Color.Green;
                if (rgbTimer > 16 && rgbTimer < 18)
                    rgbC = Color.Blue;
                if (rgbTimer > 19 && rgbTimer < 21)
                    rgbC = Color.Purple;
                if (rgbTimer > 22)
                    rgbTimer = 0;



                //Horizontal Movement
                if (kb.IsKeyDown(Keys.A) && !hitbox.Intersects(new Rectangle(0, 0, 3, GraphicsDevice.Viewport.Height)))
                {
                    if (accX > -10)
                    {
                        accX -= .5;
                        rAngle -= .1f;
                    }

                }
                if (kb.IsKeyDown(Keys.D) /*!hitbox.Intersects(new Rectangle(GraphicsDevice.Viewport.Width, 0, 3, GraphicsDevice.Viewport.Height))*/)
                {
                    if (accX < 10)
                    {
                        accX += .5;
                        rAngle += .1f;
                    }

                }
                else if (accX > 0 && !kb.IsKeyDown(Keys.D) && !(ship.X > GraphicsDevice.Viewport.Width - ship.Width))
                {
                    accX -= .5;
                    rAngle -= .1f;
                }

                else if (accX < 0 && !kb.IsKeyDown(Keys.A) && !(ship.X < 0 + ship.Width))
                {
                    accX += .5;
                    rAngle += .1f;
                }



                //Vertical Movement
                if (kb.IsKeyDown(Keys.W))
                {
                    if (accY > -10)
                    {
                        accY -= .5;
                        //rAngle -= .1f;
                    }

                }
                if (kb.IsKeyDown(Keys.S))
                {
                    if (accY < 10)
                    {
                        accY += .5;
                        //rAngle += .1f;
                    }

                }
                else if (accY < 0 && !kb.IsKeyDown(Keys.W) && !(ship.Y < 0 + ship.Height))
                {
                    accY += .5;
                    //rAngle += .1f;
                }

                else if (accY > 0 && !kb.IsKeyDown(Keys.S) && !(ship.Y > GraphicsDevice.Viewport.Height - ship.Height))
                {
                    accY -= .5;
                    //rAngle-=.1f;
                }



                ship.X += (int)accX;
                //if(accX > 0)
                //    trail.X += (int)(accX - .1);
                //if(accX < 0)
                //    trail.X += (int)(accX + .1);
                hitbox.X = ship.X - ship.Width / 2;
                ship.Y += (int)accY;
                //if (accY > 0)
                //    trail.Y += (int)(accY - .1);
                //if (accY < 0)
                //    trail.Y += (int)(accY + .1);
                hitbox.Y = ship.Y - ship.Height / 2;

                if (trail.X < ship.X)
                    trail.X += (ship.X - trail.X) / 2;
                if (trail.X > ship.X)
                    trail.X -= (trail.X - ship.X) / 2;
                if (trail.Y < ship.Y)
                    trail.Y += (ship.Y - trail.Y) / 2;
                if (trail.Y > ship.Y)
                    trail.Y -= (trail.Y - ship.Y) / 2;

                if (hitbox.Intersects(new Rectangle(0, 0, GraphicsDevice.Viewport.Width, 1)) || hitbox.Intersects(new Rectangle(0, 0, 1, GraphicsDevice.Viewport.Height)) || hitbox.Intersects(new Rectangle(0, GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width, 1)) || hitbox.Intersects(new Rectangle(GraphicsDevice.Viewport.Width, 0, 1, GraphicsDevice.Viewport.Height))) /////////////add boost to prevent sticking
                {
                    if (accX == 0 && accY == 0) //////////////////////////////////adjust collision\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                    {
                        if (kb.IsKeyDown(Keys.A))
                            accX = 5;
                        if (kb.IsKeyDown(Keys.D))
                            accX = -5;
                        if (kb.IsKeyDown(Keys.W))
                            accY = -5;
                        if (kb.IsKeyDown(Keys.S))
                            accY = 5;
                    }
                    accX = -accX;
                    accY = -accY;
                }

                if (hitbox.Right > GraphicsDevice.Viewport.Width)
                {
                    ship.X = ship.X - 30;
                }

                if (hitbox.X < 0)
                {
                    ship.X = ship.X + 30;
                }

                if (hitbox.Bottom > GraphicsDevice.Viewport.Height)
                {
                    ship.Y = ship.Y - 30;
                }

                if (hitbox.Y < 0)
                {
                    ship.Y = ship.Y + 30;
                }
            }

            oldKB = kb;
            oldMouse = mouse;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            //switch(state)
            //{
            //    case GameState.main:
            //        break;
            //    case GameState.play:
            //        break;
            //}

            if (state == GameState.main) //////////////////////////////////main
            {
                for (int i = 0; i < stCount; i++)
                {
                    spriteBatch.Draw(pixel, backStars[i], stColor[i]);
                }
                //spriteBatch.DrawString(vhsFont, "INSERT TITLE GRAPHIC HERE LOL", new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 4), rgbC * animOp);
                spriteBatch.Draw(title, titleRec, tsRec, Color.White * animOp);
                for (int i = 0; i < buttonCount; i++)
                {
                    spriteBatch.Draw(spriteSheet1, buttonsRec[i], buttonsTex[i], buttonsColor[i]/** animOp*/);
                }
            }
            if (state == GameState.play) //////////////////////////////////play
            {
                for (int i = 0; i < stCount; i++)
                {
                    spriteBatch.Draw(pixel, backStars[i], stColor[i]);
                }
                for (int i = 0; i < asCount; i++)
                {
                    spriteBatch.Draw(pixel, asteroids[i], Color.Gray);
                }

                spriteBatch.Draw(pixel, trail, null, (rgbC * .4f) * indOp, rAngle, shipOrigin, SpriteEffects.None, 0);
                spriteBatch.Draw(pixel, ship, null, Color.MediumPurple * indOp, rAngle, shipOrigin, SpriteEffects.None, 0);
                //spriteBatch.Draw(pixel, hitbox, Color.Gray * .5f);
                //spriteBatch.Draw(pixel, hitbox, null, Color.Gray * .8f, rAngle, shipOrigin, SpriteEffects.None, 0);
                //spriteBatch.DrawString(vhsFont, "Score: ", new Vector2(10, 0), rgbC);
                spriteBatch.Draw(spriteSheet1, new Rectangle(10, 6, 86, 18), new Rectangle(485, 157, 86, 18), rgbC);
                spriteBatch.DrawString(vhsFont, "" + score, new Vector2(105, 0), Color.White);
                //spriteBatch.DrawString(vhsFont, "Health: ", new Vector2(10, 25), rgbC);
                spriteBatch.Draw(spriteSheet1, new Rectangle(10, 30, 38, 18), new Rectangle(117, 499, 38, 18), rgbC);
                spriteBatch.DrawString(vhsFont, "" + health, new Vector2(55, 25), Color.White);
                spriteBatch.Draw(spriteSheet1, new Rectangle(GraphicsDevice.Viewport.Width / 2 - 134, 5, 268, 18), new Rectangle(0, 123, 268, 18), Color.White);

                if (isDebugTrue)
                {
                    spriteBatch.Draw(pixel, new Rectangle(0, GraphicsDevice.Viewport.Height - 50, GraphicsDevice.Viewport.Width, 50), Color.SlateGray * .3f);
                    spriteBatch.DrawString(debugFont, screenRes, new Vector2(0, GraphicsDevice.Viewport.Height - 50), Color.Orange);
                    spriteBatch.DrawString(debugFont, "Is Mouse Visible? " + IsMouseVisible, new Vector2(0, GraphicsDevice.Viewport.Height - 35), Color.White);
                    spriteBatch.DrawString(debugFont, "Is Window Resizable? " + Window.AllowUserResizing, new Vector2(0, GraphicsDevice.Viewport.Height - 20), Color.White);
                    spriteBatch.DrawString(debugFont, "Debug Mode? " + isDebugTrue, new Vector2(160, GraphicsDevice.Viewport.Height - 35), Color.White);
                    //spriteBatch.DrawString(debugFont, "Frame Rate: " + gameTime.TotalGameTime / gameTime.ElapsedGameTime, new Vector2(160, GraphicsDevice.Viewport.Height - 20), Color.White);
                    spriteBatch.DrawString(debugFont, "Is FPS Unlimited? " + IsFixedTimeStep, new Vector2(275, GraphicsDevice.Viewport.Height - 35), Color.White);
                    spriteBatch.DrawString(debugFont, "Global Scale " + globalSizeScale, new Vector2(275, GraphicsDevice.Viewport.Height - 20), Color.White);
                }

                //potential velocity bar takes abs value of accX and alligns it with rect behind sprite speedometer
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
