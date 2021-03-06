using System.Diagnostics.Contracts;
using Common;
using Microsoft.Xna.Framework;
using RoadTrafficSimulator.Components.SimulationMode.Elements;
using RoadTrafficSimulator.Components.SimulationMode.Elements.Cars;
using RoadTrafficSimulator.Components.SimulationMode.Messages;
using RoadTrafficSimulator.Infrastructure;

namespace RoadTrafficSimulator.Components.SimulationMode.RoadInformations
{
    public class CarRemoverRoadInformation : RoadInformationBase, IRoadInformation
    {
        private readonly CarsRemover _carsRemover;
        private readonly IEventAggregator _eventAggregator;

        public CarRemoverRoadInformation( CarsRemover carsRemover, IEventAggregator eventAggregator )
        {
            Contract.Requires( carsRemover != null );
            Contract.Requires( eventAggregator != null );
            this._carsRemover = carsRemover;
            this._eventAggregator = eventAggregator;
        }

        public override void OnEnter( Car car )
        {
            this._eventAggregator.Publish( new CarRemoved( car ) );
        }

        protected override Vector2 GetBeginLocation()
        {
            return this._carsRemover.BuildControl.Location;
        }

        protected override Vector2 GetEndLocation()
        {
            return this._carsRemover.BuildControl.Location;
        }

        public float Lenght( IRoadElement previous, IRoadElement next )
        {
            return Constans.PointSize;
        }

        public bool CanStop( IRoadElement previous, IRoadElement next )
        {
            return true;
        }

        public bool ShouldChange( Car car )
        {
            return false;
        }
    }
}