global using Console = ExtendedConsole.Console;
global using ExtendedConsole;
using System.Diagnostics;
using Memory;
using Memory.Types;

namespace DialupQuality;

public static class Program
{
    public static Mem mem;

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
        foreach (Process proc in Process.GetProcessesByName("discord"))
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

        Console.Log.WriteLine("SSQuality", "Attempting to find signatures...");

        // 8B ? ? ? ? ? 8B ? ? ? ? ? 83 ? ? 8B ? ? 89 ? ? ? ? F2 0? // Resolution
        // 8B ? ? 8B ? ? 39 ? 0F 4? ? 89 ? ? 89 ? ? 80 // FPS
        mem = new Mem();
        if (dcProc != null) mem.OpenProcess(dcProc.Id);
        else
        {
            Console.Log.WriteLine("SSQuality", "Failed to find Discord Voice node.");
        }

        Imps.Handle = (nuint)mem.MProc.Handle;
        //dcProc.ResetModMemory(dcMod?.ModuleName);
        string resSig = "8B 8? ? ? ? ? 8B ? ? ? ? ? 83 E? ? 8B";
        byte[] resBytes = { 0x90, 0x90, 0x90, 0x90, 0x90, 0x8B, 0x05, 0x17, 0x00, 0x07, 0x0D, 0x89, 0x86, 0x94, 0x00, 0x00, 0x00, 0x8B, 0x86, 0x94, 0x00, 0x00, 0x00 };
        int resReplaceCount = 0x6;
        string fpsSig = "8B ? ? 8B ? ? 39 ? 0F 4? ? 89 ? ? 89 ? ? 80";
        byte[] fpsBytes = { 0x90, 0x90, 0x90, 0x90, 0x90, 0x8B, 0x3D, 0x14, 0x00, 0xEE, 0x0C, 0x89, 0x79, 0x08, 0x8B, 0x79, 0x08, 0x8B, 0x55, 0xE4 };
        int fpsReplaceCount = 0x6; 
        nuint resAddr = mem.ScanForSig(resSig, module: dcMod?.ModuleName).FirstOrDefault();
        Console.Log.WriteLine("SSQuality", $"Found resolution address: 0x{resAddr:X}");
        nuint fpsAddr = mem.ScanForSig(fpsSig, module: dcMod?.ModuleName).FirstOrDefault();
        Console.Log.WriteLine("SSQuality", $"Found fps address: 0x{fpsAddr:X}");
        byte[] resOrig = mem.ReadArrayMemory<byte>($"{resAddr:X}", resReplaceCount);
        byte[] fpsOrig = mem.ReadArrayMemory<byte>($"{fpsAddr:X}", fpsReplaceCount);
        nuint fpsVarOffset = (nuint)fpsBytes.Length + 5;
        nuint resVarOffset = (nuint)resBytes.Length + 5; // The plus 5 here is to account for the jmp instruction
        nuint fpsVars = 0;
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
        else Console.Log.WriteLine("SSQuality", "Failed to find resolution address.");
        if (fpsAddr != 0)
        {
            Detours.FpsDetour = new($"{fpsAddr:X}", fpsOrig, fpsBytes, fpsReplaceCount, Mem.DetourType.Jump, m: mem, mutate: alloc =>
            {
                alloc += fpsVarOffset;
                fpsVars = alloc;
                mem.WriteAnyMemory(alloc, 60);
            });
            Detours.FpsDetour.Hook();
        }
        else Console.Log.WriteLine("SSQuality", "Failed to find fps address.");
        External<int> fpsVar = new(fpsVars, m: mem);
        External<int> resVar = new(resVars, m: mem);
        int res = resVar.Value;
        int fps = fpsVar.Value;
        Console.KeyOutput[] outputs = {
            new Console.KeyOutput(ConsoleKey.D1, "Resolution"),
            new Console.KeyOutput(ConsoleKey.D2, "FPS"),
            new Console.KeyOutput(ConsoleKey.D3, "Exit")
        }; 
        Thread.Sleep(3000);
        while (true)
        {
            Console.SwitchToAlternativeBuffer();
            Console.WriteLine("[ResMenu]");
            Console.Write($"1) Resolution: {res}");
            IntVec2 resTextPos = new (Console.CursorLeft - res.ToString().Length, Console.CursorTop);
            Console.WriteLine();
            Console.Write($"2) FPS: {fps}");
            IntVec2 fpsTextPos = new(Console.CursorLeft - fps.ToString().Length, Console.CursorTop);
            Console.WriteLine();
            Console.WriteLine("3) Exit");
            ConsoleKey key = Console.ReadKey("SSQuality", "Select an option: ", outputs);
            switch (key)
            {
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
                    Console.CursorLeft = fpsTextPos.X;
                    Console.CursorTop = fpsTextPos.Y;
                    Console.Write(" ".Repeat(fps.ToString().Length));
                    Console.CursorLeft = fpsTextPos.X;
                    Console.CursorTop = fpsTextPos.Y;
                    string newFps = Console.ReadLine();
                    if (newFps != null)
                    {
                        if (int.TryParse(newFps, out int fpsInt))
                        {
                            fps = fpsInt;
                            fpsVar.Value = fps;
                        }
                        else Console.Log.WriteLine("SSQuality", "Failed to parse new FPS.");
                    }

                    break;
                case ConsoleKey.D3:
                    Console.Log.WriteLine("SSQuality", "Exiting...");
                    goto exit;
            }
        }
        exit:
        if (Detours.ResDetour != null && Detours.ResDetour.IsHooked)
        {
            Detours.ResDetour.Unhook();
            Console.Log.WriteLine("SSQuality", "Unhooked resolution.");
        }
        if (Detours.FpsDetour != null && Detours.FpsDetour.IsHooked)
        {
            Detours.FpsDetour.Unhook();
            Console.Log.WriteLine("SSQuality", "Unhooked FPS.");
        }
        Thread.Sleep(2500);
        Console.SwitchToMainBuffer();
        Thread.Sleep(2500);
    }
}