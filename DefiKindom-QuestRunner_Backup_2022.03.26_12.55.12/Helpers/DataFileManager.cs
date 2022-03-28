using System;
using System.IO;
using System.Windows.Forms;

using Newtonsoft.Json;


namespace DefiKindom_QuestRunner.Helpers
{
    internal class DataFileManager
    {
        public enum DataFileTypes
        {
            Wallet
        }

 
        public T LoadDataFile<T>(DataFileTypes type)
        {
            try
            {
                var filePath = "";
                var fileContent = "";

                switch (type)
                {
                    case DataFileTypes.Wallet:
                        filePath = $"{Application.StartupPath}\\Data\\{type.ToString().ToLower().ToLower()}.dfk";

                        if (File.Exists(filePath))
                        {
                            fileContent = File.OpenText(filePath).ReadToEnd();

                            if (fileContent.Trim().Length > 0)
                                fileContent = Encryptor.Decrypt(fileContent);

                            //De-serialize datafile
                            return Deserialize<T>(fileContent);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {

            }


            return default(T);
        }

        public T LoadDataFile<T>(DataFileTypes type, string filePath)
        {
            try
            {
                var fileContent = "";

                switch (type)
                {
                    case DataFileTypes.Wallet:
                        if (File.Exists(filePath))
                        {
                            fileContent = File.OpenText(filePath).ReadToEnd();

                            //If a DFK file then we need to decrypt it before deserialization
                            if (filePath.Contains(".dfk"))
                                fileContent = Encryptor.Decrypt(fileContent);

                            //De-serialize datafile
                            return Deserialize<T>(fileContent);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {

            }


            return default(T);
        }

        public bool SaveDataFile<T>(DataFileTypes type, T data)
        {
            try
            {
                var fileName = "";
                var strObjectData = "";

                switch (type)
                {
                    case DataFileTypes.Wallet:
                        fileName = $"{Application.StartupPath}\\Data\\{type.ToString().ToLower().ToLower()}.dfk";
                        strObjectData = "";

                        if (data != null)
                        {
                            //First lets convert the object to json first!
                            strObjectData = Serialize(data);

                            if (strObjectData != null)
                            {
                                //Encrypt our data!
                                strObjectData = Encryptor.Encrypt(strObjectData);
                            }
                        }

                        //Write file (overwrites existing data)
                        File.WriteAllText(fileName, strObjectData);

                        return true;
                }
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        string Serialize<T>(T obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        T Deserialize<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }
}
