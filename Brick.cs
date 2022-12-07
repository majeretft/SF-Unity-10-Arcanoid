using SFML.Graphics;
using SFML.System;

namespace _10_Arcanoid
{
    public class Brick
    {
        public int Life { get; set; }

        private readonly Texture _texture;
        public Sprite Sprite { get; private set; }

        public Brick(string texturePath, int life, Vector2f position)
        {
            Life = life;
            _texture = new Texture(texturePath);
            Sprite = new Sprite(_texture)
            {
                Position = position,
            };
        }
    }
}
