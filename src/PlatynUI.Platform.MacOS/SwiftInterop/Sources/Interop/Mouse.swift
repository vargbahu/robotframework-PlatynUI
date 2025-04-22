import CoreGraphics
import Foundation

@_cdecl("GetMousePosition")
public func getMousePosition(_ x: UnsafeMutablePointer<Double>, _ y: UnsafeMutablePointer<Double>) {

    let p = CGEvent(source: nil)?.location ?? .zero

    x.pointee = p.x
    y.pointee = p.y
}

@_cdecl("MouseMove")
public func mouseMove(_ x: Double, _ y: Double) {
    do {
        let point = CGPoint(x: x, y: y)
        let mouseMoveEvent = CGEvent(
            mouseEventSource: nil, mouseType: .mouseMoved,
            mouseCursorPosition: point, mouseButton: .left)
        mouseMoveEvent?.post(tap: .cghidEventTap)
    }
}
