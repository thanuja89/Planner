using Android.Content;
using Android.Views;
using Android.Widget;

namespace Planner.Droid.Helpers
{
    public class ProgressBarHelper
    {
        private readonly Window _window;
        private readonly Context _context;
        private readonly RelativeLayout _relativeLayout;
        private ProgressBar _progressBar;

        public ProgressBarHelper(Context context, Window window, RelativeLayout relativeLayout)
        {
            _context = context;
            _window = window;
            _relativeLayout = relativeLayout;
        }

        public void Show()
        {
            if (_progressBar == null)
            {
                _progressBar = new ProgressBar(_context)
                {
                    Indeterminate = true
                };

                var param = new RelativeLayout.LayoutParams(100, 100);
                param.AddRule(LayoutRules.CenterInParent);
                _relativeLayout.AddView(_progressBar, param);
            }

            _progressBar.Visibility = ViewStates.Visible;
            _window.SetFlags(WindowManagerFlags.NotTouchable, WindowManagerFlags.NotTouchable);
        }

        public void Hide()
        {
            if (_progressBar != null && _progressBar.IsShown)
            {
                _progressBar.Visibility = ViewStates.Gone;
            }

            _window.ClearFlags(WindowManagerFlags.NotTouchable);
        }
    }
}