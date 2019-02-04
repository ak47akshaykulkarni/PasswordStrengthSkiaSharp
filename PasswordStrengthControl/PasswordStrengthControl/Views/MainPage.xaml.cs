using System;
using System.Collections.Generic;
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
        private SKPoint point0 = new SKPoint(0, 0);
        private SKPoint point1;
        private SKPoint point2;
        private SKPoint point3;
        private SKPoint point4;

        private PasswordScore _score;
        public PasswordScore Score
        {
            get => _score;
            set
            {
                _score = value;
                SkCanvasView.InvalidateSurface();
            }
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

            canvas.Clear();

            SKPaint paintBlack = new SKPaint() { StrokeWidth = 40, Color = SKColors.Black, IsStroke = true };
            SKPaint paintRed = new SKPaint() { StrokeWidth = 40, Color = SKColors.Red, IsStroke = true };
            SKPaint paintYellow = new SKPaint() { StrokeWidth = 40, Color = SKColors.Yellow, IsStroke = true };
            SKPaint paintBlue = new SKPaint() { StrokeWidth = 40, Color = SKColors.CornflowerBlue, IsStroke = true };
            SKPaint paintGreen = new SKPaint() { StrokeWidth = 40, Color = SKColors.Green, IsStroke = true };
            
            if (Score == PasswordScore.Blank)
            {
                canvas.DrawLine(point0, point1, paintBlack); return;
            }

            if (Score == PasswordScore.VeryWeak)
            {
                canvas.DrawLine(point0, point1, paintRed); return;
            }

            if (Score == PasswordScore.Weak)
            {
                canvas.DrawLine(point0, point1, paintRed);
                canvas.DrawLine(point1, point2, paintYellow); return;
            }

            if (Score == PasswordScore.Medium)
            {
                canvas.DrawLine(point0, point1, paintRed);
                canvas.DrawLine(point1, point2, paintYellow);
                canvas.DrawLine(point2, point3, paintBlue); return;
            }

            if (Score == PasswordScore.Strong || Score == PasswordScore.VeryStrong)
            {
                canvas.DrawLine(point0, point1, paintRed);
                canvas.DrawLine(point1, point2, paintYellow);
                canvas.DrawLine(point2, point3, paintBlue);
                canvas.DrawLine(point3, point4, paintGreen);
            }
        }

        private void Entry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Score = CheckStrength(e.NewTextValue);
        }

        public static PasswordScore CheckStrength(string password)
        {
            int score = 0;

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