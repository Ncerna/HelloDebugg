using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio;
using System.Collections.Generic;
using System.Collections;
using System.Runtime;

namespace HelloDebugg
{
   
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    //Especifica un contexto donde el editor está en modo de diseño.
    //[ProvideAutoLoad(UIContextGuids.DesignMode, PackageAutoLoadFlags.BackgroundLoad)]
    //solucion entra en modo depuracion
    [ProvideAutoLoad(UIContextGuids.Debugging, PackageAutoLoadFlags.BackgroundLoad)]
    [Guid(HelloDebuggPackage.PackageGuidString)]
    public sealed class HelloDebuggPackage : AsyncPackage, IVsDebuggerEvents
    {
        //public const string PackageGuidString = "63c1055a-e020-4c43-945a-3b7dd53a6f76";
        public const string PackageGuidString = "ADFC4E61-0397-11D1-9F4E-00A0C911004F";
        private DTE dte;
        private IVsDebugger debugService;
        private uint debugCookie;
        private int caount = 1;
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            dte = await GetServiceAsync(typeof(DTE)) as DTE;
            Assumes.Present(dte);
            debugService = (IVsDebugger)GetGlobalService(typeof(SVsShellDebugger));
            debugService.AdviseDebuggerEvents(this, out debugCookie);
            //var dte1 = (DTE2)Package.GetGlobalService(typeof(SDTE));
            ReportProgress("Inicializado Package Run Mode Veces:" + caount, Environment.UserName);
            caount++;
        }

        ///Devuelve el modo de depurador actual, un valor en el DBGMODE enumeración como DBGMODE_Break.
        /// Utilizado por un cliente para recibir notificaciones de eventos de depuración. 
        /// En general, uso AdviseDebuggerEvents ( IVsDebuggerEvents, UInt32 ) y UnadviseDebuggerEvents ( UInt32 ) en su lugar.
        int IVsDebuggerEvents.OnModeChange(DBGMODE dbgmodeNew)
        {
            switch (dbgmodeNew)
            {
                case DBGMODE.DBGMODE_Run:
                    // EnterRunMode?.Invoke();
                    var dte3 = (DTE2)Package.GetGlobalService(typeof(SDTE));
                    ReportProgress("Run Mode Veces:" + caount, Environment.UserName);
                    break;
            }
            caount++;
            return VSConstants.S_OK;
        }

        public int OnModeChange(DBGMODE dbgmodeNew)
        {
            
            return VSConstants.S_OK;
        }





















        private void ReportProgress(string currentSteps, string numberOfSteps)
        {

            UseOutputWindowAsync(currentSteps, numberOfSteps)
                .ConfigureAwait(false);
        }
        private IVsOutputWindowPane _pane;
        private async Task UseOutputWindowAsync(string mensaje, string nombreUser)
        {
            if (_pane == null)
            {
                await JoinableTaskFactory.SwitchToMainThreadAsync(DisposalToken);
                var ow = await GetServiceAsync(typeof(SVsOutputWindow)) as IVsOutputWindow;
                Assumes.Present(ow);

                var guid = Guid.NewGuid();
                ow.CreatePane(ref guid, "Plugin Cerna FIIS", 1, 1);
                ow.GetPane(ref guid, out _pane);

                _pane.Activate();
                _pane.OutputStringThreadSafe($"{mensaje} HOLA: {nombreUser} \r\n");
            }
            else
            {
                _pane.OutputStringThreadSafe($" {mensaje} Hello: {nombreUser} \r\n");
            }
        }

    }
}
