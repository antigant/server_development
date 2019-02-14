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
        readonly string connStr = "server=35.225.27.6;user=root;database=photon;port=3306;password=KptCb5eGpM3Cs3zm";
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
            host.TryRegisterType(typeof(CLogin), (byte)'B', CLogin.Serialize, CLogin.Deserialize);
            host.TryRegisterType(typeof(CSound), (byte)'C', CSound.Serialize, CSound.Deserialize);
            host.TryRegisterType(typeof(CPlayer), (byte)'D', CPlayer.Serialize, CPlayer.Deserialize);
            host.TryRegisterType(typeof(CVector3), (byte)'E', CVector3.Serialize, CVector3.Deserialize);
            host.TryRegisterType(typeof(CRegistration), (byte)'F', CRegistration.Serialize, CRegistration.Deserialize);
            host.TryRegisterType(typeof(CLogout), (byte)'G', CLogout.Serialize, CLogout.Deserialize);
            host.TryRegisterType(typeof(CUpdateItem), (byte)'H', CUpdateItem.Serialize, CUpdateItem.Deserialize);
            host.TryRegisterType(typeof(CAchievements), (byte)'I', CAchievements.Serialize, CAchievements.Deserialize);
            host.TryRegisterType(typeof(CLeaderboard), (byte)'J', CLeaderboard.Serialize, CLeaderboard.Deserialize);

            host.TryRegisterType(typeof(Test), (byte)'1', Test.Serialize, Test.Deserialize);

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
                case (byte)EvCode.LEADERBOARD:
                    {
                        Leaderboard(info);
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
            //int[] testArray = new int[3]
            //{
            //    1,2,3
            //};
            //Test test = new Test("wtf", "lol", testArray);

            //CPlayer player = new CPlayer("hello", 1, "a", new CVector3(), new CVector3(), new CSound());

            //PluginHost.BroadcastEvent(recieverActors: new List<int>() { { info.ActorNr } },
            //    senderActor: 0,
            //    evCode: info.Request.EvCode,
            //    data: new Dictionary<byte, object>() { { 245, player }, { 254, 0 } },
            //    cacheOp: CacheOperations.DoNotCache);

            //Test test = Test.Deserialize((byte[])info.Request.Data) as Test;
            //string sql = "INSERT INTO test (idtest) VALUES ('" + test.TestArray[0] + "')";
            //MySqlCommand cmd = new MySqlCommand(sql, conn);
            //cmd.ExecuteNonQuery();
        }

        void Login(IRaiseEventCallInfo info)
        {
            // check if both username and password is right
            CLogin detail = CLogin.Deserialize((byte[])info.Request.Data) as CLogin;

            // Check if username is in database
            string sql = "SELECT * FROM account WHERE username = '" + detail.Username + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();

            int accountID = 0; string username = null; string password = null; string salt = null;
            while (rdr.Read())
            {
                accountID = rdr.GetInt32(0);
                username = rdr.GetString(1);
                password = rdr.GetString(2);
                salt = rdr.GetString(3);
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

            // Init the player
            string message = "", playerName = "";
            float player_x = 0.0f, player_y = 0.0f, player_z = 0.0f;
            float pet_x = 0.0f, pet_y = 0.0f, pet_z = 0.0f;
            float master = 0.0f, bgm = 0.0f, sfx = 0.0f;
            float timeOnline = 0.0f;

            EncryptPassword pw = new EncryptPassword();
            string digest = "";
            if (salt != null)
                digest = pw.GetDigest(detail.Password, salt, System.Security.Cryptography.SHA256.Create());

            if (username == null || digest != password)
                message = "Unsuccessful, Message=Incorrect username/password";
            else if (accActive)
                message = "Unsuccessful, Message=Account is currently active";
            else
            {
                message = "Successful";

                // get player name & position
                sql = "SELECT * FROM player WHERE account_id ='" + accountID + "'";
                cmd.CommandText = sql;
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    playerName = rdr.GetString(1);
                    player_x = rdr.GetFloat(2);
                    player_y = rdr.GetFloat(3);
                    player_z = rdr.GetFloat(4);
                    timeOnline = rdr.GetFloat(5);
                }
                rdr.Close();

                // get pet's position
                sql = "SELECT * FROM pet WHERE account_id ='" + accountID + "'";
                cmd.CommandText = sql;
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    pet_x = rdr.GetFloat(1);
                    pet_y = rdr.GetFloat(2);
                    pet_z = rdr.GetFloat(3);
                }
                rdr.Close();

                // get the audio settings
                sql = "SELECT * FROM sound WHERE account_id='" + accountID + "'";
                cmd.CommandText = sql;
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    master = rdr.GetFloat(1);
                    bgm = rdr.GetFloat(2);
                    sfx = rdr.GetFloat(3);
                }
                rdr.Close();

                // insert account_id into active table list (to state that account has someone playing)
                sql = "INSERT INTO active_users (account_id) VALUES ('" + accountID + "')";
                cmd.CommandText = sql;
                //cmd.ExecuteNonQuery();
            }

            CPlayer player = new CPlayer(message, accountID, playerName, timeOnline, new CVector3(player_x, player_y, player_z), new CVector3(pet_x, pet_y, pet_z), new CSound(master, bgm, sfx));
            //string returnMessage = "hello";
            PluginHost.BroadcastEvent(recieverActors: new List<int>() { { info.ActorNr } },
                                                  senderActor: 0,
                                                  evCode: info.Request.EvCode,
                                                  data: new Dictionary<byte, object>() { { 245, player }, { 254, 0 } },
                                                  cacheOp: CacheOperations.DoNotCache);
        }

        void Registration(IRaiseEventCallInfo info)
        {
            CRegistration registration = CRegistration.Deserialize((byte[])info.Request.Data) as CRegistration;

            // Check if username is in database
            string sql = "SELECT * FROM account WHERE username ='" + registration.Username + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();

            string returnMessage = "";

            if (rdr.HasRows)
                returnMessage = "Unsuccessful, Message=Username exists already";
            rdr.Close();

            sql = "SELECT * FROM player WHERE player_name ='" + registration.PlayerName + "'";
            cmd.CommandText = sql;
            rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
                returnMessage = "Unsuccessful, Message=Character Name exist already";
            rdr.Close();

            // There's no error, thus registration is successful!
            if (returnMessage == "")
            {
                // insert into database
                sql = "INSERT INTO account (username, password, salt, date_created) VALUES ('" + registration.Username + "', '" + registration.Hash.Digest + "', '" + registration.Hash.Salt + "', now())";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();

                // get account_id from account
                sql = "SELECT * FROM account WHERE username ='" + registration.Username + "'";
                cmd.CommandText = sql;
                rdr = cmd.ExecuteReader();
                int accountID = 0;
                while (rdr.Read())
                    accountID = rdr.GetInt32(0);
                rdr.Close();

                // insert into player table
                sql = "INSERT INTO player (account_id, player_name, pos_x, pos_y, pos_z, time_online) VALUES ('" + accountID + "', '" + registration.PlayerName + "', '51.09559', '3.4', '44.22058', '0.0')";
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
            CLogout logout = CLogout.Deserialize((byte[])info.Request.Data) as CLogout;

            //// delete user from active list
            //string sql = "DELETE FROM active_users WHERE account_id='" + message[0] + "'";
            string sql = "UPDATE player SET pos_x='" + logout.PlayerPosition.x + "', pos_y='" + logout.PlayerPosition.y + "', pos_z='" + logout.PlayerPosition.z + "', time_online='" + logout.TimeOnline  + "' WHERE account_id ='" + logout.AccountID + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();

            sql = "UPDATE pet SET pos_x='" + logout.PetPosition.x + "', pos_y='" + logout.PetPosition.y + "', pos_z='" + logout.PetPosition.z + "' WHERE account_id ='" + logout.AccountID + "'";
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            sql = "UPDATE sound SET master='" + logout.Sound.Master + "', bgm='" + logout.Sound.Bgm + "', sfx='" + logout.Sound.Sfx + "' WHERE account_id ='" + logout.AccountID + "'";
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            sql = "DELETE FROM active_users WHERE account_id = '" + logout.AccountID + "'";
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        void ResetPassword(IRaiseEventCallInfo info)
        {
            //string[] message = (string[])info.Request.Data;
            CRegistration reset = CRegistration.Deserialize((byte[])info.Request.Data) as CRegistration;
            string sql = "SELECT * FROM account WHERE username ='" + reset.Username + "'";
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
                string salt = "";
                // check previous password if correct
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    prevPassword = rdr.GetString(2);
                    salt = rdr.GetString(3);
                }
                rdr.Close();

                // check if prev match with the user
                bool matches = false;

                EncryptPassword pw = new EncryptPassword();
                string digest = "";
                if (salt != null)               // lazy to create new class, so using PlayerName to store prev pw
                    digest = pw.GetDigest(reset.PlayerName, salt, System.Security.Cryptography.SHA256.Create());

                if (prevPassword == digest)
                    matches = true;
                if (matches)
                {
                    // update the password
                    sql = "UPDATE account SET password='" + reset.Hash.Digest + "', salt='" + reset.Hash.Salt + "' WHERE username ='" + reset.Username + "'";
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
            while (rdr.Read() && i < 9)
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
            CUpdateItem item = CUpdateItem.Deserialize((byte[])info.Request.Data) as CUpdateItem;

            string sql = "";
            MySqlCommand cmd;

            if (item.Message[0] == 'I')
            {
                //string[] returnMessage = new string[2]
                //{
                //    message[0],
                //    "NULL",
                //};

                // insert into db
                sql = "INSERT INTO item (account_id) VALUES ('" + item.AccountID + "')";
                cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                // get the item_id and return to the player
                sql = "SELECT * FROM item WHERE account_id='" + item.AccountID + "'";
                cmd.CommandText = sql;
                MySqlDataReader rdr = cmd.ExecuteReader();

                int itemID = 0;
                while (rdr.Read())
                    itemID = rdr.GetInt32(0);
                rdr.Close();

                // returns the message to the client
                PluginHost.BroadcastEvent(recieverActors: new List<int>() { { info.ActorNr } },
                                          senderActor: 0,
                                          evCode: info.Request.EvCode,
                                          data: new Dictionary<byte, object>() { { 245, itemID }, { 254, 0 } },
                                          cacheOp: CacheOperations.DoNotCache);

                // if the codes still run after sending message to client, return
                return;
            }
            else if (item.Message[0] == 'U')
            {
                // update the account_id when player drop/pick_up the item
                sql = "UPDATE item SET account_id='" + item.AccountID + "' WHERE item_id='" + item.ItemID + "'";
                cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                if (item.Message[1] == '1')
                    return;

            }
            else if (item.Message[0] == 'D')
            {
                // delete the item where item_id is the one received
                sql = "DELETE FROM item WHERE item_id ='" + item.ItemID + "'";
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

        void Leaderboard(IRaiseEventCallInfo info)
        {
            // using this to store the top players 
            string[] topPlayers = new string[3];

            // getting the number of players in the db
            string sql = "SELECT COUNT('account_id') FROM player";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            int count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

            float[][] score = new float[count][];

            sql = "SELECT * FROM player";
            cmd.CommandText = sql;
            MySqlDataReader rdr = cmd.ExecuteReader();

            int i = 0;
            while (rdr.Read())
            {
                float[] temp = new float[2]
                {
                    rdr.GetInt32(0),
                    rdr.GetFloat(5),
                };
                score[i] = temp;
                ++i;
            }
            rdr.Close();

            float[] tempScore = new float[count];
            for (int j = 0; j < count; ++j)
                tempScore[j] = score[j][1];

            // sort from small to big
            Array.Sort(tempScore);

            // making it decending (big to small)
            Array.Reverse(tempScore);

            float[] topScore = new float[3];
            for (int j = 0; j < topScore.Length; ++j)
                topScore[j] = tempScore[j];

            int[] account_id = new int[3];
            i = 0;
            for (int y = 0; y < count; ++y)
            {
                for (int x = 0; x < topScore.Length; ++x)
                {
                    if (topScore[y] != score[x][1])
                        continue;

                    account_id[i] = (int)score[x][0];
                    ++i;
                    break;
                }
            }

            for (i = 0; i < 3; ++i)
            {
                sql = "SELECT * FROM player WHERE account_id ='" + account_id[i] + "'";
                cmd.CommandText = sql;
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                    topPlayers[i] = rdr.GetString(1);
                rdr.Close();
            }

            CLeaderboard ldr = new CLeaderboard(topPlayers);
            PluginHost.BroadcastEvent(recieverActors: new List<int>() { { info.ActorNr } },
                                                              senderActor: 0,
                                                              evCode: info.Request.EvCode,
                                                              data: new Dictionary<byte, object>() { { 245, ldr }, { 254, 0 } },
                                                              cacheOp: CacheOperations.DoNotCache);
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

//void Login(IRaiseEventCallInfo info)
//{
//    // check if both username and password is right
//    string[] message = (string[])info.Request.Data;

//    string recvUsername = message[0];
//    string recvPassword = message[1];

//    // Check if username is in database
//    string sql = "SELECT * FROM account WHERE username = '" + recvUsername + "'";
//    MySqlCommand cmd = new MySqlCommand(sql, conn);
//    MySqlDataReader rdr = cmd.ExecuteReader();

//    int accountID = 0; string username = null; string password = null;
//    while (rdr.Read())
//    {
//        accountID = rdr.GetInt32(0);
//        username = rdr.GetString(1);
//        password = rdr.GetString(2);
//    }
//    rdr.Close();

//    // check if the account is currently active
//    bool accActive = false;
//    sql = "SELECT * FROM active_users WHERE account_id ='" + accountID + "'";
//    cmd.CommandText = sql;
//    rdr = cmd.ExecuteReader();
//    if (rdr.HasRows)
//        accActive = true;
//    rdr.Close();

//    string[] returnMessage = new string[12]
//    {
//        "NULL",
//        "NULL",
//        "NULL",
//        "NULL",
//        "NULL",
//        "NULL",
//        "NULL",
//        "NULL",
//        "NULL",
//        "NULL",
//        "NULL",
//        "NULL",
//    };
//    if (username == null || password != recvPassword)
//        returnMessage[0] = "Unsuccessful, Message=Incorrect username/password";
//    else if (accActive)
//        returnMessage[0] = "Unsuccessful, Message=Account is currently active";
//    else
//    {
//        // get player name & position
//        sql = "SELECT * FROM player WHERE account_id ='" + accountID + "'";
//        cmd.CommandText = sql;
//        rdr = cmd.ExecuteReader();
//        returnMessage[0] = "Successful";
//        returnMessage[1] = accountID.ToString();

//        while (rdr.Read())
//        {
//            returnMessage[2] = rdr.GetString(1);
//            returnMessage[3] = rdr.GetFloat(2).ToString();
//            returnMessage[4] = rdr.GetFloat(3).ToString();
//            returnMessage[5] = rdr.GetFloat(4).ToString();
//        }
//        rdr.Close();

//        // get pet's position
//        sql = "SELECT * FROM pet WHERE account_id ='" + accountID + "'";
//        cmd.CommandText = sql;
//        rdr = cmd.ExecuteReader();
//        while (rdr.Read())
//        { 
//            returnMessage[6] = rdr.GetFloat(1).ToString();
//            returnMessage[7] = rdr.GetFloat(2).ToString();
//            returnMessage[8] = rdr.GetFloat(3).ToString();
//        }
//        rdr.Close();

//        // get the audio settings
//        sql = "SELECT * FROM sound WHERE account_id='" + accountID + "'";
//        cmd.CommandText = sql;
//        rdr = cmd.ExecuteReader();
//        while (rdr.Read())
//        {
//            returnMessage[9] = rdr.GetFloat(1).ToString();
//            returnMessage[10] = rdr.GetFloat(2).ToString();
//            returnMessage[11] = rdr.GetFloat(3).ToString();
//        }
//        rdr.Close();

//        // insert account_id into active table list (to state that account has someone playing)
//        sql = "INSERT INTO active_users (account_id) VALUES ('" + accountID + "')";
//        cmd.CommandText = sql;
//        cmd.ExecuteNonQuery();
//    }

//    PluginHost.BroadcastEvent(recieverActors: new List<int>() { { info.ActorNr } },
//                              senderActor: 0,
//                              evCode: info.Request.EvCode,
//                              data: new Dictionary<byte, object>() { { 245, returnMessage }, { 254, 0 } },
//                              cacheOp: CacheOperations.DoNotCache);
//}

//void Registration(IRaiseEventCallInfo info)
//{
//    string[] message = (string[])info.Request.Data;

//    string recvUsername = message[0];
//    string recvPassword = message[1];
//    string recvPlayerName = message[2];

//    // Check if username is in database
//    string sql = "SELECT * FROM account WHERE username ='" + recvUsername + "'";
//    MySqlCommand cmd = new MySqlCommand(sql, conn);
//    MySqlDataReader rdr = cmd.ExecuteReader();

//    string returnMessage = "";

//    if (rdr.HasRows)
//        returnMessage = "Unsuccessful, Message=Username exists already";
//    rdr.Close();

//    sql = "SELECT * FROM player WHERE player_name ='" + recvPlayerName + "'";
//    cmd.CommandText = sql;
//    rdr = cmd.ExecuteReader();
//    if (rdr.HasRows)
//        returnMessage = "Unsuccessful, Message=Character Name exist already";
//    rdr.Close();

//    // There's no error, thus registration is successful!
//    if (returnMessage == "")
//    {
//        // insert into database
//        sql = "INSERT INTO account (username, password, date_created) VALUES ('" + recvUsername + "', '" + recvPassword + "', now())";
//        cmd.CommandText = sql;
//        cmd.ExecuteNonQuery();

//        // get account_id from account
//        sql = "SELECT * FROM account WHERE username ='" + recvUsername + "'";
//        cmd.CommandText = sql;
//        rdr = cmd.ExecuteReader();
//        int accountID = 0;
//        while (rdr.Read())
//            accountID = rdr.GetInt32(0);
//        rdr.Close();

//        // insert into player table
//        sql = "INSERT INTO player (account_id, player_name, pos_x, pos_y, pos_z) VALUES ('" + accountID + "', '" + recvPlayerName + "', '51.09559', '3.4', '44.22058')";
//        cmd.CommandText = sql;
//        cmd.ExecuteNonQuery();

//        // insert into pet table
//        sql = "INSERT INTO pet (account_id, pos_x, pos_y, pos_z) VALUES ('" + accountID + "', '51.0956', '2.3454', '43.52058')";
//        cmd.CommandText = sql;
//        cmd.ExecuteNonQuery();

//        sql = "INSERT INTO sound (account_id, master, bgm, sfx) VALUES ('" + accountID + "', '0.0', '0.0', '0.0')";
//        cmd.CommandText = sql;
//        cmd.ExecuteNonQuery();

//        returnMessage = "Successful, Message=Registration Complete!";
//    }

//    // returns the message to the client
//    PluginHost.BroadcastEvent(recieverActors: new List<int>() { { info.ActorNr } },
//                              senderActor: 0,
//                              evCode: info.Request.EvCode,
//                              data: new Dictionary<byte, object>() { { 245, returnMessage }, { 254, 0 } },
//                              cacheOp: CacheOperations.DoNotCache);
//}



//// check if the account is currently active
//bool accActive = false;
//sql = "SELECT * FROM active_users WHERE account_id ='" + accountID + "'";
//            cmd.CommandText = sql;
//            rdr = cmd.ExecuteReader();
//            if (rdr.HasRows)
//                accActive = true;
//rdr.Close();

//            // Init the player
//            string message = "", playerName = "";
//float player_x = 0.0f, player_y = 0.0f, player_z = 0.0f;
//float pet_x = 0.0f, pet_y = 0.0f, pet_z = 0.0f;
//float master = 0.0f, bgm = 0.0f, sfx = 0.0f;

//EncryptPassword pw = new EncryptPassword();
//string digest = pw.GetDigest(detail.Password, salt, System.Security.Cryptography.SHA256.Create());

//            if (username == null || digest != password)
//                message = "Unsuccessful, Message=Incorrect username/password";
//            else if (accActive)
//                message = "Unsuccessful, Message=Account is currently active";
//            else
//            {
//                message = "Successful";

//                // get player name & position
//                sql = "SELECT * FROM player WHERE account_id ='" + accountID + "'";
//                cmd.CommandText = sql;
//                rdr = cmd.ExecuteReader();

//                while (rdr.Read())
//                {
//                    playerName = rdr.GetString(1);
//                    player_x = rdr.GetFloat(2);
//                    player_y = rdr.GetFloat(3);
//                    player_z = rdr.GetFloat(4);
//                }
//                rdr.Close();

//                // get pet's position
//                sql = "SELECT * FROM pet WHERE account_id ='" + accountID + "'";
//                cmd.CommandText = sql;
//                rdr = cmd.ExecuteReader();
//                while (rdr.Read())
//                {
//                    pet_x = rdr.GetFloat(1);
//                    pet_y = rdr.GetFloat(2);
//                    pet_z = rdr.GetFloat(3);
//                }
//                rdr.Close();

//                // get the audio settings
//                sql = "SELECT * FROM sound WHERE account_id='" + accountID + "'";
//                cmd.CommandText = sql;
//                rdr = cmd.ExecuteReader();
//                while (rdr.Read())
//                {
//                    master = rdr.GetFloat(1);
//                    bgm = rdr.GetFloat(2);
//                    sfx = rdr.GetFloat(3);
//                }
//                rdr.Close();

//                // insert account_id into active table list (to state that account has someone playing)
//                sql = "INSERT INTO active_users (account_id) VALUES ('" + accountID + "')";
//                cmd.CommandText = sql;
//                cmd.ExecuteNonQuery();
//            }

//            CPlayer player = new CPlayer(message, accountID, playerName, new CVector3(player_x, player_y, player_z), new CVector3(pet_x, pet_y, pet_z), new CSound(master, bgm, sfx));
////string returnMessage = "hello";
//PluginHost.BroadcastEvent(recieverActors: new List<int>() { { info.ActorNr } },
//                                      senderActor: 0,
//                                      evCode: info.Request.EvCode,
//                                      data: new Dictionary<byte, object>() { { 245, player }, { 254, 0 } },
//                                      cacheOp: CacheOperations.DoNotCache);

//// using this to store the top players 
//string[] topPlayers = new string[3];

//// getting the number of players in the db
//string sql = "SELECT COUNT('account_id') FROM player";
//MySqlCommand cmd = new MySqlCommand(sql, conn);
//int count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

//float[][] score = new float[count][];

//sql = "SELECT * FROM player";
//            cmd.CommandText = sql;
//            MySqlDataReader rdr = cmd.ExecuteReader();

//int i = 0;
//            while (rdr.Read())
//            {
//                float[] temp = new float[2]
//                {
//                                    rdr.GetInt32(0),
//                                    rdr.GetFloat(5),
//                };
//score[i] = temp;
//                ++i;
//            }
//            rdr.Close();

//            float[] tempScore = new float[count];
//            for (int j = 0; j<count; ++j)
//                tempScore[j] = score[j][1];

//            // sort from small to big
//            Array.Sort(tempScore);

//            // making it decending (big to small)
//            Array.Reverse(tempScore);

//            float[] topScore = new float[3];
//            for (int j = 0; j<topScore.Length; ++j)
//                topScore[j] = tempScore[j];

//            int[] account_id = new int[3];
//i = 0;
//            for (int y = 0; y<count; ++y)
//            {
//                for (int x = 0; x<topScore.Length; ++x)
//                {
//                    if (topScore[y] != score[x][1])
//                        continue;

//                    account_id[i] = (int) score[x][0];
//                    ++i;
//                    break;
//                }
//            }

//            for (i = 0; i< 3; ++i)
//            {
//                sql = "SELECT * FROM player WHERE account_id ='" + account_id[i] + "'";
//                cmd.CommandText = sql;
//                rdr = cmd.ExecuteReader();

//                while (rdr.Read())
//                    topPlayers[i] = rdr.GetString(1);
//                rdr.Close();
//            }

//            CLeaderboard ldr = new CLeaderboard(topPlayers);
//PluginHost.BroadcastEvent(recieverActors: new List<int>() { { info.ActorNr } },
//                                                  senderActor: 0,
//                                                  evCode: info.Request.EvCode,
//                                                  data: new Dictionary<byte, object>() { { 245, ldr }, { 254, 0 } },
//                                                  cacheOp: CacheOperations.DoNotCache);