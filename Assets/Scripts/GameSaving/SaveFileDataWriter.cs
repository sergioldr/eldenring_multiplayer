using UnityEngine;
using System;
using System.IO;

namespace SL
{
    public class SaveFileDataWriter
    {
        private string saveDataDirectoryPath = "";
        private string saveDataFileName = "";

        public bool CheckIfSaveDataFileExists()
        {
            return File.Exists(Path.Combine(saveDataDirectoryPath, saveDataFileName));
        }

        public void DeleteSavedDataFile()
        {
            if (CheckIfSaveDataFileExists())
            {
                File.Delete(Path.Combine(saveDataDirectoryPath, saveDataFileName));
            }
        }

        public void CreateNewSaveDataFile(object data)
        {
            string savePath = Path.Combine(saveDataDirectoryPath, saveDataFileName);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log("Save file created at: " + savePath);

                string dataToStore = JsonUtility.ToJson(data, true);

                using (FileStream fs = new FileStream(savePath, FileMode.Create))
                {
                    using (StreamWriter fileWriter = new StreamWriter(fs))
                    {
                        fileWriter.Write(dataToStore);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error creating save file at: " + savePath + "\n" + e.Message);
            }
        }

        public T LoadSavedFile<T>()
        {
            string loadPath = Path.Combine(saveDataDirectoryPath, saveDataFileName);

            try
            {
                if (File.Exists(loadPath))
                {
                    string dataToLoad = "";

                    using (FileStream fs = new FileStream(loadPath, FileMode.Open))
                    {
                        using (StreamReader fileReader = new StreamReader(fs))
                        {
                            dataToLoad = fileReader.ReadToEnd();
                        }
                    }

                    return JsonUtility.FromJson<T>(dataToLoad);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading save file at: " + loadPath + "\n" + e.Message);
            }

            return default;
        }

        public void SetSaveDataDirectoryPath(string path)
        {
            saveDataDirectoryPath = path;
        }

        public void SetSaveDataFileName(string name)
        {
            saveDataFileName = name;
        }
    }
}
