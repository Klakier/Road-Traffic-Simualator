using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;
using RoadTrafficSimulator.Components.SimulationMode.Elements;
using RoadTrafficSimulator.Components.SimulationMode.Elements.Cars;

namespace RoadTrafficSimulator.Components.SimulationMode.Conductors
{
    public class SingleLaneConductor : IConductor
    {
        private readonly Lane _lane;
        private readonly Queue<Car> _cars = new Queue<Car>();

        public SingleLaneConductor( Lane lane )
        {
            Contract.Requires( lane != null );
            this._lane = lane;
        }

        public IRoadElement GetNextRandomElement(List<IRoadElement> route)
        {
            Debug.Assert( this._lane.Top == null );
            Debug.Assert( this._lane.Bottom == null );
            return this._lane.Next;
        }

        public void Take( Car car )
        {
            Contract.Requires( car != null );
            this._cars.Enqueue( car );
        }

        public void Remove( Car car )
        {
            var removedCar = this._cars.Dequeue();
            Debug.Assert( removedCar == car );
        }

        public bool SholdChange(Vector2 acutalCarLocation, Car car)
        {
            var distance = this._lane.RoadLaneBlock.RightEdge.Location - acutalCarLocation;
            // TODO Check value and extract some kind of property
            if ( distance.Length() <= 0.001f ) { return true; }

            return Math.Sign( distance.X ) != Math.Sign( car.Direction.X ) && Math.Sign( distance.Y ) != Math.Sign( car.Direction.Y );
        }

        public float GetDistanceToStopLine()
        {
            return float.MaxValue;
        }

        public LightInfomration GetLightInformation()
        {
            return new LightInfomration { LightDistance = float.MaxValue };
        }

        public JunctionInformation GetNextJunctionInformation()
        {
            return new JunctionInformation { JunctionDistance = float.MaxValue };
        }

        public CarInformation GetCarAheadDistance()
        {
            return new CarInformation() { CarDistance = float.MaxValue };
        }

        public Vector2 GetCarDirection( Car car )
        {
            return this._lane.RoadLaneBlock.RightEdge.Location - car.Location;
        }
    }
}