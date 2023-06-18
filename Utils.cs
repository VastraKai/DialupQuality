using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

namespace DialupQuality;

public static class Utils
{
    /// <summary>
    /// Returns the base address and end address of the requested module.
    /// </summary>
    /// <param name="moduleName"></param>
    /// <returns>MinMax</returns>
    public static ProcessModule GetModule(this Process proc, string moduleName)
    {
        ProcessModule result = null;
        if (proc?.MainModule == null) return null;
        ProcessModuleCollection mods = proc.Modules;
        foreach (ProcessModule mod in mods)
        {
            if (Path.GetFileName(mod.FileName).Trim().ToLower() != moduleName.Trim().ToLower()) continue;
            result = mod;
            break;
        }
        return result;
    }
        // Compares every byte in the module's memory to the original bytes (In the original module file) and resets the bytes that have been changed.
    private static nint _processHandle = 0;

    public static SectionHeader GetSection(string filename, string section = ".text")
    {
        SectionHeader result = new SectionHeader();
        Stream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        BinaryReader br = new BinaryReader(fs);
        PEHeaders headers = new PEHeaders(fs);
        foreach (SectionHeader sectionHeader in headers.SectionHeaders)
        {
            if (sectionHeader.Name == section)
            {
                result = sectionHeader;
                break;
            }
        }
        return result;
    }

    public static void ResetModMemory(this Process proc, string module, bool output = true)
    {
        try
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ProcessModule mod = GetModule(proc, module);
            if(_processHandle == 0)
                _processHandle = Imps.OpenProcess((uint)AccessFlags.ProcessAllAccess, true, proc.Id);
            SectionHeader section = GetSection(mod.FileName);
            nuint modBase = (nuint)mod.BaseAddress + (nuint)section.VirtualAddress;
            nint modSize = (nint)section.SizeOfRawData;
            byte[] modBytes = new byte[modSize];
            byte[] origFileBytes = new byte[modSize];
            // Read the module from the file
            using (FileStream fs = new FileStream(mod.FileName, FileMode.Open, FileAccess.Read))
            {
                fs.Seek((long)section.PointerToRawData, SeekOrigin.Begin);
                nuint bytesRead = (nuint)fs.Read(origFileBytes, 0, (int)modSize);
                // Check if the bytes read is equal to the size of the module
                if (bytesRead != (nuint)modSize)
                {
                    sw.Stop();
                    Console.Log.WriteLine("Utils", $"&cError reading module &v{module}&c from file&r", LogLevel.Error);
                    return;
                }
            }
            proc.Suspend();
            Imps.VirtualProtectEx(_processHandle, modBase, modSize, MemoryProtection.ExecuteReadWrite, out _);
            Imps.WriteProcessMemory((nuint)_processHandle, modBase, origFileBytes, (nuint)modSize, out _);
            Imps.VirtualProtectEx(_processHandle, modBase, modSize, MemoryProtection.ExecuteRead, out _);
            proc.Resume();
            sw.Stop();
            if(output) Console.Log.WriteLine("Utils", $"Memory for module &v{module}&a was reset in &v{sw.ElapsedMilliseconds}ms&r");
        } catch (Exception e)
        {
            Console.Log.WriteLine("Utils", $"&cError resetting memory for module &v{module}&r: &c{e}", LogLevel.Error);
        }
    }

    public static void ResetAllModules(this Process proc)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        foreach (ProcessModule mod in proc.Modules)
        {
            ResetModMemory(proc, mod.ModuleName);
        }
        sw.Stop();
        Console.Log.WriteLine("Utils", $"&aSuccessfully reset memory for all modules in &v{sw.ElapsedMilliseconds}ms");
    }

    private static bool IsMemoryGood(nuint address)
    {
        MEMORY_BASIC_INFORMATION mbi = new();
        Imps.VirtualQueryEx(_processHandle, address, out mbi, (nuint)Marshal.SizeOf(mbi));
        // Check if the memory is executable and readable
        if ((mbi.Protect & (uint)Imps.ExecuteRead) == (uint)Imps.ExecuteRead)
        {
            return true;
        }
        return false;
    }
}