using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using PlatynUI.Technology.UiAutomation.Client;
using PlatynUI.Technology.UiAutomation.Core;
using PlatynUI.Technology.UiAutomation.Spy.ElementsModel;
using PlatynUI.Technology.UiAutomation.Tools;

namespace PlatynUI.Technology.UiAutomation.Spy;

[Export]
public sealed class ShellViewModel : Screen, IDisposable, IUIAutomationEventHandler
{
    private readonly Highlighter _highlighter = new(autoHideTimeout: 5000);
    private ElementBase _elements = new UiaRootElement();
    private ElementBase? _selectedElement;
    private string _title = "PlatynUI UiAutomation Spy";

    readonly DispatcherTimer dispatcherTimer = new();

    public ShellViewModel()
    {
        Automation.UiAutomation.AddAutomationEventHandler(
            UIA_EventIds.UIA_Window_WindowClosedEventId,
            Automation.RootElement,
            TreeScope.TreeScope_Children,
            null,
            this
        );
        Automation.UiAutomation.AddAutomationEventHandler(
            UIA_EventIds.UIA_Window_WindowOpenedEventId,
            Automation.RootElement,
            TreeScope.TreeScope_Children,
            null,
            this
        );

        dispatcherTimer.Tick += (sender, e) => OnTimerTick();
        dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
        dispatcherTimer.Start();
    }

    public void Dispose()
    {
        Automation.UiAutomation.RemoveAllEventHandlers();
        _highlighter?.Dispose();
    }

    private void OnTimerTick()
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftShift))
        {
            Debug.WriteLine("Get element from point");
            var element = Automation.FromPoint(MouseDevice.GetPosition());

            if (element != null)
            {
                element = element.Realize();
                if (element.CurrentProcessId != UiAutomationElementExtensions.CurrentProcessId)
                {
                    SelectElement(false, element);
                }
            }
        }
    }

    bool _topmost = false;

    public bool Topmost
    {
        get { return _topmost; }
        set
        {
            if (value == _topmost)
            {
                return;
            }

            _topmost = value;
            NotifyOfPropertyChange();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (value == _title)
            {
                return;
            }

            _title = value;
            NotifyOfPropertyChange();
        }
    }

    public ElementBase RootElement
    {
        get => _elements;
        set
        {
            if (Equals(value, _elements))
            {
                return;
            }

            _elements = value;
            NotifyOfPropertyChange();
        }
    }

    public ElementBase? SelectedElement
    {
        get => _selectedElement;
        set
        {
            if (Equals(value, _selectedElement))
            {
                return;
            }

            _selectedElement = value;
            NotifyOfPropertyChange();
        }
    }

    public void SetSelectedItem(ElementBase item)
    {
        try
        {
            SelectedElement = item;
            if (item is UiaElement uiaElement && uiaElement.AutomationElement.CurrentIsOffscreen == 0)
            {
                _highlighter.Show(uiaElement.AutomationElement.CurrentBoundingRectangle.ToRect());
            }
        }
        catch
        {
            // do nothing
        }
    }

    public async void Exit()
    {
        await TryCloseAsync();
    }

    public void RefreshElements()
    {
        if (SelectedElement == null)
        {
            SelectedElement = null;
            RootElement = new UiaRootElement();
        }
        else
        {
            SelectedElement.Refresh();
        }
    }

    public string XPath { get; set; } = string.Empty;

    public void ExecuteXPath(ActionExecutionContext context)
    {
        if (context.EventArgs is KeyEventArgs keyArgs && keyArgs.Key == Key.Enter)
        {
            SearchXPath(keyArgs.KeyboardDevice.Modifiers == ModifierKeys.Control);
        }
    }

    Cursor _cursor = Cursors.Arrow;
    public Cursor Cursor
    {
        get { return _cursor; }
        set
        {
            if (value == _cursor)
            {
                return;
            }

            _cursor = value;
            NotifyOfPropertyChange();
        }
    }

    public void SearchXPath(bool focus = false)
    {
        if (string.IsNullOrWhiteSpace(XPath))
        {
            return;
        }
        try
        {
            Cursor = Cursors.Wait;
            try
            {
                var element = Finder.FindSingleElement(null, XPath.Trim(), true);
                if (element != null)
                    SelectElement(focus, element);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Element not found {ex.Message}",
                    "Search XPath",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
        finally
        {
            Cursor = Cursors.Arrow;
        }
    }

    private void SelectElement(bool focus, IUIAutomationElement element)
    {
        var stopwatch = Stopwatch.StartNew();
        stopwatch.Start();

        var parents = new List<IUIAutomationElement>();
        var parent = element.GetCurrentParent();

        while (parent != null)
        {
            parents.Insert(0, parent);
            parent = parent.GetCurrentParent();
        }

        parents.Insert(0, (RootElement.Children.First() as UiaElement)!.AutomationElement);

        var currentElement = RootElement;
        ElementBase? lastParent = null;
        foreach (var parentElement in parents)
        {
            var foundElement = currentElement.FindAutomationElement(parentElement);
            lastParent = foundElement;

            if (foundElement == null)
            {
                break;
            }

            foundElement.IsExpanded = true;

            currentElement = foundElement;
        }

        if (currentElement != null)
        {
            currentElement = currentElement!.FindAutomationElement(element);
        }

        currentElement ??= lastParent;

        if (currentElement != null)
        {
            currentElement.IsSelected = true;
            if (focus)
            {
                // TODO: Bring the window to the front
                element.SetFocus();
            }
        }

        Debug.WriteLine($"SelectElement took {stopwatch.ElapsedMilliseconds} ms");
    }

    public void HandleAutomationEvent(
        [In, MarshalAs(UnmanagedType.Interface)] IUIAutomationElement sender,
        [In] int eventId
    )
    {
        //_elements.Refresh();
        Debug.WriteLine($"HandleAutomationEvent: {sender.CurrentName} {eventId}");
    }
}
