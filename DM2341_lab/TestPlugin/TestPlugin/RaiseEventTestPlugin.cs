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
        readonly string connStr = "server=localhost;user=root;database=photon;port=3306;password=qwerty";
        MySqlConnection conn;
        string message;

        public string ServerString { get; private set; }
        public int CallsCount { get; private set; }

        public RaiseEventTestPlugin()
        {
            UseStrictMode = true;
            ServerString = "ServerMessage";
            CallsCount = 0;

            //// ----- Connects to MySQL data base
            //ConnectToMySQL();
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

            ConnectToMySQL();

            switch (info.Request.EvCode)
            {
                case (byte)EvCode.LOGIN:
                    {
                        Login(info);
                        break;
                    }
                case (byte)EvCode.REGISTRATION:
                    {
                        Registration(info);
                        break;
                    }
                case (byte)EvCode.PHOTON_TEST:
                    {
                        Photon_Test(info);
                        break;
                    }

                default:
                    break;
            }

            DisconnectFromMySQL();
        }

        void Photon_Test(IRaiseEventCallInfo info)
        {
            int[] test = { 100, 11238 };

            PluginHost.BroadcastEvent(recieverActors: new List<int>() { { info.ActorNr } }, 
                senderActor: 0,
                evCode:info.Request.EvCode, 
                data: new Dictionary<byte, object>() { { 245, test } }, 
                cacheOp: 0);
        }

        void Login(IRaiseEventCallInfo info)
        {
            // check if both username and password is right
            message = Encoding.Default.GetString((byte[])info.Request.Data);

            // extract the message
            string recvUsername = GetStringDataFromMessage("Username");
            string recvPassword = GetStringDataFromMessage("Password");

            // Check if username is in database
            string sql = "SELECT * FROM account WHERE username = '" + recvUsername + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();

            int accountID = 0; string username = null; string password = null;
            while (rdr.Read())
            {
                accountID = rdr.GetInt32(0);
                username = rdr.GetString(1);
                password = rdr.GetString(2);
            }
            rdr.Close();

            string returnMessage = "";
            if (username == null || password != recvPassword)
                returnMessage = "Unsuccessful, Message=Incorrect username/password";
            else
            {
                // get player name
                sql = "SELECT player_name FROM player WHERE account_id = '" + accountID + "'";
                cmd.CommandText = sql;
                rdr = cmd.ExecuteReader();
                string playerName = "";
                while (rdr.Read())
                    playerName = rdr.GetString(1);

                returnMessage = "Successful, AccountID=" + accountID + "PlayerName=" + playerName;
            }

            // returns the message to the client
            PluginHost.BroadcastEvent(target: (byte)info.ActorNr,
                                  senderActor: 0,
                                  targetGroup: 0,
                                  data: new Dictionary<byte, object>() { { 245, returnMessage } },
                                  evCode: info.Request.EvCode,
                                  cacheOp: 0);
        }

        void Registration(IRaiseEventCallInfo info)
        {
            message = Encoding.Default.GetString((byte[])info.Request.Data);
        }

        string GetStringDataFromMessage(string returnData)
        {
            string temp = returnData + "=";
            int pFrom = message.IndexOf(temp) + temp.Length;
            int pTo = message.LastIndexOf(",");

            // there's no comma at the end
            if (pTo - pFrom < 0)
            {
                pFrom = message.LastIndexOf(temp) + temp.Length;
                return message.Substring(pFrom);
            }

            return message.Substring(pFrom, pTo - pFrom);
        }

        // ----- Open connection to 
        void ConnectToMySQL()
        {
            // Connect to MySQL
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

        // ----- Close MySQL connection
        void DisconnectFromMySQL()
        {
            // reset data
            message = "";
            conn.Close();
        }

        //// to insert into database
        //if (1 == info.Request.EvCode)
        //{
        //    string playerName = Encoding.Default.GetString((byte[])info.Request.Data);
        //    string sql = "INSERT INTO user (name, date_created) VALUES ('" + playerName + "', now())";
        //    MySqlCommand cmd = new MySqlCommand(sql, conn);
        //    cmd.ExecuteNonQuery();

        //    ++CallsCount;
        //    int cnt = CallsCount;
        //    string ReturnMessage = info.Nickname + " clicked the button. Now the count is " + cnt.ToString();

        //    // returns the message to the client
        //    PluginHost.BroadcastEvent(target: ReciverGroup.All,
        //                              senderActor: 0,
        //                              targetGroup: 0,
        //                              data: new Dictionary<byte, object>() { { 245, ReturnMessage } },
        //                              evCode: info.Request.EvCode, 
        //                              cacheOp: 0);
        //}
        //// testing with viking client
        //else if ((byte)EvCode.LOGIN == info.Request.EvCode)
        //{
        //    message = Encoding.Default.GetString((byte[])info.Request.Data);

        //    string playerName = GetStringDataFromMessage("PlayerName");
        //    string playerPassword = GetStringDataFromMessage("Password");

        //    string sql = "INSERT INTO viking (name, password, date_created) VALUES ('" + playerName + "', '" + playerPassword + "', now())";
        //    MySqlCommand cmd = new MySqlCommand(sql, conn);
        //    cmd.ExecuteNonQuery();
        //}
    }
}
