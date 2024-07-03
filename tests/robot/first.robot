*** Settings ***
Library         PlatynUI
Variables       mapping.py


*** Test Cases ***
first
    Activate    ${calculator}
    Activate    ${calculator.clear}
    Activate    ${calculator.n1}
    Activate    ${calculator.n2}
    Activate    ${calculator.n3}
    Activate    ${calculator.n4}
    Activate    ${calculator.plus}
    Activate    ${calculator.n5}
    Activate    ${calculator.n6}
    Activate    ${calculator.n7}
    Activate    ${calculator.n8}

    Activate    ${calculator.equal}
