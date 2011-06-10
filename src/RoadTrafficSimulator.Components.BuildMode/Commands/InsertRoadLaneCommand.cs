using System;
using System.Linq;
using Microsoft.Xna.Framework;
using RoadTrafficSimulator.Components.BuildMode.Controls;
using RoadTrafficSimulator.Infrastructure;
using RoadTrafficSimulator.Infrastructure.Controls;
using RoadTrafficSimulator.Infrastructure.Mouse;
using RoadTrafficSimulator.Road;

namespace RoadTrafficSimulator.Components.BuildMode.Commands
{
    public class InsertRoadLaneCommand : ICommand
    {
        private readonly IMouseInformation _mouseInformation;
        private readonly VisitAllChildren _visitator;
        private readonly RoadLaneBuilder _roadLaneBuilder;
        private bool _isFirst;
        private readonly IControl _owner;

        public InsertRoadLaneCommand( IMouseInformation mouseInformation, RoadLayer ownr, RoadLaneBuilder roadLaneBuilder )
        {
            this._mouseInformation = mouseInformation;
            this._mouseInformation.LeftButtonPressed.Subscribe( this.MousePressed );
            this._owner = ownr;
            this._roadLaneBuilder = roadLaneBuilder;
            this._roadLaneBuilder.SetOwner( ownr );

            this._visitator = new VisitAllChildren( this._owner );
        }

        // This is stupid, should be changed
        public void SetOwner( ICompositeControl owner )
        {
            this._roadLaneBuilder.SetOwner( owner );
        }

        public Type CreatedType
        {
            get { return typeof( RoadLaneBlock ); }
        }

        public CommandType CommandType
        {
            get { return CommandType.InserRoadLane; }
        }

        public void Start()
        {
            this._isFirst = true;
            this._mouseInformation.StartRecord();
        }

        public void Stop()
        {
            this._mouseInformation.StopRecord();

            // TODO Finish road lane at some control
        }

        private void MousePressed( XnaMouseState mouseState )
        {
            var edge = this.GetRoadJuctionEdge( mouseState.Location );
            if ( this.IsAppropiate( edge ) == false ) { return; }

            if ( this._isFirst )
            {
                this.ProcessFirstControl( edge );
            }
            else
            {
                this.ProcessControl( edge, mouseState.Location );
            }
        }

        private bool IsAppropiate( IControl edge )
        {
            // TODO: Fix it, this should be resolved in more appropiate way
            return edge == null || edge is RoadJunctionEdge || edge is CarsInserter || edge is CarsRemover;
        }

        private void ProcessControl( IControl edge, Vector2 location )
        {
            if ( edge == null )
            {
                this._roadLaneBuilder.CreateBlockTo( location );
            }
            else
            {
                this._roadLaneBuilder.EndIn( edge );
                this.StartFromBegining();
            }
        }

        private void StartFromBegining()
        {
            this._isFirst = true;
        }

        private void ProcessFirstControl( IControl control )
        {
            if ( control == null ) { return; }

            this._isFirst = false;
            this._roadLaneBuilder.StartFrom( control );
        }

        private IControl GetRoadJuctionEdge( Vector2 location )
        {
            return this._visitator.Where( c => c.IsHitted( location ) ).FirstOrDefault();
        }
    }
}