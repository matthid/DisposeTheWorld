using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DisposeTheWorld
{
    // Warum Disposable?
    // Aufräumen von Ressourcen (Dateien, Handles, aber auch Management von "Scopes")
    //  - "Managed" (Scopes, "virtuell")
    //  - "Unmanaged" aber "managed" bzw. "Wrapped" von einer anderen Klasse (SafeHandle, Stream, Klassen die IDisposable selbst implementieren)
    //  - "Unmanaged"



















    // "Unmanaged" aber "managed"
























    // ✓ DO implement the Basic Dispose Pattern on types containing instances of disposable types.
    public class DisposableResourceHolder : IDisposable
    {
        private SafeHandle resource; // handle to a resource  

        public DisposableResourceHolder()
        {
            this.resource = null; // allocates the resource  
        }

        // ✓ DO implement the IDisposable interface by simply 
        // calling Dispose(true) followed by GC.SuppressFinalize(this).
        // X DO NOT make the parameterless Dispose method virtual
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // ✓ DO declare a protected virtual void Dispose(bool disposing) method to centralize 
        // all logic related to releasing unmanaged resources. 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                resource?.Dispose();
            }
        }

        // X DO NOT declare any overloads of the Dispose method other than Dispose() and Dispose(bool). 
    }


    public class MyDisposableResourceHolder : DisposableResourceHolder
    {
        bool disposed = false;

        // ✓ DO allow the Dispose(bool) method to be called more than once. The method might choose to do nothing after the first call. 
        protected override void Dispose(bool disposing)
        {
            if (disposed) return;
            //cleanup
            base.Dispose(disposing);
            disposed = true;
        }


        // X AVOID throwing an exception from within Dispose(bool) except under critical situations where the containing process has been corrupted (leaks, inconsistent shared state, etc.). 
















        // ✓ DO throw an ObjectDisposedException from any member that cannot be used after the object has been disposed of. 
        public void DoSomething()
        {
            if (disposed) throw new ObjectDisposedException("MyObject");
            // now call some native methods using the resource
        }
    }


    // ✓ CONSIDER providing method Close(), in addition to the Dispose(), if close is standard terminology in the area. 
    class CloseInsteadOfDispose : IDisposable
    {
        void IDisposable.Dispose()
        {
            Close();
        }
        public void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //...
            }
        }
    }

    // ✓ CONSIDER implementing the Basic Dispose Pattern on classes that themselves don’t hold unmanaged resources or disposable objects but are likely to have subtypes that do. 
    // -> Example: System.IO.Stream

    class Program1
    {
        static void Main(string[] args)
        {
            using (var res = new DisposableResourceHolder())
            {

            }
        }
    }


















    // "Unmanaged"













    // "Unmanaged"
    public class ComplexResourceHolder : IDisposable
    {
        private IntPtr buffer; // unmanaged memory buffer  
        private SafeHandle resource; // disposable handle to a resource
        private bool _disposed = false;

        public ComplexResourceHolder()
        {
            this.buffer = Marshal.AllocHGlobal(1024); // allocates memory (Call Unmanaged/function)
            this.resource = null; // allocates the resource  
        }

        // ✓ DO implement the Basic Dispose Pattern on every finalizable type.
        // X DO NOT access any finalizable objects in the finalizer code path, because there is significant risk that they will have already been finalized. 
        // X DO NOT let exceptions escape from the finalizer logic, except for system-critical failures.
        // ✓ CONSIDER creating and using a critical finalizable object (a type with a type hierarchy that contains CriticalFinalizerObject) for situations in which a finalizer absolutely must execute even in the face of forced application domain unloads and thread aborts. 
        protected virtual void Dispose(bool disposing)
        {
            // Accessing a static variable that refers to a finalizable object (or calling a static method that might use values stored in static variables) might not be safe if Environment.HasShutdownStarted returns true. 
            if (_disposed)
            {
                return;
            }

            Marshal.Release(buffer); // release unmanaged memory / Call Unmanaged/External function
            if (disposing)
            {
                // release other disposable objects
                resource?.Dispose();
            }

            _disposed = true;
        }

        // X AVOID making types finalizable.
        // X DO NOT make value types finalizable.
        // ✓ DO make a type finalizable if the type is responsible for releasing an unmanaged resource that does not have its own finalizer.
        // ✓ DO make your Finalize method protected. (Enforced by the C# compiler)
        ~ComplexResourceHolder()
        {
            Dispose(false);
        }

        // ✓ DO implement the Basic Dispose Pattern on every finalizable type. 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

























    //  - "Managed" (Scopes, "virtuell")
    public class ConsoleColorScope : IDisposable // Generated by R#
    {
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        private ConsoleColor _oldColor;

        public ConsoleColorScope(ConsoleColor color)
        {
            _oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Console.ForegroundColor = _oldColor;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ManagedScope() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
    
    class Program2
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Default");
            using (new ConsoleColorScope(ConsoleColor.Cyan))
            {
                Console.WriteLine("In Cyan");
                using (new ConsoleColorScope(ConsoleColor.Yellow))
                {
                    Console.WriteLine("In Yellow");
                }
                Console.WriteLine("In Cyan2");
            }
            Console.WriteLine("Default2");
        }
    }
















}
