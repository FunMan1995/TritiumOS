/* Glue code for compiling risc.c on Dusk OS */
#define STRUCT /**/
#define const /**/
#define uint32_t     uint
#define int32_t      int
#define uint8_t      uchar
#define bool         int

struct RISC_disk {
  Stream *disk_file;
  uint32_t disk_paravirt_ptr;
};

void risc_update_damage(uint32_t* RAM, uint32_t* palette, uint32_t display_start, int current_mode_span, int height, int w) {
    int row = w / current_mode_span;
    int col = w % current_mode_span;
    uint i, v, c;
    //printf(w, col, row, "UPDATE_DAMAGE %d %d %d\n");
    if (w == -1) {
        for (i=0; (int)i < current_mode_span * height; i++) {
            risc_update_damage(RAM, palette, display_start, current_mode_span, height, (int)i);
        }
    } else if (row < height) {
        v = RAM[display_start/4 + (uint)w];
        //printf(v, "DISPMEM VALUE %d\n");
        for(i=0; i<8; i++) {
            c = (v >> (i * 4)) & 0xf;
            c = palette[c];
            //printf(c, row, col * 8 + i, "SET_PIXEL %d %d %d\n");
            _set_pixel((uint)col * 8 + i, (uint)row, c);
        }
        //printf(current_mode_span, w, height, "UPDATE DAMAGE %d %d %d\n");
    }
}

void putchar(char ch) { emit((uchar)ch); }

void seek_sector(Stream *f, uint32_t secnum) {
  seek(f, secnum * 512);
}

void read_sector(Stream *f, uint32_t* buf) {
  read(f, 512, buf);
}

void write_sector(Stream *f, uint32_t* buf) {
  write(f, 512, buf);
}

void truncate_sector(Stream *f) {
  _ftruncate(f);
  flush(f);
}

int transfer_file;
uchar name_buffer[128];

void hosttransfer_open(uchar* name, bool write) {
  int i = 0;
  while (name[i] != 0 && i < 128) {
    name_buffer[i+1] = name[i]; i++;
  }
  name_buffer[0] = (uchar)i;
  transfer_file = (int)openpath(name_buffer);
}

void hosttransfer_writeclose(uint32_t* data, uint32_t len) {
  if (len > 0) {
    write((Stream*)transfer_file, len, data);
  } else {
    close((Stream*)transfer_file);
    transfer_file = NULL;
  }
}

uint32_t hosttransfer_readclose(uint32_t* data, uint32_t len) {
  len = (uint32_t) read((Stream*)transfer_file, len, data);
  if (len == 0) {
    close((Stream*)transfer_file);
    transfer_file = NULL;
  }
  return len;
}

uint32_t hosttransfer_run(uchar* command) {
  int i = 0;
  while (command[i] != 0 && i < 128) {
    name_buffer[i+1] = command[i]; i++;
  }
  name_buffer[0] = (uchar)i;
  _run_command(name_buffer);
  command[0]='!';
  return 1;
}
