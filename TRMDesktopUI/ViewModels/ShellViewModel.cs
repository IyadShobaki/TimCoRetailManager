using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<Object>, IHandle<LogOnEvent>
    {
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private ILoggedInUserModel _user;
        public ShellViewModel(IEventAggregator events,
            SalesViewModel salesVM,
            ILoggedInUserModel user) //constructor injection
        {
            _events = events;
            _salesVM = salesVM;
            _user = user;
            _events.Subscribe(this);

            ActivateItem(IoC.Get<LoginViewModel>());

        }

        public bool IsLoggIn
        {
            get
            {
                bool output = false;

                if (string.IsNullOrWhiteSpace(_user.Token) == false)
                {
                    output = true;
                }
                return output;
            }
        }
        public void ExitApplication()
        {
            TryClose();
        }

        public void LogOut()
        {
            _user.LogOffUser();

            ActivateItem(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggIn);
        }
        public void Handle(LogOnEvent message)
        {
            //because of the conductor, one form will be activate
            //so login form will be close 
            ActivateItem(_salesVM);
            NotifyOfPropertyChange(() => IsLoggIn);
        }
    }
}
