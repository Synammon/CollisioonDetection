using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionDetection
{
    public class Sprite
    {
        public Vector2 Position;
        public Vector2 Velocity;

        public float Scale { get; set; }
        public float Rotation { get; set; }

        public Texture2D Texture { get; private set; }
        public Color[] ColorData { get; private set; }
        public Rectangle BoundingBox => new Rectangle(
            (int)Position.X, 
            (int)Position.Y, 
            (int)(Texture.Width * Scale), 
            (int)(Texture.Height * Scale));
        public Vector2 Origin => new Vector2(
            Texture.Width / 2,
            Texture.Height / 2);
        public Matrix Transformation =>
            Matrix.CreateTranslation(new Vector3(-Origin, 0))
            * Matrix.CreateScale(Scale)
            * Matrix.CreateRotationZ(Rotation)
            * Matrix.CreateTranslation(new Vector3(Position, 0));

        public Sprite(Texture2D texture2D)
        {
            Texture = texture2D;
            ColorData = new Color[Texture.Width * Texture.Height];
            Texture.GetData(ColorData);
            Scale = 1f;
            Rotation = 0f;
        }

        public void Update(GameTime gameTime)
        {
            Position += Velocity;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(
                Texture, 
                Position + Origin * Scale, 
                null, 
                Color.White,
                Rotation,
                Origin,
                Scale,
                SpriteEffects.None,
                0f);
        }
    }
}
