using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;

namespace ScrabbleCheat
{
    class Serializer
    {
        public void SerializeObject(string filename, SaveObject saveObject)
        {
            try
            {
                using (FileStream stream = File.Open(filename, FileMode.Create, FileAccess.ReadWrite))
                {
                    BinaryFormatter bFormatter = new BinaryFormatter();
                    bFormatter.Serialize(stream, saveObject);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public SaveObject DeSerializeObject(string filename)
        {
            SaveObject saveObject = new SaveObject();
            try
            {
                using (FileStream stream = File.Open(filename, FileMode.Open))
                {
                    BinaryFormatter bFormatter = new BinaryFormatter();
                    saveObject = (SaveObject)bFormatter.Deserialize(stream);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
//                MessageBox.Show("No saved data");
            }
            return saveObject;
        }
    }
}
