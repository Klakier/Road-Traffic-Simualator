﻿using System;
using System.Linq;
using RoadTrafficSimulator.Components.SimulationMode.Elements.Cars;
using RoadTrafficSimulator.Components.SimulationMode.RoadInformations;
using RoadTrafficSimulator.Infrastructure;

namespace RoadTrafficSimulator.Components.SimulationMode.Conductors
{
    public class Engine : IEngine
    {
        private float _stopPointDistance;
        private float _requiredSpeed;
        private NextAvailablePointToStopInfo _availablePointsToStop;

        public void SetStopPoint( float distance, float requriredSpeed )
        {
            if ( this._stopPointDistance < distance ) { return; }
            this._stopPointDistance = distance;
            this._requiredSpeed = requriredSpeed;
        }

        public void MoveCar( Car car, int elapsedMs )
        {
            var fixedStopPoint = this.GetStopPoint( this._stopPointDistance );
            this._stopPointDistance = fixedStopPoint;
            var distance = this.GetVelocity( car, elapsedMs );
            var nextPoint = car.Direction * ( distance );
            car.Location += nextPoint;

            this.Clear();
        }

        private float GetStopPoint( float stopPointDistance )
        {
            var stopPoint = this._availablePointsToStop.Items.Reverse().SkipWhile( s => s.Left < stopPointDistance ).SkipWhile( s => s.CanStop ).FirstOrDefault();
            if ( stopPoint == null )
            {
                var stop = this._availablePointsToStop.Items.FirstOrDefault( s => s.CanStop );
                return stop != null ? stop.Left : float.MaxValue;
            }

            if ( stopPointDistance >= stopPoint.Left && stopPointDistance <= stopPoint.Right )
            {
                return stopPointDistance;
            }

            if ( stopPointDistance > stopPoint.Right )
            {
                return stopPoint.Right;
            }

            if ( stopPointDistance < stopPoint.Left )
            {
                return stopPoint.Left;
            }

            throw new InvalidOperationException( "Something went wrong" );
        }

        public void SetAvailablePointsToStop( NextAvailablePointToStopInfo availablePointToStop )
        {
            this._availablePointsToStop = availablePointToStop;
        }

        private void Clear()
        {
            this._stopPointDistance = float.MaxValue;
            this._requiredSpeed = float.MaxValue;
        }

        private float GetVelocity( Car car, int elapsedMs )
        {
            if ( this._stopPointDistance < 0 )
            {
                car.Velocity = 0.0f;
                return 0.0f;
            }

            if ( this._requiredSpeed > car.Velocity )
            {
                return this.Accelerate( car, elapsedMs );
            }

            if ( this._stopPointDistance < UnitConverter.FromMeter( 0.1f ) )
            {
                car.Velocity = this._requiredSpeed;
                return this._stopPointDistance;
            }

            return this.Break( car, elapsedMs );
        }

        private float Accelerate( Car car, int elapsedMs )
        {
            var accelerated = car.AccelerateForce * elapsedMs;
            car.Velocity = car.Velocity + accelerated;
            return Math.Min( car.Velocity * elapsedMs, this._stopPointDistance );
        }

        private float Break( Car car, int elapsedMs )
        {
            var speedDifferenc = car.Velocity - this._requiredSpeed;
            var breakingDistance = Math.Pow( speedDifferenc, 2 ) / ( 2 * car.BreakingForce );
            if ( breakingDistance < this._stopPointDistance - UnitConverter.FromMeter( 1.0f ) )
            {
                var accelerated = car.AccelerateForce * elapsedMs;
                car.Velocity = car.Velocity + accelerated;
                return Math.Min( car.Velocity * elapsedMs, this._stopPointDistance );
            }

            car.Velocity -= car.BreakingForce * elapsedMs;
            return Math.Min( car.Velocity * elapsedMs, this._stopPointDistance );
        }
    }
}