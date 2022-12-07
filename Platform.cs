using SFML.Graphics;
using SFML.System;

namespace _10_Arcanoid
{
    public class Platform
    {
        private Texture _texture;
        public Sprite Sprite { get; private set; }

        public Platform(string texturePath)
        {
            _texture = new Texture(texturePath);
            Sprite = new Sprite(_texture);
        }

        public void SetPosition(Vector2f posCorner)
        {
            var offset = Sprite.TextureRect.Width / 2;
            posCorner.X -= offset;

            Sprite.Position = posCorner;
        }
    }
}
