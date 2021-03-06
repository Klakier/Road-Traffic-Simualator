using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using RoadTrafficSimulator.Components.SimulationMode.Builder;
using RoadTrafficSimulator.Components.SimulationMode.Elements.Cars;
using RoadTrafficSimulator.Components.SimulationMode.RoadInformations.Conductors.Infrastructure;
using RoadTrafficSimulator.Infrastructure;
using RoadTrafficSimulator.Infrastructure.Controls;

namespace RoadTrafficSimulator.Components.SimulationMode.CarsSpecification
{
    public class PassengerCarFactory : ICarSpecifiaction
    {
        private static int _carId = 0;
        private readonly Random _rng = new Random();
        private readonly RouteToConductorConverter _routeToConductorConverter;

        public PassengerCarFactory( RouteToConductorConverter routeToConductorConverter )
        {
            this._routeToConductorConverter = routeToConductorConverter;
        }

        private const float Widht = 2.5f;
        private const float Length = 4.0f;

        public Car Create( IRoadElement startElement )
        {
            var randomRoute = this.GetRandomRoute( startElement );
            var condcutors = this._routeToConductorConverter.Convert( randomRoute ).ToArray();
            var car = new Car( condcutors )
                          {
                              Width = UnitConverter.FromMeter( Widht ),
                              Lenght = UnitConverter.FromMeter( Length ),
                              BreakingForce = UnitConverter.FromKmPerHour( 10.0f ) / UnitConverter.FromSecond( 1.0f ),
                              AccelerateForce = UnitConverter.FromKmPerHour( 10.0f ) / UnitConverter.FromSecond( 1.0f ),
                              MaxSpeed = this.ToVirtualUnitSpeed( 60.0f ),
                              Velocity = this.ToVirtualUnitSpeed( 10.0f ),
                              CarId = ++_carId,
                          };
            return car;
        }

        private IEnumerable<RouteElement> GetRandomRoute( IRoadElement startElement )
        {
            var result = new List<RouteElement>
                             {
                                 new RouteElement
                                     {
                                         CanStopOnIt = true,
                                         PriorityType = PriorityType.None,
                                         RoadElement = startElement
                                     }
                             };

            result.AddRange( startElement.Routes.GetRandomRoute( this._rng ) );

            while ( true )
            {
                var nextRoute = result.Last().RoadElement.Routes.GetRandomRoute( this._rng ).ToArray();
                if ( nextRoute.Length == 0 ) { break; }

                result.AddRange( nextRoute );
            }
            return result;
        }

        private float ToVirtualUnitSpeed( float kmPerHour )
        {
            var unitPerHour = Constans.KmToVirtualUnit( kmPerHour );
            return unitPerHour / Constans.MsPerHour;
        }
    }
}