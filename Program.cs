global using Console = ExtendedConsole.Console;
global using ExtendedConsole;
using System.Diagnostics;
using Memory;
using Memory.Types;

namespace DialupQuality;

public static class Program
{
    //public static Mem mem;

    public static void Main()
    {
        try
        {
            RealMain();
        } catch (Exception ex)
        {
            Console.Log.WriteLine("SSQuality", $"Exception: {ex}", LogLevel.Critical);
            Console.WaitForEnter("Press enter to exit...");
        }
    }
    public static void RealMain()
    {
        Console.Config.SetupConsole();
        Console.Log.WriteLine("SSQuality", "Attempting to find Discord Voice node...");
        Process dcProc = null;
        ProcessModule dcMod = null;
        foreach (Process proc in Process.GetProcessesByName("discorddevelopment"))
        {
            //Console.Log.WriteLine("SSQuality", $"{proc.ProcessName} | {proc.Id} (0x{proc.Id:X}) | {proc.MainModule?.FileName} ");
            foreach (ProcessModule mod in proc.Modules)
            {
                if (mod.ModuleName.Contains("voice.node"))
                {
                    Console.Log.WriteLine("SSQuality", $"Found Discord Voice node: {mod.ModuleName} | {mod.FileName} | PID 0x{proc.Id:X}/{proc.Id}");
                    dcProc = proc;
                    dcMod = mod;
                    break;
                }
            }
        }
        
        Console.Log.WriteLine("SSQuality", "Attempting to find signature...");

        var mem = new Mem();
        if (dcProc != null) mem.OpenProcess(dcProc.Id);
        else
        {
            Console.Log.WriteLine("SSQuality", "Failed to find Discord Voice node.");
        }

        Imps.Handle = (nuint)mem.MProc.Handle;
        //dcProc.ResetModMemory(dcMod?.ModuleName);
        string resSig = "48 8B ? ? ? ? ? 4C 8B ? ? ? ? ? 48 8D ? ? ? 4C 8D";
        byte[] resBytes = new byte[]
        {
            0x48, 0x8B, 0x15, 0x13, 0x00, 0x00, 0x00, 0x48,
            0x89, 0x96, 0xF8, 0x00, 0x00, 0x00, 0x48, 0x8B,
            0x96, 0xF8, 0x00, 0x00, 0x00
        };
        int resReplaceCount = 0x7;
        nuint resAddr = mem.ScanForSig(resSig, module: dcMod?.ModuleName).FirstOrDefault();
        if (resAddr == 0)
        {
            Console.Log.WriteLine("SSQuality", "Failed to find resolution address.", LogLevel.Critical);
            Console.WaitForEnter("Press enter to exit...");
            return;
        }
        Console.Log.WriteLine("SSQuality", $"Found resolution address: 0x{resAddr:X}");
        
        
        Console.Log.WriteLine("SSQuality", "&vAttempting to hook resolution...");

        byte[] resOrig = mem.ReadArrayMemory<byte>($"{resAddr:X}", resReplaceCount);
        nuint resVarOffset = (nuint)resBytes.Length + 5; // The plus 5 here is to account for the jmp instruction
        nuint resVars = 0;
        if (resAddr != 0)
        {
            Detours.ResDetour = new($"{resAddr:X}", resOrig, resBytes, resReplaceCount, Mem.DetourType.Jump, m: mem, mutate: alloc =>
            {
                alloc += resVarOffset;
                resVars = alloc;
                mem.WriteAnyMemory(alloc, 1920);
                
            });
            Detours.ResDetour.Hook();
        }
        else Console.Log.WriteLine("SSQuality", "&cFailed to find resolution address.");
       
        External<int> resVar = new(resVars, m: mem);
        
        int res = resVar.Value;
        
        Console.KeyOutput[] outputs = {
            new Console.KeyOutput(ConsoleKey.D1, "Resolution"),
            new Console.KeyOutput(ConsoleKey.D2, "Exit")
        }; 
        
        Console.Log.WriteLine("SSQuality", "&6Successfully hooked resolution.");
        
        Thread.Sleep(1000);
        while (true)
        {
            Console.SwitchToAlternativeBuffer();
            Console.Clear();
            Console.WriteLine("[ResMenu]");
            Console.Write($"1) Resolution: {res}");
            IntVec2 resTextPos = new (Console.CursorLeft - res.ToString().Length, Console.CursorTop);
            Console.WriteLine();
            Console.WriteLine("2) Exit");
            ConsoleKey key = Console.ReadKey("SSQuality", "Select an option: ", outputs);
            switch (key)
            {
                default:
                    Console.Log.WriteLine("SSQuality", "Invalid key.");
                    break;
                case ConsoleKey.D1:
                    Console.CursorLeft = resTextPos.X;
                    Console.CursorTop = resTextPos.Y;
                    Console.Write(" ".Repeat(res.ToString().Length));
                    Console.CursorLeft = resTextPos.X;
                    Console.CursorTop = resTextPos.Y;
                    string newRes = Console.ReadLine();
                    if (newRes != null)
                    {
                        if (int.TryParse(newRes, out int resInt))
                        {
                            res = resInt;
                            resVar.Value = res;
                        }
                        else Console.Log.WriteLine("SSQuality", "Failed to parse new resolution.");
                    }

                    break;
                case ConsoleKey.D2:
                    Console.Log.WriteLine("SSQuality", "Exiting...");
                    goto exit;
            }
        }
        exit:
        try
        {
            // Write the original bytes back to the address
            Console.Log.WriteLine("SSQuality", $"&bWriting original bytes back to &v{resAddr:X}&b...");
            mem.WriteArrayMemory($"{resAddr:X}", resOrig);
            Console.Log.WriteLine("SSQuality", "&vUnhooking resolution...");
            Detours.ResDetour.Unhook();
            Console.Log.WriteLine("SSQuality", "&6Successfully unhooked resolution.");
        } catch (Exception ex)
        {
            Console.Log.WriteLine("SSQuality", $"Failed to unhook resolution: {ex}", LogLevel.Critical);
        }

        Console.Log.WriteLine("SSQuality", "&cExiting!");
        Thread.Sleep(1000);
        Console.SwitchToMainBuffer();
    }
}