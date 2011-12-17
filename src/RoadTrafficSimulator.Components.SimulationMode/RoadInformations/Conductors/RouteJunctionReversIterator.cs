using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RoadTrafficSimulator.Components.SimulationMode.Builder;
using RoadTrafficSimulator.Components.SimulationMode.Route;

namespace RoadTrafficSimulator.Components.SimulationMode.RoadInformations.Conductors
{
    // NOTE We use here information that junction can only be connected with junction edge
    public class RouteJunctionReversIterator : IEnumerable<RouteElementWithDistance>
    {
        private readonly IRoadElement _current;
        private readonly List<IRoadElement> _roadTakenFrom = new List<IRoadElement>();
        private readonly float _maxDistance;

        public RouteJunctionReversIterator( IRoadElement current, float fromMeter )
        {
            this._current = current;
            this._maxDistance = fromMeter;
        }

        public IEnumerator<RouteElementWithDistance> GetEnumerator()
        {
            this._roadTakenFrom.Clear();
            var currentRoutes = this._current.Routes.BelongToRoutes;
            this._roadTakenFrom.Add( this._current );
            return currentRoutes.SelectMany( r => this.GetElements( r.Position.Clone(), r.Route, 0.0f ) ).GetEnumerator();
        }

        private IEnumerable<RouteElementWithDistance> GetElements( IRouteMark<RouteElement> current, BuildRoute owner, float distance )
        {
            if ( !current.MoveBack() )
            {
                if ( distance >= this._maxDistance || this._roadTakenFrom.Contains( owner.Owner ) )
                {
                    // Bug, but now I will leave it as it is
                    return new[] { new RouteElementWithDistance( owner.Owner, distance ), };
                }

                this._roadTakenFrom.Add( owner.Owner );
                var element = this.GetElementFromThatLeadsTo( owner.Owner, current.Current.RoadElement );
                if ( element == null ) { throw new InvalidOperationException(); }
                distance += element.Length;

                return new[] { new RouteElementWithDistance( element.RoadElement, distance ), }
                    .Concat( owner.Owner.Routes.BelongToRoutes.SelectMany( r => this.GetElements( r.Position.Clone(), r.Route, distance ) ) );
            }

            distance += current.Current.Length;
            if ( distance > this._maxDistance )
            {
                return new[] { new RouteElementWithDistance( current.Current.RoadElement, distance ) };
            }

            return new[] { new RouteElementWithDistance( current.Current.RoadElement, distance ) }.Concat( this.GetElements( current, owner, distance ) );
        }

        private RouteElement GetElementFromThatLeadsTo( IRoadElement owner, IRoadElement roadElement )
        {
            return owner.Routes.AvailableRoutes.Select( r => r.Elements.FirstOrDefault( p => p.RoadElement == roadElement ) ).FirstOrDefault();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}