import sys
import threading


class StdioStream:
    """Stream-Wrapper für stdin/stdout/Prozess-Pipes, der mit JsonRpcPeer kompatibel ist"""

    def __init__(self, stream, mode):
        """
        Erstellt einen Stream-Wrapper
        :param stream: Das zu wrappende Stream-Objekt (sys.stdin, sys.stdout, proc.stdout etc.)
        :param mode: 'r' für Lesen, 'w' für Schreiben
        """
        self.stream = stream
        self.mode = mode
        self._lock = threading.Lock()  # Verhindert parallele Schreibzugriffe

        # Bei Initialisierung Lese- und Schreib-Methode bestimmen
        self._read_method = self._determine_read_method()
        self._write_method = self._determine_write_method()

    def readable(self) -> bool:
        return self.stream.readable()

    def writable(self) -> bool:
        return self.stream.writable()

    def _determine_read_method(self):
        """Bestimmt die optimale Lesemethode basierend auf den Stream-Fähigkeiten"""
        # Bei TextIOWrapper (wie sys.stdin) nutzen wir den Buffer für binären Zugriff
        # Bei anderen Streams (wie subprocess.PIPE) ist direkter Zugriff bereits binär
        if hasattr(self.stream, "read1"):
            return self._read_with_read1
        if hasattr(self.stream, "read"):
            return self._read_with_select
        return self._read_empty

    def _determine_write_method(self):
        """Bestimmt die optimale Schreibmethode basierend auf den Stream-Fähigkeiten"""
        # Für TextIOWrapper-Streams wie sys.stdout direkter binärer Zugriff
        # über Buffer, wenn verfügbar
        if hasattr(self.stream, "buffer") and self.mode == "w":
            return self._write_to_buffer

        return self._write_direct

    def _read_with_read1(self, size):
        """Lesen mit read1-Methode (nicht-blockierend)"""
        try:
            data = self.stream.read1(size)
            return data if data else b""
        except (IOError, OSError):
            return b""

    def _read_with_select(self, size):
        """Nicht-blockierendes Lesen mit select"""
        try:
            import select
            import platform

            # Auf Windows kann select nur mit Sockets verwendet werden, nicht mit Dateien/Pipes
            if platform.system() == "Windows":
                # Windows-spezifischer Code: ohne select arbeiten
                try:
                    # Versuch zu lesen ohne zu blockieren
                    data = self.stream.read(size)
                    return data if data else b""
                except (IOError, OSError):
                    return b""
            else:
                # Unix-basierte Systeme können select für Dateien verwenden
                r, _, _ = select.select([self.stream], [], [], 0.01)
                if not r:
                    return b""
                data = self.stream.read(size)
                return data if data else b""
        except (IOError, OSError):
            return b""

    def _read_empty(self, size):
        """Fallback-Methode, wenn keine geeignete Lesemethode gefunden wurde"""
        return b""

    def _write_to_buffer(self, data):
        """
        Schreiben in den Buffer eines TextIOWrapper
        Dies umgeht die Textkonvertierung und schreibt direkt binäre Daten
        """
        try:
            return self.stream.buffer.write(data)
        except (IOError, OSError) as e:
            print(f"Fehler beim Schreiben auf Buffer: {e}", file=sys.stderr)
            return 0

    def _write_direct(self, data):
        """Direktes Schreiben auf den Stream"""
        try:
            return self.stream.write(data)
        except (IOError, OSError) as e:
            print(f"Fehler beim Schreiben: {e}", file=sys.stderr)
            return 0

    def read(self, size=4096):
        """
        Nicht-blockierendes Lesen vom Stream
        :param size: Maximale Anzahl zu lesender Bytes
        :return: Gelesene Daten als bytes
        """
        return self._read_method(size)

    def write(self, data):
        """
        Thread-sicheres Schreiben in den Stream
        :param data: Zu schreibende Daten (bytes)
        :return: Anzahl geschriebener Bytes
        """
        with self._lock:
            return self._write_method(data)

    def flush(self):
        """Stream leeren"""
        with self._lock:
            try:
                self.stream.flush()
            except (IOError, OSError) as e:
                print(f"Fehler beim Flush: {e}", file=sys.stderr)

    def fileno(self):
        """
        Gibt den Dateideskriptor zurück (benötigt für selectors)
        :return: Filedeskriptor-Nummer
        """
        return self.stream.fileno()
