using System;
using RoadTrafficSimulator.Components.SimulationMode.Conductors;
using RoadTrafficSimulator.Infrastructure.Controls;

namespace RoadTrafficSimulator.Components.SimulationMode
{
    public abstract class RoadElementBase : IRoadElement
    {
        private readonly IControl _control;
        private readonly IDrawer _drawer;

        public RoadElementBase( IControl control )
        {
            this._control = control;
            this._drawer = new StandardDrawer( this );
        }

        public IControl BuildControl
        {
            get { return this._control; }
        }

        public abstract IConductor Condutor { get; }

        public IDrawer Drawer
        {
            get { return this._drawer; }
        }
    }
}