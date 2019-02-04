using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;

namespace PasswordStrengthControl.Views
{
    public enum PasswordScore
    {
        Blank = 0,
        VeryWeak = 1,
        Weak = 2,
        Medium = 3,
        Strong = 4,
        VeryStrong = 5
    }
    public partial class MainPage : ContentPage
    {
        Stopwatch stopwatch = new Stopwatch();
        private SKPoint point0, point1, point2, point3, point4 = new SKPoint(0,0);
        float scale;

        private bool isIncreament;
        private PasswordScore observedScore;
        private PasswordScore _score;
        public PasswordScore Score
        {
            get => _score;
            set
            {
                if(Score == value)return;
                _score = value;
                AnimationLoop();
                //SkCanvasView.InvalidateSurface();
            }
        }


        async Task AnimationLoop()
        {
            stopwatch.Start();
            bool pageIsActive = true;
            
            while (pageIsActive)
            {
                double t = stopwatch.Elapsed.TotalSeconds;
                if (t > 0.5d) pageIsActive = false;
                scale = (float) t *2;
                SkCanvasView.InvalidateSurface();
                await Task.Delay(TimeSpan.FromSeconds(1.0 / 30));
            }

            stopwatch.Stop();
            stopwatch.Reset();
        }

        public MainPage()
        {
            InitializeComponent();
            Score = PasswordScore.Blank;
        }
        
        private void SKCanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            point1 = new SKPoint((SkCanvasView.CanvasSize.Width) / 4, 0);
            point2 = new SKPoint((SkCanvasView.CanvasSize.Width) / 2, 0);
            point3 = new SKPoint(3 * (SkCanvasView.CanvasSize.Width) / 4, 0);
            point4 = new SKPoint(SkCanvasView.CanvasSize.Width, 0);
            float scaling;

            
            if (observedScore != _score)
            {
                canvas.Clear();
                isIncreament = observedScore <= _score;
                observedScore = _score;
            }

            if (isIncreament)
                scaling = ((SkCanvasView.CanvasSize.Width) / 4) * scale;
            else
            {
                canvas.Clear();
                scaling = ((SkCanvasView.CanvasSize.Width) / 4) * -scale;
                scaling = scaling < 0 ? scaling : 0;
            }

            var paintRed = new SKPaint() { StrokeWidth = 40, Color = SKColors.Red, IsStroke = true };
            var paintYellow = new SKPaint() { StrokeWidth = 40, Color = SKColors.Yellow, IsStroke = true };
            var paintBlue = new SKPaint() { StrokeWidth = 40, Color = SKColors.CornflowerBlue, IsStroke = true };
            var paintGreen = new SKPaint() { StrokeWidth = 40, Color = SKColors.Green, IsStroke = true };

            switch (Score)
            {
                case PasswordScore.Blank:
                    canvas.DrawLine(point0, new SKPoint(point1.X + scaling, 0), paintRed); return;
                case PasswordScore.VeryWeak:
                    if (isIncreament)
                    {
                        canvas.DrawLine(point0, new SKPoint(point0.X + scaling, 0), paintRed); return;
                    }
                    canvas.DrawLine(point1, new SKPoint(point2.X + scaling, 0), paintYellow);
                    canvas.DrawLine(point0, point1, paintRed); return;

                case PasswordScore.Weak:
                    canvas.DrawLine(point0, point1, paintRed);
                    if (isIncreament)
                    {
                        canvas.DrawLine(point1, new SKPoint(point1.X + scaling, 0), paintYellow); return;
                    }
                    canvas.DrawLine(point2, new SKPoint(point3.X + scaling, 0), paintBlue);
                    canvas.DrawLine(point1, point2, paintYellow); return;

                case PasswordScore.Medium:
                    canvas.DrawLine(point0, point1, paintRed);
                    canvas.DrawLine(point1, point2, paintYellow);

                    if (isIncreament)
                    {
                        canvas.DrawLine(point2, new SKPoint(point2.X + scaling, 0), paintBlue); return;
                    }
                    canvas.DrawLine(point3, new SKPoint(point4.X + scaling, 0), paintGreen);
                    canvas.DrawLine(point2, point3, paintBlue); return;

                case PasswordScore.VeryStrong:
                case PasswordScore.Strong:
                    
                    canvas.DrawLine(point0, point1, paintRed);
                    canvas.DrawLine(point1, point2, paintYellow);
                    canvas.DrawLine(point2, point3, paintBlue);
                    canvas.DrawLine(point3, new SKPoint((isIncreament ? point3.X : point4.X) + scaling, 0), paintGreen);
                    break;
            }
        }

        private void Entry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Score = CheckStrength(e.NewTextValue);
        }

        public static PasswordScore CheckStrength(string password)
        {
            var score = 0;

            if (password.Length < 1)
                return PasswordScore.Blank;
            if (password.Length <= 4)
                return PasswordScore.VeryWeak;

            if (password.Length >= 8)
                score++;
            if (password.Length >= 12)
                score++;
            if (Regex.Match(password, @"\d+", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @"[a-z]", RegexOptions.ECMAScript).Success &&
                Regex.Match(password, @"[A-Z]", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @"[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]", RegexOptions.ECMAScript).Success)
                score++;

            return (PasswordScore)score;
        }
    }
}