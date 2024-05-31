using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ColorPicker.CustomControls
{
    public class CustomSlider : Slider
    {
        public bool IsMouseDown { get; set; }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            IsMouseDown = true;
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            if (IsMouseDown)
            {
                double relativePosition = e.GetPosition(this).X / ActualWidth;
                double newValue = Math.Ceiling(relativePosition * Maximum);

                Value = Math.Min(Maximum, Math.Max(Minimum, newValue));
            }
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);

            IsMouseDown = false;
        }
    }
}
