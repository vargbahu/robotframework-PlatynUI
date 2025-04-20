import asyncio
import contextlib
import subprocess
import sys
from pathlib import Path

if __name__ == "__main__" and not __package__:
    file = Path(__file__).resolve()
    parent, top = file.parent, file.parents[1]

    if str(top) not in sys.path:
        sys.path.append(str(top))

    with contextlib.suppress(ValueError):
        sys.path.remove(str(parent))

    __package__ = "playground"

from .jsonpeer import JsonRpcPeer


async def create_process_streams(program: str, *args: str, cwd: str | None = None):
    """Erstellt asyncio Streams für einen Subprocess"""
    # Process mit Pipes erstellen
    process = await asyncio.create_subprocess_exec(
        program,
        *args,
        stdin=subprocess.PIPE,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        cwd=cwd,
    )

    # In asyncio.create_subprocess_exec sind stdin, stdout und stderr bereits StreamWriter bzw. StreamReader
    # Wir müssen sie nicht manuell umwandeln
    reader = process.stdout
    writer = process.stdin

    return process, reader, writer


async def main():
    # Server-Prozess starten
    cmd = [
        "dotnet",
        "run",
        "--project",
        "src/PlatynUI.Server",
        "--stdio",
    ]

    process, reader, writer = await create_process_streams(*cmd)

    # JsonRpcPeer erstellen, der mit dem Serverprozess kommuniziert
    peer = JsonRpcPeer(reader, writer)

    # Peer starten
    await peer.start()

    try:
        # Initialisierung senden
        #await peer.send_request("Initialize", {"processId": os.getpid()})

        # Show-Anfragen in einer Schleife senden
        for x in range(40, 1000, 1):
            await peer.send_request("displayDevice/highlightRect", {"x": x, "y": x, "width": 100, "height": 100, "timeout": 3})
            await asyncio.sleep(0.001)  # Kleine Pause zwischen Anfragen

        # Warten, um die Anzeige zu sehen
        await asyncio.sleep(3)

        # Eine Notification senden
        await peer.send_notification("Exit", {})

        # Kurze Pause, um dem Server Zeit zum Verarbeiten zu geben
        await asyncio.sleep(0.5)

    except Exception as e:
        print(f"Fehler: {e}")
    finally:
        # Peer und Prozess ordnungsgemäß beenden
        await peer.stop()

        print("Prozess beenden...")
        process.terminate()
        try:
            await asyncio.wait_for(process.wait(), timeout=2)
        except asyncio.TimeoutError:
            process.kill()

        # Stderr des Servers ausgeben (zur Fehlersuche)
        stderr_output = await process.stderr.read()
        if stderr_output:
            print(f"Server stderr:\n{stderr_output.decode('utf-8')}")


if __name__ == "__main__":
    asyncio.run(main())
