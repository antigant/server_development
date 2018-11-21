using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Hive;
using Photon.Hive.Plugin;

namespace TestPlugin
{
    public class RaiseEventTestPlugin : PluginBase
    {
        public string ServerString
        {
            get;
            private set;
        }
        public int CallsCount
        {
            get;
            private set;
        }

        public RaiseEventTestPlugin()
        {
            UseStrictMode = true;
            ServerString = "ServerMessage";
            CallsCount = 0;
        }

        public override string Name
        {
            get
            {
                return GetType().Name;
            }
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            try
            {
                base.OnRaiseEvent(info);
            }
            catch (Exception e)
            {
                PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
                return;
            }

            if (info.Request.EvCode == 1)
            {
                ++CallsCount;
                int cnt = CallsCount;
                string ReturnMessage = info.Nickname + " clicked the button. Now the count is " + cnt.ToString();

                PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                          senderActor: 0,
                                          targetGroup: 0,
                                          data: new Dictionary<byte, object>() { { 245, ReturnMessage } },
                                          evCode: info.Request.EvCode, cacheOp: 0);
            }
        }
    }
}
