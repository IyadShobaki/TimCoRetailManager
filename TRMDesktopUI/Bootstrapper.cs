using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TRMDesktopUI.ViewModels;

namespace TRMDesktopUI
{
    public class Bootstrapper : BootstrapperBase
    {
        //dependency injection 
        //container to handle our instantiation of our classes or most of them
        //instead of write ( = new SomeClass();) the container will do it for us
        private SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        //the most important one. the container holds an instance of itself to pass
        //when people ask for SimpleContainer
        protected override void Configure()
        {
            _container.Instance(_container);

            //WindowManager handling the idea of bringing windows in and out 
            //EventAggregator, this where we can pass event messaging throughout our application,
            //one peice can raise the event and other one can listen for it and do something

            //both of the previous are singletons. singleton means that we create one instance of the class
            //for the life of the application (the scope of this container)
            //Don't use singletons unless you can't find a better way, because they are not great
            //on memory usage and not great on the overall using Object Oriented the way it's supposed to be used.
            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>();

            //using reflection GetType()
  
            GetType().Assembly.GetTypes()  //GetTypes() means get every type in our entire application
                .Where(type => type.IsClass) //where the type is a class
                .Where(type => type.Name.EndsWith("ViewModel"))//where the name of the class end with ViewModel like(ShellViewModel)
                .ToList() //create a list of them
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        //when we pass atype and a name , we will get instance 
        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        //to get all instances
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        //to create constructors 
        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
