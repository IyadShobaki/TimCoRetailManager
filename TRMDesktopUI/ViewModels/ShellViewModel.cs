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
        private SimpleContainer _container;
        public ShellViewModel(LoginViewModel loginVM, IEventAggregator events,
            SalesViewModel salesVM, SimpleContainer container) //constructor injection
        {
            _events = events;
            _salesVM = salesVM;
            _container = container;

            _events.Subscribe(this);

            ActivateItem(_container.GetInstance<LoginViewModel>());

        }

        public void Handle(LogOnEvent message)
        {
            //because of the conductor, one form will be activate
            //so login form will be close 
            ActivateItem(_salesVM);
        }
    }
}
