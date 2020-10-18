using UnityEngine;
using System.Collections.Generic;

public class Chip8 : MonoBehaviour
{
    // Start is called before the first frame update
    Texture2D texture;
    CPUChip8 cpu;
    Memory memory;
    ScreenDisplay screen;

    List<int> keys;

    public string path = "";
    
    ushort width = 64;
    ushort height = 32;
    void Start()
    {
        cpu = new CPUChip8();
        memory = new Memory();
        screen = new ScreenDisplay(width,height);
        keys = new List<int>(16);
        memory.LoadRom(path, cpu.pc);
        //60hz
        Time.fixedDeltaTime = 0.01666666666666666667f;
        texture = new Texture2D(width, height);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, Screen.width, Screen.height), Vector2.zero,1);
        sprite.name = "Screen";
        GameObject.Find("Screen").GetComponent<SpriteRenderer>().sprite = sprite;
        Debug.Log(memory.DumpProgramMemory(cpu.pc));
    }

    private void listenForInput() {
        if(Input.GetKey(KeyCode.Alpha1)) {
            Debug.Log("Key 1 is down");
        } else if (Input.GetKey(KeyCode.Alpha2)){
            Debug.Log("Key 2 is down");
        } else if (Input.GetKey(KeyCode.Alpha3)){
            Debug.Log("Key 3 is down");
        } else if (Input.GetKey(KeyCode.Alpha4)){
            Debug.Log("Key 4 is down");
        } else if (Input.GetKey(KeyCode.Q)){
            Debug.Log("Key Q is down");
        } else if (Input.GetKey(KeyCode.W)){
            Debug.Log("Key W is down");
        } else if (Input.GetKey(KeyCode.E)){
            Debug.Log("Key E is down");
        } else if (Input.GetKey(KeyCode.R)){
            Debug.Log("Key R is down");
        } else if (Input.GetKey(KeyCode.A)){
            Debug.Log("Key A is down");
        } else if (Input.GetKey(KeyCode.S)){
            Debug.Log("Key S is down");
        } else if (Input.GetKey(KeyCode.D)){
            Debug.Log("Key D is down");
        } else if (Input.GetKey(KeyCode.F)){
            Debug.Log("Key F is down");
        } else if (Input.GetKey(KeyCode.Z)){
            Debug.Log("Key Z is down");
        } else if (Input.GetKey(KeyCode.X)){
            Debug.Log("Key X is down");
        } else if (Input.GetKey(KeyCode.C)){
            Debug.Log("Key C is down");
        } else if (Input.GetKey(KeyCode.V)){
            Debug.Log("Key V is down");
        }

        if(Input.GetKeyUp(KeyCode.Alpha1)) {
            Debug.Log("Key 1 is released");
        } else if (Input.GetKeyUp(KeyCode.Alpha2)) {
            Debug.Log("Key 2 is released");
        } else if (Input.GetKeyUp(KeyCode.Alpha3)) {
            Debug.Log("Key 3 is released");
        } else if (Input.GetKeyUp(KeyCode.Alpha4)) {
            Debug.Log("Key 4 is released");
        } else if (Input.GetKeyUp(KeyCode.Q)) {
            Debug.Log("Key Q is released");
        } else if (Input.GetKeyUp(KeyCode.W)) {
            Debug.Log("Key W is released");
        } else if (Input.GetKeyUp(KeyCode.E)) {
            Debug.Log("Key E is released");
        } else if (Input.GetKeyUp(KeyCode.R)) {
            Debug.Log("Key R is released");
        } else if (Input.GetKeyUp(KeyCode.A)) {
            Debug.Log("Key A is released");
        } else if (Input.GetKeyUp(KeyCode.S)) {
            Debug.Log("Key S is released");
        } else if (Input.GetKeyUp(KeyCode.D)) {
            Debug.Log("Key D is released");
        } else if (Input.GetKeyUp(KeyCode.F)) {
            Debug.Log("Key F is released");
        } else if (Input.GetKeyUp(KeyCode.Z)) {
            Debug.Log("Key Z is released");
        } else if (Input.GetKeyUp(KeyCode.X)) {
            Debug.Log("Key X is released");
        } else if (Input.GetKeyUp(KeyCode.C)) {
            Debug.Log("Key C is released");
        } else if (Input.GetKeyUp(KeyCode.V)) {
            Debug.Log("Key V is released");
        } 
    }

    private void cycle() 
    {
        byte highByte = memory.memory[cpu.pc++];
        byte lowByte = memory.memory[cpu.pc++]; // kk: An 8-bit value, the lowest 8 bits of the instruction
        ushort instruction = (ushort)((highByte << 8) | lowByte);
        byte opcode = (byte)((highByte & 0xF0) >> 0x4);
        ushort addr = (ushort)(instruction & 0x0FFF); // A 12-bit value, the lowest 12 bits of the instruction
        byte nibble = (byte)(lowByte & 0x0F); // A 4-bit value, the lowest 4 bits of the instruction
        byte x = (byte)(highByte & 0x0F); // A 4-bit value, the lower 4 bits of the high byte of the instruction
        byte y = (byte)((lowByte & 0xF0) >> 0x4); // A 4-bit value, the upper 4 bits of the low byte of the instruction
        Debug.Log("HIGHBYTE: " + highByte.ToString("X2") + " LOWBYTE(kk): " + lowByte.ToString("X2") + " INSTRUCTION: " + 
        instruction.ToString("X4") + " OPCODE: "+ opcode.ToString("X1") + " nnn: " + addr.ToString("X3") + " nibble: " + 
        nibble.ToString("X1") + " x: " + x.ToString("X1") + " y: " + y.ToString("X1"));
        if(opcode == 0x0) {
            //opcode starts with 0
            //CLS 
            if((byte)(highByte & 0x0F) == 0x00 && (byte)(lowByte & 0xFF) == 0xE0) {
                Debug.Log("OPCODE:CLEAR SCREEN");
                screen.CLS();
            } else if((byte)(highByte & 0x0F) == 0x00 && (byte)(lowByte & 0xFF) == 0xEE) {
                Debug.Log("Return from a subroutine.");
                cpu.RET();
            }  else {
                Debug.Log("SYSTEM OPCODE. Treating as NOOP");
                cpu.SYS();
            }
        } else if(opcode == 0x1) {
            Debug.Log("Jumping to addr: " + addr.ToString("X3"));
            cpu.JPAddr(addr);
        } else if(opcode == 0x2) {
            Debug.Log("Calling to addr: " + addr.ToString("X3"));
            cpu.CALLAddr(addr);
        } else if(opcode == 0x3) {
            Debug.Log("Skip next instruction if Vx = kk.");
            cpu.SEVxByte(x, lowByte);
        } else if(opcode == 0x4) {
            Debug.Log("Skip next instruction if Vx != kk.");
            cpu.SNEVxByte(x, lowByte);
        } else if(opcode == 0x5) {
            Debug.Log("Skip next instruction iff Vx = Vy."); 
            cpu.SEVxVy(x,y);
        } else if(opcode == 0x6) {
            Debug.Log("Set Vx = kk."); 
            cpu.LDVxByte(x,lowByte);
        } else if(opcode == 0x7) {
            Debug.Log("Set Vx = Vx + kk.");
            cpu.ADDVxByte(x,lowByte);
        } else if(opcode == 0x8) {
            if(nibble == 0x0) {
                Debug.Log("Set Vx = Vy");
                cpu.LDVxVy(x,y);
            } else if(nibble == 0x1) {
                Debug.Log("Set Vx = Vx OR Vy.");
                cpu.ORVxVy(x,y);
            } else if(nibble == 0x2) {
                Debug.Log("Set Vx = Vx AND Vy.");
                cpu.ANDVxVy(x,y);
            } else if(nibble == 0x3) {
                Debug.Log("Set Vx XOR Vy");
                cpu.XORVxVy(x,y);
            } else if(nibble == 0x4) {
                Debug.Log("Set Vx = Vx + Vy, set VF = carry.");
                cpu.ADDVxVy(x,y);
            } else if(nibble == 0x5) {
                Debug.Log("Set Vx = Vx - Vy, set VF = NOT borrow.");
                cpu.SUBVxVy(x,y);
            } else if(nibble == 0x6) {
                Debug.Log("Set Vx = Vx SHR 1.");
                cpu.SHRVx(x);
            } else if(nibble == 0x7) {
                Debug.Log("Set Vx = Vy - Vx, set VF = NOT borrow.");
                cpu.SUBNVxVy(x,y);
            } else if(nibble == 0xE) {
                Debug.Log("Set Vx = Vx SHL 1.");
                cpu.SHLVx(x);
            } else {
                Debug.Log("Unknown opcode for number 8. Treating as NOOP");
            }
        } else if(opcode == 0x9) {
            Debug.Log("Skip next instruction if Vx != Vy.");
            cpu.SNEVxVy(x,y);
        } else if(opcode == 0xA) {
            Debug.Log("Set I = nnn");
            cpu.LDIAddr(addr);
        } else if(opcode == 0xB) {
            Debug.Log("Jump to location nnn + V0");
            cpu.JPV0Addr(addr);
        } else if(opcode == 0xC) {
            Debug.Log("Set Vx = random byte AND kk");
            cpu.RNDVxKK(x,lowByte);
        } else if(opcode == 0xD) {
            Debug.Log("Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision.");
            cpu.DRWVxVyNibble(x,y,nibble,screen,memory);
            Debug.LogWarning(screen.PrintScreenMemory());
        } else if(opcode == 0xE) {
            if(lowByte == 0x9E) {
                Debug.Log("Skip next instruction if key with the value of Vx is pressed.");
                cpu.SKPVx(x,keys);
            } else if(lowByte == 0xA1) {
                Debug.Log("Skip next instruction if key with the value of Vx is not pressed.");
                cpu.SKPNVx(x,keys);
            }
        } else if(opcode == 0xF) {
            if(lowByte == 0x07) {
                Debug.Log("Set Vx = delay timer value.");
                cpu.LDVxDT(x);
            } else if(lowByte == 0x0A){
                Debug.Log("Wait for a key press, store the value of the key in Vx.");
                Debug.LogWarning("waiting opcode");
                //cpu.LDVxK(x,0x0);
            } else if(lowByte == 0x15) {
                Debug.Log("Set delay timer = Vx.");
                cpu.LDDTVx(x);
            } else if(lowByte == 0x18) {
                Debug.Log("Set sound timer = Vx.");
                cpu.LDSTVx(x);
            } else if(lowByte == 0x1E) {
                Debug.Log("Set I = I + Vx.");
                cpu.ADDIVx(x);
            } else if(lowByte == 0x29) {
                Debug.Log("Set I = location of sprite for digit Vx.");
                cpu.LDFVx(x);
            } else if(lowByte == 0x33) {
                Debug.Log("Store BCD representation of Vx in memory locations I, I+1, and I+2.");
                cpu.LDBVx(x,memory);
            } else if(lowByte == 0x55) {
                Debug.Log("Store registers V0 through Vx in memory starting at location I.");
                cpu.LDIVx(memory,x);
            } else if(lowByte == 0x65) {
                Debug.Log("Read registers V0 through Vx from memory starting at location I.");
                cpu.LDVxI(memory,x); 
            }
         } else {
             Debug.LogWarning("UNKNOWN OPCODE!!!");
         }
    }

    private void Update()
    {
        listenForInput();
        cycle();
    }

    private void FixedUpdate()
    {
        screen.DrawScreen(texture);
        cpu.CycleDelaySoundTimers();
    }
}
