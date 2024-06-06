*** Settings ***
Library     PlatynUI


*** Test Cases ***
first
    Activate    map:AppWindow.Button1
    Set Text    asdfsd
    Get Property    map:AppWindow.Button1    Name    Should Be   ölaksdjfölaksj
