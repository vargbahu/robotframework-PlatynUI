from pythonnet import load, get_runtime_info

#load("coreclr")
load("netfx")

#import clr
from System import Environment
from System.Runtime.InteropServices import RuntimeInformation

print(get_runtime_info().kind)
print(Environment.Version)
print(RuntimeInformation.ProcessArchitecture)
print(RuntimeInformation.FrameworkDescription)
