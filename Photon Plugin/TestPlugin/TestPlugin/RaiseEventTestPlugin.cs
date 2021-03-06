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
                case (byte)EvCode.LOGOUT:
                    {
                        Logout(info);
                        break;
                    }
                case (byte)EvCode.RESET_PASSWORD:
                    {
                        ResetPassword(info);
                        break;
                    }
                case (byte)EvCode.UPDATE_ITEM:
                    {
                        UpdateItem(info);
                        break;
                    }
                case (byte)EvCode.INIT_INVENTORY:
                    {
                        InitInventory(info);
                        break;
                    }
                default:
                    break;
            }

            DisconnectFromMySQL();
        }

        //void Photon_Test(IRaiseEventCallInfo info)
        //{
        //    //int[] test = { 100, 11238 };
        //    Item[] test = new Item[2]
        //    {
        //        new Item(541, "hello"),
        //        new Item(123, "byee"),
        //    };

        //    PluginHost.BroadcastEvent(recieverActors: new List<int>() { { info.ActorNr } }, 
        //        senderActor: 0,
        //        evCode:info.Request.EvCode, 
        //        data: new Dictionary<byte, object>() { { 245, test }, { 254, 0 } }, 
        //        cacheOp: CacheOperations.DoNotCache);
        //}

        void Login(IRaiseEventCallInfo info)
        {
            // check if both username and password is right
            string[] message = (string[])info.Request.Data;

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

            // check if the account is currently active
            bool accActive = false;
            sql = "SELECT * FROM active_users WHERE account_id ='" + accountID + "'";
            cmd.CommandText = sql;
            rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
                accActive = true;
            rdr.Close();

            string[] returnMessage = new string[12]
            {
                "NULL",
                "NULL",
                "NULL",
                "NULL",
                "NULL",
                "NULL",
                "NULL",
                "NULL",
                "NULL",
                "NULL",
                "NULL",
                "NULL",
            };
            if (username == null || password != recvPassword)
                returnMessage[0] = "Unsuccessful, Message=Incorrect username/password";
            else if (accActive)
                returnMessage[0] = "Unsuccessful, Message=Account is currently active";
            else
            {
                // get player name & position
                sql = "SELECT * FROM player WHERE account_id ='" + accountID + "'";
                cmd.CommandText = sql;
                rdr = cmd.ExecuteReader();
                returnMessage[0] = "Successful";
                returnMessage[1] = accountID.ToString();

                while (rdr.Read())
                {
                    returnMessage[2] = rdr.GetString(1);
                    returnMessage[3] = rdr.GetFloat(2).ToString();
                    returnMessage[4] = rdr.GetFloat(3).ToString();
                    returnMessage[5] = rdr.GetFloat(4).ToString();
                }
                rdr.Close();

                // get pet's position
                sql = "SELECT * FROM pet WHERE account_id ='" + accountID + "'";
                cmd.CommandText = sql;
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                { 
                    returnMessage[6] = rdr.GetFloat(1).ToString();
                    returnMessage[7] = rdr.GetFloat(2).ToString();
                    returnMessage[8] = rdr.GetFloat(3).ToString();
                }
                rdr.Close();

                // get the audio settings
                sql = "SELECT * FROM sound WHERE account_id='" + accountID + "'";
                cmd.CommandText = sql;
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    returnMessage[9] = rdr.GetFloat(1).ToString();
                    returnMessage[10] = rdr.GetFloat(2).ToString();
                    returnMessage[11] = rdr.GetFloat(3).ToString();
                }
                rdr.Close();

                // insert account_id into active table list (to state that account has someone playing)
                sql = "INSERT INTO active_users (account_id) VALUES ('" + accountID + "')";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }

            PluginHost.BroadcastEvent(recieverActors: new List<int>() { { info.ActorNr } },
                                      senderActor: 0,
                                      evCode: info.Request.EvCode,
                                      data: new Dictionary<byte, object>() { { 245, returnMessage }, { 254, 0 } },
                                      cacheOp: CacheOperations.DoNotCache);
        }

        void Registration(IRaiseEventCallInfo info)
        {
            string[] message = (string[])info.Request.Data;

            string recvUsername = message[0];
            string recvPassword = message[1];
            string recvPlayerName = message[2];

            // Check if username is in database
            string sql = "SELECT * FROM account WHERE username ='" + recvUsername + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();

            string returnMessage = "";

            if (rdr.HasRows)
                returnMessage = "Unsuccessful, Message=Username exists already";
            rdr.Close();

            sql = "SELECT * FROM player WHERE player_name ='" + recvPlayerName + "'";
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

                // insert into player table
                sql = "INSERT INTO player (account_id, player_name, pos_x, pos_y, pos_z) VALUES ('" + accountID + "', '" + recvPlayerName + "', '51.09559', '3.4', '44.22058')";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();

                // insert into pet table
                sql = "INSERT INTO pet (account_id, pos_x, pos_y, pos_z) VALUES ('" + accountID + "', '51.0956', '2.3454', '43.52058')";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();

                sql = "INSERT INTO sound (account_id, master, bgm, sfx) VALUES ('" + accountID + "', '0.0', '0.0', '0.0')";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();

                returnMessage = "Successful, Message=Registration Complete!";
            }

            // returns the message to the client
            PluginHost.BroadcastEvent(recieverActors: new List<int>() { { info.ActorNr } },
                                      senderActor: 0,
                                      evCode: info.Request.EvCode,
                                      data: new Dictionary<byte, object>() { { 245, returnMessage }, { 254, 0 } },
                                      cacheOp: CacheOperations.DoNotCache);
        }

        void Logout(IRaiseEventCallInfo info)
        {
            string[] message = (string[])info.Request.Data;

            //// delete user from active list
            //string sql = "DELETE FROM active_users WHERE account_id='" + message[0] + "'";
            string sql = "UPDATE player SET pos_x='" + message[1] + "', pos_y='" + message[2] + "', pos_z='" + message[3] + "' WHERE account_id ='" + message[0] + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();

            sql = "UPDATE pet SET pos_x='" + message[4] + "', pos_y='" + message[5] + "', pos_z='" + message[6] + "' WHERE account_id ='" + message[0] + "'";
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            sql = "UPDATE sound SET master='" + message[7] + "', bgm='" + message[8] + "', sfx='" + message[9] + "' WHERE account_id ='" + message[0] + "'";
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            sql = "DELETE FROM active_users WHERE account_id = '" + message[0] + "'";
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        void ResetPassword(IRaiseEventCallInfo info)
        {
            string[] message = (string[])info.Request.Data;
            string sql = "SELECT * FROM account WHERE username ='" + message[0] + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();

            bool accExist = false;
            if (rdr.HasRows)
                accExist = true;
            rdr.Close();

            string returnMessage = "";
            // update the password
            if (accExist)
            {
                string prevPassword = "";
                // check previous password if correct
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                    prevPassword = rdr.GetString(2);
                rdr.Close();

                // check if prev match with the user
                bool matches = false;
                if (prevPassword == message[2])
                    matches = true;
                if (matches)
                {
                    // update the password
                    sql = "UPDATE account SET password='" + message[1] + "' WHERE username ='" + message[0] + "'";
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    returnMessage = "Successful, Message=Password Reset!";
                }
                else
                    returnMessage = "Unsucessful, Message=Previous Password does not match";
            }
            // return a message to tell the user no such account exist
            else
                returnMessage = "Unsuccessful, Message=Account does not exist";

            // returns the message to the client
            PluginHost.BroadcastEvent(recieverActors: new List<int>() { { info.ActorNr } },
                                      senderActor: 0,
                                      evCode: info.Request.EvCode,
                                      data: new Dictionary<byte, object>() { { 245, returnMessage }, { 254, 0 } },
                                      cacheOp: CacheOperations.DoNotCache);
        }

        void InitInventory(IRaiseEventCallInfo info)
        {
            string message = (string)info.Request.Data;

            string sql = "SELECT * FROM item WHERE account_id ='" + message + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();

            int[] itemIDs = new int[9];
            // init the items to be -1 (no item there)
            for (int index = 0; index < itemIDs.Length; ++index)
                itemIDs[index] = -1;

            // getting the data from the db
            int i = 0;
            while (rdr.Read())
            {
                itemIDs[i] = rdr.GetInt32(0);
                ++i;
            }
            rdr.Close();

            // returns the message to the client
            PluginHost.BroadcastEvent(recieverActors: new List<int>() { { info.ActorNr } },
                                      senderActor: 0,
                                      evCode: info.Request.EvCode,
                                      data: new Dictionary<byte, object>() { { 245, itemIDs }, { 254, 0 } },
                                      cacheOp: CacheOperations.DoNotCache);
        }

        void UpdateItem(IRaiseEventCallInfo info)
        {
            // index 0: message type (eg. INSERT/UPDATE), 1: account_id, 2: item id (NULL if its inserting)
            string[] message = (string[])info.Request.Data;

            string sql = "";
            MySqlCommand cmd;

            if (message[0][0] == 'I')
            {
                string[] returnMessage = new string[2]
                {
                    message[0],
                    "NULL",
                };

                // insert into db
                sql = "INSERT INTO item (account_id) VALUES ('" + message[1] + "')";
                cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                // get the item_id and return to the player
                sql = "SELECT * FROM item WHERE account_id='" + message[1] + "'";
                cmd.CommandText = sql;
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                    returnMessage[1] = rdr.GetInt32(0).ToString();
                rdr.Close();

                // returns the message to the client
                PluginHost.BroadcastEvent(recieverActors: new List<int>() { { info.ActorNr } },
                                          senderActor: 0,
                                          evCode: info.Request.EvCode,
                                          data: new Dictionary<byte, object>() { { 245, returnMessage }, { 254, 0 } },
                                          cacheOp: CacheOperations.DoNotCache);

                // if the codes still run after sending message to client, return
                return;
            }            
            else if(message[0][0] == 'U')
            {
                // update the account_id when player drop/pick_up the item
                sql = "UPDATE item SET account_id='" + message[3] + "' WHERE item_id='" + message[2] + "'";
                cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                if (message[0][1] == '1')
                    return;

            }
            else if (message[0][0] == 'D')
            {
                // delete the item where item_id is the one received
                sql = "DELETE FROM item WHERE item_id ='" + message[2] + "'";
                cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }

            // send back a message to tell the clients to off the active state of the item
            PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                      senderActor: 0,
                                      targetGroup: 0,
                                      data: new Dictionary<byte, object>() { { 245, false } },
                                      evCode: (byte)EvCode.ITEM_STATE,
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