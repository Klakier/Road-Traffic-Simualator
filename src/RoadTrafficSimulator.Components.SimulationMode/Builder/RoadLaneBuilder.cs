using System.Collections.Generic;
using System.Linq;
using RoadTrafficSimulator.Components.BuildMode.Controls;
using RoadTrafficSimulator.Components.SimulationMode.Elements;
using RoadTrafficSimulator.Components.SimulationMode.RoadInformations;
using RoadTrafficSimulator.Infrastructure;
using RoadTrafficSimulator.Infrastructure.Controls;

namespace RoadTrafficSimulator.Components.SimulationMode.Builder
{
    public class RoadLaneBuilder : IBuilerItem
    {
        public IEnumerable<BuilderAction> Create( IControl control )
        {
            var builder = new Builder();
            yield return new BuilderAction( Order.High, context => builder.Build( context, control ) );
            yield return new BuilderAction( Order.Normal, builder.Connect );
            yield return new BuilderAction( Order.Low, builder.SetUp );
            yield return new BuilderAction( Order.VeryLow, builder.SetConnection );
        }

        public bool CanCreate( IControl control )
        {
            return control != null && control.GetType() == typeof( RoadLaneBlock );
        }

        private class Builder : BuilderBase
        {
            private Lane _lane;

            public void Build( BuilderContext context, IControl control )
            {
                var laneBlock = ( RoadLaneBlock ) control;
                this._lane = new Lane( laneBlock, l => new LaneRoadInforamtion( l ) );
                context.AddElement( laneBlock, this._lane );
            }

            public void Connect( BuilderContext builderContext )
            {
                this._lane.Prev = builderContext.GetObject<IRoadElement>( this._lane.RoadLaneBlock.LeftEdge.Connector.PreviousEdge.Parent );
                this._lane.Next = builderContext.GetObject<IRoadElement>( this._lane.RoadLaneBlock.RightEdge.Connector.NextEdge.Parent );
            }

            public void SetUp(BuilderContext obj)
            {
                var routes = this._lane.RoadLaneBlock.Routes;
                var convertedRoutes = this.ConvertRoutes( routes, obj, this._lane ).ToArray();
                this._lane.Routes = new StandardRoutes( convertedRoutes );
            }

            public void SetConnection( BuilderContext context )
            {
                this.SetConnections( this._lane.Routes.AvailableRoutes );
            }
        }
    }
}