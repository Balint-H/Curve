using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Curve
{
    class BresenhamRedux
    {
        public static bool CollisionUpdate(Dictionary<Point, bool> dictIn, Point headIn, double angle, double radius, int prec)
        {
            long  headX = (int)(Math.Round(headIn.X, prec)*Math.Pow(10,prec));
            long headY = (int)(Math.Round(headIn.Y, prec) * Math.Pow(10, prec));

            long targetX = (int)(Math.Round(headIn.X+ radius * Math.Cos(angle), prec) * Math.Pow(10, prec));
            long targetY = (int)(Math.Round(headIn.Y + radius * Math.Sin(angle), prec) * Math.Pow(10, prec));
            Dictionary<Point, bool> Tally = new Dictionary<Point, bool>();

            int w = (int)(targetX - headX);
            int h = (int)(targetY - headY);
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;

            for (int i = 0; i < 2; i++)
            {
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    headX += dx1;
                    headY += dy1;
                }
                else
                {
                    headX += dx2;
                    headY += dy2;
                }
                Tally[new Point(headX, headY)] = true;
            }

            for (int i = 0; i < longest-2; i++)
            {
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    headX += dx1;
                    headY += dy1;
                }
                else
                {
                    headX += dx2;
                    headY += dy2;
                }
                for (int j=-1; j<2; j++)
                {
                    for(int k=-1; k<2; k++)
                    if (dictIn.ContainsKey(new Point(headX + j, headY+k)))
                    {
                        return true;
                    }
                }

                Tally[new Point(headX, headY)] = true;
            }
            foreach (var Cur in Tally)
            {
                dictIn.Add(Cur.Key, Cur.Value);
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    Ellipse marker=CreateEllipse(0.8, 0.8, Cur.Key.X / Math.Pow(10, prec), Cur.Key.Y / Math.Pow(10, prec));
                    ((MainWindow)Application.Current.MainWindow).addMarker(marker);
                });
            }

            return false;
        }
        public static bool CollisionCheck(Dictionary<Point, bool> dictIn, Point headIn, double angle, double radius, int prec)
        {

            long headX = (int)(Math.Round(headIn.X, prec) * Math.Pow(10, prec));
            long headY = (int)(Math.Round(headIn.Y, prec) * Math.Pow(10, prec));

            long targetX = (int)(Math.Round(headIn.X + radius * Math.Cos(angle), prec) * Math.Pow(10, prec));
            long targetY = (int)(Math.Round(headIn.Y + radius * Math.Sin(angle), prec) * Math.Pow(10, prec));

            int w = (int)(targetX - headX);
            int h = (int)(targetY - headY);
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;

            for (int i = 0; i < 2; i++)
            {
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    headX += dx1;
                    headY += dy1;
                }
                else
                {
                    headX += dx2;
                    headY += dy2;
                }
            }

            for (int i = 0; i < longest - 2; i++)
            {
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    headX += dx1;
                    headY += dy1;
                }
                else
                {
                    headX += dx2;
                    headY += dy2;
                }
                for (int j = -1; j < 2; j++)
                {
                    if (dictIn.ContainsKey(new Point(headX + j, headY)))
                    {
                        return true;
                    }
                }
                if (dictIn.ContainsKey(new Point(headX, headY - 1))) return true;
                if (dictIn.ContainsKey(new Point(headX, headY + 1))) return true;
            }
            
            return false;
        }


        static Ellipse CreateEllipse(double width, double height, double desiredCenterX, double desiredCenterY)
        {
            Ellipse ellipse = new Ellipse { Width = width, Height = height };

            SolidColorBrush mySolidColorBrush = new SolidColorBrush();

            // Describes the brush's color using RGB values. 
            // Each value has a range of 0-255.
            ellipse.StrokeThickness = 2;
            ellipse.Stroke = Brushes.Black;


            double left = desiredCenterX - (width / 2);
            double top = desiredCenterY - (height / 2);

            ellipse.Margin = new Thickness(left, top, 0, 0);
            return ellipse;
        }
    }
}
