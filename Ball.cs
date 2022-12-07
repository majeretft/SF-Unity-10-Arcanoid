using SFML.Graphics;
using SFML.System;
using System;

namespace _10_Arcanoid
{
    public class Ball
    {
        private readonly Texture _texture;
        public Sprite Sprite { get; private set; }
        
        public bool IsLaunched { get; set; }
        public float Speed { get; set; }
        public Vector2f direction;

        public int LeftBorder { get; set; }
        public uint RightBorder { get; set; }
        public int TopBorder { get; set; }
        public uint BottomBorder { get; set; }

        public Ball(string texturePath, int leftBorder, uint rightBorder, int topBorder, uint bottomBorder)
        {
            _texture = new Texture(texturePath);
            Sprite = new Sprite(_texture);

            LeftBorder = leftBorder;
            RightBorder = rightBorder;
            TopBorder = topBorder;
            BottomBorder = bottomBorder;
        }

        public void Launch(float speed, Vector2f direction)
        {
            if (IsLaunched)
                return;

            IsLaunched = true;
            Speed = speed;
            this.direction = direction;
        }

        public void SetStartPosition(Vector2f pos)
        {
            if (IsLaunched)
                return;

            Sprite.Position = pos;
        }

        public bool Fly()
        {
            if (!IsLaunched)
                return false; ;
            
            var isWall = false;

            if (Sprite.Position.X < LeftBorder || Sprite.Position.X + Sprite.TextureRect.Width > RightBorder)
            {
                direction.X *= -1;
                isWall = true;
            }

            if (Sprite.Position.Y < TopBorder)
            {
                direction.Y *= -1;
                isWall = true;
            }

            if (Sprite.Position.Y > BottomBorder)
                IsLaunched = false;

            Sprite.Position += direction * Speed;

            return isWall;
        }

        public bool DetectCollision(Sprite otherSprite, ColliderType colliderType)
        {
            var ballBounds = Sprite.GetGlobalBounds();
            var otherBounds = otherSprite.GetGlobalBounds();

            if (!ballBounds.Intersects(otherBounds))
                return false;

            switch (colliderType)
            {
                case ColliderType.Block:
                    direction.Y *= -1;
                    break;
                case ColliderType.Platform:
                    var ballCenterX = ballBounds.Left + ballBounds.Width / 2;
                    var otherCenterX = otherBounds.Left + otherBounds.Width / 2;
                    var deltaX = ballCenterX - otherCenterX;
                    var part = deltaX / ((otherBounds.Width + ballBounds.Width) / 2);
                    direction.Y *= -1;
                    direction.X = part;
                    break;
            }

            return true;
        }

        public enum ColliderType
        {
            Platform,
            Block,
        }
    }
}
