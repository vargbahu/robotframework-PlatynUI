*** Settings ***
Library     PlatynUI
Library     PlatynUI.Extended

Variables    mapping.py

*** Test Cases ***
first
    # Activate    map:AppWindow.Button1
    # Set Text    asdfsd
    # Get Property    map:AppWindow.Button1    Name    Should Be   ölaksdjfölaksj
    Activate   ${calculator.n1}
    Select
    Focus
    Set Text
    Set Value
    Get Property


    CheckBox Toggle
    Option Toggle

    Is Enable
    Is Visible
    Is Selected

    Button Activate
