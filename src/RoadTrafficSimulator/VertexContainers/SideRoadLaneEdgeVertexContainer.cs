using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RoadTrafficSimulator.Road.RoadJoiners;
using XnaRoadTrafficConstructor.Infrastucure.Draw;
using XnaRoadTrafficConstructor.Road;
using XnaRoadTrafficConstructor.Road.RoadJoiners;
using XnaVs10.Extension;
using XnaVs10.MathHelpers;

namespace RoadTrafficSimulator.VertexContainers
{
    public class SideRoadLaneEdgeVertexContainer : VertexContainerBase<SideRoadLaneEdge, VertexPositionColor>
    {
        private readonly Color _normalColor = new Color( 90, 90, 90 );
        private readonly Color _selectedColor = Color.Blue;
        private Quadrangle _quadrangle;

        public SideRoadLaneEdgeVertexContainer( SideRoadLaneEdge edge )
            : base( edge )
        {
        }

        public override IShape Shape
        {
            get { return this._quadrangle; }
        }

        protected override void DrawControl( Graphic graphic )
        {
            graphic.VertexPositionalColorDrawer.DrawTriangeList( this.Vertex );
        }

        protected override VertexPositionColor[] UpdateShapeAndCreateVertex()
        {
            this._quadrangle = this.CreateQuatrangle();
            var vertex = this.Shape.DrawableShape;

            var color = this.GetColor();
            return vertex.Select( v => new VertexPositionColor( v.ToVector3(), color ) )
                .ToArray();
        }

        private Quadrangle CreateQuatrangle()
        {
            var startLine = MyMathHelper.CreatePerpendicualrLine(
                                                                  this.Object.StartLocation,
                                                                  this.Object.EndLocation,
                                                                  Constans.PointSize );
            var endLine = MyMathHelper.CreatePerpendicualrLine(
                                                                this.Object.EndLocation,
                                                                this.Object.StartLocation,
                                                                Constans.PointSize );
            return new Quadrangle( 
                            startLine.Item1,
                            startLine.Item2,
                            endLine.Item1,
                            endLine.Item2 );
        }

        private Color GetColor()
        {
            if ( this.Object.IsSelected )
            {
                return this._selectedColor;
            }

            switch ( this.Object.LaneType )
            {
                case LaneType.SolidLine:
                    return this._normalColor;
                case LaneType.HiddenLine:
                    return new Color( 0, 0, 0, 0 );
                case LaneType.DottedLine:
                    return Color.Red;
                default:
                    return this._normalColor;
            }
        }
    }
}