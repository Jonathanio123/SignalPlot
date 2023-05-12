//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.UI.Xaml.Controls;
//using Microsoft.UI.Xaml.Documents;
//using ScottPlot;
//using ScottPlot.Axis;
//using ScottPlot.Control;

//namespace SignalPlot.Customs;

//internal sealed class LinkedInteraction : Interaction
//{
//    private readonly IPlotControl _digitalControl;
//    private readonly IPlotControl _analogueControl;
//    private readonly KeyboardState Keyboard = new();
//    private readonly MouseState Mouse = new();
//    private new bool IsDraggingMouse(Pixel pos) => Mouse.PressedButtons.Any() && Mouse.IsDragging(pos);

//    private bool LockX => Inputs.ShouldLockX(Keyboard.PressedKeys);
//    private bool LockY => Inputs.ShouldLockY(Keyboard.PressedKeys);
//    private bool IsZoomingRectangle = false;

//    public new InputBindings Inputs = InputBindings.Standard();

//    public new PlotActions Actions = PlotActions.Standard();


//    public new ContextMenuItem[] ContextMenuItems = Array.Empty<ContextMenuItem>();
//    public new string DefaultSaveImageFilename { get; set; } = "Plot.png";

//    public LinkedInteraction(IPlotControl control, IPlotControl digitalControl) : base(control)
//    {
//        _digitalControl = digitalControl;
//        _analogueControl = control;
//    }

//    public override void OnMouseMove(Pixel newPosition)
//    {
//        Mouse.LastPosition = newPosition;

//        if (IsDraggingMouse(newPosition))
//        {
//            MouseDrag(
//                from: Mouse.MouseDownPosition,
//                to: newPosition,
//                button: Mouse.PressedButtons.First(),
//                keys: Keyboard.PressedKeys,
//                start: Mouse.MouseDownAxisLimits);
//        }
//    }

//    public new Coordinates GetMouseCoordinates(IXAxis? xAxis = null, IYAxis? yAxis = null)
//    {
//        return _analogueControl.Plot.GetCoordinate(Mouse.LastPosition, xAxis, yAxis);
//    }

//    protected override void MouseDrag(Pixel from, Pixel to, MouseButton button, IEnumerable<Key> keys,
//        MultiAxisLimits start)
//    {
//        bool lockY = Inputs.ShouldLockY(keys);
//        bool lockX = Inputs.ShouldLockX(keys);

//        LockedAxes locks = new(lockX, lockY);

//        MouseDrag drag = new(start, from, to);

//        if (Inputs.ShouldZoomRectangle(button, keys))
//        {
//            Actions.DragZoomRectangle(_analogueControl, drag, locks);
//            IsZoomingRectangle = true;
//        }
//        else if (button == Inputs.DragPanButton)
//        {
//            Actions.DragPan(_analogueControl, drag, locks);
//        }
//        else if (button == Inputs.DragZoomButton)
//        {
//            Actions.DragZoom(_analogueControl, drag, locks);
//        }
//    }


//    public override void KeyUp(Key key)
//    {
//        Keyboard.Up(key);
//    }

//    public override void KeyDown(Key key)
//    {
//        Keyboard.Down(key);
//    }

//    public override void MouseDown(Pixel position, MouseButton button)
//    {
//        Mouse.Down(position, button, _analogueControl.Plot.GetMultiAxisLimits());
//    }

//    public override void MouseUp(Pixel position, MouseButton button)
//    {
//        bool isDragging = Mouse.IsDragging(position);

//        bool droppedZoomRectangle =
//            isDragging &&
//            Inputs.ShouldZoomRectangle(button, Keyboard.PressedKeys) &&
//            IsZoomingRectangle;

//        if (droppedZoomRectangle)
//        {
//            Actions.ZoomRectangleApply(_analogueControl);
//            IsZoomingRectangle = false;
//        }

//        // this covers the case where an extremely tiny zoom rectangle was made
//        if ((isDragging == false) && (button == Inputs.ClickAutoAxisButton))
//        {
//            Actions.AutoScale(_analogueControl);
//        }

//        if (button == Inputs.DragZoomRectangleButton)
//        {
//            Actions.ZoomRectangleClear(_analogueControl);
//            IsZoomingRectangle = false;
//        }

//        if (!isDragging && (button == Inputs.ClickContextMenuButton))
//        {
//            _analogueControl.ShowContextMenu(position);
//        }

//        Mouse.Up(button);
//    }

//    public override void DoubleClick()
//    {
//        Actions.ToggleBenchmark(_analogueControl);
//    }

//    public override void MouseWheelVertical(Pixel pixel, float delta)
//    {
//        this.MouseWheelVertical(pixel, delta, _analogueControl);
//        this.MouseWheelVertical(pixel, delta, _digitalControl);
//    }

//    private void MouseWheelVertical(Pixel pixel, float delta, IPlotControl control)
//    {
//        MouseWheelDirection direction = delta > 0 ? MouseWheelDirection.Up : MouseWheelDirection.Down;

//        if (Inputs.ZoomInWheelDirection.HasValue && Inputs.ZoomInWheelDirection == direction)
//        {
//            Actions.ZoomIn(control, pixel, new LockedAxes(LockX, LockY));
//        }
//        else if (Inputs.ZoomOutWheelDirection.HasValue && Inputs.ZoomOutWheelDirection == direction)
//        {
//            Actions.ZoomOut(control, pixel, new LockedAxes(LockX, LockY));
//        }
//        else if (Inputs.PanUpWheelDirection.HasValue && Inputs.PanUpWheelDirection == direction)
//        {
//            Actions.PanUp(control);
//        }
//        else if (Inputs.PanDownWheelDirection.HasValue && Inputs.PanDownWheelDirection == direction)
//        {
//            Actions.PanDown(control);
//        }
//        else if (Inputs.PanRightWheelDirection.HasValue && Inputs.PanRightWheelDirection == direction)
//        {
//            Actions.PanRight(control);
//        }
//        else if (Inputs.PanLeftWheelDirection.HasValue && Inputs.PanLeftWheelDirection == direction)
//        {
//            Actions.PanLeft(control);
//        }
//    }
//}

