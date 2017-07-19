using System;
using System.Windows;
using static FlexiWallUI.Models.FlexiWall;

namespace FlexiWallUI.Models
{
    public class CameraEmulator
    {
        private Point _size;
        private int _delta;
        private float _depth;
        private Point _position;

        private InteractionEventArgs _interactionEventArgs;

        public int Delta
        {
            get { return _delta; }
            set
            {
                _delta = value;
                RaiseInteractionEvent(_position, _delta);
            }
        }

        public Point Position
        {
            get { return _position; }
            set
            {
                _position = value;
                RaiseInteractionEvent(_position, 0);
            }
        }

        public CameraEmulator(Point DisplayResolution)
        {
            _size = DisplayResolution;

            _depth = 0;
            _position = new Point();
            _interactionEventArgs = new InteractionEventArgs();
        }

        private void RaiseInteractionEvent(Point pos, int delta)
        {
            _interactionEventArgs.ID++;

            if (Math.Abs(_depth + (delta * 0.001f)) < 6)
                _depth += delta * 0.001f;

            _interactionEventArgs.DisplayCoordinates.Set((float)(pos.X / _size.X), (float)(pos.Y / _size.Y), _depth);

            _interactionEventArgs.TypeOfInteraction = InteractionType.NONE;

            if (_depth < 0)
            {
                _interactionEventArgs.TypeOfInteraction = InteractionType.PUSHED;
            }
            else if (_depth > 0)
            {
                _interactionEventArgs.TypeOfInteraction = InteractionType.PULLED;
            }

            OnNewInteraction(this, _interactionEventArgs);
        }

        public event InteractionEventHandler NewInteraction;

        protected virtual void OnNewInteraction(object sender, InteractionEventArgs args)
        {
            NewInteraction?.Invoke(sender, args);
        }
    }
}
