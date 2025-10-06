*** Settings ***
Library     PlatynUI


*** Test Cases ***
first
    # Mouse Click    app:Application/Frame//Label[@Name="Welcome to Kate"]

    # Mouse Click    app:Application/Frame//PushButton[@Name="Open File..."]

    #    Mouse Click    app:Application/Frame//PushButton[@Name="Open File..."]
    # ${names}    Get Property Names    app:Application/Frame//Label[@Name="Welcome to Kate"]
    # Log To Console    ${names}
    # ${a}    Get Property Value    app:Application/Frame//Label[@Name="Welcome to Kate"]    Name
    # Get Text    app:Application/Frame//Label[@Name="Welcome to Kate"]    should be    Welcome to Kate
    # Log To Console    ${a}
    ${names}    Get Property Names    app:Application[@MainWindowTitle="Rechner"]//Button[@Name="Eins"]
    Log Many    ${names}
    ${class_name}    Get Property Value
    ...    app:Application[@MainWindowTitle="Rechner"]//Button[@Name="Eins"]
    ...    ClassName
    Log    class_name: ${class_name}
    Get Property Value
    ...    app:Application[@MainWindowTitle="Rechner"]//Button[@AutomationId="num1Button"]
    ...    Name
    ...    equal
    ...    Eins

    Get Property Names    app:Application[@MainWindowTitle="Rechner"]//Button[@Name="Eins"]    contains    ClassName

    # Get Text    app:Application[@MainWindowTitle="Rechner"]//Text[@AutomationId="NormalOutput"]
