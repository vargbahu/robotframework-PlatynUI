using PlatynUI.JsonRpc.Endpoints;
using PlatynUI.Runtime;
using PlatynUI.Runtime.Core;

namespace PlatynUI.Server.Endpoints;

partial class MouseDeviceEndpoint : IMouseDeviceEndpoint
{
    public Size GetDoubleClickSize()
    {
        return MouseDevice.GetDoubleClickSize();
    }

    public double GetDoubleClickTime()
    {
        return MouseDevice.GetDoubleClickTime();
    }

    public Point GetPosition()
    {
        return MouseDevice.GetPosition();
    }

    public void Move(double x, double y, bool direct = false, int maxDurationMs = 500)
    {
        if (direct)
        {
            MouseDevice.Move(x, y);
            return;
        }

        var screen = DisplayDevice.GetBoundingRectangle();

        // Get current mouse position
        var currentPosition = MouseDevice.GetPosition();

        // Calculate the distance between current position and target
        double totalDistance = Math.Sqrt(Math.Pow(x - currentPosition.X, 2) + Math.Pow(y - currentPosition.Y, 2));

        // If distance is very small, just move directly
        if (totalDistance < 5)
        {
            MouseDevice.Move(x, y);
            return;
        }

        // Dynamic steps calculation - more steps for longer distances, fewer for short distances
        int steps = Math.Min((int)(totalDistance / 5), 100);
        steps = Math.Max(steps, 10); // At least 10 steps for smoothness

        // Calculate optimal delay between steps
        int delayBetweenStepsMs = maxDurationMs / steps;

        // Start timestamp with high precision
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Prepare variables for ease-in-out movement
        double startX = currentPosition.X;
        double startY = currentPosition.Y;

        // Move mouse gradually along the path with easing
        for (int i = 1; i <= steps; i++)
        {
            // Calculate progress (0.0 to 1.0) with easing for more natural movement
            double t = (double)i / steps;

            // Ease-in-out curve: slower at start and end, faster in middle
            double easedT = EaseInOutQuad(t);

            // Calculate current position using easing function
            double intermediateX = startX + (x - startX) * easedT;
            double intermediateY = startY + (y - startY) * easedT;

            // Ensure coordinates stay within screen boundaries
            intermediateX = Math.Max(screen.Left, Math.Min(screen.Right, intermediateX));
            intermediateY = Math.Max(screen.Top, Math.Min(screen.Bottom, intermediateY));

            // Move to this position
            MouseDevice.Move(intermediateX, intermediateY);

            // Calculate remaining steps and adjust delay for consistent timing
            if (i < steps)
            {
                // Calculate elapsed time and how much we need to wait
                long elapsedMs = stopwatch.ElapsedMilliseconds;
                long expectedTimeMs = i * delayBetweenStepsMs;
                int sleepMs = (int)Math.Max(0, expectedTimeMs - elapsedMs);

                if (sleepMs > 0)
                {
                    System.Threading.Thread.Sleep(sleepMs);
                }

                // If we're already behind schedule, don't sleep but
                // if we're too far behind, jump to the end
                if (stopwatch.ElapsedMilliseconds > maxDurationMs)
                {
                    MouseDevice.Move(x, y);
                    return;
                }
            }
        }

        // Final precise move exactly to the target coordinates
        MouseDevice.Move(x, y);
    }

    // Easing function: quad ease-in-out for more natural movement
    private static double EaseInOutQuad(double t)
    {
        return t < 0.5 ? 2 * t * t : 1 - Math.Pow(-2 * t + 2, 2) / 2;
    }

    public void Press(MouseButton button)
    {
        MouseDevice.Press(button);
    }

    public void Release(MouseButton button)
    {
        MouseDevice.Release(button);
    }
}
