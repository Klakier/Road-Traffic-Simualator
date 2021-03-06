﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RoadTrafficSimulator.Components.BuildMode.VertexContainers;
using RoadTrafficSimulator.Infrastructure.Controls;
using RoadTrafficSimulator.Infrastructure.Draw;
using RoadTrafficSimulator.Infrastructure.Mouse;

namespace RoadTrafficSimulator.Components.BuildMode.Controls
{
    public abstract class Edge : CompositControl<VertexPositionColor>, IEdge, IComponent
    {
        private readonly IRouteElement _parent;
        private readonly EdgeVertexContainer _concretVertexContainer;
        private readonly IMouseHandler _mouseHandler;
        private MovablePoint _startPoint;
        private MovablePoint _endPoint;

        protected Edge( Factories.Factories factories, Style style, IRouteElement parent )
        {
            this._parent = parent;
            this.Factories = factories;
            this._concretVertexContainer = new EdgeVertexContainer( this, style.NormalColor );
            this._mouseHandler = factories.MouseHandlerFactory.Create( this );
            this._startPoint = new MovablePoint( factories, Vector2.Zero, this );
            this._endPoint = new MovablePoint( factories, Vector2.Zero, this );
            this.AddChild( this._startPoint );
            this.AddChild( this._endPoint );
        }

        public Factories.Factories Factories { get; private set; }

        protected Edge( Factories.Factories factories, MovablePoint startPoint, MovablePoint endPoint, Style style, IRouteElement parent )
            : this( factories, style, parent )
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }

        public Vector2 StartLocation
        {
            get { return this._startPoint.Location; }
        }

        public Vector2 EndLocation
        {
            get { return this._endPoint.Location; }
        }

        public MovablePoint StartPoint
        {
            get { return this._startPoint; }
            set
            {
                this.RemoveChild( this._startPoint );
                this._startPoint = value;
                this.AddChild( value );
                this.Invalidate();
            }
        }

        public MovablePoint EndPoint
        {
            get { return this._endPoint; }
            set
            {
                this.RemoveChild( this._endPoint );
                this._endPoint = value;
                this.AddChild( value );
                this.Invalidate();
            }
        }

        public override IVertexContainer VertexContainer
        {
            get { return this._concretVertexContainer; }
        }

        public override IMouseHandler MouseHandler
        {
            get { return this._mouseHandler; }
        }

        public override Vector2 Location
        {
            get { return this.StartLocation + ( ( this.EndLocation - this.StartLocation ) / 2 ); }
            set
            {
                var diff = value - this.Location;
                this.StartPoint.SetLocation( this.StartPoint.Location + diff );
                this.EndPoint.SetLocation( this.EndPoint.Location + diff );
            }
        }

        public override void TranslateWithoutNotification( Matrix translationMatrix )
        {
            this.StartPoint.TranslateWithoutEvent( translationMatrix );
            this.EndPoint.TranslateWithoutEvent( translationMatrix );
        }

        public override void Translate( Matrix matrixTranslation )
        {
            var startPoint = this.StartPoint.Location;
            var endPoint = this.EndPoint.Location;
            this.StartPoint.TranslateWithoutEvent( matrixTranslation );
            this.EndPoint.TranslateWithoutEvent( matrixTranslation );

            if ( startPoint != this.StartPoint.Location )
            {
                this.StartPoint.Invalidate();
            }

            if ( endPoint != this.EndPoint.Location )
            {
                this.EndPoint.Invalidate();
            }

            if ( startPoint != this.StartPoint.Location || endPoint != this.EndPoint.Location )
            {
                this.Invalidate();
            }
        }

        public IRouteElement Parent
        {
            get { return this._parent; }
        }
    }
}