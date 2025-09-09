*** Settings ***
Library     PlatynUI


*** Test Cases ***

Check Dynamic Content
    Mouse Click    app:Application[@Name="AvaloniaTestApp"]/MainWindow//Button[@AutomationId='AddButtonCommand']
    Sleep    1s
    Mouse Click    app:Application[@Name="AvaloniaTestApp"]/MainWindow//Button[@AutomationId='DynamicRemoveButton']

Check Dynamic Content 2
    Mouse Click    app:Application[@Name="AvaloniaTestApp"]/MainWindow//Button[@AutomationId='DummyButton']
    Sleep    1s
    Mouse Click    app:Application[@Name="AvaloniaTestApp"]/MainWindow//Button[@AutomationId='AddButtonCommand']
    Sleep    1s
    #Invalidate Node    app:Application[@Name="AvaloniaTestApp"]
    Mouse Click    app:Application[@Name="AvaloniaTestApp"]/MainWindow//Button[@AutomationId='DynamicRemoveButton']
