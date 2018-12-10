using System;
using System.Collections.Generic;
using System.Text;
using Photon.Hive.Plugin;
using MySql.Data.MySqlClient;
using CustomPlugin;

namespace TestPlugin
{
    public class RaiseEventTestPlugin : PluginBase
    {
        readonly string connStr = "server=localhost;user=root;database=photon;port=3306;password=qwerty";
        MySqlConnection conn;

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

        public override bool SetupInstance(IPluginHost host, Dictionary<string, string> config, out string errorMsg)
        {
            host.TryRegisterType(typeof(Item), (byte)'A', Item.Serialize, Item.Deserialize);
            return base.SetupInstance(host, config, out errorMsg);
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
            //int[] test = { 100, 11238 };
            Item test = new Item(513, "new test item");

            PluginHost.BroadcastEvent(recieverActors: new List<int>() { { info.ActorNr } }, 
                senderActor: 0,
                evCode:info.Request.EvCode, 
                data: new Dictionary<byte, object>() { { 245, test }, { 254, 0 } }, 
                cacheOp: CacheOperations.DoNotCache);
        }

        void Login(IRaiseEventCallInfo info)
        {
            // check if both username and password is right
            string[] message = (string[])info.Request.Data;

            // extract the message
            //string recvUsername = General.GetStringDataFromMessage(message, "Username");
            //string recvPassword = General.GetStringDataFromMessage(message, "Password");

            string recvUsername = message[0];
            string recvPassword = message[1];

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
                sql = "SELECT * FROM player WHERE account_id = '" + accountID + "'";
                cmd.CommandText = sql;
                rdr = cmd.ExecuteReader();
                string playerName = "";
                while (rdr.Read())
                    playerName = rdr.GetString(1);

                returnMessage = "Successful, AccountID=" + accountID + ", PlayerName=" + playerName;
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
            string[] message = (string[])info.Request.Data;

            string recvUsername = message[0];
            string recvPassword = message[1];
            string recvPlayerName = message[2];

            // Check if username is in database
            string sql = "SELECT * FROM account WHERE username = '" + recvUsername + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();

            string returnMessage = "";

            if (rdr.HasRows)
                returnMessage = "Unsuccessful, Message=Username exists already";
            rdr.Close();

            sql = "SELECT * FROM player WHERE player_name = '" + recvPlayerName + "'";
            cmd.CommandText = sql;
            rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
                returnMessage = "Unsuccessful, Message=Character Name exist already";
            rdr.Close();

            // There's no error, thus registration is successful!
            if (returnMessage == "")
            {
                // insert into database
                sql = "INSERT INTO account (username, password, date_created) VALUES ('" + recvUsername + "', '" + recvPassword + "', now())";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();

                // get account_id from account
                sql = "SELECT * FROM account WHERE username ='" + recvUsername + "'";
                cmd.CommandText = sql;
                rdr = cmd.ExecuteReader();
                int accountID = 0;
                while (rdr.Read())
                    accountID = rdr.GetInt32(0);
                rdr.Close();

                float val = 0.0f;

                // insert into player table
                sql = "INSERT INTO player (account_id, player_name, pos_x, pos_y, pos_z) VALUES ('" + accountID + "', '" + recvPlayerName + "', '" + val + "', '" + val + "', '" + val + "')";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();

                returnMessage = "Successful, Message=Registration Complete";
            }

            // returns the message to the client
            PluginHost.BroadcastEvent(target: (byte)info.ActorNr,
                                  senderActor: 0,
                                  targetGroup: 0,
                                  data: new Dictionary<byte, object>() { { 245, returnMessage } },
                                  evCode: info.Request.EvCode,
                                  cacheOp: 0);
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
            conn.Close();
        }
    }
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

//string GetStringDataFromMessage(string returnData)
//{
//    string temp = returnData + "=";
//    int pFrom = message.IndexOf(temp) + temp.Length;
//    int pTo = message.LastIndexOf(",");

//    // there's no comma at the end
//    if (pTo - pFrom < 0)
//    {
//        pFrom = message.LastIndexOf(temp) + temp.Length;
//        return message.Substring(pFrom);
//    }

//    return message.Substring(pFrom, pTo - pFrom);
//}