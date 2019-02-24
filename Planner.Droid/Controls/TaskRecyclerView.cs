using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Planner.Mobile.Core.Data;
using System;
using System.Collections.Generic;

namespace Planner.Droid.Controls
{
    public class TaskViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; private set; }
        public TextView Description { get; private set; }
        public TextView Start { get; private set; }
        public TextView Repeat { get; private set; }

        // Get references to the views defined in the CardView layout.
        public TaskViewHolder(View itemView, Action<int> listener, Action<int> deleteListener)
            : base(itemView)
        {
            // Locate and cache view references:
            Title = itemView.FindViewById<TextView>(Resource.Id.taskRecyclerView_TitleTextView);
            Description = itemView.FindViewById<TextView>(Resource.Id.taskRecyclerView_DescriptionTextView);
            Start = itemView.FindViewById<TextView>(Resource.Id.taskRecyclerView_StartTextView);
            Repeat = itemView.FindViewById<TextView>(Resource.Id.taskRecyclerView_RepeatTextView);

            var deleteButton = itemView.FindViewById<ImageButton>(Resource.Id.taskRecyclerView_DeleteButton);

            if (!deleteButton.HasOnClickListeners)
                deleteButton.Click += (sender, e) => deleteListener(base.LayoutPosition);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }

    public class TaskViewAdapter : RecyclerView.Adapter
    {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;
        public event EventHandler<int> ItemDeleteClick;

        // Underlying data set (a photo album):
        private List<ScheduledTask> _tasks;

        // Load the adapter with the data set (photo album) at construction time:
        public TaskViewAdapter(List<ScheduledTask> tasks)
        {
            _tasks = tasks;
        }

        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.item_task_recycler_view, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            TaskViewHolder vh = new TaskViewHolder(itemView, OnClick, OnDeleteClick);
            return vh;
        }

        // Fill in the contents of the photo card (invoked by the layout manager):
        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            TaskViewHolder vh = holder as TaskViewHolder;

            // Set the ImageView and TextView in this ViewHolder's CardView 
            // from this position in the photo album: 
            vh.Title.Text = _tasks[position].Title;
            vh.Start.Text = _tasks[position].Start.ToLongDateString();
            vh.Repeat.Text = _tasks[position].Repeat.ToString();
        }

        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get { return _tasks.Count; }
        }

        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }

        void OnDeleteClick(int position)
        {
            ItemDeleteClick?.Invoke(this, position);

            _tasks.Remove(_tasks[position]);
            NotifyItemRemoved(position);
        }
    }
}