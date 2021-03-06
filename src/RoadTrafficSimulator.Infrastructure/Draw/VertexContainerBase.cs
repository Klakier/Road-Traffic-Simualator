﻿using System;
using System.Diagnostics;
using Common;
using Microsoft.Xna.Framework;
using RoadTrafficSimulator.Infrastructure.Controls;

namespace RoadTrafficSimulator.Infrastructure.Draw
{
    public abstract class VertexContainerBase<T, TVertex> : IVertexContainer<TVertex> where T : class, IControl
    {
        private readonly Color _orignalColor;

        protected VertexContainerBase( T @object, Color orignalColor )
        {
            this._orignalColor = orignalColor;
            this._color = _orignalColor;
            this.Object = @object.NotNull();
            this.Object.Redrawed.Subscribe( s => this.Vertex = this.UpdateShapeAndCreateVertex() );
        }

        public T Object { get; private set; }

        public TVertex[] Vertex { get; private set; }

        public abstract IShape Shape { get; }

        public virtual void ReloadTextures()
        {
        }

        private Color _color;

        public virtual Color Color
        {
            get { return _color; }
            set
            {
                this._color = value;
                this.Vertex = this.UpdateShapeAndCreateVertex();
            }
        }

        public virtual void ClearColor()
        {
            this.Color = this._orignalColor;
        }

        public void Draw( Graphic graphic )
        {
            if ( this.Vertex == null )
            {
                this.Vertex = this.UpdateShapeAndCreateVertex();
                Debug.Assert( this.Vertex != null, "this.Vertex != null" );
            }

            this.DrawControl( graphic );
        }

        protected abstract TVertex[] UpdateShapeAndCreateVertex();

        protected abstract void DrawControl( Graphic graphic );

        protected void DrawControl( IControl controlBase, Graphic graphic )
        {
            if ( controlBase != null )
            {
                controlBase.VertexContainer.Draw( graphic );
            }
        }
    }
}