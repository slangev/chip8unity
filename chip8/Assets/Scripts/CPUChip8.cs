using System.Collections.Generic;
using UnityEngine;

public class CPUChip8
{
    public List<byte> gpReg; 

    //This register is generally used to store memory addresses, so only the lowest (rightmost) 12 bits are usually used.
    public ushort I { get; set; }

    private byte delayTimer { get; set; }
    private byte soundTimer { get; set; }

    public ushort pc { get; set; }

    public byte sp { get; set; }

    public List<ushort> stack;

    public List<ushort> keypad;

    public CPUChip8()
    {
        gpReg = new List<byte>(new byte[16]);
        I = 0x0;
        delayTimer = 0x0;
        soundTimer = 0x0;
        pc = 0x200;
        sp = 0x0;
        stack = new List<ushort>(new ushort[16]);
        keypad = new List<ushort>(new ushort[16]);
    }

    public void CycleDelaySoundTimers() {
        if(delayTimer > 0) {
            delayTimer--;
        }

        if(soundTimer > 0) {
            //play sound / beep
            soundTimer--;
        }
    }

    public void SYS() {
        // NO OP Jump to a machine code routine at nnn.
    }

    public void RET() {
        pc = stack[sp];
        sp--;
    }

    public void JPAddr(ushort addr) {
        pc = addr;
    }

    public void CALLAddr(ushort addr) {
        sp++;
        stack[sp] = pc;
        pc = addr;
    }

    public void SEVxByte(byte x, byte kk) {
        if(gpReg[x] == kk) {
            pc = (ushort)(pc + 2);
        }
    }

    public void SNEVxByte(byte x, byte kk) {
        if(gpReg[x] != kk) {
            pc = (ushort)(pc + 2);
        }
    }

    public void SEVxVy(byte x, byte y) {
        if(gpReg[x] == gpReg[y]) {
            pc = (ushort)(pc + 2);
        }
    }

    public void LDVxByte(byte x, byte kk) {
        gpReg[x] = kk;
    }

    public void ADDVxByte(byte x, byte kk) {
        gpReg[x] += kk;
    }

    public void LDVxVy(byte x, byte y) {
        gpReg[x] = gpReg[y];
    }

    public void ORVxVy(byte x, byte y) {
        gpReg[x] = (byte)(gpReg[x] | gpReg[y]);
    }

    public void ANDVxVy(byte x, byte y) {
        gpReg[x] = (byte)(gpReg[x] & gpReg[y]);
    }

    public void XORVxVy(byte x, byte y) {
        gpReg[x] = (byte)(gpReg[x] ^ gpReg[y]);
    }

    public void ADDVxVy(byte x, byte y) {
        ushort carry = (ushort)(gpReg[x] + gpReg[y]);
        gpReg[0xF] = (carry > 255) ? (byte)1 : (byte)0;
        gpReg[x] = (byte)(carry & 0x00FF);
    }

    public void SUBVxVy(byte x, byte y) {
        if(gpReg[x] > gpReg[y]) {
            gpReg[0xF] = 1;
        } else {
            gpReg[0xF] = 0;           
        }
        gpReg[x] -= gpReg[y];
    }

    public void SHRVx(byte x) {
        byte leastSignificantBit = (byte)(gpReg[x] & (byte)(0x1));
        if(leastSignificantBit == (byte)(0x1)) {
            gpReg[0xF] = leastSignificantBit;
        } else {
            gpReg[0xF] = 0;
        }
        gpReg[x] /= 2;
    }

    public void SUBNVxVy(byte x, byte y) {
        if(gpReg[y] > gpReg[x]) {
            gpReg[0xF] = 1;
        } else {
            gpReg[0xF] = 0;
        }
        gpReg[y] -= gpReg[x];
    }

    public void SHLVx(byte x) {
        byte mostSignificantBit = (byte)((gpReg[x] & 0x80) >> 7);
        if(mostSignificantBit == (byte)(0x1)) {
            gpReg[0xF] = 1;
        } else {
            gpReg[0xF] = 0;
        }
        gpReg[x] *= 2;
    }

    public void SNEVxVy(byte x, byte y) {
        if(gpReg[x] != gpReg[y]) {
            pc += 2;
        }
    }

    public void LDIAddr(ushort addr) {
        I = addr;
    }

    public void JPV0Addr(ushort addr) {
        pc = (ushort)(addr + gpReg[0]);
    }

    public void RNDVxKK(byte x, byte kk) {
        byte rng = (byte)(Random.Range(0,256)); //0-255
        gpReg[x] = (byte)(rng & kk);
    }

    public void DRWVxVyNibble(byte x, byte y, byte nibble, ScreenDisplay screen, Memory memory) {
        gpReg[0xF] = 0;
        byte startX = gpReg[x];
        byte startY = gpReg[y];
        for(int i = 0; i < nibble; i++) {
            byte readByte = memory.memory[I + i]; // Read byte from memory starting at I.
            byte mask = 0x80; // Mask used to extract single bits.
            for(int j = 0; j < 8; j++) {
                byte bit = (byte)(readByte & mask);
                if(bit > 0) {
                    bit = 1;
                }
                mask = (byte)(mask >> 1);
                int rowIndex = (startY + i) % (screen.height); // -1 to make zero-based index
                int colIndex = (startX + j) % (screen.width); // -1 to make zero-based index
                
                byte vMBit = 0;
                vMBit = screen.videoMemory[rowIndex][colIndex];
                if(vMBit == 1 && bit == 1) {
                    gpReg[0xF] = 1;
                }
                screen.videoMemory[rowIndex][colIndex] = (byte)(screen.videoMemory[rowIndex][colIndex] ^ bit);
            }
        }
    }

    public void SKPVx(byte x, List<int> keys) {
        if(keys[gpReg[x]] == 1) {
            pc += 2;
        }
    }

    public void SKPNVx(byte x, List<int> keys) {
        if(keys[gpReg[x]] != 1) {
            pc += 2;
        }
    }

    public void LDVxDT(byte x) {
        gpReg[x] = delayTimer;
    }

    public void LDVxK(byte x, byte value) {
        gpReg[x] = value;
    }

    public void LDDTVx(byte x) {
        delayTimer = gpReg[x];
    }

    public void LDSTVx(byte x) {
        soundTimer = gpReg[x];
    }

    public void ADDIVx(byte x) {
        I = (ushort)(I + gpReg[x]);
    }

    public void LDFVx(byte x) {
        I = (ushort)(5 * gpReg[x]);
    }

    public void LDBVx(byte x, Memory memory) {
        byte decimalValue = gpReg[x];
        memory.memory[I+2] = (byte)(decimalValue % 10);
        decimalValue /= 10;
        memory.memory[I+1] = (byte)(decimalValue % 10);
        decimalValue /= 10;
        memory.memory[I] = (byte)(decimalValue % 10);
    }

    public void LDIVx(Memory memory, byte x) {
        for(int i = 0; i <= x; i++) {
            memory.memory[I+i] = gpReg[i];
        }
    }

    public void LDVxI(Memory memory, byte x) {
        for(int i = 0; i <= x; i++) {
            gpReg[i] = memory.memory[I + i];
        }
    }
}
