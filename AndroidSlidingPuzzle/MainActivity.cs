using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Android.App;
using Android.Graphics;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace AndroidSlidingPuzzle
{
    [Activity(Label = "AndroidSlidingPuzzle", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button _resetButton;
        private GridLayout _mainLayout;

        private int _gameViewWidth;
        private int _tileWidth;
        private List<MyTextView> _tiles;
        private List<Point> _coords;

        private Point emptyLocation;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            _resetButton = FindViewById<Button>(Resource.Id.resetButtonId);
            _resetButton.Click += ResetButtonClick;
            _mainLayout = FindViewById<GridLayout>(Resource.Id.gameGridLayoutId);

            _gameViewWidth = Resources.DisplayMetrics.WidthPixels;
            _tiles = new List<MyTextView>();
            _coords = new List<Point>();

            SetGameView();
            MakeTiles();
            RandomizeTiles();
        }

        private void SetGameView()
        {
            _mainLayout.ColumnCount = 4;
            _mainLayout.RowCount = 4;

            _mainLayout.LayoutParameters = new LinearLayout.LayoutParams(_gameViewWidth, _gameViewWidth);
            _mainLayout.SetBackgroundColor(Color.Gray);
        }


        private void MakeTiles()
        {
         
            _tileWidth = _gameViewWidth / 4;
            int counter = 1;
            for (int vert = 0; vert < 4; vert++)
            {
                for (int horiz = 0; horiz < 4; horiz++)
                {
                    MyTextView textTile = new MyTextView(this);

                    GridLayout.Spec rowSpec = GridLayout.InvokeSpec(vert);
                    GridLayout.Spec colSpec = GridLayout.InvokeSpec(horiz);
                    GridLayout.LayoutParams tileLayoutParams = new GridLayout.LayoutParams(rowSpec, colSpec);
                    Point thisLoc = new Point(vert, horiz);

                    tileLayoutParams.Width = _tileWidth - 10;
                    tileLayoutParams.Height = _tileWidth - 10;
                    tileLayoutParams.SetMargins(5, 5, 5, 5);

                    textTile.Text =(counter++).ToString();
                    textTile.TextSize = 40;
                    textTile.SetTextColor(Color.Black);

                    textTile.Gravity=GravityFlags.Center;
                    
                    textTile.LayoutParameters = tileLayoutParams;
                    textTile.SetBackgroundColor(Color.Green);
                    textTile.Touch += TextTile_Touch;
                    
                    textTile.XPos = thisLoc.X;
                    textTile.YPos = thisLoc.Y;
                    _coords.Add(thisLoc);
                    _tiles.Add(textTile);
                    _mainLayout.AddView(textTile);
                }
            }
            _mainLayout.RemoveView(_tiles[15]);
            _tiles.RemoveAt(15);

        }

        private void TextTile_Touch(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Up)
            {
                MyTextView thisTile = sender as MyTextView;

                //determine if the square that was clicked is next to the open space
                if(Math.Sqrt(Math.Pow((thisTile.XPos-emptyLocation.X),2)+Math.Pow(thisTile.YPos-emptyLocation.Y,2)) == 1)
                {
                    //save the location of the clicked button before moving it to the location that the empty square occupies
                    Point currentPoint = new Point(thisTile.XPos,thisTile.YPos);

                    GridLayout.Spec rowSpec = GridLayout.InvokeSpec(emptyLocation.Y);
                    GridLayout.Spec colSpec = GridLayout.InvokeSpec(emptyLocation.X);

                    GridLayout.LayoutParams newLocParams = new GridLayout.LayoutParams(rowSpec,colSpec);

                    thisTile.XPos = emptyLocation.X;
                    thisTile.YPos = emptyLocation.Y;

                    newLocParams.Width = _tileWidth - 10;
                    newLocParams.Height = _tileWidth - 10;
                    newLocParams.SetMargins(5, 5, 5, 5);

                    thisTile.LayoutParameters = newLocParams;
                    //move the empty square to the location originally occupied by the tile
                    emptyLocation = currentPoint;
                }
                //System.Diagnostics.Debug.WriteLine($"\r\r\rThis tile is at ({thisTile.XPos},{thisTile.YPos})");
            }
        }

        /// <summary>
        /// Randomizes the location of each of the value tiles and the empty space
        /// </summary>
        private void RandomizeTiles()
        {
            List<Point> tempCoords = new List<Point>(_coords);

            Random rand = new Random();
           
            foreach (MyTextView view in _tiles)
            {
                int randIndex = rand.Next(0, tempCoords.Count);
                Point randLoc = tempCoords[randIndex];

                GridLayout.Spec rowSpec = GridLayout.InvokeSpec(randLoc.Y);
                GridLayout.Spec colSpec = GridLayout.InvokeSpec(randLoc.X);
                GridLayout.LayoutParams randLayoutParameters = new GridLayout.LayoutParams(rowSpec,colSpec);

                randLayoutParameters.Width = _tileWidth - 10;
                randLayoutParameters.Height = _tileWidth - 10;
                randLayoutParameters.SetMargins(5,5,5,5);

                view.XPos = randLoc.X;
                view.YPos = randLoc.Y;
                view.LayoutParameters = randLayoutParameters;

                tempCoords.RemoveAt(randIndex);
            }
            emptyLocation = tempCoords[0];
        }

        private void ResetButtonClick(object sender, EventArgs e)
        {
            RandomizeTiles();
        }
    }

    
}

