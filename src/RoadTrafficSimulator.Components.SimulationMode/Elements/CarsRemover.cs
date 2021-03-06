using System;
using System.Diagnostics.Contracts;
using RoadTrafficSimulator.Components.SimulationMode.RoadInformations;

namespace RoadTrafficSimulator.Components.SimulationMode.Elements
{
    public sealed class CarsRemover : RoadElementBase
    {
        private readonly IRoadInformation _roadInformation;

        public CarsRemover( BuildMode.Controls.CarsRemover control, Func<CarsRemover, IRoadInformation> conductorFactory )
            : base( control )
        {
            Contract.Requires( control != null ); Contract.Requires( conductorFactory != null ); Contract.Ensures( this.Information != null );
            this._roadInformation = conductorFactory( this );
            this.CarsRemoverBuilder = control;
        }

        public Lane Lane { get; set; }

        public override IRoadInformation Information { get { return this._roadInformation; } }

        public BuildMode.Controls.CarsRemover CarsRemoverBuilder { get; set; }
    }
}