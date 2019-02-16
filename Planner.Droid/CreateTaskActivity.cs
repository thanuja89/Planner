using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Planner.Droid
{
    [Activity(Label = "CreateTaskActivity", MainLauncher = true)]
    public class CreateTaskActivity : Activity
    {
        private EditText _titleEditText;
        private EditText _descriptionEditText;
        private Spinner _importanceSpinner;
        private Spinner _repeatSpinner;
        private Button _addNoteButton;
        private Button _saveButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CreateTaskView);

            FindViews();

            SetUpSpinners();
        }

        private void SetUpSpinners()
        {
            var importanceItems = Enum
                .GetNames(typeof(Mobile.Core.Data.Importance));

            var repeatItems = Enum
                .GetNames(typeof(Mobile.Core.Data.Frequency));

            var importanceAdapter = new ArrayAdapter<string>(this,
                Android.Resource.Layout.SimpleSpinnerItem, importanceItems);

            var repeatAdapter = new ArrayAdapter<string>(this,
                Android.Resource.Layout.SimpleSpinnerItem, repeatItems);

            _importanceSpinner.Adapter = importanceAdapter;
            _repeatSpinner.Adapter = repeatAdapter;
        }

        public void FindViews()
        {
            _titleEditText = FindViewById<EditText>(Resource.Id.createTask_TitleEditText);
            _descriptionEditText = FindViewById<EditText>(Resource.Id.createTask_DescriptionEditText);
            _importanceSpinner = FindViewById<Spinner>(Resource.Id.createTask_ImportanceSpinner);
            _repeatSpinner = FindViewById<Spinner>(Resource.Id.createTask_RepeatSpinner);
            _addNoteButton = FindViewById<Button>(Resource.Id.createTask_AddNoteButton);
            _saveButton = FindViewById<Button>(Resource.Id.createTask_SaveButton);
        }

        private void HandleEvents()
        {
            _saveButton.Click += _saveButton_Click;
        }

        private void _saveButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}