﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Microsoft.Xna.Framework;
using RoadTrafficSimulator.Infrastructure.Control;
using RoadTrafficSimulator.Infrastructure.Mouse;
using RoadTrafficSimulator.MouseHandler.Infrastructure;
using RoadTrafficSimulator.Road.Connectors;
using RoadTrafficSimulator.Road.Controls;

namespace RoadTrafficSimulator.Road
{
    public class RoadLaneCreator
    {
        private readonly CompositeConnectionCommand _connectionCommand;
        private IControl _lastConnectedControl;
        private ICompositeControl _owner;

        public RoadLaneCreator( CompositeConnectionCommand connectionCommand )
        {
            this._connectionCommand = connectionCommand;
        }

        public void StartFrom( IControl control )
        {
            if ( this._lastConnectedControl != null )
            {
                throw new InvalidOperationException();
            }

            this._lastConnectedControl = control.NotNull();
        }

        public void CreateBlockTo( Vector2 location )
        {
            var roadLane = this.CreateRoadLane();
            this._connectionCommand.Connect( this._lastConnectedControl, roadLane.LeftEdge );

            var roadLaneConnection = this.CreateRoadLaneConnection( location );
            this._connectionCommand.Connect( roadLane.RightEdge, roadLaneConnection );

            this._owner.AddChild( roadLane );

            this._lastConnectedControl = roadLaneConnection;
        }

        public void EndIn( IControl lastControl )
        {
            var roadLane = this.CreateRoadLane();
            this._connectionCommand.Connect( this._lastConnectedControl, roadLane.LeftEdge );
            this._connectionCommand.Connect( roadLane.RightEdge, lastControl );

            this._owner.AddChild( roadLane );
        }

        public void SetOwner( ICompositeControl owner )
        {
            this._owner = owner.NotNull();
        }

        private RoadLaneBlock CreateRoadLane()
        {
            return new RoadLaneBlock( this._owner );
        }

        private RoadConnectionEdge CreateRoadLaneConnection( Vector2 location )
        {
            var roadLaneConnection = new RoadConnectionEdge( location, this._owner );
            this._owner.AddChild( roadLaneConnection );
            return roadLaneConnection;
        }
    }

    public class RoadLaneCreatorController
    {
        private readonly IMouseInformation _mouseInformation;
        private readonly VisitAllChildren _visitator;
        private readonly RoadLaneCreator _roadLaneCreator;
        private bool _isFirst;

        public RoadLaneCreatorController(
            IMouseInformation mouseInformation,
            VisitAllChildren visitator,
            RoadLaneCreator roadLaneCreator )
        {
            this._mouseInformation = mouseInformation;
            this._visitator = visitator;
            this._roadLaneCreator = roadLaneCreator;
            this._mouseInformation.LeftButtonPressed.Subscribe( this.MousePressed );
        }

        // This is stupid, should be changed
        public void SetOwner( ICompositeControl owner )
        {
            this._roadLaneCreator.SetOwner( owner );
        }

        public void Begin( IControl owner )
        {
            this._isFirst = true;
            this._mouseInformation.StartRecord();
        }

        public void End()
        {
            this._mouseInformation.StopRecord();
        }

        private void MousePressed( XnaMouseState mouseState )
        {
            var edge = this.GetRoadJuctionEdge( mouseState.Location );

            if ( this._isFirst )
            {
                this.ProcessFirstControl( edge );
            }
            else
            {
                this.ProcessControl( edge, mouseState.Location );
            }
        }

        private void ProcessControl( IControl edge, Vector2 location )
        {
            if ( edge == null )
            {
                this._roadLaneCreator.CreateBlockTo( location );
            }
            else
            {
                this._roadLaneCreator.EndIn( edge );
                this.End();
            }
        }

        private void ProcessFirstControl( IControl control )
        {
            if ( control == null )
            {
                return;
            }

            this._isFirst = false;
            this._roadLaneCreator.StartFrom( control );
        }

        private RoadJunctionEdge GetRoadJuctionEdge( Vector2 location )
        {
            return this._visitator.Where( c => c.HitTest( location ) ).OfType<RoadJunctionEdge>().FirstOrDefault();
        }
    }
}