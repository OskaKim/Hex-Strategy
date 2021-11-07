using UnityEngine;
using System.IO;

public class JsonConverterBase {
    protected static string DataPath = $"{Application.dataPath}/Data";

    // NOTE : 해당 데이터를 json으로 저장
    protected static void ExportToJson(object data, string path, string fileName) {
        var filePath = $"{DataPath}/{path}";
        try {
            if (!Directory.Exists(filePath)) {
                Directory.CreateDirectory(filePath);
                Debug.Log($"Directory {filePath} has created!");
            }

            var json = JsonUtility.ToJson(data, true);
            var saveFile = $"{DataPath}/{path}/{fileName}";
            File.WriteAllText(saveFile, json);
            Debug.Log($"Json {saveFile} has written!");
        }
        catch (IOException ex) {
            Debug.Log(ex.Message);
        }
    }
}
