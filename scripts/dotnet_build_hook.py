# SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
#
# SPDX-License-Identifier: Apache-2.0

import contextlib
import subprocess
import sys
from functools import cached_property
from pathlib import Path
from typing import Any, Dict, List

from hatchling.builders.config import BuilderConfig
from hatchling.builders.hooks.plugin.interface import BuildHookInterface

file = Path(__file__).resolve()
parent, top = file.parent, file.parents[1]

if str(top) not in sys.path:
    sys.path.append(str(top))

with contextlib.suppress(ValueError):
    sys.path.remove(str(parent))

__package__ = "scripts"

from scripts.rm import rm  # noqa: E402


class DotNetBuildHook(BuildHookInterface[BuilderConfig]):
    BASE_OUTPUT_DIR = Path("src/PlatynUI/ui/runtime/coreclr")
    BASE_PROVIDERS_DIR = Path("src/PlatynUI/ui/runtime/providers")

    @cached_property
    def _dotnet_runtime_identifier(self) -> str:
        from pythonnet import load

        load("coreclr")

        import clr  # noqa: F401
        from System.Runtime.InteropServices import RuntimeInformation  # pyright: ignore[reportMissingModuleSource]

        result = str(RuntimeInformation.RuntimeIdentifier)
        self.app.display_debug(f"Dotnet RuntimeIdentifier: {result}")
        return result

    def _run_shell(self, command: str) -> None:
        self.app.display_debug(f"Run command '{command}'")
        try:
            subprocess.run(command, check=True, shell=True, capture_output=self.app.verbosity == 0)
        except subprocess.CalledProcessError as e:
            if self.app.verbosity == 0:
                if e.stdout:
                    print(e.stdout.decode())
                if e.stderr:
                    print(e.stderr.decode(), file=sys.stderr)
            raise

    def initialize(self, version: str, build_data: Dict[str, Any]) -> None:
        rm(self.BASE_OUTPUT_DIR)
        rm(self.BASE_PROVIDERS_DIR)

        self._run_shell("dotnet restore")

        self._run_shell(
            "dotnet publish -c Release -f net8.0 -p:DebugSymbols=false -P:DebugType=None "
            f"-o {self.BASE_OUTPUT_DIR} "
            f"-r {self._dotnet_runtime_identifier} ./src/PlatynUI.Spy"
        )
        self._run_shell(
            "dotnet publish -c Release -f net8.0 -p:DebugSymbols=false -P:DebugType=None "
            f"-o {self.BASE_PROVIDERS_DIR / 'avalonia'} "
            f"-r {self._dotnet_runtime_identifier} ./src/PlatynUI.Provider.Avalonia"
        )

        self._run_shell("dotnet clean -c Release")

        build_data["infer_tag"] = True
        build_data["pure_python"] = False

    def clean(self, versions: List[str]) -> None:
        rm(self.BASE_OUTPUT_DIR)
        rm(self.BASE_PROVIDERS_DIR)
