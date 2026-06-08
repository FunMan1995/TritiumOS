#include <stdio.h>
#include <stdint.h>
#include <string.h>
#include <stdlib.h>
#include <errno.h>
#include <endian.h>

// from <grub/include/grub/keyboard_layouts.h>:

#define GRUB_KEYBOARD_LAYOUTS_FILEMAGIC "GRUBLAYO"
#define GRUB_KEYBOARD_LAYOUTS_FILEMAGIC_SIZE (sizeof(GRUB_KEYBOARD_LAYOUTS_FILEMAGIC) - 1)
#define GRUB_KEYBOARD_LAYOUTS_VERSION 10

#define GRUB_KEYBOARD_LAYOUTS_ARRAY_SIZE 160

typedef enum grub_keyboard_key
  {
    GRUB_KEYBOARD_KEY_A = 0x04,
    GRUB_KEYBOARD_KEY_B = 0x05,
    GRUB_KEYBOARD_KEY_C = 0x06,
    GRUB_KEYBOARD_KEY_D = 0x07,
    GRUB_KEYBOARD_KEY_E = 0x08,
    GRUB_KEYBOARD_KEY_F = 0x09,
    GRUB_KEYBOARD_KEY_G = 0x0a,
    GRUB_KEYBOARD_KEY_H = 0x0b,
    GRUB_KEYBOARD_KEY_I = 0x0c,
    GRUB_KEYBOARD_KEY_J = 0x0d,
    GRUB_KEYBOARD_KEY_K = 0x0e,
    GRUB_KEYBOARD_KEY_L = 0x0f,
    GRUB_KEYBOARD_KEY_M = 0x10,
    GRUB_KEYBOARD_KEY_N = 0x11,
    GRUB_KEYBOARD_KEY_O = 0x12,
    GRUB_KEYBOARD_KEY_P = 0x13,
    GRUB_KEYBOARD_KEY_Q = 0x14,
    GRUB_KEYBOARD_KEY_R = 0x15,
    GRUB_KEYBOARD_KEY_S = 0x16,
    GRUB_KEYBOARD_KEY_T = 0x17,
    GRUB_KEYBOARD_KEY_U = 0x18,
    GRUB_KEYBOARD_KEY_V = 0x19,
    GRUB_KEYBOARD_KEY_W = 0x1a,
    GRUB_KEYBOARD_KEY_X = 0x1b,
    GRUB_KEYBOARD_KEY_Y = 0x1c,
    GRUB_KEYBOARD_KEY_Z = 0x1d,
    GRUB_KEYBOARD_KEY_1 = 0x1e,
    GRUB_KEYBOARD_KEY_2 = 0x1f,
    GRUB_KEYBOARD_KEY_3 = 0x20,
    GRUB_KEYBOARD_KEY_4 = 0x21,
    GRUB_KEYBOARD_KEY_5 = 0x22,
    GRUB_KEYBOARD_KEY_6 = 0x23,
    GRUB_KEYBOARD_KEY_7 = 0x24,
    GRUB_KEYBOARD_KEY_8 = 0x25,
    GRUB_KEYBOARD_KEY_9 = 0x26,
    GRUB_KEYBOARD_KEY_0 = 0x27,
    GRUB_KEYBOARD_KEY_ENTER = 0x28,
    GRUB_KEYBOARD_KEY_ESCAPE = 0x29,
    GRUB_KEYBOARD_KEY_BACKSPACE = 0x2a,
    GRUB_KEYBOARD_KEY_TAB = 0x2b,
    GRUB_KEYBOARD_KEY_SPACE = 0x2c,
    GRUB_KEYBOARD_KEY_DASH = 0x2d,
    GRUB_KEYBOARD_KEY_EQUAL = 0x2e,
    GRUB_KEYBOARD_KEY_LBRACKET = 0x2f,
    GRUB_KEYBOARD_KEY_RBRACKET = 0x30,
    GRUB_KEYBOARD_KEY_BACKSLASH = 0x32,
    GRUB_KEYBOARD_KEY_SEMICOLON = 0x33,
    GRUB_KEYBOARD_KEY_DQUOTE = 0x34,
    GRUB_KEYBOARD_KEY_RQUOTE = 0x35,
    GRUB_KEYBOARD_KEY_COMMA = 0x36,
    GRUB_KEYBOARD_KEY_DOT = 0x37,
    GRUB_KEYBOARD_KEY_SLASH = 0x38,
    GRUB_KEYBOARD_KEY_CAPS_LOCK  = 0x39,
    GRUB_KEYBOARD_KEY_F1 = 0x3a,
    GRUB_KEYBOARD_KEY_F2 = 0x3b,
    GRUB_KEYBOARD_KEY_F3 = 0x3c,
    GRUB_KEYBOARD_KEY_F4 = 0x3d,
    GRUB_KEYBOARD_KEY_F5 = 0x3e,
    GRUB_KEYBOARD_KEY_F6 = 0x3f,
    GRUB_KEYBOARD_KEY_F7 = 0x40,
    GRUB_KEYBOARD_KEY_F8 = 0x41,
    GRUB_KEYBOARD_KEY_F9 = 0x42,
    GRUB_KEYBOARD_KEY_F10 = 0x43,
    GRUB_KEYBOARD_KEY_F11 = 0x44,
    GRUB_KEYBOARD_KEY_F12 = 0x45,
    GRUB_KEYBOARD_KEY_SCROLL_LOCK  = 0x47,
    GRUB_KEYBOARD_KEY_INSERT = 0x49,
    GRUB_KEYBOARD_KEY_HOME = 0x4a,
    GRUB_KEYBOARD_KEY_PPAGE = 0x4b,
    GRUB_KEYBOARD_KEY_DELETE = 0x4c,
    GRUB_KEYBOARD_KEY_END = 0x4d,
    GRUB_KEYBOARD_KEY_NPAGE = 0x4e,
    GRUB_KEYBOARD_KEY_RIGHT = 0x4f,
    GRUB_KEYBOARD_KEY_LEFT = 0x50,
    GRUB_KEYBOARD_KEY_DOWN = 0x51,
    GRUB_KEYBOARD_KEY_UP = 0x52,
    GRUB_KEYBOARD_KEY_NUM_LOCK = 0x53,
    GRUB_KEYBOARD_KEY_NUMSLASH = 0x54,
    GRUB_KEYBOARD_KEY_NUMMUL = 0x55,
    GRUB_KEYBOARD_KEY_NUMMINUS = 0x56,
    GRUB_KEYBOARD_KEY_NUMPLUS = 0x57,
    GRUB_KEYBOARD_KEY_NUMENTER = 0x58,
    GRUB_KEYBOARD_KEY_NUM1 = 0x59,
    GRUB_KEYBOARD_KEY_NUM2 = 0x5a,
    GRUB_KEYBOARD_KEY_NUM3 = 0x5b,
    GRUB_KEYBOARD_KEY_NUM4 = 0x5c,
    GRUB_KEYBOARD_KEY_NUM5 = 0x5d,
    GRUB_KEYBOARD_KEY_NUM6 = 0x5e,
    GRUB_KEYBOARD_KEY_NUM7 = 0x5f,
    GRUB_KEYBOARD_KEY_NUM8 = 0x60,
    GRUB_KEYBOARD_KEY_NUM9 = 0x61,
    GRUB_KEYBOARD_KEY_NUM0 = 0x62,
    GRUB_KEYBOARD_KEY_NUMDOT = 0x63,
    GRUB_KEYBOARD_KEY_102ND = 0x64,
    GRUB_KEYBOARD_KEY_KPCOMMA = 0x85,
    GRUB_KEYBOARD_KEY_JP_RO = 0x87,
    GRUB_KEYBOARD_KEY_JP_YEN = 0x89,
    GRUB_KEYBOARD_KEY_LEFT_CTRL = 0xe0,
    GRUB_KEYBOARD_KEY_LEFT_SHIFT = 0xe1,
    GRUB_KEYBOARD_KEY_LEFT_ALT = 0xe2,
    GRUB_KEYBOARD_KEY_RIGHT_CTRL = 0xe4,
    GRUB_KEYBOARD_KEY_RIGHT_SHIFT = 0xe5,
    GRUB_KEYBOARD_KEY_RIGHT_ALT = 0xe6,
  } grub_keyboard_key_t;


// end from <grub/include/grub/keyboard_layouts.h>:



static char mapping[GRUB_KEYBOARD_LAYOUTS_ARRAY_SIZE] = {0};

void initMap() {
  mapping[GRUB_KEYBOARD_KEY_A] ='A';
  mapping[GRUB_KEYBOARD_KEY_B] ='B';
  mapping[GRUB_KEYBOARD_KEY_C] ='C';
  mapping[GRUB_KEYBOARD_KEY_D] ='D';
  mapping[GRUB_KEYBOARD_KEY_E] ='E';
  mapping[GRUB_KEYBOARD_KEY_F] ='F';
  mapping[GRUB_KEYBOARD_KEY_G] ='G';
  mapping[GRUB_KEYBOARD_KEY_H] ='H';
  mapping[GRUB_KEYBOARD_KEY_I] ='I';
  mapping[GRUB_KEYBOARD_KEY_J] ='J';
  mapping[GRUB_KEYBOARD_KEY_K] ='K';
  mapping[GRUB_KEYBOARD_KEY_L] ='L';
  mapping[GRUB_KEYBOARD_KEY_M] ='M';
  mapping[GRUB_KEYBOARD_KEY_N] ='N';
  mapping[GRUB_KEYBOARD_KEY_O] ='O';
  mapping[GRUB_KEYBOARD_KEY_P] ='P';
  mapping[GRUB_KEYBOARD_KEY_Q] ='Q';
  mapping[GRUB_KEYBOARD_KEY_R] ='R';
  mapping[GRUB_KEYBOARD_KEY_S] ='S';
  mapping[GRUB_KEYBOARD_KEY_T] ='T';
  mapping[GRUB_KEYBOARD_KEY_U] ='U';
  mapping[GRUB_KEYBOARD_KEY_V] ='V';
  mapping[GRUB_KEYBOARD_KEY_W] ='W';
  mapping[GRUB_KEYBOARD_KEY_X] ='X';
  mapping[GRUB_KEYBOARD_KEY_Y] ='Y';
  mapping[GRUB_KEYBOARD_KEY_Z] ='Z';
  mapping[GRUB_KEYBOARD_KEY_1] ='1';
  mapping[GRUB_KEYBOARD_KEY_2] ='2';
  mapping[GRUB_KEYBOARD_KEY_3] ='3';
  mapping[GRUB_KEYBOARD_KEY_4] ='4';
  mapping[GRUB_KEYBOARD_KEY_5] ='5';
  mapping[GRUB_KEYBOARD_KEY_6] ='6';
  mapping[GRUB_KEYBOARD_KEY_7] ='7';
  mapping[GRUB_KEYBOARD_KEY_8] ='8';
  mapping[GRUB_KEYBOARD_KEY_9] ='9';
  mapping[GRUB_KEYBOARD_KEY_0] ='0';
  mapping[GRUB_KEYBOARD_KEY_ENTER] = '\r';
  mapping[GRUB_KEYBOARD_KEY_ESCAPE] = 27;
  mapping[GRUB_KEYBOARD_KEY_BACKSPACE] = 8;
  mapping[GRUB_KEYBOARD_KEY_TAB] = 9;
  mapping[GRUB_KEYBOARD_KEY_SPACE] = ' ';
  mapping[GRUB_KEYBOARD_KEY_DASH] =	'-';
  mapping[GRUB_KEYBOARD_KEY_EQUAL] = '=';
  mapping[GRUB_KEYBOARD_KEY_LBRACKET] = '[';
  mapping[GRUB_KEYBOARD_KEY_RBRACKET] =	']';
  mapping[GRUB_KEYBOARD_KEY_BACKSLASH] = '\\';
  mapping[GRUB_KEYBOARD_KEY_SEMICOLON] = ';';
  mapping[GRUB_KEYBOARD_KEY_DQUOTE] = '\'';
  mapping[GRUB_KEYBOARD_KEY_RQUOTE] = '`';
  mapping[GRUB_KEYBOARD_KEY_COMMA] = ',';
  mapping[GRUB_KEYBOARD_KEY_DOT] = '.';
  mapping[GRUB_KEYBOARD_KEY_SLASH] = '/';
  mapping[GRUB_KEYBOARD_KEY_102ND] = '<';
}


int main (int argc, char *argv[]) {
	FILE *gf, *df;
	char magic[GRUB_KEYBOARD_LAYOUTS_FILEMAGIC_SIZE];
	uint32_t grub_map[GRUB_KEYBOARD_LAYOUTS_ARRAY_SIZE];
	char dusk_map[128];
	if (argc != 3) {
		fprintf(stderr, "Usage: kbdconv GRUBFILE DUSKFILE\n");
		return EXIT_FAILURE;
	}
	gf = fopen(argv[1], "rb");
	if (gf == NULL) {
		fprintf(stderr, "Failed to open file: %s\n", strerror(errno));
		return EXIT_FAILURE;
	}
	if (fread(magic, 1, GRUB_KEYBOARD_LAYOUTS_FILEMAGIC_SIZE, gf) != GRUB_KEYBOARD_LAYOUTS_FILEMAGIC_SIZE 
			|| memcmp(magic, GRUB_KEYBOARD_LAYOUTS_FILEMAGIC, GRUB_KEYBOARD_LAYOUTS_FILEMAGIC_SIZE) != 0) {
		fprintf(stderr, "Invalid file magic\n");
		return EXIT_FAILURE;
	}
	if (fread(magic, 1, 1, gf) != 1 || magic[0] != GRUB_KEYBOARD_LAYOUTS_VERSION) {
		fprintf(stderr, "Unsupported file version %d\n", magic[0]);
		return EXIT_FAILURE;
	}
	initMap();
	df = fopen(argv[2], "wb");
	for(int i = 0; i < 4; i++) {
		if (fread(grub_map, sizeof(uint32_t), GRUB_KEYBOARD_LAYOUTS_ARRAY_SIZE, gf) != GRUB_KEYBOARD_LAYOUTS_ARRAY_SIZE) {
			fprintf(stderr, "File too short / read error\n");
			break;
		}
		memset(dusk_map, 0, 128);
		for(int j = 0; j < GRUB_KEYBOARD_LAYOUTS_ARRAY_SIZE; j++) {
			uint32_t gv = be32toh(grub_map[j]);
			if (mapping[j] != 0 && gv > 0 && gv < 128)
				dusk_map[(int)mapping[j]] = gv;
		}
		dusk_map[8] = 8; dusk_map[9] = 9; dusk_map[13] = 13;
		if (fwrite(dusk_map, 128, 1, df) != 1) {
			fprintf(stderr, "Write error\n");
			break;
		}
	}
	fclose(gf);
	fclose(df);
	return EXIT_SUCCESS;
}
