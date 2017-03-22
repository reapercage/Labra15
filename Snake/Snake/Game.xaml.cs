using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Snake
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    /// 
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
    public partial class Game : Window
    {
        //variables and consts
        private const int minimi = 5;
        private const int maxWidth = 620;
        private const int maxHeight = 380;
        private int snakeWidth = 10;
        private int snakeLength = 100;
        private int easiness = 20; //timerin ajastin aika ms
        private int score = 0;
        private List<Point> bonusPoints = new List<Point>(); //omena kokoelma
        private const int bonusCount = 20;
        private List<Point> snakeParts = new List<Point>();
        private Point startingPoint = new Point(100, 100);
        private Point currentPosition = new Point();
        private Direction lastDirection = Direction.Right;
        private Direction currentDirection = Direction.Right; //alussa lähtee aina oikealle
        private Random rnd = new Random(); //pisteiden arvontaa varten
        private DispatcherTimer timer;

        public Game()
        {
            InitializeComponent();
            //tarvittavat alustukset
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, easiness);
            timer.Tick += new EventHandler(timer_Tick);
            //määritellään ikkunalle tapahtumankäsittelijä näppäimistön kuuntelua varten
            this.KeyDown += new KeyEventHandler(OnButtonKeyDown);
            //piirretään omenat ja käärme
            IniBonusPoints();
            PaintSnake(startingPoint);
            currentPosition = startingPoint;
            //start game
            timer.Start();
        }

        private void IniBonusPoints()
        {
            for(int n = 0; n < bonusCount; n++)
            {
                PaintBonus(n);
            }
        }
        private void PaintBonus(int index)
        {
            //arvotaan omnelle piste eli X ja Y -koordinaatti
            Point point = new Point(rnd.Next(minimi, maxWidth),
                                    rnd.Next(minimi, maxHeight));
            //omenan piirto
            Ellipse omena = new Ellipse();
            omena.Fill = Brushes.Red;
            omena.Width = snakeWidth;
            omena.Height = snakeWidth;
            Canvas.SetTop(omena, point.Y);
            Canvas.SetLeft(omena, point.X);
            paintCanvas.Children.Insert(index, omena);
            bonusPoints.Insert(index, point);
        }
        private void PaintSnake(Point currentpoint)
        {
            Ellipse snake = new Ellipse();
            snake.Fill = Brushes.Green;
            snake.Width = snakeWidth;
            snake.Height = snakeWidth;
            Canvas.SetTop(snake, currentpoint.Y);
            Canvas.SetLeft(snake, currentpoint.X);
            int count = paintCanvas.Children.Count;
            paintCanvas.Children.Add(snake);
            snakeParts.Add(currentPosition);
            //rajoitetaan käärmeen pituutta
            //TODO
        }
        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            //muutetaan suuntaa näppäimistön painalluksen mukaan
            //mutta ei sallita 180 asteen käännöstä
            switch (e.Key)
            {
                case Key.P:
                    if (timer.IsEnabled)
                        timer.Stop();
                    else
                        timer.Start();
                    break;
                case Key.Escape:
                    GameOver();
                    break;
                case Key.Left:
                    if (lastDirection != Direction.Right)
                        currentDirection = Direction.Left;
                    break;
                case Key.Up:
                    if (lastDirection != Direction.Down)
                        currentDirection = Direction.Up;
                    break;
                case Key.Right:
                    if (lastDirection != Direction.Left)
                        currentDirection = Direction.Right;
                    break;
                case Key.Down:
                    if (lastDirection != Direction.Up)
                        currentDirection = Direction.Down;
                    break;
            }
            lastDirection = currentDirection;
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            switch (currentDirection)
            {
                case Direction.Up:
                    currentPosition.Y -= 1;
                    break;
                case Direction.Right:
                    currentPosition.X += 1;
                    break;
                case Direction.Down:
                    currentPosition.Y += 1;
                    break;
                case Direction.Left:
                    currentPosition.X -= 1;
                    break;
                default:
                    break;
            }
            PaintSnake(currentPosition);
            //törmäystarkastelu 1-3
            //TT#1 tarkistetaan onko kanvaasilla
            if((currentPosition.X > MaxWidth) || (currentPosition.X < minimi) ||
              (currentPosition.Y > maxHeight) || (currentPosition.Y < minimi))
              GameOver();
            //TT#2 tarkistetaan ettei pure omaa häntäänsä
            for (int i = 0; i < snakeParts.Count - snakeWidth * 2; i++)
            {
                Point p = new Point(snakeParts[i].X, snakeParts[i].Y);
                if ((Math.Abs(p.X - currentPosition.X) < snakeWidth) &&
                    (Math.Abs(p.Y - currentPosition.Y) < snakeWidth))
                    GameOver();
            }
            //TT#3
            //tarkistetaan osuuko omenaan
            int n = 0;
            foreach (Point point in bonusPoints)
            {
                if ((Math.Abs(point.X - currentPosition.X) < snakeWidth) &&
                    Math.Abs(point.Y - currentPosition.Y) < snakeWidth)
                {
                    //syödään omena
                    score += 10;
                    this.Title = "SnakeWPF your score: " + score;
                    bonusPoints.RemoveAt(n);
                    paintCanvas.Children.RemoveAt(n);
                    PaintBonus(n);
                    break;
                }
                n++;
            }
        }

        private void GameOver()
        {
            timer.Stop();
            MessageBox.Show("Your score: " + score);
            this.Close();        
        }
        private void GameOverShow()
        {
            //TODO
        }
    }
}
