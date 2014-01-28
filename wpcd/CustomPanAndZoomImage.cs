using Telerik.Windows.Controls.SlideView;

namespace wpcd {
    public class CustomPanAndZoomImage : PanAndZoomImage {
        public bool DoubleTapOccured { get; set; }
        protected override void OnDoubleTap(System.Windows.Input.GestureEventArgs e) {
            base.OnDoubleTap(e);
            DoubleTapOccured = true;
        }
    }
}
