using System.Collections.Generic;
using System.IO;
using UnityEngine;

// provide following feature
// create a file given a name also provide the 
// 
public class CSVLogger : MonoBehaviour
{
    public string rootPath;
    public List<string> header;
    private string path;
    
    
    // called when the game starts
    public void Initialize(string fileName)
    {
        path = Path.Combine(rootPath, $"{fileName}.csv");//rootPath + "/" + fileName + ".csv";
        Debug.Log(path);
        // create a file

        using StreamWriter sw = new StreamWriter(path);
        sw.WriteLine(string.Join(",", header));

        // write the header
    }

    public void AddData(string newEntry)
    {
        using StreamWriter sw = new StreamWriter(path, true);
        sw.WriteLine(newEntry);
    }
}