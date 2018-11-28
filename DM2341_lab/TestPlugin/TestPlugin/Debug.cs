using System;
using System.Collections.Generic;
using System.Text;
using Photon.SocketServer;

namespace RaiseEventTestPlugin
{
    class Debug : ApplicationBase
    {
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            throw new NotImplementedException();
        }

        protected override void Setup()
        {
            try
            {
                System.Diagnostics.Debugger.Launch();
            }
            catch(Exception e)
            {
                e.ToString();
            }

            //throw new NotImplementedException();
        }

        protected override void TearDown()
        {
            throw new NotImplementedException();
        }
    }
}
