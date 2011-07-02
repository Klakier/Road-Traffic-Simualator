﻿using System;
using System.Collections.Generic;
using RoadTrafficSimulator.Components.BuildMode.Controls;
using RoadTrafficSimulator.Components.SimulationMode.Elements;
using RoadTrafficSimulator.Components.SimulationMode.Elements.Light;
using RoadTrafficSimulator.Infrastructure;
using RoadTrafficSimulator.Infrastructure.Controls;

namespace RoadTrafficSimulator.Components.SimulationMode.Builder
{
    public class LightBuilder : IBuilerItem
    {
        private Light _light;

        public IEnumerable<BuilderAction> Create( IControl control )
        {
            yield return new BuilderAction( Order.High, context => this.Build( context, control ) );
            yield return new BuilderAction( Order.Normal, this.Connect );
        }

        public bool CanCreate( IControl control )
        {
            return control != null && control.GetType() == typeof( LightBlock );
        }

        private void Build( BuilderContext context, IControl control )
        {
            var lightBlock = ( LightBlock ) control;
            this._light = new Light( lightBlock, l => context.ConductorFactory.Create( l ) );
            context.AddElement( lightBlock, this._light );
        }

        private void Connect( BuilderContext builderContext )
        {
            var owner = builderContext.GetObject<LaneJunction>( this._light.LightBlock.Connector.Owner.Parent );
            this._light.Owner = owner;
            var edge = owner.JunctionBuilder.GetEdgeType( this._light.LightBlock.Connector.Owner );
            if ( edge < 0 ) { throw new ArgumentException(); }
            owner.AddLight( edge, this._light );
        }
    }
}