// Some constants that mirror those in Dusk's Forth code
#define KBD_EVENT_NONE 0
#define KBD_EVENT_PRESS 1
#define KBD_EVENT_RELEASE 2
#define KBD_EVENT_BOTH 3

// Those are in sys/kbd's "Keys" namespace
#define KBD_PASSTHROUGH 0x80000000
#define KBD_LSHIFT 0x10000
#define KBD_RSHIFT 0x20000
#define KBD_LCONTROL 0x40000
#define KBD_RCONTROL 0x80000
#define KBD_LALT 0x100000
#define KBD_RALT 0x200000
#define KBD_LGUI 0x400000
#define KBD_RGUI 0x800000
