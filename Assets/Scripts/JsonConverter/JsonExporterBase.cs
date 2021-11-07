using UnityEngine;
using System.IO;

public class JsonConverterBase {
    protected static string DataPath = $"{Application.dataPath}/Data";

    // NOTE : 해당 데이터를 json으로 저장
    protected static void ExportToJson(object data, string path, string fileName) {
        var json = JsonUtility.ToJson(data, true);
        var saveFile = $"{DataPath}/{path}/{fileName}";
        File.WriteAllText(saveFile, json);
    }
}
