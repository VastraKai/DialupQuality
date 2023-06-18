using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable InconsistentNaming

namespace DialupQuality;

public static class Imps
{
    public static nuint Handle = 0;
    [DllImport("Kernel32.dll", SetLastError=true, ExactSpelling=true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CheckRemoteDebuggerPresent(nuint hProcess, out bool isDebuggerPresent);
    public static T Read<T>(nuint address) where T : unmanaged
    {
        var size = Marshal.SizeOf<T>();
        var buffer = new byte[size];
        ReadProcessMemory(Handle, address, buffer, (nuint)size, out _);
        return MemoryMarshal.Read<T>(buffer);
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool ReadProcessMemory(nuint hProcess, nuint lpBaseAddress, [Out] byte[] lpBuffer, nuint dwSize, out nuint lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteProcessMemory(nuint hProcess, nuint lpBaseAddress, byte[] lpBuffer, nuint dwSize, out nuint lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(nuint hObject);


    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern nuint VirtualAllocEx(nuint hProcess, nuint lpAddress, nuint dwSize, nuint flAllocationType, nuint flProtect);

    [DllImport("user32.dll")]
    public static extern nuint FindWindow(string lpClassName, string lpWindowName);


    
    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(nuint hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern bool GetClientRect(nuint hWnd, out RECT lpRect);
    
    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int nIndex);


    [DllImport("kernel32.dll")]
    public static extern nint OpenProcess(
        uint dwDesiredAccess,
        [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
        int dwProcessId
    );
    

    [DllImport("ntdll.dll", SetLastError = true)]
    public static extern int NtQueryInformationThread(nint threadHandle,ThreadInfoClass threadInformationClass,nint threadInformation,int threadInformationLength, nint returnLengthPtr);


    [DllImport("kernel32.dll")]
    public static extern nint OpenThread(ThreadAccess dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwThreadId);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint SuspendThread(nint hThread);

    [DllImport("kernel32.dll")]
    internal static extern int ResumeThread(nint hThread);


    [DllImport("kernel32.dll")]
    public static extern UIntPtr VirtualQueryEx(IntPtr hProcess, UIntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, UIntPtr dwLength);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool VirtualFreeEx(
        nint hProcess,
        nuint lpAddress,
        nuint dwSize,
        uint dwFreeType
    );

    [DllImport("kernel32.dll")]
    public static extern bool VirtualProtectEx(nint hProcess, nuint lpAddress, nint dwSize, MemoryProtection flNewProtect, out MemoryProtection lpflOldProtect);
    [DllImport("kernel32.dll")]
    internal static extern int CloseHandle(
        nint hObject
    );

    [DllImport("kernel32")]
    public static extern nint CreateRemoteThread(nint hProcess, nint lpThreadAttributes, uint dwStackSize, nuint lpStartAddress, nuint lpParameter, ThreadCreationFlags dwCreationFlags, out nint lpThreadId
    );
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetThreadContext(nuint hThread, ref ThreadContext threadContext);

    [DllImport("ntdll.dll", SetLastError=true, ExactSpelling=true)]
    public static extern bool NtQueryInformationProcess(nuint hProcess, int processInformationClass, out ProcessBasicInformation processInformation, int processInformationLength, out int returnLength);
    
    [DllImport("ntdll.dll", SetLastError=true, ExactSpelling=true)]
    public static extern bool NtQueryInformationThread(nuint hThread, int threadInformationClass, out ThreadBasicInformation threadInformation, int threadInformationLength, out int returnLength);
    
    [DllImport("kernel32.dll", SetLastError=true, ExactSpelling=true)]
    public static extern bool IsDebuggerPresent();
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern nuint CreateRemoteThread(nuint hProcess, nuint lpThreadAttributes, nuint dwStackSize, nuint lpStartAddress, nuint lpParameter, nuint dwCreationFlags, nuint lpThreadId);
    
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern nuint GetProcAddress(nuint hModule, string lpProcName);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern nuint LoadLibrary(string lpFileName);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern nuint WaitForSingleObject(nuint hHandle, nuint dwMilliseconds);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetExitCodeThread(nuint hThread, out nuint lpExitCode);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool TerminateThread(nuint hThread, nuint dwExitCode);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool DebugActiveProcess(nuint dwProcessId);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool DebugActiveProcessStop(nuint dwProcessId);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool ContinueDebugEvent(uint dwProcessId, uint dwThreadId, uint dwContinueStatus);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool DebugSetProcessKillOnExit(bool KillOnExit);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool DebugBreakProcess(nuint Process);
    
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool VirtualProtectEx(nuint hProcess, nuint lpAddress, nuint dwSize, nuint flNewProtect, out nuint lpflOldProtect);
    
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern nuint OpenProcess(nuint dwDesiredAccess, bool bInheritHandle, nuint dwProcessId);
    [DllImport("user32.dll")]
    public static extern nuint GetWindowThreadProcessId(nuint hWnd, out nuint lpdwProcessId);

    [DllImport("user32.dll")]
    public static extern nuint GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(nuint hWnd);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(nuint hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    public static extern nuint GetWindowLongPtr(nuint hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern bool IsIconic(nuint hWnd);

    [DllImport("user32.dll")]
    public static extern bool IsZoomed(nuint hWnd);

    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(nuint hWnd);

    [DllImport("user32.dll")]
    public static extern bool IsWindow(nuint hWnd);

    [DllImport("user32.dll")]
    public static extern bool IsWindowEnabled(nuint hWnd);

    [DllImport("user32.dll")]
    public static extern bool IsHungAppWindow(nuint hWnd);

    [DllImport("user32.dll")]
    public static extern bool IsGUIThread(bool bConvert);

    [DllImport("user32.dll")]
    public static extern bool IsChild(nuint hWndParent, nuint hWnd);

    [DllImport("user32.dll")]
    public static extern bool IsClipboardFormatAvailable(uint format);

    [DllImport("user32.dll")]
    public static extern bool InvalidateRect(nuint hWnd, nuint lpRect, bool bErase);

    [DllImport("user32.dll")]
    public static extern bool InvalidateRgn(nuint hWnd, nuint hRgn, bool bErase);
    
    [DllImport("user32.dll")]
    public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);


    [DllImport("user32.dll")]
    public static extern bool GetWindowPlacement(nuint hWnd, out WINDOWPLACEMENT lpwndpl);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPlacement(nuint hWnd, out WINDOWPLACEMENT lpwndpl);

    [DllImport("user32.dll")]
    public static extern bool GetWindowInfo(nuint hWnd, out WINDOWINFO pwi);

    [DllImport("user32.dll")]
    public static extern bool GetTitleBarInfo(nuint hWnd, out TITLEBARINFO pti);
    
    [DllImport("user32.dll")]
    public static extern nuint SendMessage(nuint hWnd, uint Msg, nuint wParam, nuint lParam);

    [DllImport("user32.dll")]
    public static extern nuint SendMessage(nuint hWnd, uint Msg, nuint wParam, string lParam);

    [DllImport("user32.dll")]
    public static extern nuint SendMessage(nuint hWnd, uint Msg, nuint wParam, ref RECT lParam);

    [DllImport("user32.dll")]
    public static extern nuint SendMessage(nuint hWnd, uint Msg, nuint wParam, ref POINT lParam);

    [DllImport("user32.dll")]
    public static extern nuint SendMessage(nuint hWnd, uint Msg, nuint wParam, ref TITLEBARINFO lParam);

    [DllImport("user32.dll")]
    public static extern nuint SendMessage(nuint hWnd, uint Msg, nuint wParam, ref TITLEBARINFOEX lParam);

    [DllImport("user32.dll")]
    public static extern nuint SendMessage(nuint hWnd, uint Msg, nuint wParam, ref WINDOWINFO lParam);

    [DllImport("user32.dll")]
    public static extern nuint SendMessage(nuint hWnd, uint Msg, nuint wParam, ref WINDOWPLACEMENT lParam);

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    public static extern bool ClientToScreen(nuint hWnd, out POINT lpPoint);

    [DllImport("user32.dll")]
    public static extern bool ScreenToClient(nuint hWnd, out POINT lpPoint);

    [DllImport("user32.dll")]
    public static extern bool ClipCursor(out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern bool GetClipCursor(out POINT lpRect);

    [DllImport("user32.dll")]
    public static extern bool GetCursorInfo(out CURSORINFO pci);

    [DllImport("user32.dll")]
    public static extern bool GetCursor();

    [DllImport("user32.dll")]
    public static extern bool SetCursor(nuint hCursor);

    [DllImport("kernel32.dll")]
    public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern uint GetPrivateProfileString(
        string lpAppName,
        string lpKeyName,
        string lpDefault,
        StringBuilder lpReturnedString,
        uint nSize,
        string lpFileName);
    
    [DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
    public static extern nuint Native_VirtualQueryEx(nint hProcess, nuint lpAddress, out MEMORY_BASIC_INFORMATION32 lpBuffer, nuint dwLength);

    [DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
    public static extern nuint Native_VirtualQueryEx(nint hProcess, nuint lpAddress, out MEMORY_BASIC_INFORMATION64 lpBuffer, nuint dwLength);
    [DllImport("kernel32.dll", SetLastError = true)] // Method to wait for an external process thread to exit
    public static extern uint WaitForSingleObject(nuint hHandle, uint dwMilliseconds);

    [DllImport("kernel32")]
    public static extern bool IsWow64Process(nint hProcess, [MarshalAs(UnmanagedType.Bool)] out bool lpSystemInfo);

    [DllImport("ntdll.dll", SetLastError = true)]
    internal static extern NTSTATUS NtCreateThreadEx(out nint hProcess, AccessMask desiredAccess, nint objectAttributes, nuint processHandle, nint startAddress, 
        nint parameter, ThreadCreationFlags inCreateSuspended, int stackZeroBits, int sizeOfStack, int maximumStackSize, nint attributeList);

    // used for memory allocation
    public const uint MemFree = 0x10000;
    public const uint MemCommit = 0x00001000;
    public const uint MemReserve = 0x00002000;

    public const uint Readonly = 0x02;
    public const uint Readwrite = 0x04;
    public const uint Writecopy = 0x08;
    public const uint ExecuteReadwrite = 0x40;
    public const uint ExecuteWritecopy = 0x80;
    public const uint Execute = 0x10;
    public const uint ExecuteRead = 0x20;

    public const uint Guard = 0x100;
    public const uint Noaccess = 0x01;

    public const uint MemPrivate = 0x20000;
    public const uint MemImage = 0x1000000;
    public const uint MemMapped = 0x40000;


}
// ContextFlags
public enum ContextFlags : uint
{
    CONTEXT_i386 = 0x10000,
    CONTEXT_i486 = 0x10000,   //  same as i386
    CONTEXT_CONTROL = CONTEXT_i386 | 0x01, // SS:SP, CS:IP, FLAGS, BP
    CONTEXT_INTEGER = CONTEXT_i386 | 0x02, // AX, BX, CX, DX, SI, DI
    CONTEXT_SEGMENTS = CONTEXT_i386 | 0x04, // DS, ES, FS, GS
    CONTEXT_FLOATING_POINT = CONTEXT_i386 | 0x08, // 387 state
    CONTEXT_DEBUG_REGISTERS = CONTEXT_i386 | 0x10, // DB 0-3,6,7
    CONTEXT_EXTENDED_REGISTERS = CONTEXT_i386 | 0x20, // cpu specific extensions
    CONTEXT_FULL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS,
    CONTEXT_ALL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS | CONTEXT_FLOATING_POINT | CONTEXT_DEBUG_REGISTERS | CONTEXT_EXTENDED_REGISTERS
}
// FloatingSaveArea
[StructLayout(LayoutKind.Sequential)]
public struct FloatingSaveArea
{
    public uint ControlWord;
    public uint StatusWord;
    public uint TagWord;
    public uint ErrorOffset;
    public uint ErrorSelector;
    public uint DataOffset;
    public uint DataSelector;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
    public byte[] RegisterArea;
    public uint Cr0NpxState;
}
[StructLayout(LayoutKind.Sequential)]
public struct ThreadContext
{
    public ContextFlags ContextFlags; //set this to an appropriate value 
                                       // Retrieved by CONTEXT_DEBUG_REGISTERS 
    public uint Dr0;
    public uint Dr1;
    public uint Dr2;
    public uint Dr3;
    public uint Dr6;
    public uint Dr7;
    // Retrieved by CONTEXT_FLOATING_POINT 
    public FloatingSaveArea FloatSave;
    // Retrieved by CONTEXT_SEGMENTS 
    public uint SegGs;
    public uint SegFs;
    public uint SegEs;
    public uint SegDs;
    // Retrieved by CONTEXT_INTEGER 
    public uint Edi;
    public uint Esi;
    public uint Ebx;
    public uint Edx;
    public uint Ecx;
    public uint Eax;
    // Retrieved by CONTEXT_CONTROL 
    public uint Ebp;
    public uint Eip;
    public uint SegCs;              // MUST BE SANITIZED 
    public uint EFlags;             // MUST BE SANITIZED 
    public uint Esp;
    public uint SegSs;
    // Retrieved by CONTEXT_EXTENDED_REGISTERS 
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
    public byte[] ExtendedRegisters;
}

// ThreadBasicInformation
[StructLayout(LayoutKind.Sequential)]
public struct ThreadBasicInformation
{
    public int ExitStatus;
    public int TebBaseAddress;
    public int ProcessId;
    public int ThreadId;
    public int AffinityMask;
    public int Priority;
    public int BasePriority;
}

// DebugContinueStatus
public enum DebugContinueStatus : uint
{
    DbgContinue = 0x00010002,
    DbgExceptionNotHandled = 0x80010001,
    DbgReplyLater = 0x40010001,
    DbgExitThread = 0x40010003,
    DbgExitProcess = 0x40010004,
    DbgTerminateThread = 0x40010006,
    DbgTerminateProcess = 0x40010007,
    DbgControlC = 0x40010008,
    DbgRipException = 0x40010009,
    DbgRipContinue = 0x4001000A
}
// ProcessBasicInformation
[StructLayout(LayoutKind.Sequential)]
public struct ProcessBasicInformation
{
    public nint ExitStatus;
    public nint PebBaseAddress;
    public nint AffinityMask;
    public nint BasePriority;
    public nint UniqueProcessId;
    public nint InheritedFromUniqueProcessId;
}
[Flags]
public enum AccessFlags : uint
{
    ProcessTerminate = 0x0001,
    ProcessCreateThread = 0x0002,
    ProcessSetSessionid = 0x0004,
    ProcessVmOperation = 0x0008,
    ProcessVmRead = 0x0010,
    ProcessVmWrite = 0x0020,
    ProcessDupHandle = 0x0040,
    ProcessCreateProcess = 0x0080,
    ProcessSetQuota = 0x0100,
    ProcessSetInformation = 0x0200,
    ProcessQueryInformation = 0x0400,
    ProcessSuspendResume = 0x0800,
    ProcessQueryLimitedInformation = 0x1000,
    ProcessSetLimitedInformation = 0x2000,
    Synchronize = 0x00100000,
    ProcessAllAccess = 0x001F0FFF
}

public enum ProcessInfoClass : int
{
    ProcessBasicInformation             = 0x00,
    ProcessQuotaLimits              = 0x01,
    ProcessIoCounters               = 0x02,
    ProcessVmCounters               = 0x03,
    ProcessTimes                = 0x04,
    ProcessBasePriority             = 0x05,
    ProcessRaisePriority            = 0x06,
    ProcessDebugPort                = 0x07,
    ProcessExceptionPort            = 0x08,
    ProcessAccessToken              = 0x09,
    ProcessLdtInformation               = 0x0A,
    ProcessLdtSize                  = 0x0B,
    ProcessDefaultHardErrorMode         = 0x0C,
    ProcessIoPortHandlers               = 0x0D,
    ProcessPooledUsageAndLimits         = 0x0E,
    ProcessWorkingSetWatch              = 0x0F,
    ProcessUserModeIOPL             = 0x10,
    ProcessEnableAlignmentFaultFixup        = 0x11,
    ProcessPriorityClass            = 0x12,
    ProcessWx86Information              = 0x13,
    ProcessHandleCount              = 0x14,
    ProcessAffinityMask             = 0x15,
    ProcessPriorityBoost            = 0x16,
    ProcessDeviceMap                = 0x17,
    ProcessSessionInformation           = 0x18,
    ProcessForegroundInformation        = 0x19,
    ProcessWow64Information             = 0x1A,
    ProcessImageFileName            = 0x1B,
    ProcessLUIDDeviceMapsEnabled        = 0x1C,
    ProcessBreakOnTermination           = 0x1D,
    ProcessDebugObjectHandle            = 0x1E,
    ProcessDebugFlags               = 0x1F,
    ProcessHandleTracing            = 0x20,
    ProcessIoPriority               = 0x21,
    ProcessExecuteFlags             = 0x22,
    ProcessResourceManagement           = 0x23,
    ProcessCookie                   = 0x24,
    ProcessImageInformation             = 0x25,
    ProcessCycleTime                = 0x26,
    ProcessPagePriority             = 0x27,
    ProcessInstrumentationCallback          = 0x28,
    ProcessThreadStackAllocation        = 0x29,
    ProcessWorkingSetWatchEx            = 0x2A,
    ProcessImageFileNameWin32           = 0x2B,
    ProcessImageFileMapping             = 0x2C,
    ProcessAffinityUpdateMode           = 0x2D,
    ProcessMemoryAllocationMode         = 0x2E,
    ProcessGroupInformation             = 0x2F,
    ProcessTokenVirtualizationEnabled       = 0x30,
    ProcessConsoleHostProcess           = 0x31,
    ProcessWindowInformation            = 0x32,
    ProcessHandleInformation            = 0x33,
    ProcessMitigationPolicy             = 0x34,
    ProcessDynamicFunctionTableInformation      = 0x35,
    ProcessHandleCheckingMode           = 0x36,
    ProcessKeepAliveCount               = 0x37,
    ProcessRevokeFileHandles            = 0x38,
    ProcessWorkingSetControl            = 0x39,
    ProcessHandleTable              = 0x3A,
    ProcessCheckStackExtentsMode        = 0x3B,
    ProcessCommandLineInformation           = 0x3C,
    ProcessProtectionInformation        = 0x3D,
    ProcessMemoryExhaustion             = 0x3E,
    ProcessFaultInformation             = 0x3F,
    ProcessTelemetryIdInformation           = 0x40,
    ProcessCommitReleaseInformation         = 0x41,
    ProcessDefaultCpuSetsInformation        = 0x42,
    ProcessAllowedCpuSetsInformation        = 0x43,
    ProcessSubsystemProcess             = 0x44,
    ProcessJobMemoryInformation         = 0x45,
    ProcessInPrivate                = 0x46,
    ProcessRaiseUMExceptionOnInvalidHandleClose = 0x47,
    ProcessIumChallengeResponse         = 0x48,
    ProcessChildProcessInformation          = 0x49,
    ProcessHighGraphicsPriorityInformation      = 0x4A,
    ProcessSubsystemInformation         = 0x4B,
    ProcessEnergyValues             = 0x4C,
    ProcessActivityThrottleState        = 0x4D,
    ProcessActivityThrottlePolicy           = 0x4E,
    ProcessWin32kSyscallFilterInformation       = 0x4F,
    ProcessDisableSystemAllowedCpuSets      = 0x50,
    ProcessWakeInformation              = 0x51,
    ProcessEnergyTrackingState          = 0x52,
    ProcessManageWritesToExecutableMemory       = 0x53,
    ProcessCaptureTrustletLiveDump          = 0x54,
    ProcessTelemetryCoverage            = 0x55,
    ProcessEnclaveInformation           = 0x56,
    ProcessEnableReadWriteVmLogging         = 0x57,
    ProcessUptimeInformation            = 0x58,
    ProcessImageSection             = 0x59,
    ProcessDebugAuthInformation         = 0x5A,
    ProcessSystemResourceManagement         = 0x5B,
    ProcessSequenceNumber               = 0x5C,
    ProcessLoaderDetour             = 0x5D,
    ProcessSecurityDomainInformation        = 0x5E,
    ProcessCombineSecurityDomainsInformation    = 0x5F,
    ProcessEnableLogging            = 0x60,
    ProcessLeapSecondInformation        = 0x61,
    ProcessFiberShadowStackAllocation       = 0x62,
    ProcessFreeFiberShadowStackAllocation       = 0x63,
    MaxProcessInfoClass             = 0x64
};

public enum ThreadInfoClass
{
    ThreadBasicInformation = 0,
    ThreadTimes = 1,
    ThreadPriority = 2,
    ThreadBasePriority = 3,
    ThreadAffinityMask = 4,
    ThreadImpersonationToken = 5,
    ThreadDescriptorTableEntry = 6,
    ThreadEnableAlignmentFaultFixup = 7,
    ThreadEventPair_Reusable = 8,
    ThreadQuerySetWin32StartAddress = 9,
    ThreadZeroTlsCell = 10,
    ThreadPerformanceCount = 11,
    ThreadAmILastThread = 12,
    ThreadIdealProcessor = 13,
    ThreadPriorityBoost = 14,
    ThreadSetTlsArrayAddress = 15, // Obsolete
    ThreadIsIoPending = 16,
    ThreadHideFromDebugger = 17,
    ThreadBreakOnTermination = 18,
    ThreadSwitchLegacyState = 19,
    ThreadIsTerminated = 20,
    ThreadLastSystemCall = 21,
    ThreadIoPriority = 22,
    ThreadCycleTime = 23,
    ThreadPagePriority = 24,
    ThreadActualBasePriority = 25,
    ThreadTebInformation = 26,
    ThreadCSwitchMon = 27, // Obsolete
    ThreadCSwitchPmu = 28,
    ThreadWow64Context = 29,
    ThreadGroupInformation = 30,
    ThreadUmsInformation = 31, // UMS
    ThreadCounterProfiling = 32,
    ThreadIdealProcessorEx = 33,
    ThreadCpuAccountingInformation = 34,
    ThreadSuspendCount = 35,
    ThreadHeterogeneousCpuPolicy = 36,
    ThreadContainerId = 37,
    ThreadNameInformation = 38,
    ThreadSelectedCpuSets = 39,
    ThreadSystemThreadInformation = 40,
    ThreadActualGroupAffinity = 41,
    ThreadDynamicCodePolicyInfo = 42,
    ThreadExplicitCaseSensitivity = 43,
    ThreadWorkOnBehalfTicket = 44,
    ThreadSubsystemInformation = 45,
    ThreadDbgkWerReportActive = 46,
    ThreadAttachContainer = 47,
    ThreadManageWritesToExecutableMemory = 48,
    ThreadPowerThrottlingState = 49,
    ThreadWorkloadClass = 50,
    MaxThreadInfoClass = 51
}
public enum NTSTATUS
{
    Success = 0x00
}

public enum AccessMask
{
    SpecificRightsAll = 0xFFFF,
    StandardRightsAll = 0x1F0000
}

public enum ThreadCreationFlags
{
    Immediately = 0x0,
    CreateSuspended = 0x01,
    HideFromDebugger = 0x04,
    StackSizeParamIsAReservation = 0x10000
}

public struct SYSTEM_INFO
{
    public ushort ProcessorArchitecture;
    public ushort _reserved;
    public uint PageSize;
    public nuint MinimumApplicationAddress;
    public nuint MaximumApplicationAddress;
    public nint ActiveProcessorMask;
    public uint NumberOfProcessors;
    public uint ProcessorType;
    public uint AllocationGranularity;
    public ushort ProcessorLevel;
    public ushort ProcessorRevision;
}

public struct MEMORY_BASIC_INFORMATION32
{
    public nuint BaseAddress;
    public nuint AllocationBase;
    public uint AllocationProtect;
    public uint RegionSize;
    public uint State;
    public uint Protect;
    public uint Type;
}

public struct MEMORY_BASIC_INFORMATION64
{
    public nuint BaseAddress;
    public nuint AllocationBase;
    public uint AllocationProtect;
    public uint Alignment1;
    public ulong RegionSize;
    public uint State;
    public uint Protect;
    public uint Type;
    public uint Alignment2;
}

public struct MEMORY_BASIC_INFORMATION
{
    public nuint BaseAddress;
    public nuint AllocationBase;
    public uint AllocationProtect;
    public long RegionSize;
    public uint State;
    public uint Protect;
    public uint Type;
}

public enum SnapshotFlags : uint
{
    HeapList = 0x00000001,
    Process = 0x00000002,
    Thread = 0x00000004,
    Module = 0x00000008,
    Module32 = 0x00000010,
    Inherit = 0x80000000,
    All = 0x0000001F,
    NoHeaps = 0x40000000
}

[Flags]
public enum ThreadAccess
{
    Terminate = 0x0001,
    SuspendResume = 0x0002,
    GetContext = 0x0008,
    SetContext = 0x0010,
    SetInformation = 0x0020,
    QueryInformation = 0x0040,
    SetThreadToken = 0x0080,
    Impersonate = 0x0100,
    DirectImpersonation = 0x0200,
    All = 0x1FFFFF
}

[Flags]
public enum MemoryProtection : uint
{
    Execute = 0x10,
    ExecuteRead = 0x20,
    ExecuteReadWrite = 0x40,
    ExecuteWriteCopy = 0x80,
    NoAccess = 0x01,
    ReadOnly = 0x02,
    ReadWrite = 0x04,
    WriteCopy = 0x08,
    GuardModifierFlag = 0x100,
    NoCacheModifierFlag = 0x200,
    WriteCombineModifierFlag = 0x400
}

[Flags]
public enum WindowLongFlags
{
    GWL_EXSTYLE = -20,
    GWLP_HINSTANCE = -6,
    GWLP_HWNDPARENT = -8,
    GWL_ID = -12,
    GWL_STYLE = -16,
    GWL_USERDATA = -21,
    GWL_WNDPROC = -4,
    DWLP_USER = 0x8,
    DWLP_MSGRESULT = 0x0,
    DWLP_DLGPROC = 0x4
}

[Flags]
public enum WindowStyles : uint
{
    WS_OVERLAPPED = 0x00000000,
    WS_POPUP = 0x80000000,
    WS_CHILD = 0x40000000,
    WS_MINIMIZE = 0x20000000,
    WS_VISIBLE = 0x10000000,
    WS_DISABLED = 0x08000000,
    WS_CLIPSIBLINGS = 0x04000000,
    WS_CLIPCHILDREN = 0x02000000,
    WS_MAXIMIZE = 0x01000000,
    WS_CAPTION = 0x00C00000,
    WS_BORDER = 0x00800000,
    WS_DLGFRAME = 0x00400000,
    WS_VSCROLL = 0x00200000,
    WS_HSCROLL = 0x00100000,
    WS_SYSMENU = 0x00080000,
    WS_THICKFRAME = 0x00040000,
    WS_GROUP = 0x00020000,
    WS_TABSTOP = 0x00010000,
    WS_MINIMIZEBOX = 0x00020000,
    WS_MAXIMIZEBOX = 0x00010000,
    WS_TILED = WS_OVERLAPPED,
    WS_ICONIC = WS_MINIMIZE,
    WS_SIZEBOX = WS_THICKFRAME,
    WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
    WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX),
    WS_POPUPWINDOW = (WS_POPUP | WS_BORDER | WS_SYSMENU),
    WS_CHILDWINDOW = (WS_CHILD)
}

[Flags]
public enum SystemMetrics : int
{
    SM_CXSCREEN = 0,
    SM_CYSCREEN = 1,
    SM_CXVSCROLL = 2,
    SM_CYHSCROLL = 3,
    SM_CYCAPTION = 4,
    SM_CXBORDER = 5,
    SM_CYBORDER = 6,
    SM_CXDLGFRAME = 7,
    SM_CYDLGFRAME = 8,
    SM_CYVTHUMB = 9,
    SM_CXHTHUMB = 10,
    SM_CXICON = 11,
    SM_CYICON = 12,

    // SM_CXCURSOR = 13,
    // SM_CYCURSOR = 14,
    SM_CYMENU = 15,
    SM_CXFULLSCREEN = 16,
    SM_CYFULLSCREEN = 17,
    SM_CYKANJIWINDOW = 18,
    SM_MOUSEPRESENT = 19,
    SM_CYVSCROLL = 20,
    SM_CXHSCROLL = 21,

    //SM_DEBUG = 22,
    //SM_SWAPBUTTON = 23,
    // SM_RESERVED1 = 24,
    // SM_RESERVED2 = 25,
    // SM_RESERVED3 = 26,
    // SM_RESERVED4 = 27,
    SM_CXMIN = 28,
    SM_CYMIN = 29,
    SM_CXSIZE = 30,
    SM_CYSIZE = 31,
    SM_CXFRAME = 32,
    SM_CYFRAME = 33,
    SM_CXMINTRACK = 34,
    SM_CYMINTRACK = 35,
    SM_CXDOUBLECLK = 36,
    SM_CYDOUBLECLK = 37,
    SM_CXICONSPACING = 38,
    SM_CYICONSPACING = 39,
    SM_MENUDROPALIGNMENT = 40,
    SM_PENWINDOWS = 41,

    // SM_DBCSENABLED = 42,
    // SM_CMOUSEBUTTONS = 43,
    SM_CXFIXEDFRAME = SM_CXDLGFRAME,
    SM_CYFIXEDFRAME = SM_CYDLGFRAME,
    SM_CXSIZEFRAME = SM_CXFRAME,
    SM_CYSIZEFRAME = SM_CYFRAME,
    SM_SECURE = 44,
    SM_CXEDGE = 45,
    SM_CYEDGE = 46,
    SM_CXMINSPACING = 47,
    SM_CYMINSPACING = 48,
    SM_CXSMICON = 49,
    SM_CYSMICON = 50,
    SM_CYSMCAPTION = 51,
    SM_CXSMSIZE = 52,
    SM_CYSMSIZE = 53,
    SM_CXMENUSIZE = 54,
    SM_CYMENUSIZE = 55,
    SM_ARRANGE = 56,
    SM_CXMINIMIZED = 57,
    SM_CYMINIMIZED = 58,
    SM_CXMAXTRACK = 59,
    SM_CYMAXTRACK = 60,
    SM_CXMAXIMIZED = 61,
    SM_CYMAXIMIZED = 62,

    // SM_NETWORK = 63,
    // SM_CLEANBOOT = 67,
    SM_CXDRAG = 68,
    SM_CYDRAG = 69,

    // SM_SHOWSOUNDS = 70,
    SM_CXMENUCHECK = 71,
    SM_CYMENUCHECK = 72,

    // SM_SLOWMACHINE = 73,
    // SM_MIDEASTENABLED = 74,
    // SM_MOUSEWHEELPRESENT = 75,
    SM_XVIRTUALSCREEN = 76,
    SM_YVIRTUALSCREEN = 77,
    SM_CXVIRTUALSCREEN = 78,
    SM_CYVIRTUALSCREEN = 79,

    // SM_CMONITORS = 80,
    // SM_SAMEDISPLAYFORMAT = 81,
    // SM_IMMENABLED = 82,
    SM_CXFOCUSBORDER = 83,
    SM_CYFOCUSBORDER = 84,

    // SM_TABLETPC = 86,
    // SM_MEDIACENTER = 87,
    // SM_STARTER = 88,
    // SM_SERVERR2 = 89,
    // SM_MOUSEHORIZONTALWHEELPRESENT = 91,
    SM_CXPADDEDBORDER = 92,
    SM_CYPADDEDBORDER = 93,
    // SM_DIGITIZER = 94,
    // SM_MAXIMUMTOUCHES = 95,
    // SM_REMOTESESSION = 0x1000,
    // SM_SHUTTINGDOWN = 0x2000,
    // SM_REMOTECONTROL = 0x2001,
    // SM_CONVERTIBLESLATEMODE = 0x2003,
    // SM_SYSTEMDOCKED = 0x2004,
    // SM_XBYTEALIGNCLIENT = 0x2018,
    // SM_XBYTEALIGNWINDOW = 0x2019,
    // SM_TABLETPCPOINTER = 0x201A,
    // SM_FOCUSSENABLED = 0x201C,
    // SM_SYSTEMCAPABILITIES = 0x201D,
    // SM_USERPREFS = 0x201E,
    // SM_DBCSENABLED = 0x020A,
    // SM_REMOTESESSION = 0x1000,
    // SM_SHUTTINGDOWN = 0x2000,
    // SM_REMOTECONTROL = 0x2001,
    // SM_CONVERTIBLESLATEMODE = 0x2003,
    // SM_SYSTEMDOCKED = 0x2004,
    // SM_XBYTEALIGNCLIENT = 0x2018,
    // SM_XBYTEALIGNWINDOW = 0x2019,
}

public struct CURSORINFO
{
    public int cbSize;
    public int flags;
    public nuint hCursor;
    public Point ptScreenPos;
}

public struct POINT
{
    public int X;
    public int Y;
    public override string ToString() => $"X: {X}, Y: {Y}";

    public POINT()
    {
    }

    public POINT(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
    public override string ToString() => $"Left: {Left}, Top: {Top}, Right: {Right}, Bottom: {Bottom}";

    public RECT()
    {
    }

    public RECT(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
}

public struct WINDOWPLACEMENT
{
    public int length;
    public int flags;
    public int showCmd;
    public Point ptMinPosition;
    public Point ptMaxPosition;
    public RECT rcNormalPosition;

    public override string ToString()
    {
        return $"length: {length}, \n" +
               $"flags: {flags}, \n" +
               $"showCmd: {showCmd}, \n" +
               $"ptMinPosition: {ptMinPosition}, \n" +
               $"ptMaxPosition: {ptMaxPosition}, \n" +
               $"rcNormalPosition: {rcNormalPosition}";
    }
}

public struct WINDOWINFO
{
    public int cbSize;
    public RECT rcWindow;
    public RECT rcClient;
    public WindowStyles dwStyle;
    public int dwExStyle;
    public int dwWindowStatus;
    public uint cxWindowBorders;
    public uint cyWindowBorders;
    public nuint atomWindowType;
    public ushort wCreatorVersion;

    public override string ToString()
    {
        return $"cbSize: {cbSize}, \n" +
               $"rcWindow: {rcWindow}, \n" +
               $"rcClient: {rcClient}, \n" +
               $"dwStyle: {dwStyle}, \n" +
               $"dwExStyle: {dwExStyle}, \n" +
               $"dwWindowStatus: {dwWindowStatus}, \n" +
               $"cxWindowBorders: {cxWindowBorders}, \n" +
               $"cyWindowBorders: {cyWindowBorders}, \n" +
               $"atomWindowType: {atomWindowType}, \n" +
               $"wCreatorVersion: {wCreatorVersion}\n";
    }
}

public struct WNDCLASSEX
{
    public uint cbSize;
    public uint style;
    public nuint lpfnWndProc;
    public int cbClsExtra;
    public int cbWndExtra;
    public nuint hInstance;
    public nuint hIcon;
    public nuint hCursor;
    public nuint hbrBackground;
    public string lpszMenuName;
    public string lpszClassName;
    public nuint hIconSm;
}

public struct TITLEBARINFO
{
    public uint cbSize;
    public RECT rcTitleBar;
    public uint rgstate;
    public RECT rgrect;

    public override string ToString()
    {
        return $"cbSize: {cbSize}, \n" +
               $"rcTitleBar: {rcTitleBar}, \n" +
               $"rgstate: {rgstate}, \n" +
               $"rgrect: {rgrect}";
    }
}

public struct TITLEBARINFOEX
{
    public uint cbSize;
    public RECT rcTitleBar;
    public uint rgstate;
    public RECT rgrect;
    public RECT rgrectTitleBar;
    public RECT rgrectFullScreen;
    public RECT rgrectReserved;

    public override string ToString()
    {
        return $"cbSize: {cbSize}, \n" +
               $"rcTitleBar: {rcTitleBar}, \n" +
               $"rgstate: {rgstate}, \n" +
               $"rgrect: {rgrect}, \n" +
               $"rgrectTitleBar: {rgrectTitleBar}, \n" +
               $"rgrectFullScreen: {rgrectFullScreen}, \n" +
               $"rgrectReserved: {rgrectReserved}";
    }
}

public struct MONITORINFO
{
    public uint cbSize;
    public RECT rcMonitor;
    public RECT rcWork;
    public uint dwFlags;

    public override string ToString()
    {
        return $"cbSize: {cbSize}, \n" +
               $"rcMonitor: {rcMonitor}, \n" +
               $"rcWork: {rcWork}, \n" +
               $"dwFlags: {dwFlags}";
    }
}

public struct MONITORINFOEX
{
    public uint cbSize;
    public RECT rcMonitor;
    public RECT rcWork;
    public uint dwFlags;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public char[] szDevice;

    public override string ToString()
    {
        return $"cbSize: {cbSize}, \n" +
               $"rcMonitor: {rcMonitor}, \n" +
               $"rcWork: {rcWork}, \n" +
               $"dwFlags: {dwFlags}, \n" +
               $"szDevice: {new string(szDevice)}";
    }
}