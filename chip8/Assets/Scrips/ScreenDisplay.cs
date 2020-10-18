using UnityEngine;
using System.Collections.Generic;

public class ScreenDisplay
{
    public List<List<byte>> videoMemory { get; set; }
    public ushort width { get; set; }
    public ushort height { get; set; }

    public ScreenDisplay(ushort width, ushort height)
    {
        this.width = width;
        this.height = height;
        videoMemory = new List<List<byte>>();
        for(int i = 0; i < this.height; i++) {
            videoMemory.Add( new List<byte>(new byte[this.width]));
        }
    }

    public void CLS() {
        ClearVideoMemory();
    }
    
    private void ClearVideoMemory() {
        for(int i = 0; i < videoMemory.Count; i++) {
            for(int j = 0; j < videoMemory[i].Count; j++) {
                videoMemory[i][j] = 0;
            }
        }
    }

    public string PrintScreenMemory() {
        string result = "";

        for(int i = 0; i < videoMemory.Count; i++) {
            for(int j = 0; j < videoMemory[i].Count; j++) {
                result = result + videoMemory[i][j] + " ";
            }
            result = result + "\n";
        }
        return result.Substring(0,result.Length-1);
    }

    public void DrawScreen(Texture2D texture)
    {
        int row = height-1;
        for (int x = 0; x < texture.height; x++)
        {
            int col = 0;
            for (int y = 0; y < texture.width; y++)
            {
                Color pixelColour;
                //Random.Range(0,2); 50/50 chance it will be 0 or 1
                if (videoMemory[row][col++] == 0) 
                {
                    pixelColour = new Color(0, 0, 0, 1); //Black
                }
                else
                {
                    pixelColour = new Color(1, 1, 1, 1); //White
                }
                texture.SetPixel(y, x, pixelColour);
            }
            row--;
        }
        texture.Apply();
    }
}