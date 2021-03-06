﻿using System;
using System.Diagnostics;
using RoadTrafficSimulator.Infrastructure.Controls;

namespace RoadTrafficSimulator.Infrastructure.Controls
{
    [DebuggerStepThrough]
    public class TranslationChangedEventArgs : EventArgs
    {
        public TranslationChangedEventArgs( IControl control )
        {
            this.Control = control;
        }

        public IControl Control { get; private  set; }
    }
}