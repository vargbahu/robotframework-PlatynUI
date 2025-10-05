*** Settings ***
Library     PlatynUI


*** Test Cases ***
first
    # Mouse Click    app:Application/Frame//Label[@Name="Welcome to Kate"]

    # Mouse Click    app:Application/Frame//PushButton[@Name="Open File..."]

    #    Mouse Click    app:Application/Frame//PushButton[@Name="Open File..."]
    ${names}  Get Property Names    app:Application/Frame//Label[@Name="Welcome to Kate"]
    Log To Console    ${names}
    ${a}    Get Property Value    app:Application/Frame//Label[@Name="Welcome to Kate"]    Name
    Get Text   app:Application/Frame//Label[@Name="Welcome to Kate"]    should be    Welcome to Kate
    Log To Console    ${a}
