﻿using System;
using Microsoft.Xna.Framework;
using RoadTrafficSimulator.Infrastructure.Draw;
using RoadTrafficSimulator.Infrastructure.Mouse;

namespace RoadTrafficSimulator.Infrastructure.Controls
{
    public interface ILogicControl
    {
        Vector2 Location { get; }

        IControl Parent { get; set; }

        IObservable<bool> SelectionChanged { get; }
        bool IsSelected { get; set; }
        void Redraw();
        void Invalidate();
    }

    public interface IControl : ILogicControl
    {
        IObservable<TranslationChangedEventArgs> Translated { get; }

        IVertexContainer VertexContainer { get; }

        IMouseHandler MouseHandler { get; }

        IObservable<Unit> Redrawed { get; }

        void Translate( Matrix matrixTranslation );

        void TranslateWithoutNotification( Matrix translationMatrix );

        Vector2 ToControlPosition( Vector2 screenPosition );

        bool IsHitted( Vector2 location );

        ILogicControl GetHittedControl( Vector2 point );

        Guid Id { get; set; }
    }

    public interface ISingleControl : IControl
    {
    }
}