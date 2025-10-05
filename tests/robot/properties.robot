*** Settings ***
Library     PlatynUI


*** Test Cases ***
first
    #Mouse Click    app:Application/Frame//Label[@Name="Welcome to Kate"]
    Mouse Click    app:Application/Frame//PushButton[@Name="Open File..."]
