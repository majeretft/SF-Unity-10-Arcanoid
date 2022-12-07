using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

namespace _10_Arcanoid
{
    public class Game
    {
        private RenderWindow _window;

        private Ball _ball;
        private Brick[,] _bricks;
        private Platform _platform;

        private float platformY = 560;

        private Sound _soundWall;
        private Sound _soundBrick;

        private uint _windowWidth = 800;
        private uint _windowHeight = 600;
        private Random rnd = new Random();

        private Text _text;

        public Game()
        {
            _window = new RenderWindow(new VideoMode(_windowWidth, _windowHeight), "Arcanoid", Styles.Titlebar | Styles.Close)
            {
                Position = new Vector2i(100, 100),
            };

            _window.SetFramerateLimit(120);

            _window.Closed += (s, e) => Environment.Exit(0);
        }

        private void HandleBallLaunch(object sender, MouseButtonEventArgs e)
        {
            if (_platform == null || _ball == null)
                return;

            if (_ball.IsLaunched || e.Button != Mouse.Button.Left)
                return;

            _ball.Launch(4.5f, new Vector2f(0, -1));
        }

        private void HandlePlatformPosition(object sender, MouseMoveEventArgs e)
        {
            if (_platform == null || _ball == null)
                return;

            var x = e.X;

            if (e.X > 1280)
                x = 1280;
            if (e.X < 0)
                x = 0;

            _platform.SetPosition(new Vector2f(x, platformY));
            _ball.SetStartPosition(new Vector2f(x - _ball.Sprite.TextureRect.Width / 2, platformY - _ball.Sprite.TextureRect.Height));
        }

        public void Start()
        {
            _window.MouseMoved += HandlePlatformPosition;
            _window.MouseButtonPressed += HandleBallLaunch;

            _text = new Text("CONGRATS!!!", new Font("assets/arial.ttf"), 50)
            {
                Position = new Vector2f(230, 230),
            };

            LoadSounds();
            PlayMusic();

            var lvl = 1;

            while (_window.IsOpen)
            {
                if (lvl > 3)
                {
                    _window.DispatchEvents();
                    _window.Clear(Color.Black);
                    _window.Draw(_text);
                    _window.Display();
                }
                else
                {
                    CreateLevel(lvl);
                    var hasBricks = true;
                    _window.DispatchEvents();
                    Draw();
                    _window.Display();

                    while (hasBricks)
                    {
                        _window.DispatchEvents();

                        if (_ball.DetectCollision(_platform.Sprite, Ball.ColliderType.Platform))
                            _soundWall.Play();

                        hasBricks = false;
                        for (var r = 0; r < _bricks.GetLength(0); r++)
                        {
                            for (var c = 0; c < _bricks.GetLength(1); c++)
                            {
                                if (_bricks[r, c].Life > 0 && _ball.DetectCollision(_bricks[r, c].Sprite, Ball.ColliderType.Block))
                                {
                                    _soundBrick.Play();
                                    _bricks[r, c].Life--;
                                }

                                if (_bricks[r, c].Life > 0)
                                    hasBricks = true;
                            }
                        }

                        if (_ball.Fly())
                            _soundWall.Play();

                        Draw();

                        _window.Display();
                    }

                    lvl++;
                }
            }
        }

        private void Draw()
        {
            _window.Clear(Color.Black);
            _window.Draw(_ball.Sprite);
            _window.Draw(_platform.Sprite);

            for (var r = 0; r < _bricks.GetLength(0); r++)
            {
                for (var c = 0; c < _bricks.GetLength(1); c++)
                {
                    if (_bricks[r, c].Life > 0)
                        _window.Draw(_bricks[r, c].Sprite);
                }
            }
        }

        private void PlayMusic()
        {
            new Music("assets/music.wav")
            {
                Loop = true,
                Volume = 10,
            }.Play();
        }

        private void LoadSounds()
        {
            _soundBrick = new Sound(new SoundBuffer("assets/Arkanoid SFX (1).wav"))
            {
                Volume = 20,
            };

            _soundWall = new Sound(new SoundBuffer("assets/Arkanoid SFX (2).wav"))
            {
                Volume = 20,
            };
        }

        private void CreateLevel(int lvlNum)
        {
            switch (lvlNum)
            {
                case 1: CreateBricks(3, 8, 1, 680, 260); break;
                case 2: CreateBricks(4, 10, 2, 740, 320); break;
                case 3: CreateBricks(5, 12, 3, 760, 400); break;
                default: CreateBricks(4, 10, 1, 700, 260); break;
            }

            _ball = new Ball("assets/Ball.png", 0, _windowWidth, 0, _windowHeight);
            _platform = new Platform("assets/Stick.png");
        }

        private void CreateBricks(int rows, int cols, int maxHp, float allWidth, float allHeight)
        {
            _bricks = new Brick[rows, cols];

            var marginCol = (allWidth - cols * 50) / cols;
            var marginRow = (allHeight - rows * 15) / rows;

            var windowOffsetX = (_windowWidth - allWidth) / 2;
            var windowOffsetY = 50;

            for (var r = 0; r < rows; r++)
            {
                var hp = maxHp > 1 ? rnd.Next(1, maxHp + 1) : 1;

                for (var c = 0; c < cols; c++)
                {
                    var pos = new Vector2f(c * (50 + marginCol) + windowOffsetX, r * (15 + marginRow) + windowOffsetY);
                    _bricks[r, c] = new Brick($"assets/Block{hp}.png", hp, pos);
                }
            }
        }
    }
}
