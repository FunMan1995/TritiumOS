SDL2_CFLAGS = `$(SDL2_CONFIG) --cflags --libs` -Wl,-rpath,/usr/pkg/lib
POSTCMD = paxctl +m $@
ARM_LDFLAGS = -larm
