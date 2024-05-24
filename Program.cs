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

    public static string PickProcess()
    {
        var discordProcs = Process.GetProcessesByName("discord");
        var discordPtbProcs = Process.GetProcessesByName("discordptb");
        var discordCanaryProcs = Process.GetProcessesByName("discordcanary");
        var discordDevProcs = Process.GetProcessesByName("discorddevelopment");
        
        bool showDiscordStable = discordProcs.Length > 0;
        bool showDiscordPtb = discordPtbProcs.Length > 0;
        bool showDiscordCanary = discordCanaryProcs.Length > 0;
        bool showDiscordDev = discordDevProcs.Length > 0;
        
        Console.KeyOutput[] outputs = {
            new Console.KeyOutput(ConsoleKey.D1, showDiscordStable ? "Discord" : "Discord (Not Running)"),
            new Console.KeyOutput(ConsoleKey.D2, showDiscordPtb ? "Discord PTB" : "Discord PTB (Not Running)"),
            new Console.KeyOutput(ConsoleKey.D3, showDiscordCanary ? "Discord Canary" : "Discord Canary (Not Running)"),
            new Console.KeyOutput(ConsoleKey.D4, showDiscordDev ? "Discord Development" : "Discord Development (Not Running)")
        };
        
        retry:
        
        Console.SwitchToAlternativeBuffer();
        Console.Clear();
        
        Console.WriteLine("[PickProcess]");
        Console.Write("1) ");
        Console.Write(showDiscordStable ? "Discord" : "Discord (Not Running)", showDiscordStable ? ConsoleColor.White : ConsoleColor.DarkGray);
        Console.WriteLine();
        Console.Write("2) ");
        Console.Write(showDiscordPtb ? "Discord PTB" : "Discord PTB (Not Running)", showDiscordPtb ? ConsoleColor.White : ConsoleColor.DarkGray);
        Console.WriteLine();
        Console.Write("3) ");
        Console.Write(showDiscordCanary ? "Discord Canary" : "Discord Canary (Not Running)", showDiscordCanary ? ConsoleColor.White : ConsoleColor.DarkGray);
        Console.WriteLine();
        Console.Write("4) ");
        Console.Write(showDiscordDev ? "Discord Development" : "Discord Development (Not Running)", showDiscordDev ? ConsoleColor.White : ConsoleColor.DarkGray);
        Console.WriteLine();
        
        ConsoleKey key = Console.ReadKey("SSQuality", "Select a process: ", outputs);
        
        string result = "";
        
        switch (key)
        {
            default:
                Console.Log.WriteLine("SSQuality", "Invalid key.");
                Thread.Sleep(1000);
                goto retry;
                break;
            case ConsoleKey.D1:
                result = "discord";
                break;
            case ConsoleKey.D2:
                result = "discordptb";
                break;
            case ConsoleKey.D3:
                result = "discordcanary";
                break;
            case ConsoleKey.D4:
                result = "discorddevelopment";
                break;
        }
        
        Console.SwitchToMainBuffer();
        
        return result;

    }
    
    public static void RealMain()
    {
        Console.Config.SetupConsole();
        Console.Log.WriteLine("SSQuality", "Attempting to find Discord Voice node...");
        Process dcProc = null;
        ProcessModule dcMod = null;
        string process = PickProcess();
        if (process == "")
        {
            Console.Log.WriteLine("SSQuality", "Failed to find Discord process.");
            Console.WaitForEnter("Press enter to exit...");
            return;
        }
        
        // check if the process is running first, if not let's wait for it to start
        bool msgShown = false;
        while (Process.GetProcessesByName(process).Length == 0)
        {
            if (!msgShown)
            {
                Console.Log.WriteLine("SSQuality", "Waiting for " + process + " to start...");
                msgShown = true;
            }
            Thread.Sleep(1000);
        }
        
        foreach (Process proc in Process.GetProcessesByName(process))
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
        string bitrateSig = "8B 8F ? ? ? ? 81 F9 ? ? ? ? 74 ? 83 F9 ? 75 ? 44 8B";
        byte[] resBytes = new byte[]
        {
            0x48, 0x8B, 0x15, 0x13, 0x00, 0x00, 0x00, 0x48,
            0x89, 0x96, 0xF8, 0x00, 0x00, 0x00, 0x48, 0x8B,
            0x96, 0xF8, 0x00, 0x00, 0x00
        };
        byte[] bitrateBytes = new byte[]
        {
            0x48, 0x8B, 0x0D, 0x12, 0x00, 0x00, 0x00, 0x48,
            0x89, 0x8F, 0xA4, 0x00, 0x00, 0x00, 0x8B, 0x8F,
            0xA4, 0x00, 0x00, 0x00
        };
        
        int resReplaceCount = 0x7;
        nuint resAddr = mem.ScanForSig(resSig, module: dcMod?.ModuleName).FirstOrDefault();
        if (resAddr == 0)
        {
            Console.Log.WriteLine("SSQuality", "Failed to find resolution address.", LogLevel.Critical);
            Console.WaitForEnter("Press enter to exit...");
            return;
        }
        Console.Log.WriteLine("SSQuality", $"Found resolution address: &v0x{resAddr:X}");
        
        int bitrateReplaceCount = 0x6;
        nuint bitrateAddr = mem.ScanForSig(bitrateSig, module: dcMod?.ModuleName).FirstOrDefault();
        if (bitrateAddr == 0)
        {
            Console.Log.WriteLine("SSQuality", "Failed to find bitrate address.", LogLevel.Critical);
            Console.WaitForEnter("Press enter to exit...");
            return;
        }
        Console.Log.WriteLine("SSQuality", $"Found bitrate address: &v0x{bitrateAddr:X}");
        
        
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
        
        Console.Log.WriteLine("SSQuality", "&6Successfully hooked resolution.");
        
        Console.Log.WriteLine("SSQuality", "&vAttempting to hook bitrate...");
        
        byte[] bitrateOrig = mem.ReadArrayMemory<byte>($"{bitrateAddr:X}", bitrateReplaceCount);
        nuint bitrateVarOffset = (nuint)bitrateBytes.Length + 5; // The plus 5 here is to account for the jmp instruction
        nuint bitrateVars = 0;
        if (bitrateAddr != 0)
        {
            Detours.BitrateDetour = new($"{bitrateAddr:X}", bitrateOrig, bitrateBytes, bitrateReplaceCount, Mem.DetourType.Jump, m: mem, mutate: alloc =>
            {
                alloc += bitrateVarOffset;
                bitrateVars = alloc;
                mem.WriteAnyMemory(alloc, 96000);
                
            });
            Detours.BitrateDetour.Hook();
        }
        else Console.Log.WriteLine("SSQuality", "&cFailed to find bitrate address.");
        
        External<int> bitrateVar = new(bitrateVars, m: mem);
        
        Console.Log.WriteLine("SSQuality", "&6Successfully hooked bitrate.");
        
        int bitrate = bitrateVar.Value;
        
        
        Console.KeyOutput[] outputs = {
            new Console.KeyOutput(ConsoleKey.D1, "Resolution"),
            new Console.KeyOutput(ConsoleKey.D2, "Voice Bitrate"),
            new Console.KeyOutput(ConsoleKey.D3, "Exit")
        }; 
        
        // Add a process exit hook to unhook the resolution
        AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
        {
            try
            {
                // Write the original bytes back to the address
                Console.WriteLine();
                Console.Log.WriteLine("SSQuality", $"&bWriting original bytes back to &v{resAddr:X}&b...");
                mem.WriteArrayMemory($"{resAddr:X}", resOrig);
                Console.Log.WriteLine("SSQuality", "&vUnhooking resolution...");
                Detours.ResDetour.Unhook();
                Console.Log.WriteLine("SSQuality", "&6Successfully unhooked resolution.");
                
                Console.Log.WriteLine("SSQuality", $"&bWriting original bytes back to &v{bitrateAddr:X}&b...");
                mem.WriteArrayMemory($"{bitrateAddr:X}", bitrateOrig);
                Console.Log.WriteLine("SSQuality", "&vUnhooking bitrate...");
                Detours.BitrateDetour.Unhook();
                Console.Log.WriteLine("SSQuality", "&6Successfully unhooked bitrate.");
                
                Thread.Sleep(250);
                
            } catch (Exception ex)
            {
                Console.Log.WriteLine("SSQuality", $"Failed to unhook resolution: {ex}", LogLevel.Critical);
            }
        };
        
        Thread.Sleep(1000);
        while (true)
        {
            Console.SwitchToAlternativeBuffer();
            Console.Clear();
            Console.WriteLine("[ResMenu]");
            Console.Write($"1) Resolution: {res}");
            IntVec2 resTextPos = new (Console.CursorLeft - res.ToString().Length, Console.CursorTop);
            Console.WriteLine();
            Console.Write($"2) Voice Bitrate: {bitrate}");
            IntVec2 bitrateTextPos = new (Console.CursorLeft - bitrate.ToString().Length, Console.CursorTop);
            Console.WriteLine();
            Console.WriteLine("3) Exit");
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
                    Console.CursorLeft = bitrateTextPos.X;
                    Console.CursorTop = bitrateTextPos.Y;
                    Console.Write(" ".Repeat(bitrate.ToString().Length));
                    Console.CursorLeft = bitrateTextPos.X;
                    Console.CursorTop = bitrateTextPos.Y;
                    string newBitrate = Console.ReadLine();
                    if (newBitrate != null)
                    {
                        if (int.TryParse(newBitrate, out int bitrateInt))
                        {
                            bitrate = bitrateInt;
                            bitrateVar.Value = bitrate;
                        }
                        else Console.Log.WriteLine("SSQuality", "Failed to parse new bitrate.");
                    }
                    break;
                case ConsoleKey.D3:
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
            
            Console.Log.WriteLine("SSQuality", $"&bWriting original bytes back to &v{bitrateAddr:X}&b...");
            mem.WriteArrayMemory($"{bitrateAddr:X}", bitrateOrig);
            Console.Log.WriteLine("SSQuality", "&vUnhooking bitrate...");
            Detours.BitrateDetour.Unhook();
            Console.Log.WriteLine("SSQuality", "&6Successfully unhooked bitrate.");
        } catch (Exception ex)
        {
            Console.Log.WriteLine("SSQuality", $"Failed to unhook resolution: {ex}", LogLevel.Critical);
        }

        Console.Log.WriteLine("SSQuality", "&cExiting!");
        Thread.Sleep(1000);
        Console.SwitchToMainBuffer();
        Process.GetCurrentProcess().Kill();
    }
}