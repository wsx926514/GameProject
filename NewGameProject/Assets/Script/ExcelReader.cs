using UnityEngine;
using System.Collections.Generic;   
using System.IO;    
using ExcelDataReader;  
using System.Text;
using System.Collections;


public class ExcelReader
{
public struct ExcelData
    {
        public string speakerName;
     
        public string speakingContent;

        public string avatarImageFileName; // Optional field for avatar image file name

        public string vocalAudioName; // Optional field for vocal audio file name

        public string backgroundImageFileName;

        public string backgroundMusicFileName;

        public string character1Action; // Optional field for character action

        public string coordinateX1;
        
        public string character1ImageFileName;

        public string character2Action;

        public string coordinateX2;

        public string character2ImageFileName;

        public string lastBackgroundImage;

        public string lastBackgroundMusic;
        
        public string lastCoordinateX1;
        
        public string lastCoordinateX2;

        public string englishName;

        public string englishContent;

        public string japaneseName;

        public string japaneseContent;
    }
    public static List<ExcelData> ReadExcel(string filePath)
    {
        List<ExcelData> excelData = new List<ExcelData>();
       Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                do
                {
                    while (reader.Read())
                    {
                        ExcelData data = new ExcelData();
                        data.speakerName = reader.IsDBNull(0) ? string.Empty : reader.GetValue(0)?.ToString();
                        data.speakingContent = reader.IsDBNull(1) ? string.Empty : reader.GetValue(1)?.ToString();
                        data.avatarImageFileName = reader.IsDBNull(2) ? string.Empty : reader.GetValue(2)?.ToString();
                        data.vocalAudioName = reader.IsDBNull(3) ? string.Empty : reader.GetValue(3)?.ToString();
                        data.backgroundImageFileName = reader.IsDBNull(4) ? string.Empty : reader.GetValue(4)?.ToString();
                        data.backgroundMusicFileName = reader.IsDBNull(5) ? string.Empty : reader.GetValue(5)?.ToString();
                        data.character1Action = reader.IsDBNull(6) ? string.Empty : reader.GetValue(6)?.ToString();
                        data.coordinateX1 = reader.IsDBNull(7) ? string.Empty : reader.GetValue(7)?.ToString();
                        data.character1ImageFileName = reader.IsDBNull(8) ? string.Empty : reader.GetValue(8)?.ToString();
                        data.character2Action = reader.IsDBNull(9) ? string.Empty : reader.GetValue(9)?.ToString();
                        data.coordinateX2 = reader.IsDBNull(10) ? string.Empty : reader.GetValue(10)?.ToString();
                        data.character2ImageFileName = reader.IsDBNull(11) ? string.Empty : reader.GetValue(11)?.ToString();
                        data.lastBackgroundImage = reader.IsDBNull(12) ? string.Empty : reader.GetValue(12)?.ToString();
                        data.lastBackgroundMusic = reader.IsDBNull(13) ? string.Empty : reader.GetValue(13)?.ToString();
                        data.lastCoordinateX1 = reader.IsDBNull(14) ? string.Empty : reader.GetValue(14)?.ToString();
                        data.lastCoordinateX2 = reader.IsDBNull(15) ? string.Empty : reader.GetValue(15)?.ToString();
                        data.englishName = reader.IsDBNull(16) ? string.Empty : reader.GetValue(16)?.ToString();
                        data.englishContent = reader.IsDBNull(17) ? string.Empty : reader.GetValue(17)?.ToString();
                        data.japaneseName = reader.IsDBNull(18) ? string.Empty : reader.GetValue(18)?.ToString();
                        data.japaneseContent = reader.IsDBNull(19) ? string.Empty : reader.GetValue(19)?.ToString();
                        excelData.Add(data);

                           
                    }
            } while (reader.NextResult());
        }
        return excelData;
        }
    }
}
