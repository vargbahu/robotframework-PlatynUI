import sys
from PySide6 import QtCore, QtWidgets, QtGui


if __name__ == "__main__":
    app = QtWidgets.QApplication([])

    my_widget = QtWidgets.QWidget(
        None,
        QtCore.Qt.WindowType.ToolTip
        | QtCore.Qt.WindowType.BypassGraphicsProxyWidget
        | QtCore.Qt.WindowType.X11BypassWindowManagerHint
        | QtCore.Qt.WindowType.WindowStaysOnTopHint
        | QtCore.Qt.WindowType.NoDropShadowWindowHint,
    )

    my_widget.move(0,0)
    my_widget.resize(800, 600)
    my_widget.show()

    sys.exit(app.exec())
