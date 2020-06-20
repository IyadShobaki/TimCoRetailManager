using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<Object>, IHandle<LogOnEvent>
    {
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        public ShellViewModel(LoginViewModel loginVM, IEventAggregator events,
            SalesViewModel salesVM) //constructor injection
        {
            _events = events;
            _salesVM = salesVM;
    
            _events.Subscribe(this);

            ActivateItem(IoC.Get<LoginViewModel>());

        }

        public void Handle(LogOnEvent message)
        {
            //because of the conductor, one form will be activate
            //so login form will be close 
            ActivateItem(_salesVM);
        }
    }
}
