using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Planner.Droid.Fragments
{
    public class InputDialogFragment : DialogFragment
    {
        public const string TAG = "X:InputDialogFragment";
        private Action<string> _onOkButtonClicked;
        private string _labelText;

        private TextView labelTextView;
        private EditText inputEditText;
        private Button okButton;

        public static InputDialogFragment NewInstance(string labelText, Action<string> onOkButtonClicked)
        {
            InputDialogFragment frag = new InputDialogFragment
            {
                _onOkButtonClicked = onOkButtonClicked,
                _labelText = labelText
            };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_input_dialog, container);

            FindViews(view);

            labelTextView.Text = _labelText;

            HandleEvents();

            return view;
        }

        private void FindViews(View view)
        {

            labelTextView = view.FindViewById<TextView>(Resource.Id.inputDialog_LabelTextView);
            inputEditText = view.FindViewById<EditText>(Resource.Id.inputDialog_InputEditText);

            okButton = view.FindViewById<Button>(Resource.Id.inputDialog_OkButton);
        }

        private void HandleEvents()
        {
            okButton.Click += OkButton_Click;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            var text = inputEditText.Text;

            _onOkButtonClicked?.Invoke(text);
        }
    }
}