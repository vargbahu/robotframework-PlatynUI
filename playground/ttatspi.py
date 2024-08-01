from typing import Union, cast
import pyatspi


app: Union[pyatspi.Accessible, pyatspi.Application]
w: Union[pyatspi.Accessible, pyatspi.Component]
desktop = pyatspi.Registry.getDesktop(0)
for app in desktop:

    print("Application", app.name, app.id)
    for w in app:
        print(w.getRoleName(), w.name)
