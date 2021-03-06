using System;
using System.Diagnostics.Contracts;
using RoadTrafficSimulator.Components.BuildMode.Controls;
using RoadTrafficSimulator.Components.SimulationMode.RoadInformations;

namespace RoadTrafficSimulator.Components.SimulationMode.Elements
{
    public sealed class LaneCorner : LaneConnector
    {
        private readonly IRoadInformation _roadInformation;

        public LaneCorner( RoadConnection control, Func<LaneCorner, IRoadInformation> conductorFactory )
            : base( control )
        {
            Contract.Requires( control != null ); Contract.Requires( conductorFactory != null ); Contract.Ensures( this.Information != null ); this.LaneCornerBuild = control;
            this._roadInformation = conductorFactory( this );
        }

        public RoadConnection LaneCornerBuild { get; private set; }

        public Lane Prev { get; set; }
        public Lane Next { get; set; }

        public override IRoadInformation Information { get { return this._roadInformation; } }
    }
}