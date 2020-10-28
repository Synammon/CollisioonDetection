using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation;
using System;
using System.Drawing.Imaging;

namespace CollisionDetection
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Sprite ninja;
        private Sprite indie;
        private Color background;
        public static Random Random = new Random();
        public static Rectangle Screen = new Rectangle(0, 0, 800, 600);
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = Screen.Width;
            _graphics.PreferredBackBufferHeight = Screen.Height;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            ninja = new Sprite(Content.Load<Texture2D>(@"NinjaGirl\Idle__000"))
            {
                Scale = (float)(.5 + Random.NextDouble()),
                Rotation = (float)(Math.PI * 2 * Random.NextDouble())
            };

            indie = new Sprite(Content.Load<Texture2D>(@"IndianaJones\Idle__000"))
            {
                Scale = (float)(.5 + Random.NextDouble()),
                Rotation = (float)(Math.PI * 2 * Random.NextDouble())
            };
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            KeyboardState keystate = Keyboard.GetState();

            background = Color.CornflowerBlue;
            ninja.Velocity = Vector2.Zero;
            indie.Velocity = Vector2.Zero;

            if (keystate.IsKeyDown(Keys.Q))
            {
                ninja.Rotation -= 0.05f;
            }
            else if (keystate.IsKeyDown(Keys.R))
            {
                ninja.Rotation += 0.05f;
            }

            if (keystate.IsKeyDown(Keys.W))
            {
                ninja.Velocity.Y = -4;
            }
            else if (keystate.IsKeyDown(Keys.S))
            {
                ninja.Velocity.Y = 4;
            }

            if (keystate.IsKeyDown(Keys.A))
            {
                ninja.Velocity.X = -4;
            }
            else if (keystate.IsKeyDown(Keys.D))
            {
                ninja.Velocity.X = 4;
            }

            if (keystate.IsKeyDown(Keys.PageUp))
            {
                indie.Rotation -= 0.05f;
            }
            else if (keystate.IsKeyDown(Keys.PageDown))
            {
                indie.Rotation += 0.05f;
            }

            if (keystate.IsKeyDown(Keys.Up))
            {
                indie.Velocity.Y = -4;
            }
            else if (keystate.IsKeyDown(Keys.Down))
            {
                indie.Velocity.Y = 4;
            }

            if (keystate.IsKeyDown(Keys.Left))
            {
                indie.Velocity.X = -4;
            }
            else if (keystate.IsKeyDown(Keys.Right))
            {
                indie.Velocity.X = 4;
            }

            indie.Update(gameTime);
            ninja.Update(gameTime);

            ConstrainSprite(indie);
            ConstrainSprite(ninja);

            if (TransformedPixelsCollide(indie, ninja))
            {
                background = Color.Red;
            }
            else
            {
                background = Color.CornflowerBlue;
            }

            base.Update(gameTime);
        }

        private bool TransformedPixelsCollide(Sprite spriteA, Sprite spriteB)
        {
            // Transformation of sprite A to sprite B
            Matrix mat1to2 = spriteA.Transformation * Matrix.Invert(spriteB.Transformation);

            // Loop over the source rectangle of sprite A
            for (int x1 = 0; x1 < spriteA.Texture.Width; x1++)
            {
                for (int y1 = 0; y1 < spriteA.Texture.Height; y1++)
                {
                    // Calculate the position of the pixel in sprite A in sprite B
                    Vector2 pos1 = new Vector2(x1, y1);
                    Vector2 pos2 = Vector2.Transform(pos1, mat1to2);

                    // Round to the nearest pixel
                    int x2 = (int)Math.Round(pos2.X);
                    int y2 = (int)Math.Round(pos2.Y);

                    // Check to see if the pixel in sprite A is in the bounds of sprite B
                    if ((x2 >= 0) && (x2 < spriteB.Texture.Width))
                    {
                        if ((y2 >= 0) && (y2 < spriteB.Texture.Height))
                        {
                            // If the alpha channel of sprite A and sprite B is greater
                            // than zero we have a collision
                            if (spriteA.ColorData[x1 + y1 * spriteA.Texture.Width].A > 0)
                            {
                                if (spriteB.ColorData[x2 + y2 * spriteB.Texture.Width].A > 0)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            // No collision occurred
            return false;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(background);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointClamp);

            indie.Draw(_spriteBatch);
            ninja.Draw(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public static bool Collides(Sprite spriteA, Sprite spriteB)
        {
            return spriteA.BoundingBox.Intersects(spriteB.BoundingBox);
        }

        public static void ConstrainSprite(Sprite sprite)
        {
            if (sprite.Position.X < 0)
            {
                sprite.Position.X = 0;
                sprite.Velocity.X *= -1;
            }

            if (sprite.Position.X + sprite.BoundingBox.Width > Screen.Width)
            {
                sprite.Position.X = Screen.Width - sprite.BoundingBox.Width;
                sprite.Velocity.X *= -1;
            }

            if (sprite.Position.Y < 0)
            {
                sprite.Position.Y = 0;
                sprite.Velocity.Y *= -1;
            }

            if (sprite.Position.Y + sprite.BoundingBox.Height > Screen.Height)
            {
                sprite.Position.Y = Screen.Height - sprite.BoundingBox.Height;
                sprite.Velocity.Y *= -1;
            }
        }

        public static bool PixelsCollide(Sprite spriteA, Sprite spriteB)
        {
            Rectangle a = spriteA.BoundingBox;
            Rectangle b = spriteB.BoundingBox;

            // Find the bounds of the rectangle intersection
            int top = Math.Max(a.Top, b.Top);
            int bottom = Math.Min(a.Bottom, b.Bottom);
            int left = Math.Max(a.Left, b.Left);
            int right = Math.Min(a.Right, b.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = spriteA.ColorData[(x - a.Left) +
                                            (y - a.Top) * a.Width];
                    Color colorB = spriteB.ColorData[(x - b.Left) +
                                            (y - b.Top) * b.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A > 0 && colorB.A > 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }
    }
}
