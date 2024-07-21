// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PlatynUI.Runtime;

public enum Platform
{
    Any,
    Windows,
    Linux,
    FreeBSD,
    MacOS,
    Android,
    IOS,
    Unknown
}

public static class PlatformHelper
{
    public static Platform GetCurrentPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Platform.Windows;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return Platform.Linux;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            return Platform.FreeBSD;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return Platform.MacOS;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID")))
        {
            return Platform.Android;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("IOS")))
        {
            return Platform.IOS;
        }
        else
        {
            return Platform.Unknown;
        }
    }
}

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class PlatynUiExtensionAttribute(Platform[] supportedPlatforms) : Attribute
{
    public Platform[] SupportedPlatforms { get; } = supportedPlatforms;
}

static class PlatynUiExtensions
{
    public static bool IsValidPlatyUiPlatformExtension(this Assembly assembly)
    {
        var platformAttribute = assembly.GetCustomAttribute<PlatynUiExtensionAttribute>();
        if (platformAttribute != null)
        {
            if (platformAttribute.SupportedPlatforms.Contains(Platform.Any))
            {
                return true;
            }
            var currentPlatform = PlatformHelper.GetCurrentPlatform();
            return platformAttribute.SupportedPlatforms.Contains(currentPlatform);
        }
        return false;
    }

    public static AggregateCatalog GetPlatynUIExtensionCatalog()
    {
        var catalog = new AggregateCatalog();
        var addedAssemblies = new List<AssemblyName>();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly == null)
            {
                continue;
            }

            if (
                (assembly.GetName()?.Name?.StartsWith("PlatynUI.") ?? false)
                && assembly.IsValidPlatyUiPlatformExtension()
            )
            {
                addedAssemblies.Add(assembly.GetName());
                catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            }
        }

        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

        foreach (var dll in Directory.GetFiles(currentDirectory, "PlatynUI.*.dll"))
        {
            try
            {
                var assemblyName = AssemblyName.GetAssemblyName(dll);
                if (addedAssemblies.Contains(assemblyName))
                {
                    continue;
                }
                var assembly = Assembly.LoadFrom(dll);
                if (assembly.IsValidPlatyUiPlatformExtension())
                {
                    catalog.Catalogs.Add(new AssemblyCatalog(assembly));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load assembly {dll}: {ex.Message}");
            }
        }

        return catalog;
    }
}
