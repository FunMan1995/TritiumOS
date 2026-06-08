/*
 * Based on <https://github.com/schierlm/oberon-risc-emu-enhanced>
 * Based on <https://github.com/pdewacht/oberon-risc-emu>
 *
 * Copyright (c) 2014 Peter De Wachter
 * Copyright (c) 2018-2024 Michael Schierl
 *
 * Permission to use, copy, modify, and/or distribute this software for
 * any purpose with or without fee is hereby granted, provided that the
 * above copyright notice and this permission notice appear in all
 * copies.
 *
 * THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL
 * WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE
 * AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL
 * DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR
 * PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER
 * TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
 * PERFORMANCE OF THIS SOFTWARE.
 */

#define ROMStart     0xFFFFF800
#define ROMWords     100
#define IOStart      0xFFFFFFC0
#define PaletteStart 0xFFFFFF80

// #define HW_ENUM_ID(a,b,c,d) (((a) << 24) | ((b) << 16) | ((c) << 8) | (d))

struct RISC {
  uint32_t PC;
  uint32_t R[16];
  uint32_t H;
  bool     Z, N, C, V;
  uint32_t mem_size;
  uint32_t display_start;
  uint32_t progress;
  uint32_t current_tick;
  uint32_t mouse;
  uint32_t key_buf[16];
  uint32_t key_cnt;
  STRUCT RISC_disk disk;
  int clipboard_index;
  int clipboard_len;
  int current_mode_width, current_mode_height;
  int current_mode_span; // in words
  uint32_t hwenum_buf[16];
  uint32_t hwenum_idx, hwenum_cnt;
  uint32_t *RAM;
};

STRUCT RISC risc_instance;
bool enable_leds, enable_console;

#define MOV 0
#define LSL 1
#define ASR 2
#define ROR 3
#define AND 4
#define ANN 5
#define IOR 6
#define XOR 7
#define ADD 8
#define SUB 9
#define MUL 10
#define DIV 11

const uint32_t ROM[ROMWords] = {
  0xE700003A, 0x00000000, 0x00000000, 0x00000000,
  0x00000000, 0x00000000, 0x00000000, 0x00000000,
  0x4EE90024, 0xAFE00000, 0x5000FFFC, 0x6100426F,
  0x41166F74, 0xA1000000, 0x5000FFFC, 0x80000000,
  0xA0E00010, 0x40000002, 0xA0E00004, 0x40000000,
  0xA0E00008, 0x5000FFE4, 0x81E00008, 0xA1000000,
  0x60008000, 0x81E00004, 0x00080001, 0x5100FFE4,
  0xA0100000, 0x80E00008, 0xE9000003, 0x40000010,
  0x80000000, 0xA0E0000C, 0x80E00004, 0x40080001,
  0xA0E00004, 0x80E00008, 0x40080400, 0xA0E00008,
  0x80E00008, 0x81E0000C, 0x00090001, 0xE5FFFFE9,
  0x80E00010, 0x61000007, 0x00080001, 0x40020014,
  0x40010013, 0x0B000000, 0x4000000C, 0x81E00010,
  0xA1000000, 0x0000000B, 0x41000018, 0xA0100000,
  0x8FE00000, 0x4EE80024, 0xC700000F, 0x5E00FFC0,
  0x4C000020, 0x0000000F, 0x40090000, 0xE9000006,
  0x60000008, 0x0E000000, 0x40000082, 0x5100FFC4,
  0xA0100000, 0xF7FFFFC2, 0x0000000B, 0x40090000,
  0xE1000003, 0x0000000B, 0x0E000000, 0xE700000B,
  0x40000018, 0x41000018, 0x42000001, 0x83000000,
  0x40080004, 0xA3100000, 0x41180004, 0x42290001,
  0xE9FFFFFA, 0x00000003, 0x0E000000, 0x40000084,
  0x5100FFC4, 0xA0100000, 0x40000000, 0xC7000000,
  0x00000100, 0x0000EC00, 0xFFFFFF00, 0x000000FF,
  0x00000000, 0x00000000, 0x0000EC00, 0x00004F00
};

uint32_t palette[16] = {
  0xffffff, 0xff0000, 0x00ff00, 0x0000ff, 0xff00ff, 0xffff00, 0x00ffff, 0xaa0000,
  0x009a00, 0x00009a, 0x0acbf3, 0x008282, 0x8a8a8a, 0xbebebe, 0xdfdfdf, 0x000000
};

STRUCT RISC *risc_new(int reset, int width, int height, uint32_t* ram, uint32_t ramsize, bool leds, bool console) {
  int i;
  STRUCT RISC *risc = &risc_instance;
  if (risc->mem_size == 0) {
    risc->mem_size = ramsize << 20;
    risc->display_start = risc->mem_size - (uint32_t)(width * height / 2);
    risc->current_mode_width = width;
    risc->current_mode_height = height;
    risc->current_mode_span = width / 8;
    risc->RAM = ram;
    reset = 2;
  } else {
    risc_update_damage(risc->RAM, palette, risc->display_start, risc->current_mode_span, risc->current_mode_height, -1);
  }
  if (reset > 0) {
    risc->PC = ROMStart/4;
    if (reset > 1) {
      for (i = 0; i < 16; i++) {
        risc->R[i] = 0;
      }
    }
  }
  enable_leds = leds;
  enable_console = console;
  return risc;
}

void show_leds(uint32_t value) {
  uint i;
  if (!enable_leds)
    return;
  stype("LEDs: ");
  for (i = 7; i >= 0; i--) {
    if (value & (1 << i)) {
      `. (i);
    } else {
      stype("-");
    }
  }
  stype("\n");
}

void dbg_print(char value) {
  if (!enable_console)
    return;
  if (enable_console && value != 0) {
    if (value == 13) value = 10;
    putchar((char) value);
  }
}

void paravirtual_write(STRUCT RISC_disk *disk, uint32_t value, uint32_t *RAM) {
  uint32_t sector;
  if ((value & 0xC0000000) == 0) { // setPtr
    disk->disk_paravirt_ptr = value;
  }
  if ((value & 0xC0000000) == 0x80000000) { // read
    sector = value & 0x3FFFFFFF;
    seek_sector(disk->disk_file, sector * 2 - 2);
    read_sector(disk->disk_file, &RAM[disk->disk_paravirt_ptr / 4]);
    read_sector(disk->disk_file, &RAM[disk->disk_paravirt_ptr / 4 + 128]);
  }
  if ((value & 0xC0000000) == 0xC0000000) { // write
    sector = value & 0x3FFFFFFF;
    seek_sector(disk->disk_file, sector * 2 - 2);
    if (disk->disk_paravirt_ptr == 0x3FFFFFFF) {
      truncate_sector(disk->disk_file);
    } else {
      write_sector(disk->disk_file, &RAM[disk->disk_paravirt_ptr / 4]);
      write_sector(disk->disk_file, &RAM[disk->disk_paravirt_ptr / 4 + 128]);
    }
  }
}

void risc_set_register(STRUCT RISC *risc, uint32_t reg, uint32_t value) {
  risc->R[reg] = value;
  risc->Z = value == 0;
  risc->N = (int32_t)value < 0;
}

uint32_t clipboard_control_read(STRUCT RISC *risc) {
  uint32_t r = risc_read_clipboard();
  risc->clipboard_index = 0;
  risc->clipboard_len = (int32_t)r;
  return r;
}

void clipboard_control_write(STRUCT RISC *risc, uint32_t len) {
  risc->clipboard_index = 0;
  risc->clipboard_len = (int32_t)len;
  risc_ensure_clipboard(len);
}

uint32_t clipboard_data_read(STRUCT RISC *risc) {
  uchar* clip = risc_get_clip_pointer();
  uint32_t result = 0;
  if (risc->clipboard_index < risc->clipboard_len) {
    result = (uint32_t)clip[risc->clipboard_index];
    risc->clipboard_index++;
  }
  if (result == 10) {
    result = 13;
  }
  return result;
}

void clipboard_data_write(STRUCT RISC *risc, uint32_t c) {
  uchar* clip;
  if (risc->clipboard_index < risc->clipboard_len) {
    if ((uchar)c == 13) {
      c = 10;
    }
    clip = risc_get_clip_pointer();
    clip[risc->clipboard_index] = (uchar)c;
    risc->clipboard_index++;
    if (risc->clipboard_index == risc->clipboard_len) {
      risc_set_clipboard((uint32_t)risc->clipboard_len);
    }
  }
}

void hosttransfer_write(STRUCT RISC *risc, uint32_t value, uint32_t *ram) {
  uint32_t offset = value / 4;
  uint32_t len = ram[offset + 1];
  uchar* name;
  uchar* command;
  switch(ram[offset]) {
    case 0x20001: { // OpWriteToHost = 20001H;
      name = (uchar*) (ram+offset+2);
      name[len] = 0;
      hosttransfer_open(name, 1);
      ram[offset + 1] = 0;
      break;
    }
    case 0x20002: { // OpWriteBuffer = 20002H;
      hosttransfer_writeclose(&ram[offset + 2], len);
      ram[offset + 1] = 0;
      break;
    }
    case 0x20003: { // OpReadFromHost = 20003H;
      name = (uchar*) (ram+offset+2);
      name[len] = 0;
      hosttransfer_open(name, 0);
      ram[offset + 1] = 0;
      break;
    }
    case 0x20004: { // OpRunOnHost = 20004H;
      command = (uchar*) (ram+offset+2);
      ram[offset+1] = hosttransfer_run(command);
      break;
    }
    case 0x20005: { // OpReadBuffer = 20005H;
      len = hosttransfer_readclose(&ram[offset + 2], len);
      ram[offset + 1] = len;
      break;
    }
  }
}

uint32_t risc_load_io(STRUCT RISC *risc, uint32_t address) {
  uint32_t mouse;
  uint32_t scancode;
  uint32_t i;

  if (address >= PaletteStart && address < PaletteStart + 0x40) {
    return palette[(address - PaletteStart)/4];
  }
  switch (address - IOStart) {
    case 0: {
      // Millisecond counter
      risc->progress--;
      return risc->current_tick;
    }
    case 24: {
      // Mouse input / keyboard status
      mouse = risc->mouse;
      if (risc->key_cnt > 0) {
        mouse |= 0x10000000;
      } else {
        risc->progress--;
      }
      return mouse;
    }
    case 28: {
      // Keyboard input
      if (risc->key_cnt > 0) {
        scancode = risc->key_buf[0];
        risc->key_cnt--;
        for(i=0; i<risc->key_cnt; i++) {
          risc->key_buf[i] = risc->key_buf[i+1];
        }
        return (scancode & 0xff) << 24 | ((scancode >> 8) & 0xff) << 16;
      }
      return 0;
    }
    case 40: {
      // Clipboard control
      return clipboard_control_read(risc);
    }
    case 44: {
      // Clipboard data
      return clipboard_data_read(risc);
    }
    case 60: {
      // hardware enumerator
      if (risc->hwenum_idx < risc->hwenum_cnt) {
        return risc->hwenum_buf[risc->hwenum_idx++];
      }
      return 0;
    }
    default: {
      return 0;
    }
  }
}

void risc_store_io(STRUCT RISC *risc, uint32_t address, uint32_t value) {
  if (address >= PaletteStart && address < PaletteStart + 0x40) {
    palette[(address - PaletteStart)/4] = value;
    risc_update_damage(risc->RAM, palette, risc->display_start, risc->current_mode_span, risc->current_mode_height, -1);
    return;
  }
  switch (address - IOStart) {
    case 4: {
      show_leds(value);
      break;
    }
    case 32: {
      // Host Transfer
      if ((risc->RAM[value/4] >> 16) == 2) {
        hosttransfer_write(risc, value, risc->RAM);
      }
    }
    case 36: {
      // Paravirtual disk
      paravirtual_write(&risc->disk, value, risc->RAM);
      break;
    }
    case 40: {
      // Clipboard control
      clipboard_control_write(risc, value);
      break;
    }
    case 44: {
      // Clipboard data
      clipboard_data_write(risc, value);
      break;
    }
    case 52: {
      // Debug console
      dbg_print((char) value);
      break;
    }
    case 60: {
      // hardware enumerator
      risc->hwenum_cnt = 0;
      risc->hwenum_idx = 0;
      switch(value) {
      case 0:
        risc->hwenum_buf[risc->hwenum_cnt++] = 1; // version
        risc->hwenum_buf[risc->hwenum_cnt++] = /*HW_ENUM_ID('1','6','c','V')*/((('1') << 24) | (('6') << 16) | (('c') << 8) | ('V'));
        risc->hwenum_buf[risc->hwenum_cnt++] = /*HW_ENUM_ID('T','i','m','r')*/((('T') << 24) | (('i') << 16) | (('m') << 8) | ('r'));
        risc->hwenum_buf[risc->hwenum_cnt++] = /*HW_ENUM_ID('S','w','t','c')*/((('S') << 24) | (('w') << 16) | (('t') << 8) | ('c'));
        risc->hwenum_buf[risc->hwenum_cnt++] = /*HW_ENUM_ID('M','s','K','b')*/((('M') << 24) | (('s') << 16) | (('K') << 8) | ('b'));
        risc->hwenum_buf[risc->hwenum_cnt++] = /*HW_ENUM_ID('R','s','e','t')*/((('R') << 24) | (('s') << 16) | (('e') << 8) | ('t'));
        risc->hwenum_buf[risc->hwenum_cnt++] = /*HW_ENUM_ID('D','b','g','C')*/((('D') << 24) | (('b') << 16) | (('g') << 8) | ('C'));
        risc->hwenum_buf[risc->hwenum_cnt++] = /*HW_ENUM_ID('L','E','D','s')*/((('L') << 24) | (('E') << 16) | (('D') << 8) | ('s'));
        risc->hwenum_buf[risc->hwenum_cnt++] = /*HW_ENUM_ID('v','D','s','k')*/((('v') << 24) | (('D') << 16) | (('s') << 8) | ('k'));
        risc->hwenum_buf[risc->hwenum_cnt++] = /*HW_ENUM_ID('v','C','l','p')*/((('v') << 24) | (('C') << 16) | (('l') << 8) | ('p'));
        risc->hwenum_buf[risc->hwenum_cnt++] = /*HW_ENUM_ID('v','H','T','x')*/((('v') << 24) | (('H') << 16) | (('T') << 8) | ('x'));
        break;
      case /*HW_ENUM_ID('1','6','c','V')*/((('1') << 24) | (('6') << 16) | (('c') << 8) | ('V')):
        risc->hwenum_buf[risc->hwenum_cnt++] = 1; // number of modes
        risc->hwenum_buf[risc->hwenum_cnt++] = 0; // first mode
        risc->hwenum_buf[risc->hwenum_cnt++] = 0; // mode switching MMIO address
        risc->hwenum_buf[risc->hwenum_cnt++] = PaletteStart; // palette address
        risc->hwenum_buf[risc->hwenum_cnt++] = (uint32_t)risc->current_mode_width; // screen width
        risc->hwenum_buf[risc->hwenum_cnt++] = (uint32_t)risc->current_mode_height; // screen height
        risc->hwenum_buf[risc->hwenum_cnt++] = (uint32_t)risc->current_mode_span * 4; // scanline span
        risc->hwenum_buf[risc->hwenum_cnt++] = risc->display_start; // base address
        break;
      case /*HW_ENUM_ID('T','i','m','r')*/((('T') << 24) | (('i') << 16) | (('m') << 8) | ('r')):
        risc->hwenum_buf[risc->hwenum_cnt++] = -64; // MMIO address
        break;
      case /*HW_ENUM_ID('L','E','D','s')*/((('L') << 24) | (('E') << 16) | (('D') << 8) | ('s')):
        risc->hwenum_buf[risc->hwenum_cnt++] = 8; // number of LEDs
        risc->hwenum_buf[risc->hwenum_cnt++] = -60; // MMIO address
        break;
      case /*HW_ENUM_ID('M','s','K','b')*/((('M') << 24) | (('s') << 16) | (('K') << 8) | ('b')):
        risc->hwenum_buf[risc->hwenum_cnt++] = -40; // MMIO mouse address + keyboard status
        risc->hwenum_buf[risc->hwenum_cnt++] = -36; // MMIO keyboard address
        risc->hwenum_buf[risc->hwenum_cnt++] = 1; // Paravirtual keyboard mode
        break;
      case /*HW_ENUM_ID('v','D','s','k')*/((('v') << 24) | (('D') << 16) | (('s') << 8) | ('k')):
        risc->hwenum_buf[risc->hwenum_cnt++] = -28; // MMIO address
        break;
      case /*HW_ENUM_ID('v','C','l','p')*/((('v') << 24) | (('C') << 16) | (('l') << 8) | ('p')):
        risc->hwenum_buf[risc->hwenum_cnt++] = -24; // MMIO clipboard control address
        risc->hwenum_buf[risc->hwenum_cnt++] = -20; // MMIO clipboard data address
        break;
      case /*HW_ENUM_ID('v','H','T','x')*/((('v') << 24) | (('H') << 16) | (('T') << 8) | ('x')):
        risc->hwenum_buf[risc->hwenum_cnt++] = -32; // MMIO host transfer address
        break;
      case /*HW_ENUM_ID('D','b','g','C')*/((('D') << 24) | (('b') << 16) | (('g') << 8) | ('C')):
        risc->hwenum_buf[risc->hwenum_cnt++] = -12; // MMIO debug console address
        break;
      case /*HW_ENUM_ID('R','s','e','t')*/((('R') << 24) | (('s') << 16) | (('e') << 8) | ('t')):
        risc->hwenum_buf[risc->hwenum_cnt++] = ROMStart; // Soft reset vector
        risc->hwenum_buf[risc->hwenum_cnt++] = 0; // Hard reset vector
        risc->hwenum_buf[risc->hwenum_cnt++] = ROMStart - 4; // Quit vector
        break;
      case /*HW_ENUM_ID('B','o','o','t')*/((('B') << 24) | (('o') << 16) | (('o') << 8) | ('t')):
        risc->hwenum_buf[risc->hwenum_cnt++] = risc->display_start;
        break;
      }
      break;
    }
  }
}

uint32_t risc_load_word(STRUCT RISC *risc, uint32_t address) {
  if (address < risc->mem_size) {
    return risc->RAM[address/4];
  } else {
    return risc_load_io(risc, address);
  }
}

uint8_t risc_load_byte(STRUCT RISC *risc, uint32_t address) {
  uint32_t w = risc_load_word(risc, address);
  return (uint8_t)(w >> (address % 4 * 8));
}

void risc_store_word(STRUCT RISC *risc, uint32_t address, uint32_t value) {
  if (address < risc->display_start) {
    risc->RAM[address/4] = value;
  } else if (address < risc->mem_size) {
    risc->RAM[address/4] = value;
    risc_update_damage(risc->RAM, palette, risc->display_start, risc->current_mode_span, risc->current_mode_height, (int)(address/4 - risc->display_start/4));
  } else {
    risc_store_io(risc, address, value);
  }
}

void risc_store_byte(STRUCT RISC *risc, uint32_t address, uint8_t value) {
  uint32_t w, shift;
  if (address < risc->mem_size) {
    w = risc_load_word(risc, address);
    shift = (address & 3) * 8;
    w &= ~((0xFF) << shift);
    w |= (uint32_t)value << shift;
    risc_store_word(risc, address, w);
  } else {
    risc_store_io(risc, address, (uint32_t)value);
  }
}

const uint32_t pbit = 0x80000000;
const uint32_t qbit = 0x40000000;
const uint32_t ubit = 0x20000000;
const uint32_t vbit = 0x10000000;

void risc_single_step(STRUCT RISC *risc) {
  uint32_t ir, a, b, c, op, im, a_val, b_val, c_val, address;
  uint32_t b_p0, b_p1, b_p2, c_p0, c_p1, c_p2, a_p0, a_p1, a_p2, a_p3, a_p4;
  int32_t off, b_sign, c_sign;
  bool t;

  if (risc->PC < risc->mem_size / 4) {
    ir = risc->RAM[risc->PC];
  } else if (risc->PC >= ROMStart/4 && risc->PC < ROMStart/4 + ROMWords) {
    ir = ROM[risc->PC - ROMStart/4];
  } else {
    return;
  }
  risc->PC++;


  if ((ir & pbit) == 0) {
    // Register instructions

    a  = (ir & 0x0F000000) >> 24;
    b  = (ir & 0x00F00000) >> 20;
    op = (ir & 0x000F0000) >> 16;
    im =  ir & 0x0000FFFF;
    c  =  ir & 0x0000000F;

    b_val = risc->R[b];
    if ((ir & qbit) == 0) {
      c_val = risc->R[c];
    } else if ((ir & vbit) == 0) {
      c_val = im;
    } else {
      c_val = 0xFFFF0000 | im;
    }

    switch (op) {
      case MOV: {
        if ((ir & ubit) == 0) {
          a_val = c_val;
        } else if ((ir & qbit) != 0) {
          a_val = c_val << 16;
        } else if ((ir & vbit) != 0) {
          a_val = 0xD0 |
            ((uint32_t)risc->N * 0x80000000) |
            ((uint32_t)risc->Z * 0x40000000) |
            ((uint32_t)risc->C * 0x20000000) |
            ((uint32_t)risc->V * 0x10000000);
        } else {
          a_val = (uint32_t)risc->H;
        }
        break;
      }
      case LSL: {
        a_val = b_val << (c_val & 31);
        break;
      }
      case ASR: {
        a_val = (uint32_t)((int32_t)b_val >> (int32_t)(c_val & 31));
        break;
      }
      case ROR: {
        a_val = (b_val >> (c_val & 31)) | (b_val << (-c_val & 31));
        break;
      }
      case AND: {
        a_val = b_val & c_val;
        break;
      }
      case ANN: {
        a_val = b_val & ~c_val;
        break;
      }
      case IOR: {
        a_val = b_val | c_val;
        break;
      }
      case XOR: {
        a_val = b_val ^ c_val;
        break;
      }
      case ADD: {
        a_val = b_val + c_val;
        if ((ir & ubit) != 0) {
          a_val += (uint32_t) risc->C;
        }
        risc->C = a_val < b_val;
        risc->V = (bool)(((a_val ^ c_val) & (a_val ^ b_val)) >> 31);
        break;
      }
      case SUB: {
        a_val = b_val - c_val;
        if ((ir & ubit) != 0) {
          a_val -= (uint32_t)risc->C;
        }
        risc->C = a_val > b_val;
        risc->V = (bool)(((b_val ^ c_val) & (a_val ^ b_val)) >> 31);
        break;
      }
      case MUL: {
        // calculate 33-bit integers in 11-bit parts
        b_sign = c_sign = 0;
        if ((ir & ubit) == 0) {
          b_sign = (int32_t)b_val;
          if (b_sign < 0) {
            b_val = -b_val;
            b_sign = 1;
          } else {
            b_sign = 0;
          }
          c_sign = (int32_t)c_val;
          if (c_sign < 0) {
            c_val = -c_val;
            c_sign = 1;
          } else {
            c_sign = 0;
          }
        }
        b_p0 = (((b_val) >> 0) & 0x7FF);
        b_p1 = (((b_val) >> 11) & 0x7FF);
        b_p2 = (((b_val) >> 22) & 0x7FF);
        c_p0 = (((c_val) >> 0) & 0x7FF);
        c_p1 = (((c_val) >> 11) & 0x7FF);
        c_p2 = (((c_val) >> 22) & 0x7FF);
        a_p0 = b_p0 * c_p0;
        a_p1 = b_p0 * c_p1 + b_p1 * c_p0;
        a_p2 = b_p0 * c_p2 + b_p1 * c_p1 + b_p2 * c_p0;
        a_p3 = b_p1 * c_p2 + b_p2 * c_p1;
        a_p4 = b_p2 * c_p2;
        a_val = a_p0 + (a_p1 << 11) + (a_p2 << 22);
        risc->H = (a_p1 >> 21) + (a_p2 >> 10) + (a_p3 << 1) + (a_p4 << 12);
        if ((b_sign ^ c_sign) != 0) {
          a_val = -a_val;
          risc->H = -risc->H;
        }
        break;
      }
      case DIV: {
        b_sign = c_sign = 0;
        if ((ir & ubit) == 0) {
          b_sign = (int32_t)b_val;
          if (b_sign  < 0) {
            b_val = -b_val;
            b_sign = 1;
          } else {
            b_sign = 0;
          }
          c_sign = (int32_t)c_val;
          if (c_sign  < 0) {
            c_val = -c_val;
            c_sign = 1;
          } else {
            c_sign = 0;
          }
        }
        a_val = b_val / c_val;
        risc->H = b_val % c_val;
        if ((b_sign ^ c_sign) != 0) {
          a_val = -a_val;
          risc->H =-risc->H;
          if (risc->H != 0) {
            a_val--;
            risc->H += c_val;
          }
        }
        break;
      }
      default: a_val = 0; // unreachable
    }
    risc_set_register(risc, a, a_val);
  }
  else if ((ir & qbit) == 0) {
    // Memory instructions

    a = (ir & 0x0F000000) >> 24;
    b = (ir & 0x00F00000) >> 20;
    off = (int32_t)(ir & 0x000FFFFF);
    off = (off ^ 0x00080000) - 0x00080000;  // sign-extend

    address = (uint32_t)((int32_t)risc->R[b] + off);
    if ((ir & ubit) == 0) {
      if ((ir & vbit) == 0) {
        a_val = risc_load_word(risc, address);
      } else {
        a_val = (uint32_t)risc_load_byte(risc, address);
      }
      risc_set_register(risc, a, a_val);
    } else {
      if ((ir & vbit) == 0) {
        risc_store_word(risc, address, risc->R[a]);
      } else {
        risc_store_byte(risc, address, (uint8_t)risc->R[a]);
      }
    }
  }
  else {
    // Branch instructions
    t = (bool)((ir >> 27) & 1);
    switch ((ir >> 24) & 7) {
      case 0: t ^= risc->N; break;
      case 1: t ^= risc->Z; break;
      case 2: t ^= risc->C; break;
      case 3: t ^= risc->V; break;
      case 4: t ^= risc->C | risc->Z; break;
      case 5: t ^= risc->N ^ risc->V; break;
      case 6: t ^= (risc->N ^ risc->V) | risc->Z; break;
      case 7: t ^= (1==1); break;
      default: t = 0;  // unreachable
    }
    if (t) {
      if ((ir & vbit) != 0) {
        risc_set_register(risc, 15, risc->PC * 4);
      }
      if ((ir & ubit) == 0) {
        c = ir & 0x0000000F;
        risc->PC = risc->R[c] / 4;
      } else {
        off = (int32_t)(ir & 0x00FFFFFF);
        off = (off ^ 0x00800000) - 0x00800000;  // sign-extend
        risc->PC = (uint32_t)((int32_t)risc->PC + off);
      }
    }
  }
}

bool risc_run(STRUCT RISC *risc, int cycles) {
  int i;
  risc->progress = 20;
  // The progress value is used to detect that the RISC cpu is busy
  // waiting on the millisecond counter or on the keyboard ready
  // bit. In that case it's better to just pause emulation until the
  // next frame.
  for (i = 0; i < cycles && risc->progress; i++) {
    if (risc->PC == ROMStart/4 - 1) {
      risc->PC++;
      return 0;
    }
    risc_single_step(risc);
  }
  return 1;
}

void risc_set_time(STRUCT RISC *risc, uint32_t tick) {
  risc->current_tick = tick;
}

void risc_mouse_moved(STRUCT RISC *risc, uint mouse_x, uint mouse_y) {
  if (mouse_x < 4096) {
    risc->mouse = (risc->mouse & ~0x00000FFF ) | mouse_x;
  }
  if (mouse_y < 4096) {
    risc->mouse = (risc->mouse & ~0x00FFF000 ) | (mouse_y << 12);
  }
}
void risc_mouse_button(STRUCT RISC *risc, int button, bool down) {
  uint32_t bit;
  if (button >= 1 && button < 4) {
    bit = 1 << (27 - (uint32_t)button);
    if (down) {
      risc->mouse |= bit;
    } else {
      risc->mouse &= ~bit;
    }
  }
}

void risc_keyboard_input(STRUCT RISC *risc, uint32_t scancode) {
  if (risc->key_cnt < /*sizeof(risc->key_buf)*/16) {
    risc->key_buf[risc->key_cnt] = scancode;
    risc->key_cnt++;
  }
}

uint32_t *risc_get_framebuffer_ptr(STRUCT RISC *risc) {
  return &risc->RAM[risc->display_start/4];
}

uint32_t *risc_get_palette_ptr(STRUCT RISC *risc) {
  return palette;
}
