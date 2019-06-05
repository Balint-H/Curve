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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using static Curve.BresenhamRedux;

namespace Curve
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Point mousePos;
        Point headPos=new Point(20,20);
        Polyline polyline;
        double stepSize = 3; 
        bool leftTurn = false;
        bool rightTurn = false;
        double curangle = 0;
        Thread updateThread;
        bool run = true;
        Dictionary<Point, bool> collision;
        int baselength = 200;
        int dotlength = 5;
        int gaplength = 20;

       SolidColorBrush myBrush = new SolidColorBrush(Color.FromArgb(128, 100, 100, 100));

        public MainWindow()
        {
            InitializeComponent();
            polyline = new Polyline();
            polyline.Stroke = myBrush;
            polyline.StrokeThickness = 5;
            collision = new Dictionary<Point, bool>();

            polyline.Points = new PointCollection(); // Starts with an empty snake
            canvas.Children.Add(polyline);
            

            updateThread = new Thread(() => SnakeUpdate());
            updateThread.IsBackground = true;
            updateThread.Start();
        }
        
        protected void SnakeUpdate()
        {
            int round = 0;
            Thread.Sleep(1000);
            while (run)
            {
                if (leftTurn) curangle -= 0.1;
                else if (rightTurn) curangle += 0.1;
                headPos.X += Math.Cos(curangle) * stepSize;
                headPos.Y += Math.Sin(curangle) * stepSize;

                if (headPos.X < 4) run = false;
                if (headPos.Y < 4) run = false;
                if (headPos.X > ActualWidth) run = false;
                if (headPos.Y > ActualHeight) run = false;


                Application.Current.Dispatcher.Invoke((Action)delegate
                {

                    /* Old collision detection
                    for (int j = 0; j < canvas.Children.Count; j++)
                    {
                        Polyline curLine = canvas.Children[j] as Polyline;
                        for (int i = 0; i < curLine.Points.Count; i++)
                        {
                            if (headPos.X > curLine.Points[i].X - 2 && headPos.X < curLine.Points[i].X + 2 && headPos.Y < curLine.Points[i].Y + 2 && headPos.Y > curLine.Points[i].Y - 2)
                            {
                                run = false;
                            }
                        }
                    }
                    */


                    

                    if (round < baselength)
                    {
                        polyline.Points.Add(headPos);
                        if (CollisionUpdate(collision, headPos, curangle, stepSize, 1)) run = false;
                    }
                    else if (round == baselength)
                    {
                        polyline = new Polyline();
                        polyline.Stroke = myBrush;

                        polyline.StrokeThickness = 5;

                        polyline.Points = new PointCollection(); // Starts with an empty snake
                        canvas.Children.Add(polyline);
                        polyline.Points.Add(headPos);
                        if (CollisionUpdate(collision, headPos, curangle, stepSize, 1)) run = false;
                    }
                    else if (round < baselength+dotlength)
                    {
                        if (CollisionUpdate(collision, headPos, curangle, stepSize, 1)) run = false;
                        polyline.Points.Add(headPos);
                        
                    }
                    else if (round < baselength+dotlength+gaplength)
                    {
                        polyline.Points.RemoveAt(polyline.Points.Count - dotlength);
                       polyline.Points.Add(headPos);
                        if (CollisionCheck(collision, headPos, curangle, stepSize, 1)) run = false;
                    }
                    else
                    {
                        round = 0;
                    }

                });
                Thread.Sleep(40);
                round++;
            }
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Left:
                    rightTurn = false;
                    leftTurn = true;
                    break;
                case Key.Right:
                    leftTurn = false;
                    rightTurn = true;
                    break;
                case Key.R:
                    canvas.Children.Clear();
                    run = false;
                    updateThread.Join();
                    run = true;
                    curangle = 0;
                    headPos = new Point(20, 20) ;
                    polyline = new Polyline();
                    polyline.Stroke = Brushes.Black;
                    polyline.StrokeThickness = 5;
                    collision = new Dictionary<Point, bool>();

                    polyline.Points = new PointCollection(); // Starts with an empty snake
                    canvas.Children.Add(polyline);

                    updateThread = new Thread(() => SnakeUpdate());
                    updateThread.IsBackground = true;
                    updateThread.Start();
                    break;

            }
        }

        public void addMarker(Ellipse marker)
        {
            canvas.Children.Add(marker);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    leftTurn = false;
                    break;
                case Key.Right:
                    rightTurn = false;
                    break;
            }
        }
    }
}
