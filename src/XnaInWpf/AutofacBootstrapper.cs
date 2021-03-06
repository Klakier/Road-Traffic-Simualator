using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Caliburn.Micro;
using NLog;
using System.Linq;
using RoadTrafficConstructor.Presenters;
using RoadTrafficConstructor.Presenters.BottomTabs.Settings;
using RoadTrafficSimulator.Infrastructure;
using Module = Autofac.Module;
using Common.Extensions;
using IEventAggregator = Common.IEventAggregator;
using IHandle = Common.IHandle;

namespace RoadTrafficConstructor
{
    public class AutofacBootstrapper : Bootstrapper<IShellViewModel>
    {
        private readonly static Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private IContainer _container;

        private IContainer BuildContainer()
        {
            Application.DispatcherUnhandledException += ( s, a ) => Logger.FatalException( a.Exception.ToString(), a.Exception );
            var autofacBuilder = new ContainerBuilder();
            autofacBuilder.Register( c => this._container ).As<IContainer>().SingleInstance();
            autofacBuilder.RegisterType<ServiceProviderAdapter>().As<IServiceProvider>().SingleInstance();

            autofacBuilder.RegisterModule( new CaliburnMicroModule() );
            autofacBuilder.RegisterModule( new PresentersModule() );
            autofacBuilder.RegisterModule( new XnaInWpfModule() );

            Logger.Trace( "Test log" );

            return autofacBuilder.Build();
        }

        protected override void OnExit( object sender, EventArgs e )
        {
            Console.WriteLine( "sddf" );
            base.OnExit( sender, e );
        }

        protected override void Configure()
        {
            this._container = this.BuildContainer();
        }

        protected override object GetInstance( Type service, string key )
        {
            if ( string.IsNullOrEmpty( key ) )
            {
                return this._container.Resolve( service );
            }

            return this._container.ResolveNamed( key, service );
        }

        protected override IEnumerable<object> GetAllInstances( Type service )
        {
            var enumerableType = typeof( IEnumerable<> ).MakeGenericType( service );
            return ( IEnumerable<object> ) this._container.Resolve( enumerableType );
        }
    }

    public class ServiceProviderAdapter : IServiceProvider
    {
        private readonly IContainer _container;

        public ServiceProviderAdapter( IContainer container )
        {
            _container = container;
        }

        public object GetService( Type serviceType )
        {
            return this._container.Resolve( serviceType );
        }
    }

    public class CaliburnMicroModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterType<WindowManager>().As<IWindowManager>();
        }
    }

    public class PresentersModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterAssemblyTypes( Assembly.GetExecutingAssembly() )
                .Where( c => c.Name.EndsWith( "Model" ) )
                .As( c => this.GetDefaultInterface( c ) )
                .OnActivating( this.RegisterEventHandlers );

            var settings = typeof( DefaultSettingViewModel );
            builder.RegisterAssemblyTypes( Assembly.GetExecutingAssembly() )
                .Where( c => c.Namespace == settings.Namespace )
                .Where( c => c.IsImplementingInterface<ISettingViewMode>() )
                .Where( c => c != settings )
                .AsImplementedInterfaces()
                .WithMetadata<NumberMeta>( s => s.For( m => m.Order, 0 ) )
                .OnActivating( this.RegisterEventHandlers );
            builder.RegisterType<DefaultSettingViewModel>()
                .AsImplementedInterfaces()
                .WithMetadata<NumberMeta>(
                s => s.For( m => m.Order, 1 ) );
        }

        private void RegisterEventHandlers( IActivatingEventArgs<object> activatingEventArgs )
        {
            if ( activatingEventArgs.Instance.GetType().IsImplementingInterface<IHandle>() )
            {
                var eventAggregator = activatingEventArgs.Context.Resolve<IEventAggregator>();
                eventAggregator.Subscribe( activatingEventArgs.Instance );
            }
        }

        private Type GetDefaultInterface( Type type )
        {
            var interfaceType = type.GetInterfaces()
                                    .Where( c => c.Name.EndsWith( "Model" ) )
                                    .FirstOrDefault();
            return interfaceType ?? type;
        }
    }
}