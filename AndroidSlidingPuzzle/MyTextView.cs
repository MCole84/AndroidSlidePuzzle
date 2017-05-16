using Android.App;
using Android.Widget;

namespace AndroidSlidingPuzzle
{
    class MyTextView : TextView
    {
        private Activity myContext;

        public int XPos { get; set; }
        public int YPos { get; set; }



        public MyTextView(Activity context) : base(context)
        {
            myContext = context;
        }
    }
}