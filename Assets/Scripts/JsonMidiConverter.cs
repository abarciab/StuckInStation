using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class JsonMidiConverter : MonoBehaviour
{
    [SerializeField] private TextAsset _input;
    [SerializeField] private TextAsset _output;

    [ButtonMethod]
    private void Convert()
    {
        var lines = _input.text.Split("\n");

        var output = lines.Where(x => (x.Contains("time") || x.Contains("name")) && !(x.Contains("]") || x.Contains("["))).ToList();
        for (int i = 0; i < output.Count; i++) {
            if (output[i].Contains("time")) {
                var decodedTime = ConvertToTime(output[i]);
                output[i - 1] = decodedTime + "," + output[i - 1];
                output[i] = "";
            }
            else if (output[i].Contains("name")) output[i] = ConvertToNote(output[i]);
        }
        output = output.Where(x => !string.IsNullOrEmpty(x)).ToList();
        File.WriteAllText(AssetDatabase.GetAssetPath(_output), string.Join("\n", output));
    }

    private string ConvertToTime(string input)
    {
        var parts = input.Split(":");
        parts[1] = parts[1].Replace(",", "");
        float num = float.Parse(parts[1].Trim());
        //num *= 0.3f;
        return num.ToString();
    }

    private string ConvertToNote(string input)
    {
        if (input.ToLower().Contains("d2")) return "0";
        if (input.ToLower().Contains("f2")) return "1";
        if (input.ToLower().Contains("a2")) return "2";
        if (input.ToLower().Contains("c3")) return "3";
        return "";
    }
}
