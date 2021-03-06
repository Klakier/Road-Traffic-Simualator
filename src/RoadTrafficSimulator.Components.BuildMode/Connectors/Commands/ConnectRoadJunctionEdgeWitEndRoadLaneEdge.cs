﻿using RoadTrafficSimulator.Components.BuildMode.Controls;
using RoadTrafficSimulator.Infrastructure.Controls;

namespace RoadTrafficSimulator.Components.BuildMode.Connectors.Commands
{
    public class ConnectRoadJunctionEdgeWitEndRoadLaneEdge : IConnectionCommand
    {
        public virtual bool Connect( ILogicControl first, ILogicControl second )
        {
            var roadJunctionEdge = first as JunctionEdge;
            var roadLaneEdge = second as RoadLaneBlock;

            if ( roadLaneEdge == null || roadJunctionEdge == null )
            {
                return false;
            }

            roadJunctionEdge.Connector.ConnectBeginFrom( roadLaneEdge );
            roadLaneEdge.LeftEdge.Connector.ConnectEndOn( roadJunctionEdge );
            return true;
        }
    }
}