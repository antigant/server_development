using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Hive;
using Photon.Hive.Plugin;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace TestPlugin
{
    public class RaiseEventTestPlugin : PluginBase
    {
        string connStr;
        MySqlConnection conn;

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

            // ----- Connects to MySQL data base
            ConnectToMySQL();
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

            if (1 == info.Request.EvCode)
            {
                string playerName = Encoding.Default.GetString((byte[])info.Request.Data);
                string sql = "INSERT INTO users (name, date_created) VALUES ('" + playerName + "', now())";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

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

        public void ConnectToMySQL()
        {
            // Connect to MySQL
            connStr = "server=localhost;user=root;database=photon;port=3306;password=qwerty";
            conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void DisconnectFromMySQL()
        {
            conn.Close();
        }
    }
}
