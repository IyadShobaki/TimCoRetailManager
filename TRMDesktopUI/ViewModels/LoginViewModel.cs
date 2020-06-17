using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Helpers;

namespace TRMDesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _userName;
        private string _password;
        private IAPIHelper _apiHelper;

        public LoginViewModel(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }
        public string UserName
        {
            get { return _userName; }
            set { 
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
               NotifyOfPropertyChange(() => CanLogIn);
            }
        }
        public string Password
        {
            get { return _password; }
            set { 
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        //to show the error message if the user didn't enter the correct username or pass
        public bool IsErrorVisible
        {
            get 
            {
                bool output = false;

                if (ErrorMessage?.Length > 0)
                {
                    output = true;
                }
                return output; 
            }
          

        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set 
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => IsErrorVisible);
                NotifyOfPropertyChange(() => ErrorMessage);
               
            }
         
        }


        public bool CanLogIn
        {
            get
            {
                bool output = false;

                if (UserName?.Length > 0 && Password?.Length > 0) //question mark is a null check
                {
                    output = true;
                }

                return output;
            }
        }

        public async Task LogIn()  //async to not lock your application up
        {
            try
            {
                ErrorMessage = "";
                //the result will be AthunticatedUser model
                var result = await _apiHelper.Authenticate(UserName, Password);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }


    }
}
