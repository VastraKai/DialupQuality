using System.ComponentModel;
using System.Diagnostics;
using ThreadState = System.Diagnostics.ThreadState;

//using ThreadState = System.Threading.ThreadState;

namespace DialupQuality;

public static class ProcessExtensions
{
    public static void Suspend(this Process process)
    {
        _threads = new ProcessThreadCollection(Array.Empty<ProcessThread>());
        if (process == null)
        {
            throw new ArgumentNullException(nameof(process));
        }

        if (process.HasExited)
        {
            throw new InvalidOperationException("The process has exited.");
        }

        if (process.Threads.Count == 0)
        {
            throw new InvalidOperationException("The process has no threads.");
        }

        foreach (ProcessThread thread in process.Threads)
        {
            IntPtr threadHandle = Imps.OpenThread(ThreadAccess.SuspendResume, true, thread.Id);
            if (threadHandle == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            try
            {
                if (thread.ThreadState == ThreadState.Running)
                {
                    _threads.Add(thread);
                    Imps.SuspendThread(threadHandle);
                }
            }
            finally
            {
                Imps.CloseHandle(threadHandle);
            }
        }
    }

    private static ProcessThreadCollection _threads;
    public static void Resume(this Process process)
    {
        if (process == null)
            throw new ArgumentNullException(nameof(process));
        

        if (process.HasExited)
            throw new InvalidOperationException("The process has exited.");
        

        if (process.Threads.Count == 0)
            throw new InvalidOperationException("The process has no threads.");
        

        foreach (ProcessThread thread in _threads)
        {
            IntPtr threadHandle = Imps.OpenThread(ThreadAccess.SuspendResume, true, (int)thread.Id);
            if (threadHandle == IntPtr.Zero)
                throw new Win32Exception();
            

            try
            {
                Imps.ResumeThread(threadHandle);
            }
            finally
            {
                Imps.CloseHandle(threadHandle);
            }
        }
    }
}