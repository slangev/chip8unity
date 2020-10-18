using System.Collections.Generic;
using System.IO;

public class Memory
{
    public List<byte> memory;
    private uint size = 4096;
    private uint fontSize = 80;

    private uint romSize;

    private byte[] fonts = {
        0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
		0x20, 0x60, 0x20, 0x20, 0x70, // 1
		0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
		0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
		0x90, 0x90, 0xF0, 0x10, 0x10, // 4
		0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
		0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
		0xF0, 0x10, 0x20, 0x40, 0x40, // 7
		0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
		0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
		0xF0, 0x90, 0xF0, 0x90, 0x90, // A
		0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
		0xF0, 0x80, 0x80, 0x80, 0xF0, // C
		0xE0, 0x90, 0x90, 0x90, 0xE0, // D
		0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
		0xF0, 0x80, 0xF0, 0x80, 0x80  // F
    };


    public Memory()
    {
        memory = new List<byte>(new byte[size]);
        for(int i = 0; i < fontSize; i++) {
            memory[i] = fonts[i];
        }
    }

    public void LoadRom(string rom, ushort startpc) {
        byte[] bytes = File.ReadAllBytes(rom);
        romSize = (uint)bytes.Length;
        for(int i = 0; i < bytes.Length; i++) {
            memory[startpc + i] = bytes[i];
        }
    }

    public string DumpProgramMemory(ushort startpc) {
        string result = "";
        for(int i = 0; i < romSize; i++) {
            result = result + "Pos: " + "[" + (startpc + (ushort)i) + "] " + " 0x" + memory[startpc + i].ToString("X2") + " ";
        }
        return result;
    }

    public string PrintMemory() {
        string result = "";
        for(int i = 0; i < memory.Count; i++) {
            result = result + "Pos: " + "[" + i + "] " + " 0x" + memory[i].ToString("X2") + " ";
        }
        return result.Substring(0,result.Length-1);
    }
}
