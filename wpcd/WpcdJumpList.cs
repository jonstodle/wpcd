using Telerik.Windows.Controls;

namespace wpcd {
    public class WpcdJumpList : RadJumpList {
        public RadWindow TopWindow;
        protected override void OnManipulationDelta(System.Windows.Input.ManipulationDeltaEventArgs e) {
            if(this.ScrollState == ScrollState.TopStretch) {
                if(e.CumulativeManipulation.Translation.Y > 100) {
                    TopWindow.IsOpen = !TopWindow.IsOpen;
                }
            }
            System.Diagnostics.Debug.WriteLine("From WpcdJumpList: " + this.ScrollState + " - " + e.CumulativeManipulation.Translation.Y);
            base.OnManipulationDelta(e);
        }
    }
}
