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
using Firebase;
using Firebase.Auth;

namespace Planner.Droid.Services
{
    public class FirebaseService
    {
        private FirebaseApp _app;
        private FirebaseAuth _auth; 

        private FirebaseService()
        {
        }

        public void InitFirebase()
        {
            if (_app == null)
            {
                var opt = new FirebaseOptions.Builder()
                        .SetApplicationId("planner-16729")
                        .SetApiKey("AIzaSyApwc838d4MLL5iVzYzvzfe9JG8e7LiO78")
                        .Build();

                _app = FirebaseApp.InitializeApp(Application.Context, opt);

                _auth = FirebaseAuth.Instance;
            }
            else
            {
                _auth = FirebaseAuth.Instance;
            }
        }

        //public void SendVerificationEmail()
        //{
        //    if (_auth == null)
        //        throw new InvalidOperationException();

        //    _auth.Sign
        //}
    }
}