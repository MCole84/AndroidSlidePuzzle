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

        private Point _emptyLocation;
        
        //set the colors for when a tile is in the correct or incorrect location
        private Color _correctCoordColor = Color.Blue;
        private Color _wrongCoordColor = Color.Green;

        private bool _isPuzzleSolved;

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
            for (int h = 0; h < 4; h++)
            {
                for (int v = 0; v < 4; v++)
                {
                    MyTextView textTile = new MyTextView(this);

                    GridLayout.Spec rowSpec = GridLayout.InvokeSpec(h);
                    GridLayout.Spec colSpec = GridLayout.InvokeSpec(v);
                    GridLayout.LayoutParams tileLayoutParams = new GridLayout.LayoutParams(rowSpec, colSpec);
                    Point thisLoc = new Point(v, h);

                    tileLayoutParams.Width = _tileWidth - 10;
                    tileLayoutParams.Height = _tileWidth - 10;
                    tileLayoutParams.SetMargins(5, 5, 5, 5);

                    textTile.Text = (counter++).ToString();
                    textTile.TextSize = 40;
                    textTile.SetTextColor(Color.Black);

                    textTile.Gravity = GravityFlags.Center;

                    textTile.LayoutParameters = tileLayoutParams;
                    textTile.SetBackgroundColor(_wrongCoordColor);
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
            if (!_isPuzzleSolved)
            {
                if (e.Event.Action == MotionEventActions.Up)
                {
                    MyTextView thisTile = sender as MyTextView;

                    //determine if the square that was clicked is next to the open space
                    if (Math.Sqrt(Math.Pow((thisTile.XPos - _emptyLocation.X), 2) +
                                  Math.Pow(thisTile.YPos - _emptyLocation.Y, 2)) == 1)
                    {
                        //save the location of the clicked button before moving it to the location that the empty square occupies
                        Point currentPoint = new Point(thisTile.XPos, thisTile.YPos);

                        GridLayout.Spec rowSpec = GridLayout.InvokeSpec(_emptyLocation.Y);
                        GridLayout.Spec colSpec = GridLayout.InvokeSpec(_emptyLocation.X);

                        GridLayout.LayoutParams newLocParams = new GridLayout.LayoutParams(rowSpec, colSpec);

                        thisTile.XPos = _emptyLocation.X;
                        thisTile.YPos = _emptyLocation.Y;

                        newLocParams.Width = _tileWidth - 10;
                        newLocParams.Height = _tileWidth - 10;
                        newLocParams.SetMargins(5, 5, 5, 5);

                        thisTile.LayoutParameters = newLocParams;
                        //move the empty square to the location originally occupied by the tile
                        _emptyLocation = currentPoint;
                        TileIsInCorrectPosition(thisTile);

                    }
                    System.Diagnostics.Debug.WriteLine($"\r\r\rThis tile is at ({thisTile.XPos},{thisTile.YPos})");
                }
            }
        }

        /// <summary>
        /// Randomizes the location of each of the value tiles and the empty space
        /// </summary>
        private void RandomizeTiles()
        {
            _isPuzzleSolved = false;
            List<Point> tempCoords = new List<Point>(_coords);
            
            Random rand = new Random();

            foreach (MyTextView view in _tiles)
            {
                int randIndex = rand.Next(0, tempCoords.Count);
                Point randLoc = tempCoords[randIndex];

                GridLayout.Spec rowSpec = GridLayout.InvokeSpec(randLoc.Y);
                GridLayout.Spec colSpec = GridLayout.InvokeSpec(randLoc.X);
                GridLayout.LayoutParams randLayoutParameters = new GridLayout.LayoutParams(rowSpec, colSpec);

                randLayoutParameters.Width = _tileWidth - 10;
                randLayoutParameters.Height = _tileWidth - 10;
                randLayoutParameters.SetMargins(5, 5, 5, 5);
                
                view.XPos = randLoc.X;
                view.YPos = randLoc.Y;
                TileIsInCorrectPosition(view);
                view.LayoutParameters = randLayoutParameters;

                tempCoords.RemoveAt(randIndex);
            }
            _emptyLocation = tempCoords[0];
        }

        private bool AreTilesInCorrectOrder()
        {
            return false;
        }

        private void TileIsInCorrectPosition(MyTextView view)
        {
            int viewIndex = 0;
            foreach (MyTextView v in _tiles)
            {
                if (view == v)
                {
                    break;
                }
                viewIndex++;
            }
            if (_tiles[viewIndex].XPos == _coords[viewIndex].X && _tiles[viewIndex].YPos == _coords[viewIndex].Y)
            {
                view.SetBackgroundColor(_correctCoordColor);
            }
            else
            {
                view.SetBackgroundColor(_wrongCoordColor);
            }

        }

        private void ResetButtonClick(object sender, EventArgs e)
        {
            RandomizeTiles();
        }
    }


}
