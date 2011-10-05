﻿using System.Collections.Generic;
using RoadTrafficSimulator.Infrastructure.Controls;

namespace RoadTrafficSimulator.Components.BuildMode.Controls
{
    public class PriorityTypeFactory
    {
        private IEnumerable<IPriorityPossible> _posibilityChecker = new IPriorityPossible[]
                                                                        {
                                                                            new LigthPriorityPosible()
                                                                        };
        public IEnumerable<PriorityType> PossiblePriorityTypes( IControl baseControl, IControl connectedControl )
        {
            return new []
                       {
                           PriorityType.Light,
                           PriorityType.FromRight,
                           PriorityType.FromLeft,
                           PriorityType.FromFront,
                       };
        }
    }

    public interface IPriorityPossible
    {
        IEnumerable<PriorityType> GetPossiblePriorityTypes( IControl baseControl, IControl connectedControls );
    }
}