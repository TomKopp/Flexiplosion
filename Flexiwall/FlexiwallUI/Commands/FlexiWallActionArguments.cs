using System.Windows;

namespace FlexiWallUI.Commands
{
    public class FlexiWallActionArguments
    {
        public Point Position { get; set; }

        public FlexiWallActionState State { get; set; }

        public FlexiWallActionType Type { get; set; }
    }
}