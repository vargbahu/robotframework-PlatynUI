*** Settings ***
Library     PlatynUI


*** Test Cases ***
first
    ${pos}    Mouse Position

    GROUP    Move to end
        Mouse Press    app:Application[@Name="AvaloniaTestApp"]/MainWindow//ScrollBar[@Name='MyScrollBar']//Thumb
        Mouse Release
        ...    app:Application[@Name="AvaloniaTestApp"]/MainWindow//ScrollBar[@Name='MyScrollBar']
        ...    x=0
        ...    y=0
    END

    GROUP    Move to Start
        Mouse Press    app:Application[@Name="AvaloniaTestApp"]/MainWindow//ScrollBar[@Name='MyScrollBar']//Thumb
        ${rect}    Get Property Value
        ...    app:Application[@Name="AvaloniaTestApp"]/MainWindow//ScrollBar[@Name='MyScrollBar']
        ...    BoundingRectangle
        Sleep    1s

        Mouse Release
        ...    app:Application[@Name="AvaloniaTestApp"]/MainWindow//ScrollBar[@Name='MyScrollBar']
        ...    x=${rect.width}
    END
