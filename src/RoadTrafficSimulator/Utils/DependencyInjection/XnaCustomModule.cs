﻿using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RoadTrafficSimulator.Factories;
using RoadTrafficSimulator.Infrastructure;
using RoadTrafficSimulator.Infrastructure.Mouse;
using RoadTrafficSimulator.Integration;
using RoadTrafficSimulator.Road;
using RoadTrafficSimulator.Road.Connectors.Commands;
using RoadTrafficSimulator.Road.Controls;
using RoadTrafficSimulator.Utile.DependencyInjection;
using Xna;
using XnaRoadTrafficConstructor.Infrastucure;
using XnaRoadTrafficConstructor.MouseHandler;
using XnaRoadTrafficConstructor.MouseHandler.JunctionMouseHandler;
using XnaRoadTrafficConstructor.Road;
using XnaRoadTrafficConstructor.Utils.DependencyInjection;
using XnaVs10.Sprites;
using XnaVs10.Utils;
using Module = Autofac.Module;

namespace RoadTrafficSimulator.Utils.DependencyInjection
{
    public class XnaCustomModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterType<MessageBroker>().SingleInstance();

            builder.RegisterType<Camera3D>().SingleInstance();
            builder.RegisterType<SpriteBatch>().As<SpriteBatch>();
            builder.RegisterType<PrimitiveBatch>().As<PrimitiveBatch>();
            builder.RegisterType<Stored>().SingleInstance();
            builder.RegisterType<RoadLayer>().InstancePerLifetimeScope();
            builder.Register( s => new VisitAllChildren( s.Resolve<RoadLayer>() ) );


            builder.RegisterType<ContentManagerAdapter>().As<IContentManager>();
            builder.RegisterType<KeyboardInputNotify>().As<KeyboardInputNotify>().SingleInstance();
            builder.RegisterType<MouseInputNotify>().As<MouseInputNotify>().SingleInstance();
            builder.RegisterType<RoadComponent>()
                .OnActivated( s =>
                                  {
                                      s.Context.Resolve<BuilderControl>().RoadComponent = s.Instance;
                                      s.Context.Resolve<WorldController>().RoadComponent = s.Instance;
                                  } )
                .InstancePerLifetimeScope();
            builder.RegisterType<BuilderControl>().InstancePerLifetimeScope();
            builder.RegisterType<WorldController>().InstancePerLifetimeScope();
            builder.RegisterType<Layer2D>().SingleInstance();

            // NOTE Mouse support
            builder.RegisterType<MouseInformation>().Named<IMouseInformation>( "MainMouseInformation" )
                                                     .InstancePerLifetimeScope()
                                                     .OnActivated( t => t.Instance.StartRecord() );
            builder.RegisterType<FilterMouseInformation>().As<IMouseInformation>()
                                                          .InstancePerDependency();
            builder.Register( s => new PriorityMouseInfomrmation( s.ResolveNamed<IMouseInformation>( "MainMouseInformation" ) ) )
                   .InstancePerLifetimeScope();

            builder.RegisterType<RoadLaneCreator>();
            builder.RegisterType<RoadLaneCreatorController>();
            builder.RegisterType<RoadJunctionCreator>();

            builder.RegisterType<JunctionCornerMouseHandler>();
            builder.RegisterType<JunctionEdgeMouseHandler>();

            builder.RegisterType<JuntionMouseHandlerComposite>()
                .As<IMouseHandler>()
                .WithMetadata<IOrderMeta>( order => order.For( s => s.Order, 10 ) );

            builder.RegisterType<MouseDownCompositeHandler>();

            builder.RegisterModule( new InfrastructureModule() );

            builder.RegisterType<ConnectObjectCommand>();
            builder.RegisterType<CompositeConnectionCommand>();
            builder.RegisterType<ConnectRoadJunctionEdge>().As<IConnectionCommand>();
            builder.RegisterType<ConnectEndRoadLaneEdgeWithRoadLaneConnection>().As<IConnectionCommand>();
            builder.RegisterType<ConnectEndRoadLaneEdgeWithRoadJunctionEdge>().As<IConnectionCommand>();
            builder.RegisterType<ConnectRoadConnectionWithEndRoadLane>().As<IConnectionCommand>();
            builder.RegisterType<ConnectRoadJunctionEdgeWitEndRoadLaneEdge>().As<IConnectionCommand>();
            builder.RegisterType<ConnectSideRoadLaneEdges>().As<IConnectionCommand>();
            builder.RegisterType<ConnectRoadConnectionWithRoadConnection>().As<IConnectionCommand>();
            builder.RegisterType<ScreenZoom>().As<IBackgroundJob>();

            this.RegisterFactoryMethods( builder );

            this.RegisterFactories( builder );

            builder.RegisterType<SelectControlCommand>().SingleInstance();
        }

        private void RegisterFactoryMethods( ContainerBuilder builder )
        {
            builder.Register( s => new Func<Vector2, ICompositeControl, IRoadJunctionBlock>(
                                       ( location, owner ) => new RoadJunctionBlock( s.Resolve<Factories.Factories>(), location, owner ) ) );

            builder.Register( s => new Func<ICompositeControl, IRoadLaneBlock>(
                                      cc => new RoadLaneBlock( s.Resolve<Factories.Factories>(), cc ) ) );
            builder.Register( s => new Func<Vector2, ICompositeControl, RoadConnection>(
                                      ( locatio, owner ) => new RoadConnection( s.Resolve<Factories.Factories>(), locatio, owner ) ) );
        }

        private void RegisterFactories( ContainerBuilder builder )
        {
            builder.RegisterType<Factories.Factories>();
            builder.RegisterAssemblyTypes( Assembly.GetAssembly( typeof( Factories.Factories ) ) )
                .Where( s => s.Namespace == typeof( Factories.Factories ).Namespace && s.Name.EndsWith( "Factory" ) )
                .AsImplementedInterfaces();
        }
    }
}