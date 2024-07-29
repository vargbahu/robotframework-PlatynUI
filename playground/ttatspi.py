import pyatspi
import pyatspi.component


desktop = pyatspi.Registry.getDesktop(0)
for application in desktop:
    print(application.name)
    if application.name=="gnome-text-editor":
        for w in application:
            print(w.name)
            print(w.get_role_name())
            print(w.get_position(pyatspi.component.XY_SCREEN).x)
            print(w.get_position(pyatspi.component.XY_SCREEN).y)
            print(w.get_extents(pyatspi.component.XY_SCREEN).x)
            print(w.get_extents(pyatspi.component.XY_SCREEN).y)
